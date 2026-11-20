using System;

namespace Skyline_Sports_Project.Engine;

public static class Bios
{
	public const string CityHometown = "*";

	public const string DistrictMark = "*d:";

	private static readonly string[] US = new string[70]
	{
		"Москва, Россия", "Санкт-Петербург, Россия", "Новосибирск, Россия", "Екатеринбург, Россия", "Казань, Россия", "Нижний Новгород, Россия", "Челябинск, Россия", "Самара, Россия", "Омск, Россия", "Ростов-на-Дону, Россия",
		"Уфа, Россия", "Красноярск, Россия", "Пермь, Россия", "Воронеж, Россия", "Волгоград, Россия", "Краснодар, Россия", "Саратов, Россия", "Тюмень, Россия", "Тольятти, Россия", "Ижевск, Россия",
		"Барнаул, Россия", "Иркутск, Россия", "Хабаровск, Россия", "Ярославль, Россия", "Владивосток, Россия", "Махачкала, Россия", "Томск, Россия", "Оренбург, Россия", "Кемерово, Россия", "Новокузнецк, Россия",
		"Рязань, Россия", "Астрахань, Россия", "Пенза, Россия", "Липецк, Россия", "Киров, Россия", "Чебоксары, Россия", "Калининград, Россия", "Тула, Россия", "Курск, Россия", "Ставрополь, Россия",
		"Сочи, Россия", "Белгород, Россия", "Брянск, Россия", "Владимир, Россия", "Архангельск, Россия", "Мурманск, Россия", "Петрозаводск, Россия", "Смоленск, Россия", "Тверь, Россия", "Иваново, Россия",
		"Кострома, Россия", "Тамбов, Россия", "Сургут, Россия", "Нижневартовск, Россия", "Норильск, Россия", "Якутск, Россия", "Улан-Удэ, Россия", "Чита, Россия", "Благовещенск, Россия", "Южно-Сахалинск, Россия",
		"Магнитогорск, Россия", "Череповец, Россия", "Волжский, Россия", "Орёл, Россия", "Калуга, Россия", "Севастополь, Россия", "Псков, Россия", "Великий Новгород, Россия", "Старый Оскол, Россия", "Курган, Россия"
	};

	private static readonly string[] USHockey = new string[30]
	{
		"Москва, Россия", "Санкт-Петербург, Россия", "Ярославль, Россия", "Казань, Россия", "Магнитогорск, Россия", "Омск, Россия", "Челябинск, Россия", "Новосибирск, Россия", "Уфа, Россия", "Череповец, Россия",
		"Нижний Новгород, Россия", "Екатеринбург, Россия", "Тольятти, Россия", "Самара, Россия", "Пермь, Россия", "Владивосток, Россия", "Хабаровск, Россия", "Красноярск, Россия", "Тюмень, Россия", "Ижевск, Россия",
		"Киров, Россия", "Воронеж, Россия", "Сочи, Россия", "Ростов-на-Дону, Россия", "Минск", "Астана", "Алматы", "Рига", "Таллин", "Хельсинки"
	};

	private static readonly string[] Canada = new string[23]
	{
		"Toronto, ON", "Mississauga, ON", "Oakville, ON", "London, ON", "Ottawa, ON", "Sudbury, ON", "Kingston, ON", "Hamilton, ON", "Sault Ste. Marie, ON", "Thunder Bay, ON",
		"Calgary, AB", "Edmonton, AB", "Red Deer, AB", "Regina, SK", "Saskatoon, SK", "Winnipeg, MB", "Brandon, MB", "Vancouver, BC", "Kelowna, BC", "Burnaby, BC",
		"Halifax, NS", "Moncton, NB", "St. John's, NL"
	};

	private static readonly string[] Quebec = new string[8] { "Montreal, QC", "Quebec City, QC", "Laval, QC", "Sherbrooke, QC", "Trois-Rivieres, QC", "Gatineau, QC", "Rimouski, QC", "Chicoutimi, QC" };

	private static readonly string[] France = new string[8] { "Paris, France", "Lyon, France", "Marseille, France", "Lille, France", "Bordeaux, France", "Nantes, France", "Le Mans, France", "Strasbourg, France" };

	private static readonly string[] Russia = new string[10] {
		"Москва, Россия", "Санкт-Петербург, Россия", "Ярославль, Россия", "Казань, Россия", "Магнитогорск, Россия", "Омск, Россия", "Челябинск, Россия", "Новосибирск, Россия", "Уфа, Россия", "Череповец, Россия"
	};

	private static readonly string[] Ukraine = new string[6] { "Kyiv, Ukraine", "Kharkiv, Ukraine", "Lviv, Ukraine", "Odesa, Ukraine", "Dnipro, Ukraine", "Zaporizhzhia, Ukraine" };

	private static readonly string[] Czech = new string[11]
	{
		"Prague, Czechia", "Brno, Czechia", "Ostrava, Czechia", "Pardubice, Czechia", "Plzen, Czechia", "Kladno, Czechia", "Liberec, Czechia", "Bratislava, Slovakia", "Kosice, Slovakia", "Trencin, Slovakia",
		"Nitra, Slovakia"
	};

	private static readonly string[] Sweden = new string[11]
	{
		"Stockholm, Sweden", "Gothenburg, Sweden", "Malmo, Sweden", "Ornskoldsvik, Sweden", "Vasteras, Sweden", "Lulea, Sweden", "Sundsvall, Sweden", "Linkoping, Sweden", "Uppsala, Sweden", "Oslo, Norway",
		"Copenhagen, Denmark"
	};

	private static readonly string[] Finland = new string[7] { "Helsinki, Finland", "Tampere, Finland", "Turku, Finland", "Oulu, Finland", "Espoo, Finland", "Lahti, Finland", "Jyvaskyla, Finland" };

	private static readonly string[] Germany = new string[10] { "Berlin, Germany", "Munich, Germany", "Cologne, Germany", "Mannheim, Germany", "Hamburg, Germany", "Dusseldorf, Germany", "Zurich, Switzerland", "Bern, Switzerland", "Vienna, Austria", "Salzburg, Austria" };

	private static readonly string[] Italy = new string[6] { "Milan, Italy", "Rome, Italy", "Naples, Italy", "Turin, Italy", "Florence, Italy", "Bologna, Italy" };

	private static readonly string[] LatinBaseball = new string[15]
	{
		"Santo Domingo, Dominican Republic", "San Pedro de Macoris, Dominican Republic", "Santiago, Dominican Republic", "Maracaibo, Venezuela", "Caracas, Venezuela", "Valencia, Venezuela", "Havana, Cuba", "Cienfuegos, Cuba", "San Juan, Puerto Rico", "Bayamon, Puerto Rico",
		"Monterrey, Mexico", "Hermosillo, Mexico", "Panama City, Panama", "Barranquilla, Colombia", "Managua, Nicaragua"
	};

	private static readonly string[] LatinSoccer = new string[16]
	{
		"Buenos Aires, Argentina", "Rosario, Argentina", "Madrid, Spain", "Barcelona, Spain", "Seville, Spain", "Valencia, Spain", "Mexico City, Mexico", "Guadalajara, Mexico", "Monterrey, Mexico", "Bogota, Colombia",
		"Medellin, Colombia", "Montevideo, Uruguay", "Santiago, Chile", "Lima, Peru", "San Jose, Costa Rica", "Quito, Ecuador"
	};

	private static readonly string[] LatinOther = new string[7] { "Madrid, Spain", "Barcelona, Spain", "Buenos Aires, Argentina", "Santo Domingo, Dominican Republic", "San Juan, Puerto Rico", "Mexico City, Mexico", "Monterrey, Mexico" };

	private static readonly string[] Brazil = new string[9] { "Sao Paulo, Brazil", "Rio de Janeiro, Brazil", "Belo Horizonte, Brazil", "Porto Alegre, Brazil", "Salvador, Brazil", "Recife, Brazil", "Lisbon, Portugal", "Porto, Portugal", "Braga, Portugal" };

	private static readonly string[] Balkans = new string[7] { "Belgrade, Serbia", "Novi Sad, Serbia", "Nis, Serbia", "Kragujevac, Serbia", "Podgorica, Montenegro", "Banja Luka, Bosnia", "Sombor, Serbia" };

	private static readonly string[] Britain = new string[10] { "London, England", "Manchester, England", "Liverpool, England", "Birmingham, England", "Leeds, England", "Newcastle, England", "Glasgow, Scotland", "Edinburgh, Scotland", "Cardiff, Wales", "Belfast, Northern Ireland" };

	private static readonly string[] Australia = new string[7] { "Melbourne, Australia", "Sydney, Australia", "Brisbane, Australia", "Perth, Australia", "Adelaide, Australia", "Geelong, Australia", "Gold Coast, Australia" };

	private static readonly string[] Japan = new string[8] { "Tokyo, Japan", "Osaka, Japan", "Yokohama, Japan", "Nagoya, Japan", "Sapporo, Japan", "Fukuoka, Japan", "Hiroshima, Japan", "Sendai, Japan" };

	private static readonly string[] Korea = new string[5] { "Seoul, South Korea", "Busan, South Korea", "Incheon, South Korea", "Daegu, South Korea", "Gwangju, South Korea" };

	private static readonly string[] Ghana = new string[5] { "Accra, Ghana", "Kumasi, Ghana", "Tamale, Ghana", "Takoradi, Ghana", "Cape Coast, Ghana" };

	private static readonly string[] Nigeria = new string[6] { "Lagos, Nigeria", "Abuja, Nigeria", "Enugu, Nigeria", "Ibadan, Nigeria", "Port Harcourt, Nigeria", "Benin City, Nigeria" };

	private static readonly string[] WestAfricaFrench = new string[6] { "Dakar, Senegal", "Thies, Senegal", "Saint-Louis, Senegal", "Bamako, Mali", "Abidjan, Ivory Coast", "Conakry, Guinea" };

	private static readonly string[] NorthAfrica = new string[8] { "Casablanca, Morocco", "Rabat, Morocco", "Marrakesh, Morocco", "Tangier, Morocco", "Cairo, Egypt", "Tunis, Tunisia", "Algiers, Algeria", "Oran, Algeria" };

	private static readonly string[] Polynesia = new string[10] { "Honolulu, HI", "Laie, HI", "Kahuku, HI", "Pago Pago, American Samoa", "Salt Lake City, UT", "Carson, CA", "Euless, TX", "Auckland, New Zealand", "Nuku'alofa, Tonga", "Apia, Samoa" };

	private static readonly string[] AfricanAmericanCities = new string[8] { "Houston, TX", "Dallas, TX", "Atlanta, GA", "Baltimore, MD", "Newark, NJ", "Charlotte, NC", "Washington, DC", "Columbus, OH" };

	private static void Size(Position pos, out double height, out double weight, out double hSd, out double wSd)
	{
		hSd = 2.0;
		wSd = 12.0;
		switch (pos)
		{
		case Position.Center:
		case Position.LeftWing:
		case Position.RightWing:
			height = 72.5;
			weight = 196.0;
			break;
		case Position.Defense:
			height = 74.0;
			weight = 205.0;
			break;
		case Position.Goalie:
			height = 74.0;
			weight = 198.0;
			break;
		case Position.PointGuard:
			height = 74.0;
			weight = 192.0;
			hSd = 1.6;
			wSd = 13.0;
			break;
		case Position.ShootingGuard:
			height = 77.0;
			weight = 206.0;
			hSd = 1.6;
			wSd = 13.0;
			break;
		case Position.SmallForward:
			height = 79.0;
			weight = 220.0;
			hSd = 1.6;
			wSd = 13.0;
			break;
		case Position.PowerForward:
			height = 81.0;
			weight = 234.0;
			hSd = 1.6;
			wSd = 13.0;
			break;
		case Position.BigMan:
			height = 83.0;
			weight = 250.0;
			hSd = 1.6;
			wSd = 14.0;
			break;
		case Position.Quarterback:
			height = 75.0;
			weight = 225.0;
			hSd = 1.5;
			wSd = 10.0;
			break;
		case Position.RunningBack:
			height = 70.0;
			weight = 215.0;
			hSd = 1.5;
			wSd = 12.0;
			break;
		case Position.WideReceiver:
			height = 72.0;
			weight = 200.0;
			hSd = 2.0;
			wSd = 13.0;
			break;
		case Position.TightEnd:
			height = 76.0;
			weight = 250.0;
			hSd = 1.3;
			wSd = 9.0;
			break;
		case Position.OffensiveLine:
			height = 77.0;
			weight = 315.0;
			hSd = 1.3;
			wSd = 12.0;
			break;
		case Position.DefensiveLine:
			height = 75.0;
			weight = 290.0;
			hSd = 1.6;
			wSd = 22.0;
			break;
		case Position.Linebacker:
			height = 74.0;
			weight = 240.0;
			hSd = 1.3;
			wSd = 9.0;
			break;
		case Position.Cornerback:
			height = 71.0;
			weight = 193.0;
			hSd = 1.4;
			wSd = 8.0;
			break;
		case Position.Safety:
			height = 72.0;
			weight = 205.0;
			hSd = 1.4;
			wSd = 8.0;
			break;
		case Position.Kicker:
			height = 72.0;
			weight = 192.0;
			hSd = 2.0;
			wSd = 14.0;
			break;
		case Position.Punter:
			height = 74.0;
			weight = 215.0;
			hSd = 1.8;
			wSd = 14.0;
			break;
		case Position.StartingPitcher:
		case Position.ReliefPitcher:
			height = 74.6;
			weight = 214.0;
			wSd = 14.0;
			break;
		case Position.Catcher:
			height = 72.0;
			weight = 215.0;
			wSd = 14.0;
			break;
		case Position.FirstBase:
			height = 75.0;
			weight = 225.0;
			wSd = 14.0;
			break;
		case Position.SecondBase:
			height = 71.0;
			weight = 190.0;
			wSd = 14.0;
			break;
		case Position.Shortstop:
			height = 72.0;
			weight = 195.0;
			wSd = 14.0;
			break;
		case Position.ThirdBase:
			height = 73.0;
			weight = 210.0;
			wSd = 14.0;
			break;
		case Position.LeftField:
		case Position.CenterField:
		case Position.RightField:
			height = 73.0;
			weight = 205.0;
			wSd = 14.0;
			break;
		case Position.Goalkeeper:
			height = 74.5;
			weight = 186.0;
			hSd = 1.8;
			wSd = 10.0;
			break;
		case Position.Defender:
			height = 72.3;
			weight = 172.0;
			hSd = 2.0;
			wSd = 10.0;
			break;
		case Position.Midfielder:
			height = 69.6;
			weight = 160.0;
			hSd = 2.2;
			wSd = 10.0;
			break;
		default:
			height = 70.5;
			weight = 166.0;
			hSd = 2.2;
			wSd = 11.0;
			break;
		}
	}

	public static void Give(Player p, Random random, bool cityKid)
	{
		Size(p.Position, out var h, out var w, out var hSd, out var wSd);
		double z = Rosters.Normal(random, 0.0, 1.0);
		int height = (int)Math.Round(h + z * hSd);
		int weight = (int)Math.Round(w + ((double)height - h) * 6.0 + Rosters.Normal(random, 0.0, wSd * 0.8));
		p.Height = Math.Max(64, Math.Min(88, height));
		p.Weight = Math.Max(150, Math.Min(360, weight));
		p.Hand = Hand(p, random);
		p.Hometown = (cityKid ? "*" : Hometown(p, random));
	}

	private static string Hand(Player p, Random random)
	{
		double r = random.NextDouble();
		switch (p.Sport)
		{
		case Sport.Hockey:
			if (p.Position != Position.Goalie)
			{
				if (!(r < 0.62))
				{
					return "Бросает: правой";
				}
				return "Бросает: левой";
			}
			if (!(r < 0.9))
			{
				return "Ловит: правой";
			}
			return "Ловит: левой";
		case Sport.Basketball:
			if (!(r < 0.9))
			{
				return "Бросает: левой";
			}
			return "Бросает: правой";
		case Sport.Football:
			if (p.Position == Position.Quarterback)
			{
				if (!(r < 0.94))
				{
					return "Бросает: левой";
				}
				return "Бросает: правой";
			}
			if (p.Position == Position.Kicker || p.Position == Position.Punter)
			{
				if (!(r < 0.85))
				{
					return "Бьёт: левой";
				}
				return "Бьёт: правой";
			}
			return "";
		case Sport.Baseball:
		{
			if (Rosters.IsPitcher(p.Position))
			{
				if (!(r < 0.72))
				{
					return "Бросает: левой";
				}
				return "Бросает: правой";
			}
			string bats;
			if (r < 0.55)
			{
				bats = "R";
			}
			else
			{
				bats = ((r < 0.9) ? "L" : "S");
			}
			string throws = (((p.Position == Position.FirstBase || p.Position == Position.LeftField || p.Position == Position.CenterField || p.Position == Position.RightField) && bats == "L" && random.NextDouble() < 0.5) ? "L" : "R");
			return "Бьёт: " + (bats == "L" ? "левой" : bats == "S" ? "обеими" : "правой") + ", бросает: " + (throws == "L" ? "левой" : "правой");
		}
		default:
			if (!(r < 0.77))
			{
				return "Левая нога";
			}
			return "Правая нога";
		}
	}

	public static int ShownHeight(Player p)
	{
		if (p.Height <= 0)
		{
			return 0;
		}
		if (p.Level != Level.HighSchool)
		{
			return p.Height;
		}
		int cls = Math.Max(1, Math.Min(4, p.ClassYear));
		return p.Height - ((cls == 1) ? 2 : ((cls == 2) ? 1 : 0));
	}

	public static int ShownWeight(Player p)
	{
		if (p.Weight <= 0)
		{
			return 0;
		}
		int cls = Math.Max(1, Math.Min(4, p.ClassYear));
		if (p.Level == Level.HighSchool)
		{
			return (int)Math.Round((double)p.Weight * (0.8 + 0.04 * (double)cls));
		}
		if (p.Level == Level.College)
		{
			return (int)Math.Round((double)p.Weight * (0.93 + 0.015 * (double)cls));
		}
		if (p.Level == Level.Junior)
		{
			return (int)Math.Round((double)p.Weight * (0.9 + 0.02 * (double)cls));
		}
		return p.Weight;
	}

	public static string HeightLabel(int inches)
	{
		if (inches <= 0)
		{
			return "";
		}
		return inches / 12 + "'" + inches % 12 + "\"";
	}

	public static string HometownLabel(Player p, string cityName)
	{
		string town = p.Hometown ?? "";
		if (town == "*")
		{
			if (!string.IsNullOrEmpty(cityName))
			{
				return cityName;
			}
			return "Ваш город";
		}
		if (town.StartsWith("*d:", StringComparison.Ordinal))
		{
			string district = town.Substring("*d:".Length);
			if (!string.IsNullOrEmpty(cityName))
			{
				return district + ", " + cityName;
			}
			return district;
		}
		return town;
	}

	public static bool IsLocal(Player p)
	{
		string town = p?.Hometown;
		if (town != null)
		{
			if (!(town == "*"))
			{
				return town.StartsWith("*d:", StringComparison.Ordinal);
			}
			return true;
		}
		return false;
	}

	private static double LocalShare(Player p)
	{
		switch (p.Level)
		{
		case Level.Pro:
		case Level.Minor:
			return 0.025;
		case Level.Junior:
			return 0.05;
		case Level.College:
			if (p.TeamId < 0)
			{
				return 0.0;
			}
			return 0.04;
		default:
			return 0.0;
		}
	}

	private static string Any(string[] list, Random random)
	{
		return list[random.Next(list.Length)];
	}

	private static string[][] PlacesOf(string origin)
	{
		return origin switch
		{
			"ru" => new string[1][] { Russia }, 
			"ua" => new string[1][] { Ukraine }, 
			"cz" => new string[1][] { Czech }, 
			"se" => new string[1][] { Sweden }, 
			"fi" => new string[1][] { Finland }, 
			"fr" => new string[2][] { Quebec, France }, 
			"de" => new string[1][] { Germany }, 
			"it" => new string[1][] { Italy }, 
			"es" => new string[3][] { LatinBaseball, LatinSoccer, LatinOther }, 
			"br" => new string[1][] { Brazil }, 
			"rs" => new string[1][] { Balkans }, 
			"gb" => new string[1][] { Britain }, 
			"au" => new string[1][] { Australia }, 
			"jp" => new string[1][] { Japan }, 
			"kr" => new string[1][] { Korea }, 
			"gh" => new string[2][] { Ghana, AfricanAmericanCities }, 
			"ng" => new string[2][] { Nigeria, AfricanAmericanCities }, 
			"sn" => new string[1][] { WestAfricaFrench }, 
			"ma" => new string[1][] { NorthAfrica }, 
			"ps" => new string[1][] { Polynesia }, 
			_ => new string[3][] { US, USHockey, Canada }, 
		};
	}

	public static string HometownFor(string firstName, Sport sport, Random random)
	{
		return Hometown(new Player
		{
			FirstName = (firstName ?? ""),
			Sport = sport,
			Level = Level.Pro,
			TeamId = -1
		}, random);
	}

	public static string Hometown(Player p, Random random)
	{
		string origin = PlayerNames.OriginOf(p.FirstName);
		if (origin == "us")
		{
			if (random.NextDouble() < LocalShare(p))
			{
				return "*";
			}
			if (p.Sport == Sport.Hockey)
			{
				double canada;
				if (p.Level == Level.Pro || p.Level == Level.Minor)
				{
					canada = 0.55;
				}
				else if (p.Level == Level.College)
				{
					canada = 0.3;
				}
				else
				{
					canada = ((p.Level == Level.Junior) ? 0.25 : 0.1);
				}
				if (!(random.NextDouble() < canada))
				{
					return Any(USHockey, random);
				}
				return Any(Canada, random);
			}
			return Any(US, random);
		}
		if (p.Level == Level.HighSchool && origin != "fr" && random.NextDouble() < 0.75)
		{
			return Any(US, random);
		}
		switch (origin)
		{
		case "fr":
			if (p.Sport != Sport.Hockey)
			{
				return Any(France, random);
			}
			return Any(Quebec, random);
		case "es":
			if (p.Sport != Sport.Baseball)
			{
				if (p.Sport != Sport.Soccer)
				{
					return Any(LatinOther, random);
				}
				return Any(LatinSoccer, random);
			}
			return Any(LatinBaseball, random);
		case "ng":
			if (p.Sport != Sport.Football && (p.Sport != Sport.Basketball || !(random.NextDouble() < 0.5)))
			{
				return Any(Nigeria, random);
			}
			return Any(AfricanAmericanCities, random);
		case "gh":
			if (p.Sport != Sport.Football)
			{
				return Any(Ghana, random);
			}
			return Any(AfricanAmericanCities, random);
		default:
			return Any(PlacesOf(origin)[0], random);
		}
	}

	public static bool HometownFits(Player p)
	{
		string town = p.Hometown ?? "";
		if (town.Length == 0 || IsLocal(p))
		{
			return true;
		}
		string origin = PlayerNames.OriginOf(p.FirstName);
		if (origin != "us" && Array.IndexOf(US, town) >= 0)
		{
			return true;
		}
		string[][] array = PlacesOf(origin);
		for (int i = 0; i < array.Length; i++)
		{
			if (Array.IndexOf(array[i], town) >= 0)
			{
				return true;
			}
		}
		return false;
	}
}
