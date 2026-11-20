using System;
using System.Collections.Generic;
using System.Text;

namespace Skyline_Sports_Project.Engine;

public class NameGenerator
{
	private readonly Random _random;

	public static readonly string[] Mascots = new string[80]
	{
		"Медведи", "Волки", "Орлы", "Ястребы", "Соколы", "Тигры", "Львы", "Рыси", "Барсы", "Беркуты",
		"Витязи", "Богатыри", "Атланты", "Ураганы", "Молнии", "Шторм", "Тайфун", "Буран", "Север", "Полярники",
		"Спартак", "Динамо", "Зенит", "Локомотив", "Торпедо", "Авангард", "Арсенал", "Рубин", "Салют", "Факел",
		"Искра", "Звезда", "Метеор", "Ракета", "Сатурн", "Вымпел", "Олимп", "Стрела", "Кристалл", "Алмаз",
		"Гранит", "Металлург", "Нефтяник", "Шахтёр", "Моряк", "Десант", "Патриот", "Легион", "Дружина", "Дозор",
		"Ратник", "Грифоны", "Драконы", "Фениксы", "Скорпионы", "Кобры", "Акулы", "Киты", "Бобры", "Лоси",
		"Олени", "Ласточки", "Чайки", "Буревестники", "Пеликаны", "Стерхи", "Ястребы", "Снегири", "Совы", "Вороны",
		"Вихрь", "Пламя", "Сила", "Легенды", "Чемпионы", "Мастера", "Победа", "Рекорд", "Медведи", "Волки"
	};

	public static readonly string[] HockeyMascots = new string[10] {
		"Буран", "Авангард", "Полярники", "Ледоколы", "Метель", "Белые медведи", "Север", "Снежные барсы", "Снегири", "Тайфун"
	};

	public static readonly string[] BaseballMascots = new string[10] {
		"Удар", "Тайфун", "Речники", "Молнии", "Летучие", "Алмазы", "Искры", "Моряки", "Беркуты", "Буревестники"
	};

	public static readonly string[] SoccerProNames = new string[10] {
		"Спартак", "Динамо", "Зенит", "Локомотив", "Торпедо", "Арсенал", "Рубин", "Урал", "Сибирь", "Спартак"
	};

	public static readonly string[] Places = new string[60]
	{
		"Москва", "Санкт-Петербург", "Новосибирск", "Екатеринбург", "Казань", "Нижний Новгород", "Челябинск", "Самара", "Омск", "Ростов-на-Дону",
		"Уфа", "Красноярск", "Пермь", "Воронеж", "Волгоград", "Краснодар", "Саратов", "Тюмень", "Тольятти", "Ижевск",
		"Барнаул", "Иркутск", "Хабаровск", "Ярославль", "Владивосток", "Махачкала", "Томск", "Оренбург", "Кемерово", "Новокузнецк",
		"Рязань", "Астрахань", "Пенза", "Липецк", "Киров", "Чебоксары", "Калининград", "Тула", "Курск", "Ставрополь",
		"Сочи", "Белгород", "Брянск", "Владимир", "Архангельск", "Мурманск", "Петрозаводск", "Смоленск", "Тверь", "Иваново",
		"Кострома", "Тамбов", "Сургут", "Нижневартовск", "Норильск", "Якутск", "Улан-Удэ", "Чита", "Благовещенск", "Южно-Сахалинск"
	};

	public static readonly string[] HighSchoolNames = new string[40]
	{
		"Центральная школа", "Северная школа", "Южная школа", "Восточная школа", "Западная школа", "Городская школа", "Спортивная школа", "Олимпийская школа", "Школа № 1", "Школа № 2",
		"Школа № 3", "Школа № 4", "Школа № 5", "Школа № 6", "Школа № 7", "Школа № 8", "Школа № 9", "Школа № 10", "Лицей № 1", "Гимназия № 1",
		"Смена", "Олимп", "Победа", "Старт", "Рекорд", "Юность", "Заря", "Вымпел", "Факел", "Метеор",
		"Сокол", "Орбита", "Горизонт", "Спарта", "Дружба", "Надежда", "Созвездие", "Перспектива", "Эверест", "Чемпион"
	};

	private static readonly string[] HighSchoolSuffixes = new string[5] {
		"Север", "Юг", "Восток", "Запад", "Центр"
	};

	public NameGenerator(Random random)
	{
		_random = random;
	}

	public string PickMascot(Sport sport, Level level, ICollection<string> taken, string place = null)
	{
		List<string> pool = new List<string>();
		if (level == Level.Pro && sport == Sport.Soccer)
		{
			pool.AddRange(SoccerProNames);
			if (!string.IsNullOrEmpty(place))
			{
				pool.RemoveAll((string x) => place.EndsWith(" " + x, StringComparison.OrdinalIgnoreCase) || place.Equals(x, StringComparison.OrdinalIgnoreCase));
			}
		}
		else
		{
			pool.AddRange(Mascots);
			if (sport == Sport.Hockey)
			{
				pool.AddRange(HockeyMascots);
			}
			if (sport == Sport.Baseball)
			{
				pool.AddRange(BaseballMascots);
			}
		}
		return PickUnique(pool, taken);
	}

	public string PickSchoolMascot(ICollection<string> taken)
	{
		return PickUnique(new List<string>(Mascots), taken);
	}

	public string PickPlace(ICollection<string> taken)
	{
		return PickUnique(new List<string>(Places), taken);
	}

	public string HighSchoolName(string districtName, ICollection<string> takenNames)
	{
		string baseName = CleanPlace(districtName);
		string[] highSchoolSuffixes;
		if (baseName.Length > 0)
		{
			string candidate = baseName + " школа";
			if (!Contains(takenNames, candidate))
			{
				return candidate;
			}
			highSchoolSuffixes = HighSchoolSuffixes;
			foreach (string suffix in highSchoolSuffixes)
			{
				candidate = baseName + " " + suffix + " школа";
				if (!Contains(takenNames, candidate))
				{
					return candidate;
				}
			}
		}
		List<string> options = new List<string>();
		highSchoolSuffixes = HighSchoolNames;
		for (int i = 0; i < highSchoolSuffixes.Length; i++)
		{
			string candidate2 = highSchoolSuffixes[i] + " школа";
			if (!Contains(takenNames, candidate2))
			{
				options.Add(candidate2);
			}
		}
		if (options.Count > 0)
		{
			return options[_random.Next(options.Count)];
		}
		return HighSchoolNames[_random.Next(HighSchoolNames.Length)] + " №" + _random.Next(2, 99);
	}

	public string CollegeName(string place, ICollection<string> takenNames)
	{
		string p = CleanPlace(place);
		if (p.Length == 0)
		{
			p = Places[_random.Next(Places.Length)];
		}
		List<string> styles = new List<string>
		{
			"Университет " + p,
			p + " Государственный университет",
			p + " Институт",
			p + " Академия"
		};
		Shuffle(styles);
		foreach (string s in styles)
		{
			if (!Contains(takenNames, s))
			{
				return s;
			}
		}
		return styles[0] + " " + HighSchoolSuffixes[_random.Next(HighSchoolSuffixes.Length)];
	}

	public static string Abbreviation(string name)
	{
		string[] skip = new string[17] { "high", "university", "of", "state", "college", "tech", "the", "fc", "sc", "школа", "университет", "государственный", "институт", "академия", "лицей", "гимназия", "имени" };
		List<string> words = new List<string>();
		string[] array = (name ?? "").Split(new[] { ' ' }, StringSplitOptions.None);
		for (int i = 0; i < array.Length; i++)
		{
			string w = array[i].Trim();
			if (w.Length == 0)
			{
				continue;
			}
			bool skipped = false;
			string[] array2 = skip;
			foreach (string s in array2)
			{
				if (string.Equals(w, s, StringComparison.OrdinalIgnoreCase))
				{
					skipped = true;
					break;
				}
			}
			if (!skipped)
			{
				words.Add(w);
			}
		}
		if (words.Count == 0)
		{
			words.Add(string.IsNullOrEmpty(name) ? "Н/Д" : name.Trim());
		}
		StringBuilder sb = new StringBuilder();
		if (words.Count == 1)
		{
			string text = words[0];
			foreach (char c in text)
			{
				if (char.IsLetterOrDigit(c))
				{
					sb.Append(c);
				}
				if (sb.Length == 3)
				{
					break;
				}
			}
		}
		else
		{
			for (int k = 0; k < words.Count; k++)
			{
				if (sb.Length >= 3)
				{
					break;
				}
				char c2 = FirstLetter(words[k]);
				if (c2 != '\0')
				{
					sb.Append(c2);
				}
			}
			string last = words[words.Count - 1];
			for (int l = 1; l < last.Length; l++)
			{
				if (sb.Length >= 3)
				{
					break;
				}
				if (char.IsLetterOrDigit(last[l]))
				{
					sb.Append(last[l]);
				}
			}
		}
		if (sb.Length == 0)
		{
			sb.Append("Н/Д");
		}
		return sb.ToString().ToUpperInvariant();
	}

	public static string CleanName(string value, int maxLength)
	{
		if (value == null)
		{
			return "";
		}
		StringBuilder sb = new StringBuilder();
		foreach (char c in value)
		{
			if (!char.IsControl(c))
			{
				sb.Append(c);
			}
		}
		string result = sb.ToString().Trim();
		while (result.Contains("  "))
		{
			result = result.Replace("  ", " ");
		}
		if (result.Length > maxLength)
		{
			result = result.Substring(0, maxLength).Trim();
		}
		return result;
	}

	private static string CleanPlace(string place)
	{
		string p = CleanName(place, 32);
		if (p.StartsWith("Район", StringComparison.OrdinalIgnoreCase))
		{
			return "";
		}
		return p;
	}

	private static char FirstLetter(string word)
	{
		foreach (char c in word)
		{
			if (char.IsLetterOrDigit(c))
			{
				return c;
			}
		}
		return '\0';
	}

	private string PickUnique(List<string> pool, ICollection<string> taken)
	{
		List<string> free = new List<string>();
		foreach (string s in pool)
		{
			if (!Contains(taken, s))
			{
				free.Add(s);
			}
		}
		if (free.Count > 0)
		{
			return free[_random.Next(free.Count)];
		}
		return pool[_random.Next(pool.Count)];
	}

	private static bool Contains(ICollection<string> set, string value)
	{
		if (set == null)
		{
			return false;
		}
		foreach (string item in set)
		{
			if (string.Equals(item, value, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	private void Shuffle<T>(List<T> list)
	{
		for (int i = list.Count - 1; i > 0; i--)
		{
			int j = _random.Next(i + 1);
			T tmp = list[i];
			list[i] = list[j];
			list[j] = tmp;
		}
	}
}
