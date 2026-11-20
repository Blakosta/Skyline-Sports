using System.Collections.Generic;

namespace Skyline_Sports_Project.Engine;

public class Season
{
	public int Year;

	public int StartDay;

	public int RegularEndDay;

	public int PlayoffEndDay;

	public bool PlayoffsCreated;

	public bool Complete;

	public int ChampionTeamId = -1;

	public int PlayoffRounds;

	public List<Match> Games = new List<Match>();

	public List<PlayoffSeries> Bracket = new List<PlayoffSeries>();

	public List<int> PlannedRoundDays = new List<int>();

	public bool AnyPlayed
	{
		get
		{
			foreach (Match game in Games)
			{
				if (game.Played)
				{
					return true;
				}
			}
			return false;
		}
	}

	public PlayoffSeries FindSeries(int id)
	{
		if (id == 0)
		{
			return null;
		}
		foreach (PlayoffSeries ps in Bracket)
		{
			if (ps.Id == id)
			{
				return ps;
			}
		}
		return null;
	}
}
