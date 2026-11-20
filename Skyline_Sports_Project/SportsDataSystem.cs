using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Colossal.Serialization.Entities;
using Game;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Simulation;
using Game.Tools;
using Game.UI;
using Skyline_Sports_Project.Engine;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Skyline_Sports_Project;

[CompilerGenerated]
public class SportsDataSystem : GameSystemBase, IDefaultSerializable, ISerializable
{
	private sealed class FanApi
	{
		public Type Citizen;

		public Type HouseholdMember;

		public Type CurrentBuilding;

		public Type TravelPurpose;

		public Type TripNeeded;

		public Type PropertyRenter;

		public Type PurposeEnum;

		public FieldInfo TripTarget;

		public FieldInfo TripPurpose;

		public FieldInfo CurrentBuildingField;

		public FieldInfo HouseholdField;

		public FieldInfo PropertyField;

		public FieldInfo TravelPurposeField;

		public Type TouristHousehold;

		public Type Attractiveness;

		public Type CurrentTransport;

		public Type Target;

		public FieldInfo TransportField;

		public FieldInfo TargetField;

		public FieldInfo HotelField;

		public FieldInfo AttractField;

		public MethodInfo SetComponent;

		public object GoOut;

		public object GoHome;

		public string GoOutName = "";

		public MethodInfo HasComponent;

		public MethodInfo GetComponent;

		public MethodInfo HasBuffer;

		public MethodInfo GetBuffer;
	}

	private sealed class FanGame
	{
		public Entity Building;

		public string Name = "";

		public List<Entity> Fans = new List<Entity>();

		public HashSet<Entity> Visitors = new HashSet<Entity>();

		public int VisitorTarget;

		public HashSet<Entity> Sent = new HashSet<Entity>();

		public Dictionary<Entity, int> Tries = new Dictionary<Entity, int>();

		public int Target;

		public int LastLog = int.MinValue;

		public int LastCount = int.MinValue;

		public FanCount Count;

		public bool KickoffLogged;
	}

	internal struct FanCount
	{
		public int Inside;

		public int OnTheWay;

		public int Elsewhere;

		public int VisitorsInside;

		public int HeadingHere;

		public int HeadingElsewhere;

		public int InOtherBuilding;

		public int NoTarget;

		public string Doing;
	}

	private sealed class Boost
	{
		public int Original;

		public int Boosted;

		public int Since;

		public bool Added;

		public bool Checked;
	}

	private const int kSaveVersion = 2;

	private const int kChunkSize = 8000;

	private readonly Dictionary<int, Entity> m_VenueEntities = new Dictionary<int, Entity>();

	private TimeSystem m_TimeSystem;

	private CitySystem m_CitySystem;

	private readonly object m_TransferLock = new object();

	private readonly List<PendingTransfer> m_PendingTransfers = new List<PendingTransfer>();

	private sealed class PendingTransfer
	{
		public int TeamId;
		public int Amount;
		public bool ToClub;
	}

	private Type m_PlayerMoneyType;

	private MemberInfo m_PlayerMoneyBalance;

	private MemberInfo m_PlayerMoneyUnlimited;

	private MethodInfo m_PlayerMoneyAdd;

	private MethodInfo m_PlayerMoneyHas;

	private MethodInfo m_PlayerMoneyGet;

	private MethodInfo m_PlayerMoneySet;

	private NameSystem m_NameSystem;

	private EntityQuery m_DistrictQuery;

	private int m_DistrictScanMonth = int.MinValue;

	private bool m_ResetRequested;

	private object m_Climate;

	private bool m_ClimateGaveUp;

	private int m_ClimateTries;

	private string m_UnreadBlob;

	private Dictionary<int, Entity> m_UnreadEntities;

	private FanApi m_FanApi;

	private bool m_FansBroken;

	private EntityQuery m_CitizenQuery;

	private bool m_CitizenQueryMade;

	private readonly Dictionary<int, FanGame> m_FanTrips = new Dictionary<int, FanGame>();

	private readonly HashSet<int> m_FansDone = new HashSet<int>();

	private readonly System.Random m_FanRandom = new System.Random();

	private const int FansFirstWave = 150;

	private const int FansLastWave = 15;

	private readonly Dictionary<Entity, Boost> m_Boosts = new Dictionary<Entity, Boost>();

	private readonly Dictionary<Type, MethodInfo> m_SetCache = new Dictionary<Type, MethodInfo>();

	private bool m_BoostBroken;

	private const int TourismBoost = 100;

	private readonly Dictionary<Type, MethodInfo> m_HasCache = new Dictionary<Type, MethodInfo>();

	private readonly Dictionary<Type, MethodInfo> m_GetCache = new Dictionary<Type, MethodInfo>();

	public SportsWorld Sports { get; private set; }

	public string CityName { get; set; } = "";

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 512;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		Sports = new SportsWorld();
		m_TimeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TimeSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetExistingSystemManaged<CitySystem>();
		m_NameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NameSystem>();
		m_DistrictQuery = new EntityQueryBuilder(Allocator.Temp)
			.WithAll<District>()
			.WithNone<Deleted, Temp>()
			.Build(this);
		Mod.log.Info((object)"Система спортивных данных создана");
	}

	[Preserve]
	protected override void OnUpdate()
	{
		try
		{
			ApplySettings();
			if (m_ResetRequested)
			{
				m_ResetRequested = false;
				Sports.Clear();
				m_VenueEntities.Clear();
				Mod.log.Info((object)"Все спортивные данные сброшены через меню настроек.");
			}
			if (TryGetNow(out var now))
			{
				RemoveMissingVenues(now);
				if (m_UnreadBlob != null && Sports.Venues.Count > 0)
				{
					m_UnreadBlob = null;
					m_UnreadEntities = null;
					Sports.LoadProblem = "";
				}
				ScanDistricts(now);
				ReadCityWeather(now);
				Sports.VisitorAttendancePercent = ((Mod.Settings == null || Mod.Settings.VisitorFans) ? 20 : 0);
				UpdateCityBalance();
				ProcessPendingTransfers();
				Sports.Advance(now);
				SettleTicketIncome();
				RunFans(now);
			}
		}
		catch (Exception ex)
		{
			Mod.Error("Не удалось обновить спортивные данные", ex);
		}
	}


	private bool EnsurePlayerMoneyApi()
	{
		if (m_PlayerMoneyType != null && m_PlayerMoneyBalance != null && m_PlayerMoneyUnlimited != null && m_PlayerMoneyAdd != null && m_PlayerMoneyHas != null && m_PlayerMoneyGet != null && m_PlayerMoneySet != null)
		{
			return true;
		}
		m_PlayerMoneyType = FindGameType("Game.PlayerMoney") ?? FindGameType("Game.City.PlayerMoney");
		if (m_PlayerMoneyType == null)
		{
			Mod.log.Warn((object)"Казна города: тип PlayerMoney не найден.");
			return false;
		}
		BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		m_PlayerMoneyBalance = (MemberInfo)m_PlayerMoneyType.GetProperty("money", flags) ?? (MemberInfo)m_PlayerMoneyType.GetField("money", flags);
		m_PlayerMoneyUnlimited = (MemberInfo)m_PlayerMoneyType.GetField("m_Unlimited", flags) ?? (MemberInfo)m_PlayerMoneyType.GetProperty("m_Unlimited", flags);
		m_PlayerMoneyAdd = null;
		foreach (MethodInfo method in m_PlayerMoneyType.GetMethods(flags))
		{
			if (method.Name == "Add" && method.GetParameters().Length == 1 && (method.GetParameters()[0].ParameterType == typeof(int) || method.GetParameters()[0].ParameterType == typeof(long)))
			{
				m_PlayerMoneyAdd = method;
				break;
			}
		}
		m_PlayerMoneyHas = GenericEntityMethod("HasComponent", 1);
		m_PlayerMoneyGet = GenericEntityMethod("GetComponentData", 1);
		m_PlayerMoneySet = GenericEntityMethod("SetComponentData", 2);
		if (m_PlayerMoneyBalance == null || m_PlayerMoneyUnlimited == null || m_PlayerMoneyAdd == null || m_PlayerMoneyHas == null || m_PlayerMoneyGet == null || m_PlayerMoneySet == null)
		{
			Mod.log.Warn((object)("Казна города: API PlayerMoney неполное (" + m_PlayerMoneyType.FullName + ")."));
			return false;
		}
		Mod.log.Info((object)("Казна города: найден API " + m_PlayerMoneyType.FullName + "."));
		return true;
	}

	private static object ReadMember(MemberInfo member, object target)
	{
		if (member is FieldInfo)
		{
			return ((FieldInfo)member).GetValue(target);
		}
		return ((PropertyInfo)member).GetValue(target, null);
	}

	private long CityMoneyValue(object money)
	{
		return Convert.ToInt64(ReadMember(m_PlayerMoneyBalance, money));
	}

	private bool CityMoneyUnlimited(object money)
	{
		return Convert.ToBoolean(ReadMember(m_PlayerMoneyUnlimited, money));
	}

	private bool TryGetCityMoney(out Entity city, out object money)
	{
		city = Entity.Null;
		money = null;
		if (m_CitySystem == null)
		{
			m_CitySystem = ((ComponentSystemBase)this).World.GetExistingSystemManaged<CitySystem>();
		}
		if (m_CitySystem == null || !EnsurePlayerMoneyApi())
		{
			return false;
		}
		city = m_CitySystem.City;
		EntityManager em = ((ComponentSystemBase)this).EntityManager;
		if (city == Entity.Null || !em.Exists(city))
		{
			return false;
		}
		try
		{
			if (!(bool)m_PlayerMoneyHas.MakeGenericMethod(m_PlayerMoneyType).Invoke(em, new object[1] { city }))
			{
				return false;
			}
			money = m_PlayerMoneyGet.MakeGenericMethod(m_PlayerMoneyType).Invoke(em, new object[1] { city });
			return money != null;
		}
		catch (Exception ex)
		{
			Mod.log.Warn((object)("Казна города: не удалось прочитать PlayerMoney (" + ex.Message + ")."));
			return false;
		}
	}

	private void UpdateCityBalance()
	{
		if (TryGetCityMoney(out var city, out var money))
		{
			long balance = CityMoneyValue(money);
			bool unlimited = CityMoneyUnlimited(money);
			if (Sports.CityBalance != balance || Sports.CityUnlimited != unlimited)
			{
				Sports.CityBalance = balance;
				Sports.CityUnlimited = unlimited;
				Sports.Revision++;
			}
		}
	}

	private bool ChangeCityMoney(int amount)
	{
		if (!TryGetCityMoney(out var city, out var money))
		{
			return false;
		}
		Sports.CityUnlimited = CityMoneyUnlimited(money);
		if (Sports.CityUnlimited)
		{
			Sports.CityBalance = CityMoneyValue(money);
			return true;
		}
		long before = CityMoneyValue(money);
		if (amount < 0 && before < -(long)amount)
		{
			return false;
		}
		try
		{
			Type amountType = m_PlayerMoneyAdd.GetParameters()[0].ParameterType;
			object delta = Convert.ChangeType(amount, amountType);
			m_PlayerMoneyAdd.Invoke(money, new object[1] { delta });
			m_PlayerMoneySet.MakeGenericMethod(m_PlayerMoneyType).Invoke(((ComponentSystemBase)this).EntityManager, new object[2] { city, money });
			long after = CityMoneyValue(money);
			Sports.CityBalance = after;
			return after - before == amount;
		}
		catch (Exception ex)
		{
			Mod.log.Warn((object)("Казна города: не удалось изменить баланс (" + ex.Message + ")."));
			return false;
		}
	}

	public CommandResult TransferCityFunds(int teamId, int amount, bool toClub)
	{
		Team team = Sports.GetTeam(teamId);
		if (team == null || team.Retired || !team.IsUser)
		{
			return CommandResult.Fail("Переводы доступны только для вашей команды.");
		}
		Sports.EnsureClubBalance(team);
		if (amount <= 0)
		{
			return CommandResult.Fail("Укажите положительную сумму перевода.");
		}
		if (!toClub && team.ClubBalance < amount)
		{
			return CommandResult.Fail("На счёте клуба недостаточно средств.");
		}
		lock (m_TransferLock)
		{
			m_PendingTransfers.Add(new PendingTransfer { TeamId = teamId, Amount = amount, ToClub = toClub });
		}
		return CommandResult.Success("Перевод поставлен в очередь обработки.");
	}

	private void ProcessPendingTransfers()
	{
		List<PendingTransfer> pending;
		lock (m_TransferLock)
		{
			if (m_PendingTransfers.Count == 0) return;
			pending = new List<PendingTransfer>(m_PendingTransfers);
			m_PendingTransfers.Clear();
		}
		foreach (PendingTransfer transfer in pending)
		{
			CommandResult result = ApplyCityFundsTransfer(transfer.TeamId, transfer.Amount, transfer.ToClub);
			Sports.FinanceNotice = result.Message;
			Sports.Revision++;
			if (!result.Ok)
			{
				Mod.log.Warn((object)("Перевод между казной города и клубом отклонён: " + result.Message));
			}
		}
	}

	private CommandResult ApplyCityFundsTransfer(int teamId, int amount, bool toClub)
	{
		Team team = Sports.GetTeam(teamId);
		if (team == null || team.Retired || !team.IsUser)
		{
			return CommandResult.Fail("Команда больше не доступна для перевода.");
		}
		if (toClub)
		{
			if (!TryGetCityMoney(out var city, out var money))
			{
				return CommandResult.Fail("Не удалось получить доступ к казне города.");
			}
			if (!CityMoneyUnlimited(money) && CityMoneyValue(money) < amount)
			{
				return CommandResult.Fail("В городской казне недостаточно средств.");
			}
			if (!ChangeCityMoney(-amount))
			{
				return CommandResult.Fail("Не удалось списать средства из городской казны.");
			}
			CommandResult result = Sports.TransferClubFunds(teamId, amount, toClub: true);
			if (!result.Ok)
			{
				ChangeCityMoney(amount);
				return result;
			}
			return result;
		}
		if (TryGetCityMoney(out var cityAccount, out var cityMoney) && CityMoneyUnlimited(cityMoney))
		{
			return CommandResult.Fail("Городской бюджет безлимитный — отключите безлимитные деньги, чтобы переводить средства из клуба в казну.");
		}
		CommandResult withdrawal = Sports.TransferClubFunds(teamId, amount, toClub: false);
		if (!withdrawal.Ok)
		{
			return withdrawal;
		}
		if (!ChangeCityMoney(amount))
		{
			Sports.TransferClubFunds(teamId, amount, toClub: true);
			return CommandResult.Fail("Не удалось зачислить средства в городскую казну; перевод отменён.");
		}
		return withdrawal;
	}

	private void SettleTicketIncome()
	{
		long total = 0L;
		foreach (League league in Sports.Leagues)
		{
			Season[] seasons = new Season[2] { league.Current, league.Previous };
			foreach (Season season in seasons)
			{
				if (season == null) continue;
				foreach (Match game in season.Games)
				{
					if (!game.Played || game.TicketIncomePaid) continue;
					if (game.TicketRevenue <= 0)
					{
						game.TicketIncomePaid = true;
						continue;
					}
					if (TryGetCityMoney(out var city, out var balance) && (CityMoneyUnlimited(balance) || CityMoneyValue(balance) >= 2000000000L))
					{
						game.TicketIncomePaid = true;
						continue;
					}
					if (ChangeCityMoney(game.TicketRevenue))
					{
						game.TicketIncomePaid = true;
						total += game.TicketRevenue;
					}
				}
			}
		}
		if (total > 0)
		{
			Sports.Revision++;
			Mod.log.Info((object)("Билетные доходы: в городской бюджет зачислено " + total.ToString("N0") + "."));
		}
	}

	private void ReadCityWeather(int now)
	{
		if (m_ClimateGaveUp)
		{
			return;
		}
		try
		{
			if (m_Climate == null)
			{
				if (++m_ClimateTries > 20)
				{
					m_ClimateGaveUp = true;
					Mod.log.Info((object)"Погода города: система климата не найдена, для матчей используется обычный климат с четырьмя сезонами.");
					return;
				}
				Type type = FindType("Game.Simulation.ClimateSystem");
				if (type == null)
				{
					return;
				}
				string[] array = new string[2] { "GetExistingSystemManaged", "GetOrCreateSystemManaged" };
				foreach (string name in array)
				{
					MethodInfo get = typeof(World).GetMethod(name, new Type[1] { typeof(Type) });
					if (!(get == null))
					{
						m_Climate = get.Invoke(((ComponentSystemBase)this).World, new object[1] { type });
						if (m_Climate != null)
						{
							break;
						}
					}
				}
				if (m_Climate == null)
				{
					return;
				}
				Mod.log.Info((object)"Погода города: получены данные о климате города.");
			}
			float temp = ReadFloat(m_Climate, "temperature");
			if (float.IsNaN(temp))
			{
				m_ClimateGaveUp = true;
				Mod.log.Info((object)"Погода города: температура недоступна, для матчей используется обычный климат с четырьмя сезонами.");
				return;
			}
			float precip = ReadFloat(m_Climate, "precipitation");
			float cloud = ReadFloat(m_Climate, "cloudiness");
			bool raining = ReadBool(m_Climate, "isRaining");
			bool snowing = ReadBool(m_Climate, "isSnowing");
			Sports.RecordCityWeather(now, temp, float.IsNaN(precip) ? 0f : precip, float.IsNaN(cloud) ? 0f : cloud, raining, snowing);
		}
		catch (Exception ex)
		{
			m_ClimateGaveUp = true;
			Mod.log.Info((object)("Синхронизация погоды отключена: " + ex.Message));
		}
	}

	private static Type FindType(string fullName)
	{
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly a in assemblies)
		{
			Type t = null;
			try
			{
				t = a.GetType(fullName, throwOnError: false);
			}
			catch (Exception)
			{
			}
			if (t != null)
			{
				return t;
			}
		}
		return null;
	}

	private static object Member(object o, string name)
	{
		Type t = o.GetType();
		FieldInfo f = t.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (f != null)
		{
			return f.GetValue(o);
		}
		PropertyInfo p = t.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (p != null && p.GetIndexParameters().Length == 0)
		{
			return p.GetValue(o, null);
		}
		return null;
	}

	private static float ReadFloat(object o, string name)
	{
		object v = Member(o, name);
		if (v == null)
		{
			return float.NaN;
		}
		if (v is float)
		{
			return (float)v;
		}
		if (v is double)
		{
			return (float)(double)v;
		}
		MethodInfo[] methods = v.GetType().GetMethods(BindingFlags.Static | BindingFlags.Public);
		foreach (MethodInfo m in methods)
		{
			if (!(m.Name != "op_Implicit") && !(m.ReturnType != typeof(float)))
			{
				ParameterInfo[] ps = m.GetParameters();
				if (ps.Length == 1 && ps[0].ParameterType.IsAssignableFrom(v.GetType()))
				{
					return (float)m.Invoke(null, new object[1] { v });
				}
			}
		}
		object inner = Member(v, "value");
		if (inner is float)
		{
			return (float)inner;
		}
		return float.NaN;
	}

	private static bool ReadBool(object o, string name)
	{
		object v = Member(o, name);
		if (v is bool)
		{
			return (bool)v;
		}
		return false;
	}

	public void ApplySettings()
	{
		Setting s = Mod.Settings;
		if (s != null)
		{
			Sports.Settings.LeagueSize = s.LeagueSize;
			Sports.Settings.SoccerLeagueSize = s.SoccerLeagueSize;
			Sports.Settings.SeasonLengthPercent = s.SeasonGames;
			Sports.Settings.InjuryPercent = s.InjuryFrequency;
			if (!string.IsNullOrEmpty(CityName))
			{
				Sports.CityName = CityName;
			}
		}
	}

	public void RequestReset()
	{
		m_ResetRequested = true;
	}

	public bool TryGetNow(out int now)
	{
		now = Sports.LastTime;
		try
		{
			DateTime dt = m_TimeSystem.GetCurrentDateTime();
			if (dt.Year <= 1)
			{
				return false;
			}
			int month = dt.DayOfYear;
			if (month < 1 || month > 12)
			{
				month = dt.Month;
			}
			now = GameClock.FromCs2(dt.Year, month, dt.Hour * 60 + dt.Minute);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public int NowOrLast()
	{
		if (TryGetNow(out var now))
		{
			return now;
		}
		if (Sports.LastTime != int.MinValue)
		{
			return Sports.LastTime;
		}
		return GameClock.FromDate(2026, 1, 1, 9, 0);
	}

	public int VenueIdFor(Entity building)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<int, Entity> kv in m_VenueEntities)
		{
			if (kv.Value == building)
			{
				return kv.Key;
			}
		}
		return -1;
	}

	public bool IsBuilding(Entity entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (!(entity == Entity.Null))
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (entityManager.Exists(entity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!entityManager.HasComponent<Building>(entity))
				{
					return false;
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (entityManager.HasComponent<Deleted>(entity))
				{
					return false;
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (entityManager.HasComponent<Temp>(entity))
				{
					return false;
				}
				return true;
			}
		}
		return false;
	}

	public CommandResult MakeVenue(Entity building, VenueType type)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (!IsBuilding(building))
		{
			return CommandResult.Fail("Сначала выберите здание.");
		}
		if (VenueIdFor(building) >= 0)
		{
			return CommandResult.Fail("Это здание уже является спортивным объектом.");
		}
		Venue v = Sports.CreateVenue(type);
		m_VenueEntities[v.Id] = building;
		return CommandResult.Success("Теперь это здание — " + SportRules.VenueTypeLabel(type) + ". Выберите уровень, затем добавьте команды.");
	}

	public CommandResult RemoveVenue(int venueId)
	{
		CommandResult result = Sports.RemoveVenue(venueId, NowOrLast());
		m_VenueEntities.Remove(venueId);
		return result;
	}

	private void RemoveMissingVenues(int now)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		List<int> gone = null;
		foreach (KeyValuePair<int, Entity> kv in m_VenueEntities)
		{
			Entity e = kv.Value;
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (entityManager.Exists(e))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!entityManager.HasComponent<Deleted>(e))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (entityManager.HasComponent<Building>(e))
					{
						continue;
					}
				}
			}
			if (gone == null)
			{
				gone = new List<int>();
			}
			gone.Add(kv.Key);
		}
		foreach (int id in new List<int>(Sports.Venues.Keys))
		{
			if (!m_VenueEntities.ContainsKey(id))
			{
				if (gone == null)
				{
					gone = new List<int>();
				}
				gone.Add(id);
			}
		}
		if (gone == null)
		{
			return;
		}
		foreach (int id2 in gone)
		{
			Mod.log.Info((object)("Спортивный объект " + id2 + " удалён из города; связанные с ним команды также удаляются."));
			Sports.RemoveVenue(id2, now);
			m_VenueEntities.Remove(id2);
		}
	}

	private void ScanDistricts(int now)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		int month = GameClock.MonthIndex(now);
		if (month == m_DistrictScanMonth)
		{
			return;
		}
		m_DistrictScanMonth = month;
		try
		{
			List<string> names = new List<string>();
			NativeArray<Entity> districts = m_DistrictQuery.ToEntityArray(Allocator.Temp);
			try
			{
				for (int i = 0; i < districts.Length; i++)
				{
					string name = m_NameSystem.GetRenderedLabelName(districts[i]) ?? "";
					if (name.Trim().Length > 0)
					{
						names.Add(name.Trim());
					}
				}
			}
			finally
			{
				districts.Dispose();
			}
			Sports.SetDistricts(names);
		}
		catch (Exception ex)
		{
			Mod.log.Warn((object)("Не удалось получить названия районов: " + ex.Message));
		}
	}

	public NameContext NameContextFor(Entity building)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		NameContext ctx = new NameContext
		{
			CityName = (string.IsNullOrEmpty(CityName) ? "" : CityName)
		};
		try
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (entityManager.HasComponent<CurrentDistrict>(building))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				Entity district = entityManager.GetComponentData<CurrentDistrict>(building).m_District;
				if (district != Entity.Null)
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (entityManager.Exists(district))
					{
						string name = m_NameSystem.GetRenderedLabelName(district) ?? "";
						ctx.DistrictName = name.Trim();
						ctx.DistrictKey = ((ctx.DistrictName.Length > 0) ? ("dn:" + ctx.DistrictName.ToLowerInvariant()) : "");
					}
				}
			}
		}
		catch (Exception ex)
		{
			Mod.log.Warn((object)("Не удалось получить название района: " + ex.Message));
		}
		return ctx;
	}

	public string BuildingName(Entity building)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			return m_NameSystem.GetRenderedLabelName(building) ?? "";
		}
		catch (Exception)
		{
			return "";
		}
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		int num;
		string text;
		if (m_UnreadBlob != null)
		{
			num = ((Sports.Venues.Count == 0) ? 1 : 0);
			if (num != 0)
			{
				text = m_UnreadBlob;
				goto IL_0034;
			}
		}
		else
		{
			num = 0;
		}
		text = SaveCodec.Encode(Sports);
		goto IL_0034;
		IL_0034:
		string blob = text;
		Dictionary<int, Entity> venues = ((num != 0) ? m_UnreadEntities : m_VenueEntities);
		((IWriter)writer/*cast due to constrained. prefix*/).Write(2);
		int chunks = (blob.Length + 8000 - 1) / 8000;
		((IWriter)writer/*cast due to constrained. prefix*/).Write(chunks);
		TWriter val;
		for (int i = 0; i < chunks; i++)
		{
			int start = i * 8000;
			ref TWriter reference = ref writer;
			val = default;
			if (val == null)
			{
				val = reference;
				reference = ref val;
			}
			string text2 = blob.Substring(start, Math.Min(8000, blob.Length - start));
			((IWriter)reference/*cast due to constrained. prefix*/).Write(text2);
		}
		ref TWriter reference2 = ref writer;
		val = default;
		if (val == null)
		{
			val = reference2;
			reference2 = ref val;
		}
		int count = venues.Count;
		((IWriter)reference2/*cast due to constrained. prefix*/).Write(count);
		foreach (KeyValuePair<int, Entity> kv in venues)
		{
			ref TWriter reference3 = ref writer;
			val = default;
			if (val == null)
			{
				val = reference3;
				reference3 = ref val;
			}
			int key = kv.Key;
			((IWriter)reference3/*cast due to constrained. prefix*/).Write(key);
			ref TWriter reference4 = ref writer;
			val = default;
			if (val == null)
			{
				val = reference4;
				reference4 = ref val;
			}
			Entity value = kv.Value;
			((IWriter)reference4/*cast due to constrained. prefix*/).Write(value);
		}
		ref TWriter reference5 = ref writer;
		val = default;
		if (val == null)
		{
			val = reference5;
			reference5 = ref val;
		}
		int count2 = m_Boosts.Count;
		((IWriter)reference5/*cast due to constrained. prefix*/).Write(count2);
		foreach (KeyValuePair<Entity, Boost> kv2 in m_Boosts)
		{
			ref TWriter reference6 = ref writer;
			val = default;
			if (val == null)
			{
				val = reference6;
				reference6 = ref val;
			}
			Entity key2 = kv2.Key;
			((IWriter)reference6/*cast due to constrained. prefix*/).Write(key2);
			ref TWriter reference7 = ref writer;
			val = default;
			if (val == null)
			{
				val = reference7;
				reference7 = ref val;
			}
			int value2 = kv2.Value.Original;
			((IWriter)reference7/*cast due to constrained. prefix*/).Write(value2);
			ref TWriter reference8 = ref writer;
			val = default;
			if (val == null)
			{
				val = reference8;
				reference8 = ref val;
			}
			int value3 = kv2.Value.Boosted;
			((IWriter)reference8/*cast due to constrained. prefix*/).Write(value3);
		}
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		Sports.Clear();
		m_VenueEntities.Clear();
		m_Boosts.Clear();
		m_UnreadBlob = null;
		m_UnreadEntities = null;
		Sports.LoadProblem = "";
		int version = default;
		((IReader)reader/*cast due to constrained. prefix*/).Read(out version);
		if (version < 1 || version > 2)
		{
			Sports.LoadProblem = "Спортивные данные этого города сохранены более новой версией мода «Спорт России», поэтому их нельзя отобразить. Обновите мод, чтобы восстановить их.";
			Mod.log.Warn((object)("Спортивные данные сохранения созданы более новой версией мода (" + version + "); данные не загружены."));
			return;
		}
		int chunks = default;
		((IReader)reader/*cast due to constrained. prefix*/).Read(out chunks);
		StringBuilder sb = new StringBuilder();
		string chunk = default;
		for (int i = 0; i < chunks; i++)
		{
			((IReader)reader/*cast due to constrained. prefix*/).Read(out chunk);
			sb.Append(chunk);
		}
		int count = default;
		((IReader)reader/*cast due to constrained. prefix*/).Read(out count);
		Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
		int id = default;
		Entity e = default;
		for (int j = 0; j < count; j++)
		{
			((IReader)reader/*cast due to constrained. prefix*/).Read(out id);
			((IReader)reader/*cast due to constrained. prefix*/).Read(out e);
			entities[id] = e;
		}
		if (version >= 2)
		{
			int boosts = default;
			((IReader)reader/*cast due to constrained. prefix*/).Read(out boosts);
			Entity b = default;
			int original = default;
			int boosted = default;
			for (int k = 0; k < boosts; k++)
			{
				((IReader)reader/*cast due to constrained. prefix*/).Read(out b);
				((IReader)reader/*cast due to constrained. prefix*/).Read(out original);
				((IReader)reader/*cast due to constrained. prefix*/).Read(out boosted);
				if (b != Entity.Null)
				{
					Dictionary<Entity, Boost> dictionary = m_Boosts;
					Entity key = b;
					Boost boost = new Boost();
					boost.Original = original;
					boost.Boosted = boosted;
					boost.Since = 0;
					boost.Checked = true;
					dictionary[key] = boost;
				}
			}
		}
		string text = sb.ToString();
		if (!SaveCodec.Decode(text, Sports, out var error))
		{
			Sports.Clear();
			m_UnreadBlob = text;
			m_UnreadEntities = entities;
			string copy = BackUp(text);
			Sports.LoadProblem = "Не удалось прочитать спортивные данные этого города (" + error + "). Данные сохранены в файле города" + ((copy.Length > 0) ? (" и в копии " + copy) : "") + ". Создание нового спортивного объекта начнёт спортивную историю этого города заново.";
			Mod.log.Warn((object)("Не удалось прочитать спортивные данные: " + error + ((copy.Length > 0) ? (" Копия сохранена в " + copy) : "")));
			return;
		}
		foreach (KeyValuePair<int, Entity> kv in entities)
		{
			if (Sports.GetVenue(kv.Key) != null)
			{
				m_VenueEntities[kv.Key] = kv.Value;
			}
		}
		Mod.log.Info((object)("Загружены спортивные данные: " + Sports.Venues.Count + " спортивных объектов, " + Sports.UserTeams().Count + " команд."));
	}

	public void SetDefaults(Context context)
	{
		Sports.Clear();
		m_VenueEntities.Clear();
		m_Boosts.Clear();
		m_UnreadBlob = null;
		m_UnreadEntities = null;
		Sports.LoadProblem = "";
	}

	private static string BackUp(string text)
	{
		try
		{
			string text2 = Path.Combine(Application.persistentDataPath, "ModsData", "SkylineSports");
			Directory.CreateDirectory(text2);
			string text3 = Path.Combine(text2, "unread-sports-data-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".txt");
			File.WriteAllText(text3, text);
			return text3;
		}
		catch (Exception ex)
		{
			Mod.log.Warn((object)("Не удалось создать резервную копию спортивных данных: " + ex.Message));
			return "";
		}
	}

	private void RunFans(int now)
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		Setting s = Mod.Settings;
		if (s == null || !s.RealFans)
		{
			if (m_FanTrips.Count > 0 && m_FanApi != null)
			{
				foreach (int id in new List<int>(m_FanTrips.Keys))
				{
					EndFans(id);
				}
			}
			if (m_Boosts.Count > 0 && (m_FanApi != null || (!m_FansBroken && LoadFanApi())))
			{
				foreach (Entity b in new List<Entity>(m_Boosts.Keys))
				{
					EndBoost(b);
				}
			}
			if (Sports.FanNotes.Count > 0)
			{
				Sports.FanNotes.Clear();
			}
		}
		else
		{
			if (m_FansBroken)
			{
				return;
			}
			try
			{
				if (m_FanApi == null && !LoadFanApi())
				{
					return;
				}
				HashSet<int> seen = new HashSet<int>();
				foreach (Match g in Sports.SpotlightGames(now))
				{
					seen.Add(g.Id);
					if (!m_VenueEntities.TryGetValue(g.VenueId, out var building) || building == Entity.Null)
					{
						continue;
					}
					EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!entityManager.Exists(building))
					{
						continue;
					}
					Sports.CityClockWindow(g, now, out var start, out var end);
					if (!m_FanTrips.TryGetValue(g.Id, out var fg))
					{
						if (m_FansDone.Contains(g.Id) || g.Played || now < start - 150 || now >= end)
						{
							continue;
						}
						FanGame fanGame = new FanGame();
						fanGame.Building = building;
						fanGame.Target = Math.Max(1, s.RealFansCount);
						fanGame.Name = Sports.TeamName(g.AwayTeamId) + " — " + Sports.TeamName(g.HomeTeamId);
						fg = fanGame;
						fg.VisitorTarget = ((s.VisitorFans && m_FanApi.TouristHousehold != null) ? Math.Max(1, fg.Target / 4) : 0);
						m_FanTrips[g.Id] = fg;
						Mod.log.Info((object)("Болельщики: " + fg.Name + ", " + fg.Target + " болельщиков отправляются из дома (цель поездки " + m_FanApi.GoOutName + ")."));
					}
					if (now < start)
					{
						double share = Math.Max(0.0, Math.Min(1.0, (double)(now - (start - 150)) / 135.0));
						int due = (int)Math.Ceiling((double)fg.Target * share);
						int locals = fg.Fans.Count - fg.Visitors.Count;
						if (locals < due)
						{
							SendFans(fg, due - locals, visitors: false);
						}
						int dueVisitors = (int)Math.Ceiling((double)fg.VisitorTarget * share);
						if (fg.Visitors.Count < dueVisitors)
						{
							SendFans(fg, dueVisitors - fg.Visitors.Count, visitors: true);
						}
						RetryFans(fg);
					}
					if (fg.LastCount == int.MinValue || now - fg.LastCount >= 10 || (now >= start && !fg.KickoffLogged) || g.Played || now >= end)
					{
						fg.Count = CountFans(fg);
						fg.LastCount = now;
					}
					FanCount c = fg.Count;
					string note = "Болельщики из вашего города: " + c.Inside + " на объекте" + ((c.HeadingHere > 0) ? (", " + c.HeadingHere + " в пути") : "") + ((c.VisitorsInside > 0) ? (" (" + c.VisitorsInside + " на объекте находятся гости из других городов)") : ((fg.Visitors.Count > 0) ? (" (" + fg.Visitors.Count + " гостей из других городов также направляются на матч)") : "")) + "." + ((BoostNote(building).Length > 0) ? (" " + BoostNote(building)) : "");
					if (!Sports.FanNotes.TryGetValue(g.Id, out var old) || old != note)
					{
						Sports.FanNotes[g.Id] = note;
						Sports.Revision++;
					}
					bool kickoff = now >= start && !fg.KickoffLogged;
					if (kickoff)
					{
						fg.KickoffLogged = true;
					}
					if (kickoff || fg.LastLog == int.MinValue || now - fg.LastLog >= 60)
					{
						fg.LastLog = now;
						Mod.log.Info((object)("Болельщики: " + fg.Name + (kickoff ? " (начало матча)" : "") + ": " + fg.Fans.Count + " отправлено, " + c.OnTheWay + " в пути (" + c.HeadingHere + " направляются к объекту, " + c.HeadingElsewhere + " направляются в другое место, " + c.InOtherBuilding + " находятся в другом здании, " + c.NoTarget + " место назначения не найдено), " + c.Inside + " на объекте, " + c.Elsewhere + " в других местах" + ((c.Doing.Length > 0) ? (" (" + c.Doing + ")") : "") + "."));
					}
					if (g.Played || now >= end)
					{
						Mod.log.Info((object)("Болельщики: " + fg.Name + " финальный свисток, " + c.Inside + " из " + fg.Fans.Count + " болельщиков были на объекте; они возвращаются домой."));
						EndFans(g.Id);
					}
				}
				foreach (int id2 in new List<int>(m_FanTrips.Keys))
				{
					if (!seen.Contains(id2))
					{
						EndFans(id2);
					}
				}
				RunBoosts(now, s);
				if (m_FansDone.Count > 200)
				{
					m_FansDone.Clear();
				}
			}
			catch (Exception ex)
			{
				StopFans("произошла ошибка (" + ex.GetType().Name + ": " + ex.Message + ")");
			}
		}
	}

	private FanCount CountFans(FanGame fg)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		FanCount c = new FanCount
		{
			Doing = ""
		};
		Dictionary<string, int> doing = new Dictionary<string, int>();
		foreach (Entity f in fg.Fans)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!entityManager.Exists(f))
			{
				Bump(doing, "gone");
				c.Elsewhere++;
				continue;
			}
			if (InBuilding(f, fg.Building))
			{
				c.Inside++;
				if (fg.Visitors.Contains(f))
				{
					c.VisitorsInside++;
				}
				continue;
			}
			string purpose = PurposeOf(f);
			if (purpose == m_FanApi.GoOutName && !AtHome(f))
			{
				c.OnTheWay++;
				if (CurrentBuildingOf(f) != Entity.Null)
				{
					c.InOtherBuilding++;
					continue;
				}
				Entity target = TravelTarget(f);
				if (target == Entity.Null)
				{
					c.NoTarget++;
				}
				else if (target == fg.Building)
				{
					c.HeadingHere++;
				}
				else
				{
					c.HeadingElsewhere++;
				}
			}
			else
			{
				c.Elsewhere++;
				Dictionary<string, int> d = doing;
				string key;
				if (purpose.Length != 0)
				{
					key = purpose;
				}
				else
				{
					key = (AtHome(f) ? "home" : "out");
				}
				Bump(d, key);
			}
		}
		List<string> parts = new List<string>();
		foreach (KeyValuePair<string, int> kv in doing)
		{
			parts.Add(kv.Key + " " + kv.Value);
		}
		c.Doing = string.Join(", ", parts.ToArray());
		return c;
	}

	private static void Bump(Dictionary<string, int> d, string key)
	{
		d.TryGetValue(key, out var n);
		d[key] = n + 1;
	}

	private string PurposeOf(Entity citizen)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (m_FanApi.TravelPurpose == null || m_FanApi.TravelPurposeField == null || !Has(m_FanApi.TravelPurpose, citizen))
		{
			return "";
		}
		object p = m_FanApi.TravelPurposeField.GetValue(Get(m_FanApi.TravelPurpose, citizen));
		if (p != null)
		{
			return p.ToString();
		}
		return "";
	}

	private Entity CurrentBuildingOf(Entity citizen)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!Has(m_FanApi.CurrentBuilding, citizen))
		{
			return Entity.Null;
		}
		return (Entity)m_FanApi.CurrentBuildingField.GetValue(Get(m_FanApi.CurrentBuilding, citizen));
	}

	private Entity TravelTarget(Entity citizen)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		if (m_FanApi.CurrentTransport == null || m_FanApi.TransportField == null || m_FanApi.Target == null || m_FanApi.TargetField == null)
		{
			return Entity.Null;
		}
		if (!Has(m_FanApi.CurrentTransport, citizen))
		{
			return Entity.Null;
		}
		Entity mover = (Entity)m_FanApi.TransportField.GetValue(Get(m_FanApi.CurrentTransport, citizen));
		if (!(mover == Entity.Null))
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (entityManager.Exists(mover) && Has(m_FanApi.Target, mover))
			{
				return (Entity)m_FanApi.TargetField.GetValue(Get(m_FanApi.Target, mover));
			}
		}
		return Entity.Null;
	}

	private bool AtHome(Entity citizen)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Entity home = HomeOf(citizen);
		if (home != Entity.Null)
		{
			return InBuilding(citizen, home);
		}
		return false;
	}

	private void RetryFans(FanGame fg)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		foreach (Entity f in fg.Fans)
		{
			fg.Tries.TryGetValue(f, out var tries);
			if (tries < 2)
			{
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				if (entityManager.Exists(f) && !InBuilding(f, fg.Building) && !(PurposeOf(f) == m_FanApi.GoOutName) && AtHome(f) && AddTrip(f, fg.Building, m_FanApi.GoOut))
				{
					fg.Tries[f] = tries + 1;
				}
			}
		}
	}

	private void RunBoosts(int now, Setting s)
	{
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		HashSet<Entity> want = new HashSet<Entity>();
		if (s.GameDayTourism && !m_BoostBroken && m_FanApi.Attractiveness != null && m_FanApi.AttractField != null && m_FanApi.SetComponent != null)
		{
			foreach (Match g in Sports.SpotlightGames(now))
			{
				if (g.Played || !m_VenueEntities.TryGetValue(g.VenueId, out var b) || b == Entity.Null)
				{
					continue;
				}
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				if (entityManager.Exists(b))
				{
					Sports.CityClockWindow(g, now, out var start, out var end);
					if (now >= start - 360 && now < end)
					{
						want.Add(b);
					}
				}
			}
		}
		foreach (Entity b2 in new List<Entity>(m_Boosts.Keys))
		{
			if (!want.Contains(b2))
			{
				EndBoost(b2);
			}
		}
		foreach (Entity b3 in want)
		{
			if (!m_Boosts.TryGetValue(b3, out var bo))
			{
				StartBoost(b3, now);
			}
			else if (!bo.Checked && now - bo.Since >= 5)
			{
				bo.Checked = true;
				int value = (Has(m_FanApi.Attractiveness, b3) ? ((int)m_FanApi.AttractField.GetValue(Get(m_FanApi.Attractiveness, b3))) : int.MinValue);
				if (value == bo.Boosted)
				{
					Mod.log.Info((object)("Болельщики: бонус привлекательности для туристов сохранён (привлекательность объекта " + value + ")."));
					continue;
				}
				m_BoostBroken = true;
				m_Boosts.Remove(b3);
				Mod.log.Info((object)("Болельщики: игра самостоятельно изменила привлекательность объекта (" + value + ", после повышения до " + bo.Boosted + "); бонус туризма не может работать таким образом и отключён."));
			}
		}
	}

	private void Set(Type component, Entity e, object value)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (!m_SetCache.TryGetValue(component, out var m))
		{
			m = m_FanApi.SetComponent.MakeGenericMethod(component);
			m_SetCache[component] = m;
		}
		m.Invoke(((ComponentSystemBase)this).EntityManager, new object[2] { e, value });
	}

	private static ComponentType ReadWrite(Type t)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		MethodInfo rw = typeof(ComponentType).GetMethod("ReadWrite", new Type[1] { typeof(Type) });
		if (!(rw == null))
		{
			return (ComponentType)rw.Invoke(null, new object[1] { t });
		}
		return default;
	}

	private void StartBoost(Entity b, int now)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Boost boost = new Boost();
			boost.Since = now;
			Boost bo = boost;
			if (!Has(m_FanApi.Attractiveness, b))
			{
				m_BoostBroken = true;
				Mod.log.Info((object)"Болельщики: объект не считается достопримечательностью в игре, поэтому бонус туризма для него отключён.");
				return;
			}
			object v = Get(m_FanApi.Attractiveness, b);
			bo.Original = (int)m_FanApi.AttractField.GetValue(v);
			bo.Boosted = bo.Original + 100;
			m_FanApi.AttractField.SetValue(v, bo.Boosted);
			Set(m_FanApi.Attractiveness, b, v);
			m_Boosts[b] = bo;
			Mod.log.Info((object)("Болельщики: в день матча объект привлекает туристов (привлекательность " + bo.Original + " до " + bo.Boosted + (bo.Added ? ", объект временно стал достопримечательностью" : "") + ")."));
		}
		catch (Exception ex)
		{
			m_BoostBroken = true;
			Mod.log.Info((object)("Болельщики: бонус туризма отключён (" + ex.GetType().Name + ": " + ex.Message + ")."));
		}
	}

	private void EndBoost(Entity b)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if (!m_Boosts.TryGetValue(b, out var bo))
		{
			return;
		}
		m_Boosts.Remove(b);
		try
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!entityManager.Exists(b) || !Has(m_FanApi.Attractiveness, b))
			{
				return;
			}
			if (bo.Added)
			{
				MethodInfo remove = typeof(EntityManager).GetMethod("RemoveComponent", new Type[2]
				{
					typeof(Entity),
					typeof(ComponentType)
				});
				if (remove != null)
				{
					remove.Invoke(((ComponentSystemBase)this).EntityManager, new object[2]
					{
						b,
						ReadWrite(m_FanApi.Attractiveness)
					});
				}
			}
			else
			{
				object v = Get(m_FanApi.Attractiveness, b);
				if ((int)m_FanApi.AttractField.GetValue(v) == bo.Boosted)
				{
					m_FanApi.AttractField.SetValue(v, bo.Original);
					Set(m_FanApi.Attractiveness, b, v);
				}
			}
			Mod.log.Info((object)"Болельщики: матч завершён, объект вернулся к обычному состоянию.");
		}
		catch (Exception)
		{
		}
	}

	private string BoostNote(Entity building)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		Setting s = Mod.Settings;
		if (s == null || !s.GameDayTourism)
		{
			return "";
		}
		if (m_BoostBroken)
		{
			return "Туризм в день матча отключён: игра самостоятельно управляет привлекательностью объекта.";
		}
		if (!m_Boosts.ContainsKey(building))
		{
			return "";
		}
		return "В день матча объект привлекает туристов.";
	}

	private void StopFans(string why)
	{
		m_FansBroken = true;
		Sports.FanStatus = "Система болельщиков временно отключена: " + why + ". Подробности — в журнале мода «Спорт России».";
		Mod.log.Info((object)("Система болельщиков отключена: " + why));
		Sports.Revision++;
	}

	private bool LoadFanApi()
	{
		//IL_06db: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		//IL_071f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0724: Unknown result type (might be due to invalid IL or missing references)
		//IL_0729: Unknown result type (might be due to invalid IL or missing references)
		//IL_072e: Unknown result type (might be due to invalid IL or missing references)
		FanApi a = new FanApi();
		a.Citizen = FindGameType("Game.Citizens.Citizen");
		a.HouseholdMember = FindGameType("Game.Citizens.HouseholdMember");
		a.CurrentBuilding = FindGameType("Game.Citizens.CurrentBuilding");
		a.TravelPurpose = FindGameType("Game.Citizens.TravelPurpose");
		a.TripNeeded = FindGameType("Game.Citizens.TripNeeded");
		a.PropertyRenter = FindGameType("Game.Buildings.PropertyRenter");
		a.PurposeEnum = FindGameType("Game.Citizens.Purpose");
		List<string> report = new List<string>();
		KeyValuePair<string, Type>[] array = new KeyValuePair<string, Type>[7]
		{
			new KeyValuePair<string, Type>("Citizen", a.Citizen),
			new KeyValuePair<string, Type>("HouseholdMember", a.HouseholdMember),
			new KeyValuePair<string, Type>("CurrentBuilding", a.CurrentBuilding),
			new KeyValuePair<string, Type>("TripNeeded", a.TripNeeded),
			new KeyValuePair<string, Type>("TravelPurpose", a.TravelPurpose),
			new KeyValuePair<string, Type>("PropertyRenter", a.PropertyRenter),
			new KeyValuePair<string, Type>("Purpose", a.PurposeEnum)
		};
		for (int i = 0; i < array.Length; i++)
		{
			KeyValuePair<string, Type> kv = array[i];
			report.Add(kv.Key + ((kv.Value == null) ? " ОТСУТСТВУЕТ" : (" = " + kv.Value.FullName + Fields(kv.Value))));
		}
		Mod.log.Info((object)("Система болельщиков, компоненты жителей игры: " + string.Join("; ", report.ToArray())));
		if (a.Citizen == null || a.HouseholdMember == null || a.CurrentBuilding == null || a.TripNeeded == null || a.PropertyRenter == null || a.PurposeEnum == null)
		{
			StopFans("в этой версии игры отсутствуют необходимые компоненты");
			return false;
		}
		a.TripTarget = FieldOf(a.TripNeeded, "m_TargetAgent", typeof(Entity));
		a.TripPurpose = FieldOf(a.TripNeeded, "m_Purpose", a.PurposeEnum);
		a.CurrentBuildingField = FieldOf(a.CurrentBuilding, "m_CurrentBuilding", typeof(Entity));
		a.HouseholdField = FieldOf(a.HouseholdMember, "m_Household", typeof(Entity));
		a.PropertyField = FieldOf(a.PropertyRenter, "m_Property", typeof(Entity));
		if (a.TravelPurpose != null)
		{
			a.TravelPurposeField = FieldOf(a.TravelPurpose, "m_Purpose", a.PurposeEnum);
		}
		if (a.TripTarget == null || a.TripPurpose == null || a.CurrentBuildingField == null || a.HouseholdField == null || a.PropertyField == null)
		{
			StopFans("компонент жителя устроен иначе, чем ожидалось");
			return false;
		}
		string[] names = Enum.GetNames(a.PurposeEnum);
		Mod.log.Info((object)("Система болельщиков, цели поездок: " + string.Join(", ", names)));
		string[] array2 = new string[3] { "Leisure", "VisitAttractions", "Shopping" };
		foreach (string n in array2)
		{
			if (Array.IndexOf(names, n) >= 0)
			{
				a.GoOut = Enum.Parse(a.PurposeEnum, n);
				a.GoOutName = n;
				break;
			}
		}
		if (Array.IndexOf(names, "GoingHome") >= 0)
		{
			a.GoHome = Enum.Parse(a.PurposeEnum, "GoingHome");
		}
		if (a.GoOut == null || a.GoHome == null)
		{
			StopFans("не найдены цели поездки для отправки болельщиков на матч и возвращения домой");
			return false;
		}
		a.HasComponent = GenericEntityMethod("HasComponent", 1);
		a.GetComponent = GenericEntityMethod("GetComponentData", 1);
		a.HasBuffer = GenericEntityMethod("HasBuffer", 1);
		a.GetBuffer = GenericEntityMethod("GetBuffer", 2);
		a.TouristHousehold = FindGameType("Game.Citizens.TouristHousehold");
		if (a.TouristHousehold != null)
		{
			a.HotelField = FieldOf(a.TouristHousehold, "m_Hotel", typeof(Entity));
		}
		a.Attractiveness = FindGameType("Game.Buildings.AttractivenessProvider");
		if (a.Attractiveness != null)
		{
			a.AttractField = FieldOf(a.Attractiveness, "m_Attractiveness", typeof(int));
		}
		a.SetComponent = GenericEntityMethod("SetComponentData", 2);
		a.CurrentTransport = FindGameType("Game.Citizens.CurrentTransport");
		if (a.CurrentTransport != null)
		{
			a.TransportField = FieldOf(a.CurrentTransport, "m_CurrentTransport", typeof(Entity));
		}
		a.Target = FindGameType("Game.Common.Target");
		if (a.Target != null)
		{
			a.TargetField = FieldOf(a.Target, "m_Target", typeof(Entity));
		}
		Mod.log.Info((object)("Система болельщиков, транспорт: текущий транспорт " + ((a.CurrentTransport == null) ? "ОТСУТСТВУЕТ" : (a.CurrentTransport.FullName + Fields(a.CurrentTransport))) + "; цель " + ((a.Target == null) ? "ОТСУТСТВУЕТ" : (a.Target.FullName + Fields(a.Target)))));
		Mod.log.Info((object)("Система болельщиков, посетители: домохозяйства туристов " + ((a.TouristHousehold == null) ? "ОТСУТСТВУЕТ" : (a.TouristHousehold.FullName + Fields(a.TouristHousehold))) + "; компонент привлекательности " + ((a.Attractiveness == null) ? "ОТСУТСТВУЕТ" : (a.Attractiveness.FullName + Fields(a.Attractiveness)))));
		if (a.HasComponent == null || a.GetComponent == null || a.HasBuffer == null || a.GetBuffer == null)
		{
			StopFans("менеджер сущностей работает иначе, чем ожидалось");
			return false;
		}
		MethodInfo readOnly = typeof(ComponentType).GetMethod("ReadOnly", new Type[1] { typeof(Type) });
		if (readOnly == null)
		{
			StopFans("не удалось получить доступ к жителям");
			return false;
		}
		ComponentType[] citizenTypes = new ComponentType[]
		{
			(ComponentType)readOnly.Invoke(null, new object[1] { a.Citizen }),
			(ComponentType)readOnly.Invoke(null, new object[1] { a.HouseholdMember }),
			(ComponentType)readOnly.Invoke(null, new object[1] { a.CurrentBuilding })
		};
		m_CitizenQuery = GetEntityQuery(new EntityQueryDesc
		{
			All = citizenTypes
		});
		m_CitizenQueryMade = true;
		m_FanApi = a;
		Sports.FanStatus = "";
		Mod.log.Info((object)("Система болельщиков готова (поездка на матч: " + a.GoOutName + " до арены, затем возвращение домой)."));
		return true;
	}

	private static Type FindGameType(string fullName)
	{
		Type t = FindType(fullName);
		if (t != null)
		{
			return t;
		}
		string shortName = fullName.Substring(fullName.LastIndexOf('.') + 1);
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly asm in assemblies)
		{
			Type[] types;
			try
			{
				types = asm.GetTypes();
			}
			catch (ReflectionTypeLoadException ex)
			{
				types = ex.Types;
			}
			catch (Exception)
			{
				continue;
			}
			Type[] array = types;
			foreach (Type x in array)
			{
				if (x != null && x.Name == shortName && x.Namespace != null && x.Namespace.StartsWith("Game."))
				{
					return x;
				}
			}
		}
		return null;
	}

	private static string Fields(Type t)
	{
		if (t.IsEnum)
		{
			return "";
		}
		List<string> f = new List<string>();
		FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (FieldInfo fi in fields)
		{
			f.Add(fi.Name + ":" + fi.FieldType.Name);
		}
		return " {" + string.Join(", ", f.ToArray()) + "}";
	}

	private static FieldInfo FieldOf(Type t, string name, Type fieldType)
	{
		FieldInfo f = t.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (f != null && f.FieldType == fieldType)
		{
			return f;
		}
		FieldInfo only = null;
		FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (FieldInfo x in fields)
		{
			if (!(x.FieldType != fieldType))
			{
				if (only != null)
				{
					return null;
				}
				only = x;
			}
		}
		return only;
	}

	private static MethodInfo GenericEntityMethod(string name, int parameters)
	{
		MethodInfo[] methods = typeof(EntityManager).GetMethods(BindingFlags.Instance | BindingFlags.Public);
		foreach (MethodInfo m in methods)
		{
			if (!(m.Name != name) && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 1)
			{
				ParameterInfo[] ps = m.GetParameters();
				if (ps.Length == parameters && ps[0].ParameterType == typeof(Entity))
				{
					return m;
				}
			}
		}
		return null;
	}

	private static MethodInfo Closed(Dictionary<Type, MethodInfo> cache, MethodInfo open, Type t)
	{
		if (!cache.TryGetValue(t, out var m))
		{
			m = (cache[t] = open.MakeGenericMethod(t));
		}
		return m;
	}

	private bool Has(Type component, Entity e)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		return (bool)Closed(m_HasCache, m_FanApi.HasComponent, component).Invoke(((ComponentSystemBase)this).EntityManager, new object[1] { e });
	}

	private object Get(Type component, Entity e)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		return Closed(m_GetCache, m_FanApi.GetComponent, component).Invoke(((ComponentSystemBase)this).EntityManager, new object[1] { e });
	}

	private bool InBuilding(Entity citizen, Entity building)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!entityManager.Exists(citizen) || !Has(m_FanApi.CurrentBuilding, citizen))
		{
			return false;
		}
		return (Entity)m_FanApi.CurrentBuildingField.GetValue(Get(m_FanApi.CurrentBuilding, citizen)) == building;
	}

	private Entity HouseholdOf(Entity citizen)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (!Has(m_FanApi.HouseholdMember, citizen))
		{
			return Entity.Null;
		}
		Entity household = (Entity)m_FanApi.HouseholdField.GetValue(Get(m_FanApi.HouseholdMember, citizen));
		if (household != Entity.Null)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (entityManager.Exists(household))
			{
				return household;
			}
		}
		return Entity.Null;
	}

	private bool IsVisitor(Entity citizen)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (m_FanApi.TouristHousehold == null)
		{
			return false;
		}
		Entity household = HouseholdOf(citizen);
		if (household != Entity.Null)
		{
			return Has(m_FanApi.TouristHousehold, household);
		}
		return false;
	}

	private Entity HomeOf(Entity citizen)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		Entity household = HouseholdOf(citizen);
		if (household == Entity.Null)
		{
			return Entity.Null;
		}
		if (Has(m_FanApi.PropertyRenter, household))
		{
			return (Entity)m_FanApi.PropertyField.GetValue(Get(m_FanApi.PropertyRenter, household));
		}
		if (m_FanApi.TouristHousehold != null && m_FanApi.HotelField != null && Has(m_FanApi.TouristHousehold, household))
		{
			Entity hotel = (Entity)m_FanApi.HotelField.GetValue(Get(m_FanApi.TouristHousehold, household));
			if (hotel != Entity.Null)
			{
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				if (entityManager.Exists(hotel))
				{
					return hotel;
				}
			}
			return Entity.Null;
		}
		return Entity.Null;
	}

	private bool AddTrip(Entity citizen, Entity target, object purpose)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		if (!(bool)m_FanApi.HasBuffer.MakeGenericMethod(m_FanApi.TripNeeded).Invoke(((ComponentSystemBase)this).EntityManager, new object[1] { citizen }))
		{
			return false;
		}
		object buffer = m_FanApi.GetBuffer.MakeGenericMethod(m_FanApi.TripNeeded).Invoke(((ComponentSystemBase)this).EntityManager, new object[2] { citizen, false });
		object trip = Activator.CreateInstance(m_FanApi.TripNeeded);
		m_FanApi.TripTarget.SetValue(trip, target);
		m_FanApi.TripPurpose.SetValue(trip, purpose);
		MethodInfo add = buffer.GetType().GetMethod("Add", new Type[1] { m_FanApi.TripNeeded });
		if (add == null)
		{
			return false;
		}
		add.Invoke(buffer, new object[1] { trip });
		return true;
	}

	private void SendFans(FanGame fg, int count, bool visitors)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		if (!m_CitizenQueryMade || count <= 0)
		{
			return;
		}
		NativeArray<Entity> citizens = m_CitizenQuery.ToEntityArray(Allocator.Temp);
		try
		{
			int n = citizens.Length;
			if (n == 0)
			{
				return;
			}
			int tries = Math.Min(n, count * 25);
			int sent = 0;
			for (int k = 0; k < tries; k++)
			{
				if (sent >= count)
				{
					break;
				}
				Entity c = citizens[m_FanRandom.Next(n)];
				if (!fg.Sent.Contains(c) && (!(m_FanApi.TravelPurpose != null) || !Has(m_FanApi.TravelPurpose, c)) && IsVisitor(c) == visitors && (visitors || AtHome(c)) && (!visitors || !InBuilding(c, fg.Building)) && AddTrip(c, fg.Building, m_FanApi.GoOut))
				{
					fg.Sent.Add(c);
					fg.Fans.Add(c);
					if (visitors)
					{
						fg.Visitors.Add(c);
					}
					sent++;
				}
			}
		}
		finally
		{
			citizens.Dispose();
		}
	}

	private void EndFans(int gameId)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (!m_FanTrips.TryGetValue(gameId, out var fg))
		{
			return;
		}
		m_FanTrips.Remove(gameId);
		m_FansDone.Add(gameId);
		if (m_FanApi == null)
		{
			return;
		}
		int home = 0;
		foreach (Entity c in fg.Fans)
		{
			try
			{
				if (InBuilding(c, fg.Building))
				{
					Entity h = HomeOf(c);
					if (h != Entity.Null && AddTrip(c, h, m_FanApi.GoHome))
					{
						home++;
					}
				}
			}
			catch (Exception)
			{
			}
		}
		Mod.log.Info((object)("Болельщики: " + home + " болельщиков возвращаются домой после матча " + fg.Name + "."));
	}

	[Preserve]
	public SportsDataSystem()
	{
	}
}
