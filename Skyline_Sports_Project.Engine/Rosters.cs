using System;
using System.Collections.Generic;

namespace Skyline_Sports_Project.Engine;

public static class Rosters
{
	private class Group
	{
		public Position[] Positions;

		public int Top;

		public double Weight;

		public Group(double weight, int top, params Position[] positions)
		{
			Weight = weight;
			Top = top;
			Positions = positions;
		}
	}

	public enum Change
	{
		None,
		Breakout,
		Bust
	}

	private static readonly Dictionary<string, double> _par = new Dictionary<string, double>();

	public const int BenchGap = 7;

	public const double RatingScale = 2.5;

	public static RosterSlot[] Template(Sport sport, Level level)
	{
		bool pro = SportRules.Scale(level) == Level.Pro;
		return sport switch
		{
			Sport.Hockey => new RosterSlot[5]
			{
				new RosterSlot(Position.Center, 4, 3),
				new RosterSlot(Position.LeftWing, 4, 3),
				new RosterSlot(Position.RightWing, 4, 3),
				new RosterSlot(Position.Defense, 6, 4),
				new RosterSlot(Position.Goalie, 2, 1)
			}, 
			Sport.Basketball => new RosterSlot[5]
			{
				new RosterSlot(Position.PointGuard, pro ? 3 : 2, 1),
				new RosterSlot(Position.ShootingGuard, 3, 1),
				new RosterSlot(Position.SmallForward, 3, 1),
				new RosterSlot(Position.PowerForward, 2, 1),
				new RosterSlot(Position.BigMan, 2, 1)
			}, 
			Sport.Football => new RosterSlot[11]
			{
				new RosterSlot(Position.Quarterback, 3, 1),
				new RosterSlot(Position.RunningBack, 3, 1),
				new RosterSlot(Position.WideReceiver, 5, 3),
				new RosterSlot(Position.TightEnd, 2, 1),
				new RosterSlot(Position.OffensiveLine, 8, 5),
				new RosterSlot(Position.DefensiveLine, 6, 4),
				new RosterSlot(Position.Linebacker, 4, 3),
				new RosterSlot(Position.Cornerback, 4, 3),
				new RosterSlot(Position.Safety, 3, 2),
				new RosterSlot(Position.Kicker, 1, 1),
				new RosterSlot(Position.Punter, 1, 1)
			}, 
			Sport.Baseball => new RosterSlot[10]
			{
				new RosterSlot(Position.StartingPitcher, 5, 5),
				new RosterSlot(Position.ReliefPitcher, pro ? 10 : 9, 4),
				new RosterSlot(Position.Catcher, 2, 1),
				new RosterSlot(Position.FirstBase, 1, 1),
				new RosterSlot(Position.SecondBase, 2, 1),
				new RosterSlot(Position.Shortstop, 1, 1),
				new RosterSlot(Position.ThirdBase, 1, 1),
				new RosterSlot(Position.LeftField, 1, 1),
				new RosterSlot(Position.CenterField, 2, 1),
				new RosterSlot(Position.RightField, 1, 1)
			}, 
			_ => new RosterSlot[4]
			{
				new RosterSlot(Position.Goalkeeper, pro ? 3 : 2, 1),
				new RosterSlot(Position.Defender, pro ? 8 : 7, 4),
				new RosterSlot(Position.Midfielder, 7, 4),
				new RosterSlot(Position.Forward, 4, 2)
			}, 
		};
	}

	public static int Base(Level level)
	{
		return level switch
		{
			Level.HighSchool => 50, 
			Level.Junior => 57, 
			Level.College => 62, 
			Level.Minor => 67, 
			_ => 74, 
		};
	}

	public static int Display(Level level, double overall)
	{
		return Math.Max(20, Math.Min(99, (int)Math.Round(SportRules.Scale(level) switch
		{
			Level.Pro => (overall <= 82.0) ? (82.0 + (overall - 74.0) * 1.25) : (92.0 + (overall - 82.0) * 0.6), 
			Level.College => 68.0 + (overall - 62.0) * 1.2, 
			_ => 45.0 + (overall - 50.0) * 1.2, 
		})));
	}

	public static int Ovr(Player p)
	{
		return Display(p.Sport, p.Level, p.Overall);
	}

	public static int Display(Sport sport, Level level, double overall)
	{
		if (SportRules.Scale(level) != Level.Pro || overall <= 74.0)
		{
			return Display(level, overall);
		}
		TopAnchors(sport, out var a90, out var a95, out var a99);
		double d;
		if (overall <= a90)
		{
			d = 82.0 + (overall - 74.0) * 8.0 / (a90 - 74.0);
		}
		else
		{
			d = ((!(overall <= a95)) ? (95.0 + (overall - a95) * 4.0 / (a99 - a95)) : (90.0 + (overall - a90) * 5.0 / (a95 - a90)));
		}
		int shown = (int)Math.Round(d);
		if (shown >= 99 && overall < a99)
		{
			shown = 98;
		}
		return Math.Max(20, Math.Min(99, shown));
	}

	private static void TopAnchors(Sport sport, out double a90, out double a95, out double a99)
	{
		switch (sport)
		{
		case Sport.Hockey:
			a90 = 81.8;
			a95 = 85.6;
			a99 = 88.6;
			break;
		case Sport.Basketball:
			a90 = 79.9;
			a95 = 82.7;
			a99 = 85.7;
			break;
		case Sport.Football:
			a90 = 80.5;
			a95 = 85.6;
			a99 = 90.0;
			break;
		case Sport.Baseball:
			a90 = 86.1;
			a95 = 89.2;
			a99 = 91.6;
			break;
		default:
			a90 = 82.4;
			a95 = 86.2;
			a99 = 88.1;
			break;
		}
	}

	public static int MinOverall(Level level)
	{
		level = SportRules.Scale(level);
		return level switch
		{
			Level.College => 35, 
			Level.HighSchool => 25, 
			_ => 45, 
		};
	}

	public static int MaxOverall(Level level)
	{
		level = SportRules.Scale(level);
		return level switch
		{
			Level.College => 92, 
			Level.HighSchool => 85, 
			_ => 99, 
		};
	}

	public static string PositionLabel(Position p, Sport sport)
	{
		if (sport == Sport.Hockey)
		{
			return p switch
			{
				Position.Center => "Ц",
				Position.LeftWing => "ЛК",
				Position.RightWing => "ПК",
				Position.Defense => "З",
				Position.Goalie => "В",
				_ => PositionLabel(p),
			};
		}
		if (sport == Sport.Soccer)
		{
			return p switch
			{
				Position.Goalkeeper => "ВР",
				Position.Defender => "ЗЩ",
				Position.Midfielder => "ПЗ",
				Position.Forward => "НП",
				_ => PositionLabel(p),
			};
		}
		return PositionLabel(p);
	}

	public static Position[] Positions(Sport sport)
	{
		return sport switch
		{
			Sport.Hockey => new Position[] { Position.Center, Position.LeftWing, Position.RightWing, Position.Defense, Position.Goalie },
			Sport.Basketball => new Position[] { Position.PointGuard, Position.ShootingGuard, Position.SmallForward, Position.PowerForward, Position.BigMan },
			Sport.Football => new Position[] { Position.Quarterback, Position.RunningBack, Position.WideReceiver, Position.TightEnd, Position.OffensiveLine, Position.DefensiveLine, Position.Linebacker, Position.Cornerback, Position.Safety, Position.Kicker, Position.Punter },
			Sport.Baseball => new Position[] { Position.StartingPitcher, Position.ReliefPitcher, Position.Catcher, Position.FirstBase, Position.SecondBase, Position.Shortstop, Position.ThirdBase, Position.LeftField, Position.CenterField, Position.RightField },
			_ => new Position[] { Position.Goalkeeper, Position.Defender, Position.Midfielder, Position.Forward }
		};
	}

	public static string NewsPositionLabel(Position p, Sport sport)
	{
		if (sport == Sport.Hockey)
		{
			return p switch
			{
				Position.Center => "ЦН",
				Position.LeftWing => "ЛН",
				Position.RightWing => "ПН",
				Position.Defense => "З",
				Position.Goalie => "ВР",
				_ => PositionLabel(p)
			};
		}
		if (sport == Sport.Soccer)
		{
			return p switch
			{
				Position.Goalkeeper => "ВР",
				Position.Defender => "ЦЗ",
				Position.Midfielder => "ЦП",
				Position.Forward => "НП",
				_ => PositionLabel(p)
			};
		}
		if (sport == Sport.Football)
		{
			return p switch
			{
				Position.Quarterback => "КБ",
				Position.RunningBack => "РБ",
				Position.WideReceiver => "ВР",
				Position.TightEnd => "ТЭ",
				Position.OffensiveLine => "ОЛ",
				Position.DefensiveLine => "ДЛ",
				Position.Linebacker => "ЛБ",
				Position.Cornerback => "КБ",
				Position.Safety => "ДБ",
				Position.Kicker => "К",
				Position.Punter => "П",
				_ => PositionLabel(p)
			};
		}
		if (sport == Sport.Basketball)
		{
			return p switch
			{
				Position.PointGuard => "РЗ",
				Position.ShootingGuard => "АЗ",
				Position.SmallForward => "ЛФ",
				Position.PowerForward => "ТФ",
				Position.BigMan => "Ц",
				_ => PositionLabel(p)
			};
		}
		return PositionLabel(p);
	}

	public static string PositionLabel(Position p)
	{
		return p switch
		{
			Position.Center => "ЦН",
			Position.LeftWing => "ЛН",
			Position.RightWing => "ПН",
			Position.Defense => "З",
			Position.Goalie => "ВР",
			Position.PointGuard => "РЗ",
			Position.ShootingGuard => "АЗ",
			Position.SmallForward => "ЛФ",
			Position.PowerForward => "ТФ",
			Position.BigMan => "Ц",
			Position.Quarterback => "КБ",
			Position.RunningBack => "РБ",
			Position.WideReceiver => "ВР",
			Position.TightEnd => "ТЭ",
			Position.OffensiveLine => "ОЛ",
			Position.DefensiveLine => "ДЛ",
			Position.Linebacker => "ЛБ",
			Position.Cornerback => "КБ",
			Position.Safety => "ДБ",
			Position.Kicker => "К",
			Position.Punter => "П",
			Position.StartingPitcher => "СП",
			Position.ReliefPitcher => "РП",
			Position.Catcher => "К",
			Position.FirstBase => "1Б",
			Position.SecondBase => "2Б",
			Position.Shortstop => "ШС",
			Position.ThirdBase => "3Б",
			Position.LeftField => "ЛФ",
			Position.CenterField => "ЦФ",
			Position.RightField => "ПФ",
			Position.Goalkeeper => "ВР",
			Position.Defender => "ЦЗ",
			Position.Midfielder => "ЦП",
			Position.Forward => "НП",
			_ => p.ToString()
		};
	}

	public static bool IsPitcher(Position p)
	{
		if (p != Position.StartingPitcher)
		{
			return p == Position.ReliefPitcher;
		}
		return true;
	}

	public static string ClassLabel(int classYear)
	{
		return classYear switch
		{
			1 => "1 курс", 
			2 => "2 курс", 
			3 => "3 курс", 
			_ => "4 курс", 
		};
	}

	private static Group[] Groups(Sport sport)
	{
		return sport switch
		{
			Sport.Hockey => new Group[5]
			{
				new Group(0.16, 3, default(Position)),
				new Group(0.14, 3, Position.LeftWing),
				new Group(0.14, 3, Position.RightWing),
				new Group(0.26, 4, Position.Defense),
				new Group(0.3, 1, Position.Goalie)
			}, 
			Sport.Basketball => new Group[6]
			{
				new Group(0.15, 1, Position.PointGuard),
				new Group(0.15, 1, Position.ShootingGuard),
				new Group(0.15, 1, Position.SmallForward),
				new Group(0.15, 1, Position.PowerForward),
				new Group(0.15, 1, Position.BigMan),
				new Group(0.25, -4)
			}, 
			Sport.Football => new Group[11]
			{
				new Group(0.22, 1, Position.Quarterback),
				new Group(0.07, 1, Position.RunningBack),
				new Group(0.12, 3, Position.WideReceiver),
				new Group(0.04, 1, Position.TightEnd),
				new Group(0.15, 5, Position.OffensiveLine),
				new Group(0.14, 4, Position.DefensiveLine),
				new Group(0.1, 3, Position.Linebacker),
				new Group(0.08, 3, Position.Cornerback),
				new Group(0.05, 2, Position.Safety),
				new Group(0.02, 1, Position.Kicker),
				new Group(0.01, 1, Position.Punter)
			}, 
			Sport.Baseball => new Group[10]
			{
				new Group(0.0625, 1, Position.Catcher),
				new Group(0.0625, 1, Position.FirstBase),
				new Group(0.0625, 1, Position.SecondBase),
				new Group(0.0625, 1, Position.Shortstop),
				new Group(0.0625, 1, Position.ThirdBase),
				new Group(0.0625, 1, Position.LeftField),
				new Group(0.0625, 1, Position.CenterField),
				new Group(0.0625, 1, Position.RightField),
				new Group(0.3, 5, Position.StartingPitcher),
				new Group(0.2, 4, Position.ReliefPitcher)
			}, 
			_ => new Group[4]
			{
				new Group(0.15, 1, Position.Goalkeeper),
				new Group(0.3, 4, Position.Defender),
				new Group(0.3, 4, Position.Midfielder),
				new Group(0.25, 2, Position.Forward)
			}, 
		};
	}

	public static double Strength(Sport sport, List<Player> roster)
	{
		if (roster == null || roster.Count == 0)
		{
			return 0.0;
		}
		HashSet<int> used = new HashSet<int>();
		double total = 0.0;
		double weights = 0.0;
		Group[] array = Groups(sport);
		foreach (Group g in array)
		{
			List<Player> pool = new List<Player>();
			int top = g.Top;
			if (top < 0)
			{
				top = -top;
				foreach (Player p in roster)
				{
					if (!used.Contains(p.Id))
					{
						pool.Add(p);
					}
				}
			}
			else
			{
				foreach (Player p2 in roster)
				{
					if (Array.IndexOf(g.Positions, p2.Position) >= 0)
					{
						pool.Add(p2);
					}
				}
			}
			pool.Sort((Player a, Player b) => b.Overall.CompareTo(a.Overall));
			int n = Math.Min(top, pool.Count);
			if (n != 0)
			{
				double sum = 0.0;
				for (int i2 = 0; i2 < n; i2++)
				{
					sum += (double)pool[i2].Overall;
					used.Add(pool[i2].Id);
				}
				total += g.Weight * sum / (double)n;
				weights += g.Weight;
			}
		}
		if (!(weights <= 0.0))
		{
			return total / weights;
		}
		return 0.0;
	}

	public static double Par(Sport sport, Level level)
	{
		string key = sport.ToString() + "-" + level;
		if (_par.TryGetValue(key, out var par))
		{
			return par;
		}
		List<Player> roster = new List<Player>();
		int id = 1;
		RosterSlot[] array = Template(sport, level);
		foreach (RosterSlot slot in array)
		{
			for (int j = 0; j < slot.Count; j++)
			{
				roster.Add(new Player
				{
					Id = id++,
					Position = slot.Pos,
					Overall = Base(level) - ((j >= slot.Starters) ? 7 : 0)
				});
			}
		}
		par = Strength(sport, roster);
		_par[key] = par;
		return par;
	}

	public static int TeamRating(Sport sport, Level level, List<Player> roster)
	{
		if (roster == null || roster.Count == 0)
		{
			return 50;
		}
		double r = 50.0 + (Strength(sport, roster) - Par(sport, level)) * 2.5;
		return Math.Max(25, Math.Min(95, (int)Math.Round(r)));
	}

	public static int TeamOverall(Sport sport, List<Player> roster)
	{
		return (int)Math.Round(Strength(sport, roster));
	}

	public static List<Player> GenerateRoster(Team team, int targetRating, int seasonYear, Random random, Func<int> nextId, Dictionary<string, int> nameUse)
	{
		List<Player> roster = new List<Player>();
		double offset = (double)(targetRating - 50) / 2.5;
		HashSet<int> numbers = new HashSet<int>();
		RosterSlot[] array = Template(team.Sport, team.Level);
		foreach (RosterSlot slot in array)
		{
			for (int j = 0; j < slot.Count; j++)
			{
				bool starter = j < slot.Starters;
				int ovr = (int)Math.Round((double)Base(team.Level) + offset - (double)((!starter) ? 7 : 0) + Normal(random, 0.0, 3.5));
				Player p = CreatePlayer(team.Sport, team.Level, slot.Pos, ovr, team.Id, random, nextId, numbers, nameUse);
				if (SportRules.ProLike(team.Sport, team.Level))
				{
					int age = (starter ? (24 + random.Next(0, 9)) : (21 + random.Next(0, 14)));
					p.BirthYear = seasonYear - age;
					PeakAges(p.Sport, p.Position, out var peakStart, out var _);
					p.Potential = ((age < peakStart) ? (p.Overall + random.Next(2, 4 * (peakStart - age) + 4)) : (p.Overall + random.Next(0, 3)));
					Economy.InitialContract(p, team, age, random);
				}
				else if (team.Level == Level.Minor)
				{
					int age2 = (starter ? (22 + random.Next(0, 8)) : (20 + random.Next(0, 13)));
					p.BirthYear = seasonYear - age2;
					PeakAges(p.Sport, p.Position, out var peakStart2, out var _);
					p.Potential = ((age2 < peakStart2) ? (p.Overall + random.Next(2, 4 * (peakStart2 - age2) + 6)) : (p.Overall + random.Next(0, 3)));
				}
				else if (team.Level == Level.Junior)
				{
					p.ClassYear = (starter ? (2 + random.Next(0, 2)) : (1 + random.Next(0, 3)));
					p.BirthYear = seasonYear - (16 + p.ClassYear);
					p.Potential = p.Overall + random.Next(4, 17);
				}
				else
				{
					p.ClassYear = (starter ? (2 + random.Next(0, 3)) : (1 + random.Next(0, 4)));
					p.Potential = p.Overall + random.Next(3, 16);
				}
				roster.Add(p);
			}
		}
		return roster;
	}

	public static Player CreatePlayer(Sport sport, Level level, Position pos, int overall, int teamId, Random random, Func<int> nextId, HashSet<int> numbers, Dictionary<string, int> nameUse)
	{
		Player p = new Player
		{
			Id = nextId(),
			TeamId = teamId,
			Sport = sport,
			Level = level,
			Status = PlayerStatus.Active,
			Position = pos,
			Overall = Clamp(overall, MinOverall(level), MaxOverall(level))
		};
		p.PrevOverall = p.Overall;
		p.Priority = SportsWorld.NewPriority(random);
		PlayerNames.Pick(sport, level, random, nameUse, out p.FirstName, out p.LastName, pos);
		p.Number = PickNumber(pos, random, numbers ?? new HashSet<int>());
		numbers?.Add(p.Number);
		Bios.Give(p, random, cityKid: false);
		p.Season = Stats.Empty(sport);
		p.Career = Stats.Empty(sport);
		p.LastSeason = new int[0];
		p.Playoff = new int[0];
		return p;
	}

	public static int NewNumber(Position pos, Random random, List<Player> teammates, IEnumerable<int> retired = null)
	{
		HashSet<int> used = new HashSet<int>();
		foreach (Player p in teammates)
		{
			used.Add(p.Number);
		}
		if (retired != null)
		{
			foreach (int n in retired)
			{
				used.Add(n);
			}
		}
		return PickNumber(pos, random, used);
	}

	private static int PickNumber(Position pos, Random random, HashSet<int> used)
	{
		int[] range;
		switch (pos)
		{
		case Position.Goalie:
			range = new int[13]
			{
				1, 29, 30, 31, 33, 34, 35, 37, 39, 40,
				41, 70, 72
			};
			break;
		case Position.Quarterback:
		case Position.Kicker:
		case Position.Punter:
			range = Range(1, 19);
			break;
		case Position.RunningBack:
		case Position.Cornerback:
		case Position.Safety:
			range = Range(20, 49);
			break;
		case Position.WideReceiver:
			range = Concat(Range(1, 19), Range(80, 89));
			break;
		case Position.TightEnd:
			range = Range(80, 89);
			break;
		case Position.OffensiveLine:
			range = Range(50, 79);
			break;
		case Position.DefensiveLine:
			range = Concat(Range(90, 99), Range(50, 79));
			break;
		case Position.Linebacker:
			range = Concat(Range(40, 59), Range(90, 99));
			break;
		case Position.Goalkeeper:
			range = new int[6] { 1, 12, 13, 23, 30, 31 };
			break;
		case Position.PointGuard:
		case Position.ShootingGuard:
		case Position.SmallForward:
		case Position.PowerForward:
		case Position.BigMan:
			range = Concat(Range(0, 35), new int[7] { 40, 41, 42, 44, 45, 50, 55 });
			break;
		default:
			range = Range(2, 98);
			break;
		}
		List<int> free = new List<int>();
		int[] array = range;
		foreach (int n in array)
		{
			if (!used.Contains(n))
			{
				free.Add(n);
			}
		}
		if (free.Count > 0)
		{
			return free[random.Next(free.Count)];
		}
		for (int j = 1; j < 100; j++)
		{
			if (!used.Contains(j))
			{
				return j;
			}
		}
		return random.Next(1, 100);
	}

	private static int[] Range(int from, int to)
	{
		int[] r = new int[to - from + 1];
		for (int i = 0; i < r.Length; i++)
		{
			r[i] = from + i;
		}
		return r;
	}

	private static int[] Concat(int[] a, int[] b)
	{
		int[] r = new int[a.Length + b.Length];
		a.CopyTo(r, 0);
		b.CopyTo(r, a.Length);
		return r;
	}

	public static Player GenerateProspect(Sport sport, Position pos, int draftYear, Random random, Func<int> nextId, Dictionary<string, int> nameUse)
	{
		int age = sport switch
		{
			Sport.Hockey => 18 + ((random.NextDouble() < 0.3) ? 1 : 0), 
			Sport.Basketball => 19 + random.Next(0, 3), 
			Sport.Football => 21 + random.Next(0, 3), 
			_ => 18 + random.Next(0, 5), 
		};
		int gap = Math.Max(7, 16 - 2 * (age - 18));
		int ovr = (int)Math.Round((double)(Base(Level.Pro) - gap) + Normal(random, 0.0, 4.0));
		Player player = CreatePlayer(sport, Level.Pro, pos, ovr, -1, random, nextId, null, nameUse);
		player.Status = PlayerStatus.DraftPool;
		player.BirthYear = draftYear - age;
		PeakAges(sport, pos, out var peakStart, out var _);
		int growth = (int)Math.Round(Math.Max(2.0, (double)(peakStart - age) * 2.2 + Normal(random, 2.0, 5.0)));
		if (random.NextDouble() < 0.04)
		{
			growth += random.Next(6, 12);
		}
		player.Potential = Math.Min(player.Overall + growth, Base(Level.Pro) + 20);
		player.From = "Prospect";
		return player;
	}

	public static List<Player> AdvanceSchoolSeason(Team team, List<Player> roster, Random random, Func<int> nextId, Dictionary<string, int> nameUse, Func<Position, Player> recruit, out List<Player> joined, List<KeyValuePair<Player, Change>> changes, double developmentBoost = 0.0, double recruitingEdge = 0.0)
	{
		List<Player> left = new List<Player>();
		joined = new List<Player>();
		foreach (Player p in roster)
		{
			if (p.ClassYear >= ((team.Level == Level.Junior) ? 3 : 4))
			{
				left.Add(p);
				continue;
			}
			if (team.Level == Level.College && p.ClassYear == 3 && p.Overall >= 78 && random.NextDouble() < 0.4)
			{
				left.Add(p);
				continue;
			}
			if (team.Level == Level.Junior && p.ClassYear == 2 && p.Overall >= 70 && random.NextDouble() < 0.35)
			{
				left.Add(p);
				continue;
			}
			p.PrevOverall = p.Overall;
			Change c = DevelopStudent(p, team.Level, random, developmentBoost);
			if (c != Change.None)
			{
				changes?.Add(new KeyValuePair<Player, Change>(p, c));
			}
			p.ClassYear++;
		}
		foreach (Player p2 in left)
		{
			roster.Remove(p2);
		}
		HashSet<int> numbers = new HashSet<int>();
		foreach (Player p3 in roster)
		{
			numbers.Add(p3.Number);
		}
		double program = (double)(team.Rating - 50) / 2.5;
		RosterSlot[] array = Template(team.Sport, team.Level);
		foreach (RosterSlot slot in array)
		{
			int have = 0;
			foreach (Player item in roster)
			{
				if (item.Position == slot.Pos)
				{
					have++;
				}
			}
			for (int j = have; j < slot.Count; j++)
			{
				Player p4 = recruit?.Invoke(slot.Pos);
				if (p4 != null)
				{
					p4.TeamId = team.Id;
					p4.Level = team.Level;
					p4.Status = PlayerStatus.Active;
					p4.ClassYear = 1;
					p4.Number = PickNumber(slot.Pos, random, numbers);
					numbers.Add(p4.Number);
				}
				else
				{
					int gap = ((team.Level == Level.HighSchool) ? 12 : 9);
					int ovr = (int)Math.Round((double)(Base(team.Level) - gap) + program * 0.8 + recruitingEdge + Normal(random, 0.0, 5.0));
					p4 = CreatePlayer(team.Sport, team.Level, slot.Pos, ovr, team.Id, random, nextId, numbers, nameUse);
					p4.ClassYear = 1;
					p4.Potential = p4.Overall + random.Next(8, 23);
				}
				roster.Add(p4);
				joined.Add(p4);
			}
		}
		return left;
	}

	public static void PeakAges(Sport sport, Position pos, out int start, out int end)
	{
		switch (pos)
		{
		case Position.Center:
		case Position.LeftWing:
		case Position.RightWing:
			start = 24;
			end = 28;
			return;
		case Position.Defense:
			start = 25;
			end = 29;
			return;
		case Position.Goalie:
			start = 26;
			end = 31;
			return;
		case Position.Quarterback:
			start = 27;
			end = 33;
			return;
		case Position.RunningBack:
			start = 23;
			end = 26;
			return;
		case Position.WideReceiver:
		case Position.TightEnd:
		case Position.DefensiveLine:
		case Position.Safety:
			start = 25;
			end = 29;
			return;
		case Position.OffensiveLine:
			start = 26;
			end = 31;
			return;
		case Position.Linebacker:
		case Position.Cornerback:
			start = 24;
			end = 28;
			return;
		case Position.Kicker:
		case Position.Punter:
			start = 26;
			end = 36;
			return;
		case Position.ReliefPitcher:
			start = 26;
			end = 31;
			return;
		case Position.Forward:
			start = 24;
			end = 28;
			return;
		case Position.Defender:
			start = 25;
			end = 30;
			return;
		case Position.Goalkeeper:
			start = 27;
			end = 33;
			return;
		case Position.Midfielder:
			start = 25;
			end = 29;
			return;
		}
		if (sport == Sport.Baseball)
		{
			start = 26;
			end = 30;
		}
		else
		{
			start = 25;
			end = 29;
		}
	}

	public static Change DevelopPro(Player p, int age, Random random)
	{
		PeakAges(p.Sport, p.Position, out var start, out var end);
		Change result = ((age <= start) ? UpdatePotential(p, Level.Pro, random) : Change.None);
		int delta;
		if (age < start)
		{
			double num = Math.Max(0, p.Potential - p.Overall);
			delta = (int)Math.Round(num * (0.22 + 0.25 * random.NextDouble()) + Normal(random, 0.0, 1.2));
			if (num > 0.0 && delta < 1)
			{
				delta = 1;
			}
			if (num <= 0.0)
			{
				delta = random.Next(-1, 2);
			}
		}
		else if (age <= end)
		{
			delta = Clamp((int)Math.Round(Normal(random, 0.2, 1.2)), -2, 2);
			if (p.Overall + delta > p.Potential)
			{
				delta = Math.Max(0, Math.Min(delta, p.Potential - p.Overall));
			}
		}
		else
		{
			int years = age - end;
			double decline = 1.0 + 0.7 * (double)years + ((age >= 33) ? 1.0 : 0.0) + ((age >= 36) ? 1.5 : 0.0);
			delta = -(int)Math.Round(Math.Max(0.0, Normal(random, decline, 1.2)));
			if (years <= 2 && random.NextDouble() < 0.06)
			{
				delta = random.Next(1, 3);
			}
		}
		p.Overall = Clamp(p.Overall + Soften(p.Overall, delta, Level.Pro, random), MinOverall(Level.Pro), MaxOverall(Level.Pro));
		CapPotential(p, Level.Pro);
		return result;
	}

	private static int Soften(int current, int delta, Level level, Random random)
	{
		if (delta <= 0)
		{
			return delta;
		}
		int elite = Base(level) + 12;
		if (current < elite)
		{
			return delta;
		}
		double factor = ((current >= elite + 6) ? 0.25 : 0.5);
		return (int)Math.Floor((double)delta * factor + random.NextDouble());
	}

	private static void CapPotential(Player p, Level level)
	{
		int cap = Math.Max(p.Overall, Base(level) + 20);
		if (p.Potential > cap)
		{
			p.Potential = cap;
		}
		if (p.Potential < p.Overall)
		{
			p.Potential = p.Overall;
		}
	}

	public static double TeamDevelopmentSpread(Sport sport)
	{
		return sport switch
		{
			Sport.Basketball => 1.9, 
			Sport.Football => 1.7, 
			_ => 1.6, 
		};
	}

	public static void ApplyEnvironment(Player p, int age, double environment)
	{
		PeakAges(p.Sport, p.Position, out var start, out var end);
		double weight;
		if (age < start)
		{
			weight = 1.0;
		}
		else
		{
			weight = ((age <= end) ? 0.6 : 0.4);
		}
		int delta = (int)Math.Round(environment * weight);
		if (delta > 0 && p.Overall >= Base(Level.Pro) + 12)
		{
			delta = (delta + 1) / 2;
		}
		if (delta != 0)
		{
			p.Overall = Clamp(p.Overall + delta, MinOverall(Level.Pro), MaxOverall(Level.Pro));
			if (age < end)
			{
				p.Potential += delta;
			}
			CapPotential(p, Level.Pro);
		}
	}

	public static Change DevelopStudent(Player p, Level level, Random random, double coachBoost = 0.0)
	{
		Change result = UpdatePotential(p, level, random);
		double num = Math.Max(0, p.Potential - p.Overall);
		int delta = (int)Math.Round(num * (0.3 + coachBoost + 0.25 * random.NextDouble()) + Normal(random, 0.0, 1.2));
		if (num > 0.0 && delta < 1)
		{
			delta = 1;
		}
		if (num <= 0.0)
		{
			delta = random.Next(0, 2);
		}
		p.Overall = Clamp(p.Overall + Soften(p.Overall, delta, level, random), MinOverall(level), MaxOverall(level));
		CapPotential(p, level);
		return result;
	}

	private static Change UpdatePotential(Player p, Level level, Random random)
	{
		double r = random.NextDouble();
		Change change = Change.None;
		bool elite = p.Potential >= Base(level) + 12;
		if (r < 0.08)
		{
			p.Potential += (elite ? random.Next(2, 5) : random.Next(4, 11));
			change = Change.Breakout;
		}
		else if (r < 0.16)
		{
			p.Potential -= random.Next(4, 11);
			change = Change.Bust;
		}
		else
		{
			p.Potential += (int)Math.Round(Normal(random, elite ? (-0.3) : 0.0, 1.2));
		}
		CapPotential(p, level);
		return change;
	}

	public static int Projected(Player p, int age)
	{
		if (p.Level != Level.Pro && p.Status != PlayerStatus.DraftPool)
		{
			return Math.Max(p.Overall, p.Potential);
		}
		PeakAges(p.Sport, p.Position, out var _, out var end);
		if (age >= end)
		{
			return p.Overall;
		}
		return Math.Max(p.Overall, p.Potential);
	}

	public static string Grade(Player p, int age)
	{
		int diff = Projected(p, age) - Base(p.Level);
		if (diff >= 14)
		{
			return "A+";
		}
		if (diff >= 10)
		{
			return "A";
		}
		if (diff >= 7)
		{
			return "A-";
		}
		if (diff >= 4)
		{
			return "B+";
		}
		if (diff >= 1)
		{
			return "B";
		}
		if (diff >= -2)
		{
			return "B-";
		}
		if (diff >= -5)
		{
			return "C+";
		}
		if (diff >= -8)
		{
			return "C";
		}
		return "D";
	}

	public static void Normalize(Sport sport, Level level, List<List<Player>> rosters, List<Player> extras, double target = 50.0)
	{
		if (rosters.Count == 0)
		{
			return;
		}
		double mean = 0.0;
		int n = 0;
		foreach (List<Player> r in rosters)
		{
			if (r.Count != 0)
			{
				mean += (double)TeamRating(sport, level, r);
				n++;
			}
		}
		if (n == 0)
		{
			return;
		}
		mean /= (double)n;
		int shift = (int)Math.Round((target - mean) / 2.5);
		if (shift == 0)
		{
			return;
		}
		List<Player> all = new List<Player>();
		foreach (List<Player> r2 in rosters)
		{
			all.AddRange(r2);
		}
		if (extras != null)
		{
			all.AddRange(extras);
		}
		foreach (Player item in all)
		{
			item.Overall = Clamp(item.Overall + shift, MinOverall(level), MaxOverall(level));
			item.Potential += shift;
			item.PrevOverall += shift;
		}
	}

	public static int Clamp(int v, int min, int max)
	{
		if (v >= min)
		{
			if (v <= max)
			{
				return v;
			}
			return max;
		}
		return min;
	}

	public static double Normal(Random random, double mean, double sd)
	{
		double u1 = 1.0 - random.NextDouble();
		double u2 = random.NextDouble();
		return mean + sd * Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(Math.PI * 2.0 * u2);
	}
}
