namespace Skyline_Sports_Project.Engine;

public class PlayoffSeries
{
	public int Id;

	public int Round;

	public int Slot;

	public int HighSeed;

	public int LowSeed;

	public int HighTeamId = -1;

	public int LowTeamId = -1;

	public int BestOf = 1;

	public bool Neutral;

	public int WinnerId = -1;

	public int PlannedEndDay;

	public int HighEdge;

	public int LowEdge;

	public bool IsBye => LowTeamId < 0;

	public int WinsNeeded => BestOf / 2 + 1;

	public int SeedOf(int teamId)
	{
		if (teamId != HighTeamId)
		{
			return LowSeed;
		}
		return HighSeed;
	}

	public bool Involves(int teamId)
	{
		if (teamId >= 0)
		{
			if (HighTeamId != teamId)
			{
				return LowTeamId == teamId;
			}
			return true;
		}
		return false;
	}
}
