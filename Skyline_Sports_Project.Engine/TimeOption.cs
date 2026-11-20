namespace Skyline_Sports_Project.Engine;

public class TimeOption
{
	public int DayOffset;

	public int Minute;

	public double Weight;

	public int Cap;

	public TimeOption(int dayOffset, int minute, double weight, int cap)
	{
		DayOffset = dayOffset;
		Minute = minute;
		Weight = weight;
		Cap = cap;
	}
}
