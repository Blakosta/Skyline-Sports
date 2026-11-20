using System;
using System.Collections.Generic;
using System.Globalization;

namespace Skyline_Sports_Project.Engine;

public static class Timeline
{
	public class Raw
	{
		public int Period;

		public int Seconds;

		public bool Home;

		public int Points;

		public int Kind;

		public string Text = "";

		public string Clock = "";

		public List<int> Players = new List<int>();
	}

	public static string Ordinal(int n)
	{
		int m100 = n % 100;
		int m101 = n % 10;
		string suffix = ((m100 >= 11 && m100 <= 13) ? "th" : (m101 switch
		{
			3 => "rd", 
			2 => "nd", 
			1 => "st", 
			_ => "th", 
		}));
		return n.ToString(CultureInfo.InvariantCulture) + suffix;
	}

	public static string MinSec(int seconds)
	{
		if (seconds < 0)
		{
			seconds = 0;
		}
		return (seconds / 60).ToString(CultureInfo.InvariantCulture) + ":" + (seconds % 60).ToString("00", CultureInfo.InvariantCulture);
	}

	public static string PeriodLabel(Sport sport, int period, int regulation)
	{
		switch (sport)
		{
		case Sport.Baseball:
			return period.ToString(CultureInfo.InvariantCulture);
		case Sport.Soccer:
			return period switch
			{
				2 => "2-й тайм", 
				1 => "1-й тайм", 
				_ => "Доп. время", 
			};
		default:
		{
			if (period <= regulation)
			{
				if (sport != Sport.Hockey)
				{
					return "Ч" + period;
				}
				return period.ToString(CultureInfo.InvariantCulture);
			}
			int ot = period - regulation;
			if (ot != 1)
			{
				return ot + "ОТ";
			}
			return "ОТ";
		}
		}
	}

	public static string PeriodHeading(Sport sport, int period, int regulation)
	{
		switch (sport)
		{
		case Sport.Hockey:
			if (period <= regulation)
			{
				return Ordinal(period) + " период";
			}
			if (period - regulation != 1)
			{
				return Ordinal(period - regulation) + " овертайм";
			}
			return "Овертайм";
		case Sport.Basketball:
		case Sport.Football:
			if (period <= regulation)
			{
				return Ordinal(period) + " четверть";
			}
			if (period - regulation != 1)
			{
				return Ordinal(period - regulation) + " овертайм";
			}
			return "Овертайм";
		case Sport.Baseball:
			return Ordinal((period + 1) / 2) + " иннинг";
		default:
			if (period != 1)
			{
				return "Второй тайм";
			}
			return "Первый тайм";
		}
	}

	public static int Regulation(Sport sport)
	{
		switch (sport)
		{
		case Sport.Hockey:
			return 3;
		case Sport.Basketball:
		case Sport.Football:
			return 4;
		case Sport.Baseball:
			return 9;
		default:
			return 2;
		}
	}

	public static void Finish(GameBox box, Sport sport, List<Raw> raw, int periods, bool periodsFromEvents)
	{
		raw.Sort((Raw a, Raw b) =>
		{
			int result = ((a.Period == b.Period) ? ((a.Seconds == b.Seconds) ? a.Kind.CompareTo(b.Kind) : a.Seconds.CompareTo(b.Seconds)) : a.Period.CompareTo(b.Period));
			return result;
		});
		int home = 0;
		int away = 0;
		if (periodsFromEvents)
		{
			box.HomePeriods.Clear();
			box.AwayPeriods.Clear();
			for (int i = 0; i < periods; i++)
			{
				box.HomePeriods.Add(0);
				box.AwayPeriods.Add(0);
			}
		}
		int regulation = Regulation(sport);
		foreach (Raw r in raw)
		{
			if (r.Points > 0)
			{
				if (r.Home)
				{
					home += r.Points;
				}
				else
				{
					away += r.Points;
				}
				if (periodsFromEvents && r.Period >= 1 && r.Period <= periods)
				{
					if (r.Home)
					{
						box.HomePeriods[r.Period - 1] += r.Points;
					}
					else
					{
						box.AwayPeriods[r.Period - 1] += r.Points;
					}
				}
			}
			string clock = r.Clock;
			if (clock.Length == 0)
			{
				clock = sport switch
				{
					Sport.Hockey => MinSec(r.Seconds), 
					Sport.Football => MinSec(((r.Period > regulation) ? 600 : 900) - r.Seconds), 
					Sport.Soccer => r.Seconds + "'", 
					_ => "", 
				};
			}
			box.Events.Add(new BoxEvent
			{
				Period = r.Period,
				Clock = clock,
				Home = r.Home,
				Kind = r.Kind,
				Text = r.Text,
				HomeScore = home,
				AwayScore = away,
				Players = r.Players
			});
		}
	}

	public static List<int> SplitPoints(int total, int periods, double spread, Random random)
	{
		List<int> list = new List<int>();
		if (periods <= 0)
		{
			return list;
		}
		double[] w = new double[periods];
		double sum = 0.0;
		for (int i = 0; i < periods; i++)
		{
			w[i] = Math.Max(0.2, 1.0 + Rosters.Normal(random, 0.0, spread));
			sum += w[i];
		}
		int given = 0;
		for (int j = 0; j < periods; j++)
		{
			int v = (int)Math.Floor((double)total * w[j] / sum);
			list.Add(v);
			given += v;
		}
		int k = 0;
		for (; given < total; given++)
		{
			list[random.Next(periods)]++;
			k++;
		}
		return list;
	}
}
