using System.Collections.Generic;

namespace Skyline_Sports_Project.Engine;

public static class Standings
{
	public static List<StandingRow> Compute(League league, Season season)
	{
		Dictionary<int, StandingRow> rows = new Dictionary<int, StandingRow>();
		List<int> order = new List<int>();
		if (season != null)
		{
			foreach (Match g in season.Games)
			{
				AddRow(rows, order, g.HomeTeamId);
				AddRow(rows, order, g.AwayTeamId);
			}
		}
		if (season == league.Current)
		{
			foreach (int id in league.TeamIds)
			{
				AddRow(rows, order, id);
			}
		}
		Dictionary<int, List<Match>> playedByTeam = new Dictionary<int, List<Match>>();
		if (season != null)
		{
			List<Match> list = new List<Match>(season.Games);
			list.Sort((Match a, Match b) => a.Time.CompareTo(b.Time));
			foreach (Match g2 in list)
			{
				if (g2.Played && g2.Stage == GameStage.Regular && g2.HomeTeamId >= 0 && g2.AwayTeamId >= 0)
				{
					Apply(league.Sport, rows[g2.HomeTeamId], g2, isHome: true);
					Apply(league.Sport, rows[g2.AwayTeamId], g2, isHome: false);
					AddPlayed(playedByTeam, g2.HomeTeamId, g2);
					AddPlayed(playedByTeam, g2.AwayTeamId, g2);
				}
			}
		}
		List<StandingRow> list2 = new List<StandingRow>();
		foreach (int id2 in order)
		{
			StandingRow row = rows[id2];
			if (league.Sport == Sport.Hockey)
			{
				row.Points = 2 * row.Wins + row.OvertimeLosses;
			}
			else if (league.Sport == Sport.Soccer)
			{
				row.Points = 3 * row.Wins + row.Ties;
			}
			else
			{
				row.Points = 2 * row.Wins + row.Ties;
			}
			row.Streak = (playedByTeam.TryGetValue(id2, out var played) ? StreakOf(id2, played) : "");
			list2.Add(row);
		}
		bool usesPoints = league.Sport == Sport.Hockey || league.Sport == Sport.Soccer;
		list2.Sort((StandingRow a, StandingRow b) =>
		{
			int num;
			if (usesPoints)
			{
				num = b.Points.CompareTo(a.Points);
				if (num != 0)
				{
					return num;
				}
				num = a.Played.CompareTo(b.Played);
				if (num != 0)
				{
					return num;
				}
			}
			else
			{
				num = b.WinPct.CompareTo(a.WinPct);
				if (num != 0)
				{
					return num;
				}
			}
			num = b.Wins.CompareTo(a.Wins);
			if (num != 0)
			{
				return num;
			}
			num = b.Differential.CompareTo(a.Differential);
			if (num != 0)
			{
				return num;
			}
			num = b.PointsFor.CompareTo(a.PointsFor);
			return (num != 0) ? num : a.TeamId.CompareTo(b.TeamId);
		});
		for (int i = 0; i < list2.Count; i++)
		{
			list2[i].Rank = i + 1;
		}
		return list2;
	}

	public static string RecordText(Sport sport, StandingRow row)
	{
		if (row == null)
		{
			return "0-0";
		}
		return sport switch
		{
			Sport.Hockey => row.Wins + "-" + row.Losses + "-" + row.OvertimeLosses, 
			Sport.Soccer => row.Wins + "-" + row.Ties + "-" + row.Losses, 
			_ => row.Wins + "-" + row.Losses, 
		};
	}

	private static void AddRow(Dictionary<int, StandingRow> rows, List<int> order, int teamId)
	{
		if (teamId >= 0 && !rows.ContainsKey(teamId))
		{
			rows[teamId] = new StandingRow
			{
				TeamId = teamId
			};
			order.Add(teamId);
		}
	}

	private static void AddPlayed(Dictionary<int, List<Match>> map, int teamId, Match g)
	{
		if (!map.TryGetValue(teamId, out var list))
		{
			list = (map[teamId] = new List<Match>());
		}
		list.Add(g);
	}

	private static void Apply(Sport sport, StandingRow row, Match g, bool isHome)
	{
		int forScore = (isHome ? g.HomeScore : g.AwayScore);
		int againstScore = (isHome ? g.AwayScore : g.HomeScore);
		row.Played++;
		row.PointsFor += forScore;
		row.PointsAgainst += againstScore;
		if (forScore > againstScore)
		{
			row.Wins++;
		}
		else if (forScore < againstScore)
		{
			if (sport == Sport.Hockey && (g.Result == ResultType.Overtime || g.Result == ResultType.Shootout))
			{
				row.OvertimeLosses++;
			}
			else
			{
				row.Losses++;
			}
		}
		else
		{
			row.Ties++;
		}
	}

	private static string StreakOf(int teamId, List<Match> played)
	{
		if (played.Count == 0)
		{
			return "";
		}
		char kind = ' ';
		int count = 0;
		for (int i = played.Count - 1; i >= 0; i--)
		{
			Match g = played[i];
			int mine = ((g.HomeTeamId == teamId) ? g.HomeScore : g.AwayScore);
			int theirs = ((g.HomeTeamId == teamId) ? g.AwayScore : g.HomeScore);
			char k;
			if (mine > theirs)
			{
				k = 'W';
			}
			else
			{
				k = ((mine < theirs) ? 'L' : 'T');
			}
			if (count == 0)
			{
				kind = k;
				count = 1;
			}
			else
			{
				if (k != kind)
				{
					break;
				}
				count++;
			}
		}
		return kind.ToString() + count;
	}
}
