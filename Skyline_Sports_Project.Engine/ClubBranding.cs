using System;

namespace Skyline_Sports_Project.Engine;

public static class ClubBranding
{
	public const int LogoCount = 24;

	private static readonly string[] Sponsors = new string[]
	{
		"СеверЭнерго", "ВолгаТелеком", "Полюс Банк", "Гранит Страхование",
		"Вектор Моторс", "Нева Финанс", "АрктикТех", "Старт Спорт",
		"Маяк Мобайл", "Орбита Маркет", "Титан Индастри", "Прайм Инвест"
	};

	public static int SponsorCount => Sponsors.Length;

	public static string SponsorName(int id)
	{
		return id >= 0 && id < Sponsors.Length ? Sponsors[id] : "Без спонсора";
	}

	public static int SponsorTier(int id)
	{
		if (id < 0 || id >= Sponsors.Length) return 0;
		if (id < 3) return 1;
		if (id < 6) return 2;
		if (id < 9) return 3;
		return 4;
	}

	public static int NormalizeLogo(int id)
	{
		return id >= 0 && id < LogoCount ? id : -1;
	}

	public static int NormalizeSponsor(int id)
	{
		return id >= 0 && id < Sponsors.Length ? id : -1;
	}
}
