using System;

namespace Skyline_Sports_Project.Engine;

public static class GameClock
{
	public const int MinutesPerDay = 1440;

	private static readonly DateTime Epoch = new DateTime(2000, 1, 1);

	private static readonly string[] Months = new string[12]
	{
		"январь", "февраль", "март", "апрель", "май", "июнь", "июль", "август", "сентябрь", "октябрь",
		"ноябрь", "декабрь"
	};

	private static readonly string[] MonthsShort = new string[12]
	{
		"янв", "фев", "мар", "апр", "май", "июн", "июл", "авг", "сен", "окт",
		"ноя", "дек"
	};

	private static readonly string[] MonthsGenitive = new string[12]
	{
		"января", "февраля", "марта", "апреля", "мая", "июня", "июля", "августа", "сентября", "октября",
		"ноября", "декабря"
	};

	private static readonly string[] Weekdays = new string[7] { "воскресенье", "понедельник", "вторник", "среда", "четверг", "пятница", "суббота" };

	private static readonly string[] WeekdaysShort = new string[7] { "вс", "пн", "вт", "ср", "чт", "пт", "сб" };

	public static int DayNumber(DateTime date)
	{
		return (int)Math.Floor((date.Date - Epoch).TotalDays + 0.5);
	}

	public static DateTime DateOf(int dayNumber)
	{
		return Epoch.AddDays(dayNumber);
	}

	public static int ToTime(int dayNumber, int minuteOfDay)
	{
		return dayNumber * 1440 + minuteOfDay;
	}

	public static int ToTime(DateTime date, int minuteOfDay)
	{
		return ToTime(DayNumber(date), minuteOfDay);
	}

	public static int FromDate(int year, int month, int day, int hour, int minute)
	{
		return ToTime(new DateTime(ClampYear(year), month, day), hour * 60 + minute);
	}

	public static int FromCs2(int year, int month, int minuteOfDay)
	{
		year = ClampYear(year);
		if (month < 1)
		{
			month = 1;
		}
		if (month > 12)
		{
			month = 12;
		}
		if (minuteOfDay < 0)
		{
			minuteOfDay = 0;
		}
		if (minuteOfDay > 1439)
		{
			minuteOfDay = 1439;
		}
		return DayNumber(new DateTime(year, month, 1)) * 1440 + minuteOfDay;
	}

	public static int MonthStartDay(int now)
	{
		DateTime d = DateOf(DayOf(now));
		return DayNumber(new DateTime(d.Year, d.Month, 1));
	}

	public static int NextMonthStartDay(int now)
	{
		DateTime d = DateOf(DayOf(now));
		return DayNumber(new DateTime(d.Year, d.Month, 1).AddMonths(1));
	}

	public static int MonthIndex(int now)
	{
		DateTime d = DateOf(DayOf(now));
		return d.Year * 12 + d.Month - 1;
	}

	public static string MonthLabel(int now)
	{
		DateTime d = DateOf(DayOf(now));
		return Months[d.Month - 1] + " " + d.Year;
	}

	public static int DayOf(int time)
	{
		return FloorDiv(time, 1440);
	}

	public static int MinuteOfDay(int time)
	{
		return time - DayOf(time) * 1440;
	}

	public static DayOfWeek Weekday(int dayNumber)
	{
		return DateOf(dayNumber).DayOfWeek;
	}

	public static int YearOfDay(int dayNumber)
	{
		return DateOf(dayNumber).Year;
	}

	public static string DayLabel(int dayNumber)
	{
		DateTime d = DateOf(dayNumber);
		return Weekdays[(int)d.DayOfWeek] + ", " + d.Day + " " + MonthsGenitive[d.Month - 1] + " " + d.Year;
	}

	public static string ShortDayLabel(int dayNumber)
	{
		DateTime d = DateOf(dayNumber);
		return WeekdaysShort[(int)d.DayOfWeek] + ", " + d.Day + " " + MonthsShort[d.Month - 1];
	}

	public static string ClockLabel(int minuteOfDay)
	{
		int num = minuteOfDay / 60;
		int minute = minuteOfDay % 60;
		return num.ToString("00") + ":" + minute.ToString("00");
	}

	public static string TimeLabel(int time)
	{
		return ShortDayLabel(DayOf(time)) + ", " + ClockLabel(MinuteOfDay(time));
	}

	public static string MonthName(int month)
	{
		return Months[Math.Max(1, Math.Min(12, month)) - 1];
	}

	public static DateTime OnOrAfter(DateTime date, DayOfWeek weekday)
	{
		int diff = (weekday - date.DayOfWeek + 7) % 7;
		return date.Date.AddDays(diff);
	}

	public static DateTime OnOrBefore(DateTime date, DayOfWeek weekday)
	{
		int diff = (date.DayOfWeek - weekday + 7) % 7;
		return date.Date.AddDays(-diff);
	}

	public static DateTime Nth(int year, int month, DayOfWeek weekday, int n)
	{
		return OnOrAfter(new DateTime(ClampYear(year), month, 1), weekday).AddDays(7 * (n - 1));
	}

	public static DateTime Last(int year, int month, DayOfWeek weekday)
	{
		year = ClampYear(year);
		return OnOrBefore(new DateTime(year, month, DateTime.DaysInMonth(year, month)), weekday);
	}

	public static DateTime D(int year, int month, int day)
	{
		return new DateTime(ClampYear(year), month, day);
	}

	public static int ClampYear(int year)
	{
		if (year < 1901)
		{
			return 1901;
		}
		if (year > 5000)
		{
			return 5000;
		}
		return year;
	}

	private static int FloorDiv(int a, int b)
	{
		int q = a / b;
		if (a % b != 0 && a < 0 != b < 0)
		{
			q--;
		}
		return q;
	}
}
