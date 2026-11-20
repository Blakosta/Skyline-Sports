using System;
using System.Collections.Generic;
using System.Globalization;

namespace Skyline_Sports_Project.Engine;

public static class AwardBook
{
	public static readonly Dictionary<string, string> OldTrophyNames = new Dictionary<string, string>
	{
		{ "Frozen Star Award", "Crestwood Award" },
		{ "Golden Hoop Award", "Hollis Award" },
		{ "Golden Helmet Trophy", "Stanton Trophy" },
		{ "Golden Cleat Award", "Galloway Trophy" },
		{ "First Season Award", "Bright Futures Award" },
		{ "Summit Trophy", "Harrow Memorial Trophy" },
		{ "Iron Mask Trophy", "Vasilev Trophy" },
		{ "Blue Line Trophy", "Delacroix Trophy" },
		{ "First Frost Trophy", "Whitfield Trophy" },
		{ "Silver Stick", "Ashworth Trophy" },
		{ "Lamplighter Trophy", "Lamont Trophy" },
		{ "Final Horn Trophy", "Corbeau Trophy" },
		{ "Hardwood Crown", "Maxwell Prescott Trophy" },
		{ "Lockdown Trophy", "Thornbury Trophy" },
		{ "Rising Star Trophy", "Ainsley Trophy" },
		{ "Spark Plug Award", "Carraway Award" },
		{ "Leap Award", "Rising Tide Award" },
		{ "Golden Net", "Fairbanks Trophy" },
		{ "Finals Hero Trophy", "Whitlock Trophy" },
		{ "Gridiron Crown", "Halcomb Trophy" },
		{ "Field General Award", "Brennan Award" },
		{ "Iron Curtain Award", "Ironside Award" },
		{ "First Down Award", "Kincaid Award" },
		{ "Fresh Steel Award", "Mercer Award" },
		{ "Big Game Hero Trophy", "Marlowe Trophy" },
		{ "Diamond Crown", "Whitaker Trophy" },
		{ "Golden Arm Award", "Hollister Award" },
		{ "First Pitch Award", "Pemberton Award" },
		{ "Last Out Award", "Fireman Award" },
		{ "October Hero Trophy", "October Crown" },
		{ "Pitch Crown", "Castellano Trophy" },
		{ "Top Boot", "Ferreira Boot" },
		{ "Safe Hands Award", "Golden Glove" },
		{ "Back Line Award", "Rocha Award" },
		{ "Next Wave Award", "Aurora Award" },
		{ "New Arrival Award", "Harbor Award" },
		{ "Cup Final Hero", "Meridian Final MVP" }
	};

	private static string I(int v)
	{
		return v.ToString(CultureInfo.InvariantCulture);
	}

	private static string D1(double v)
	{
		return v.ToString("0.0", CultureInfo.InvariantCulture);
	}

	private static string D2(double v)
	{
		return v.ToString("0.00", CultureInfo.InvariantCulture);
	}

	private static string P3(double v)
	{
		string t = v.ToString("0.000", CultureInfo.InvariantCulture);
		if (!(v < 1.0))
		{
			return t;
		}
		return t.Substring(1);
	}

	private static int G(int[] s, int i)
	{
		return Stats.Get(s, i);
	}

	private static double Per(int a, int b)
	{
		if (b > 0)
		{
			return (double)a / (double)b;
		}
		return 0.0;
	}

	private static double TeamFactor(AwardContext c, Player p)
	{
		return 0.7 + 0.6 * c.TeamWinPct(p);
	}

	private static bool Rookie(AwardContext c, Player p)
	{
		if (p.Level == Level.Minor)
		{
			if (p.Seasons == 0)
			{
				return c.Year - p.BirthYear <= 24;
			}
			return false;
		}
		if (p.Level != Level.Pro)
		{
			return p.ClassYear == 1;
		}
		if (p.Seasons == 0)
		{
			if (p.DraftYear <= 0)
			{
				return c.Year - p.BirthYear <= 23;
			}
			return true;
		}
		return false;
	}

	private static bool Skater(Player p)
	{
		return p.Position != Position.Goalie;
	}

	private static double SkaterValue(int[] s)
	{
		return (double)G(s, 1) * 1.5 + (double)G(s, 2) + (double)G(s, 3) * 0.03;
	}

	private static double SavePct(int[] s)
	{
		return Per(G(s, 8) - G(s, 9), G(s, 8));
	}

	private static double GoalieValue(AwardContext c, int[] s)
	{
		double share = Math.Min(1.0, Per(G(s, 11), Math.Max(1, c.TeamGames)));
		return (double)G(s, 5) * 1.5 + (SavePct(s) - 0.905) * 3000.0 * share;
	}

	private static string SkaterLine(int[] s)
	{
		return I(G(s, 1)) + " гол., " + I(G(s, 2)) + " передач, " + I(G(s, 1) + G(s, 2)) + " очк.";
	}

	private static string GoalieLine(int[] s)
	{
		return I(G(s, 5)) + " побед, " + P3(SavePct(s)) + " % отражённых бросков, " + D2(Per(G(s, 9), G(s, 11))) + " пропущенных шайб за матч";
	}

	private static double HockeyValue(AwardContext c, Player p, int[] s)
	{
		if (!Skater(p))
		{
			return GoalieValue(c, s);
		}
		return SkaterValue(s);
	}

	private static string HockeyLine(Player p, int[] s)
	{
		if (!Skater(p))
		{
			return GoalieLine(s);
		}
		return SkaterLine(s);
	}

	private static bool HockeyQualifies(AwardContext c, Player p, int[] s)
	{
		if (!Skater(p))
		{
			return G(s, 11) >= Math.Max(1, c.TeamGames / 3);
		}
		return G(s, 0) >= c.Half;
	}

	private static double PerGame(int[] s, int stat)
	{
		return Per(G(s, stat), G(s, 0));
	}

	private static double HoopValue(int[] s)
	{
		return PerGame(s, 1) + 1.2 * PerGame(s, 2) + 1.5 * PerGame(s, 3) + 2.0 * (PerGame(s, 4) + PerGame(s, 5));
	}

	private static string HoopLine(int[] s)
	{
		return D1(PerGame(s, 1)) + " очк./матч, " + D1(PerGame(s, 2)) + " подб./матч, " + D1(PerGame(s, 3)) + " передач/матч";
	}

	private static bool HoopQualifies(AwardContext c, int[] s)
	{
		return G(s, 0) >= c.Half;
	}

	private static bool IsDefense(Position p)
	{
		if (p != Position.DefensiveLine && p != Position.Linebacker && p != Position.Cornerback)
		{
			return p == Position.Safety;
		}
		return true;
	}

	private static double QbValue(int[] s)
	{
		return (double)G(s, 3) / 25.0 + (double)(G(s, 4) * 4) - (double)(G(s, 5) * 2) + (double)G(s, 7) / 10.0 + (double)(G(s, 8) * 6);
	}

	private static double SkillValue(int[] s)
	{
		return (double)(G(s, 7) + G(s, 10)) / 10.0 + (double)((G(s, 8) + G(s, 11)) * 6);
	}

	private static double DefenseValue(int[] s)
	{
		return (double)G(s, 12) * 0.5 + (double)(G(s, 13) * 4) + (double)(G(s, 14) * 5);
	}

	private static double OffenseValue(Player p, int[] s)
	{
		if (p.Position != Position.Quarterback)
		{
			return SkillValue(s);
		}
		return QbValue(s);
	}

	private static string OffenseLine(Player p, int[] s)
	{
		if (p.Position == Position.Quarterback)
		{
			return I(G(s, 3)) + " ярдов пасом, " + I(G(s, 4)) + " тачдаунов, " + I(G(s, 5)) + " перехватов";
		}
		return I(G(s, 7) + G(s, 10)) + " ярдов в атаке, " + I(G(s, 8) + G(s, 11)) + " тачдаунов";
	}

	private static string DefenseLine(int[] s)
	{
		return I(G(s, 12)) + " захватов, " + I(G(s, 13)) + " сэков, " + I(G(s, 14)) + " перехватов";
	}

	private static bool Offense(Position p)
	{
		if (p != Position.Quarterback && p != Position.RunningBack && p != Position.WideReceiver)
		{
			return p == Position.TightEnd;
		}
		return true;
	}

	private static double FootballValue(Player p, int[] s)
	{
		if (!Offense(p.Position))
		{
			if (!IsDefense(p.Position))
			{
				return 0.0;
			}
			return DefenseValue(s) * 1.4;
		}
		return OffenseValue(p, s);
	}

	private static string FootballLine(Player p, int[] s)
	{
		if (!Offense(p.Position))
		{
			return DefenseLine(s);
		}
		return OffenseLine(p, s);
	}

	private static int PA(int[] s)
	{
		return G(s, 1) + G(s, 7);
	}

	private static double Ops(int[] s)
	{
		double num = Per(G(s, 3) + G(s, 7), PA(s));
		double slg = Per(G(s, 3) + G(s, 4) + 3 * G(s, 5), G(s, 1));
		return num + slg;
	}

	private static double HitterValue(int[] s)
	{
		return (Ops(s) - 0.7) * (double)PA(s) * 0.25;
	}

	private static double Era(int[] s)
	{
		return Per(G(s, 13) * 27, G(s, 11));
	}

	private static double PitcherValue(int[] s)
	{
		return (4.6 - Era(s)) * (double)G(s, 11) / 27.0 + (double)G(s, 15) / 40.0;
	}

	private static double RelieverValue(int[] s)
	{
		return (double)G(s, 18) * 1.2 + (4.0 - Era(s)) * (double)G(s, 11) / 27.0 + (double)G(s, 15) / 20.0;
	}

	private static bool HitterQualifies(AwardContext c, Player p, int[] s)
	{
		if (!Rosters.IsPitcher(p.Position))
		{
			return PA(s) >= Math.Max(1, (int)((double)c.TeamGames * 2.5));
		}
		return false;
	}

	private static bool StarterQualifies(AwardContext c, Player p, int[] s)
	{
		if (Rosters.IsPitcher(p.Position))
		{
			return G(s, 11) >= Math.Max(3, (int)((double)c.TeamGames * 2.4));
		}
		return false;
	}

	private static string HitterLine(int[] s)
	{
		return P3(Per(G(s, 3), G(s, 1))) + " ср. отб., " + I(G(s, 5)) + " хоум-ранов, " + I(G(s, 6)) + " ранов с участием бьющего";
	}

	private static string PitcherLine(int[] s)
	{
		return I(G(s, 16)) + "-" + I(G(s, 17)) + ", " + D2(Era(s)) + " пропущенных ранов за 9 иннингов, " + I(G(s, 15)) + " страйкаутов";
	}

	private static double BaseballValue(AwardContext c, Player p, int[] s)
	{
		if (Rosters.IsPitcher(p.Position))
		{
			if (!StarterQualifies(c, p, s))
			{
				return 0.0;
			}
			return PitcherValue(s) * 0.8;
		}
		if (!HitterQualifies(c, p, s))
		{
			return 0.0;
		}
		return HitterValue(s);
	}

	private static string BaseballLine(Player p, int[] s)
	{
		if (!Rosters.IsPitcher(p.Position))
		{
			return HitterLine(s);
		}
		return PitcherLine(s);
	}

	private static double PlayoffBaseball(Player p, int[] s)
	{
		if (Rosters.IsPitcher(p.Position))
		{
			return (4.6 - Era(s)) * (double)G(s, 11) / 27.0 + (double)(G(s, 16) * 2) + (double)G(s, 18);
		}
		return (Ops(s) - 0.6) * (double)PA(s) * 0.3 + (double)G(s, 5) * 1.5;
	}

	private static double FieldValue(int[] s)
	{
		return (double)(G(s, 1) * 3 + G(s, 2) * 2) + (double)G(s, 3) * 0.1;
	}

	private static double KeeperValue(int[] s)
	{
		return (double)(G(s, 7) * 3) + (double)G(s, 5) * 0.3 - (double)G(s, 6) * 0.5;
	}

	private static string FieldLine(int[] s)
	{
		return I(G(s, 1)) + " голов, " + I(G(s, 2)) + " передач";
	}

	private static string KeeperLine(int[] s)
	{
		return I(G(s, 7)) + " матчей без пропущенных голов, " + I(G(s, 5)) + " сейвов";
	}

	private static bool Keeper(Player p)
	{
		return p.Position == Position.Goalkeeper;
	}

	private static double SoccerValue(Player p, int[] s)
	{
		if (!Keeper(p))
		{
			return FieldValue(s);
		}
		return KeeperValue(s) * 0.8;
	}

	private static string SoccerLine(Player p, int[] s)
	{
		if (!Keeper(p))
		{
			return FieldLine(s);
		}
		return KeeperLine(s);
	}

	private static AwardDef Mvp(Sport sport, string award, string trophy)
	{
		return new AwardDef
		{
			Award = award,
			Trophy = trophy,
			Headline = true,
			Eligible = (AwardContext c, Player p, int[] s) => SportQualifies(sport, c, p, s),
			Score = (AwardContext c, Player p, int[] s) => SportValue(sport, c, p, s) * TeamFactor(c, p),
			Detail = (AwardContext c, Player p, int[] s) => SportLine(sport, p, s)
		};
	}

	private static AwardDef Freshman(Sport sport, string award, string trophy)
	{
		return new AwardDef
		{
			Award = award,
			Trophy = trophy,
			Eligible = (AwardContext c, Player p, int[] s) => Rookie(c, p) && SportQualifies(sport, c, p, s),
			Score = (AwardContext c, Player p, int[] s) => SportValue(sport, c, p, s),
			Detail = (AwardContext c, Player p, int[] s) => SportLine(sport, p, s)
		};
	}

	private static AwardDef PlayoffMvp(Sport sport, string award, string trophy)
	{
		return new AwardDef
		{
			Award = award,
			Trophy = trophy,
			Playoff = true,
			Headline = true,
			Eligible = (AwardContext c, Player p, int[] s) => !Stats.IsEmpty(s),
			Score = (AwardContext c, Player p, int[] s) => PlayoffValue(sport, c, p, s),
			Detail = (AwardContext c, Player p, int[] s) => SportLine(sport, p, s) + " в плей-офф"
		};
	}

	private static bool SportQualifies(Sport sport, AwardContext c, Player p, int[] s)
	{
		switch (sport)
		{
		case Sport.Hockey:
			return HockeyQualifies(c, p, s);
		case Sport.Basketball:
			return HoopQualifies(c, s);
		case Sport.Football:
			if (!Offense(p.Position))
			{
				return IsDefense(p.Position);
			}
			return true;
		case Sport.Baseball:
			if (!HitterQualifies(c, p, s))
			{
				return StarterQualifies(c, p, s);
			}
			return true;
		default:
			if (!Keeper(p))
			{
				return G(s, 0) >= c.Half;
			}
			return G(s, 8) >= c.Half;
		}
	}

	private static double SportValue(Sport sport, AwardContext c, Player p, int[] s)
	{
		return sport switch
		{
			Sport.Hockey => HockeyValue(c, p, s), 
			Sport.Basketball => HoopValue(s), 
			Sport.Football => FootballValue(p, s), 
			Sport.Baseball => BaseballValue(c, p, s), 
			_ => SoccerValue(p, s), 
		};
	}

	public static double SelectionValue(Sport sport, AwardContext c, Player p, int[] s)
	{
		switch (sport)
		{
		case Sport.Football:
			if (p.Position == Position.OffensiveLine)
			{
				return (double)(p.Overall - 60) * 2.5 + PointsFor(c, p) * 0.8;
			}
			if (p.Position == Position.Kicker)
			{
				return (double)(G(s, 15) * 3 + G(s, 17)) + (double)(p.Overall - 60) * 0.5;
			}
			if (p.Position == Position.Punter)
			{
				return (p.Overall - 60) * 2;
			}
			return FootballValue(p, s) + (double)(p.Overall - 70) * 0.5;
		case Sport.Hockey:
			if (p.Position == Position.Defense)
			{
				return SkaterValue(s) + (double)p.Overall * 0.8;
			}
			break;
		}
		return SportValue(sport, c, p, s);
	}

	private static double PointsFor(AwardContext c, Player p)
	{
		if (!c.PointsPerGame.TryGetValue(p.TeamId, out var v))
		{
			return 0.0;
		}
		return v;
	}

	public static bool SelectionQualifies(Sport sport, AwardContext c, Player p, int[] s, bool allLeague)
	{
		return sport switch
		{
			Sport.Hockey => HockeyQualifies(c, p, s), 
			Sport.Basketball => G(s, 0) >= (allLeague ? Math.Max(1, (int)Math.Ceiling((double)c.TeamGames * 65.0 / 82.0)) : c.Half), 
			_ => G(s, 0) >= c.Half, 
		};
	}

	public static string SelectionLine(Sport sport, Player p, int[] s)
	{
		if (sport == Sport.Football)
		{
			if (p.Position == Position.OffensiveLine || p.Position == Position.Punter)
			{
				return I(G(s, 0)) + " матчей";
			}
			if (p.Position == Position.Kicker)
			{
				return I(G(s, 15)) + "/" + I(G(s, 16)) + " точных бросков с игры";
			}
		}
		return SportLine(sport, p, s);
	}

	public static string SportLine(Sport sport, Player p, int[] s)
	{
		return sport switch
		{
			Sport.Hockey => HockeyLine(p, s), 
			Sport.Basketball => HoopLine(s), 
			Sport.Football => FootballLine(p, s), 
			Sport.Baseball => BaseballLine(p, s), 
			_ => SoccerLine(p, s), 
		};
	}

	private static double PlayoffValue(Sport sport, AwardContext c, Player p, int[] s)
	{
		switch (sport)
		{
		case Sport.Hockey:
			if (!Skater(p))
			{
				return (double)G(s, 5) * 2.5 + (SavePct(s) - 0.9) * 400.0;
			}
			return SkaterValue(s);
		case Sport.Basketball:
			return HoopValue(s) * Math.Min(1.0, (double)G(s, 0) / 4.0);
		case Sport.Football:
			return FootballValue(p, s);
		case Sport.Baseball:
			return PlayoffBaseball(p, s);
		default:
			if (!Keeper(p))
			{
				return FieldValue(s);
			}
			return KeeperValue(s) * 0.8;
		}
	}

	public static string ChampionTrophy(Sport sport, Level level)
	{
		return level switch
		{
			Level.HighSchool => "Кубок чемпионов региона", 
			Level.Junior => "Кубок «Доминион»", 
			Level.Minor => sport switch
			{
				Sport.Hockey => "Кубок «Риджуэй»", 
				Sport.Basketball => "Кубок «Литейщик»", 
				_ => "Кубок «Гавань»", 
			}, 
			Level.College => sport switch
			{
				Sport.Football => "Кубок «Основателей»", 
				Sport.Basketball => "Кубок «Холлис»", 
				_ => "Кубок национального чемпионата", 
			}, 
			_ => sport switch
			{
				Sport.Hockey => "Кубок «Кингсли»", 
				Sport.Basketball => "Кубок «Хэллоран»", 
				Sport.Football => "Кубок «Грейнджер»", 
				Sport.Baseball => "Вымпел «Основателей»", 
				_ => "Щит «Меридиан»", 
			}, 
		};
	}

	public static List<AwardDef> For(Sport sport, Level level)
	{
		List<AwardDef> list = new List<AwardDef>();
		switch (level)
		{
		case Level.HighSchool:
			list.Add(Mvp(sport, "Игрок года штата", "Приз лучшему игроку — " + SportRules.SportLabel(sport)));
			list.Add(Freshman(sport, "Первокурсник года", "Премия «Восходящая смена»"));
			list.Add(PlayoffMvp(sport, "Лучший игрок финала штата", "Лучший игрок финала штата"));
			return list;
		case Level.Minor:
			switch (sport)
			{
			case Sport.Hockey:
				list.Add(Mvp(sport, "Самый ценный игрок", "Кубок «Гаррисон»"));
				list.Add(Freshman(sport, "Новичок года", "Премия «Пелл»"));
				list.Add(PlayoffMvp(sport, "Самый ценный игрок плей-офф", "Приз лучшему игроку кубка «Риджуэй»"));
				break;
			case Sport.Basketball:
				list.Add(Mvp(sport, "Самый ценный игрок", "Премия «Корвин»"));
				list.Add(Freshman(sport, "Новичок года", "Премия «Стартовый бросок»"));
				list.Add(PlayoffMvp(sport, "Самый ценный игрок финала", "Приз лучшему игроку финала «Литейщик»"));
				break;
			default:
				list.Add(Mvp(sport, "Самый ценный игрок", "Премия «Липа»"));
				list.Add(Freshman(sport, "Новичок года", "Премия «Песчаная площадка»"));
				list.Add(PlayoffMvp(sport, "Самый ценный игрок чемпионата", "Приз лучшему игроку кубка «Маяк»"));
				break;
			}
			return list;
		case Level.Junior:
			list.Add(Mvp(sport, "Игрок года", "Кубок «Чистая вода»"));
			list.Add(Freshman(sport, "Новичок года", "Премия «Кэллоуэй»"));
			list.Add(PlayoffMvp(sport, "Самый ценный игрок плей-офф", "Приз лучшему игроку кубка «Доминион»"));
			return list;
		case Level.College:
			list.Add(Mvp(sport, "Игрок года страны", sport switch
			{
				Sport.Hockey => "Кубок «Крестовый»", 
				Sport.Basketball => "Премия «Холлис»", 
				Sport.Football => "Кубок «Стэнтон»", 
				Sport.Baseball => "Премия «Бриллиантовый шип»", 
				_ => "Кубок «Гэллоуэй»", 
			}));
			list.Add(Freshman(sport, "Первокурсник года", "Премия «Большое будущее»"));
			list.Add(PlayoffMvp(sport, "Лучший игрок турнира", "Лучший игрок турнира"));
			return list;
		default:
			switch (sport)
			{
			case Sport.Hockey:
				list.Add(Mvp(sport, "Самый ценный игрок", "Мемориальный кубок «Харроу»"));
				list.Add(new AwardDef
				{
					Award = "Лучший вратарь",
					Trophy = "Кубок Васильева",
					Eligible = (AwardContext c, Player p, int[] s) => !Skater(p) && HockeyQualifies(c, p, s),
					Score = (AwardContext c, Player p, int[] s) => GoalieValue(c, s),
					Detail = (AwardContext c, Player p, int[] s) => GoalieLine(s)
				});
				list.Add(new AwardDef
				{
					Award = "Лучший защитник",
					Trophy = "Кубок «Делакруа»",
					Eligible = (AwardContext c, Player p, int[] s) => p.Position == Position.Defense && HockeyQualifies(c, p, s),
					Score = (AwardContext c, Player p, int[] s) => SkaterValue(s) + (double)p.Overall * 0.8,
					Detail = (AwardContext c, Player p, int[] s) => SkaterLine(s)
				});
				list.Add(Freshman(sport, "Новичок года", "Кубок «Уитфилд»"));
				list.Add(new AwardDef
				{
					Award = "Лучший бомбардир",
					Trophy = "Кубок «Эшворт»",
					Eligible = (AwardContext c, Player p, int[] s) => Skater(p),
					Score = (AwardContext c, Player p, int[] s) => (double)(G(s, 1) + G(s, 2)) + (double)G(s, 1) * 0.01,
					Detail = (AwardContext c, Player p, int[] s) => SkaterLine(s)
				});
				list.Add(new AwardDef
				{
					Award = "Лучший снайпер",
					Trophy = "Кубок «Ламонт»",
					Eligible = (AwardContext c, Player p, int[] s) => Skater(p),
					Score = (AwardContext c, Player p, int[] s) => (double)G(s, 1) + (double)G(s, 2) * 0.01,
					Detail = (AwardContext c, Player p, int[] s) => I(G(s, 1)) + " голов"
				});
				list.Add(PlayoffMvp(sport, "Самый ценный игрок плей-офф", "Кубок «Корбо»"));
				break;
			case Sport.Basketball:
				list.Add(Mvp(sport, "Самый ценный игрок", "Кубок Максвелла Прескотта"));
				list.Add(new AwardDef
				{
					Award = "Лучший игрок обороны года",
					Trophy = "Кубок «Торнбери»",
					Eligible = (AwardContext c, Player p, int[] s) => HoopQualifies(c, s),
					Score = (AwardContext c, Player p, int[] s) => 3.0 * PerGame(s, 4) + 3.0 * PerGame(s, 5) + 0.5 * PerGame(s, 2) + (double)(p.Overall - 74) * 0.2,
					Detail = (AwardContext c, Player p, int[] s) => D1(PerGame(s, 4)) + " перехватов, " + D1(PerGame(s, 5)) + " блок-шотов, " + D1(PerGame(s, 2)) + " подборов за матч"
				});
				list.Add(Freshman(sport, "Новичок года", "Кубок «Эйнсли»"));
				list.Add(new AwardDef
				{
					Award = "Лучший запасной игрок года",
					Trophy = "Премия «Кэррэуэй»",
					Eligible = (AwardContext c, Player p, int[] s) => HoopQualifies(c, s) && G(s, 12) * 2 < G(s, 0),
					Score = (AwardContext c, Player p, int[] s) => HoopValue(s),
					Detail = (AwardContext c, Player p, int[] s) => HoopLine(s) + " выходя со скамейки запасных"
				});
				list.Add(new AwardDef
				{
					Award = "Самый прогрессирующий игрок",
					Trophy = "Премия «Прилив»",
					Eligible = (AwardContext c, Player p, int[] s) => HoopQualifies(c, s) && p.Overall - p.PrevOverall >= 3,
					Score = (AwardContext c, Player p, int[] s) => (double)((p.Overall - p.PrevOverall) * 3) + HoopValue(s) * 0.3,
					Detail = (AwardContext c, Player p, int[] s) => HoopLine(s) + ", рост рейтинга: " + I(p.Overall - p.PrevOverall)
				});
				list.Add(new AwardDef
				{
					Award = "Лучший бомбардир",
					Trophy = "Кубок «Фэрбанкс»",
					Eligible = (AwardContext c, Player p, int[] s) => HoopQualifies(c, s),
					Score = (AwardContext c, Player p, int[] s) => PerGame(s, 1),
					Detail = (AwardContext c, Player p, int[] s) => D1(PerGame(s, 1)) + " очков за матч"
				});
				list.Add(PlayoffMvp(sport, "Самый ценный игрок финала", "Кубок «Уитлок»"));
				break;
			case Sport.Football:
				list.Add(Mvp(sport, "Самый ценный игрок", "Кубок «Холкомб»"));
				list.Add(new AwardDef
				{
					Award = "Лучший игрок нападения года",
					Trophy = "Премия «Бреннан»",
					Eligible = (AwardContext c, Player p, int[] s) => Offense(p.Position),
					Score = (AwardContext c, Player p, int[] s) => (p.Position != Position.Quarterback) ? (SkillValue(s) * 1.3) : QbValue(s),
					Detail = (AwardContext c, Player p, int[] s) => OffenseLine(p, s)
				});
				list.Add(new AwardDef
				{
					Award = "Лучший игрок обороны года",
					Trophy = "Премия «Айронсайд»",
					Eligible = (AwardContext c, Player p, int[] s) => IsDefense(p.Position),
					Score = (AwardContext c, Player p, int[] s) => DefenseValue(s),
					Detail = (AwardContext c, Player p, int[] s) => DefenseLine(s)
				});
				list.Add(new AwardDef
				{
					Award = "Лучший новичок нападения",
					Trophy = "Премия «Кинкейд»",
					Eligible = (AwardContext c, Player p, int[] s) => Offense(p.Position) && Rookie(c, p),
					Score = (AwardContext c, Player p, int[] s) => OffenseValue(p, s),
					Detail = (AwardContext c, Player p, int[] s) => OffenseLine(p, s)
				});
				list.Add(new AwardDef
				{
					Award = "Лучший новичок обороны",
					Trophy = "Премия «Мерсер»",
					Eligible = (AwardContext c, Player p, int[] s) => IsDefense(p.Position) && Rookie(c, p),
					Score = (AwardContext c, Player p, int[] s) => DefenseValue(s),
					Detail = (AwardContext c, Player p, int[] s) => DefenseLine(s)
				});
				list.Add(new AwardDef
				{
					Award = "Лучший линейный игрок года",
					Trophy = "Щит «Слепая зона»",
					Eligible = (AwardContext c, Player p, int[] s) => p.Position == Position.OffensiveLine && G(s, 0) >= c.Half,
					Score = (AwardContext c, Player p, int[] s) =>
					{
						c.PointsPerGame.TryGetValue(p.TeamId, out var value);
						return (double)p.Overall + value * 0.3;
					},
					Detail = (AwardContext c, Player p, int[] s) => I(G(s, 0)) + " матчей, общий рейтинг " + I(Rosters.Ovr(p))
				});
				list.Add(PlayoffMvp(sport, "Самый ценный игрок главного матча", "Кубок «Марлоу»"));
				break;
			case Sport.Baseball:
				list.Add(Mvp(sport, "Самый ценный игрок", "Кубок «Уитакер»"));
				list.Add(new AwardDef
				{
					Award = "Лучший питчер",
					Trophy = "Премия «Холлистер»",
					Eligible = StarterQualifies,
					Score = (AwardContext c, Player p, int[] s) => PitcherValue(s) + (double)G(s, 16) * 0.5,
					Detail = (AwardContext c, Player p, int[] s) => PitcherLine(s)
				});
				list.Add(Freshman(sport, "Новичок года", "Премия «Пембертон»"));
				list.Add(new AwardDef
				{
					Award = "Реливер года",
					Trophy = "Премия «Пожарный»",
					Eligible = (AwardContext c, Player p, int[] s) => p.Position == Position.ReliefPitcher && G(s, 9) >= Math.Max(1, c.TeamGames / 4),
					Score = (AwardContext c, Player p, int[] s) => RelieverValue(s),
					Detail = (AwardContext c, Player p, int[] s) => I(G(s, 18)) + " сейвов, " + D2(Era(s)) + " пропущенных ранов за 9 иннингов"
				});
				list.Add(new AwardDef
				{
					Award = "Лучший отбивающий",
					Trophy = "Серебряная бита",
					Eligible = HitterQualifies,
					Score = (AwardContext c, Player p, int[] s) => HitterValue(s),
					Detail = (AwardContext c, Player p, int[] s) => HitterLine(s)
				});
				list.Add(PlayoffMvp(sport, "Самый ценный игрок серии", "Корона октября"));
				break;
			default:
				list.Add(Mvp(sport, "Самый ценный игрок", "Кубок «Кастельяно»"));
				list.Add(new AwardDef
				{
					Award = "Золотая бутса",
					Trophy = "Бутса «Феррейра»",
					Eligible = (AwardContext c, Player p, int[] s) => !Keeper(p),
					Score = (AwardContext c, Player p, int[] s) => (double)G(s, 1) + (double)G(s, 2) * 0.01,
					Detail = (AwardContext c, Player p, int[] s) => FieldLine(s)
				});
				list.Add(new AwardDef
				{
					Award = "Вратарь года",
					Trophy = "Золотая перчатка",
					Eligible = (AwardContext c, Player p, int[] s) => Keeper(p) && G(s, 8) >= c.Half,
					Score = (AwardContext c, Player p, int[] s) => KeeperValue(s),
					Detail = (AwardContext c, Player p, int[] s) => KeeperLine(s)
				});
				list.Add(new AwardDef
				{
					Award = "Защитник года",
					Trophy = "Премия «Роша»",
					Eligible = (AwardContext c, Player p, int[] s) => p.Position == Position.Defender && G(s, 0) >= c.Half,
					Score = (AwardContext c, Player p, int[] s) => (double)((p.Overall - 70) * 2 + G(s, 1) * 3 + G(s, 2) * 2) + c.TeamWinPct(p) * 10.0,
					Detail = (AwardContext c, Player p, int[] s) => I(G(s, 0)) + " матчей, " + FieldLine(s)
				});
				list.Add(new AwardDef
				{
					Award = "Молодой игрок года",
					Trophy = "Премия «Аврора»",
					Eligible = (AwardContext c, Player p, int[] s) => c.Year - p.BirthYear <= 22 && SportQualifies(Sport.Soccer, c, p, s),
					Score = (AwardContext c, Player p, int[] s) => SoccerValue(p, s),
					Detail = (AwardContext c, Player p, int[] s) => SoccerLine(p, s)
				});
				list.Add(Freshman(sport, "Лучший дебютант года", "Премия «Гавань»"));
				list.Add(PlayoffMvp(sport, "Самый ценный игрок финала кубка", "Приз лучшему игроку финала «Меридиан»"));
				break;
			}
			return list;
		}
	}
}
