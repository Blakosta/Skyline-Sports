using System;

namespace Skyline_Sports_Project.Engine;

public class EconomyProfile
{
	public Sport Sport;

	public CapType Type;

	public int RealCap;

	public int RealRoster;

	public int MinSalary;

	public double MaxShare;

	public int MaxYearsResign;

	public int MaxYearsFree;

	public int RookieYears;

	public int DraftRounds;

	public int MaxProspects;

	public int[] LotteryOdds;

	public int LotteryPicks;

	public int MaxJump;

	public string CapName = "Потолок зарплат";

	public string DraftName = "Драфт";

	public Func<int, DateTime> LotteryDate;

	public Func<int, DateTime> DraftDate;

	public Func<int, DateTime> LeagueYearDate;

	public Func<int, DateTime> TradeDeadline;
}
