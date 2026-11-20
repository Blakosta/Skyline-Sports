namespace Skyline_Sports_Project.Engine;

public class Match
{
	public int Id;

	public int Time;

	public int HomeTeamId;

	public int AwayTeamId;

	public int VenueId = -1;

	public GameStage Stage;

	public bool Played;

	public int HomeScore;

	public int AwayScore;

	public ResultType Result;

	public int HomePenalties;

	public int AwayPenalties;

	public int SeriesId;

	public int GameNumber = 1;

	public int PlayoffRound;

	public bool Featured;

	public string Summary = "";

	public GameBox Box;

	public GameFlow Flow;

	public int Weather = -1;

	public int RainDelay;

	public int PostponedFrom = -1;

	public int Attendance;

	public int HomeGoalieId = -1;

	public int AwayGoalieId = -1;

	public LiveGame Live;

	public bool Spotlight;

	public int WinnerId
	{
		get
		{
			if (!Played)
			{
				return -1;
			}
			if (HomeScore > AwayScore)
			{
				return HomeTeamId;
			}
			if (AwayScore > HomeScore)
			{
				return AwayTeamId;
			}
			if (Result == ResultType.Penalties)
			{
				if (HomePenalties <= AwayPenalties)
				{
					return AwayTeamId;
				}
				return HomeTeamId;
			}
			return -1;
		}
	}

	public int LoserId
	{
		get
		{
			int w = WinnerId;
			if (w < 0)
			{
				return -1;
			}
			if (w != HomeTeamId)
			{
				return HomeTeamId;
			}
			return AwayTeamId;
		}
	}

	public bool Involves(int teamId)
	{
		if (HomeTeamId != teamId)
		{
			return AwayTeamId == teamId;
		}
		return true;
	}
}
