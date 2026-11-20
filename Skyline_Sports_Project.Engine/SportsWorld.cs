using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Skyline_Sports_Project.Engine;

public class SportsWorld
{
	private sealed class TourSpec
	{
		public string Name = "";

		public bool Under20;

		public int Every;

		public int Offset;

		public int Month;

		public int Day;

		public string[] Nations;

		public int[] Depth;
	}

	private class Account
	{
		public string Name;

		public string Handle;
	}

	private class FinderAsset
	{
		public Player Player;

		public PickAsset Pick;

		public double Theirs;

		public double Mine;
	}

	private class TradeItem
	{
		public Player Player;

		public PickAsset Pick;

		public double SellerValue;

		public double BuyerValue;
	}

	public const int AllStarSeasonsKept = 4;

	public readonly Dictionary<int, Coach> Coaches = new Dictionary<int, Coach>();

	private Dictionary<int, List<Coach>> _staff;

	private int _staffRevision = int.MinValue;

	private int _staffStamp;

	private int _staffBuiltStamp = -1;

	private int _staffCount = -1;

	private static readonly List<Coach> _noStaff = new List<Coach>();

	private readonly Dictionary<string, double> _staffMeans = new Dictionary<string, double>();

	private int _staffMeansStamp = int.MinValue;

	public readonly Dictionary<int, string> FanNotes = new Dictionary<int, string>();

	public string FanStatus = "";

	public string LoadProblem = "";

	public int NextId = 1;

	public int LastTime = int.MinValue;

	public readonly Dictionary<int, Organization> Orgs = new Dictionary<int, Organization>();

	public readonly Dictionary<int, Team> Teams = new Dictionary<int, Team>();

	public readonly Dictionary<int, Venue> Venues = new Dictionary<int, Venue>();

	public readonly List<League> Leagues = new List<League>();

	public WorldSettings Settings = new WorldSettings();

	public int Revision;

	public string CityName = "";

	public long CityBalance;

	public bool CityUnlimited;

	public string FinanceNotice = "";

	public int VisitorAttendancePercent = 20;

	private readonly Random _random;

	private readonly NameGenerator _names;

	private readonly GameSimulator _simulator;

	public const int SecondDivisionRating = 59;

	public readonly Dictionary<int, Player> Retired = new Dictionary<int, Player>();

	public const int CareerPageYears = 10;

	public const int CandidateYears = 4;

	private const int MaxCandidatesPerSport = 14;

	private int _exPlayersYear = int.MinValue;

	public const int OtherBoxDays = 7;

	private int _boxPruneDay = int.MinValue;

	private const int HallMaxEntries = 300;

	private const int MaxRetiredPerTeam = 20;

	public readonly List<string> Districts = new List<string>();

	private int _localMonth = int.MinValue;

	private const int MaxDistricts = 80;

	private static readonly string[] MinorInjuries = new string[10] { "Верхняя часть тела", "Нижняя часть тела", "Ушиб колена", "Боль в спине", "Мышца-сгибатель бедра", "Растяжение паховой мышцы", "Ушиб рёбер", "Ушиб пальца", "Болезнь", "Затёкшая шея" };

	private static readonly string[] MediumInjuries = new string[9] { "Растяжение голеностопа", "Растяжение задней мышцы бедра", "Растяжение связок колена", "Растяжение связок плеча", "Сотрясение мозга", "Перелом пальца", "Растяжение икроножной мышцы", "Растяжение связок запястья", "Растяжение косой мышцы" };

	private static readonly string[] MajorInjuries = new string[7] { "Перелом запястья", "Перелом стопы", "Растяжение верхних связок голеностопа", "Вывих плеча", "Разрыв медиальной связки колена", "Перелом кисти", "Перелом челюсти" };

	private static readonly string[] SeasonInjuries = new string[6] { "Разрыв передней крестообразной связки", "Разрыв ахиллова сухожилия", "Операция на плече", "Перелом ноги", "Разрыв суставной губы", "Операция на спине" };

	public Action<League, Team, Player, int> PlayerInjured;

	public readonly List<Tournament> Tournaments = new List<Tournament>();

	private static readonly Dictionary<Sport, TourSpec> Specs = BuildSpecs();

	private static readonly HashSet<string> Provinces = new HashSet<string>
	{
		"ON", "QC", "BC", "AB", "MB", "SK", "NS", "NB", "NL", "PE",
		"YT", "NT", "NU"
	};

	public string CountryName = "";

	public string CountryAbbrev = "";

	private static readonly Dictionary<string, string> Codes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		{ "USA", "USA" },
		{ "Canada", "CAN" },
		{ "Mexico", "MEX" },
		{ "Sweden", "SWE" },
		{ "Finland", "FIN" },
		{ "Russia", "RUS" },
		{ "Czechia", "CZE" },
		{ "Slovakia", "SVK" },
		{ "Switzerland", "SUI" },
		{ "Germany", "GER" },
		{ "Latvia", "LAT" },
		{ "Norway", "NOR" },
		{ "Denmark", "DEN" },
		{ "Austria", "AUT" },
		{ "France", "FRA" },
		{ "Kazakhstan", "KAZ" },
		{ "Slovenia", "SLO" },
		{ "Italy", "ITA" },
		{ "Hungary", "HUN" },
		{ "Poland", "POL" },
		{ "Великобритания", "GBR" },
		{ "England", "ENG" },
		{ "Scotland", "SCO" },
		{ "Wales", "WAL" },
		{ "Ireland", "IRL" },
		{ "Brazil", "BRA" },
		{ "Argentina", "ARG" },
		{ "Spain", "ESP" },
		{ "Portugal", "POR" },
		{ "Netherlands", "NED" },
		{ "Belgium", "BEL" },
		{ "Uruguay", "URU" },
		{ "Colombia", "COL" },
		{ "Ecuador", "ECU" },
		{ "Chile", "CHI" },
		{ "Peru", "PER" },
		{ "Venezuela", "VEN" },
		{ "Nigeria", "NGA" },
		{ "Ghana", "GHA" },
		{ "Senegal", "SEN" },
		{ "Morocco", "MAR" },
		{ "Cameroon", "CMR" },
		{ "Egypt", "EGY" },
		{ "South Africa", "RSA" },
		{ "Japan", "JPN" },
		{ "South Korea", "KOR" },
		{ "China", "CHN" },
		{ "Chinese Taipei", "TPE" },
		{ "Australia", "AUS" },
		{ "New Zealand", "NZL" },
		{ "Philippines", "PHI" },
		{ "India", "IND" },
		{ "Iran", "IRN" },
		{ "Saudi Arabia", "KSA" },
		{ "Israel", "ISR" },
		{ "Turkey", "TUR" },
		{ "Greece", "GRE" },
		{ "Serbia", "SRB" },
		{ "Croatia", "CRO" },
		{ "Montenegro", "MNE" },
		{ "Lithuania", "LTU" },
		{ "Estonia", "EST" },
		{ "Ukraine", "UKR" },
		{ "Belarus", "BLR" },
		{ "Romania", "ROU" },
		{ "Bulgaria", "BUL" },
		{ "Iceland", "ISL" },
		{ "Puerto Rico", "PUR" },
		{ "Dominican Republic", "DOM" },
		{ "Cuba", "CUB" },
		{ "Panama", "PAN" },
		{ "Nicaragua", "NCA" },
		{ "Jamaica", "JAM" }
	};

	private static readonly Dictionary<string, string> CountryAliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		{ "Россия", "Russia" },
		{ "United States of America", "USA" },
		{ "US", "USA" },
		{ "America", "USA" },
		{ "UK", "Великобритания" },
		{ "Великобритания", "Великобритания" },
		{ "Britain", "Великобритания" },
		{ "Korea", "South Korea" },
		{ "Holland", "Netherlands" },
		{ "Czech Republic", "Czechia" },
		{ "Taiwan", "Chinese Taipei" },
		{ "Deutschland", "Germany" },
		{ "США", "USA" },
		{ "Канада", "Canada" },
		{ "Мексика", "Mexico" },
		{ "Швеция", "Sweden" },
		{ "Финляндия", "Finland" },
		{ "Чехия", "Czechia" },
		{ "Словакия", "Slovakia" },
		{ "Швейцария", "Switzerland" },
		{ "Германия", "Germany" },
		{ "Латвия", "Latvia" },
		{ "Норвегия", "Norway" },
		{ "Дания", "Denmark" },
		{ "Австрия", "Austria" },
		{ "Франция", "France" },
		{ "Казахстан", "Kazakhstan" },
		{ "Словения", "Slovenia" },
		{ "Италия", "Italy" },
		{ "Венгрия", "Hungary" },
		{ "Польша", "Poland" },
		{ "Англия", "England" },
		{ "Шотландия", "Scotland" },
		{ "Уэльс", "Wales" },
		{ "Ирландия", "Ireland" },
		{ "Бразилия", "Brazil" },
		{ "Аргентина", "Argentina" },
		{ "Испания", "Spain" },
		{ "Португалия", "Portugal" },
		{ "Нидерланды", "Netherlands" },
		{ "Бельгия", "Belgium" },
		{ "Уругвай", "Uruguay" },
		{ "Колумбия", "Colombia" },
		{ "Эквадор", "Ecuador" },
		{ "Чили", "Chile" },
		{ "Перу", "Peru" },
		{ "Венесуэла", "Venezuela" },
		{ "Нигерия", "Nigeria" },
		{ "Гана", "Ghana" },
		{ "Сенегал", "Senegal" },
		{ "Марокко", "Morocco" },
		{ "Камерун", "Cameroon" },
		{ "Египет", "Egypt" },
		{ "ЮАР", "South Africa" },
		{ "Япония", "Japan" },
		{ "Южная Корея", "South Korea" },
		{ "Китай", "China" },
		{ "Тайвань", "Chinese Taipei" },
		{ "Австралия", "Australia" },
		{ "Новая Зеландия", "New Zealand" },
		{ "Филиппины", "Philippines" },
		{ "Индия", "India" },
		{ "Иран", "Iran" },
		{ "Саудовская Аравия", "Saudi Arabia" },
		{ "Израиль", "Israel" },
		{ "Турция", "Turkey" },
		{ "Греция", "Greece" },
		{ "Сербия", "Serbia" },
		{ "Хорватия", "Croatia" },
		{ "Черногория", "Montenegro" },
		{ "Литва", "Lithuania" },
		{ "Эстония", "Estonia" },
		{ "Украина", "Ukraine" },
		{ "Беларусь", "Belarus" },
		{ "Румыния", "Romania" },
		{ "Болгария", "Bulgaria" },
		{ "Исландия", "Iceland" },
		{ "Пуэрто-Рико", "Puerto Rico" },
		{ "Доминиканская Республика", "Dominican Republic" },
		{ "Куба", "Cuba" },
		{ "Панама", "Panama" },
		{ "Никарагуа", "Nicaragua" },
		{ "Ямайка", "Jamaica" }
	};

	private Dictionary<int, int[]> _groupRows;

	public const int SendDownMaxAge = 25;

	private int _linkedRevision = int.MinValue;

	public const int FarmAuto = 0;

	public const int FarmCity = 1;

	public const int FarmOutOfTown = 2;

	public readonly List<NewsPost> Feed = new List<NewsPost>();

	public const int MaxPosts = 400;

	private static readonly Account Wire = new Account
	{
		Name = "Спорт России",
		Handle = "@SportRossii"
	};

	private static readonly Account Insider = new Account
	{
		Name = "Алексей Новиков",
		Handle = "@AlexeyNovikov"
	};

	private static readonly Account DraftRoom = new Account
	{
		Name = "Комната драфта",
		Handle = "@DraftRoomRU"
	};

	private static readonly Account Prep = new Account
	{
		Name = "Школьный спорт",
		Handle = "@SchoolSportRU"
	};

	private static readonly Account Campus = new Account
	{
		Name = "Студенческий инсайдер",
		Handle = "@CampusInsiderRU"
	};

	public static Action<string, SportsWorld, League> Probe;

	public readonly Dictionary<int, Player> Players = new Dictionary<int, Player>();

	private readonly Dictionary<int, List<Player>> _rosters = new Dictionary<int, List<Player>>();

	private readonly Dictionary<int, List<Player>> _prospects = new Dictionary<int, List<Player>>();

	private readonly Dictionary<int, List<Player>> _pools = new Dictionary<int, List<Player>>();

	private bool _rostersDirty = true;

	private Dictionary<string, int> _nameUse;

	private int _clockHint = int.MinValue;

	public bool LoadedWithoutCoaches;

	public bool LoadedWithoutBios;

	public static double CinderellaK = 2.0;

	public const int RecordsShown = 10;

	private const int CountBoardKeep = 30;

	public const int HistorySeasons = 10;

	private const int MaxNotes = 8;

	public const string GroupSeason = "Single season";

	public const string GroupCareer = "Career";

	public const string GroupAwards = "Awards";

	public const string GroupTeams = "Teams";

	public List<RecruitTarget> RecruitBoard = new List<RecruitTarget>();

	public List<RecruitEffort> RecruitEfforts = new List<RecruitEffort>();

	public const int CallHours = 1;

	public const int WatchHours = 3;

	public const int HomeVisitHours = 4;

	public const int OfficialVisitHours = 5;

	public const double FootballBonusShare = 0.3;

	public const int SoccerBuyoutsPerYear = 2;

	public const int RivalHeat = 50;

	private static readonly int[] FallbackMinutes = new int[4] { 780, 930, 1140, 1200 };

	private int _appliedLeagueSize = int.MinValue;

	private int _appliedSoccerLeagueSize = int.MinValue;

	public int LiveTick;

	public const int LiveBuffer = 15;

	public Action<League, Season> SeasonCompleted;

	public Action<League, Match, double, double> GamePlayed;

	public readonly List<TransferOffer> TransferOffers = new List<TransferOffer>();

	private int _transferMonth = int.MinValue;

	private static readonly string[] AbroadClubs = new string[24]
	{
		"Реал Вальдера (Испания)", "Атлетико Серрано (Испания)", "Олимпик Марлон (Франция)", "АС Бомон (Франция)", "Витория Азул (Португалия)", "Спортинг Алвор (Португалия)", "ФК Рейн-Тальбах (Германия)", "Боруссия Эльмсхорн (Германия)", "АК Монтальдо (Италия)", "Кальчо Равена (Италия)",
		"Аяксбург (Нидерланды)", "ПСВ Линдховен (Нидерланды)", "Кингсбери Альбион (Англия)", "Харроугейт Юнайтед (Англия)", "Селтик Роверс (Шотландия)", "Расинг Андино (Аргентина)", "Клуб Атлетико Плата (Аргентина)", "Фламенго ду Сул (Бразилия)", "Гремио Паулино (Бразилия)", "Клуб Реал Сьерра (Мексика)",
		"Депортиво Тихерас (Мексика)", "Викинг Фьорд (Норвегия)", "Брондаль ИФ (Дания)", "Галатас СК (Турция)"
	};

	public List<ContractTalk> Talks = new List<ContractTalk>();

	public List<TradeOffer> Offers = new List<TradeOffer>();

	public readonly List<CityWeatherMonth> CityWeather = new List<CityWeatherMonth>();

	public int CitySampleTime = int.MinValue;

	public double CityTempC;

	public double CityPrecip;

	public double CityCloud;

	public bool CityRaining;

	public bool CitySnowing;

	private int _lastCountedSample = int.MinValue;

	private static readonly double[] NormalTempC = new double[12]
	{
		-3.0, -1.0, 4.0, 10.0, 16.0, 21.0, 24.0, 23.0, 19.0, 12.0,
		6.0, 0.0
	};

	private static readonly double[] NormalWet = new double[12]
	{
		0.16, 0.16, 0.19, 0.21, 0.21, 0.18, 0.16, 0.16, 0.15, 0.15,
		0.17, 0.18
	};

	public static int SecondDivisionOverall => (int)Math.Round((double)Rosters.Base(Level.Minor) + 3.6);

	public string HomeCountry
	{
		get
		{
			if (CountryName.Length <= 0)
			{
				return "Russia";
			}
			return CountryName;
		}
	}

	public string HomeAbbrev
	{
		get
		{
			if (CountryAbbrev.Length <= 0)
			{
				return Abbrev(HomeCountry);
			}
			return CountryAbbrev;
		}
	}

	public bool CustomCountry
	{
		get
		{
			if (CountryName.Length > 0)
			{
				return !Codes.ContainsKey(CountryName);
			}
			return false;
		}
	}

	private void GiveAwards(League league, Season s, int now)
	{
		List<Player> players = PlayersInLeague(league);
		if (players.Count == 0)
		{
			return;
		}
		AwardContext c = new AwardContext
		{
			Year = CurrentYear()
		};
		foreach (StandingRow r in Standings.Compute(league, s))
		{
			c.TeamGames = Math.Max(c.TeamGames, r.Played);
			c.WinPct[r.TeamId] = ((r.Played == 0) ? 0.5 : r.WinPct);
			c.PointsPerGame[r.TeamId] = ((r.Played == 0) ? 0.0 : ((double)r.PointsFor / (double)r.Played));
		}
		if (c.TeamGames == 0)
		{
			return;
		}
		string label = SeasonLabel(league, s);
		List<AwardRecord> records = new List<AwardRecord>();
		foreach (AwardDef def in AwardBook.For(league.Sport, league.Level))
		{
			Player best = null;
			double bestScore = 0.0;
			foreach (Player p in players)
			{
				if (def.Playoff && p.TeamId != s.ChampionTeamId)
				{
					continue;
				}
				int[] stats = Stats.Fit(def.Playoff ? p.Playoff : p.Season, league.Sport);
				if (!Stats.IsEmpty(stats) && def.Eligible(c, p, stats))
				{
					double score = def.Score(c, p, stats);
					if (score > bestScore || (score == bestScore && best != null && p.Id < best.Id))
					{
						bestScore = score;
						best = p;
					}
				}
			}
			if (best != null)
			{
				int[] line = Stats.Fit(def.Playoff ? best.Playoff : best.Season, league.Sport);
				Team team = GetTeam(best.TeamId);
				AwardRecord rec = new AwardRecord
				{
					SeasonLabel = label,
					Award = def.Award,
					Trophy = def.Trophy,
					PlayerId = best.Id,
					PlayerName = best.Name,
					TeamId = best.TeamId,
					TeamName = TeamName(best.TeamId),
					Detail = def.Detail(c, best, line),
					IsUser = (team?.IsUser ?? false)
				};
				records.Add(rec);
				AddHonor(best, league.Level, label, def.Award, def.Trophy);
				string who = Rosters.NewsPositionLabel(best.Position, best.Sport) + " " + best.Name + " из клуба " + rec.TeamName;
				if (def.Headline)
				{
					Post(league, NewsKind.Award, Wire, "НАГРАДЫ: " + who + " получает награду «" + def.Trophy + " в категории " + def.Award + " (" + rec.Detail + ").", rec.IsUser, now, 2);
				}
				else if (rec.IsUser && team != null)
				{
					PostAsTeam(league, team, NewsKind.Award, best.Name + " получает награду «" + def.Trophy + " (" + def.Award + ")! " + rec.Detail + ".", now);
				}
			}
		}
		league.Awards.InsertRange(0, records);
		while (league.Awards.Count > 160)
		{
			league.Awards.RemoveAt(league.Awards.Count - 1);
		}
		Revision++;
	}

	public static bool HasAllStars(League league)
	{
		if (league.Level == Level.Pro)
		{
			if (league.Sport != Sport.Hockey && league.Sport != Sport.Basketball)
			{
				return league.Sport == Sport.Football;
			}
			return true;
		}
		return false;
	}

	public static string AllStarGameName(Sport sport)
	{
		if (sport != Sport.Football)
		{
			return "Матч всех звёзд";
		}
		return "Матч звёзд";
	}

	public static string AllLeagueName(Sport sport, int kind)
	{
		string n = kind switch
		{
			2 => "Вторая", 
			1 => "Первая", 
			_ => "Третья", 
		};
		return sport switch
		{
			Sport.Football => n + " символическая сборная профессиональной лиги", 
			Sport.Hockey => n + " сборная всех звёзд", 
			_ => n + " символическая сборная лиги", 
		};
	}

	public static bool IsSelection(string award)
	{
		if (!(award == "All-Star") && !(award == "Матч звёзд") && !(award == "Матч всех звёзд") && !award.Contains("Сборная всех звёзд"))
		{
			return award.Contains("Сборная лиги");
		}
		return true;
	}

	private AwardContext SelectionContext(League league, Season s)
	{
		AwardContext c = new AwardContext
		{
			Year = CurrentYear()
		};
		foreach (StandingRow r in Standings.Compute(league, s))
		{
			c.TeamGames = Math.Max(c.TeamGames, r.Played);
			c.WinPct[r.TeamId] = ((r.Played == 0) ? 0.5 : r.WinPct);
			c.PointsPerGame[r.TeamId] = ((r.Played == 0) ? 0.0 : ((double)r.PointsFor / (double)r.Played));
		}
		return c;
	}

	private void AllStarsMidseason(League league, int now)
	{
		if (!HasAllStars(league))
		{
			return;
		}
		Season s = league.Current;
		if (s == null || s.PlayoffsCreated || league.AllStarYear == s.Year)
		{
			return;
		}
		int total = 0;
		int played = 0;
		foreach (Match g in s.Games)
		{
			if (g.Stage == GameStage.Regular)
			{
				total++;
				if (g.Played)
				{
					played++;
				}
			}
		}
		double when;
		if (league.Sport == Sport.Football)
		{
			when = 0.8;
		}
		else
		{
			when = ((league.Sport == Sport.Basketball) ? 0.65 : 0.6);
		}
		if (total != 0 && !((double)played < (double)total * when))
		{
			league.AllStarYear = s.Year;
			PickAllStars(league, s, now);
		}
	}

	private void PickAllStars(League league, Season s, int now)
	{
		AwardContext c = SelectionContext(league, s);
		if (c.TeamGames == 0)
		{
			return;
		}
		string label = SeasonLabel(league, s);
		Sport sport = league.Sport;
		List<Player> pool = PlayersInLeague(league).FindAll((Player player) => player.Status == PlayerStatus.Active && !Stats.IsEmpty(player.Season));
		int teams = Math.Max(2, league.TeamIds.Count);
		Func<Player, double> vote = (Player player) =>
		{
			int[] s2 = Stats.Fit(player.Season, sport);
			return (!AwardBook.SelectionQualifies(sport, c, player, s2, allLeague: false)) ? double.MinValue : (AwardBook.SelectionValue(sport, c, player, s2) * (0.85 + 0.3 * c.TeamWinPct(player)) + (double)(Rosters.Ovr(player) - 80) * 0.15 + _random.NextDouble() * 1.5);
		};
		List<AllStarPick> picks = new List<AllStarPick>();
		Action<Player, bool> add = (Player player, bool starter) =>
		{
			picks.Add(new AllStarPick
			{
				Year = s.Year,
				Season = label,
				Kind = 0,
				PlayerId = player.Id,
				Name = player.Name,
				TeamId = player.TeamId,
				Team = TeamName(player.TeamId),
				Pos = Rosters.NewsPositionLabel(player.Position, player.Sport),
				Starter = starter,
				Detail = AwardBook.SelectionLine(sport, player, Stats.Fit(player.Season, sport))
			});
		};
		Func<Func<Player, bool>, int, List<Player>> best = (Func<Player, bool> who, int num4) =>
		{
			List<KeyValuePair<Player, double>> list = new List<KeyValuePair<Player, double>>();
			foreach (Player p5 in pool)
			{
				if (who(p5) && !picks.Exists((AllStarPick x) => x.PlayerId == p5.Id))
				{
					double num2 = vote(p5);
					if (num2 > double.MinValue)
					{
						list.Add(new KeyValuePair<Player, double>(p5, num2));
					}
				}
			}
			list.Sort((KeyValuePair<Player, double> a, KeyValuePair<Player, double> b) => (b.Value == a.Value) ? a.Key.Id.CompareTo(b.Key.Id) : b.Value.CompareTo(a.Value));
			List<Player> list2 = new List<Player>();
			for (int num3 = 0; num3 < list.Count && num3 < num4; num3++)
			{
				list2.Add(list[num3].Key);
			}
			return list2;
		};
		if (sport == Sport.Basketball)
		{
			int count = Math.Max(10, Math.Min(24, (int)Math.Round((double)teams * 0.8)));
			int starters = Math.Max(4, (int)Math.Round((double)count * 10.0 / 24.0));
			List<Player> chosen = best((Player player) => true, count);
			for (int i = 0; i < chosen.Count; i++)
			{
				add(chosen[i], i < starters);
			}
		}
		else if (sport == Sport.Hockey)
		{
			int count2 = Math.Max(11, Math.Min(44, (int)Math.Round((double)teams * 1.375)));
			int defense = (int)Math.Round((double)count2 * 3.0 / 11.0);
			int goalies = Math.Max(2, (int)Math.Round((double)count2 * 2.0 / 11.0));
			int forwards = count2 - defense - goalies;
			foreach (Player p in best((Player player) => player.Position == Position.Center || player.Position == Position.LeftWing || player.Position == Position.RightWing, forwards))
			{
				add(p, arg2: false);
			}
			foreach (Player p2 in best((Player player) => player.Position == Position.Defense, defense))
			{
				add(p2, arg2: false);
			}
			foreach (Player p3 in best((Player player) => player.Position == Position.Goalie, goalies))
			{
				add(p3, arg2: false);
			}
			foreach (int id in league.TeamIds)
			{
				if (!picks.Exists((AllStarPick x) => x.TeamId == id))
				{
					List<Player> one = best((Player player) => player.TeamId == id, 1);
					if (one.Count > 0)
					{
						add(one[0], arg2: false);
					}
				}
			}
		}
		else
		{
			double f = (double)teams / 16.0;
			KeyValuePair<Position, int>[] array = new KeyValuePair<Position, int>[11]
			{
				new KeyValuePair<Position, int>(Position.Quarterback, 3),
				new KeyValuePair<Position, int>(Position.RunningBack, 3),
				new KeyValuePair<Position, int>(Position.WideReceiver, 4),
				new KeyValuePair<Position, int>(Position.TightEnd, 2),
				new KeyValuePair<Position, int>(Position.OffensiveLine, 9),
				new KeyValuePair<Position, int>(Position.DefensiveLine, 8),
				new KeyValuePair<Position, int>(Position.Linebacker, 5),
				new KeyValuePair<Position, int>(Position.Cornerback, 4),
				new KeyValuePair<Position, int>(Position.Safety, 3),
				new KeyValuePair<Position, int>(Position.Kicker, 1),
				new KeyValuePair<Position, int>(Position.Punter, 1)
			};
			for (int num = 0; num < array.Length; num++)
			{
				KeyValuePair<Position, int> slot = array[num];
				int count3 = Math.Max(1, (int)Math.Round((double)slot.Value * f));
				List<Player> chosen2 = best((Player player) => player.Position == slot.Key, count3);
				for (int i2 = 0; i2 < chosen2.Count; i2++)
				{
					add(chosen2[i2], i2 == 0);
				}
			}
		}
		if (picks.Count == 0)
		{
			return;
		}
		foreach (AllStarPick k in picks)
		{
			Player p4 = GetPlayer(k.PlayerId);
			if (p4 != null)
			{
				AddHonor(p4, Level.Pro, label, (sport == Sport.Football) ? "Пробоул" : "Матч всех звёзд", AllStarGameName(sport));
			}
		}
		StoreSelections(league, s.Year, 0, picks);
		List<string> top = new List<string>();
		foreach (AllStarPick k2 in picks)
		{
			if ((k2.Starter || sport == Sport.Hockey) && top.Count < 4)
			{
				top.Add(k2.Pos + " " + k2.Name);
			}
		}
		Post(league, NewsKind.Award, Wire, "ВСЕ ЗВЁЗДЫ: " + label + " " + AllStarGameName(sport) + " " + ((sport == Sport.Football) ? "состав сформирован" : "составы сформированы") + " объявлен, " + picks.Count + " игроков. Лидирует: " + string.Join(", ", top.ToArray()) + ".", isUser: false, now, 2);
		PostOwnSelections(league, picks, (sport == Sport.Football) ? "попал на матч звёзд" : "стал участником матча всех звёзд", now);
	}

	private void PickAllLeague(League league, Season s, int now)
	{
		if (!HasAllStars(league) || league.AllLeagueYear == s.Year)
		{
			return;
		}
		league.AllLeagueYear = s.Year;
		AwardContext c = SelectionContext(league, s);
		if (c.TeamGames == 0)
		{
			return;
		}
		string label = SeasonLabel(league, s);
		Sport sport = league.Sport;
		List<Player> pool = PlayersInLeague(league).FindAll((Player player) => !Stats.IsEmpty(player.Season));
		HashSet<int> taken = new HashSet<int>();
		List<AllStarPick> picks = new List<AllStarPick>();
		Func<Func<Player, bool>, List<Player>> ranked = (Func<Player, bool> who) =>
		{
			List<KeyValuePair<Player, double>> list2 = new List<KeyValuePair<Player, double>>();
			foreach (Player current in pool)
			{
				if (!taken.Contains(current.Id) && who(current))
				{
					int[] s2 = Stats.Fit(current.Season, sport);
					if (AwardBook.SelectionQualifies(sport, c, current, s2, allLeague: true))
					{
						list2.Add(new KeyValuePair<Player, double>(current, AwardBook.SelectionValue(sport, c, current, s2) * ((sport == Sport.Basketball) ? (0.8 + 0.4 * c.TeamWinPct(current)) : 1.0)));
					}
				}
			}
			list2.Sort((KeyValuePair<Player, double> a, KeyValuePair<Player, double> b) => (b.Value == a.Value) ? a.Key.Id.CompareTo(b.Key.Id) : b.Value.CompareTo(a.Value));
			List<Player> list3 = new List<Player>();
			foreach (KeyValuePair<Player, double> item in list2)
			{
				list3.Add(item.Key);
			}
			return list3;
		};
		int teamsCount = ((sport == Sport.Basketball) ? 3 : 2);
		List<KeyValuePair<Func<Player, bool>, int>> slots = new List<KeyValuePair<Func<Player, bool>, int>>();
		if (sport == Sport.Basketball)
		{
			slots.Add(new KeyValuePair<Func<Player, bool>, int>((Player player) => true, 5));
		}
		else if (sport == Sport.Hockey)
		{
			slots.Add(new KeyValuePair<Func<Player, bool>, int>((Player player) => player.Position == Position.Center, 1));
			slots.Add(new KeyValuePair<Func<Player, bool>, int>((Player player) => player.Position == Position.LeftWing, 1));
			slots.Add(new KeyValuePair<Func<Player, bool>, int>((Player player) => player.Position == Position.RightWing, 1));
			slots.Add(new KeyValuePair<Func<Player, bool>, int>((Player player) => player.Position == Position.Defense, 2));
			slots.Add(new KeyValuePair<Func<Player, bool>, int>((Player player) => player.Position == Position.Goalie, 1));
		}
		else
		{
			KeyValuePair<Position, int>[] array = new KeyValuePair<Position, int>[11]
			{
				new KeyValuePair<Position, int>(Position.Quarterback, 1),
				new KeyValuePair<Position, int>(Position.RunningBack, 1),
				new KeyValuePair<Position, int>(Position.WideReceiver, 3),
				new KeyValuePair<Position, int>(Position.TightEnd, 1),
				new KeyValuePair<Position, int>(Position.OffensiveLine, 5),
				new KeyValuePair<Position, int>(Position.DefensiveLine, 5),
				new KeyValuePair<Position, int>(Position.Linebacker, 2),
				new KeyValuePair<Position, int>(Position.Cornerback, 3),
				new KeyValuePair<Position, int>(Position.Safety, 2),
				new KeyValuePair<Position, int>(Position.Kicker, 1),
				new KeyValuePair<Position, int>(Position.Punter, 1)
			};
			for (int num = 0; num < array.Length; num++)
			{
				KeyValuePair<Position, int> kv = array[num];
				Position pos = kv.Key;
				slots.Add(new KeyValuePair<Func<Player, bool>, int>((Player player) => player.Position == pos, kv.Value));
			}
		}
		for (int kind = 1; kind <= teamsCount; kind++)
		{
			foreach (KeyValuePair<Func<Player, bool>, int> slot in slots)
			{
				List<Player> list = ranked(slot.Key);
				for (int i = 0; i < list.Count && i < slot.Value; i++)
				{
					Player p = list[i];
					taken.Add(p.Id);
					picks.Add(new AllStarPick
					{
						Year = s.Year,
						Season = label,
						Kind = kind,
						PlayerId = p.Id,
						Name = p.Name,
						TeamId = p.TeamId,
						Team = TeamName(p.TeamId),
						Pos = Rosters.NewsPositionLabel(p.Position, p.Sport),
						Starter = (kind == 1),
						Detail = AwardBook.SelectionLine(sport, p, Stats.Fit(p.Season, sport))
					});
					AddHonor(p, Level.Pro, label, AllLeagueName(sport, kind), "");
				}
			}
		}
		if (picks.Count == 0)
		{
			return;
		}
		StoreSelections(league, s.Year, 1, picks);
		List<string> first = new List<string>();
		foreach (AllStarPick k in picks)
		{
			if (k.Kind == 1 && first.Count < 6)
			{
				first.Add(k.Pos + " " + k.Name);
			}
		}
		Post(league, NewsKind.Award, Wire, "СИМВОЛИЧЕСКИЕ СБОРНЫЕ: " + label + " " + AllLeagueName(sport, 1) + ": " + string.Join(", ", first.ToArray()) + ((picks.FindAll((AllStarPick allStarPick) => allStarPick.Kind == 1).Count > first.Count) ? " и другие" : "") + ".", isUser: false, now, 2);
		PostOwnSelections(league, picks, "", now);
	}

	private void StoreSelections(League league, int year, int group, List<AllStarPick> picks)
	{
		league.AllStars.RemoveAll((AllStarPick allStarPick) => allStarPick.Year == year && ((group != 0) ? (allStarPick.Kind > 0) : (allStarPick.Kind == 0)));
		league.AllStars.InsertRange(0, picks);
		List<int> years = new List<int>();
		foreach (AllStarPick k in league.AllStars)
		{
			if (!years.Contains(k.Year))
			{
				years.Add(k.Year);
			}
		}
		years.Sort((int a, int b) => b.CompareTo(a));
		if (years.Count > 4)
		{
			int cutoff = years[3];
			league.AllStars.RemoveAll((AllStarPick allStarPick) => allStarPick.Year < cutoff);
		}
		Revision++;
	}

	private void PostOwnSelections(League league, List<AllStarPick> picks, string what, int now)
	{
		Dictionary<int, List<string>> mine = new Dictionary<int, List<string>>();
		foreach (AllStarPick k in picks)
		{
			Team t = GetTeam(k.TeamId);
			if (t != null && t.IsUser)
			{
				if (!mine.TryGetValue(t.Id, out var list))
				{
					list = (mine[t.Id] = new List<string>());
				}
				list.Add((k.Kind == 0) ? k.Name : (k.Name + " (" + AllLeagueName(league.Sport, k.Kind) + ")"));
			}
		}
		foreach (KeyValuePair<int, List<string>> kv in mine)
		{
			Team t2 = GetTeam(kv.Key);
			string names = ((kv.Value.Count == 1) ? kv.Value[0] : (string.Join(", ", kv.Value.GetRange(0, kv.Value.Count - 1).ToArray()) + " и " + kv.Value[kv.Value.Count - 1]));
			PostAsTeam(league, t2, NewsKind.Award, (what.Length > 0) ? ("Поздравляем " + names + ((kv.Value.Count == 1) ? (" — " + what) : (": " + ((league.Sport == Sport.Football) ? "они попали на матч звёзд" : "они стали участниками матча всех звёзд"))) + "!") : ("Поздравляем " + names + " с попаданием в символические сборные лиги!"), now);
		}
	}

	public static string SelectionsLine(Player p)
	{
		Dictionary<string, int> count = new Dictionary<string, int>();
		List<string> order = new List<string>();
		foreach (string honor in p.Honors)
		{
			if (ParseHonor(honor, out var _, out var award, out var _, out var _) && IsSelection(award))
			{
				if (!count.TryGetValue(award, out var n))
				{
					order.Add(award);
				}
				count[award] = n + 1;
			}
		}
		order.Sort((string award2, string b) => SelectionRank(award2).CompareTo(SelectionRank(b)));
		List<string> parts = new List<string>();
		foreach (string a in order)
		{
			parts.Add((count[a] > 1) ? (a + " " + count[a].ToString(CultureInfo.InvariantCulture) + "x") : a);
		}
		return string.Join(" · ", parts.ToArray());
	}

	private static int SelectionRank(string award)
	{
		if (award.StartsWith("First"))
		{
			return 1;
		}
		if (award.StartsWith("Second"))
		{
			return 2;
		}
		if (award.StartsWith("Third"))
		{
			return 3;
		}
		return 0;
	}

	public static string DateText(int day)
	{
		return GameClock.ShortDayLabel(day) + ", " + GameClock.YearOfDay(day);
	}

	private static int DayOf(DateTime d)
	{
		return GameClock.DayNumber(d);
	}

	private static void AddEvent(List<CalendarEvent> list, int day, string title, string detail, int now, int doneFlag)
	{
		int monthStart = GameClock.MonthStartDay(now);
		int nextMonth = GameClock.NextMonthStartDay(now);
		int state = doneFlag switch
		{
			1 => 0, 
			0 => (day < nextMonth) ? 1 : 2, 
			_ => (day >= monthStart) ? ((day < nextMonth) ? 1 : 2) : 0, 
		};
		list.Add(new CalendarEvent
		{
			Day = day,
			Date = DateText(day),
			Title = title,
			Detail = detail,
			State = state
		});
	}

	public List<CalendarEvent> Calendar(League league, int now)
	{
		List<CalendarEvent> list = new List<CalendarEvent>();
		SportProfile p = Profile(league);
		Season cur = league.Current;
		Season prev = league.Previous;
		bool pro = SportRules.ProLike(league.Sport, league.Level);
		EconomyProfile e = Economy.Get(league.Sport);
		if (pro && prev != null && prev != cur && (league.LeagueYearDoneFor < prev.Year || league.DraftDoneFor < prev.Year))
		{
			AddOffseason(list, league, prev.Year, now);
		}
		int y = cur?.Year ?? ((prev != null) ? (prev.Year + 1) : CurrentYear());
		if (cur != null)
		{
			int first = int.MaxValue;
			int last = int.MinValue;
			foreach (Match g in cur.Games)
			{
				if (g.Stage == GameStage.Regular)
				{
					first = Math.Min(first, GameClock.DayOf(g.Time));
					last = Math.Max(last, GameClock.DayOf(g.Time));
				}
			}
			if (first == int.MaxValue)
			{
				first = DayOf(p.Start(y));
				last = DayOf(p.End(y));
			}
			AddEvent(list, first, "Начало регулярного сезона", SeasonLabel(league, cur) + " сезон: " + TargetGames(league) + " матчей на команду.", now, -1);
			if (pro && league.Sport == Sport.Soccer)
			{
				AddEvent(list, DayOf(new DateTime(y, 7, 13)), "Летнее трансферное окно", "Открыто до 2 сентября: клубы покупают и продают игроков за трансферную плату (Офис клуба → Трансферы).", now, -1);
				AddEvent(list, DayOf(new DateTime(y + 1, 1, 26)), "Зимнее трансферное окно", "Открыто до 26 марта: клубы покупают и продают игроков за трансферную плату (Офис клуба → Трансферы).", now, -1);
			}
			else if (pro)
			{
				AddEvent(list, DayOf(e.TradeDeadline(y)), "Дедлайн обменов", "Обмены запрещены до окончания плей-офф.", now, -1);
			}
			AddEvent(list, last, "Окончание регулярного сезона", "Начинается плей-офф. Команды, не прошедшие в него, могут сменить тренеров (открывается окно смены тренеров).", now, cur.PlayoffsCreated ? 1 : (-1));
			if (!pro)
			{
				int end = ((cur.PlayoffsCreated && cur.PlayoffEndDay > 0) ? cur.PlayoffEndDay : (last + 21));
				List<CalendarEvent> list2 = list;
				int day = end;
				string detail;
				if (league.Level == Level.Minor)
				{
					detail = "Игроки развиваются и взрослеют, ветераны завершают карьеру, а клубы пополняют состав молодыми перспективными спортсменами и игроками, которых не подписали в период свободных агентов.";
				}
				else
				{
					detail = ((league.Level == Level.Junior) ? "Игроки третьего года переходят на следующий уровень (большинство — в студенческие лиги, лучшие — на профессиональный драфт), приходят выпускники школ. Летом игроки развиваются." : "Выпускники завершают обучение, лучшие школьники выбирают университеты, лучшие студенческие игроки выходят на профессиональный драфт, приходят первокурсники. Летом игроки развиваются.");
				}
				AddEvent(list2, day, "Окончание сезона", detail, now, cur.Complete ? 1 : (-1));
			}
			if (pro)
			{
				AddOffseason(list, league, y, now);
			}
		}
		int nextStart = DayOf(p.Start(y + 1));
		if (cur == null && prev == null)
		{
			nextStart = DayOf(p.Start(y));
		}
		AddEvent(list, nextStart, "Начало следующего сезона", pro ? "Окно смены тренеров закрывается (для профессиональных команд — с открытием рынка свободных агентов)." : "Окно смены тренеров закрывается после первого матча.", now, -1);
		list.Sort((CalendarEvent a, CalendarEvent b) => a.Day.CompareTo(b.Day));
		return list;
	}

	private void AddOffseason(List<CalendarEvent> list, League league, int y, int now)
	{
		EconomyProfile e = Economy.Get(league.Sport);
		if (SportRules.IsSecondDivision(league.Sport, league.Level))
		{
			AddEvent(list, DayOf(e.DraftDate(y)), "Повышение и вылет", "После завершения сезона в обоих дивизионах: лидер этой таблицы повышается в классе, а последняя команда высшего дивизиона выбывает ниже.", now, (league.PromotionDoneFor >= y) ? 1 : 0);
			AddEvent(list, DayOf(e.LeagueYearDate(y)), "Открытие рынка свободных агентов", "Начинается новый сезон лиги: контракты заканчиваются. Продлите договоры игроков до этой даты, иначе они станут свободными агентами. Игроки развиваются, взрослеют, некоторые завершают карьеру.", now, (league.LeagueYearDoneFor >= y) ? 1 : 0);
			return;
		}
		if (e.LotteryDate != null)
		{
			AddEvent(list, DayOf(e.LotteryDate(y)), "Лотерея драфта", "Команды, не попавшие в плей-офф, разыгрывают первые " + e.LotteryPicks + ((e.LotteryPicks == 1) ? " выбор." : " выбора."), now, (league.LotteryDoneFor >= y) ? 1 : 0);
		}
		if (!HasDraft(league.Sport))
		{
			AddEvent(list, DayOf(e.DraftDate(y)), "Молодые перспективные игроки", "В футболе нет драфта: молодые перспективные игроки года выходят на рынок. Подписывайте их на вкладке «Свободные агенты»; остальные клубы будут заключать контракты ежемесячно.", now, (league.DraftDoneFor >= y) ? 1 : 0);
		}
		else
		{
			AddEvent(list, DayOf(e.DraftDate(y)), DraftYearLabel(league, y) + " " + e.DraftName, e.DraftRounds + " раундов. Команды под вашим управлением делают выбор самостоятельно в месяц драфта (если месяц закончится, игроков выберут скауты).", now, (league.DraftDoneFor >= y) ? 1 : 0);
		}
		AddEvent(list, DayOf(e.LeagueYearDate(y)), "Открытие рынка свободных агентов", "Начинается новый сезон лиги: контракты заканчиваются. Продлите договоры до этой даты, иначе игроки станут свободными агентами. Окно смены тренеров закрывается. Игроки развиваются, взрослеют, некоторые завершают карьеру.", now, (league.LeagueYearDoneFor >= y) ? 1 : 0);
	}

	public List<string> Windows(League league, int now)
	{
		List<string> lines = new List<string>();
		bool pro = SportRules.ProLike(league.Sport, league.Level);
		EconomyProfile e = Economy.Get(league.Sport);
		if (pro && league.Sport == Sport.Soccer)
		{
			lines.Add("Трансферы: " + TransferWindowText(now));
		}
		else if (pro)
		{
			bool trade = TradeWindowOpen(league, now);
			string deadline = TradeDeadlineLabel(league);
			lines.Add(trade ? ("Обмены: открыты" + ((league.Current != null && league.Current.AnyPlayed && deadline.Length > 0) ? (" до дедлайна, " + deadline + ".") : ".")) : "Обмены: закрыты до окончания плей-офф.");
		}
		lines.Add(CoachingWindowOpen(league, now) ? ("Смена тренеров: открыта" + (pro ? " до открытия рынка свободных агентов." : " до первого матча следующего сезона.")) : "Смена тренеров закрыта до окончания регулярного сезона.");
		if (pro)
		{
			int y = NextLeagueYear(league);
			string fa = DateText(DayOf(e.LeagueYearDate(y)));
			lines.Add("Продление контрактов: продлевайте договоры игроков, у которых остался последний год, до " + fa + ". После этой даты они становятся свободными агентами.");
			lines.Add("Свободные агенты: большой рынок откроется " + fa + ". Игроки без контракта могут подписать соглашение в любое время, а их запросы по зарплате постепенно снижаются.");
		}
		else
		{
			List<string> list = lines;
			string item;
			if (league.Level == Level.Minor)
			{
				item = "Вызовы: каждый клуб является фарм-клубом профессиональной команды. Профессиональный клуб вызывает игроков по необходимости и отправляет молодых получать игровую практику в любое время года.";
			}
			else
			{
				item = ((league.Level == Level.Junior) ? "Набор: выпускники школ присоединяются к юниорским командам после окончания сезона. Игроки проводят там до трёх сезонов, затем переходят в студенческие или профессиональные лиги." : "Набор: выпускники школ выбирают университеты после окончания сезона. Хорошие тренеры по набору игроков и местные воспитанники помогают вашим учебным заведениям.");
			}
			list.Add(item);
		}
		return lines;
	}

	private int NextLeagueYear(League league)
	{
		foreach (Season s in SeasonsOldestFirst(league))
		{
			if (league.LeagueYearDoneFor < s.Year)
			{
				return s.Year;
			}
		}
		if (league.Current != null)
		{
			return league.Current.Year;
		}
		if (league.LeagueYearDoneFor != int.MinValue)
		{
			return league.LeagueYearDoneFor + 1;
		}
		return CurrentYear();
	}

	public CalendarEvent NextEvent(League league, int now)
	{
		foreach (CalendarEvent ev in Calendar(league, now))
		{
			if (ev.State != 0)
			{
				return ev;
			}
		}
		return null;
	}

	public static int CoachBase(Level level)
	{
		return level switch
		{
			Level.Pro => 76, 
			Level.Minor => 70, 
			Level.College => 68, 
			Level.Junior => 63, 
			_ => 58, 
		};
	}

	private static double CoachSpread(Level level)
	{
		if (level != Level.Pro)
		{
			return (level == Level.College) ? 9 : 9;
		}
		return 7.5;
	}

	public static int RescaleOldCoachRating(int rating, Level level)
	{
		return Rosters.Clamp((int)Math.Round((double)CoachBase(level) + (double)(rating - level switch
		{
			Level.College => 58, 
			Level.Pro => 66, 
			_ => 50, 
		}) * 1.45), 25, 99);
	}

	public static string RoleLabel(CoachRole role, Sport sport)
	{
		switch (role)
		{
		case CoachRole.Offense:
			return "Координатор нападения";
		case CoachRole.Defense:
			return "Координатор защиты";
		default:
			if (sport != Sport.Baseball)
			{
				return "Главный тренер";
			}
			return "Менеджер";
		}
	}

	private static CoachRole[] RolesFor(Sport sport)
	{
		if (sport != Sport.Football)
		{
			return new CoachRole[1];
		}
		return new CoachRole[3]
		{
			CoachRole.Head,
			CoachRole.Offense,
			CoachRole.Defense
		};
	}

	private void StaffChanged()
	{
		_staffStamp++;
	}

	private List<Coach> StaffList(int teamId)
	{
		if (_staff == null || _staffRevision != Revision || _staffBuiltStamp != _staffStamp || _staffCount != Coaches.Count)
		{
			_staff = new Dictionary<int, List<Coach>>();
			foreach (Coach c in Coaches.Values)
			{
				if (c.TeamId >= 0)
				{
					if (!_staff.TryGetValue(c.TeamId, out var list))
					{
						list = new List<Coach>();
						_staff[c.TeamId] = list;
					}
					list.Add(c);
				}
			}
			foreach (List<Coach> value in _staff.Values)
			{
				value.Sort((Coach a, Coach b) => (a.Role == b.Role) ? a.Id.CompareTo(b.Id) : a.Role.CompareTo(b.Role));
			}
			_staffRevision = Revision;
			_staffBuiltStamp = _staffStamp;
			_staffCount = Coaches.Count;
		}
		if (!_staff.TryGetValue(teamId, out var found))
		{
			return _noStaff;
		}
		return found;
	}

	public List<Coach> StaffOf(int teamId)
	{
		return new List<Coach>(StaffList(teamId));
	}

	public Coach CoachOf(int teamId, CoachRole role)
	{
		foreach (Coach c in StaffList(teamId))
		{
			if (c.Role == role)
			{
				return c;
			}
		}
		return null;
	}

	public List<Coach> AvailableCoaches(Sport sport, Level level)
	{
		List<Coach> list = new List<Coach>();
		foreach (Coach c in Coaches.Values)
		{
			if (c.TeamId < 0 && c.Sport == sport && c.Level == level)
			{
				list.Add(c);
			}
		}
		list.Sort((Coach a, Coach b) => (b.Overall == a.Overall) ? a.Id.CompareTo(b.Id) : b.Overall.CompareTo(a.Overall));
		return list;
	}

	private double TeamRating(Team t, Func<Coach, int> rating, double headWeight)
	{
		Coach head = CoachOf(t.Id, CoachRole.Head);
		int b = CoachBase(t.Level);
		double hr = ((head == null) ? (b - 8) : rating(head));
		if (t.Sport != Sport.Football)
		{
			return hr;
		}
		Coach oc = CoachOf(t.Id, CoachRole.Offense);
		Coach dc = CoachOf(t.Id, CoachRole.Defense);
		double o = ((oc == null) ? (b - 8) : rating(oc));
		double d = ((dc == null) ? (b - 8) : rating(dc));
		return headWeight * hr + (1.0 - headWeight) / 2.0 * (o + d);
	}

	private double LeagueStaffMean(Team t, string kind, Func<Coach, int> rating, double headWeight)
	{
		if (_staffMeansStamp != _staffStamp)
		{
			_staffMeans.Clear();
			_staffMeansStamp = _staffStamp;
		}
		string[] array = new string[5];
		int sport = (int)t.Sport;
		array[0] = sport.ToString();
		array[1] = ":";
		sport = (int)t.Level;
		array[2] = sport.ToString();
		array[3] = ":";
		array[4] = kind;
		string key = string.Concat(array);
		if (_staffMeans.TryGetValue(key, out var mean))
		{
			return mean;
		}
		League league = LeagueOf(t);
		double sum = 0.0;
		int n = 0;
		if (league != null)
		{
			foreach (int id in league.TeamIds)
			{
				Team o = GetTeam(id);
				if (o != null && !o.Retired)
				{
					sum += TeamRating(o, rating, headWeight);
					n++;
				}
			}
		}
		mean = ((n == 0) ? ((double)CoachBase(t.Level)) : (sum / (double)n));
		_staffMeans[key] = mean;
		return mean;
	}

	public double TacticsBonus(Team t)
	{
		if (t == null)
		{
			return 0.0;
		}
		double v = (TeamRating(t, (Coach c) => c.Tactics, 0.4) - LeagueStaffMean(t, "tac", (Coach c) => c.Tactics, 0.4)) / 24.0;
		return Math.Max(-1.5, Math.Min(1.5, v));
	}

	public double MotivationFactor(Team t)
	{
		if (t == null)
		{
			return 1.0;
		}
		double v = 1.0 + (TeamRating(t, (Coach c) => c.Motivation, 0.6) - LeagueStaffMean(t, "mot", (Coach c) => c.Motivation, 0.6)) / 98.0;
		return Math.Max(0.75, Math.Min(1.25, v));
	}

	public double DevelopmentShift(Team t)
	{
		if (t == null)
		{
			return 0.0;
		}
		double v = (TeamRating(t, (Coach c) => c.Development, 0.5) - LeagueStaffMean(t, "dev", (Coach c) => c.Development, 0.5)) / 30.0;
		return Math.Max(-0.8, Math.Min(0.8, v));
	}

	public double SchoolDevelopmentBoost(Team t)
	{
		if (t == null)
		{
			return 0.0;
		}
		double v = (TeamRating(t, (Coach c) => c.Development, 0.5) - LeagueStaffMean(t, "dev", (Coach c) => c.Development, 0.5)) / 490.0;
		return Math.Max(-0.04, Math.Min(0.04, v));
	}

	public double RecruitingEdge(Team t)
	{
		if (t == null)
		{
			return 0.0;
		}
		double r = CoachOf(t.Id, CoachRole.Head)?.Recruiting ?? (CoachBase(t.Level) - 8);
		return Math.Max(-3.0, Math.Min(3.0, (r - LeagueStaffMean(t, "rec", (Coach c) => c.Recruiting, 1.0)) / 7.3));
	}

	private Coach NewCoach(Sport sport, Level level, CoachRole role, double quality, int age, Player forceEx = null)
	{
		Player ex = forceEx ?? TakeExPlayer(sport, level);
		if (ex != null)
		{
			age = CurrentYear() - ex.BirthYear;
		}
		PlayerNames.Pick(sport, Level.College, _random, null, out var first, out var last);
		int b = CoachBase(level);
		double talent = Rosters.Normal(_random, 0.0, CoachSpread(level));
		int growth = ((age < 46) ? ((int)Math.Round((double)(46 - age) * 0.45)) : 0);
		talent -= growth;
		Func<int> rate = () => Rosters.Clamp((int)Math.Round((double)b + quality + talent + Rosters.Normal(_random, 0.0, 4.5)), 25, 99);
		Coach c = new Coach
		{
			Id = NextId++,
			FirstName = first,
			LastName = last,
			Sport = sport,
			Level = level,
			Role = role,
			BirthYear = CurrentYear() - age,
			Development = rate(),
			Tactics = rate(),
			Motivation = rate(),
			Recruiting = rate()
		};
		int ovr = c.Overall;
		c.Potential = Math.Min(99, ovr + growth + _random.Next(0, 4));
		c.PrevOverall = ovr;
		c.Hometown = Bios.HometownFor(first, sport, _random);
		if (ex != null)
		{
			BecomeCoach(c, ex);
		}
		c.PrevOverall = c.Overall;
		Coaches[c.Id] = c;
		return c;
	}

	private void EnsureStaff(Team team, int now)
	{
		if (team == null || team.Retired)
		{
			return;
		}
		CoachRole[] array = RolesFor(team.Sport);
		foreach (CoachRole role in array)
		{
			if (CoachOf(team.Id, role) != null)
			{
				continue;
			}
			Coach pick = null;
			foreach (Coach c in AvailableCoaches(team.Sport, team.Level))
			{
				if (c.Role == role && (pick == null || c.Overall > pick.Overall))
				{
					pick = c;
				}
			}
			if (pick == null || _random.NextDouble() < 0.35)
			{
				double quality = (double)(team.Rating - 50) * 0.12 + Rosters.Normal(_random, 0.0, 2.0);
				pick = NewCoach(team.Sport, team.Level, role, quality, 34 + _random.Next(0, 26));
				if (team.IsUser && SportRules.IsSchool(team.Level) && pick.FormerPlayerId < 0 && _random.NextDouble() < 0.4)
				{
					pick.Hometown = "*";
				}
			}
			Hire(team, pick, role, now, announce: false);
		}
	}

	private void Hire(Team team, Coach c, CoachRole role, int now, bool announce)
	{
		c.TeamId = team.Id;
		c.Role = role;
		c.Sport = team.Sport;
		c.Level = team.Level;
		if (c.Potential < c.Overall)
		{
			c.Potential = c.Overall;
		}
		c.HiredYear = CurrentYear();
		c.BadSeasons = 0;
		StaffChanged();
		Revision++;
		if (announce)
		{
			League league = LeagueOf(team);
			string text = "НАНЯТ: " + TeamName(team.Id) + " " + c.Name + " нанимает нового " + RoleLabel(role, team.Sport).ToLowerInvariant() + " (общий рейтинг " + c.Overall + ((c.Wins + c.Losses > 0) ? (", статистика за карьеру: " + c.Wins + "-" + c.Losses) : "") + ").";
			Post(league, NewsKind.General, Wire, text, team.IsUser, now, 2);
			if (team.IsUser)
			{
				PostAsTeam(league, team, NewsKind.General, "Приветствуем " + c.Name + " — наш новый " + RoleLabel(role, team.Sport).ToLowerInvariant() + "!", now);
			}
		}
	}

	public void EnsureAllStaff(int now)
	{
		foreach (League l in Leagues)
		{
			foreach (int id in l.TeamIds)
			{
				EnsureStaff(GetTeam(id), now);
			}
			foreach (int id2 in l.PendingTeamIds)
			{
				EnsureStaff(GetTeam(id2), now);
			}
		}
		CleanUpCoaches();
	}

	private void CleanUpCoaches()
	{
		List<int> remove = new List<int>();
		foreach (Coach c in Coaches.Values)
		{
			if (c.TeamId >= 0 && GetTeam(c.TeamId) == null)
			{
				c.TeamId = -1;
			}
			if (c.TeamId < 0 && GetLeague(c.Sport, c.Level) == null)
			{
				remove.Add(c.Id);
			}
		}
		foreach (int id in remove)
		{
			Coaches.Remove(id);
		}
		StaffChanged();
	}

	private void RefillCoachPool(League league)
	{
		List<Coach> pool = AvailableCoaches(league.Sport, league.Level);
		CoachRole[] array = RolesFor(league.Sport);
		foreach (CoachRole role in array)
		{
			for (int i2 = pool.FindAll((Coach c) => c.Role == role).Count; i2 < 3; i2++)
			{
				NewCoach(league.Sport, league.Level, role, -6.0 + Rosters.Normal(_random, 0.0, 2.0), 32 + _random.Next(0, 22));
			}
		}
		pool = AvailableCoaches(league.Sport, league.Level);
		if (pool.Count > 10)
		{
			pool.Sort((Coach a, Coach b) => a.BirthYear.CompareTo(b.BirthYear));
			for (int i3 = 0; i3 < pool.Count - 10; i3++)
			{
				Coaches.Remove(pool[i3].Id);
			}
		}
	}

	private void RecordCoachResult(Match g)
	{
		int winner = g.WinnerId;
		int[] array = new int[2] { g.HomeTeamId, g.AwayTeamId };
		foreach (int teamId in array)
		{
			foreach (Coach c in StaffOf(teamId))
			{
				if (g.Stage == GameStage.Regular)
				{
					if (winner < 0)
					{
						c.Ties++;
					}
					else if (winner == teamId)
					{
						c.Wins++;
					}
					else
					{
						c.Losses++;
					}
				}
				else if (winner == teamId)
				{
					c.PlayoffWins++;
				}
				else if (winner >= 0)
				{
					c.PlayoffLosses++;
				}
			}
		}
	}

	private void CoachesSeasonEnd(League league, Season s, int now)
	{
		int year = CurrentYear();
		foreach (Coach c in new List<Coach>(Coaches.Values))
		{
			if (c.Sport != league.Sport || c.Level != league.Level)
			{
				continue;
			}
			if (c.TeamId >= 0 && league.TeamIds.Contains(c.TeamId))
			{
				c.Seasons++;
				if (c.TeamId == s.ChampionTeamId)
				{
					c.Titles++;
				}
			}
			c.PrevOverall = c.Overall;
			DevelopCoach(c, year - c.BirthYear);
			int age = year - c.BirthYear;
			double retire;
			if (age >= 75)
			{
				retire = 1.0;
			}
			else
			{
				retire = ((age >= 62) ? (0.08 + 0.05 * (double)(age - 62)) : 0.0);
			}
			if (c.TeamId < 0 && age >= 58)
			{
				retire += 0.15;
			}
			if (!(_random.NextDouble() >= retire))
			{
				Team t = GetTeam(c.TeamId);
				if (t != null && (t.IsUser || c.Titles >= 2))
				{
					Post(league, NewsKind.Retirement, Wire, "" + TeamName(t.Id) + " — " + RoleLabel(c.Role, c.Sport).ToLowerInvariant() + " " + c.Name + " завершает карьеру в возрасте " + age + " (статистика за карьеру: " + c.Wins + "-" + c.Losses + ((c.Titles > 0) ? (", " + c.Titles + ((c.Titles == 1) ? " титул" : " титула")) : "") + ").", t.IsUser, now, 2);
				}
				Coaches.Remove(c.Id);
			}
		}
		StaffChanged();
		foreach (int id in league.TeamIds)
		{
			EnsureStaff(GetTeam(id), now);
		}
		RefillCoachPool(league);
	}

	private void DevelopCoach(Coach c, int age)
	{
		if (age < 50 && c.Overall < c.Potential)
		{
			if (_random.NextDouble() < 0.6)
			{
				c.Development = Math.Min(99, c.Development + _random.Next(0, 3));
			}
			if (_random.NextDouble() < 0.6)
			{
				c.Tactics = Math.Min(99, c.Tactics + _random.Next(0, 3));
			}
			if (_random.NextDouble() < 0.6)
			{
				c.Motivation = Math.Min(99, c.Motivation + _random.Next(0, 3));
			}
			if (_random.NextDouble() < 0.6)
			{
				c.Recruiting = Math.Min(99, c.Recruiting + _random.Next(0, 3));
			}
		}
		if (age <= 45 && _random.NextDouble() < 0.05)
		{
			c.Potential = Math.Min(99, c.Potential + _random.Next(3, 7));
		}
		if (age >= 58 && _random.NextDouble() < 0.25)
		{
			int which = _random.Next(4);
			int drop = _random.Next(1, 3);
			switch (which)
			{
			case 0:
				c.Development = Math.Max(25, c.Development - drop);
				break;
			case 1:
				c.Tactics = Math.Max(25, c.Tactics - drop);
				break;
			case 2:
				c.Motivation = Math.Max(25, c.Motivation - drop);
				break;
			default:
				c.Recruiting = Math.Max(25, c.Recruiting - drop);
				break;
			}
		}
		if (c.Potential < c.Overall)
		{
			c.Potential = c.Overall;
		}
	}

	private void RunCoachingCarousel(League league, Season s, int now)
	{
		if (league.CoachingDoneFor >= s.Year)
		{
			return;
		}
		league.CoachingDoneFor = s.Year;
		List<StandingRow> table = Standings.Compute(league, s);
		int q = PlayoffTeamCount(Profile(league), table.Count);
		if (q >= table.Count)
		{
			q = table.Count * 2 / 3;
		}
		int year = CurrentYear();
		int n = table.Count;
		List<int> byRating = new List<int>();
		foreach (StandingRow row in table)
		{
			byRating.Add(row.TeamId);
		}
		byRating.Sort((int a, int b) =>
		{
			Team team = GetTeam(a);
			return (GetTeam(b)?.Rating ?? 0).CompareTo(team?.Rating ?? 0);
		});
		foreach (StandingRow row2 in table)
		{
			Team t = GetTeam(row2.TeamId);
			Coach head = ((t == null) ? null : CoachOf(t.Id, CoachRole.Head));
			if (t == null || head == null || !league.TeamIds.Contains(t.Id))
			{
				continue;
			}
			int expectedRank = byRating.IndexOf(t.Id) + 1;
			bool missed = row2.Rank > q;
			bool bottom = row2.Rank > n - Math.Max(1, n / 4);
			bool disappointing = missed && ((row2.Rank - expectedRank >= Math.Max(2, n / 5)) | bottom);
			head.BadSeasons = (disappointing ? (head.BadSeasons + 1) : 0);
			if (t.Manual || !missed || head.HiredYear >= year)
			{
				continue;
			}
			double fire;
			if (head.BadSeasons >= 3)
			{
				fire = 0.9;
			}
			else if (head.BadSeasons == 2)
			{
				fire = 0.55;
			}
			else if (disappointing)
			{
				fire = (bottom ? 0.3 : 0.18);
			}
			else
			{
				fire = 0.03;
			}
			if (head.Titles > 0 && head.Seasons - head.BadSeasons < 6)
			{
				fire *= 0.5;
			}
			if (t.Level != Level.Pro)
			{
				fire *= 0.6;
			}
			if (_random.NextDouble() >= fire)
			{
				continue;
			}
			Fire(t, head, row2, now);
			if (t.Sport == Sport.Football)
			{
				foreach (Coach c in StaffOf(t.Id))
				{
					if (c.Role != CoachRole.Head && _random.NextDouble() < 0.5)
					{
						c.TeamId = -1;
						c.BadSeasons = 0;
						StaffChanged();
					}
				}
			}
			HireBest(league, t, CoachRole.Head, now);
			EnsureStaff(t, now);
		}
		RefillCoachPool(league);
	}

	private void Fire(Team t, Coach head, StandingRow row, int now)
	{
		League league = LeagueOf(t);
		Post(league, NewsKind.General, Wire, "УВОЛЕН: " + TeamName(t.Id) + " расстаётся с " + RoleLabel(head.Role, t.Sport).ToLowerInvariant() + " " + head.Name + ((row != null) ? (" после " + Standings.RecordText(t.Sport, row) + " сезона") : "") + ".", t.IsUser, now, 2);
		if (t.IsUser)
		{
			AddNote(league, "" + TeamName(t.Id) + " отпускает " + RoleLabel(head.Role, t.Sport).ToLowerInvariant() + " " + head.Name + ".");
		}
		head.TeamId = -1;
		head.BadSeasons = 0;
		head.History = AddHistory(head.History, TeamName(t.Id), head.HiredYear, CurrentYear());
		StaffChanged();
		Revision++;
	}

	private static string AddHistory(string history, string team, int from, int to)
	{
		string entry = team + " (" + from + "-" + to + ")";
		if (!string.IsNullOrEmpty(history))
		{
			return entry + ", " + history;
		}
		return entry;
	}

	private void HireBest(League league, Team t, CoachRole role, int now)
	{
		Coach best = null;
		double bestScore = double.MinValue;
		foreach (Coach c in Coaches.Values)
		{
			if (c.Sport != t.Sport)
			{
				continue;
			}
			double score;
			if (c.TeamId < 0 && c.Level == t.Level && (c.Role == role || role == CoachRole.Head))
			{
				score = c.Overall;
			}
			else if (role == CoachRole.Head && c.Level == t.Level && c.Role != CoachRole.Head && c.TeamId >= 0 && c.TeamId != t.Id)
			{
				score = c.Overall - 4;
			}
			else
			{
				if (role != CoachRole.Head || c.Role != CoachRole.Head || c.TeamId < 0 || !IsLevelBelow(c.Level, t.Level))
				{
					continue;
				}
				score = OverallAtLevel(c, t.Level) + c.Titles * 2 - 2;
			}
			score += Rosters.Normal(_random, 0.0, 6.0);
			if (score > bestScore)
			{
				bestScore = score;
				best = c;
			}
		}
		if (best == null)
		{
			best = NewCoach(t.Sport, t.Level, role, Rosters.Normal(_random, 0.0, 2.0), 36 + _random.Next(0, 20));
		}
		Team oldTeam = ((best.TeamId >= 0) ? GetTeam(best.TeamId) : null);
		if (oldTeam != null)
		{
			best.History = AddHistory(best.History, TeamName(oldTeam.Id), best.HiredYear, CurrentYear());
			best.TeamId = -1;
			StaffChanged();
		}
		if (best.Level != t.Level)
		{
			MoveToLevel(best, t.Level);
		}
		Hire(t, best, role, now, announce: true);
		if (oldTeam != null)
		{
			EnsureStaff(oldTeam, now);
		}
	}

	private static int RatingAtLevel(int rating, Level from, Level to)
	{
		if (from == to)
		{
			return rating;
		}
		return Rosters.Clamp((int)Math.Round((double)CoachBase(to) + (double)(rating - CoachBase(from)) * 0.8 - 1.0), 25, 99);
	}

	private static int OverallAtLevel(Coach c, Level level)
	{
		return RatingAtLevel(c.Overall, c.Level, level);
	}

	private static void MoveToLevel(Coach c, Level level)
	{
		Level from = c.Level;
		int before = c.Overall;
		c.Development = RatingAtLevel(c.Development, from, level);
		c.Tactics = RatingAtLevel(c.Tactics, from, level);
		c.Motivation = RatingAtLevel(c.Motivation, from, level);
		c.Recruiting = RatingAtLevel(c.Recruiting, from, level);
		c.Potential = Math.Max(c.Overall, RatingAtLevel(c.Potential, from, level));
		c.PrevOverall = c.Overall - (before - c.PrevOverall);
		c.Level = level;
	}

	private static bool IsLevelBelow(Level lower, Level higher)
	{
		switch (higher)
		{
		case Level.College:
			if (lower != Level.HighSchool)
			{
				return lower == Level.Junior;
			}
			return true;
		case Level.Pro:
			if (lower != Level.College)
			{
				return lower == Level.Minor;
			}
			return true;
		default:
			return false;
		}
	}

	public bool CoachingWindowOpen(League league, int now)
	{
		Season s = league.Current;
		if (s == null)
		{
			return true;
		}
		if (s.PlayoffsCreated)
		{
			return true;
		}
		if (s.AnyPlayed)
		{
			return false;
		}
		if (!SportRules.ProLike(league.Sport, league.Level) || league.Previous == null)
		{
			return true;
		}
		return league.LeagueYearDoneFor < league.Previous.Year;
	}

	private Team UserTeamForCoaching(int teamId, int now, out string error)
	{
		error = "";
		Team t = GetTeam(teamId);
		if (t == null || t.Retired || !t.IsUser)
		{
			error = "Это не одна из ваших команд.";
			return null;
		}
		if (!t.Manual)
		{
			error = "Сначала выберите «Самостоятельное управление командой» в офисе клуба, чтобы менять тренерский состав.";
			return null;
		}
		League league = LeagueOf(t);
		if (league != null && !CoachingWindowOpen(league, now))
		{
			error = "Смена тренеров происходит между окончанием регулярного сезона и " + (SportRules.ProLike(t.Sport, t.Level) ? "началом периода свободных агентов." : "начала следующего сезона.");
			return null;
		}
		return t;
	}

	public CommandResult FireCoach(int coachId, int now)
	{
		_clockHint = now;
		if (!Coaches.TryGetValue(coachId, out var c) || c.TeamId < 0)
		{
			return CommandResult.Fail("Тренер не найден.");
		}
		Team t = UserTeamForCoaching(c.TeamId, now, out var error);
		if (t == null)
		{
			return CommandResult.Fail(error);
		}
		Fire(t, c, null, now);
		return CommandResult.Success(c.Name + " был уволен. Наймите нового " + RoleLabel(c.Role, t.Sport).ToLowerInvariant() + " из списка ниже (если этого не сделать, команда наймёт тренера перед следующим матчем).");
	}

	public CommandResult HireCoach(int teamId, int coachId, int role, int now)
	{
		_clockHint = now;
		Team t = UserTeamForCoaching(teamId, now, out var error);
		if (t == null)
		{
			return CommandResult.Fail(error);
		}
		if (!Coaches.TryGetValue(coachId, out var c) || c.Sport != t.Sport)
		{
			return CommandResult.Fail("Тренер не найден.");
		}
		CoachRole r = (Enum.IsDefined(typeof(CoachRole), role) ? ((CoachRole)role) : CoachRole.Head);
		if (Array.IndexOf(RolesFor(t.Sport), r) < 0)
		{
			return CommandResult.Fail("Для этой команды такой должности нет.");
		}
		if (CoachOf(t.Id, r) != null)
		{
			return CommandResult.Fail("Сначала отпустите нынешнего " + RoleLabel(r, t.Sport).ToLowerInvariant() + ".");
		}
		bool num = c.TeamId < 0 && c.Level == t.Level;
		bool promotion = r == CoachRole.Head && c.Role != CoachRole.Head && c.Level == t.Level;
		bool fromBelow = r == CoachRole.Head && c.Role == CoachRole.Head && c.TeamId >= 0 && IsLevelBelow(c.Level, t.Level);
		if (!num && !promotion && !fromBelow)
		{
			return CommandResult.Fail(c.Name + " недоступен для этой должности.");
		}
		Team oldTeam = ((c.TeamId >= 0) ? GetTeam(c.TeamId) : null);
		if (oldTeam != null && oldTeam.IsUser && oldTeam.Manual)
		{
			return CommandResult.Fail(c.Name + " тренирует другую вашу команду.");
		}
		if (oldTeam != null)
		{
			c.History = AddHistory(c.History, TeamName(oldTeam.Id), c.HiredYear, CurrentYear());
			c.TeamId = -1;
			StaffChanged();
		}
		if (c.Level != t.Level)
		{
			MoveToLevel(c, t.Level);
		}
		Hire(t, c, r, now, announce: true);
		if (oldTeam != null)
		{
			EnsureStaff(oldTeam, now);
		}
		return CommandResult.Success(c.Name + " назначен на должность: " + RoleLabel(r, t.Sport).ToLowerInvariant() + ".");
	}

	public List<Coach> CoachCandidates(Team t, CoachRole role)
	{
		List<Coach> list = new List<Coach>();
		foreach (Coach c in Coaches.Values)
		{
			if (c.Sport == t.Sport)
			{
				bool num = c.TeamId < 0 && c.Level == t.Level && (c.Role == role || role == CoachRole.Head);
				bool promotion = role == CoachRole.Head && c.Role != CoachRole.Head && c.Level == t.Level && c.TeamId >= 0 && c.TeamId != t.Id;
				bool fromBelow = role == CoachRole.Head && c.Role == CoachRole.Head && c.TeamId >= 0 && IsLevelBelow(c.Level, t.Level);
				if (num | promotion | fromBelow)
				{
					list.Add(c);
				}
			}
		}
		list.Sort((Coach a, Coach b) => (b.Overall == a.Overall) ? a.Id.CompareTo(b.Id) : b.Overall.CompareTo(a.Overall));
		if (list.Count > 12)
		{
			list.RemoveRange(12, list.Count - 12);
		}
		return list;
	}

	public static string CoachGrade(Coach c)
	{
		int diff = c.Potential - CoachBase(c.Level);
		if (diff >= 20)
		{
			return "A+";
		}
		if (diff >= 15)
		{
			return "A";
		}
		if (diff >= 11)
		{
			return "A-";
		}
		if (diff >= 7)
		{
			return "B+";
		}
		if (diff >= 3)
		{
			return "B";
		}
		if (diff >= -1)
		{
			return "B-";
		}
		if (diff >= -5)
		{
			return "C+";
		}
		if (diff >= -10)
		{
			return "C";
		}
		return "D";
	}

	public int VenueCapacity(Sport sport, Level level, int venueId)
	{
		bool second = SportRules.IsSecondDivision(sport, level);
		int seats = sport switch
		{
			Sport.Hockey => level switch
			{
				Level.Junior => 4500, 
				Level.College => 6000, 
				Level.Minor => 7500, 
				Level.Pro => 18000, 
				_ => 1000, 
			}, 
			Sport.Basketball => level switch
			{
				Level.College => 11000, 
				Level.Minor => 4000, 
				Level.Pro => 19000, 
				_ => 1800, 
			}, 
			Sport.Football => level switch
			{
				Level.College => 55000, 
				Level.Pro => 68000, 
				_ => 7000, 
			}, 
			Sport.Baseball => level switch
			{
				Level.College => 3500, 
				Level.Minor => 9000, 
				Level.Pro => 41000, 
				_ => 800, 
			}, 
			_ => second ? 9000 : (level switch
			{
				Level.College => 3000, 
				Level.Pro => 22000, 
				_ => 1200, 
			}), 
		};
		if (venueId >= 0)
		{
			Venue venue = GetVenue(venueId);
			if (venue != null && venue.Capacity > 0)
			{
				return Math.Max(500, Math.Min(150000, venue.Capacity));
			}
			seats = (int)Math.Round((double)seats * (0.9 + (double)(Hash((int)(venueId * 31 + sport)) % 21) / 100.0) / 10.0) * 10;
		}
		return seats;
	}

	private static double UsualFill(Sport sport, Level level)
	{
		if (SportRules.IsSecondDivision(sport, level))
		{
			return 0.6;
		}
		switch (level)
		{
		case Level.Pro:
			return sport switch
			{
				Sport.Soccer => 0.82, 
				Sport.Basketball => 0.9, 
				Sport.Hockey => 0.93, 
				Sport.Football => 0.95, 
				_ => 0.65, 
			};
		case Level.Minor:
			return 0.62;
		case Level.College:
			return sport switch
			{
				Sport.Basketball => 0.72, 
				Sport.Football => 0.82, 
				_ => 0.5, 
			};
		case Level.Junior:
			return 0.6;
		default:
			if (sport != Sport.Football)
			{
				return 0.45;
			}
			return 0.65;
		}
	}

	public void EnsureTicketPrices(Team team)
	{
		if (team == null) return;
		if (team.TicketPriceResidents <= 0)
		{
			team.TicketPriceResidents = team.Level switch
			{
				Level.Pro => ((team.Sport == Sport.Soccer) ? 30 : 25),
				Level.Minor => 14,
				Level.College => 10,
				Level.Junior => 5,
				_ => 3,
			};
		}
		if (team.TicketPriceVisitors <= 0)
		{
			team.TicketPriceVisitors = Math.Min(1500, (int)Math.Round((double)team.TicketPriceResidents * 1.5));
		}
	}

	private int Attendance(League league, Match g)
	{
		Team home = GetTeam(g.HomeTeamId);
		Team away = GetTeam(g.AwayTeamId);
		if (home == null)
		{
			return 0;
		}
		EnsureTicketPrices(home);
		int seats = VenueCapacity(league.Sport, league.Level, g.VenueId);
		Random r = new Random((int)Hash(g.Id * 7 + 3));
		double fill = UsualFill(league.Sport, league.Level);
		fill += (double)(home.Rating - 50) * 0.006 + (double)home.Form / 1000.0 * 0.08;
		if (away != null)
		{
			fill += (double)(away.Rating - 50) * 0.002;
		}
		if (league.Level == Level.College)
		{
			fill += (double)(home.Prestige - 50) * 0.003;
		}
		Rivalry rv = FindRivalry(league, g.HomeTeamId, g.AwayTeamId, create: false);
		if (rv != null && rv.Heat >= 50)
		{
			fill += 0.08;
		}
		if (g.Stage != GameStage.Regular)
		{
			fill += 0.2;
		}
		DayOfWeek day = GameClock.Weekday(GameClock.DayOf(g.Time));
		bool weekend = day == DayOfWeek.Friday || day == DayOfWeek.Saturday || day == DayOfWeek.Sunday;
		fill += ((!weekend) ? 0.0 : ((league.Sport == Sport.Baseball) ? 0.12 : 0.04));
		if (g.Spotlight)
		{
			fill += 0.06;
		}
		else if (g.Featured)
		{
			fill += 0.03;
		}
		if (g.Weather >= 0)
		{
			WeatherKind k = WeatherCodes.Kind(g.Weather);
			int num = WeatherCodes.TempF(g.Weather);
			switch (k)
			{
			case WeatherKind.HeavyRain:
			case WeatherKind.Snow:
			case WeatherKind.Storm:
				fill -= ((league.Sport == Sport.Football) ? 0.04 : 0.14);
				break;
			case WeatherKind.Rain:
				fill -= ((league.Sport == Sport.Football) ? 0.02 : 0.07);
				break;
			}
			if (num < 40 && league.Sport == Sport.Baseball)
			{
				fill -= 0.06;
			}
		}
		double referencePrice = league.Level switch
		{
			Level.Pro => ((league.Sport == Sport.Soccer) ? 30.0 : 25.0),
			Level.Minor => 14.0,
			Level.College => 10.0,
			Level.Junior => 5.0,
			_ => 3.0,
		};
		List<Player> homeRoster = RosterOf(home.Id);
		int homeOverall = (homeRoster == null || homeRoster.Count == 0) ? home.Rating : Rosters.Display(home.Level, Rosters.Strength(home.Sport, homeRoster));
		referencePrice *= 0.75 + (double)homeOverall / 200.0;
		Venue venue = GetVenue(g.VenueId);
		if (venue != null)
		{
			referencePrice *= venue.Type switch
			{
				VenueType.Stadium => 1.12,
				VenueType.Ballpark => 1.10,
				VenueType.IndoorArena => 1.06,
				VenueType.IceRink => 1.0,
				VenueType.Field => 0.88,
				_ => 0.84,
			};
			referencePrice *= venue.Level switch
			{
				Level.Pro => 1.08,
				Level.Minor => 1.0,
				Level.College => 0.95,
				Level.Junior => 0.88,
				_ => 0.82,
			};
		}
		double touristShare = Math.Max(0.0, Math.Min(1.0, (double)VisitorAttendancePercent / 100.0));
		double effectivePrice = (double)home.TicketPriceResidents * (1.0 - touristShare) + (double)home.TicketPriceVisitors * touristShare;
		double priceFactor = Math.Pow(referencePrice / (double)Math.Max(1, (int)Math.Round(effectivePrice)), 0.28);
		fill *= Math.Max(0.35, Math.Min(1.25, priceFactor));
		fill += Rosters.Normal(r, 0.0, 0.04);
		if (fill >= 0.99)
		{
			return seats;
		}
		return (int)Math.Round((double)seats * Math.Max(0.12, fill));
	}

	public void HomeCrowds(League league, Team t, out int games, out int average, out int sellouts, out int capacity)
	{
		games = 0;
		average = 0;
		sellouts = 0;
		capacity = VenueCapacity(t.Sport, t.Level, t.VenueId);
		long total = 0L;
		Season[] array = new Season[2] { league.Current, league.Previous };
		foreach (Season s in array)
		{
			if (s == null)
			{
				continue;
			}
			foreach (Match g in s.Games)
			{
				if (g.Played && g.HomeTeamId == t.Id && g.Attendance > 0)
				{
					games++;
					total += g.Attendance;
					if (g.Attendance >= VenueCapacity(t.Sport, t.Level, g.VenueId))
					{
						sellouts++;
					}
				}
			}
			if (games > 0)
			{
				break;
			}
		}
		average = (int)((games > 0) ? (total / games) : 0);
	}

	public League LeagueOfGame(Match g)
	{
		foreach (League l in Leagues)
		{
			Season[] array = new Season[2] { l.Current, l.Previous };
			foreach (Season s in array)
			{
				if (s != null && s.Games.Contains(g))
				{
					return l;
				}
			}
		}
		return null;
	}

	public void CityClockWindow(Match g, int now, out int start, out int end)
	{
		if (g.Live != null)
		{
			start = g.Live.Start;
			end = g.Live.End;
		}
		else
		{
			League l = LeagueOfGame(g);
			start = GameClock.ToTime(GameClock.MonthStartDay(now), GameClock.MinuteOfDay(g.Time)) + g.RainDelay;
			end = start + ((l == null) ? 180 : PlannedMinutes(l.Sport, l.Level));
		}
	}

	public string AttendanceText(League league, Match g)
	{
		if (g.Attendance <= 0)
		{
			return "";
		}
		bool full = g.Attendance >= VenueCapacity(league.Sport, league.Level, g.VenueId);
		return "Посещаемость " + g.Attendance.ToString("N0", CultureInfo.InvariantCulture) + (full ? " (аншлаг)" : "");
	}

	public SportsWorld()
		: this(new Random())
	{
	}

	public SportsWorld(Random random)
	{
		_random = random;
		_names = new NameGenerator(random);
		_simulator = new GameSimulator(random);
	}

	public void Clear()
	{
		NextId = 1;
		LastTime = int.MinValue;
		Orgs.Clear();
		Teams.Clear();
		Venues.Clear();
		Leagues.Clear();
		Players.Clear();
		Feed.Clear();
		Coaches.Clear();
		Talks.Clear();
		Offers.Clear();
		CityWeather.Clear();
		Tournaments.Clear();
		CountryName = "";
		CountryAbbrev = "";
		CitySampleTime = int.MinValue;
		RecruitBoard.Clear();
		RecruitEfforts.Clear();
		Districts.Clear();
		_localMonth = int.MinValue;
		Retired.Clear();
		_exPlayersYear = int.MinValue;
		_boxPruneDay = int.MinValue;
		StaffChanged();
		_nameUse = null;
		_clockHint = int.MinValue;
		RostersChanged();
		Revision++;
	}

	public Team GetTeam(int id)
	{
		if (!Teams.TryGetValue(id, out var t))
		{
			return null;
		}
		return t;
	}

	public Organization GetOrg(int id)
	{
		if (!Orgs.TryGetValue(id, out var o))
		{
			return null;
		}
		return o;
	}

	public Venue GetVenue(int id)
	{
		if (!Venues.TryGetValue(id, out var v))
		{
			return null;
		}
		return v;
	}

	public League GetLeague(Sport sport, Level level)
	{
		foreach (League l in Leagues)
		{
			if (l.Sport == sport && l.Level == level)
			{
				return l;
			}
		}
		return null;
	}

	public League LeagueOf(Team team)
	{
		if (team != null)
		{
			return GetLeague(team.Sport, team.Level);
		}
		return null;
	}

	public string TeamName(int teamId)
	{
		Team t = GetTeam(teamId);
		Organization o = ((t == null) ? null : GetOrg(t.OrgId));
		if (o != null)
		{
			if (o.Level == Level.HighSchool || o.Level == Level.College || string.IsNullOrEmpty(o.Mascot)) return o.Name;
			return o.Mascot;
		}
		return "Не определено";
	}

	public string TeamFullName(int teamId)
	{
		Team t = GetTeam(teamId);
		Organization o = ((t == null) ? null : GetOrg(t.OrgId));
		return (o == null) ? "Не определено" : o.FullName;
	}

	public string TeamShortName(int teamId)
	{
		Team t = GetTeam(teamId);
		Organization o = ((t == null) ? null : GetOrg(t.OrgId));
		if (o == null)
		{
			return "Не определено";
		}
		if (o.Level == Level.HighSchool || o.Level == Level.College || string.IsNullOrEmpty(o.Mascot)) return o.Name;
		return o.Mascot;
	}

	public string TeamAbbrev(int teamId)
	{
		Team t = GetTeam(teamId);
		Organization o = ((t == null) ? null : GetOrg(t.OrgId));
		if (o != null)
		{
			return NameGenerator.Abbreviation(o.Name);
		}
		return "Не определено";
	}

	public List<Team> TeamsAtVenue(int venueId)
	{
		List<Team> list = new List<Team>();
		foreach (Team t in Teams.Values)
		{
			if (!t.Retired && t.IsUser && t.VenueId == venueId)
			{
				list.Add(t);
			}
		}
		list.Sort((Team a, Team b) => (a.Sport == b.Sport) ? a.Id.CompareTo(b.Id) : a.Sport.CompareTo(b.Sport));
		return list;
	}

	public List<Team> UserTeams()
	{
		List<Team> list = new List<Team>();
		foreach (Team t in Teams.Values)
		{
			if (!t.Retired && t.IsUser)
			{
				list.Add(t);
			}
		}
		list.Sort((Team a, Team b) =>
		{
			if (a.Level != b.Level)
			{
				return SportRules.Rank(b.Level).CompareTo(SportRules.Rank(a.Level));
			}
			return (a.Sport != b.Sport) ? a.Sport.CompareTo(b.Sport) : a.Id.CompareTo(b.Id);
		});
		return list;
	}

	public bool IsPending(Team team)
	{
		return LeagueOf(team)?.PendingTeamIds.Contains(team.Id) ?? false;
	}

	public Venue CreateVenue(VenueType type)
	{
		Venue v = new Venue
		{
			Id = NextId++,
			Type = type,
			Level = SportRules.DefaultLevel(type)
		};
		Venues[v.Id] = v;
		Revision++;
		return v;
	}

	public CommandResult SetVenueType(int venueId, VenueType type)
	{
		Venue v = GetVenue(venueId);
		if (v == null)
		{
			return CommandResult.Fail("Это здание не является спортивным объектом.");
		}
		List<Team> here = TeamsAtVenue(venueId);
		foreach (Team t in here)
		{
			if (!SportRules.IsAllowed(type, t.Sport))
			{
				return CommandResult.Fail("Сначала удалите команду по виду спорта «" + SportRules.SportLabel(t.Sport) + "»: объект типа «" + SportRules.VenueTypeLabel(type) + "» не подходит для этого вида спорта.");
			}
		}
		if (v.Type != type && (here.Count > SportRules.MaxTeams(type) || VenueLoad(venueId) > SportRules.HomeDateCapacity(type)))
		{
			return CommandResult.Fail("Для объекта типа «" + SportRules.VenueTypeLabel(type) + "» недостаточно свободных дат для этих " + here.Count + " команд (" + VenueLoad(venueId) + " домашних матчей, доступно " + SportRules.HomeDateCapacity(type) + "). Сначала переместите или удалите одну из команд.");
		}
		v.Type = type;
		Revision++;
		return CommandResult.Success("Тип спортивного объекта изменён на " + SportRules.VenueTypeLabel(type) + ".");
	}

	public CommandResult SetVenueCapacity(int venueId, int capacity)
	{
		Venue venue = GetVenue(venueId);
		if (venue == null)
		{
			return CommandResult.Fail("Это здание не является спортивным объектом.");
		}
		if (capacity != 0 && (capacity < 500 || capacity > 150000))
		{
			return CommandResult.Fail("Вместимость должна быть от 500 до 150 000 мест или 0 для автоматического расчёта.");
		}
		venue.Capacity = Math.Max(0, capacity);
		Revision++;
		return CommandResult.Success(capacity == 0 ? "Вместимость снова рассчитывается автоматически." : ("Вместимость арены установлена: " + capacity.ToString("N0") + " мест."));
	}

	public CommandResult SetVenueLevel(int venueId, Level level)
	{
		Venue v = GetVenue(venueId);
		if (v == null)
		{
			return CommandResult.Fail("Это здание не является спортивным объектом.");
		}
		if (v.Level == level)
		{
			return CommandResult.Success("");
		}
		if (!Array.Exists(SportRules.VenueLevels, (Level x) => x == level))
		{
			return CommandResult.Fail("Этот уровень недоступен.");
		}
		v.Level = level;
		Revision++;
		return CommandResult.Success("Новые команды здесь будут относиться к уровню «" + SportRules.LevelLabel(level) + "».");
	}

	public CommandResult MoveTeam(int teamId, int venueId, string venueName, int now)
	{
		_clockHint = now;
		Team t = GetTeam(teamId);
		Venue v = GetVenue(venueId);
		if (t == null || t.Retired || !t.IsUser)
		{
			return CommandResult.Fail("Команда не найдена.");
		}
		if (v == null)
		{
			return CommandResult.Fail("Сначала сделайте это здание спортивным объектом.");
		}
		if (t.VenueId == venueId)
		{
			return CommandResult.Fail(TeamName(teamId) + " уже играют здесь.");
		}
		if (!CanHost(v, t.Sport, t.Level, -1, out var reason))
		{
			return CommandResult.Fail(reason);
		}
		int old = t.VenueId;
		t.VenueId = venueId;
		League league = LeagueOf(t);
		if (league != null && league.Current != null)
		{
			foreach (Match g in league.Current.Games)
			{
				if (!g.Played && g.HomeTeamId == t.Id && g.VenueId == old)
				{
					g.VenueId = venueId;
				}
			}
			ReassignVenues(league);
		}
		string where = (string.IsNullOrEmpty(venueName) ? "новый спортивный объект" : NameGenerator.CleanName(venueName, 60));
		Post(league, NewsKind.General, Wire, "ПЕРЕЕЗД: " + TeamName(t.Id) + " переносят домашние матчи на арену " + where + ".", isUser: true, now, 2);
		Revision++;
		return CommandResult.Success(TeamName(t.Id) + " теперь проводят здесь домашние матчи.");
	}

	public static int HomeDates(Sport sport, Level level)
	{
		return (SportProfiles.Get(sport, level).GamesPerTeam + 1) / 2;
	}

	public int VenueLoad(int venueId)
	{
		int load = 0;
		foreach (Team t in TeamsAtVenue(venueId))
		{
			load += HomeDates(t.Sport, t.Level);
		}
		return load;
	}

	public bool CanHost(Venue v, Sport sport, Level level, int ignoreTeamId, out string reason)
	{
		reason = "";
		if (!SportRules.IsAllowed(v.Type, sport))
		{
			reason = SportRules.VenueTypeLabel(v.Type) + " не может принимать " + SportRules.SportLabel(sport) + ".";
			return false;
		}
		if (!SportRules.LevelAllowed(sport, level))
		{
			reason = SportRules.LevelLabel(level) + " " + SportRules.SportLabel(sport) + " не поддерживается: юниорские лиги доступны только для хоккея, малые — для хоккея, баскетбола и бейсбола.";
			return false;
		}
		int sameSportPro = 0;
		int count = 0;
		int load = 0;
		foreach (Team t in TeamsAtVenue(v.Id))
		{
			if (t.Id != ignoreTeamId)
			{
				count++;
				load += HomeDates(t.Sport, t.Level);
				if (t.Sport == sport && t.Level == Level.Pro)
				{
					sameSportPro++;
				}
			}
		}
		if (count >= SportRules.MaxTeams(v.Type))
		{
			reason = "Для объекта «" + SportRules.VenueTypeLabel(v.Type) + "» достигнут лимит: он может быть домашней ареной не более чем для " + SportRules.MaxTeams(v.Type) + " команд.";
			return false;
		}
		if (level == Level.Pro && sameSportPro >= 2)
		{
			reason = "Здесь уже есть " + sameSportPro + " профессиональных команд по виду спорта " + SportRules.SportLabel(sport) + " — это максимальное количество команд для одного объекта.";
			return false;
		}
		int need = HomeDates(sport, level);
		int capacity = SportRules.HomeDateCapacity(v.Type);
		if (load + need > capacity)
		{
			reason = "Недостаточно свободных дат: команде " + SportRules.LevelLabel(level) + " " + SportRules.SportLabel(sport) + " требуется " + need + " домашних дат за сезон, доступно только " + Math.Max(0, capacity - load) + " из " + capacity + ".";
			return false;
		}
		return true;
	}

	public CommandResult RemoveVenue(int venueId, int now)
	{
		if (GetVenue(venueId) == null)
		{
			return CommandResult.Fail("Это здание не является спортивным объектом.");
		}
		foreach (Team t in TeamsAtVenue(venueId))
		{
			RemoveTeam(t.Id, now);
		}
		Venues.Remove(venueId);
		Revision++;
		return CommandResult.Success("Спортивный объект удалён.");
	}

	public CommandResult AddTeam(int venueId, Sport sport, NameContext ctx, int now)
	{
		Venue v = GetVenue(venueId);
		if (v == null)
		{
			return CommandResult.Fail("Сначала сделайте это здание спортивным объектом.");
		}
		if (!CanHost(v, sport, v.Level, -1, out var reason))
		{
			return CommandResult.Fail(reason);
		}
		if (ctx == null)
		{
			ctx = new NameContext();
		}
		NoteDistrict(ctx.DistrictName);
		Organization org = FindOrCreateUserOrg(v, sport, ctx);
		Team team = new Team
		{
			Id = NextId++,
			OrgId = org.Id,
			Sport = sport,
			Level = v.Level,
			VenueId = v.Id,
			Rating = _random.Next(45, 66),
			IsUser = true
		};
		Teams[team.Id] = team;
		_clockHint = now;
		CreateRoster(team, team.Rating);
		League league = GetOrCreateLeague(sport, v.Level);
		string message = JoinLeague(league, team, now);
		Revision++;
		return CommandResult.Success("Создана команда " + org.FullName + " (" + SportRules.LevelLabel(v.Level) + " " + SportRules.SportLabel(sport) + "). " + message);
	}

	public CommandResult RemoveTeam(int teamId, int now)
	{
		_clockHint = now;
		Team team = GetTeam(teamId);
		if (team == null || team.Retired || !team.IsUser)
		{
			return CommandResult.Fail("Команда не найдена.");
		}
		string text = TeamName(teamId);
		League league = LeagueOf(team);
		if (league != null)
		{
			league.PendingTeamIds.Remove(teamId);
			if (league.TeamIds.Contains(teamId))
			{
				Season s = league.Current;
				if (s != null && s.AnyPlayed)
				{
					Team replacement = CreateAiTeam(league, team.Rating);
					ReplaceTeamInSeason(league, s, teamId, replacement.Id);
					league.TeamIds[league.TeamIds.IndexOf(teamId)] = replacement.Id;
					team.Retired = true;
					if (!KeepsRunning(league))
					{
						AbandonSeason(league);
					}
					else
					{
						ReassignVenues(league);
					}
				}
				else
				{
					league.TeamIds.Remove(teamId);
					team.Retired = true;
					if (!KeepsRunning(league))
					{
						AbandonSeason(league);
					}
					else if (s != null)
					{
						BuildSeason(league, now);
					}
				}
			}
			else
			{
				team.Retired = true;
			}
		}
		else
		{
			team.Retired = true;
		}
		StopUnneededFarmLeagues();
		CleanUpUnused();
		Revision++;
		return CommandResult.Success(text + " удалена.");
	}

	public CommandResult RenameOrg(int orgId, string name, string mascot)
	{
		Organization o = GetOrg(orgId);
		if (o == null || !o.IsUser)
		{
			return CommandResult.Fail("Команда не найдена.");
		}
		string n = NameGenerator.CleanName(name, 40);
		string m = NameGenerator.CleanName(mascot, 30);
		if (n.Length == 0)
		{
			return CommandResult.Fail("Название не может быть пустым.");
		}
		o.Name = n;
		o.Mascot = m;
		Revision++;
		return CommandResult.Success("Новое название: " + o.FullName + ".");
	}

	public CommandResult RerollOrg(int orgId, NameContext ctx)
	{
		Organization o = GetOrg(orgId);
		if (o == null || !o.IsUser)
		{
			return CommandResult.Fail("Команда не найдена.");
		}
		if (ctx == null)
		{
			ctx = new NameContext();
		}
		List<string> takenNames = OrgNames(o.Level, o.Id);
		List<string> takenMascots = OrgMascots(o.Level, o.Id);
		takenMascots.Add(o.Mascot);
		Sport sport = FirstSportOf(o.Id);
		switch (o.Level)
		{
		case Level.HighSchool:
			if (string.IsNullOrEmpty(o.DistrictKey) || ctx.DistrictName.Length == 0)
			{
				takenNames.Add(o.Name);
				o.Name = _names.HighSchoolName(ctx.DistrictName, takenNames);
			}
			o.Mascot = _names.PickSchoolMascot(takenMascots);
			break;
		case Level.College:
			takenNames.Add(o.Name);
			o.Name = _names.CollegeName(PlaceFor(ctx), takenNames);
			o.Mascot = _names.PickSchoolMascot(takenMascots);
			break;
		case Level.Junior:
			if (ctx.CityName.Length > 0)
			{
				o.Name = NameGenerator.CleanName(ctx.CityName, 40);
			}
			o.Mascot = "Юниоры " + _names.PickMascot(sport, Level.Junior, takenMascots);
			break;
		case Level.Minor:
			if (UsefulDistrictName(ctx.DistrictName))
			{
				o.Name = NameGenerator.CleanName(ctx.DistrictName, 40);
			}
			else if (ctx.CityName.Length > 0)
			{
				o.Name = NameGenerator.CleanName(ctx.CityName, 40);
			}
			o.Mascot = _names.PickMascot(sport, Level.Pro, takenMascots, o.Name);
			break;
		default:
			if (ctx.CityName.Length > 0)
			{
				o.Name = NameGenerator.CleanName(ctx.CityName, 40);
			}
			o.Mascot = _names.PickMascot(sport, Level.Pro, takenMascots, o.Name);
			break;
		}
		Revision++;
		return CommandResult.Success("Новое название: " + o.FullName + ".");
	}

	private string JoinLeague(League league, Team team, int now)
	{
		Season s = league.Current;
		if (s == null)
		{
			league.TeamIds.Add(team.Id);
			BuildSeason(league, now);
			return SeasonStartMessage(league, team.Id);
		}
		if (!s.AnyPlayed)
		{
			league.TeamIds.Add(team.Id);
			RemoveExtraAiTeams(league);
			BuildSeason(league, now);
			return SeasonStartMessage(league, team.Id);
		}
		if (!s.PlayoffsCreated)
		{
			int aiId = PickAiToReplace(league);
			if (aiId >= 0)
			{
				ReplaceTeamInSeason(league, s, aiId, team.Id);
				league.TeamIds[league.TeamIds.IndexOf(aiId)] = team.Id;
				Team ai = GetTeam(aiId);
				if (ai != null)
				{
					ai.Retired = true;
				}
				PrunePlayers();
				ReassignVenues(league);
				return "Присоединилась к текущему сезону, заняв свободное место.";
			}
		}
		league.PendingTeamIds.Add(team.Id);
		return "Сезон уже начался, поэтому команда присоединится в следующем сезоне.";
	}

	private string SeasonStartMessage(League league, int teamId)
	{
		Season s = league.Current;
		if (s == null)
		{
			return "";
		}
		int first = int.MaxValue;
		foreach (Match g in s.Games)
		{
			if (g.Involves(teamId))
			{
				first = Math.Min(first, g.Time);
			}
		}
		if (first == int.MaxValue)
		{
			return "";
		}
		return "Первый матч: " + GameClock.DayLabel(GameClock.DayOf(first)) + ", " + GameClock.ClockLabel(GameClock.MinuteOfDay(first)) + ".";
	}

	private void MergePending(League league)
	{
		if (league.PendingTeamIds.Count == 0)
		{
			return;
		}
		foreach (int id in league.PendingTeamIds)
		{
			if (!league.TeamIds.Contains(id))
			{
				league.TeamIds.Add(id);
			}
		}
		league.PendingTeamIds.Clear();
		RemoveExtraAiTeams(league);
	}

	private int TargetSize(League league)
	{
		int users = CountUserTeams(league);
		int configured = (league.Sport == Sport.Soccer) ? Settings.SoccerLeagueSize : Settings.LeagueSize;
		int target = Math.Max(Math.Max((league.Sport == Sport.Soccer) ? 16 : 4, Math.Min(20, configured)), users);
		if (target % 2 == 1)
		{
			target++;
		}
		return target;
	}

	private void FillWithAiTeams(League league)
	{
		RemoveExtraAiTeams(league, fold: true);
		int target = TargetSize(league);
		bool established = league.History.Count > 0 || league.Previous != null;
		List<string> joined = new List<string>();
		while (league.TeamIds.Count < target)
		{
			int rating = (SportRules.IsSecondDivision(league.Sport, league.Level) ? Rosters.Clamp((int)Math.Round(Rosters.Normal(_random, 59.0, 5.0)), 45, 72) : NewAiRating(league.Level));
			Team ai = CreateAiTeam(league, rating);
			league.TeamIds.Add(ai.Id);
			joined.Add(TeamName(ai.Id));
		}
		if (established && joined.Count > 0)
		{
			Post(league, NewsKind.General, Wire, "РАСШИРЕНИЕ: " + ((joined.Count == 1) ? ("" + joined[0] + " присоединяются") : (joined.Count + " новых команд присоединяются")) + " " + Views.LeagueTitle(league) + " лига" + ((joined.Count == 1) ? "" : (" — " + string.Join(", ", joined.ToArray()))) + ".", isUser: false, NowHint(), 2);
		}
	}

	private int NowHint()
	{
		if (_clockHint == int.MinValue)
		{
			if (LastTime == int.MinValue)
			{
				return 0;
			}
			return LastTime;
		}
		return _clockHint;
	}

	public int PlannedSize(League league)
	{
		return TargetSize(league);
	}

	private int NewAiRating(Level level)
	{
		return level switch
		{
			Level.Pro => Rosters.Clamp((int)Math.Round(Rosters.Normal(_random, 50.0, 6.0)), 36, 66), 
			Level.College => Rosters.Clamp((int)Math.Round(Rosters.Normal(_random, 52.0, 8.0)), 32, 74), 
			Level.Junior => Rosters.Clamp((int)Math.Round(Rosters.Normal(_random, 51.0, 8.0)), 32, 74), 
			Level.Minor => Rosters.Clamp((int)Math.Round(Rosters.Normal(_random, 50.0, 5.0)), 38, 62), 
			_ => Rosters.Clamp((int)Math.Round(Rosters.Normal(_random, 52.0, 10.0)), 30, 80), 
		};
	}

	private void RemoveExtraAiTeams(League league, bool fold = false)
	{
		int target = TargetSize(league);
		for (int i = league.TeamIds.Count - 1; i >= 0; i--)
		{
			Team t = GetTeam(league.TeamIds[i]);
			if (t == null || t.Retired)
			{
				league.TeamIds.RemoveAt(i);
			}
		}
		bool established = league.History.Count > 0 || league.Previous != null;
		while (league.TeamIds.Count > target)
		{
			int aiId = PickAiToReplace(league);
			if (aiId < 0)
			{
				break;
			}
			league.TeamIds.Remove(aiId);
			Team ai = GetTeam(aiId);
			if (ai != null)
			{
				if (fold & established)
				{
					FoldTeam(league, ai);
				}
				else
				{
					ai.Retired = true;
				}
			}
		}
	}

	private void FoldTeam(League league, Team t)
	{
		int now = NowHint();
		int year = CurrentYear();
		int month = GameClock.MonthIndex(now);
		t.Retired = true;
		t.FoldedYear = year;
		int retired = 0;
		int freeAgents = 0;
		int secondTier = 0;
		foreach (Player p in new List<Player>(SignedPlayers(t.Id)))
		{
			int age = year - p.BirthYear;
			if (SportRules.ProLike(league.Sport, league.Level))
			{
				int baseOvr = ((league.Level == Level.Pro) ? Rosters.Base(Level.Pro) : SecondDivisionOverall);
				if (age >= 34 || (age >= 31 && p.Overall < baseOvr - 6))
				{
					if (p.Seasons > 0)
					{
						CareerOver(p, league.Level, year - 1, now);
					}
					KeepForCoaching(p, age, year - 1);
					RemovePlayer(p);
					retired++;
				}
				else if (age <= 24 && p.Overall < baseOvr - 6 && ToSecondTier(p))
				{
					secondTier++;
				}
				else
				{
					BecomeFreeAgent(p, month);
					freeAgents++;
				}
			}
			else if (league.Level == Level.Minor)
			{
				if (age < 31 && ToSecondTier(p))
				{
					secondTier++;
					continue;
				}
				RemovePlayer(p);
				retired++;
			}
			else
			{
				RemovePlayer(p);
			}
		}
		foreach (PickAsset a in league.Picks)
		{
			if (a.OwnerTeamId == t.Id)
			{
				a.OwnerTeamId = a.OriginalTeamId;
			}
		}
		league.Picks.RemoveAll((PickAsset pickAsset) => pickAsset.OriginalTeamId == t.Id);
		foreach (Coach item in StaffOf(t.Id))
		{
			item.TeamId = -1;
		}
		StaffChanged();
		RostersChanged();
		string where = (SportRules.ProLike(league.Sport, league.Level) ? (" " + freeAgents + " игроков стали свободными агентами" + ((retired > 0) ? (", " + retired + " завершили карьеру") : "") + ((secondTier > 0) ? (", " + secondTier + " перешёл в " + SecondTierName(league.Sport)) : "") + ".") : "");
		Post(league, NewsKind.General, Wire, "РАСФОРМИРОВАНА: команда «" + TeamName(t.Id) + "» прекратила существование после " + ((league.History.Count > 0) ? ("сезона " + league.History[0].Season) : "завершения сезона") + "." + where, isUser: false, now, 2);
	}

	private int PickAiToReplace(League league)
	{
		int best = -1;
		int bestRating = int.MaxValue;
		foreach (int id in league.TeamIds)
		{
			Team t = GetTeam(id);
			if (t != null && !t.IsUser && !t.Retired && t.Rating < bestRating)
			{
				best = id;
				bestRating = t.Rating;
			}
		}
		return best;
	}

	private Team CreateAiTeam(League league, int rating)
	{
		List<string> takenNames = OrgNames(league.Level, -1);
		List<string> takenMascots = OrgMascots(league.Level, -1);
		Organization org = new Organization
		{
			Id = NextId++,
			Level = league.Level,
			IsUser = false
		};
		switch (league.Level)
		{
		case Level.HighSchool:
			org.Name = _names.HighSchoolName("", takenNames);
			org.Mascot = _names.PickSchoolMascot(takenMascots);
			break;
		case Level.College:
			org.Name = _names.CollegeName(_names.PickPlace(PlacesInUse()), takenNames);
			org.Mascot = _names.PickSchoolMascot(takenMascots);
			break;
		default:
			org.Name = _names.PickPlace(PlacesInUse());
			org.Mascot = _names.PickMascot(league.Sport, Level.Pro, takenMascots, org.Name);
			break;
		}
		Orgs[org.Id] = org;
		Team team = new Team
		{
			Id = NextId++,
			OrgId = org.Id,
			Sport = league.Sport,
			Level = league.Level,
			VenueId = -1,
			Rating = rating,
			IsUser = false
		};
		Teams[team.Id] = team;
		CreateRoster(team, rating);
		return team;
	}

	private void ReplaceTeamInSeason(League league, Season s, int oldId, int newId)
	{
		foreach (Match g in s.Games)
		{
			if (g.HomeTeamId == oldId)
			{
				g.HomeTeamId = newId;
			}
			if (g.AwayTeamId == oldId)
			{
				g.AwayTeamId = newId;
			}
		}
		foreach (PlayoffSeries ps in s.Bracket)
		{
			if (ps.HighTeamId == oldId)
			{
				ps.HighTeamId = newId;
			}
			if (ps.LowTeamId == oldId)
			{
				ps.LowTeamId = newId;
			}
			if (ps.WinnerId == oldId)
			{
				ps.WinnerId = newId;
			}
		}
		Revision++;
	}

	private League GetOrCreateLeague(Sport sport, Level level)
	{
		League league = GetLeague(sport, level);
		if (league != null)
		{
			return league;
		}
		league = new League
		{
			Sport = sport,
			Level = level
		};
		Leagues.Add(league);
		Leagues.Sort((League a, League b) => (a.Sport == b.Sport) ? SportRules.Rank(b.Level).CompareTo(SportRules.Rank(a.Level)) : a.Sport.CompareTo(b.Sport));
		return league;
	}

	public List<League> OrderedLeagues()
	{
		List<League> list = new List<League>(Leagues);
		list.Sort((League a, League b) => (a.Sport == b.Sport) ? SportRules.Rank(b.Level).CompareTo(SportRules.Rank(a.Level)) : a.Sport.CompareTo(b.Sport));
		return list;
	}

	public int CountUserTeams(League league)
	{
		int count = 0;
		foreach (int id in league.TeamIds)
		{
			Team t = GetTeam(id);
			if (t != null && t.IsUser && !t.Retired)
			{
				count++;
			}
		}
		return count;
	}

	private Organization FindOrCreateUserOrg(Venue venue, Sport sport, NameContext ctx)
	{
		if (venue.Level == Level.HighSchool || venue.Level == Level.College)
		{
			foreach (Team t in TeamsAtVenue(venue.Id))
			{
				Organization o = GetOrg(t.OrgId);
				if (o != null && o.Level == venue.Level && !OrgHasSport(o.Id, sport))
				{
					return o;
				}
			}
			if (!string.IsNullOrEmpty(ctx.DistrictKey))
			{
				foreach (Organization o2 in Orgs.Values)
				{
					if (o2.IsUser && o2.Level == venue.Level && o2.DistrictKey == ctx.DistrictKey && OrgHasActiveTeams(o2.Id) && !OrgHasSport(o2.Id, sport))
					{
						return o2;
					}
				}
			}
		}
		List<string> takenNames = OrgNames(venue.Level, -1);
		List<string> takenMascots = OrgMascots(venue.Level, -1);
		Organization org = new Organization
		{
			Id = NextId++,
			Level = venue.Level,
			IsUser = true,
			DistrictKey = (ctx.DistrictKey ?? "")
		};
		switch (venue.Level)
		{
		case Level.HighSchool:
			org.Name = _names.HighSchoolName(ctx.DistrictName, takenNames);
			org.Mascot = _names.PickSchoolMascot(takenMascots);
			break;
		case Level.College:
			org.Name = _names.CollegeName(PlaceFor(ctx), takenNames);
			org.Mascot = _names.PickSchoolMascot(takenMascots);
			break;
		case Level.Junior:
			org.Name = ((ctx.CityName.Length > 0) ? NameGenerator.CleanName(ctx.CityName, 40) : "Город");
			org.Mascot = "Юниоры " + _names.PickMascot(sport, Level.Junior, takenMascots);
			break;
		case Level.Minor:
		{
			ref string name = ref org.Name;
			string text;
			if (UsefulDistrictName(ctx.DistrictName))
			{
				text = NameGenerator.CleanName(ctx.DistrictName, 40);
			}
			else
			{
				text = ((ctx.CityName.Length > 0) ? NameGenerator.CleanName(ctx.CityName, 40) : "Город");
			}
			name = text;
			org.Mascot = _names.PickMascot(sport, Level.Pro, takenMascots, org.Name);
			break;
		}
		default:
			org.Name = ((ctx.CityName.Length > 0) ? NameGenerator.CleanName(ctx.CityName, 40) : "Город");
			org.Mascot = _names.PickMascot(sport, Level.Pro, takenMascots, org.Name);
			break;
		}
		Orgs[org.Id] = org;
		return org;
	}

	private string PlaceFor(NameContext ctx)
	{
		if (ctx.DistrictName.Length > 0 && !ctx.DistrictName.StartsWith("District", StringComparison.OrdinalIgnoreCase) && (ctx.CityName.Length == 0 || _random.Next(2) == 0))
		{
			return ctx.DistrictName;
		}
		if (ctx.CityName.Length <= 0)
		{
			return ctx.DistrictName;
		}
		return ctx.CityName;
	}

	private bool OrgHasSport(int orgId, Sport sport)
	{
		foreach (Team t in Teams.Values)
		{
			if (t.OrgId == orgId && t.Sport == sport && !t.Retired)
			{
				return true;
			}
		}
		return false;
	}

	private bool OrgHasActiveTeams(int orgId)
	{
		foreach (Team t in Teams.Values)
		{
			if (t.OrgId == orgId && !t.Retired)
			{
				return true;
			}
		}
		return false;
	}

	private Sport FirstSportOf(int orgId)
	{
		foreach (Team t in Teams.Values)
		{
			if (t.OrgId == orgId && !t.Retired)
			{
				return t.Sport;
			}
		}
		return Sport.Basketball;
	}

	private List<string> OrgNames(Level level, int excludeId)
	{
		List<string> list = new List<string>();
		foreach (Organization o in Orgs.Values)
		{
			if (o.Level == level && o.Id != excludeId && OrgHasActiveTeams(o.Id))
			{
				list.Add(o.Name);
			}
		}
		return list;
	}

	private List<string> OrgMascots(Level level, int excludeId)
	{
		List<string> list = new List<string>();
		foreach (Organization o in Orgs.Values)
		{
			if (o.Level == level && o.Id != excludeId && OrgHasActiveTeams(o.Id))
			{
				list.Add(o.Mascot);
			}
		}
		return list;
	}

	private List<string> PlacesInUse()
	{
		List<string> list = new List<string>();
		foreach (Organization o in Orgs.Values)
		{
			if (o.IsUser || !OrgHasActiveTeams(o.Id))
			{
				continue;
			}
			list.Add(o.Name);
			string[] places = NameGenerator.Places;
			foreach (string place in places)
			{
				if (o.Name.Contains(place))
				{
					list.Add(place);
				}
			}
		}
		return list;
	}

	private void CleanUpUnused()
	{
		HashSet<int> referenced = new HashSet<int>();
		foreach (League l in Leagues)
		{
			foreach (int id in l.TeamIds)
			{
				referenced.Add(id);
			}
			foreach (int id2 in l.PendingTeamIds)
			{
				referenced.Add(id2);
			}
			AddGameTeams(referenced, l.Current);
			AddGameTeams(referenced, l.Previous);
		}
		List<int> removeTeams = new List<int>();
		foreach (Team t in Teams.Values)
		{
			if (t.Retired && !referenced.Contains(t.Id))
			{
				removeTeams.Add(t.Id);
			}
		}
		foreach (int id3 in removeTeams)
		{
			Teams.Remove(id3);
		}
		HashSet<int> usedOrgs = new HashSet<int>();
		foreach (Team t2 in Teams.Values)
		{
			usedOrgs.Add(t2.OrgId);
		}
		List<int> removeOrgs = new List<int>();
		foreach (Organization o in Orgs.Values)
		{
			if (!usedOrgs.Contains(o.Id))
			{
				removeOrgs.Add(o.Id);
			}
		}
		foreach (int id4 in removeOrgs)
		{
			Orgs.Remove(id4);
		}
		PrunePlayers();
		CleanUpCoaches();
		foreach (League l2 in Leagues)
		{
			EnsurePicks(l2);
		}
		Talks.RemoveAll((ContractTalk k) => !Teams.ContainsKey(k.TeamId) || !Players.ContainsKey(k.PlayerId));
		Offers.RemoveAll((TradeOffer tradeOffer) => !Teams.ContainsKey(tradeOffer.FromTeamId) || !Teams.ContainsKey(tradeOffer.ToTeamId) || tradeOffer.Give.Exists((int key) => !Players.ContainsKey(key)) || tradeOffer.Get.Exists((int key) => !Players.ContainsKey(key)));
		foreach (Team value in Teams.Values)
		{
			value.Block.RemoveAll((int key) => !Players.ContainsKey(key));
		}
		RecruitBoard.RemoveAll((RecruitTarget r) => !Players.ContainsKey(r.PlayerId) || (r.CommittedTo >= 0 && !Teams.ContainsKey(r.CommittedTo)));
		RecruitEfforts.RemoveAll((RecruitEffort e) => !Players.ContainsKey(e.PlayerId) || !Teams.ContainsKey(e.TeamId));
	}

	private static void AddGameTeams(HashSet<int> set, Season s)
	{
		if (s == null)
		{
			return;
		}
		foreach (Match g in s.Games)
		{
			set.Add(g.HomeTeamId);
			set.Add(g.AwayTeamId);
		}
		foreach (PlayoffSeries ps in s.Bracket)
		{
			set.Add(ps.HighTeamId);
			set.Add(ps.LowTeamId);
		}
	}

	public static int UpDownCount(int teams)
	{
		if (teams > 6)
		{
			if (teams > 12)
			{
				return 3;
			}
			return 2;
		}
		return 1;
	}

	public League SecondDivision()
	{
		return GetLeague(Sport.Soccer, Level.Minor);
	}

	private bool InSoccerDivisions(int teamId)
	{
		League top = GetLeague(Sport.Soccer, Level.Pro);
		League second = SecondDivision();
		if (top == null || !top.TeamIds.Contains(teamId))
		{
			return second?.TeamIds.Contains(teamId) ?? false;
		}
		return true;
	}

	private List<League> SoccerDivisions()
	{
		List<League> list = new List<League>();
		League top = GetLeague(Sport.Soccer, Level.Pro);
		League second = SecondDivision();
		if (top != null)
		{
			list.Add(top);
		}
		if (second != null)
		{
			list.Add(second);
		}
		return list;
	}

	private void EnsureSecondDivision(int now)
	{
		League top = GetLeague(Sport.Soccer, Level.Pro);
		if (top != null && top.Current != null && !top.Current.AnyPlayed && KeepsRunning(top) && SecondDivision() == null)
		{
			League second = GetOrCreateLeague(Sport.Soccer, Level.Minor);
			BuildSeason(second, now);
		}
	}

	private static Season SeasonOfYear(League league, int year)
	{
		if (league.Current != null && league.Current.Year == year)
		{
			return league.Current;
		}
		if (league.Previous != null && league.Previous.Year == year)
		{
			return league.Previous;
		}
		return null;
	}

	private bool PromotionPending(League league, int y)
	{
		if (league.Sport != Sport.Soccer)
		{
			return false;
		}
		League top = GetLeague(Sport.Soccer, Level.Pro);
		League second = SecondDivision();
		if (top == null || second == null || second.PromotionDoneFor >= y)
		{
			return false;
		}
		if (SeasonOfYear(top, y) != null)
		{
			return SeasonOfYear(second, y) != null;
		}
		return false;
	}

	private void PromotionAndRelegation(int now)
	{
		League top = GetLeague(Sport.Soccer, Level.Pro);
		League second = SecondDivision();
		if (top == null || second == null)
		{
			return;
		}
		Season a = top.Previous;
		Season b = second.Previous;
		if (a == null || b == null || !a.Complete || !b.Complete || a.Year != b.Year || second.PromotionDoneFor >= a.Year || SeasonOfYear(top, a.Year) != a || SeasonOfYear(second, b.Year) != b)
		{
			return;
		}
		int year = (second.PromotionDoneFor = a.Year);
		League[] array = new League[2] { top, second };
		foreach (League l in array)
		{
			if (l.Current != null && l.Current.Year > year && (l.Current.AnyPlayed || l.Current.PlayoffsCreated))
			{
				return;
			}
		}
		int n = UpDownCount(Math.Min(top.TeamIds.Count, second.TeamIds.Count));
		List<StandingRow> ta = Standings.Compute(top, a);
		List<StandingRow> tb = Standings.Compute(second, b);
		List<int> down = new List<int>();
		int i2 = ta.Count - 1;
		while (i2 >= 0 && down.Count < n)
		{
			Team t = GetTeam(ta[i2].TeamId);
			if (t != null && !t.Retired && top.TeamIds.Contains(t.Id))
			{
				down.Add(t.Id);
			}
			i2--;
		}
		List<int> table = new List<int>();
		foreach (StandingRow r in tb)
		{
			Team t2 = GetTeam(r.TeamId);
			if (t2 != null && !t2.Retired && second.TeamIds.Contains(t2.Id))
			{
				table.Add(t2.Id);
			}
		}
		List<int> up = new List<int>();
		for (int j = 0; j < table.Count; j++)
		{
			if (up.Count >= Math.Max(0, n - 1))
			{
				break;
			}
			up.Add(table[j]);
		}
		int champion = ((n >= 2 && b.ChampionTeamId >= 0 && table.Contains(b.ChampionTeamId) && !up.Contains(b.ChampionTeamId)) ? b.ChampionTeamId : (-1));
		if (champion >= 0)
		{
			up.Add(champion);
		}
		for (int k = 0; k < table.Count; k++)
		{
			if (up.Count >= n)
			{
				break;
			}
			if (!up.Contains(table[k]))
			{
				up.Add(table[k]);
			}
		}
		int count = Math.Min(down.Count, up.Count);
		down.RemoveRange(count, down.Count - count);
		up.RemoveRange(count, up.Count - count);
		second.LastUp = new List<int>(up);
		second.LastDown = new List<int>(down);
		second.LastUpByPlayoff = champion;
		if (count == 0)
		{
			return;
		}
		string label = SeasonLabel(top, a);
		for (int m = 0; m < count; m++)
		{
			Team d = GetTeam(down[m]);
			Team u = GetTeam(up[m]);
			int di = top.TeamIds.IndexOf(d.Id);
			int ui = second.TeamIds.IndexOf(u.Id);
			top.TeamIds[di] = u.Id;
			second.TeamIds[ui] = d.Id;
			MoveDivision(d, Level.Minor);
			MoveDivision(u, Level.Pro);
			EnsureTransferBudget(u);
			EnsureClubBalance(u);
			int promotedBudget = u.TransferBudget;
			u.TransferBudget = Math.Min(60000, u.TransferBudget + 6000);
			u.ClubBalance = (int)Math.Max(0L, Math.Min(int.MaxValue, (long)u.ClubBalance + (long)(u.TransferBudget - promotedBudget) * 1000L));
			EnsureTransferBudget(d);
			EnsureClubBalance(d);
			int relegatedBudget = d.TransferBudget;
			d.TransferBudget = Math.Max(1500, (int)Math.Round((double)d.TransferBudget * 0.7 / 50.0) * 50) + 2000;
			d.ClubBalance = (int)Math.Max(0L, Math.Min(int.MaxValue, (long)d.ClubBalance + (long)(d.TransferBudget - relegatedBudget) * 1000L));
		}
		RostersChanged();
		StaffChanged();
		List<string> downNames = down.ConvertAll((int teamId) => "команда «" + TeamName(teamId) + "»");
		List<string> upNames = new List<string>();
		foreach (int id in up)
		{
			int place = table.IndexOf(id) + 1;
			List<string> list = upNames;
			string text = TeamName(id);
			string text2;
			if (id == champion)
			{
				text2 = " (победитель плей-офф)";
			}
			else
			{
				text2 = ((place == 1) ? " (чемпионы)" : (" (" + Views.Ordinal(place) + ")"));
			}
			list.Add("команда «" + text + "»" + text2);
		}
		string topTitle = Views.LeagueTitle(top);
		string secondTitle = "Вторая лига";
		Post(top, NewsKind.General, Wire, "ВЫЛЕТ: " + Capital(JoinNames(downNames)) + ((count == 1) ? " выбывает" : " выбывают") + " во вторую лигу " + secondTitle + " после " + label + " сезона. " + Capital(JoinNames(upNames.ConvertAll((string x) => x.Split(new[] { '(' }, StringSplitOptions.None)[0].Trim()))) + ((count == 1) ? " занимает его место" : " занимают их места") + ".", isUser: false, now, 3);
		Post(second, NewsKind.General, Wire, "ПОВЫШЕНИЕ: " + Capital(JoinNames(upNames)) + ((count == 1) ? " выходит" : " выходят") + " в " + topTitle + ". " + Capital(JoinNames(downNames)) + ((count == 1) ? " выбывает" : " выбывают") + " в обратном направлении.", isUser: false, now, 3);
		foreach (int id2 in up)
		{
			Team t3 = GetTeam(id2);
			if (t3 != null && t3.IsUser)
			{
				PostAsTeam(top, t3, NewsKind.General, "МЫ ВЫШЛИ ВЫШЕ! Повышение в " + topTitle + " — наша победа. Спасибо за поддержку в этом сезоне. До встречи в высшей лиге.", now);
			}
		}
		foreach (int id3 in down)
		{
			Team t4 = GetTeam(id3);
			if (t4 != null && t4.IsUser)
			{
				PostAsTeam(second, t4, NewsKind.General, "Мы вылетели в " + secondTitle + ". Это болезненно, но мы вернёмся. Команда продолжит играть дома, а борьба за возвращение начинается уже сейчас.", now);
			}
		}
		array = new League[2] { top, second };
		foreach (League l2 in array)
		{
			if (l2.Current != null && !l2.Current.AnyPlayed && !l2.Current.PlayoffsCreated)
			{
				BuildSeason(l2, now);
			}
		}
		RepickGamesOfTheMonth();
		Revision++;
	}

	private void MoveDivision(Team t, Level to)
	{
		t.Level = to;
		Organization o = GetOrg(t.OrgId);
		if (o != null && !o.IsUser)
		{
			o.Level = to;
		}
		foreach (Player p in new List<Player>(Players.Values))
		{
			if (p.TeamId == t.Id && p.Sport == t.Sport && (p.Status == PlayerStatus.Active || p.Status == PlayerStatus.Prospect))
			{
				ChangeLevel(p, to);
			}
		}
		foreach (Coach c in StaffOf(t.Id))
		{
			if (c.Level != to)
			{
				MoveToLevel(c, to);
			}
		}
		UpdateRating(t);
	}

	private bool ToSecondDivision(Player p)
	{
		League second = SecondDivision();
		if (second == null || second.Current == null)
		{
			return false;
		}
		List<Team> clubs = new List<Team>();
		foreach (int id in second.TeamIds)
		{
			Team t = GetTeam(id);
			if (t != null && !t.Retired && !t.IsUser && NoRoomFor(t, p) == null)
			{
				clubs.Add(t);
			}
		}
		if (clubs.Count == 0)
		{
			return false;
		}
		Team club = clubs[_random.Next(clubs.Count)];
		if (p.TeamId >= 0)
		{
			p.LastTeamId = p.TeamId;
		}
		int age = AgeOf(p);
		Sign(club, p, Economy.Get(Sport.Soccer).MinSalary, Math.Max(1, Economy.AskingYears(Sport.Soccer, age, resign: false)), NowHint(), announce: false);
		ArrangeRoster(club, NowHint());
		UpdateRating(club);
		return true;
	}

	public string DivisionZone(League league, int rank, int teams)
	{
		if (league.Sport != Sport.Soccer || SecondDivision() == null)
		{
			return "";
		}
		int n = UpDownCount(teams);
		if (league.Level == Level.Pro)
		{
			if (rank <= teams - n)
			{
				return "";
			}
			return "down";
		}
		if (league.Level != Level.Minor)
		{
			return "";
		}
		if (rank <= Math.Max(0, n - 1) || (n == 1 && rank == 1))
		{
			return "up";
		}
		if (n < 2 || rank > PlayoffTeamCount(Profile(league), teams))
		{
			return "";
		}
		return "playoff";
	}

	public string DivisionNote(League league)
	{
		if (league.Sport != Sport.Soccer || SecondDivision() == null)
		{
			return "";
		}
		League top = GetLeague(Sport.Soccer, Level.Pro);
		League second = SecondDivision();
		int n = UpDownCount(Math.Min(top?.TeamIds.Count ?? 0, second.TeamIds.Count));
		string last = "";
		if (second.LastUp.Count > 0)
		{
			List<string> names = ((league.Level == Level.Pro) ? second.LastUp : second.LastDown).ConvertAll((int id) => "команда «" + TeamName(id) + "»");
			last = " В прошлом сезоне " + JoinNames(names) + ((league.Level == Level.Pro) ? " вышли в высший дивизион." : " выбыли в низший дивизион.");
		}
		if (league.Level == Level.Pro)
		{
			return "Последняя команда" + ((n == 1) ? " выбывает" : (n + " команды выбывают")) + " во вторую лигу по итогам сезона (красный цвет)." + last;
		}
		if (n == 1)
		{
			return "Лидер выходит в " + Views.LeagueTitle(top) + " по итогам сезона (зелёный цвет)." + last;
		}
		return "Первые " + ((n == 2) ? " выбывает" : (n - 1 + " команды выбывают")) + " выходит в " + Views.LeagueTitle(top) + " по итогам сезона (зелёный цвет), а победитель стыкового плей-офф получает последнее место (бирюзовый цвет: остальные места плей-офф)." + last;
	}

	private static string JoinNames(List<string> names)
	{
		if (names.Count == 0)
		{
			return "";
		}
		if (names.Count == 1)
		{
			return names[0];
		}
		return string.Join(", ", names.GetRange(0, names.Count - 1).ToArray()) + " и " + names[names.Count - 1];
	}

	private static string Capital(string s)
	{
		if (!string.IsNullOrEmpty(s))
		{
			return s.Substring(0, 1).ToUpperInvariant() + s.Substring(1);
		}
		return s;
	}

	public Player AnyPlayer(int id)
	{
		Player p = GetPlayer(id);
		if (p != null)
		{
			return p;
		}
		if (!Retired.TryGetValue(id, out p))
		{
			return null;
		}
		return p;
	}

	private static bool CoachingPosition(Position pos)
	{
		switch (pos)
		{
		case Position.Center:
		case Position.Defense:
		case Position.Goalie:
		case Position.PointGuard:
		case Position.Quarterback:
		case Position.Linebacker:
		case Position.Catcher:
		case Position.Shortstop:
		case Position.Midfielder:
			return true;
		default:
			return false;
		}
	}

	private void KeepForCoaching(Player p, int age, int seasonYear)
	{
		if (p == null || p.Level != Level.Pro || p.Seasons < 4)
		{
			return;
		}
		double chance = 0.14;
		if (CoachingPosition(p.Position))
		{
			chance *= 1.8;
		}
		if (p.Seasons >= 10)
		{
			chance *= 1.4;
		}
		if (!(_random.NextDouble() >= Math.Min(0.5, chance)))
		{
			p.Status = PlayerStatus.Retired;
			if (p.TeamId >= 0)
			{
				p.LastTeamId = p.TeamId;
			}
			p.TeamId = -1;
			p.RetiredYear = seasonYear;
			p.DecideMonth = -1;
			p.InjuredUntil = int.MinValue;
			p.Injury = "";
			p.Salary = 0;
			p.ContractYears = 0;
			Retired[p.Id] = p;
		}
	}

	private HashSet<int> CoachingExPlayers()
	{
		HashSet<int> ids = new HashSet<int>();
		foreach (Coach c in Coaches.Values)
		{
			if (c.FormerPlayerId >= 0)
			{
				ids.Add(c.FormerPlayerId);
			}
		}
		return ids;
	}

	public Coach CoachFor(Player p)
	{
		if (p == null)
		{
			return null;
		}
		foreach (Coach c in Coaches.Values)
		{
			if (c.FormerPlayerId == p.Id)
			{
				return c;
			}
		}
		return null;
	}

	private Player TakeExPlayer(Sport sport, Level level)
	{
		if (Retired.Count == 0)
		{
			return null;
		}
		if (_random.NextDouble() >= level switch
		{
			Level.College => 0.35, 
			Level.Pro => 0.45, 
			_ => 0.25, 
		})
		{
			return null;
		}
		int year = CurrentYear();
		HashSet<int> busy = CoachingExPlayers();
		Player pick = null;
		int seen = 0;
		foreach (Player p in Retired.Values)
		{
			if (p.Sport != sport || busy.Contains(p.Id) || p.RetiredYear >= year)
			{
				continue;
			}
			int age = year - p.BirthYear;
			if (age >= 33 && age <= 62)
			{
				seen++;
				if (_random.Next(seen) == 0)
				{
					pick = p;
				}
			}
		}
		return pick;
	}

	private string PlayedLine(Player p)
	{
		List<SeasonLine> pro = p.History.FindAll((SeasonLine seasonLine) => seasonLine.Level == Level.Pro);
		List<string> parts = new List<string> { Rosters.NewsPositionLabel(p.Position, p.Sport) };
		int seasons = Math.Max(p.Seasons, pro.Count);
		string years = "";
		if (pro.Count > 0)
		{
			years = StartYear(pro[0].Season) + "-" + EndYear(pro[pro.Count - 1].Season);
		}
		parts.Add(seasons + ((seasons == 1) ? " профессиональный сезон" : " профессиональных сезонов") + ((years.Length > 0) ? (", " + years) : ""));
		Dictionary<string, int> teams = new Dictionary<string, int>();
		List<string> order = new List<string>();
		foreach (SeasonLine l in pro)
		{
			if (!string.IsNullOrEmpty(l.Team))
			{
				if (!teams.TryGetValue(l.Team, out var n))
				{
					order.Add(l.Team);
				}
				teams[l.Team] = n + 1;
			}
		}
		order.Sort((string a, string b) => teams[b].CompareTo(teams[a]));
		if (order.Count > 0)
		{
			parts.Add((order.Count <= 2) ? string.Join(", ", order.ToArray()) : (order[0] + ", " + order[1] + " и " + (order.Count - 2) + " ещё"));
		}
		int[] career = Stats.Fit(p.Career, p.Sport);
		if (!Stats.IsEmpty(career))
		{
			parts.Add(AwardBook.SportLine(p.Sport, p, career));
		}
		return string.Join(" · ", parts.ToArray());
	}

	private static string StartYear(string label)
	{
		if (!string.IsNullOrEmpty(label) && label.Length >= 4)
		{
			return label.Substring(0, 4);
		}
		return label ?? "";
	}

	private static string EndYear(string label)
	{
		if (string.IsNullOrEmpty(label) || label.Length < 4 || !int.TryParse(label.Substring(0, 4), NumberStyles.Integer, CultureInfo.InvariantCulture, out var y))
		{
			return label ?? "";
		}
		return ((label.Length >= 7 && label[4] == '-') ? (y + 1) : y).ToString(CultureInfo.InvariantCulture);
	}

	private void BecomeCoach(Coach c, Player ex)
	{
		c.FirstName = ex.FirstName;
		c.LastName = ex.LastName;
		c.BirthYear = ex.BirthYear;
		c.Hometown = ex.Hometown ?? "";
		c.FormerPlayerId = ex.Id;
		c.Played = PlayedLine(ex);
		if (CoachingPosition(ex.Position))
		{
			c.Tactics = Math.Min(99, c.Tactics + 2);
		}
		if (ex.PeakOvr >= Rosters.Base(Level.Pro) + 8)
		{
			c.Motivation = Math.Min(99, c.Motivation + 3);
		}
		if (c.Potential < c.Overall)
		{
			c.Potential = c.Overall;
		}
	}

	private void ExPlayersYearly()
	{
		int year = CurrentYear();
		if (year == _exPlayersYear)
		{
			return;
		}
		_exPlayersYear = year;
		if (Retired.Count == 0)
		{
			return;
		}
		HashSet<int> busy = CoachingExPlayers();
		List<int> drop = new List<int>();
		Dictionary<Sport, List<Player>> idle = new Dictionary<Sport, List<Player>>();
		foreach (Player p in Retired.Values)
		{
			int years = year - p.RetiredYear;
			if (busy.Contains(p.Id))
			{
				if (years > 10)
				{
					drop.Add(p.Id);
				}
				continue;
			}
			if (years > 4 || year - p.BirthYear > 62)
			{
				drop.Add(p.Id);
				continue;
			}
			if (!idle.TryGetValue(p.Sport, out var list))
			{
				list = (idle[p.Sport] = new List<Player>());
			}
			list.Add(p);
		}
		foreach (List<Player> list3 in idle.Values)
		{
			if (list3.Count > 14)
			{
				list3.Sort((Player a, Player b) => (a.RetiredYear == b.RetiredYear) ? a.Id.CompareTo(b.Id) : a.RetiredYear.CompareTo(b.RetiredYear));
				for (int i = 0; i < list3.Count - 14; i++)
				{
					drop.Add(list3[i].Id);
				}
			}
		}
		foreach (int id in drop)
		{
			Retired.Remove(id);
			foreach (Coach c in Coaches.Values)
			{
				if (c.FormerPlayerId == id)
				{
					c.FormerPlayerId = -1;
				}
			}
		}
		busy = CoachingExPlayers();
		List<Player> ready = new List<Player>();
		foreach (Player p2 in Retired.Values)
		{
			if (!busy.Contains(p2.Id) && p2.RetiredYear < year)
			{
				int age = year - p2.BirthYear;
				if (age >= 33 && age <= 60)
				{
					ready.Add(p2);
				}
			}
		}
		ready.Sort((Player a, Player b) => a.Id.CompareTo(b.Id));
		foreach (Player p3 in ready)
		{
			if (_random.NextDouble() >= 0.22)
			{
				continue;
			}
			List<League> options = new List<League>();
			foreach (League l in Leagues)
			{
				if (l.Sport == p3.Sport && (l.Level != Level.Pro || _random.NextDouble() < 0.5))
				{
					options.Add(l);
				}
			}
			if (options.Count != 0)
			{
				League where = options[_random.Next(options.Count)];
				CoachRole role = ((where.Sport == Sport.Football) ? ((CoachRole)_random.Next(3)) : CoachRole.Head);
				NewCoach(where.Sport, where.Level, role, -5.0 + Rosters.Normal(_random, 0.0, 2.0), year - p3.BirthYear, p3);
			}
		}
		StaffChanged();
	}

	private void CoachHometowns()
	{
		foreach (Coach c in Coaches.Values)
		{
			if (string.IsNullOrEmpty(c.Hometown))
			{
				c.Hometown = Bios.HometownFor(c.FirstName, c.Sport, new Random(c.Id * 13 + 5));
			}
		}
	}

	public Match FindGame(int id, out League league, out Season season)
	{
		league = null;
		season = null;
		if (id < 0)
		{
			return null;
		}
		foreach (League l in Leagues)
		{
			Season[] array = new Season[2] { l.Current, l.Previous };
			foreach (Season s in array)
			{
				if (s == null)
				{
					continue;
				}
				foreach (Match g in s.Games)
				{
					if (g.Id == id)
					{
						league = l;
						season = s;
						return g;
					}
				}
			}
		}
		return null;
	}

	public double HomeChance(League league, Season s, Match g)
	{
		Team home = GetTeam(g.HomeTeamId);
		Team away = GetTeam(g.AwayTeamId);
		if (home == null || away == null)
		{
			return 0.5;
		}
		PlayoffSeries ps = ((g.Stage == GameStage.Playoff) ? s.FindSeries(g.SeriesId) : null);
		GameRatings(s, ps, g, home, away, out var homeRating, out var awayRating);
		int day = Math.Max(Today(), GameClock.DayOf(g.Time));
		homeRating -= InjuryPenalty(home, day);
		awayRating -= InjuryPenalty(away, day);
		RivalryEdge(league, g, home, away, ref homeRating, ref awayRating);
		return Momentum.HomeWinChance(league.Sport, league.Level, homeRating, awayRating, g.VenueId == -2, g.Stage != GameStage.Regular);
	}

	private void PruneBoxes(int now)
	{
		int today = GameClock.DayOf(now);
		if (today == _boxPruneDay)
		{
			return;
		}
		_boxPruneDay = today;
		foreach (League l in Leagues)
		{
			if (l.Previous != null)
			{
				foreach (Match g in l.Previous.Games)
				{
					if (g.Box != null && (g.Stage == GameStage.Regular || !UserGame(g)))
					{
						g.Box = null;
					}
				}
			}
			if (l.Current == null)
			{
				continue;
			}
			foreach (Match g2 in l.Current.Games)
			{
				if (g2.Box != null && !UserGame(g2) && GameClock.DayOf(g2.Time) < today - 7)
				{
					g2.Box = null;
				}
			}
		}
	}

	private bool UserGame(Match g)
	{
		Team h = GetTeam(g.HomeTeamId);
		Team a = GetTeam(g.AwayTeamId);
		if (h == null || !h.IsUser)
		{
			return a?.IsUser ?? false;
		}
		return true;
	}

	public int KeptBoxes()
	{
		int n = 0;
		foreach (League l in Leagues)
		{
			Season[] array = new Season[2] { l.Current, l.Previous };
			foreach (Season s in array)
			{
				if (s == null)
				{
					continue;
				}
				foreach (Match game in s.Games)
				{
					if (game.Box != null)
					{
						n++;
					}
				}
			}
		}
		return n;
	}

	public static int HallThreshold(Level level)
	{
		if (level != Level.Pro)
		{
			return 18;
		}
		return 28;
	}

	private static int HallClassSize(Level level)
	{
		if (level != Level.Pro)
		{
			return 3;
		}
		return 4;
	}

	private static void AddHonor(Player p, Level level, string season, string award, string trophy)
	{
		List<string> honors = p.Honors;
		string[] array = new string[7] { season, "|", award, "|", trophy, "|", null };
		int num = (int)level;
		array[6] = num.ToString();
		honors.Add(string.Concat(array));
		while (p.Honors.Count > 60)
		{
			p.Honors.RemoveAt(0);
		}
	}

	public static bool ParseHonor(string h, out string season, out string award, out string trophy, out Level level)
	{
		season = (award = (trophy = ""));
		level = Level.Pro;
		if (string.IsNullOrEmpty(h))
		{
			return false;
		}
		string[] parts = h.Split(new[] { '|' }, StringSplitOptions.None);
		if (parts.Length < 3)
		{
			return false;
		}
		season = parts[0];
		award = parts[1];
		trophy = parts[2];
		if (parts.Length >= 4 && int.TryParse(parts[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out var lv) && Enum.IsDefined(typeof(Level), lv))
		{
			level = (Level)lv;
		}
		return true;
	}

	public static int HallScore(Player p, Level level)
	{
		int awards;
		return HallScore(p, level, out awards);
	}

	public static int HallScore(Player p, Level level, out int awards)
	{
		int seasons = 0;
		int peak = 0;
		awards = 0;
		foreach (SeasonLine l in p.History)
		{
			if (l.Level == level)
			{
				seasons++;
				if (l.Ovr > peak)
				{
					peak = l.Ovr;
				}
			}
		}
		double score = Math.Min(18, seasons);
		List<AwardDef> defs = AwardBook.For(p.Sport, level);
		foreach (string honor in p.Honors)
		{
			if (!ParseHonor(honor, out var _, out var award, out var _, out var lv) || lv != level)
			{
				continue;
			}
			if (award == "Champion")
			{
				score += 2.0;
				continue;
			}
			if (IsSelection(award))
			{
				score += (award.StartsWith("First") ? 1.5 : 0.5);
				continue;
			}
			awards++;
			int index = defs.FindIndex((AwardDef d) => d.Award == award);
			score += (double)((index == 0) ? 8 : 4);
		}
		score += Math.Min(10.0, (double)Math.Max(0, peak - ((level == Level.Pro) ? 86 : 84)) * 1.2);
		return (int)Math.Round(score);
	}

	private int MainTeam(Player p, Level level, out int seasonsThere)
	{
		Dictionary<int, int> count = new Dictionary<int, int>();
		int best = -1;
		seasonsThere = 0;
		foreach (SeasonLine l in p.History)
		{
			if (l.Level == level && l.TeamId >= 0)
			{
				count.TryGetValue(l.TeamId, out var n);
				n = (count[l.TeamId] = n + 1);
				if (n >= seasonsThere)
				{
					seasonsThere = n;
					best = l.TeamId;
				}
			}
		}
		return best;
	}

	private static string YearSpan(List<SeasonLine> lines)
	{
		if (lines.Count == 0)
		{
			return "";
		}
		int first = LabelYear(lines[0].Season, end: false);
		int last = LabelYear(lines[lines.Count - 1].Season, end: true);
		if (first <= 0)
		{
			return "";
		}
		if (first != last && last > 0)
		{
			return first + "-" + last;
		}
		return first.ToString(CultureInfo.InvariantCulture);
	}

	private static int LabelYear(string label, bool end)
	{
		if (string.IsNullOrEmpty(label) || label.Length < 4)
		{
			return 0;
		}
		if (!int.TryParse(label.Substring(0, 4), NumberStyles.Integer, CultureInfo.InvariantCulture, out var y))
		{
			return 0;
		}
		if (!end || label.Length <= 4 || label[4] != '-')
		{
			return y;
		}
		return y + 1;
	}

	private string HallSummary(Player p, Level level, int seasons)
	{
		Dictionary<string, int> awards = new Dictionary<string, int>();
		List<string> order = new List<string>();
		int titles = 0;
		foreach (string honor in p.Honors)
		{
			if (!ParseHonor(honor, out var _, out var award, out var _, out var lv) || lv != level)
			{
				continue;
			}
			if (award == "Champion")
			{
				titles++;
				continue;
			}
			if (!awards.TryGetValue(award, out var n))
			{
				order.Add(award);
			}
			awards[award] = n + 1;
		}
		List<string> parts = new List<string>();
		parts.Add(seasons + ((seasons == 1) ? " сезона" : " сезонов"));
		foreach (string a in order)
		{
			parts.Add(((awards[a] > 1) ? (awards[a] + "x ") : "") + a);
		}
		if (titles > 0)
		{
			parts.Add(titles + ((titles == 1) ? " титул" : " титула"));
		}
		int[] career = Stats.Fit(p.Career, p.Sport);
		if (!Stats.IsEmpty(career))
		{
			parts.Add(AwardBook.SportLine(p.Sport, p, career));
		}
		return string.Join(", ", parts.ToArray());
	}

	private void CareerOver(Player p, Level level, int seasonYear, int now)
	{
		if (level == Level.HighSchool || level == Level.Junior)
		{
			return;
		}
		League league = GetLeague(p.Sport, level);
		if (league == null)
		{
			return;
		}
		List<SeasonLine> lines = p.History.FindAll((SeasonLine seasonLine) => seasonLine.Level == level);
		if (lines.Count == 0)
		{
			return;
		}
		int score = HallScore(p, level, out var awards);
		int threshold = HallThreshold(level);
		int main = MainTeam(p, level, out var seasonsThere);
		Team mainTeam = GetTeam(main);
		string label = ((league.Current != null) ? SeasonLabel(league, league.Current) : seasonYear.ToString(CultureInfo.InvariantCulture));
		if (level == Level.Pro && mainTeam != null && !mainTeam.Retired && seasonsThere >= 12 && (double)score >= (double)threshold * 0.75)
		{
			RetireNumber(league, mainTeam, p, lines, label, now, hall: false);
		}
		if (score < threshold || awards == 0)
		{
			return;
		}
		List<string> teams = new List<string>();
		if (mainTeam != null)
		{
			teams.Add(TeamName(main));
		}
		foreach (SeasonLine l in lines)
		{
			string name = ((GetTeam(l.TeamId) != null) ? TeamName(l.TeamId) : l.Team);
			if (name.Length > 0 && !teams.Contains(name))
			{
				teams.Add(name);
			}
		}
		league.HallWaiting.Add(new HallEntry
		{
			PlayerId = p.Id,
			Name = p.Name,
			Position = Rosters.NewsPositionLabel(p.Position, p.Sport),
			Teams = string.Join(", ", teams.ToArray()),
			Years = YearSpan(lines),
			Summary = HallSummary(p, level, lines.Count),
			Score = score,
			EligibleYear = seasonYear + ((level == Level.Pro) ? 3 : 2),
			Number = p.Number,
			MainTeamId = main
		});
		Revision++;
	}

	private void RetireNumber(League league, Team team, Player p, List<SeasonLine> lines, string label, int now, bool hall)
	{
		RetireNumber(league, team, p.Number, p.Name, YearSpan(lines.FindAll((SeasonLine l) => l.TeamId == team.Id)), label, now, hall);
	}

	private void RetireNumber(League league, Team team, int number, string name, string years, string label, int now, bool hall)
	{
		if (team == null || team.Retired || number < 0 || team.RetiredNumbers.Count >= 20)
		{
			return;
		}
		foreach (RetiredNumber r in team.RetiredNumbers)
		{
			if (r.Number == number || r.Name == name)
			{
				return;
			}
		}
		team.RetiredNumbers.Add(new RetiredNumber
		{
			Number = number,
			Name = name,
			Years = years,
			Season = label
		});
		team.RetiredNumbers.Sort((RetiredNumber a, RetiredNumber b) => a.Number.CompareTo(b.Number));
		string text = "" + TeamName(team.Id) + " выводит из обращения номер " + number + " в честь " + name + (hall ? ", который теперь включён в Зал славы клуба." : ". Этот номер больше никто не получит.");
		if (team.IsUser)
		{
			PostAsTeam(league, team, NewsKind.HallOfFame, "№ " + number + " поднят под своды арены. Спасибо, " + name + ".", now);
		}
		else
		{
			Post(league, NewsKind.HallOfFame, Wire, text, isUser: false, now, 1);
		}
		Revision++;
	}

	private void HallOfFameVote(League league, Season s, int now)
	{
		if (league.Level == Level.HighSchool || league.Level == Level.Junior || league.HallWaiting.Count == 0)
		{
			return;
		}
		List<HallEntry> eligible = league.HallWaiting.FindAll((HallEntry hallEntry) => hallEntry.EligibleYear <= s.Year);
		if (eligible.Count == 0)
		{
			return;
		}
		eligible.Sort((HallEntry a, HallEntry b) => (b.Score == a.Score) ? a.EligibleYear.CompareTo(b.EligibleYear) : b.Score.CompareTo(a.Score));
		string label = SeasonLabel(league, s);
		List<HallEntry> inducted = new List<HallEntry>();
		for (int i = 0; i < eligible.Count; i++)
		{
			if (inducted.Count >= HallClassSize(league.Level))
			{
				break;
			}
			inducted.Add(eligible[i]);
		}
		foreach (HallEntry e in inducted)
		{
			league.HallWaiting.Remove(e);
			e.Inducted = label;
			league.HallOfFame.Insert(0, e);
			Team team = GetTeam(e.MainTeamId);
			if (team != null && !team.Retired && league.TeamIds.Contains(team.Id))
			{
				RetireNumber(league, team, e.Number, e.Name, e.Years, label, now, hall: true);
			}
		}
		league.HallWaiting.RemoveAll((HallEntry hallEntry) => hallEntry.EligibleYear + 5 < s.Year);
		while (league.HallOfFame.Count > 300)
		{
			league.HallOfFame.RemoveAt(league.HallOfFame.Count - 1);
		}
		List<string> names = new List<string>();
		bool user = false;
		foreach (HallEntry e2 in inducted)
		{
			names.Add(e2.Position + " " + e2.Name + " (" + e2.Teams.Split(new[] { ',' }, StringSplitOptions.None)[0] + ")");
			Team t = GetTeam(e2.MainTeamId);
			if (t != null && t.IsUser)
			{
				user = true;
			}
		}
		string what = ((league.Level == Level.Pro) ? "Зал славы" : "Зал славы студенческой лиги");
		Post(league, NewsKind.HallOfFame, Wire, what.ToUpperInvariant() + ": выпуск " + label + ": " + string.Join(", ", names.ToArray()) + ".", user, now, 3);
		AddNote(league, "Включён в " + what + ": " + string.Join(", ", inducted.ConvertAll((HallEntry hallEntry) => hallEntry.Name).ToArray()) + ".");
		Revision++;
	}

	private int NumberFor(Team team, Position pos)
	{
		List<int> retired = new List<int>();
		foreach (RetiredNumber r in team.RetiredNumbers)
		{
			retired.Add(r.Number);
		}
		return Rosters.NewNumber(pos, _random, SignedPlayers(team.Id), retired);
	}

	private void FixRetiredNumbers(Team team, List<Player> joined)
	{
		if (team.RetiredNumbers.Count == 0 || joined.Count == 0)
		{
			return;
		}
		HashSet<int> retired = new HashSet<int>();
		foreach (RetiredNumber r in team.RetiredNumbers)
		{
			retired.Add(r.Number);
		}
		foreach (Player p in joined)
		{
			if (retired.Contains(p.Number))
			{
				List<Player> mates = new List<Player>(RosterOf(team.Id));
				mates.AddRange(joined);
				p.Number = Rosters.NewNumber(p.Position, _random, mates, retired);
			}
		}
	}

	public List<HallEntry> HallMembersOf(League league, int teamId)
	{
		List<HallEntry> list = new List<HallEntry>();
		if (league == null)
		{
			return list;
		}
		foreach (HallEntry e in league.HallOfFame)
		{
			if (e.MainTeamId == teamId)
			{
				list.Add(e);
			}
		}
		return list;
	}

	public static bool UsefulDistrictName(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return false;
		}
		string n = name.Trim();
		if (n.Length < 2 || n.Length > 40)
		{
			return false;
		}
		if (n.StartsWith("District", StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		return true;
	}

	private bool HasDistrict(string name)
	{
		foreach (string district in Districts)
		{
			if (string.Equals(district, name, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	public void NoteDistrict(string name)
	{
		if (UsefulDistrictName(name))
		{
			name = NameGenerator.CleanName(name.Trim(), 40);
			if (name.Length != 0 && !HasDistrict(name) && Districts.Count < 80)
			{
				Districts.Add(name);
			}
		}
	}

	public void SetDistricts(IEnumerable<string> names)
	{
		List<string> list = new List<string>();
		foreach (string raw in names)
		{
			if (!UsefulDistrictName(raw))
			{
				continue;
			}
			string n = NameGenerator.CleanName(raw.Trim(), 40);
			if (n.Length == 0)
			{
				continue;
			}
			bool dup = false;
			foreach (string item in list)
			{
				if (string.Equals(item, n, StringComparison.OrdinalIgnoreCase))
				{
					dup = true;
				}
			}
			if (!dup && list.Count < 80)
			{
				list.Add(n);
			}
		}
		if (list.Count != 0)
		{
			bool same = list.Count == Districts.Count;
			int i = 0;
			while (same && i < list.Count)
			{
				same = list[i] == Districts[i];
				i++;
			}
			if (!same)
			{
				Districts.Clear();
				Districts.AddRange(list);
				_localMonth = int.MinValue;
				Revision++;
			}
		}
	}

	private string DistrictKeyName(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return null;
		}
		int colon = key.IndexOf(':');
		string want = ((colon >= 0) ? key.Substring(colon + 1) : key).Trim().ToLowerInvariant();
		foreach (string d in Districts)
		{
			if (d.ToLowerInvariant() == want)
			{
				return d;
			}
		}
		return null;
	}

	private Organization OrgNamed(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		foreach (Organization o in Orgs.Values)
		{
			if (o.IsUser && o.Name == name)
			{
				return o;
			}
		}
		return null;
	}

	private string DistrictHome(Player p, Organization school)
	{
		Random r = new Random(p.Id * 7919 + 101);
		string own = ((school == null) ? null : DistrictKeyName(school.DistrictKey));
		string pick = ((own != null && r.NextDouble() < 0.5) ? own : Districts[r.Next(Districts.Count)]);
		return "*d:" + pick;
	}

	private void LocalHometowns(int now)
	{
		int month = GameClock.MonthIndex(now);
		if (month == _localMonth)
		{
			return;
		}
		_localMonth = month;
		bool changed = false;
		foreach (Player p in Players.Values)
		{
			string town = p.Hometown ?? "";
			if (town.StartsWith("*d:", StringComparison.Ordinal))
			{
				continue;
			}
			Organization school = null;
			bool cityKid = town == "*";
			if (!cityKid)
			{
				Team t = ((p.Level == Level.HighSchool) ? GetTeam(p.TeamId) : null);
				if (t != null && t.IsUser)
				{
					cityKid = true;
					school = GetOrg(t.OrgId);
				}
				else
				{
					school = OrgNamed(p.From);
					cityKid = school != null;
				}
			}
			else if (p.Level == Level.HighSchool)
			{
				Team t2 = GetTeam(p.TeamId);
				if (t2 != null)
				{
					school = GetOrg(t2.OrgId);
				}
			}
			if (cityKid)
			{
				string home = ((Districts.Count > 0) ? DistrictHome(p, school) : "*");
				if (home != town)
				{
					p.Hometown = home;
					changed = true;
				}
			}
		}
		CoachHometowns();
		foreach (Coach c in Coaches.Values)
		{
			if (!(c.Hometown != "*") && Districts.Count != 0)
			{
				c.Hometown = "*d:" + Districts[new Random(c.Id * 7 + 3).Next(Districts.Count)];
				changed = true;
			}
		}
		if (changed)
		{
			Revision++;
		}
	}

	public int FixMixedNames()
	{
		Dictionary<string, string> renamed = new Dictionary<string, string>();
		HashSet<string> keep = new HashSet<string>();
		int fixedCount = 0;
		foreach (Player p in Players.Values)
		{
			Random r = new Random(p.Id * 31 + 7);
			if (!PlayerNames.Fits(p.FirstName, p.LastName))
			{
				string old = p.Name;
				p.LastName = PlayerNames.LastFor(p.FirstName, r);
				fixedCount++;
				if (renamed.ContainsKey(old))
				{
					keep.Add(old);
				}
				else
				{
					renamed[old] = p.Name;
				}
			}
			else
			{
				keep.Add(p.Name);
			}
			if (!Bios.HometownFits(p))
			{
				p.Hometown = Bios.Hometown(p, r);
			}
		}
		foreach (Coach c in Coaches.Values)
		{
			if (PlayerNames.Fits(c.FirstName, c.LastName))
			{
				keep.Add(c.FirstName + " " + c.LastName);
				continue;
			}
			string old2 = c.FirstName + " " + c.LastName;
			c.LastName = PlayerNames.LastFor(c.FirstName, new Random(c.Id * 37 + 11));
			fixedCount++;
			if (renamed.ContainsKey(old2))
			{
				keep.Add(old2);
			}
			else
			{
				renamed[old2] = c.FirstName + " " + c.LastName;
			}
		}
		foreach (string k in keep)
		{
			renamed.Remove(k);
		}
		if (renamed.Count == 0)
		{
			return fixedCount;
		}
		foreach (League l in Leagues)
		{
			foreach (AwardRecord a in l.Awards)
			{
				Player p2 = GetPlayer(a.PlayerId);
				if (p2 != null)
				{
					a.PlayerName = p2.Name;
				}
				else
				{
					a.PlayerName = Renamed(renamed, a.PlayerName);
				}
				a.Detail = Renamed(renamed, a.Detail);
			}
			foreach (HallEntry h in l.HallOfFame)
			{
				h.Name = Renamed(renamed, h.Name);
			}
			foreach (HallEntry h2 in l.HallWaiting)
			{
				h2.Name = Renamed(renamed, h2.Name);
			}
			foreach (RecordBoard record in l.Records)
			{
				foreach (RecordEntry e in record.Entries)
				{
					e.Name = Renamed(renamed, e.Name);
				}
			}
			foreach (SeasonSummary s in l.History)
			{
				RenameAll(renamed, s.Awards);
				RenameAll(renamed, s.Leaders);
				RenameAll(renamed, s.Notes);
				RenameAll(renamed, s.Playoffs);
			}
			RenameAll(renamed, l.SeasonNotes);
			if (l.Draft == null)
			{
				continue;
			}
			foreach (DraftPick d in l.Draft.Picks)
			{
				d.PlayerName = Renamed(renamed, d.PlayerName);
			}
		}
		foreach (Team t in Teams.Values)
		{
			foreach (RetiredNumber n in t.RetiredNumbers)
			{
				n.Name = Renamed(renamed, n.Name);
			}
			foreach (DeadCharge d2 in t.Dead)
			{
				d2.Name = Renamed(renamed, d2.Name);
			}
		}
		foreach (NewsPost f in Feed)
		{
			f.Text = Renamed(renamed, f.Text);
		}
		Revision++;
		return fixedCount;
	}

	private static void RenameAll(Dictionary<string, string> renamed, List<string> list)
	{
		if (list != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				list[i] = Renamed(renamed, list[i]);
			}
		}
	}

	private static string Renamed(Dictionary<string, string> renamed, string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		if (renamed.TryGetValue(text, out var direct))
		{
			return direct;
		}
		bool any = false;
		foreach (string old in renamed.Keys)
		{
			if (text.IndexOf(old, StringComparison.Ordinal) >= 0)
			{
				any = true;
				break;
			}
		}
		if (!any)
		{
			return text;
		}
		string result = text;
		foreach (KeyValuePair<string, string> kv in renamed)
		{
			int start = 0;
			while (true)
			{
				int i = result.IndexOf(kv.Key, start, StringComparison.Ordinal);
				if (i < 0)
				{
					break;
				}
				int end = i + kv.Key.Length;
				bool num = i == 0 || !char.IsLetter(result[i - 1]);
				bool after = end >= result.Length || !char.IsLetter(result[end]);
				if (num & after)
				{
					result = result.Substring(0, i) + kv.Value + result.Substring(end);
					start = i + kv.Value.Length;
				}
				else
				{
					start = end;
				}
			}
		}
		return result;
	}

	public int Today()
	{
		int t = ((_clockHint != int.MinValue) ? _clockHint : LastTime);
		if (t != int.MinValue)
		{
			return GameClock.DayOf(t);
		}
		return -2147483647;
	}

	public string InjuryText(Player p)
	{
		int day = Today();
		if (!IsInjured(p, day))
		{
			return "";
		}
		return p.Injury + ", " + InjuryTimeline(p, day).ToLowerInvariant();
	}

	public bool IsInjured(Player p, int day)
	{
		if (p != null)
		{
			return p.InjuredUntil > day;
		}
		return false;
	}

	public static int DaysOut(Player p, int day)
	{
		if (p != null && p.InjuredUntil > day)
		{
			return p.InjuredUntil - day;
		}
		return 0;
	}

	public static string InjuryTimeline(Player p, int day)
	{
		int d = DaysOut(p, day);
		if (d <= 0)
		{
			return "";
		}
		if (d <= 4)
		{
			return "День за днём";
		}
		if (d <= 10)
		{
			return "Около недели";
		}
		if (d <= 17)
		{
			return "1–2 недели";
		}
		if (d <= 45)
		{
			return (d + 6) / 7 + " недель";
		}
		if (d <= 120)
		{
			return (d + 15) / 30 + " месяцев";
		}
		return "До конца сезона";
	}

	public double PickGoalie(Team t, int day, bool playoff, out int goalieId)
	{
		goalieId = -1;
		if (t == null)
		{
			return 0.0;
		}
		List<Player> goalies = Healthy(t.Id, day).FindAll((Player p) => p.Position == Position.Goalie && p.Status == PlayerStatus.Active);
		if (goalies.Count == 0)
		{
			return 0.0;
		}
		goalies.Sort((Player a, Player b) => (b.Overall == a.Overall) ? a.Id.CompareTo(b.Id) : b.Overall.CompareTo(a.Overall));
		Player starter = goalies[0];
		if (goalies.Count > 1)
		{
			double gap = goalies[0].Overall - goalies[1].Overall;
			double share = Math.Max(0.5, Math.Min(0.8, 0.5 + 0.034 * gap));
			if (playoff)
			{
				share = ((gap >= 3.0) ? 0.95 : Math.Max(share, 0.65));
			}
			if (_random.NextDouble() >= share)
			{
				starter = goalies[1];
			}
		}
		goalieId = starter.Id;
		return 0.25 * (double)(starter.Overall - goalies[0].Overall);
	}

	public List<Player> Healthy(int teamId, int day)
	{
		List<Player> roster = RosterOf(teamId);
		bool any = false;
		foreach (Player item in roster)
		{
			if (item.InjuredUntil > day)
			{
				any = true;
				break;
			}
		}
		if (!any)
		{
			return roster;
		}
		List<Player> list = new List<Player>();
		foreach (Player p in roster)
		{
			if (p.InjuredUntil <= day)
			{
				list.Add(p);
			}
		}
		return list;
	}

	public double InjuryPenalty(Team t, int day)
	{
		if (t == null)
		{
			return 0.0;
		}
		List<Player> roster = RosterOf(t.Id);
		bool any = false;
		foreach (Player item in roster)
		{
			if (item.InjuredUntil > day)
			{
				any = true;
				break;
			}
		}
		if (!any)
		{
			return 0.0;
		}
		int healthy = Rosters.TeamRating(t.Sport, t.Level, Healthy(t.Id, day));
		int full = Rosters.TeamRating(t.Sport, t.Level, roster);
		return Math.Max(0, full - healthy);
	}

	private static double InjuryChance(Sport sport, Level level)
	{
		double c = sport switch
		{
			Sport.Hockey => 0.1, 
			Sport.Basketball => 0.09, 
			Sport.Football => 0.6, 
			Sport.Baseball => 0.035, 
			_ => 0.2, 
		};
		if (level != Level.Pro)
		{
			return c * 0.8;
		}
		return c;
	}

	private static int MaxInjured(Sport sport, Level level)
	{
		int n = sport switch
		{
			Sport.Football => 6, 
			Sport.Basketball => 2, 
			_ => 3, 
		};
		if (level != Level.Pro)
		{
			return Math.Max(2, n - 1);
		}
		return n;
	}

	private void MaybeInjure(League league, Match g, int now)
	{
		if (Settings.InjuryPercent <= 0)
		{
			return;
		}
		int day = GameClock.DayOf(g.Time);
		int[] array = new int[2] { g.HomeTeamId, g.AwayTeamId };
		foreach (int teamId in array)
		{
			Team t = GetTeam(teamId);
			if (t != null)
			{
				double chance = InjuryChance(league.Sport, league.Level) * (double)Settings.InjuryPercent / 100.0;
				if (!(_random.NextDouble() >= chance))
				{
					Injure(league, t, day, now);
				}
			}
		}
	}

	private void Injure(League league, Team t, int day, int now)
	{
		List<Player> roster = RosterOf(t.Id);
		int out_ = 0;
		foreach (Player item in roster)
		{
			if (item.InjuredUntil > day)
			{
				out_++;
			}
		}
		if (out_ >= MaxInjured(t.Sport, t.Level))
		{
			return;
		}
		List<KeyValuePair<Player, double>> pool = new List<KeyValuePair<Player, double>>();
		double total = 0.0;
		RosterSlot[] array = Rosters.Template(t.Sport, t.Level);
		foreach (RosterSlot slot in array)
		{
			List<Player> atPos = roster.FindAll((Player p) => p.Position == slot.Pos && p.InjuredUntil <= day);
			int need = ((slot.Starters >= 3) ? (slot.Starters - 1) : slot.Starters);
			if (atPos.Count - 1 >= Math.Max(1, need))
			{
				atPos.Sort((Player a, Player b) => (b.Overall == a.Overall) ? a.Id.CompareTo(b.Id) : b.Overall.CompareTo(a.Overall));
				for (int i2 = 0; i2 < atPos.Count; i2++)
				{
					double w = ((i2 < slot.Starters) ? 3.0 : 1.0);
					pool.Add(new KeyValuePair<Player, double>(atPos[i2], w));
					total += w;
				}
			}
		}
		if (pool.Count == 0 || total <= 0.0)
		{
			return;
		}
		double pick = _random.NextDouble() * total;
		Player hurt = pool[pool.Count - 1].Key;
		foreach (KeyValuePair<Player, double> kv in pool)
		{
			pick -= kv.Value;
			if (pick <= 0.0)
			{
				hurt = kv.Key;
				break;
			}
		}
		double r = _random.NextDouble();
		int tier;
		int days;
		string what;
		if (r < 0.5)
		{
			tier = 0;
			days = _random.Next(1, 5);
			what = MinorInjuries[_random.Next(MinorInjuries.Length)];
		}
		else if (r < 0.75)
		{
			tier = 1;
			days = _random.Next(5, 15);
			what = MinorInjuries[_random.Next(MinorInjuries.Length)];
		}
		else if (r < 0.9)
		{
			tier = 2;
			days = _random.Next(15, 43);
			what = MediumInjuries[_random.Next(MediumInjuries.Length)];
		}
		else if (r < 0.98)
		{
			tier = 3;
			days = _random.Next(43, 121);
			what = MajorInjuries[_random.Next(MajorInjuries.Length)];
		}
		else
		{
			tier = 4;
			days = _random.Next(150, 300);
			what = SeasonInjuries[_random.Next(SeasonInjuries.Length)];
		}
		if (t.Sport == Sport.Football)
		{
			days = ((tier == 0) ? _random.Next(2, 10) : ((int)((double)days * 1.4)));
		}
		hurt.InjuredUntil = day + days;
		if (PlayerInjured != null)
		{
			PlayerInjured(league, t, hurt, days);
		}
		hurt.Injury = what;
		string season = ((league.Current != null) ? SeasonLabel(league, league.Current) : "");
		hurt.InjuryLog.Insert(0, ((season.Length > 0) ? (season + ": ") : "") + what + " (" + InjuryTimeline(hurt, day) + ")");
		while (hurt.InjuryLog.Count > 8)
		{
			hurt.InjuryLog.RemoveAt(hurt.InjuryLog.Count - 1);
		}
		if (tier >= 3 && _random.NextDouble() < 0.3)
		{
			hurt.Overall = Rosters.Clamp(hurt.Overall - _random.Next(1, 3), Rosters.MinOverall(hurt.Level), Rosters.MaxOverall(hurt.Level));
			if (hurt.Potential < hurt.Overall)
			{
				hurt.Potential = hurt.Overall;
			}
		}
		Revision++;
		bool star = Rosters.Ovr(hurt) >= ((t.Level == Level.Pro) ? 88 : ((t.Level == Level.College) ? 80 : 60));
		if ((t.IsUser && tier >= 1) || (star && tier >= 2))
		{
			string text = "ТРАВМА: " + Rosters.NewsPositionLabel(hurt.Position, hurt.Sport) + " " + hurt.Name + " из клуба " + TeamName(t.Id) + " (" + what.ToLowerInvariant() + ")" + ((tier == 4) ? " пропустит остаток сезона." : (" пропустит " + InjuryTimeline(hurt, day).ToLowerInvariant() + "."));
			Post(league, NewsKind.Injury, Insider, text, t.IsUser, now, (tier >= 3) ? 3 : 2);
		}
		if (tier == 4)
		{
			AddNote(league, hurt.Name + " (" + TeamName(t.Id) + ") выбыл до конца сезона (" + what.ToLowerInvariant() + ").");
		}
	}

	private static Dictionary<Sport, TourSpec> BuildSpecs()
	{
		Dictionary<Sport, TourSpec> d = new Dictionary<Sport, TourSpec>();
		d[Sport.Hockey] = new TourSpec
		{
			Name = "Молодёжный чемпионат мира по хоккею",
			Under20 = true,
			Every = 1,
			Offset = 0,
			Month = 12,
			Day = 26,
			Nations = new string[20]
			{
				"Canada", "USA", "Sweden", "Finland", "Russia", "Czechia", "Slovakia", "Switzerland", "Germany", "Latvia",
				"Norway", "Denmark", "Austria", "France", "Kazakhstan", "Slovenia", "Italy", "Hungary", "Poland", "Великобритания"
			},
			Depth = new int[20]
			{
				67, 66, 65, 65, 64, 63, 61, 61, 60, 58,
				57, 57, 56, 56, 55, 54, 54, 53, 53, 53
			}
		};
		d[Sport.Soccer] = new TourSpec
		{
			Name = "Чемпионат мира U-20",
			Under20 = true,
			Every = 2,
			Offset = 1,
			Month = 5,
			Day = 20,
			Nations = new string[20]
			{
				"Brazil", "Argentina", "France", "Spain", "England", "Germany", "Portugal", "Italy", "Netherlands", "Uruguay",
				"USA", "Mexico", "Colombia", "Nigeria", "Japan", "Morocco", "Senegal", "South Korea", "Ghana", "Russia"
			},
			Depth = new int[20]
			{
				66, 66, 66, 66, 65, 65, 64, 64, 64, 62,
				62, 62, 61, 61, 61, 61, 60, 60, 59, 59
			}
		};
		d[Sport.Basketball] = new TourSpec
		{
			Name = "Кубок мира по баскетболу",
			Every = 4,
			Offset = 3,
			Month = 8,
			Day = 25,
			Nations = new string[20]
			{
				"USA", "Canada", "Serbia", "France", "Spain", "Australia", "Germany", "Greece", "Slovenia", "Lithuania",
				"Argentina", "Italy", "Brazil", "Puerto Rico", "Dominican Republic", "Latvia", "Nigeria", "Montenegro", "Japan", "Russia"
			},
			Depth = new int[20]
			{
				70, 66, 65, 65, 64, 64, 64, 63, 62, 62,
				61, 61, 60, 59, 59, 59, 58, 58, 57, 57
			}
		};
		d[Sport.Baseball] = new TourSpec
		{
			Name = "Мировая классика бейсбола",
			Every = 3,
			Offset = 1,
			Month = 3,
			Day = 5,
			Nations = new string[20]
			{
				"USA", "Japan", "Dominican Republic", "Venezuela", "Puerto Rico", "Mexico", "South Korea", "Cuba", "Canada", "Netherlands",
				"Chinese Taipei", "Panama", "Colombia", "Australia", "Italy", "Israel", "Nicaragua", "Великобритания", "Czechia", "Russia"
			},
			Depth = new int[20]
			{
				69, 68, 68, 66, 64, 64, 63, 63, 61, 61,
				59, 59, 59, 58, 58, 56, 56, 55, 55, 54
			}
		};
		d[Sport.Football] = new TourSpec
		{
			Name = "Чемпионат мира по американскому футболу",
			Every = 4,
			Offset = 3,
			Month = 7,
			Day = 8,
			Nations = new string[16]
			{
				"USA", "Canada", "Mexico", "Japan", "Germany", "Austria", "Великобритания", "France", "Italy", "Finland",
				"Sweden", "Australia", "Denmark", "Brazil", "Panama", "Russia"
			},
			Depth = new int[16]
			{
				72, 62, 58, 57, 57, 56, 55, 54, 54, 54,
				53, 53, 52, 52, 51, 51
			}
		};
		return d;
	}

	public static string TournamentStageLabel(string stage)
	{
		if (string.IsNullOrEmpty(stage))
		{
			return "";
		}
		if (stage == "Qualifier")
		{
			return "Квалификация";
		}
		if (stage == "Quarterfinal")
		{
			return "Четвертьфинал";
		}
		if (stage == "Semifinal")
		{
			return "Полуфинал";
		}
		if (stage.StartsWith("Group ", StringComparison.Ordinal))
		{
			return "Группа " + stage.Substring(6);
		}
		return stage;
	}

	public static string NationLabel(string nation)
	{
		return nation switch
		{
			"USA" => "США",
			"Canada" => "Канада",
			"Mexico" => "Мексика",
			"Sweden" => "Швеция",
			"Finland" => "Финляндия",
			"Russia" => "Россия",
			"Czechia" => "Чехия",
			"Slovakia" => "Словакия",
			"Switzerland" => "Швейцария",
			"Germany" => "Германия",
			"Latvia" => "Латвия",
			"Norway" => "Норвегия",
			"Denmark" => "Дания",
			"Austria" => "Австрия",
			"France" => "Франция",
			"Kazakhstan" => "Казахстан",
			"Slovenia" => "Словения",
			"Italy" => "Италия",
			"Hungary" => "Венгрия",
			"Poland" => "Польша",
			"England" => "Англия",
			"Scotland" => "Шотландия",
			"Wales" => "Уэльс",
			"Ireland" => "Ирландия",
			"Brazil" => "Бразилия",
			"Argentina" => "Аргентина",
			"Spain" => "Испания",
			"Portugal" => "Португалия",
			"Netherlands" => "Нидерланды",
			"Belgium" => "Бельгия",
			"Uruguay" => "Уругвай",
			"Colombia" => "Колумбия",
			"Ecuador" => "Эквадор",
			"Chile" => "Чили",
			"Peru" => "Перу",
			"Venezuela" => "Венесуэла",
			"Nigeria" => "Нигерия",
			"Ghana" => "Гана",
			"Senegal" => "Сенегал",
			"Morocco" => "Марокко",
			"Cameroon" => "Камерун",
			"Egypt" => "Египет",
			"South Africa" => "ЮАР",
			"Japan" => "Япония",
			"South Korea" => "Южная Корея",
			"China" => "Китай",
			"Chinese Taipei" => "Тайвань",
			"Australia" => "Австралия",
			"New Zealand" => "Новая Зеландия",
			"Philippines" => "Филиппины",
			"India" => "Индия",
			"Iran" => "Иран",
			"Saudi Arabia" => "Саудовская Аравия",
			"Israel" => "Израиль",
			"Turkey" => "Турция",
			"Greece" => "Греция",
			"Serbia" => "Сербия",
			"Croatia" => "Хорватия",
			"Montenegro" => "Черногория",
			"Lithuania" => "Литва",
			"Estonia" => "Эстония",
			"Ukraine" => "Украина",
			"Belarus" => "Беларусь",
			"Romania" => "Румыния",
			"Bulgaria" => "Болгария",
			"Iceland" => "Исландия",
			"Puerto Rico" => "Пуэрто-Рико",
			"Dominican Republic" => "Доминиканская Республика",
			"Cuba" => "Куба",
			"Panama" => "Панама",
			"Nicaragua" => "Никарагуа",
			"Jamaica" => "Ямайка",
			"Великобритания" => "Великобритания",
			_ => nation ?? ""
		};
	}

	public static string Abbrev(string nation)
	{
		if (nation != null && CountryAliases.TryGetValue(nation, out var alias))
		{
			nation = alias;
		}
		if (nation != null && Codes.TryGetValue(nation, out var code))
		{
			return code;
		}
		string letters = new string((nation ?? "").Where(char.IsLetter).ToArray()).ToUpperInvariant();
		if (letters.Length > 3)
		{
			return letters.Substring(0, 3);
		}
		return letters;
	}

	public string NationAbbrev(string nation)
	{
		if (CountryAbbrev.Length > 0 && string.Equals(nation, HomeCountry, StringComparison.OrdinalIgnoreCase))
		{
			return CountryAbbrev;
		}
		return Abbrev(nation);
	}

	public static List<string> KnownCountries()
	{
		List<string> list = new List<string>(Codes.Keys);
		list.Remove("Russia");
		list.Sort(StringComparer.OrdinalIgnoreCase);
		list.Insert(0, "Russia");
		return list.ConvertAll(NationLabel);
	}

	public List<string> UpcomingTournaments(int now)
	{
		List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();
		int today = ((now == int.MinValue) ? GameClock.DayNumber(new DateTime(2026, 1, 1)) : GameClock.DayOf(now));
		int year = GameClock.DateOf(today).Year;
		foreach (KeyValuePair<Sport, TourSpec> kv in Specs)
		{
			if (!PlaysSport(kv.Key))
			{
				continue;
			}
			for (int y = year; y <= year + 4; y++)
			{
				if (Happens(kv.Value, y))
				{
					int start = GameClock.DayNumber(StartOf(kv.Value, y));
					if (start >= today)
					{
						list.Add(new KeyValuePair<int, string>(start, y + " " + kv.Value.Name + ": " + StartOf(kv.Value, y).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)));
						break;
					}
				}
			}
		}
		list.Sort((KeyValuePair<int, string> a, KeyValuePair<int, string> b) => a.Key.CompareTo(b.Key));
		return list.ConvertAll((KeyValuePair<int, string> x) => x.Value);
	}

	public CommandResult SetCountry(string name, string abbrev, int now)
	{
		string clean = NameGenerator.CleanName(name ?? "", 32).Trim();
		string code = new string((abbrev ?? "").Where(char.IsLetter).ToArray()).ToUpperInvariant();
		if (code.Length > 4)
		{
			return CommandResult.Fail("Аббревиатура должна содержать от 2 до 4 букв, например RUS.");
		}
		if (clean.Length == 0)
		{
			CountryName = "";
			CountryAbbrev = "";
			Revision++;
			return CommandResult.Success("Ваш город снова находится в России.");
		}
		if (CountryAliases.TryGetValue(clean, out var known))
		{
			clean = known;
		}
		known = null;
		foreach (string k in Codes.Keys)
		{
			if (string.Equals(k, clean, StringComparison.OrdinalIgnoreCase))
			{
				known = k;
			}
		}
		if (known == null && code.Length < 2)
		{
			return CommandResult.Fail("Укажите для " + clean + " аббревиатуру из 2–4 букв, например " + Abbrev(clean) + ".");
		}
		if (code.Length == 1)
		{
			return CommandResult.Fail("Аббревиатура должна содержать от 2 до 4 букв.");
		}
		CountryName = known ?? clean;
		CountryAbbrev = ((known != null && code == Codes[known]) ? "" : code);
		Revision++;
		string home = NationLabel(HomeCountry);
		return CommandResult.Success("Теперь ваш город находится в стране " + home + " (" + HomeAbbrev + "). Игроки, выросшие в вашем городе, выступают за " + home + " в следующих международных турнирах, а " + home + " всегда получает хотя бы одно место в квалификации.");
	}

	public static string NationOf(Player p)
	{
		return NationOf(p, "Russia");
	}

	public string NationOfPlayer(Player p)
	{
		return NationOf(p, HomeCountry);
	}

	public string HomeNation(Sport sport)
	{
		return Normalize(HomeCountry, sport);
	}

	private static string NationOf(Player p, string home)
	{
		string town = p.Hometown ?? "";
		if (town.Length == 0)
		{
			return "";
		}
		if (town.StartsWith("*"))
		{
			return Normalize(home, p.Sport);
		}
		int comma = town.LastIndexOf(", ", StringComparison.Ordinal);
		if (comma < 0)
		{
			return "";
		}
		string tail = town.Substring(comma + 2).Trim();
		if (tail.Length == 2 && tail.ToUpperInvariant() == tail)
		{
			if (!Provinces.Contains(tail))
			{
				return "USA";
			}
			return "Canada";
		}
		return Normalize(tail, p.Sport);
	}

	private static string Normalize(string nation, Sport sport)
	{
		switch (nation)
		{
		case "England":
		case "Scotland":
		case "Wales":
			if (sport != Sport.Soccer)
			{
				return "Великобритания";
			}
			return nation;
		case "Korea":
			return "South Korea";
		case "American Samoa":
			return "USA";
		default:
			return nation;
		}
	}

	private static double ProEquivalent(Player p)
	{
		switch (p.Level)
		{
		case Level.Pro:
		case Level.Minor:
			return p.Overall;
		case Level.College:
		case Level.Junior:
			return 64.0 + (double)(p.Overall - 70) * 0.6;
		default:
			return 64.0 + (double)(p.Overall - 62) * 0.5;
		}
	}

	private static bool Happens(TourSpec spec, int year)
	{
		return year % spec.Every == spec.Offset;
	}

	private static DateTime StartOf(TourSpec spec, int year)
	{
		return new DateTime(year, spec.Month, spec.Day);
	}

	private bool PlaysSport(Sport sport)
	{
		foreach (League l in Leagues)
		{
			if (l.Sport == sport && KeepsRunning(l))
			{
				return true;
			}
		}
		return false;
	}

	private void InternationalMonthly(int now)
	{
		int today = GameClock.DayOf(now);
		int year = GameClock.DateOf(today).Year;
		foreach (KeyValuePair<Sport, TourSpec> kv in Specs)
		{
			if (!PlaysSport(kv.Key))
			{
				continue;
			}
			int y;
			for (y = year - 1; y <= year + 1; y++)
			{
				if (Happens(kv.Value, y))
				{
					int start = GameClock.DayNumber(StartOf(kv.Value, y));
					if (today >= start - 30 && today <= start + 5 && !Tournaments.Exists((Tournament tournament) => tournament.Sport == kv.Key && tournament.Year == y))
					{
						CreateTournament(kv.Key, kv.Value, y, start, now);
					}
				}
			}
		}
		foreach (Tournament t in Tournaments)
		{
			if (!t.Complete)
			{
				PlayTournament(t, now);
			}
		}
		while (Tournaments.Count > 12)
		{
			Tournaments.RemoveAt(0);
		}
	}

	private void CreateTournament(Sport sport, TourSpec spec, int year, int start, int now)
	{
		Tournament t = new Tournament
		{
			Id = NextId++,
			Sport = sport,
			Name = spec.Name,
			Year = year,
			StartDay = start,
			Under20 = spec.Under20
		};
		Random r = new Random(t.Id * 7919 + year);
		Dictionary<string, List<Player>> byNation = new Dictionary<string, List<Player>>();
		foreach (Player p in Players.Values)
		{
			if (p.Sport != sport || (p.Status != PlayerStatus.Active && p.Status != PlayerStatus.Prospect && p.Status != PlayerStatus.FreeAgent) || (spec.Under20 && year - p.BirthYear > 19))
			{
				continue;
			}
			string nation = NationOfPlayer(p);
			if (nation.Length != 0)
			{
				if (!byNation.TryGetValue(nation, out var list))
				{
					list = (byNation[nation] = new List<Player>());
				}
				list.Add(p);
			}
		}
		int shift = (spec.Under20 ? (-7) : 0);
		List<NationTeam> nations = new List<NationTeam>();
		List<string> names = new List<string>(spec.Nations);
		string home = HomeNation(sport);
		foreach (KeyValuePair<string, List<Player>> kv in byNation)
		{
			if (!names.Contains(kv.Key) && (kv.Value.Count >= 3 || kv.Key == home))
			{
				names.Add(kv.Key);
			}
		}
		foreach (string nation2 in names)
		{
			int idx = Array.IndexOf(spec.Nations, nation2);
			double depth = ((idx >= 0) ? spec.Depth[idx] : 52) + shift;
			byNation.TryGetValue(nation2, out var pool);
			NationTeam team = PickNationalTeam(sport, nation2, pool ?? new List<Player>(), depth, r);
			if (idx >= 0 || team.PlayerIds.Count >= ((nation2 == home) ? 1 : 3))
			{
				nations.Add(team);
			}
		}
		if (nations.Count < 4)
		{
			return;
		}
		double mean = 0.0;
		foreach (NationTeam n in nations)
		{
			mean += n.Strength;
		}
		mean /= (double)nations.Count;
		foreach (NationTeam n2 in nations)
		{
			n2.Rating = Math.Max(10, Math.Min(95, (int)Math.Round(50.0 + (n2.Strength - mean) * 2.5)));
		}
		nations.Sort((NationTeam a, NationTeam b) => b.Rating.CompareTo(a.Rating));
		t.Teams = nations;
		int field;
		if (nations.Count >= 16)
		{
			field = 16;
		}
		else
		{
			field = ((nations.Count >= 8) ? 8 : 4);
		}
		int direct = ((nations.Count >= field + 4) ? (field - 4) : field);
		int lastIn = ((direct < field) ? (direct + 8) : field) - 1;
		int homeAt = nations.FindIndex((NationTeam nationTeam) => nationTeam.Nation == home);
		if (homeAt > lastIn && lastIn >= 0)
		{
			NationTeam h = nations[homeAt];
			nations.RemoveAt(homeAt);
			nations.Insert(lastIn, h);
		}
		if (direct < field)
		{
			for (int k = 0; k < 4; k++)
			{
				int hi = direct + k;
				int lo = direct + 7 - k;
				t.Teams[hi].Qualifier = true;
				t.Teams[lo].Qualifier = true;
				t.Games.Add(new TourGame
				{
					Day = start - 6,
					Minute = 1140,
					Stage = "Qualifier",
					Home = lo,
					Away = hi
				});
				t.Games.Add(new TourGame
				{
					Day = start - 4,
					Minute = 1140,
					Stage = "Qualifier",
					Home = hi,
					Away = lo
				});
			}
		}
		else
		{
			for (int i = 0; i < field; i++)
			{
				t.Teams[i].Group = 0;
			}
			DrawGroups(t, field, r);
		}
		List<string> hosts = new List<string>();
		for (int i2 = 0; i2 < Math.Min(field, t.Teams.Count); i2++)
		{
			hosts.Add(t.Teams[i2].Nation);
		}
		t.Host = hosts[r.Next(hosts.Count)];
		Tournaments.Add(t);
		AnnounceRosters(t, now);
		Revision++;
	}

	private NationTeam PickNationalTeam(Sport sport, string nation, List<Player> pool, double depth, Random r)
	{
		NationTeam team = new NationTeam
		{
			Nation = nation
		};
		double strength = 0.0;
		int starters = 0;
		RosterSlot[] array = Rosters.Template(sport, Level.Pro);
		foreach (RosterSlot slot in array)
		{
			List<Player> atPos = pool.FindAll((Player p) => p.Position == slot.Pos && ProEquivalent(p) >= depth);
			atPos.Sort((Player a, Player b) => ProEquivalent(b).CompareTo(ProEquivalent(a)));
			for (int i2 = 0; i2 < slot.Count; i2++)
			{
				double value = ((i2 < atPos.Count) ? ProEquivalent(atPos[i2]) : depth);
				if (i2 < atPos.Count)
				{
					team.PlayerIds.Add(atPos[i2].Id);
				}
				else
				{
					team.Unnamed++;
				}
				if (i2 < slot.Starters)
				{
					strength += value;
					starters++;
				}
			}
		}
		team.Strength = ((starters > 0) ? (strength / (double)starters) : depth) + Rosters.Normal(r, 0.0, 0.6);
		return team;
	}

	private static void DrawGroups(Tournament t, int field, Random r)
	{
		List<int> inField = new List<int>();
		for (int i = 0; i < t.Teams.Count; i++)
		{
			if (t.Teams[i].Group >= 0)
			{
				inField.Add(i);
			}
		}
		inField.Sort((int a, int b) => t.Teams[b].Rating.CompareTo(t.Teams[a].Rating));
		int groups = field / 4;
		for (int pot = 0; pot < 4; pot++)
		{
			List<int> potTeams = inField.GetRange(pot * groups, groups);
			for (int i2 = potTeams.Count - 1; i2 > 0; i2--)
			{
				int j = r.Next(i2 + 1);
				int x = potTeams[i2];
				potTeams[i2] = potTeams[j];
				potTeams[j] = x;
			}
			for (int g = 0; g < groups; g++)
			{
				t.Teams[potTeams[g]].Group = g;
			}
		}
		int[] times = new int[3] { 780, 990, 1200 };
		for (int g2 = 0; g2 < groups; g2++)
		{
			List<int> members = new List<int>();
			for (int i3 = 0; i3 < t.Teams.Count; i3++)
			{
				if (t.Teams[i3].Group == g2)
				{
					members.Add(i3);
				}
			}
			int[,] rounds = new int[3, 4]
			{
				{ 0, 3, 1, 2 },
				{ 0, 2, 3, 1 },
				{ 0, 1, 2, 3 }
			};
			for (int round = 0; round < 3; round++)
			{
				for (int m = 0; m < 2; m++)
				{
					t.Games.Add(new TourGame
					{
						Day = t.StartDay + round * 2 + g2 % 2,
						Minute = times[(g2 + m) % 3],
						Stage = "Group " + (char)(65 + g2),
						Home = members[rounds[round, m * 2]],
						Away = members[rounds[round, m * 2 + 1]]
					});
				}
			}
		}
	}

	private int MainEventSize(Tournament t)
	{
		int n = 0;
		foreach (NationTeam team in t.Teams)
		{
			if (team.Group >= 0)
			{
				n++;
			}
		}
		return n;
	}

	private void PlayTournament(Tournament t, int now)
	{
		int monthStart = GameClock.MonthStartDay(now);
		GameSimulator sim = new GameSimulator(new Random(t.Id * 31 + t.Games.Count * 7 + monthStart));
		for (int guard = 0; guard < 10; guard++)
		{
			bool any = false;
			foreach (TourGame g in t.Games)
			{
				if (!g.Played && g.Day < monthStart)
				{
					PlayTourGame(t, g, sim);
					any = true;
				}
			}
			if (!NextRound(t, now) && !any)
			{
				break;
			}
		}
	}

	private void PlayTourGame(Tournament t, TourGame g, GameSimulator sim)
	{
		bool knockout = g.Stage != "Qualifier" && !g.Stage.StartsWith("Group");
		Match m = new Match
		{
			Id = -1,
			HomeTeamId = -1,
			AwayTeamId = -2,
			VenueId = -2,
			Stage = (knockout ? GameStage.Playoff : GameStage.Regular)
		};
		sim.Play(m, t.Sport, Level.Pro, t.Teams[g.Home].Rating, t.Teams[g.Away].Rating);
		g.HomeScore = m.HomeScore;
		g.AwayScore = m.AwayScore;
		g.Suffix = ((m.Result == ResultType.Penalties) ? "П" : SportRules.ResultSuffix(t.Sport, m.Result));
		if (m.Result == ResultType.Penalties)
		{
			g.TieWinner = ((m.HomePenalties > m.AwayPenalties) ? g.Home : g.Away);
		}
		g.Played = true;
		Revision++;
	}

	private static int Winner(TourGame g)
	{
		if (g.HomeScore == g.AwayScore)
		{
			if (g.TieWinner < 0)
			{
				return g.Home;
			}
			return g.TieWinner;
		}
		if (g.HomeScore <= g.AwayScore)
		{
			return g.Away;
		}
		return g.Home;
	}

	private static int Loser(TourGame g)
	{
		if (Winner(g) != g.Home)
		{
			return g.Home;
		}
		return g.Away;
	}

	private bool NextRound(Tournament t, int now)
	{
		if (t.Complete)
		{
			return false;
		}
		List<TourGame> quals = t.Games.FindAll((TourGame tourGame2) => tourGame2.Stage == "Qualifier");
		if (quals.Count > 0 && MainEventSize(t) == 0)
		{
			if (!quals.TrueForAll((TourGame tourGame2) => tourGame2.Played))
			{
				return false;
			}
			for (int k = 0; k + 1 < quals.Count; k += 2)
			{
				TourGame tourGame = quals[k];
				TourGame b = quals[k + 1];
				int x = tourGame.Away;
				int y = tourGame.Home;
				int gx = tourGame.AwayScore + b.HomeScore;
				int gy = tourGame.HomeScore + b.AwayScore;
				int through;
				if (gx != gy)
				{
					through = ((gx > gy) ? x : y);
				}
				else
				{
					through = ((t.Teams[x].Rating >= t.Teams[y].Rating) ? x : y);
				}
				t.Teams[through].Group = 0;
			}
			for (int i = 0; i < t.Teams.Count && i < 16; i++)
			{
				if (!t.Teams[i].Qualifier)
				{
					t.Teams[i].Group = 0;
				}
			}
			int field = MainEventSize(t);
			DrawGroups(t, field - field % 4, new Random(t.Id * 13 + 5));
			return true;
		}
		List<TourGame> groupGames = t.Games.FindAll((TourGame tourGame2) => tourGame2.Stage.StartsWith("Group"));
		if (groupGames.Count == 0 || !groupGames.TrueForAll((TourGame tourGame2) => tourGame2.Played))
		{
			return false;
		}
		int groups = MainEventSize(t) / 4;
		List<TourGame> qf = t.Games.FindAll((TourGame tourGame2) => tourGame2.Stage == "Quarterfinal");
		List<TourGame> sf = t.Games.FindAll((TourGame tourGame2) => tourGame2.Stage == "Semifinal");
		if (groups >= 4 && qf.Count == 0)
		{
			List<int>[] tables = new List<int>[groups];
			for (int g = 0; g < groups; g++)
			{
				tables[g] = GroupTable(t, g);
			}
			int[,] pairs = new int[4, 2]
			{
				{ 0, 1 },
				{ 1, 0 },
				{ 2, 3 },
				{ 3, 2 }
			};
			for (int k2 = 0; k2 < 4; k2++)
			{
				t.Games.Add(new TourGame
				{
					Day = t.StartDay + 6,
					Minute = (new int[4] { 780, 990, 1080, 1230 })[k2],
					Stage = "Quarterfinal",
					Home = tables[pairs[k2, 0]][0],
					Away = tables[pairs[k2, 1]][1]
				});
			}
			return true;
		}
		if (sf.Count == 0)
		{
			if (groups >= 4)
			{
				qf = t.Games.FindAll((TourGame tourGame2) => tourGame2.Stage == "Quarterfinal");
				if (!qf.TrueForAll((TourGame tourGame2) => tourGame2.Played))
				{
					return false;
				}
				t.Games.Add(new TourGame
				{
					Day = t.StartDay + 8,
					Minute = 960,
					Stage = "Semifinal",
					Home = Winner(qf[0]),
					Away = Winner(qf[2])
				});
				t.Games.Add(new TourGame
				{
					Day = t.StartDay + 8,
					Minute = 1200,
					Stage = "Semifinal",
					Home = Winner(qf[1]),
					Away = Winner(qf[3])
				});
			}
			else if (groups == 2)
			{
				List<int> a = GroupTable(t, 0);
				List<int> b2 = GroupTable(t, 1);
				t.Games.Add(new TourGame
				{
					Day = t.StartDay + 8,
					Minute = 960,
					Stage = "Semifinal",
					Home = a[0],
					Away = b2[1]
				});
				t.Games.Add(new TourGame
				{
					Day = t.StartDay + 8,
					Minute = 1200,
					Stage = "Semifinal",
					Home = b2[0],
					Away = a[1]
				});
			}
			else
			{
				List<int> a2 = GroupTable(t, 0);
				t.Games.Add(new TourGame
				{
					Day = t.StartDay + 10,
					Minute = 900,
					Stage = "Матч за бронзовые медали",
					Home = a2[2],
					Away = a2[3]
				});
				t.Games.Add(new TourGame
				{
					Day = t.StartDay + 10,
					Minute = 1200,
					Stage = "Матч за золотые медали",
					Home = a2[0],
					Away = a2[1]
				});
			}
			return true;
		}
		List<TourGame> finals = t.Games.FindAll((TourGame tourGame2) => tourGame2.Stage == "Матч за золотые медали");
		if (finals.Count == 0)
		{
			if (!sf.TrueForAll((TourGame tourGame2) => tourGame2.Played))
			{
				return false;
			}
			t.Games.Add(new TourGame
			{
				Day = t.StartDay + 10,
				Minute = 900,
				Stage = "Матч за бронзовые медали",
				Home = Loser(sf[0]),
				Away = Loser(sf[1])
			});
			t.Games.Add(new TourGame
			{
				Day = t.StartDay + 10,
				Minute = 1200,
				Stage = "Матч за золотые медали",
				Home = Winner(sf[0]),
				Away = Winner(sf[1])
			});
			return true;
		}
		TourGame final = finals[0];
		TourGame bronze = t.Games.Find((TourGame tourGame2) => tourGame2.Stage == "Матч за бронзовые медали");
		if (!final.Played || bronze == null || !bronze.Played)
		{
			return false;
		}
		t.Gold = t.Teams[Winner(final)].Nation;
		t.Silver = t.Teams[Loser(final)].Nation;
		t.Bronze = t.Teams[Winner(bronze)].Nation;
		t.Complete = true;
		AwardMedals(t, final, bronze, now);
		return true;
	}

	public List<int> GroupTable(Tournament t, int group)
	{
		List<int> members = new List<int>();
		for (int i = 0; i < t.Teams.Count; i++)
		{
			if (t.Teams[i].Group == group)
			{
				members.Add(i);
			}
		}
		Dictionary<int, int[]> rows = new Dictionary<int, int[]>();
		foreach (int i2 in members)
		{
			rows[i2] = new int[7];
		}
		string stage = "Группа " + (char)(65 + group);
		foreach (TourGame g in t.Games)
		{
			if (g.Stage != stage || !g.Played)
			{
				continue;
			}
			bool[] array = new bool[2] { true, false };
			foreach (bool home in array)
			{
				int me = (home ? g.Home : g.Away);
				if (!rows.TryGetValue(me, out var row))
				{
					continue;
				}
				int f = (home ? g.HomeScore : g.AwayScore);
				int a = (home ? g.AwayScore : g.HomeScore);
				row[6]++;
				row[1] += f - a;
				row[2] += f;
				bool extra = g.Suffix.Length > 0;
				if (f > a)
				{
					row[3]++;
					row[0] += (((t.Sport == Sport.Hockey) & extra) ? 2 : ((t.Sport == Sport.Soccer || t.Sport == Sport.Hockey) ? 3 : 2));
					continue;
				}
				if (f == a)
				{
					row[4]++;
					row[0]++;
					continue;
				}
				row[5]++;
				if ((t.Sport == Sport.Hockey) & extra)
				{
					row[0]++;
				}
			}
		}
		members.Sort((int x, int y) =>
		{
			int[] array2 = rows[x];
			int[] array3 = rows[y];
			if (array2[0] != array3[0])
			{
				return array3[0].CompareTo(array2[0]);
			}
			if (array2[1] != array3[1])
			{
				return array3[1].CompareTo(array2[1]);
			}
			return (array2[2] != array3[2]) ? array3[2].CompareTo(array2[2]) : t.Teams[y].Rating.CompareTo(t.Teams[x].Rating);
		});
		_groupRows = rows;
		return members;
	}

	public int[] GroupRow(int team)
	{
		if (_groupRows == null || !_groupRows.TryGetValue(team, out var row))
		{
			return new int[7];
		}
		return row;
	}

	private League NewsLeague(Sport sport)
	{
		League pro = GetLeague(sport, Level.Pro);
		if (pro != null && KeepsRunning(pro))
		{
			return pro;
		}
		foreach (League l in OrderedLeagues())
		{
			if (l.Sport == sport && KeepsRunning(l))
			{
				return l;
			}
		}
		return pro;
	}

	private string ClubOf(Player p)
	{
		Team t = GetTeam(p.TeamId);
		if (t == null)
		{
			if (p.Status != PlayerStatus.FreeAgent)
			{
				return "";
			}
			return "свободный агент";
		}
		return TeamName(t.Id);
	}

	private void AnnounceRosters(Tournament t, int now)
	{
		League league = NewsLeague(t.Sport);
		if (league == null)
		{
			return;
		}
		List<string> mine = new List<string>();
		foreach (NationTeam n in t.Teams)
		{
			foreach (int id in n.PlayerIds)
			{
				Player p = GetPlayer(id);
				Team club = ((p == null) ? null : GetTeam(p.TeamId));
				if (club != null && club.IsUser)
				{
					mine.Add(Rosters.NewsPositionLabel(p.Position, p.Sport) + " " + p.Name + " (" + NationLabel(n.Nation) + ", " + TeamName(club.Id) + ")");
				}
			}
		}
		string when = GameClock.DateOf(t.StartDay).ToString("dd.MM", CultureInfo.InvariantCulture);
		string homeLine = "";
		int hi = t.Teams.FindIndex((NationTeam nationTeam) => nationTeam.Nation == HomeNation(t.Sport));
		int firstQualifier = t.Teams.FindIndex((NationTeam nationTeam) => nationTeam.Qualifier);
		if (hi >= 0 && (t.Teams[hi].Group >= 0 || t.Teams[hi].Qualifier || (firstQualifier >= 0 && hi < firstQualifier)))
		{
			NationTeam h = t.Teams[hi];
			int local = h.PlayerIds.FindAll((int id2) =>
			{
				Player player = GetPlayer(id2);
				return player != null && (player.Hometown ?? "").StartsWith("*");
			}).Count;
			homeLine = " " + NationLabel(h.Nation) + ", страна вашего города, " + (h.Qualifier ? "участвует в квалификации" : "вышла в основной турнир") + ((local > 0) ? (", в составе " + local + (local == 1 ? " игрок, выросший в вашем городе" : " игрока, выросших в вашем городе")) : "") + ".";
		}
		string text = "СБОРНЫЕ: турнир «" + t.Label + "» начинается " + when + " в городе " + t.Host + "." + homeLine + ((mine.Count > 0) ? (" Ваши игроки вызваны в сборные: " + string.Join("; ", mine.GetRange(0, Math.Min(8, mine.Count)).ToArray()) + ((mine.Count > 8) ? (" и ещё " + (mine.Count - 8)) : "") + ".") : "");
		Post(league, NewsKind.General, Wire, text, mine.Count > 0 || homeLine.Length > 0, now, (mine.Count <= 0 && homeLine.Length <= 0) ? 1 : 2);
	}

	private void AwardMedals(Tournament t, TourGame final, TourGame bronze, int now)
	{
		string[] medal = new string[3] { "Золотая медаль", "Серебряная медаль", "Бронзовая медаль" };
		int[] who = new int[3]
		{
			Winner(final),
			Loser(final),
			Winner(bronze)
		};
		List<string> mine = new List<string>();
		for (int k = 0; k < 3; k++)
		{
			foreach (int id in t.Teams[who[k]].PlayerIds)
			{
				Player p = GetPlayer(id);
				if (p != null)
				{
					AddHonor(p, p.Level, t.Year.ToString(CultureInfo.InvariantCulture), medal[k], t.Name + " (" + NationLabel(t.Teams[who[k]].Nation) + ")");
					Team club = GetTeam(p.TeamId);
					if (club != null && club.IsUser)
					{
						mine.Add(p.Name + " (" + medal[k].Split(new[] { ' ' }, StringSplitOptions.None)[0].ToLowerInvariant() + " со сборной " + NationLabel(t.Teams[who[k]].Nation) + ")");
					}
				}
			}
		}
		League league = NewsLeague(t.Sport);
		if (league != null)
		{
			Post(league, NewsKind.Champion, Wire, "ЗОЛОТО: " + t.Gold + " победила " + t.Silver + " " + Math.Max(final.HomeScore, final.AwayScore) + "-" + Math.Min(final.HomeScore, final.AwayScore) + ((final.Suffix.Length > 0) ? (" (" + final.Suffix + ")") : "") + " и завоевала " + t.Label + ". Бронза: " + t.Bronze + "." + ((mine.Count > 0) ? (" Медали ваших игроков: " + string.Join("; ", mine.ToArray()) + ".") : ""), mine.Count > 0, now, 3);
		}
	}

	public Team ParentOf(Team t)
	{
		if (t == null || t.Level != Level.Minor || t.ParentTeamId < 0)
		{
			return null;
		}
		Team p = GetTeam(t.ParentTeamId);
		if (p == null || p.Retired || p.Level != Level.Pro || p.Sport != t.Sport)
		{
			return null;
		}
		return p;
	}

	public Team AffiliateOf(Team pro)
	{
		if (pro == null || pro.Level != Level.Pro || !SportRules.HasMinors(pro.Sport))
		{
			return null;
		}
		League minors = GetLeague(pro.Sport, Level.Minor);
		if (minors == null)
		{
			return null;
		}
		foreach (int id in TeamIdsOf(minors))
		{
			Team t = GetTeam(id);
			if (t != null && !t.Retired && t.ParentTeamId == pro.Id)
			{
				return t;
			}
		}
		return null;
	}

	private static List<int> TeamIdsOf(League league)
	{
		List<int> ids = new List<int>(league.TeamIds);
		foreach (int id in league.PendingTeamIds)
		{
			if (!ids.Contains(id))
			{
				ids.Add(id);
			}
		}
		return ids;
	}

	public static string FarmPrefLabel(int pref)
	{
		return pref switch
		{
			2 => "За пределами города", 
			1 => "В вашем городе", 
			_ => "Автоматически", 
		};
	}

	private List<Team> UserTeamsAt(Sport sport, Level level)
	{
		List<Team> list = new List<Team>();
		League l = GetLeague(sport, level);
		if (l == null)
		{
			return list;
		}
		foreach (int id in TeamIdsOf(l))
		{
			Team t = GetTeam(id);
			if (t != null && !t.Retired && t.IsUser)
			{
				list.Add(t);
			}
		}
		list.Sort((Team a, Team b) => a.Id.CompareTo(b.Id));
		return list;
	}

	private Dictionary<int, KeyValuePair<Team, bool>> FarmPlans(Sport sport)
	{
		Dictionary<int, KeyValuePair<Team, bool>> plans = new Dictionary<int, KeyValuePair<Team, bool>>();
		if (!SportRules.HasMinors(sport))
		{
			return plans;
		}
		List<Team> pros = UserTeamsAt(sport, Level.Pro);
		if (pros.Count == 0)
		{
			return plans;
		}
		List<Team> city = UserTeamsAt(sport, Level.Minor);
		int next = 0;
		List<Team> list = new List<Team>(pros);
		list.Sort((Team a, Team b) => (((a.FarmPref != 1) ? 1 : 0).CompareTo((b.FarmPref != 1) ? 1 : 0) == 0) ? a.Id.CompareTo(b.Id) : ((a.FarmPref != 1) ? 1 : 0).CompareTo((b.FarmPref != 1) ? 1 : 0));
		foreach (Team pro in list)
		{
			if (pro.FarmPref == 2)
			{
				plans[pro.Id] = new KeyValuePair<Team, bool>(null, value: true);
			}
			else if (next < city.Count)
			{
				plans[pro.Id] = new KeyValuePair<Team, bool>(city[next++], value: false);
			}
			else
			{
				plans[pro.Id] = new KeyValuePair<Team, bool>(null, pro.FarmPref == 0);
			}
		}
		return plans;
	}

	public bool WantsOutOfTownFarm(Sport sport)
	{
		foreach (KeyValuePair<Team, bool> value in FarmPlans(sport).Values)
		{
			if (value.Value)
			{
				return true;
			}
		}
		return false;
	}

	public bool KeepsRunning(League league)
	{
		if (CountUserTeams(league) > 0)
		{
			return true;
		}
		if (league.Sport == Sport.Soccer && SportRules.ProLike(league.Sport, league.Level))
		{
			League other = GetLeague(Sport.Soccer, (league.Level == Level.Pro) ? Level.Minor : Level.Pro);
			if (other != null)
			{
				return CountUserTeams(other) > 0;
			}
			return false;
		}
		if (league.Level == Level.Minor)
		{
			return WantsOutOfTownFarm(league.Sport);
		}
		return false;
	}

	private void StopUnneededFarmLeagues()
	{
		foreach (League league in Leagues)
		{
			bool soccerPro = league.Sport == Sport.Soccer && league.Level == Level.Pro;
			if ((league.Level == Level.Minor || soccerPro) && league.Current != null && league.PendingTeamIds.Count <= 0 && !KeepsRunning(league))
			{
				AbandonSeason(league);
			}
		}
	}

	private void EnsureFarmLeagues(int now)
	{
		Sport[] allSports = SportRules.AllSports;
		foreach (Sport sport in allSports)
		{
			if (SportRules.HasMinors(sport))
			{
				League minors = GetLeague(sport, Level.Minor);
				if ((minors == null || (minors.Current == null && minors.TeamIds.Count <= 0)) && WantsOutOfTownFarm(sport))
				{
					minors = GetOrCreateLeague(sport, Level.Minor);
					BuildSeason(minors, now);
				}
			}
		}
	}

	public void LinkAffiliates()
	{
		if (Revision == _linkedRevision)
		{
			return;
		}
		bool changed = false;
		Sport[] allSports = SportRules.AllSports;
		foreach (Sport sport in allSports)
		{
			if (!SportRules.HasMinors(sport))
			{
				continue;
			}
			League minorLeague = GetLeague(sport, Level.Minor);
			if (minorLeague == null)
			{
				continue;
			}
			League proLeague = GetLeague(sport, Level.Pro);
			List<Team> minors = new List<Team>();
			List<Team> pros = new List<Team>();
			foreach (int id in TeamIdsOf(minorLeague))
			{
				Team t = GetTeam(id);
				if (t != null && !t.Retired)
				{
					minors.Add(t);
				}
			}
			if (proLeague != null)
			{
				foreach (int id2 in TeamIdsOf(proLeague))
				{
					Team t2 = GetTeam(id2);
					if (t2 != null && !t2.Retired)
					{
						pros.Add(t2);
					}
				}
			}
			minors.Sort((Team a, Team b) =>
			{
				if (a.IsUser == b.IsUser)
				{
					return a.Id.CompareTo(b.Id);
				}
				return (!a.IsUser) ? 1 : (-1);
			});
			pros.Sort((Team a, Team b) => a.Id.CompareTo(b.Id));
			Dictionary<int, int> want = new Dictionary<int, int>();
			HashSet<int> takenPros = new HashSet<int>();
			HashSet<int> takenMinors = new HashSet<int>();
			Dictionary<int, KeyValuePair<Team, bool>> plans = FarmPlans(sport);
			foreach (Team pro in pros)
			{
				if (!pro.IsUser || !plans.TryGetValue(pro.Id, out var plan))
				{
					continue;
				}
				Team pick = plan.Key;
				if (pick == null && plan.Value)
				{
					pick = minors.Find((Team team) => !team.IsUser && team.ParentTeamId == pro.Id && !takenMinors.Contains(team.Id)) ?? minors.Find((Team team) => !team.IsUser && !takenMinors.Contains(team.Id) && (team.ParentTeamId < 0 || GetTeam(team.ParentTeamId) == null || !GetTeam(team.ParentTeamId).IsUser));
					if (pick == null)
					{
						pick = minors.Find((Team team) => !team.IsUser && !takenMinors.Contains(team.Id));
					}
				}
				if (pick != null && minors.Contains(pick) && !takenMinors.Contains(pick.Id))
				{
					want[pick.Id] = pro.Id;
					takenMinors.Add(pick.Id);
					takenPros.Add(pro.Id);
				}
			}
			foreach (Team m in minors)
			{
				if (!takenMinors.Contains(m.Id))
				{
					Team parent = ParentOf(m);
					if (parent != null && pros.Contains(parent) && !takenPros.Contains(parent.Id) && !parent.IsUser)
					{
						want[m.Id] = parent.Id;
						takenPros.Add(parent.Id);
						takenMinors.Add(m.Id);
					}
				}
			}
			foreach (Team m2 in minors)
			{
				if (takenMinors.Contains(m2.Id))
				{
					continue;
				}
				List<Team> open = pros.FindAll((Team x) => !takenPros.Contains(x.Id) && !x.IsUser);
				if (open.Count == 0)
				{
					open = pros.FindAll((Team x) => !takenPros.Contains(x.Id) && (!x.IsUser || x.FarmPref != 2));
				}
				if (open.Count != 0)
				{
					Team pick2 = open[new Random(m2.Id * 31 + 7).Next(open.Count)];
					want[m2.Id] = pick2.Id;
					takenPros.Add(pick2.Id);
					takenMinors.Add(m2.Id);
				}
			}
			foreach (Team m3 in minors)
			{
				if (!want.TryGetValue(m3.Id, out var parent2))
				{
					parent2 = -1;
				}
				if (m3.ParentTeamId != parent2)
				{
					m3.ParentTeamId = parent2;
					changed = true;
				}
			}
		}
		if (changed)
		{
			Revision++;
		}
		_linkedRevision = Revision;
	}

	public CommandResult SetFarmPref(int teamId, int pref, int now)
	{
		_clockHint = now;
		Team t = GetTeam(teamId);
		if (t == null || t.Retired || !t.IsUser || t.Level != Level.Pro)
		{
			return CommandResult.Fail("Выберите один из ваших профессиональных клубов.");
		}
		if (!SportRules.HasMinors(t.Sport))
		{
			return CommandResult.Fail(SportRules.SportLabel(t.Sport) + " не имеет команды малой лиги.");
		}
		if (pref < 0 || pref > 2)
		{
			return CommandResult.Fail("Неизвестный вариант.");
		}
		t.FarmPref = pref;
		Revision++;
		EnsureFarmLeagues(now);
		LinkAffiliates();
		Team farm = AffiliateOf(t);
		string where;
		if (farm != null)
		{
			where = " Фарм-клуб: " + TeamName(farm.Id) + (farm.IsUser ? " (ваш город)." : " (другой город).");
		}
		else
		{
			where = ((pref == 1) ? (" У вас пока нет свободной команды малой лиги " + SportRules.SportLabel(t.Sport).ToLowerInvariant() + " в вашем городе: сделайте здание спортивным объектом, выберите уровень «Малая лига» и добавьте команду.") : "");
		}
		return CommandResult.Success("Фарм-клуб: " + FarmPrefLabel(pref).ToLowerInvariant() + "." + where);
	}

	private void ChangeLevel(Player p, Level to)
	{
		if (p.Level == to)
		{
			return;
		}
		League from = GetLeague(p.Sport, p.Level);
		if (!Stats.IsEmpty(p.Season) || !Stats.IsEmpty(p.Playoff))
		{
			string label = ((from != null && from.Current != null) ? SeasonLabel(from, from.Current) : CurrentYear().ToString());
			int teamId = ((p.TeamId >= 0) ? p.TeamId : p.LastTeamId);
			p.History.Add(new SeasonLine
			{
				Season = label,
				TeamId = teamId,
				Team = ((teamId >= 0) ? TeamName(teamId) : ""),
				Level = p.Level,
				Ovr = Rosters.Ovr(p),
				Stats = (int[])Stats.Fit(p.Season, p.Sport).Clone(),
				Playoff = (Stats.IsEmpty(p.Playoff) ? new int[0] : ((int[])Stats.Fit(p.Playoff, p.Sport).Clone()))
			});
			while (p.History.Count > 30)
			{
				p.History.RemoveAt(0);
			}
		}
		p.Level = to;
		p.Season = Stats.Empty(p.Sport);
		p.Playoff = new int[0];
		int[] career = Stats.Empty(p.Sport);
		foreach (SeasonLine l in p.History)
		{
			if (l.Level == to)
			{
				Stats.AddInto(career, Stats.Fit(l.Stats, p.Sport));
			}
		}
		p.Career = career;
	}

	public string SendDownBlock(Team pro, Player p)
	{
		if (pro == null || p == null || p.TeamId != pro.Id || p.Level != Level.Pro)
		{
			return "Этот игрок не входит в состав команды.";
		}
		if (AffiliateOf(pro) == null)
		{
			return "Фарм-клуб отсутствует.";
		}
		if (AgeOf(p) > 25)
		{
			return "Только игроки " + 25 + " лет и младше могут быть отправлены в фарм-клуб.";
		}
		if (p.Salary > Economy.Get(pro.Sport).MinSalary * 2)
		{
			return "В фарм-клуб можно отправлять только игроков с недорогими двусторонними контрактами.";
		}
		return null;
	}

	public string CallUpBlock(Team pro, Player p)
	{
		Team farm = AffiliateOf(pro);
		if (farm == null || p == null || p.TeamId != farm.Id || p.Level != Level.Minor)
		{
			return "Этот игрок не входит в ваш фарм-клуб.";
		}
		if (NoRoomFor(pro, p) != null)
		{
			return "В составе нет свободного места.";
		}
		if (!Economy.CanAfford(pro, SignedPlayers(pro.Id), Economy.Get(pro.Sport).MinSalary, resign: false))
		{
			return "Недостаточно места под потолком зарплат даже для минимального контракта.";
		}
		return null;
	}

	private void CallUpQuiet(Team pro, Team farm, Player p, int now)
	{
		EconomyProfile e = Economy.Get(pro.Sport);
		ChangeLevel(p, Level.Pro);
		if (string.IsNullOrEmpty(p.From))
		{
			p.From = TeamName(farm.Id);
		}
		p.TeamId = pro.Id;
		p.Status = PlayerStatus.Active;
		p.Salary = e.MinSalary;
		p.ContractYears = ((AgeOf(p) > 24) ? 1 : 2);
		p.FreeAgentMonth = -1;
		p.DecideMonth = -1;
		p.Number = NumberFor(pro, p.Position);
		RostersChanged();
		RefillFarm(farm, now);
	}

	private void DoCallUp(Team pro, Team farm, Player p, int now, bool announce)
	{
		CallUpQuiet(pro, farm, p, now);
		ArrangeRoster(pro, now);
		UpdateRating(pro);
		League league = LeagueOf(pro);
		if (announce && league != null)
		{
			if (pro.IsUser)
			{
				PostAsTeam(league, pro, NewsKind.Signing, "Мы вызвали в основную команду " + Rosters.NewsPositionLabel(p.Position, p.Sport) + " " + p.Name + " (" + Rosters.Ovr(p) + " ОБЩ) из фарм-клуба " + TeamName(farm.Id) + ".", now);
			}
			else if (p.Overall >= Rosters.Base(Level.Pro))
			{
				Post(league, NewsKind.Signing, Insider, "ВЫЗВАН В ОСНОВУ: " + TeamName(pro.Id) + " вызывает в основную команду " + Rosters.NewsPositionLabel(p.Position, p.Sport) + " " + p.Name + " из " + TeamName(farm.Id) + ".", isUser: false, now, 2);
			}
		}
	}

	private void SendDownQuiet(Team pro, Team farm, Player p)
	{
		pro.Block.Remove(p.Id);
		JoinMinorClub(farm, p);
	}

	private void JoinMinorClub(Team farm, Player p)
	{
		ChangeLevel(p, Level.Minor);
		p.TeamId = farm.Id;
		p.Status = PlayerStatus.Active;
		p.Salary = 0;
		p.ContractYears = 0;
		p.FreeAgentMonth = -1;
		p.DecideMonth = -1;
		int slots = 0;
		RosterSlot[] array = Rosters.Template(farm.Sport, Level.Minor);
		foreach (RosterSlot s in array)
		{
			if (s.Pos == p.Position)
			{
				slots = s.Count;
			}
		}
		List<Player> atPos = RosterOf(farm.Id).FindAll((Player x) => x.Position == p.Position && x != p);
		if (atPos.Count >= slots && atPos.Count > 0)
		{
			atPos.Sort((Player a, Player b) => (FarmValue(a) == FarmValue(b)) ? a.Id.CompareTo(b.Id) : FarmValue(a).CompareTo(FarmValue(b)));
			RemovePlayer(atPos[0]);
		}
		p.Number = NumberFor(farm, p.Position);
		RostersChanged();
		UpdateRating(farm);
	}

	private void DoSendDown(Team pro, Team farm, Player p, int now, bool announce)
	{
		SendDownQuiet(pro, farm, p);
		FillHoles(pro, CurrentYear(), now, p);
		UpdateRating(pro);
		League league = LeagueOf(pro);
		if (announce && league != null && pro.IsUser)
		{
			PostAsTeam(league, pro, NewsKind.Signing, "Мы отправили " + Rosters.NewsPositionLabel(p.Position, p.Sport) + " " + p.Name + " во вторую лигу " + TeamName(farm.Id) + " получать больше игровой практики.", now);
		}
	}

	private double FarmValue(Player p)
	{
		int age = AgeOf(p);
		return (double)p.Overall + (double)Math.Max(0, p.Potential - p.Overall) * 0.5 - (double)Math.Max(0, age - 27) * 1.0;
	}

	private bool ToSecondTier(Player p)
	{
		if (p.Sport == Sport.Soccer)
		{
			return ToSecondDivision(p);
		}
		League minors = (SportRules.HasMinors(p.Sport) ? GetLeague(p.Sport, Level.Minor) : null);
		if (minors == null)
		{
			return false;
		}
		List<Team> clubs = new List<Team>();
		foreach (int id in minors.TeamIds)
		{
			Team t = GetTeam(id);
			if (t != null && !t.Retired)
			{
				clubs.Add(t);
			}
		}
		if (clubs.Count == 0)
		{
			return false;
		}
		if (p.TeamId >= 0)
		{
			p.LastTeamId = p.TeamId;
		}
		JoinMinorClub(clubs[_random.Next(clubs.Count)], p);
		return true;
	}

	public static string SecondTierName(Sport sport)
	{
		if (sport != Sport.Soccer)
		{
			return "малые лиги";
		}
		return "второй дивизион";
	}

	public CommandResult CallUp(int teamId, int playerId, int now)
	{
		_clockHint = now;
		Team t = UserProTeam(teamId, out var error);
		if (t == null)
		{
			return CommandResult.Fail(error);
		}
		if (!t.Manual)
		{
			return CommandResult.Fail("Сначала выберите «Самостоятельное управление командой» в офисе клуба, чтобы вызывать игроков в основной состав.");
		}
		Team farm = AffiliateOf(t);
		if (farm == null)
		{
			return CommandResult.Fail("" + TeamName(t.Id) + " не имеет фарм-клуба.");
		}
		Player p = GetPlayer(playerId);
		string block = CallUpBlock(t, p);
		if (block != null)
		{
			if (block == "В составе нет свободного места.")
			{
				return CommandResult.Fail(NoRoomFor(t, p));
			}
			return CommandResult.Fail(block);
		}
		DoCallUp(t, farm, p, now, announce: true);
		Revision++;
		return CommandResult.Success(p.Name + " вызван в основную команду по минимальному контракту (" + Economy.ContractText(p) + "). " + ((p.Status == PlayerStatus.Active) ? "Он включён в основной состав." : "Он начнёт в резерве."));
	}

	public CommandResult SendDown(int teamId, int playerId, int now)
	{
		_clockHint = now;
		Team t = UserProTeam(teamId, out var error);
		if (t == null)
		{
			return CommandResult.Fail(error);
		}
		if (!t.Manual)
		{
			return CommandResult.Fail("Сначала выберите «Самостоятельное управление командой» в офисе клуба, чтобы отправлять игроков в фарм-клуб.");
		}
		Team farm = AffiliateOf(t);
		if (farm == null)
		{
			return CommandResult.Fail("" + TeamName(t.Id) + " не имеет фарм-клуба.");
		}
		Player p = GetPlayer(playerId);
		string block = SendDownBlock(t, p);
		if (block != null)
		{
			return CommandResult.Fail(block);
		}
		DoSendDown(t, farm, p, now, announce: true);
		Revision++;
		return CommandResult.Success(p.Name + " отправлен в " + TeamName(farm.Id) + ". Вы можете вернуть его в основную команду в любое время.");
	}

	private Player FarmCandidate(Team pro, Position pos, int marketOverall, Player exclude)
	{
		Team farm = AffiliateOf(pro);
		if (farm == null)
		{
			return null;
		}
		Player best = null;
		foreach (Player p in RosterOf(farm.Id))
		{
			if (p.Position == pos && p != exclude && (best == null || p.Overall > best.Overall))
			{
				best = p;
			}
		}
		if (best == null || best.Overall < marketOverall)
		{
			return null;
		}
		return best;
	}

	private void FarmMoves(Team pro, int now)
	{
		if (pro.Manual || pro.Level != Level.Pro)
		{
			return;
		}
		Team farm = AffiliateOf(pro);
		if (farm == null)
		{
			return;
		}
		int calls = 0;
		List<Player> list = new List<Player>(RosterOf(farm.Id));
		list.Sort((Player a, Player b) => (b.Overall == a.Overall) ? a.Id.CompareTo(b.Id) : b.Overall.CompareTo(a.Overall));
		foreach (Player p in list)
		{
			if (calls >= 2)
			{
				break;
			}
			List<Player> atPos = RosterOf(pro.Id).FindAll((Player x) => x.Position == p.Position && x.Status == PlayerStatus.Active);
			if (atPos.Count != 0)
			{
				atPos.Sort((Player a, Player b) => a.Overall.CompareTo(b.Overall));
				if (p.Overall >= atPos[0].Overall + 3 && CallUpBlock(pro, p) == null)
				{
					DoCallUp(pro, farm, p, now, announce: true);
					calls++;
				}
			}
		}
		int downs = 0;
		foreach (Player p2 in new List<Player>(ProspectsOf(pro.Id)))
		{
			if (downs >= 3)
			{
				break;
			}
			if (SendDownBlock(pro, p2) == null)
			{
				DoSendDown(pro, farm, p2, now, announce: false);
				downs++;
			}
		}
	}

	private void AdvanceMinorRosters(League league, int now)
	{
		int nextYear = CurrentYear() + 1;
		int month = GameClock.MonthIndex(now);
		foreach (int id in league.TeamIds)
		{
			Team team = GetTeam(id);
			if (team == null || team.Retired)
			{
				continue;
			}
			List<KeyValuePair<Player, Rosters.Change>> changes = new List<KeyValuePair<Player, Rosters.Change>>();
			double environment = Rosters.Normal(_random, 0.0, Rosters.TeamDevelopmentSpread(team.Sport)) + DevelopmentShift(team);
			foreach (Player p in new List<Player>(RosterOf(team.Id)))
			{
				int age = nextYear - p.BirthYear;
				p.PrevOverall = p.Overall;
				Rosters.Change c = Rosters.DevelopPro(p, age, _random);
				Rosters.ApplyEnvironment(p, age, environment);
				if (c != Rosters.Change.None)
				{
					changes.Add(new KeyValuePair<Player, Rosters.Change>(p, c));
				}
				double chance;
				if (age >= 36)
				{
					chance = 0.9;
				}
				else if (age >= 34)
				{
					chance = 0.5;
				}
				else if (age >= 32)
				{
					chance = 0.25;
				}
				else
				{
					chance = ((age >= 30 && p.Overall < Rosters.Base(Level.Minor) - 4) ? 0.3 : 0.0);
				}
				if (age >= 26 && p.Overall >= Rosters.Base(Level.Minor) + 4)
				{
					chance = Math.Max(chance, 0.3);
				}
				if (_random.NextDouble() < chance)
				{
					RemovePlayer(p);
				}
			}
			if (team.IsUser)
			{
				PostDevelopment(league, team, changes, now);
			}
			RefillFarm(team, now, month);
		}
		List<List<Player>> rosters = new List<List<Player>>();
		foreach (int id2 in league.TeamIds)
		{
			rosters.Add(RosterOf(id2));
		}
		Rosters.Normalize(league.Sport, Level.Minor, rosters, null);
		foreach (int id3 in league.TeamIds)
		{
			Team team2 = GetTeam(id3);
			if (team2 != null)
			{
				UpdateRating(team2);
			}
		}
	}

	private void RefillFarm(Team farm, int now)
	{
		RefillFarm(farm, now, GameClock.MonthIndex(now));
	}

	private void RefillFarm(Team farm, int now, int month)
	{
		if (farm == null || farm.Level != Level.Minor)
		{
			return;
		}
		int year = CurrentYear();
		int baseOvr = Rosters.Base(Level.Minor);
		RosterSlot[] array = Rosters.Template(farm.Sport, Level.Minor);
		foreach (RosterSlot slot in array)
		{
			for (int k = RosterOf(farm.Id).FindAll((Player player) => player.Position == slot.Pos).Count; k < slot.Count; k++)
			{
				Player fa = null;
				foreach (Player p in FreeAgents(farm.Sport))
				{
					if (p.Position == slot.Pos && year - p.BirthYear <= 32 && p.Overall <= Rosters.Base(Level.Pro) - 2 && (p.FreeAgentMonth < 0 || month - p.FreeAgentMonth >= 2) && (fa == null || p.Overall > fa.Overall))
					{
						fa = p;
					}
				}
				if (fa != null && _random.NextDouble() < 0.6)
				{
					ChangeLevel(fa, Level.Minor);
					fa.TeamId = farm.Id;
					fa.Status = PlayerStatus.Active;
					fa.Salary = 0;
					fa.ContractYears = 0;
					fa.FreeAgentMonth = -1;
					fa.DecideMonth = -1;
					fa.Number = NumberFor(farm, slot.Pos);
					RostersChanged();
					continue;
				}
				bool young = _random.NextDouble() < 0.65;
				int ovr = (int)Math.Round((double)(baseOvr - ((!young) ? 1 : 3)) + Rosters.Normal(_random, 0.0, 3.5));
				Player n = Rosters.CreatePlayer(farm.Sport, Level.Minor, slot.Pos, ovr, farm.Id, _random, () => NextId++, null, NameUse());
				int age = (young ? (20 + _random.Next(0, 4)) : (26 + _random.Next(0, 6)));
				n.BirthYear = year - age;
				Rosters.PeakAges(n.Sport, n.Position, out var peakStart, out var _);
				n.Potential = ((age < peakStart) ? (n.Overall + _random.Next(2, 4 * (peakStart - age) + 6)) : (n.Overall + _random.Next(0, 3)));
				n.Number = NumberFor(farm, slot.Pos);
				ref string reference = ref n.From;
				string text;
				if (young)
				{
					text = ((_random.NextDouble() < 0.5) ? "Не выбран на драфте" : "Студенческая лига");
				}
				else
				{
					text = "Свободный агент";
				}
				reference = text;
				AddPlayer(n);
			}
		}
		RostersChanged();
		UpdateRating(farm);
	}

	private bool IsUserTeam(int teamId)
	{
		return GetTeam(teamId)?.IsUser ?? false;
	}

	private void Post(League league, NewsKind kind, Account account, string text, bool isUser, int now, int buzz)
	{
		if (league != null && account == Wire)
		{
			if (league.Level == Level.HighSchool)
			{
				account = Prep;
			}
			else if (league.Level == Level.College)
			{
				account = Campus;
			}
		}
		AddPost(league, kind, account.Name, account.Handle, text, isUser, now, buzz);
	}

	private void PostAsTeam(League league, Team team, NewsKind kind, string text, int now)
	{
		Organization o = ((team == null) ? null : GetOrg(team.OrgId));
		if (o != null)
		{
			AddPost(league, kind, o.FullName, Handle(o.FullName), text, isUser: true, now, 1);
		}
	}

	private void AddPost(League league, NewsKind kind, string name, string handle, string text, bool isUser, int now, int buzz)
	{
		int id = ((Feed.Count <= 0) ? 1 : (Feed[0].Id + 1));
		double scale = (double)(buzz * buzz * 90) * (0.5 + _random.NextDouble()) + (double)(isUser ? 60 : 0);
		NewsPost p = new NewsPost
		{
			Id = id,
			Time = ((now != int.MinValue) ? now : ((LastTime != int.MinValue) ? LastTime : 0)),
			LeagueKey = ((league == null) ? "" : league.Key),
			Kind = kind,
			Account = name,
			Handle = handle,
			Text = text,
			IsUser = isUser,
			Likes = (int)Math.Round(scale),
			Reposts = (int)Math.Round(scale * (0.08 + 0.12 * _random.NextDouble()))
		};
		Feed.Insert(0, p);
		if (Feed.Count > 400)
		{
			Feed.RemoveRange(400, Feed.Count - 400);
		}
		Revision++;
	}

	public static string Handle(string name)
	{
		StringBuilder sb = new StringBuilder("@");
		string text = name ?? "";
		foreach (char ch in text)
		{
			if (char.IsLetterOrDigit(ch) && ch < '\u0080')
			{
				sb.Append(ch);
			}
			if (sb.Length >= 21)
			{
				break;
			}
		}
		if (sb.Length <= 1)
		{
			return "@SkylineTeam";
		}
		return sb.ToString();
	}

	public bool IsCityPlayer(Player p)
	{
		if (p != null)
		{
			if (!Bios.IsLocal(p))
			{
				return IsCityOrgName(p.From);
			}
			return true;
		}
		return false;
	}

	private bool IsCityOrgName(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return false;
		}
		foreach (Organization o in Orgs.Values)
		{
			if (o.IsUser && o.Name == name)
			{
				return true;
			}
		}
		return false;
	}

	private void PostChampion(League league, Season s, PlayoffSeries final, int champion, int runnerUp, int now)
	{
		SportProfile profile = Profile(league);
		string label = SeasonLabel(league, s);
		int seed = final.SeedOf(champion);
		string text = "ЧЕМПИОНЫ! " + TeamName(champion) + " стали " + label + " " + profile.ChampionLabel.ToLowerInvariant() + ", обыграв " + TeamName(runnerUp) + " со счётом " + FinalScoreText(league, s, final) + " и завоевав " + AwardBook.ChampionTrophy(league.Sport, league.Level) + ".";
		int teams = 0;
		foreach (PlayoffSeries ps in s.Bracket)
		{
			if (ps.Round == 0)
			{
				teams += (ps.IsBye ? 1 : 2);
			}
		}
		if (seed > 1 && seed * 2 > teams)
		{
			text = text + " Посев № " + seed + " прошёл весь путь до титула — невероятное выступление.";
		}
		Post(league, NewsKind.Champion, Wire, text, IsUserTeam(champion), now, 4);
		Team t = GetTeam(champion);
		if (t != null && t.IsUser)
		{
			PostAsTeam(league, t, NewsKind.Champion, "ЧЕМПИОНЫ! Спасибо лучшим болельщикам в мире. Скоро сообщим подробности чемпионского парада.", now);
		}
	}

	private void PostUpset(League league, Season s, PlayoffSeries ps, int now)
	{
		int gap = ps.LowSeed - ps.HighSeed;
		if (gap < 2 && ps.HighSeed != 1)
		{
			return;
		}
		string how;
		if (ps.BestOf > 1)
		{
			SeriesWins(s, ps, out var high, out var low);
			how = "выиграв серию " + Math.Max(high, low) + "-" + Math.Min(high, low);
		}
		else
		{
			Match game = null;
			foreach (Match g in s.Games)
			{
				if (g.SeriesId == ps.Id && g.Played)
				{
					game = g;
				}
			}
			how = ((game == null) ? "" : ("победив " + Math.Max(game.HomeScore, game.AwayScore) + "-" + Math.Min(game.HomeScore, game.AwayScore) + ((game.Result == ResultType.Regulation) ? "" : (" " + SportRules.ResultSuffix(league.Sport, game.Result)))));
		}
		string round = FormatFor(Profile(league), s, ps.Round).Name;
		string text = "СЕНСАЦИЯ! Команда с посевом № " + ps.LowSeed + " " + TeamName(ps.LowTeamId) + " выбила команду с посевом № " + ps.HighSeed + " " + TeamName(ps.HighTeamId) + " на стадии " + round + ((how.Length > 0) ? (", " + how) : "") + ".";
		Post(league, NewsKind.Upset, Wire, text, IsUserTeam(ps.LowTeamId) || IsUserTeam(ps.HighTeamId), now, (gap >= 4) ? 4 : 3);
	}

	private void PostDevelopment(League league, Team team, List<KeyValuePair<Player, Rosters.Change>> changes, int now)
	{
		int posted = 0;
		int year = CurrentYear();
		foreach (KeyValuePair<Player, Rosters.Change> kv in changes)
		{
			Player p = kv.Key;
			if (p.TeamId != team.Id || posted >= 2 || !Players.ContainsKey(p.Id))
			{
				continue;
			}
			int age = year - p.BirthYear;
			string who = Rosters.NewsPositionLabel(p.Position, p.Sport) + " " + p.Name + ((team.Level == Level.Pro || team.Level == Level.Junior || team.Level == Level.Minor) ? (" (" + age + ")") : (" (" + Rosters.ClassLabel(p.ClassYear) + ")"));
			string text;
			if (kv.Value == Rosters.Change.Breakout)
			{
				text = "ПРОРЫВ: " + who + " из клуба " + TeamName(team.Id) + " сделал большой шаг вперёд в межсезонье, " + Rosters.Display(p.Sport, p.Level, p.PrevOverall) + " → " + Rosters.Ovr(p) + " ОБЩ. Теперь скауты оценивают его как " + Rosters.Grade(p, age) + ".";
			}
			else
			{
				if (p.Potential >= Rosters.Base(team.Level) + 4)
				{
					continue;
				}
				text = "Скауты снизили оценку игрока " + who + " из клуба " + TeamName(team.Id) + ": потенциал игрока оценивается ниже (оценка " + Rosters.Grade(p, age) + ").";
			}
			Post(league, NewsKind.Development, Wire, text, isUser: true, now, 2);
			posted++;
		}
	}

	private void PostCommit(Player p, Team college, int now)
	{
		bool cityKid = IsCityOrgName(p.From);
		if (college.IsUser || cityKid)
		{
			League league = LeagueOf(college);
			Organization o = GetOrg(college.OrgId);
			string text = "СОГЛАСИЕ: " + Rosters.NewsPositionLabel(p.Position, p.Sport) + " " + p.Name + ((p.From.Length > 0) ? (" из " + p.From) : "") + " согласился перейти в " + ((o == null) ? TeamName(college.Id) : o.Name) + " (" + SportRules.SportLabel(college.Sport) + "). Оценка скаутов: " + Rosters.Grade(p, 0) + ".";
			AddPost(league, NewsKind.Recruiting, Campus.Name, Campus.Handle, text, isUser: true, now, 2);
		}
	}

	private void PostDeclares(Player p, Team college, bool early, int now)
	{
		League pro = GetLeague(p.Sport, Level.Pro);
		if (pro != null)
		{
			int age = CurrentYear() - p.BirthYear;
			string text = Rosters.NewsPositionLabel(p.Position, p.Sport) + " " + p.Name + " из " + p.From + (early ? " досрочно покидает учебное заведение и выходит на " : " выходит на ") + " профессиональный " + SportRules.SportLabel(p.Sport).ToLowerInvariant() + " драфт. Оценка скаутов " + Rosters.Grade(p, age) + ".";
			AddPost(pro, NewsKind.Draft, DraftRoom.Name, DraftRoom.Handle, text, isUser: true, now, 2);
		}
	}

	private static int MonthStartOf(DateTime date)
	{
		return GameClock.DayNumber(new DateTime(date.Year, date.Month, 1));
	}

	private static bool Reached(DateTime date, int now)
	{
		return GameClock.MonthStartDay(now) >= MonthStartOf(date);
	}

	private static List<Season> SeasonsOldestFirst(League league)
	{
		List<Season> list = new List<Season>();
		if (league.Previous != null)
		{
			list.Add(league.Previous);
		}
		if (league.Current != null && league.Current != league.Previous)
		{
			list.Add(league.Current);
		}
		list.Sort((Season a, Season b) => a.Year.CompareTo(b.Year));
		return list;
	}

	private List<Team> LeagueTeams(League league)
	{
		List<Team> list = new List<Team>();
		foreach (int id in league.TeamIds)
		{
			Team t = GetTeam(id);
			if (t != null && !t.Retired)
			{
				list.Add(t);
			}
		}
		return list;
	}

	public static int DraftYearLabel(League league, int seasonYear)
	{
		return Economy.Get(league.Sport).DraftDate(seasonYear).Year;
	}

	private void StepOffseason(League league, int now)
	{
		if (!SportRules.ProLike(league.Sport, league.Level))
		{
			return;
		}
		EconomyProfile e = Economy.Get(league.Sport);
		bool second = league.Level != Level.Pro;
		League top = (second ? GetLeague(league.Sport, Level.Pro) : null);
		foreach (Season s in SeasonsOldestFirst(league))
		{
			int y = s.Year;
			if (!s.PlayoffsCreated)
			{
				continue;
			}
			if (!second && e.LotteryOdds != null && league.LotteryDoneFor < y && Reached(e.LotteryDate(y), now))
			{
				RunLottery(league, s, now);
			}
			if (!s.Complete)
			{
				continue;
			}
			if (!second && league.DraftDoneFor < y && Reached(e.DraftDate(y), now) && !PromotionPending(league, y) && !HasDraft(league.Sport))
			{
				OpenYouthMarket(league, s, now);
			}
			if (!second && league.DraftDoneFor < y && Reached(e.DraftDate(y), now) && !PromotionPending(league, y))
			{
				if (e.LotteryOdds != null && league.LotteryDoneFor < y)
				{
					RunLottery(league, s, now);
				}
				if (league.Draft == null || league.Draft.Year != y || !league.Draft.Started)
				{
					OpenDraft(league, s, now);
				}
				RunDraft(league, now, GameClock.MonthIndex(now) > league.Draft.Month);
			}
			bool draftFirst = e.DraftDate(y) <= e.LeagueYearDate(y);
			bool ready = ((!second) ? (!draftFirst || league.DraftDoneFor >= y) : (top == null || top.LeagueYearDoneFor >= y));
			if (((league.LeagueYearDoneFor < y && Reached(e.LeagueYearDate(y), now)) & ready) && !PromotionPending(league, y))
			{
				NewLeagueYear(league, y, now);
			}
		}
		MonthlyMoves(league, now);
	}

	private void ForceOffseason(League league, int y, int now)
	{
		Season s = ((league.Previous != null && league.Previous.Year == y) ? league.Previous : null);
		if (s == null)
		{
			return;
		}
		EconomyProfile e = Economy.Get(league.Sport);
		if (league.Level == Level.Pro && e.LotteryOdds != null && league.LotteryDoneFor < y && s.PlayoffsCreated)
		{
			RunLottery(league, s, now);
		}
		if (league.Level == Level.Pro && league.DraftDoneFor < y && !HasDraft(league.Sport))
		{
			OpenYouthMarket(league, s, now);
		}
		if (league.Level == Level.Pro && league.DraftDoneFor < y)
		{
			if (league.Draft == null || league.Draft.Year != y || !league.Draft.Started)
			{
				OpenDraft(league, s, now);
			}
			RunDraft(league, now, auto: true);
		}
		if (league.LeagueYearDoneFor < y)
		{
			NewLeagueYear(league, y, now);
		}
	}

	private List<int> NonPlayoffTeams(League league, Season s)
	{
		List<StandingRow> table = Standings.Compute(league, s);
		int q = PlayoffTeamCount(Profile(league), table.Count);
		List<int> list = new List<int>();
		for (int i = table.Count - 1; i >= q; i--)
		{
			if (league.TeamIds.Contains(table[i].TeamId))
			{
				list.Add(table[i].TeamId);
			}
		}
		return list;
	}

	private void RunLottery(League league, Season s, int now)
	{
		league.LotteryDoneFor = s.Year;
		EconomyProfile e = Economy.Get(league.Sport);
		DraftState d = (league.Draft = new DraftState
		{
			Year = s.Year,
			LotteryDone = true
		});
		List<int> non = NonPlayoffTeams(league, s);
		int n = non.Count;
		int draws = Math.Min(e.LotteryPicks, n - 1);
		string draft = DraftYearLabel(league, s.Year) + " " + e.DraftName;
		if (draws <= 0 || e.LotteryOdds == null)
		{
			d.Order.AddRange(non);
			d.Lottery.Add("Только " + n + ((n == 1) ? " команда не попала" : " команд не попали") + " в плей-офф, поэтому лотерея не проводилась.");
			Revision++;
			return;
		}
		double[] weights = new double[n];
		double total = 0.0;
		for (int i = 0; i < n; i++)
		{
			weights[i] = e.LotteryOdds[Math.Min(i, e.LotteryOdds.Length - 1)];
			total += weights[i];
		}
		List<int> remaining = new List<int>();
		for (int j = 0; j < n; j++)
		{
			remaining.Add(j);
		}
		List<int> winners = new List<int>();
		for (int k = 0; k < draws; k++)
		{
			double sum = 0.0;
			foreach (int i2 in remaining)
			{
				sum += weights[i2];
			}
			double r = _random.NextDouble() * sum;
			int chosen = remaining[remaining.Count - 1];
			foreach (int i3 in remaining)
			{
				r -= weights[i3];
				if (r < 0.0)
				{
					chosen = i3;
					break;
				}
			}
			winners.Add(chosen);
			remaining.Remove(chosen);
		}
		List<int> order = new List<int>(winners);
		order.AddRange(remaining);
		for (int slot = 0; slot < order.Count; slot++)
		{
			int i4 = order[slot];
			d.Order.Add(non[i4]);
			if (slot < draws || slot < 3)
			{
				string move;
				if (i4 == slot)
				{
					move = "сохранила свою позицию";
				}
				else
				{
					move = ((i4 > slot) ? ("поднялась на " + (i4 - slot) + ((i4 - slot == 1) ? " позицию" : " позиций")) : ("опустилась на " + (slot - i4) + ((slot - i4 == 1) ? " позицию" : " позиций")));
				}
				d.Lottery.Add("№ " + (slot + 1) + ": " + TeamName(non[i4]) + " (" + Views.Ordinal(i4 + 1) + " худший результат, " + (weights[i4] * 100.0 / total).ToString("0.0", CultureInfo.InvariantCulture) + "% вероятности получить первый выбор), " + move + ".");
			}
		}
		int winner = non[order[0]];
		string text = ((order[0] == 0) ? ("ЛОТЕРЕЯ: " + TeamName(winner) + " сохраняют первый выбор на " + draft + ".") : ("ЛОТЕРЕЯ: " + TeamName(winner) + " выигрывают " + draft + " лотерею и поднимаются с позиции № " + (order[0] + 1) + " на первое место!"));
		Post(league, NewsKind.Lottery, Wire, text, IsUserTeam(winner), now, 3);
		foreach (int id in non)
		{
			Team t = GetTeam(id);
			if (t != null && t.IsUser)
			{
				int slot2 = d.Order.IndexOf(id);
				PostAsTeam(league, t, NewsKind.Lottery, "Наш выбор под номером " + (slot2 + 1) + " на стадии " + draft + ".", now);
			}
		}
		Revision++;
	}

	private List<int> FirstRoundOrder(League league, Season s, DraftState d)
	{
		List<int> order = new List<int>();
		List<int> non = ((d.LotteryDone && d.Order.Count > 0) ? new List<int>(d.Order) : NonPlayoffTeams(league, s));
		foreach (int id in non)
		{
			if (league.TeamIds.Contains(id) && !order.Contains(id))
			{
				order.Add(id);
			}
		}
		List<StandingRow> list = Standings.Compute(league, s);
		Dictionary<int, int> rank = new Dictionary<int, int>();
		foreach (StandingRow r in list)
		{
			rank[r.TeamId] = r.Rank;
		}
		List<int> playoff = new List<int>();
		Dictionary<int, int> reached = new Dictionary<int, int>();
		foreach (PlayoffSeries ps in s.Bracket)
		{
			int[] array = new int[2] { ps.HighTeamId, ps.LowTeamId };
			foreach (int id2 in array)
			{
				if (id2 >= 0)
				{
					int value = ps.Round * 2 + ((ps.WinnerId == id2) ? 1 : 0);
					if (!reached.TryGetValue(id2, out var round) || value > round)
					{
						reached[id2] = value;
					}
				}
			}
		}
		foreach (KeyValuePair<int, int> kv in reached)
		{
			if (league.TeamIds.Contains(kv.Key) && !order.Contains(kv.Key))
			{
				playoff.Add(kv.Key);
			}
		}
		playoff.Sort((int a, int b) =>
		{
			int num = reached[a].CompareTo(reached[b]);
			if (num != 0)
			{
				return num;
			}
			rank.TryGetValue(a, out var value2);
			rank.TryGetValue(b, out var value3);
			return value3.CompareTo(value2);
		});
		if (s.ChampionTeamId >= 0 && playoff.Remove(s.ChampionTeamId))
		{
			playoff.Add(s.ChampionTeamId);
		}
		order.AddRange(playoff);
		foreach (int id3 in league.TeamIds)
		{
			Team t = GetTeam(id3);
			if (t != null && !t.Retired && !order.Contains(id3))
			{
				order.Insert(Math.Min(non.Count, order.Count), id3);
			}
		}
		return order;
	}

	private void OpenDraft(League league, Season s, int now)
	{
		EconomyProfile e = Economy.Get(league.Sport);
		DraftState d = (league.Draft = ((league.Draft != null && league.Draft.Year == s.Year) ? league.Draft : new DraftState
		{
			Year = s.Year
		}));
		d.Started = true;
		d.Rounds = e.DraftRounds;
		d.Month = GameClock.MonthIndex(now);
		d.Order = FirstRoundOrder(league, s, d);
		d.Picks.Clear();
		int n = d.Order.Count;
		EnsurePicks(league);
		for (int r = 1; r <= d.Rounds; r++)
		{
			for (int i = 0; i < n; i++)
			{
				int original = d.Order[i];
				int owner = original;
				foreach (PickAsset a in league.Picks)
				{
					if (a.Year == s.Year && a.Round == r && a.OriginalTeamId == original && league.TeamIds.Contains(a.OwnerTeamId))
					{
						owner = a.OwnerTeamId;
					}
				}
				d.Picks.Add(new DraftPick
				{
					Number = (r - 1) * n + i + 1,
					Round = r,
					TeamId = owner,
					OriginalTeamId = original
				});
			}
		}
		int target = Math.Max(12, (int)Math.Round((double)(n * d.Rounds) * 1.5));
		int have = DraftPool(league.Sport).Count;
		double outsideShare = ((league.Sport == Sport.Football || league.Sport == Sport.Basketball) ? 1.2 : 0.6);
		target = Math.Max(target, have + (int)Math.Round((double)(n * d.Rounds) * outsideShare));
		List<Position> positions = new List<Position>();
		RosterSlot[] array = Rosters.Template(league.Sport, Level.Pro);
		foreach (RosterSlot slot in array)
		{
			for (int k = 0; k < slot.Count; k++)
			{
				positions.Add(slot.Pos);
			}
		}
		int year = CurrentYear();
		for (int l = have; l < target; l++)
		{
			Position pos = positions[_random.Next(positions.Count)];
			Player prospect = Rosters.GenerateProspect(league.Sport, pos, year, _random, () => NextId++, NameUse());
			GiveProspectBackground(prospect);
			AddPlayer(prospect);
		}
		RostersChanged();
		string draft = DraftYearLabel(league, s.Year) + " " + e.DraftName;
		if (n > 0)
		{
			Post(league, NewsKind.Draft, DraftRoom, "" + draft + " открыт! " + TeamName(d.Order[0]) + " делают первый выбор. " + d.Rounds + " раундов, " + d.Picks.Count + " выбора.", IsUserTeam(d.Order[0]), now, 2);
		}
		Revision++;
	}

	private void RunDraft(League league, int now, bool auto)
	{
		DraftState d = league.Draft;
		if (d == null || d.Complete || !d.Started)
		{
			return;
		}
		DraftPick pick;
		while ((pick = d.NextPick) != null)
		{
			Team team = GetTeam(pick.TeamId);
			if (team == null || team.Retired)
			{
				pick.PlayerId = 0;
				pick.PlayerName = "Выбор аннулирован";
				continue;
			}
			if (team.Manual && !auto)
			{
				break;
			}
			Player p = BestAvailable(league, team);
			if (p == null)
			{
				pick.PlayerId = 0;
				pick.PlayerName = "Игроков больше нет";
			}
			else
			{
				MakePick(league, d, pick, team, p, now);
			}
		}
		if (d.NextPick == null)
		{
			FinishDraft(league, d, now);
		}
		Revision++;
	}

	private Player BestAvailable(League league, Team team)
	{
		List<Player> signed = SignedPlayers(team.Id);
		Player best = null;
		double bestScore = double.MinValue;
		foreach (Player p in DraftPool(league.Sport))
		{
			double score = 0.65 * (double)p.Potential + 0.35 * (double)p.Overall + Rosters.Normal(_random, 0.0, 1.5);
			int count = 0;
			int want = 0;
			RosterSlot[] array = Rosters.Template(league.Sport, Level.Pro);
			foreach (RosterSlot slot in array)
			{
				if (slot.Pos == p.Position)
				{
					want = slot.Count;
				}
			}
			foreach (Player item in signed)
			{
				if (item.Position == p.Position)
				{
					count++;
				}
			}
			if (count < want)
			{
				score += 2.0;
			}
			if (score > bestScore)
			{
				bestScore = score;
				best = p;
			}
		}
		return best;
	}

	private void MakePick(League league, DraftState d, DraftPick pick, Team team, Player p, int now)
	{
		int age = CurrentYear() - p.BirthYear;
		p.Number = NumberFor(team, p.Position);
		p.Status = PlayerStatus.Prospect;
		p.TeamId = team.Id;
		p.Level = (SportRules.ProLike(team.Sport, team.Level) ? team.Level : Level.Pro);
		p.DraftYear = d.Year;
		p.DraftRound = pick.Round;
		p.DraftPick = pick.Number;
		p.DraftTeamId = team.Id;
		Economy.RookieContract(p, pick.Number, d.Order.Count);
		pick.PlayerId = p.Id;
		pick.PlayerName = p.Name;
		pick.Position = Rosters.NewsPositionLabel(p.Position, p.Sport);
		pick.Overall = p.Overall;
		pick.Grade = Rosters.Grade(p, age);
		pick.From = ((string.IsNullOrEmpty(p.From) || p.From == "Prospect") ? "" : p.From);
		RostersChanged();
		string draft = DraftYearLabel(league, d.Year) + " " + Economy.Get(league.Sport).DraftName;
		string who = pick.Position + " " + p.Name + ((pick.From.Length > 0) ? (" (" + pick.From + ")") : "") + "; возраст " + age + "; оценка скаутов " + pick.Grade;
		if (pick.Number == 1)
		{
			Post(league, NewsKind.Draft, DraftRoom, "Под первым номером на драфте " + draft + " команда " + TeamName(team.Id) + " выбирает " + who + ".", team.IsUser, now, 3);
		}
		if (team.IsUser)
		{
			PostAsTeam(league, team, NewsKind.Draft, "Под номером " + pick.Number + " (раунд " + pick.Round + ") мы выбираем " + who + ". Добро пожаловать в город!", now);
		}
	}

	private void FinishDraft(League league, DraftState d, int now)
	{
		if (d.Complete)
		{
			return;
		}
		d.Complete = true;
		league.DraftDoneFor = Math.Max(league.DraftDoneFor, d.Year);
		EnsurePicks(league);
		int month = GameClock.MonthIndex(now);
		int floor = Rosters.Base(Level.Pro) - 17;
		int signed = 0;
		foreach (Player p in new List<Player>(DraftPool(league.Sport)))
		{
			if (p.Overall >= floor || p.Potential >= Rosters.Base(Level.Pro))
			{
				p.Status = PlayerStatus.FreeAgent;
				p.FreeAgentMonth = month;
				p.Salary = 0;
				p.ContractYears = 0;
				signed++;
			}
			else
			{
				RemovePlayer(p);
			}
		}
		RostersChanged();
		foreach (Team t in LeagueTeams(league))
		{
			ArrangeRoster(t, now);
			UpdateRating(t);
		}
		if (Probe != null)
		{
			Probe("draft", this, league);
		}
		Post(league, NewsKind.Draft, DraftRoom, "" + DraftYearLabel(league, d.Year) + " " + Economy.Get(league.Sport).DraftName + " завершён: " + d.Picks.Count + " выборов сделано. " + signed + " не выбранных на драфте игроков теперь стали свободными агентами.", isUser: false, now, 1);
	}

	private void NewLeagueYear(League league, int y, int now)
	{
		league.LeagueYearDoneFor = Math.Max(league.LeagueYearDoneFor, y);
		Sport sport = league.Sport;
		TransferBudgetsNewYear(league);
		EconomyProfile e = Economy.Get(sport);
		int nextYear = y + 1;
		int month = GameClock.MonthIndex(now);
		int baseOvr = Rosters.Base(Level.Pro);
		Level level = league.Level;
		bool second = level != Level.Pro;
		List<Team> teams = LeagueTeams(league);
		if (Probe != null)
		{
			Probe("start", this, league);
		}
		foreach (Team t in teams)
		{
			EnsureStaff(t, now);
		}
		List<Player> everyone = new List<Player>();
		foreach (Player p in Players.Values)
		{
			if (p.Sport == sport && p.Level == level && (p.Status == PlayerStatus.Active || p.Status == PlayerStatus.Prospect || p.Status == PlayerStatus.FreeAgent))
			{
				everyone.Add(p);
			}
		}
		List<KeyValuePair<Player, Rosters.Change>> changes = new List<KeyValuePair<Player, Rosters.Change>>();
		Dictionary<int, double> environment = new Dictionary<int, double>();
		foreach (Team t2 in teams)
		{
			environment[t2.Id] = Rosters.Normal(_random, 0.0, Rosters.TeamDevelopmentSpread(sport)) + DevelopmentShift(t2);
		}
		foreach (Player p2 in everyone)
		{
			int age = nextYear - p2.BirthYear;
			p2.PrevOverall = p2.Overall;
			Rosters.Change c = Rosters.DevelopPro(p2, age, _random);
			if (environment.TryGetValue(p2.TeamId, out var env))
			{
				Rosters.ApplyEnvironment(p2, age, env);
			}
			if (c != Rosters.Change.None)
			{
				changes.Add(new KeyValuePair<Player, Rosters.Change>(p2, c));
			}
		}
		int retiredPosts = 0;
		foreach (Player p3 in everyone)
		{
			int age2 = nextYear - p3.BirthYear;
			if (Retires(p3, age2, month))
			{
				Team t3 = GetTeam(p3.TeamId);
				if ((p3.Overall >= baseOvr + 4 || p3.Seasons >= 12 || (t3 != null && t3.IsUser && p3.Status == PlayerStatus.Active)) && retiredPosts < 6)
				{
					retiredPosts++;
					Post(league, NewsKind.Retirement, Wire, Rosters.NewsPositionLabel(p3.Position, p3.Sport) + " " + p3.Name + " завершает карьеру в возрасте " + age2 + " после " + p3.Seasons + ((p3.Seasons == 1) ? " сезона" : " сезонов") + ((t3 != null) ? (", последний из которых он провёл в клубе " + TeamName(t3.Id)) : "") + ".", t3?.IsUser ?? false, now, 2);
				}
				CareerOver(p3, level, nextYear - 1, now);
				KeepForCoaching(p3, age2, nextYear - 1);
				RemovePlayer(p3);
			}
		}
		foreach (Team t4 in teams)
		{
			if (t4.IsUser)
			{
				PostDevelopment(league, t4, changes, now);
			}
		}
		if (Probe != null)
		{
			Probe("develop+retire", this, league);
		}
		int resignPosts = 0;
		foreach (Team t5 in teams)
		{
			List<Player> signed = SignedPlayers(t5.Id);
			foreach (Player p4 in signed)
			{
				if (p4.DraftYear != y || p4.DraftTeamId != t5.Id || p4.ContractYears < e.RookieYears)
				{
					p4.ContractYears--;
				}
			}
			NewDeadYear(t5);
			List<Player> list = signed.FindAll((Player player) => player.ContractYears <= 0);
			list.Sort((Player a, Player b) => b.Overall.CompareTo(a.Overall));
			List<Player> keeping = signed.FindAll((Player player) => player.ContractYears > 0);
			foreach (Player p5 in list)
			{
				int age3 = nextYear - p5.BirthYear;
				bool stays = false;
				bool wanted = !t5.Manual && WantsToKeep(t5, p5, age3, signed);
				if (wanted)
				{
					double leave = ((p5.Overall >= baseOvr + 4) ? 0.15 : 0.05) * LeaveMultiplier(p5, t5);
					double discount;
					if (p5.Priority == PlayerPriority.Loyalty)
					{
						discount = 0.75 + 0.12 * _random.NextDouble();
					}
					else
					{
						discount = ((p5.Priority == PlayerPriority.Money) ? (0.95 + 0.05 * _random.NextDouble()) : (0.82 + 0.13 * _random.NextDouble()));
					}
					int salary = Economy.Discount(Economy.AskingSalary(p5, age3, -1), sport, discount);
					if (_random.NextDouble() >= leave && Economy.CanAfford(t5, keeping, salary, resign: true))
					{
						p5.Salary = salary;
						p5.ContractYears = Economy.AskingYears(sport, age3, resign: true);
						keeping.Add(p5);
						stays = true;
						if ((p5.Overall >= baseOvr + 4 || t5.IsUser) && resignPosts < 6)
						{
							resignPosts++;
							Post(league, NewsKind.Signing, Insider, "" + TeamName(t5.Id) + " продлевают контракт с " + Rosters.NewsPositionLabel(p5.Position, p5.Sport) + " " + p5.Name + " (" + Rosters.Ovr(p5) + " ОБЩ): " + Economy.ContractText(p5) + ".", t5.IsUser, now, 2);
						}
					}
				}
				if ((!stays & wanted) && p5.Overall >= baseOvr + 4 && resignPosts < 6)
				{
					resignPosts++;
					Post(league, NewsKind.Signing, Insider, "Переговоры о контракте между " + TeamName(t5.Id) + " и " + Rosters.NewsPositionLabel(p5.Position, p5.Sport) + " " + p5.Name + " сорвались. Игрок выйдет на рынок свободных агентов.", t5.IsUser, now, 2);
				}
				if (!stays)
				{
					BecomeFreeAgent(p5, month);
				}
			}
		}
		RostersChanged();
		TalksNewYear(league);
		if (Probe != null)
		{
			Probe("contracts", this, league);
		}
		foreach (Team t6 in teams)
		{
			if (e.Type == CapType.Budget)
			{
				if (t6.Budget <= 0)
				{
					t6.Budget = Economy.NewBudget(sport, _random);
				}
				else
				{
					t6.Budget = (int)Math.Round((double)t6.Budget * (0.95 + 0.1 * _random.NextDouble()));
				}
			}
		}
		List<Player> market = new List<Player>(FreeAgents(sport));
		market.Sort((Player a, Player b) => b.Overall.CompareTo(a.Overall));
		if (market.Count > 0 && !second)
		{
			List<string> top = new List<string>();
			for (int i = 0; i < market.Count && i < 3; i++)
			{
				top.Add(market[i].Name + " (" + Rosters.PositionLabel(market[i].Position) + ", " + Rosters.Ovr(market[i]) + ")");
			}
			Post(league, NewsKind.Signing, Insider, "Рынок свободных агентов открыт: " + market.Count + " игроков доступны для подписания. Лучшие варианты: " + string.Join(", ", top.ToArray()) + ".", isUser: false, now, 2);
		}
		FreeAgency(league, nextYear, now, offseason: true);
		if (Probe != null)
		{
			Probe("free agency", this, league);
		}
		foreach (Team t7 in teams)
		{
			ArrangeRoster(t7, now);
			FillHoles(t7, nextYear, now);
			FarmMoves(t7, now);
		}
		List<List<Player>> rosters = new List<List<Player>>();
		List<Player> extras = new List<Player>();
		foreach (Team t8 in teams)
		{
			rosters.Add(RosterOf(t8.Id));
			extras.AddRange(ProspectsOf(t8.Id));
		}
		if (second)
		{
			Rosters.Normalize(sport, level, rosters, extras, 59.0);
			foreach (Team t9 in teams)
			{
				UpdateRating(t9);
			}
		}
		else
		{
			extras.AddRange(FreeAgents(sport));
			extras.AddRange(DraftPool(sport));
			Rosters.Normalize(sport, Level.Pro, rosters, extras);
			foreach (Team t10 in teams)
			{
				UpdateRating(t10);
			}
			TrimFreeAgents(league);
		}
		if (Probe != null)
		{
			Probe("end", this, league);
		}
		Revision++;
	}

	private bool Retires(Player p, int age, int month)
	{
		int baseOvr = Rosters.Base(Level.Pro);
		int levelBase = (SportRules.IsSecondDivision(p.Sport, p.Level) ? SecondDivisionOverall : baseOvr);
		double a = (double)age - 3.0 * Drive(p);
		if (p.Position == Position.Kicker || p.Position == Position.Punter)
		{
			a -= 4.0;
		}
		else if (p.Position == Position.Quarterback || p.Position == Position.Goalie || p.Position == Position.Goalkeeper)
		{
			a -= 1.5;
		}
		double hazard;
		if (a < 30.0)
		{
			hazard = 0.004;
		}
		else if (a < 32.0)
		{
			hazard = 0.03;
		}
		else if (a < 34.0)
		{
			hazard = 0.08;
		}
		else if (a < 36.0)
		{
			hazard = 0.17;
		}
		else if (a < 38.0)
		{
			hazard = 0.33;
		}
		else if (a < 40.0)
		{
			hazard = 0.55;
		}
		else
		{
			hazard = ((a < 42.0) ? 0.8 : 1.0);
		}
		int q = p.Overall - levelBase;
		double form;
		if (q >= 8)
		{
			form = 0.35;
		}
		else if (q >= 4)
		{
			form = 0.55;
		}
		else if (q >= 0)
		{
			form = 0.85;
		}
		else if (q >= -4)
		{
			form = 1.25;
		}
		else
		{
			form = ((q >= -8) ? 1.7 : 2.3);
		}
		if (p.PrevOverall > 0 && p.Overall <= p.PrevOverall - 3)
		{
			form *= 1.3;
		}
		if (age >= 28)
		{
			hazard *= form;
		}
		if (age >= 31 && q >= 6)
		{
			hazard += 0.015;
		}
		double chance = Math.Min(1.0, hazard);
		if (p.Status == PlayerStatus.FreeAgent)
		{
			int monthsOut = ((p.FreeAgentMonth >= 0) ? Math.Max(0, month - p.FreeAgentMonth) : 0);
			int gap = baseOvr - p.Overall;
			double quit = 0.0;
			if (monthsOut >= 6 && gap >= 13 && p.Potential < baseOvr - 4)
			{
				quit = 0.9;
			}
			else if (monthsOut >= 9)
			{
				double perYear = 0.03 + 0.012 * (double)Math.Max(0, gap) + ((age >= 30) ? 0.08 : 0.0);
				if (p.Potential >= baseOvr)
				{
					perYear *= 0.5;
				}
				quit = Math.Min(0.95, perYear * (double)monthsOut / 12.0);
			}
			if (monthsOut >= 66)
			{
				quit = 1.0;
			}
			chance = 1.0 - (1.0 - chance) * (1.0 - quit);
		}
		if (age >= 45)
		{
			chance = 1.0;
		}
		return _random.NextDouble() < chance;
	}

	private static double Drive(Player p)
	{
		return (double)((uint)(p.Id * -1640531535) % 1001u) / 500.0 - 1.0;
	}

	private bool WantsToKeep(Team t, Player p, int age, List<Player> signed)
	{
		int baseOvr = Rosters.Base(Level.Pro);
		int better = 0;
		foreach (Player q in signed)
		{
			if (q != p && q.Position == p.Position && q.Overall > p.Overall)
			{
				better++;
			}
		}
		int count = SlotCount(t.Sport, p.Position);
		if (better < count && (age <= 33 || p.Overall >= baseOvr + 2))
		{
			return true;
		}
		if (age <= 24 && p.Potential >= baseOvr - 2)
		{
			return better < count + 2;
		}
		return false;
	}

	private static int SlotCount(Sport sport, Position pos)
	{
		RosterSlot[] array = Rosters.Template(sport, Level.Pro);
		foreach (RosterSlot slot in array)
		{
			if (slot.Pos == pos)
			{
				return slot.Count;
			}
		}
		return 0;
	}

	private static int SlotStarters(Sport sport, Position pos)
	{
		RosterSlot[] array = Rosters.Template(sport, Level.Pro);
		foreach (RosterSlot slot in array)
		{
			if (slot.Pos == pos)
			{
				return slot.Starters;
			}
		}
		return 0;
	}

	private static int SlotStarters(Sport sport, Position pos, Level level)
	{
		RosterSlot[] array = Rosters.Template(sport, level);
		foreach (RosterSlot slot in array)
		{
			if (slot.Pos == pos)
			{
				return slot.Starters;
			}
		}
		return 0;
	}

	private void BecomeFreeAgent(Player p, int month)
	{
		if (SportRules.IsSecondDivision(p.Sport, p.Level))
		{
			ChangeLevel(p, Level.Pro);
		}
		if (p.TeamId >= 0)
		{
			p.LastTeamId = p.TeamId;
		}
		p.TeamId = -1;
		p.Status = PlayerStatus.FreeAgent;
		p.FreeAgentMonth = month;
		p.DecideMonth = -1;
		p.ContractYears = 0;
		p.Salary = 0;
		RostersChanged();
	}

	private void TrimFreeAgents(League league)
	{
		List<Player> fas = new List<Player>(FreeAgents(league.Sport));
		int perTeam;
		if (league.Sport == Sport.Football)
		{
			perTeam = 14;
		}
		else if (league.Sport == Sport.Hockey || league.Sport == Sport.Baseball)
		{
			perTeam = 9;
		}
		else
		{
			perTeam = ((league.Sport == Sport.Soccer) ? 7 : 5);
		}
		int keep = Math.Max(24, LeagueTeams(league).Count * perTeam * 2);
		int floor = Rosters.Base(Level.Pro) - 17;
		int month = GameClock.MonthIndex(NowHint());
		Func<Player, double> worth = (Player p) => (double)p.Overall + ((AgeOf(p) <= 23) ? ((double)Math.Max(0, p.Potential - p.Overall) * 0.5) : 0.0);
		fas.Sort((Player a, Player b) => (worth(b) == worth(a)) ? a.Id.CompareTo(b.Id) : worth(b).CompareTo(worth(a)));
		for (int i = 0; i < fas.Count; i++)
		{
			if ((!IsYoungProspect(fas[i]) || fas[i].FreeAgentMonth < 0 || month - fas[i].FreeAgentMonth >= 12) && (i >= keep || (fas[i].Overall < floor && fas[i].Potential < Rosters.Base(Level.Pro))))
			{
				RemovePlayer(fas[i]);
			}
		}
	}

	private void ArrangeRoster(Team team, int now)
	{
		if (!SportRules.ProLike(team.Sport, team.Level))
		{
			return;
		}
		List<Player> signed = SignedPlayers(team.Id);
		if (signed.Count == 0)
		{
			return;
		}
		EconomyProfile e = Economy.Get(team.Sport);
		int year = CurrentYear();
		List<Player> reserves = new List<Player>();
		bool changed = false;
		RosterSlot[] array = Rosters.Template(team.Sport, Level.Pro);
		foreach (RosterSlot slot in array)
		{
			List<Player> atPos = signed.FindAll((Player player) => player.Position == slot.Pos);
			atPos.Sort((Player a, Player b) => (b.Overall == a.Overall) ? a.Id.CompareTo(b.Id) : b.Overall.CompareTo(a.Overall));
			for (int i2 = 0; i2 < atPos.Count; i2++)
			{
				if (i2 < slot.Count)
				{
					if (atPos[i2].Status != PlayerStatus.Active)
					{
						changed = true;
					}
					atPos[i2].Status = PlayerStatus.Active;
				}
				else
				{
					reserves.Add(atPos[i2]);
				}
			}
		}
		if (reserves.Count > e.MaxProspects)
		{
			int cap = Math.Max(1, Economy.Cap(team.Sport));
			Dictionary<int, double> keep = new Dictionary<int, double>();
			foreach (Player p in reserves)
			{
				keep[p.Id] = ProspectValue(p, year) + 150.0 * (double)QuoteRelease(p, team, now, stretch: false).Total / (double)cap;
			}
			reserves.Sort((Player a, Player b) => (keep[b.Id] == keep[a.Id]) ? a.Id.CompareTo(b.Id) : keep[b.Id].CompareTo(keep[a.Id]));
		}
		else
		{
			reserves.Sort((Player a, Player b) => ProspectValue(b, year).CompareTo(ProspectValue(a, year)));
		}
		GameClock.MonthIndex(now);
		Team farm = AffiliateOf(team);
		for (int i3 = 0; i3 < reserves.Count; i3++)
		{
			if (i3 < e.MaxProspects)
			{
				if (reserves[i3].Status != PlayerStatus.Prospect)
				{
					changed = true;
				}
				reserves[i3].Status = PlayerStatus.Prospect;
				continue;
			}
			if (farm != null && SendDownBlock(team, reserves[i3]) == null)
			{
				SendDownQuiet(team, farm, reserves[i3]);
				changed = true;
				continue;
			}
			ReleaseQuote q = QuoteRelease(reserves[i3], team, now, stretch: false);
			if (team.IsUser)
			{
				PostAsTeam(LeagueOf(team), team, NewsKind.Release, "Мы " + q.Verb + " игрока " + Rosters.PositionLabel(reserves[i3].Position) + " " + reserves[i3].Name + ". Спасибо за его вклад!" + ((q.Total > 0) ? (" " + q.Short) : ""), now);
			}
			ApplyRelease(reserves[i3], team, q, now);
			changed = true;
		}
		if (changed)
		{
			RostersChanged();
		}
	}

	private static double ProspectValue(Player p, int year)
	{
		int age = year - p.BirthYear;
		return (double)p.Overall * 0.4 + (double)p.Potential * 0.6 - (double)Math.Max(0, age - 24) * 1.5;
	}

	private void FillHoles(Team team, int year, int now, Player exclude = null)
	{
		if (!SportRules.ProLike(team.Sport, team.Level))
		{
			return;
		}
		EconomyProfile e = Economy.Get(team.Sport);
		int month = GameClock.MonthIndex(now);
		RosterSlot[] array = Rosters.Template(team.Sport, Level.Pro);
		foreach (RosterSlot slot in array)
		{
			for (int k = RosterOf(team.Id).FindAll((Player player) => player.Position == slot.Pos).Count; k < slot.Count; k++)
			{
				Player reserve = null;
				foreach (Player p in ProspectsOf(team.Id))
				{
					if (p.Position == slot.Pos && (reserve == null || p.Overall > reserve.Overall))
					{
						reserve = p;
					}
				}
				if (reserve != null)
				{
					reserve.Status = PlayerStatus.Active;
					RostersChanged();
					continue;
				}
				Player pick = null;
				List<Player> signed = SignedPlayers(team.Id);
				foreach (Player p2 in FreeAgents(team.Sport))
				{
					if (p2.Position == slot.Pos && p2 != exclude && !YouthHold(p2, month))
					{
						int salary = Economy.AskingSalary(p2, year - p2.BirthYear, month);
						if (Economy.CanAfford(team, signed, salary, resign: false) && (pick == null || p2.Overall > pick.Overall))
						{
							pick = p2;
						}
					}
				}
				Player farmed = (Economy.CanAfford(team, signed, e.MinSalary, resign: false) ? FarmCandidate(team, slot.Pos, pick?.Overall ?? int.MinValue, exclude) : null);
				if (farmed != null)
				{
					CallUpQuiet(team, AffiliateOf(team), farmed, now);
					continue;
				}
				if (pick != null)
				{
					Sign(team, pick, Economy.AskingSalary(pick, year - pick.BirthYear, month), 1, now, announce: false);
					continue;
				}
				int depthBase = ((team.Level == Level.Pro) ? Rosters.Base(Level.Pro) : SecondDivisionOverall);
				Player depth = Rosters.CreatePlayer(team.Sport, team.Level, slot.Pos, depthBase - 10 + _random.Next(-3, 3), team.Id, _random, () => NextId++, null, NameUse());
				depth.BirthYear = year - (24 + _random.Next(0, 8));
				depth.Potential = depth.Overall + _random.Next(0, 3);
				depth.Number = NumberFor(team, slot.Pos);
				depth.Salary = e.MinSalary;
				depth.ContractYears = 1;
				depth.From = "Малые лиги";
				AddPlayer(depth);
			}
		}
		UpdateRating(team);
	}

	private void Sign(Team team, Player p, int salary, int years, int now, bool announce)
	{
		if (p.Level != team.Level && SportRules.ProLike(team.Sport, team.Level))
		{
			ChangeLevel(p, team.Level);
		}
		p.TeamId = team.Id;
		p.Status = PlayerStatus.Active;
		p.Salary = salary;
		p.ContractYears = Math.Max(1, years);
		p.FreeAgentMonth = -1;
		p.DecideMonth = -1;
		p.Number = NumberFor(team, p.Position);
		RostersChanged();
		League league = LeagueOf(team);
		if (league != null)
		{
			if (team.IsUser)
			{
				PostAsTeam(league, team, NewsKind.Signing, "Мы подписали " + Rosters.NewsPositionLabel(p.Position, p.Sport) + " " + p.Name + " (" + Rosters.Ovr(p) + " ОБЩ): " + Economy.ContractText(p) + ". Добро пожаловать в город!", now);
			}
			if (announce)
			{
				Post(league, NewsKind.Signing, Insider, "ПОДПИСАНИЕ: " + Rosters.NewsPositionLabel(p.Position, p.Sport) + " " + p.Name + " (" + Rosters.Ovr(p) + " ОБЩ) договорился с " + TeamName(team.Id) + ", " + Economy.ContractText(p) + ".", team.IsUser, now, 2);
			}
		}
	}

	private static int Weakest(List<Player> signed, Position pos, int count)
	{
		if (count <= 0)
		{
			return int.MaxValue;
		}
		List<int> ovr = new List<int>();
		foreach (Player p in signed)
		{
			if (p.Position == pos)
			{
				ovr.Add(p.Overall);
			}
		}
		if (ovr.Count < count)
		{
			return 0;
		}
		ovr.Sort((int a, int b) => b.CompareTo(a));
		return ovr[count - 1];
	}

	private void FreeAgency(League league, int year, int now, bool offseason)
	{
		Sport sport = league.Sport;
		int month = GameClock.MonthIndex(now);
		int baseOvr = Rosters.Base(Level.Pro);
		List<Team> teams = LeagueTeams(league).FindAll((Team team) => !team.Manual);
		if (teams.Count == 0)
		{
			return;
		}
		List<Player> list = new List<Player>(FreeAgents(sport));
		list.Sort((Player a, Player b) => (b.Overall == a.Overall) ? a.Id.CompareTo(b.Id) : b.Overall.CompareTo(a.Overall));
		int limit = (offseason ? int.MaxValue : Math.Max(1, teams.Count / 4));
		int posts = 0;
		bool holdStars = offseason && LeagueTeams(league).Exists((Team team) => team.Manual && team.IsUser);
		foreach (Player p in list)
		{
			if (YouthHold(p, month))
			{
				continue;
			}
			bool deciding = p.DecideMonth >= 0 && p.DecideMonth <= month;
			if (p.DecideMonth > month)
			{
				continue;
			}
			if (deciding)
			{
				p.DecideMonth = -1;
			}
			if (holdStars && p.Overall >= baseOvr && _random.NextDouble() < 0.65)
			{
				p.DecideMonth = month + 1;
			}
			else
			{
				if (limit <= 0 && !deciding)
				{
					continue;
				}
				bool openMarket = offseason | deciding;
				int age = year - p.BirthYear;
				int salary = Economy.AskingSalary(p, age, month);
				int years = Economy.AskingYears(sport, age, resign: false);
				Team best = null;
				Team second = null;
				double bestScore = 0.0;
				double secondScore = 0.0;
				int bestSalary = salary;
				foreach (Team t in teams)
				{
					List<Player> signed = SignedPlayers(t.Id);
					int count = SlotCount(sport, p.Position);
					int starters = SlotStarters(sport, p.Position);
					int weakest = Weakest(signed, p.Position, count);
					int weakestStarter = Weakest(signed, p.Position, starters);
					double gain = Math.Max(0, p.Overall - weakestStarter) * 2 + Math.Max(0, p.Overall - weakest);
					if (weakest == 0)
					{
						gain += 6.0;
					}
					if (gain < (double)(openMarket ? 3 : 8))
					{
						continue;
					}
					int teamSalary = Economy.Discount(salary, sport, PriorityFactor(p, t, resign: false, out var refuses));
					if (!refuses && Economy.CanAfford(t, signed, teamSalary, resign: false) && (!(gain < 12.0) || NoRoomFor(t, p) == null || CheapestCut(t, now) <= Economy.Cap(sport) / 100))
					{
						double score = (double)((p.Overall > weakestStarter) ? 8 : 3) + Math.Min(gain, 10.0) * 0.3 + (double)(t.Rating - 50) * 0.35 + _random.NextDouble() * 4.0 + PriorityPull(p, t, p.Overall > weakestStarter) + RecruitingEdge(t) * 0.5;
						if (score > bestScore)
						{
							secondScore = bestScore;
							second = best;
							bestScore = score;
							best = t;
							bestSalary = teamSalary;
						}
						else if (score > secondScore)
						{
							secondScore = score;
							second = t;
						}
					}
				}
				if (best == null)
				{
					continue;
				}
				if (StandingOfferWins(league, p, best, now))
				{
					if (!deciding)
					{
						limit--;
					}
					continue;
				}
				bool notable = p.Overall >= baseOvr || best.IsUser;
				bool war = notable && second != null && p.Overall >= baseOvr + 4 && posts < 10;
				Sign(best, p, bestSalary, years, now, notable && posts < 10 && !war);
				if (war)
				{
					Post(league, NewsKind.Signing, Insider, "БОРЬБА ЗА ИГРОКА: " + Rosters.NewsPositionLabel(p.Position, p.Sport) + " " + p.Name + " (" + Rosters.Ovr(p) + " ОБЩ) выбрал " + TeamName(best.Id) + " вместо " + TeamName(second.Id) + ": " + Economy.ContractText(p) + ".", best.IsUser || second.IsUser, now, 3);
				}
				if (notable)
				{
					posts++;
				}
				TalksLost(league, p, best, now);
				if (!deciding)
				{
					limit--;
				}
				ArrangeRoster(best, now);
			}
		}
	}

	private void MonthlyMoves(League league, int now)
	{
		int month = GameClock.MonthIndex(now);
		if (league.TradeMonth == month)
		{
			return;
		}
		league.TradeMonth = month;
		TalksMonthly(league, now);
		if (LeagueTeams(league).Count < 2)
		{
			return;
		}
		BlockOffers(league, now);
		int year = CurrentYear();
		if (league.Level == Level.Pro)
		{
			EnsurePicks(league);
		}
		YouthSigningsMonthly(league, now);
		FreeAgency(league, year, now, offseason: false);
		if (TradeWindowOpen(league, now))
		{
			Season s = league.Current;
			bool deadlineMonth = s != null && GameClock.MonthStartDay(now) == MonthStartOf(Economy.Get(league.Sport).TradeDeadline(s.Year));
			double expected = (double)LeagueTeams(league).Count / 10.0 * (deadlineMonth ? 2.0 : 1.0);
			int attempts = (int)expected + ((_random.NextDouble() < expected - (double)(int)expected) ? 1 : 0);
			for (int i = 0; i < attempts; i++)
			{
				TryAiTrade(league, now);
			}
			if (Probe != null && attempts > 0)
			{
				Probe("trades", this, league);
			}
		}
		foreach (Team t in LeagueTeams(league))
		{
			FillHoles(t, year, now);
		}
	}

	public bool TradeWindowOpen(League league, int now)
	{
		if (league.Sport == Sport.Soccer)
		{
			return false;
		}
		Season s = league.Current;
		if (s == null)
		{
			return true;
		}
		if (s.PlayoffsCreated)
		{
			return s.Complete;
		}
		bool started = false;
		foreach (Match g in s.Games)
		{
			if (g.Played && g.Stage == GameStage.Regular)
			{
				started = true;
				break;
			}
		}
		if (!started)
		{
			return true;
		}
		return GameClock.MonthStartDay(now) <= GameClock.DayNumber(Economy.Get(league.Sport).TradeDeadline(s.Year));
	}

	public string TradeDeadlineLabel(League league)
	{
		Season s = league.Current;
		if (s == null)
		{
			return "";
		}
		return GameClock.DayLabel(GameClock.DayNumber(Economy.Get(league.Sport).TradeDeadline(s.Year)));
	}

	private Team UserProTeam(int teamId, out string error)
	{
		Team t = GetTeam(teamId);
		error = "";
		if (t == null || t.Retired || !t.IsUser)
		{
			error = "Это не одна из ваших команд.";
			return null;
		}
		if (!SportRules.ProLike(t.Sport, t.Level))
		{
			error = "Контракты, обмены и драфт доступны только профессиональным командам.";
			return null;
		}
		return t;
	}

	public CommandResult SetManaged(int teamId, bool manual, int now)
	{
		Team t = GetTeam(teamId);
		if (t == null || t.Retired || !t.IsUser)
		{
			return CommandResult.Fail("Это не одна из ваших команд.");
		}
		t.Manual = manual;
		if (!SportRules.ProLike(t.Sport, t.Level))
		{
			Revision++;
			return CommandResult.Success(manual ? ("Теперь вы сами выбираете тренеров " + TeamName(t.Id) + " (между окончанием регулярного сезона и началом следующего сезона).") : ("Спортивный директор снова отвечает за тренеров " + TeamName(t.Id) + "."));
		}
		_clockHint = now;
		League league = LeagueOf(t);
		if (!manual && league != null && league.Draft != null && !league.Draft.Complete && league.Draft.Started)
		{
			RunDraft(league, now, auto: false);
		}
		Revision++;
		return CommandResult.Success(manual ? ("Теперь вы управляете " + TeamName(t.Id) + ": контракты, свободные агенты, обмены, драфт и тренерский состав. Недостающие места в составе автоматически заполняются, чтобы матчи могли проходить.") : ("Генеральный менеджер снова управляет " + TeamName(t.Id) + "."));
	}

	public CommandResult EditClub(int teamId, string city, string clubName, int venueId, int now)
	{
		Team team = GetTeam(teamId);
		if (team == null || team.Retired || !team.IsUser)
		{
			return CommandResult.Fail("Редактировать можно только вашу команду.");
		}
		Organization org = GetOrg(team.OrgId);
		if (org == null)
		{
			return CommandResult.Fail("Организация клуба не найдена.");
		}
		Venue venue = GetVenue(venueId);
		if (venue == null)
		{
			return CommandResult.Fail("Выберите существующий спортивный объект.");
		}
		if (venueId != team.VenueId && !CanHost(venue, team.Sport, team.Level, team.Id, out var reason))
		{
			return CommandResult.Fail(reason);
		}
		CommandResult renamed = RenameOrg(team.OrgId, city, clubName);
		if (!renamed.Ok) return renamed;
		if (venueId != team.VenueId)
		{
			CommandResult moved = MoveTeam(team.Id, venueId, "арену №" + venueId, now);
			if (!moved.Ok) return CommandResult.Fail("Название клуба обновлено, но стадион не изменён: " + moved.Message);
			return CommandResult.Success(renamed.Message + " " + moved.Message);
		}
		return renamed;
	}

	public CommandResult EditPlayer(int playerId, string firstName, string lastName, int number, int birthYear, int position, int overall, int potential, int height, int weight, string hometown, string hand)
	{
		Player player = GetPlayer(playerId);
		if (player == null) return CommandResult.Fail("Игрок не найден.");
		Team team = GetTeam(player.TeamId);
		if (team == null || !team.IsUser) return CommandResult.Fail("Редактировать можно игроков ваших клубов.");
		string first = NameGenerator.CleanName(firstName, 30);
		string last = NameGenerator.CleanName(lastName, 30);
		if (first.Length == 0 || last.Length == 0) return CommandResult.Fail("Имя и фамилия не могут быть пустыми.");
		Position selected = (Position)position;
		if (!Array.Exists(Rosters.Positions(player.Sport), (Position value) => value == selected))
		{
			return CommandResult.Fail("Эта позиция недоступна для выбранного вида спорта.");
		}
		int year = CurrentYear();
		if (birthYear < year - 60 || birthYear > year - 13) return CommandResult.Fail("Проверьте год рождения игрока.");
		if (number < 0 || number > 99) return CommandResult.Fail("Игровой номер должен быть от 0 до 99.");
		if (overall < 1 || overall > 99 || potential < 1 || potential > 99) return CommandResult.Fail("Рейтинги должны быть от 1 до 99.");
		player.FirstName = first;
		player.LastName = last;
		player.Number = number;
		player.BirthYear = birthYear;
		player.Position = selected;
		player.Overall = overall;
		player.Potential = Math.Max(overall, potential);
		player.Height = Math.Max(0, Math.Min(100, height));
		player.Weight = Math.Max(0, Math.Min(500, weight));
		player.Hometown = NameGenerator.CleanName(hometown, 40);
		player.Hand = NameGenerator.CleanName(hand, 12);
		player.PeakOvr = Math.Max(player.PeakOvr, overall);
		UpdateRating(team);
		RostersChanged();
		Revision++;
		return CommandResult.Success("Данные игрока " + player.Name + " обновлены. Возраст автоматически пересчитан по году рождения.");
	}

	public CommandResult EditCoach(int coachId, string firstName, string lastName, int birthYear, int role, int development, int tactics, int motivation, int recruiting, int potential)
	{
		if (!Coaches.TryGetValue(coachId, out var coach)) return CommandResult.Fail("Тренер не найден.");
		Team team = GetTeam(coach.TeamId);
		if (team == null || !team.IsUser) return CommandResult.Fail("Редактировать можно тренеров ваших клубов.");
		string first = NameGenerator.CleanName(firstName, 30);
		string last = NameGenerator.CleanName(lastName, 30);
		if (first.Length == 0 || last.Length == 0) return CommandResult.Fail("Имя и фамилия не могут быть пустыми.");
		CoachRole selectedRole = (CoachRole)role;
		CoachRole[] allowed = coach.Sport == Sport.Football ? new CoachRole[] { CoachRole.Head, CoachRole.Offense, CoachRole.Defense } : new CoachRole[] { CoachRole.Head };
		if (!Array.Exists(allowed, (CoachRole value) => value == selectedRole)) return CommandResult.Fail("Эта специализация недоступна для данного вида спорта.");
		Coach occupied = CoachOf(team.Id, selectedRole);
		if (selectedRole != coach.Role && occupied != null && occupied.Id != coach.Id) return CommandResult.Fail("В этой команде уже есть тренер с такой специализацией.");
		int year = CurrentYear();
		if (birthYear < year - 85 || birthYear > year - 22) return CommandResult.Fail("Проверьте год рождения тренера.");
		if (development < 1 || development > 99 || tactics < 1 || tactics > 99 || motivation < 1 || motivation > 99 || recruiting < 1 || recruiting > 99 || potential < 1 || potential > 99)
			return CommandResult.Fail("Характеристики тренера должны быть от 1 до 99.");
		coach.FirstName = first;
		coach.LastName = last;
		coach.BirthYear = birthYear;
		coach.Role = selectedRole;
		coach.Development = development;
		coach.Tactics = tactics;
		coach.Motivation = motivation;
		coach.Recruiting = recruiting;
		coach.Potential = Math.Max(coach.Overall, potential);
		StaffChanged();
		Revision++;
		return CommandResult.Success("Данные тренера " + coach.Name + " обновлены.");
	}

	public CommandResult SetTicketPrices(int teamId, int residents, int visitors)
	{
		Team t = GetTeam(teamId);
		if (t == null || t.Retired || !t.IsUser)
		{
			return CommandResult.Fail("Изменять цены на билеты можно только у вашей команды.");
		}
		t.TicketPriceResidents = Math.Max(1, Math.Min(1500, residents));
		t.TicketPriceVisitors = Math.Max(1, Math.Min(1500, visitors));
		Revision++;
		return CommandResult.Success("Цены билетов обновлены: жители — " + t.TicketPriceResidents.ToString("N0") + " ¢, туристы — " + t.TicketPriceVisitors.ToString("N0") + " ¢.");
	}

	public static string SponsorTierLabel(int tier)
	{
		return tier switch
		{
			1 => "Мелкие спонсоры",
			2 => "Средние спонсоры",
			3 => "Топ-спонсоры",
			4 => "Топ-спонсор и инвесторы",
			_ => "Без дополнительного финансирования"
		};
	}

	public static string SponsorFrequencyLabel(int frequency)
	{
		return frequency switch
		{
			1 => "Еженедельно",
			2 => "Ежегодно",
			_ => "Ежемесячно"
		};
	}

	public static long SponsorAnnualRevenue(Team team)
	{
		if (team == null || team.SponsorTier <= 0) return 0L;
		double share = team.SponsorTier switch
		{
			1 => 0.02,
			2 => 0.05,
			3 => 0.12,
			4 => 0.25,
			_ => 0.0
		};
		double levelFactor = team.Level switch
		{
			Level.Minor => 0.45,
			Level.College => 0.20,
			Level.Junior => 0.08,
			_ => 1.0
		};
		return (long)Math.Round((double)Economy.Cap(team.Sport) * 1000.0 * share * levelFactor);
	}

	public static long SponsorPeriodRevenue(Team team)
	{
		long annual = SponsorAnnualRevenue(team);
		if (team == null) return 0L;
		return team.SponsorFrequency switch
		{
			1 => Math.Max(1L, annual / 52L),
			2 => annual,
			_ => Math.Max(1L, annual / 12L)
		};
	}

	private static int SponsorPeriodIndex(Team team, int now)
	{
		return team.SponsorFrequency switch
		{
			1 => GameClock.DayOf(now) / 7,
			2 => GameClock.YearOfDay(GameClock.DayOf(now)),
			_ => GameClock.MonthIndex(now)
		};
	}

	public CommandResult SetSponsorSettings(int teamId, int tier, int frequency, int now)
	{
		Team team = GetTeam(teamId);
		if (team == null || team.Retired || !team.IsUser)
		{
			return CommandResult.Fail("Настраивать спонсоров можно только у вашей команды.");
		}
		if (tier < 0 || tier > 4 || frequency < 0 || frequency > 2)
		{
			return CommandResult.Fail("Недопустимый уровень спонсорской поддержки или период выплат.");
		}
		team.SponsorTier = tier;
		team.SponsorFrequency = frequency;
		team.SponsorLastPeriod = SponsorPeriodIndex(team, now);
		Revision++;
		return CommandResult.Success("Спонсорская поддержка: " + SponsorTierLabel(tier) + ". Выплаты: " + SponsorFrequencyLabel(frequency).ToLowerInvariant() + "; ожидаемое поступление — " + SponsorPeriodRevenue(team).ToString("N0") + " ¢ за период.");
	}

	private void PaySponsors(int now)
	{
		foreach (Team team in Teams.Values)
		{
			if (!team.IsUser || team.Retired || team.SponsorTier <= 0) continue;
			EnsureClubBalance(team);
			int period = SponsorPeriodIndex(team, now);
			if (team.SponsorLastPeriod == int.MinValue)
			{
				team.SponsorLastPeriod = period;
				continue;
			}
			if (period <= team.SponsorLastPeriod) continue;
			long payment = SponsorPeriodRevenue(team) * (long)(period - team.SponsorLastPeriod);
			team.SponsorLastPeriod = period;
			if (payment <= 0L) continue;
			long credited = Math.Min(payment, (long)int.MaxValue - team.ClubBalance);
			if (credited <= 0L) continue;
			team.ClubBalance += (int)credited;
			int budgetUnits = (int)Math.Min(int.MaxValue, credited / 1000L);
			if (team.Sport == Sport.Soccer && SportRules.ProLike(team.Sport, team.Level))
			{
				EnsureTransferBudget(team);
				team.TransferBudget = Math.Min(60000, team.TransferBudget + budgetUnits);
			}
			else if (Economy.Get(team.Sport).Type == CapType.Budget)
			{
				if (team.Budget <= 0) team.Budget = Economy.Limit(team);
				team.Budget = (int)Math.Min(int.MaxValue, (long)team.Budget + budgetUnits);
			}
			Revision++;
		}
	}

	public CommandResult SetPayrollBudget(int teamId, int budgetThousands)
	{
		Team team = GetTeam(teamId);
		if (team == null || team.Retired || !team.IsUser)
		{
			return CommandResult.Fail("Изменять зарплатный бюджет можно только у вашей команды.");
		}
		if (Economy.Get(team.Sport).Type != CapType.Budget)
		{
			return CommandResult.Fail("В этой лиге зарплатный лимит установлен правилами соревнования и не редактируется вручную.");
		}
		EnsureClubBalance(team);
		int payroll = Economy.Payroll(SignedPlayers(team.Id)) + Economy.DeadNow(team);
		int current = Economy.Limit(team);
		int desired = Math.Max(payroll, Math.Max(1, Math.Min(1000000, budgetThousands)));
		long difference = (long)desired - current;
		long cost = Math.Abs(difference) * 1000L;
		if (difference > 0L && team.ClubBalance < cost)
		{
			return CommandResult.Fail("В клубной казне недостаточно средств для увеличения зарплатного бюджета на " + Economy.Money((int)Math.Min(int.MaxValue, difference)) + ".");
		}
		if (difference > 0L)
		{
			team.ClubBalance -= (int)cost;
		}
		else if (difference < 0L)
		{
			team.ClubBalance = (int)Math.Min(int.MaxValue, (long)team.ClubBalance + cost);
		}
		team.Budget = desired;
		Revision++;
		return CommandResult.Success("Зарплатный бюджет установлен: " + Economy.Money(desired) + ". Фонд зарплат с обязательствами: " + Economy.Money(payroll) + ".");
	}

	public void EnsureClubBalance(Team team)
	{
		if (team == null || team.ClubBalance >= 0)
		{
			return;
		}
		long initial = 0L;
		if (team.Sport == Sport.Soccer && SportRules.ProLike(team.Sport, team.Level))
		{
			EnsureTransferBudget(team);
			initial = (long)Math.Max(0, team.TransferBudget) * 1000L;
		}
		else if (Economy.Get(team.Sport).Type == CapType.Budget)
		{
			initial = (long)Math.Max(0, Economy.Limit(team)) * 1000L;
		}
		team.ClubBalance = (int)Math.Max(0L, Math.Min(int.MaxValue, initial));
	}

	public CommandResult TransferClubFunds(int teamId, int amount, bool toClub)
	{
		Team t = GetTeam(teamId);
		if (t == null || t.Retired || !t.IsUser)
		{
			return CommandResult.Fail("Переводы доступны только для вашей команды.");
		}
		EnsureClubBalance(t);
		if (amount <= 0)
		{
			return CommandResult.Fail("Укажите положительную сумму перевода.");
		}
		if (toClub)
		{
			if ((long)t.ClubBalance + amount > int.MaxValue)
			{
				return CommandResult.Fail("Баланс клуба достиг максимального значения.");
			}
			t.ClubBalance += amount;
			int budgetUnits = amount / 1000;
			if (t.Sport == Sport.Soccer && SportRules.ProLike(t.Sport, t.Level))
			{
				EnsureTransferBudget(t);
				t.TransferBudget = Math.Min(60000, t.TransferBudget + budgetUnits);
			}
			else if (Economy.Get(t.Sport).Type == CapType.Budget)
			{
				if (t.Budget <= 0) t.Budget = Economy.Limit(t);
				t.Budget = (int)Math.Min(int.MaxValue, (long)t.Budget + budgetUnits);
			}
		}
		else
		{
			if (t.ClubBalance < amount)
			{
				return CommandResult.Fail("На счёте клуба недостаточно средств.");
			}
			t.ClubBalance -= amount;
			int budgetUnits = amount / 1000;
			if (t.Sport == Sport.Soccer && SportRules.ProLike(t.Sport, t.Level))
			{
				EnsureTransferBudget(t);
				t.TransferBudget = Math.Max(0, t.TransferBudget - budgetUnits);
			}
			else if (Economy.Get(t.Sport).Type == CapType.Budget)
			{
				if (t.Budget <= 0) t.Budget = Economy.Limit(t);
				int payrollFloor = Economy.Payroll(SignedPlayers(t.Id)) + Economy.DeadNow(t);
				t.Budget = Math.Max(payrollFloor, t.Budget - budgetUnits);
			}
		}
		Revision++;
		return CommandResult.Success(toClub ? ("В бюджет клуба перечислено " + amount.ToString("N0") + " ¢.") : ("Из бюджета клуба перечислено в город " + amount.ToString("N0") + " ¢."));
	}

	public CommandResult Resign(int playerId, int now)
	{
		_clockHint = now;
		Player p = GetPlayer(playerId);
		if (p == null)
		{
			return CommandResult.Fail("Игрок не найден.");
		}
		Team t = UserProTeam(p.TeamId, out var error);
		if (t == null)
		{
			return CommandResult.Fail(error);
		}
		if (!t.Manual)
		{
			return CommandResult.Fail("Сначала выберите «Самостоятельное управление командой» в офисе клуба. До этого контрактами занимается ИИ-менеджер.");
		}
		if (p.ContractYears > 1)
		{
			return CommandResult.Fail(p.Name + " подписал контракт ещё на " + p.ContractYears + " сезона. Вы можете продлевать контракты игроков, у которых остался последний год.");
		}
		if (FindTalk(p.Id, t.Id) != null)
		{
			return AcceptAsk(t.Id, p.Id, now);
		}
		if (!ResignTerms(p, t, out var salary, out var years))
		{
			return CommandResult.Fail(p.Name + " больше всего хочет побеждать. Он выйдет на рынок свободных агентов и будет искать претендента на титул.");
		}
		List<Player> others = SignedPlayers(t.Id).FindAll((Player q) => q != p);
		if (!Economy.CanAfford(t, others, salary, resign: true))
		{
			return CommandResult.Fail(p.Name + " хочет получать " + Economy.Money(salary) + " за сезон. Это не укладывается в ваш " + Economy.Get(t.Sport).CapName.ToLowerInvariant() + ".");
		}
		p.Salary = salary;
		p.ContractYears = ((p.ContractYears > 0) ? p.ContractYears : 0) + years;
		Revision++;
		PostAsTeam(LeagueOf(t), t, NewsKind.Signing, "Мы продлили контракт с " + Rosters.NewsPositionLabel(p.Position, p.Sport) + " " + p.Name + ": " + Economy.Money(salary) + " за сезон до " + (CurrentYear() + p.ContractYears) + ".", now);
		return CommandResult.Success(p.Name + " продлил контракт: " + Economy.Money(salary) + " за сезон, " + years + ((years == 1) ? " ещё на один сезон." : " ещё на несколько сезонов."));
	}

	public bool ResignTerms(Player p, Team t, out int salary, out int years)
	{
		int age = AgeOf(p) + 1;
		double factor = PriorityFactor(p, t, resign: true, out var refuses);
		salary = Economy.Discount(Economy.AskingSalary(p, age, -1), t.Sport, factor);
		years = Economy.AskingYears(t.Sport, age, resign: true);
		return !refuses;
	}

	public CommandResult Release(int playerId, int now, bool stretch = false)
	{
		_clockHint = now;
		Player p = GetPlayer(playerId);
		if (p == null)
		{
			return CommandResult.Fail("Игрок не найден.");
		}
		Team t = UserProTeam(p.TeamId, out var error);
		if (t == null)
		{
			return CommandResult.Fail(error);
		}
		if (!t.Manual)
		{
			return CommandResult.Fail("Сначала выберите «Самостоятельное управление командой» в офисе клуба, чтобы отпускать игроков.");
		}
		ReleaseQuote q = QuoteRelease(p, t, now, stretch);
		ApplyRelease(p, t, q, now);
		string verb = q.Verb;
		PostAsTeam(LeagueOf(t), t, NewsKind.Release, "Мы " + verb + " игрока " + Rosters.NewsPositionLabel(p.Position, p.Sport) + " " + p.Name + "." + ((q.Total > 0) ? (" " + q.Short) : ""), now);
		ArrangeRoster(t, now);
		FillHoles(t, CurrentYear(), now, p);
		UpdateRating(t);
		Revision++;
		return CommandResult.Success(p.Name + ": " + verb + ". Теперь он свободный агент. " + ((q.Total > 0) ? q.Short : q.Rule));
	}

	public CommandResult SignFreeAgent(int teamId, int playerId, int now)
	{
		_clockHint = now;
		Team t = UserProTeam(teamId, out var error);
		if (t == null)
		{
			return CommandResult.Fail(error);
		}
		if (!t.Manual)
		{
			return CommandResult.Fail("Сначала выберите «Самостоятельное управление командой» в офисе клуба, чтобы подписывать свободных агентов.");
		}
		Player p = GetPlayer(playerId);
		if (p == null || p.Status != PlayerStatus.FreeAgent || p.Sport != t.Sport)
		{
			return CommandResult.Fail("Этот игрок не является свободным агентом в данной лиге.");
		}
		string full = NoRoomFor(t, p);
		if (full != null)
		{
			return CommandResult.Fail(full);
		}
		if (FindTalk(p.Id, t.Id) != null)
		{
			return AcceptAsk(t.Id, p.Id, now);
		}
		int age = AgeOf(p);
		int salary = AskingFrom(p, t, now, out var refuses);
		int years = Economy.AskingYears(t.Sport, age, resign: false);
		if (refuses)
		{
			return CommandResult.Fail(p.Name + " хочет побеждать и не заинтересован в переходе в команду из нижней части таблицы.");
		}
		if (!Economy.CanAfford(t, SignedPlayers(t.Id), salary, resign: false))
		{
			return CommandResult.Fail(p.Name + " хочет получать " + Economy.Money(salary) + " за сезон. Это не укладывается в ваш " + Economy.Get(t.Sport).CapName.ToLowerInvariant() + ".");
		}
		Sign(t, p, salary, years, now, p.Overall >= Rosters.Base(Level.Pro));
		ArrangeRoster(t, now);
		UpdateRating(t);
		Revision++;
		string where;
		if (p.Status != PlayerStatus.Active)
		{
			where = ((p.Status == PlayerStatus.Prospect) ? "Он начнёт в резерве." : "Для него не нашлось места в составе, поэтому его снова отпустили.");
		}
		else
		{
			where = "Он включён в основной состав.";
		}
		return CommandResult.Success(p.Name + " подписал контракт: " + Economy.ContractText(p) + ". " + where);
	}

	public int AskingFrom(Player p, Team t, int now, out bool refuses)
	{
		return Economy.Discount(Economy.AskingSalary(p, AgeOf(p), GameClock.MonthIndex(now)), t.Sport, PriorityFactor(p, t, resign: false, out refuses));
	}

	public CommandResult DraftPlayer(int teamId, int playerId, int now)
	{
		_clockHint = now;
		Team t = UserProTeam(teamId, out var error);
		if (t == null)
		{
			return CommandResult.Fail(error);
		}
		League league = LeagueOf(t);
		DraftState d = league?.Draft;
		DraftPick pick = ((d == null || d.Complete || !d.Started) ? null : d.NextPick);
		if (pick == null)
		{
			return CommandResult.Fail("Сейчас драфт не проводится.");
		}
		if (pick.TeamId != t.Id)
		{
			return CommandResult.Fail("Сейчас выбирает " + TeamName(pick.TeamId) + ".");
		}
		Player p = GetPlayer(playerId);
		if (p == null || p.Status != PlayerStatus.DraftPool || p.Sport != t.Sport)
		{
			return CommandResult.Fail("Этот игрок недоступен.");
		}
		MakePick(league, d, pick, t, p, now);
		RunDraft(league, now, auto: false);
		return CommandResult.Success("Вы выбрали на драфте " + p.Name + " под номером " + pick.Number + ".");
	}

	public CommandResult AutoPick(int teamId, int now)
	{
		_clockHint = now;
		Team t = UserProTeam(teamId, out var error);
		if (t == null)
		{
			return CommandResult.Fail(error);
		}
		League league = LeagueOf(t);
		DraftState d = league?.Draft;
		DraftPick pick = ((d == null || d.Complete || !d.Started) ? null : d.NextPick);
		if (pick == null || pick.TeamId != t.Id)
		{
			return CommandResult.Fail("Сейчас не ваш ход на драфте.");
		}
		Player p = BestAvailable(league, t);
		if (p == null)
		{
			return CommandResult.Fail("На драфте больше не осталось игроков.");
		}
		MakePick(league, d, pick, t, p, now);
		RunDraft(league, now, auto: false);
		return CommandResult.Success("Ваши скауты выбрали " + p.Name + " под номером " + pick.Number + ".");
	}

	public bool IsContending(Team t)
	{
		League league = LeagueOf(t);
		if (league == null)
		{
			return true;
		}
		List<int> order = ContentionOrder(league);
		int i = order.IndexOf(t.Id);
		if (i >= 0)
		{
			return i < Math.Max(1, order.Count / 2);
		}
		return false;
	}

	public string OffseasonNote(League league, int now)
	{
		if (!SportRules.ProLike(league.Sport, league.Level))
		{
			return "";
		}
		EconomyProfile e = Economy.Get(league.Sport);
		List<string> parts = new List<string>();
		bool second = league.Level != Level.Pro;
		foreach (Season item in SeasonsOldestFirst(league))
		{
			int y = item.Year;
			if (second)
			{
				if (league.LeagueYearDoneFor < y)
				{
					parts.Add("Открытие рынка свободных агентов: " + MonthYear(e.LeagueYearDate(y)));
				}
				if (parts.Count > 0)
				{
					break;
				}
				continue;
			}
			if (!HasDraft(league.Sport))
			{
				if (league.DraftDoneFor < y)
				{
					parts.Add("Молодые перспективные игроки: " + MonthYear(e.DraftDate(y)));
				}
				if (league.LeagueYearDoneFor < y)
				{
					parts.Add("Открытие рынка свободных агентов: " + MonthYear(e.LeagueYearDate(y)));
				}
				if (parts.Count > 0)
				{
					break;
				}
				continue;
			}
			if (e.LotteryOdds != null && league.LotteryDoneFor < y)
			{
				parts.Add("Лотерея драфта: " + MonthYear(e.LotteryDate(y)));
			}
			if (league.DraftDoneFor < y)
			{
				parts.Add(DraftYearLabel(league, y) + " " + e.DraftName + ": " + MonthYear(e.DraftDate(y)));
			}
			if (league.LeagueYearDoneFor < y)
			{
				parts.Add("Открытие рынка свободных агентов: " + MonthYear(e.LeagueYearDate(y)));
			}
			if (parts.Count > 0)
			{
				break;
			}
		}
		if (parts.Count != 0)
		{
			return string.Join(". ", parts.ToArray()) + ".";
		}
		return "";
	}

	private static string MonthYear(DateTime d)
	{
		return SportRules.MonthNames[d.Month - 1] + " " + d.Year;
	}

	public Team WaitingOnUserPick(League league)
	{
		DraftState d = league.Draft;
		if (d == null || d.Complete || !d.Started)
		{
			return null;
		}
		DraftPick pick = d.NextPick;
		Team t = ((pick == null) ? null : GetTeam(pick.TeamId));
		if (t == null || !t.Manual)
		{
			return null;
		}
		return t;
	}

	public void RostersChanged()
	{
		_rostersDirty = true;
	}

	private void RebuildIndex()
	{
		if (!_rostersDirty)
		{
			return;
		}
		_rosters.Clear();
		_prospects.Clear();
		_pools.Clear();
		foreach (Player p in Players.Values)
		{
			Dictionary<int, List<Player>> map;
			int key;
			switch (p.Status)
			{
			case PlayerStatus.Active:
				map = _rosters;
				key = p.TeamId;
				break;
			case PlayerStatus.Prospect:
				map = _prospects;
				key = p.TeamId;
				break;
			default:
				map = _pools;
				key = PoolKey(p.Status, p.Sport);
				break;
			case PlayerStatus.Retired:
			case PlayerStatus.Abroad:
				continue;
			}
			if (!map.TryGetValue(key, out var list))
			{
				list = (map[key] = new List<Player>());
			}
			list.Add(p);
		}
		foreach (List<Player> value in _rosters.Values)
		{
			SortById(value);
		}
		foreach (List<Player> value2 in _prospects.Values)
		{
			SortById(value2);
		}
		foreach (List<Player> value3 in _pools.Values)
		{
			SortById(value3);
		}
		_rostersDirty = false;
	}

	private static int ById(Player a, Player b)
	{
		return a.Id.CompareTo(b.Id);
	}

	private static void SortById(List<Player> list)
	{
		for (int i = 1; i < list.Count; i++)
		{
			if (list[i - 1].Id > list[i].Id)
			{
				list.Sort(ById);
				break;
			}
		}
	}

	private static int PoolKey(PlayerStatus status, Sport sport)
	{
		return (int)((int)status * 100 + sport);
	}

	private static List<Player> Lookup(Dictionary<int, List<Player>> map, int key)
	{
		if (map.TryGetValue(key, out var list))
		{
			return list;
		}
		return map[key] = new List<Player>();
	}

	public List<Player> RosterOf(int teamId)
	{
		RebuildIndex();
		return Lookup(_rosters, teamId);
	}

	public List<Player> ProspectsOf(int teamId)
	{
		RebuildIndex();
		return Lookup(_prospects, teamId);
	}

	public List<Player> SignedPlayers(int teamId)
	{
		List<Player> list = new List<Player>(RosterOf(teamId));
		list.AddRange(ProspectsOf(teamId));
		return list;
	}

	public List<Player> FreeAgents(Sport sport)
	{
		RebuildIndex();
		return Lookup(_pools, PoolKey(PlayerStatus.FreeAgent, sport));
	}

	public List<Player> DraftPool(Sport sport)
	{
		RebuildIndex();
		return Lookup(_pools, PoolKey(PlayerStatus.DraftPool, sport));
	}

	public List<Player> RecruitPool(Sport sport)
	{
		RebuildIndex();
		return Lookup(_pools, PoolKey(PlayerStatus.RecruitPool, sport));
	}

	public Player GetPlayer(int id)
	{
		if (!Players.TryGetValue(id, out var p))
		{
			return null;
		}
		return p;
	}

	private Dictionary<string, int> NameUse()
	{
		if (_nameUse == null)
		{
			_nameUse = new Dictionary<string, int>();
			foreach (Player p in Players.Values)
			{
				_nameUse.TryGetValue(p.Name, out var n);
				_nameUse[p.Name] = n + 1;
			}
		}
		return _nameUse;
	}

	private void AddPlayer(Player p)
	{
		Players[p.Id] = p;
		RostersChanged();
	}

	private void RemovePlayer(Player p)
	{
		if (p != null && Players.Remove(p.Id))
		{
			if (_nameUse != null && _nameUse.TryGetValue(p.Name, out var n))
			{
				_nameUse[p.Name] = n - 1;
			}
			RostersChanged();
		}
	}

	public int CurrentYear()
	{
		int t = ((_clockHint != int.MinValue) ? _clockHint : LastTime);
		if (t != int.MinValue)
		{
			return GameClock.YearOfDay(GameClock.DayOf(t));
		}
		return 2026;
	}

	public int AgeOf(Player p)
	{
		return CurrentYear() - p.BirthYear;
	}

	private void CreateRoster(Team team, int targetRating)
	{
		List<Player> roster = Rosters.GenerateRoster(team, targetRating, CurrentYear(), _random, () => NextId++, NameUse());
		if (SportRules.ProLike(team.Sport, team.Level))
		{
			if (team.Budget <= 0)
			{
				team.Budget = Economy.NewBudget(team.Sport, _random, targetRating);
			}
			Economy.BalancePayroll(team, roster, _random);
		}
		foreach (Player p in roster)
		{
			Players[p.Id] = p;
		}
		RostersChanged();
		UpdateRating(team);
		EnsureStaff(team, _clockHint);
	}

	public void UpdateRating(Team team)
	{
		List<Player> roster = RosterOf(team.Id);
		if (roster.Count > 0)
		{
			team.Rating = Rosters.TeamRating(team.Sport, team.Level, roster);
		}
	}

	public void EnsureRosters()
	{
		HashSet<int> withPlayers = new HashSet<int>();
		foreach (Player p in Players.Values)
		{
			if (p.Status == PlayerStatus.Active)
			{
				withPlayers.Add(p.TeamId);
			}
		}
		foreach (Team t in new List<Team>(Teams.Values))
		{
			if (!t.Retired && !withPlayers.Contains(t.Id))
			{
				CreateRoster(t, t.Rating);
			}
		}
	}

	public void AfterLoad()
	{
		_nameUse = null;
		RostersChanged();
		int year = CurrentYear();
		HashSet<int> needContracts = new HashSet<int>();
		foreach (Player p in Players.Values)
		{
			Team t = GetTeam(p.TeamId);
			if (t != null && p.Status != PlayerStatus.FreeAgent && p.Status != PlayerStatus.DraftPool && p.Status != PlayerStatus.RecruitPool)
			{
				p.Sport = t.Sport;
				p.Level = t.Level;
			}
			if (SportRules.ProLike(p.Sport, p.Level) && p.Status == PlayerStatus.Active && p.ContractYears <= 0 && t != null)
			{
				if (t.Budget <= 0)
				{
					t.Budget = Economy.NewBudget(t.Sport, _random, t.Rating);
				}
				Economy.InitialContract(p, t, year - p.BirthYear, _random);
				needContracts.Add(t.Id);
			}
			p.Season = Stats.Fit(p.Season, p.Sport);
			p.Career = Stats.Fit(p.Career, p.Sport);
			if (p.Playoff == null)
			{
				p.Playoff = new int[0];
			}
			if (p.LastSeason == null)
			{
				p.LastSeason = new int[0];
			}
		}
		RostersChanged();
		foreach (Team t2 in Teams.Values)
		{
			if (SportRules.ProLike(t2.Sport, t2.Level) && t2.Budget <= 0)
			{
				t2.Budget = Economy.NewBudget(t2.Sport, _random, t2.Rating);
			}
			if (needContracts.Contains(t2.Id))
			{
				Economy.BalancePayroll(t2, SignedPlayers(t2.Id), _random);
			}
		}
		PrunePlayers();
		EnsureRosters();
		if (LoadedWithoutCoaches)
		{
			EnsureAllStaff((_clockHint != int.MinValue) ? _clockHint : LastTime);
		}
		else
		{
			CleanUpCoaches();
		}
		LoadedWithoutCoaches = false;
		if (LoadedWithoutBios)
		{
			GiveBios();
		}
		LoadedWithoutBios = false;
	}

	private void GiveBios()
	{
		foreach (Player p in Players.Values)
		{
			if (p.Height <= 0)
			{
				Bios.Give(p, new Random(p.Id * 7919 + 13), cityKid: false);
			}
			p.PeakOvr = Math.Max(p.PeakOvr, Rosters.Ovr(p));
		}
		foreach (League league in Leagues)
		{
			foreach (AwardRecord a in league.Awards)
			{
				if (AwardBook.OldTrophyNames.TryGetValue(a.Trophy, out var renamed))
				{
					a.Trophy = renamed;
				}
			}
		}
	}

	public string HometownOf(Player p)
	{
		if (p == null)
		{
			return "";
		}
		if (Bios.IsLocal(p))
		{
			return Bios.HometownLabel(p, CityName);
		}
		Team t = GetTeam(p.TeamId);
		if (IsCityPlayer(p) || (p.Level == Level.HighSchool && t != null && t.IsUser))
		{
			if (!string.IsNullOrEmpty(CityName))
			{
				return CityName;
			}
			return "Ваш город";
		}
		return p.Hometown ?? "";
	}

	private void PrunePlayers()
	{
		List<Player> remove = null;
		foreach (Player p in Players.Values)
		{
			bool keep;
			if (p.Status == PlayerStatus.Active || p.Status == PlayerStatus.Prospect)
			{
				Team t = GetTeam(p.TeamId);
				keep = t != null && !t.Retired;
			}
			else
			{
				keep = GetLeague(p.Sport, p.Level) != null;
			}
			if (!keep)
			{
				if (remove == null)
				{
					remove = new List<Player>();
				}
				remove.Add(p);
			}
		}
		if (remove == null)
		{
			return;
		}
		foreach (Player p2 in remove)
		{
			RemovePlayer(p2);
		}
	}

	private void RollStats(Player p, Sport sport, string seasonLabel)
	{
		p.Season = Stats.Fit(p.Season, sport);
		p.Career = Stats.Fit(p.Career, sport);
		int ovr = Rosters.Ovr(p);
		if (ovr > p.PeakOvr)
		{
			p.PeakOvr = ovr;
		}
		if (!Stats.IsEmpty(p.Season))
		{
			p.Seasons++;
			int teamId = ((p.TeamId >= 0) ? p.TeamId : p.LastTeamId);
			p.History.Add(new SeasonLine
			{
				Season = seasonLabel,
				TeamId = teamId,
				Team = ((teamId >= 0) ? TeamName(teamId) : ""),
				Level = p.Level,
				Ovr = ovr,
				Stats = (int[])p.Season.Clone(),
				Playoff = (Stats.IsEmpty(p.Playoff) ? new int[0] : ((int[])Stats.Fit(p.Playoff, sport).Clone()))
			});
			while (p.History.Count > 30)
			{
				p.History.RemoveAt(0);
			}
		}
		p.Career = Stats.Fit(p.Career, sport);
		Stats.AddInto(p.Career, Stats.Fit(p.Season, sport));
		p.LastSeason = p.Season;
		p.Season = Stats.Empty(sport);
	}

	private void AdvanceRosters(League league, int now)
	{
		string label = ((league.Current != null) ? SeasonLabel(league, league.Current) : CurrentYear().ToString());
		foreach (Player p in new List<Player>(Players.Values))
		{
			if (p.Sport == league.Sport && p.Level == league.Level && p.Status != PlayerStatus.DraftPool && p.Status != PlayerStatus.RecruitPool && p.Status != PlayerStatus.Abroad && (p.Status != PlayerStatus.FreeAgent || !Stats.IsEmpty(p.Season)))
			{
				RollStats(p, league.Sport, label);
			}
		}
		if (SportRules.IsSecondDivision(league.Sport, league.Level))
		{
			return;
		}
		if (league.Level == Level.Minor)
		{
			AdvanceMinorRosters(league, now);
		}
		else
		{
			if (!SportRules.IsSchool(league.Level))
			{
				return;
			}
			Sport sport = league.Sport;
			League college = ((league.Level == Level.HighSchool || league.Level == Level.Junior) ? GetLeague(sport, Level.College) : null);
			League junior = ((league.Level == Level.HighSchool) ? GetLeague(sport, Level.Junior) : null);
			League pro = ((league.Level == Level.College || league.Level == Level.Junior) ? GetLeague(sport, Level.Pro) : null);
			List<Player> pool = ((league.Level == Level.College || league.Level == Level.Junior) ? RecruitPool(sport).FindAll((Player player) => player.Level == league.Level) : null);
			HashSet<int> fromPool = new HashSet<int>();
			if (pool != null)
			{
				foreach (Player p2 in pool)
				{
					fromPool.Add(p2.Id);
				}
			}
			Dictionary<int, List<Player>> committed = null;
			if (league.Level == Level.College && league.Current != null)
			{
				UpdatePrestige(league, league.Current);
				committed = SigningDay(league, league.Current.Year, pool);
				foreach (List<Player> value in committed.Values)
				{
					foreach (Player p3 in value)
					{
						fromPool.Add(p3.Id);
					}
				}
			}
			List<KeyValuePair<Team, double>> order = new List<KeyValuePair<Team, double>>();
			foreach (int teamId in league.TeamIds)
			{
				Team team = GetTeam(teamId);
				if (team != null && !team.Retired)
				{
					order.Add(new KeyValuePair<Team, double>(team, (double)team.Rating + Rosters.Normal(_random, 0.0, 6.0) + (double)(team.IsUser ? 6 : 0) + RecruitingEdge(team) * 2.0 + ((team.Prestige > 0) ? ((double)(team.Prestige - 50) * 0.15) : 0.0)));
				}
			}
			order.Sort((KeyValuePair<Team, double> a, KeyValuePair<Team, double> b) => b.Value.CompareTo(a.Value));
			List<Team> byStrength = new List<Team>();
			foreach (KeyValuePair<Team, double> kv in order)
			{
				byStrength.Add(kv.Key);
			}
			byStrength.Sort((Team a, Team b) => b.Rating.CompareTo(a.Rating));
			int posted = 0;
			foreach (KeyValuePair<Team, double> entry in order)
			{
				Team team2 = entry.Key;
				Organization org = GetOrg(team2.OrgId);
				List<Player> roster = new List<Player>(RosterOf(team2.Id));
				List<KeyValuePair<Player, Rosters.Change>> changes = new List<KeyValuePair<Player, Rosters.Change>>();
				Func<Position, Player> recruit = null;
				if (pool != null)
				{
					recruit = (Position pos) => TakeCommitted(committed, team2, pos) ?? TakeRecruit(pool, pos, team2, byStrength);
				}
				List<Player> joined;
				foreach (Player p4 in Rosters.AdvanceSchoolSeason(team2, roster, _random, () =>
				{
					SportsWorld sportsWorld = this;
					int nextId = NextId;
					sportsWorld.NextId = nextId + 1;
					return nextId;
				}, NameUse(), recruit, out joined, changes, SchoolDevelopmentBoost(team2), RecruitingEdge(team2)))
				{
					Graduate(p4, team2, org, college, junior, pro, now);
				}
				if (team2.Level == Level.Junior)
				{
					foreach (Player p5 in joined)
					{
						if (p5.BirthYear <= 0 || CurrentYear() - p5.BirthYear > 21)
						{
							p5.BirthYear = CurrentYear() - 17;
						}
					}
				}
				FixRetiredNumbers(team2, joined);
				foreach (Player p6 in joined)
				{
					Players[p6.Id] = p6;
					if (fromPool.Contains(p6.Id) && posted < 12)
					{
						PostCommit(p6, team2, now);
						posted++;
					}
				}
				if (team2.IsUser)
				{
					PostDevelopment(league, team2, changes, now);
				}
				RostersChanged();
			}
			if (pool != null)
			{
				foreach (Player p7 in pool)
				{
					RemovePlayer(p7);
				}
			}
			if (committed != null)
			{
				foreach (List<Player> value2 in committed.Values)
				{
					foreach (Player p8 in value2)
					{
						RemovePlayer(p8);
					}
				}
			}
			RostersChanged();
			List<List<Player>> rosters = new List<List<Player>>();
			foreach (int teamId2 in league.TeamIds)
			{
				rosters.Add(RosterOf(teamId2));
			}
			Rosters.Normalize(league.Sport, league.Level, rosters, null);
			foreach (int teamId3 in league.TeamIds)
			{
				Team team3 = GetTeam(teamId3);
				if (team3 != null)
				{
					UpdateRating(team3);
				}
			}
		}
	}

	private Player TakeRecruit(List<Player> pool, Position pos, Team team, List<Team> byStrength)
	{
		Player best = null;
		double bestScore = double.MinValue;
		foreach (Player p in pool)
		{
			if (p.Position == pos)
			{
				double score = (double)p.Overall + 0.5 * (double)(p.Potential - p.Overall);
				if (score > bestScore)
				{
					bestScore = score;
					best = p;
				}
			}
		}
		if (best == null)
		{
			return null;
		}
		double chance = (team.IsUser ? 0.85 : 0.6) * RecruitInterest(best, team, byStrength);
		if (_random.NextDouble() >= Math.Min(0.95, chance))
		{
			return null;
		}
		pool.Remove(best);
		best.Career = Stats.Empty(best.Sport);
		best.LastSeason = new int[0];
		best.Season = Stats.Empty(best.Sport);
		best.Seasons = 0;
		return best;
	}

	private void Graduate(Player p, Team team, Organization org, League college, League junior, League pro, int now)
	{
		string school;
		if (org == null)
		{
			school = "";
		}
		else
		{
			school = ((team.Level == Level.Junior) ? org.FullName : org.Name);
		}
		if (team.Level == Level.Junior)
		{
			if (pro != null && p.Overall >= 68)
			{
				ToDraftPool(p, team, school, 20, now, p.ClassYear < 3);
			}
			else if (college != null && p.Overall >= 50)
			{
				p.Level = Level.College;
				p.Status = PlayerStatus.RecruitPool;
				p.TeamId = -1;
				p.ClassYear = 0;
				p.From = school;
				p.PrevOverall = p.Overall;
				RostersChanged();
			}
			else
			{
				RemovePlayer(p);
			}
			return;
		}
		if (team.Level == Level.College)
		{
			League mine = GetLeague(team.Sport, Level.College);
			CareerOver(p, Level.College, (mine != null && mine.Current != null) ? mine.Current.Year : (CurrentYear() - 1), now);
		}
		if (team.Level == Level.HighSchool && (college != null || junior != null))
		{
			int ovr = (int)Math.Round(53.0 + (double)(p.Overall - 55) * 0.7);
			bool committedToCollege = college != null && IsCommitted(p.Id);
			bool num = !committedToCollege && junior != null && ((college == null) ? (ovr >= 41) : ((ovr >= 47) ? (_random.NextDouble() < 0.3) : (ovr >= 41)));
			if (committedToCollege)
			{
				ovr = Math.Max(ovr, 47);
			}
			if (num)
			{
				int gap = Math.Max(0, p.Potential - p.Overall);
				p.Overall = Rosters.Clamp(ovr, Rosters.MinOverall(Level.Junior), Rosters.MaxOverall(Level.Junior));
				p.Potential = p.Overall + Math.Max(4, (int)Math.Round((double)gap * 0.8) + _random.Next(3, 10));
				p.PrevOverall = p.Overall;
				p.Level = Level.Junior;
				p.Status = PlayerStatus.RecruitPool;
				p.TeamId = -1;
				p.ClassYear = 0;
				p.BirthYear = CurrentYear() - 18;
				p.From = school;
				RostersChanged();
				return;
			}
			if (college != null && ovr >= 47)
			{
				int gap2 = Math.Max(0, p.Potential - p.Overall);
				p.Overall = Rosters.Clamp(ovr, Rosters.MinOverall(Level.College), Rosters.MaxOverall(Level.College));
				p.Potential = p.Overall + Math.Max(3, (int)Math.Round((double)gap2 * 0.8) + _random.Next(2, 9));
				p.PrevOverall = p.Overall;
				p.Level = Level.College;
				p.Status = PlayerStatus.RecruitPool;
				p.TeamId = -1;
				p.ClassYear = 0;
				p.From = school;
				RostersChanged();
				return;
			}
		}
		if (team.Level == Level.College && pro != null && (int)Math.Round(64.0 + (double)(p.Overall - 70) * 0.6) >= 58)
		{
			ToDraftPool(p, team, school, (p.ClassYear >= 4) ? 22 : 21, now, p.ClassYear < 4);
		}
		else
		{
			RemovePlayer(p);
		}
	}

	private void ToDraftPool(Player p, Team team, string school, int age, int now, bool early)
	{
		int ovr = (int)Math.Round(64.0 + (double)(p.Overall - 70) * 0.6);
		Rosters.PeakAges(p.Sport, p.Position, out var peakStart, out var _);
		int collegeGap = Math.Max(0, p.Potential - p.Overall);
		int growth = (int)Math.Round(Math.Max(2.0, (double)(peakStart - age) * 1.8 + (double)collegeGap * 0.5 + Rosters.Normal(_random, 1.0, 3.0)));
		p.Overall = Rosters.Clamp(ovr, Rosters.MinOverall(Level.Pro), Rosters.MaxOverall(Level.Pro));
		p.Potential = Math.Min(p.Overall + growth, Rosters.Base(Level.Pro) + 20);
		p.PrevOverall = p.Overall;
		p.Level = Level.Pro;
		p.Status = PlayerStatus.DraftPool;
		p.TeamId = -1;
		p.ClassYear = 0;
		p.BirthYear = CurrentYear() - age;
		p.From = school;
		p.Career = Stats.Empty(p.Sport);
		p.LastSeason = new int[0];
		p.Season = Stats.Empty(p.Sport);
		p.Seasons = 0;
		RostersChanged();
		if (team.IsUser)
		{
			PostDeclares(p, team, early, now);
		}
	}

	public List<Player> PlayersInLeague(League league)
	{
		List<Player> list = new List<Player>();
		foreach (int id in league.TeamIds)
		{
			list.AddRange(RosterOf(id));
			if (SportRules.ProLike(league.Sport, league.Level))
			{
				list.AddRange(ProspectsOf(id));
			}
		}
		return list;
	}

	public bool ShowingLastSeason(League league)
	{
		Season s = league.Current;
		if (s == null)
		{
			return true;
		}
		foreach (Match g in s.Games)
		{
			if (g.Played && g.Stage == GameStage.Regular)
			{
				return false;
			}
		}
		return true;
	}

	public static PlayerPriority NewPriority(Random random)
	{
		int r = random.Next(100);
		if (r < 20)
		{
			return PlayerPriority.Balanced;
		}
		if (r < 42)
		{
			return PlayerPriority.Winning;
		}
		if (r < 64)
		{
			return PlayerPriority.Money;
		}
		if (r < 80)
		{
			return PlayerPriority.PlayingTime;
		}
		if (r < 92)
		{
			return PlayerPriority.Loyalty;
		}
		return PlayerPriority.Hometown;
	}

	public static string PriorityLabel(PlayerPriority p)
	{
		return p switch
		{
			PlayerPriority.Winning => "Победы", 
			PlayerPriority.Loyalty => "Верность", 
			PlayerPriority.Money => "Деньги", 
			PlayerPriority.PlayingTime => "Игровое время", 
			PlayerPriority.Hometown => "Родной город", 
			_ => "Сбалансированные условия", 
		};
	}

	private bool WouldStart(Player p, Team t)
	{
		List<Player> signed = SignedPlayers(t.Id);
		signed.Remove(p);
		int starters = SlotStarters(t.Sport, p.Position);
		if (starters <= 0)
		{
			return false;
		}
		return p.Overall > Weakest(signed, p.Position, starters);
	}

	public double PriorityFactor(Player p, Team t, bool resign, out bool refuses)
	{
		refuses = false;
		switch (p.Priority)
		{
		case PlayerPriority.Winning:
		{
			League league = LeagueOf(t);
			List<int> order = ((league == null) ? new List<int>() : ContentionOrder(league));
			int i = order.IndexOf(t.Id);
			double rank = ((order.Count <= 1 || i < 0) ? 0.5 : ((double)i / (double)(order.Count - 1)));
			if (rank > 0.75 && p.Overall >= Rosters.Base(Level.Pro))
			{
				refuses = true;
			}
			if (!(rank < 0.34))
			{
				if (!(rank < 0.67))
				{
					return 1.25;
				}
				return 1.05;
			}
			return 0.92;
		}
		case PlayerPriority.Loyalty:
			if (!resign && p.LastTeamId != t.Id)
			{
				return 1.0;
			}
			return 0.82;
		case PlayerPriority.Money:
			return 1.1;
		case PlayerPriority.PlayingTime:
			if (!WouldStart(p, t))
			{
				return 1.25;
			}
			return 0.95;
		case PlayerPriority.Hometown:
			if (!t.IsUser || !IsCityPlayer(p))
			{
				return 1.05;
			}
			return 0.82;
		default:
			return 1.0;
		}
	}

	private double LeaveMultiplier(Player p, Team t)
	{
		switch (p.Priority)
		{
		case PlayerPriority.Loyalty:
			return 0.3;
		case PlayerPriority.Winning:
			if (!IsContending(t))
			{
				return 3.0;
			}
			return 0.7;
		case PlayerPriority.Money:
			return 1.6;
		case PlayerPriority.PlayingTime:
			if (!WouldStart(p, t))
			{
				return 2.5;
			}
			return 0.8;
		case PlayerPriority.Hometown:
			if (!t.IsUser || !IsCityPlayer(p))
			{
				return 1.2;
			}
			return 0.3;
		default:
			return 1.0;
		}
	}

	private double PriorityPull(Player p, Team t, bool wouldStart)
	{
		switch (p.Priority)
		{
		case PlayerPriority.Winning:
			return (double)(t.Rating - 50) * 0.35;
		case PlayerPriority.Money:
		{
			int room = Economy.Limit(t) - Economy.Payroll(SignedPlayers(t.Id)) - Economy.DeadNow(t);
			return Math.Max(0.0, Math.Min(8.0, (double)room * 10.0 / (double)Math.Max(1, Economy.Limit(t))));
		}
		case PlayerPriority.PlayingTime:
			return wouldStart ? 6 : (-3);
		case PlayerPriority.Loyalty:
			return (p.LastTeamId == t.Id) ? 8 : 0;
		case PlayerPriority.Hometown:
			return (t.IsUser && IsCityPlayer(p)) ? 8 : 0;
		default:
			return 0.0;
		}
	}

	private double RecruitInterest(Player p, Team college, List<Team> byStrength)
	{
		switch (p.Priority)
		{
		case PlayerPriority.Winning:
		{
			int i = byStrength.IndexOf(college);
			double rank = ((byStrength.Count <= 1 || i < 0) ? 0.5 : ((double)i / (double)(byStrength.Count - 1)));
			if (!(rank < 0.34))
			{
				if (!(rank < 0.67))
				{
					return 0.6;
				}
				return 1.0;
			}
			return 1.3;
		}
		case PlayerPriority.PlayingTime:
			if (!WouldStartCollege(p, college))
			{
				return 0.8;
			}
			return 1.3;
		case PlayerPriority.Hometown:
			if (!college.IsUser || !IsCityPlayer(p))
			{
				return 0.7;
			}
			return 1.4;
		default:
			return 1.0;
		}
	}

	private bool WouldStartCollege(Player p, Team t)
	{
		int starters = SlotStarters(t.Sport, p.Position, t.Level);
		if (starters <= 0)
		{
			return false;
		}
		return p.Overall + 3 > Weakest(RosterOf(t.Id), p.Position, starters);
	}

	private void UpdateSurprise(League league, Team t, double result, double expected, int now)
	{
		if (t == null)
		{
			return;
		}
		int games = Math.Max(10, TargetGames(league));
		double d = 1.0 - 2.5 / (double)games;
		t.Surprise = (int)Math.Round((double)t.Surprise * d + 1000.0 * (result - expected));
		double threshold = CinderellaK * 480.0 / Math.Sqrt(1.0 - d * d);
		int seasonYear = ((league.Current != null) ? league.Current.Year : CurrentYear());
		if (t.Cinderella == 0 && (double)t.Surprise >= threshold && t.CinderellaYear != seasonYear && IsUnderdog(league, t) && WinningRecord(league, t))
		{
			t.CinderellaYear = seasonYear;
			if (!(_random.NextDouble() >= CinderellaChance(league.Sport)))
			{
				t.Cinderella = 10;
				Post(league, NewsKind.Upset, Wire, "НЕОЖИДАННЫЙ ПРОРЫВ: Никто этого не ожидал. " + TeamName(t.Id) + " продолжает выигрывать матчи, в которых считалась аутсайдером, и теперь она " + TeamRecordText(league, t) + ". Вот это сенсация!", t.IsUser, now, 3);
				AddNote(league, "Неожиданный прорыв: " + TeamName(t.Id) + " неожиданно прошла далеко в плей-офф (" + TeamRecordText(league, t) + ").");
				if (_random.NextDouble() < 0.45)
				{
					EmergeStar(league, t, now, "во время своего неожиданного прорыва");
				}
			}
		}
		else if (t.Cinderella > 0 && (double)t.Surprise < threshold / 3.0)
		{
			t.Cinderella = 0;
		}
	}

	public static double CinderellaChance(Sport sport)
	{
		return sport switch
		{
			Sport.Hockey => 0.3, 
			Sport.Baseball => 0.3, 
			Sport.Basketball => 0.55, 
			Sport.Football => 0.55, 
			_ => 0.6, 
		};
	}

	private bool IsUnderdog(League league, Team t)
	{
		int stronger = 0;
		int count = 0;
		foreach (int id in league.TeamIds)
		{
			Team o = GetTeam(id);
			if (o != null && !o.Retired)
			{
				count++;
				if (o.Rating > t.Rating)
				{
					stronger++;
				}
			}
		}
		if (count >= 4)
		{
			return stronger >= count / 2;
		}
		return false;
	}

	private bool WinningRecord(League league, Team t)
	{
		Season s = league.Current;
		if (s == null)
		{
			return false;
		}
		foreach (StandingRow r in Standings.Compute(league, s))
		{
			if (r.TeamId == t.Id)
			{
				return r.Played >= 5 && r.Wins > r.Losses + r.OvertimeLosses;
			}
		}
		return false;
	}

	private string TeamRecordText(League league, Team t)
	{
		Season s = league.Current;
		if (s == null)
		{
			return "в отличной форме";
		}
		foreach (StandingRow r in Standings.Compute(league, s))
		{
			if (r.TeamId == t.Id)
			{
				return Standings.RecordText(t.Sport, r);
			}
		}
		return "в отличной форме";
	}

	private void EmergeStar(League league, Team t, int now, string when)
	{
		int seasonYear = ((league.Current != null) ? league.Current.Year : CurrentYear());
		if (t.LastStarYear == seasonYear)
		{
			return;
		}
		int year = CurrentYear();
		Player best = null;
		double bestScore = double.MinValue;
		foreach (Player p in RosterOf(t.Id))
		{
			int age = year - p.BirthYear;
			if (t.Level != Level.Pro || age <= 26)
			{
				double score = (double)((p.Potential - p.Overall) * 2) + (double)p.Overall * 0.5 + _random.NextDouble() * 3.0;
				if (score > bestScore)
				{
					bestScore = score;
					best = p;
				}
			}
		}
		if (best != null)
		{
			t.LastStarYear = seasonYear;
			int before = Rosters.Ovr(best);
			best.Overall = Rosters.Clamp(best.Overall + _random.Next(2, 5), Rosters.MinOverall(best.Level), Rosters.MaxOverall(best.Level));
			best.Potential = Math.Min(Rosters.Base(best.Level) + 20, Math.Max(best.Potential, best.Overall) + _random.Next(3, 7));
			if (best.Potential < best.Overall)
			{
				best.Potential = best.Overall;
			}
			UpdateRating(t);
			int age2 = year - best.BirthYear;
			Post(league, NewsKind.Development, Wire, "РОЖДЕНИЕ ЗВЕЗДЫ: " + Rosters.NewsPositionLabel(best.Position, best.Sport) + " " + best.Name + " из клуба " + TeamName(t.Id) + " стал лидером " + when + " (" + before + " → " + Rosters.Ovr(best) + " ОБЩ, оценка скаутов " + ((t.Level == Level.Pro) ? Rosters.Grade(best, age2) : Rosters.Grade(best, 0)) + ").", t.IsUser, now, 3);
			AddNote(league, "Родилась новая звезда: " + best.Name + " из клуба " + TeamName(t.Id) + " совершил прорыв " + when + ".");
			Revision++;
		}
	}

	private void GiveProspectBackground(Player p)
	{
		int draftYear = CurrentYear();
		int age = draftYear - p.BirthYear;
		Level level;
		int seasons;
		switch (p.Sport)
		{
		case Sport.Football:
			level = Level.College;
			seasons = Math.Max(2, Math.Min(4, age - 18));
			break;
		case Sport.Basketball:
			level = Level.College;
			seasons = Math.Max(1, Math.Min(4, age - 18));
			break;
		case Sport.Hockey:
			level = ((age > 19 && !(_random.NextDouble() < 0.6)) ? Level.College : Level.Junior);
			seasons = ((level == Level.Junior) ? Math.Max(2, Math.Min(3, age - 16)) : Math.Max(1, Math.Min(4, age - 18)));
			break;
		case Sport.Baseball:
			level = ((age > 19) ? Level.College : Level.HighSchool);
			seasons = ((level == Level.HighSchool) ? 2 : Math.Max(2, Math.Min(4, age - 18)));
			break;
		default:
			level = ((age >= 20) ? Level.College : Level.HighSchool);
			seasons = ((level == Level.College) ? Math.Max(1, Math.Min(4, age - 18)) : 2);
			break;
		}
		string school = (p.From = ProspectSchool(p.Sport, level));
		bool spans = p.Sport == Sport.Hockey || p.Sport == Sport.Basketball;
		for (int k = seasons; k >= 1; k--)
		{
			int start = draftYear - k;
			double growth = 1.0 - 0.12 * (double)(k - 1);
			double role = ((k == seasons && seasons >= 3) ? 0.55 : 1.0);
			int ovr = Math.Max(30, (int)Math.Round((double)(p.Overall + 6) - 3.5 * (double)(k - 1) + Rosters.Normal(_random, 0.0, 1.5)));
			p.History.Add(new SeasonLine
			{
				Season = (spans ? (start + "-" + ((start + 1) % 100).ToString("00")) : start.ToString()),
				TeamId = -1,
				Team = school,
				Level = level,
				Ovr = ovr,
				Stats = ProspectSeason(p, level, growth * role)
			});
		}
	}

	private string ProspectSchool(Sport sport, Level level)
	{
		string place = _names.PickPlace(new List<string>());
		return level switch
		{
			Level.College => _names.CollegeName(place, new List<string>()), 
			Level.Junior => place + " " + _names.PickMascot(sport, Level.Junior, new List<string>()), 
			_ => place + " школа", 
		};
	}

	private int[] ProspectSeason(Player p, Level level, double role)
	{
		int[] s = Stats.Empty(p.Sport);
		double skill = Math.Max(0.5, Math.Min(1.8, 1.0 + (double)(p.Overall - Rosters.Base(Level.Pro) + 10) / 20.0)) * role;
		Func<double, int> n = (double x) => Math.Max(0, (int)Math.Round(x * (0.85 + 0.3 * _random.NextDouble())));
		switch (p.Sport)
		{
		case Sport.Football:
		{
			int g4 = (s[0] = ((level == Level.HighSchool) ? 10 : (12 + ((_random.NextDouble() < 0.5) ? 1 : 0))));
			switch (p.Position)
			{
			case Position.Quarterback:
				s[2] = n((double)(30 * g4) * Math.Min(1.0, role + 0.2));
				s[1] = n((double)s[2] * (0.55 + 0.05 * skill));
				s[3] = n((double)s[1] * 12.2 * Math.Sqrt(skill));
				s[4] = n((double)s[3] / 125.0);
				s[5] = n((double)s[2] / 45.0 / Math.Sqrt(skill));
				s[6] = n(5 * g4);
				s[7] = n(18 * g4);
				s[8] = n(0.25 * (double)g4);
				break;
			case Position.RunningBack:
				s[6] = n((double)(15 * g4) * skill);
				s[7] = n((double)s[6] * (4.4 + 0.5 * skill));
				s[8] = n((double)s[7] / 95.0);
				s[9] = n(1.8 * (double)g4);
				s[10] = n((double)s[9] * 8.5);
				break;
			case Position.WideReceiver:
				s[9] = n(4.2 * (double)g4 * skill);
				s[10] = n((double)s[9] * (12.5 + skill));
				s[11] = n((double)s[10] / 115.0);
				break;
			case Position.TightEnd:
				s[9] = n(2.4 * (double)g4 * skill);
				s[10] = n((double)s[9] * 11.5);
				s[11] = n((double)s[10] / 120.0);
				break;
			case Position.DefensiveLine:
				s[12] = n(3.2 * (double)g4 * skill);
				s[13] = n(0.45 * (double)g4 * skill);
				break;
			case Position.Linebacker:
				s[12] = n(6.5 * (double)g4 * skill);
				s[13] = n(0.25 * (double)g4 * skill);
				s[14] = n(0.06 * (double)g4);
				break;
			case Position.Cornerback:
				s[12] = n((double)(4 * g4) * skill);
				s[14] = n(0.18 * (double)g4 * skill);
				break;
			case Position.Safety:
				s[12] = n((double)(5 * g4) * skill);
				s[14] = n(0.14 * (double)g4 * skill);
				break;
			case Position.Kicker:
				s[16] = n(1.6 * (double)g4);
				s[15] = Math.Min(s[16], n((double)s[16] * (0.72 + 0.05 * skill)));
				s[18] = n(3.2 * (double)g4);
				s[17] = Math.Min(s[18], n((double)s[18] * 0.97));
				break;
			}
			break;
		}
		case Sport.Basketball:
		{
			int g2 = (s[0] = ((level == Level.HighSchool) ? 25 : 31));
			s[12] = ((role >= 0.9) ? g2 : n((double)g2 * 0.3));
			s[11] = n((double)(g2 * 29) * Math.Min(1.15, role));
			double ppg = 12.0 * skill;
			bool guard = p.Position == Position.PointGuard || p.Position == Position.ShootingGuard;
			bool big = p.Position == Position.BigMan || p.Position == Position.PowerForward;
			s[8] = n((double)g2 * (guard ? 1.7 : (big ? 0.3 : 1.1)) * skill);
			s[10] = n((double)g2 * 2.6 * skill);
			s[9] = Math.Min(s[10], n((double)s[10] * (big ? 0.66 : 0.76)));
			int twos = Math.Max(0, (int)Math.Round((ppg * (double)g2 - (double)(3 * s[8]) - (double)s[9]) / 2.0));
			s[6] = twos + s[8];
			s[7] = n((double)twos / (big ? 0.55 : 0.48) + (double)s[8] / 0.35);
			s[7] = Math.Max(s[7], s[6]);
			s[1] = 2 * twos + 3 * s[8] + s[9];
			s[2] = n((double)g2 * (big ? 7.5 : (guard ? 3.2 : 5.0)) * skill);
			s[3] = n((double)g2 * ((p.Position == Position.PointGuard) ? 5.2 : (guard ? 2.6 : (big ? 1.3 : 2.0))) * skill);
			s[4] = n((double)g2 * (guard ? 1.2 : 0.8));
			s[5] = n((double)g2 * (big ? 1.3 : 0.3));
			break;
		}
		case Sport.Hockey:
		{
			int g5 = level switch
			{
				Level.College => 35, 
				Level.Junior => 64, 
				_ => 25, 
			};
			if (p.Position == Position.Goalie)
			{
				int gs = (s[11] = (s[0] = n((double)g5 * 0.7 * Math.Min(1.1, role))));
				s[5] = n((double)gs * (0.45 + 0.08 * skill));
				s[6] = Math.Max(0, gs - s[5] - n((double)gs * 0.08));
				s[7] = Math.Max(0, gs - s[5] - s[6]);
				s[8] = n(gs * 29);
				s[9] = n((double)s[8] * (0.115 - 0.012 * skill));
				s[10] = n((double)gs * 0.05 * skill);
			}
			else
			{
				bool d = p.Position == Position.Defense;
				s[0] = n((double)g5 * 0.95);
				int gp2 = s[0];
				s[1] = n((double)gp2 * (d ? 0.1 : 0.35) * skill);
				s[2] = n((double)gp2 * (d ? 0.35 : 0.45) * skill);
				s[3] = Math.Max(s[1], n((double)gp2 * (d ? 1.6 : 2.5)));
				s[4] = n((double)gp2 * 0.7);
				s[12] = (int)Math.Round(Rosters.Normal(_random, 4.0 * (skill - 1.0), 6.0));
			}
			break;
		}
		case Sport.Baseball:
		{
			int g3 = ((level == Level.HighSchool) ? 25 : 52);
			if (p.Position == Position.StartingPitcher || p.Position == Position.ReliefPitcher)
			{
				bool sp = p.Position == Position.StartingPitcher;
				int apps = (s[9] = n((double)(sp ? 14 : 22) * Math.Min(1.1, role)));
				s[10] = (sp ? apps : 0);
				s[11] = n((double)apps * (sp ? 17.0 : 4.5));
				s[15] = n((double)s[11] / 3.0 * (0.9 + 0.2 * skill));
				s[14] = n((double)s[11] / 3.0 * 0.38);
				s[12] = n((double)s[11] / 3.0 * (1.05 - 0.1 * skill));
				s[13] = n((double)s[11] / 27.0 * (5.2 - 1.2 * skill));
				s[16] = (sp ? n((double)apps * 0.42 * skill) : n(2.0));
				s[17] = (sp ? n((double)apps * 0.3) : n(1.5));
				s[18] = ((!sp) ? n(4.0 * skill) : 0);
			}
			else
			{
				s[0] = n((double)g3 * 0.95);
				s[1] = n((double)s[0] * 3.7);
				s[3] = n((double)s[1] * (0.27 + 0.04 * skill));
				s[4] = n((double)s[3] * 0.2);
				s[5] = n((double)s[3] * 0.11 * skill);
				s[2] = n((double)s[3] * 0.6);
				s[6] = n((double)s[3] * 0.55 + (double)s[5]);
				s[7] = n((double)s[0] * 0.45);
				s[8] = n((double)s[1] * 0.2);
			}
			break;
		}
		default:
		{
			int g = ((level == Level.HighSchool) ? 16 : 19);
			s[0] = n((double)g * 0.95);
			s[8] = ((role >= 0.9) ? s[0] : n((double)s[0] * 0.4));
			int gp = s[0];
			switch (p.Position)
			{
			case Position.Goalkeeper:
				s[6] = n((double)gp * (1.4 - 0.25 * skill));
				s[5] = n(gp * 4);
				s[7] = n((double)gp * 0.25 * skill);
				break;
			case Position.Forward:
				s[1] = n((double)gp * 0.42 * skill);
				s[2] = n((double)gp * 0.18 * skill);
				s[3] = Math.Max(s[1], n((double)gp * 2.6));
				s[4] = n((double)gp * 0.1);
				break;
			case Position.Midfielder:
				s[1] = n((double)gp * 0.14 * skill);
				s[2] = n((double)gp * 0.24 * skill);
				s[3] = Math.Max(s[1], n((double)gp * 1.3));
				s[4] = n((double)gp * 0.15);
				break;
			default:
				s[1] = n((double)gp * 0.04 * skill);
				s[2] = n((double)gp * 0.07 * skill);
				s[3] = Math.Max(s[1], n((double)gp * 0.5));
				s[4] = n((double)gp * 0.18);
				break;
			}
			break;
		}
		}
		return s;
	}

	private static RecordBoard Board(League league, string group, string title, bool lowerIsBetter)
	{
		foreach (RecordBoard b in league.Records)
		{
			if (b.Group == group && b.Title == title)
			{
				return b;
			}
		}
		RecordBoard nb = new RecordBoard
		{
			Group = group,
			Title = title,
			LowerIsBetter = lowerIsBetter
		};
		league.Records.Add(nb);
		return nb;
	}

	private static bool Better(RecordBoard b, double a, double than)
	{
		if (!b.LowerIsBetter)
		{
			return a > than + 1E-09;
		}
		return a < than - 1E-09;
	}

	private static int Insert(RecordBoard b, RecordEntry e, int keep, bool onePerId)
	{
		if (onePerId)
		{
			b.Entries.RemoveAll((RecordEntry x) => x.Id == e.Id);
		}
		int i;
		for (i = 0; i < b.Entries.Count && !Better(b, e.Value, b.Entries[i].Value); i++)
		{
		}
		if (i >= keep)
		{
			return -1;
		}
		b.Entries.Insert(i, e);
		if (b.Entries.Count > keep)
		{
			b.Entries.RemoveRange(keep, b.Entries.Count - keep);
		}
		return i;
	}

	private static void Count(RecordBoard b, int id, string name, string team, string text)
	{
		double value = 1.0;
		foreach (RecordEntry x in b.Entries)
		{
			if (x.Id == id)
			{
				value = x.Value + 1.0;
				break;
			}
		}
		RecordEntry e = new RecordEntry
		{
			Id = id,
			Name = name,
			Team = team,
			Value = value,
			Text = (text ?? ((int)value).ToString())
		};
		if (text == null)
		{
			e.Text = ((int)value).ToString();
		}
		Insert(b, e, 30, onePerId: true);
	}

	private void UpdateRecords(League league, Season s, int now)
	{
		List<StandingRow> table = Standings.Compute(league, s);
		int teamGames = 0;
		foreach (StandingRow r in table)
		{
			teamGames = Math.Max(teamGames, r.Played);
		}
		if (teamGames == 0)
		{
			return;
		}
		string label = SeasonLabel(league, s);
		bool established = league.History.Count >= 3;
		List<Player> players = new List<Player>();
		foreach (Player p in PlayersInLeague(league))
		{
			if (!Stats.IsEmpty(p.Season))
			{
				players.Add(p);
			}
		}
		if (league.Level == Level.Pro)
		{
			foreach (Player p2 in FreeAgents(league.Sport))
			{
				if (p2.Level == Level.Pro && !Stats.IsEmpty(p2.Season))
				{
					players.Add(p2);
				}
			}
		}
		int posts = 0;
		Stats.Leader[] single = Stats.Leaders(league.Sport, teamGames);
		Stats.Leader[] career = Stats.Leaders(league.Sport, teamGames * 4);
		for (int i = 0; i < single.Length; i++)
		{
			Stats.Leader l = single[i];
			RecordBoard b = Board(league, "Single season", l.Title, l.LowerIsBetter);
			RecordEntry oldTop = ((b.Entries.Count > 0) ? b.Entries[0] : null);
			foreach (Player p3 in players)
			{
				int[] st = Stats.Fit(p3.Season, league.Sport);
				if (l.Qualifies(p3, st))
				{
					double v = l.Value(st);
					if (l.LowerIsBetter || !(v <= 0.0))
					{
						Insert(b, new RecordEntry
						{
							Id = p3.Id,
							Name = p3.Name,
							Team = PlayerTeamName(p3),
							Season = label,
							Value = v,
							Text = l.Format(v)
						}, 10, onePerId: false);
					}
				}
			}
			RecordEntry top = ((b.Entries.Count > 0) ? b.Entries[0] : null);
			if (established && oldTop != null && top != null && top != oldTop && top.Season == label && posts < 3)
			{
				posts++;
				bool user = IsUserTeamName(league, top.Team);
				string text = "НОВЫЙ РЕКОРД: " + top.Name + " (" + top.Team + ") установил новый " + LeagueLabel(league) + " рекорд одного сезона по показателю " + l.Title.ToLowerInvariant() + ": " + top.Text + ". Прежний рекорд составлял " + oldTop.Text + " и принадлежал " + oldTop.Name + " (" + oldTop.Season + ").";
				Post(league, NewsKind.General, Wire, text, user, now, 3);
				AddNote(league, top.Name + " установил рекорд одного сезона по показателю " + l.Title.ToLowerInvariant() + " (" + top.Text + ").");
			}
			Stats.Leader cl = career[i];
			RecordBoard cb = Board(league, "Career", cl.Title, cl.LowerIsBetter);
			RecordEntry oldLeader = ((cb.Entries.Count > 0) ? cb.Entries[0] : null);
			foreach (Player p4 in players)
			{
				int[] total = Stats.Fit(p4.Career, league.Sport);
				total = (int[])total.Clone();
				Stats.AddInto(total, Stats.Fit(p4.Season, league.Sport));
				if (!cl.Qualifies(p4, total))
				{
					cb.Entries.RemoveAll((RecordEntry x) => x.Id == p4.Id);
					continue;
				}
				double v2 = cl.Value(total);
				if (cl.LowerIsBetter || !(v2 <= 0.0))
				{
					int seasons = p4.Seasons + 1;
					Insert(cb, new RecordEntry
					{
						Id = p4.Id,
						Name = p4.Name,
						Team = PlayerTeamName(p4),
						Season = seasons + ((seasons == 1) ? " сезона" : " сезонов"),
						Value = v2,
						Text = cl.Format(v2)
					}, 10, onePerId: true);
				}
			}
			RecordEntry leader = ((cb.Entries.Count > 0) ? cb.Entries[0] : null);
			if (established && oldLeader != null && leader != null && leader.Id != oldLeader.Id && !cl.LowerIsBetter && posts < 3)
			{
				posts++;
				bool user2 = IsUserTeamName(league, leader.Team);
				Post(league, NewsKind.General, Wire, "РЕКОРД КАРЬЕРЫ: " + leader.Name + " (" + leader.Team + ") превзошёл результат " + oldLeader.Name + " и теперь является " + LeagueLabel(league) + " рекордсменом карьеры по показателю " + cl.Title.ToLowerInvariant() + " (" + leader.Text + ").", user2, now, 3);
				AddNote(league, leader.Name + " стал абсолютным рекордсменом по показателю " + cl.Title.ToLowerInvariant() + ".");
			}
		}
		foreach (AwardRecord a in league.Awards)
		{
			if (!(a.SeasonLabel != label) && a.PlayerId >= 0)
			{
				Count(Board(league, "Awards", a.Award, lowerIsBetter: false), a.PlayerId, a.PlayerName, a.TeamName, null);
				Count(Board(league, "Awards", "Все награды", lowerIsBetter: false), a.PlayerId, a.PlayerName, a.TeamName, null);
			}
		}
		if (s.ChampionTeamId >= 0)
		{
			Count(Board(league, "Teams", "Больше всего чемпионств", lowerIsBetter: false), s.ChampionTeamId, TeamName(s.ChampionTeamId), "", null);
		}
		RecordBoard best = Board(league, "Teams", "Лучший регулярный сезон", lowerIsBetter: false);
		foreach (StandingRow r2 in table)
		{
			if (r2.Played >= teamGames * 3 / 4)
			{
				double pct = ((double)r2.Wins + 0.5 * (double)r2.Ties + 0.5 * (double)r2.OvertimeLosses) / (double)r2.Played;
				Team t = GetTeam(r2.TeamId);
				Insert(best, new RecordEntry
				{
					Id = r2.TeamId,
					Name = TeamName(r2.TeamId),
					Team = "",
					Season = label,
					Value = pct,
					Text = ((t == null) ? "" : Standings.RecordText(t.Sport, r2))
				}, 10, onePerId: false);
			}
		}
		OrderBoards(league);
		Revision++;
	}

	private static void OrderBoards(League league)
	{
		Dictionary<string, int> group = new Dictionary<string, int>
		{
			{ "Single season", 0 },
			{ "Career", 1 },
			{ "Awards", 2 },
			{ "Teams", 3 }
		};
		List<RecordBoard> sorted = new List<RecordBoard>(league.Records);
		Dictionary<RecordBoard, int> original = new Dictionary<RecordBoard, int>();
		for (int i = 0; i < sorted.Count; i++)
		{
			original[sorted[i]] = i;
		}
		sorted.Sort((RecordBoard a, RecordBoard b) =>
		{
			if (!group.TryGetValue(a.Group, out var value))
			{
				value = 9;
			}
			if (!group.TryGetValue(b.Group, out var value2))
			{
				value2 = 9;
			}
			if (value != value2)
			{
				return value.CompareTo(value2);
			}
			return (a.Group == "Awards" && a.Title == "Все награды" != (b.Title == "Все награды")) ? ((!(a.Title == "Все награды")) ? 1 : (-1)) : original[a].CompareTo(original[b]);
		});
		league.Records.Clear();
		league.Records.AddRange(sorted);
	}

	private string PlayerTeamName(Player p)
	{
		Team t = ((p.TeamId >= 0) ? GetTeam(p.TeamId) : null);
		if (t != null)
		{
			return TeamName(t.Id);
		}
		return "Свободный агент";
	}

	private bool IsUserTeamName(League league, string name)
	{
		foreach (int id in league.TeamIds)
		{
			Team t = GetTeam(id);
			if (t != null && t.IsUser && TeamName(id) == name)
			{
				return true;
			}
		}
		return false;
	}

	public static string LeagueLabel(League league)
	{
		return SportRules.LevelLabel(league.Sport, league.Level).ToLowerInvariant() + " " + league.Sport.ToString().ToLowerInvariant();
	}

	private void AddNote(League league, string text)
	{
		if (league != null && !string.IsNullOrEmpty(text) && !league.SeasonNotes.Contains(text) && league.SeasonNotes.Count < 8)
		{
			league.SeasonNotes.Add(text);
		}
	}

	private void AddSeasonSummary(League league, Season s, PlayoffSeries final)
	{
		string label = SeasonLabel(league, s);
		SeasonSummary h = new SeasonSummary
		{
			Season = label
		};
		if (final != null && final.WinnerId >= 0 && !final.IsBye)
		{
			int champion = final.WinnerId;
			int runnerUp = ((champion == final.HighTeamId) ? final.LowTeamId : final.HighTeamId);
			Team ct = GetTeam(champion);
			h.Champion = TeamName(champion);
			h.RunnerUp = TeamName(runnerUp);
			h.FinalScore = FinalScoreText(league, s, final);
			h.ChampionIsUser = ct?.IsUser ?? false;
		}
		List<StandingRow> list = Standings.Compute(league, s);
		int teamGames = 0;
		foreach (StandingRow r in list)
		{
			teamGames = Math.Max(teamGames, r.Played);
			Team t = GetTeam(r.TeamId);
			if (h.Standings.Count < 32)
			{
				h.Standings.Add(r.Rank + ". " + TeamName(r.TeamId) + "  " + ((t == null) ? "" : Standings.RecordText(t.Sport, r)));
			}
		}
		for (int round = 0; round < s.PlayoffRounds; round++)
		{
			foreach (PlayoffSeries ps in SeriesInRound(s, round))
			{
				if (!ps.IsBye && ps.WinnerId >= 0)
				{
					int loser = ((ps.WinnerId == ps.HighTeamId) ? ps.LowTeamId : ps.HighTeamId);
					int winSeed = ((ps.WinnerId == ps.HighTeamId) ? ps.HighSeed : ps.LowSeed);
					int loseSeed = ((ps.WinnerId == ps.HighTeamId) ? ps.LowSeed : ps.HighSeed);
					h.Playoffs.Add(RoundName(league, s, round) + ": (" + winSeed + ") " + TeamName(ps.WinnerId) + " победила (" + loseSeed + ") " + TeamName(loser) + ", " + SeriesScore(league, s, ps));
				}
			}
		}
		foreach (AwardRecord a in league.Awards)
		{
			if (!(a.SeasonLabel != label))
			{
				h.Awards.Add(a.Award + ": " + a.PlayerName + " (" + a.TeamName + ")" + (string.IsNullOrEmpty(a.Detail) ? "" : (", " + a.Detail)));
			}
		}
		if (teamGames > 0)
		{
			List<Player> players = new List<Player>(PlayersInLeague(league));
			if (league.Level == Level.Pro)
			{
				players.AddRange(FreeAgents(league.Sport));
			}
			Stats.Leader[] array = Stats.Leaders(league.Sport, teamGames);
			foreach (Stats.Leader l in array)
			{
				Player best = null;
				double bestValue = 0.0;
				foreach (Player p in players)
				{
					if (Stats.IsEmpty(p.Season))
					{
						continue;
					}
					int[] st = Stats.Fit(p.Season, league.Sport);
					if (l.Qualifies(p, st))
					{
						double v = l.Value(st);
						if (best == null || (l.LowerIsBetter ? (v < bestValue) : (v > bestValue)))
						{
							best = p;
							bestValue = v;
						}
					}
				}
				if (best != null)
				{
					h.Leaders.Add(l.Title + ": " + best.Name + " (" + PlayerTeamName(best) + "), " + l.Format(bestValue));
				}
			}
		}
		h.Notes.AddRange(league.SeasonNotes);
		league.SeasonNotes.Clear();
		league.History.Insert(0, h);
		while (league.History.Count > 10)
		{
			league.History.RemoveAt(league.History.Count - 1);
		}
		Revision++;
	}

	private string SeriesScore(League league, Season s, PlayoffSeries ps)
	{
		if (ps.BestOf > 1)
		{
			SeriesWins(s, ps, out var high, out var low);
			return Math.Max(high, low) + "-" + Math.Min(high, low);
		}
		Match game = null;
		foreach (Match g in s.Games)
		{
			if (g.SeriesId == ps.Id && g.Played)
			{
				game = g;
			}
		}
		if (game == null)
		{
			return "";
		}
		string score = Math.Max(game.HomeScore, game.AwayScore) + "-" + Math.Min(game.HomeScore, game.AwayScore);
		if (game.Result == ResultType.Penalties)
		{
			score = score + " (" + Math.Max(game.HomePenalties, game.AwayPenalties) + "-" + Math.Min(game.HomePenalties, game.AwayPenalties) + " пен.)";
		}
		else if (game.Result != ResultType.Regulation)
		{
			score = score + " " + SportRules.ResultSuffix(league.Sport, game.Result);
		}
		return score;
	}

	private void CleanFeed(League league, Season s)
	{
		int cutoff = GameClock.ToTime(s.StartDay, 0);
		if (Feed.RemoveAll((NewsPost p) => (p.LeagueKey == league.Key || p.LeagueKey == "") && p.Time < cutoff) > 0)
		{
			Revision++;
		}
	}

	public void GivePriorities()
	{
		foreach (Player value in Players.Values)
		{
			value.Priority = NewPriority(_random);
		}
	}

	public static string PrestigeLabel(int prestige)
	{
		if (prestige >= 80)
		{
			return "Элитная программа";
		}
		if (prestige >= 65)
		{
			return "Сильная программа";
		}
		if (prestige >= 50)
		{
			return "Стабильная программа";
		}
		if (prestige >= 35)
		{
			return "Перестройка";
		}
		return "Глубокая перестройка";
	}

	private int NewPrestige(Team t)
	{
		return Rosters.Clamp((int)Math.Round((double)(t.IsUser ? 45 : 50) + (double)(t.Rating - 50) * (t.IsUser ? 0.5 : 0.8) + (t.IsUser ? 0.0 : Rosters.Normal(_random, 0.0, 12.0))), 10, 95);
	}

	private void UpdatePrestige(League league, Season s)
	{
		if (league.Level != Level.College || s == null)
		{
			return;
		}
		foreach (StandingRow r in Standings.Compute(league, s))
		{
			Team t = GetTeam(r.TeamId);
			if (t == null)
			{
				continue;
			}
			if (t.Prestige <= 0)
			{
				t.Prestige = NewPrestige(t);
			}
			double pct = ((r.Played == 0) ? 0.5 : r.WinPct);
			int won = 0;
			foreach (PlayoffSeries ps in s.Bracket)
			{
				if (ps.WinnerId == t.Id && !ps.IsBye)
				{
					won++;
				}
			}
			double score = pct * 70.0 + 30.0 * (double)won / (double)Math.Max(1, s.PlayoffRounds) + (double)((s.ChampionTeamId == t.Id) ? 10 : 0);
			t.Prestige = Rosters.Clamp((int)Math.Round((double)t.Prestige * 0.8 + score * 0.2), 5, 99);
		}
	}

	private int SigningYear(League league)
	{
		if (league.Current != null && !league.Current.Complete)
		{
			return league.Current.Year;
		}
		if (league.Previous != null)
		{
			return league.Previous.Year + 1;
		}
		return CurrentYear();
	}

	private static int CollegeProjection(Player p)
	{
		if (p.Level == Level.HighSchool)
		{
			return (int)Math.Round(53.0 + (double)(p.Overall - 55) * 0.7);
		}
		return p.Overall;
	}

	private static int StarsFor(Player p)
	{
		int proj = CollegeProjection(p);
		int pot = ((p.Level == Level.HighSchool) ? ((int)Math.Round(53.0 + (double)(p.Potential - 55) * 0.7)) : p.Potential);
		double score = (double)proj + 0.35 * (double)Math.Max(0, pot - proj);
		if (score >= 66.0)
		{
			return 5;
		}
		if (score >= 61.5)
		{
			return 4;
		}
		if (score >= 57.0)
		{
			return 3;
		}
		if (score >= 53.0)
		{
			return 2;
		}
		return 1;
	}

	public static int CommitBar(int stars)
	{
		return Math.Max(55, 40 + stars * 9);
	}

	public Dictionary<Position, int> RecruitNeeds(Team t)
	{
		Dictionary<Position, int> needs = new Dictionary<Position, int>();
		RosterSlot[] array = Rosters.Template(t.Sport, t.Level);
		foreach (RosterSlot slot in array)
		{
			needs[slot.Pos] = 0;
		}
		foreach (Player p in RosterOf(t.Id))
		{
			if (p.ClassYear >= 4)
			{
				needs.TryGetValue(p.Position, out var n);
				needs[p.Position] = n + 1;
			}
		}
		return needs;
	}

	private void EnsureBoard(League league, int year, int now)
	{
		if (RecruitBoard.Exists((RecruitTarget r) => r.Sport == league.Sport && r.Year == year))
		{
			return;
		}
		RecruitBoard.RemoveAll((RecruitTarget r) => r.Sport == league.Sport && r.Year < year);
		int open = 0;
		foreach (int id in league.TeamIds)
		{
			Team t = GetTeam(id);
			if (t == null)
			{
				continue;
			}
			if (t.Prestige <= 0)
			{
				t.Prestige = NewPrestige(t);
			}
			foreach (int n in RecruitNeeds(t).Values)
			{
				open += n;
			}
		}
		League hs = GetLeague(league.Sport, Level.HighSchool);
		if (hs != null)
		{
			foreach (int id2 in hs.TeamIds)
			{
				foreach (Player p in RosterOf(id2))
				{
					if (p.ClassYear >= 4 && CollegeProjection(p) >= 44)
					{
						RecruitBoard.Add(new RecruitTarget
						{
							PlayerId = p.Id,
							Sport = league.Sport,
							Year = year,
							Stars = StarsFor(p)
						});
					}
				}
			}
		}
		double factor = ((league.Sport == Sport.Football || league.Sport == Sport.Basketball) ? 2.0 : 1.2);
		int national = (int)Math.Round((double)Math.Max(8, open) * factor);
		List<Position> positions = new List<Position>();
		RosterSlot[] array = Rosters.Template(league.Sport, Level.College);
		foreach (RosterSlot slot in array)
		{
			for (int i = 0; i < slot.Count; i++)
			{
				positions.Add(slot.Pos);
			}
		}
		for (int k = 0; k < national; k++)
		{
			Position pos = positions[_random.Next(positions.Count)];
			int ovr = (int)Math.Round((double)(Rosters.Base(Level.College) - 9) + Rosters.Normal(_random, 0.0, 4.5));
			Player p2 = Rosters.CreatePlayer(league.Sport, Level.College, pos, ovr, -1, _random, () =>
			{
				SportsWorld sportsWorld = this;
				int nextId = NextId;
				sportsWorld.NextId = nextId + 1;
				return nextId;
			}, null, NameUse());
			p2.Status = PlayerStatus.RecruitPool;
			p2.ClassYear = 0;
			p2.Potential = p2.Overall + _random.Next(6, 20);
			p2.PrevOverall = p2.Overall;
			p2.BirthYear = CurrentYear() - 17;
			p2.From = HighSchoolFor(p2);
			AddPlayer(p2);
			RecruitBoard.Add(new RecruitTarget
			{
				PlayerId = p2.Id,
				Sport = league.Sport,
				Year = year,
				Stars = StarsFor(p2),
				National = true
			});
		}
		RostersChanged();
		Revision++;
	}

	private string HighSchoolFor(Player p)
	{
		string town = HometownOf(p);
		int comma = town.IndexOf(',');
		string text = ((comma > 0) ? town.Substring(0, comma) : town);
		string[] names = NameGenerator.HighSchoolNames;
		string school = names[_random.Next(names.Length)];
		if (!school.EndsWith(" школа", StringComparison.OrdinalIgnoreCase) &&
			!school.StartsWith("Лицей", StringComparison.OrdinalIgnoreCase) &&
			!school.StartsWith("Гимназия", StringComparison.OrdinalIgnoreCase))
		{
			school += " школа";
		}
		if (text.Length <= 0)
		{
			return school;
		}
		return school + " (" + town + ")";
	}

	public RecruitTarget FindTarget(int playerId)
	{
		foreach (RecruitTarget r in RecruitBoard)
		{
			if (r.PlayerId == playerId)
			{
				return r;
			}
		}
		return null;
	}

	public RecruitEffort FindEffort(int playerId, int teamId)
	{
		foreach (RecruitEffort e in RecruitEfforts)
		{
			if (e.PlayerId == playerId && e.TeamId == teamId)
			{
				return e;
			}
		}
		return null;
	}

	public static string WantsLabel(PlayerPriority p)
	{
		return p switch
		{
			PlayerPriority.Winning => "Хочет играть в команде-победителе", 
			PlayerPriority.PlayingTime => "Хочет быстро попасть в стартовый состав", 
			PlayerPriority.Hometown => "Хочет играть недалеко от дома", 
			PlayerPriority.Loyalty => "Хочет тренера, которому доверяет", 
			PlayerPriority.Money => "Хочет играть на большой арене", 
			_ => "Рассматривает любые варианты", 
		};
	}

	public int BaseRecruitInterest(Player p, Team t, int stars)
	{
		int prestige = ((t.Prestige > 0) ? t.Prestige : 50);
		double v = 35.0 + (double)(prestige - 50) * 0.35 + RecruitingEdge(t) * 3.0;
		switch (p.Priority)
		{
		case PlayerPriority.Winning:
			v += (double)(prestige - 50) * 0.4;
			break;
		case PlayerPriority.PlayingTime:
		{
			int starters = SlotStarters(t.Sport, p.Position, t.Level);
			List<Player> roster = RosterOf(t.Id).FindAll((Player q) => q.ClassYear < 4);
			v += (double)((CollegeProjection(p) > Weakest(roster, p.Position, Math.Max(1, starters)) - 3) ? 15 : (-10));
			break;
		}
		case PlayerPriority.Hometown:
			v += (double)((t.IsUser && IsCityPlayer(p)) ? 30 : (-5));
			break;
		case PlayerPriority.Money:
			v += (double)(prestige - 50) * 0.3;
			break;
		}
		if (t.IsUser && IsCityPlayer(p) && p.Priority != PlayerPriority.Hometown)
		{
			v += 6.0;
		}
		v -= (double)((stars - 3) * 6);
		return Rosters.Clamp((int)Math.Round(v), 5, 80);
	}

	public int RecruitInterestIn(Player p, Team t)
	{
		int stars = FindTarget(p.Id)?.Stars ?? StarsFor(p);
		int effort = FindEffort(p.Id, t.Id)?.Effort ?? 0;
		return Math.Min(100, BaseRecruitInterest(p, t, stars) + effort);
	}

	public int RecruitHoursPerMonth(Team t)
	{
		return 10 + (int)Math.Round(RecruitingEdge(t));
	}

	public int Scholarships(Team t)
	{
		int n = 0;
		foreach (int v in RecruitNeeds(t).Values)
		{
			n += v;
		}
		return n + 1;
	}

	public int OffersMade(Team t)
	{
		int n = 0;
		foreach (RecruitEffort e in RecruitEfforts)
		{
			if (e.TeamId == t.Id && e.Offered)
			{
				n++;
			}
		}
		return n;
	}

	private int OffersAt(Team t, Position pos)
	{
		int n = 0;
		foreach (RecruitEffort e in RecruitEfforts)
		{
			if (e.TeamId == t.Id && e.Offered)
			{
				Player p = GetPlayer(e.PlayerId);
				if (p != null && p.Position == pos)
				{
					n++;
				}
			}
		}
		return n;
	}

	public CommandResult Recruit(int teamId, int playerId, string action, int now)
	{
		_clockHint = now;
		Team t = GetTeam(teamId);
		if (t == null || t.Retired || !t.IsUser || t.Level != Level.College)
		{
			return CommandResult.Fail("Набор игроков доступен только для студенческих команд.");
		}
		League league = LeagueOf(t);
		Player p = GetPlayer(playerId);
		RecruitTarget r = FindTarget(playerId);
		if (league == null || p == null || r == null || r.Sport != t.Sport)
		{
			return CommandResult.Fail("Этого новичка больше нет в списке доступных игроков.");
		}
		if (r.CommittedTo == t.Id)
		{
			return CommandResult.Fail(p.Name + " уже согласился перейти к вам.");
		}
		if (r.CommittedTo == -2)
		{
			return CommandResult.Fail(p.Name + " подписал контракт со школой вне лиги.");
		}
		int month = GameClock.MonthIndex(now);
		if (league.TradeMonth != month)
		{
			RecruitingMonthly(league, now);
		}
		RecruitEffort e = FindEffort(playerId, teamId);
		if (e == null)
		{
			e = new RecruitEffort
			{
				PlayerId = playerId,
				TeamId = teamId
			};
			RecruitEfforts.Add(e);
		}
		if (e.Month != month)
		{
			e.Calls = 0;
			e.Watched = false;
		}
		int hours;
		int gain;
		switch (action)
		{
		case "call":
			if (e.Calls >= 2)
			{
				return CommandResult.Fail("Вы уже звонили " + p.Name + " дважды в этом месяце.");
			}
			hours = 1;
			gain = 3;
			e.Calls++;
			break;
		case "watch":
			if (e.Watched)
			{
				return CommandResult.Fail("Ваши тренеры уже смотрели игру игрока " + p.Name + " в этом месяце.");
			}
			hours = 3;
			gain = 8;
			e.Watched = true;
			break;
		case "home":
			if (e.HomeVisit)
			{
				return CommandResult.Fail("Вы уже посещали дом игрока " + p.Name + ".");
			}
			hours = 4;
			gain = 12;
			e.HomeVisit = true;
			break;
		case "official":
			if (!e.Offered)
			{
				return CommandResult.Fail("Предложите " + p.Name + " стипендию до официального визита.");
			}
			if (e.OfficialVisit)
			{
				return CommandResult.Fail(p.Name + " уже посетил вашу школу с официальным визитом.");
			}
			hours = 5;
			gain = 18;
			e.OfficialVisit = true;
			break;
		case "offer":
		{
			if (e.Offered)
			{
				return CommandResult.Fail("Вы уже предлагали стипендию игроку " + p.Name + ".");
			}
			if (OffersMade(t) >= Scholarships(t))
			{
				return CommandResult.Fail("Стипендии на этот набор закончились (" + Scholarships(t) + ") на этот год. Сначала отзовите одно из предложений.");
			}
			RecruitNeeds(t).TryGetValue(p.Position, out var need);
			if (OffersAt(t, p.Position) >= need + 1)
			{
				return CommandResult.Fail("У вас нет места в " + Rosters.NewsPositionLabel(p.Position, p.Sport) + " на следующий сезон (" + need + " свободных мест).");
			}
			e.Offered = true;
			hours = 0;
			gain = 5;
			break;
		}
		case "pull":
			if (!e.Offered)
			{
				return CommandResult.Fail("Вы ещё не предлагали стипендию игроку " + p.Name + ".");
			}
			e.Offered = false;
			e.Effort = Math.Max(0, e.Effort - 15);
			Revision++;
			return CommandResult.Success("Вы отозвали предложение игроку " + p.Name + ".");
		default:
			return CommandResult.Fail("Неизвестное действие при наборе игроков.");
		}
		if (t.RecruitHours < hours)
		{
			return CommandResult.Fail("В этом месяце осталось недостаточно часов на набор (" + t.RecruitHours + " из " + RecruitHoursPerMonth(t) + "). Часы восстановятся в следующем месяце.");
		}
		t.RecruitHours -= hours;
		if (p.Priority == PlayerPriority.Loyalty)
		{
			gain = (int)Math.Round((double)gain * 1.3);
		}
		e.Effort = Math.Min(80, e.Effort + gain);
		e.Month = month;
		Revision++;
		int interest = RecruitInterestIn(p, t);
		string what = action switch
		{
			"official" => p.Name + " посетил вашу школу с официальным визитом.", 
			"home" => "Вы посетили дом игрока " + p.Name + " и встретились с его семьёй.", 
			"watch" => "Ваши тренеры посмотрели игру игрока " + p.Name + ".", 
			"call" => "Вы позвонили игроку " + p.Name + ".", 
			_ => "Вы предложили игроку " + p.Name + " стипендию.", 
		};
		if (TryCommit(league, r, p, now))
		{
			return CommandResult.Success(what + " " + ((r.CommittedTo == t.Id) ? "Он согласился перейти к вам!" : ""));
		}
		int bar = CommitBar(r.Stars) + ((r.CommittedTo >= 0) ? 10 : 0);
		return CommandResult.Success(what + " Интерес к вашей школе: " + interest + " (он согласится при уровне интереса " + bar + (e.Offered ? "" : ", если вы сделаете предложение") + ").");
	}

	private bool TryCommit(League league, RecruitTarget r, Player p, int now)
	{
		if (r.CommittedTo == -2)
		{
			return false;
		}
		Team best = null;
		int bestInterest = 0;
		foreach (RecruitEffort e in RecruitEfforts)
		{
			if (e.PlayerId != p.Id || !e.Offered || e.TeamId == r.CommittedTo)
			{
				continue;
			}
			Team t = GetTeam(e.TeamId);
			if (t != null && !t.Retired && t.IsUser)
			{
				int interest = RecruitInterestIn(p, t);
				int bar = CommitBar(r.Stars) + ((r.CommittedTo >= 0) ? 10 : 0);
				if (interest >= bar && interest > bestInterest)
				{
					best = t;
					bestInterest = interest;
				}
			}
		}
		if (best == null)
		{
			return false;
		}
		bool flip = r.CommittedTo >= 0;
		string from = (flip ? TeamShortName(r.CommittedTo) : "");
		r.CommittedTo = best.Id;
		string stars = r.Stars + "-звёздный";
		Organization o = GetOrg(best.OrgId);
		AddPost(league, NewsKind.Recruiting, Campus.Name, Campus.Handle, (flip ? "СМЕНА РЕШЕНИЯ: " : "ВЫБОР: ") + stars + " " + Rosters.NewsPositionLabel(p.Position, p.Sport) + " " + p.Name + ((p.From.Length > 0 && !r.National) ? (" из " + p.From) : (r.National ? (" (" + HometownOf(p) + ")") : "")) + (flip ? (" меняет решение: вместо " + from + " выбирает ") : " выбирает ") + ((o == null) ? TeamName(best.Id) : o.Name) + "!", isUser: true, now, (r.Stars >= 4) ? 3 : 2);
		Revision++;
		return true;
	}

	private void RecruitingMonthly(League league, int now)
	{
		if (league.Level != Level.College || CountUserTeams(league) == 0)
		{
			return;
		}
		int month = GameClock.MonthIndex(now);
		if (league.TradeMonth == month)
		{
			return;
		}
		league.TradeMonth = month;
		int year = SigningYear(league);
		EnsureBoard(league, year, now);
		List<Team> aiSchools = new List<Team>();
		foreach (int id in league.TeamIds)
		{
			Team t = GetTeam(id);
			if (t != null && !t.Retired)
			{
				if (t.Prestige <= 0)
				{
					t.Prestige = NewPrestige(t);
				}
				if (t.IsUser)
				{
					t.RecruitHours = RecruitHoursPerMonth(t);
				}
				else
				{
					aiSchools.Add(t);
				}
			}
		}
		foreach (RecruitEffort e in RecruitEfforts)
		{
			if (e.Month >= 0 && e.Month < month - 1)
			{
				e.Effort = (int)Math.Round((double)e.Effort * 0.9);
				e.Month = month - 1;
			}
		}
		foreach (RecruitTarget r in RecruitBoard)
		{
			if (r.Sport != league.Sport || r.Year != year || r.CommittedTo == -2)
			{
				continue;
			}
			Player p = GetPlayer(r.PlayerId);
			if (p == null || (r.CommittedTo >= 0 && GetTeam(r.CommittedTo) != null && GetTeam(r.CommittedTo).IsUser) || TryCommit(league, r, p, now) || r.CommittedTo >= 0)
			{
				continue;
			}
			double chance = 0.03 + 0.03 * (double)r.Stars;
			foreach (RecruitEffort e2 in RecruitEfforts)
			{
				if (e2.PlayerId == r.PlayerId && e2.Offered)
				{
					Team mine = GetTeam(e2.TeamId);
					if (mine != null && mine.IsUser && (double)RecruitInterestIn(p, mine) >= (double)CommitBar(r.Stars) * 0.6)
					{
						chance *= 0.3;
						break;
					}
				}
			}
			if (_random.NextDouble() >= chance)
			{
				continue;
			}
			if (r.National && _random.NextDouble() < 0.5)
			{
				r.CommittedTo = -2;
			}
			else
			{
				if (aiSchools.Count == 0)
				{
					continue;
				}
				double total = 0.0;
				foreach (Team t2 in aiSchools)
				{
					total += Math.Pow(Math.Max(5, t2.Prestige), 2.0);
				}
				double pick = _random.NextDouble() * total;
				Team school = aiSchools[aiSchools.Count - 1];
				foreach (Team t3 in aiSchools)
				{
					pick -= Math.Pow(Math.Max(5, t3.Prestige), 2.0);
					if (pick <= 0.0)
					{
						school = t3;
						break;
					}
				}
				r.CommittedTo = school.Id;
				if (r.Stars >= 5 || IsCityPlayer(p))
				{
					Organization o = GetOrg(school.OrgId);
					AddPost(league, NewsKind.Recruiting, Campus.Name, Campus.Handle, "СОГЛАСИЕ: " + r.Stars + "звёздный игрок " + Rosters.NewsPositionLabel(p.Position, p.Sport) + " " + p.Name + " согласился перейти в " + ((o == null) ? TeamName(school.Id) : o.Name) + ".", IsCityPlayer(p), now, (r.Stars >= 5) ? 3 : 2);
				}
			}
		}
		Revision++;
	}

	private bool IsCommitted(int playerId)
	{
		RecruitTarget r = FindTarget(playerId);
		if (r != null)
		{
			return r.CommittedTo >= 0;
		}
		return false;
	}

	private Dictionary<int, List<Player>> SigningDay(League league, int year, List<Player> pool)
	{
		Dictionary<int, List<Player>> committed = new Dictionary<int, List<Player>>();
		foreach (RecruitTarget r in RecruitBoard)
		{
			if (r.Sport != league.Sport || r.Year != year)
			{
				continue;
			}
			Player p = GetPlayer(r.PlayerId);
			if (p == null || p.Status != PlayerStatus.RecruitPool || p.Level != Level.College)
			{
				continue;
			}
			if (r.CommittedTo == -2)
			{
				pool.Remove(p);
				RemovePlayer(p);
			}
			else if (r.CommittedTo >= 0 && GetTeam(r.CommittedTo) != null && league.TeamIds.Contains(r.CommittedTo))
			{
				pool.Remove(p);
				if (!committed.TryGetValue(r.CommittedTo, out var list))
				{
					list = new List<Player>();
					committed[r.CommittedTo] = list;
				}
				list.Add(p);
			}
		}
		RecruitBoard.RemoveAll((RecruitTarget recruitTarget) => recruitTarget.Sport == league.Sport && recruitTarget.Year <= year);
		RecruitEfforts.RemoveAll((RecruitEffort e) =>
		{
			Team team = GetTeam(e.TeamId);
			return team == null || (team.Sport == league.Sport && FindTarget(e.PlayerId) == null);
		});
		return committed;
	}

	private static Player TakeCommitted(Dictionary<int, List<Player>> committed, Team team, Position pos)
	{
		if (committed == null || !committed.TryGetValue(team.Id, out var list))
		{
			return null;
		}
		Player best = null;
		foreach (Player p in list)
		{
			if (p.Position == pos && (best == null || p.Overall > best.Overall))
			{
				best = p;
			}
		}
		if (best == null)
		{
			return null;
		}
		list.Remove(best);
		best.Career = Stats.Empty(best.Sport);
		best.LastSeason = new int[0];
		best.Season = Stats.Empty(best.Sport);
		best.Seasons = 0;
		return best;
	}

	public ReleaseQuote QuoteRelease(Player p, Team t, int now, bool stretch)
	{
		ReleaseQuote q = new ReleaseQuote();
		int years = Math.Max(0, p.ContractYears);
		int salary = Math.Max(0, p.Salary);
		Sport sport = t.Sport;
		bool minimum = Economy.IsMinimum(sport, salary);
		DateTime date = GameClock.DateOf(GameClock.DayOf(now));
		if (!SportRules.ProLike(t.Sport, t.Level) || years <= 0 || salary <= 0)
		{
			q.Rule = "Срок его контракта истёк, поэтому отпуск игрока ничего не стоит.";
			return q;
		}
		switch (sport)
		{
		case Sport.Hockey:
		{
			bool young = AgeOf(p) < 26;
			int total = (int)Math.Round((young ? (1.0 / 3.0) : (2.0 / 3.0)) * (double)salary * (double)years);
			Spread(q, total, years * 2);
			q.Label = "Выкупить контракт";
			q.Verb = "расторгли контракт с";
			q.Reason = "Контракт выкуплен";
			q.Rule = "Выкуп хоккейного контракта: выплачивается " + (young ? "1/3 (если ему меньше 26 лет)" : "2/3 (если ему 26 лет или больше)") + " от суммы " + Economy.Money(salary * years) + " оставшейся по контракту, выплаты распределяются на удвоенный оставшийся срок (" + years * 2 + " сезонов).";
			break;
		}
		case Sport.Basketball:
			q.Label = "Отчислить";
			q.Verb = "отчислили";
			q.Reason = "Отчисление";
			if (minimum)
			{
				if (date.Month < 1 || date.Month >= 7 || (date.Month == 1 && date.Day < 10))
				{
					q.Rule = "Минимальные контракты не гарантированы до 10 января, поэтому сейчас игрока можно отпустить бесплатно.";
					break;
				}
				q.Amounts.Add(salary);
				q.Rule = "Его минимальный контракт стал гарантированным 10 января: зарплата за этот сезон останется в платёжной ведомости, зарплаты за следующие сезоны — нет.";
				break;
			}
			q.CanStretch = true;
			if (stretch)
			{
				int seasons = years * 2 + 1;
				Spread(q, salary * years, seasons);
				q.Label = "Расторгнуть контракт с распределением выплат";
				q.Reason = "Контракт расторгнут, выплаты распределены";
				q.Rule = "Правило распределения выплат: сумма " + Economy.Money(salary * years) + " оставшихся выплат распределяется на срок, вдвое превышающий оставшиеся годы контракта, плюс один (" + seasons + " сезонов). Платежи меньше, но растянуты на более долгий срок.";
			}
			else
			{
				for (int k = 0; k < years; k++)
				{
					q.Amounts.Add(salary);
				}
				q.Rule = "Контракты в баскетболе гарантированы: полная зарплата игрока учитывается в платёжной ведомости каждый сезон действия договора. Вместо этого выплаты можно растянуть.";
			}
			break;
		case Sport.Football:
		{
			q.Label = "Уволить";
			q.Verb = "уволили";
			q.Reason = "Увольнение";
			if (minimum)
			{
				q.Rule = "Зарплаты в американском футболе не гарантированы, а минимальные контракты не предусматривают бонуса при подписании, поэтому увольнение такого игрока ничего не стоит.";
				break;
			}
			int bonus = (int)Math.Round((double)salary * 0.3);
			int left2 = bonus * years;
			DateTime leagueYear = Economy.Get(sport).LeagueYearDate(date.Year - 1);
			if ((date >= leagueYear && date < new DateTime(date.Year, 6, 1)) || years == 1)
			{
				q.Amounts.Add(left2);
				q.Rule = "Зарплаты в американском футболе не гарантированы, но " + Economy.Money(left2) + " неучтённой части бонуса за подписание превращается в мёртвые деньги" + ((years == 1) ? "." : ", вся сумма учитывается в этом сезоне, поскольку дата раньше 1 июня. После 1 июня она распределялась бы между этим и следующим сезонами.");
			}
			else
			{
				q.Amounts.Add(bonus);
				q.Amounts.Add(left2 - bonus);
				q.Rule = "Зарплаты в американском футболе не гарантированы, но " + Economy.Money(left2) + " неучтённой части бонуса за подписание превращается в мёртвые деньги. После 1 июня в текущем сезоне учитывается только его часть, а остаток переносится на следующий сезон.";
			}
			break;
		}
		case Sport.Baseball:
			if (minimum)
			{
				Season s = LeagueOf(t)?.Current;
				int today = GameClock.DayOf(now);
				if (s == null || s.Complete || today < s.StartDay)
				{
					q.Amounts.Add(Math.Max(1, (int)Math.Round((double)salary / 6.0)));
					q.Rule = "Игроки с минимальными контрактами, отпущенные до первого матча сезона, получают компенсацию только за 30 дней (примерно за одну шестую сезона).";
				}
				else
				{
					q.Amounts.Add(salary);
					q.Rule = "После первого матча зарплата за текущий сезон выплачивается полностью. Следующие сезоны минимального контракта не гарантированы.";
				}
			}
			else
			{
				for (int j = 0; j < years; j++)
				{
					q.Amounts.Add(salary);
				}
				q.Rule = "Контракты в бейсболе гарантированы: оставшаяся сумма договора всё равно выплачивается и учитывается в расходах клуба.";
			}
			break;
		case Sport.Soccer:
		{
			q.Label = "Выкупить контракт";
			q.Verb = "расторгли контракт с";
			q.Reason = "Отчисление";
			int left = 2 - t.Buyouts;
			if (left > 0)
			{
				q.UsesBuyout = true;
				q.Rule = "Используется один из ваших " + 2 + " выкупов контрактов в этом году (" + left + " осталось): клуб выплачивает игроку компенсацию, которая не учитывается в зарплатном бюджете.";
			}
			else
			{
				q.Label = "Отчислить";
				q.Verb = "отчислили";
				int charge = Math.Min(salary, Economy.MaxBudgetCharge(sport));
				for (int i = 0; i < years; i++)
				{
					q.Amounts.Add(charge);
				}
				q.Rule = "Вы уже использовали оба выкупа контрактов в этом году, поэтому выплаты за игрока будут учитываться в зарплатном бюджете до окончания его контракта. Количество выкупов обновится с началом нового сезона лиги.";
			}
			break;
		}
		}
		foreach (int a in q.Amounts)
		{
			q.Total += a;
		}
		int owedCount = ((sport == Sport.Soccer) ? (Math.Min(salary, Economy.MaxBudgetCharge(sport)) * years) : (salary * years));
		q.Saved = Math.Max(0, owedCount - q.Total);
		q.Short = DeadText(q.Amounts);
		return q;
	}

	private static void Spread(ReleaseQuote q, int total, int seasons)
	{
		if (total > 0 && seasons > 0)
		{
			int each = total / seasons;
			int rest = total - each * seasons;
			for (int i = 0; i < seasons; i++)
			{
				q.Amounts.Add(each + ((i < rest) ? 1 : 0));
			}
		}
	}

	public static string DeadText(List<int> amounts)
	{
		int total = 0;
		foreach (int a in amounts)
		{
			total += a;
		}
		if (total <= 0)
		{
			return "Нет мёртвых денег.";
		}
		string text = "Мёртвые деньги: " + Economy.Money(amounts[0]) + " в этом сезоне";
		if (amounts.Count == 1)
		{
			return text + ".";
		}
		bool even = true;
		for (int i = 2; i < amounts.Count; i++)
		{
			if (Math.Abs(amounts[i] - amounts[1]) > 1)
			{
				even = false;
			}
		}
		if (even)
		{
			return text + ", затем " + Economy.Money(amounts[1]) + " за сезон в течение " + (amounts.Count - 1) + ((amounts.Count == 2) ? " дополнительного сезона" : " дополнительных сезонов") + " (" + Economy.Money(total) + " всего).";
		}
		List<string> later = new List<string>();
		for (int j = 1; j < amounts.Count; j++)
		{
			later.Add(Economy.Money(amounts[j]));
		}
		return text + ", затем " + string.Join(", ", later.ToArray()) + " (" + Economy.Money(total) + " всего).";
	}

	private void ApplyRelease(Player p, Team t, ReleaseQuote q, int now)
	{
		if (q.UsesBuyout)
		{
			t.Buyouts++;
		}
		if (q.Total > 0)
		{
			DeadCharge c = new DeadCharge
			{
				PlayerId = p.Id,
				Name = p.Name,
				Reason = q.Reason
			};
			c.Amounts.AddRange(q.Amounts);
			t.Dead.Add(c);
		}
		RemoveFromBlock(t, p.Id);
		BecomeFreeAgent(p, GameClock.MonthIndex(now));
	}

	private static void RemoveFromBlock(Team t, int playerId)
	{
		if (t.Block != null)
		{
			t.Block.Remove(playerId);
		}
	}

	private void NewDeadYear(Team t)
	{
		t.Buyouts = 0;
		for (int i = t.Dead.Count - 1; i >= 0; i--)
		{
			DeadCharge c = t.Dead[i];
			if (c.Amounts.Count > 0)
			{
				c.Amounts.RemoveAt(0);
			}
			if (c.Amounts.Count == 0)
			{
				t.Dead.RemoveAt(i);
			}
		}
	}

	public string NoRoomFor(Team t, Player p)
	{
		if (!SportRules.ProLike(t.Sport, t.Level))
		{
			return null;
		}
		List<Player> signed = SignedPlayers(t.Id);
		if (!signed.Contains(p))
		{
			signed.Add(p);
		}
		int reserves = 0;
		int size = 0;
		RosterSlot[] array = Rosters.Template(t.Sport, Level.Pro);
		foreach (RosterSlot slot in array)
		{
			size += slot.Count;
			int n = 0;
			foreach (Player item in signed)
			{
				if (item.Position == slot.Pos)
				{
					n++;
				}
			}
			reserves += Math.Max(0, n - slot.Count);
		}
		int max = Economy.Get(t.Sport).MaxProspects;
		if (reserves <= max)
		{
			return null;
		}
		return "Нет места для " + p.Name + ": основной состав (" + size + " игроков) и резерв (" + max + ") заполнены. Сначала отпустите или обменяйте кого-нибудь, чтобы самостоятельно решить, кто покинет команду.";
	}

	private int CheapestCut(Team t, int now)
	{
		int best = int.MaxValue;
		foreach (Player q in SignedPlayers(t.Id))
		{
			if (q.Status == PlayerStatus.Prospect)
			{
				best = Math.Min(best, QuoteRelease(q, t, now, stretch: false).Total);
				if (best == 0)
				{
					break;
				}
			}
		}
		if (best != int.MaxValue)
		{
			return best;
		}
		return 0;
	}

	public static string ReleaseRules(Sport sport)
	{
		return sport switch
		{
			Sport.Hockey => "Расторжение контракта — это выкуп: вы выплачиваете 2/3 оставшейся суммы (1/3, если игроку меньше 26 лет), распределённой на срок вдвое больше оставшегося срока договора. Любой выкуп стоит денег, но выкуп молодых игроков обходится дешевле. Если контракт просто истекает, штрафа нет.", 
			Sport.Basketball => "Контракты гарантированы: зарплата отпущенного игрока остаётся в платёжной ведомости за каждый оставшийся сезон. Можно применить распределение выплат на срок вдвое больше оставшегося срока плюс один. Бесплатно: минимальные контракты, расторгнутые до 10 января, и истёкшие контракты.", 
			Sport.Football => "Зарплаты не гарантированы: увольнение игрока экономит его зарплату, но неучтённый бонус за подписание (около 30% годовой зарплаты за каждый оставшийся год) становится мёртвыми деньгами. До 1 июня вся сумма учитывается сразу, после 1 июня делится между текущим и следующим сезонами. Бесплатно: минимальные контракты.", 
			Sport.Baseball => "Контракты гарантированы: отпущенному игроку выплачивается вся оставшаяся сумма, которая учитывается в фонде зарплат. Дёшево: минимальные контракты, расторгнутые до первого матча (компенсация за 30 дней). После первого матча минимальный контракт обойдётся в полную зарплату за этот сезон.", 
			_ => "Каждый клуб получает два выкупа контрактов в год, которые вообще не учитываются в зарплатном бюджете. После этого выплаты за отпущенного игрока продолжают учитываться до окончания его контракта. Количество выкупов обновляется с каждым новым сезоном лиги.", 
		};
	}

	public Rivalry FindRivalry(League league, int a, int b, bool create)
	{
		if (league == null || a < 0 || b < 0 || a == b)
		{
			return null;
		}
		int lo = Math.Min(a, b);
		int hi = Math.Max(a, b);
		foreach (Rivalry r in league.Rivalries)
		{
			if (r.TeamA == lo && r.TeamB == hi)
			{
				return r;
			}
		}
		if (!create)
		{
			return null;
		}
		Rivalry n = new Rivalry
		{
			TeamA = lo,
			TeamB = hi
		};
		league.Rivalries.Add(n);
		return n;
	}

	public static string RivalryLabel(Rivalry r)
	{
		if (r.Heat >= 80)
		{
			if (!r.Crosstown)
			{
				return "Принципиальные соперники";
			}
			return "Принципиальные соперники из одного города";
		}
		if (r.Heat >= 50)
		{
			if (!r.Crosstown)
			{
				return "Соперники";
			}
			return "Городские соперники";
		}
		if (r.Heat >= 30)
		{
			if (!r.Crosstown)
			{
				return "Соперничество накаляется";
			}
			return "Соседи по городу";
		}
		return "";
	}

	private static int CloseMargin(Sport sport)
	{
		return sport switch
		{
			Sport.Basketball => 5, 
			Sport.Football => 7, 
			_ => 1, 
		};
	}

	private void RivalryEdge(League league, Match g, Team home, Team away, ref double homeRating, ref double awayRating)
	{
		if (home == null || away == null)
		{
			return;
		}
		Rivalry r = FindRivalry(league, home.Id, away.Id, create: false);
		if (r != null && r.Heat >= 50)
		{
			double edge = ((r.Heat >= 80) ? 1.0 : 0.6);
			if (homeRating < awayRating)
			{
				homeRating += edge;
			}
			else if (awayRating < homeRating)
			{
				awayRating += edge;
			}
		}
	}

	private void UpdateRivalry(League league, Season s, Match g, int now)
	{
		Team home = GetTeam(g.HomeTeamId);
		Team away = GetTeam(g.AwayTeamId);
		if (home != null && away != null)
		{
			Rivalry r = FindRivalry(league, home.Id, away.Id, create: true);
			int before = r.Heat;
			_ = r.TeamA;
			_ = home.Id;
			if (g.WinnerId < 0)
			{
				r.Ties++;
			}
			else if (g.WinnerId == r.TeamA)
			{
				r.WinsA++;
			}
			else
			{
				r.WinsB++;
			}
			string score = TeamAbbrev(g.AwayTeamId) + " " + g.AwayScore + ", " + TeamAbbrev(g.HomeTeamId) + " " + g.HomeScore;
			if (g.Result != ResultType.Regulation)
			{
				score = score + " (" + SportRules.ResultSuffix(league.Sport, g.Result) + ")";
			}
			r.LastMeeting = GameClock.ShortDayLabel(GameClock.DayOf(g.Time)) + ", " + GameClock.YearOfDay(GameClock.DayOf(g.Time)) + ": " + score;
			if (home.IsUser && away.IsUser && !r.Crosstown)
			{
				r.Crosstown = true;
				r.Heat = Math.Max(r.Heat, 50);
			}
			int num = Math.Abs(g.HomeScore - g.AwayScore);
			double gain = 0.0;
			if (num <= CloseMargin(league.Sport))
			{
				gain++;
			}
			if (g.Result != ResultType.Regulation)
			{
				gain++;
			}
			if (g.Stage == GameStage.Regular)
			{
				int teams = Math.Max(2, league.TeamIds.Count);
				double meetings = Math.Max(1.0, (double)TargetGames(league) / (double)(teams - 1));
				gain *= Math.Min(3.0, 4.0 / meetings);
			}
			else
			{
				gain++;
			}
			int whole = (int)gain;
			if (_random.NextDouble() < gain - (double)whole)
			{
				whole++;
			}
			r.Heat = Math.Min(100, r.Heat + whole);
			CheckRivalryBorn(league, r, before, now);
		}
	}

	private void RivalrySeries(League league, Season s, PlayoffSeries ps, int now)
	{
		if (!ps.IsBye && ps.WinnerId >= 0)
		{
			Rivalry r = FindRivalry(league, ps.HighTeamId, ps.LowTeamId, create: true);
			int before = r.Heat;
			if (ps.WinnerId == r.TeamA)
			{
				r.SeriesA++;
			}
			else
			{
				r.SeriesB++;
			}
			int heat = 15 + ps.Round * 5;
			if (ps.Round == s.PlayoffRounds - 1)
			{
				heat += 10;
			}
			SeriesWins(s, ps, out var high, out var low);
			if (ps.BestOf > 1 && high + low == ps.BestOf)
			{
				heat += 8;
			}
			r.Heat = Math.Min(100, r.Heat + heat);
			CheckRivalryBorn(league, r, before, now);
		}
	}

	private void CheckRivalryBorn(League league, Rivalry r, int before, int now)
	{
		if (before < 50 && r.Heat >= 50)
		{
			Team a = GetTeam(r.TeamA);
			Team b = GetTeam(r.TeamB);
			bool user = (a != null && a.IsUser) || (b?.IsUser ?? false);
			string record = r.WinsA + "-" + r.WinsB + ((r.Ties > 0) ? ("-" + r.Ties) : "");
			Post(league, NewsKind.Rivalry, Wire, "СОПЕРНИЧЕСТВО: " + TeamName(r.TeamA) + " и " + TeamName(r.TeamB) + " не ладят друг с другом. За всю историю: " + TeamAbbrev(r.TeamA) + " ведёт " + record + ((r.SeriesA + r.SeriesB > 0) ? (", в плей-офф " + r.SeriesA + "-" + r.SeriesB) : "") + ".", user, now, 2);
			AddNote(league, "Зарождается соперничество: " + TeamName(r.TeamA) + " против " + TeamName(r.TeamB) + ".");
		}
	}

	private void CoolRivalries(League league)
	{
		HashSet<int> teams = new HashSet<int>(league.TeamIds);
		foreach (Rivalry r in league.Rivalries)
		{
			r.Heat = (int)Math.Round((double)r.Heat * 0.85);
			if (r.Crosstown)
			{
				r.Heat = Math.Max(50, r.Heat);
			}
		}
		league.Rivalries.RemoveAll((Rivalry rivalry) => (!teams.Contains(rivalry.TeamA) || !teams.Contains(rivalry.TeamB)) && rivalry.Heat < 30);
		if (league.Rivalries.Count > 160)
		{
			league.Rivalries.Sort((Rivalry x, Rivalry y) => (y.Heat == x.Heat) ? (y.WinsA + y.WinsB + y.Ties).CompareTo(x.WinsA + x.WinsB + x.Ties) : y.Heat.CompareTo(x.Heat));
			league.Rivalries.RemoveRange(160, league.Rivalries.Count - 160);
		}
	}

	public List<Rivalry> RivalriesOf(League league, int teamId)
	{
		List<Rivalry> list = new List<Rivalry>();
		if (league == null)
		{
			return list;
		}
		foreach (Rivalry r in league.Rivalries)
		{
			if (r.TeamA == teamId || r.TeamB == teamId)
			{
				list.Add(r);
			}
		}
		list.Sort((Rivalry x, Rivalry y) => (y.Heat == x.Heat) ? (x.WinsA + x.WinsB).CompareTo(y.WinsA + y.WinsB) : y.Heat.CompareTo(x.Heat));
		return list;
	}

	public bool AreRivals(League league, int a, int b)
	{
		Rivalry r = FindRivalry(league, a, b, create: false);
		if (r != null)
		{
			return r.Heat >= 50;
		}
		return false;
	}

	public static SportProfile Profile(League league)
	{
		return SportProfiles.Get(league.Sport, league.Level);
	}

	public bool Advance(int now)
	{
		int before = Revision;
		_clockHint = now;
		PaySponsors(now);
		PruneTransferOffers();
		EnsureFarmLeagues(now);
		EnsureSecondDivision(now);
		LinkAffiliates();
		StopUnneededFarmLeagues();
		ApplyLeagueSizes(now);
		foreach (League league in OrderedLeagues())
		{
			for (int guard = 0; guard < 200; guard++)
			{
				if (!StepLeague(league, now))
				{
					break;
				}
			}
			if (league.Sport == Sport.Soccer)
			{
				PromotionAndRelegation(now);
			}
			StepOffseason(league, now);
			if (league.Level == Level.College)
			{
				RecruitingMonthly(league, now);
			}
			AllStarsMidseason(league, now);
			TransfersMonthly(league, now);
		}
		PruneTransferOffers();
		InternationalMonthly(now);
		PickGamesOfTheMonth(now);
		LocalHometowns(now);
		PruneBoxes(now);
		ExPlayersYearly();
		if (now != LastTime && AnyLive())
		{
			LiveTick = now / 2;
		}
		LastTime = now;
		return Revision != before;
	}

	private void ApplyLeagueSizes(int now)
	{
		bool otherSizeChanged = Settings.LeagueSize != _appliedLeagueSize;
		bool soccerSizeChanged = Settings.SoccerLeagueSize != _appliedSoccerLeagueSize;
		if (!otherSizeChanged && !soccerSizeChanged)
		{
			return;
		}
		_appliedLeagueSize = Settings.LeagueSize;
		_appliedSoccerLeagueSize = Settings.SoccerLeagueSize;
		foreach (League league in OrderedLeagues())
		{
			if ((league.Sport == Sport.Soccer && !soccerSizeChanged) || (league.Sport != Sport.Soccer && !otherSizeChanged))
			{
				continue;
			}
			Season s = league.Current;
			if (s != null && !s.AnyPlayed && !s.PlayoffsCreated && league.TeamIds.Count != TargetSize(league))
			{
				BuildSeason(league, now);
			}
		}
	}

	public static bool IsDue(Match g, int now)
	{
		if (g.Played)
		{
			return false;
		}
		return IsDue(g, GameClock.MonthStartDay(now), GameClock.NextMonthStartDay(now), GameClock.MinuteOfDay(now));
	}

	private static bool IsDue(Match g, int monthStart, int nextMonth, int clock)
	{
		if (g.Played)
		{
			return false;
		}
		int day = GameClock.DayOf(g.Time);
		if (day < monthStart)
		{
			return true;
		}
		if (g.Featured && day < nextMonth)
		{
			return GameClock.MinuteOfDay(g.Time) <= clock;
		}
		return false;
	}

	private bool StepLeague(League league, int now)
	{
		Season s = league.Current;
		if (s == null)
		{
			if (!KeepsRunning(league) && league.PendingTeamIds.Count == 0)
			{
				return false;
			}
			MergePending(league);
			if (!KeepsRunning(league))
			{
				return false;
			}
			BuildSeason(league, now);
			return league.Current != null;
		}
		bool changed = PlayDueGames(league, s, now);
		if (!s.PlayoffsCreated)
		{
			if (!AllRegularPlayed(s))
			{
				return changed;
			}
			PickAllLeague(league, s, now);
			StartPlayoffs(league, s, now);
			return true;
		}
		int round = CurrentRound(s);
		if (round < 0 || s.PlayoffRounds <= 0)
		{
			CompleteSeason(league, s, now);
			return true;
		}
		if (!RoundDecided(league, s, round, now))
		{
			return changed;
		}
		if (round >= s.PlayoffRounds - 1)
		{
			CompleteSeason(league, s, now);
		}
		else
		{
			CreateRound(league, s, round + 1, now);
		}
		return true;
	}

	private bool PlayDueGames(League league, Season s, int now)
	{
		int monthStart = GameClock.MonthStartDay(now);
		int nextMonth = GameClock.NextMonthStartDay(now);
		int clock = GameClock.MinuteOfDay(now);
		List<Match> due = null;
		foreach (Match g in s.Games)
		{
			if (IsDue(g, monthStart, nextMonth, clock))
			{
				if (due == null)
				{
					due = new List<Match>();
				}
				due.Add(g);
			}
		}
		if (due == null)
		{
			return false;
		}
		due.Sort(ByTime);
		bool changed = false;
		foreach (Match g2 in due)
		{
			PlayoffSeries ps = ((g2.Stage == GameStage.Playoff) ? s.FindSeries(g2.SeriesId) : null);
			if (ps != null && ps.WinnerId >= 0)
			{
				g2.Live = null;
				continue;
			}
			bool monthOver = GameClock.DayOf(g2.Time) < monthStart;
			if (g2.Live == null && WeatherStops(league, s, ps, g2, now, g2.Featured && !monthOver))
			{
				changed = true;
				continue;
			}
			if (g2.Live == null)
			{
				g2.Attendance = Attendance(league, g2);
			}
			if (g2.Featured && !monthOver)
			{
				if (g2.Live == null)
				{
					g2.Live = ComputeGame(league, s, ps, g2, now);
					g2.Live.Start = GameClock.ToTime(monthStart, GameClock.MinuteOfDay(g2.Time)) + g2.RainDelay;
					g2.Live.End = g2.Live.Start + LiveMinutes(league.Sport, league.Level, g2.Live);
					changed = true;
					Revision++;
				}
				if (now < g2.Live.End)
				{
					continue;
				}
			}
			LiveGame result = g2.Live ?? ComputeGame(league, s, ps, g2, now);
			ApplyGame(league, s, ps, g2, result, now);
			changed = true;
		}
		if (changed)
		{
			Revision++;
		}
		return changed;
	}

	public static int LiveMinutes(Sport sport, Level level, LiveGame live)
	{
		bool school = level == Level.HighSchool;
		bool college = level == Level.College;
		int extra = 0;
		int periods = ((live.Box != null) ? Math.Max(live.Box.HomePeriods.Count, live.Box.AwayPeriods.Count) : 0);
		int minutes;
		switch (sport)
		{
		case Sport.Hockey:
			if (school)
			{
				minutes = 105;
			}
			else
			{
				minutes = (college ? 135 : 140);
			}
			if (periods > 3)
			{
				extra = ((live.Result == ResultType.Shootout) ? 20 : ((periods - 3) * (live.Playoff ? 40 : 12)));
			}
			break;
		case Sport.Basketball:
			if (school)
			{
				minutes = 90;
			}
			else
			{
				minutes = (college ? 120 : 135);
			}
			if (periods > 4)
			{
				extra = (periods - 4) * 15;
			}
			break;
		case Sport.Football:
			if (school)
			{
				minutes = 150;
			}
			else
			{
				minutes = (college ? 205 : 185);
			}
			if (periods > 4)
			{
				extra = 20;
			}
			break;
		case Sport.Baseball:
			if (school)
			{
				minutes = 120;
			}
			else if (college)
			{
				minutes = 175;
			}
			else
			{
				minutes = ((level == Level.Minor) ? 150 : 162);
			}
			if (periods > 9)
			{
				extra = (periods - 9) * 18;
			}
			break;
		default:
			if (school)
			{
				minutes = 100;
			}
			else
			{
				minutes = (college ? 110 : 115);
			}
			if (live.Result == ResultType.Penalties)
			{
				extra = 15;
			}
			break;
		}
		return minutes + extra;
	}

	private LiveGame ComputeGame(League league, Season s, PlayoffSeries ps, Match g, int now)
	{
		Team home = GetTeam(g.HomeTeamId);
		Team away = GetTeam(g.AwayTeamId);
		if (home != null && CoachOf(home.Id, CoachRole.Head) == null)
		{
			EnsureStaff(home, now);
		}
		if (away != null && CoachOf(away.Id, CoachRole.Head) == null)
		{
			EnsureStaff(away, now);
		}
		GameRatings(s, ps, g, home, away, out var homeRating, out var awayRating);
		int gameDay = GameClock.DayOf(g.Time);
		homeRating -= InjuryPenalty(home, gameDay);
		awayRating -= InjuryPenalty(away, gameDay);
		RivalryEdge(league, g, home, away, ref homeRating, ref awayRating);
		int homeGoalie = -1;
		int awayGoalie = -1;
		if (league.Sport == Sport.Hockey)
		{
			bool playoff = g.Stage != GameStage.Regular;
			homeRating += PickGoalie(home, gameDay, playoff, out homeGoalie);
			awayRating += PickGoalie(away, gameDay, playoff, out awayGoalie);
		}
		LiveGame live = new LiveGame
		{
			HomeChance = Momentum.HomeWinChance(league.Sport, league.Level, homeRating, awayRating, g.VenueId == -2, g.Stage != GameStage.Regular),
			HomeRating = homeRating,
			AwayRating = awayRating,
			Playoff = (g.Stage != GameStage.Regular)
		};
		Match sim = new Match
		{
			Id = g.Id,
			Time = g.Time,
			HomeTeamId = g.HomeTeamId,
			AwayTeamId = g.AwayTeamId,
			VenueId = g.VenueId,
			Stage = g.Stage,
			SeriesId = g.SeriesId,
			GameNumber = g.GameNumber,
			PlayoffRound = g.PlayoffRound,
			Featured = g.Featured,
			HomeGoalieId = homeGoalie,
			AwayGoalieId = awayGoalie
		};
		_simulator.Play(sim, league.Sport, league.Level, homeRating, awayRating, g.Weather);
		live.HomeScore = sim.HomeScore;
		live.AwayScore = sim.AwayScore;
		live.Result = sim.Result;
		live.HomePenalties = sim.HomePenalties;
		live.AwayPenalties = sim.AwayPenalties;
		if (home != null && away != null)
		{
			live.Box = Stats.Build(sim, league.Sport, league.Level, home, away, Healthy(home.Id, gameDay), Healthy(away.Id, gameDay), _random, out var stars);
			live.Stars = stars;
		}
		return live;
	}

	private void ApplyGame(League league, Season s, PlayoffSeries ps, Match g, LiveGame live, int now)
	{
		Team home = GetTeam(g.HomeTeamId);
		Team away = GetTeam(g.AwayTeamId);
		g.HomeScore = live.HomeScore;
		g.AwayScore = live.AwayScore;
		g.Result = live.Result;
		g.HomePenalties = live.HomePenalties;
		g.AwayPenalties = live.AwayPenalties;
		g.Played = true;
		if (home == null || !Venues.ContainsKey(g.VenueId))
		{
			g.TicketLocalAttendance = 0;
			g.TicketVisitorAttendance = 0;
			g.TicketRevenue = 0;
			g.TicketIncomePaid = true;
		}
		else
		{
			EnsureTicketPrices(home);
			int visitorCount = (int)Math.Round((double)g.Attendance * Math.Max(0.0, Math.Min(1.0, (double)VisitorAttendancePercent / 100.0)));
			g.TicketVisitorAttendance = Math.Max(0, Math.Min(g.Attendance, visitorCount));
			g.TicketLocalAttendance = Math.Max(0, g.Attendance - g.TicketVisitorAttendance);
			g.TicketRevenue = g.TicketLocalAttendance * home.TicketPriceResidents + g.TicketVisitorAttendance * home.TicketPriceVisitors;
			g.TicketIncomePaid = g.TicketRevenue <= 0;
		}
		g.Live = null;
		Momentum.UpdateForm(home, away, g, live.HomeChance);
		RecordCoachResult(g);
		UpdateRivalry(league, s, g, now);
		if (g.Stage == GameStage.Regular)
		{
			double homeResult;
			if (g.WinnerId < 0)
			{
				homeResult = 0.5;
			}
			else
			{
				homeResult = ((g.WinnerId == g.HomeTeamId) ? 1.0 : 0.0);
			}
			UpdateSurprise(league, home, homeResult, live.HomeChance, now);
			UpdateSurprise(league, away, 1.0 - homeResult, 1.0 - live.HomeChance, now);
		}
		if (GamePlayed != null)
		{
			GamePlayed(league, g, live.HomeRating, live.AwayRating);
		}
		if (home != null && away != null && live.Box != null)
		{
			g.Box = live.Box;
			if (league.Sport == Sport.Basketball)
			{
				g.Box.Flow.Clear();
			}
			Stats.Apply(live.Box, AnyPlayer, league.Sport, g.Stage == GameStage.Regular);
			if (home.IsUser || away.IsUser)
			{
				g.Summary = live.Stars;
			}
			MaybeInjure(league, g, now);
		}
		if (ps != null && UpdateSeries(s, ps))
		{
			OnSeriesDecided(league, s, ps, now);
		}
	}

	public bool AnyLive()
	{
		foreach (League l in Leagues)
		{
			if (l.Current == null)
			{
				continue;
			}
			foreach (Match game in l.Current.Games)
			{
				if (game.Live != null)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void GameRatings(Season s, PlayoffSeries ps, Match g, Team home, Team away, out double homeRating, out double awayRating)
	{
		if (ps == null || ps.IsBye)
		{
			homeRating = (double)(home?.Rating ?? 50) + Momentum.FormBonus(home) + CoachEdge(home) + ((home == null) ? 0.0 : ((double)home.Cinderella / 10.0));
			awayRating = (double)(away?.Rating ?? 50) + Momentum.FormBonus(away) + CoachEdge(away) + ((away == null) ? 0.0 : ((double)away.Cinderella / 10.0));
			return;
		}
		SeriesWins(s, ps, out var _, out var lowWins);
		Team highTeam = GetTeam(ps.HighTeamId);
		Team lowTeam = GetTeam(ps.LowTeamId);
		Momentum.SeriesRatings(highTeam, lowTeam, ps, lowWins, out var high, out var low);
		high += CoachEdge(highTeam);
		low += CoachEdge(lowTeam);
		bool highHome = g.HomeTeamId == ps.HighTeamId;
		homeRating = (highHome ? high : low);
		awayRating = (highHome ? low : high);
	}

	private double CoachEdge(Team t)
	{
		if (t == null)
		{
			return 0.0;
		}
		return TacticsBonus(t) + Momentum.FormBonus(t) * (MotivationFactor(t) - 1.0);
	}

	private void OnSeriesDecided(League league, Season s, PlayoffSeries ps, int now)
	{
		if (ps.IsBye || ps.WinnerId < 0)
		{
			return;
		}
		bool lowerWon = ps.WinnerId == ps.LowTeamId && ps.LowSeed > ps.HighSeed;
		Team winner = GetTeam(ps.WinnerId);
		Momentum.AfterSeries(winner, lowerWon);
		RivalrySeries(league, s, ps, now);
		if (lowerWon)
		{
			PostUpset(league, s, ps, now);
			if (ps.LowSeed - ps.HighSeed >= 3)
			{
				AddNote(league, "№ " + ps.LowSeed + " посев " + TeamName(ps.LowTeamId) + " выбил команду с посевом № " + ps.HighSeed + " посев " + TeamName(ps.HighTeamId) + " на стадии " + RoundName(league, s, ps.Round).ToLowerInvariant() + ".");
			}
			if (winner != null && ps.Round >= 1 && ps.LowSeed - ps.HighSeed >= 3 && _random.NextDouble() < 0.35)
			{
				EmergeStar(league, winner, now, "во время своего выступления в плей-офф");
			}
		}
	}

	public static int PlannedMinutes(Sport sport, Level level)
	{
		LiveGame regulation = new LiveGame
		{
			Result = ResultType.Regulation
		};
		return LiveMinutes(sport, level, regulation) + 15;
	}

	private void PickGamesOfTheMonth(int now)
	{
		int month = GameClock.MonthIndex(now);
		int monthStart = GameClock.MonthStartDay(now);
		int nextMonth = GameClock.NextMonthStartDay(now);
		int clock = GameClock.MinuteOfDay(now);
		List<KeyValuePair<Team, League>> waiting = null;
		foreach (League league in OrderedLeagues())
		{
			if (league.Current == null)
			{
				continue;
			}
			foreach (int teamId in league.TeamIds)
			{
				Team t = GetTeam(teamId);
				if (t != null && t.IsUser && !t.Retired && t.FeaturedMonth != month)
				{
					t.FeaturedMonth = month;
					if (waiting == null)
					{
						waiting = new List<KeyValuePair<Team, League>>();
					}
					waiting.Add(new KeyValuePair<Team, League>(t, league));
				}
			}
		}
		if (waiting == null)
		{
			return;
		}
		waiting.Sort((KeyValuePair<Team, League> a, KeyValuePair<Team, League> b) =>
		{
			if (a.Key.WatchFirst != b.Key.WatchFirst)
			{
				if (!a.Key.WatchFirst)
				{
					return 1;
				}
				return -1;
			}
			return (SportRules.Rank(b.Key.Level) == SportRules.Rank(a.Key.Level)) ? a.Key.Id.CompareTo(b.Key.Id) : SportRules.Rank(b.Key.Level).CompareTo(SportRules.Rank(a.Key.Level));
		});
		HashSet<int> teamsWithGame = new HashSet<int>();
		Dictionary<int, List<KeyValuePair<int, Sport>>> booked = FeaturedBookings(monthStart, nextMonth, teamsWithGame);
		bool picked = false;
		foreach (KeyValuePair<Team, League> entry in waiting)
		{
			Team t2 = entry.Key;
			League league2 = entry.Value;
			Season s = league2.Current;
			if (teamsWithGame.Contains(t2.Id))
			{
				t2.LastLiveMonth = month;
				continue;
			}
			bool already = false;
			List<Match> candidates = new List<Match>();
			foreach (Match g in s.Games)
			{
				if (!g.Involves(t2.Id))
				{
					continue;
				}
				int day = GameClock.DayOf(g.Time);
				if (day >= monthStart && day < nextMonth)
				{
					if (g.Featured)
					{
						already = true;
						break;
					}
					if (!g.Played)
					{
						candidates.Add(g);
					}
				}
			}
			if (already || candidates.Count == 0)
			{
				continue;
			}
			candidates.Sort(ByTime);
			Func<Match, bool> fits = (Match match) => GameClock.MinuteOfDay(match.Time) > clock + 15 && !FeaturedClash(booked, match, league2.Sport) && !teamsWithGame.Contains((match.HomeTeamId == t2.Id) ? match.AwayTeamId : match.HomeTeamId);
			Match best = null;
			if (candidates[0].Stage != GameStage.Regular)
			{
				if (fits(candidates[0]))
				{
					best = candidates[0];
				}
			}
			else
			{
				double bestScore = double.MinValue;
				foreach (Match g2 in candidates)
				{
					if (g2.Stage == GameStage.Regular && fits(g2))
					{
						double score = GameAppeal(league2, t2, g2) + _random.NextDouble() * 3.0;
						if (score > bestScore)
						{
							bestScore = score;
							best = g2;
						}
					}
				}
			}
			if (best == null)
			{
				continue;
			}
			best.Featured = true;
			picked = true;
			t2.LastLiveMonth = month;
			teamsWithGame.Add(best.HomeTeamId);
			teamsWithGame.Add(best.AwayTeamId);
			if (best.VenueId >= 0)
			{
				if (!booked.TryGetValue(best.VenueId, out var list))
				{
					list = new List<KeyValuePair<int, Sport>>();
					booked[best.VenueId] = list;
				}
				list.Add(new KeyValuePair<int, Sport>(GameClock.MinuteOfDay(best.Time), league2.Sport));
			}
			Revision++;
		}
		if (picked)
		{
			ChooseSpotlight(now);
		}
	}

	private double GameAppeal(League league, Team t, Match g)
	{
		Team home = GetTeam(g.HomeTeamId);
		Team away = GetTeam(g.AwayTeamId);
		DayOfWeek w = GameClock.Weekday(GameClock.DayOf(g.Time));
		int minute = GameClock.MinuteOfDay(g.Time);
		double score = 0.0;
		if (t != null && g.HomeTeamId == t.Id && g.VenueId == t.VenueId)
		{
			score += 100.0;
		}
		if (home != null && away != null && home.IsUser && away.IsUser)
		{
			score += 20.0;
		}
		Rivalry rv = FindRivalry(league, g.HomeTeamId, g.AwayTeamId, create: false);
		if (rv != null && rv.Heat >= 50)
		{
			score += (double)(15 + rv.Heat / 5);
		}
		if (w == DayOfWeek.Friday || w == DayOfWeek.Saturday || w == DayOfWeek.Sunday)
		{
			score += 10.0;
		}
		if (minute >= 1020 && minute <= 1260)
		{
			score += 5.0;
		}
		return score;
	}

	private void ChooseSpotlight(int now)
	{
		int monthStart = GameClock.MonthStartDay(now);
		int nextMonth = GameClock.NextMonthStartDay(now);
		Dictionary<int, Match> current = new Dictionary<int, Match>();
		Dictionary<int, Match> best = new Dictionary<int, Match>();
		Dictionary<int, double> bestScore = new Dictionary<int, double>();
		foreach (League l in Leagues)
		{
			if (l.Current == null)
			{
				continue;
			}
			foreach (Match g in l.Current.Games)
			{
				int day = GameClock.DayOf(g.Time);
				if (day < monthStart || day >= nextMonth)
				{
					continue;
				}
				if (g.Spotlight && (!g.Featured || g.VenueId < 0) && !g.Played && g.Live == null)
				{
					g.Spotlight = false;
					Revision++;
				}
				if (g.Featured && g.VenueId >= 0)
				{
					if (g.Spotlight)
					{
						current[g.VenueId] = g;
					}
					Team home = GetTeam(g.HomeTeamId);
					Team away = GetTeam(g.AwayTeamId);
					Team mine = ((home != null && home.IsUser) ? home : away);
					double score = (double)(((mine != null && mine.WatchFirst) ? 10000 : 0) + (g.Played ? (-5000) : 0)) + GameAppeal(l, mine, g) + (double)(SportRules.Rank(l.Level) * 8) + (double)((g.Stage != GameStage.Regular) ? 40 : 0) - (double)g.Id * 1E-09;
					if (!bestScore.TryGetValue(g.VenueId, out var have) || score > have)
					{
						bestScore[g.VenueId] = score;
						best[g.VenueId] = g;
					}
				}
			}
		}
		foreach (KeyValuePair<int, Match> kv in best)
		{
			current.TryGetValue(kv.Key, out var cur);
			if (kv.Value != cur && (cur == null || (!cur.Played && cur.Live == null)))
			{
				if (cur != null)
				{
					cur.Spotlight = false;
				}
				kv.Value.Spotlight = true;
				Revision++;
			}
		}
	}

	public List<Match> SpotlightGames(int now)
	{
		List<Match> list = new List<Match>();
		int monthStart = GameClock.MonthStartDay(now);
		int nextMonth = GameClock.NextMonthStartDay(now);
		foreach (League l in Leagues)
		{
			if (l.Current == null)
			{
				continue;
			}
			foreach (Match g in l.Current.Games)
			{
				if (g.Spotlight)
				{
					int day = GameClock.DayOf(g.Time);
					if (day >= monthStart && day < nextMonth)
					{
						list.Add(g);
					}
				}
			}
		}
		list.Sort(ByTime);
		return list;
	}

	public Match SpotlightGame(int now)
	{
		int monthStart = GameClock.MonthStartDay(now);
		int nextMonth = GameClock.NextMonthStartDay(now);
		foreach (League l in Leagues)
		{
			if (l.Current == null)
			{
				continue;
			}
			foreach (Match g in l.Current.Games)
			{
				if (g.Spotlight)
				{
					int day = GameClock.DayOf(g.Time);
					if (day >= monthStart && day < nextMonth)
					{
						return g;
					}
				}
			}
		}
		return null;
	}

	public void RepickGamesOfTheMonth()
	{
		foreach (League l in Leagues)
		{
			if (l.Current == null)
			{
				continue;
			}
			foreach (Match g in l.Current.Games)
			{
				if (g.Featured && !g.Played && g.Live == null)
				{
					g.Featured = false;
					g.Spotlight = false;
				}
			}
			foreach (int id in l.TeamIds)
			{
				Team t = GetTeam(id);
				if (t != null && t.IsUser)
				{
					t.FeaturedMonth = -1;
				}
			}
		}
		Revision++;
	}

	public CommandResult ToggleWatchFirst(int teamId, int now)
	{
		Team t = GetTeam(teamId);
		if (t == null || t.Retired || !t.IsUser)
		{
			return CommandResult.Fail("Это не одна из ваших команд.");
		}
		bool on = !t.WatchFirst;
		foreach (Team value in Teams.Values)
		{
			value.WatchFirst = false;
		}
		t.WatchFirst = on;
		ChooseSpotlight(now);
		Revision++;
		return CommandResult.Success(on ? (TeamName(t.Id) + " — ваша главная команда месяца: её матч станет главным спортивным событием города.") : "Главная команда месяца не выбрана: главным станет лучший матч месяца.");
	}

	private Dictionary<int, List<KeyValuePair<int, Sport>>> FeaturedBookings(int monthStart, int nextMonth, HashSet<int> teams)
	{
		Dictionary<int, List<KeyValuePair<int, Sport>>> booked = new Dictionary<int, List<KeyValuePair<int, Sport>>>();
		foreach (League l in Leagues)
		{
			if (l.Current == null)
			{
				continue;
			}
			foreach (Match g in l.Current.Games)
			{
				if (!g.Featured)
				{
					continue;
				}
				int day = GameClock.DayOf(g.Time);
				if (day < monthStart || day >= nextMonth)
				{
					continue;
				}
				teams.Add(g.HomeTeamId);
				teams.Add(g.AwayTeamId);
				if (g.VenueId >= 0)
				{
					if (!booked.TryGetValue(g.VenueId, out var list))
					{
						list = new List<KeyValuePair<int, Sport>>();
						booked[g.VenueId] = list;
					}
					list.Add(new KeyValuePair<int, Sport>(GameClock.MinuteOfDay(g.Time), l.Sport));
				}
			}
		}
		return booked;
	}

	private bool FeaturedClash(Dictionary<int, List<KeyValuePair<int, Sport>>> booked, Match g, Sport sport)
	{
		if (g.VenueId < 0 || !booked.TryGetValue(g.VenueId, out var list))
		{
			return false;
		}
		int minute = GameClock.MinuteOfDay(g.Time);
		bool outdoor = IsOutdoorVenue(g.VenueId);
		foreach (KeyValuePair<int, Sport> m in list)
		{
			if (Math.Abs(m.Key - minute) < ScheduleBuilder.GapMinutes(m.Value, sport, outdoor))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsOutdoorVenue(int venueId)
	{
		Venue v = GetVenue(venueId);
		if (v != null)
		{
			if (v.Type != VenueType.Stadium && v.Type != VenueType.Ballpark)
			{
				return v.Type == VenueType.Field;
			}
			return true;
		}
		return false;
	}

	public Match GameOfTheMonth(Team t, int now)
	{
		Season s = LeagueOf(t)?.Current;
		if (s == null)
		{
			return null;
		}
		int monthStart = GameClock.MonthStartDay(now);
		int nextMonth = GameClock.NextMonthStartDay(now);
		foreach (Match g in s.Games)
		{
			if (g.Featured && g.Involves(t.Id))
			{
				int day = GameClock.DayOf(g.Time);
				if (day >= monthStart && day < nextMonth)
				{
					return g;
				}
			}
		}
		return null;
	}

	private static int ByTime(Match a, Match b)
	{
		if (a.Time == b.Time)
		{
			return a.Id.CompareTo(b.Id);
		}
		return a.Time.CompareTo(b.Time);
	}

	public int TargetGames(SportProfile p)
	{
		int pct = Math.Max(25, Math.Min(100, Settings.SeasonLengthPercent));
		int games = (int)Math.Round((double)(p.GamesPerTeam * pct) / 100.0, MidpointRounding.AwayFromZero);
		return Math.Max(Math.Min(p.GamesPerTeam, 4), games);
	}

	public int TargetGames(League league)
	{
		if (league != null && league.Sport == Sport.Soccer)
		{
			return Math.Max(0, (TargetSize(league) - 1) * 2);
		}
		return TargetGames(Profile(league));
	}

	private static int MinimumPartialGames(int target)
	{
		return Math.Max(2, (int)Math.Ceiling((double)target * 0.2));
	}

	private void BuildSeason(League league, int now)
	{
		SportProfile p = Profile(league);
		int today = GameClock.MonthStartDay(now) - 1;
		int year = GameClock.YearOfDay(today + 1);
		int target = TargetGames(league);
		int chosen = int.MinValue;
		for (int y = year - 1; y <= year + 1; y++)
		{
			if (chosen != int.MinValue)
			{
				break;
			}
			int startDay = GameClock.DayNumber(p.Start(y));
			if (GameClock.DayNumber(p.End(y)) > today)
			{
				if (startDay > today)
				{
					chosen = y;
					break;
				}
				if (ScheduleBuilder.RemainingGames(p, y, target, today) >= MinimumPartialGames(target))
				{
					chosen = y;
				}
			}
		}
		if (chosen == int.MinValue)
		{
			chosen = year + 1;
		}
		FillWithAiTeams(league);
		PrunePlayers();
		foreach (int id in league.TeamIds)
		{
			Team t = GetTeam(id);
			if (t != null)
			{
				t.FeaturedMonth = -1;
			}
		}
		Season s = MakeSeason(p, league, chosen, today, target);
		if (s.Games.Count == 0)
		{
			s = MakeSeason(p, league, chosen + 1, today, target);
		}
		if (s.Games.Count == 0)
		{
			league.Current = null;
			Revision++;
		}
		else
		{
			league.Current = s;
			ReassignVenues(league);
			Revision++;
		}
	}

	private Season MakeSeason(SportProfile p, League league, int year, int today, int target)
	{
		Season season = new Season
		{
			Year = year,
			StartDay = GameClock.DayNumber(p.Start(year)),
			RegularEndDay = GameClock.DayNumber(p.End(year))
		};
		season.PlayoffEndDay = season.RegularEndDay;
		season.Games = ScheduleBuilder.BuildRegularSeason(p, year, league.TeamIds, today, target, _random, () => NextId++);
		return season;
	}

	private static bool AllRegularPlayed(Season s)
	{
		foreach (Match g in s.Games)
		{
			if (g.Stage == GameStage.Regular && !g.Played)
			{
				return false;
			}
		}
		return true;
	}

	public string SeasonLabel(League league, Season s)
	{
		if (s == null)
		{
			return "";
		}
		SportProfile sportProfile = Profile(league);
		int endYear = sportProfile.End(s.Year).Year;
		if (sportProfile.SingleYearLabel || endYear <= s.Year)
		{
			return s.Year.ToString();
		}
		return s.Year + "-" + (endYear % 100).ToString("00");
	}

	public static int PlayoffTeamCount(SportProfile p, int teams)
	{
		if (teams < 2)
		{
			return 0;
		}
		int q = (int)Math.Round((double)teams * p.PlayoffShare, MidpointRounding.AwayFromZero);
		q = Math.Max(p.MinPlayoffTeams, Math.Min(p.MaxPlayoffTeams, q));
		q = Math.Max(2, Math.Min(q, teams));
		if (p.PlayoffPowerOfTwo)
		{
			int pow = 2;
			while (pow * 2 <= q)
			{
				pow *= 2;
			}
			q = pow;
		}
		return q;
	}

	public static PlayoffRoundFormat FormatFor(SportProfile p, Season s, int round)
	{
		int total = Math.Max(1, s.PlayoffRounds);
		int formats = p.RoundsFromFinal.Length;
		if (round == 0 && total > 1 && total < formats && HasByes(s))
		{
			return p.RoundsFromFinal[formats - 1];
		}
		return p.RoundFormat(round, total);
	}

	private static bool HasByes(Season s)
	{
		foreach (PlayoffSeries ps in s.Bracket)
		{
			if (ps.Round == 0 && ps.IsBye)
			{
				return true;
			}
		}
		return false;
	}

	public string RoundName(League league, Season s, int round)
	{
		return FormatFor(Profile(league), s, round).Name;
	}

	private void StartPlayoffs(League league, Season s, int now)
	{
		SportProfile p = Profile(league);
		List<StandingRow> table = Standings.Compute(league, s);
		int q = PlayoffTeamCount(p, table.Count);
		s.PlayoffsCreated = true;
		s.Bracket.Clear();
		s.PlannedRoundDays.Clear();
		s.PlayoffRounds = 0;
		Revision++;
		RunCoachingCarousel(league, s, now);
		foreach (int id in league.TeamIds)
		{
			Team t = GetTeam(id);
			if (t != null)
			{
				t.RunBonus = 0;
				t.Cinderella = 0;
				t.Surprise = 0;
			}
			foreach (Player item in RosterOf(id))
			{
				item.Playoff = new int[0];
			}
			foreach (Player item2 in ProspectsOf(id))
			{
				item2.Playoff = new int[0];
			}
		}
		if (q < 2)
		{
			return;
		}
		int size = 1;
		int rounds = 0;
		while (size < q)
		{
			size *= 2;
			rounds++;
		}
		s.PlayoffRounds = rounds;
		int[] order = SeedOrder(size);
		for (int i = 0; i < size / 2; i++)
		{
			int high = order[2 * i];
			int low = order[2 * i + 1];
			PlayoffSeries ps = new PlayoffSeries
			{
				Id = NextId++,
				Round = 0,
				Slot = i,
				HighSeed = high,
				LowSeed = low,
				HighTeamId = table[high - 1].TeamId,
				LowTeamId = ((low <= q) ? table[low - 1].TeamId : (-1))
			};
			if (ps.IsBye)
			{
				ps.WinnerId = ps.HighTeamId;
			}
			s.Bracket.Add(ps);
		}
		if (p.PlayoffsBackwardFromFinal)
		{
			List<int> plan = BackwardPlan(p, s);
			if (plan != null && plan[0] >= RoundStartDay(p, s, 0, now))
			{
				s.PlannedRoundDays.AddRange(plan);
			}
		}
		CreateRound(league, s, 0, now);
	}

	public static int[] SeedOrder(int size)
	{
		List<int> order = new List<int> { 1, 2 };
		while (order.Count < size)
		{
			int n = order.Count * 2;
			List<int> next = new List<int>();
			foreach (int seed in order)
			{
				next.Add(seed);
				next.Add(n + 1 - seed);
			}
			order = next;
		}
		return order.ToArray();
	}

	private List<int> BackwardPlan(SportProfile p, Season s)
	{
		int rounds = s.PlayoffRounds;
		PlayoffRoundFormat final = FormatFor(p, s, rounds - 1);
		if (final.FixedDate == null)
		{
			return null;
		}
		int[] starts = new int[rounds];
		starts[rounds - 1] = AlignForward(GameClock.DayNumber(final.FixedDate(s.Year)), final.StartDays);
		for (int r = rounds - 2; r >= 0; r--)
		{
			PlayoffRoundFormat next = FormatFor(p, s, r + 1);
			PlayoffRoundFormat f = FormatFor(p, s, r);
			int latestEnd = starts[r + 1] - next.RestDays - 1;
			int start = latestEnd;
			for (int guard = 0; guard < 28; guard++)
			{
				if (IsAllowedDay(start, f.StartDays) && start + RoundSpan(f, start) <= latestEnd)
				{
					break;
				}
				start--;
			}
			starts[r] = start;
		}
		return new List<int>(starts);
	}

	private static int RoundSpan(PlayoffRoundFormat f, int startDay)
	{
		int span = 0;
		for (int k = 0; k < f.BestOf && k < f.DayOffsets.Length; k++)
		{
			span = Math.Max(span, f.DayOffsets[k]);
		}
		if (f.BestOf > 1)
		{
			span += f.Stagger;
		}
		else if (f.Times != null)
		{
			TimeOption[] array = f.Times(GameClock.Weekday(startDay));
			foreach (TimeOption o in array)
			{
				span = Math.Max(span, o.DayOffset);
			}
		}
		return span;
	}

	private static bool IsAllowedDay(int day, DayOfWeek[] days)
	{
		if (days == null || days.Length == 0)
		{
			return true;
		}
		DayOfWeek w = GameClock.Weekday(day);
		for (int i = 0; i < days.Length; i++)
		{
			if (days[i] == w)
			{
				return true;
			}
		}
		return false;
	}

	private static int AlignForward(int day, DayOfWeek[] days)
	{
		for (int i = 0; i < 7; i++)
		{
			if (IsAllowedDay(day + i, days))
			{
				return day + i;
			}
		}
		return day;
	}

	private int RoundStartDay(SportProfile p, Season s, int round, int now)
	{
		PlayoffRoundFormat f = FormatFor(p, s, round);
		int earliest = PreviousRoundEnd(s, round, now) + f.RestDays + 1;
		if (f.FixedDate != null)
		{
			earliest = Math.Max(earliest, GameClock.DayNumber(f.FixedDate(s.Year)));
		}
		if (round == 0 && p.PlayoffStart != null)
		{
			earliest = Math.Max(earliest, GameClock.DayNumber(p.PlayoffStart(s.Year)));
		}
		if (round < s.PlannedRoundDays.Count)
		{
			earliest = Math.Max(earliest, s.PlannedRoundDays[round]);
		}
		return AlignForward(earliest, f.StartDays);
	}

	private static int PreviousRoundEnd(Season s, int round, int now)
	{
		int prevEnd = ((round == 0) ? LastGameDay(s, GameStage.Regular, -1) : LastGameDay(s, GameStage.Playoff, round - 1));
		if (prevEnd == int.MinValue)
		{
			prevEnd = ((round == 0) ? s.RegularEndDay : (GameClock.MonthStartDay(now) - 1));
		}
		return prevEnd;
	}

	private static int LastGameDay(Season s, GameStage stage, int round)
	{
		int last = int.MinValue;
		foreach (Match g in s.Games)
		{
			if (g.Stage == stage && (stage != GameStage.Playoff || g.PlayoffRound == round))
			{
				last = Math.Max(last, GameClock.DayOf(g.Time));
			}
		}
		return last;
	}

	private void CreateRound(League league, Season s, int round, int now)
	{
		SportProfile p = Profile(league);
		if (round > 0)
		{
			List<PlayoffSeries> prev = SeriesInRound(s, round - 1);
			for (int k = 0; k + 1 < prev.Count; k += 2)
			{
				PlayoffSeries a = prev[k];
				PlayoffSeries b = prev[k + 1];
				int seedA = a.SeedOf(a.WinnerId);
				int seedB = b.SeedOf(b.WinnerId);
				bool aHigh = seedA <= seedB;
				s.Bracket.Add(new PlayoffSeries
				{
					Id = NextId++,
					Round = round,
					Slot = k / 2,
					HighSeed = (aHigh ? seedA : seedB),
					LowSeed = (aHigh ? seedB : seedA),
					HighTeamId = (aHigh ? a.WinnerId : b.WinnerId),
					LowTeamId = (aHigh ? b.WinnerId : a.WinnerId)
				});
			}
		}
		PlayoffRoundFormat f = FormatFor(p, s, round);
		int start = RoundStartDay(p, s, round, now);
		int today = PreviousRoundEnd(s, round, now);
		Dictionary<int, List<ScheduleBuilder.Booking>> occupancy = PlayoffBookings(now);
		TimePicker picker = new TimePicker(_random);
		int index = 0;
		foreach (PlayoffSeries ps in SeriesInRound(s, round))
		{
			ps.BestOf = Math.Max(1, f.BestOf);
			ps.Neutral = f.Neutral;
			if (!ps.IsBye)
			{
				ps.HighEdge = Momentum.NewEdge(league.Sport, _random);
				ps.LowEdge = Momentum.NewEdge(league.Sport, _random);
				int seriesStart = start + ((ps.BestOf > 1 && f.Stagger > 0) ? (index % (f.Stagger + 1)) : 0);
				index++;
				ScheduleSeries(s, ps, f, round, seriesStart, today, picker, occupancy, league.Sport);
				s.PlayoffEndDay = Math.Max(s.PlayoffEndDay, ps.PlannedEndDay);
			}
		}
		foreach (League other in Leagues)
		{
			if (other != league && other.Current != null && !other.Current.PlayoffsCreated)
			{
				ReassignVenues(other);
			}
		}
		Revision++;
	}

	private void ScheduleSeries(Season s, PlayoffSeries ps, PlayoffRoundFormat f, int round, int startDay, int today, TimePicker picker, Dictionary<int, List<ScheduleBuilder.Booking>> occupancy, Sport sport)
	{
		int shift = 0;
		int prevOffset = int.MinValue;
		int prevTime = 0;
		int prevVenue = -1;
		int lastDay = startDay;
		for (int k = 0; k < ps.BestOf; k++)
		{
			int offset = OffsetOf(f, k);
			bool num = ((k < f.HighSeedHome.Length) ? f.HighSeedHome[k] : (k % 2 == 0));
			int home = (num ? ps.HighTeamId : ps.LowTeamId);
			int away = (num ? ps.LowTeamId : ps.HighTeamId);
			int venue;
			int time;
			if (k > 0 && offset == prevOffset)
			{
				venue = prevVenue;
				time = prevTime + 150;
			}
			else
			{
				venue = (f.Neutral ? (-2) : HomeVenue(home));
				int day = startDay + offset + shift;
				TimeOption o = PickPlayoffTime(f, round, day, today, picker);
				int d = day + ((ps.BestOf == 1 && o != null) ? o.DayOffset : 0);
				if (d <= today)
				{
					d = today + 1;
				}
				time = GameClock.ToTime(d, o?.Minute ?? 1140);
				if (venue >= 0)
				{
					time = FreeTime(f, venue, time, ps.Id, occupancy, sport, out var extraDays);
					shift += extraDays;
				}
			}
			s.Games.Add(new Match
			{
				Id = NextId++,
				SeriesId = ps.Id,
				GameNumber = k + 1,
				PlayoffRound = round,
				Stage = GameStage.Playoff,
				Time = time,
				HomeTeamId = home,
				AwayTeamId = away,
				VenueId = venue
			});
			ScheduleBuilder.Occupy(occupancy, venue, time, ps.Id, sport);
			prevOffset = offset;
			prevTime = time;
			prevVenue = venue;
			lastDay = Math.Max(lastDay, GameClock.DayOf(time));
		}
		ps.PlannedEndDay = lastDay;
	}

	private static int OffsetOf(PlayoffRoundFormat f, int game)
	{
		if (f.DayOffsets == null || f.DayOffsets.Length == 0)
		{
			return game * 2;
		}
		if (game < f.DayOffsets.Length)
		{
			return f.DayOffsets[game];
		}
		return f.DayOffsets[f.DayOffsets.Length - 1] + 2 * (game - f.DayOffsets.Length + 1);
	}

	private TimeOption PickPlayoffTime(PlayoffRoundFormat f, int round, int day, int today, TimePicker picker)
	{
		TimeOption[] all = ((f.Times == null) ? null : f.Times(GameClock.Weekday(day)));
		if (all == null || all.Length == 0)
		{
			return null;
		}
		List<TimeOption> valid = new List<TimeOption>();
		TimeOption[] array = all;
		foreach (TimeOption o in array)
		{
			if (day + ((f.BestOf == 1) ? o.DayOffset : 0) > today)
			{
				valid.Add(o);
			}
		}
		if (valid.Count == 0)
		{
			valid.AddRange(all);
		}
		string scope = ((f.BestOf <= 1) ? ("p" + round) : ("p" + round + "d" + day));
		return picker.Pick(scope, valid);
	}

	private int FreeTime(PlayoffRoundFormat f, int venue, int time, int seriesId, Dictionary<int, List<ScheduleBuilder.Booking>> occupancy, Sport sport, out int extraDays)
	{
		extraDays = 0;
		if (!ScheduleBuilder.Conflicts(occupancy, venue, time, seriesId, sport, IsOutdoorVenue))
		{
			return time;
		}
		int day = GameClock.DayOf(time);
		for (int add = 0; add <= 7; add++)
		{
			int d = day + add;
			List<int> minutes = new List<int> { GameClock.MinuteOfDay(time) };
			if (f.Times != null)
			{
				TimeOption[] array = f.Times(GameClock.Weekday(d));
				foreach (TimeOption o in array)
				{
					if (o.DayOffset == 0 && !minutes.Contains(o.Minute))
					{
						minutes.Add(o.Minute);
					}
				}
			}
			int[] fallbackMinutes = FallbackMinutes;
			foreach (int m in fallbackMinutes)
			{
				if (!minutes.Contains(m))
				{
					minutes.Add(m);
				}
			}
			foreach (int m2 in minutes)
			{
				int t = GameClock.ToTime(d, m2);
				if (!ScheduleBuilder.Conflicts(occupancy, venue, t, seriesId, sport, IsOutdoorVenue))
				{
					extraDays = add;
					return t;
				}
			}
		}
		return time;
	}

	private int HomeVenue(int teamId)
	{
		Team t = GetTeam(teamId);
		if (t == null || !t.IsUser || t.Retired)
		{
			return -1;
		}
		return t.VenueId;
	}

	private Dictionary<int, List<ScheduleBuilder.Booking>> PlayoffBookings(int now)
	{
		Dictionary<int, List<ScheduleBuilder.Booking>> occupancy = new Dictionary<int, List<ScheduleBuilder.Booking>>();
		int recent = GameClock.DayOf(now) - 45;
		foreach (League l in Leagues)
		{
			if (l.Current == null)
			{
				continue;
			}
			foreach (Match g in l.Current.Games)
			{
				if (g.VenueId >= 0 && (g.Played ? (GameClock.DayOf(g.Time) >= recent) : (g.Stage != GameStage.Regular)))
				{
					ScheduleBuilder.Occupy(occupancy, g.VenueId, g.Time, BookingKey(g), l.Sport);
				}
			}
		}
		return occupancy;
	}

	private static int BookingKey(Match g)
	{
		if (g.SeriesId == 0)
		{
			return -g.Id;
		}
		return g.SeriesId;
	}

	private static bool UpdateSeries(Season s, PlayoffSeries ps)
	{
		if (ps.WinnerId >= 0 || ps.IsBye)
		{
			return false;
		}
		Count(s, ps, out var highWins, out var lowWins, out var unplayed);
		if (highWins >= ps.WinsNeeded)
		{
			ps.WinnerId = ps.HighTeamId;
		}
		else if (lowWins >= ps.WinsNeeded)
		{
			ps.WinnerId = ps.LowTeamId;
		}
		else if (unplayed == 0)
		{
			ps.WinnerId = ((lowWins > highWins) ? ps.LowTeamId : ps.HighTeamId);
		}
		if (ps.WinnerId < 0)
		{
			return false;
		}
		s.Games.RemoveAll((Match g) => g.SeriesId == ps.Id && g.Stage == GameStage.Playoff && !g.Played);
		return true;
	}

	public static void SeriesWins(Season s, PlayoffSeries ps, out int highWins, out int lowWins)
	{
		Count(s, ps, out highWins, out lowWins, out var _);
	}

	private static void Count(Season s, PlayoffSeries ps, out int highWins, out int lowWins, out int unplayed)
	{
		highWins = 0;
		lowWins = 0;
		unplayed = 0;
		foreach (Match g in s.Games)
		{
			if (g.SeriesId != ps.Id || g.Stage != GameStage.Playoff)
			{
				continue;
			}
			if (!g.Played)
			{
				unplayed++;
				continue;
			}
			int w = g.WinnerId;
			if (w == ps.HighTeamId)
			{
				highWins++;
			}
			else if (w == ps.LowTeamId)
			{
				lowWins++;
			}
		}
	}

	public static int CurrentRound(Season s)
	{
		int round = -1;
		foreach (PlayoffSeries ps in s.Bracket)
		{
			round = Math.Max(round, ps.Round);
		}
		return round;
	}

	private bool RoundDecided(League league, Season s, int round, int now)
	{
		foreach (PlayoffSeries ps in s.Bracket)
		{
			if (ps.Round == round)
			{
				if (ps.WinnerId < 0 && UpdateSeries(s, ps))
				{
					OnSeriesDecided(league, s, ps, now);
				}
				if (ps.WinnerId < 0)
				{
					return false;
				}
			}
		}
		return true;
	}

	public static List<PlayoffSeries> SeriesInRound(Season s, int round)
	{
		List<PlayoffSeries> list = new List<PlayoffSeries>();
		foreach (PlayoffSeries ps in s.Bracket)
		{
			if (ps.Round == round)
			{
				list.Add(ps);
			}
		}
		list.Sort((PlayoffSeries a, PlayoffSeries b) => a.Slot.CompareTo(b.Slot));
		return list;
	}

	public string StageLabel(League league, Match g)
	{
		switch (g.Stage)
		{
		case GameStage.Regular:
			return "";
		case GameStage.Semifinal:
			return "Полуфинал";
		case GameStage.Final:
			return "Финал";
		default:
		{
			Season[] array = new Season[2] { league.Current, league.Previous };
			foreach (Season s in array)
			{
				PlayoffSeries ps = s?.FindSeries(g.SeriesId);
				if (ps != null)
				{
					string name = FormatFor(Profile(league), s, ps.Round).Name;
					if (ps.BestOf <= 1)
					{
						return name;
					}
					return name + "; матч " + g.GameNumber;
				}
			}
			return "Плей-офф";
		}
		}
	}

	private void CompleteSeason(League league, Season s, int now)
	{
		s.Complete = true;
		PlayoffSeries final = null;
		foreach (PlayoffSeries ps in s.Bracket)
		{
			if (ps.Round == s.PlayoffRounds - 1 && (final == null || ps.Slot < final.Slot))
			{
				final = ps;
			}
		}
		if (final != null && final.WinnerId >= 0 && !final.IsBye)
		{
			int champion = final.WinnerId;
			int runnerUp = ((champion == final.HighTeamId) ? final.LowTeamId : final.HighTeamId);
			s.ChampionTeamId = champion;
			Team champTeam = GetTeam(champion);
			league.Champions.Insert(0, new ChampionRecord
			{
				SeasonLabel = SeasonLabel(league, s),
				TeamName = TeamName(champion),
				RunnerUpName = TeamName(runnerUp),
				FinalScore = FinalScoreText(league, s, final),
				IsUser = (champTeam?.IsUser ?? false),
				TeamId = champion
			});
			while (league.Champions.Count > 50)
			{
				league.Champions.RemoveAt(league.Champions.Count - 1);
			}
			PostChampion(league, s, final, champion, runnerUp, now);
			string trophy = AwardBook.ChampionTrophy(league.Sport, league.Level);
			foreach (Player item in RosterOf(champion))
			{
				item.Titles++;
				AddHonor(item, league.Level, SeasonLabel(league, s), "Champion", trophy);
			}
		}
		GiveAwards(league, s, now);
		HallOfFameVote(league, s, now);
		RunCoachingCarousel(league, s, now);
		UpdateRecords(league, s, now);
		AddSeasonSummary(league, s, final);
		CoachesSeasonEnd(league, s, now);
		CoolRivalries(league);
		CleanFeed(league, s);
		if (SportRules.ProLike(league.Sport, league.Level) && league.Previous != null && league.Previous != s)
		{
			if (league.Sport == Sport.Soccer)
			{
				PromotionAndRelegation(now);
			}
			ForceOffseason(league, league.Previous.Year, now);
		}
		AdvanceRosters(league, now);
		foreach (int id in league.TeamIds)
		{
			Team t = GetTeam(id);
			if (t != null)
			{
				t.Form = 0;
				t.RunBonus = 0;
				t.Cinderella = 0;
				t.Surprise = 0;
			}
		}
		league.Previous = s;
		league.Current = null;
		MergePending(league);
		CleanUpUnused();
		Revision++;
		if (SeasonCompleted != null)
		{
			SeasonCompleted(league, s);
		}
		if (KeepsRunning(league))
		{
			BuildSeason(league, now);
		}
	}

	private void AbandonSeason(League league)
	{
		league.Current = null;
		Revision++;
	}

	private static string FinalScoreText(League league, Season s, PlayoffSeries final)
	{
		if (final.BestOf > 1)
		{
			SeriesWins(s, final, out var high, out var low);
			return Math.Max(high, low) + "-" + Math.Min(high, low) + " в серии";
		}
		Match game = null;
		foreach (Match g in s.Games)
		{
			if (g.SeriesId == final.Id && g.Played)
			{
				game = g;
			}
		}
		if (game == null)
		{
			return "";
		}
		string score = Math.Max(game.HomeScore, game.AwayScore) + "-" + Math.Min(game.HomeScore, game.AwayScore);
		if (game.Result == ResultType.Penalties)
		{
			score = score + " (" + Math.Max(game.HomePenalties, game.AwayPenalties) + "-" + Math.Min(game.HomePenalties, game.AwayPenalties) + " по пенальти)";
		}
		else if (game.Result != ResultType.Regulation)
		{
			score = score + " " + SportRules.ResultSuffix(league.Sport, game.Result);
		}
		return score;
	}

	public void RescheduleVenues()
	{
		foreach (League l in OrderedLeagues())
		{
			if (l.Current != null && !l.Current.PlayoffsCreated)
			{
				ReassignVenues(l);
			}
		}
	}

	private void ReassignVenues(League league)
	{
		Season s = league.Current;
		if (s == null)
		{
			return;
		}
		Dictionary<int, List<ScheduleBuilder.Booking>> occupancy = new Dictionary<int, List<ScheduleBuilder.Booking>>();
		foreach (League other in Leagues)
		{
			if (other.Current == null)
			{
				continue;
			}
			foreach (Match g in other.Current.Games)
			{
				if (!g.Played && g.VenueId >= 0 && (other != league || g.Stage != GameStage.Regular))
				{
					ScheduleBuilder.Occupy(occupancy, g.VenueId, g.Time, BookingKey(g), other.Sport);
				}
			}
		}
		ScheduleBuilder.AssignVenues(s.Games, GetTeam, occupancy, league.Sport, IsOutdoorVenue);
		Revision++;
	}

	public static bool TransferWindowOpen(int now)
	{
		int month = GameClock.DateOf(GameClock.DayOf(now)).Month;
		if (month != 1 && month != 2 && month != 3 && month != 7 && month != 8)
		{
			return month == 9;
		}
		return true;
	}

	public static string TransferWindowText(int now)
	{
		int month = GameClock.DateOf(GameClock.DayOf(now)).Month;
		if (month >= 1 && month <= 3)
		{
			return "Зимнее трансферное окно открыто до 26 марта.";
		}
		if (month >= 7 && month <= 9)
		{
			return "Летнее трансферное окно открыто до 2 сентября.";
		}
		if (month < 10)
		{
			return "Трансферное окно закрыто. Летнее окно откроется 13 июля.";
		}
		return "Трансферное окно закрыто. Зимнее окно откроется 26 января.";
	}

	public int TransferValue(Player p)
	{
		int age = AgeOf(p);
		double ovr = Rosters.Ovr(p);
		double v = 400.0 * Math.Exp((ovr - 70.0) * 0.18);
		double ageF;
		if (age <= 21)
		{
			ageF = 1.6;
		}
		else if (age <= 24)
		{
			ageF = 1.3;
		}
		else if (age <= 27)
		{
			ageF = 1.0;
		}
		else if (age <= 29)
		{
			ageF = 0.75;
		}
		else
		{
			ageF = ((age <= 31) ? 0.5 : 0.3);
		}
		v *= ageF;
		if (age <= 24)
		{
			v *= 1.0 + (double)Math.Max(0, p.Potential - p.Overall) * 0.03;
		}
		if (p.Status == PlayerStatus.Active || p.Status == PlayerStatus.Prospect)
		{
			v *= ((p.ContractYears <= 0) ? 0.25 : ((p.ContractYears == 1) ? 0.7 : 1.0));
		}
		int k = (int)Math.Round(v / 50.0) * 50;
		return Math.Max(50, k);
	}

	public int AskingFee(Player p)
	{
		double f = 1.1;
		if (p.Status == PlayerStatus.Active)
		{
			Team t = GetTeam(p.TeamId);
			if (t != null)
			{
				List<Player> list = RosterOf(t.Id).FindAll((Player x) => x.Position == p.Position);
				list.Sort((Player a, Player b) => b.Overall.CompareTo(a.Overall));
				int rank = list.IndexOf(p);
				if (rank >= 0 && rank < Math.Max(1, SlotStarters(Sport.Soccer, p.Position)))
				{
					f = 1.3;
				}
			}
		}
		if (p.Status == PlayerStatus.Abroad)
		{
			f = 1.2;
		}
		int fee = (int)Math.Round((double)TransferValue(p) * f / 50.0) * 50;
		return Math.Max(50, fee);
	}

	private void EnsureTransferBudget(Team t)
	{
		if (t.TransferBudget < 0 && SportRules.ProLike(t.Sport, t.Level) && t.Sport == Sport.Soccer)
		{
			if (t.Level != Level.Pro)
			{
				t.TransferBudget = (t.IsUser ? 3000 : Math.Max(500, (int)Math.Round(((double)(2000 + (t.Rating - 59) * 80) + Rosters.Normal(_random, 0.0, 600.0)) / 50.0) * 50));
			}
			else
			{
				t.TransferBudget = (t.IsUser ? 8000 : Math.Max(1500, (int)Math.Round(((double)(5000 + (t.Rating - 50) * 150) + Rosters.Normal(_random, 0.0, 1500.0)) / 50.0) * 50));
			}
		}
	}

	public int TransferBudgetOf(Team t)
	{
		EnsureTransferBudget(t);
		EnsureClubBalance(t);
		return Math.Max(0, t.TransferBudget);
	}

	private void TransferBudgetsNewYear(League league)
	{
		if (league.Sport != Sport.Soccer || !SportRules.ProLike(league.Sport, league.Level))
		{
			return;
		}
		bool second = league.Level != Level.Pro;
		List<StandingRow> table = ((league.Previous != null) ? Standings.Compute(league, league.Previous) : new List<StandingRow>());
		foreach (int id in league.TeamIds)
		{
			Team t = GetTeam(id);
			if (t != null)
			{
				EnsureTransferBudget(t);
				EnsureClubBalance(t);
				int rank = table.FindIndex((StandingRow r) => r.TeamId == id);
				int add = (second ? (1400 + ((rank >= 0) ? ((rank == 0) ? 1200 : ((rank < table.Count / 2) ? 600 : 200)) : 0)) : (3500 + ((rank >= 0) ? ((rank == 0) ? 3000 : ((rank < table.Count / 2) ? 1500 : 500)) : 0)));
				if (league.Previous != null && league.Previous.ChampionTeamId == id)
				{
					add += (second ? 800 : 2000);
				}
				int previousBudget = t.TransferBudget;
				t.TransferBudget = Math.Min(60000, t.TransferBudget + add);
				int actualIncrease = Math.Max(0, t.TransferBudget - previousBudget);
				t.ClubBalance = (int)Math.Min(int.MaxValue, (long)t.ClubBalance + (long)actualIncrease * 1000L);
			}
		}
	}

	private void TransfersMonthly(League league, int now)
	{
		if (league.Sport != Sport.Soccer || league.Level != Level.Pro)
		{
			return;
		}
		int month = GameClock.MonthIndex(now);
		if (month == _transferMonth)
		{
			return;
		}
		_transferMonth = month;
		bool flag = TransferWindowOpen(now);
		if (!flag)
		{
			TransferOffers.Clear();
		}
		if (!flag)
		{
			foreach (Player p in new List<Player>(Players.Values))
			{
				if (p.Status == PlayerStatus.Abroad)
				{
					RemovePlayer(p);
				}
			}
			return;
		}
		List<League> divisions = SoccerDivisions();
		foreach (League item in divisions)
		{
			foreach (int id in item.TeamIds)
			{
				Team t = GetTeam(id);
				if (t != null)
				{
					EnsureTransferBudget(t);
				}
			}
		}
		RefreshAbroadMarket(now);
		foreach (League l in divisions)
		{
			AiTransfers(l, now);
		}
		foreach (League l2 in divisions)
		{
			MakeTransferOffers(l2, now);
		}
		Revision++;
	}

	private void PruneTransferOffers()
	{
		if (TransferOffers.Count != 0 && TransferOffers.RemoveAll((TransferOffer o) =>
		{
			Player player = GetPlayer(o.PlayerId);
			Team team = GetTeam(o.SellerTeamId);
			Team team2 = ((o.BuyerTeamId < 0) ? null : GetTeam(o.BuyerTeamId));
			return player == null || team == null || team.Retired || !team.IsUser || !team.Manual || player.TeamId != team.Id || (o.BuyerTeamId >= 0 && (team2?.Retired ?? true));
		}) > 0)
		{
			Revision++;
		}
	}

	private void RefreshAbroadMarket(int now)
	{
		int have = 0;
		foreach (Player value in Players.Values)
		{
			if (value.Status == PlayerStatus.Abroad)
			{
				have++;
			}
		}
		int year = CurrentYear();
		Position[] positions = new Position[9]
		{
			Position.Goalkeeper,
			Position.Defender,
			Position.Defender,
			Position.Defender,
			Position.Midfielder,
			Position.Midfielder,
			Position.Midfielder,
			Position.Forward,
			Position.Forward
		};
		for (int k = have; k < 14; k++)
		{
			Position pos = positions[_random.Next(positions.Length)];
			double roll = _random.NextDouble();
			int ovr = (int)Math.Round((double)(Rosters.Base(Level.Pro) + ((roll < 0.15) ? 6 : ((roll < 0.5) ? 2 : (-2)))) + Rosters.Normal(_random, 0.0, 3.0));
			Player p = Rosters.CreatePlayer(Sport.Soccer, Level.Pro, pos, ovr, -1, _random, () => NextId++, null, NameUse());
			int age = 19 + _random.Next(0, 13);
			p.BirthYear = year - age;
			Rosters.PeakAges(p.Sport, p.Position, out var peakStart, out var _);
			p.Potential = ((age < peakStart) ? (p.Overall + _random.Next(2, 4 * (peakStart - age) + 6)) : (p.Overall + _random.Next(0, 3)));
			p.Status = PlayerStatus.Abroad;
			p.TeamId = -1;
			p.From = AbroadClubs[_random.Next(AbroadClubs.Length)];
			p.Salary = 0;
			p.ContractYears = 0;
			AddPlayer(p);
		}
	}

	public List<Player> TransferTargets(Team buyer)
	{
		List<Player> list = new List<Player>();
		if (LeagueOf(buyer) == null)
		{
			return list;
		}
		foreach (League item in SoccerDivisions())
		{
			foreach (int id in item.TeamIds)
			{
				if (id != buyer.Id)
				{
					list.AddRange(SignedPlayers(id));
				}
			}
		}
		foreach (Player p in Players.Values)
		{
			if (p.Status == PlayerStatus.Abroad && p.Sport == Sport.Soccer)
			{
				list.Add(p);
			}
		}
		return list;
	}

	private void CompleteTransfer(League league, Team buyer, Player p, int fee, int now, bool announce)
	{
		Team seller = ((p.Status == PlayerStatus.Abroad) ? null : GetTeam(p.TeamId));
		string from;
		if (seller != null)
		{
			from = TeamName(seller.Id);
		}
		else
		{
			from = (string.IsNullOrEmpty(p.From) ? "зарубежный клуб" : p.From);
		}
		EnsureTransferBudget(buyer);
		EnsureClubBalance(buyer);
		buyer.TransferBudget = Math.Max(0, buyer.TransferBudget - fee);
		buyer.ClubBalance = Math.Max(0, buyer.ClubBalance - fee * 1000);
		if (seller != null)
		{
			EnsureTransferBudget(seller);
			EnsureClubBalance(seller);
			seller.TransferBudget = Math.Min(60000, seller.TransferBudget + fee);
			seller.ClubBalance = (int)Math.Min(int.MaxValue, (long)seller.ClubBalance + (long)fee * 1000L);
			seller.Block.Remove(p.Id);
			p.LastTeamId = seller.Id;
		}
		int month = GameClock.MonthIndex(now);
		int age = AgeOf(p);
		p.From = from;
		Sign(buyer, p, Economy.AskingSalary(p, age, month), Economy.AskingYears(Sport.Soccer, age, resign: false), now, announce: false);
		TransferOffers.RemoveAll((TransferOffer o) => o.PlayerId == p.Id);
		Team[] array = ((seller != null) ? new Team[2] { buyer, seller } : new Team[1] { buyer });
		foreach (Team t in array)
		{
			ArrangeRoster(t, now);
			FillHoles(t, CurrentYear(), now);
			UpdateRating(t);
		}
		string line = Rosters.NewsPositionLabel(p.Position, p.Sport) + " " + p.Name + " (" + Rosters.Ovr(p) + "): из " + from + " в " + TeamName(buyer.Id) + ", " + Economy.Money(fee);
		League sellerLeague = ((seller != null) ? LeagueOf(seller) : null);
		league = LeagueOf(buyer) ?? league;
		League[] array2 = ((sellerLeague == null || sellerLeague == league) ? new League[1] { league } : new League[2] { league, sellerLeague });
		foreach (League l in array2)
		{
			l.Transfers.Insert(0, GameClock.MonthLabel(now) + ": " + line);
			while (l.Transfers.Count > 40)
			{
				l.Transfers.RemoveAt(l.Transfers.Count - 1);
			}
		}
		if (announce || buyer.IsUser || (seller != null && seller.IsUser))
		{
			Post(league, NewsKind.Signing, Insider, "ТРАНСФЕР: «" + TeamName(buyer.Id) + "» подписывает " + Rosters.NewsPositionLabel(p.Position, p.Sport) + " " + p.Name + " (" + Rosters.Ovr(p) + " ОБЩ) из клуба " + from + " за " + Economy.Money(fee) + ".", buyer.IsUser || (seller?.IsUser ?? false), now, 2);
		}
		if (seller != null && seller.IsUser)
		{
			PostAsTeam(league, seller, NewsKind.Signing, "Спасибо, " + p.Name + "; удачи в " + TeamName(buyer.Id) + ".", now);
		}
		Revision++;
	}

	private void SellAbroad(League league, Team seller, Player p, string club, int fee, int now)
	{
		EnsureTransferBudget(seller);
		EnsureClubBalance(seller);
		seller.TransferBudget = Math.Min(60000, seller.TransferBudget + fee);
		seller.ClubBalance = (int)Math.Min(int.MaxValue, (long)seller.ClubBalance + (long)fee * 1000L);
		seller.Block.Remove(p.Id);
		TransferOffers.RemoveAll((TransferOffer o) => o.PlayerId == p.Id);
		string line = Rosters.NewsPositionLabel(p.Position, p.Sport) + " " + p.Name + " (" + Rosters.Ovr(p) + "): из клуба " + TeamName(seller.Id) + " в " + club + ", " + Economy.Money(fee);
		league.Transfers.Insert(0, GameClock.MonthLabel(now) + ": " + line);
		while (league.Transfers.Count > 40)
		{
			league.Transfers.RemoveAt(league.Transfers.Count - 1);
		}
		Post(league, NewsKind.Signing, Insider, "ПРОДАН ЗА РУБЕЖ: " + Rosters.NewsPositionLabel(p.Position, p.Sport) + " " + p.Name + " (" + Rosters.Ovr(p) + " ОБЩ) покидает клуб " + TeamName(seller.Id) + " в клуб " + club + " за " + Economy.Money(fee) + " трансфер. ", seller.IsUser, now, 3);
		if (seller.IsUser)
		{
			PostAsTeam(league, seller, NewsKind.Signing, "Спасибо, " + p.Name + "; удачи в " + club + ".", now);
		}
		RemovePlayer(p);
		ArrangeRoster(seller, now);
		FillHoles(seller, CurrentYear(), now);
		UpdateRating(seller);
		Revision++;
	}

	private void AiTransfers(League league, int now)
	{
		List<Team> clubs = LeagueTeams(league).FindAll((Team t) => !t.Manual);
		int year = CurrentYear();
		foreach (Team buyer in clubs)
		{
			if (_random.NextDouble() >= 0.35)
			{
				continue;
			}
			int budget = TransferBudgetOf(buyer);
			List<Player> roster = SignedPlayers(buyer.Id);
			Position need = Position.Midfielder;
			int weakest = int.MaxValue;
			RosterSlot[] array = Rosters.Template(Sport.Soccer, Level.Pro);
			foreach (RosterSlot slot in array)
			{
				int w = Weakest(roster, slot.Pos, Math.Max(1, slot.Starters));
				if (w < weakest)
				{
					weakest = w;
					need = slot.Pos;
				}
			}
			Player pick = null;
			int pickFee = 0;
			foreach (Player p in TransferTargets(buyer))
			{
				if (p.Position != need || p.Overall < weakest + 2)
				{
					continue;
				}
				Team owner = GetTeam(p.TeamId);
				if ((owner == null || !owner.IsUser) && (owner == null || !owner.Manual))
				{
					int fee = AskingFee(p);
					if (!((double)fee > (double)budget * 0.6) && NoRoomFor(buyer, p) == null && Economy.CanAfford(buyer, roster, Economy.AskingSalary(p, year - p.BirthYear, GameClock.MonthIndex(now)), resign: false) && (pick == null || (double)p.Overall - (double)fee / 2000.0 > (double)pick.Overall - (double)pickFee / 2000.0))
					{
						pick = p;
						pickFee = fee;
					}
				}
			}
			if (pick != null)
			{
				CompleteTransfer(league, buyer, pick, pickFee, now, pick.Overall >= Rosters.Base(Level.Pro) + 2);
			}
		}
		foreach (Team seller in clubs)
		{
			foreach (Player p2 in new List<Player>(RosterOf(seller.Id)))
			{
				if (year - p2.BirthYear <= 25 && Rosters.Ovr(p2) >= 84 && !(_random.NextDouble() >= 0.08))
				{
					SellAbroad(league, seller, p2, AbroadClubs[_random.Next(AbroadClubs.Length)], (int)Math.Round((double)TransferValue(p2) * 1.25 / 50.0) * 50, now);
					break;
				}
			}
		}
	}

	private void MakeTransferOffers(League league, int now)
	{
		int month = GameClock.MonthIndex(now);
		int year = CurrentYear();
		foreach (Team mine in LeagueTeams(league))
		{
			if (!mine.IsUser || !mine.Manual)
			{
				continue;
			}
			mine.Block.RemoveAll((int id) =>
			{
				Player player = GetPlayer(id);
				return player == null || player.TeamId != mine.Id;
			});
			int made = 0;
			foreach (Player p in SignedPlayers(mine.Id))
			{
				bool listed = mine.Block.Contains(p.Id);
				bool star = year - p.BirthYear <= 26 && Rosters.Ovr(p) >= 82;
				if ((!listed && (!star || !(_random.NextDouble() < 0.12))) || TransferOffers.Exists((TransferOffer o) => o.PlayerId == p.Id))
				{
					continue;
				}
				int value = TransferValue(p);
				Team buyer = null;
				List<Team> clubs = new List<Team>();
				foreach (League l in SoccerDivisions())
				{
					clubs.AddRange(LeagueTeams(l));
				}
				foreach (Team t in clubs)
				{
					if (t != mine && !t.IsUser && !((double)TransferBudgetOf(t) < (double)value * 0.8))
					{
						int weakest = Weakest(SignedPlayers(t.Id), p.Position, Math.Max(1, SlotStarters(Sport.Soccer, p.Position)));
						if (p.Overall > weakest && (buyer == null || _random.NextDouble() < 0.4))
						{
							buyer = t;
						}
					}
				}
				bool abroad = buyer == null || (star && _random.NextDouble() < 0.5);
				if ((!abroad || star || Rosters.Ovr(p) >= 78) && (!listed || !(_random.NextDouble() < 0.25)))
				{
					double factor = (abroad ? (0.95 + 0.35 * _random.NextDouble()) : (0.8 + 0.3 * _random.NextDouble()));
					int fee = Math.Max(50, (int)Math.Round((double)value * factor / 50.0) * 50);
					TransferOffers.Add(new TransferOffer
					{
						Id = NextId++,
						PlayerId = p.Id,
						SellerTeamId = mine.Id,
						BuyerTeamId = (abroad ? (-1) : buyer.Id),
						BuyerName = (abroad ? AbroadClubs[_random.Next(AbroadClubs.Length)] : TeamName(buyer.Id)),
						Fee = fee,
						Month = month
					});
					made++;
					if (made >= 4)
					{
						break;
					}
				}
			}
			if (made > 0)
			{
				PostAsTeam(league, mine, NewsKind.Signing, "ТРАНСФЕРНЫЕ НОВОСТИ: " + made + ((made == 1) ? " предложение поступило" : " предложений поступило") + " по нашим игрокам.", now);
			}
		}
	}

	private Team UserSoccerClub(int teamId, out string error)
	{
		Team t = GetTeam(teamId);
		error = "";
		if (t == null || t.Retired || !t.IsUser)
		{
			error = "Это не одна из ваших команд.";
			return null;
		}
		if (t.Sport != Sport.Soccer || !SportRules.ProLike(t.Sport, t.Level))
		{
			error = "Трансферы доступны только профессиональным футбольным клубам.";
			return null;
		}
		if (!t.Manual)
		{
			error = "Сначала выберите «Самостоятельное управление командой» в офисе клуба, чтобы проводить трансферы.";
			return null;
		}
		return t;
	}

	public CommandResult BidForPlayer(int teamId, int playerId, int fee, int now)
	{
		_clockHint = now;
		Team t = UserSoccerClub(teamId, out var error);
		if (t == null)
		{
			return CommandResult.Fail(error);
		}
		League league = LeagueOf(t);
		if (league == null)
		{
			return CommandResult.Fail("Ваш клуб не входит ни в одну лигу.");
		}
		if (!TransferWindowOpen(now))
		{
			return CommandResult.Fail(TransferWindowText(now));
		}
		Player p = GetPlayer(playerId);
		if (p == null || p.Sport != Sport.Soccer || (p.Status != PlayerStatus.Abroad && (p.TeamId == t.Id || !InSoccerDivisions(p.TeamId))))
		{
			return CommandResult.Fail("Этот игрок не выставлен на рынок.");
		}
		Team owner = GetTeam(p.TeamId);
		if (owner != null && owner.IsUser && owner.Manual)
		{
			return CommandResult.Fail("Вы управляете обоими клубами: это не считается трансфером.");
		}
		fee = Math.Max(50, fee);
		if (fee > TransferBudgetOf(t))
		{
			return CommandResult.Fail("Это превышает ваш трансферный бюджет (" + Economy.Money(TransferBudgetOf(t)) + ").");
		}
		string full = NoRoomFor(t, p);
		if (full != null)
		{
			return CommandResult.Fail(full);
		}
		int age = AgeOf(p);
		int salary = Economy.AskingSalary(p, age, GameClock.MonthIndex(now));
		if (!Economy.CanAfford(t, SignedPlayers(t.Id), salary, resign: false))
		{
			return CommandResult.Fail(p.Name + " запросит " + Economy.Money(salary) + " за сезон. Это не укладывается в ваш зарплатный бюджет.");
		}
		int asking = AskingFee(p);
		string club;
		if (owner != null)
		{
			club = "клуб «" + TeamName(owner.Id) + "»";
		}
		else
		{
			club = (string.IsNullOrEmpty(p.From) ? "его клуб" : p.From);
		}
		if (fee < asking && (!((double)fee >= (double)asking * 0.9) || !(_random.NextDouble() < 0.5)))
		{
			return CommandResult.Fail(club.Substring(0, 1).ToUpperInvariant() + club.Substring(1) + " отклонил предложение " + Economy.Money(fee) + " за игрока " + p.Name + ". Они рассчитывают примерно на " + Economy.Money(asking) + ".");
		}
		CompleteTransfer(league, t, p, fee, now, announce: true);
		return CommandResult.Success(p.Name + " переходит за " + Economy.Money(fee) + " (" + Economy.ContractText(p) + "). Остаток трансферного бюджета: " + Economy.Money(TransferBudgetOf(t)) + ".");
	}

	public CommandResult AcceptTransferOffer(int offerId, int now)
	{
		_clockHint = now;
		TransferOffer o = TransferOffers.Find((TransferOffer x) => x.Id == offerId);
		if (o == null)
		{
			return CommandResult.Fail("Это предложение больше не действует.");
		}
		Team mine = GetTeam(o.SellerTeamId);
		Player p = GetPlayer(o.PlayerId);
		League league = ((mine == null) ? null : LeagueOf(mine));
		if (mine == null || p == null || p.TeamId != mine.Id || league == null)
		{
			TransferOffers.Remove(o);
			return CommandResult.Fail("Это предложение больше не действует.");
		}
		if (!TransferWindowOpen(now))
		{
			TransferOffers.Remove(o);
			return CommandResult.Fail(TransferWindowText(now));
		}
		if (o.BuyerTeamId < 0)
		{
			SellAbroad(league, mine, p, o.BuyerName, o.Fee, now);
			return CommandResult.Success(p.Name + " продан в клуб " + o.BuyerName + " за " + Economy.Money(o.Fee) + ". Трансферный бюджет: " + Economy.Money(TransferBudgetOf(mine)) + ".");
		}
		Team buyer = GetTeam(o.BuyerTeamId);
		if (buyer == null || buyer.Retired)
		{
			TransferOffers.Remove(o);
			return CommandResult.Fail("Этого клуба больше не существует.");
		}
		CompleteTransfer(league, buyer, p, o.Fee, now, announce: true);
		return CommandResult.Success(p.Name + " продан в " + TeamName(buyer.Id) + " за " + Economy.Money(o.Fee) + ". Трансферный бюджет: " + Economy.Money(TransferBudgetOf(mine)) + ".");
	}

	public CommandResult DeclineTransferOffer(int offerId)
	{
		int num = TransferOffers.RemoveAll((TransferOffer x) => x.Id == offerId);
		Revision++;
		if (num <= 0)
		{
			return CommandResult.Fail("Это предложение больше не действует.");
		}
		return CommandResult.Success("Предложение отклонено.");
	}

	public ContractTalk FindTalk(int playerId, int teamId)
	{
		foreach (ContractTalk k in Talks)
		{
			if (k.PlayerId == playerId && k.TeamId == teamId)
			{
				return k;
			}
		}
		return null;
	}

	public int Interest(Player p, Team t, bool extension)
	{
		PriorityFactor(p, t, extension, out var refuses);
		if (refuses)
		{
			return 0;
		}
		double v = 50.0;
		League league = LeagueOf(t);
		List<int> order = ((league == null) ? new List<int>() : ContentionOrder(league));
		int i = order.IndexOf(t.Id);
		double rank = ((order.Count <= 1 || i < 0) ? 0.5 : ((double)i / (double)(order.Count - 1)));
		switch (p.Priority)
		{
		case PlayerPriority.Winning:
			v += (double)((rank < 0.34) ? 25 : ((!(rank < 0.67)) ? (-30) : 0));
			break;
		case PlayerPriority.Loyalty:
			v += (double)((extension || p.LastTeamId == t.Id) ? 30 : 0);
			break;
		case PlayerPriority.Money:
		{
			int limit = Math.Max(1, Economy.Limit(t));
			int room = limit - Economy.Payroll(SignedPlayers(t.Id)) - Economy.DeadNow(t);
			v += Math.Max(-10.0, Math.Min(15.0, (double)room * 60.0 / (double)limit));
			break;
		}
		case PlayerPriority.PlayingTime:
			v += (double)(WouldStart(p, t) ? 20 : (-20));
			break;
		case PlayerPriority.Hometown:
			v += (double)((t.IsUser && IsCityPlayer(p)) ? 35 : (-5));
			break;
		}
		v += (double)((rank < 0.34) ? 8 : ((rank > 0.67) ? (-8) : 0));
		v += RecruitingEdge(t) * 2.0;
		if (extension && p.Priority != PlayerPriority.Money)
		{
			v += 10.0;
		}
		return (int)Math.Round(Math.Max(5.0, Math.Min(95.0, v)));
	}

	private static int StartPatience(Player p, int interest)
	{
		double v = 30.0 + (double)interest * 0.6;
		if (p.Priority == PlayerPriority.Money)
		{
			v -= 10.0;
		}
		if (p.Priority == PlayerPriority.Loyalty)
		{
			v += 10.0;
		}
		return (int)Math.Round(Math.Max(15.0, Math.Min(95.0, v)));
	}

	private static double Flex(int interest)
	{
		return 0.02 + (double)interest * 0.0008;
	}

	public void AskOf(Player p, Team t, bool extension, int now, out int salary, out int years)
	{
		if (extension)
		{
			ResignTerms(p, t, out salary, out years);
			return;
		}
		salary = AskingFrom(p, t, now, out var _);
		years = Economy.AskingYears(t.Sport, AgeOf(p), resign: false);
	}

	public ContractTalk TalkOrPreview(Player p, Team t, bool extension, int now)
	{
		ContractTalk k = FindTalk(p.Id, t.Id);
		if (k != null)
		{
			return k;
		}
		k = new ContractTalk
		{
			PlayerId = p.Id,
			TeamId = t.Id,
			Extension = extension
		};
		AskOf(p, t, extension, now, out k.Ask, out k.AskYears);
		k.Interest = Interest(p, t, extension);
		k.Patience = StartPatience(p, k.Interest);
		return k;
	}

	public int Suitors(Player p, int now)
	{
		League league = GetLeague(p.Sport, Level.Pro);
		if (league == null)
		{
			return 0;
		}
		int n = 0;
		int salary = Economy.AskingSalary(p, AgeOf(p), GameClock.MonthIndex(now));
		List<Team> clubs = LeagueTeams(league);
		if (p.Sport == Sport.Soccer && SecondDivision() != null)
		{
			clubs.AddRange(LeagueTeams(SecondDivision()));
		}
		foreach (Team t in clubs)
		{
			if (t.Manual)
			{
				continue;
			}
			List<Player> signed = SignedPlayers(t.Id);
			int count = SlotCount(t.Sport, p.Position);
			int starters = SlotStarters(t.Sport, p.Position);
			int weakest = Weakest(signed, p.Position, count);
			int weakestStarter = Weakest(signed, p.Position, starters);
			double gain = Math.Max(0, p.Overall - weakestStarter) * 2 + Math.Max(0, p.Overall - weakest);
			if (weakest == 0)
			{
				gain += 6.0;
			}
			if (!(gain < 3.0))
			{
				int teamSalary = Economy.Discount(salary, t.Sport, PriorityFactor(p, t, resign: false, out var refuses));
				if (!refuses && Economy.CanAfford(t, signed, teamSalary, resign: false))
				{
					n++;
				}
			}
		}
		return n;
	}

	public static string Mood(ContractTalk k)
	{
		if (k.Status == 2)
		{
			return "Он прекратил переговоры с вами.";
		}
		if (k.Patience >= 60)
		{
			return "Он хочет договориться.";
		}
		if (k.Patience >= 30)
		{
			return "Его агент теряет терпение.";
		}
		return "Ещё одно заниженное предложение — и он уйдёт.";
	}

	public CommandResult OfferContract(int teamId, int playerId, int salary, int years, int now)
	{
		_clockHint = now;
		Team t = UserProTeam(teamId, out var error);
		if (t == null)
		{
			return CommandResult.Fail(error);
		}
		if (!t.Manual)
		{
			return CommandResult.Fail("Сначала выберите «Самостоятельное управление командой» в офисе клуба. До этого контрактами занимается ИИ-менеджер.");
		}
		Player p = GetPlayer(playerId);
		if (p == null || p.Sport != t.Sport)
		{
			return CommandResult.Fail("Игрок не найден.");
		}
		bool extension;
		if (p.TeamId == t.Id && p.Status != PlayerStatus.FreeAgent)
		{
			if (p.ContractYears > 1)
			{
				return CommandResult.Fail(p.Name + " подписал контракт ещё на " + p.ContractYears + " сезона. Вы можете продлевать контракты игроков, у которых остался последний год.");
			}
			extension = true;
		}
		else
		{
			if (p.Status != PlayerStatus.FreeAgent)
			{
				return CommandResult.Fail(p.Name + " не является свободным агентом.");
			}
			extension = false;
		}
		if (!extension)
		{
			string full = NoRoomFor(t, p);
			if (full != null)
			{
				return CommandResult.Fail(full);
			}
		}
		ContractTalk k = TalkOrPreview(p, t, extension, now);
		if (k.Status == 2)
		{
			return CommandResult.Fail(p.Name + " прекратил переговоры с вами" + (extension ? ". Он выйдет на рынок свободных агентов." : " в это межсезонье."));
		}
		if (k.Interest <= 0)
		{
			return CommandResult.Fail(p.Name + " хочет побеждать и не заинтересован в переходе в команду из нижней части таблицы.");
		}
		EconomyProfile e = Economy.Get(t.Sport);
		int maxYears = (extension ? e.MaxYearsResign : e.MaxYearsFree);
		years = Math.Max(1, Math.Min(maxYears, years));
		salary = Math.Max(e.MinSalary, Math.Min(Economy.MaxSalary(t.Sport), Economy.RoundSalary(salary)));
		List<Player> others = SignedPlayers(t.Id).FindAll((Player q) => q != p);
		if (!Economy.CanAfford(t, others, salary, extension))
		{
			return CommandResult.Fail(Economy.Money(salary) + " за сезон не укладывается в ваш " + e.CapName.ToLowerInvariant() + ". Освободите место в бюджете или предложите меньшую сумму.");
		}
		if (!Talks.Contains(k))
		{
			Talks.Add(k);
		}
		int month = GameClock.MonthIndex(now);
		k.Rounds++;
		k.Offer = salary;
		k.OfferYears = years;
		k.LastMonth = month;
		double ratio = OfferRatio(k, salary, years);
		if (ratio >= 1.0 - Flex(k.Interest))
		{
			Talks.Remove(k);
			if (extension)
			{
				p.Salary = salary;
				p.ContractYears = Math.Max(0, p.ContractYears) + years;
				Revision++;
				PostAsTeam(LeagueOf(t), t, NewsKind.Signing, "Мы продлили контракт с " + Rosters.NewsPositionLabel(p.Position, p.Sport) + " " + p.Name + ": " + Economy.Money(salary) + " за сезон до " + (CurrentYear() + p.ContractYears) + ".", now);
				return CommandResult.Success("Договорились! " + p.Name + " продлил контракт: " + Economy.Money(salary) + " за сезон, " + years + ((years == 1) ? " ещё на один сезон." : " ещё на несколько сезонов."));
			}
			Sign(t, p, salary, years, now, p.Overall >= Rosters.Base(Level.Pro));
			ArrangeRoster(t, now);
			UpdateRating(t);
			Revision++;
			string where;
			if (p.Status != PlayerStatus.Active)
			{
				where = ((p.Status == PlayerStatus.Prospect) ? "Он начнёт в резерве." : "Для него не нашлось места в составе, поэтому его снова отпустили.");
			}
			else
			{
				where = "Он включён в основной состав.";
			}
			return CommandResult.Success("Договорились! " + p.Name + " подписал контракт: " + Economy.ContractText(p) + ". " + where);
		}
		bool insult = ratio < 0.75;
		int cost = ((!insult) ? ((int)Math.Round((8.0 + (1.0 - ratio) * 60.0) * ((p.Priority == PlayerPriority.Money) ? 1.3 : ((p.Priority == PlayerPriority.Loyalty) ? 0.7 : 1.0)))) : 30);
		if (k.Rounds > 4)
		{
			cost += 5;
		}
		k.Patience = Math.Max(0, k.Patience - cost);
		if (k.Patience <= 0)
		{
			k.Status = 2;
			k.Message = "Его агент прекращает переговоры." + (extension ? (" " + p.Name + " выйдет на рынок свободных агентов.") : " Он не подпишет с вами контракт в это межсезонье.");
			Revision++;
			if (p.Overall >= Rosters.Base(Level.Pro))
			{
				Post(LeagueOf(t), NewsKind.Signing, Insider, "Переговоры между " + Rosters.NewsPositionLabel(p.Position, p.Sport) + " " + p.Name + " и " + TeamName(t.Id) + " прекращены." + (extension ? " Он выйдет на рынок свободных агентов." : ""), isUser: true, now, 2);
			}
			return CommandResult.Fail("Сделка не состоялась. " + k.Message);
		}
		double concession = 0.15 + (double)k.Interest * 0.0035;
		double offered = (double)salary * YearsFactor(years, k.AskYears);
		int newAsk = Economy.RoundSalary((double)k.Ask - ((double)k.Ask - offered) * concession);
		k.Ask = Math.Max(e.MinSalary, Math.Min(k.Ask, newAsk));
		if (years != k.AskYears && _random.NextDouble() < 0.5)
		{
			k.AskYears += ((years > k.AskYears) ? 1 : (-1));
		}
		k.Message = (insult ? "Это оскорбительное предложение. " : "") + "Он предлагает встречную сумму " + Economy.Money(k.Ask) + " за сезон в течение " + k.AskYears + ((k.AskYears == 1) ? " за год. " : " за год. ") + Mood(k);
		Revision++;
		return CommandResult.Success("Встречное предложение: " + k.Message);
	}

	public CommandResult AcceptAsk(int teamId, int playerId, int now)
	{
		Player p = GetPlayer(playerId);
		Team t = GetTeam(teamId);
		if (p == null || t == null)
		{
			return CommandResult.Fail("Игрок не найден.");
		}
		bool extension = p.TeamId == t.Id && p.Status != PlayerStatus.FreeAgent;
		ContractTalk k = TalkOrPreview(p, t, extension, now);
		return OfferContract(teamId, playerId, k.Ask, k.AskYears, now);
	}

	private static double YearsFactor(int years, int wanted)
	{
		return Math.Max(0.6, 1.0 - 0.04 * (double)Math.Abs(years - wanted));
	}

	private static double OfferRatio(ContractTalk k, int salary, int years)
	{
		if (k.Ask > 0)
		{
			return (double)salary * YearsFactor(years, k.AskYears) / (double)k.Ask;
		}
		return 9.0;
	}

	private bool StandingOfferWins(League league, Player p, Team aiTeam, int now)
	{
		ContractTalk best = null;
		double bestChance = 0.0;
		foreach (ContractTalk k in Talks)
		{
			if (k.PlayerId != p.Id || k.Status != 0 || k.Extension || k.Offer <= 0)
			{
				continue;
			}
			Team t = GetTeam(k.TeamId);
			if (t != null && t.Manual && t.IsUser)
			{
				double ratio = OfferRatio(k, k.Offer, k.OfferYears);
				double chance = Math.Max(0.0, Math.Min(0.9, (ratio - 0.85) * 4.0 + (double)(k.Interest - 50) / 100.0));
				if (chance > bestChance && Economy.CanAfford(t, SignedPlayers(t.Id), k.Offer, resign: false) && NoRoomFor(t, p) == null)
				{
					bestChance = chance;
					best = k;
				}
			}
		}
		if (best == null || _random.NextDouble() >= bestChance)
		{
			return false;
		}
		Team mine = GetTeam(best.TeamId);
		Talks.Remove(best);
		Sign(mine, p, best.Offer, best.OfferYears, now, announce: true);
		ArrangeRoster(mine, now);
		UpdateRating(mine);
		Post(league, NewsKind.Signing, Insider, Rosters.NewsPositionLabel(p.Position, p.Sport) + " " + p.Name + " выбирает " + TeamName(mine.Id) + " вместо " + TeamName(aiTeam.Id) + ".", isUser: true, now, 3);
		return true;
	}

	private void TalksLost(League league, Player p, Team signedWith, int now)
	{
		for (int i = Talks.Count - 1; i >= 0; i--)
		{
			ContractTalk k = Talks[i];
			if (k.PlayerId == p.Id && k.TeamId != signedWith.Id)
			{
				Talks.RemoveAt(i);
				Team t = GetTeam(k.TeamId);
				if (t != null && t.IsUser && k.Status == 0 && k.Offer > 0)
				{
					Post(league, NewsKind.Signing, Insider, Rosters.NewsPositionLabel(p.Position, p.Sport) + " " + p.Name + " подписал контракт с " + TeamName(signedWith.Id) + " пока " + TeamName(t.Id) + " продолжали переговоры.", isUser: true, now, 2);
				}
			}
		}
	}

	private void TalksMonthly(League league, int now)
	{
		int month = GameClock.MonthIndex(now);
		for (int i = Talks.Count - 1; i >= 0; i--)
		{
			ContractTalk k = Talks[i];
			Player p = GetPlayer(k.PlayerId);
			Team t = GetTeam(k.TeamId);
			if (t == null || p == null || t.Sport != league.Sport)
			{
				if (t == null || p == null)
				{
					Talks.RemoveAt(i);
				}
				continue;
			}
			bool num;
			if (!k.Extension)
			{
				num = p.Status == PlayerStatus.FreeAgent;
			}
			else
			{
				if (p.TeamId != t.Id || p.Status == PlayerStatus.FreeAgent)
				{
					goto IL_00af;
				}
				num = p.ContractYears <= 1;
			}
			if (!num)
			{
				goto IL_00af;
			}
			goto IL_00c5;
			IL_00af:
			if (k.Status == 0)
			{
				Talks.RemoveAt(i);
				continue;
			}
			goto IL_00c5;
			IL_00c5:
			if (k.Status == 0 && !k.Extension && k.LastMonth >= 0 && month > k.LastMonth)
			{
				k.Patience = Math.Max(0, k.Patience - 8);
				k.LastMonth = month;
				if (k.Patience <= 0)
				{
					k.Status = 2;
					k.Message = "Он устал ждать. В это межсезонье он не подпишет с вами контракт.";
				}
			}
		}
	}

	private void TalksNewYear(League league)
	{
		for (int i = Talks.Count - 1; i >= 0; i--)
		{
			ContractTalk k = Talks[i];
			Team t = GetTeam(k.TeamId);
			if (t == null || t.Sport == league.Sport)
			{
				if (t != null && k.Extension && k.Status == 2)
				{
					k.Extension = false;
					k.Message = "Он прекратил переговоры о продлении контракта. В это межсезонье он не подпишет с вами контракт.";
				}
				else
				{
					Talks.RemoveAt(i);
				}
			}
		}
	}

	public List<BlockEntry> TradeBlockOf(Team t)
	{
		List<BlockEntry> list = new List<BlockEntry>();
		if (t == null || t.Level != Level.Pro)
		{
			return list;
		}
		if (t.IsUser && t.Manual)
		{
			foreach (int id in t.Block)
			{
				Player p = GetPlayer(id);
				if (p != null && p.TeamId == t.Id && p.Status != PlayerStatus.FreeAgent)
				{
					list.Add(new BlockEntry
					{
						Player = p,
						Team = t,
						Reason = "В вашем списке обмена"
					});
				}
			}
			return list;
		}
		int year = CurrentYear();
		int baseOvr = Rosters.Base(Level.Pro);
		bool contending = IsContending(t);
		List<Player> signed = SignedPlayers(t.Id);
		signed.Sort((Player a, Player b) => (b.Overall == a.Overall) ? a.Id.CompareTo(b.Id) : b.Overall.CompareTo(a.Overall));
		HashSet<int> core = new HashSet<int>();
		for (int i = 0; i < signed.Count && i < 2; i++)
		{
			core.Add(signed[i].Id);
		}
		int limit = Economy.Limit(t);
		int payroll = Economy.Payroll(signed) + Economy.DeadNow(t);
		Action<Player, string> add = (Player player, string why) =>
		{
			if (!core.Contains(player.Id) && !list.Exists((BlockEntry e) => e.Player == player))
			{
				list.Add(new BlockEntry
				{
					Player = player,
					Team = t,
					Reason = why
				});
			}
		};
		foreach (Player p2 in signed)
		{
			int age = year - p2.BirthYear;
			bool starter = p2.Overall >= Weakest(signed, p2.Position, Math.Max(1, SlotStarters(t.Sport, p2.Position)));
			if (!contending && age >= 29 && p2.Overall >= baseOvr - 1)
			{
				add(p2, "Перестройка состава: готовы обменивать ветеранов");
			}
			else if (contending && age <= 24 && !starter && p2.Potential >= baseOvr)
			{
				add(p2, "Борьба за титул: готовы обменять молодых запасных ради усиления сейчас");
			}
			else if (payroll > limit * 95 / 100 && age >= 27 && p2.Salary >= limit / 15)
			{
				add(p2, "Ограничены потолком зарплат: готовы обменять крупный контракт");
			}
		}
		RosterSlot[] array = Rosters.Template(t.Sport, Level.Pro);
		foreach (RosterSlot slot in array)
		{
			List<Player> atPos = signed.FindAll((Player player) => player.Position == slot.Pos);
			if (atPos.Count > slot.Starters && atPos[slot.Starters].Overall >= baseOvr - 1)
			{
				add(atPos[slot.Starters], "Сильных игроков больше, чем " + Rosters.PositionLabel(slot.Pos) + " мест в составе");
			}
		}
		list.Sort((BlockEntry a, BlockEntry b) => (b.Player.Overall == a.Player.Overall) ? a.Player.Id.CompareTo(b.Player.Id) : b.Player.Overall.CompareTo(a.Player.Overall));
		if (list.Count > 4)
		{
			list.RemoveRange(4, list.Count - 4);
		}
		return list;
	}

	public CommandResult ToggleBlock(int playerId, int now)
	{
		_clockHint = now;
		Player p = GetPlayer(playerId);
		if (p == null)
		{
			return CommandResult.Fail("Игрок не найден.");
		}
		Team t = UserProTeam(p.TeamId, out var error);
		if (t == null)
		{
			return CommandResult.Fail(error);
		}
		if (!t.Manual)
		{
			return CommandResult.Fail("Сначала выберите «Самостоятельное управление командой» в офисе клуба, чтобы искать игроков.");
		}
		Revision++;
		bool soccer = t.Sport == Sport.Soccer;
		if (t.Block.Remove(p.Id))
		{
			return CommandResult.Success(p.Name + (soccer ? " убран из списка трансферов." : " убран из списка обмена."));
		}
		t.Block.Add(p.Id);
		return CommandResult.Success(soccer ? (p.Name + " выставлен на трансфер. Заинтересованные клубы будут присылать предложения каждый месяц трансферного окна.") : (p.Name + " выставлен на обмен. Заинтересованные команды будут звонить с предложениями раз в месяц, пока открыто окно обменов."));
	}

	public List<TradeOffer> FindOffers(Team mine, List<Player> shop, string want, int now)
	{
		List<TradeOffer> offers = new List<TradeOffer>();
		League league = LeagueOf(mine);
		if (league == null || shop == null || shop.Count == 0 || shop.Count > 3 || league.Sport == Sport.Soccer)
		{
			return offers;
		}
		EnsurePicks(league);
		int year = CurrentYear();
		int baseOvr = Rosters.Base(Level.Pro);
		bool mineContending = IsContending(mine);
		double myShop = 0.0;
		foreach (Player p in shop)
		{
			myShop += PlayerValue(p, mineContending, year);
		}
		List<KeyValuePair<TradeOffer, double>> scored = new List<KeyValuePair<TradeOffer, double>>();
		foreach (Team other in LeagueTeams(league))
		{
			if (other == mine || (other.IsUser && other.Manual))
			{
				continue;
			}
			bool contending = IsContending(other);
			double value = 0.0;
			foreach (Player p2 in shop)
			{
				value += PlayerValue(p2, contending, year);
			}
			double budget = value / (AreRivals(league, mine.Id, other.Id) ? 1.15 : 1.05);
			List<Player> signed = SignedPlayers(other.Id);
			signed.Sort((Player p7, Player p6) => PlayerValue(p6, contending, year).CompareTo(PlayerValue(p7, contending, year)));
			List<FinderAsset> assets = new List<FinderAsset>();
			for (int i = 2; i < signed.Count; i++)
			{
				Player p3 = signed[i];
				int age = year - p3.BirthYear;
				if (!(want == "picks") && (!(want == "now") || p3.Overall >= baseOvr - 3) && (!(want == "young") || age <= 24 || p3.Status == PlayerStatus.Prospect))
				{
					assets.Add(new FinderAsset
					{
						Player = p3,
						Theirs = PlayerValue(p3, contending, year),
						Mine = PlayerValue(p3, mineContending, year)
					});
				}
			}
			if (want != "now")
			{
				foreach (PickAsset a in TradeablePicks(league, other.Id))
				{
					assets.Add(new FinderAsset
					{
						Pick = a,
						Theirs = PickValue(league, a, contending),
						Mine = PickValue(league, a, mineContending)
					});
				}
			}
			assets.RemoveAll((FinderAsset finderAsset) => finderAsset.Theirs > budget);
			List<FinderAsset> players = assets.FindAll((FinderAsset finderAsset) => finderAsset.Player != null);
			List<FinderAsset> pickList = assets.FindAll((FinderAsset finderAsset) => finderAsset.Player == null);
			players.Sort((FinderAsset finderAsset2, FinderAsset finderAsset) => finderAsset.Mine.CompareTo(finderAsset2.Mine));
			pickList.Sort((FinderAsset finderAsset2, FinderAsset finderAsset) => finderAsset.Mine.CompareTo(finderAsset2.Mine));
			int maxPicks = ((want == "picks") ? 10 : 5);
			if (players.Count > 9)
			{
				players.RemoveRange(9, players.Count - 9);
			}
			if (pickList.Count > maxPicks)
			{
				pickList.RemoveRange(maxPicks, pickList.Count - maxPicks);
			}
			assets = players;
			assets.AddRange(pickList);
			assets.Sort((FinderAsset finderAsset2, FinderAsset finderAsset) => finderAsset.Mine.CompareTo(finderAsset2.Mine));
			List<List<FinderAsset>> combos = new List<List<FinderAsset>>();
			for (int a2 = 0; a2 < assets.Count; a2++)
			{
				combos.Add(new List<FinderAsset> { assets[a2] });
				for (int b = a2 + 1; b < assets.Count; b++)
				{
					combos.Add(new List<FinderAsset>
					{
						assets[a2],
						assets[b]
					});
					for (int c = b + 1; c < assets.Count; c++)
					{
						combos.Add(new List<FinderAsset>
						{
							assets[a2],
							assets[b],
							assets[c]
						});
					}
				}
			}
			List<KeyValuePair<List<FinderAsset>, double>> fair = new List<KeyValuePair<List<FinderAsset>, double>>();
			List<KeyValuePair<List<FinderAsset>, double>> shortOf = new List<KeyValuePair<List<FinderAsset>, double>>();
			foreach (List<FinderAsset> combo in combos)
			{
				double theirs = 0.0;
				double worth = 0.0;
				bool anyPlayer = false;
				foreach (FinderAsset x in combo)
				{
					theirs += x.Theirs;
					worth += x.Mine;
					if (x.Player != null)
					{
						anyPlayer = true;
					}
				}
				if (!(theirs > budget) && (!(want == "young") || anyPlayer))
				{
					if (worth >= myShop * 0.95)
					{
						fair.Add(new KeyValuePair<List<FinderAsset>, double>(combo, 0.0 - theirs + _random.NextDouble() * myShop * 0.12));
					}
					else if (worth >= myShop * 0.5)
					{
						shortOf.Add(new KeyValuePair<List<FinderAsset>, double>(combo, worth - (double)(combo.Count - 1) * 0.1));
					}
				}
			}
			fair.Sort((KeyValuePair<List<FinderAsset>, double> keyValuePair, KeyValuePair<List<FinderAsset>, double> y) => y.Value.CompareTo(keyValuePair.Value));
			shortOf.Sort((KeyValuePair<List<FinderAsset>, double> keyValuePair, KeyValuePair<List<FinderAsset>, double> y) => y.Value.CompareTo(keyValuePair.Value));
			List<KeyValuePair<List<FinderAsset>, double>> list = new List<KeyValuePair<List<FinderAsset>, double>>(fair);
			list.AddRange(shortOf);
			foreach (KeyValuePair<List<FinderAsset>, double> kv in list)
			{
				List<Player> give = new List<Player>();
				List<PickAsset> picks = new List<PickAsset>();
				foreach (FinderAsset x2 in kv.Key)
				{
					if (x2.Player != null)
					{
						give.Add(x2.Player);
					}
					else
					{
						picks.Add(x2.Pick);
					}
				}
				if (!CapLegal(other, give, shop) || !CapLegal(mine, shop, give))
				{
					continue;
				}
				TradeOffer o = new TradeOffer
				{
					FromTeamId = other.Id,
					ToTeamId = mine.Id,
					Month = GameClock.MonthIndex(now)
				};
				foreach (Player p4 in give)
				{
					o.Give.Add(p4.Id);
				}
				foreach (PickAsset a3 in picks)
				{
					o.GivePicks.Add(a3.Id);
				}
				foreach (Player p5 in shop)
				{
					o.Get.Add(p5.Id);
				}
				double worth2 = 0.0;
				foreach (FinderAsset x3 in kv.Key)
				{
					worth2 += x3.Mine;
				}
				scored.Add(new KeyValuePair<TradeOffer, double>(o, Math.Min(worth2, myShop * 1.25)));
				break;
			}
		}
		scored.Sort((KeyValuePair<TradeOffer, double> keyValuePair, KeyValuePair<TradeOffer, double> y) => y.Value.CompareTo(keyValuePair.Value));
		foreach (KeyValuePair<TradeOffer, double> item in scored)
		{
			offers.Add(item.Key);
		}
		return offers;
	}

	private void BlockOffers(League league, int now)
	{
		int month = GameClock.MonthIndex(now);
		Offers.RemoveAll((TradeOffer tradeOffer) => tradeOffer.Month < month - 1 || GetTeam(tradeOffer.ToTeamId) == null || GetTeam(tradeOffer.FromTeamId) == null);
		if (!TradeWindowOpen(league, now))
		{
			Offers.RemoveAll((TradeOffer tradeOffer) =>
			{
				Team team = GetTeam(tradeOffer.ToTeamId);
				return team != null && team.Sport == league.Sport;
			});
			return;
		}
		foreach (Team mine in LeagueTeams(league))
		{
			if (!mine.IsUser || !mine.Manual)
			{
				continue;
			}
			mine.Block.RemoveAll((int id2) =>
			{
				Player player = GetPlayer(id2);
				return player == null || player.TeamId != mine.Id;
			});
			if (mine.Block.Count == 0)
			{
				continue;
			}
			Offers.RemoveAll((TradeOffer tradeOffer) => tradeOffer.ToTeamId == mine.Id);
			int made = 0;
			foreach (int id in mine.Block)
			{
				Player p = GetPlayer(id);
				if (p == null)
				{
					continue;
				}
				List<TradeOffer> list = FindOffers(mine, new List<Player> { p }, "any", now);
				int calls = 0;
				foreach (TradeOffer o in list)
				{
					if (calls >= 2 || made >= 5)
					{
						break;
					}
					if (!(_random.NextDouble() < 0.35))
					{
						o.Id = NextId++;
						Offers.Add(o);
						calls++;
						made++;
					}
				}
			}
			if (made > 0)
			{
				PostAsTeam(league, mine, NewsKind.Trade, "ТЕЛЕФОНЫ РАЗРЫВАЮТСЯ: " + made + ((made == 1) ? " команда позвонила" : " команд позвонили") + " по поводу игроков из нашего списка обмена.", now);
			}
		}
	}

	public List<TradeOffer> OffersFor(int teamId)
	{
		return Offers.FindAll((TradeOffer o) => o.ToTeamId == teamId);
	}

	public CommandResult AcceptOffer(int offerId, int now)
	{
		TradeOffer o = Offers.Find((TradeOffer x) => x.Id == offerId);
		if (o == null)
		{
			return CommandResult.Fail("Это предложение больше не действует.");
		}
		Offers.Remove(o);
		Revision++;
		CommandResult r = ProposeTrade(o.ToTeamId, o.FromTeamId, o.Get, o.Give, new List<int>(), o.GivePicks, now);
		if (r.Ok)
		{
			Team mine = GetTeam(o.ToTeamId);
			if (mine != null)
			{
				foreach (int id in o.Get)
				{
					mine.Block.Remove(id);
				}
			}
			Offers.RemoveAll((TradeOffer x) => x.ToTeamId == o.ToTeamId && x.Get.Exists((int item) => o.Get.Contains(item)));
		}
		return r;
	}

	public CommandResult DeclineOffer(int offerId)
	{
		int num = Offers.RemoveAll((TradeOffer x) => x.Id == offerId);
		Revision++;
		if (num <= 0)
		{
			return CommandResult.Fail("Это предложение больше не действует.");
		}
		return CommandResult.Success("Предложение отклонено.");
	}

	private static double ContendValue(Player p)
	{
		return Math.Exp((double)(p.Overall - Rosters.Base(Level.Pro)) / 5.0);
	}

	private static double RebuildValue(Player p, int year)
	{
		int age = year - p.BirthYear;
		Rosters.PeakAges(p.Sport, p.Position, out var _, out var end);
		int num = ((age < end) ? Math.Max(p.Overall, p.Potential) : p.Overall);
		double youth;
		if (age <= 23)
		{
			youth = 1.4;
		}
		else if (age <= 26)
		{
			youth = 1.1;
		}
		else if (age <= 29)
		{
			youth = 0.8;
		}
		else
		{
			youth = ((age <= 32) ? 0.5 : 0.3);
		}
		return Math.Exp((double)(num - Rosters.Base(Level.Pro)) / 5.0) * youth;
	}

	private double PlayerValue(Player p, bool contending, int year)
	{
		if (!contending)
		{
			return RebuildValue(p, year);
		}
		return ContendValue(p);
	}

	public int TradePoints(Team viewer, Player p)
	{
		return Points(PlayerValue(p, viewer != null && IsContending(viewer), CurrentYear()));
	}

	public int TradePoints(Team viewer, PickAsset a)
	{
		League league = ((viewer == null) ? null : LeagueOf(viewer));
		if (league != null)
		{
			return Points(PickValue(league, a, IsContending(viewer)));
		}
		return 0;
	}

	private static int Points(double value)
	{
		return Math.Max(1, (int)Math.Round(value * 10.0));
	}

	public int NextDraftYear(League league)
	{
		if (league.Draft != null && league.Draft.Started && !league.Draft.Complete)
		{
			return league.Draft.Year;
		}
		foreach (Season s in SeasonsOldestFirst(league))
		{
			if (s.Year > league.DraftDoneFor)
			{
				return s.Year;
			}
		}
		if (league.DraftDoneFor != int.MinValue)
		{
			return league.DraftDoneFor + 1;
		}
		return CurrentYear();
	}

	public void EnsurePicks(League league)
	{
		if (league.Level != Level.Pro)
		{
			return;
		}
		int y0 = NextDraftYear(league);
		int rounds = Economy.Get(league.Sport).DraftRounds;
		HashSet<int> teams = new HashSet<int>();
		foreach (Team t in LeagueTeams(league))
		{
			teams.Add(t.Id);
		}
		league.Picks.RemoveAll((PickAsset pickAsset) => pickAsset.Year < y0 || pickAsset.Year > y0 + 2 || !teams.Contains(pickAsset.OriginalTeamId) || pickAsset.Round > rounds);
		foreach (PickAsset a in league.Picks)
		{
			if (!teams.Contains(a.OwnerTeamId))
			{
				a.OwnerTeamId = a.OriginalTeamId;
			}
		}
		HashSet<string> have = new HashSet<string>();
		foreach (PickAsset a2 in league.Picks)
		{
			have.Add(a2.Year + "/" + a2.Round + "/" + a2.OriginalTeamId);
		}
		foreach (int id in teams)
		{
			for (int y1 = y0; y1 <= y0 + 2; y1++)
			{
				for (int r = 1; r <= rounds; r++)
				{
					if (!have.Contains(y1 + "/" + r + "/" + id))
					{
						league.Picks.Add(new PickAsset
						{
							Id = NextId++,
							Year = y1,
							Round = r,
							OriginalTeamId = id,
							OwnerTeamId = id
						});
					}
				}
			}
		}
		league.Picks.Sort((PickAsset pickAsset, PickAsset b) =>
		{
			int result = ((pickAsset.Year == b.Year) ? ((pickAsset.Round == b.Round) ? pickAsset.OriginalTeamId.CompareTo(b.OriginalTeamId) : pickAsset.Round.CompareTo(b.Round)) : pickAsset.Year.CompareTo(b.Year));
			return result;
		});
	}

	public List<PickAsset> TradeablePicks(League league, int teamId)
	{
		EnsurePicks(league);
		int locked = ((league.Draft != null && league.Draft.Started && !league.Draft.Complete) ? league.Draft.Year : int.MinValue);
		List<PickAsset> list = new List<PickAsset>();
		foreach (PickAsset a in league.Picks)
		{
			if (a.OwnerTeamId == teamId && a.Year != locked)
			{
				list.Add(a);
			}
		}
		return list;
	}

	public PickAsset FindPick(League league, int id)
	{
		foreach (PickAsset a in league.Picks)
		{
			if (a.Id == id)
			{
				return a;
			}
		}
		return null;
	}

	public double ExpectedSlot(League league, int originalTeamId, int year)
	{
		int n = Math.Max(2, LeagueTeams(league).Count);
		if (league.Draft != null && league.Draft.Year == year && league.Draft.Order.Count > 0)
		{
			int i = league.Draft.Order.IndexOf(originalTeamId);
			if (i >= 0 && league.Draft.Started)
			{
				return i + 1;
			}
		}
		int idx = ContentionOrder(league).IndexOf(originalTeamId);
		double mid = (double)(n + 1) / 2.0;
		double slot = ((idx < 0) ? mid : ((double)(n - idx)));
		int ahead = year - NextDraftYear(league);
		double weight;
		if (ahead <= 0)
		{
			weight = 0.85;
		}
		else
		{
			weight = ((ahead == 1) ? 0.45 : 0.2);
		}
		return mid + (slot - mid) * weight;
	}

	public double PickValue(League league, PickAsset a, bool contending)
	{
		int n = Math.Max(2, LeagueTeams(league).Count);
		int rounds = Economy.Get(league.Sport).DraftRounds;
		double overall = (double)((a.Round - 1) * n) + ExpectedSlot(league, a.OriginalTeamId, a.Year);
		double total = Math.Max(2, n * rounds);
		double value = Math.Exp((11.0 - 15.0 * (overall - 1.0) / (total - 1.0)) / 5.0) * 1.4;
		int ahead = Math.Max(0, a.Year - NextDraftYear(league));
		value *= Math.Pow(0.85, ahead);
		if (!contending)
		{
			return value;
		}
		return value * 0.4;
	}

	public string PickLabel(League league, PickAsset a)
	{
		string round;
		if (a.Round == 1)
		{
			round = "1-й";
		}
		else if (a.Round == 2)
		{
			round = "2-й";
		}
		else
		{
			round = ((a.Round == 3) ? "3-й" : (a.Round + "-й"));
		}
		string label = DraftYearLabel(league, a.Year) + " выбор " + round + " раунда";
		if (a.OriginalTeamId != a.OwnerTeamId)
		{
			label = label + " (от " + TeamShortName(a.OriginalTeamId) + ")";
		}
		return label;
	}

	public string PickProjection(League league, PickAsset a)
	{
		int n = Math.Max(2, LeagueTeams(league).Count);
		int slot = (int)Math.Round(ExpectedSlot(league, a.OriginalTeamId, a.Year));
		slot = Math.Max(1, Math.Min(n, slot));
		int overall = (a.Round - 1) * n + slot;
		return ((league.Draft != null && league.Draft.Year == a.Year && league.Draft.Started) ? "Выбор № " : "Прогнозируемый номер выбора: ") + overall;
	}

	private bool CapLegal(Team team, List<Player> outgoing, List<Player> incoming)
	{
		EconomyProfile economyProfile = Economy.Get(team.Sport);
		List<Player> after = SignedPlayers(team.Id).FindAll((Player p) => !outgoing.Contains(p));
		after.AddRange(incoming);
		int dead = Economy.DeadNow(team);
		int payroll = Economy.Payroll(after) + dead;
		int inSal = Economy.Payroll(incoming);
		int outSal = Economy.Payroll(outgoing);
		int cap = Economy.Cap(team.Sport);
		switch (economyProfile.Type)
		{
		case CapType.Soft:
			if (payroll > cap)
			{
				return inSal <= outSal * 5 / 4 + 250;
			}
			return true;
		case CapType.Budget:
			if (payroll > Economy.Limit(team) * 105 / 100)
			{
				return inSal <= outSal;
			}
			return true;
		case CapType.Mls:
			if (Economy.DesignatedPlayers(team.Sport, after) <= 3)
			{
				if (Economy.CapHit(team.Sport, after) + dead > cap)
				{
					return Economy.CapHit(team.Sport, incoming) <= Economy.CapHit(team.Sport, outgoing);
				}
				return true;
			}
			return false;
		default:
			if (payroll > cap)
			{
				return inSal <= outSal;
			}
			return true;
		}
	}

	private List<int> ContentionOrder(League league)
	{
		List<int> order = new List<int>();
		Season s = league.Current;
		bool started = false;
		if (s != null)
		{
			foreach (Match g in s.Games)
			{
				if (g.Played && g.Stage == GameStage.Regular)
				{
					started = true;
					break;
				}
			}
		}
		if (started)
		{
			foreach (StandingRow r in Standings.Compute(league, s))
			{
				if (league.TeamIds.Contains(r.TeamId))
				{
					order.Add(r.TeamId);
				}
			}
		}
		List<Team> list = LeagueTeams(league);
		list.Sort((Team a, Team b) => b.Rating.CompareTo(a.Rating));
		foreach (Team t in list)
		{
			if (!order.Contains(t.Id))
			{
				order.Add(t.Id);
			}
		}
		return order;
	}

	private void TryAiTrade(League league, int now)
	{
		List<Team> teams = LeagueTeams(league).FindAll((Team team) => !team.Manual);
		if (teams.Count < 2)
		{
			return;
		}
		EnsurePicks(league);
		int year = CurrentYear();
		List<int> order = ContentionOrder(league);
		int half = Math.Max(1, order.Count / 2);
		List<Team> buyers = teams.FindAll((Team team) => order.IndexOf(team.Id) >= 0 && order.IndexOf(team.Id) < half);
		List<Team> sellers = teams.FindAll((Team team) => order.IndexOf(team.Id) >= half);
		if (buyers.Count == 0 || sellers.Count == 0)
		{
			return;
		}
		Team buyer = buyers[_random.Next(buyers.Count)];
		Team seller = sellers[_random.Next(sellers.Count)];
		List<Player> buyerSigned = SignedPlayers(buyer.Id);
		Player target = null;
		int bestGain = 2;
		foreach (Player p in RosterOf(seller.Id))
		{
			if (year - p.BirthYear >= 26)
			{
				int gain = p.Overall - Weakest(buyerSigned, p.Position, Math.Max(1, SlotStarters(league.Sport, p.Position)));
				if (gain > bestGain)
				{
					bestGain = gain;
					target = p;
				}
			}
		}
		if (target == null)
		{
			return;
		}
		List<Player> get = new List<Player> { target };
		double needSeller = RebuildValue(target, year);
		double limitBuyer = ContendValue(target) / 1.05;
		if (_random.NextDouble() < 0.3)
		{
			List<Player> extras = RosterOf(seller.Id).FindAll((Player player) => player != target && year - player.BirthYear >= 24 && RebuildValue(player, year) < needSeller * 0.4);
			if (extras.Count > 0)
			{
				Player extra = extras[_random.Next(extras.Count)];
				get.Add(extra);
				needSeller += RebuildValue(extra, year);
				limitBuyer += ContendValue(extra) / 1.05;
			}
		}
		bool picksOnly = _random.NextDouble() < 0.18;
		List<TradeItem> assets = new List<TradeItem>();
		if (!picksOnly)
		{
			foreach (Player p2 in buyerSigned)
			{
				bool young = year - p2.BirthYear <= 25;
				if ((p2.Position == target.Position || young) && (p2.Status != PlayerStatus.Active || young || Weakest(buyerSigned, p2.Position, Math.Max(1, SlotStarters(league.Sport, p2.Position))) > p2.Overall))
				{
					assets.Add(new TradeItem
					{
						Player = p2,
						SellerValue = RebuildValue(p2, year),
						BuyerValue = ContendValue(p2)
					});
				}
			}
		}
		foreach (PickAsset a in TradeablePicks(league, buyer.Id))
		{
			assets.Add(new TradeItem
			{
				Pick = a,
				SellerValue = PickValue(league, a, contending: false),
				BuyerValue = PickValue(league, a, contending: true)
			});
		}
		assets.Sort((TradeItem tradeItem, TradeItem b) => (b.SellerValue / Math.Max(0.01, b.BuyerValue)).CompareTo(tradeItem.SellerValue / Math.Max(0.01, tradeItem.BuyerValue)));
		List<TradeItem> playerItems = assets.FindAll((TradeItem x) => x.Player != null);
		List<TradeItem> pickItems = assets.FindAll((TradeItem x) => x.Player == null);
		if (playerItems.Count > 6)
		{
			playerItems.RemoveRange(6, playerItems.Count - 6);
		}
		int pickCap = (picksOnly ? 10 : 4);
		if (pickItems.Count > pickCap)
		{
			pickItems.RemoveRange(pickCap, pickItems.Count - pickCap);
		}
		assets = playerItems;
		assets.AddRange(pickItems);
		double roll = _random.NextDouble();
		int wanted = ((roll < 0.25) ? 1 : ((roll < 0.6) ? 2 : ((roll < 0.87) ? 3 : 4)));
		List<TradeItem> give = null;
		double bestCost = double.MaxValue;
		int n = assets.Count;
		List<TradeItem> combo = new List<TradeItem>();
		Action<int> search = null;
		search = (int startAt) =>
		{
			if (combo.Count > 0)
			{
				double num = 0.0;
				double num2 = 0.0;
				foreach (TradeItem current in combo)
				{
					num += current.SellerValue;
					num2 += current.BuyerValue;
				}
				if (num >= needSeller && num2 <= limitBuyer)
				{
					double num3 = (double)(Math.Abs(combo.Count - wanted) * 1000) + num2 + _random.NextDouble() * 0.05;
					if (num3 < bestCost)
					{
						bestCost = num3;
						give = new List<TradeItem>(combo);
					}
				}
			}
			if (combo.Count < 4)
			{
				for (int i = startAt; i < n; i++)
				{
					combo.Add(assets[i]);
					search(i + 1);
					combo.RemoveAt(combo.Count - 1);
				}
			}
		};
		search(0);
		if (give == null)
		{
			return;
		}
		List<Player> givePlayers = new List<Player>();
		List<PickAsset> givePicks = new List<PickAsset>();
		foreach (TradeItem t in give)
		{
			if (t.Player != null)
			{
				givePlayers.Add(t.Player);
			}
			else
			{
				givePicks.Add(t.Pick);
			}
		}
		if (CapLegal(buyer, givePlayers, get) && CapLegal(seller, get, givePlayers))
		{
			ExecuteTrade(league, buyer, seller, givePlayers, get, givePicks, new List<PickAsset>(), now);
		}
	}

	private void ExecuteTrade(League league, Team a, Team b, List<Player> fromA, List<Player> fromB, List<PickAsset> picksFromA, List<PickAsset> picksFromB, int now)
	{
		foreach (Player p in fromA)
		{
			p.LastTeamId = a.Id;
			p.TeamId = b.Id;
			p.Number = NumberFor(b, p.Position);
		}
		foreach (Player p2 in fromB)
		{
			p2.LastTeamId = b.Id;
			p2.TeamId = a.Id;
			p2.Number = NumberFor(a, p2.Position);
		}
		foreach (PickAsset item in picksFromA)
		{
			item.OwnerTeamId = b.Id;
		}
		foreach (PickAsset item2 in picksFromB)
		{
			item2.OwnerTeamId = a.Id;
		}
		string aGets = AssetList(league, fromB, picksFromB);
		string bGets = AssetList(league, fromA, picksFromA);
		RostersChanged();
		int year = CurrentYear();
		Team[] array = new Team[2] { a, b };
		foreach (Team t in array)
		{
			ArrangeRoster(t, now);
			FillHoles(t, year, now);
			UpdateRating(t);
		}
		Post(league, NewsKind.Trade, Insider, "ОБМЕН: " + TeamName(a.Id) + " получает " + aGets + " от клуба " + TeamName(b.Id) + " в обмен на " + bGets + ".", a.IsUser || b.IsUser, now, 3);
		if (a.IsUser)
		{
			PostAsTeam(league, a, NewsKind.Trade, "Мы получили " + aGets + " из " + TeamName(b.Id) + ".", now);
		}
		if (b.IsUser)
		{
			PostAsTeam(league, b, NewsKind.Trade, "Мы получили " + bGets + " из " + TeamName(a.Id) + ".", now);
		}
		Revision++;
	}

	private static string PlayerList(List<Player> players)
	{
		List<string> names = new List<string>();
		foreach (Player p in players)
		{
			names.Add(Rosters.NewsPositionLabel(p.Position, p.Sport) + " " + p.Name + " (" + Rosters.Ovr(p) + ")");
		}
		return JoinAnd(names);
	}

	private string AssetList(League league, List<Player> players, List<PickAsset> picks)
	{
		List<string> names = new List<string>();
		foreach (Player p in players)
		{
			names.Add(Rosters.NewsPositionLabel(p.Position, p.Sport) + " " + p.Name + " (" + Rosters.Ovr(p) + ")");
		}
		foreach (PickAsset k in picks)
		{
			names.Add("выбор на драфте " + PickLabel(league, k));
		}
		return JoinAnd(names);
	}

	private static string JoinAnd(List<string> names)
	{
		if (names.Count <= 1)
		{
			if (names.Count != 0)
			{
				return names[0];
			}
			return "ничего";
		}
		return string.Join(", ", names.GetRange(0, names.Count - 1).ToArray()) + " и " + names[names.Count - 1];
	}

	public TradeCheck CheckTrade(int teamId, int otherTeamId, List<int> give, List<int> get, List<int> givePicks, List<int> getPicks, int now)
	{
		Team mine;
		Team other;
		League league;
		List<Player> outgoing;
		List<Player> incoming;
		List<PickAsset> picksOut;
		List<PickAsset> picksIn;
		return Check(teamId, otherTeamId, give, get, givePicks, getPicks, now, out mine, out other, out league, out outgoing, out incoming, out picksOut, out picksIn);
	}

	private TradeCheck Check(int teamId, int otherTeamId, List<int> give, List<int> get, List<int> givePicks, List<int> getPicks, int now, out Team mine, out Team other, out League league, out List<Player> outgoing, out List<Player> incoming, out List<PickAsset> picksOut, out List<PickAsset> picksIn)
	{
		_clockHint = now;
		TradeCheck c = new TradeCheck();
		outgoing = new List<Player>();
		incoming = new List<Player>();
		picksOut = new List<PickAsset>();
		picksIn = new List<PickAsset>();
		other = null;
		league = null;
		mine = UserProTeam(teamId, out var error);
		if (mine == null)
		{
			c.Message = error;
			return c;
		}
		if (!mine.Manual)
		{
			c.Message = "Сначала выберите «Самостоятельное управление командой» в офисе клуба, чтобы проводить обмены.";
			return c;
		}
		other = GetTeam(otherTeamId);
		league = LeagueOf(mine);
		if (other == null || other.Retired || other == mine || LeagueOf(other) != league || !league.TeamIds.Contains(other.Id))
		{
			c.Message = "Выберите команду из вашей лиги для обмена.";
			return c;
		}
		if (other.IsUser && other.Manual)
		{
			c.Message = "Вы управляете обеими командами. Обмены между собственными командами запрещены.";
			return c;
		}
		if (league.Sport == Sport.Soccer)
		{
			c.Message = "Футбольные клубы не обмениваются игроками: покупайте и продавайте их на вкладке «Трансферы».";
			return c;
		}
		if (!TradeWindowOpen(league, now))
		{
			c.Message = "Окно обменов закрыто (дедлайн прошёл или идёт плей-офф). Оно откроется снова после окончания сезона.";
			return c;
		}
		foreach (int id in give ?? new List<int>())
		{
			Player p = GetPlayer(id);
			if (p == null || p.TeamId != mine.Id)
			{
				c.Message = "Один из предложенных вами игроков больше не входит в вашу команду.";
				return c;
			}
			if (!outgoing.Contains(p))
			{
				outgoing.Add(p);
			}
		}
		foreach (int id2 in get ?? new List<int>())
		{
			Player p2 = GetPlayer(id2);
			if (p2 == null || p2.TeamId != other.Id)
			{
				c.Message = "Один из запрошенных вами игроков больше не входит в их команду.";
				return c;
			}
			if (!incoming.Contains(p2))
			{
				incoming.Add(p2);
			}
		}
		List<PickAsset> mineTradeable = TradeablePicks(league, mine.Id);
		List<PickAsset> theirTradeable = TradeablePicks(league, other.Id);
		foreach (int id3 in givePicks ?? new List<int>())
		{
			PickAsset a = FindPick(league, id3);
			if (a == null || !mineTradeable.Contains(a))
			{
				c.Message = "Один из предложенных вами выборов на драфте нельзя обменять.";
				return c;
			}
			if (!picksOut.Contains(a))
			{
				picksOut.Add(a);
			}
		}
		foreach (int id4 in getPicks ?? new List<int>())
		{
			PickAsset a2 = FindPick(league, id4);
			if (a2 == null || !theirTradeable.Contains(a2))
			{
				c.Message = "Один из запрошенных вами выборов на драфте нельзя обменять.";
				return c;
			}
			if (!picksIn.Contains(a2))
			{
				picksIn.Add(a2);
			}
		}
		if (outgoing.Count + picksOut.Count == 0 || incoming.Count + picksIn.Count == 0)
		{
			c.Message = "Добавьте хотя бы одного игрока или выбор на драфте с каждой стороны.";
			return c;
		}
		if (outgoing.Count + picksOut.Count > 3 || incoming.Count + picksIn.Count > 3)
		{
			c.Message = "С каждой стороны обмена может быть не более трёх игроков или выборов на драфте.";
			return c;
		}
		int year = CurrentYear();
		bool contending = IsContending(other);
		double receive = 0.0;
		double send = 0.0;
		foreach (Player p3 in outgoing)
		{
			receive += PlayerValue(p3, contending, year);
		}
		foreach (PickAsset a3 in picksOut)
		{
			receive += PickValue(league, a3, contending);
		}
		foreach (Player p4 in incoming)
		{
			send += PlayerValue(p4, contending, year);
		}
		foreach (PickAsset a4 in picksIn)
		{
			send += PickValue(league, a4, contending);
		}
		c.Ratio = ((send <= 0.0) ? 9.99 : (receive / send));
		c.Offered = Points(receive);
		c.Asked = Points(send);
		string why = (contending ? "Они пытаются победить прямо сейчас, поэтому им нужны игроки, которые помогут немедленно." : "Они перестраивают команду, поэтому им нужны молодые игроки и выборы на драфте.");
		bool flag = AreRivals(league, mine.Id, other.Id);
		double needed = (c.Needed = (flag ? 1.15 : 1.05));
		if (flag)
		{
			why += " Кроме того, они не хотят помогать принципиальному сопернику.";
		}
		if (c.Ratio < needed)
		{
			c.Legal = true;
			c.Message = ((c.Ratio >= needed - 0.15) ? ("Почти договорились, но " + TeamName(other.Id) + " хотят немного больше. ") : ("" + TeamName(other.Id) + " отказывают: ценность обмена недостаточна. ")) + why;
			return c;
		}
		if (!CapLegal(mine, outgoing, incoming))
		{
			c.Message = "Этот обмен приведёт к превышению " + Economy.Get(mine.Sport).CapName.ToLowerInvariant() + ".";
			return c;
		}
		if (!CapLegal(other, incoming, outgoing))
		{
			c.Message = "" + TeamName(other.Id) + " не смогут вместить эти зарплаты в свой " + Economy.Get(other.Sport).CapName.ToLowerInvariant() + ".";
			return c;
		}
		c.Legal = true;
		c.Accept = true;
		c.Message = "" + TeamName(other.Id) + " согласны на этот обмен.";
		return c;
	}

	public CommandResult ProposeTrade(int teamId, int otherTeamId, List<int> give, List<int> get, List<int> givePicks, List<int> getPicks, int now)
	{
		TradeCheck c = Check(teamId, otherTeamId, give, get, givePicks, getPicks, now, out var mine, out var other, out var league, out var outgoing, out var incoming, out var picksOut, out var picksIn);
		if (!c.Accept)
		{
			return CommandResult.Fail("Обмен отклонён. " + c.Message);
		}
		string received = AssetList(league, incoming, picksIn);
		string sent = AssetList(league, outgoing, picksOut);
		ExecuteTrade(league, mine, other, outgoing, incoming, picksOut, picksIn, now);
		return CommandResult.Success("Обмен принят! Вы получаете " + received + " в обмен на " + sent + ".");
	}

	public CommandResult ProposeTrade(int teamId, int otherTeamId, List<int> give, List<int> get, int now)
	{
		return ProposeTrade(teamId, otherTeamId, give, get, null, null, now);
	}

	public void RecordCityWeather(int now, double tempC, double precipitation, double cloudiness, bool raining, bool snowing)
	{
		if (double.IsNaN(tempC) || double.IsInfinity(tempC))
		{
			return;
		}
		CityTempC = Math.Max(-50.0, Math.Min(50.0, tempC));
		CityPrecip = (double.IsNaN(precipitation) ? 0.0 : Math.Max(0.0, Math.Min(1.0, precipitation)));
		CityCloud = (double.IsNaN(cloudiness) ? 0.0 : Math.Max(0.0, Math.Min(1.0, cloudiness)));
		CityRaining = raining;
		CitySnowing = snowing;
		CitySampleTime = now;
		if (_lastCountedSample != int.MinValue && now >= _lastCountedSample && now - _lastCountedSample < 30)
		{
			return;
		}
		_lastCountedSample = now;
		int month = GameClock.MonthIndex(now);
		CityWeatherMonth m = CityWeather.Find((CityWeatherMonth x) => x.Month == month);
		if (m == null)
		{
			m = new CityWeatherMonth
			{
				Month = month
			};
			CityWeather.Add(m);
			CityWeather.Sort((CityWeatherMonth a, CityWeatherMonth b) => a.Month.CompareTo(b.Month));
			while (CityWeather.Count > 3)
			{
				CityWeather.RemoveAt(0);
			}
		}
		m.Samples++;
		m.SumTempC += CityTempC;
		int num;
		if (!(raining | snowing))
		{
			num = ((CityPrecip > 0.15) ? 1 : 0);
			if (num == 0)
			{
				goto IL_01c1;
			}
		}
		else
		{
			num = 1;
		}
		m.Wet++;
		goto IL_01c1;
		IL_01c1:
		if (snowing)
		{
			m.Snow++;
		}
		if (num != 0 && CityPrecip > 0.6)
		{
			m.Heavy++;
		}
	}

	private static uint Hash(int x)
	{
		int num = x * -1640531535;
		return (uint)((num ^ (num >>> 15)) * -2048144777);
	}

	public static bool OutdoorSport(Sport sport)
	{
		if (sport != Sport.Football && sport != Sport.Baseball)
		{
			return sport == Sport.Soccer;
		}
		return true;
	}

	private bool UnderDome(League league, Match g)
	{
		if (league.Level != Level.Pro)
		{
			return false;
		}
		if (g.VenueId == -2)
		{
			return league.Sport == Sport.Football;
		}
		if (g.VenueId >= 0)
		{
			return false;
		}
		uint h = Hash(g.HomeTeamId);
		if (league.Sport != Sport.Football)
		{
			if (league.Sport != Sport.Baseball)
			{
				return false;
			}
			return h % 6 == 0;
		}
		return h % 4 == 0;
	}

	private int DecideWeather(League league, Match g, int now, bool live)
	{
		if (!OutdoorSport(league.Sport))
		{
			return -1;
		}
		if (g.VenueId >= 0 && !IsOutdoorVenue(g.VenueId))
		{
			return -1;
		}
		if (UnderDome(league, g))
		{
			return WeatherCodes.Encode(WeatherKind.Dome, 72);
		}
		int day = GameClock.DayOf(g.Time);
		int minute = GameClock.MinuteOfDay(g.Time);
		bool inCity = g.VenueId >= 0;
		if ((live & inCity) && CitySampleTime != int.MinValue && GameClock.MonthIndex(CitySampleTime) == GameClock.MonthIndex(now) && Math.Abs(now - CitySampleTime) <= 240)
		{
			return CityCode();
		}
		Random r = new Random((int)Hash(day * 7919 + (inCity ? g.VenueId : (50000 + g.HomeTeamId))));
		int monthOfYear = GameClock.DateOf(day).Month - 1;
		CityWeatherMonth cm = (inCity ? CityWeather.Find((CityWeatherMonth x) => x.Month == GameClock.MonthIndex(g.Time)) : null);
		double snowShare = -1.0;
		double heavyShare = 0.22;
		double mean;
		double wet;
		if (cm != null && cm.Samples >= 4)
		{
			mean = cm.MeanTempC;
			wet = Math.Max(0.02, Math.Min(0.7, cm.WetShare));
			if (cm.Wet > 0)
			{
				snowShare = (double)cm.Snow / (double)cm.Wet;
				heavyShare = Math.Max(0.1, Math.Min(0.6, (double)cm.Heavy / (double)cm.Wet));
			}
		}
		else
		{
			uint h = Hash(g.HomeTeamId + 777);
			double warmer = (inCity ? 0.0 : ((double)(h % 13) - 4.0));
			double wetter = (inCity ? 1.0 : (0.7 + (double)(h % 7) * 0.1));
			mean = NormalTempC[monthOfYear] + warmer;
			wet = NormalWet[monthOfYear] * wetter;
		}
		double temp = mean + Rosters.Normal(r, 0.0, 3.5) + ((minute >= 1080) ? (-2.0) : ((minute <= 810) ? (-0.5) : 1.0));
		WeatherKind k;
		if (r.NextDouble() >= wet)
		{
			k = ((r.NextDouble() < 0.35) ? WeatherKind.Cloudy : WeatherKind.Clear);
		}
		else
		{
			bool num;
			if (!(snowShare >= 0.0))
			{
				num = temp <= 0.5;
			}
			else
			{
				if (!(temp < 3.0))
				{
					goto IL_031f;
				}
				num = r.NextDouble() < snowShare;
			}
			if (!num)
			{
				goto IL_031f;
			}
			k = WeatherKind.Snow;
			temp = Math.Min(temp, 1.0);
		}
		goto IL_0363;
		IL_031f:
		double roll = r.NextDouble();
		if (temp > 18.0 && roll < 0.18)
		{
			k = WeatherKind.Storm;
		}
		else
		{
			k = ((roll < heavyShare) ? WeatherKind.HeavyRain : WeatherKind.Rain);
		}
		temp = Math.Max(temp, 1.5);
		goto IL_0363;
		IL_0363:
		return WeatherCodes.Encode(k, (int)Math.Round(temp * 9.0 / 5.0 + 32.0));
	}

	private int CityCode()
	{
		WeatherKind k;
		if (CitySnowing)
		{
			k = WeatherKind.Snow;
		}
		else if (!CityRaining && !(CityPrecip > 0.15))
		{
			k = ((CityCloud > 0.6) ? WeatherKind.Cloudy : WeatherKind.Clear);
		}
		else if (CityPrecip > 0.85 && CityTempC > 15.0)
		{
			k = WeatherKind.Storm;
		}
		else
		{
			k = ((CityPrecip > 0.55) ? WeatherKind.HeavyRain : WeatherKind.Rain);
		}
		return WeatherCodes.Encode(k, (int)Math.Round(CityTempC * 9.0 / 5.0 + 32.0));
	}

	private static void HoldUp(Sport sport, Match g, int code, out int delay, out bool callOff)
	{
		delay = 0;
		callOff = false;
		if (code < 0)
		{
			return;
		}
		WeatherKind k = WeatherCodes.Kind(code);
		Random random = new Random((int)Hash(g.Id * 92821 + GameClock.DayOf(g.Time)));
		double p = random.NextDouble();
		double q = random.NextDouble();
		switch (sport)
		{
		case Sport.Baseball:
			switch (k)
			{
			case WeatherKind.Rain:
				if (p < 0.06)
				{
					callOff = true;
				}
				else if (p < 0.45)
				{
					delay = 15 + (int)(q * 60.0);
				}
				break;
			case WeatherKind.HeavyRain:
				if (p < 0.4)
				{
					callOff = true;
				}
				else
				{
					delay = 40 + (int)(q * 90.0);
				}
				break;
			case WeatherKind.Storm:
				if (p < 0.45)
				{
					callOff = true;
				}
				else
				{
					delay = 30 + (int)(q * 80.0);
				}
				break;
			case WeatherKind.Snow:
				if (p < 0.85)
				{
					callOff = true;
				}
				else
				{
					delay = 30 + (int)(q * 40.0);
				}
				break;
			}
			break;
		case Sport.Football:
			if (k == WeatherKind.Storm)
			{
				if (p < 0.04)
				{
					callOff = true;
				}
				else if (p < 0.7)
				{
					delay = 30 + (int)(q * 60.0);
				}
			}
			break;
		case Sport.Soccer:
			switch (k)
			{
			case WeatherKind.Storm:
				if (p < 0.25)
				{
					callOff = true;
				}
				else if (p < 0.75)
				{
					delay = 30 + (int)(q * 45.0);
				}
				break;
			case WeatherKind.HeavyRain:
				if (p < 0.08)
				{
					callOff = true;
				}
				break;
			case WeatherKind.Snow:
				if (p < 0.12)
				{
					callOff = true;
				}
				break;
			}
			break;
		}
	}

	private bool WeatherStops(League league, Season s, PlayoffSeries ps, Match g, int now, bool live)
	{
		g.Weather = DecideWeather(league, g, now, live);
		g.RainDelay = 0;
		if (g.Weather < 0)
		{
			return false;
		}
		HoldUp(league.Sport, g, g.Weather, out var delay, out var callOff);
		if (callOff && ps == null && g.Stage == GameStage.Regular && g.PostponedFrom < 0 && MoveToMakeUp(league, s, g, now))
		{
			return true;
		}
		if (callOff)
		{
			delay = Math.Max(delay, 75);
		}
		g.RainDelay = delay;
		return false;
	}

	private static string CallOffReason(int code)
	{
		return WeatherCodes.Kind(code) switch
		{
			WeatherKind.Snow => "снег", 
			WeatherKind.Storm => "молния и гроза", 
			WeatherKind.HeavyRain => "сильный дождь", 
			_ => "дождь", 
		};
	}

	private bool MoveToMakeUp(League league, Season s, Match g, int now)
	{
		int from = GameClock.DayOf(g.Time);
		int first = Math.Max(from + 1, GameClock.MonthStartDay(now));
		int last = s.RegularEndDay;
		if (first > last)
		{
			return false;
		}
		HashSet<int> teamDays = new HashSet<int>();
		foreach (Match o in s.Games)
		{
			if (o != g && (o.HomeTeamId == g.HomeTeamId || o.AwayTeamId == g.HomeTeamId || o.HomeTeamId == g.AwayTeamId || o.AwayTeamId == g.AwayTeamId))
			{
				teamDays.Add(GameClock.DayOf(o.Time));
			}
		}
		Dictionary<int, int> venueGames = new Dictionary<int, int>();
		if (g.VenueId >= 0)
		{
			foreach (League l in Leagues)
			{
				Season[] array = new Season[1] { l.Current };
				foreach (Season x in array)
				{
					if (x == null)
					{
						continue;
					}
					foreach (Match o2 in x.Games)
					{
						if (o2 != g && o2.VenueId == g.VenueId)
						{
							int d = GameClock.DayOf(o2.Time);
							venueGames.TryGetValue(d, out var c);
							venueGames[d] = c + 1;
						}
					}
				}
			}
		}
		SportProfile prof = Profile(league);
		Func<int, bool> inSeason = (int dayNumber) =>
		{
			DateTime dateTime = GameClock.DateOf(dayNumber);
			return dateTime >= prof.Start(s.Year).Date && dateTime <= prof.End(s.Year).Date && !prof.IsBreak(s.Year, dateTime);
		};
		int minute = GameClock.MinuteOfDay(g.Time);
		int newTime = int.MinValue;
		bool doubleheader = false;
		int window = ((league.Sport == Sport.Baseball) ? 20 : 12);
		for (int d2 = first; d2 <= Math.Min(last, first + window); d2++)
		{
			if (newTime != int.MinValue)
			{
				break;
			}
			if (!teamDays.Contains(d2) && !venueGames.ContainsKey(d2) && inSeason(d2))
			{
				DayOfWeek dow = GameClock.DateOf(d2).DayOfWeek;
				int m = ((dow == DayOfWeek.Saturday || dow == DayOfWeek.Sunday) ? minute : Math.Max(minute, (league.Sport == Sport.Baseball) ? 1145 : 1170));
				newTime = GameClock.ToTime(d2, m);
			}
		}
		if (newTime == int.MinValue && league.Sport == Sport.Baseball)
		{
			Match next = null;
			foreach (Match o3 in s.Games)
			{
				if (o3 == g || o3.Played || o3.Featured || o3.Stage != GameStage.Regular || o3.HomeTeamId != g.HomeTeamId || o3.AwayTeamId != g.AwayTeamId || o3.VenueId != g.VenueId)
				{
					continue;
				}
				int d3 = GameClock.DayOf(o3.Time);
				if (d3 < first || d3 > last || GameClock.MinuteOfDay(o3.Time) < 1080 || !inSeason(d3) || (g.VenueId >= 0 && venueGames.TryGetValue(d3, out var c2) && c2 > 1))
				{
					continue;
				}
				int same = 0;
				foreach (Match x2 in s.Games)
				{
					if (x2 != g && GameClock.DayOf(x2.Time) == d3 && (x2.HomeTeamId == g.HomeTeamId || x2.AwayTeamId == g.HomeTeamId || x2.HomeTeamId == g.AwayTeamId || x2.AwayTeamId == g.AwayTeamId))
					{
						same++;
					}
				}
				if (same == 1 && (next == null || o3.Time < next.Time))
				{
					next = o3;
				}
			}
			if (next != null)
			{
				newTime = GameClock.ToTime(GameClock.DayOf(next.Time), 785);
				doubleheader = true;
				g.SeriesId = next.SeriesId;
			}
		}
		if (newTime == int.MinValue)
		{
			return false;
		}
		string reason = CallOffReason(g.Weather);
		int wasDay = from;
		g.PostponedFrom = from;
		g.Time = newTime;
		g.Featured = false;
		g.Spotlight = false;
		g.Live = null;
		g.Weather = -1;
		g.RainDelay = 0;
		s.Games.Sort(ByTime);
		if (IsUserTeam(g.HomeTeamId) || IsUserTeam(g.AwayTeamId))
		{
			Post(league, NewsKind.General, Wire, "ПЕРЕНЕСЕНО: " + TeamName(g.AwayTeamId) + " — " + TeamName(g.HomeTeamId) + ", " + GameClock.ShortDayLabel(wasDay) + " отменён (" + reason + "). Перенесённый матч: " + GameClock.ShortDayLabel(GameClock.DayOf(newTime)) + ", " + GameClock.ClockLabel(GameClock.MinuteOfDay(newTime)) + (doubleheader ? " (двойной матч)." : "."), isUser: true, now, 1);
		}
		Revision++;
		return true;
	}

	public static string WeatherText(Sport sport, Match g)
	{
		List<string> parts = new List<string>();
		if (g.Weather >= 0 && (g.Played || g.Live != null))
		{
			parts.Add(WeatherCodes.Label(g.Weather));
		}
		if (g.RainDelay > 0 && (g.Played || g.Live != null))
		{
			List<string> list = parts;
			string text = g.RainDelay.ToString();
			string text2;
			if (WeatherCodes.Kind(g.Weather) == WeatherKind.Storm)
			{
				text2 = "задержка из-за молнии";
			}
			else
			{
				text2 = ((WeatherCodes.Kind(g.Weather) == WeatherKind.Snow) ? "задержка из-за снега" : "задержка из-за дождя");
			}
			list.Add(text + " мин. " + text2);
		}
		if (g.PostponedFrom >= 0)
		{
			parts.Add("перенесённый матч " + GameClock.ShortDayLabel(g.PostponedFrom) + " из-за переноса");
		}
		return string.Join(", ", parts.ToArray());
	}

	public static bool HasDraft(Sport sport)
	{
		return sport != Sport.Soccer;
	}

	public bool IsYoungProspect(Player p)
	{
		if (p.Status == PlayerStatus.FreeAgent && p.Seasons == 0 && AgeOf(p) <= 22)
		{
			return p.Potential >= Rosters.Base(Level.Pro);
		}
		return false;
	}

	private bool YouthHold(Player p, int month)
	{
		if (p.Sport == Sport.Soccer && !HasDraft(p.Sport) && p.Seasons == 0 && p.FreeAgentMonth >= 0 && month - p.FreeAgentMonth < 1 && AgeOf(p) <= 22)
		{
			return UserTeamsAt(Sport.Soccer, Level.Pro).Count + UserTeamsAt(Sport.Soccer, Level.Minor).Count > 0;
		}
		return false;
	}

	private void OpenYouthMarket(League league, Season s, int now)
	{
		league.DraftDoneFor = Math.Max(league.DraftDoneFor, s.Year);
		Sport sport = league.Sport;
		int month = GameClock.MonthIndex(now);
		int clubs = league.TeamIds.Count;
		League second = SecondDivision();
		if (second != null && league.Sport == Sport.Soccer)
		{
			clubs += second.TeamIds.Count;
		}
		int target = Math.Max(12, clubs * 3);
		int have = DraftPool(sport).Count;
		List<Position> positions = new List<Position>();
		RosterSlot[] array = Rosters.Template(sport, Level.Pro);
		foreach (RosterSlot slot in array)
		{
			for (int j = 0; j < slot.Count; j++)
			{
				positions.Add(slot.Pos);
			}
		}
		int year = CurrentYear();
		for (int k = have; k < target; k++)
		{
			Player prospect = Rosters.GenerateProspect(sport, positions[_random.Next(positions.Count)], year, _random, () => NextId++, NameUse());
			GiveProspectBackground(prospect);
			AddPlayer(prospect);
		}
		List<Player> pool = new List<Player>(DraftPool(sport));
		foreach (Player item in pool)
		{
			item.Status = PlayerStatus.FreeAgent;
			item.Level = Level.Pro;
			item.FreeAgentMonth = month;
			item.Salary = 0;
			item.ContractYears = 0;
		}
		RostersChanged();
		pool.Sort((Player a, Player b) => (b.Potential == a.Potential) ? a.Id.CompareTo(b.Id) : b.Potential.CompareTo(a.Potential));
		List<string> top = new List<string>();
		for (int i2 = 0; i2 < pool.Count && i2 < 3; i2++)
		{
			top.Add(Rosters.PositionLabel(pool[i2].Position) + " " + pool[i2].Name + " (" + AgeOf(pool[i2]) + ", " + pool[i2].From + ")");
		}
		Post(league, NewsKind.Draft, DraftRoom, "МОЛОДЫЕ ПЕРСПЕКТИВНЫЕ ИГРОКИ: " + pool.Count + " молодых игроков вышли на рынок, клубы ведут за ними наблюдение. Самые востребованные: " + string.Join("; ", top.ToArray()) + ". Подпишите их на вкладке «Свободные агенты», пока это не сделали другие клубы.", isUser: false, now, 2);
		Revision++;
	}

	private void YouthSigningsMonthly(League league, int now)
	{
		if (HasDraft(league.Sport) || !SportRules.ProLike(league.Sport, league.Level))
		{
			return;
		}
		int thisMonth = GameClock.MonthIndex(now);
		List<Player> young = FreeAgents(league.Sport).FindAll((Player p2) => IsYoungProspect(p2) && !YouthHold(p2, thisMonth));
		if (young.Count == 0)
		{
			return;
		}
		EconomyProfile e = Economy.Get(league.Sport);
		int month = GameClock.MonthIndex(now);
		List<Team> clubs = LeagueTeams(league).FindAll((Team team) => !team.Manual);
		Dictionary<int, double> order = new Dictionary<int, double>();
		foreach (Team t in clubs)
		{
			order[t.Id] = (double)t.Rating + Rosters.Normal(_random, 0.0, 8.0);
		}
		clubs.Sort((Team a, Team b) => order[b.Id].CompareTo(order[a.Id]));
		foreach (Team t2 in clubs)
		{
			if (young.Count == 0)
			{
				break;
			}
			if (_random.NextDouble() >= 0.5)
			{
				continue;
			}
			List<Player> reserves = ProspectsOf(t2.Id);
			int worstPotential = int.MaxValue;
			foreach (Player x in reserves)
			{
				worstPotential = Math.Min(worstPotential, x.Potential);
			}
			Player best = null;
			double bestValue = double.MinValue;
			foreach (Player p in young)
			{
				double value = (double)p.Potential + 0.3 * (double)(p.Overall - Rosters.Base(Level.Pro)) + Rosters.Normal(_random, 0.0, 3.0);
				if (value > bestValue)
				{
					bestValue = value;
					best = p;
				}
			}
			if (best == null || (reserves.Count >= e.MaxProspects && best.Potential < worstPotential + 3))
			{
				continue;
			}
			int salary = Math.Max(e.MinSalary, Economy.AskingSalary(best, AgeOf(best), month));
			if (Economy.CanAfford(t2, SignedPlayers(t2.Id), salary, resign: false))
			{
				young.Remove(best);
				Sign(t2, best, salary, e.RookieYears + 1, now, announce: false);
				best.Status = PlayerStatus.Prospect;
				RostersChanged();
				ArrangeRoster(t2, now);
				if (best.Potential >= Rosters.Base(Level.Pro) + 10 || t2.IsUser)
				{
					Post(LeagueOf(t2) ?? league, NewsKind.Signing, Insider, "" + TeamName(t2.Id) + " подписывает молодого игрока " + Rosters.NewsPositionLabel(best.Position, best.Sport) + " " + best.Name + " (" + AgeOf(best) + ", из клуба " + best.From + ") в резервную команду.", t2.IsUser, now, 1);
				}
			}
		}
	}
}
