using System;
using System.Collections.Generic;

namespace Skyline_Sports_Project.Engine;

public static class SportProfiles
{
	private static readonly Dictionary<string, SportProfile> _profiles = Build();

	private const DayOfWeek Mon = DayOfWeek.Monday;

	private const DayOfWeek Tue = DayOfWeek.Tuesday;

	private const DayOfWeek Wed = DayOfWeek.Wednesday;

	private const DayOfWeek Thu = DayOfWeek.Thursday;

	private const DayOfWeek Fri = DayOfWeek.Friday;

	private const DayOfWeek Sat = DayOfWeek.Saturday;

	private const DayOfWeek Sun = DayOfWeek.Sunday;

	public static SportProfile Get(Sport sport, Level level)
	{
		if (_profiles.TryGetValue(sport.ToString() + "-" + level, out var p))
		{
			return p;
		}
		return _profiles[Sport.Basketball.ToString() + "-" + Level.Pro];
	}

	private static int M(int hhmm)
	{
		return hhmm / 100 * 60 + hhmm % 100;
	}

	private static TimeOption T(int hhmm, double weight)
	{
		return new TimeOption(0, M(hhmm), weight, 0);
	}

	private static TimeOption T(int dayOffset, int hhmm, double weight, int cap)
	{
		return new TimeOption(dayOffset, M(hhmm), weight, cap);
	}

	private static DayOfWeek[] Days(params DayOfWeek[] d)
	{
		return d;
	}

	private static TimeOption[] Spread(params int[] hhmm)
	{
		TimeOption[] list = new TimeOption[hhmm.Length];
		for (int i = 0; i < hhmm.Length; i++)
		{
			list[i] = new TimeOption(0, M(hhmm[i]), 1.0, 1);
		}
		return list;
	}

	private static bool Weekend(DayOfWeek d)
	{
		if (d != DayOfWeek.Saturday)
		{
			return d == DayOfWeek.Sunday;
		}
		return true;
	}

	private static PlayoffRoundFormat Series7(string name, Func<DayOfWeek, TimeOption[]> times, int rest)
	{
		PlayoffRoundFormat playoffRoundFormat = new PlayoffRoundFormat();
		playoffRoundFormat.Name = name;
		playoffRoundFormat.BestOf = 7;
		playoffRoundFormat.DayOffsets = new int[7] { 0, 2, 4, 6, 8, 10, 12 };
		playoffRoundFormat.HighSeedHome = new bool[7] { true, true, false, false, true, false, true };
		playoffRoundFormat.RestDays = rest;
		playoffRoundFormat.Times = times;
		playoffRoundFormat.Stagger = 1;
		return playoffRoundFormat;
	}

	private static Dictionary<string, SportProfile> Build()
	{
		List<SportProfile> list = new List<SportProfile>();
		Func<DayOfWeek, TimeOption[]> proHockeyPlayoffTimes = (DayOfWeek d) => Weekend(d) ? new TimeOption[3]
		{
			T(1500, 2.0),
			T(1900, 3.0),
			T(2000, 2.0)
		} : new TimeOption[3]
		{
			T(1900, 4.0),
			T(1930, 3.0),
			T(2000, 2.0)
		};
		list.Add(new SportProfile
		{
			Sport = Sport.Hockey,
			Level = Level.Pro,
			Summary = "84 матча с конца сентября до начала апреля, примерно 3–4 игры в неделю, начало большинства матчей в 19:00, с праздничной паузой и перерывом на Матч всех звёзд. В плей-офф выходят лучшие команды половины таблицы; каждый раунд — до четырёх побед (схема домашних матчей 2–2–1–1–1).",
			GamesPerTeam = 84,
			Start = (int y) => GameClock.OnOrAfter(GameClock.D(y, 9, 28), DayOfWeek.Tuesday),
			End = (int y) => GameClock.OnOrAfter(GameClock.D(y + 1, 4, 9), DayOfWeek.Saturday),
			Breaks = (int y) => new DateRange[2]
			{
				new DateRange(GameClock.D(y, 12, 24), GameClock.D(y, 12, 26)),
				new DateRange(GameClock.Nth(y + 1, 2, DayOfWeek.Sunday, 1).AddDays(-3.0), GameClock.Nth(y + 1, 2, DayOfWeek.Sunday, 1).AddDays(2.0))
			},
			Week = new SlotTemplate[4]
			{
				new SlotTemplate
				{
					Anchor = DayOfWeek.Tuesday,
					Times = (DayOfWeek d, bool last) => new TimeOption[5]
					{
						T(0, 1900, 5.0, 0),
						T(0, 1930, 3.0, 0),
						T(0, 2000, 1.0, 0),
						T(1, 1900, 3.0, 0),
						T(1, 1930, 2.0, 0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Thursday,
					Times = (DayOfWeek d, bool last) => new TimeOption[5]
					{
						T(0, 1900, 5.0, 0),
						T(0, 1930, 3.0, 0),
						T(0, 2000, 1.0, 0),
						T(1, 1900, 2.0, 0),
						T(1, 1930, 1.0, 0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Saturday,
					Times = (DayOfWeek d, bool last) => new TimeOption[4]
					{
						T(0, 1300, 1.0, 0),
						T(0, 1600, 1.0, 0),
						T(0, 1900, 5.0, 0),
						T(0, 2000, 1.0, 0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Sunday,
					Optional = true,
					Times = (DayOfWeek d, bool last) => new TimeOption[4]
					{
						T(0, 1230, 2.0, 0),
						T(0, 1500, 2.0, 0),
						T(0, 1700, 1.0, 0),
						T(0, 1900, 1.0, 0)
					}
				}
			},
			PlayoffShare = 0.5,
			MaxPlayoffTeams = 16,
			PlayoffStart = (int y) => GameClock.OnOrAfter(GameClock.D(y + 1, 4, 9), DayOfWeek.Saturday).AddDays(3.0),
			RoundsFromFinal = new PlayoffRoundFormat[4]
			{
				Series7("Финал кубка", proHockeyPlayoffTimes, 3),
				Series7("Финал конференции", proHockeyPlayoffTimes, 2),
				Series7("Второй раунд", proHockeyPlayoffTimes, 2),
				Series7("Первый раунд", proHockeyPlayoffTimes, 2)
			}
		});
		Func<DayOfWeek, TimeOption[]> proHoopsPlayoffTimes = (DayOfWeek d) => Weekend(d) ? new TimeOption[4]
		{
			T(1300, 1.0),
			T(1530, 2.0),
			T(1800, 1.0),
			T(2030, 2.0)
		} : new TimeOption[4]
		{
			T(1900, 2.0),
			T(1930, 3.0),
			T(2000, 2.0),
			T(2130, 1.0)
		};
		list.Add(new SportProfile
		{
			Sport = Sport.Basketball,
			Level = Level.Pro,
			Summary = "82 матча с конца октября до середины апреля, примерно 3–4 игры в неделю, начало большинства матчей в 19:00–20:00. Плей-офф начинается после квалификационной недели: выходят лучшие команды половины таблицы, каждый раунд — до четырёх побед.",
			GamesPerTeam = 82,
			Start = (int y) => GameClock.OnOrAfter(GameClock.D(y, 10, 20), DayOfWeek.Tuesday),
			End = (int y) => GameClock.Nth(y + 1, 4, DayOfWeek.Sunday, 2),
			Breaks = (int y) => new DateRange[3]
			{
				new DateRange(GameClock.Nth(y, 11, DayOfWeek.Thursday, 4), GameClock.Nth(y, 11, DayOfWeek.Thursday, 4)),
				new DateRange(GameClock.D(y, 12, 24), GameClock.D(y, 12, 24)),
				new DateRange(GameClock.Nth(y + 1, 2, DayOfWeek.Sunday, 3).AddDays(-3.0), GameClock.Nth(y + 1, 2, DayOfWeek.Sunday, 3).AddDays(3.0))
			},
			Week = new SlotTemplate[4]
			{
				new SlotTemplate
				{
					Anchor = DayOfWeek.Monday,
					Times = (DayOfWeek d, bool last) => new TimeOption[6]
					{
						T(0, 1900, 3.0, 0),
						T(0, 1930, 3.0, 0),
						T(0, 2000, 1.0, 0),
						T(1, 1900, 3.0, 0),
						T(1, 1930, 3.0, 0),
						T(1, 2200, 1.0, 0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Wednesday,
					Times = (DayOfWeek d, bool last) => new TimeOption[6]
					{
						T(0, 1900, 3.0, 0),
						T(0, 1930, 3.0, 0),
						T(0, 2000, 1.0, 0),
						T(1, 1930, 3.0, 0),
						T(1, 2000, 2.0, 0),
						T(1, 2200, 1.0, 0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Friday,
					Times = (DayOfWeek d, bool last) => new TimeOption[6]
					{
						T(0, 1900, 3.0, 0),
						T(0, 1930, 3.0, 0),
						T(0, 2000, 2.0, 0),
						T(1, 1700, 1.0, 0),
						T(1, 1930, 2.0, 0),
						T(1, 2030, 1.0, 0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Sunday,
					Optional = true,
					Times = (DayOfWeek d, bool last) => new TimeOption[5]
					{
						T(0, 1230, 1.0, 0),
						T(0, 1500, 2.0, 0),
						T(0, 1530, 1.0, 0),
						T(0, 1800, 2.0, 0),
						T(0, 1900, 1.0, 0)
					}
				}
			},
			PlayoffShare = 0.5,
			MaxPlayoffTeams = 16,
			PlayoffStart = (int y) => GameClock.Nth(y + 1, 4, DayOfWeek.Sunday, 2).AddDays(6.0),
			RoundsFromFinal = new PlayoffRoundFormat[4]
			{
				Series7("Финал", proHoopsPlayoffTimes, 3),
				Series7("Финалы конференций", proHoopsPlayoffTimes, 2),
				Series7("Полуфиналы конференций", proHoopsPlayoffTimes, 2),
				Series7("Первый раунд", proHoopsPlayoffTimes, 6)
			}
		});
		list.Add(new SportProfile
		{
			Sport = Sport.Football,
			Level = Level.Pro,
			Summary = "17 матчей за 18 недель с одной свободной неделей, с сентября до начала января. Игры проходят по вечерам в четверг, днём и вечером в воскресенье, а также в понедельник. Плей-офф состоит из матчей на вылет, затем в феврале проводится финал на нейтральном поле.",
			GamesPerTeam = 17,
			ByesPerTeam = 1,
			Weekly = true,
			Start = (int y) => GameClock.Nth(y, 9, DayOfWeek.Monday, 1).AddDays(3.0),
			End = (int y) => GameClock.Nth(y, 9, DayOfWeek.Monday, 1).AddDays(125.0),
			Week = new SlotTemplate[1]
			{
				new SlotTemplate
				{
					Anchor = DayOfWeek.Sunday,
					Times = (DayOfWeek d, bool last) => new TimeOption[6]
					{
						T(-3, 2015, 1.0, 1),
						T(0, 1300, 9.0, 0),
						T(0, 1605, 2.0, 2),
						T(0, 1625, 3.0, 3),
						T(0, 2020, 1.0, 1),
						T(1, 2015, 1.0, 1)
					}
				}
			},
			PlayoffShare = 0.4375,
			PlayoffPowerOfTwo = false,
			MaxPlayoffTeams = 14,
			SingleYearLabel = true,
			ChampionLabel = "Чемпионы",
			RoundsFromFinal = new PlayoffRoundFormat[4]
			{
				new PlayoffRoundFormat
				{
					Name = "Финал чемпионата",
					Neutral = true,
					FixedDate = (int y) => GameClock.Nth(y + 1, 2, DayOfWeek.Sunday, 2),
					StartDays = Days(default(DayOfWeek)),
					RestDays = 13,
					Times = (DayOfWeek d) => new TimeOption[1] { T(1830, 1.0) }
				},
				new PlayoffRoundFormat
				{
					Name = "Чемпионат конференции",
					FixedDate = (int y) => GameClock.Nth(y + 1, 2, DayOfWeek.Sunday, 2).AddDays(-14.0),
					StartDays = Days(default(DayOfWeek)),
					RestDays = 6,
					Times = (DayOfWeek d) => new TimeOption[2]
					{
						T(0, 1500, 1.0, 1),
						T(0, 1830, 1.0, 1)
					}
				},
				new PlayoffRoundFormat
				{
					Name = "Раунд дивизионов",
					FixedDate = (int y) => GameClock.Nth(y + 1, 2, DayOfWeek.Sunday, 2).AddDays(-22.0),
					StartDays = Days(DayOfWeek.Saturday),
					RestDays = 4,
					Times = (DayOfWeek d) => new TimeOption[4]
					{
						T(0, 1630, 1.0, 1),
						T(0, 2015, 1.0, 1),
						T(1, 1500, 1.0, 1),
						T(1, 1830, 1.0, 1)
					}
				},
				new PlayoffRoundFormat
				{
					Name = "Уайлд-кард",
					FixedDate = (int y) => GameClock.Nth(y + 1, 2, DayOfWeek.Sunday, 2).AddDays(-29.0),
					StartDays = Days(DayOfWeek.Saturday),
					RestDays = 5,
					Times = (DayOfWeek d) => new TimeOption[6]
					{
						T(0, 1630, 1.0, 1),
						T(0, 2015, 1.0, 1),
						T(1, 1300, 1.0, 1),
						T(1, 1630, 1.0, 1),
						T(1, 2015, 1.0, 1),
						T(2, 2015, 1.0, 1)
					}
				}
			}
		});
		Func<DayOfWeek, bool, TimeOption[]> mlbTimes = (DayOfWeek d, bool last) =>
		{
			switch (d)
			{
			case DayOfWeek.Saturday:
				return new TimeOption[5]
				{
					T(1305, 2.0),
					T(1605, 3.0),
					T(1815, 1.0),
					T(1910, 2.0),
					T(1915, 1.0)
				};
			case DayOfWeek.Sunday:
				return new TimeOption[5]
				{
					T(1305, 3.0),
					T(1310, 2.0),
					T(1335, 3.0),
					T(1610, 1.0),
					T(1910, 0.3)
				};
			case DayOfWeek.Friday:
				return new TimeOption[4]
				{
					T(1840, 2.0),
					T(1905, 3.0),
					T(1910, 2.0),
					T(2010, 1.0)
				};
			default:
				if (last)
				{
					return new TimeOption[4]
					{
						T(1235, 1.0),
						T(1305, 2.0),
						T(1840, 2.0),
						T(1905, 3.0)
					};
				}
				return new TimeOption[4]
				{
					T(1840, 2.0),
					T(1905, 4.0),
					T(1910, 2.0),
					T(1820, 1.0)
				};
			}
		};
		Func<DayOfWeek, TimeOption[]> mlbPlayoffTimes = (DayOfWeek d) => Weekend(d) ? new TimeOption[3]
		{
			T(1308, 1.0),
			T(1608, 2.0),
			T(2008, 2.0)
		} : new TimeOption[3]
		{
			T(1608, 1.0),
			T(1808, 2.0),
			T(2008, 3.0)
		};
		list.Add(new SportProfile
		{
			Sport = Sport.Baseball,
			Level = Level.Pro,
			Summary = "162 матча с конца марта до конца сентября, преимущественно серии по три игры с выходными днями и перерывом на Матч всех звёзд в июле. В октябре проходит плей-офф: уайлд-кард — до двух побед, серия дивизионов — до трёх, чемпионская серия и Мировая серия — до четырёх (малые лиги начинают с более поздней стадии).",
			GamesPerTeam = 162,
			Start = (int y) => GameClock.Last(y, 3, DayOfWeek.Thursday),
			End = (int y) => GameClock.Last(y, 9, DayOfWeek.Sunday),
			Breaks = (int y) => new DateRange[1]
			{
				new DateRange(GameClock.Nth(y, 7, DayOfWeek.Tuesday, 2).AddDays(-1.0), GameClock.Nth(y, 7, DayOfWeek.Tuesday, 2).AddDays(2.0))
			},
			Week = new SlotTemplate[3]
			{
				new SlotTemplate
				{
					Anchor = DayOfWeek.Monday,
					Optional = true,
					Times = (DayOfWeek d, bool last) => new TimeOption[3]
					{
						T(1840, 2.0),
						T(1905, 2.0),
						T(1910, 1.0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Tuesday,
					Length = 3,
					Times = mlbTimes
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Friday,
					Length = 3,
					Times = mlbTimes
				}
			},
			PlayoffShare = 0.4,
			PlayoffPowerOfTwo = false,
			MinPlayoffTeams = 4,
			MaxPlayoffTeams = 12,
			RoundsFromFinal = new PlayoffRoundFormat[4]
			{
				new PlayoffRoundFormat
				{
					Name = "Мировая серия",
					BestOf = 7,
					DayOffsets = new int[7] { 0, 1, 3, 4, 5, 7, 8 },
					HighSeedHome = new bool[7] { true, true, false, false, false, true, true },
					RestDays = 3,
					Times = (DayOfWeek d) => new TimeOption[1] { T(2003, 1.0) }
				},
				new PlayoffRoundFormat
				{
					Name = "Финальная серия лиги",
					BestOf = 7,
					DayOffsets = new int[7] { 0, 1, 3, 4, 5, 7, 8 },
					HighSeedHome = new bool[7] { true, true, false, false, false, true, true },
					RestDays = 1,
					Times = mlbPlayoffTimes
				},
				new PlayoffRoundFormat
				{
					Name = "Серия дивизионов",
					BestOf = 5,
					DayOffsets = new int[5] { 0, 1, 3, 4, 6 },
					HighSeedHome = new bool[5] { true, true, false, false, true },
					RestDays = 1,
					Times = mlbPlayoffTimes
				},
				new PlayoffRoundFormat
				{
					Name = "Серия уайлд-кард",
					BestOf = 3,
					DayOffsets = new int[3] { 0, 1, 2 },
					HighSeedHome = new bool[3] { true, true, true },
					RestDays = 1,
					Times = mlbPlayoffTimes
				}
			}
		});
		Func<DayOfWeek, TimeOption[]> mlsPlayoffTimes = (DayOfWeek d) => Weekend(d) ? new TimeOption[4]
		{
			T(1530, 1.0),
			T(1800, 1.0),
			T(1930, 2.0),
			T(2030, 1.0)
		} : new TimeOption[2]
		{
			T(1930, 3.0),
			T(2000, 1.0)
		};
		list.Add(new SportProfile
		{
			Sport = Sport.Soccer,
			Level = Level.Pro,
			Summary = "30 туров: июль–декабрь, январский отпуск, возобновление в феврале и завершение в мае. Последний, 30-й тур проводится одновременно для всех клубов.",
			GamesPerTeam = 30,
			Weekly = true,
			Start = (int y) => GameClock.D(y, 7, 1),
			End = (int y) => GameClock.D(y + 1, 5, 31),
			Breaks = (int y) => new DateRange[1]
			{
				new DateRange(GameClock.D(y + 1, 1, 1), GameClock.D(y + 1, 1, 31))
			},
			Week = new SlotTemplate[2]
			{
				new SlotTemplate
				{
					Anchor = DayOfWeek.Wednesday,
					Optional = true,
					Times = (DayOfWeek d, bool last) => new TimeOption[2]
					{
						T(1930, 4.0),
						T(2000, 1.0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Saturday,
					Times = (DayOfWeek d, bool last) => new TimeOption[7]
					{
						T(0, 1930, 6.0, 0),
						T(0, 1900, 1.0, 0),
						T(0, 2030, 1.0, 0),
						T(0, 1630, 1.0, 0),
						T(1, 1300, 1.0, 1),
						T(1, 1600, 1.0, 1),
						T(1, 1800, 1.0, 1)
					}
				}
			},
			PlayoffShare = 0.0,
			MaxPlayoffTeams = 0,
			RoundsFromFinal = new PlayoffRoundFormat[4]
			{
				new PlayoffRoundFormat
				{
					Name = "Финал кубка",
					FixedDate = (int y) => GameClock.Last(y + 1, 5, DayOfWeek.Saturday),
					StartDays = Days(DayOfWeek.Saturday),
					RestDays = 6,
					Times = (DayOfWeek d) => new TimeOption[1] { T(2000, 1.0) }
				},
				new PlayoffRoundFormat
				{
					Name = "Финал конференции",
					StartDays = Days(DayOfWeek.Saturday, DayOfWeek.Sunday),
					RestDays = 5,
					Times = mlsPlayoffTimes
				},
				new PlayoffRoundFormat
				{
					Name = "Полуфинал конференции",
					StartDays = Days(DayOfWeek.Saturday, DayOfWeek.Sunday),
					RestDays = 5,
					Times = mlsPlayoffTimes
				},
				new PlayoffRoundFormat
				{
					Name = "Первый раунд",
					BestOf = 3,
					DayOffsets = new int[3] { 0, 5, 9 },
					HighSeedHome = new bool[3] { true, false, true },
					StartDays = Days(DayOfWeek.Friday, DayOfWeek.Saturday),
					RestDays = 4,
					Times = mlsPlayoffTimes
				}
			}
		});
		list.Add(new SportProfile
		{
			Sport = Sport.Soccer,
			Level = Level.Minor,
			Summary = "30 туров: июль–декабрь, январский отпуск, возобновление в феврале и завершение в мае. Первые три клуба выходят в РПЛ напрямую.",
			GamesPerTeam = 30,
			Weekly = true,
			Start = (int y) => GameClock.D(y, 7, 1),
			End = (int y) => GameClock.D(y + 1, 5, 31),
			Breaks = (int y) => new DateRange[1]
			{
				new DateRange(GameClock.D(y + 1, 1, 1), GameClock.D(y + 1, 1, 31))
			},
			Week = new SlotTemplate[2]
			{
				new SlotTemplate
				{
					Anchor = DayOfWeek.Tuesday,
					Optional = true,
					Times = (DayOfWeek d, bool last) => new TimeOption[2]
					{
						T(1945, 4.0),
						T(1900, 1.0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Saturday,
					Times = (DayOfWeek d, bool last) => new TimeOption[5]
					{
						T(0, 1500, 6.0, 0),
						T(0, 1730, 2.0, 0),
						T(0, 1930, 1.0, 0),
						T(1, 1300, 1.0, 1),
						T(1, 1530, 1.0, 1)
					}
				}
			},
			PlayoffShare = 0.0,
			MaxPlayoffTeams = 0,
			RoundsFromFinal = new PlayoffRoundFormat[3]
			{
				new PlayoffRoundFormat
				{
					Name = "Финал плей-офф",
					FixedDate = (int y) => GameClock.Last(y + 1, 5, DayOfWeek.Saturday).AddDays(-7.0),
					StartDays = Days(DayOfWeek.Saturday),
					RestDays = 6,
					Times = (DayOfWeek d) => new TimeOption[1] { T(1530, 1.0) }
				},
				new PlayoffRoundFormat
				{
					Name = "Полуфинал плей-офф",
					StartDays = Days(DayOfWeek.Saturday, DayOfWeek.Sunday),
					RestDays = 5,
					Times = mlsPlayoffTimes
				},
				new PlayoffRoundFormat
				{
					Name = "Четвертьфинал плей-офф",
					StartDays = Days(DayOfWeek.Saturday, DayOfWeek.Sunday),
					RestDays = 4,
					Times = mlsPlayoffTimes
				}
			}
		});
		list.Add(new SportProfile
		{
			Sport = Sport.Football,
			Level = Level.College,
			Summary = "12 матчей за 14 недель (две свободные недели), с конца августа до субботы после Дня благодарения. Игры по субботам в 12:00, 15:30 и 19:30, некоторые — вечером в четверг и пятницу. Плей-офф проходит в декабре и январе, финал — вечером в понедельник на нейтральном поле.",
			GamesPerTeam = 12,
			ByesPerTeam = 2,
			Weekly = true,
			Start = (int y) => GameClock.Nth(y, 9, DayOfWeek.Monday, 1).AddDays(-9.0),
			End = (int y) => GameClock.Nth(y, 11, DayOfWeek.Thursday, 4).AddDays(2.0),
			Week = new SlotTemplate[1]
			{
				new SlotTemplate
				{
					Anchor = DayOfWeek.Saturday,
					Times = (DayOfWeek d, bool last) => new TimeOption[7]
					{
						T(-2, 1930, 0.5, 1),
						T(-1, 2000, 0.6, 1),
						T(0, 1200, 4.0, 0),
						T(0, 1530, 4.0, 0),
						T(0, 1900, 2.0, 0),
						T(0, 1930, 2.0, 0),
						T(0, 2230, 0.15, 1)
					}
				}
			},
			PlayoffShare = 0.5,
			MinPlayoffTeams = 2,
			MaxPlayoffTeams = 8,
			ChampionLabel = "Чемпионы страны",
			SingleYearLabel = true,
			RoundsFromFinal = new PlayoffRoundFormat[4]
			{
				new PlayoffRoundFormat
				{
					Name = "Национальный чемпионат",
					Neutral = true,
					FixedDate = (int y) => GameClock.OnOrAfter(GameClock.D(y + 1, 1, 19), DayOfWeek.Monday),
					StartDays = Days(DayOfWeek.Monday),
					RestDays = 7,
					Times = (DayOfWeek d) => new TimeOption[1] { T(1930, 1.0) }
				},
				new PlayoffRoundFormat
				{
					Name = "Полуфинал плей-офф",
					Neutral = true,
					FixedDate = (int y) => GameClock.OnOrAfter(GameClock.D(y + 1, 1, 14), DayOfWeek.Thursday),
					StartDays = Days(DayOfWeek.Thursday),
					RestDays = 7,
					Times = (DayOfWeek d) => new TimeOption[2]
					{
						T(0, 1930, 1.0, 1),
						T(1, 1930, 1.0, 1)
					}
				},
				new PlayoffRoundFormat
				{
					Name = "Четвертьфинал плей-офф",
					Neutral = true,
					FixedDate = (int y) => GameClock.D(y, 12, 31),
					RestDays = 7,
					Times = (DayOfWeek d) => new TimeOption[4]
					{
						T(-1, 1930, 1.0, 1),
						T(0, 1200, 1.0, 1),
						T(0, 1930, 1.0, 1),
						T(1, 1300, 1.0, 1)
					}
				},
				new PlayoffRoundFormat
				{
					Name = "Первый раунд плей-офф",
					FixedDate = (int y) => GameClock.Nth(y, 12, DayOfWeek.Saturday, 3),
					StartDays = Days(DayOfWeek.Saturday),
					RestDays = 7,
					Times = (DayOfWeek d) => new TimeOption[4]
					{
						T(-1, 2000, 1.0, 1),
						T(0, 1200, 1.0, 1),
						T(0, 1530, 1.0, 1),
						T(0, 1930, 1.0, 1)
					}
				}
			}
		});
		list.Add(new SportProfile
		{
			Sport = Sport.Basketball,
			Level = Level.College,
			Summary = "31 матч с начала ноября до начала марта: игры по будням (в 19:00 и 21:00) и по субботам. В марте проводится турнир конференции на нейтральной площадке; квалифицируются все команды, по одному матчу в день.",
			GamesPerTeam = 31,
			Start = (int y) => GameClock.Nth(y, 11, DayOfWeek.Monday, 1),
			End = (int y) => GameClock.OnOrAfter(GameClock.D(y + 1, 3, 1), DayOfWeek.Sunday),
			Breaks = (int y) => new DateRange[2]
			{
				new DateRange(GameClock.Nth(y, 11, DayOfWeek.Thursday, 4), GameClock.Nth(y, 11, DayOfWeek.Thursday, 4)),
				new DateRange(GameClock.D(y, 12, 22), GameClock.D(y, 12, 27))
			},
			Week = new SlotTemplate[3]
			{
				new SlotTemplate
				{
					Anchor = DayOfWeek.Tuesday,
					Times = (DayOfWeek d, bool last) => new TimeOption[7]
					{
						T(0, 1900, 3.0, 0),
						T(0, 2100, 2.0, 0),
						T(1, 1900, 3.0, 0),
						T(1, 2100, 2.0, 0),
						T(2, 1900, 2.0, 0),
						T(2, 2100, 1.0, 0),
						T(1, 1830, 1.0, 0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Saturday,
					Times = (DayOfWeek d, bool last) => new TimeOption[7]
					{
						T(0, 1200, 2.0, 0),
						T(0, 1400, 2.0, 0),
						T(0, 1600, 2.0, 0),
						T(0, 1800, 2.0, 0),
						T(0, 2000, 2.0, 0),
						T(1, 1300, 1.0, 0),
						T(1, 1800, 1.0, 0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Friday,
					Optional = true,
					Times = (DayOfWeek d, bool last) => new TimeOption[2]
					{
						T(1900, 2.0),
						T(2100, 1.0)
					}
				}
			},
			PlayoffShare = 1.0,
			PlayoffPowerOfTwo = false,
			MaxPlayoffTeams = 16,
			ChampionLabel = "Чемпионы турнира конференции",
			RoundsFromFinal = new PlayoffRoundFormat[4]
			{
				new PlayoffRoundFormat
				{
					Name = "Чемпионат",
					Neutral = true,
					RestDays = 0,
					Times = (DayOfWeek d) => new TimeOption[1] { T(1800, 1.0) }
				},
				new PlayoffRoundFormat
				{
					Name = "Полуфинал",
					Neutral = true,
					RestDays = 0,
					Times = (DayOfWeek d) => Spread(1900, 2130)
				},
				new PlayoffRoundFormat
				{
					Name = "Четвертьфинал",
					Neutral = true,
					FixedDate = (int y) => GameClock.OnOrAfter(GameClock.D(y + 1, 3, 10), DayOfWeek.Thursday),
					RestDays = 0,
					Times = (DayOfWeek d) => Spread(1200, 1430, 1900, 2130)
				},
				new PlayoffRoundFormat
				{
					Name = "Первый раунд",
					Neutral = true,
					FixedDate = (int y) => GameClock.OnOrAfter(GameClock.D(y + 1, 3, 9), DayOfWeek.Wednesday),
					RestDays = 2,
					Times = (DayOfWeek d) => Spread(1200, 1430, 1900, 2130)
				}
			}
		});
		list.Add(new SportProfile
		{
			Sport = Sport.Hockey,
			Level = Level.College,
			Summary = "34 матча в сериях по пятницам и субботам против одного соперника, с октября до начала марта, с праздничным перерывом в декабре. Плей-офф конференции: четвертьфиналы до двух побед на площадке университета, затем полуфиналы и финал на нейтральной арене.",
			GamesPerTeam = 34,
			Start = (int y) => GameClock.Nth(y, 10, DayOfWeek.Friday, 1),
			End = (int y) => GameClock.OnOrAfter(GameClock.D(y + 1, 3, 1), DayOfWeek.Saturday),
			Breaks = (int y) => new DateRange[1]
			{
				new DateRange(GameClock.D(y, 12, 15), GameClock.D(y, 12, 27))
			},
			Week = new SlotTemplate[1]
			{
				new SlotTemplate
				{
					Anchor = DayOfWeek.Friday,
					Length = 2,
					HomeAndHomeChance = 0.3,
					Times = (DayOfWeek d, bool last) => (d == DayOfWeek.Friday) ? new TimeOption[3]
					{
						T(1900, 5.0),
						T(1930, 2.0),
						T(1800, 1.0)
					} : new TimeOption[4]
					{
						T(1900, 4.0),
						T(1600, 1.0),
						T(1800, 1.0),
						T(1700, 1.0)
					}
				}
			},
			PlayoffShare = 1.0,
			MaxPlayoffTeams = 8,
			ChampionLabel = "Чемпионы конференции",
			RoundsFromFinal = new PlayoffRoundFormat[4]
			{
				new PlayoffRoundFormat
				{
					Name = "Чемпионат",
					Neutral = true,
					StartDays = Days(DayOfWeek.Saturday),
					RestDays = 0,
					Times = (DayOfWeek d) => new TimeOption[1] { T(1900, 1.0) }
				},
				new PlayoffRoundFormat
				{
					Name = "Полуфинал",
					Neutral = true,
					StartDays = Days(DayOfWeek.Friday),
					RestDays = 4,
					Times = (DayOfWeek d) => Spread(1600, 1930)
				},
				new PlayoffRoundFormat
				{
					Name = "Четвертьфинал",
					BestOf = 3,
					DayOffsets = new int[3] { 0, 1, 2 },
					HighSeedHome = new bool[3] { true, true, true },
					StartDays = Days(DayOfWeek.Friday),
					RestDays = 4,
					Times = (DayOfWeek d) => new TimeOption[2]
					{
						T(1900, 3.0),
						T(1600, 1.0)
					}
				},
				new PlayoffRoundFormat
				{
					Name = "Первый раунд",
					BestOf = 3,
					DayOffsets = new int[3] { 0, 1, 2 },
					HighSeedHome = new bool[3] { true, true, true },
					StartDays = Days(DayOfWeek.Friday),
					RestDays = 4,
					Times = (DayOfWeek d) => new TimeOption[2]
					{
						T(1900, 3.0),
						T(1600, 1.0)
					}
				}
			}
		});
		list.Add(new SportProfile
		{
			Sport = Sport.Baseball,
			Level = Level.College,
			Summary = "56 матчей с середины февраля до середины мая: серии по пятницам, субботам и воскресеньям, а также игры среди недели. В начале июня проходят суперрегиональные серии до двух побед на площадках университетов, в конце июня — финальная серия до двух побед на нейтральном поле.",
			GamesPerTeam = 56,
			Start = (int y) => GameClock.OnOrAfter(GameClock.D(y, 2, 13), DayOfWeek.Friday),
			End = (int y) => GameClock.OnOrAfter(GameClock.D(y, 5, 14), DayOfWeek.Saturday).AddDays(1.0),
			Week = new SlotTemplate[3]
			{
				new SlotTemplate
				{
					Anchor = DayOfWeek.Tuesday,
					Times = (DayOfWeek d, bool last) => new TimeOption[3]
					{
						T(1800, 3.0),
						T(1830, 2.0),
						T(1600, 1.0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Wednesday,
					Optional = true,
					Times = (DayOfWeek d, bool last) => new TimeOption[2]
					{
						T(1800, 3.0),
						T(1830, 2.0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Friday,
					Length = 3,
					Times = (DayOfWeek d, bool last) => d switch
					{
						DayOfWeek.Saturday => new TimeOption[4]
						{
							T(1400, 3.0),
							T(1300, 2.0),
							T(1600, 1.0),
							T(1800, 1.0)
						}, 
						DayOfWeek.Friday => new TimeOption[3]
						{
							T(1830, 3.0),
							T(1800, 2.0),
							T(1900, 1.0)
						}, 
						_ => new TimeOption[3]
						{
							T(1300, 4.0),
							T(1200, 2.0),
							T(1400, 1.0)
						}, 
					}
				}
			},
			PlayoffShare = 0.5,
			MaxPlayoffTeams = 8,
			ChampionLabel = "Чемпионы страны",
			RoundsFromFinal = new PlayoffRoundFormat[3]
			{
				new PlayoffRoundFormat
				{
					Name = "Финал студенческой мировой серии",
					BestOf = 3,
					DayOffsets = new int[3] { 0, 1, 2 },
					HighSeedHome = new bool[3] { true, false, true },
					Neutral = true,
					FixedDate = (int y) => GameClock.OnOrAfter(GameClock.D(y, 6, 19), DayOfWeek.Saturday),
					RestDays = 5,
					Times = (DayOfWeek d) => (d == DayOfWeek.Sunday) ? new TimeOption[1] { T(1400, 1.0) } : new TimeOption[1] { T(1900, 1.0) }
				},
				new PlayoffRoundFormat
				{
					Name = "Суперрегиональный этап",
					BestOf = 3,
					DayOffsets = new int[3] { 0, 1, 2 },
					HighSeedHome = new bool[3] { true, true, true },
					FixedDate = (int y) => GameClock.OnOrAfter(GameClock.D(y, 6, 5), DayOfWeek.Friday),
					RestDays = 4,
					Times = (DayOfWeek d) => new TimeOption[4]
					{
						T(1200, 1.0),
						T(1500, 1.0),
						T(1800, 1.0),
						T(2100, 1.0)
					}
				},
				new PlayoffRoundFormat
				{
					Name = "Региональный финал",
					BestOf = 3,
					DayOffsets = new int[3] { 0, 1, 2 },
					HighSeedHome = new bool[3] { true, true, true },
					FixedDate = (int y) => GameClock.OnOrAfter(GameClock.D(y, 5, 29), DayOfWeek.Friday),
					RestDays = 4,
					Times = (DayOfWeek d) => new TimeOption[4]
					{
						T(1200, 1.0),
						T(1500, 1.0),
						T(1800, 1.0),
						T(2100, 1.0)
					}
				}
			}
		});
		list.Add(new SportProfile
		{
			Sport = Sport.Soccer,
			Level = Level.College,
			Summary = "18 матчей с конца августа до середины ноября, по вечерам в четверг или пятницу и днём в воскресенье. Раунды российского студенческого первенства проходят на площадках университетов, затем в декабре проводится студенческий кубок на нейтральном поле (полуфиналы в пятницу, финал в понедельник).",
			GamesPerTeam = 18,
			Start = (int y) => GameClock.OnOrAfter(GameClock.D(y, 8, 20), DayOfWeek.Thursday),
			End = (int y) => GameClock.OnOrAfter(GameClock.D(y, 11, 7), DayOfWeek.Saturday).AddDays(1.0),
			Week = new SlotTemplate[2]
			{
				new SlotTemplate
				{
					Anchor = DayOfWeek.Thursday,
					Times = (DayOfWeek d, bool last) => new TimeOption[4]
					{
						T(0, 1900, 2.0, 0),
						T(1, 1900, 3.0, 0),
						T(1, 1800, 1.0, 0),
						T(1, 1930, 1.0, 0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Sunday,
					Times = (DayOfWeek d, bool last) => new TimeOption[4]
					{
						T(0, 1300, 3.0, 0),
						T(0, 1200, 1.0, 0),
						T(0, 1400, 1.0, 0),
						T(0, 1700, 1.0, 0)
					}
				}
			},
			PlayoffShare = 0.5,
			MaxPlayoffTeams = 8,
			ChampionLabel = "Чемпионы страны",
			RoundsFromFinal = new PlayoffRoundFormat[3]
			{
				new PlayoffRoundFormat
				{
					Name = "Финал студенческого кубка",
					Neutral = true,
					StartDays = Days(DayOfWeek.Monday),
					RestDays = 2,
					Times = (DayOfWeek d) => new TimeOption[1] { T(1800, 1.0) }
				},
				new PlayoffRoundFormat
				{
					Name = "Полуфинал студенческого кубка",
					Neutral = true,
					FixedDate = (int y) => GameClock.OnOrAfter(GameClock.D(y, 12, 8), DayOfWeek.Friday),
					StartDays = Days(DayOfWeek.Friday),
					RestDays = 4,
					Times = (DayOfWeek d) => Spread(1700, 1930)
				},
				new PlayoffRoundFormat
				{
					Name = "Четвертьфинал студенческого первенства",
					StartDays = Days(DayOfWeek.Saturday, DayOfWeek.Sunday),
					RestDays = 3,
					Times = (DayOfWeek d) => new TimeOption[4]
					{
						T(1200, 1.0),
						T(1300, 1.0),
						T(1500, 1.0),
						T(1900, 1.0)
					}
				}
			}
		});
		list.Add(new SportProfile
		{
			Sport = Sport.Football,
			Level = Level.HighSchool,
			Summary = "10 матчей по пятницам вечером (одна свободная неделя), с конца августа до конца октября. Плей-офф штата проходит по пятницам на поле команды с более высоким посевом, финал штата — на нейтральном стадионе в середине декабря.",
			GamesPerTeam = 10,
			ByesPerTeam = 1,
			Weekly = true,
			Start = (int y) => GameClock.OnOrAfter(GameClock.D(y, 8, 20), DayOfWeek.Friday),
			End = (int y) => GameClock.OnOrAfter(GameClock.D(y, 8, 20), DayOfWeek.Friday).AddDays(70.0),
			Week = new SlotTemplate[1]
			{
				new SlotTemplate
				{
					Anchor = DayOfWeek.Friday,
					Times = (DayOfWeek d, bool last) => new TimeOption[4]
					{
						T(0, 1930, 9.0, 0),
						T(0, 1900, 2.0, 0),
						T(-1, 1930, 0.5, 1),
						T(1, 1300, 0.3, 1)
					}
				}
			},
			PlayoffShare = 0.5,
			MaxPlayoffTeams = 32,
			PlayoffsBackwardFromFinal = true,
			ChampionLabel = "Чемпионы штата",
			RoundsFromFinal = new PlayoffRoundFormat[5]
			{
				new PlayoffRoundFormat
				{
					Name = "Чемпионат штата",
					Neutral = true,
					FixedDate = (int y) => GameClock.OnOrAfter(GameClock.D(y, 12, 14), DayOfWeek.Monday),
					RestDays = 7,
					Times = (DayOfWeek d) => new TimeOption[3]
					{
						T(1200, 1.0),
						T(1530, 1.0),
						T(1900, 1.0)
					}
				},
				new PlayoffRoundFormat
				{
					Name = "Полуфинал",
					StartDays = Days(DayOfWeek.Friday),
					RestDays = 6,
					Times = (DayOfWeek d) => new TimeOption[1] { T(1930, 1.0) }
				},
				new PlayoffRoundFormat
				{
					Name = "Четвертьфинал",
					StartDays = Days(DayOfWeek.Friday),
					RestDays = 6,
					Times = (DayOfWeek d) => new TimeOption[1] { T(1930, 1.0) }
				},
				new PlayoffRoundFormat
				{
					Name = "Второй раунд",
					StartDays = Days(DayOfWeek.Friday),
					RestDays = 6,
					Times = (DayOfWeek d) => new TimeOption[1] { T(1930, 1.0) }
				},
				new PlayoffRoundFormat
				{
					Name = "Первый раунд",
					StartDays = Days(DayOfWeek.Friday),
					RestDays = 6,
					Times = (DayOfWeek d) => new TimeOption[1] { T(1930, 1.0) }
				}
			}
		});
		list.Add(new SportProfile
		{
			Sport = Sport.Basketball,
			Level = Level.HighSchool,
			Summary = "25 матчей с середины ноября до начала февраля, по вечерам во вторник и пятницу, а также отдельные субботние турниры. Плей-офф штата проходит в конце февраля на площадке команды с более высоким посевом, полуфиналы и финал в марте — на нейтральной площадке.",
			GamesPerTeam = 25,
			Start = (int y) => GameClock.OnOrAfter(GameClock.D(y, 11, 16), DayOfWeek.Tuesday),
			End = (int y) => GameClock.OnOrAfter(GameClock.D(y + 1, 2, 6), DayOfWeek.Friday),
			Breaks = (int y) => new DateRange[3]
			{
				new DateRange(GameClock.Nth(y, 11, DayOfWeek.Thursday, 4).AddDays(-1.0), GameClock.Nth(y, 11, DayOfWeek.Thursday, 4).AddDays(3.0)),
				new DateRange(GameClock.D(y, 12, 23), GameClock.D(y, 12, 26)),
				new DateRange(GameClock.D(y + 1, 1, 1), GameClock.D(y + 1, 1, 1))
			},
			Week = new SlotTemplate[3]
			{
				new SlotTemplate
				{
					Anchor = DayOfWeek.Tuesday,
					Times = (DayOfWeek d, bool last) => new TimeOption[3]
					{
						T(1930, 5.0),
						T(1800, 2.0),
						T(2000, 1.0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Friday,
					Times = (DayOfWeek d, bool last) => new TimeOption[3]
					{
						T(1930, 5.0),
						T(1800, 2.0),
						T(2000, 1.0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Saturday,
					Optional = true,
					Times = (DayOfWeek d, bool last) => new TimeOption[3]
					{
						T(1400, 2.0),
						T(1800, 2.0),
						T(1930, 1.0)
					}
				}
			},
			PlayoffShare = 0.5,
			MaxPlayoffTeams = 32,
			PlayoffsBackwardFromFinal = true,
			ChampionLabel = "Чемпионы штата",
			RoundsFromFinal = new PlayoffRoundFormat[5]
			{
				new PlayoffRoundFormat
				{
					Name = "Чемпионат штата",
					Neutral = true,
					FixedDate = (int y) => GameClock.OnOrAfter(GameClock.D(y + 1, 3, 5), DayOfWeek.Saturday),
					RestDays = 2,
					Times = (DayOfWeek d) => new TimeOption[4]
					{
						T(1200, 1.0),
						T(1400, 1.0),
						T(1800, 1.0),
						T(2000, 1.0)
					}
				},
				new PlayoffRoundFormat
				{
					Name = "Полуфинал",
					Neutral = true,
					StartDays = Days(DayOfWeek.Tuesday, DayOfWeek.Wednesday),
					RestDays = 2,
					Times = (DayOfWeek d) => Spread(1800, 2000)
				},
				new PlayoffRoundFormat
				{
					Name = "Четвертьфинал",
					StartDays = Days(DayOfWeek.Friday, DayOfWeek.Saturday),
					RestDays = 2,
					Times = (DayOfWeek d) => new TimeOption[1] { T(1930, 1.0) }
				},
				new PlayoffRoundFormat
				{
					Name = "Второй раунд",
					StartDays = Days(DayOfWeek.Tuesday, DayOfWeek.Wednesday),
					RestDays = 2,
					Times = (DayOfWeek d) => new TimeOption[1] { T(1930, 1.0) }
				},
				new PlayoffRoundFormat
				{
					Name = "Первый раунд",
					StartDays = Days(DayOfWeek.Friday, DayOfWeek.Saturday),
					RestDays = 2,
					Times = (DayOfWeek d) => new TimeOption[1] { T(1930, 1.0) }
				}
			}
		});
		Func<DayOfWeek, TimeOption[]> hsBaseballPlayoffTimes = (DayOfWeek d) => Weekend(d) ? new TimeOption[1] { T(1200, 1.0) } : new TimeOption[1] { T(1700, 1.0) };
		list.Add(new SportProfile
		{
			Sport = Sport.Baseball,
			Level = Level.HighSchool,
			Summary = "25 матчей с середины февраля до середины апреля, игры по будням в 17:30 и отдельные матчи по субботам, весенний перерыв в начале апреля. Каждый раунд плей-офф штата проводится до двух побед (два матча в один день и третий при необходимости), финал — в конце мая на нейтральном поле.",
			GamesPerTeam = 25,
			Start = (int y) => GameClock.OnOrAfter(GameClock.D(y, 2, 10), DayOfWeek.Tuesday),
			End = (int y) => GameClock.OnOrAfter(GameClock.D(y, 4, 11), DayOfWeek.Saturday),
			Breaks = (int y) => new DateRange[1]
			{
				new DateRange(GameClock.OnOrAfter(GameClock.D(y, 4, 1), DayOfWeek.Monday), GameClock.OnOrAfter(GameClock.D(y, 4, 1), DayOfWeek.Monday).AddDays(4.0))
			},
			Week = new SlotTemplate[4]
			{
				new SlotTemplate
				{
					Anchor = DayOfWeek.Tuesday,
					Times = (DayOfWeek d, bool last) => new TimeOption[4]
					{
						T(1730, 2.0),
						T(1755, 2.0),
						T(1800, 1.0),
						T(1630, 1.0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Friday,
					Times = (DayOfWeek d, bool last) => new TimeOption[3]
					{
						T(1730, 2.0),
						T(1900, 2.0),
						T(1800, 1.0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Saturday,
					Optional = true,
					Times = (DayOfWeek d, bool last) => new TimeOption[3]
					{
						T(1100, 1.0),
						T(1200, 2.0),
						T(1400, 1.0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Wednesday,
					Optional = true,
					Times = (DayOfWeek d, bool last) => new TimeOption[2]
					{
						T(1730, 2.0),
						T(1800, 1.0)
					}
				}
			},
			PlayoffShare = 0.5,
			MaxPlayoffTeams = 32,
			PlayoffsBackwardFromFinal = true,
			ChampionLabel = "Чемпионы штата",
			RoundsFromFinal = new PlayoffRoundFormat[5]
			{
				new PlayoffRoundFormat
				{
					Name = "Серия чемпионата штата",
					BestOf = 3,
					DayOffsets = new int[3] { 0, 0, 1 },
					HighSeedHome = new bool[3] { true, false, true },
					Neutral = true,
					FixedDate = (int y) => GameClock.OnOrAfter(GameClock.D(y, 5, 20), DayOfWeek.Friday),
					RestDays = 3,
					Times = (DayOfWeek d) => (d == DayOfWeek.Saturday) ? new TimeOption[1] { T(1200, 1.0) } : new TimeOption[1] { T(1100, 1.0) }
				},
				HsBaseballSeries("Полуфинал", Days(DayOfWeek.Wednesday, DayOfWeek.Thursday)),
				HsBaseballSeries("Четвертьфинал", Days(DayOfWeek.Wednesday, DayOfWeek.Thursday)),
				HsBaseballSeries("Второй раунд", Days(DayOfWeek.Wednesday, DayOfWeek.Thursday)),
				HsBaseballSeries("Первый раунд", Days(DayOfWeek.Wednesday, DayOfWeek.Thursday))
			}
		});
		list.Add(new SportProfile
		{
			Sport = Sport.Soccer,
			Level = Level.HighSchool,
			Summary = "16 матчей весной, с конца января до середины апреля, по вечерам во вторник и пятницу. Плей-офф штата проходит на поле команды с более высоким посевом, финал в мае — на нейтральном стадионе.",
			GamesPerTeam = 16,
			Start = (int y) => GameClock.OnOrAfter(GameClock.D(y, 1, 27), DayOfWeek.Tuesday),
			End = (int y) => GameClock.OnOrAfter(GameClock.D(y, 4, 10), DayOfWeek.Friday),
			Breaks = (int y) => new DateRange[1]
			{
				new DateRange(GameClock.OnOrAfter(GameClock.D(y, 4, 1), DayOfWeek.Monday), GameClock.OnOrAfter(GameClock.D(y, 4, 1), DayOfWeek.Monday).AddDays(4.0))
			},
			Week = new SlotTemplate[2]
			{
				new SlotTemplate
				{
					Anchor = DayOfWeek.Tuesday,
					Times = (DayOfWeek d, bool last) => new TimeOption[3]
					{
						T(1930, 3.0),
						T(1730, 2.0),
						T(1800, 1.0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Friday,
					Times = (DayOfWeek d, bool last) => new TimeOption[2]
					{
						T(1930, 3.0),
						T(1730, 2.0)
					}
				}
			},
			PlayoffShare = 0.5,
			MaxPlayoffTeams = 32,
			PlayoffsBackwardFromFinal = true,
			ChampionLabel = "Чемпионы штата",
			RoundsFromFinal = new PlayoffRoundFormat[5]
			{
				new PlayoffRoundFormat
				{
					Name = "Чемпионат штата",
					Neutral = true,
					FixedDate = (int y) => GameClock.OnOrAfter(GameClock.D(y, 5, 7), DayOfWeek.Friday),
					RestDays = 5,
					Times = (DayOfWeek d) => new TimeOption[2]
					{
						T(1730, 1.0),
						T(2000, 1.0)
					}
				},
				new PlayoffRoundFormat
				{
					Name = "Полуфинал",
					StartDays = Days(DayOfWeek.Tuesday, DayOfWeek.Friday),
					RestDays = 3,
					Times = (DayOfWeek d) => new TimeOption[1] { T(1930, 1.0) }
				},
				new PlayoffRoundFormat
				{
					Name = "Четвертьфинал",
					StartDays = Days(DayOfWeek.Tuesday, DayOfWeek.Friday),
					RestDays = 3,
					Times = (DayOfWeek d) => new TimeOption[1] { T(1930, 1.0) }
				},
				new PlayoffRoundFormat
				{
					Name = "Второй раунд",
					StartDays = Days(DayOfWeek.Tuesday, DayOfWeek.Friday),
					RestDays = 3,
					Times = (DayOfWeek d) => new TimeOption[1] { T(1930, 1.0) }
				},
				new PlayoffRoundFormat
				{
					Name = "Первый раунд",
					StartDays = Days(DayOfWeek.Tuesday, DayOfWeek.Friday),
					RestDays = 3,
					Times = (DayOfWeek d) => new TimeOption[1] { T(1930, 1.0) }
				}
			}
		});
		list.Add(new SportProfile
		{
			Sport = Sport.Hockey,
			Level = Level.HighSchool,
			Summary = "25 матчей с конца ноября до середины февраля, по вечерам во вторник и четверг и днём в субботу. Плей-офф секций проходит в конце февраля на площадке команды с более высоким посевом, затем в марте проводится турнир штата на большой арене.",
			GamesPerTeam = 25,
			Start = (int y) => GameClock.OnOrAfter(GameClock.D(y, 11, 24), DayOfWeek.Tuesday),
			End = (int y) => GameClock.OnOrAfter(GameClock.D(y + 1, 2, 12), DayOfWeek.Saturday),
			Breaks = (int y) => new DateRange[2]
			{
				new DateRange(GameClock.D(y, 12, 24), GameClock.D(y, 12, 25)),
				new DateRange(GameClock.D(y, 12, 31), GameClock.D(y + 1, 1, 1))
			},
			Week = new SlotTemplate[3]
			{
				new SlotTemplate
				{
					Anchor = DayOfWeek.Tuesday,
					Times = (DayOfWeek d, bool last) => new TimeOption[2]
					{
						T(1900, 3.0),
						T(1915, 1.0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Thursday,
					Optional = true,
					Times = (DayOfWeek d, bool last) => new TimeOption[2]
					{
						T(1900, 3.0),
						T(1915, 1.0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Saturday,
					Times = (DayOfWeek d, bool last) => new TimeOption[3]
					{
						T(1400, 1.0),
						T(1500, 1.0),
						T(1900, 2.0)
					}
				}
			},
			PlayoffShare = 0.5,
			MaxPlayoffTeams = 16,
			PlayoffsBackwardFromFinal = true,
			ChampionLabel = "Чемпионы штата",
			RoundsFromFinal = new PlayoffRoundFormat[5]
			{
				new PlayoffRoundFormat
				{
					Name = "Чемпионат штата",
					Neutral = true,
					FixedDate = (int y) => GameClock.OnOrAfter(GameClock.D(y + 1, 3, 6), DayOfWeek.Saturday),
					RestDays = 0,
					Times = (DayOfWeek d) => new TimeOption[1] { T(2000, 1.0) }
				},
				new PlayoffRoundFormat
				{
					Name = "Полуфиналы штата",
					Neutral = true,
					StartDays = Days(DayOfWeek.Friday),
					RestDays = 0,
					Times = (DayOfWeek d) => Spread(1100, 1300, 1800, 2000)
				},
				new PlayoffRoundFormat
				{
					Name = "Четвертьфиналы штата",
					Neutral = true,
					StartDays = Days(DayOfWeek.Wednesday, DayOfWeek.Thursday),
					RestDays = 4,
					Times = (DayOfWeek d) => Spread(1100, 1300, 1800, 2000)
				},
				new PlayoffRoundFormat
				{
					Name = "Финал секции",
					StartDays = Days(DayOfWeek.Thursday, DayOfWeek.Friday),
					RestDays = 2,
					Times = (DayOfWeek d) => new TimeOption[1] { T(1900, 1.0) }
				},
				new PlayoffRoundFormat
				{
					Name = "Полуфинал секции",
					StartDays = Days(DayOfWeek.Tuesday, DayOfWeek.Wednesday),
					RestDays = 2,
					Times = (DayOfWeek d) => new TimeOption[1] { T(1900, 1.0) }
				}
			}
		});
		list.Add(new SportProfile
		{
			Sport = Sport.Hockey,
			Level = Level.Junior,
			Summary = "62 матча с конца сентября до начала апреля, преимущественно по пятницам и субботам в 19:05, также отдельные игры по воскресеньям и среди недели. Большинство команд выходят в плей-офф: первый раунд — до двух побед (лучшие команды получают пропуск), затем серии до трёх побед за юниорский кубок в мае.",
			GamesPerTeam = 62,
			Start = (int y) => GameClock.Last(y, 9, DayOfWeek.Friday),
			End = (int y) => GameClock.Nth(y + 1, 4, DayOfWeek.Saturday, 1),
			Breaks = (int y) => new DateRange[1]
			{
				new DateRange(GameClock.D(y, 12, 21), GameClock.D(y, 12, 28))
			},
			Week = new SlotTemplate[3]
			{
				new SlotTemplate
				{
					Anchor = DayOfWeek.Friday,
					Length = 2,
					HomeAndHomeChance = 0.35,
					Times = (DayOfWeek d, bool last) => (d == DayOfWeek.Friday) ? new TimeOption[3]
					{
						T(1905, 5.0),
						T(1835, 1.0),
						T(1930, 1.0)
					} : new TimeOption[3]
					{
						T(1905, 5.0),
						T(1805, 1.0),
						T(1600, 1.0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Sunday,
					Optional = true,
					Times = (DayOfWeek d, bool last) => new TimeOption[2]
					{
						T(1505, 2.0),
						T(1705, 1.0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Wednesday,
					Optional = true,
					Times = (DayOfWeek d, bool last) => new TimeOption[2]
					{
						T(1905, 2.0),
						T(1835, 1.0)
					}
				}
			},
			PlayoffShare = 0.75,
			PlayoffPowerOfTwo = false,
			MaxPlayoffTeams = 12,
			ChampionLabel = "Обладатели юниорского кубка",
			RoundsFromFinal = new PlayoffRoundFormat[4]
			{
				JuniorSeries("Финал юниорского кубка", 3),
				JuniorSeries("Финал конференции", 2),
				JuniorSeries("Полуфинал конференции", 2),
				new PlayoffRoundFormat
				{
					Name = "Первый раунд",
					BestOf = 3,
					DayOffsets = new int[3] { 0, 1, 2 },
					HighSeedHome = new bool[3] { true, true, true },
					StartDays = Days(DayOfWeek.Friday),
					RestDays = 5,
					Times = (DayOfWeek d) => new TimeOption[1] { T(1905, 1.0) }
				}
			}
		});
		Func<DayOfWeek, TimeOption[]> minorHockeyPlayoffTimes = (DayOfWeek d) => Weekend(d) ? new TimeOption[3]
		{
			T(1500, 1.0),
			T(1700, 2.0),
			T(1900, 3.0)
		} : new TimeOption[2]
		{
			T(1900, 4.0),
			T(1905, 2.0)
		};
		list.Add(new SportProfile
		{
			Sport = Sport.Hockey,
			Level = Level.Minor,
			Summary = "72 матча с середины октября до середины апреля, преимущественно по пятницам, субботам и воскресеньям, также отдельные игры среди недели и утром в учебные дни. Большинство команд выходят в плей-офф: первый раунд — до двух побед (лучшие команды пропускают его), раунды дивизионов — до трёх побед, финальная серия за кубок в июне — до четырёх.",
			GamesPerTeam = 72,
			Start = (int y) => GameClock.OnOrAfter(GameClock.D(y, 10, 9), DayOfWeek.Friday),
			End = (int y) => GameClock.OnOrAfter(GameClock.D(y + 1, 4, 13), DayOfWeek.Sunday),
			Breaks = (int y) => new DateRange[2]
			{
				new DateRange(GameClock.D(y, 12, 23), GameClock.D(y, 12, 26)),
				new DateRange(GameClock.Nth(y + 1, 2, DayOfWeek.Sunday, 1).AddDays(-1.0), GameClock.Nth(y + 1, 2, DayOfWeek.Sunday, 1).AddDays(2.0))
			},
			Week = new SlotTemplate[4]
			{
				new SlotTemplate
				{
					Anchor = DayOfWeek.Friday,
					Times = (DayOfWeek d, bool last) => new TimeOption[3]
					{
						T(1900, 5.0),
						T(1905, 2.0),
						T(1930, 1.0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Saturday,
					Times = (DayOfWeek d, bool last) => new TimeOption[4]
					{
						T(1900, 4.0),
						T(1800, 2.0),
						T(1700, 1.0),
						T(1905, 1.0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Sunday,
					Optional = true,
					Times = (DayOfWeek d, bool last) => new TimeOption[3]
					{
						T(1500, 3.0),
						T(1700, 2.0),
						T(1400, 1.0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Wednesday,
					Optional = true,
					Times = (DayOfWeek d, bool last) => new TimeOption[3]
					{
						T(1900, 4.0),
						T(1030, 1.0),
						T(1100, 1.0)
					}
				}
			},
			PlayoffShare = 0.72,
			PlayoffPowerOfTwo = false,
			MaxPlayoffTeams = 16,
			ChampionLabel = "Обладатели кубка",
			RoundsFromFinal = new PlayoffRoundFormat[5]
			{
				Series7("Финалы кубка", minorHockeyPlayoffTimes, 2),
				Series7("Финалы конференций", minorHockeyPlayoffTimes, 2),
				MinorSeries("Финал дивизиона", minorHockeyPlayoffTimes, 2),
				MinorSeries("Полуфинал дивизиона", minorHockeyPlayoffTimes, 2),
				new PlayoffRoundFormat
				{
					Name = "Первый раунд",
					BestOf = 3,
					DayOffsets = new int[3] { 0, 1, 2 },
					HighSeedHome = new bool[3] { false, true, true },
					RestDays = 4,
					Times = minorHockeyPlayoffTimes
				}
			}
		});
		list.Add(new SportProfile
		{
			Sport = Sport.Basketball,
			Level = Level.Minor,
			Summary = "50 матчей с начала ноября до конца марта (с перерывом на зимний турнир примерно на Рождество), начало большинства игр в 19:00, отдельные матчи в 11:00 в учебные дни. В плей-офф выходят лучшие команды половины таблицы: до финалов конференций играется один матч, финал в апреле — до двух побед.",
			GamesPerTeam = 50,
			Start = (int y) => GameClock.OnOrAfter(GameClock.D(y, 11, 6), DayOfWeek.Friday),
			End = (int y) => GameClock.Last(y + 1, 3, DayOfWeek.Saturday),
			Breaks = (int y) => new DateRange[3]
			{
				new DateRange(GameClock.Nth(y, 11, DayOfWeek.Thursday, 4), GameClock.Nth(y, 11, DayOfWeek.Thursday, 4)),
				new DateRange(GameClock.D(y, 12, 19), GameClock.D(y, 12, 26)),
				new DateRange(GameClock.Nth(y + 1, 2, DayOfWeek.Sunday, 3).AddDays(-2.0), GameClock.Nth(y + 1, 2, DayOfWeek.Sunday, 3).AddDays(1.0))
			},
			Week = new SlotTemplate[3]
			{
				new SlotTemplate
				{
					Anchor = DayOfWeek.Tuesday,
					Times = (DayOfWeek d, bool last) => new TimeOption[4]
					{
						T(0, 1900, 4.0, 0),
						T(0, 1100, 1.0, 0),
						T(1, 1900, 3.0, 0),
						T(1, 1930, 1.0, 0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Friday,
					Times = (DayOfWeek d, bool last) => new TimeOption[4]
					{
						T(0, 1900, 4.0, 0),
						T(0, 1930, 1.0, 0),
						T(1, 1900, 3.0, 0),
						T(1, 1700, 1.0, 0)
					}
				},
				new SlotTemplate
				{
					Anchor = DayOfWeek.Sunday,
					Optional = true,
					Times = (DayOfWeek d, bool last) => new TimeOption[3]
					{
						T(1500, 2.0),
						T(1700, 1.0),
						T(1300, 1.0)
					}
				}
			},
			PlayoffShare = 0.5,
			MaxPlayoffTeams = 8,
			ChampionLabel = "Чемпионы",
			RoundsFromFinal = new PlayoffRoundFormat[3]
			{
				new PlayoffRoundFormat
				{
					Name = "Финал",
					BestOf = 3,
					DayOffsets = new int[3] { 0, 2, 4 },
					HighSeedHome = new bool[3] { false, true, true },
					RestDays = 3,
					Times = (DayOfWeek d) => new TimeOption[2]
					{
						T(1900, 2.0),
						T(1930, 1.0)
					}
				},
				new PlayoffRoundFormat
				{
					Name = "Финалы конференций",
					RestDays = 2,
					Times = (DayOfWeek d) => new TimeOption[2]
					{
						T(1900, 2.0),
						T(1930, 1.0)
					}
				},
				new PlayoffRoundFormat
				{
					Name = "Полуфиналы конференций",
					RestDays = 2,
					Times = (DayOfWeek d) => new TimeOption[2]
					{
						T(1900, 2.0),
						T(1930, 1.0)
					}
				}
			}
		});
		Func<DayOfWeek, bool, TimeOption[]> minorBaseballTimes = (DayOfWeek d, bool last) => d switch
		{
			DayOfWeek.Sunday => new TimeOption[3]
			{
				T(1305, 3.0),
				T(1335, 1.0),
				T(1405, 1.0)
			}, 
			DayOfWeek.Saturday => new TimeOption[4]
			{
				T(1805, 2.0),
				T(1835, 1.0),
				T(1905, 2.0),
				T(1605, 1.0)
			}, 
			DayOfWeek.Wednesday => new TimeOption[4]
			{
				T(1835, 2.0),
				T(1905, 3.0),
				T(1105, 1.0),
				T(1205, 1.0)
			}, 
			_ => new TimeOption[3]
			{
				T(1835, 2.0),
				T(1905, 4.0),
				T(1835, 1.0)
			}, 
		};
		list.Add(new SportProfile
		{
			Sport = Sport.Baseball,
			Level = Level.Minor,
			Summary = "150 матчей с конца марта до конца сентября, серии по шесть игр со вторника по воскресенье, понедельники — выходные (вечерние игры в 19:05, по воскресеньям днём, отдельные матчи в учебные дни в 11:05). Четыре лучшие команды играют полуфиналы до двух побед, затем проводится один финальный матч за чемпионство на нейтральном стадионе.",
			GamesPerTeam = 150,
			Start = (int y) => GameClock.OnOrAfter(GameClock.D(y, 3, 24), DayOfWeek.Tuesday),
			End = (int y) => GameClock.OnOrAfter(GameClock.D(y, 9, 14), DayOfWeek.Sunday),
			Week = new SlotTemplate[1]
			{
				new SlotTemplate
				{
					Anchor = DayOfWeek.Tuesday,
					Length = 6,
					Times = minorBaseballTimes
				}
			},
			PlayoffShare = 0.4,
			PlayoffPowerOfTwo = true,
			MinPlayoffTeams = 2,
			MaxPlayoffTeams = 4,
			ChampionLabel = "Чемпионы",
			RoundsFromFinal = new PlayoffRoundFormat[2]
			{
				new PlayoffRoundFormat
				{
					Name = "Финал чемпионата",
					Neutral = true,
					RestDays = 2,
					Times = (DayOfWeek d) => new TimeOption[1] { T(1905, 1.0) }
				},
				new PlayoffRoundFormat
				{
					Name = "Полуфинал",
					BestOf = 3,
					DayOffsets = new int[3] { 0, 1, 2 },
					HighSeedHome = new bool[3] { true, true, true },
					RestDays = 1,
					Times = (DayOfWeek d) => Weekend(d) ? new TimeOption[2]
					{
						T(1305, 1.0),
						T(1805, 1.0)
					} : new TimeOption[2]
					{
						T(1905, 2.0),
						T(1835, 1.0)
					}
				}
			}
		});
		Dictionary<string, SportProfile> map = new Dictionary<string, SportProfile>();
		foreach (SportProfile p in list)
		{
			map[p.Sport.ToString() + "-" + p.Level] = p;
		}
		Sport[] allSports = SportRules.AllSports;
		for (int num = 0; num < allSports.Length; num++)
		{
			Sport s = allSports[num];
			Level[] allLevels = SportRules.AllLevels;
			for (int num2 = 0; num2 < allLevels.Length; num2++)
			{
				Level l = allLevels[num2];
				if (!map.ContainsKey(s.ToString() + "-" + l))
				{
					map[s.ToString() + "-" + l] = map[s.ToString() + "-" + Level.Pro];
				}
			}
		}
		return map;
		PlayoffRoundFormat HsBaseballSeries(string name, DayOfWeek[] days)
		{
			return new PlayoffRoundFormat
			{
				Name = name,
				BestOf = 3,
				DayOffsets = new int[3] { 0, 0, 1 },
				HighSeedHome = new bool[3] { true, false, true },
				StartDays = days,
				RestDays = 2,
				Times = hsBaseballPlayoffTimes
			};
		}
		static PlayoffRoundFormat JuniorSeries(string name, int rest)
		{
			return new PlayoffRoundFormat
			{
				Name = name,
				BestOf = 5,
				DayOffsets = new int[5] { 0, 1, 4, 5, 7 },
				HighSeedHome = new bool[5] { true, true, false, false, true },
				StartDays = Days(DayOfWeek.Friday, DayOfWeek.Saturday),
				RestDays = rest,
				Times = (DayOfWeek d) => new TimeOption[2]
				{
					T(1905, 3.0),
					T(1835, 1.0)
				}
			};
		}
		static PlayoffRoundFormat MinorSeries(string name, Func<DayOfWeek, TimeOption[]> times, int rest)
		{
			return new PlayoffRoundFormat
			{
				Name = name,
				BestOf = 5,
				DayOffsets = new int[5] { 0, 1, 3, 4, 6 },
				HighSeedHome = new bool[5] { true, true, false, false, true },
				RestDays = rest,
				Times = times,
				Stagger = 1
			};
		}
	}
}
