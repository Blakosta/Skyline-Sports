using System;

namespace Skyline_Sports_Project.Engine;

public class SportProfile
{
	public Sport Sport;

	public Level Level;

	public string Summary = "";

	public int GamesPerTeam;

	public int ByesPerTeam;

	public Func<int, DateTime> Start;

	public Func<int, DateTime> End;

	public Func<int, DateRange[]> Breaks;

	public SlotTemplate[] Week;

	public bool Weekly;

	public double PlayoffShare = 0.5;

	public bool PlayoffPowerOfTwo = true;

	public int MinPlayoffTeams = 2;

	public int MaxPlayoffTeams = 16;

	public PlayoffRoundFormat[] RoundsFromFinal;

	public bool PlayoffsBackwardFromFinal;

	public string ChampionLabel = "Чемпионы";

	public bool SingleYearLabel;

	public Func<int, DateTime> PlayoffStart;

	public PlayoffRoundFormat RoundFormat(int round, int totalRounds)
	{
		int fromFinal = totalRounds - 1 - round;
		if (fromFinal < RoundsFromFinal.Length)
		{
			return RoundsFromFinal[fromFinal];
		}
		return RoundsFromFinal[RoundsFromFinal.Length - 1];
	}

	public bool IsBreak(int seasonYear, DateTime date)
	{
		if (Breaks == null)
		{
			return false;
		}
		DateRange[] array = Breaks(seasonYear);
		foreach (DateRange r in array)
		{
			if (r.Contains(date))
			{
				return true;
			}
		}
		return false;
	}
}
