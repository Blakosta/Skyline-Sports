using System;

namespace Skyline_Sports_Project.Engine;

public class SlotTemplate
{
	public DayOfWeek Anchor;

	public int Length = 1;

	public bool Optional;

	public double HomeAndHomeChance;

	public Func<DayOfWeek, bool, TimeOption[]> Times;
}
