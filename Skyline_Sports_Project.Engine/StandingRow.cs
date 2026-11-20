namespace Skyline_Sports_Project.Engine;

public class StandingRow
{
	public int TeamId;

	public int Played;

	public int Wins;

	public int Losses;

	public int Ties;

	public int OvertimeLosses;

	public int PointsFor;

	public int PointsAgainst;

	public int Points;

	public string Streak = "";

	public int Rank;

	public double WinPct
	{
		get
		{
			if (Played != 0)
			{
				return ((double)Wins + 0.5 * (double)Ties) / (double)Played;
			}
			return 0.0;
		}
	}

	public int Differential => PointsFor - PointsAgainst;
}
