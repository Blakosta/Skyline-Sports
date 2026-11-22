using System.Collections.Generic;
using Colossal;
using Game.Modding;

namespace Skyline_Sports_Project;

public class LocaleEN : IDictionarySource
{
	private readonly Setting m_Setting;

	public LocaleEN(Setting setting)
	{
		m_Setting = setting;
	}

	public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
	{
		return new Dictionary<string, string>
		{
			{
				((ModSetting)m_Setting).GetSettingsLocaleID(),
				"Спорт России"
			},
			{
				((ModSetting)m_Setting).GetOptionTabLocaleID("Main"),
				"Основное"
			},
			{
				((ModSetting)m_Setting).GetOptionGroupLocaleID("Leagues"),
				"Лиги"
			},
			{
				((ModSetting)m_Setting).GetOptionGroupLocaleID("Display"),
				"Отображение"
			},
			{
				((ModSetting)m_Setting).GetOptionGroupLocaleID("Data"),
				"Данные"
			},
			{
				((ModSetting)m_Setting).GetOptionGroupLocaleID("Fans"),
				"Болельщики"
			},
			{
				((ModSetting)m_Setting).GetOptionGroupLocaleID("Residents"),
				"Реальные спортсмены"
			},
			{
				((ModSetting)m_Setting).GetOptionLabelLocaleID("RealAthleteResidentsEnabled"),
				"Создавать игроков и тренеров жителями города"
			},
			{
				((ModSetting)m_Setting).GetOptionDescLocaleID("RealAthleteResidentsEnabled"),
				"Создаёт настоящих жителей для игроков и тренеров пользовательских футбольных и хоккейных клубов. Домохозяйства и жильё создаются штатными системами игры; начальный достаток определяется встроенными данными домохозяйства без привязки спортивной зарплаты к городской экономике. Работа назначается на стадион через штатную систему рабочих мест. Игроки клубов ИИ не создаются в городе. При отключении новые жители не создаются и занятость на стадионе снимается; уже созданные жители, их дома и автомобили сохраняются."
			},
			{
				((ModSetting)m_Setting).GetOptionLabelLocaleID("RealFans"),
				"Болельщики на главных матчах"
			},
			{
				((ModSetting)m_Setting).GetOptionDescLocaleID("RealFans"),
				"Жители вашего города приходят на каждый главный матч. Примерно за 2,5 часа до начала они отправляются к арене на автомобилях, общественном транспорте или пешком. После матча болельщики возвращаются домой. На странице матча отображается число зрителей на арене. Если функция несовместима с вашей игрой, мод автоматически отключит её и объяснит причину."
			},
			{
				((ModSetting)m_Setting).GetOptionLabelLocaleID("VisitorFans"),
				"Гости из других городов"
			},
			{
				((ModSetting)m_Setting).GetOptionDescLocaleID("VisitorFans"),
				"Если включены реальные болельщики, туристы, остановившиеся в городе, также смогут посещать главные матчи. Они могут увеличить число зрителей до 25%, а после игры вернутся в свои гостиницы."
			},
			{
				((ModSetting)m_Setting).GetOptionLabelLocaleID("GameDayTourism"),
				"Матчи привлекают туристов (экспериментально)"
			},
			{
				((ModSetting)m_Setting).GetOptionDescLocaleID("GameDayTourism"),
				"При включённых реальных болельщиках в день главного матча спортивный объект, являющийся достопримечательностью, временно получает повышенную привлекательность. Это может привлечь дополнительных туристов самолётами, поездами, кораблями и автомобилями. Остальные объекты не затрагиваются. Если игра самостоятельно сбросит изменение, функция отключится, а причина появится в журнале."
			},
			{
				((ModSetting)m_Setting).GetOptionLabelLocaleID("RealFansCount"),
				"Болельщиков на матч"
			},
			{
				((ModSetting)m_Setting).GetOptionDescLocaleID("RealFansCount"),
				"Количество жителей, посещающих каждый главный матч. Большое число зрителей увеличивает транспортную нагрузку и заполняемость парковок возле арены."
			},
			{
				((ModSetting)m_Setting).GetOptionGroupLocaleID("About"),
				"О моде"
			},
			{
				((ModSetting)m_Setting).GetOptionLabelLocaleID("LeagueSize"),
				"Команд в лиге"
			},
			{
				((ModSetting)m_Setting).GetOptionDescLocaleID("LeagueSize"),
				"Количество команд в каждой лиге. Сначала учитываются команды вашего города, остальные места занимают соперники из других городов. Если сезон ещё не начался, изменение применяется сразу. Во время сезона состав сохранится до следующего чемпионата."
			},
			{
				((ModSetting)m_Setting).GetOptionLabelLocaleID("SoccerLeagueSize"),
				"Команд в футбольной лиге"
			},
			{
				((ModSetting)m_Setting).GetOptionDescLocaleID("SoccerLeagueSize"),
				"Количество команд в футбольных лигах (РПЛ, ФНЛ и др.). При 18 командах каждая команда проводит 34 матча: по одному дома и в гостях с каждым соперником. Изменение применяется при создании новой лиги или следующего сезона."
			},
			{
				((ModSetting)m_Setting).GetOptionLabelLocaleID("SeasonGames"),
				"Матчей за сезон"
			},
			{
				((ModSetting)m_Setting).GetOptionDescLocaleID("SeasonGames"),
				"100% соответствует полному календарю сезона: 82 матча в профессиональном баскетболе, 162 в бейсболе и 17 в американском футболе. Уменьшите значение, чтобы сократить число матчей. Изменение вступит в силу со следующего сезона."
			},
			{
				((ModSetting)m_Setting).GetOptionLabelLocaleID("InjuryFrequency"),
				"Частота травм"
			},
			{
				((ModSetting)m_Setting).GetOptionDescLocaleID("InjuryFrequency"),
				"Как часто спортсмены получают травмы. При 100% действует стандартная частота мода: обычно травмы краткосрочные, серьёзные случаются редко. При 0% травмы отключены."
			},
			{
				((ModSetting)m_Setting).GetOptionLabelLocaleID("ShowLeagueButton"),
				"Показывать кнопку «Спорт»"
			},
			{
				((ModSetting)m_Setting).GetOptionDescLocaleID("ShowLeagueButton"),
				"Показывать в левом верхнем углу кнопку для открытия календарей, результатов и турнирных таблиц."
			},
			{
				((ModSetting)m_Setting).GetOptionLabelLocaleID("Temperature"),
				"Температура"
			},
			{
				((ModSetting)m_Setting).GetOptionDescLocaleID("Temperature"),
				"Единицы измерения температуры в дни матчей: как в настройках игры, всегда градусы Цельсия или Фаренгейта."
			},
			{
				((ModSetting)m_Setting).GetEnumValueLocaleID<Setting.TemperatureUnit>(Setting.TemperatureUnit.SameAsGame),
				"Как в игре"
			},
			{
				((ModSetting)m_Setting).GetEnumValueLocaleID<Setting.TemperatureUnit>(Setting.TemperatureUnit.Celsius),
				"Цельсий (°C)"
			},
			{
				((ModSetting)m_Setting).GetEnumValueLocaleID<Setting.TemperatureUnit>(Setting.TemperatureUnit.Fahrenheit),
				"Фаренгейт (°F)"
			},
			{
				((ModSetting)m_Setting).GetOptionLabelLocaleID("ResetAllData"),
				"Сбросить спортивные данные"
			},
			{
				((ModSetting)m_Setting).GetOptionDescLocaleID("ResetAllData"),
				"Удаляет все спортивные объекты, команды, расписания и результаты в текущем городе. Доступно только при открытом городе."
			},
			{
				((ModSetting)m_Setting).GetOptionWarningLocaleID("ResetAllData"),
				"Удалить все спортивные данные этого города? Отменить действие нельзя, кроме как загрузив более раннее сохранение."
			},
			{
				((ModSetting)m_Setting).GetOptionLabelLocaleID("Version"),
				"Версия"
			},
			{
				((ModSetting)m_Setting).GetOptionDescLocaleID("Version"),
				"Версия мода «Спорт России»."
			}
		};
	}

	public void Unload()
	{
	}
}
