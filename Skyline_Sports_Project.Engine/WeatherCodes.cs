using System;

namespace Skyline_Sports_Project.Engine;

public static class WeatherCodes
{
	public static bool UseCelsius;

	public static int Encode(WeatherKind kind, int tempF)
	{
		return (int)kind * 1000 + Math.Max(0, Math.Min(999, tempF + 100));
	}

	public static WeatherKind Kind(int code)
	{
		if (code >= 0)
		{
			return (WeatherKind)(code / 1000);
		}
		return WeatherKind.Clear;
	}

	public static int TempF(int code)
	{
		if (code >= 0)
		{
			return code % 1000 - 100;
		}
		return 0;
	}

	public static string KindLabel(WeatherKind kind)
	{
		return kind switch
		{
			WeatherKind.Cloudy => "Облачно", 
			WeatherKind.Rain => "Дождь", 
			WeatherKind.HeavyRain => "Сильный дождь", 
			WeatherKind.Snow => "Снег", 
			WeatherKind.Storm => "Гроза", 
			WeatherKind.Dome => "Под крышей", 
			_ => "Ясно", 
		};
	}

	public static string TempLabel(int code)
	{
		int f = TempF(code);
		if (!UseCelsius)
		{
			return f + "°F";
		}
		return (int)Math.Round((double)(f - 32) / 1.8) + "°C";
	}

	public static string Label(int code)
	{
		if (code < 0)
		{
			return "";
		}
		WeatherKind k = Kind(code);
		if (k == WeatherKind.Dome)
		{
			return KindLabel(k);
		}
		return KindLabel(k) + ", " + TempLabel(code);
	}

	public static double Scoring(Sport sport, int code)
	{
		if (code < 0)
		{
			return 1.0;
		}
		WeatherKind k = Kind(code);
		int t = TempF(code);
		double m = 1.0;
		switch (sport)
		{
		case Sport.Soccer:
			m = k switch
			{
				WeatherKind.Rain => 0.97, 
				WeatherKind.Storm => 0.94, 
				WeatherKind.HeavyRain => 0.92, 
				WeatherKind.Snow => 0.88, 
				_ => 1.0, 
			};
			break;
		case Sport.Baseball:
			m = ((t < 45) ? 0.9 : ((t < 55) ? 0.95 : ((t > 85) ? 1.04 : 1.0))) * ((k == WeatherKind.HeavyRain || k == WeatherKind.Storm) ? 0.97 : 1.0);
			break;
		}
		return m;
	}
}
