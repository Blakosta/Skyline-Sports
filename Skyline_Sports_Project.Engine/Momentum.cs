using System;

namespace Skyline_Sports_Project.Engine;

public static class Momentum
{
	public static bool PlayoffMomentum = true;

	public static double Sharpness(Sport sport, Level level)
	{
		level = SportRules.Scale(level);
		return level switch
		{
			Level.Pro => sport switch
			{
				Sport.Hockey => 1.45, 
				Sport.Basketball => 1.95, 
				Sport.Football => 3.3, 
				Sport.Baseball => 1.08, 
				_ => 2.5, 
			}, 
			Level.College => sport switch
			{
				Sport.Hockey => 1.4, 
				Sport.Basketball => 2.4, 
				Sport.Football => 4.6, 
				Sport.Baseball => 1.6, 
				_ => 2.5, 
			}, 
			_ => sport switch
			{
				Sport.Hockey => 1.6, 
				Sport.Basketball => 2.6, 
				Sport.Football => 5.0, 
				Sport.Baseball => 1.55, 
				_ => 2.4, 
			}, 
		};
	}

	public static double HomeAdvantage(Sport sport, Level level)
	{
		level = SportRules.Scale(level);
		return level switch
		{
			Level.Pro => sport switch
			{
				Sport.Hockey => 3.1, 
				Sport.Basketball => 2.0, 
				Sport.Football => 2.0, 
				Sport.Baseball => 4.0, 
				_ => 8.0, 
			}, 
			Level.College => sport switch
			{
				Sport.Hockey => 4.0, 
				Sport.Basketball => 4.5, 
				Sport.Football => 2.5, 
				Sport.Baseball => 3.1, 
				_ => 7.0, 
			}, 
			_ => sport switch
			{
				Sport.Hockey => 2.7, 
				Sport.Basketball => 2.8, 
				Sport.Football => 1.9, 
				Sport.Baseball => 2.8, 
				_ => 6.0, 
			}, 
		};
	}

	public static double MaxFormPoints(Sport sport)
	{
		if (sport != Sport.Football)
		{
			return 2.0;
		}
		return 1.5;
	}

	public static double PlayoffIntensity(Sport sport)
	{
		return sport switch
		{
			Sport.Basketball => 1.15, 
			Sport.Football => 1.4, 
			_ => 1.0, 
		};
	}

	public static double EdgeSigma(Sport sport)
	{
		return sport switch
		{
			Sport.Hockey => 1.0, 
			Sport.Baseball => 1.2, 
			Sport.Soccer => 1.0, 
			Sport.Football => 0.4, 
			_ => 0.6, 
		};
	}

	public static void Belief(Sport sport, out double gapShare, out double maxShare, out double cap)
	{
		switch (sport)
		{
		case Sport.Basketball:
			gapShare = 0.15;
			maxShare = 0.3;
			cap = 2.0;
			break;
		case Sport.Hockey:
			gapShare = 0.2;
			maxShare = 0.4;
			cap = 2.5;
			break;
		default:
			gapShare = 0.2;
			maxShare = 0.4;
			cap = 2.5;
			break;
		}
	}

	public static void Run(Sport sport, out double step, out double cap)
	{
		switch (sport)
		{
		case Sport.Basketball:
			step = 0.5;
			cap = 1.0;
			break;
		case Sport.Football:
			step = 0.5;
			cap = 0.5;
			break;
		default:
			step = 0.5;
			cap = 1.0;
			break;
		}
	}

	public static double FormBonus(Team t)
	{
		if (t != null)
		{
			return (double)t.Form / 1000.0 * MaxFormPoints(t.Sport);
		}
		return 0.0;
	}

	public static double HomeWinChance(Sport sport, Level level, double homeRating, double awayRating, bool neutral, bool playoff)
	{
		double gap = (homeRating - awayRating + (neutral ? 0.0 : HomeAdvantage(sport, level))) * Sharpness(sport, level) * (playoff ? PlayoffIntensity(sport) : 1.0);
		return 1.0 / (1.0 + Math.Exp((0.0 - gap) * Slope(sport, level)));
	}

	private static double Slope(Sport sport, Level level)
	{
		level = SportRules.Scale(level);
		return level switch
		{
			Level.Pro => sport switch
			{
				Sport.Hockey => 0.04, 
				Sport.Basketball => 0.06, 
				Sport.Football => 0.032, 
				Sport.Baseball => 0.036, 
				_ => 0.03, 
			}, 
			Level.College => sport switch
			{
				Sport.Hockey => 0.041, 
				Sport.Basketball => 0.048, 
				Sport.Football => 0.038, 
				Sport.Baseball => 0.042, 
				_ => 0.033, 
			}, 
			_ => sport switch
			{
				Sport.Hockey => 0.043, 
				Sport.Basketball => 0.037, 
				Sport.Football => 0.035, 
				Sport.Baseball => 0.042, 
				_ => 0.034, 
			}, 
		};
	}

	public static void UpdateForm(Team home, Team away, Match g, double homeChance)
	{
		int winner = g.WinnerId;
		double homeResult;
		if (winner < 0)
		{
			homeResult = 0.5;
		}
		else
		{
			homeResult = ((winner == g.HomeTeamId) ? 1.0 : 0.0);
		}
		if (home != null)
		{
			home.Form = Step(home.Form, homeResult - homeChance);
		}
		if (away != null)
		{
			away.Form = Step(away.Form, 1.0 - homeResult - (1.0 - homeChance));
		}
	}

	private static int Step(int form, double surprise)
	{
		double f = (double)form * 0.8 + 250.0 * surprise;
		return (int)Math.Round(Math.Max(-1000.0, Math.Min(1000.0, f)));
	}

	public static int NewEdge(Sport sport, Random random)
	{
		return (int)Math.Round(Rosters.Normal(random, 0.0, EdgeSigma(sport)) * 10.0);
	}

	public static void SeriesRatings(Team high, Team low, PlayoffSeries ps, int lowWins, out double highRating, out double lowRating)
	{
		if (!PlayoffMomentum)
		{
			highRating = (double)(high?.Rating ?? 50) + FormBonus(high);
			lowRating = (double)(low?.Rating ?? 50) + FormBonus(low);
			return;
		}
		highRating = (double)(high?.Rating ?? 50) + FormBonus(high) + (double)ps.HighEdge / 10.0 + ((high == null) ? 0.0 : ((double)high.RunBonus / 10.0));
		lowRating = (double)(low?.Rating ?? 50) + FormBonus(low) + (double)ps.LowEdge / 10.0 + ((low == null) ? 0.0 : ((double)low.RunBonus / 10.0));
		if (lowWins > 0 && high != null)
		{
			Belief(high.Sport, out var share, out var maxShare, out var cap);
			double gap = Math.Max(0.0, highRating - lowRating);
			lowRating += Math.Min(cap, gap * Math.Min(maxShare, (double)lowWins * share));
		}
	}

	public static void AfterSeries(Team winner, bool lowerSeedWon)
	{
		if (winner != null && lowerSeedWon)
		{
			Run(winner.Sport, out var step, out var cap);
			winner.RunBonus = (int)Math.Round(Math.Min(cap * 10.0, (double)winner.RunBonus + step * 10.0));
		}
	}
}
