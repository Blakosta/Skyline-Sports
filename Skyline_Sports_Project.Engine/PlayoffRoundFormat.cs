using System;

namespace Skyline_Sports_Project.Engine;

public class PlayoffRoundFormat
{
	public string Name = "Плей-офф";

	public int BestOf = 1;

	public int[] DayOffsets = new int[1];

	public bool[] HighSeedHome = new bool[1] { true };

	public bool Neutral;

	public Func<int, DateTime> FixedDate;

	public int RestDays = 2;

	public DayOfWeek[] StartDays;

	public int Stagger;

	public Func<DayOfWeek, TimeOption[]> Times;

	public int WinsNeeded => BestOf / 2 + 1;
}
