using System.Collections.Generic;

namespace Skyline_Sports_Project.Engine;

public class League
{
	public Sport Sport;

	public Level Level;

	public List<int> TeamIds = new List<int>();

	public List<int> PendingTeamIds = new List<int>();

	public Season Current;

	public Season Previous;

	public List<ChampionRecord> Champions = new List<ChampionRecord>();

	public List<AwardRecord> Awards = new List<AwardRecord>();

	public DraftState Draft;

	public int LotteryDoneFor = int.MinValue;

	public int DraftDoneFor = int.MinValue;

	public int LeagueYearDoneFor = int.MinValue;

	public int TradeMonth = -1;

	public List<PickAsset> Picks = new List<PickAsset>();

	public List<RecordBoard> Records = new List<RecordBoard>();

	public List<SeasonSummary> History = new List<SeasonSummary>();

	public int CoachingDoneFor = int.MinValue;

	public List<string> SeasonNotes = new List<string>();

	public List<Rivalry> Rivalries = new List<Rivalry>();

	public List<HallEntry> HallOfFame = new List<HallEntry>();

	public List<HallEntry> HallWaiting = new List<HallEntry>();

	public List<string> Transfers = new List<string>();

	public List<AllStarPick> AllStars = new List<AllStarPick>();

	public int AllStarYear = int.MinValue;

	public int AllLeagueYear = int.MinValue;

	public int PromotionDoneFor = int.MinValue;

	public List<int> LastUp = new List<int>();

	public List<int> LastDown = new List<int>();

	public int LastUpByPlayoff = -1;

	public string Key => MakeKey(Sport, Level);

	public static string MakeKey(Sport sport, Level level)
	{
		return sport.ToString() + "-" + level;
	}
}
