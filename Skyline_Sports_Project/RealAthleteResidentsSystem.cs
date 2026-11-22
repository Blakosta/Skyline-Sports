using System;
using System.Collections.Generic;
using Colossal.Serialization.Entities;
using Game;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Game.UI;
using Skyline_Sports_Project.Engine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Skyline_Sports_Project;

[Preserve]
public sealed class RealAthleteResidentsSystem : GameSystemBase, IDefaultSerializable, ISerializable
{
    private const int kSaveVersion = 1;
    private const int kWorkplaceCapacity = 1000;

    private sealed class ResidentLink
    {
        public Entity Household;
        public Entity Citizen;
        public int TeamId = -1;
        public int VenueId = -1;
    }

    private SportsDataSystem m_SportsData;
    private NameSystem m_NameSystem;
    private SimulationSystem m_SimulationSystem;
    private TimeSystem m_TimeSystem;
    private EndFrameBarrier m_EndFrameBarrier;

    private EntityQuery m_HouseholdPrefabQuery;
    private EntityQuery m_OutsideConnectionQuery;
    private EntityQuery m_WorkplacePrefabQuery;
    private EntityQuery m_TimeDataQuery;

    private readonly Dictionary<int, ResidentLink> m_PlayerResidents = new();
    private readonly Dictionary<int, ResidentLink> m_CoachResidents = new();
    private readonly Dictionary<int, Dictionary<WorkplaceComplexity, Entity>> m_VenueWorkplaces = new();
    private readonly Dictionary<WorkplaceComplexity, Entity> m_WorkplacePrefabs = new();

    private bool m_LastEnabled;
    private bool m_WorkplacePrefabWarningLogged;
    private bool m_HouseholdPrefabWarningLogged;
    private bool m_OutsideConnectionWarningLogged;

    public override int GetUpdateInterval(SystemUpdatePhase phase) => 32;

    [Preserve]
    protected override void OnCreate()
    {
        base.OnCreate();

        m_SportsData = World.GetOrCreateSystemManaged<SportsDataSystem>();
        m_NameSystem = World.GetOrCreateSystemManaged<NameSystem>();
        m_SimulationSystem = World.GetOrCreateSystemManaged<SimulationSystem>();
        m_TimeSystem = World.GetOrCreateSystemManaged<TimeSystem>();
        m_EndFrameBarrier = World.GetOrCreateSystemManaged<EndFrameBarrier>();

        m_HouseholdPrefabQuery = GetEntityQuery(
            ComponentType.ReadOnly<HouseholdData>(),
            ComponentType.ReadOnly<ArchetypeData>(),
            ComponentType.Exclude<DynamicHousehold>(),
            ComponentType.Exclude<Deleted>(),
            ComponentType.Exclude<Temp>());

        m_OutsideConnectionQuery = GetEntityQuery(
            ComponentType.ReadOnly<OutsideConnection>(),
            ComponentType.Exclude<ElectricityOutsideConnection>(),
            ComponentType.Exclude<WaterPipeOutsideConnection>(),
            ComponentType.Exclude<Deleted>(),
            ComponentType.Exclude<Temp>());

        m_WorkplacePrefabQuery = GetEntityQuery(
            ComponentType.ReadOnly<PrefabData>(),
            ComponentType.ReadOnly<WorkplaceData>(),
            ComponentType.Exclude<Deleted>(),
            ComponentType.Exclude<Temp>());

        m_TimeDataQuery = GetEntityQuery(ComponentType.ReadOnly<TimeData>());

        Mod.log.Info("[Жители-спортсмены] Система создана. Функция управляется отдельной настройкой.");
    }

    [Preserve]
    protected override void OnUpdate()
    {
        try
        {
            if (Mod.Settings == null || !Mod.Settings.RealAthleteResidentsEnabled)
            {
                if (m_LastEnabled)
                {
                    ReleaseAllEmployment();
                    DestroyAllWorkplaces();
                    Mod.log.Info("[Жители-спортсмены] Функция выключена. Спортсмены остаются жителями города, их дома и автомобили не удаляются.");
                }

                m_LastEnabled = false;
                return;
            }

            m_LastEnabled = true;

            if (m_SportsData == null || m_SportsData.Sports == null)
            {
                return;
            }

            RefreshWorkplacePrefabs();
            SyncResidents();
        }
        catch (Exception ex)
        {
            Mod.Error("Не удалось синхронизировать реальных жителей-спортсменов", ex);
        }
    }

    public bool TryGetPlayerResident(int playerId, out Entity citizen)
    {
        return TryGetResident(m_PlayerResidents, playerId, out citizen);
    }

    public bool TryGetCoachResident(int coachId, out Entity citizen)
    {
        return TryGetResident(m_CoachResidents, coachId, out citizen);
    }

    public bool HasPlayerResident(int playerId) => TryGetPlayerResident(playerId, out _);

    public bool HasCoachResident(int coachId) => TryGetCoachResident(coachId, out _);

    private bool TryGetResident(Dictionary<int, ResidentLink> links, int id, out Entity citizen)
    {
        citizen = Entity.Null;
        if (!links.TryGetValue(id, out ResidentLink link))
        {
            return false;
        }

        if (link.Citizen == Entity.Null || !EntityManager.Exists(link.Citizen))
        {
            return false;
        }

        if (!EntityManager.HasComponent<Citizen>(link.Citizen) ||
            !EntityManager.HasComponent<HouseholdMember>(link.Citizen))
        {
            return false;
        }

        citizen = link.Citizen;
        return true;
    }

    private void SyncResidents()
    {
        SportsWorld world = m_SportsData.Sports;
        HashSet<int> activePlayers = new();
        HashSet<int> activeCoaches = new();
        HashSet<int> activeVenues = new();

        foreach (Team team in world.UserTeams())
        {
            if (team == null || team.Retired || !IsSupportedSport(team.Sport))
            {
                continue;
            }

            if (team.VenueId >= 0)
            {
                activeVenues.Add(team.VenueId);
            }

            foreach (Player player in world.RosterOf(team.Id))
            {
                if (player == null)
                {
                    continue;
                }

                activePlayers.Add(player.Id);
                SyncPlayer(player, team);
            }

            foreach (Coach coach in world.StaffOf(team.Id))
            {
                if (coach == null)
                {
                    continue;
                }

                activeCoaches.Add(coach.Id);
                SyncCoach(coach, team);
            }
        }

        foreach (KeyValuePair<int, ResidentLink> pair in m_PlayerResidents)
        {
            if (!activePlayers.Contains(pair.Key))
            {
                ReleaseLinkEmployment(pair.Value);
                pair.Value.TeamId = -1;
                pair.Value.VenueId = -1;
            }
        }

        foreach (KeyValuePair<int, ResidentLink> pair in m_CoachResidents)
        {
            if (!activeCoaches.Contains(pair.Key))
            {
                ReleaseLinkEmployment(pair.Value);
                pair.Value.TeamId = -1;
                pair.Value.VenueId = -1;
            }
        }

        CleanupUnusedWorkplaces(activeVenues);
    }

    private static bool IsSupportedSport(Sport sport)
    {
        return sport == Sport.Football || sport == Sport.Hockey;
    }

    private void SyncPlayer(Player player, Team team)
    {
        if (!m_PlayerResidents.TryGetValue(player.Id, out ResidentLink link))
        {
            link = new ResidentLink();
            m_PlayerResidents.Add(player.Id, link);
        }

        link.TeamId = team.Id;
        link.VenueId = team.VenueId;

        EnsureHousehold(link, player.Id);
        if (!TryResolveCitizen(link))
        {
            return;
        }

        ApplyIdentity(link.Citizen, player.Name, player.BirthYear);
        AssignEmployment(link, team);
    }

    private void SyncCoach(Coach coach, Team team)
    {
        if (!m_CoachResidents.TryGetValue(coach.Id, out ResidentLink link))
        {
            link = new ResidentLink();
            m_CoachResidents.Add(coach.Id, link);
        }

        link.TeamId = team.Id;
        link.VenueId = team.VenueId;

        EnsureHousehold(link, coach.Id ^ int.MinValue);
        if (!TryResolveCitizen(link))
        {
            return;
        }

        ApplyIdentity(link.Citizen, coach.Name, coach.BirthYear);
        AssignEmployment(link, team);
    }

    private void EnsureHousehold(ResidentLink link, int stableId)
    {
        if (link.Citizen != Entity.Null && EntityManager.Exists(link.Citizen))
        {
            return;
        }

        if (link.Household != Entity.Null && EntityManager.Exists(link.Household))
        {
            return;
        }

        link.Household = Entity.Null;
        link.Citizen = Entity.Null;

        if (TryCreateHousehold(stableId, out Entity household))
        {
            link.Household = household;
        }
    }

    private bool TryCreateHousehold(int stableId, out Entity household)
    {
        household = Entity.Null;

        if (m_HouseholdPrefabQuery.IsEmptyIgnoreFilter)
        {
            if (!m_HouseholdPrefabWarningLogged)
            {
                m_HouseholdPrefabWarningLogged = true;
                Mod.log.Warn("[Жители-спортсмены] В игровых библиотеках пока нет подходящего префаба домохозяйства.");
            }
            return false;
        }

        if (m_OutsideConnectionQuery.IsEmptyIgnoreFilter)
        {
            if (!m_OutsideConnectionWarningLogged)
            {
                m_OutsideConnectionWarningLogged = true;
                Mod.log.Warn("[Жители-спортсмены] Не найдены автомобильные/дорожные внешние соединения для штатного создания домохозяйства.");
            }
            return false;
        }

        Entity selectedPrefab = Entity.Null;
        NativeArray<Entity> prefabs = m_HouseholdPrefabQuery.ToEntityArray(Allocator.Temp);
        try
        {
            for (int i = 0; i < prefabs.Length; i++)
            {
                Entity prefab = prefabs[i];
                HouseholdData data = EntityManager.GetComponentData<HouseholdData>(prefab);

                if (data.m_AdultCount != 1 ||
                    data.m_ChildCount != 0 ||
                    data.m_ElderCount != 0 ||
                    data.m_StudentCount != 0 ||
                    data.m_FirstPetProbability != 0 ||
                    data.m_NextPetProbability != 0 ||
                    data.m_Weight <= 0)
                {
                    continue;
                }

                selectedPrefab = prefab;
                break;
            }
        }
        finally
        {
            prefabs.Dispose();
        }

        if (selectedPrefab == Entity.Null)
        {
            if (!m_HouseholdPrefabWarningLogged)
            {
                m_HouseholdPrefabWarningLogged = true;
                Mod.log.Warn("[Жители-спортсмены] Не найдено штатное домохозяйство с одним взрослым без детей и питомцев. Создание пропущено, чтобы не порождать лишних горожан.");
            }
            return false;
        }

        ArchetypeData archetypeData = EntityManager.GetComponentData<ArchetypeData>(selectedPrefab);
        household = EntityManager.CreateEntity(archetypeData.m_Archetype);
        EntityManager.SetComponentData(household, new PrefabRef(selectedPrefab));

        NativeArray<Entity> outsideConnections = m_OutsideConnectionQuery.ToEntityArray(Allocator.Temp);
        try
        {
            if (outsideConnections.Length == 0)
            {
                EntityManager.DestroyEntity(household);
                household = Entity.Null;
                return false;
            }

            uint index = (uint)stableId;
            Entity outside = outsideConnections[(int)(index % (uint)outsideConnections.Length)];
            CurrentBuilding currentBuilding = new() { m_CurrentBuilding = outside };

            if (EntityManager.HasComponent<CurrentBuilding>(household))
            {
                EntityManager.SetComponentData(household, currentBuilding);
            }
            else
            {
                EntityManager.AddComponentData(household, currentBuilding);
            }
        }
        finally
        {
            outsideConnections.Dispose();
        }

        return true;
    }

    private bool TryResolveCitizen(ResidentLink link)
    {
        if (link.Citizen != Entity.Null &&
            EntityManager.Exists(link.Citizen) &&
            EntityManager.HasComponent<Citizen>(link.Citizen) &&
            EntityManager.HasComponent<HouseholdMember>(link.Citizen) &&
            EntityManager.HasComponent<PrefabRef>(link.Citizen))
        {
            return true;
        }

        link.Citizen = Entity.Null;

        if (link.Household == Entity.Null ||
            !EntityManager.Exists(link.Household) ||
            !EntityManager.HasComponent<HouseholdCitizen>(link.Household))
        {
            return false;
        }

        DynamicBuffer<HouseholdCitizen> members = EntityManager.GetBuffer<HouseholdCitizen>(link.Household);
        for (int i = 0; i < members.Length; i++)
        {
            Entity citizen = members[i].m_Citizen;
            if (citizen == Entity.Null ||
                !EntityManager.Exists(citizen) ||
                !EntityManager.HasComponent<Citizen>(citizen) ||
                !EntityManager.HasComponent<HouseholdMember>(citizen) ||
                !EntityManager.HasComponent<PrefabRef>(citizen))
            {
                continue;
            }

            link.Citizen = citizen;
            return true;
        }

        return false;
    }

    private void ApplyIdentity(Entity citizenEntity, string fullName, int birthYear)
    {
        if (citizenEntity == Entity.Null ||
            !EntityManager.Exists(citizenEntity) ||
            !EntityManager.HasComponent<Citizen>(citizenEntity))
        {
            return;
        }

        string name = string.IsNullOrWhiteSpace(fullName) ? "Спортсмен" : fullName.Trim();
        if (!m_NameSystem.TryGetCustomName(citizenEntity, out string currentName) ||
            !string.Equals(currentName, name, StringComparison.Ordinal))
        {
            m_NameSystem.SetCustomName(citizenEntity, name);
        }

        if (m_TimeDataQuery.IsEmptyIgnoreFilter)
        {
            return;
        }

        int daysPerYear = Math.Max(1, m_TimeSystem.daysPerYear);
        int currentYear = m_TimeSystem.year;
        int age = Math.Max(18, Math.Min(80, currentYear - birthYear));
        TimeData timeData = m_TimeDataQuery.GetSingleton<TimeData>();
        int currentDay = TimeSystem.GetDay(m_SimulationSystem.frameIndex, timeData);
        short desiredBirthDay = (short)(currentDay - age * daysPerYear);

        Citizen citizen = EntityManager.GetComponentData<Citizen>(citizenEntity);
        bool changed = false;

        if (citizen.m_BirthDay != desiredBirthDay)
        {
            citizen.m_BirthDay = desiredBirthDay;
            changed = true;
        }

        if ((citizen.m_State & CitizenFlags.Male) == 0)
        {
            citizen.m_State |= CitizenFlags.Male;
            changed = true;
        }

        if (citizen.GetAge() != CitizenAge.Adult)
        {
            citizen.SetAge(CitizenAge.Adult);
            changed = true;
        }

        if (changed)
        {
            EntityManager.SetComponentData(citizenEntity, citizen);
        }
    }

    private void AssignEmployment(ResidentLink link, Team team)
    {
        if (link.Citizen == Entity.Null ||
            !EntityManager.Exists(link.Citizen) ||
            !EntityManager.HasComponent<Citizen>(link.Citizen) ||
            !EntityManager.HasComponent<CurrentBuilding>(link.Citizen) ||
            !EntityManager.HasComponent<TripNeeded>(link.Citizen))
        {
            return;
        }

        if (team.VenueId < 0 ||
            !m_SportsData.TryGetVenueEntity(team.VenueId, out Entity stadium) ||
            stadium == Entity.Null ||
            !EntityManager.Exists(stadium) ||
            !EntityManager.HasComponent<PrefabRef>(stadium))
        {
            ReleaseLinkEmployment(link);
            return;
        }

        Citizen citizenData = EntityManager.GetComponentData<Citizen>(link.Citizen);
        int educationLevel = Math.Max(0, Math.Min(4, citizenData.GetEducationLevel()));
        int buildingLevel = GetBuildingLevel(stadium);
        if (!TryChooseWorkplacePrefab(educationLevel, buildingLevel, out WorkplaceComplexity complexity, out int jobLevel, out Entity workplacePrefab))
        {
            return;
        }

        Entity workplace = EnsureVenueWorkplace(team.VenueId, stadium, complexity, workplacePrefab);
        if (workplace == Entity.Null)
        {
            return;
        }

        if (EntityManager.HasComponent<Worker>(link.Citizen))
        {
            Worker currentWorker = EntityManager.GetComponentData<Worker>(link.Citizen);
            if (currentWorker.m_Workplace == workplace && currentWorker.m_Level == jobLevel)
            {
                EnsureEmployeeEntry(workplace, link.Citizen, jobLevel);
                return;
            }

            RemoveEmployeeEntry(currentWorker.m_Workplace, link.Citizen);
            currentWorker.m_Workplace = workplace;
            currentWorker.m_Level = (byte)jobLevel;
            currentWorker.m_Shift = Workshift.Day;
            EntityManager.SetComponentData(link.Citizen, currentWorker);
        }
        else
        {
            Worker worker = new()
            {
                m_Workplace = workplace,
                m_LastCommuteTime = 0f,
                m_Level = (byte)jobLevel,
                m_Shift = Workshift.Day
            };
            EntityManager.AddComponentData(link.Citizen, worker);
        }

        EnsureEmployeeEntry(workplace, link.Citizen, jobLevel);
    }

    private int GetBuildingLevel(Entity stadium)
    {
        PrefabRef prefabRef = EntityManager.GetComponentData<PrefabRef>(stadium);
        ComponentLookup<SpawnableBuildingData> spawnableLookup = GetComponentLookup<SpawnableBuildingData>(true);
        return Math.Max(1, PropertyUtils.GetBuildingLevel(prefabRef.m_Prefab, spawnableLookup));
    }

    private bool TryChooseWorkplacePrefab(
        int educationLevel,
        int buildingLevel,
        out WorkplaceComplexity complexity,
        out int jobLevel,
        out Entity prefab)
    {
        complexity = WorkplaceComplexity.Simple;
        jobLevel = educationLevel;
        prefab = Entity.Null;

        if (m_WorkplacePrefabs.Count == 0)
        {
            if (!m_WorkplacePrefabWarningLogged)
            {
                m_WorkplacePrefabWarningLogged = true;
                Mod.log.Warn("[Жители-спортсмены] Не найдены штатные префабы рабочих мест. Спортсмены будут созданы жителями, но без назначения на стадион.");
            }
            return false;
        }

        int bestScore = int.MinValue;
        WorkplaceComplexity bestComplexity = WorkplaceComplexity.Simple;
        int bestJobLevel = educationLevel;
        Entity bestPrefab = Entity.Null;

        foreach (KeyValuePair<WorkplaceComplexity, Entity> pair in m_WorkplacePrefabs)
        {
            Workplaces available = EconomyUtils.CalculateNumberOfWorkplaces(kWorkplaceCapacity, pair.Key, buildingLevel);
            if (available[educationLevel] > 0)
            {
                complexity = pair.Key;
                jobLevel = educationLevel;
                prefab = pair.Value;
                return true;
            }

            for (int level = 0; level < 5; level++)
            {
                int capacity = available[level];
                if (capacity <= 0)
                {
                    continue;
                }

                int score = capacity - Math.Abs(level - educationLevel) * 1000;
                if (score > bestScore)
                {
                    bestScore = score;
                    bestComplexity = pair.Key;
                    bestJobLevel = level;
                    bestPrefab = pair.Value;
                }
            }
        }

        if (bestPrefab == Entity.Null)
        {
            return false;
        }

        complexity = bestComplexity;
        jobLevel = bestJobLevel;
        prefab = bestPrefab;
        return true;
    }

    private void RefreshWorkplacePrefabs()
    {
        if (m_WorkplacePrefabQuery.IsEmptyIgnoreFilter)
        {
            return;
        }

        NativeArray<Entity> prefabs = m_WorkplacePrefabQuery.ToEntityArray(Allocator.Temp);
        try
        {
            for (int i = 0; i < prefabs.Length; i++)
            {
                Entity prefab = prefabs[i];
                WorkplaceData data = EntityManager.GetComponentData<WorkplaceData>(prefab);
                if (data.m_MaxWorkers <= 0)
                {
                    continue;
                }

                if (!m_WorkplacePrefabs.ContainsKey(data.m_Complexity))
                {
                    m_WorkplacePrefabs.Add(data.m_Complexity, prefab);
                }
            }
        }
        finally
        {
            prefabs.Dispose();
        }
    }

    private Entity EnsureVenueWorkplace(int venueId, Entity stadium, WorkplaceComplexity complexity, Entity prefab)
    {
        if (!m_VenueWorkplaces.TryGetValue(venueId, out Dictionary<WorkplaceComplexity, Entity> byComplexity))
        {
            byComplexity = new Dictionary<WorkplaceComplexity, Entity>();
            m_VenueWorkplaces.Add(venueId, byComplexity);
        }

        if (byComplexity.TryGetValue(complexity, out Entity existing) &&
            existing != Entity.Null &&
            EntityManager.Exists(existing))
        {
            PropertyRenter propertyRenter = EntityManager.GetComponentData<PropertyRenter>(existing);
            if (propertyRenter.m_Property != stadium)
            {
                propertyRenter.m_Property = stadium;
                EntityManager.SetComponentData(existing, propertyRenter);
            }
            return existing;
        }

        Entity workplace = EntityManager.CreateEntity(
            ComponentType.ReadWrite<Building>(),
            ComponentType.ReadWrite<WorkProvider>(),
            ComponentType.ReadWrite<PrefabRef>(),
            ComponentType.ReadWrite<PropertyRenter>(),
            ComponentType.ReadWrite<Employee>(),
            ComponentType.ReadWrite<UpdateFrame>());

        EntityManager.SetComponentData(workplace, new PrefabRef(prefab));
        EntityManager.SetComponentData(workplace, new PropertyRenter
        {
            m_Property = stadium,
            m_Rent = 0
        });
        EntityManager.SetComponentData(workplace, new WorkProvider
        {
            m_MaxWorkers = kWorkplaceCapacity
        });
        EntityManager.SetSharedComponent(workplace, new UpdateFrame((uint)(venueId & 15)));

        byComplexity[complexity] = workplace;
        return workplace;
    }

    private void EnsureEmployeeEntry(Entity workplace, Entity citizen, int level)
    {
        if (workplace == Entity.Null ||
            !EntityManager.Exists(workplace) ||
            !EntityManager.HasComponent<Employee>(workplace))
        {
            return;
        }

        DynamicBuffer<Employee> employees = EntityManager.GetBuffer<Employee>(workplace);
        for (int i = 0; i < employees.Length; i++)
        {
            if (employees[i].m_Worker == citizen)
            {
                Employee employee = employees[i];
                employee.m_Level = (byte)level;
                employees[i] = employee;
                return;
            }
        }

        employees.Add(new Employee
        {
            m_Worker = citizen,
            m_Level = (byte)level
        });
    }

    private void RemoveEmployeeEntry(Entity workplace, Entity citizen)
    {
        if (workplace == Entity.Null ||
            !EntityManager.Exists(workplace) ||
            !EntityManager.HasComponent<Employee>(workplace))
        {
            return;
        }

        DynamicBuffer<Employee> employees = EntityManager.GetBuffer<Employee>(workplace);
        for (int i = employees.Length - 1; i >= 0; i--)
        {
            if (employees[i].m_Worker == citizen)
            {
                employees.RemoveAtSwapBack(i);
            }
        }
    }

    private void ReleaseLinkEmployment(ResidentLink link)
    {
        if (link == null || link.Citizen == Entity.Null || !EntityManager.Exists(link.Citizen))
        {
            return;
        }

        if (EntityManager.HasComponent<Worker>(link.Citizen))
        {
            Worker worker = EntityManager.GetComponentData<Worker>(link.Citizen);
            RemoveEmployeeEntry(worker.m_Workplace, link.Citizen);
            EntityManager.RemoveComponent<Worker>(link.Citizen);
        }

        if (EntityManager.HasComponent<TravelPurpose>(link.Citizen))
        {
            TravelPurpose purpose = EntityManager.GetComponentData<TravelPurpose>(link.Citizen);
            if (purpose.m_Purpose == Purpose.GoingToWork || purpose.m_Purpose == Purpose.Working)
            {
                EntityManager.RemoveComponent<TravelPurpose>(link.Citizen);
            }
        }

        if (EntityManager.HasComponent<TripNeeded>(link.Citizen))
        {
            DynamicBuffer<TripNeeded> trips = EntityManager.GetBuffer<TripNeeded>(link.Citizen);
            for (int i = trips.Length - 1; i >= 0; i--)
            {
                if (trips[i].m_Purpose == Purpose.GoingToWork)
                {
                    trips.RemoveAtSwapBack(i);
                }
            }
        }
    }

    private void ReleaseAllEmployment()
    {
        foreach (ResidentLink link in m_PlayerResidents.Values)
        {
            ReleaseLinkEmployment(link);
        }

        foreach (ResidentLink link in m_CoachResidents.Values)
        {
            ReleaseLinkEmployment(link);
        }
    }

    private void CleanupUnusedWorkplaces(HashSet<int> activeVenues)
    {
        if (m_VenueWorkplaces.Count == 0)
        {
            return;
        }

        EntityCommandBuffer commandBuffer = m_EndFrameBarrier.CreateCommandBuffer();
        List<int> removeVenues = new();

        foreach (KeyValuePair<int, Dictionary<WorkplaceComplexity, Entity>> venue in m_VenueWorkplaces)
        {
            if (activeVenues.Contains(venue.Key))
            {
                continue;
            }

            foreach (Entity workplace in venue.Value.Values)
            {
                if (workplace != Entity.Null && EntityManager.Exists(workplace))
                {
                    commandBuffer.DestroyEntity(workplace);
                }
            }

            removeVenues.Add(venue.Key);
        }

        foreach (int venueId in removeVenues)
        {
            m_VenueWorkplaces.Remove(venueId);
        }
    }

    private void DestroyAllWorkplaces()
    {
        if (m_VenueWorkplaces.Count == 0)
        {
            return;
        }

        EntityCommandBuffer commandBuffer = m_EndFrameBarrier.CreateCommandBuffer();
        foreach (Dictionary<WorkplaceComplexity, Entity> venue in m_VenueWorkplaces.Values)
        {
            foreach (Entity workplace in venue.Values)
            {
                if (workplace != Entity.Null && EntityManager.Exists(workplace))
                {
                    commandBuffer.DestroyEntity(workplace);
                }
            }
        }

        m_VenueWorkplaces.Clear();
    }

    public void ForgetAllLinks()
    {
        ReleaseAllEmployment();
        DestroyAllWorkplaces();
        m_PlayerResidents.Clear();
        m_CoachResidents.Clear();
    }

    public void SetDefaults(Context context)
    {
        m_PlayerResidents.Clear();
        m_CoachResidents.Clear();
        m_VenueWorkplaces.Clear();
    }

    public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
    {
        writer.Write(kSaveVersion);

        writer.Write(m_PlayerResidents.Count);
        foreach (KeyValuePair<int, ResidentLink> pair in m_PlayerResidents)
        {
            writer.Write(pair.Key);
            writer.Write(pair.Value.Household);
            writer.Write(pair.Value.Citizen);
            writer.Write(pair.Value.TeamId);
            writer.Write(pair.Value.VenueId);
        }

        writer.Write(m_CoachResidents.Count);
        foreach (KeyValuePair<int, ResidentLink> pair in m_CoachResidents)
        {
            writer.Write(pair.Key);
            writer.Write(pair.Value.Household);
            writer.Write(pair.Value.Citizen);
            writer.Write(pair.Value.TeamId);
            writer.Write(pair.Value.VenueId);
        }

        writer.Write(m_VenueWorkplaces.Count);
        foreach (KeyValuePair<int, Dictionary<WorkplaceComplexity, Entity>> venue in m_VenueWorkplaces)
        {
            writer.Write(venue.Key);
            writer.Write(venue.Value.Count);
            foreach (KeyValuePair<WorkplaceComplexity, Entity> workplace in venue.Value)
            {
                writer.Write((int)workplace.Key);
                writer.Write(workplace.Value);
            }
        }
    }

    public void Deserialize<TReader>(TReader reader) where TReader : IReader
    {
        m_PlayerResidents.Clear();
        m_CoachResidents.Clear();
        m_VenueWorkplaces.Clear();

        reader.Read(out int version);
        if (version != kSaveVersion)
        {
            Mod.log.Warn("[Жители-спортсмены] Версия сохранённых связей не поддерживается. Жители города останутся нетронутыми; связи будут восстановлены для текущего состава.");
            return;
        }

        ReadResidentLinks(reader, m_PlayerResidents);
        ReadResidentLinks(reader, m_CoachResidents);

        reader.Read(out int venueCount);
        venueCount = Math.Max(0, Math.Min(venueCount, 10000));
        for (int i = 0; i < venueCount; i++)
        {
            reader.Read(out int venueId);
            reader.Read(out int workplaceCount);
            workplaceCount = Math.Max(0, Math.Min(workplaceCount, 8));

            Dictionary<WorkplaceComplexity, Entity> byComplexity = new();
            for (int j = 0; j < workplaceCount; j++)
            {
                reader.Read(out int complexityValue);
                reader.Read(out Entity workplace);
                if (Enum.IsDefined(typeof(WorkplaceComplexity), complexityValue))
                {
                    byComplexity[(WorkplaceComplexity)complexityValue] = workplace;
                }
            }

            m_VenueWorkplaces[venueId] = byComplexity;
        }
    }

    private static void ReadResidentLinks<TReader>(TReader reader, Dictionary<int, ResidentLink> destination)
        where TReader : IReader
    {
        reader.Read(out int count);
        count = Math.Max(0, Math.Min(count, 100000));

        for (int i = 0; i < count; i++)
        {
            reader.Read(out int id);
            reader.Read(out Entity household);
            reader.Read(out Entity citizen);
            reader.Read(out int teamId);
            reader.Read(out int venueId);

            destination[id] = new ResidentLink
            {
                Household = household,
                Citizen = citizen,
                TeamId = teamId,
                VenueId = venueId
            };
        }
    }
}
