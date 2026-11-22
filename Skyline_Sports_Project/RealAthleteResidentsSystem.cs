using System;
using System.Collections.Generic;
using System.Reflection;
using Colossal.Serialization.Entities;
using Game;
using Game.Agents;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
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
    private const int kSaveVersion = 4;

    private sealed class ResidentLink
    {
        public Entity Household;
        public Entity Citizen;
        public Entity Workplace;
        public int TeamId = -1;
        public int VenueId = -1;
        public int BirthYear = int.MinValue;
        public bool EmploymentLogged;
        public int PendingSinceDay = int.MinValue;
        public int LastSalaryDay = int.MinValue;
        public bool StartingWealthInitialized;
    }

    private SportsDataSystem m_SportsData;
    private NameSystem m_NameSystem;
    private SimulationSystem m_SimulationSystem;
    private TimeSystem m_TimeSystem;

    private EntityQuery m_HouseholdPrefabQuery;
    private EntityQuery m_OutsideConnectionQuery;
    private EntityQuery m_WorkplacePrefabQuery;
    private EntityQuery m_CompanyWorkplacePrefabQuery;
    private EntityQuery m_TimeDataQuery;
    private EntityQuery m_DemandParameterQuery;
    private EntityQuery m_CitizenPrefabQuery;
    private EntityQuery m_HappinessParameterQuery;

    private readonly Dictionary<int, ResidentLink> m_PlayerResidents = new();
    private readonly Dictionary<int, ResidentLink> m_CoachResidents = new();
    private readonly Dictionary<WorkplaceComplexity, Entity> m_WorkplacePrefabs = new();
    private readonly List<Entity> m_LegacyWorkplaces = new();

    private bool m_LastEnabled;
    private bool m_InitialCleanupPending = true;
    private bool m_WorkplacePrefabWarningLogged;
    private bool m_HouseholdPrefabWarningLogged;
    private bool m_OutsideConnectionWarningLogged;
    private bool m_CitizenPrefabWarningLogged;
    private bool m_ProfileRefreshPending;

    public override int GetUpdateInterval(SystemUpdatePhase phase) => 256;

    [Preserve]
    protected override void OnCreate()
    {
        base.OnCreate();

        m_SportsData = World.GetOrCreateSystemManaged<SportsDataSystem>();
        m_NameSystem = World.GetOrCreateSystemManaged<NameSystem>();
        m_SimulationSystem = World.GetOrCreateSystemManaged<SimulationSystem>();
        m_TimeSystem = World.GetOrCreateSystemManaged<TimeSystem>();

        m_HouseholdPrefabQuery = GetEntityQuery(
            ComponentType.ReadOnly<HouseholdData>(),
            ComponentType.ReadOnly<ArchetypeData>(),
            ComponentType.Exclude<DynamicHousehold>(),
            ComponentType.Exclude<Deleted>(),
            ComponentType.Exclude<Temp>());

        m_OutsideConnectionQuery = GetEntityQuery(
            ComponentType.ReadOnly<Game.Objects.OutsideConnection>(),
            ComponentType.Exclude<Game.Objects.ElectricityOutsideConnection>(),
            ComponentType.Exclude<Game.Objects.WaterPipeOutsideConnection>(),
            ComponentType.Exclude<Deleted>(),
            ComponentType.Exclude<Temp>());

        m_WorkplacePrefabQuery = GetEntityQuery(
            ComponentType.ReadOnly<PrefabData>(),
            ComponentType.ReadOnly<WorkplaceData>(),
            ComponentType.Exclude<Deleted>(),
            ComponentType.Exclude<Temp>());

        m_CompanyWorkplacePrefabQuery = GetEntityQuery(
            ComponentType.ReadOnly<PrefabData>(),
            ComponentType.ReadOnly<WorkplaceData>(),
            ComponentType.ReadOnly<CompanyBrandElement>(),
            ComponentType.Exclude<Deleted>(),
            ComponentType.Exclude<Temp>());

        m_TimeDataQuery = GetEntityQuery(ComponentType.ReadOnly<TimeData>());
        m_DemandParameterQuery = GetEntityQuery(ComponentType.ReadOnly<DemandParameterData>());
        m_CitizenPrefabQuery = GetEntityQuery(ComponentType.ReadOnly<CitizenData>(), ComponentType.ReadOnly<ArchetypeData>());
        m_HappinessParameterQuery = GetEntityQuery(ComponentType.ReadOnly<CitizenHappinessParameterData>());

        Mod.log.Info("[Жители-спортсмены] Система создана. Функция управляется отдельной настройкой.");
    }

    [Preserve]
    protected override void OnUpdate()
    {
        try
        {
            if (m_LegacyWorkplaces.Count > 0)
            {
                ReleaseAllEmployment();
                DeactivateLegacyWorkplaces();
            }

            if (Mod.Settings == null || !Mod.Settings.RealAthleteResidentsEnabled)
            {
                if (m_LastEnabled || m_InitialCleanupPending)
                {
                    ReleaseAllEmployment();
                    Mod.log.Info("[Жители-спортсмены] Функция выключена. Спортсмены остаются жителями города, их дома и автомобили не удаляются.");
                }

                ResetSalaryPaymentDays();
                m_LastEnabled = false;
                m_InitialCleanupPending = false;
                return;
            }

            m_LastEnabled = true;
            m_InitialCleanupPending = false;

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

    public bool ForceCreatePlayerResident(int playerId)
    {
        if (Mod.Settings == null || !Mod.Settings.RealAthleteResidentsEnabled || m_SportsData?.Sports == null)
        {
            return false;
        }

        foreach (Team team in m_SportsData.Sports.UserTeams())
        {
            if (team == null || team.Retired || team.Sport != Sport.Soccer)
            {
                continue;
            }

            foreach (Player player in m_SportsData.Sports.RosterOf(team.Id))
            {
                if (player != null && player.Id == playerId)
                {
                    SyncPlayer(player, team, forceCreate: true);
                    return TryGetPlayerResident(playerId, out _);
                }
            }
        }

        return false;
    }

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
            !EntityManager.HasComponent<HouseholdMember>(link.Citizen) ||
            EntityManager.HasComponent<Created>(link.Citizen))
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

        foreach (Team team in world.UserTeams())
        {
            if (team == null || team.Retired || !IsSupportedSport(team.Sport))
            {
                continue;
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

        if (m_ProfileRefreshPending)
        {
            m_ProfileRefreshPending = false;
            World.GetOrCreateSystemManaged<SportsUISystem>().NotifyResidentLinksChanged();
        }
    }

    private static bool IsSupportedSport(Sport sport)
    {
        return sport == Sport.Soccer;
    }

    private void SyncPlayer(Player player, Team team, bool forceCreate = false)
    {
        if (!m_PlayerResidents.TryGetValue(player.Id, out ResidentLink link))
        {
            link = new ResidentLink();
            m_PlayerResidents.Add(player.Id, link);
        }

        link.TeamId = team.Id;
        link.VenueId = team.VenueId;

        bool hadResident = link.Citizen != Entity.Null && EntityManager.Exists(link.Citizen) && EntityManager.HasComponent<Citizen>(link.Citizen) && EntityManager.HasComponent<HouseholdMember>(link.Citizen) && !EntityManager.HasComponent<Created>(link.Citizen);
        if (!hadResident && link.Citizen != Entity.Null)
        {
            m_ProfileRefreshPending = true;
        }
        EnsureHousehold(link, player.Id);
        if (!TryResolveCitizen(link))
        {
            int currentDay = CurrentDay();
            bool timedOut = link.PendingSinceDay == int.MinValue || currentDay - link.PendingSinceDay >= 1;
            if ((forceCreate || timedOut) && TrySpawnMaleCitizen(link, player.Id))
            {
                Mod.log.Warn("[Жители-спортсмены] Штатное создание гражданина не завершилось; создан отдельный мужчина-футболист для «" + player.Name + "».");
            }
            if (!TryResolveCitizen(link))
            {
                return;
            }
        }

        if (!hadResident)
        {
            m_ProfileRefreshPending = true;
            Mod.log.Info("[Жители-спортсмены] Создан городской житель для игрока «" + player.Name + "» (ID " + player.Id + ").");
        }

        ApplyIdentity(link, player.Name, player.BirthYear);
        ApplyPlayerSalary(link, player.Salary);
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

        bool hadResident = link.Citizen != Entity.Null && EntityManager.Exists(link.Citizen) && EntityManager.HasComponent<Citizen>(link.Citizen) && EntityManager.HasComponent<HouseholdMember>(link.Citizen);
        if (!hadResident && link.Citizen != Entity.Null)
        {
            m_ProfileRefreshPending = true;
        }
        EnsureHousehold(link, coach.Id ^ int.MinValue);
        if (!TryResolveCitizen(link))
        {
            return;
        }

        if (!hadResident)
        {
            m_ProfileRefreshPending = true;
            Mod.log.Info("[Жители-спортсмены] Создан городской житель для тренера «" + coach.Name + "» (ID " + coach.Id + ").");
        }

        ApplyIdentity(link, coach.Name, coach.BirthYear);
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
            link.PendingSinceDay = CurrentDay();
            link.LastSalaryDay = CurrentDay();
            link.StartingWealthInitialized = false;
            link.BirthYear = int.MinValue;
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
                Mod.log.Warn("[Жители-спортсмены] Не найдены дорожные внешние соединения для штатного создания домохозяйства.");
            }
            return false;
        }

        List<Entity> petFreePrefabs = new();
        List<int> petFreeWeights = new();
        List<Entity> fallbackPrefabs = new();
        List<int> fallbackWeights = new();
        int petFreeWeight = 0;
        int fallbackWeight = 0;

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
                    data.m_Weight <= 0)
                {
                    continue;
                }

                fallbackPrefabs.Add(prefab);
                fallbackWeights.Add(data.m_Weight);
                fallbackWeight += data.m_Weight;

                if (data.m_FirstPetProbability == 0 && data.m_NextPetProbability == 0)
                {
                    petFreePrefabs.Add(prefab);
                    petFreeWeights.Add(data.m_Weight);
                    petFreeWeight += data.m_Weight;
                }
            }
        }
        finally
        {
            prefabs.Dispose();
        }

        List<Entity> candidates = petFreeWeight > 0 ? petFreePrefabs : fallbackPrefabs;
        List<int> weights = petFreeWeight > 0 ? petFreeWeights : fallbackWeights;
        int totalWeight = petFreeWeight > 0 ? petFreeWeight : fallbackWeight;

        if (candidates.Count == 0 || totalWeight <= 0)
        {
            if (!m_HouseholdPrefabWarningLogged)
            {
                m_HouseholdPrefabWarningLogged = true;
                Mod.log.Warn("[Жители-спортсмены] Не найдено штатное домохозяйство с одним взрослым без детей. Создание пропущено, чтобы не порождать лишних горожан.");
            }
            return false;
        }

        uint seed = (uint)stableId ^ m_SimulationSystem.frameIndex;
        Unity.Mathematics.Random random = Unity.Mathematics.Random.CreateFromIndex(seed);
        int roll = random.NextInt(totalWeight);
        Entity selectedPrefab = candidates[0];
        for (int i = 0; i < candidates.Count; i++)
        {
            roll -= weights[i];
            if (roll < 0)
            {
                selectedPrefab = candidates[i];
                break;
            }
        }

        household = EntityManager.CreateEntity(
            ComponentType.ReadWrite<Household>(),
            ComponentType.ReadWrite<PrefabRef>(),
            ComponentType.ReadWrite<HouseholdCitizen>(),
            ComponentType.ReadWrite<Resources>(),
            ComponentType.ReadWrite<CurrentBuilding>());

        EntityManager.SetComponentData(household, new PrefabRef(selectedPrefab));

        NativeArray<Entity> outsideArray = m_OutsideConnectionQuery.ToEntityArray(Allocator.Temp);
        NativeList<Entity> outsideConnections = new NativeList<Entity>(Allocator.Temp);
        for (int i = 0; i < outsideArray.Length; i++)
        {
            outsideConnections.Add(outsideArray[i]);
        }
        outsideArray.Dispose();

        try
        {
            if (outsideConnections.Length == 0 || m_DemandParameterQuery.IsEmptyIgnoreFilter)
            {
                EntityManager.DestroyEntity(household);
                household = Entity.Null;
                return false;
            }

            ComponentLookup<OutsideConnectionData> outsideData = GetComponentLookup<OutsideConnectionData>(true);
            ComponentLookup<PrefabRef> prefabRefs = GetComponentLookup<PrefabRef>(true);
            DemandParameterData demandParameters = m_DemandParameterQuery.GetSingleton<DemandParameterData>();

            bool hasOutside = BuildingUtils.GetRandomOutsideConnectionByParameters(
                ref outsideConnections,
                ref outsideData,
                ref prefabRefs,
                random,
                demandParameters.m_CitizenOCSpawnParameters,
                out Entity outside);

            // Vanilla first uses the city's configured distribution. If that roll
            // selects a transport type the city does not have, retry with road only.
            if (!hasOutside)
            {
                float4[] fallbackTypes =
                {
                    new float4(1f, 0f, 0f, 0f),
                    new float4(0f, 1f, 0f, 0f),
                    new float4(0f, 0f, 1f, 0f),
                    new float4(0f, 0f, 0f, 1f)
                };

                for (int i = 0; i < fallbackTypes.Length && !hasOutside; i++)
                {
                    hasOutside = BuildingUtils.GetRandomOutsideConnectionByParameters(
                        ref outsideConnections,
                        ref outsideData,
                        ref prefabRefs,
                        random,
                        fallbackTypes[i],
                        out outside);
                }
            }

            if (!hasOutside)
            {
                if (!m_OutsideConnectionWarningLogged)
                {
                    m_OutsideConnectionWarningLogged = true;
                    Mod.log.Warn("[Жители-спортсмены] Не найдено подходящее внешнее соединение. Создание домохозяйства отложено без появления пустого домохозяйства.");
                }
                EntityManager.DestroyEntity(household);
                household = Entity.Null;
                return false;
            }

            EntityManager.SetComponentData(household, new CurrentBuilding { m_CurrentBuilding = outside });
        }
        finally
        {
            outsideConnections.Dispose();
        }

        return true;
    }

    private int CurrentDay()
    {
        if (m_TimeDataQuery.IsEmptyIgnoreFilter)
        {
            return unchecked((int)m_SimulationSystem.frameIndex);
        }

        return TimeSystem.GetDay(m_SimulationSystem.frameIndex, m_TimeDataQuery.GetSingleton<TimeData>());
    }

    private void ResetSalaryPaymentDays()
    {
        int day = CurrentDay();
        foreach (ResidentLink link in m_PlayerResidents.Values)
        {
            link.LastSalaryDay = day;
        }
    }

    private bool TryGetMaleCitizenPrefab(out Entity prefab)
    {
        prefab = Entity.Null;
        if (m_CitizenPrefabQuery.IsEmptyIgnoreFilter)
        {
            return false;
        }

        NativeArray<Entity> prefabs = m_CitizenPrefabQuery.ToEntityArray(Allocator.Temp);
        try
        {
            for (int i = 0; i < prefabs.Length; i++)
            {
                Entity candidate = prefabs[i];
                if (EntityManager.GetComponentData<CitizenData>(candidate).m_Male)
                {
                    prefab = candidate;
                    return true;
                }
            }
        }
        finally
        {
            prefabs.Dispose();
        }

        return false;
    }

    private bool TrySpawnMaleCitizen(ResidentLink link, int stableId)
    {
        if (link == null || link.Household == Entity.Null || !EntityManager.Exists(link.Household) ||
            !EntityManager.HasComponent<Household>(link.Household) || !EntityManager.HasBuffer<HouseholdCitizen>(link.Household))
        {
            return false;
        }

        if (!TryGetMaleCitizenPrefab(out Entity malePrefab) || !EntityManager.HasComponent<ArchetypeData>(malePrefab))
        {
            if (!m_CitizenPrefabWarningLogged)
            {
                m_CitizenPrefabWarningLogged = true;
                Mod.log.Warn("[Жители-спортсмены] Не найден мужской префаб гражданина. Принудительное создание отложено.");
            }
            return false;
        }

        Entity spawnBuilding = Entity.Null;
        if (EntityManager.HasComponent<CurrentBuilding>(link.Household))
        {
            spawnBuilding = EntityManager.GetComponentData<CurrentBuilding>(link.Household).m_CurrentBuilding;
        }
        if (spawnBuilding == Entity.Null || !EntityManager.Exists(spawnBuilding))
        {
            if (m_OutsideConnectionQuery.IsEmptyIgnoreFilter)
            {
                return false;
            }
            NativeArray<Entity> outside = m_OutsideConnectionQuery.ToEntityArray(Allocator.Temp);
            try
            {
                if (outside.Length > 0)
                {
                    uint seed = unchecked((uint)stableId + m_SimulationSystem.frameIndex + 1u);
                    Unity.Mathematics.Random random = Unity.Mathematics.Random.CreateFromIndex(seed);
                    spawnBuilding = outside[random.NextInt(outside.Length)];
                }
            }
            finally
            {
                outside.Dispose();
            }
        }
        if (spawnBuilding == Entity.Null || !EntityManager.Exists(spawnBuilding))
        {
            return false;
        }

        if (EntityManager.HasComponent<CurrentBuilding>(link.Household))
        {
            EntityManager.RemoveComponent<CurrentBuilding>(link.Household);
        }
        if (!EntityManager.HasComponent<PropertySeeker>(link.Household))
        {
            EntityManager.AddComponent<PropertySeeker>(link.Household);
        }
        else if (!EntityManager.IsComponentEnabled<PropertySeeker>(link.Household))
        {
            EntityManager.SetComponentEnabled<PropertySeeker>(link.Household, true);
        }

        ArchetypeData archetype = EntityManager.GetComponentData<ArchetypeData>(malePrefab);
        object citizenArchetype = archetype.m_Archetype;
        MethodInfo createEntityMethod = typeof(EntityManager).GetMethod(
            "CreateEntity",
            BindingFlags.Instance | BindingFlags.Public,
            null,
            new[] { citizenArchetype.GetType() },
            null);
        if (createEntityMethod == null)
        {
            Mod.log.Warn("[Жители-спортсмены] Не найдена штатная ECS-функция создания гражданина по префабу.");
            return false;
        }
        Entity citizen = (Entity)createEntityMethod.Invoke(EntityManager, new[] { citizenArchetype });
        EntityManager.SetComponentData(citizen, new PrefabRef(malePrefab));
        EntityManager.SetComponentData(citizen, new Citizen
        {
            m_BirthDay = 1,
            m_State = CitizenFlags.Male
        });
        EntityManager.AddComponentData(citizen, new HouseholdMember { m_Household = link.Household });
        EntityManager.AddComponentData(citizen, new CurrentBuilding { m_CurrentBuilding = spawnBuilding });
        if (!EntityManager.HasComponent<Created>(citizen))
        {
            EntityManager.AddComponent<Created>(citizen);
        }

        link.Citizen = citizen;
        link.PendingSinceDay = int.MinValue;
        link.BirthYear = int.MinValue;
        link.EmploymentLogged = false;
        return true;
    }

    private void ApplyPlayerSalary(ResidentLink link, int seasonSalary)
    {
        if (link == null || link.Household == Entity.Null || !EntityManager.Exists(link.Household) ||
            !EntityManager.HasComponent<Household>(link.Household) || !EntityManager.HasBuffer<Resources>(link.Household))
        {
            return;
        }

        DynamicBuffer<Resources> resources = EntityManager.GetBuffer<Resources>(link.Household);
        if (!link.StartingWealthInitialized && !m_HappinessParameterQuery.IsEmptyIgnoreFilter)
        {
            CitizenHappinessParameterData parameters = m_HappinessParameterQuery.GetSingleton<CitizenHappinessParameterData>();
            int poorFloor = Math.Max(0, parameters.m_WealthyMoneyAmount.x);
            int poorCeiling = Math.Max(poorFloor + 1, parameters.m_WealthyMoneyAmount.y);
            int targetWealth = Math.Min(poorCeiling - 1, poorFloor + 1);

            Household household = EntityManager.GetComponentData<Household>(link.Household);
            household.m_Resources = 0;
            EntityManager.SetComponentData(link.Household, household);
            int currentMoney = EconomyUtils.GetResources(Resource.Money, resources);
            EconomyUtils.AddResources(Resource.Money, targetWealth - currentMoney, resources);
            link.StartingWealthInitialized = true;
            Mod.log.Info("[Жители-спортсмены] Домохозяйство футболиста начинает с уровня «Бедные».");
        }

        int currentDay = CurrentDay();
        if (link.LastSalaryDay == int.MinValue)
        {
            link.LastSalaryDay = currentDay;
            return;
        }
        if (currentDay <= link.LastSalaryDay)
        {
            return;
        }

        int elapsedDays = Math.Min(30, currentDay - link.LastSalaryDay);
        long monthlySalary = Math.Max(0L, (long)seasonSalary) * 1000L / 12L;
        long payment = monthlySalary * elapsedDays;
        if (payment > 0)
        {
            EconomyUtils.AddResources(Resource.Money, (int)Math.Min(int.MaxValue, payment), resources);
        }
        link.LastSalaryDay = currentDay;
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
            link.BirthYear = int.MinValue;
            link.EmploymentLogged = false;
            return true;
        }

        return false;
    }

    private void ApplyIdentity(ResidentLink link, string fullName, int birthYear)
    {
        if (link == null ||
            link.Citizen == Entity.Null ||
            !EntityManager.Exists(link.Citizen) ||
            !EntityManager.HasComponent<Citizen>(link.Citizen))
        {
            return;
        }

        Citizen citizen = EntityManager.GetComponentData<Citizen>(link.Citizen);
        if ((citizen.m_State & CitizenFlags.Male) == 0)
        {
            citizen.m_State |= CitizenFlags.Male;
            EntityManager.SetComponentData(link.Citizen, citizen);
        }
        if (TryGetMaleCitizenPrefab(out Entity malePrefab) && EntityManager.HasComponent<PrefabRef>(link.Citizen))
        {
            PrefabRef currentPrefab = EntityManager.GetComponentData<PrefabRef>(link.Citizen);
            if (currentPrefab.m_Prefab != malePrefab)
            {
                currentPrefab.m_Prefab = malePrefab;
                EntityManager.SetComponentData(link.Citizen, currentPrefab);
            }
        }

        string name = string.IsNullOrWhiteSpace(fullName) ? "Футболист" : fullName.Trim();
        if (!m_NameSystem.TryGetCustomName(link.Citizen, out string currentName) ||
            !string.Equals(currentName, name, StringComparison.Ordinal))
        {
            m_NameSystem.SetCustomName(link.Citizen, name);
        }

        // CitizenInitializeSystem assigns a random adult age to newly spawned citizens.
        // Wait until its Created marker is gone before applying the footballer's exact age.
        if (EntityManager.HasComponent<Created>(link.Citizen))
        {
            return;
        }

        // Set the birthday only when the roster birth year changes. Recalculating
        // it every update would move the citizen's birthday forward every day and
        // prevent the citizen from aging naturally in the simulation.
        if (link.BirthYear == birthYear || m_TimeDataQuery.IsEmptyIgnoreFilter)
        {
            return;
        }

        int daysPerYear = Math.Max(1, m_TimeSystem.daysPerYear);
        int currentYear = m_TimeSystem.year;
        int age = Math.Max(1, Math.Min(100, currentYear - birthYear));
        TimeData timeData = m_TimeDataQuery.GetSingleton<TimeData>();
        int currentDay = TimeSystem.GetDay(m_SimulationSystem.frameIndex, timeData);
        short desiredBirthDay = unchecked((short)(currentDay - age * daysPerYear));

        Citizen citizenData = EntityManager.GetComponentData<Citizen>(link.Citizen);
        bool changed = false;

        if (citizenData.m_BirthDay != desiredBirthDay)
        {
            citizenData.m_BirthDay = desiredBirthDay;
            changed = true;
        }

        CitizenAge desiredAge = age < 13 ? CitizenAge.Child : age < 18 ? CitizenAge.Teen : age < 65 ? CitizenAge.Adult : CitizenAge.Elderly;
        if (citizenData.GetAge() != desiredAge)
        {
            citizenData.SetAge(desiredAge);
            changed = true;
        }

        if (changed)
        {
            EntityManager.SetComponentData(link.Citizen, citizenData);
        }

        link.BirthYear = birthYear;
    }

    private void AssignEmployment(ResidentLink link, Team team)
    {
        if (link.Citizen == Entity.Null ||
            !EntityManager.Exists(link.Citizen) ||
            !EntityManager.HasComponent<Citizen>(link.Citizen) ||
            !EntityManager.HasComponent<CurrentBuilding>(link.Citizen) ||
            !EntityManager.HasComponent<TripNeeded>(link.Citizen) ||
            !EntityManager.HasComponent<HouseholdMember>(link.Citizen))
        {
            ReleaseLinkEmployment(link);
            return;
        }

        HouseholdMember member = EntityManager.GetComponentData<HouseholdMember>(link.Citizen);
        if (member.m_Household == Entity.Null ||
            !EntityManager.Exists(member.m_Household) ||
            !EntityManager.HasComponent<PropertyRenter>(member.m_Household))
        {
            ReleaseLinkEmployment(link);
            return;
        }

        PropertyRenter home = EntityManager.GetComponentData<PropertyRenter>(member.m_Household);
        if (home.m_Property == Entity.Null ||
            !EntityManager.Exists(home.m_Property) ||
            !EntityManager.HasComponent<PrefabRef>(home.m_Property))
        {
            ReleaseLinkEmployment(link);
            return;
        }

        if (EntityManager.HasComponent<MovingAway>(member.m_Household))
        {
            ReleaseLinkEmployment(link);
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
        if (!TryChooseWorkplacePrefab(educationLevel, stadium, out int jobLevel, out Entity workplacePrefab))
        {
            ReleaseLinkEmployment(link);
            return;
        }

        Entity workplace = EnsureResidentWorkplace(link, team.VenueId, stadium, workplacePrefab);
        if (workplace == Entity.Null)
        {
            ReleaseLinkEmployment(link);
            return;
        }

        bool newlyAssigned = !EntityManager.HasComponent<Worker>(link.Citizen);
        if (EntityManager.HasComponent<Worker>(link.Citizen))
        {
            Worker currentWorker = EntityManager.GetComponentData<Worker>(link.Citizen);
            if (currentWorker.m_Workplace != workplace)
            {
                newlyAssigned = true;
                RemoveEmployeeEntry(currentWorker.m_Workplace, link.Citizen);
                currentWorker.m_Workplace = workplace;
                currentWorker.m_Level = (byte)jobLevel;
                currentWorker.m_Shift = Workshift.Day;
                EntityManager.SetComponentData(link.Citizen, currentWorker);
            }
            else if (currentWorker.m_Level != jobLevel)
            {
                currentWorker.m_Level = (byte)jobLevel;
                EntityManager.SetComponentData(link.Citizen, currentWorker);
            }
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
        if (newlyAssigned && !link.EmploymentLogged)
        {
            link.EmploymentLogged = true;
            Mod.log.Info("[Жители-спортсмены] Назначено рабочее место на стадионе для жителя «" + (m_NameSystem.TryGetCustomName(link.Citizen, out string residentName) ? residentName : "Спортсмен") + "».");
        }
    }

    private bool TryChooseWorkplacePrefab(int educationLevel, Entity stadium, out int jobLevel, out Entity prefab)
    {
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
        int bestLevel = educationLevel;
        Entity bestPrefab = Entity.Null;
        ComponentLookup<SpawnableBuildingData> spawnableLookup = GetComponentLookup<SpawnableBuildingData>(true);
        PrefabRef stadiumPrefab = EntityManager.GetComponentData<PrefabRef>(stadium);
        int stadiumLevel = Math.Max(1, PropertyUtils.GetBuildingLevel(stadiumPrefab.m_Prefab, spawnableLookup));

        foreach (KeyValuePair<WorkplaceComplexity, Entity> pair in m_WorkplacePrefabs)
        {
            Workplaces available = EconomyUtils.CalculateNumberOfWorkplaces(1, pair.Key, stadiumLevel);
            if (available[educationLevel] > 0)
            {
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
                    bestLevel = level;
                    bestPrefab = pair.Value;
                }
            }
        }

        if (bestPrefab == Entity.Null)
        {
            return false;
        }

        jobLevel = bestLevel;
        prefab = bestPrefab;
        return true;
    }

    private void RefreshWorkplacePrefabs()
    {
        EntityQuery source = !m_CompanyWorkplacePrefabQuery.IsEmptyIgnoreFilter
            ? m_CompanyWorkplacePrefabQuery
            : m_WorkplacePrefabQuery;

        if (source.IsEmptyIgnoreFilter)
        {
            return;
        }

        NativeArray<Entity> prefabs = source.ToEntityArray(Allocator.Temp);
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

    private Entity EnsureResidentWorkplace(ResidentLink link, int venueId, Entity stadium, Entity prefab)
    {
        if (link.Workplace != Entity.Null && EntityManager.Exists(link.Workplace) &&
            EntityManager.HasComponent<CompanyData>(link.Workplace) &&
            EntityManager.HasComponent<PropertyRenter>(link.Workplace) &&
            EntityManager.HasComponent<PrefabRef>(link.Workplace) &&
            EntityManager.HasComponent<WorkProvider>(link.Workplace) &&
            EntityManager.HasBuffer<Employee>(link.Workplace))
        {
            PropertyRenter renter = EntityManager.GetComponentData<PropertyRenter>(link.Workplace);
            if (renter.m_Property != stadium || renter.m_Rent != 0)
            {
                renter.m_Property = stadium;
                renter.m_Rent = 0;
                EntityManager.SetComponentData(link.Workplace, renter);
            }

            PrefabRef currentPrefab = EntityManager.GetComponentData<PrefabRef>(link.Workplace);
            if (currentPrefab.m_Prefab != prefab)
            {
                currentPrefab.m_Prefab = prefab;
                EntityManager.SetComponentData(link.Workplace, currentPrefab);
            }

            WorkProvider provider = EntityManager.GetComponentData<WorkProvider>(link.Workplace);
            if (provider.m_MaxWorkers != 1)
            {
                provider.m_MaxWorkers = 1;
                EntityManager.SetComponentData(link.Workplace, provider);
            }

            return link.Workplace;
        }

        if (link.Workplace != Entity.Null && EntityManager.Exists(link.Workplace))
        {
            if (EntityManager.HasBuffer<Employee>(link.Workplace))
            {
                EntityManager.GetBuffer<Employee>(link.Workplace).Clear();
            }

            if (EntityManager.HasComponent<WorkProvider>(link.Workplace))
            {
                WorkProvider oldProvider = EntityManager.GetComponentData<WorkProvider>(link.Workplace);
                oldProvider.m_MaxWorkers = 0;
                EntityManager.SetComponentData(link.Workplace, oldProvider);
            }
        }
        link.Workplace = Entity.Null;

        uint seed = unchecked((uint)link.Citizen.Index + (uint)venueId * 397u + 1u);
        Entity workplace = EntityManager.CreateEntity(
            ComponentType.ReadWrite<CompanyData>(),
            ComponentType.ReadWrite<PropertyRenter>(),
            ComponentType.ReadWrite<WorkProvider>(),
            ComponentType.ReadWrite<PrefabRef>(),
            ComponentType.ReadWrite<Employee>(),
            ComponentType.ReadWrite<UpdateFrame>());

        EntityManager.SetComponentData(workplace, new CompanyData
        {
            m_RandomSeed = Unity.Mathematics.Random.CreateFromIndex(seed),
            m_Brand = Entity.Null
        });
        EntityManager.SetComponentData(workplace, new PropertyRenter
        {
            m_Property = stadium,
            m_Rent = 0
        });
        EntityManager.SetComponentData(workplace, new PrefabRef(prefab));
        EntityManager.SetComponentData(workplace, new WorkProvider
        {
            m_MaxWorkers = 1
        });
        EntityManager.SetSharedComponent(workplace, new UpdateFrame((uint)(venueId & 15)));

        link.Workplace = workplace;
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
        if (link == null)
        {
            return;
        }

        link.EmploymentLogged = false;

        if (link.Citizen != Entity.Null && EntityManager.Exists(link.Citizen))
        {
            if (EntityManager.HasComponent<Worker>(link.Citizen))
            {
                Worker worker = EntityManager.GetComponentData<Worker>(link.Citizen);
                RemoveEmployeeEntry(worker.m_Workplace, link.Citizen);
                EntityManager.RemoveComponent<Worker>(link.Citizen);
            }

            if (EntityManager.HasComponent<TravelPurpose>(link.Citizen))
            {
                TravelPurpose purpose = EntityManager.GetComponentData<TravelPurpose>(link.Citizen);
                if (purpose.m_Purpose == Game.Citizens.Purpose.GoingToWork || purpose.m_Purpose == Game.Citizens.Purpose.Working)
                {
                    EntityManager.RemoveComponent<TravelPurpose>(link.Citizen);
                }
            }

            if (EntityManager.HasComponent<TripNeeded>(link.Citizen))
            {
                DynamicBuffer<TripNeeded> trips = EntityManager.GetBuffer<TripNeeded>(link.Citizen);
                for (int i = trips.Length - 1; i >= 0; i--)
                {
                    if (trips[i].m_Purpose == Game.Citizens.Purpose.GoingToWork)
                    {
                        trips.RemoveAtSwapBack(i);
                    }
                }
            }
        }

        if (link.Workplace != Entity.Null && EntityManager.Exists(link.Workplace))
        {
            RemoveEmployeeEntry(link.Workplace, link.Citizen);

            if (EntityManager.HasBuffer<Employee>(link.Workplace))
            {
                EntityManager.GetBuffer<Employee>(link.Workplace).Clear();
            }

            if (EntityManager.HasComponent<WorkProvider>(link.Workplace))
            {
                WorkProvider provider = EntityManager.GetComponentData<WorkProvider>(link.Workplace);
                provider.m_MaxWorkers = 0;
                EntityManager.SetComponentData(link.Workplace, provider);
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

    private void DeactivateLegacyWorkplaces()
    {
        foreach (Entity workplace in m_LegacyWorkplaces)
        {
            if (workplace == Entity.Null || !EntityManager.Exists(workplace))
            {
                continue;
            }

            if (EntityManager.HasBuffer<Employee>(workplace))
            {
                EntityManager.GetBuffer<Employee>(workplace).Clear();
            }

            if (EntityManager.HasComponent<WorkProvider>(workplace))
            {
                WorkProvider provider = EntityManager.GetComponentData<WorkProvider>(workplace);
                provider.m_MaxWorkers = 0;
                EntityManager.SetComponentData(workplace, provider);
            }
        }

        m_LegacyWorkplaces.Clear();
    }

    public void ForgetAllLinks()
    {
        ReleaseAllEmployment();
        DeactivateLegacyWorkplaces();
        m_PlayerResidents.Clear();
        m_CoachResidents.Clear();
    }

    public void SetDefaults(Context context)
    {
        m_PlayerResidents.Clear();
        m_CoachResidents.Clear();
        m_LegacyWorkplaces.Clear();
    }

    public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
    {
        writer.Write(kSaveVersion);

        WriteResidentLinks(writer, m_PlayerResidents);
        WriteResidentLinks(writer, m_CoachResidents);
    }

    private static void WriteResidentLinks<TWriter>(TWriter writer, Dictionary<int, ResidentLink> links)
        where TWriter : IWriter
    {
        writer.Write(links.Count);
        foreach (KeyValuePair<int, ResidentLink> pair in links)
        {
            writer.Write(pair.Key);
            writer.Write(pair.Value.Household);
            writer.Write(pair.Value.Citizen);
            writer.Write(pair.Value.Workplace);
            writer.Write(pair.Value.TeamId);
            writer.Write(pair.Value.VenueId);
            writer.Write(pair.Value.BirthYear);
            writer.Write(pair.Value.PendingSinceDay);
            writer.Write(pair.Value.LastSalaryDay);
            writer.Write(pair.Value.StartingWealthInitialized);
        }
    }

    public void Deserialize<TReader>(TReader reader) where TReader : IReader
    {
        m_PlayerResidents.Clear();
        m_CoachResidents.Clear();
        m_LegacyWorkplaces.Clear();

        reader.Read(out int version);
        if (version != 1 && version != 2 && version != 3 && version != kSaveVersion)
        {
            Mod.log.Warn("[Жители-спортсмены] Версия сохранённых связей не поддерживается. Жители города останутся нетронутыми; связи будут восстановлены для текущего состава.");
            return;
        }

        ReadResidentLinks(reader, m_PlayerResidents, version);
        ReadResidentLinks(reader, m_CoachResidents, version);

        if (version == 1)
        {
            reader.Read(out int legacyVenueCount);
            legacyVenueCount = Math.Max(0, Math.Min(legacyVenueCount, 10000));
            for (int i = 0; i < legacyVenueCount; i++)
            {
                reader.Read(out int venueId);
                reader.Read(out int workplaceCount);
                workplaceCount = Math.Max(0, Math.Min(workplaceCount, 8));
                for (int j = 0; j < workplaceCount; j++)
                {
                    reader.Read(out int complexity);
                    reader.Read(out Entity workplace);
                    m_LegacyWorkplaces.Add(workplace);
                }
            }
        }
    }

    private static void ReadResidentLinks<TReader>(TReader reader, Dictionary<int, ResidentLink> destination, int version)
        where TReader : IReader
    {
        reader.Read(out int count);
        count = Math.Max(0, Math.Min(count, 100000));

        for (int i = 0; i < count; i++)
        {
            reader.Read(out int id);
            reader.Read(out Entity household);
            reader.Read(out Entity citizen);

            Entity workplace = Entity.Null;
            if (version >= 2)
            {
                reader.Read(out workplace);
            }

            reader.Read(out int teamId);
            reader.Read(out int venueId);

            int birthYear = int.MinValue;
            if (version >= 3)
            {
                reader.Read(out birthYear);
            }

            int pendingSinceDay = int.MinValue;
            int lastSalaryDay = int.MinValue;
            bool startingWealthInitialized = false;
            if (version >= 4)
            {
                reader.Read(out pendingSinceDay);
                reader.Read(out lastSalaryDay);
                reader.Read(out startingWealthInitialized);
            }

            destination[id] = new ResidentLink
            {
                Household = household,
                Citizen = citizen,
                Workplace = workplace,
                TeamId = teamId,
                VenueId = venueId,
                BirthYear = birthYear,
                PendingSinceDay = pendingSinceDay,
                LastSalaryDay = lastSalaryDay,
                StartingWealthInitialized = startingWealthInitialized
            };
        }
    }
}
