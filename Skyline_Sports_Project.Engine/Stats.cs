using System;
using System.Collections.Generic;
using System.Globalization;

namespace Skyline_Sports_Project.Engine;

public static class Stats
{
	private class Box
	{
		public readonly Dictionary<Player, int[]> Lines = new Dictionary<Player, int[]>();

		public readonly Sport Sport;

		public readonly GameBox Game = new GameBox();

		public readonly List<Timeline.Raw> Raw = new List<Timeline.Raw>();

		public Box(Sport sport)
		{
			Sport = sport;
		}

		public int[] Of(Player p)
		{
			if (!Lines.TryGetValue(p, out var line))
			{
				line = Empty(Sport);
				Lines[p] = line;
			}
			return line;
		}

		public void Add(Player p, int stat, int amount)
		{
			if (p != null && amount != 0)
			{
				Of(p)[stat] += amount;
			}
		}
	}

	private class HockeyTeam
	{
		public readonly List<Player> Skaters = new List<Player>();

		public readonly Dictionary<Player, double> ScoreW = new Dictionary<Player, double>();

		public readonly Dictionary<Player, double> AssistW = new Dictionary<Player, double>();

		public readonly Dictionary<Player, double> ShotW = new Dictionary<Player, double>();

		public readonly List<List<Player>> Lines = new List<List<Player>>();

		public readonly List<List<Player>> Pairs = new List<List<Player>>();

		public Player Goalie;
	}

	private class HGoal
	{
		public Player Scorer;

		public Player First;

		public Player Second;

		public bool PowerPlay;

		public bool Home;
	}

	private class FootballScore
	{
		public int Touchdowns;

		public int FieldGoals;

		public int ExtraPoints;

		public int TwoPoint;

		public int Safeties;

		public int MissedXp;

		public int WalkOffs;
	}

	public class StatGroup
	{
		public string Name;

		public Position[] Positions;

		public string[] Columns;

		public Func<int[], string[]> Cells;
	}

	public class Leader
	{
		public string Title;

		public Func<Player, int[], bool> Qualifies;

		public Func<int[], double> Value;

		public Func<double, string> Format;

		public bool LowerIsBetter;
	}

	private class FbSide
	{
		public Player Qb;

		public Player Kicker;

		public Player Returner;

		public readonly List<Player> Receivers = new List<Player>();

		public readonly List<Player> Runners = new List<Player>();

		public readonly List<Player> Defense = new List<Player>();

		public readonly List<double> TargetW = new List<double>();

		public readonly List<double> CarryW = new List<double>();

		public readonly List<double> TdRunW = new List<double>();

		public readonly List<double> IntW = new List<double>();

		public readonly Dictionary<Player, int> CatchTds = new Dictionary<Player, int>();

		public readonly Dictionary<Player, int> CatchTdYards = new Dictionary<Player, int>();

		public readonly Dictionary<Player, int> RunTds = new Dictionary<Player, int>();

		public readonly Dictionary<Player, int> RunTdYards = new Dictionary<Player, int>();

		public readonly Dictionary<Player, int> ReturnedPicks = new Dictionary<Player, int>();
	}

	public const int H_GP = 0;

	public const int H_G = 1;

	public const int H_A = 2;

	public const int H_SOG = 3;

	public const int H_PIM = 4;

	public const int H_W = 5;

	public const int H_L = 6;

	public const int H_OTL = 7;

	public const int H_SA = 8;

	public const int H_GA = 9;

	public const int H_SO = 10;

	public const int H_GS = 11;

	public const int H_PM = 12;

	public const int H_LEN = 13;

	public const int B_GP = 0;

	public const int B_PTS = 1;

	public const int B_REB = 2;

	public const int B_AST = 3;

	public const int B_STL = 4;

	public const int B_BLK = 5;

	public const int B_FGM = 6;

	public const int B_FGA = 7;

	public const int B_3PM = 8;

	public const int B_FTM = 9;

	public const int B_FTA = 10;

	public const int B_MIN = 11;

	public const int B_GS = 12;

	public const int B_PM = 13;

	public const int B_LEN = 14;

	public const int F_GP = 0;

	public const int F_CMP = 1;

	public const int F_ATT = 2;

	public const int F_PYD = 3;

	public const int F_PTD = 4;

	public const int F_INT = 5;

	public const int F_CAR = 6;

	public const int F_RYD = 7;

	public const int F_RTD = 8;

	public const int F_REC = 9;

	public const int F_RECYD = 10;

	public const int F_RECTD = 11;

	public const int F_TCK = 12;

	public const int F_SACK = 13;

	public const int F_DINT = 14;

	public const int F_FGM = 15;

	public const int F_FGA = 16;

	public const int F_XPM = 17;

	public const int F_XPA = 18;

	public const int F_LEN = 19;

	public const int S_G = 0;

	public const int S_AB = 1;

	public const int S_R = 2;

	public const int S_H = 3;

	public const int S_2B = 4;

	public const int S_HR = 5;

	public const int S_RBI = 6;

	public const int S_BB = 7;

	public const int S_SO = 8;

	public const int S_PG = 9;

	public const int S_GS = 10;

	public const int S_OUTS = 11;

	public const int S_HA = 12;

	public const int S_ER = 13;

	public const int S_PBB = 14;

	public const int S_K = 15;

	public const int S_W = 16;

	public const int S_L = 17;

	public const int S_SV = 18;

	public const int S_LEN = 19;

	public const int C_GP = 0;

	public const int C_G = 1;

	public const int C_A = 2;

	public const int C_SH = 3;

	public const int C_YC = 4;

	public const int C_SV = 5;

	public const int C_GA = 6;

	public const int C_CS = 7;

	public const int C_GS = 8;

	public const int C_RC = 9;

	public const int C_LEN = 10;

	private static readonly double[] LineTime = new double[4] { 1.0, 0.8, 0.55, 0.35 };

	private static readonly double[] PairTime = new double[3] { 1.0, 0.8, 0.55 };

	public static int Length(Sport sport)
	{
		return sport switch
		{
			Sport.Hockey => 13, 
			Sport.Basketball => 14, 
			Sport.Football => 19, 
			Sport.Baseball => 19, 
			_ => 10, 
		};
	}

	public static int[] Empty(Sport sport)
	{
		return new int[Length(sport)];
	}

	public static int Get(int[] a, int i)
	{
		if (a == null || i >= a.Length)
		{
			return 0;
		}
		return a[i];
	}

	public static int[] Fit(int[] a, Sport sport)
	{
		int len = Length(sport);
		if (a != null && a.Length == len)
		{
			return a;
		}
		int[] r = new int[len];
		if (a != null)
		{
			Array.Copy(a, r, Math.Min(a.Length, len));
		}
		return r;
	}

	public static void AddInto(int[] target, int[] add)
	{
		for (int i = 0; i < target.Length && i < add.Length; i++)
		{
			target[i] += add[i];
		}
	}

	public static bool IsEmpty(int[] a)
	{
		if (a == null)
		{
			return true;
		}
		for (int i = 0; i < a.Length; i++)
		{
			if (a[i] != 0)
			{
				return false;
			}
		}
		return true;
	}

	public static string Record(Match g, Sport sport, Level level, Team homeTeam, Team awayTeam, List<Player> home, List<Player> away, Random random, bool addToSeason)
	{
		GameBox box = Build(g, sport, level, homeTeam, awayTeam, home, away, random, out var stars);
		if (box == null)
		{
			return "";
		}
		g.Box = box;
		Dictionary<int, Player> lookup = new Dictionary<int, Player>();
		foreach (Player p in home)
		{
			lookup[p.Id] = p;
		}
		foreach (Player p2 in away)
		{
			lookup[p2.Id] = p2;
		}
		Apply(box, (int id) => (!lookup.TryGetValue(id, out var value)) ? null : value, sport, addToSeason);
		return stars;
	}

	public static GameBox Build(Match g, Sport sport, Level level, Team homeTeam, Team awayTeam, List<Player> home, List<Player> away, Random random, out string stars)
	{
		stars = "";
		level = SportRules.Scale(level);
		if (!g.Played || home == null || away == null || home.Count == 0 || away.Count == 0)
		{
			return null;
		}
		Box box = new Box(sport);
		bool homeWon = g.WinnerId == g.HomeTeamId;
		switch (sport)
		{
		case Sport.Hockey:
			Hockey(box, g, level, homeTeam, awayTeam, home, away, homeWon, random);
			break;
		case Sport.Basketball:
			Basketball(box, g, level, home, away, homeWon, random);
			break;
		case Sport.Football:
			Football(box, g, level, home, away, random);
			break;
		case Sport.Baseball:
			Baseball(box, g, level, homeTeam, awayTeam, home, away, homeWon, random);
			break;
		default:
			Soccer(box, g, level, home, away, random);
			break;
		}
		HashSet<Player> homeSet = new HashSet<Player>(home);
		foreach (KeyValuePair<Player, int[]> kv in box.Lines)
		{
			box.Game.Lines.Add(new BoxLine
			{
				PlayerId = kv.Key.Id,
				Home = homeSet.Contains(kv.Key),
				Stats = (int[])kv.Value.Clone()
			});
		}
		stars = Stars(box);
		return box.Game;
	}

	public static void Apply(GameBox box, Func<int, Player> find, Sport sport, bool addToSeason)
	{
		if (box == null)
		{
			return;
		}
		foreach (BoxLine line in box.Lines)
		{
			Player p = find(line.PlayerId);
			if (p != null)
			{
				if (addToSeason)
				{
					p.Season = Fit(p.Season, sport);
					AddInto(p.Season, line.Stats);
				}
				else
				{
					p.Playoff = Fit(p.Playoff, sport);
					AddInto(p.Playoff, line.Stats);
				}
			}
		}
	}

	private static List<Player> ByPos(List<Player> roster, params Position[] positions)
	{
		List<Player> list = new List<Player>();
		foreach (Player p in roster)
		{
			if (Array.IndexOf(positions, p.Position) >= 0)
			{
				list.Add(p);
			}
		}
		list.Sort((Player a, Player b) => (b.Overall == a.Overall) ? a.Id.CompareTo(b.Id) : b.Overall.CompareTo(a.Overall));
		return list;
	}

	private static double Skill(Player p, double power)
	{
		return Math.Pow((double)Math.Max(20, p.Overall) / 70.0, power);
	}

	private static Player PickWeighted(List<Player> players, List<double> weights, Random random, Player exclude1 = null, Player exclude2 = null)
	{
		double total = 0.0;
		for (int i = 0; i < players.Count; i++)
		{
			if (players[i] != exclude1 && players[i] != exclude2)
			{
				total += weights[i];
			}
		}
		if (total <= 0.0)
		{
			return null;
		}
		double r = random.NextDouble() * total;
		for (int j = 0; j < players.Count; j++)
		{
			if (players[j] != exclude1 && players[j] != exclude2)
			{
				r -= weights[j];
				if (r <= 0.0)
				{
					return players[j];
				}
			}
		}
		for (int i2 = players.Count - 1; i2 >= 0; i2--)
		{
			if (players[i2] != exclude1 && players[i2] != exclude2)
			{
				return players[i2];
			}
		}
		return null;
	}

	private static int[] Split(int total, List<double> weights, Random random)
	{
		int[] parts = new int[weights.Count];
		if (total <= 0 || weights.Count == 0)
		{
			return parts;
		}
		double sum = 0.0;
		foreach (double w in weights)
		{
			sum += w;
		}
		if (sum <= 0.0)
		{
			return parts;
		}
		for (int k = 0; k < total; k++)
		{
			double r = random.NextDouble() * sum;
			int chosen = weights.Count - 1;
			for (int i = 0; i < weights.Count; i++)
			{
				r -= weights[i];
				if (r <= 0.0)
				{
					chosen = i;
					break;
				}
			}
			parts[chosen]++;
		}
		return parts;
	}

	private static int[] SplitLarge(int total, List<double> weights, Random random)
	{
		int[] parts = new int[weights.Count];
		if (weights.Count == 0)
		{
			return parts;
		}
		double sum = 0.0;
		List<double> noisy = new List<double>();
		foreach (double w in weights)
		{
			double v = ((w <= 0.0) ? 0.0 : (w * (0.6 + 0.8 * random.NextDouble())));
			noisy.Add(v);
			sum += v;
		}
		if (sum <= 0.0)
		{
			return parts;
		}
		int assigned = 0;
		int best = 0;
		for (int i = 0; i < parts.Length; i++)
		{
			parts[i] = (int)Math.Floor((double)total * noisy[i] / sum);
			assigned += parts[i];
			if (noisy[i] > noisy[best])
			{
				best = i;
			}
		}
		parts[best] += total - assigned;
		return parts;
	}

	private static int Poisson(Random random, double lambda)
	{
		if (lambda <= 0.0)
		{
			return 0;
		}
		double l = Math.Exp(0.0 - lambda);
		double p = 1.0;
		int k = 0;
		do
		{
			k++;
			p *= random.NextDouble();
		}
		while (p > l);
		return k - 1;
	}

	private static int NormalInt(Random random, double mean, double sd, int min)
	{
		return Math.Max(min, (int)Math.Round(Rosters.Normal(random, mean, sd)));
	}

	private static void Hockey(Box box, Match g, Level level, Team homeTeam, Team awayTeam, List<Player> home, List<Player> away, bool homeWon, Random random)
	{
		if (g.Flow != null && g.Flow.HomeStats.Length >= 5)
		{
			HockeyFromFlow(box, g, home, away, homeWon, random);
			return;
		}
		int homeGoals = g.HomeScore - (((g.Result == ResultType.Shootout) & homeWon) ? 1 : 0);
		int awayGoals = g.AwayScore - ((g.Result == ResultType.Shootout && !homeWon) ? 1 : 0);
		double shotMean = ((level == Level.HighSchool) ? 27 : 30);
		int homeShots = Math.Max(homeGoals + 4, NormalInt(random, shotMean, 5.0, 10));
		int awayShots = Math.Max(awayGoals + 4, NormalInt(random, shotMean, 5.0, 10));
		HockeyTeam h = HockeyLineup(box, home, random, randomPim: true, g.HomeGoalieId);
		HockeyTeam a = HockeyLineup(box, away, random, randomPim: true, g.AwayGoalieId);
		List<HGoal> goals = new List<HGoal>();
		for (int k = 0; k < homeGoals; k++)
		{
			HGoal x = HockeyGoal(box, h, a, home: true, random);
			if (x != null)
			{
				goals.Add(x);
			}
		}
		for (int i = 0; i < awayGoals; i++)
		{
			HGoal x2 = HockeyGoal(box, a, h, home: false, random);
			if (x2 != null)
			{
				goals.Add(x2);
			}
		}
		HockeyShots(box, h, homeShots - homeGoals, random);
		HockeyShots(box, a, awayShots - awayGoals, random);
		HockeyGoalie(box, h.Goalie, awayShots, awayGoals, homeWon, g.Result);
		HockeyGoalie(box, a.Goalie, homeShots, homeGoals, !homeWon, g.Result);
		HockeyTimeline(box, g, goals, homeWon, random);
	}

	private static List<double> WeightsOf(List<Player> players, Dictionary<Player, double> w)
	{
		List<double> list = new List<double>();
		foreach (Player p in players)
		{
			list.Add(w.TryGetValue(p, out var v) ? v : 0.0);
		}
		return list;
	}

	private static HockeyTeam HockeyLineup(Box box, List<Player> roster, Random random, bool randomPim = true, int goalieId = -1)
	{
		HockeyTeam t = new HockeyTeam();
		List<Player>[] forwards = new List<Player>[3]
		{
			ByPos(roster, default(Position)),
			ByPos(roster, Position.LeftWing),
			ByPos(roster, Position.RightWing)
		};
		for (int i = 0; i < 4; i++)
		{
			List<Player> line = new List<Player>();
			List<Player>[] array = forwards;
			foreach (List<Player> group in array)
			{
				if (i < group.Count)
				{
					Player p = group[i];
					double w = LineTime[i] * Skill(p, 3.0);
					t.Skaters.Add(p);
					t.ScoreW[p] = w;
					t.AssistW[p] = w * ((p.Position == Position.Center) ? 1.2 : 1.0);
					t.ShotW[p] = w;
					line.Add(p);
				}
			}
			if (line.Count > 0)
			{
				t.Lines.Add(line);
			}
		}
		List<Player> ds = ByPos(roster, Position.Defense);
		for (int k = 0; k < ds.Count && k < 6; k++)
		{
			double w2 = PairTime[k / 2] * Skill(ds[k], 3.0);
			t.Skaters.Add(ds[k]);
			t.ScoreW[ds[k]] = 0.28 * w2;
			t.AssistW[ds[k]] = 0.8 * w2;
			t.ShotW[ds[k]] = 0.6 * w2;
			if (k % 2 == 0)
			{
				t.Pairs.Add(new List<Player>());
			}
			t.Pairs[k / 2].Add(ds[k]);
		}
		foreach (Player p2 in t.Skaters)
		{
			box.Add(p2, 0, 1);
			if (randomPim)
			{
				double r = random.NextDouble();
				if (r < 0.012)
				{
					box.Add(p2, 4, 5);
				}
				else if (r < 0.1)
				{
					box.Add(p2, 4, 2);
				}
			}
		}
		List<Player> goalies = ByPos(roster, Position.Goalie);
		Player picked = ((goalieId >= 0) ? goalies.Find((Player player) => player.Id == goalieId) : null);
		if (picked != null)
		{
			t.Goalie = picked;
		}
		else if (goalies.Count > 0)
		{
			t.Goalie = ((goalies.Count > 1 && random.NextDouble() < 0.22) ? goalies[1] : goalies[0]);
		}
		return t;
	}

	private static int PickIndex(double[] weights, int n, Random random)
	{
		double sum = 0.0;
		for (int i = 0; i < n; i++)
		{
			sum += weights[i];
		}
		double r = random.NextDouble() * sum;
		for (int j = 0; j < n; j++)
		{
			r -= weights[j];
			if (r < 0.0)
			{
				return j;
			}
		}
		return n - 1;
	}

	private static List<Player> OnIce(HockeyTeam t, Player scorer, Random random)
	{
		List<Player> line = null;
		List<Player> pair = null;
		if (scorer != null)
		{
			foreach (List<Player> l in t.Lines)
			{
				if (l.Contains(scorer))
				{
					line = l;
				}
			}
			foreach (List<Player> d in t.Pairs)
			{
				if (d.Contains(scorer))
				{
					pair = d;
				}
			}
		}
		if (line == null && t.Lines.Count > 0)
		{
			line = t.Lines[PickIndex(LineTime, t.Lines.Count, random)];
		}
		if (pair == null && t.Pairs.Count > 0)
		{
			pair = t.Pairs[PickIndex(PairTime, t.Pairs.Count, random)];
		}
		List<Player> ice = new List<Player>();
		if (line != null)
		{
			ice.AddRange(line);
		}
		if (pair != null)
		{
			ice.AddRange(pair);
		}
		if (scorer != null && !ice.Contains(scorer))
		{
			ice.Add(scorer);
		}
		return ice;
	}

	private static HGoal HockeyGoal(Box box, HockeyTeam us, HockeyTeam them, bool home, Random random, int kind = -1)
	{
		Player scorer = PickWeighted(us.Skaters, WeightsOf(us.Skaters, us.ScoreW), random);
		if (scorer == null)
		{
			return null;
		}
		box.Add(scorer, 1, 1);
		box.Add(scorer, 3, 1);
		List<Player> ice = OnIce(us, scorer, random);
		List<Player> mates = ice.FindAll((Player player) => player != scorer);
		double r = random.NextDouble();
		int num = ((!(r < 0.08)) ? ((r < 0.35) ? 1 : 2) : 0);
		Player first = ((num > 0) ? PickWeighted(mates, WeightsOf(mates, us.AssistW), random) : null);
		if (first != null)
		{
			box.Add(first, 2, 1);
		}
		Player second = null;
		if (num > 1 && first != null)
		{
			second = PickWeighted(mates, WeightsOf(mates, us.AssistW), random, first);
			if (second != null)
			{
				box.Add(second, 2, 1);
			}
		}
		bool pp = ((kind >= 0) ? (kind == 2) : (random.NextDouble() < 0.19));
		if (!pp)
		{
			foreach (Player p in ice)
			{
				box.Add(p, 12, 1);
			}
			foreach (Player p2 in OnIce(them, null, random))
			{
				box.Add(p2, 12, -1);
			}
		}
		return new HGoal
		{
			Scorer = scorer,
			First = first,
			Second = second,
			PowerPlay = pp,
			Home = home
		};
	}

	private static void HockeyShots(Box box, HockeyTeam t, int shots, Random random)
	{
		int[] extra = Split(Math.Max(0, shots), WeightsOf(t.Skaters, t.ShotW), random);
		for (int i = 0; i < t.Skaters.Count; i++)
		{
			box.Add(t.Skaters[i], 3, extra[i]);
		}
	}

	private static void HockeyTimeline(Box box, Match g, List<HGoal> goals, bool homeWon, Random random)
	{
		bool ot = g.Result == ResultType.Overtime;
		bool so = g.Result == ResultType.Shootout;
		bool playoff = g.Stage != GameStage.Regular;
		HGoal otGoal = null;
		if (ot)
		{
			for (int i = goals.Count - 1; i >= 0; i--)
			{
				if (goals[i].Home == homeWon)
				{
					otGoal = goals[i];
					break;
				}
			}
		}
		int otPeriods = ((!ot) ? (so ? 1 : 0) : ((!playoff) ? 1 : ((random.NextDouble() < 0.7) ? 1 : ((random.NextDouble() < 0.75) ? 2 : 3))));
		foreach (HGoal x in goals)
		{
			Timeline.Raw r = new Timeline.Raw
			{
				Home = x.Home,
				Points = 1
			};
			r.Players.Add(x.Scorer.Id);
			if (x.First != null)
			{
				r.Players.Add(x.First.Id);
			}
			if (x.Second != null)
			{
				r.Players.Add(x.Second.Id);
			}
			string text = x.Scorer.Name;
			text = ((x.First == null) ? (text + " (без передачи)") : (text + " (" + x.First.ShortName + ((x.Second != null) ? (", " + x.Second.ShortName) : "") + ")"));
			if (x.PowerPlay)
			{
				text += "  БОЛ";
			}
			r.Text = text;
			if (x == otGoal)
			{
				r.Period = 3 + otPeriods;
				r.Seconds = random.Next(playoff ? 1200 : 300);
			}
			else
			{
				int t = random.Next(3600);
				r.Period = 1 + t / 1200;
				r.Seconds = t % 1200;
			}
			box.Raw.Add(r);
		}
		int periods = 3 + otPeriods + (so ? 1 : 0);
		Timeline.Finish(box.Game, Sport.Hockey, box.Raw, periods, periodsFromEvents: true);
		if (so)
		{
			int last = periods - 1;
			if (homeWon)
			{
				box.Game.HomePeriods[last] = 1;
			}
			else
			{
				box.Game.AwayPeriods[last] = 1;
			}
			int loser = random.Next(0, 3);
			box.Game.Note = "Серия буллитов " + (loser + 1) + "-" + loser;
		}
	}

	private static void HockeyGoalie(Box box, Player goalie, int shotsAgainst, int goalsAgainst, bool won, ResultType result)
	{
		if (goalie != null)
		{
			box.Add(goalie, 0, 1);
			box.Add(goalie, 11, 1);
			box.Add(goalie, 8, shotsAgainst);
			box.Add(goalie, 9, goalsAgainst);
			if (won)
			{
				box.Add(goalie, 5, 1);
			}
			else if (result == ResultType.Overtime || result == ResultType.Shootout)
			{
				box.Add(goalie, 7, 1);
			}
			else
			{
				box.Add(goalie, 6, 1);
			}
			if (won && goalsAgainst == 0)
			{
				box.Add(goalie, 10, 1);
			}
		}
	}

	private static void Basketball(Box box, Match g, Level level, List<Player> home, List<Player> away, bool homeWon, Random random)
	{
		int regulation = level switch
		{
			Level.College => 40, 
			Level.Pro => 48, 
			_ => 32, 
		};
		int num;
		int num2;
		if (g.Flow != null)
		{
			num = ((g.Flow.HomeLine.Count >= 4) ? 1 : 0);
			if (num != 0)
			{
				num2 = Math.Max(0, g.Flow.HomeLine.Count - 4);
				goto IL_0071;
			}
		}
		else
		{
			num = 0;
		}
		num2 = ((g.Result == ResultType.Overtime) ? (1 + ((random.NextDouble() < 0.15) ? 1 : 0)) : 0);
		goto IL_0071;
		IL_0071:
		int overtimes = num2;
		BasketballSide(box, home, g.HomeScore, g.AwayScore, level, regulation + 5 * overtimes, random);
		BasketballSide(box, away, g.AwayScore, g.HomeScore, level, regulation + 5 * overtimes, random);
		if (num != 0)
		{
			box.Game.HomePeriods = new List<int>(g.Flow.HomeLine);
			box.Game.AwayPeriods = new List<int>(g.Flow.AwayLine);
			box.Game.Flow.AddRange(g.Flow.Track);
		}
		else
		{
			BasketballPeriods(box, g, level, overtimes, random);
		}
	}

	private static void BasketballPeriods(Box box, Match g, Level level, int overtimes, Random random)
	{
		GameBox b = box.Game;
		if (overtimes == 0)
		{
			b.HomePeriods = Timeline.SplitPoints(g.HomeScore, 4, 0.18, random);
			b.AwayPeriods = Timeline.SplitPoints(g.AwayScore, 4, 0.18, random);
			return;
		}
		bool homeWon = g.HomeScore > g.AwayScore;
		int winner = Math.Max(g.HomeScore, g.AwayScore);
		int loser = Math.Min(g.HomeScore, g.AwayScore);
		double perOt = (double)(level switch
		{
			Level.College => 72, 
			Level.Pro => 112, 
			_ => 56, 
		}) * 5.0 / 48.0;
		int last = Math.Max(2, Math.Min(loser / 4, (int)Math.Round(Rosters.Normal(random, perOt, 3.0))));
		int earlier = Math.Max(2, Math.Min(loser / 6, (int)Math.Round(Rosters.Normal(random, perOt, 3.0))));
		int tie = loser - last - earlier * (overtimes - 1);
		if (tie < loser / 2)
		{
			last = 2;
			earlier = 2;
			tie = loser - last - earlier * (overtimes - 1);
		}
		List<int> w = Timeline.SplitPoints(tie, 4, 0.18, random);
		List<int> l = Timeline.SplitPoints(tie, 4, 0.18, random);
		for (int k = 0; k < overtimes - 1; k++)
		{
			w.Add(earlier);
			l.Add(earlier);
		}
		w.Add(last + winner - loser);
		l.Add(last);
		b.HomePeriods = (homeWon ? w : l);
		b.AwayPeriods = (homeWon ? l : w);
	}

	private static void BasketballSide(Box box, List<Player> roster, int points, int allowed, Level level, int gameMinutes, Random random)
	{
		List<Player> starters = new List<Player>();
		Position[] array = new Position[5]
		{
			Position.PointGuard,
			Position.ShootingGuard,
			Position.SmallForward,
			Position.PowerForward,
			Position.BigMan
		};
		foreach (Position pos in array)
		{
			List<Player> list = ByPos(roster, pos);
			if (list.Count > 0)
			{
				starters.Add(list[0]);
			}
		}
		List<Player> rest = new List<Player>();
		foreach (Player p in roster)
		{
			if (!starters.Contains(p))
			{
				rest.Add(p);
			}
		}
		rest.Sort((Player a, Player b) => b.Overall.CompareTo(a.Overall));
		List<Player> active = new List<Player>(starters);
		for (int i2 = 0; i2 < rest.Count && i2 < 5; i2++)
		{
			active.Add(rest[i2]);
		}
		int total = gameMinutes * 5;
		List<double> minuteW = new List<double>();
		foreach (Player p2 in active)
		{
			minuteW.Add((starters.Contains(p2) ? 2.6 : 1.0) * Skill(p2, 1.0));
		}
		int[] minutes = SplitLarge(total, minuteW, random);
		List<double> usage = new List<double>();
		List<double> threeW = new List<double>();
		List<double> twoW = new List<double>();
		List<double> rebW = new List<double>();
		List<double> astW = new List<double>();
		List<double> blkW = new List<double>();
		for (int i3 = 0; i3 < active.Count; i3++)
		{
			Player p3 = active[i3];
			double u = (double)minutes[i3] * Skill(p3, 3.0);
			usage.Add(u);
			bool guard = p3.Position == Position.PointGuard || p3.Position == Position.ShootingGuard;
			bool big = p3.Position == Position.BigMan || p3.Position == Position.PowerForward;
			threeW.Add(u * (guard ? 1.4 : ((p3.Position == Position.SmallForward) ? 1.1 : ((p3.Position == Position.PowerForward) ? 0.6 : 0.25))));
			twoW.Add(u * (big ? 1.2 : 1.0));
			rebW.Add((double)minutes[i3] * ((p3.Position == Position.BigMan) ? 2.0 : ((p3.Position == Position.PowerForward) ? 1.6 : ((p3.Position == Position.SmallForward) ? 1.0 : ((p3.Position == Position.ShootingGuard) ? 0.7 : 0.6)))));
			astW.Add((double)minutes[i3] * ((p3.Position == Position.PointGuard) ? 2.6 : ((p3.Position == Position.ShootingGuard) ? 1.2 : ((p3.Position == Position.SmallForward) ? 1.0 : 0.6))) * Skill(p3, 1.0));
			blkW.Add((double)minutes[i3] * (big ? 2.0 : 0.4));
		}
		int threes = (int)Math.Round((double)points * level switch
		{
			Level.College => 0.11, 
			Level.Pro => 0.12, 
			_ => 0.09, 
		});
		int fts = (int)Math.Round((double)points * 0.16);
		int rem = points - 3 * threes - fts;
		if (rem < 0)
		{
			threes = 0;
			fts = points % 2;
			rem = points - fts;
		}
		if (rem % 2 != 0)
		{
			fts++;
			rem--;
		}
		int twos = rem / 2;
		int[] made3 = Split(threes, threeW, random);
		int[] made4 = Split(twos, twoW, random);
		int[] madeFt = Split(fts, usage, random);
		double rebMean = level switch
		{
			Level.College => 36, 
			Level.Pro => 44, 
			_ => 31, 
		};
		int[] reb = Split(NormalInt(random, rebMean, 5.0, 15), rebW, random);
		int[] ast = Split((int)Math.Round((double)(threes + twos) * (0.55 + 0.1 * random.NextDouble())), astW, random);
		int[] stl = Split(NormalInt(random, 7.0, 2.0, 0), minuteW, random);
		int[] blk = Split(NormalInt(random, (level == Level.Pro) ? 5.0 : 3.5, 2.0, 0), blkW, random);
		for (int i4 = 0; i4 < active.Count; i4++)
		{
			Player p4 = active[i4];
			if (minutes[i4] > 0)
			{
				int fgm = made4[i4] + made3[i4];
				int fga = fgm + (int)Math.Round((double)fgm * (0.9 + 0.5 * random.NextDouble())) + ((random.NextDouble() < 0.5) ? 1 : 0);
				int fta = madeFt[i4] + (int)Math.Round((double)madeFt[i4] * 0.3 * random.NextDouble()) + ((random.NextDouble() < 0.3) ? 1 : 0);
				box.Add(p4, 0, 1);
				if (starters.Contains(p4))
				{
					box.Add(p4, 12, 1);
				}
				box.Add(p4, 11, minutes[i4]);
				box.Add(p4, 1, 2 * made4[i4] + 3 * made3[i4] + madeFt[i4]);
				box.Add(p4, 6, fgm);
				box.Add(p4, 7, fga);
				box.Add(p4, 8, made3[i4]);
				box.Add(p4, 9, madeFt[i4]);
				box.Add(p4, 10, fta);
				box.Add(p4, 2, reb[i4]);
				box.Add(p4, 3, ast[i4]);
				box.Add(p4, 4, stl[i4]);
				box.Add(p4, 5, blk[i4]);
			}
		}
		int margin = points - allowed;
		int target = 5 * margin;
		int sum = 0;
		int most = -1;
		int[] pm = new int[active.Count];
		for (int i5 = 0; i5 < active.Count; i5++)
		{
			if (minutes[i5] > 0)
			{
				double share = (double)minutes[i5] / (double)Math.Max(1, gameMinutes);
				pm[i5] = (int)Math.Round((double)margin * share + Rosters.Normal(random, 0.0, 3.5 * Math.Sqrt(share)));
				sum += pm[i5];
				if (most < 0 || minutes[i5] > minutes[most])
				{
					most = i5;
				}
			}
		}
		List<int> order = new List<int>();
		for (int i6 = 0; i6 < active.Count; i6++)
		{
			if (minutes[i6] > 0)
			{
				order.Add(i6);
			}
		}
		order.Sort((int x, int y) => minutes[y].CompareTo(minutes[x]));
		int top = Math.Min(5, order.Count);
		int k = 0;
		while (most >= 0 && sum != target && top > 0)
		{
			int step = ((target > sum) ? 1 : (-1));
			pm[order[k % top]] += step;
			sum += step;
			k++;
		}
		for (int i7 = 0; i7 < active.Count; i7++)
		{
			if (minutes[i7] > 0)
			{
				box.Add(active[i7], 13, pm[i7]);
			}
		}
	}

	private static FootballScore Decompose(int score)
	{
		FootballScore best = new FootballScore();
		double bestCost = double.MaxValue;
		for (int td = score / 6; td >= 0; td--)
		{
			for (int miss = 0; miss <= td; miss++)
			{
				for (int two = 0; two + miss <= td; two++)
				{
					int pts = td * 6 + (td - miss - two) + two * 2;
					for (int safety = 0; safety <= 1; safety++)
					{
						int rest = score - pts - 2 * safety;
						if (rest >= 0 && rest % 3 == 0)
						{
							int fg = rest / 3;
							double cost = (double)miss * 2.5 + (double)two * 1.0 + (double)(safety * 4) + (double)Math.Max(0, fg - 2) * 0.45 + (double)Math.Min(fg, 2) * 0.15;
							if (cost < bestCost)
							{
								bestCost = cost;
								best = new FootballScore
								{
									Touchdowns = td,
									ExtraPoints = td - miss - two,
									TwoPoint = two,
									MissedXp = miss,
									FieldGoals = fg,
									Safeties = safety
								};
							}
						}
					}
				}
			}
		}
		return best;
	}

	private static FootballScore DecomposeOt(int regulation, int margin)
	{
		FootballScore s = Decompose(regulation);
		switch (margin)
		{
		case 3:
			s.FieldGoals++;
			break;
		case 6:
			s.Touchdowns++;
			s.WalkOffs++;
			break;
		case 7:
			s.Touchdowns++;
			s.ExtraPoints++;
			break;
		default:
			s = Decompose(regulation + margin);
			break;
		}
		return s;
	}

	private static void Football(Box box, Match g, Level level, List<Player> home, List<Player> away, Random random)
	{
		if (g.Flow != null && g.Flow.HomeStats.Length >= 18)
		{
			FootballFromFlow(box, g, home, away, random);
			return;
		}
		List<Timeline.Raw> homePlays = new List<Timeline.Raw>();
		List<Timeline.Raw> awayPlays = new List<Timeline.Raw>();
		bool flag = g.Result == ResultType.Overtime;
		int margin = Math.Abs(g.HomeScore - g.AwayScore);
		bool homeWon = g.HomeScore > g.AwayScore;
		FootballScore homeScore = ((flag & homeWon) ? DecomposeOt(g.HomeScore - margin, margin) : Decompose(g.HomeScore));
		FootballScore awayScore = ((flag && !homeWon) ? DecomposeOt(g.AwayScore - margin, margin) : Decompose(g.AwayScore));
		int homeInts = FootballOffense(box, home, homeScore, level, random, homePlays, home: true);
		int awayInts = FootballOffense(box, away, awayScore, level, random, awayPlays, home: false);
		FootballDefense(box, home, awayInts, random);
		FootballDefense(box, away, homeInts, random);
		foreach (Player p in home)
		{
			box.Add(p, 0, 1);
		}
		foreach (Player p2 in away)
		{
			box.Add(p2, 0, 1);
		}
		FootballTimeline(box, g, homePlays, awayPlays, random);
	}

	private static void FootballTimeline(Box box, Match g, List<Timeline.Raw> homePlays, List<Timeline.Raw> awayPlays, Random random)
	{
		bool ot = g.Result == ResultType.Overtime;
		bool homeWon = g.HomeScore > g.AwayScore;
		Timeline.Raw decider = null;
		if (ot)
		{
			List<Timeline.Raw> winner = (homeWon ? homePlays : awayPlays);
			int margin = Math.Abs(g.HomeScore - g.AwayScore);
			decider = ((margin == 6) ? winner.Find((Timeline.Raw raw) => raw.Points == 6 && !raw.Text.EndsWith(")")) : null) ?? winner.Find((Timeline.Raw raw) => raw.Points == margin) ?? ((winner.Count > 0) ? winner[winner.Count - 1] : null);
		}
		List<Timeline.Raw> all = new List<Timeline.Raw>(homePlays);
		all.AddRange(awayPlays);
		for (int k = all.Count - 1; k > 0; k--)
		{
			int x = random.Next(k + 1);
			Timeline.Raw tmp = all[k];
			all[k] = all[x];
			all[x] = tmp;
		}
		List<Timeline.Raw>[] quarters = new List<Timeline.Raw>[4]
		{
			new List<Timeline.Raw>(),
			new List<Timeline.Raw>(),
			new List<Timeline.Raw>(),
			new List<Timeline.Raw>()
		};
		foreach (Timeline.Raw r in all)
		{
			if (r == decider)
			{
				r.Period = 5;
				r.Seconds = random.Next(30, 600);
				if (r.Text.EndsWith(" (удар неудачен)"))
				{
					r.Text = r.Text.Substring(0, r.Text.Length - 14);
				}
				box.Raw.Add(r);
				continue;
			}
			int q = random.Next(4);
			for (int tries = 0; tries < 4; tries++)
			{
				if (quarters[q].Count < 4)
				{
					break;
				}
				q = random.Next(4);
			}
			quarters[q].Add(r);
		}
		for (int q2 = 0; q2 < 4; q2++)
		{
			int n = quarters[q2].Count;
			List<int> slots = new List<int>();
			for (int k2 = 0; k2 < 12; k2++)
			{
				slots.Add(k2);
			}
			for (int k3 = slots.Count - 1; k3 > 0; k3--)
			{
				int x2 = random.Next(k3 + 1);
				int tmp2 = slots[k3];
				slots[k3] = slots[x2];
				slots[x2] = tmp2;
			}
			for (int k4 = 0; k4 < n; k4++)
			{
				Timeline.Raw r2 = quarters[q2][k4];
				r2.Period = q2 + 1;
				r2.Seconds = ((k4 < 12) ? (slots[k4] * 75 + random.Next(0, 60)) : random.Next(900));
				box.Raw.Add(r2);
			}
		}
		Timeline.Finish(box.Game, Sport.Football, box.Raw, ot ? 5 : 4, periodsFromEvents: true);
	}

	private static int Yards(Random random, int max, double typical)
	{
		return Math.Max(1, Math.Min(max, (int)Math.Round(Math.Abs(Rosters.Normal(random, 0.0, typical))) + 1));
	}

	private static int FootballOffense(Box box, List<Player> roster, FootballScore score, Level level, Random random, List<Timeline.Raw> plays, bool home)
	{
		List<Player> qbs = ByPos(roster, Position.Quarterback);
		List<Player> rbs = ByPos(roster, Position.RunningBack);
		List<Player> wrs = ByPos(roster, Position.WideReceiver);
		List<Player> tes = ByPos(roster, Position.TightEnd);
		List<Player> ks = ByPos(roster, Position.Kicker);
		Player qb;
		if (qbs.Count != 0)
		{
			qb = ((qbs.Count > 1 && random.NextDouble() < 0.04) ? qbs[1] : qbs[0]);
		}
		else
		{
			qb = null;
		}
		List<Player> receivers = new List<Player>();
		List<double> targetW = new List<double>();
		double[] wrRank = new double[5] { 1.0, 0.75, 0.5, 0.2, 0.1 };
		for (int i = 0; i < wrs.Count && i < 5; i++)
		{
			receivers.Add(wrs[i]);
			targetW.Add(wrRank[i] * Skill(wrs[i], 2.0));
		}
		if (tes.Count > 0)
		{
			receivers.Add(tes[0]);
			targetW.Add(0.55 * Skill(tes[0], 2.0));
		}
		if (tes.Count > 1)
		{
			receivers.Add(tes[1]);
			targetW.Add(0.12);
		}
		if (rbs.Count > 0)
		{
			receivers.Add(rbs[0]);
			targetW.Add(0.35);
		}
		if (rbs.Count > 1)
		{
			receivers.Add(rbs[1]);
			targetW.Add(0.1);
		}
		List<Player> runners = new List<Player>();
		List<double> carryW = new List<double>();
		if (rbs.Count > 0)
		{
			runners.Add(rbs[0]);
			carryW.Add(0.66);
		}
		if (rbs.Count > 1)
		{
			runners.Add(rbs[1]);
			carryW.Add(0.22);
		}
		if (qb != null)
		{
			runners.Add(qb);
			carryW.Add(0.08);
		}
		if (wrs.Count > 0)
		{
			runners.Add(wrs[0]);
			carryW.Add(0.04);
		}
		int passTd = 0;
		int rushTd = 0;
		List<int> pats = new List<int>();
		for (int k = 0; k < score.ExtraPoints; k++)
		{
			pats.Add(1);
		}
		for (int j = 0; j < score.TwoPoint; j++)
		{
			pats.Add(2);
		}
		for (int l = 0; l < score.MissedXp; l++)
		{
			pats.Add(0);
		}
		for (int k2 = pats.Count - 1; k2 > 0; k2--)
		{
			int x = random.Next(k2 + 1);
			int tmp = pats[k2];
			pats[k2] = pats[x];
			pats[x] = tmp;
		}
		for (int m = 0; m < score.WalkOffs; m++)
		{
			pats.Add(-1);
		}
		Player kicker = ((ks.Count > 0) ? ks[0] : null);
		List<Player> backs = ByPos(roster, Position.Cornerback, Position.Safety, Position.Linebacker);
		for (int n = 0; n < score.Touchdowns; n++)
		{
			double r = random.NextDouble();
			List<int> who = new List<int>();
			string text;
			if (r < 0.58 && qb != null && receivers.Count > 0)
			{
				passTd++;
				Player catcher = PickWeighted(receivers, targetW, random);
				box.Add(catcher, 11, 1);
				text = catcher.Name + " " + Yards(random, 80, 22.0) + " ярдовый пас от " + qb.ShortName;
				who.Add(catcher.Id);
				who.Add(qb.Id);
			}
			else if (r < 0.95 && runners.Count > 0)
			{
				rushTd++;
				List<double> tdW = new List<double>();
				foreach (Player p in runners)
				{
					List<double> list = tdW;
					double item;
					if (p == qb)
					{
						item = 0.12;
					}
					else
					{
						item = ((rbs.Count > 0 && p == rbs[0]) ? 0.7 : 0.18);
					}
					list.Add(item);
				}
				Player runner = PickWeighted(runners, tdW, random);
				box.Add(runner, 8, 1);
				text = runner.Name + " " + Yards(random, 70, 9.0) + " ярдовый вынос";
				who.Add(runner.Id);
			}
			else
			{
				Player d = ((backs.Count > 0) ? backs[random.Next(Math.Min(backs.Count, 6))] : null);
				string how;
				if (random.NextDouble() < 0.6)
				{
					how = "возврат перехвата";
				}
				else
				{
					how = ((random.NextDouble() < 0.5) ? "возврат после потери мяча" : "возврат панта");
				}
				text = ((d != null) ? (d.Name + " ") : "") + Yards(random, 99, 35.0) + " ярд " + how;
			}
			int pat = ((n >= pats.Count) ? 1 : pats[n]);
			text += pat switch
			{
				0 => " (удар неудачен)", 
				2 => " (реализация двухочкового)", 
				1 => " (" + ((kicker != null) ? (kicker.ShortName + " ") : "") + "удар)", 
				_ => "", 
			};
			if (plays != null)
			{
				Timeline.Raw td = new Timeline.Raw
				{
					Home = home,
					Points = 6 + Math.Max(0, pat),
					Text = text
				};
				td.Players.AddRange(who);
				plays.Add(td);
			}
		}
		if (plays != null)
		{
			for (int num = 0; num < score.FieldGoals; num++)
			{
				int yds = 18 + random.Next(0, 38);
				Timeline.Raw fg = new Timeline.Raw
				{
					Home = home,
					Points = 3,
					Text = ((kicker != null) ? (kicker.Name + " ") : "") + yds + " ярдовый филд-гол"
				};
				if (kicker != null)
				{
					fg.Players.Add(kicker.Id);
				}
				plays.Add(fg);
			}
			for (int num2 = 0; num2 < score.Safeties; num2++)
			{
				plays.Add(new Timeline.Raw
				{
					Home = home,
					Points = 2,
					Text = "Сейфти"
				});
			}
		}
		double scale = level switch
		{
			Level.College => 1.05, 
			Level.HighSchool => 0.72, 
			_ => 1.0, 
		};
		int passYds = Math.Max(passTd * 12, NormalInt(random, 225.0 * scale + (double)(12 * passTd), 60.0, 40));
		int completions = Math.Max(passTd, Math.Max(5, (int)Math.Round((double)passYds / Math.Max(7.0, Rosters.Normal(random, 11.0, 1.5)))));
		int attempts = Math.Max(completions, (int)Math.Round((double)completions / Math.Max(0.45, Math.Min(0.8, Rosters.Normal(random, 0.64, 0.06)))));
		int ints = Poisson(random, 0.8);
		int carries = NormalInt(random, 26.0, 4.0, 12);
		int rushYds = Math.Max(rushTd * 3, NormalInt(random, 115.0 * scale + (double)(8 * rushTd), 35.0, 10));
		if (qb != null)
		{
			box.Add(qb, 1, completions);
			box.Add(qb, 2, attempts);
			box.Add(qb, 3, passYds);
			box.Add(qb, 4, passTd);
			box.Add(qb, 5, ints);
		}
		int[] catches = new int[receivers.Count];
		int left = completions;
		for (int num3 = 0; num3 < receivers.Count; num3++)
		{
			int tds = (box.Lines.ContainsKey(receivers[num3]) ? box.Of(receivers[num3])[11] : 0);
			catches[num3] = Math.Min(tds, left);
			left -= catches[num3];
		}
		int[] more = Split(left, targetW, random);
		List<double> yardW = new List<double>();
		for (int num4 = 0; num4 < receivers.Count; num4++)
		{
			catches[num4] += more[num4];
			yardW.Add((double)catches[num4] * ((receivers[num4].Position == Position.WideReceiver) ? 1.25 : ((receivers[num4].Position == Position.TightEnd) ? 1.0 : 0.7)));
		}
		int[] recYds = SplitLarge(passYds, yardW, random);
		for (int num5 = 0; num5 < receivers.Count; num5++)
		{
			box.Add(receivers[num5], 9, catches[num5]);
			box.Add(receivers[num5], 10, recYds[num5]);
		}
		int[] carriesSplit = Split(carries, carryW, random);
		List<double> rushW = new List<double>();
		for (int num6 = 0; num6 < runners.Count; num6++)
		{
			rushW.Add((double)carriesSplit[num6] * Skill(runners[num6], 1.0));
		}
		int[] rushSplit = SplitLarge(rushYds, rushW, random);
		for (int num7 = 0; num7 < runners.Count; num7++)
		{
			box.Add(runners[num7], 6, carriesSplit[num7]);
			box.Add(runners[num7], 7, rushSplit[num7]);
		}
		if (ks.Count > 0)
		{
			box.Add(ks[0], 15, score.FieldGoals);
			box.Add(ks[0], 16, score.FieldGoals + Poisson(random, 0.35));
			box.Add(ks[0], 17, score.ExtraPoints);
			box.Add(ks[0], 18, score.ExtraPoints + score.MissedXp);
		}
		return ints;
	}

	private static void FootballDefense(Box box, List<Player> roster, int interceptions, Random random)
	{
		List<Player> defenders = new List<Player>();
		List<double> tackleW = new List<double>();
		List<double> sackW = new List<double>();
		List<double> intW = new List<double>();
		AddDefenders(defenders, tackleW, sackW, intW, ByPos(roster, Position.Linebacker), 3, 2.5, 1.0, 0.5);
		AddDefenders(defenders, tackleW, sackW, intW, ByPos(roster, Position.Safety), 2, 1.8, 0.15, 1.2);
		AddDefenders(defenders, tackleW, sackW, intW, ByPos(roster, Position.Cornerback), 3, 1.3, 0.05, 1.5);
		AddDefenders(defenders, tackleW, sackW, intW, ByPos(roster, Position.DefensiveLine), 4, 1.0, 1.5, 0.05);
		int[] tackles = Split(NormalInt(random, 60.0, 8.0, 30), tackleW, random);
		int[] sacks = Split(Poisson(random, 2.4), sackW, random);
		int[] picks = Split(interceptions, intW, random);
		for (int i = 0; i < defenders.Count; i++)
		{
			box.Add(defenders[i], 12, tackles[i]);
			box.Add(defenders[i], 13, sacks[i]);
			box.Add(defenders[i], 14, picks[i]);
		}
	}

	private static void AddDefenders(List<Player> defenders, List<double> tackleW, List<double> sackW, List<double> intW, List<Player> group, int starters, double t, double s, double i)
	{
		for (int k = 0; k < group.Count; k++)
		{
			double depth = ((k < starters) ? 1.0 : 0.15);
			double skill = Skill(group[k], 2.0);
			defenders.Add(group[k]);
			tackleW.Add(t * depth * skill);
			sackW.Add(s * depth * skill);
			intW.Add(i * depth * skill);
		}
	}

	private static void Baseball(Box box, Match g, Level level, Team homeTeam, Team awayTeam, List<Player> home, List<Player> away, bool homeWon, Random random)
	{
		bool extra = g.Result == ResultType.ExtraInnings;
		int awayOuts = (extra ? 30 : 27);
		int homeOuts;
		if (homeWon)
		{
			homeOuts = (extra ? 29 : 24);
		}
		else
		{
			homeOuts = awayOuts;
		}
		if (g.Flow != null && g.Flow.AwayLine.Count >= 9)
		{
			int innings = g.Flow.AwayLine.Count;
			awayOuts = 3 * innings;
			if (((g.Flow.HomeLine.Count > 0 && g.Flow.HomeLine[g.Flow.HomeLine.Count - 1] != 0) ? 1 : 0) < (false ? 1 : 0))
			{
				homeOuts = 3 * (innings - 1);
			}
			else
			{
				homeOuts = ((!homeWon) ? (3 * innings) : (3 * (innings - 1) + random.Next(0, 3)));
			}
		}
		BaseballBatting(box, home, g.HomeScore, homeOuts, level, random, out var homeHits, out var homeWalks, out var homeK);
		BaseballBatting(box, away, g.AwayScore, awayOuts, level, random, out var awayHits, out var awayWalks, out var awayK);
		BaseballPitching(box, homeTeam, home, awayOuts, awayHits, awayWalks, awayK, g.AwayScore, homeWon, Math.Abs(g.HomeScore - g.AwayScore), level, random);
		BaseballPitching(box, awayTeam, away, homeOuts, homeHits, homeWalks, homeK, g.HomeScore, !homeWon, Math.Abs(g.HomeScore - g.AwayScore), level, random);
		BaseballTimeline(box, g, Lineup(home), Lineup(away), homeWon, random);
	}

	private static void BaseballTimeline(Box box, Match g, List<Player> homeLineup, List<Player> awayLineup, bool homeWon, Random random)
	{
		bool extra = g.Result == ResultType.ExtraInnings;
		bool flow = g.Flow != null && g.Flow.AwayLine.Count >= 9;
		int innings;
		if (flow)
		{
			innings = g.Flow.AwayLine.Count;
		}
		else
		{
			innings = (extra ? (10 + ((!(random.NextDouble() < 0.7)) ? ((random.NextDouble() < 0.67) ? 1 : 2) : 0)) : 9);
		}
		int[] away = new int[innings];
		int[] home = new int[innings];
		int awayReg = (extra ? Math.Min(g.HomeScore, g.AwayScore) : g.AwayScore);
		int homeReg = (extra ? awayReg : g.HomeScore);
		Action<int[], int, int> spread = (int[] line, int num, int upTo) =>
		{
			for (int j = 0; j < num; j++)
			{
				line[random.Next(upTo)]++;
			}
		};
		bool noBottomNinth = !extra & homeWon;
		if (flow)
		{
			for (int i = 0; i < innings; i++)
			{
				away[i] = g.Flow.AwayLine[i];
				home[i] = ((i < g.Flow.HomeLine.Count) ? g.Flow.HomeLine[i] : (-1));
			}
			noBottomNinth = home[innings - 1] < 0;
		}
		else
		{
			spread(away, awayReg, 9);
			spread(home, homeReg, noBottomNinth ? 8 : 9);
			if (extra)
			{
				int margin = Math.Abs(g.HomeScore - g.AwayScore);
				if (homeWon)
				{
					home[innings - 1] += margin;
				}
				else
				{
					away[innings - 1] += margin;
				}
			}
		}
		box.Game.AwayPeriods = new List<int>(away);
		box.Game.HomePeriods = new List<int>(home);
		if (noBottomNinth)
		{
			box.Game.HomePeriods[innings - 1] = -1;
		}
		Dictionary<Player, int> rbi = new Dictionary<Player, int>();
		Dictionary<Player, int> hr = new Dictionary<Player, int>();
		List<Player> list = new List<Player>(homeLineup);
		list.AddRange(awayLineup);
		foreach (Player p in list)
		{
			if (box.Lines.ContainsKey(p))
			{
				rbi[p] = box.Lines[p][6];
				hr[p] = box.Lines[p][5];
			}
		}
		for (int inning = 0; inning < innings; inning++)
		{
			for (int half = 0; half < 2; half++)
			{
				bool isHome = half == 1;
				int runs = (isHome ? Math.Max(0, home[inning]) : away[inning]);
				if (runs <= 0)
				{
					continue;
				}
				List<Player> lineup = (isHome ? homeLineup : awayLineup);
				string when = (isHome ? "Низ " : "Верх ") + Timeline.Ordinal(inning + 1);
				int order = 0;
				Player previous = null;
				while (runs > 0)
				{
					List<double> w = new List<double>();
					foreach (Player p2 in lineup)
					{
						w.Add(rbi.TryGetValue(p2, out var v) ? v : 0);
					}
					Player batter = PickWeighted(lineup, w, random, previous) ?? PickWeighted(lineup, w, random);
					previous = batter;
					int k;
					string text;
					if (batter == null)
					{
						k = runs;
						string[] how = new string[4] { "ошибку", "дикий бросок", "пропущенный мяч", "выбор филдера" };
						text = ((k == 1) ? "Ран засчитан" : (k + " рана засчитано")) + " после " + how[random.Next(how.Length)];
					}
					else
					{
						int most = Math.Min(4, Math.Min(runs, rbi[batter]));
						k = 1 + random.Next(most);
						if (hr[batter] > 0 && random.NextDouble() < 0.6)
						{
							hr[batter]--;
							text = batter.Name + k switch
							{
								4 => " выбил гранд-слэм", 
								1 => " выбил сольный хоум-ран", 
								_ => " выбил " + ((k == 2) ? "двух" : "трёх") + "рановый хоум-ран", 
							};
						}
						else if (k == 1)
						{
							string[] how2 = new string[5] { "одиночный хит с результативным ударом", "дабл с результативным ударом", "сакрифайс-флай", "гроаундаут с результативным ударом", "трипл с результативным ударом" };
							double r = random.NextDouble();
							text = batter.Name + " принёс ран ударом " + how2[(!(r < 0.45)) ? ((r < 0.7) ? 1 : ((r < 0.85) ? 2 : ((r < 0.96) ? 3 : 4))) : 0];
						}
						else
						{
							string name = batter.Name;
							string text2;
							if (k >= 3)
							{
								text2 = " очистил базы даблом";
							}
							else
							{
								text2 = ((random.NextDouble() < 0.5) ? " выбил сингл, принёсший два рана" : " выбил дабл, принёсший два рана");
							}
							text = name + text2;
						}
						rbi[batter] -= k;
					}
					Timeline.Raw run = new Timeline.Raw
					{
						Period = inning * 2 + half + 1,
						Seconds = order++,
						Home = isHome,
						Points = k,
						Text = text,
						Clock = when
					};
					if (batter != null)
					{
						run.Players.Add(batter.Id);
					}
					box.Raw.Add(run);
					runs -= k;
				}
			}
		}
		if ((flow & homeWon) && innings >= 9 && home[innings - 1] > 0)
		{
			Timeline.Raw last = null;
			foreach (Timeline.Raw r2 in box.Raw)
			{
				if (r2.Home && r2.Period == innings * 2)
				{
					last = r2;
				}
			}
			if (last != null)
			{
				last.Text += " (уок-офф)";
			}
		}
		Timeline.Finish(box.Game, Sport.Baseball, box.Raw, innings, periodsFromEvents: false);
	}

	private static List<Player> Lineup(List<Player> roster)
	{
		List<Player> lineup = new List<Player>();
		Position[] array = new Position[8]
		{
			Position.Catcher,
			Position.FirstBase,
			Position.SecondBase,
			Position.Shortstop,
			Position.ThirdBase,
			Position.LeftField,
			Position.CenterField,
			Position.RightField
		};
		foreach (Position pos in array)
		{
			List<Player> list = ByPos(roster, pos);
			if (list.Count > 0)
			{
				lineup.Add(list[0]);
			}
		}
		Player dh = null;
		foreach (Player p in roster)
		{
			if (!Rosters.IsPitcher(p.Position) && !lineup.Contains(p) && (dh == null || p.Overall > dh.Overall))
			{
				dh = p;
			}
		}
		if (dh != null)
		{
			lineup.Add(dh);
		}
		lineup.Sort((Player a, Player b) => b.Overall.CompareTo(a.Overall));
		if (lineup.Count >= 4)
		{
			Player best = lineup[0];
			lineup.RemoveAt(0);
			lineup.Insert(2, best);
		}
		return lineup;
	}

	private static void BaseballBatting(Box box, List<Player> roster, int runs, int outs, Level level, Random random, out int hits, out int walks, out int strikeouts)
	{
		List<Player> lineup = Lineup(roster);
		hits = Math.Max((int)Math.Ceiling((double)runs * 0.55), NormalInt(random, 0.85 * (double)runs + ((level == Level.Pro) ? 4.6 : 5.2), 1.7, 1));
		int hr = Math.Min(Math.Min(runs, hits), Poisson(random, 0.27 * (double)runs + 0.25));
		int doubles = Math.Min(hits - hr, Poisson(random, 1.6));
		walks = Poisson(random, (level == Level.Pro) ? 3.1 : 3.8);
		strikeouts = Math.Min(outs, Poisson(random, (level == Level.Pro) ? 8.4 : 7.0));
		if (lineup.Count == 0)
		{
			return;
		}
		int plateAppearances = outs + hits + walks;
		int n = lineup.Count;
		int[] pa = new int[n];
		for (int i = 0; i < n; i++)
		{
			pa[i] = plateAppearances / n + ((i < plateAppearances % n) ? 1 : 0);
		}
		List<double> walkW = new List<double>();
		List<double> hitW = new List<double>();
		List<double> powerW = new List<double>();
		List<double> kW = new List<double>();
		for (int j = 0; j < n; j++)
		{
			Player p = lineup[j];
			walkW.Add((double)pa[j] * Skill(p, 1.0));
			hitW.Add((double)pa[j] * Skill(p, 2.5));
			bool slugger = p.Position == Position.FirstBase || p.Position == Position.LeftField || p.Position == Position.RightField || p.Position == Position.ThirdBase;
			powerW.Add((double)pa[j] * Skill(p, 4.0) * (slugger ? 1.4 : 1.0));
			kW.Add((double)pa[j] / Skill(p, 1.5));
		}
		int[] bb = Split(walks, walkW, random);
		int[] ab = new int[n];
		for (int k = 0; k < n; k++)
		{
			ab[k] = Math.Max(0, pa[k] - bb[k]);
		}
		int[] h = new int[n];
		for (int l = 0; l < hits; l++)
		{
			List<double> w = new List<double>();
			for (int m = 0; m < n; m++)
			{
				w.Add((h[m] < ab[m]) ? hitW[m] : 0.0);
			}
			Player p2 = PickWeighted(lineup, w, random);
			if (p2 == null)
			{
				break;
			}
			h[lineup.IndexOf(p2)]++;
		}
		int[] homers = new int[n];
		for (int num = 0; num < hr; num++)
		{
			List<double> w2 = new List<double>();
			for (int num2 = 0; num2 < n; num2++)
			{
				w2.Add((homers[num2] < h[num2]) ? powerW[num2] : 0.0);
			}
			Player p3 = PickWeighted(lineup, w2, random);
			if (p3 == null)
			{
				break;
			}
			homers[lineup.IndexOf(p3)]++;
		}
		int[] dbl = new int[n];
		for (int num3 = 0; num3 < doubles; num3++)
		{
			List<double> w3 = new List<double>();
			for (int num4 = 0; num4 < n; num4++)
			{
				w3.Add((dbl[num4] + homers[num4] < h[num4]) ? powerW[num4] : 0.0);
			}
			Player p4 = PickWeighted(lineup, w3, random);
			if (p4 == null)
			{
				break;
			}
			dbl[lineup.IndexOf(p4)]++;
		}
		int[] r = new int[n];
		for (int num5 = 0; num5 < n; num5++)
		{
			r[num5] = homers[num5];
		}
		List<double> onBase = new List<double>();
		for (int num6 = 0; num6 < n; num6++)
		{
			onBase.Add((double)(h[num6] - homers[num6] + bb[num6]) + 0.05);
		}
		int[] moreRuns = Split(Math.Max(0, runs - hr), onBase, random);
		int rbis = Math.Max(hr, runs - ((runs > 0 && random.NextDouble() < 0.3) ? 1 : 0));
		int[] rbi = new int[n];
		for (int num7 = 0; num7 < n; num7++)
		{
			rbi[num7] = homers[num7];
		}
		List<double> rbiW = new List<double>();
		for (int num8 = 0; num8 < n; num8++)
		{
			rbiW.Add(((double)h[num8] + 0.1) * ((num8 >= 2 && num8 <= 5) ? 1.5 : 1.0));
		}
		int[] moreRbi = Split(Math.Max(0, rbis - hr), rbiW, random);
		int[] so = new int[n];
		for (int num9 = 0; num9 < strikeouts; num9++)
		{
			List<double> w4 = new List<double>();
			for (int num10 = 0; num10 < n; num10++)
			{
				w4.Add((so[num10] < ab[num10] - h[num10]) ? kW[num10] : 0.0);
			}
			Player p5 = PickWeighted(lineup, w4, random);
			if (p5 == null)
			{
				break;
			}
			so[lineup.IndexOf(p5)]++;
		}
		for (int num11 = 0; num11 < n; num11++)
		{
			Player p6 = lineup[num11];
			box.Add(p6, 0, 1);
			box.Add(p6, 1, ab[num11]);
			box.Add(p6, 3, h[num11]);
			box.Add(p6, 4, dbl[num11]);
			box.Add(p6, 5, homers[num11]);
			box.Add(p6, 2, r[num11] + moreRuns[num11]);
			box.Add(p6, 6, rbi[num11] + moreRbi[num11]);
			box.Add(p6, 7, bb[num11]);
			box.Add(p6, 8, so[num11]);
		}
	}

	private static void BaseballPitching(Box box, Team team, List<Player> roster, int outs, int hitsAllowed, int walksAllowed, int strikeouts, int runsAllowed, bool won, int margin, Level level, Random random)
	{
		List<Player> starters = new List<Player>();
		foreach (Player p in roster)
		{
			if (p.Position == Position.StartingPitcher)
			{
				starters.Add(p);
			}
		}
		starters.Sort((Player a, Player b) => a.Id.CompareTo(b.Id));
		List<Player> relievers = ByPos(roster, Position.ReliefPitcher);
		if (starters.Count == 0 && relievers.Count == 0)
		{
			return;
		}
		Player starter = ((starters.Count > 0) ? starters[team.Rotation % starters.Count] : relievers[0]);
		team.Rotation = (team.Rotation + 1) % 1000;
		double starterMean = ((level == Level.Pro) ? 17.5 : 16.0);
		int starterOuts = Math.Min(outs, Math.Max(6, NormalInt(random, starterMean + (double)(starter.Overall - Rosters.Base(level)) * 0.3, 3.5, 3)));
		List<Player> staff = new List<Player> { starter };
		List<int> pitched = new List<int> { starterOuts };
		int remaining = outs - starterOuts;
		Player closer = ((relievers.Count > 0) ? relievers[0] : null);
		bool save = won && margin <= 3 && closer != null && remaining >= 3 && random.NextDouble() < ((level == Level.Pro) ? 0.75 : 0.45);
		List<Player> bullpen = new List<Player>();
		for (int i = 1; i < relievers.Count; i++)
		{
			bullpen.Add(relievers[i]);
		}
		int closerOuts = (save ? 3 : 0);
		int middle = remaining - closerOuts;
		while (middle > 0 && bullpen.Count > 0)
		{
			Player rp = bullpen[random.Next(bullpen.Count)];
			bullpen.Remove(rp);
			int o = Math.Min(middle, random.Next(2, 7));
			staff.Add(rp);
			pitched.Add(o);
			middle -= o;
		}
		if (middle > 0)
		{
			pitched[0] += middle;
		}
		if (save)
		{
			staff.Add(closer);
			pitched.Add(closerOuts);
		}
		List<double> share = new List<double>();
		for (int i2 = 0; i2 < staff.Count; i2++)
		{
			share.Add((double)pitched[i2] / Skill(staff[i2], 1.5));
		}
		int[] hits = Split(hitsAllowed, share, random);
		int[] walks = Split(walksAllowed, share, random);
		int[] runs = Split(runsAllowed, share, random);
		List<double> kShare = new List<double>();
		for (int i3 = 0; i3 < staff.Count; i3++)
		{
			kShare.Add((double)pitched[i3] * Skill(staff[i3], 2.0));
		}
		int[] ks = Split(strikeouts, kShare, random);
		for (int i4 = 0; i4 < staff.Count; i4++)
		{
			Player p2 = staff[i4];
			box.Add(p2, 9, 1);
			if (i4 == 0)
			{
				box.Add(p2, 10, 1);
			}
			box.Add(p2, 11, pitched[i4]);
			box.Add(p2, 12, hits[i4]);
			box.Add(p2, 14, walks[i4]);
			box.Add(p2, 15, ks[i4]);
			int er = Math.Max(0, runs[i4] - ((runs[i4] > 0 && random.NextDouble() < 0.12) ? 1 : 0));
			box.Add(p2, 13, er);
		}
		Player decision = ((pitched[0] >= 15 || staff.Count == 1) ? staff[0] : staff[1 + random.Next(Math.Max(1, staff.Count - ((!save) ? 1 : 2)))]);
		if (save && decision == closer)
		{
			decision = staff[0];
		}
		box.Add(decision, won ? 16 : 17, 1);
		if (save)
		{
			box.Add(closer, 18, 1);
		}
	}

	private static void Soccer(Box box, Match g, Level level, List<Player> home, List<Player> away, Random random)
	{
		if (g.Flow != null)
		{
			SoccerFromFlow(box, g, home, away, random);
			return;
		}
		int homeShots = Math.Max(g.HomeScore + 3, NormalInt(random, 12.0, 3.5, 3));
		int awayShots = Math.Max(g.AwayScore + 3, NormalInt(random, 12.0, 3.5, 3));
		Player homeKeeper = SoccerSide(box, home, g.HomeScore, homeShots, random, home: true);
		Player awayKeeper = SoccerSide(box, away, g.AwayScore, awayShots, random, home: false);
		SoccerKeeper(box, homeKeeper, awayShots, g.AwayScore);
		SoccerKeeper(box, awayKeeper, homeShots, g.HomeScore);
		foreach (Timeline.Raw item in box.Raw)
		{
			bool first = random.NextDouble() < 0.45;
			int minute = (first ? (1 + random.Next(45)) : (46 + random.Next(45)));
			int added = 0;
			if ((minute == 45 || minute == 90) && random.NextDouble() < 0.6)
			{
				added = 1 + random.Next(5);
			}
			item.Period = (first ? 1 : 2);
			item.Seconds = minute * 60 + added * 10 + random.Next(10);
			item.Clock = minute + ((added > 0) ? ("+" + added) : "") + "'";
		}
		Timeline.Finish(box.Game, Sport.Soccer, box.Raw, 2, periodsFromEvents: true);
		if (g.Result == ResultType.Penalties)
		{
			box.Game.Note = "Штрафные минуты " + Math.Max(g.HomePenalties, g.AwayPenalties) + "-" + Math.Min(g.HomePenalties, g.AwayPenalties);
		}
	}

	private static Player SoccerSide(Box box, List<Player> roster, int goals, int shots, Random random, bool home)
	{
		List<Player> keepers = ByPos(roster, Position.Goalkeeper);
		List<Player> xi = new List<Player>();
		List<double> goalW = new List<double>();
		List<double> assistW = new List<double>();
		List<double> shotW = new List<double>();
		AddField(xi, goalW, assistW, shotW, ByPos(roster, Position.Defender), 4, 0.12, 0.35, 0.2);
		AddField(xi, goalW, assistW, shotW, ByPos(roster, Position.Midfielder), 4, 0.45, 1.0, 0.6);
		AddField(xi, goalW, assistW, shotW, ByPos(roster, Position.Forward), 2, 1.0, 0.6, 1.0);
		foreach (Player p in xi)
		{
			box.Add(p, 0, 1);
		}
		for (int k = 0; k < goals; k++)
		{
			Player scorer = PickWeighted(xi, goalW, random);
			if (scorer == null)
			{
				break;
			}
			box.Add(scorer, 1, 1);
			box.Add(scorer, 3, 1);
			Player helper = ((random.NextDouble() < 0.7) ? PickWeighted(xi, assistW, random, scorer) : null);
			if (helper != null)
			{
				box.Add(helper, 2, 1);
			}
			string how;
			if (helper != null)
			{
				how = " (передача: " + helper.ShortName + ")";
			}
			else
			{
				how = ((random.NextDouble() < 0.25) ? " (штраф)" : "");
			}
			Timeline.Raw goal = new Timeline.Raw
			{
				Home = home,
				Points = 1,
				Text = scorer.Name + how
			};
			goal.Players.Add(scorer.Id);
			if (helper != null)
			{
				goal.Players.Add(helper.Id);
			}
			box.Raw.Add(goal);
		}
		int[] extra = Split(Math.Max(0, shots - goals), shotW, random);
		for (int i = 0; i < xi.Count; i++)
		{
			box.Add(xi[i], 3, extra[i]);
		}
		int[] cards = Split(Poisson(random, 1.8), shotW, random);
		for (int j = 0; j < xi.Count; j++)
		{
			if (cards[j] > 0)
			{
				box.Add(xi[j], 4, 1);
				Timeline.Raw card = new Timeline.Raw
				{
					Home = home,
					Points = 0,
					Kind = 1,
					Text = "Жёлтая карточка: " + xi[j].Name
				};
				card.Players.Add(xi[j].Id);
				box.Raw.Add(card);
			}
		}
		if (keepers.Count != 0)
		{
			return keepers[0];
		}
		return null;
	}

	private static void AddField(List<Player> xi, List<double> goalW, List<double> assistW, List<double> shotW, List<Player> group, int starters, double g, double a, double s)
	{
		for (int k = 0; k < group.Count && k < starters + 1; k++)
		{
			double minutes = ((k < starters) ? 1.0 : 0.3);
			double skill = Skill(group[k], 3.0);
			xi.Add(group[k]);
			goalW.Add(g * minutes * skill);
			assistW.Add(a * minutes * skill);
			shotW.Add(s * minutes * skill);
		}
	}

	private static void SoccerKeeper(Box box, Player keeper, int shotsAgainst, int goalsAgainst)
	{
		if (keeper != null)
		{
			int onTarget = Math.Max(goalsAgainst, (int)Math.Round((double)shotsAgainst * 0.38));
			box.Add(keeper, 0, 1);
			box.Add(keeper, 8, 1);
			box.Add(keeper, 5, onTarget - goalsAgainst);
			box.Add(keeper, 6, goalsAgainst);
			if (goalsAgainst == 0)
			{
				box.Add(keeper, 7, 1);
			}
		}
	}

	private static string Stars(Box box)
	{
		List<KeyValuePair<Player, int[]>> lines = new List<KeyValuePair<Player, int[]>>(box.Lines);
		lines.Sort((KeyValuePair<Player, int[]> a, KeyValuePair<Player, int[]> b) => Score(box.Sport, b.Key, b.Value).CompareTo(Score(box.Sport, a.Key, a.Value)));
		List<string> parts = new List<string>();
		for (int i = 0; i < lines.Count; i++)
		{
			if (parts.Count >= 3)
			{
				break;
			}
			string text = StarText(box.Sport, lines[i].Key, lines[i].Value);
			if (text.Length > 0)
			{
				parts.Add(lines[i].Key.ShortName + " " + text);
			}
		}
		return string.Join(" · ", parts.ToArray());
	}

	private static double Score(Sport sport, Player p, int[] s)
	{
		switch (sport)
		{
		case Sport.Hockey:
			if (p.Position == Position.Goalie)
			{
				if (Get(s, 11) != 0)
				{
					return (double)(Get(s, 8) - Get(s, 9)) / 3.5 + (double)(Get(s, 5) * 3) + (double)(Get(s, 10) * 8) - (double)(Get(s, 9) * 2);
				}
				return 0.0;
			}
			return (double)(Get(s, 1) * 10 + Get(s, 2) * 6) + (double)Get(s, 3) * 0.3;
		case Sport.Basketball:
			return (double)Get(s, 1) + (double)Get(s, 2) * 1.2 + (double)Get(s, 3) * 1.5 + (double)((Get(s, 4) + Get(s, 5)) * 2);
		case Sport.Football:
			return (double)Get(s, 3) / 25.0 + (double)(Get(s, 4) * 4) - (double)(Get(s, 5) * 3) + (double)Get(s, 7) / 10.0 + (double)(Get(s, 8) * 6) + (double)Get(s, 10) / 10.0 + (double)(Get(s, 11) * 6) + (double)(Get(s, 13) * 4) + (double)(Get(s, 14) * 6) + (double)Get(s, 12) * 0.5 + (double)(Get(s, 15) * 2);
		case Sport.Baseball:
			if (Rosters.IsPitcher(p.Position))
			{
				return (double)Get(s, 11) / 3.0 * 1.3 + (double)Get(s, 15) - (double)(Get(s, 13) * 2) + (double)(Get(s, 16) * 2) + (double)(Get(s, 18) * 2);
			}
			return (double)(Get(s, 3) * 2 + Get(s, 5) * 4) + (double)Get(s, 6) * 1.5 + (double)Get(s, 4);
		default:
			if (p.Position == Position.Goalkeeper)
			{
				return Get(s, 5) * 2 + Get(s, 7) * 6;
			}
			return (double)(Get(s, 1) * 10 + Get(s, 2) * 6) + (double)Get(s, 3) * 0.3;
		}
	}

	private static string StarText(Sport sport, Player p, int[] s)
	{
		List<string> bits = new List<string>();
		switch (sport)
		{
		case Sport.Hockey:
			if (p.Position == Position.Goalie)
			{
				if (Get(s, 11) == 0)
				{
					return "";
				}
				bits.Add(Get(s, 8) - Get(s, 9) + " сейвов");
				if (Get(s, 10) > 0)
				{
					bits.Add("сухой матч");
				}
			}
			else
			{
				if (Get(s, 1) > 0)
				{
					bits.Add(Get(s, 1) + "G");
				}
				if (Get(s, 2) > 0)
				{
					bits.Add(Get(s, 2) + "A");
				}
			}
			break;
		case Sport.Basketball:
			bits.Add(Get(s, 1) + " очков");
			if (Get(s, 2) >= 8)
			{
				bits.Add(Get(s, 2) + " подборов");
			}
			if (Get(s, 3) >= 6)
			{
				bits.Add(Get(s, 3) + " передач");
			}
			break;
		case Sport.Football:
			if (Get(s, 2) > 0)
			{
				bits.Add(Get(s, 3) + " ярдов пасом, " + Get(s, 4) + " тачдаунов");
			}
			else if (Get(s, 6) >= 8)
			{
				bits.Add(Get(s, 7) + " ярдов на выносе" + ((Get(s, 8) > 0) ? (", " + Get(s, 8) + " тачдаунов") : ""));
			}
			else if (Get(s, 9) > 0)
			{
				bits.Add(Get(s, 9) + " приёмов, " + Get(s, 10) + " ярдов" + ((Get(s, 11) > 0) ? (", " + Get(s, 11) + " тачдаунов") : ""));
			}
			else if (Get(s, 13) + Get(s, 14) > 0)
			{
				bits.Add(((Get(s, 13) > 0) ? (Get(s, 13) + " сэков ") : "") + ((Get(s, 14) > 0) ? (Get(s, 14) + " перехватов") : "").Trim());
			}
			else if (Get(s, 15) > 0)
			{
				bits.Add(Get(s, 15) + " FG");
			}
			else if (Get(s, 12) > 0)
			{
				bits.Add(Get(s, 12) + " захватов");
			}
			break;
		case Sport.Baseball:
			if (Rosters.IsPitcher(p.Position))
			{
				bits.Add(Innings(Get(s, 11)) + " иннингов, " + Get(s, 15) + " страйкаутов");
				if (Get(s, 16) > 0)
				{
					bits.Add("побед");
				}
				if (Get(s, 18) > 0)
				{
					bits.Add("СЕЙВ");
				}
			}
			else
			{
				bits.Add(Get(s, 3) + " из " + Get(s, 1));
				if (Get(s, 5) > 0)
				{
					bits.Add(Get(s, 5) + " хоум-ранов");
				}
				if (Get(s, 6) > 0)
				{
					bits.Add(Get(s, 6) + " РБИ");
				}
			}
			break;
		default:
			if (p.Position == Position.Goalkeeper)
			{
				bits.Add(Get(s, 5) + " сейвов");
				if (Get(s, 7) > 0)
				{
					bits.Add("сухой матч");
				}
				break;
			}
			if (Get(s, 1) > 0)
			{
				bits.Add(Get(s, 1) + ((Get(s, 1) == 1) ? " гол" : " голов"));
			}
			if (Get(s, 2) > 0)
			{
				bits.Add(Get(s, 2) + ((Get(s, 2) == 1) ? " передача" : " передач"));
			}
			break;
		}
		return string.Join(", ", bits.ToArray());
	}

	public static string Innings(int outs)
	{
		return outs / 3 + "." + outs % 3;
	}

	private static string I(int v)
	{
		return v.ToString(CultureInfo.InvariantCulture);
	}

	public static string PlusMinus(int v)
	{
		if (v <= 0)
		{
			return I(v);
		}
		return "+" + I(v);
	}

	private static string D1(double v)
	{
		return v.ToString("0.0", CultureInfo.InvariantCulture);
	}

	private static string D2(double v)
	{
		return v.ToString("0.00", CultureInfo.InvariantCulture);
	}

	private static string Pct3(double v)
	{
		string t = v.ToString("0.000", CultureInfo.InvariantCulture);
		if (!(v < 1.0))
		{
			return t;
		}
		return t.Substring(1);
	}

	private static double Per(int a, int b)
	{
		if (b > 0)
		{
			return (double)a / (double)b;
		}
		return 0.0;
	}

	public static StatGroup[] Groups(Sport sport)
	{
		return sport switch
		{
			Sport.Hockey => new StatGroup[3]
			{
				new StatGroup
				{
					Name = "Нападающие",
					Positions = new Position[3]
					{
						Position.Center,
						Position.LeftWing,
						Position.RightWing
					},
					Columns = new string[7] { "И", "Г", "П", "О", "+/-", "БВ", "ШТР" },
					Cells = (int[] s) => new string[7]
					{
						I(Get(s, 0)),
						I(Get(s, 1)),
						I(Get(s, 2)),
						I(Get(s, 1) + Get(s, 2)),
						PlusMinus(Get(s, 12)),
						I(Get(s, 3)),
						I(Get(s, 4))
					}
				},
				new StatGroup
				{
					Name = "Защита",
					Positions = new Position[1] { Position.Defense },
					Columns = new string[7] { "И", "Г", "П", "О", "+/-", "БВ", "ШТР" },
					Cells = (int[] s) => new string[7]
					{
						I(Get(s, 0)),
						I(Get(s, 1)),
						I(Get(s, 2)),
						I(Get(s, 1) + Get(s, 2)),
						PlusMinus(Get(s, 12)),
						I(Get(s, 3)),
						I(Get(s, 4))
					}
				},
				new StatGroup
				{
					Name = "Вратари",
					Positions = new Position[1] { Position.Goalie },
					Columns = new string[7] { "И", "В", "П", "ПБ", "ОБ%", "КН", "СУХ" },
					Cells = (int[] s) => new string[7]
					{
						I(Get(s, 11)),
						I(Get(s, 5)),
						I(Get(s, 6)),
						I(Get(s, 7)),
						Pct3(Per(Get(s, 8) - Get(s, 9), Get(s, 8))),
						D2(Per(Get(s, 9), Get(s, 11))),
						I(Get(s, 10))
					}
				}
			}, 
			Sport.Basketball => new StatGroup[1]
			{
				new StatGroup
				{
					Name = "Игроки",
					Positions = new Position[5]
					{
						Position.PointGuard,
						Position.ShootingGuard,
						Position.SmallForward,
						Position.PowerForward,
						Position.BigMan
					},
					Columns = new string[8] { "И", "МИН", "О", "ПД", "ПР", "ПХ", "БЛ", "БР%" },
					Cells = (int[] s) => new string[8]
					{
						I(Get(s, 0)),
						D1(Per(Get(s, 11), Get(s, 0))),
						D1(Per(Get(s, 1), Get(s, 0))),
						D1(Per(Get(s, 2), Get(s, 0))),
						D1(Per(Get(s, 3), Get(s, 0))),
						D1(Per(Get(s, 4), Get(s, 0))),
						D1(Per(Get(s, 5), Get(s, 0))),
						Pct3(Per(Get(s, 6), Get(s, 7)))
					}
				}
			}, 
			Sport.Football => new StatGroup[5]
			{
				new StatGroup
				{
					Name = "Квотербеки",
					Positions = new Position[1] { Position.Quarterback },
					Columns = new string[6] { "ИГ", "ЗБР", "ПОП", "ЯРД", "ТД", "ПЕР" },
					Cells = (int[] s) => new string[6]
					{
						I(Get(s, 0)),
						I(Get(s, 1)),
						I(Get(s, 2)),
						I(Get(s, 3)),
						I(Get(s, 4)),
						I(Get(s, 5))
					}
				},
				new StatGroup
				{
					Name = "Раннинбеки и принимающие",
					Positions = new Position[3]
					{
						Position.RunningBack,
						Position.WideReceiver,
						Position.TightEnd
					},
					Columns = new string[6] { "ИГ", "ВЫН", "ВЫН", "ПРМ", "ЯРД", "ТД" },
					Cells = (int[] s) => new string[6]
					{
						I(Get(s, 0)),
						I(Get(s, 6)),
						I(Get(s, 7)),
						I(Get(s, 9)),
						I(Get(s, 10)),
						I(Get(s, 8) + Get(s, 11))
					}
				},
				new StatGroup
				{
					Name = "Линия нападения",
					Positions = new Position[1] { Position.OffensiveLine },
					Columns = new string[1] { "ИГ" },
					Cells = (int[] s) => new string[1] { I(Get(s, 0)) }
				},
				new StatGroup
				{
					Name = "Защита",
					Positions = new Position[4]
					{
						Position.DefensiveLine,
						Position.Linebacker,
						Position.Cornerback,
						Position.Safety
					},
					Columns = new string[4] { "ИГ", "ТКЛ", "СЭК", "ПЕР" },
					Cells = (int[] s) => new string[4]
					{
						I(Get(s, 0)),
						I(Get(s, 12)),
						I(Get(s, 13)),
						I(Get(s, 14))
					}
				},
				new StatGroup
				{
					Name = "Спецкоманды",
					Positions = new Position[2]
					{
						Position.Kicker,
						Position.Punter
					},
					Columns = new string[4] { "ИГ", "ЗБ", "ПОП", "ДОП" },
					Cells = (int[] s) => new string[4]
					{
						I(Get(s, 0)),
						I(Get(s, 15)),
						I(Get(s, 16)),
						I(Get(s, 17))
					}
				}
			}, 
			Sport.Baseball => new StatGroup[2]
			{
				new StatGroup
				{
					Name = "Бьющие",
					Positions = new Position[8]
					{
						Position.Catcher,
						Position.FirstBase,
						Position.SecondBase,
						Position.Shortstop,
						Position.ThirdBase,
						Position.LeftField,
						Position.CenterField,
						Position.RightField
					},
					Columns = new string[8] { "И", "АБ", "Х", "ХР", "РБИ", "РА", "УБ", "СР" },
					Cells = (int[] s) => new string[8]
					{
						I(Get(s, 0)),
						I(Get(s, 1)),
						I(Get(s, 3)),
						I(Get(s, 5)),
						I(Get(s, 6)),
						I(Get(s, 2)),
						I(Get(s, 7)),
						Pct3(Per(Get(s, 3), Get(s, 1)))
					}
				},
				new StatGroup
				{
					Name = "Питчеры",
					Positions = new Position[2]
					{
						Position.StartingPitcher,
						Position.ReliefPitcher
					},
					Columns = new string[8] { "И", "СТ", "В", "П", "СЕЙВ", "ИНН", "СТР", "ЭРА" },
					Cells = (int[] s) => new string[8]
					{
						I(Get(s, 9)),
						I(Get(s, 10)),
						I(Get(s, 16)),
						I(Get(s, 17)),
						I(Get(s, 18)),
						Innings(Get(s, 11)),
						I(Get(s, 15)),
						D2(Per(Get(s, 13) * 27, Get(s, 11)))
					}
				}
			}, 
			_ => new StatGroup[2]
			{
				new StatGroup
				{
					Name = "Полевые игроки",
					Positions = new Position[3]
					{
						Position.Forward,
						Position.Midfielder,
						Position.Defender
					},
					Columns = new string[6] { "ИГ", "Г", "П", "УД", "ЖК", "КК" },
					Cells = (int[] s) => new string[6]
					{
						I(Get(s, 0)),
						I(Get(s, 1)),
						I(Get(s, 2)),
						I(Get(s, 3)),
						I(Get(s, 4)),
						I(Get(s, 9))
					}
				},
				new StatGroup
				{
					Name = "Вратари",
					Positions = new Position[1] { Position.Goalkeeper },
					Columns = new string[4] { "ИГ", "СЕЙВ", "ПГ", "СМ" },
					Cells = (int[] s) => new string[4]
					{
						I(Get(s, 8)),
						I(Get(s, 5)),
						I(Get(s, 6)),
						I(Get(s, 7))
					}
				}
			}, 
		};
	}

	public static Leader[] Leaders(Sport sport, int teamGames)
	{
		int half = Math.Max(1, teamGames / 2);
		return sport switch
		{
			Sport.Hockey => new Leader[7]
			{
				new Leader
				{
					Title = "Очки",
					Qualifies = (Player p, int[] s) => p.Position != Position.Goalie,
					Value = (int[] s) => Get(s, 1) + Get(s, 2),
					Format = (double v) => I((int)v)
				},
				new Leader
				{
					Title = "Голы",
					Qualifies = (Player p, int[] s) => p.Position != Position.Goalie,
					Value = (int[] s) => Get(s, 1),
					Format = (double v) => I((int)v)
				},
				new Leader
				{
					Title = "Передачи",
					Qualifies = (Player p, int[] s) => p.Position != Position.Goalie,
					Value = (int[] s) => Get(s, 2),
					Format = (double v) => I((int)v)
				},
				new Leader
				{
					Title = "Плюс/минус",
					Qualifies = (Player p, int[] s) => p.Position != Position.Goalie && Get(s, 0) > 0,
					Value = (int[] s) => Get(s, 12),
					Format = (double v) => PlusMinus((int)v)
				},
				new Leader
				{
					Title = "Победы вратаря",
					Qualifies = (Player p, int[] s) => p.Position == Position.Goalie,
					Value = (int[] s) => Get(s, 5),
					Format = (double v) => I((int)v)
				},
				new Leader
				{
					Title = "Процент отражённых бросков",
					Qualifies = (Player p, int[] s) => p.Position == Position.Goalie && Get(s, 11) >= Math.Max(1, teamGames / 4),
					Value = (int[] s) => Per(Get(s, 8) - Get(s, 9), Get(s, 8)),
					Format = Pct3
				},
				new Leader
				{
					Title = "Среднее число пропущенных голов",
					Qualifies = (Player p, int[] s) => p.Position == Position.Goalie && Get(s, 11) >= Math.Max(1, teamGames / 4),
					Value = (int[] s) => Per(Get(s, 9), Get(s, 11)),
					Format = D2,
					LowerIsBetter = true
				}
			}, 
			Sport.Basketball => new Leader[5]
			{
				new Leader
				{
					Title = "Очков за матч",
					Qualifies = (Player p, int[] s) => Get(s, 0) >= half,
					Value = (int[] s) => Per(Get(s, 1), Get(s, 0)),
					Format = D1
				},
				new Leader
				{
					Title = "Подборов за матч",
					Qualifies = (Player p, int[] s) => Get(s, 0) >= half,
					Value = (int[] s) => Per(Get(s, 2), Get(s, 0)),
					Format = D1
				},
				new Leader
				{
					Title = "Передач за матч",
					Qualifies = (Player p, int[] s) => Get(s, 0) >= half,
					Value = (int[] s) => Per(Get(s, 3), Get(s, 0)),
					Format = D1
				},
				new Leader
				{
					Title = "Перехватов за матч",
					Qualifies = (Player p, int[] s) => Get(s, 0) >= half,
					Value = (int[] s) => Per(Get(s, 4), Get(s, 0)),
					Format = D1
				},
				new Leader
				{
					Title = "Блок-шотов за матч",
					Qualifies = (Player p, int[] s) => Get(s, 0) >= half,
					Value = (int[] s) => Per(Get(s, 5), Get(s, 0)),
					Format = D1
				}
			}, 
			Sport.Football => new Leader[6]
			{
				new Leader
				{
					Title = "Ярды пасом",
					Qualifies = (Player p, int[] s) => Get(s, 2) > 0,
					Value = (int[] s) => Get(s, 3),
					Format = (double v) => I((int)v)
				},
				new Leader
				{
					Title = "Пасовые тачдауны",
					Qualifies = (Player p, int[] s) => Get(s, 2) > 0,
					Value = (int[] s) => Get(s, 4),
					Format = (double v) => I((int)v)
				},
				new Leader
				{
					Title = "Ярды на выносе",
					Qualifies = (Player p, int[] s) => Get(s, 6) > 0,
					Value = (int[] s) => Get(s, 7),
					Format = (double v) => I((int)v)
				},
				new Leader
				{
					Title = "Ярды на приёме",
					Qualifies = (Player p, int[] s) => Get(s, 9) > 0,
					Value = (int[] s) => Get(s, 10),
					Format = (double v) => I((int)v)
				},
				new Leader
				{
					Title = "Сэки",
					Qualifies = (Player p, int[] s) => Get(s, 13) > 0,
					Value = (int[] s) => Get(s, 13),
					Format = (double v) => I((int)v)
				},
				new Leader
				{
					Title = "Перехваты",
					Qualifies = (Player p, int[] s) => Get(s, 14) > 0,
					Value = (int[] s) => Get(s, 14),
					Format = (double v) => I((int)v)
				}
			}, 
			Sport.Baseball => new Leader[7]
			{
				new Leader
				{
					Title = "Средний процент отбивания",
					Qualifies = (Player p, int[] s) => !Rosters.IsPitcher(p.Position) && Get(s, 1) + Get(s, 7) >= Math.Max(1, (int)((double)teamGames * 2.5)),
					Value = (int[] s) => Per(Get(s, 3), Get(s, 1)),
					Format = Pct3
				},
				new Leader
				{
					Title = "Хоум-раны",
					Qualifies = (Player p, int[] s) => !Rosters.IsPitcher(p.Position),
					Value = (int[] s) => Get(s, 5),
					Format = (double v) => I((int)v)
				},
				new Leader
				{
					Title = "Раны, принесённые отбивающим",
					Qualifies = (Player p, int[] s) => !Rosters.IsPitcher(p.Position),
					Value = (int[] s) => Get(s, 6),
					Format = (double v) => I((int)v)
				},
				new Leader
				{
					Title = "Победы питчера",
					Qualifies = (Player p, int[] s) => Rosters.IsPitcher(p.Position),
					Value = (int[] s) => Get(s, 16),
					Format = (double v) => I((int)v)
				},
				new Leader
				{
					Title = "ПСР",
					Qualifies = (Player p, int[] s) => Rosters.IsPitcher(p.Position) && Get(s, 11) >= Math.Max(3, (int)((double)teamGames * 2.4)),
					Value = (int[] s) => Per(Get(s, 13) * 27, Get(s, 11)),
					Format = D2,
					LowerIsBetter = true
				},
				new Leader
				{
					Title = "Страйкауты",
					Qualifies = (Player p, int[] s) => Rosters.IsPitcher(p.Position),
					Value = (int[] s) => Get(s, 15),
					Format = (double v) => I((int)v)
				},
				new Leader
				{
					Title = "Сейвы",
					Qualifies = (Player p, int[] s) => p.Position == Position.ReliefPitcher,
					Value = (int[] s) => Get(s, 18),
					Format = (double v) => I((int)v)
				}
			}, 
			_ => new Leader[3]
			{
				new Leader
				{
					Title = "Голы",
					Qualifies = (Player p, int[] s) => p.Position != Position.Goalkeeper,
					Value = (int[] s) => Get(s, 1),
					Format = (double v) => I((int)v)
				},
				new Leader
				{
					Title = "Передачи",
					Qualifies = (Player p, int[] s) => p.Position != Position.Goalkeeper,
					Value = (int[] s) => Get(s, 2),
					Format = (double v) => I((int)v)
				},
				new Leader
				{
					Title = "Сухие матчи",
					Qualifies = (Player p, int[] s) => p.Position == Position.Goalkeeper,
					Value = (int[] s) => Get(s, 7),
					Format = (double v) => I((int)v)
				}
			}, 
		};
	}

	private static void Bump(Dictionary<Player, int> d, Player p, int by)
	{
		d.TryGetValue(p, out var v);
		d[p] = v + by;
	}

	private static int Get(Dictionary<Player, int> d, Player p)
	{
		if (!d.TryGetValue(p, out var v))
		{
			return 0;
		}
		return v;
	}

	private static FbSide FootballSideOf(List<Player> roster, Random random)
	{
		FbSide s = new FbSide();
		List<Player> qbs = ByPos(roster, Position.Quarterback);
		List<Player> rbs = ByPos(roster, Position.RunningBack);
		List<Player> wrs = ByPos(roster, Position.WideReceiver);
		List<Player> tes = ByPos(roster, Position.TightEnd);
		List<Player> ks = ByPos(roster, Position.Kicker);
		ref Player qb = ref s.Qb;
		Player player;
		if (qbs.Count != 0)
		{
			player = ((qbs.Count > 1 && random.NextDouble() < 0.04) ? qbs[1] : qbs[0]);
		}
		else
		{
			player = null;
		}
		qb = player;
		s.Kicker = ((ks.Count > 0) ? ks[0] : null);
		double[] wrRank = new double[5] { 1.0, 0.75, 0.5, 0.2, 0.1 };
		for (int i = 0; i < wrs.Count && i < 5; i++)
		{
			s.Receivers.Add(wrs[i]);
			s.TargetW.Add(wrRank[i] * Skill(wrs[i], 2.0));
		}
		if (tes.Count > 0)
		{
			s.Receivers.Add(tes[0]);
			s.TargetW.Add(0.55 * Skill(tes[0], 2.0));
		}
		if (tes.Count > 1)
		{
			s.Receivers.Add(tes[1]);
			s.TargetW.Add(0.12);
		}
		if (rbs.Count > 0)
		{
			s.Receivers.Add(rbs[0]);
			s.TargetW.Add(0.35);
		}
		if (rbs.Count > 1)
		{
			s.Receivers.Add(rbs[1]);
			s.TargetW.Add(0.1);
		}
		if (rbs.Count > 0)
		{
			s.Runners.Add(rbs[0]);
			s.CarryW.Add(0.66);
			s.TdRunW.Add(0.7);
		}
		if (rbs.Count > 1)
		{
			s.Runners.Add(rbs[1]);
			s.CarryW.Add(0.22);
			s.TdRunW.Add(0.18);
		}
		if (s.Qb != null)
		{
			s.Runners.Add(s.Qb);
			s.CarryW.Add(0.08);
			s.TdRunW.Add(0.12);
		}
		if (wrs.Count > 0)
		{
			s.Runners.Add(wrs[0]);
			s.CarryW.Add(0.04);
			s.TdRunW.Add(0.03);
		}
		KeyValuePair<Position, double>[] array = new KeyValuePair<Position, double>[3]
		{
			new KeyValuePair<Position, double>(Position.Cornerback, 1.5),
			new KeyValuePair<Position, double>(Position.Safety, 1.2),
			new KeyValuePair<Position, double>(Position.Linebacker, 0.5)
		};
		for (int j = 0; j < array.Length; j++)
		{
			KeyValuePair<Position, double> g = array[j];
			List<Player> group = ByPos(roster, g.Key);
			for (int k = 0; k < group.Count && k < 3; k++)
			{
				s.Defense.Add(group[k]);
				s.IntW.Add(g.Value * Skill(group[k], 2.0));
			}
		}
		List<Player> cbs = ByPos(roster, Position.Cornerback);
		ref Player returner = ref s.Returner;
		Player player2;
		if (wrs.Count > 2)
		{
			player2 = wrs[2];
		}
		else if (cbs.Count > 2)
		{
			player2 = cbs[2];
		}
		else if (rbs.Count > 1)
		{
			player2 = rbs[1];
		}
		else
		{
			player2 = ((wrs.Count > 0) ? wrs[0] : null);
		}
		returner = player2;
		return s;
	}

	private static void FootballFromFlow(Box box, Match g, List<Player> home, List<Player> away, Random random)
	{
		GameFlow f = g.Flow;
		FbSide h = FootballSideOf(home, random);
		FbSide a = FootballSideOf(away, random);
		foreach (FlowEvent e in f.Events)
		{
			FbSide us = (e.Home ? h : a);
			List<int> who = new List<int>();
			string text;
			switch (e.Kind)
			{
			case 1:
			{
				Player c = ((us.Receivers.Count > 0) ? PickWeighted(us.Receivers, us.TargetW, random) : null);
				if (c != null)
				{
					Bump(us.CatchTds, c, 1);
					Bump(us.CatchTdYards, c, e.Yards);
					who.Add(c.Id);
				}
				if (us.Qb != null)
				{
					who.Add(us.Qb.Id);
				}
				text = ((c != null) ? (c.Name + " ") : "") + e.Yards + " ярдовый пас" + ((us.Qb != null) ? (" от " + us.Qb.ShortName) : "");
				break;
			}
			case 2:
			{
				Player r2 = ((us.Runners.Count > 0) ? PickWeighted(us.Runners, us.TdRunW, random) : null);
				if (r2 != null)
				{
					Bump(us.RunTds, r2, 1);
					Bump(us.RunTdYards, r2, e.Yards);
					who.Add(r2.Id);
				}
				text = ((r2 != null) ? (r2.Name + " ") : "") + e.Yards + " ярдовый вынос";
				break;
			}
			case 3:
				if (us.Kicker != null)
				{
					who.Add(us.Kicker.Id);
				}
				text = ((us.Kicker != null) ? (us.Kicker.Name + " ") : "") + e.Yards + " ярдовый филд-гол";
				break;
			case 4:
				text = "Сейфти";
				break;
			case 5:
			case 6:
			{
				Player d = ((us.Defense.Count > 0) ? PickWeighted(us.Defense, us.IntW, random) : null);
				if (d != null)
				{
					who.Add(d.Id);
					if (e.Kind == 5)
					{
						Bump(us.ReturnedPicks, d, 1);
					}
				}
				text = ((d != null) ? (d.Name + " ") : "") + e.Yards + " ярд " + ((e.Kind == 5) ? "возврат перехвата" : "возврат после потери мяча");
				break;
			}
			case 7:
			case 8:
				if (us.Returner != null)
				{
					who.Add(us.Returner.Id);
				}
				text = ((us.Returner != null) ? (us.Returner.Name + " ") : "") + e.Yards + " ярд " + ((e.Kind == 7) ? "возврат после начального удара" : "возврат панта");
				break;
			default:
			{
				Player r = ((us.Runners.Count > 0) ? PickWeighted(us.Runners, us.TdRunW, random) : null);
				if (r != null)
				{
					who.Add(r.Id);
				}
				text = "Реализация двухочкового" + ((r != null) ? (", " + r.Name) : "");
				break;
			}
			}
			if (e.Extra == 1)
			{
				text = text + " (" + ((us.Kicker != null) ? (us.Kicker.ShortName + " ") : "") + "удар)";
			}
			else if (e.Extra == 2)
			{
				text += " (реализация двухочкового)";
			}
			else if (e.Extra == 0)
			{
				text += " (удар неудачен)";
			}
			else if (e.Extra == -2)
			{
				text += " (двухочковая попытка не удалась)";
			}
			Timeline.Raw raw = new Timeline.Raw
			{
				Period = e.Period,
				Seconds = e.Seconds,
				Home = e.Home,
				Points = e.Points,
				Kind = 0,
				Text = text
			};
			raw.Players.AddRange(who);
			box.Raw.Add(raw);
		}
		FootballOffenseFromFlow(box, h, f.HomeStats, random);
		FootballOffenseFromFlow(box, a, f.AwayStats, random);
		FootballDefenseFromFlow(box, home, h, f.AwayStats, random);
		FootballDefenseFromFlow(box, away, a, f.HomeStats, random);
		foreach (Player p in home)
		{
			box.Add(p, 0, 1);
		}
		foreach (Player p2 in away)
		{
			box.Add(p2, 0, 1);
		}
		Timeline.Finish(box.Game, Sport.Football, box.Raw, Math.Max(4, f.Periods), periodsFromEvents: true);
		box.Game.Flow.AddRange(f.Track);
	}

	private static int[] SplitAll(int total, List<double> weights, Random random, int fallback)
	{
		double sum = 0.0;
		foreach (double w in weights)
		{
			sum += Math.Max(0.0, w);
		}
		int[] parts = ((sum > 0.0) ? SplitLarge(total, weights, random) : new int[weights.Count]);
		if (sum <= 0.0 && weights.Count > 0)
		{
			parts[Math.Max(0, Math.Min(weights.Count - 1, fallback))] = total;
		}
		return parts;
	}

	private static void FootballOffenseFromFlow(Box box, FbSide s, int[] st, Random random)
	{
		int cmp = st[1];
		int pyd = st[2];
		if (s.Qb != null)
		{
			box.Add(s.Qb, 1, cmp);
			box.Add(s.Qb, 2, st[0]);
			box.Add(s.Qb, 3, pyd);
			box.Add(s.Qb, 4, st[3]);
			box.Add(s.Qb, 5, st[4]);
		}
		if (s.Receivers.Count > 0)
		{
			int tdCatches = 0;
			int tdYards = 0;
			foreach (Player p in s.Receivers)
			{
				tdCatches += Get(s.CatchTds, p);
				tdYards += Get(s.CatchTdYards, p);
			}
			int[] more = Split(Math.Max(0, cmp - tdCatches), s.TargetW, random);
			List<double> yardW = new List<double>();
			for (int i = 0; i < s.Receivers.Count; i++)
			{
				yardW.Add((double)more[i] * ((s.Receivers[i].Position == Position.WideReceiver) ? 1.25 : ((s.Receivers[i].Position == Position.TightEnd) ? 1.0 : 0.7)));
			}
			int[] yds = SplitAll(pyd - tdYards, yardW, random, 0);
			for (int j = 0; j < s.Receivers.Count; j++)
			{
				Player p2 = s.Receivers[j];
				int tds = Get(s.CatchTds, p2);
				box.Add(p2, 9, more[j] + tds);
				box.Add(p2, 10, yds[j] + Get(s.CatchTdYards, p2));
				if (tds > 0)
				{
					box.Add(p2, 11, tds);
				}
			}
		}
		if (s.Runners.Count > 0)
		{
			int tdRuns = 0;
			int tdYards2 = 0;
			foreach (Player p3 in s.Runners)
			{
				tdRuns += Get(s.RunTds, p3);
				tdYards2 += Get(s.RunTdYards, p3);
			}
			int[] carries = Split(Math.Max(0, st[7] - tdRuns), s.CarryW, random);
			List<double> rushW = new List<double>();
			for (int k = 0; k < s.Runners.Count; k++)
			{
				rushW.Add((double)carries[k] * Skill(s.Runners[k], 1.0));
			}
			int[] yds2 = SplitAll(st[8] - tdYards2, rushW, random, 0);
			for (int l = 0; l < s.Runners.Count; l++)
			{
				Player p4 = s.Runners[l];
				int tds2 = Get(s.RunTds, p4);
				box.Add(p4, 6, carries[l] + tds2);
				box.Add(p4, 7, yds2[l] + Get(s.RunTdYards, p4));
				if (tds2 > 0)
				{
					box.Add(p4, 8, tds2);
				}
			}
		}
		if (s.Kicker != null)
		{
			box.Add(s.Kicker, 15, st[13]);
			box.Add(s.Kicker, 16, st[12]);
			box.Add(s.Kicker, 17, st[15]);
			box.Add(s.Kicker, 18, st[14]);
		}
	}

	private static void FootballDefenseFromFlow(Box box, List<Player> roster, FbSide s, int[] opp, Random random)
	{
		List<Player> defenders = new List<Player>();
		List<double> tackleW = new List<double>();
		List<double> sackW = new List<double>();
		List<double> intW = new List<double>();
		AddDefenders(defenders, tackleW, sackW, intW, ByPos(roster, Position.Linebacker), 3, 2.5, 1.0, 0.5);
		AddDefenders(defenders, tackleW, sackW, intW, ByPos(roster, Position.Safety), 2, 1.8, 0.15, 1.2);
		AddDefenders(defenders, tackleW, sackW, intW, ByPos(roster, Position.Cornerback), 3, 1.3, 0.05, 1.5);
		AddDefenders(defenders, tackleW, sackW, intW, ByPos(roster, Position.DefensiveLine), 4, 1.0, 1.5, 0.05);
		if (defenders.Count == 0)
		{
			return;
		}
		int[] t = Split(opp[7] + opp[1] + opp[5] + NormalInt(random, 9.0, 3.0, 2), tackleW, random);
		int[] k = Split(opp[5], sackW, random);
		int returned = 0;
		foreach (KeyValuePair<Player, int> returnedPick in s.ReturnedPicks)
		{
			returned += returnedPick.Value;
		}
		int[] picks = Split(Math.Max(0, opp[4] - returned), intW, random);
		for (int i = 0; i < defenders.Count; i++)
		{
			box.Add(defenders[i], 12, t[i]);
			box.Add(defenders[i], 13, k[i]);
			box.Add(defenders[i], 14, picks[i] + Get(s.ReturnedPicks, defenders[i]));
		}
		foreach (KeyValuePair<Player, int> kv in s.ReturnedPicks)
		{
			if (!defenders.Contains(kv.Key))
			{
				box.Add(kv.Key, 14, kv.Value);
			}
		}
	}

	private static void HockeyFromFlow(Box box, Match g, List<Player> home, List<Player> away, bool homeWon, Random random)
	{
		GameFlow f = g.Flow;
		HockeyTeam h = HockeyLineup(box, home, random, randomPim: false, g.HomeGoalieId);
		HockeyTeam a = HockeyLineup(box, away, random, randomPim: false, g.AwayGoalieId);
		int homeGoals = 0;
		int awayGoals = 0;
		int homeEn = 0;
		int awayEn = 0;
		foreach (FlowEvent e in f.Events)
		{
			if (e.Points <= 0)
			{
				continue;
			}
			HockeyTeam us = (e.Home ? h : a);
			HockeyTeam them = (e.Home ? a : h);
			HGoal x = HockeyGoal(box, us, them, e.Home, random, e.Kind);
			if (e.Home)
			{
				homeGoals++;
				if (e.Kind == 4)
				{
					homeEn++;
				}
			}
			else
			{
				awayGoals++;
				if (e.Kind == 4)
				{
					awayEn++;
				}
			}
			if (x != null)
			{
				Timeline.Raw r = new Timeline.Raw
				{
					Period = e.Period,
					Seconds = e.Seconds,
					Home = e.Home,
					Points = 1
				};
				r.Players.Add(x.Scorer.Id);
				if (x.First != null)
				{
					r.Players.Add(x.First.Id);
				}
				if (x.Second != null)
				{
					r.Players.Add(x.Second.Id);
				}
				string text = x.Scorer.Name;
				text = ((x.First == null) ? (text + " (без передачи)") : (text + " (" + x.First.ShortName + ((x.Second != null) ? (", " + x.Second.ShortName) : "") + ")"));
				if (e.Kind == 2)
				{
					text += "  БОЛ";
				}
				else if (e.Kind == 3)
				{
					text += "  МЕН";
				}
				else if (e.Kind == 4)
				{
					text += "  ПУСТ";
				}
				r.Text = text;
				box.Raw.Add(r);
			}
		}
		int homeShots = Math.Max(homeGoals, f.HomeStats[0]);
		int awayShots = Math.Max(awayGoals, f.AwayStats[0]);
		HockeyShots(box, h, homeShots - homeGoals, random);
		HockeyShots(box, a, awayShots - awayGoals, random);
		HockeyGoalie(box, h.Goalie, awayShots - awayEn, awayGoals - awayEn, homeWon, g.Result);
		HockeyGoalie(box, a.Goalie, homeShots - homeEn, homeGoals - homeEn, !homeWon, g.Result);
		HockeyPenalties(box, h, f.AwayStats[1], random);
		HockeyPenalties(box, a, f.HomeStats[1], random);
		int periods = Math.Max(3, f.Periods);
		Timeline.Finish(box.Game, Sport.Hockey, box.Raw, periods, periodsFromEvents: true);
		if (g.Result == ResultType.Shootout)
		{
			int last = periods - 1;
			if (homeWon)
			{
				box.Game.HomePeriods[last] = 1;
			}
			else
			{
				box.Game.AwayPeriods[last] = 1;
			}
			box.Game.Note = ((f.Note.Length > 0) ? f.Note : "Буллиты 1–0");
		}
	}

	private static void HockeyPenalties(Box box, HockeyTeam t, int penalties, Random random)
	{
		if (t.Skaters.Count == 0)
		{
			return;
		}
		List<double> w = new List<double>();
		foreach (Player p in t.Skaters)
		{
			w.Add((p.Position == Position.Defense) ? 1.3 : 1.0);
		}
		int[] minors = Split(penalties, w, random);
		for (int i = 0; i < t.Skaters.Count; i++)
		{
			if (minors[i] > 0)
			{
				box.Add(t.Skaters[i], 4, 2 * minors[i]);
			}
			if (random.NextDouble() < 0.006)
			{
				box.Add(t.Skaters[i], 4, 5);
			}
		}
	}

	private static void SoccerFromFlow(Box box, Match g, List<Player> home, List<Player> away, Random random)
	{
		GameFlow flow = g.Flow;
		Player homeKeeper = SoccerXi(box, home, random, out var hx, out var hg, out var ha, out var hs);
		Player awayKeeper = SoccerXi(box, away, random, out var ax, out var ag, out var aa, out var asw);
		int homeGoals = 0;
		int awayGoals = 0;
		Dictionary<int, double> sentOff = new Dictionary<int, double>();
		HashSet<int> booked = new HashSet<int>();
		Func<List<Player>, List<double>, List<double>> onPitch = (List<Player> list3, List<double> w) =>
		{
			List<double> list2 = new List<double>();
			for (int j = 0; j < list3.Count; j++)
			{
				list2.Add(sentOff.ContainsKey(list3[j].Id) ? 0.0 : w[j]);
			}
			return list2;
		};
		foreach (FlowEvent e in flow.Events)
		{
			List<Player> xi = (e.Home ? hx : ax);
			List<double> goalW = onPitch(xi, e.Home ? hg : ag);
			List<double> assistW = onPitch(xi, e.Home ? ha : aa);
			List<double> shotW = onPitch(xi, e.Home ? hs : asw);
			Timeline.Raw r = new Timeline.Raw
			{
				Period = e.Period,
				Seconds = e.Seconds,
				Home = e.Home,
				Clock = e.Clock
			};
			if (e.Points > 0)
			{
				if (e.Home)
				{
					homeGoals++;
				}
				else
				{
					awayGoals++;
				}
				bool penalty = e.Kind == 2;
				Player scorer = PickWeighted(xi, penalty ? shotW : goalW, random);
				if (scorer == null)
				{
					continue;
				}
				box.Add(scorer, 1, 1);
				box.Add(scorer, 3, 1);
				Player helper = ((!penalty && random.NextDouble() < 0.7) ? PickWeighted(xi, assistW, random, scorer) : null);
				if (helper != null)
				{
					box.Add(helper, 2, 1);
				}
				r.Points = 1;
				ref string text = ref r.Text;
				string name = scorer.Name;
				string text2;
				if (penalty)
				{
					text2 = " (штраф)";
				}
				else
				{
					text2 = ((helper != null) ? (" (передача: " + helper.ShortName + ")") : "");
				}
				text = name + text2;
				r.Players.Add(scorer.Id);
				if (helper != null)
				{
					r.Players.Add(helper.Id);
				}
			}
			else
			{
				bool red = e.Kind == 5;
				List<double> cardW = new List<double>();
				foreach (Player p in xi)
				{
					List<double> list = cardW;
					double item;
					if (sentOff.ContainsKey(p.Id) || (!red && booked.Contains(p.Id)))
					{
						item = 0.0;
					}
					else if (p.Position == Position.Defender)
					{
						item = 1.4;
					}
					else
					{
						item = ((p.Position == Position.Midfielder) ? 1.2 : 0.8);
					}
					list.Add(item);
				}
				Player who = PickWeighted(xi, cardW, random);
				if (who == null)
				{
					continue;
				}
				if (!red)
				{
					box.Add(who, 4, 1);
					booked.Add(who.Id);
				}
				else
				{
					box.Add(who, 9, 1);
					sentOff[who.Id] = Math.Max(0.02, Math.Min(1.0, (double)e.Seconds / 5400.0));
				}
				r.Kind = 1;
				r.Text = (red ? "Красная карточка: " : "Жёлтая карточка: ") + who.Name + (red ? " (удалён)" : "");
				r.Players.Add(who.Id);
			}
			box.Raw.Add(r);
		}
		int homeShots = Math.Max(homeGoals + 2, NormalInt(random, 10.5 + 1.6 * (double)homeGoals, 3.2, 3));
		int awayShots = Math.Max(awayGoals + 2, NormalInt(random, 10.5 + 1.6 * (double)awayGoals, 3.2, 3));
		for (int i = 0; i < hx.Count; i++)
		{
			if (sentOff.TryGetValue(hx[i].Id, out var share))
			{
				hs[i] *= share;
			}
		}
		for (int i2 = 0; i2 < ax.Count; i2++)
		{
			if (sentOff.TryGetValue(ax[i2].Id, out var share2))
			{
				asw[i2] *= share2;
			}
		}
		int[] he = Split(homeShots - homeGoals, hs, random);
		for (int i3 = 0; i3 < hx.Count; i3++)
		{
			box.Add(hx[i3], 3, he[i3]);
		}
		int[] ae = Split(awayShots - awayGoals, asw, random);
		for (int i4 = 0; i4 < ax.Count; i4++)
		{
			box.Add(ax[i4], 3, ae[i4]);
		}
		SoccerKeeper(box, homeKeeper, awayShots, awayGoals);
		SoccerKeeper(box, awayKeeper, homeShots, homeGoals);
		Timeline.Finish(box.Game, Sport.Soccer, box.Raw, 2, periodsFromEvents: true);
		if (g.Result == ResultType.Penalties)
		{
			box.Game.Note = "Штрафные минуты " + Math.Max(g.HomePenalties, g.AwayPenalties) + "-" + Math.Min(g.HomePenalties, g.AwayPenalties);
		}
	}

	private static Player SoccerXi(Box box, List<Player> roster, Random random, out List<Player> xi, out List<double> goalW, out List<double> assistW, out List<double> shotW)
	{
		List<Player> keepers = ByPos(roster, Position.Goalkeeper);
		xi = new List<Player>();
		goalW = new List<double>();
		assistW = new List<double>();
		shotW = new List<double>();
		AddField(xi, goalW, assistW, shotW, ByPos(roster, Position.Defender), 4, 0.12, 0.35, 0.2);
		AddField(xi, goalW, assistW, shotW, ByPos(roster, Position.Midfielder), 4, 0.45, 1.0, 0.6);
		AddField(xi, goalW, assistW, shotW, ByPos(roster, Position.Forward), 2, 1.0, 0.6, 1.0);
		foreach (Player p in xi)
		{
			box.Add(p, 0, 1);
		}
		if (keepers.Count != 0)
		{
			return keepers[0];
		}
		return null;
	}
}
