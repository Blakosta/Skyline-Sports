namespace Skyline_Sports_Project.Engine;

public class CityWeatherMonth
{
	public int Month;

	public int Samples;

	public double SumTempC;

	public int Wet;

	public int Snow;

	public int Heavy;

	public double MeanTempC
	{
		get
		{
			if (Samples <= 0)
			{
				return 0.0;
			}
			return SumTempC / (double)Samples;
		}
	}

	public double WetShare
	{
		get
		{
			if (Samples <= 0)
			{
				return 0.0;
			}
			return (double)Wet / (double)Samples;
		}
	}
}
