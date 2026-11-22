using System;
using System.Collections.Generic;

namespace Skyline_Sports_Project.Engine;

public static class ScheduleBuilder
{
	private class HomeBalancer
	{
		private readonly Dictionary<int, int> _home = new Dictionary<int, int>();

		private readonly Dictionary<long, int> _lastHost = new Dictionary<long, int>();

		public Pairing Orient(Pairing pair, int games, bool homeAndHome)
		{
			int a = pair.Home;
			int b = pair.Away;
			int ha = Count(a);
			int hb = Count(b);
			long key = ((a < b) ? (((long)a << 32) | (uint)b) : (((long)b << 32) | (uint)a));
			int host;
			if (_lastHost.TryGetValue(key, out var last))
			{
				host = ((last == a) ? b : a);
				int other = ((host == a) ? b : a);
				if (Count(host) - Count(other) > games)
				{
					host = other;
				}
			}
			else
			{
				host = ((ha <= hb) ? a : b);
			}
			int guest = ((host == a) ? b : a);
			_lastHost[key] = host;
			if (homeAndHome)
			{
				_home[host] = Count(host) + games / 2;
				_home[guest] = Count(guest) + (games - games / 2);
			}
			else
			{
				_home[host] = Count(host) + games;
			}
			return new Pairing(host, guest);
		}

		private int Count(int team)
		{
			if (!_home.TryGetValue(team, out var c))
			{
				return 0;
			}
			return c;
		}
	}

	public class Booking
	{
		public int Time;

		public int SeriesId;

		public Sport Sport;
	}

	public const int VenueGapMinutes = 240;

	public const int IndoorChangeoverMinutes = 600;

	public const int OutdoorChangeoverMinutes = 720;

	public const int DoubleheaderGapMinutes = 150;

	public static int GapMinutes(Sport a, Sport b, bool outdoor)
	{
		if (a == b)
		{
			return 240;
		}
		if (!outdoor)
		{
			return 600;
		}
		return 720;
	}

	private static List<Slot> SoccerSeasonSlots(SportProfile p, int year)
	{
		List<Slot> slots = new List<Slot>();
		if (p.Week == null || p.Week.Length == 0) return slots;
		SlotTemplate primary = p.Week[p.Week.Length - 1];
		int[] months = new int[10] { 7, 8, 9, 10, 11, 12, 2, 3, 4, 5 };
		int[] counts = new int[10] { 3, 3, 4, 3, 3, 3, 3, 4, 3, 1 };
		for (int m = 0; m < months.Length; m++)
		{
			int month = months[m];
			int calendarYear = (month >= 7) ? year : year + 1;
			DateTime first = new DateTime(calendarYear, month, 1);
			DateTime last = first.AddMonths(1).AddDays(-1.0);
			List<DateTime> saturdays = new List<DateTime>();
			for (DateTime d = first; d <= last; d = d.AddDays(1.0))
			{
				if (d.DayOfWeek == DayOfWeek.Saturday) saturdays.Add(d);
			}
			int count = counts[m];
			for (int i = 0; i < count && saturdays.Count > 0; i++)
			{
				int index = (count == 1) ? (saturdays.Count - 1) : (int)Math.Round((double)i * (double)(saturdays.Count - 1) / (double)(count - 1), MidpointRounding.AwayFromZero);
				DateTime date = saturdays[index];
				slots.Add(new Slot { Template = primary, Days = new int[1] { GameClock.DayNumber(date) }, Optional = false });
			}
		}
		// Optional midweek slots extend the calendar only when a division has more than 16 clubs.
		if (p.Week.Length > 1 && p.Week[0].Optional)
		{
			int[] extraMonths = new int[8] { 7, 8, 9, 10, 11, 12, 2, 3 };
			foreach (int month in extraMonths)
			{
				int calendarYear = (month >= 7) ? year : year + 1;
				DateTime date = GameClock.Nth(calendarYear, month, p.Week[0].Anchor, 2);
				if (!p.IsBreak(year, date))
				{
					slots.Add(new Slot { Template = p.Week[0], Days = new int[1] { GameClock.DayNumber(date) }, Optional = true });
				}
			}
		}
		slots.Sort((Slot a, Slot b) => a.FirstDay.CompareTo(b.FirstDay));
		return slots;
	}

	public static List<Slot> SeasonSlots(SportProfile p, int year)
	{
		if (p.Sport == Sport.Soccer && (p.Level == Level.Pro || p.Level == Level.Minor))
		{
			return SoccerSeasonSlots(p, year);
		}
		List<Slot> slots = new List<Slot>();
		DateTime start = p.Start(year).Date;
		DateTime end = p.End(year).Date;
		DateTime week = GameClock.OnOrBefore(start, DayOfWeek.Monday);
		while (week <= end)
		{
			SlotTemplate[] week2 = p.Week;
			foreach (SlotTemplate t in week2)
			{
				DateTime anchor = week.AddDays((int)(t.Anchor + 6) % 7);
				int length = Math.Max(1, t.Length);
				int[] days = new int[length];
				bool ok = true;
				for (int j = 0; j < length; j++)
				{
					DateTime d = anchor.AddDays(j);
					if (d < start || d > end || p.IsBreak(year, d))
					{
						ok = false;
						break;
					}
					days[j] = GameClock.DayNumber(d);
				}
				if (ok)
				{
					slots.Add(new Slot
					{
						Template = t,
						Days = days,
						Optional = t.Optional
					});
				}
			}
			week = week.AddDays(7.0);
		}
		slots.Sort((Slot a, Slot b) => a.FirstDay.CompareTo(b.FirstDay));
		return slots;
	}

	public static List<Slot> SelectSlots(List<Slot> all, int targetGames)
	{
		List<Slot> collection = all.FindAll((Slot slot) => !slot.Optional);
		List<Slot> optional = all.FindAll((Slot slot) => slot.Optional);
		List<Slot> selected = new List<Slot>(collection);
		int total = Total(selected);
		if (total < targetGames && optional.Count > 0)
		{
			int picks = Math.Min(optional.Count, targetGames - total);
			foreach (int index in EvenIndices(optional.Count, picks))
			{
				selected.Add(optional[index]);
			}
			selected.Sort((Slot a, Slot b) => a.FirstDay.CompareTo(b.FirstDay));
			total = Total(selected);
		}
		int guard = 0;
		while (total > targetGames && guard++ < 1000)
		{
			int over = total - targetGames;
			int smallest = int.MaxValue;
			foreach (Slot s in selected)
			{
				smallest = Math.Min(smallest, s.Size);
			}
			if (smallest > over)
			{
				break;
			}
			List<Slot> candidates = selected.FindAll((Slot slot) => slot.Size == smallest);
			int removeCount = Math.Min(candidates.Count, Math.Max(1, over / smallest));
			foreach (int index2 in EvenIndices(candidates.Count, removeCount))
			{
				selected.Remove(candidates[index2]);
			}
			total = Total(selected);
		}
		return selected;
	}

	public static List<Match> BuildRegularSeason(SportProfile p, int year, List<int> teamIds, int today, int targetGames, Random random, Func<int> nextId)
	{
		List<Match> games = new List<Match>();
		if (teamIds.Count < 2)
		{
			return games;
		}
		List<Slot> all = SeasonSlots(p, year);
		int byes = (p.Weekly ? p.ByesPerTeam : 0);
		List<Slot> selected = SelectSlots(all, targetGames + byes);
		if (selected.Count > 0 && EarliestDay(selected[0]) <= today)
		{
			selected = selected.FindAll((Slot s) => EarliestDay(s) > today);
			byes = 0;
		}
		if (selected.Count == 0)
		{
			return games;
		}
		List<List<Pairing>> rounds = Rounds(teamIds, selected.Count, random);
		if (byes > 0)
		{
			RemoveByes(rounds, byes, random);
		}
		if (p.Sport == Sport.Soccer)
		{
			BalanceSoccerRounds(rounds, teamIds, random);
		}
		TimePicker picker = new TimePicker(random);
		HomeBalancer balancer = new HomeBalancer();
		DateTime start = p.Start(year).Date;
		DateTime end = p.End(year).Date;
		for (int i = 0; i < rounds.Count && i < selected.Count; i++)
		{
			Slot slot = selected[i];
			string scope = "r" + i;
			foreach (Pairing original in rounds[i])
			{
				bool homeAndHome = slot.Size > 1 && slot.Template.HomeAndHomeChance > 0.0 && random.NextDouble() < slot.Template.HomeAndHomeChance;
				Pairing pair = (p.Sport == Sport.Soccer) ? original : balancer.Orient(original, slot.Size, homeAndHome);
				if (slot.Size == 1)
				{
					List<TimeOption> options = ValidOptions(p, year, slot.FirstDay, slot.Template.Times(GameClock.Weekday(slot.FirstDay), arg2: true), start, end, today);
					TimeOption o = picker.Pick(scope, options);
					bool finalRound = p.Sport == Sport.Soccer && i == rounds.Count - 1;
					int day = finalRound ? slot.FirstDay : ((o == null) ? slot.FirstDay : (slot.FirstDay + o.DayOffset));
					int minute = finalRound ? 1200 : (o?.Minute ?? 1140);
					games.Add(new Match
					{
						Id = nextId(),
						SeriesId = nextId(),
						GameNumber = 1,
						Time = GameClock.ToTime(day, minute),
						HomeTeamId = pair.Home,
						AwayTeamId = pair.Away,
						Stage = GameStage.Regular
					});
					continue;
				}
				int seriesId = nextId();
				for (int d = 0; d < slot.Size; d++)
				{
					int day2 = slot.Days[d];
					List<TimeOption> options2 = new List<TimeOption>(slot.Template.Times(GameClock.Weekday(day2), d == slot.Size - 1));
					TimeOption o2 = picker.Pick(scope + "d" + d, options2);
					bool swap = homeAndHome && d % 2 == 1;
					games.Add(new Match
					{
						Id = nextId(),
						SeriesId = (homeAndHome ? nextId() : seriesId),
						GameNumber = (homeAndHome ? 1 : (d + 1)),
						Time = GameClock.ToTime(day2, o2?.Minute ?? 1140),
						HomeTeamId = (swap ? pair.Away : pair.Home),
						AwayTeamId = (swap ? pair.Home : pair.Away),
						Stage = GameStage.Regular
					});
				}
			}
		}
		return games;
	}

	public static int RemainingGames(SportProfile p, int year, int targetGames, int today)
	{
		int byes = (p.Weekly ? p.ByesPerTeam : 0);
		int total = 0;
		foreach (Slot s in SelectSlots(SeasonSlots(p, year), targetGames + byes))
		{
			if (EarliestDay(s) > today)
			{
				total += s.Size;
			}
		}
		return total;
	}

	private static int EarliestDay(Slot s)
	{
		if (s.Size > 1)
		{
			return s.FirstDay;
		}
		int min = s.FirstDay;
		TimeOption[] array = s.Template.Times(GameClock.Weekday(s.FirstDay), arg2: true);
		foreach (TimeOption o in array)
		{
			min = Math.Min(min, s.FirstDay + Math.Min(0, o.DayOffset));
		}
		return min;
	}

	private static List<TimeOption> ValidOptions(SportProfile p, int year, int anchorDay, TimeOption[] options, DateTime start, DateTime end, int today)
	{
		List<TimeOption> valid = new List<TimeOption>();
		TimeOption[] array = options;
		foreach (TimeOption o in array)
		{
			int day = anchorDay + o.DayOffset;
			DateTime d = GameClock.DateOf(day);
			if (!(d < start) && !(d > end) && !p.IsBreak(year, d) && day > today)
			{
				valid.Add(o);
			}
		}
		if (valid.Count == 0)
		{
			array = options;
			foreach (TimeOption o2 in array)
			{
				if (o2.DayOffset == 0)
				{
					valid.Add(o2);
				}
			}
		}
		return valid;
	}

	private static int Total(List<Slot> slots)
	{
		int total = 0;
		foreach (Slot s in slots)
		{
			total += s.Size;
		}
		return total;
	}

	public static List<int> EvenIndices(int n, int k)
	{
		List<int> result = new List<int>();
		if (k <= 0 || n <= 0)
		{
			return result;
		}
		if (k >= n)
		{
			for (int i = 0; i < n; i++)
			{
				result.Add(i);
			}
			return result;
		}
		HashSet<int> used = new HashSet<int>();
		for (int j = 0; j < k; j++)
		{
			int index = (int)Math.Floor(((double)j + 0.5) * (double)n / (double)k);
			while (used.Contains(index))
			{
				index = (index + 1) % n;
			}
			used.Add(index);
			result.Add(index);
		}
		return result;
	}

	public static List<List<Pairing>> Rounds(List<int> teamIds, int roundsNeeded, Random random)
	{
		List<List<Pairing>> result = new List<List<Pairing>>();
		if (teamIds.Count < 2 || roundsNeeded <= 0)
		{
			return result;
		}
		List<int> teams = new List<int>(teamIds);
		for (int i = teams.Count - 1; i > 0; i--)
		{
			int j = random.Next(i + 1);
			int tmp = teams[i];
			teams[i] = teams[j];
			teams[j] = tmp;
		}
		if (teams.Count % 2 == 1)
		{
			teams.Add(-1);
		}
		int n = teams.Count;
		List<List<Pairing>> cycle = new List<List<Pairing>>();
		List<int> rotation = new List<int>(teams);
		for (int r = 0; r < n - 1; r++)
		{
			List<Pairing> round = new List<Pairing>();
			for (int k = 0; k < n / 2; k++)
			{
				int a = rotation[k];
				int b = rotation[n - 1 - k];
				if (a >= 0 && b >= 0)
				{
					bool aHome = ((k == 0) ? (r % 2 == 0) : ((r + k) % 2 == 0));
					round.Add(aHome ? new Pairing(a, b) : new Pairing(b, a));
				}
			}
			cycle.Add(round);
			int last = rotation[n - 1];
			rotation.RemoveAt(n - 1);
			rotation.Insert(1, last);
		}
		int cycleNumber = 0;
		while (result.Count < roundsNeeded)
		{
			bool flip = cycleNumber % 2 == 1;
			List<List<Pairing>> order = new List<List<Pairing>>(cycle);
			if (cycleNumber > 1)
			{
				for (int i2 = order.Count - 1; i2 > 0; i2--)
				{
					int j2 = random.Next(i2 + 1);
					List<Pairing> tmp2 = order[i2];
					order[i2] = order[j2];
					order[j2] = tmp2;
				}
			}
			foreach (List<Pairing> round2 in order)
			{
				if (result.Count >= roundsNeeded)
				{
					break;
				}
				List<Pairing> copy = new List<Pairing>();
				foreach (Pairing p in round2)
				{
					copy.Add(flip ? new Pairing(p.Away, p.Home) : new Pairing(p.Home, p.Away));
				}
				result.Add(copy);
			}
			cycleNumber++;
			if (cycle.Count == 0)
			{
				break;
			}
		}
		return result;
	}

	private static long PairKey(int a, int b)
	{
		return (a < b) ? (((long)a << 32) | (uint)b) : (((long)b << 32) | (uint)a);
	}

	private static int SoccerHomePenalty(int team, Dictionary<int, int> home, Dictionary<int, int> away, Dictionary<int, bool> lastHome, Dictionary<int, int> streak)
	{
		int penalty = Math.Max(0, home[team] - away[team]) * 4;
		if (lastHome.TryGetValue(team, out var last) && last) penalty += (streak[team] >= 2) ? 1000 : 20;
		return penalty;
	}

	private static int SoccerAwayPenalty(int team, Dictionary<int, int> home, Dictionary<int, int> away, Dictionary<int, bool> lastHome, Dictionary<int, int> streak)
	{
		int penalty = Math.Max(0, away[team] - home[team]) * 4;
		if (lastHome.TryGetValue(team, out var last) && !last) penalty += (streak[team] >= 2) ? 1000 : 20;
		return penalty;
	}

	private static void RecordSoccerVenue(int team, bool isHome, Dictionary<int, bool> lastHome, Dictionary<int, int> streak)
	{
		if (lastHome.TryGetValue(team, out var last) && last == isHome) streak[team]++;
		else streak[team] = 1;
		lastHome[team] = isHome;
	}

	private static void BalanceSoccerRounds(List<List<Pairing>> rounds, List<int> teamIds, Random random)
	{
		Dictionary<int, int> home = new Dictionary<int, int>();
		Dictionary<int, int> away = new Dictionary<int, int>();
		Dictionary<int, bool> lastHome = new Dictionary<int, bool>();
		Dictionary<int, int> streak = new Dictionary<int, int>();
		Dictionary<long, int> firstHost = new Dictionary<long, int>();
		Dictionary<long, int> meetings = new Dictionary<long, int>();
		foreach (int team in teamIds) { home[team] = 0; away[team] = 0; }
		foreach (List<Pairing> round in rounds)
		{
			for (int i = 0; i < round.Count; i++)
			{
				Pairing original = round[i];
				int a = original.Home;
				int b = original.Away;
				long key = PairKey(a, b);
				int seen = meetings.TryGetValue(key, out var count) ? count : 0;
				int host;
				if (seen == 1 && firstHost.TryGetValue(key, out var previousHost)) host = (previousHost == a) ? b : a;
				else
				{
					int scoreA = SoccerHomePenalty(a, home, away, lastHome, streak) + SoccerAwayPenalty(b, home, away, lastHome, streak);
					int scoreB = SoccerHomePenalty(b, home, away, lastHome, streak) + SoccerAwayPenalty(a, home, away, lastHome, streak);
					host = (scoreA == scoreB) ? ((random.Next(2) == 0) ? a : b) : ((scoreA < scoreB) ? a : b);
				}
				int guest = (host == a) ? b : a;
				round[i] = new Pairing(host, guest);
				home[host]++;
				away[guest]++;
				RecordSoccerVenue(host, true, lastHome, streak);
				RecordSoccerVenue(guest, false, lastHome, streak);
				if (seen == 0) firstHost[key] = host;
				meetings[key] = seen + 1;
			}
		}
	}

	public static void RemoveByes(List<List<Pairing>> rounds, int byes, Random random)
	{
		int count = rounds.Count;
		if (count < 4 || byes <= 0)
		{
			return;
		}
		int from = count / 4;
		int to = Math.Max(from + 1, count * 4 / 5);
		HashSet<int> teams = new HashSet<int>();
		foreach (List<Pairing> round2 in rounds)
		{
			foreach (Pairing p in round2)
			{
				teams.Add(p.Home);
				teams.Add(p.Away);
			}
		}
		for (int attempt = 0; attempt < 30; attempt++)
		{
			List<List<Pairing>> work = new List<List<Pairing>>();
			foreach (List<Pairing> r in rounds)
			{
				work.Add(new List<Pairing>(r));
			}
			Dictionary<int, int> byeCount = new Dictionary<int, int>();
			foreach (int t in teams)
			{
				byeCount[t] = 0;
			}
			for (int b = 1; b <= byes; b++)
			{
				List<int> order = new List<int>();
				for (int i = from; i < to; i++)
				{
					order.Add(i);
				}
				for (int i2 = order.Count - 1; i2 > 0; i2--)
				{
					int j = random.Next(i2 + 1);
					int tmp = order[i2];
					order[i2] = order[j];
					order[j] = tmp;
				}
				for (int pass = 0; pass < 3; pass++)
				{
					foreach (int ri in order)
					{
						List<Pairing> round = work[ri];
						for (int gi = 0; gi < round.Count; gi++)
						{
							Pairing p2 = round[gi];
							if (byeCount[p2.Home] < b && byeCount[p2.Away] < b)
							{
								round.RemoveAt(gi);
								byeCount[p2.Home]++;
								byeCount[p2.Away]++;
								break;
							}
						}
					}
				}
			}
			bool ok = true;
			foreach (int t2 in teams)
			{
				if (byeCount[t2] != byes)
				{
					ok = false;
				}
			}
			if (ok || attempt == 29)
			{
				for (int k = 0; k < rounds.Count; k++)
				{
					rounds[k] = work[k];
				}
				break;
			}
		}
	}

	public static bool Conflicts(Dictionary<int, List<Booking>> occupancy, int venueId, int time, int seriesId, Sport sport, Func<int, bool> outdoor)
	{
		if (venueId < 0 || !occupancy.TryGetValue(venueId, out var list))
		{
			return false;
		}
		bool open = outdoor?.Invoke(venueId) ?? false;
		foreach (Booking b in list)
		{
			int distance = (sport == Sport.Soccer && b.Sport == Sport.Soccer)
				? Math.Abs(GameClock.MonthIndex(b.Time) * GameClock.MinutesPerDay + GameClock.MinuteOfDay(b.Time) - (GameClock.MonthIndex(time) * GameClock.MinutesPerDay + GameClock.MinuteOfDay(time)))
				: Math.Abs(b.Time - time);
			if ((b.SeriesId != seriesId || seriesId == 0) && distance < GapMinutes(b.Sport, sport, open))
			{
				return true;
			}
		}
		return false;
	}

	public static void Occupy(Dictionary<int, List<Booking>> occupancy, int venueId, int time, int seriesId, Sport sport)
	{
		if (venueId >= 0)
		{
			if (!occupancy.TryGetValue(venueId, out var list))
			{
				list = (occupancy[venueId] = new List<Booking>());
			}
			list.Add(new Booking
			{
				Time = time,
				SeriesId = seriesId,
				Sport = sport
			});
		}
	}

	public static void AssignVenues(List<Match> games, Func<int, Team> getTeam, Dictionary<int, List<Booking>> occupancy, Sport sport, Func<int, bool> outdoor)
	{
		Dictionary<int, List<Match>> bySeries = new Dictionary<int, List<Match>>();
		List<int> order = new List<int>();
		List<Match> sorted = new List<Match>();
		foreach (Match g in games)
		{
			if (!g.Played && g.Stage == GameStage.Regular)
			{
				sorted.Add(g);
			}
		}
		sorted.Sort((Match a, Match b) => (a.Time == b.Time) ? a.Id.CompareTo(b.Id) : a.Time.CompareTo(b.Time));
		foreach (Match g2 in sorted)
		{
			int key = ((g2.SeriesId != 0) ? g2.SeriesId : (-g2.Id));
			if (!bySeries.TryGetValue(key, out var list))
			{
				list = (bySeries[key] = new List<Match>());
				order.Add(key);
			}
			list.Add(g2);
		}
		foreach (int key2 in order)
		{
			List<Match> series = bySeries[key2];
			Team home = getTeam(series[0].HomeTeamId);
			Team away = getTeam(series[0].AwayTeamId);
			int venue = ((home != null && home.IsUser) ? home.VenueId : (-1));
			if (venue >= 0 && AnyConflict(occupancy, venue, series, key2, sport, outdoor))
			{
				if (sport == Sport.Soccer)
				{
					// A game day represents a whole sports month. Keep the home venue and
					// move kickoff to a free time instead of sending soccer matches neutral.
					if (!TryFindSoccerTimes(occupancy, venue, series, key2, sport, outdoor))
					{
						venue = -2;
					}
				}
				else
				{
					int other = ((away != null && away.IsUser) ? away.VenueId : (-1));
					if (other < 0 || !AnyConflict(occupancy, other, series, key2, sport, outdoor))
					{
						foreach (Match item in series)
						{
							int tmp = item.HomeTeamId;
							item.HomeTeamId = item.AwayTeamId;
							item.AwayTeamId = tmp;
						}
						venue = other;
					}
					else
					{
						venue = -2;
					}
				}
			}
			foreach (Match g3 in series)
			{
				g3.VenueId = venue;
				Occupy(occupancy, venue, g3.Time, key2, sport);
			}
		}
	}

	private static bool TryFindSoccerTimes(Dictionary<int, List<Booking>> occupancy, int venue, List<Match> series, int seriesKey, Sport sport, Func<int, bool> outdoor)
	{
		int[] candidates = new int[15] { 480, 540, 600, 660, 720, 780, 840, 900, 960, 1020, 1080, 1140, 1200, 1260, 1320 };
		List<int> chosenMinutes = new List<int>();
		foreach (Match game in series)
		{
			int originalMinute = GameClock.MinuteOfDay(game.Time);
			int day = GameClock.DayOf(game.Time);
			int chosen = -1;
			int bestDistance = int.MaxValue;
			foreach (int minute in candidates)
			{
				int candidateTime = GameClock.ToTime(day, minute);
				if (Conflicts(occupancy, venue, candidateTime, seriesKey, sport, outdoor))
				{
					continue;
				}
				int distance = Math.Abs(minute - originalMinute);
				if (distance < bestDistance)
				{
					bestDistance = distance;
					chosen = minute;
				}
			}
			if (chosen < 0)
			{
				return false;
			}
			chosenMinutes.Add(chosen);
		}
		for (int i = 0; i < series.Count; i++)
		{
			Match game2 = series[i];
			game2.Time = GameClock.ToTime(GameClock.DayOf(game2.Time), chosenMinutes[i]);
		}
		return true;
	}

	private static bool AnyConflict(Dictionary<int, List<Booking>> occupancy, int venue, List<Match> series, int seriesKey, Sport sport, Func<int, bool> outdoor)
	{
		foreach (Match g in series)
		{
			if (Conflicts(occupancy, venue, g.Time, seriesKey, sport, outdoor))
			{
				return true;
			}
		}
		return false;
	}
}
