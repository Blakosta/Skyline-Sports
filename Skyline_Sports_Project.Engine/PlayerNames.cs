using System;
using System.Collections.Generic;

namespace Skyline_Sports_Project.Engine;

public static class PlayerNames
{
	public const string American = "us";

	private static readonly string[] FirstUS = new string[]
	{
		"Александр",
		"Алексей",
		"Андрей",
		"Антон",
		"Артём",
		"Борис",
		"Вадим",
		"Валерий",
		"Василий",
		"Виктор",
		"Виталий",
		"Владимир",
		"Владислав",
		"Георгий",
		"Глеб",
		"Даниил",
		"Денис",
		"Дмитрий",
		"Евгений",
		"Егор",
		"Иван",
		"Игнат",
		"Игорь",
		"Илья",
		"Кирилл",
		"Константин",
		"Лев",
		"Макар",
		"Максим",
		"Марк",
		"Матвей",
		"Михаил",
		"Назар",
		"Никита",
		"Николай",
		"Олег",
		"Павел",
		"Пётр",
		"Рамиль",
		"Ратмир",
		"Ринат",
		"Роман",
		"Руслан",
		"Савелий",
		"Самир",
		"Семён",
		"Сергей",
		"Станислав",
		"Степан",
		"Тимофей",
		"Тимур",
		"Фёдор",
		"Эмиль",
		"Юрий",
		"Ярослав",
		"Азамат",
		"Арсен",
		"Батыр",
		"Давид",
		"Заур",
		"Марат",
		"Рашид",
		"Тамерлан",
		"Эльдар",
		"Рустам",
		"Тагир",
		"Аслан",
		"Богдан",
		"Данила",
		"Мирослав",
		"Родион",
		"Святослав",
		"Тихон"
	};	private static readonly string[] LastUS = new string[]
	{
		"Акинфеев",
		"Головин",
		"Миранчук",
		"Захарян",
		"Сафонов",
		"Тюкавин",
		"Чалов",
		"Глушенков",
		"Баринов",
		"Дивеев",
		"Джикия",
		"Соболев",
		"Мостовой",
		"Обляков",
		"Круговой",
		"Осипенко",
		"Карпукас",
		"Пиняев",
		"Сперцян",
		"Кварацхелия",
		"Азмун",
		"Малком",
		"Вендел",
		"Клаудиньо",
		"Тикнизян",
		"Митай",
		"Кузяев",
		"Оздоев",
		"Черышев",
		"Смолов",
		"Кокорин",
		"Дзюба",
		"Аршавин",
		"Кержаков",
		"Павлюченко",
		"Жирков",
		"Игнашевич",
		"Березуцкий",
		"Титов",
		"Аленичев",
		"Карпин",
		"Онопко",
		"Семак",
		"Сычёв",
		"Измайлов",
		"Билялетдинов",
		"Денисов",
		"Широков",
		"Абаев",
		"Абрамов",
		"Агапов",
		"Адамов",
		"Акимов",
		"Аксенов",
		"Александров",
		"Алдонин",
		"Андреев",
		"Антонов",
		"Артёмов",
		"Астафьев",
		"Ахметов",
		"Бабурин",
		"Баженов",
		"Бакаев",
		"Балакшин",
		"Баранов",
		"Батраков",
		"Беляев",
		"Беляков",
		"Березин",
		"Бессмертный",
		"Бикфалви",
		"Бирюков",
		"Бобров",
		"Богданов",
		"Болдырев",
		"Бондаренко",
		"Борисов",
		"Бурлак",
		"Быков",
		"Васильев",
		"Волков",
		"Воробьёв",
		"Гаврилов",
		"Гайич",
		"Гапонов",
		"Глушаков",
		"Голубев",
		"Горшков",
		"Гришин",
		"Гудиев",
		"Гуренко",
		"Данилин",
		"Дмитриев",
		"Дорский",
		"Дроздов",
		"Евсеев",
		"Егоров",
		"Елисеев",
		"Емельянов",
		"Ерохин",
		"Жемалетдинов",
		"Жигулёв",
		"Жиров",
		"Жуков",
		"Заболотный",
		"Зайдензаль",
		"Зайцев",
		"Захаров",
		"Зиньковский",
		"Зобнин",
		"Зуев",
		"Иванов",
		"Игнатьев",
		"Ионов",
		"Исаев",
		"Кабутов",
		"Калинин",
		"Каменщиков",
		"Караваев",
		"Карпов",
		"Каштанов",
		"Кисляк",
		"Коваленко",
		"Козлов",
		"Колесниченко",
		"Комличенко",
		"Коновалов",
		"Кордоба",
		"Корян",
		"Костюков",
		"Кравцов",
		"Красильниченко",
		"Кривцов",
		"Кузнецов",
		"Куликов",
		"Лантратов",
		"Лесовой",
		"Лисакович",
		"Ломаев",
		"Лукин",
		"Макаров",
		"Максименко",
		"Мампасси",
		"Мантуан",
		"Мелёхин",
		"Мельников",
		"Миронов",
		"Мухин",
		"Набиуллин",
		"Николаев",
		"Никитин",
		"Новиков",
		"Обухов",
		"Олейников",
		"Орлов",
		"Павлов",
		"Панченко",
		"Песьяков",
		"Петров",
		"Печенин",
		"Писарский",
		"Плотников",
		"Подберёзкин",
		"Прокин",
		"Прохин",
		"Раков",
		"Рассказов",
		"Репях",
		"Родригао",
		"Рыбус",
		"Рыбчинский",
		"Савельев",
		"Садулаев",
		"Самошников",
		"Сергеев",
		"Сильянов",
		"Скопинцев",
		"Смирнов",
		"Соколов",
		"Солдатенков",
		"Сутормин",
		"Тарасов",
		"Терехов",
		"Тихий",
		"Ткачук",
		"Трошечкин",
		"Умяров",
		"Файзуллаев",
		"Фернандес",
		"Фомин",
		"Хлусевич",
		"Ходжа",
		"Цакташ",
		"Чистяков",
		"Шамкин",
		"Шапи",
		"Шитов",
		"Шуманский",
		"Щетинин",
		"Эктов",
		"Юсупов",
		"Яковлев",
		"Языджи",
		"Яранцев",
		"Яшин",
		"Абдуллин",
		"Абросимов",
		"Аверьянов",
		"Агафонов",
		"Агкацев",
		"Алексич",
		"Анисимов",
		"Арутюнян",
		"Бабкин",
		"Байрамян",
		"Барач",
		"Батырев",
		"Бегич",
		"Беликов",
		"Бессонов",
		"Бикмаев",
		"Болотов",
		"Бородин",
		"Брагин",
		"Бурмистров",
		"Васютин",
		"Веретенников",
		"Власов",
		"Галкин",
		"Гапич",
		"Герасимов",
		"Горбунов",
		"Гордеев",
		"Горелов",
		"Григорьев",
		"Давыдов",
		"Данилов",
		"Деменко",
		"Демидов",
		"Демченко",
		"Дьяков",
		"Ефимов",
		"Жданов",
		"Жестоков",
		"Журавлёв",
		"Зотов",
		"Зубарев",
		"Иванченко",
		"Калмыков",
		"Капленко",
		"Карасёв",
		"Касинтура",
		"Кириллов",
		"Киселёв",
		"Князев",
		"Козловский",
		"Коломейцев",
		"Корниленко",
		"Котов",
		"Коченков",
		"Кочетов",
		"Кравчук",
		"Криворучко",
		"Кузьмин",
		"Курочкин",
		"Лазарев",
		"Лапин",
		"Лобанов",
		"Логашов",
		"Лунев",
		"Малых",
		"Марков",
		"Маслов",
		"Матвеев",
		"Медведев",
		"Мещеряков",
		"Михайлов",
		"Морозов",
		"Мустафин",
		"Назаров",
		"Нечаев",
		"Овсянников",
		"Панков",
		"Панфилов",
		"Поляков",
		"Пономарёв",
		"Попов",
		"Портнягин",
		"Радченко",
		"Рахимов",
		"Рогов",
		"Рыжиков",
		"Савин",
		"Сазонов",
		"Селихов",
		"Семёнов",
		"Сенников",
		"Сидоров",
		"Симонян",
		"Собкин",
		"Соловьёв",
		"Сорокин",
		"Стариков",
		"Степанов",
		"Сухарев",
		"Терентьев",
		"Тихонов",
		"Ткачёв",
		"Третьяков",
		"Туманов",
		"Федотов",
		"Филиппов",
		"Харин",
		"Хвичия",
		"Хохлов",
		"Цаллагов",
		"Чернов",
		"Шаронов",
		"Шевченко",
		"Шишкин",
		"Щербаков",
		"Юдин",
		"Якушев"
	};	private static readonly string[] Heritage = new string[24]
	{
		"Garcia", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Perez", "Sanchez", "Ramirez", "Torres",
		"Rivera", "Flores", "Russo", "Rossi", "Esposito", "Romano", "Kim", "Park", "Nguyen", "Tran",
		"Patel", "Jensen", "Lindgren", "Dubois"
	};

	private static readonly NameOrigin[] Origins = new NameOrigin[20]
	{
		new NameOrigin("ru", new string[14]
		{
			"Alexei", "Dmitri", "Nikita", "Ivan", "Sergei", "Artem", "Mikhail", "Andrei", "Yegor", "Kirill",
			"Vladislav", "Pavel", "Maxim", "Ilya"
		}, new string[24]
		{
			"Ivanov", "Petrov", "Volkov", "Sokolov", "Kuznetsov", "Popov", "Morozov", "Orlov", "Zaitsev", "Belov",
			"Smirnov", "Lebedev", "Kozlov", "Novikov", "Pavlov", "Semenov", "Golubev", "Vinogradov", "Vorobyov", "Frolov",
			"Zakharov", "Tarasov", "Belyaev", "Komarov"
		}),
		new NameOrigin("ua", new string[10] { "Bohdan", "Taras", "Oleksandr", "Mykola", "Viktor", "Andriy", "Yurii", "Dmytro", "Vasyl", "Ostap" }, new string[16]
		{
			"Shevchenko", "Kovalenko", "Bondarenko", "Tkachenko", "Melnyk", "Kravets", "Babiy", "Boyko", "Kovalchuk", "Lysenko",
			"Marchenko", "Rudenko", "Savchenko", "Petrenko", "Moroz", "Hrytsenko"
		}),
		new NameOrigin("cz", new string[12]
		{
			"Jakub", "Tomas", "Ondrej", "Filip", "Lukas", "Matej", "Radek", "Vojtech", "Jiri", "Marek",
			"Libor", "Dominik"
		}, new string[16]
		{
			"Novotny", "Dvorak", "Svoboda", "Horak", "Hanzal", "Cerny", "Prochazka", "Kucera", "Vesely", "Pospisil",
			"Nemec", "Zeman", "Kral", "Benes", "Sykora", "Holik"
		}),
		new NameOrigin("se", new string[15]
		{
			"Erik", "Lars", "Anders", "Mikael", "Oskar", "Elias", "Rasmus", "Henrik", "Gustav", "Johan",
			"Nils", "Emil", "Axel", "Linus", "Isak"
		}, new string[20]
		{
			"Lindqvist", "Johansson", "Karlsson", "Nilsson", "Lindholm", "Backstrom", "Forsberg", "Nyquist", "Andersson", "Eriksson",
			"Larsson", "Persson", "Svensson", "Gustafsson", "Lindberg", "Sandstrom", "Bergqvist", "Holmqvist", "Wallin", "Engstrom"
		}),
		new NameOrigin("fi", new string[12]
		{
			"Juuso", "Aleksi", "Patrik", "Mikko", "Sami", "Jussi", "Ville", "Eetu", "Kasperi", "Joonas",
			"Otto", "Valtteri"
		}, new string[16]
		{
			"Laine", "Koivu", "Virtanen", "Makinen", "Aho", "Ranta", "Korhonen", "Nieminen", "Heikkinen", "Lehtonen",
			"Salonen", "Hamalainen", "Laaksonen", "Jokinen", "Kallio", "Lehtinen"
		}),
		new NameOrigin("fr", new string[14]
		{
			"Mathieu", "Antoine", "Gabriel", "Felix", "Samuel", "Pierre", "Olivier", "Etienne", "Maxime", "Jeremie",
			"Alexis", "Guillaume", "Theo", "Hugo"
		}, new string[18]
		{
			"Tremblay", "Gagnon", "Roy", "Bouchard", "Gauthier", "Lavoie", "Fortin", "Pelletier", "Cote", "Belanger",
			"Lefebvre", "Girard", "Leblanc", "Morin", "Fournier", "Theriault", "Lambert", "Mercier"
		}),
		new NameOrigin("de", new string[10] { "Moritz", "Tobias", "Jonas", "Leon", "Niklas", "Florian", "Lennart", "Fabian", "Jannik", "Maximilian" }, new string[16]
		{
			"Muller", "Wagner", "Hoffmann", "Schulz", "Koch", "Richter", "Wolf", "Krause", "Neumann", "Schwarz",
			"Zimmermann", "Hartmann", "Lange", "Brandt", "Vogel", "Seidel"
		}),
		new NameOrigin("it", new string[10] { "Luca", "Marco", "Alessandro", "Lorenzo", "Matteo", "Giovanni", "Francesco", "Davide", "Simone", "Federico" }, new string[16]
		{
			"Rossi", "Russo", "Ferrari", "Esposito", "Bianchi", "Romano", "Colombo", "Ricci", "Marino", "Greco",
			"Gallo", "Conti", "De Luca", "Mancini", "Costa", "Rinaldi"
		}),
		new NameOrigin("es", new string[22]
		{
			"Mateo", "Diego", "Carlos", "Javier", "Luis", "Santiago", "Alejandro", "Miguel", "Jose", "Juan",
			"Pablo", "Andres", "Fernando", "Ricardo", "Eduardo", "Sergio", "Raul", "Emilio", "Yordan", "Wander",
			"Julio", "Ramon"
		}, new string[30]
		{
			"Garcia", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Perez", "Sanchez", "Ramirez", "Torres",
			"Flores", "Rivera", "Gomez", "Diaz", "Morales", "Ortiz", "Castillo", "Vargas", "Reyes", "Mendoza",
			"Cruz", "Herrera", "Medina", "Jimenez", "Ruiz", "Alvarez", "Guerrero", "Soto", "Peralta", "Acuna"
		}),
		new NameOrigin("br", new string[13]
		{
			"Thiago", "Rafael", "Bruno", "Joao", "Gustavo", "Felipe", "Rodrigo", "Vinicius", "Caio", "Matheus",
			"Tiago", "Diogo", "Goncalo"
		}, new string[17]
		{
			"Silva", "Santos", "Oliveira", "Souza", "Pereira", "Almeida", "Ferreira", "Rodrigues", "Carvalho", "Lima",
			"Ribeiro", "Barbosa", "Rocha", "Gomes", "Martins", "Araujo", "Costa"
		}),
		new NameOrigin("rs", new string[10] { "Nikola", "Bogdan", "Luka", "Milos", "Marko", "Stefan", "Nemanja", "Vasilije", "Dusan", "Ognjen" }, new string[14]
		{
			"Jovanovic", "Petrovic", "Nikolic", "Markovic", "Djordjevic", "Stojanovic", "Ilic", "Pavlovic", "Popovic", "Savic",
			"Radovic", "Kovacevic", "Lukic", "Vukovic"
		}),
		new NameOrigin("gb", new string[10] { "Callum", "Declan", "Harry", "Alfie", "Kieran", "Rhys", "Archie", "Jamie", "Reece", "Ollie" }, new string[14]
		{
			"Clarke", "Hughes", "Walsh", "Fraser", "McLeod", "Docherty", "Kennedy", "Harrison", "Lloyd", "Davies",
			"Pritchard", "Barker", "Whitfield", "Fletcher"
		}),
		new NameOrigin("au", new string[10] { "Lachlan", "Hamish", "Angus", "Mitchell", "Brodie", "Jarrod", "Riley", "Fletcher", "Bailey", "Darcy" }, new string[12]
		{
			"Murray", "Fraser", "McLeod", "Harrison", "Docherty", "Kennedy", "Wright", "Lawson", "Ingram", "Sharpe",
			"Thorpe", "Whitlock"
		}),
		new NameOrigin("jp", new string[13]
		{
			"Kenji", "Hiroshi", "Takumi", "Haruto", "Yuki", "Ryota", "Daiki", "Kaito", "Sho", "Yuto",
			"Shota", "Kazuki", "Ren"
		}, new string[16]
		{
			"Tanaka", "Suzuki", "Sato", "Takahashi", "Watanabe", "Ito", "Yamamoto", "Nakamura", "Kobayashi", "Kato",
			"Yoshida", "Yamada", "Matsumoto", "Inoue", "Kimura", "Hayashi"
		}),
		new NameOrigin("kr", new string[9] { "Min-jun", "Ji-ho", "Seo-jun", "Hyun-woo", "Jae-won", "Dong-hyun", "Sung-min", "Ji-hoon", "Tae-yang" }, new string[12]
		{
			"Kim", "Park", "Lee", "Choi", "Jung", "Kang", "Cho", "Yoon", "Jang", "Lim",
			"Han", "Shin"
		}),
		new NameOrigin("gh", new string[7] { "Kofi", "Kwame", "Kwabena", "Yaw", "Kojo", "Kwaku", "Kwesi" }, new string[10] { "Mensah", "Owusu", "Boateng", "Asante", "Appiah", "Danso", "Osei", "Agyeman", "Ofori", "Amoah" }),
		new NameOrigin("ng", new string[10] { "Emeka", "Chidi", "Obinna", "Tunde", "Femi", "Ikenna", "Chinedu", "Segun", "Uche", "Tobi" }, new string[12]
		{
			"Okafor", "Adeyemi", "Okonkwo", "Eze", "Nwosu", "Balogun", "Adebayo", "Obi", "Okeke", "Oladipo",
			"Nnamdi", "Afolabi"
		}),
		new NameOrigin("sn", new string[9] { "Moussa", "Ibrahim", "Mamadou", "Cheikh", "Abdoulaye", "Ousmane", "Seydou", "Lamine", "Sekou" }, new string[12]
		{
			"Diallo", "Traore", "Diop", "Ndiaye", "Sow", "Camara", "Kone", "Cisse", "Toure", "Sarr",
			"Fall", "Coulibaly"
		}),
		new NameOrigin("ma", new string[9] { "Youssef", "Omar", "Karim", "Hakim", "Mehdi", "Amine", "Bilal", "Hamza", "Ayoub" }, new string[12]
		{
			"Haddad", "Mansour", "El Amrani", "Benali", "Bennani", "Saidi", "Belkacem", "Hassan", "Khalil", "Ziani",
			"Chaouki", "Tazi"
		}),
		new NameOrigin("ps", new string[10] { "Malakai", "Sione", "Tevita", "Keanu", "Manu", "Kalani", "Viliami", "Isileli", "Pita", "Lealofi" }, new string[14]
		{
			"Fuimaono", "Faleolo", "Mauga", "Tafuna", "Sapolu", "Leota", "Aumua", "Fonoti", "Tuitama", "Vea",
			"Taufa", "Moala", "Tuilagi", "Fifita"
		})
	};

	private static readonly Dictionary<string, NameOrigin> ByCode = BuildCodes();

	private static readonly Dictionary<string, string> FirstOrigin = BuildFirstNames();

	private static readonly HashSet<string> UsLast = new HashSet<string>(LastUS);

	private static readonly HashSet<string> HeritageSet = new HashSet<string>(Heritage);

	private static readonly Dictionary<Sport, KeyValuePair<string, int>[]> Weights = new Dictionary<Sport, KeyValuePair<string, int>[]>
	{
		{
			Sport.Hockey,
			W("fr", 30, "se", 20, "fi", 13, "ru", 13, "cz", 11, "de", 5, "ua", 3)
		},
		{
			Sport.Basketball,
			W("rs", 14, "fr", 12, "ng", 12, "es", 10, "sn", 8, "de", 8, "au", 8, "it", 5, "br", 5, "gh", 4, "ua", 3, "ru", 3, "jp", 3, "gb", 2, "ma", 2, "se", 2, "fi", 2)
		},
		{
			Sport.Football,
			W("ps", 45, "ng", 20, "au", 15, "de", 10, "gh", 5, "gb", 5)
		},
		{
			Sport.Baseball,
			W("es", 75, "jp", 13, "kr", 9, "au", 3)
		},
		{
			Sport.Soccer,
			W("es", 26, "br", 16, "fr", 9, "gb", 8, "de", 7, "it", 6, "sn", 6, "gh", 5, "ng", 5, "ma", 5, "jp", 4, "kr", 4, "se", 3, "rs", 3, "au", 2, "ua", 1, "ru", 1, "cz", 1)
		}
	};

	private static Dictionary<string, NameOrigin> BuildCodes()
	{
		Dictionary<string, NameOrigin> d = new Dictionary<string, NameOrigin>();
		NameOrigin[] origins = Origins;
		foreach (NameOrigin o in origins)
		{
			d[o.Code] = o;
		}
		return d;
	}

	private static Dictionary<string, string> BuildFirstNames()
	{
		Dictionary<string, string> d = new Dictionary<string, string>();
		string[] firstUS = FirstUS;
		foreach (string n in firstUS)
		{
			d[n] = "us";
		}
		NameOrigin[] origins = Origins;
		foreach (NameOrigin o in origins)
		{
			firstUS = o.First;
			foreach (string n2 in firstUS)
			{
				d[n2] = o.Code;
			}
		}
		return d;
	}

	public static string OriginOf(string first)
	{
		if (first == null || !FirstOrigin.TryGetValue(first, out var code))
		{
			return "us";
		}
		return code;
	}

	public static bool Fits(string first, string last)
	{
		string code = OriginOf(first);
		if (code == "us")
		{
			if (!UsLast.Contains(last ?? ""))
			{
				return HeritageSet.Contains(last ?? "");
			}
			return true;
		}
		return Array.IndexOf(ByCode[code].Last, last) >= 0;
	}

	public static string LastFor(string first, Random random)
	{
		string code = OriginOf(first);
		if (code == "us")
		{
			if (!(random.NextDouble() < 0.08))
			{
				return LastUS[random.Next(LastUS.Length)];
			}
			return Heritage[random.Next(Heritage.Length)];
		}
		NameOrigin o = ByCode[code];
		return o.Last[random.Next(o.Last.Length)];
	}

	public static double WorldShare(Sport sport, Level level)
	{
		switch (sport)
		{
		case Sport.Hockey:
			switch (level)
			{
			default:
				return 0.06;
			case Level.Junior:
				return 0.15;
			case Level.College:
				return 0.22;
			case Level.Pro:
			case Level.Minor:
				return 0.4;
			}
		case Sport.Soccer:
			switch (level)
			{
			default:
				return 0.1;
			case Level.College:
				return 0.3;
			case Level.Pro:
			case Level.Minor:
				return 0.6;
			}
		case Sport.Baseball:
			switch (level)
			{
			default:
				return 0.06;
			case Level.College:
				return 0.12;
			case Level.Pro:
			case Level.Minor:
				return 0.32;
			}
		case Sport.Basketball:
			switch (level)
			{
			default:
				return 0.03;
			case Level.College:
				return 0.12;
			case Level.Pro:
			case Level.Minor:
				return 0.22;
			}
		default:
			switch (level)
			{
			default:
				return 0.02;
			case Level.College:
				return 0.06;
			case Level.Pro:
			case Level.Minor:
				return 0.08;
			}
		}
	}

	private static KeyValuePair<string, int>[] W(params object[] pairs)
	{
		KeyValuePair<string, int>[] list = new KeyValuePair<string, int>[pairs.Length / 2];
		for (int i = 0; i < list.Length; i++)
		{
			list[i] = new KeyValuePair<string, int>((string)pairs[i * 2], (int)pairs[i * 2 + 1]);
		}
		return list;
	}

	private static NameOrigin PickOrigin(Sport sport, Random random)
	{
		KeyValuePair<string, int>[] list = Weights[sport];
		int total = 0;
		KeyValuePair<string, int>[] array = list;
		foreach (KeyValuePair<string, int> kv in array)
		{
			total += kv.Value;
		}
		int roll = random.Next(total);
		array = list;
		for (int i = 0; i < array.Length; i++)
		{
			KeyValuePair<string, int> kv2 = array[i];
			roll -= kv2.Value;
			if (roll < 0)
			{
				return ByCode[kv2.Key];
			}
		}
		return ByCode[list[0].Key];
	}

	public static void Pick(Sport sport, Level level, Random random, Dictionary<string, int> used, out string first, out string last)
	{
		Pick(sport, level, random, used, out first, out last, (Position)(-1));
	}

	public static void Pick(Sport sport, Level level, Random random, Dictionary<string, int> used, out string first, out string last, Position pos)
	{
		first = "";
		last = "";
		for (int attempt = 0; attempt < 20; attempt++)
		{
			first = FirstUS[random.Next(FirstUS.Length)];
			last = LastUS[random.Next(LastUS.Length)];
			if (used == null || !used.ContainsKey(first + " " + last))
			{
				break;
			}
		}
		if (used != null)
		{
			used.TryGetValue(first + " " + last, out var n);
			used[first + " " + last] = n + 1;
		}
	}

	public static IEnumerable<KeyValuePair<string, string[]>> AllFirstNames()
	{
		yield return new KeyValuePair<string, string[]>("us", FirstUS);
		NameOrigin[] origins = Origins;
		foreach (NameOrigin o in origins)
		{
			yield return new KeyValuePair<string, string[]>(o.Code, o.First);
		}
	}
}
