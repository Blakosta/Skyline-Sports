using System;
using System.Collections.Generic;
using System.Globalization;

namespace Skyline_Sports_Project.Engine;

public static class Economy
{
	private static readonly Dictionary<Sport, EconomyProfile> _profiles = Build();

	private static readonly Dictionary<Sport, double> _unit = new Dictionary<Sport, double>();

	private static readonly DayOfWeek Tue = DayOfWeek.Tuesday;

	private static readonly DayOfWeek Wed = DayOfWeek.Wednesday;

	private static readonly DayOfWeek Thu = DayOfWeek.Thursday;

	private static readonly DayOfWeek Fri = DayOfWeek.Friday;

	private static readonly DayOfWeek Sun = DayOfWeek.Sunday;

	public static EconomyProfile Get(Sport sport)
	{
		return _profiles[sport];
	}

	private static Dictionary<Sport, EconomyProfile> Build()
	{
		Dictionary<Sport, EconomyProfile> map = new Dictionary<Sport, EconomyProfile>();
		map[Sport.Hockey] = new EconomyProfile
		{
			Sport = Sport.Hockey,
			Type = CapType.Hard,
			RealCap = 104000,
			RealRoster = 23,
			MinSalary = 850,
			MaxShare = 0.2,
			MaxYearsResign = 7,
			MaxYearsFree = 6,
			RookieYears = 3,
			DraftRounds = 7,
			MaxProspects = 12,
			LotteryOdds = new int[16]
			{
				185, 135, 115, 95, 85, 75, 65, 60, 50, 35,
				30, 25, 20, 15, 5, 5
			},
			LotteryPicks = 2,
			MaxJump = 10,
			LotteryDate = (int y) => GameClock.Nth(y + 1, 5, Tue, 1),
			DraftDate = (int y) => GameClock.Last(y + 1, 6, Fri),
			LeagueYearDate = (int y) => GameClock.D(y + 1, 7, 1),
			TradeDeadline = (int y) => GameClock.Nth(y + 1, 3, Fri, 1)
		};
		map[Sport.Basketball] = new EconomyProfile
		{
			Sport = Sport.Basketball,
			Type = CapType.Soft,
			RealCap = 164961,
			RealRoster = 15,
			MinSalary = 1300,
			MaxShare = 0.35,
			MaxYearsResign = 5,
			MaxYearsFree = 4,
			RookieYears = 4,
			DraftRounds = 2,
			MaxProspects = 3,
			LotteryOdds = new int[14]
			{
				140, 140, 140, 125, 105, 90, 75, 60, 45, 30,
				20, 15, 10, 5
			},
			LotteryPicks = 4,
			LotteryDate = (int y) => GameClock.Nth(y + 1, 5, Sun, 2),
			DraftDate = (int y) => GameClock.Nth(y + 1, 6, Tue, 4),
			LeagueYearDate = (int y) => GameClock.D(y + 1, 7, 1),
			TradeDeadline = (int y) => GameClock.Nth(y + 1, 2, Thu, 1)
		};
		map[Sport.Football] = new EconomyProfile
		{
			Sport = Sport.Football,
			Type = CapType.Hard,
			RealCap = 301200,
			RealRoster = 53,
			MinSalary = 885,
			MaxShare = 0.2,
			MaxYearsResign = 5,
			MaxYearsFree = 5,
			RookieYears = 4,
			DraftRounds = 7,
			MaxProspects = 12,
			LotteryOdds = null,
			LotteryDate = null,
			DraftDate = (int y) => GameClock.Last(y + 1, 4, Thu),
			LeagueYearDate = (int y) => GameClock.Nth(y + 1, 3, Wed, 2),
			TradeDeadline = (int y) => GameClock.Nth(y, 11, Tue, 1)
		};
		map[Sport.Baseball] = new EconomyProfile
		{
			Sport = Sport.Baseball,
			Type = CapType.Budget,
			RealCap = 244000,
			RealRoster = 26,
			MinSalary = 780,
			MaxShare = 0.28,
			MaxYearsResign = 8,
			MaxYearsFree = 8,
			RookieYears = 4,
			DraftRounds = 4,
			MaxProspects = 10,
			LotteryOdds = new int[18]
			{
				165, 165, 165, 132, 100, 75, 55, 39, 27, 18,
				11, 8, 6, 4, 3, 2, 2, 2
			},
			LotteryPicks = 6,
			CapName = "Порог налога на роскошь",
			LotteryDate = (int y) => GameClock.Nth(y, 12, Tue, 2),
			DraftDate = (int y) => GameClock.Nth(y + 1, 7, Sun, 2),
			LeagueYearDate = (int y) => GameClock.D(y, 11, 6),
			TradeDeadline = (int y) => GameClock.D(y, 7, 31)
		};
		map[Sport.Soccer] = new EconomyProfile
		{
			Sport = Sport.Soccer,
			Type = CapType.Mls,
			RealCap = 6425,
			RealRoster = 20,
			MinSalary = 110,
			MaxShare = 0.0,
			MaxYearsResign = 4,
			MaxYearsFree = 4,
			RookieYears = 2,
			DraftRounds = 0,
			MaxProspects = 4,
			LotteryOdds = null,
			CapName = "Бюджет зарплат",
			DraftName = "Супердрафт",
			LotteryDate = null,
			DraftDate = (int y) => GameClock.Nth(y + 1, 6, Thu, 1),
			LeagueYearDate = (int y) => GameClock.D(y + 1, 6, 15),
			TradeDeadline = (int y) => GameClock.D(y + 1, 3, 1).AddDays(-1.0)
		};
		return map;
	}

	public static int RosterSize(Sport sport)
	{
		int n = 0;
		RosterSlot[] array = Rosters.Template(sport, Level.Pro);
		foreach (RosterSlot s in array)
		{
			n += s.Count;
		}
		return n;
	}

	public static int Cap(Sport sport)
	{
		EconomyProfile e = Get(sport);
		return (int)Math.Round((double)e.RealCap * (double)RosterSize(sport) / (double)e.RealRoster);
	}

	public static int MaxBudgetCharge(Sport sport)
	{
		if (sport != Sport.Soccer)
		{
			return int.MaxValue;
		}
		return 803;
	}

	public static int MaxSalary(Sport sport)
	{
		EconomyProfile e = Get(sport);
		if (e.Type == CapType.Mls)
		{
			return 12000;
		}
		return (int)Math.Round((double)e.RealCap * e.MaxShare);
	}

	public static int CapHit(Sport sport, IEnumerable<Player> players)
	{
		if (Get(sport).Type != CapType.Mls)
		{
			int total = 0;
			{
				foreach (Player p in players)
				{
					total += p.Salary;
				}
				return total;
			}
		}
		int charge = 0;
		foreach (Player p2 in players)
		{
			charge += Math.Min(p2.Salary, MaxBudgetCharge(sport));
		}
		return charge;
	}

	public static int Payroll(IEnumerable<Player> players)
	{
		int total = 0;
		foreach (Player p in players)
		{
			total += p.Salary;
		}
		return total;
	}

	public static int DeadNow(Team team)
	{
		int total = 0;
		if (team == null || team.Dead == null)
		{
			return 0;
		}
		foreach (DeadCharge c in team.Dead)
		{
			if (c.Amounts.Count > 0)
			{
				total += c.Amounts[0];
			}
		}
		return total;
	}

	public static int DeadLater(Team team)
	{
		int total = 0;
		if (team == null || team.Dead == null)
		{
			return 0;
		}
		foreach (DeadCharge c in team.Dead)
		{
			for (int i = 1; i < c.Amounts.Count; i++)
			{
				total += c.Amounts[i];
			}
		}
		return total;
	}

	public static int Committed(Team team, List<Player> signed)
	{
		return ((Get(team.Sport).Type == CapType.Mls) ? CapHit(team.Sport, signed) : Payroll(signed)) + DeadNow(team);
	}

	public static bool IsMinimum(Sport sport, int salary)
	{
		return salary <= Get(sport).MinSalary * 6 / 5;
	}

	public static int DesignatedPlayers(Sport sport, IEnumerable<Player> players)
	{
		int n = 0;
		foreach (Player player in players)
		{
			if (player.Salary > MaxBudgetCharge(sport))
			{
				n++;
			}
		}
		return n;
	}

	public static int Limit(Team team)
	{
		if (Get(team.Sport).Type == CapType.Budget)
		{
			if (team.Budget <= 0)
			{
				return Cap(team.Sport) * 7 / 10;
			}
			return team.Budget;
		}
		return Cap(team.Sport);
	}

	public static bool CanAfford(Team team, List<Player> signed, int salary, bool resign)
	{
		EconomyProfile e = Get(team.Sport);
		if (salary <= e.MinSalary * 6 / 5)
		{
			return true;
		}
		int cap = Cap(team.Sport);
		int dead = DeadNow(team);
		switch (e.Type)
		{
		case CapType.Soft:
		{
			int payroll = Payroll(signed) + dead;
			int apron = cap * 5 / 4;
			if (resign)
			{
				return payroll + salary <= apron + cap / 10;
			}
			if (payroll + salary <= cap)
			{
				return true;
			}
			if (salary <= cap * 85 / 1000)
			{
				return payroll + salary <= apron;
			}
			return false;
		}
		case CapType.Budget:
		{
			int limit = Limit(team);
			return Payroll(signed) + dead + salary <= (resign ? (limit * 105 / 100) : limit);
		}
		case CapType.Mls:
			if (salary > MaxBudgetCharge(team.Sport) && DesignatedPlayers(team.Sport, signed) >= 3)
			{
				return false;
			}
			return CapHit(team.Sport, signed) + dead + Math.Min(salary, MaxBudgetCharge(team.Sport)) <= cap;
		default:
			return Payroll(signed) + dead + salary <= cap;
		}
	}

	public static int NewBudget(Sport sport, Random random, int rating = 50)
	{
		if (Get(sport).Type != CapType.Budget)
		{
			return 0;
		}
		double share = 0.62 + (double)(rating - 50) * 0.012 + Rosters.Normal(random, 0.0, 0.12);
		share = Math.Max(0.42, Math.Min(1.18, share));
		return (int)Math.Round((double)Cap(sport) * share);
	}

	public static void BalancePayroll(Team team, List<Player> roster, Random random)
	{
		EconomyProfile e = Get(team.Sport);
		int cap = Cap(team.Sport);
		double target = e.Type switch
		{
			CapType.Soft => (double)cap * (0.88 + 0.22 * random.NextDouble()), 
			CapType.Budget => (double)Limit(team) * (0.85 + 0.15 * random.NextDouble()), 
			_ => (double)cap * (0.82 + 0.16 * random.NextDouble()), 
		};
		if (e.Type == CapType.Mls)
		{
			List<Player> paid = new List<Player>(roster);
			paid.Sort((Player a, Player b) => b.Salary.CompareTo(a.Salary));
			for (int i = 3; i < paid.Count; i++)
			{
				paid[i].Salary = Math.Min(paid[i].Salary, MaxBudgetCharge(team.Sport));
			}
		}
		for (int pass = 0; pass < 3; pass++)
		{
			double total = 0.0;
			double flexible = 0.0;
			foreach (Player p in roster)
			{
				int counted = ((e.Type == CapType.Mls) ? Math.Min(p.Salary, MaxBudgetCharge(team.Sport)) : p.Salary);
				total += (double)counted;
				if (counted > e.MinSalary)
				{
					flexible += (double)(counted - e.MinSalary);
				}
			}
			if (flexible <= 0.0 || Math.Abs(total - target) < target * 0.01)
			{
				break;
			}
			double factor = Math.Max(0.2, Math.Min(2.5, (target - (total - flexible)) / flexible));
			foreach (Player p2 in roster)
			{
				if ((e.Type != CapType.Mls || p2.Salary <= MaxBudgetCharge(team.Sport)) && p2.Salary > e.MinSalary)
				{
					double s = (double)e.MinSalary + (double)(p2.Salary - e.MinSalary) * factor;
					if (e.Type == CapType.Mls)
					{
						s = Math.Min(s, MaxBudgetCharge(team.Sport));
					}
					p2.Salary = RoundSalary(Math.Min(MaxSalary(team.Sport), s));
				}
			}
		}
	}

	private static double PositionFactor(Sport sport, Position pos)
	{
		switch (pos)
		{
		case Position.Quarterback:
			return 2.6;
		case Position.DefensiveLine:
			return 1.25;
		case Position.WideReceiver:
			return 1.2;
		case Position.Cornerback:
			return 1.1;
		case Position.Linebacker:
			return 0.9;
		case Position.TightEnd:
		case Position.Safety:
			return 0.85;
		case Position.RunningBack:
			return 0.7;
		case Position.Kicker:
			return 0.35;
		case Position.Punter:
			return 0.3;
		case Position.StartingPitcher:
			return 1.2;
		case Position.ReliefPitcher:
			return 0.6;
		case Position.Catcher:
			return 0.9;
		case Position.Goalie:
			return 1.1;
		case Position.Center:
			return 1.05;
		case Position.LeftWing:
		case Position.RightWing:
			return 0.95;
		case Position.Goalkeeper:
			return 0.7;
		case Position.Forward:
			return 1.2;
		case Position.Midfielder:
			return 1.05;
		case Position.Defender:
			return 0.9;
		default:
			return 1.0;
		}
	}

	private static double AgeFactor(int age)
	{
		if (age <= 30)
		{
			return 1.0;
		}
		return age switch
		{
			31 => 0.92, 
			32 => 0.84, 
			33 => 0.75, 
			34 => 0.65, 
			_ => 0.55, 
		};
	}

	private static double Weight(Sport sport, Position pos, int overall, int age)
	{
		return Math.Exp((double)(overall - Rosters.Base(Level.Pro)) / 8.0) * PositionFactor(sport, pos) * AgeFactor(age);
	}

	private static double Unit(Sport sport)
	{
		if (_unit.TryGetValue(sport, out var unit))
		{
			return unit;
		}
		EconomyProfile e = Get(sport);
		double total = 0.0;
		int n = 0;
		RosterSlot[] array = Rosters.Template(sport, Level.Pro);
		foreach (RosterSlot slot in array)
		{
			for (int j = 0; j < slot.Count; j++)
			{
				total += Weight(sport, slot.Pos, Rosters.Base(Level.Pro) - ((j >= slot.Starters) ? 7 : 0), 27);
				n++;
			}
		}
		double target = ((e.Type == CapType.Budget) ? ((double)Cap(sport) * 0.68) : ((double)Cap(sport) * 0.88));
		unit = Math.Max(1.0, (target - (double)(n * e.MinSalary)) / total);
		_unit[sport] = unit;
		return unit;
	}

	public static int MarketValue(Player p, int age)
	{
		EconomyProfile economyProfile = Get(p.Sport);
		double w = Weight(p.Sport, p.Position, p.Overall, age);
		double v = (double)economyProfile.MinSalary + Unit(p.Sport) * w;
		if (economyProfile.Type == CapType.Mls && p.Overall >= Rosters.Base(Level.Pro) + 6)
		{
			v = Math.Max(v, 900.0 + 1500.0 * (w - 1.0));
		}
		v = Math.Max(economyProfile.MinSalary, Math.Min(MaxSalary(p.Sport), v));
		return RoundSalary(v);
	}

	public static int RoundSalary(double v)
	{
		if (v < 1000.0)
		{
			return (int)(Math.Round(v / 5.0) * 5.0);
		}
		return (int)(Math.Round(v / 25.0) * 25.0);
	}

	public static int AskingSalary(Player p, int age, int monthIndex)
	{
		int value = MarketValue(p, age);
		int months = ((p.FreeAgentMonth >= 0) ? Math.Max(0, monthIndex - p.FreeAgentMonth) : 0);
		double factor = Math.Max(0.4, 1.0 - 0.1 * (double)months);
		return Math.Max(Get(p.Sport).MinSalary, RoundSalary((double)value * factor));
	}

	public static int Discount(int salary, Sport sport, double factor)
	{
		return Math.Max(Get(sport).MinSalary, RoundSalary((double)salary * factor));
	}

	public static int AskingYears(Sport sport, int age, bool resign)
	{
		EconomyProfile e = Get(sport);
		int max = (resign ? e.MaxYearsResign : e.MaxYearsFree);
		int years;
		if (age <= 25)
		{
			years = 5;
		}
		else if (age <= 28)
		{
			years = 4;
		}
		else
		{
			years = ((age <= 30) ? 3 : ((age > 33) ? 1 : 2));
		}
		return Math.Max(1, Math.Min(max, years));
	}

	public static void RookieContract(Player p, int pick, int teams)
	{
		EconomyProfile e = Get(p.Sport);
		double factor = ((pick <= teams) ? (1.0 + 2.2 * (1.0 - ((double)pick - 1.0) / (double)Math.Max(1, teams))) : 1.0);
		p.Salary = RoundSalary((double)e.MinSalary * factor);
		p.ContractYears = e.RookieYears;
	}

	public static void InitialContract(Player p, Team team, int age, Random random)
	{
		EconomyProfile e = Get(p.Sport);
		if (age <= 23)
		{
			p.Salary = RoundSalary((double)e.MinSalary * (1.0 + random.NextDouble()));
			p.ContractYears = 1 + random.Next(e.RookieYears);
		}
		else
		{
			p.Salary = RoundSalary(Math.Max(e.MinSalary, (double)MarketValue(p, age) * (0.75 + 0.45 * random.NextDouble())));
			p.ContractYears = 1 + random.Next(Math.Max(1, AskingYears(p.Sport, age, resign: true)));
		}
	}

	public static string Money(int thousands)
	{
		if (thousands >= 1000)
		{
			return "$" + ((double)thousands / 1000.0).ToString((thousands >= 10000) ? "0.0" : "0.00", CultureInfo.InvariantCulture) + " млн";
		}
		return "$" + thousands + " тыс.";
	}

	public static string ContractText(Player p)
	{
		if (p.ContractYears <= 0)
		{
			return "";
		}
		return Money(p.Salary) + " x " + p.ContractYears + ((p.ContractYears == 1) ? " год" : " лет");
	}
}
