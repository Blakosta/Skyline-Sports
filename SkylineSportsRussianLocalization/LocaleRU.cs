using System;
using System.Collections.Generic;
using System.Reflection;
using Colossal;
using Game.Modding;

namespace SkylineSportsRussianLocalization
{
    internal sealed class LocaleRU : IDictionarySource
    {
        // Use the REAL settings instance created by Skyline Sports. ModSetting locale IDs
        // are tied to the owning IMod; creating a mirror with this localizer's IMod
        // generates keys for the wrong mod.
        private static ModSetting FindOriginalSettings()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type modType = assembly.GetType("Skyline_Sports_Project.Mod", false);
                if (modType == null) continue;

                PropertyInfo property = modType.GetProperty(
                    "Settings", BindingFlags.Public | BindingFlags.Static);
                ModSetting settings = property?.GetValue(null, null) as ModSetting;
                if (settings != null) return settings;
            }
            return null;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            ModSetting s = FindOriginalSettings();
            if (s == null)
                return new Dictionary<string, string>();

            var entries = new Dictionary<string, string>
            {
                { s.GetSettingsLocaleID(), "Спорт России" },
                { s.GetOptionTabLocaleID("Main"), "Основное" },
                { s.GetOptionGroupLocaleID("Leagues"), "Лиги" },
                { s.GetOptionGroupLocaleID("Display"), "Отображение" },
                { s.GetOptionGroupLocaleID("Data"), "Данные" },
                { s.GetOptionGroupLocaleID("Fans"), "Болельщики" },
                { s.GetOptionGroupLocaleID("About"), "О моде" },

                { s.GetOptionLabelLocaleID("RealFans"), "Болельщики на главных матчах" },
                { s.GetOptionDescLocaleID("RealFans"), "Жители вашего города приходят на главные матчи. Примерно за 2,5 часа до начала они отправляются к арене на автомобилях, общественном транспорте или пешком. После матча болельщики возвращаются домой." },
                { s.GetOptionLabelLocaleID("VisitorFans"), "Гости из других городов" },
                { s.GetOptionDescLocaleID("VisitorFans"), "Туристы, остановившиеся в городе, также смогут посещать главные матчи и увеличивать число зрителей. После игры они возвращаются в свои гостиницы." },
                { s.GetOptionLabelLocaleID("GameDayTourism"), "Матчи привлекают туристов (экспериментально)" },
                { s.GetOptionDescLocaleID("GameDayTourism"), "В день главного матча спортивный объект, являющийся достопримечательностью, временно получает повышенную привлекательность и может привлечь дополнительных туристов." },
                { s.GetOptionLabelLocaleID("RealFansCount"), "Болельщиков на матч" },
                { s.GetOptionDescLocaleID("RealFansCount"), "Количество жителей, посещающих каждый главный матч. Большое число зрителей увеличивает транспортную нагрузку и заполняемость парковок возле арены." },

                { s.GetOptionLabelLocaleID("LeagueSize"), "Команд в лиге" },
                { s.GetOptionDescLocaleID("LeagueSize"), "Количество команд в каждой лиге. Сначала учитываются команды вашего города, остальные места занимают соперники из других городов." },
                { s.GetOptionLabelLocaleID("SeasonGames"), "Матчей за сезон" },
                { s.GetOptionDescLocaleID("SeasonGames"), "Доля матчей в календаре сезона. Уменьшите значение, чтобы сократить число матчей. Изменение вступит в силу со следующего сезона." },
                { s.GetOptionLabelLocaleID("InjuryFrequency"), "Частота травм" },
                { s.GetOptionDescLocaleID("InjuryFrequency"), "Как часто спортсмены получают травмы. При 0% травмы отключены." },
                { s.GetOptionLabelLocaleID("ShowLeagueButton"), "Показывать кнопку «Спорт»" },
                { s.GetOptionDescLocaleID("ShowLeagueButton"), "Показывать кнопку для открытия календарей, результатов и турнирных таблиц." },
                { s.GetOptionLabelLocaleID("Temperature"), "Температура" },
                { s.GetOptionDescLocaleID("Temperature"), "Единицы измерения температуры в дни матчей." },
                { s.GetOptionLabelLocaleID("ResetAllData"), "Сбросить спортивные данные" },
                { s.GetOptionDescLocaleID("ResetAllData"), "Удаляет все спортивные объекты, команды, расписания и результаты в текущем городе." },
                { s.GetOptionWarningLocaleID("ResetAllData"), "Удалить все спортивные данные этого города? Отменить действие нельзя, кроме как загрузив более раннее сохранение." },
                { s.GetOptionLabelLocaleID("Version"), "Версия" },
                { s.GetOptionDescLocaleID("Version"), "Версия мода «Спорт России»." }
            };

            AddEnumEntries(entries, s);
            return entries;
        }

        private static void AddEnumEntries(Dictionary<string, string> entries, ModSetting setting)
        {
            Type modType = null;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                modType = assembly.GetType("Skyline_Sports_Project.Setting", false);
                if (modType != null) break;
            }
            if (modType == null) return;

            Type enumType = modType.GetNestedType("TemperatureUnit", BindingFlags.Public);
            if (enumType == null || !enumType.IsEnum) return;

            MethodInfo method = typeof(ModSetting).GetMethod("GetEnumValueLocaleID");
            if (method == null || !method.IsGenericMethodDefinition) return;
            MethodInfo generic = method.MakeGenericMethod(enumType);

            AddEnum(entries, generic, setting, enumType, "SameAsGame", "Как в игре");
            AddEnum(entries, generic, setting, enumType, "Celsius", "Цельсий (°C)");
            AddEnum(entries, generic, setting, enumType, "Fahrenheit", "Фаренгейт (°F)");
        }

        private static void AddEnum(Dictionary<string, string> entries, MethodInfo method,
            ModSetting setting, Type enumType, string valueName, string translation)
        {
            object value = Enum.Parse(enumType, valueName);
            string key = (string)method.Invoke(setting, new object[] { value });
            entries[key] = translation;
        }

        public void Unload() { }
    }
}
