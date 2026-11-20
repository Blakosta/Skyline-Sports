using System.Collections.Generic;

namespace Skyline_Sports_Project.Engine;

public static class SportRules
{
	public static readonly Sport[] AllSports = new Sport[5]
	{
		Sport.Hockey,
		Sport.Basketball,
		Sport.Football,
		Sport.Baseball,
		Sport.Soccer
	};

	public static readonly Level[] AllLevels = new Level[3]
	{
		Level.HighSchool,
		Level.College,
		Level.Pro
	};

	public static readonly Level[] VenueLevels = new Level[5]
	{
		Level.HighSchool,
		Level.Junior,
		Level.College,
		Level.Minor,
		Level.Pro
	};

	public static readonly VenueType[] AllVenueTypes = new VenueType[6]
	{
		VenueType.IndoorArena,
		VenueType.Gym,
		VenueType.IceRink,
		VenueType.Stadium,
		VenueType.Ballpark,
		VenueType.Field
	};

	public const int MaxProTeamsPerSportAtVenue = 2;

	public static string[] MonthNames = new string[12]
	{
		"Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь",
		"Ноябрь", "Декабрь"
	};

	public static string[] MonthShort = new string[12]
	{
		"янв", "фев", "мар", "апр", "май", "июн", "июл", "авг", "сен", "окт",
		"ноя", "дек"
	};

	public static bool LevelAllowed(Sport sport, Level level)
	{
		return level switch
		{
			Level.Junior => sport == Sport.Hockey, 
			Level.Minor => HasMinors(sport), 
			_ => true, 
		};
	}

	public static bool ProLike(Sport sport, Level level)
	{
		return level switch
		{
			Level.Minor => sport == Sport.Soccer, 
			Level.Pro => true, 
			_ => false, 
		};
	}

	public static bool IsSecondDivision(Sport sport, Level level)
	{
		if (level == Level.Minor)
		{
			return sport == Sport.Soccer;
		}
		return false;
	}

	public static bool HasMinors(Sport sport)
	{
		if (sport != Sport.Hockey && sport != Sport.Basketball)
		{
			return sport == Sport.Baseball;
		}
		return true;
	}

	public static Level Scale(Level level)
	{
		return level switch
		{
			Level.Junior => Level.College, 
			Level.Minor => Level.Pro, 
			_ => level, 
		};
	}

	public static int Rank(Level level)
	{
		return level switch
		{
			Level.Pro => 4, 
			Level.Minor => 3, 
			Level.College => 2, 
			Level.Junior => 1, 
			_ => 0, 
		};
	}

	public static bool IsSchool(Level level)
	{
		if (level != Level.HighSchool && level != Level.College)
		{
			return level == Level.Junior;
		}
		return true;
	}

	public static int HomeDateCapacity(VenueType type)
	{
		return type switch
		{
			VenueType.IndoorArena => 180, 
			VenueType.Gym => 60, 
			VenueType.IceRink => 100, 
			VenueType.Stadium => 50, 
			VenueType.Ballpark => 110, 
			_ => 75, 
		};
	}

	public static int MaxTeams(VenueType type)
	{
		return type switch
		{
			VenueType.IndoorArena => 5, 
			VenueType.Gym => 4, 
			VenueType.IceRink => 5, 
			VenueType.Stadium => 4, 
			VenueType.Ballpark => 3, 
			_ => 5, 
		};
	}

	public static Sport[] AllowedSports(VenueType type)
	{
		return type switch
		{
			VenueType.IndoorArena => new Sport[4]
			{
				Sport.Hockey,
				Sport.Basketball,
				Sport.Football,
				Sport.Soccer
			}, 
			VenueType.Gym => new Sport[1] { Sport.Basketball }, 
			VenueType.IceRink => new Sport[1], 
			VenueType.Stadium => new Sport[2]
			{
				Sport.Football,
				Sport.Soccer
			}, 
			VenueType.Ballpark => new Sport[1] { Sport.Baseball }, 
			VenueType.Field => new Sport[3]
			{
				Sport.Football,
				Sport.Soccer,
				Sport.Baseball
			}, 
			_ => new Sport[0], 
		};
	}

	public static bool IsAllowed(VenueType type, Sport sport)
	{
		Sport[] array = AllowedSports(type);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == sport)
			{
				return true;
			}
		}
		return false;
	}

	public static Level DefaultLevel(VenueType type)
	{
		switch (type)
		{
		case VenueType.Gym:
		case VenueType.Field:
			return Level.HighSchool;
		case VenueType.IceRink:
			return Level.College;
		default:
			return Level.Pro;
		}
	}

	public static bool AllowsTies(Sport sport)
	{
		return sport == Sport.Soccer;
	}

	public static string SportLabel(Sport sport)
	{
		return sport switch
		{
			Sport.Hockey => "Хоккей", 
			Sport.Basketball => "Баскетбол", 
			Sport.Football => "Американский футбол", 
			Sport.Baseball => "Бейсбол", 
			Sport.Soccer => "Футбол", 
			_ => sport.ToString(), 
		};
	}

	public static string LevelLabel(Sport sport, Level level)
	{
		if (!IsSecondDivision(sport, level))
		{
			return LevelLabel(level);
		}
		return "Вторая лига";
	}

	public static string LevelLabel(Level level)
	{
		return level switch
		{
			Level.HighSchool => "Школа", 
			Level.College => "Студенческая лига", 
			Level.Pro => "Профессиональная лига", 
			Level.Junior => "Юниорская лига", 
			Level.Minor => "Малая лига", 
			_ => level.ToString(), 
		};
	}

	public static string VenueTypeLabel(VenueType type)
	{
		return type switch
		{
			VenueType.IndoorArena => "Крытая арена", 
			VenueType.Gym => "Спортзал", 
			VenueType.IceRink => "Ледовая арена", 
			VenueType.Stadium => "Стадион", 
			VenueType.Ballpark => "Бейсбольное поле", 
			VenueType.Field => "Поле", 
			_ => type.ToString(), 
		};
	}

	public static string ResultSuffix(Sport sport, ResultType result)
	{
		return result switch
		{
			ResultType.Overtime => "ОТ", 
			ResultType.Shootout => "Б", 
			ResultType.ExtraInnings => "ДИ", 
			ResultType.Penalties => "П", 
			_ => "", 
		};
	}

	public static IEnumerable<Sport> SportsOrder()
	{
		return AllSports;
	}
}
