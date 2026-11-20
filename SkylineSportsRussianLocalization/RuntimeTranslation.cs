using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using HarmonyLib;

namespace SkylineSportsRussianLocalization
{
    /// <summary>
    /// Translates strings at the boundary where Skyline Sports serializes its UI data.
    /// No original game/mod methods or simulation logic are replaced.
    /// </summary>
    internal static class RuntimeTranslation
    {
        private const string HarmonyId = "Blakosta.SkylineSports.RussianLocalization";
        private static Harmony _harmony;
        private static bool _installed;

        private static readonly Dictionary<string, string> Exact = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "Single season", "Один сезон" },
            { "Career", "Карьера" },
            { "Awards", "Награды" },
            { "Teams", "Команды" },
            { "Metro", "Метро" },
            { "District", "Округ" },
            { "Second", "Второй" },
            { "First", "Первый" },
            { "Third", "Третий" },
            { "Pens", "Пенальти" },
            { "America", "Америка" },
            { "Britain", "Великобритания" },
            { "Korea", "Корея" },
            { "Holland", "Нидерланды" },
            { "Deutschland", "Германия" },
            { "Prospect", "Перспективный игрок" },
            { "Recruiting", "Подбор игроков" },
            { "Weight", "Вес" },
            { "Yellow", "Жёлтая карточка" },
            { "Red", "Красная карточка" },
            { "Shootout", "Буллиты" },
            { "Penalties", "Пенальти" },
            { "Qualifier", "Отборочный турнир" },
            { "Quarterfinal", "Четвертьфинал" },
            { "Semifinal", "Полуфинал" },
            { "All-Star", "Матч всех звёзд" },
            { "Champion", "Чемпион" },
            { "Winning", "Победы" },
            { "Loyalty", "Преданность" },
            { "Money", "Деньги" },
            { "Balanced", "Сбалансированный" },
            { "Canada", "Канада" },
            { "Mexico", "Мексика" },
            { "Sweden", "Швеция" },
            { "Finland", "Финляндия" },
            { "Russia", "Россия" },
            { "Czechia", "Чехия" },
            { "Slovakia", "Словакия" },
            { "Switzerland", "Швейцария" },
            { "Germany", "Германия" },
            { "Latvia", "Латвия" },
            { "Norway", "Норвегия" },
            { "Denmark", "Дания" },
            { "Austria", "Австрия" },
            { "France", "Франция" },
            { "Kazakhstan", "Казахстан" },
            { "Slovenia", "Словения" },
            { "Italy", "Италия" },
            { "Hungary", "Венгрия" },
            { "Poland", "Польша" },
            { "England", "Англия" },
            { "Scotland", "Шотландия" },
            { "Wales", "Уэльс" },
            { "Ireland", "Ирландия" },
            { "Brazil", "Бразилия" },
            { "Argentina", "Аргентина" },
            { "Spain", "Испания" },
            { "Portugal", "Португалия" },
            { "Netherlands", "Нидерланды" },
            { "Belgium", "Бельгия" },
            { "Uruguay", "Уругвай" },
            { "Colombia", "Колумбия" },
            { "Ecuador", "Эквадор" },
            { "Chile", "Чили" },
            { "Peru", "Перу" },
            { "Venezuela", "Венесуэла" },
            { "Nigeria", "Нигерия" },
            { "Ghana", "Гана" },
            { "Senegal", "Сенегал" },
            { "Morocco", "Марокко" },
            { "Cameroon", "Камерун" },
            { "Egypt", "Египет" },
            { "Japan", "Япония" },
            { "China", "Китай" },
            { "Australia", "Австралия" },
            { "Philippines", "Филиппины" },
            { "India", "Индия" },
            { "Iran", "Иран" },
            { "Israel", "Израиль" },
            { "Turkey", "Турция" },
            { "Greece", "Греция" },
            { "Serbia", "Сербия" },
            { "Croatia", "Хорватия" },
            { "Montenegro", "Черногория" },
            { "Lithuania", "Литва" },
            { "Estonia", "Эстония" },
            { "Ukraine", "Украина" },
            { "Belarus", "Беларусь" },
            { "Romania", "Румыния" },
            { "Bulgaria", "Болгария" },
            { "Iceland", "Исландия" },
            { "Cuba", "Куба" },
            { "Panama", "Панама" },
            { "Nicaragua", "Никарагуа" },
            { "Jamaica", "Ямайка" },
            { "The best single regular seasons ever in this league. A record stays until someone beats it.", "Лучшие отдельные регулярные сезоны в истории этой лиги. Рекорд сохраняется, пока его не побьют." },
            { "Career totals while playing in this league (regular season). Rate stats need about four full seasons to qualify.", "Карьерная статистика в этой лиге (регулярный сезон). Для рейтинговых показателей требуется примерно четыре полных сезона." },
            { "Most awards won.", "Больше всего полученных наград." },
            { "Championships and the best regular season records.", "Чемпионские титулы и лучшие результаты регулярного сезона." },
            { "Under 20.", "До 20 лет." },
            { "nations", "сборных" },
            { "plays in the qualifiers", "участвует в отборочном турнире" },
            { "is in the main event", "участвует в основном турнире" },
            { "The qualifiers decide the last spots.", "Последние места определяются по итогам отборочного турнира." },
            { "National teams are picked from the players in your sports world by where they're from; the spots nobody fills go to players from clubs abroad.", "Составы сборных формируются из игроков вашего спортивного мира по месту их происхождения; свободные места занимают игроки зарубежных клубов." },
            { "play, ", "игра, " },
            { " plays, ", " игр, " },
            { " yds", " ярд." },
            { " lbs", " фунт." },
            { "free agency", "свободные агенты" },
            { "We have ", "У нас " },
            { ". Thank you!", ". Спасибо!" },
            { "the season", "сезон" },
            { "Pro Bowl", "Матч всех звёзд" },
            { "South Africa", "ЮАР" },
            { "South Korea", "Южная Корея" },
            { "Chinese Taipei", "Китайский Тайбэй" },
            { "New Zealand", "Новая Зеландия" },
            { "Saudi Arabia", "Саудовская Аравия" },
            { "Puerto Rico", "Пуэрто-Рико" },
            { "Dominican Republic", "Доминиканская Республика" },
            { "United States of America", "Соединённые Штаты Америки" },
            { "Czech Republic", "Чехия" },
            { "American Samoa", "Американское Самоа" },
            { "Today", "Сегодня" },
            { "Tomorrow", "Завтра" },
            { "Yesterday", "Вчера" },
            { "Home", "Дома" },
            { "Away", "В гостях" },
            { "Schedule", "Расписание" },
            { "Results", "Результаты" },
            { "Standings", "Турнирная таблица" },
            { "Statistics", "Статистика" },
            { "Stats", "Статистика" },
            { "Players", "Игроки" },
            { "Player", "Игрок" },
            { "Team", "Команда" },
            { "Roster", "Состав" },
            { "Lineup", "Стартовый состав" },
            { "Coaches", "Тренеры" },
            { "Coach", "Тренер" },
            { "League", "Лига" },
            { "Leagues", "Лиги" },
            { "Season", "Сезон" },
            { "Seasons", "Сезоны" },
            { "Playoffs", "Плей-офф" },
            { "Championship", "Чемпионат" },
            { "Tournament", "Турнир" },
            { "Tournaments", "Турниры" },
            { "National teams", "Сборные" },
            { "National team", "Сборная" },
            { "Country", "Страна" },
            { "Nation", "Сборная" },
            { "Venue", "Арена" },
            { "Venues", "Арены" },
            { "Stadium", "Стадион" },
            { "Arena", "Арена" },
            { "Games", "Матчи" },
            { "Game", "Матч" },
            { "Match", "Матч" },
            { "Matches", "Матчи" },
            { "News", "Новости" },
            { "Award", "Награда" },
            { "Records", "Рекорды" },
            { "Record", "Рекорд" },
            { "History", "История" },
            { "All time", "За всё время" },
            { "Free agents", "Свободные агенты" },
            { "Free agency", "Свободные агенты" },
            { "Transfers", "Трансферы" },
            { "Transfer", "Трансфер" },
            { "Trades", "Обмены" },
            { "Trade", "Обмен" },
            { "Contracts", "Контракты" },
            { "Contract", "Контракт" },
            { "Salary", "Зарплата" },
            { "Budget", "Бюджет" },
            { "Injuries", "Травмы" },
            { "Injury", "Травма" },
            { "Healthy", "Здоров" },
            { "Injured", "Травмирован" },
            { "Retired", "Завершил карьеру" },
            { "Retirement", "Завершение карьеры" },
            { "Draft", "Драфт" },
            { "Draft picks", "Выборы на драфте" },
            { "All Stars", "Все звёзды" },
            { "Champions", "Чемпионы" },
            { "Final", "Финал" },
            { "Finals", "Финал" },
            { "Fourth", "Четвёртый" },
            { "Wins", "Победы" },
            { "Losses", "Поражения" },
            { "Draws", "Ничьи" },
            { "Points", "Очки" },
            { "Goals", "Голы" },
            { "Goals for", "Забитые голы" },
            { "Goals against", "Пропущенные голы" },
            { "Win percentage", "Процент побед" },
            { "Rank", "Место" },
            { "Ranking", "Рейтинг" },
            { "Overall", "Общий рейтинг" },
            { "Rating", "Рейтинг" },
            { "Age", "Возраст" },
            { "Height", "Рост" },
            { "Position", "Позиция" },
            { "Status", "Статус" },
            { "Name", "Имя" },
            { "Names", "Имена" },
            { "Date", "Дата" },
            { "Time", "Время" },
            { "Location", "Место проведения" },
            { "Details", "Подробности" },
            { "Overview", "Обзор" },
            { "Summary", "Сводка" },
            { "Back", "Назад" },
            { "Next", "Далее" },
            { "Previous", "Назад" },
            { "Close", "Закрыть" },
            { "Cancel", "Отмена" },
            { "Save", "Сохранить" },
            { "Reset", "Сбросить" },
            { "Apply", "Применить" },
            { "Settings", "Настройки" },
            { "Options", "Параметры" },
            { "About", "О моде" },
            { "Enabled", "Включено" },
            { "Disabled", "Отключено" },
            { "Yes", "Да" },
            { "No", "Нет" },
            { "None", "Нет" },
            { "Unknown", "Неизвестно" },
            { "Pending", "Ожидается" },
            { "Complete", "Завершено" },
            { "Completed", "Завершено" },
            { "Upcoming", "Предстоящие" },
            { "Current", "Текущий" },
            { "Previous season", "Прошлый сезон" },
            { "Next season", "Следующий сезон" },
            { "Regular season", "Регулярный сезон" },
            { "Postseason", "Плей-офф" },
            { "Playoff", "Плей-офф" },
            { "Home game", "Домашний матч" },
            { "Away game", "Выездной матч" },
            { "Attendance", "Посещаемость" },
            { "Fans", "Болельщики" },
            { "Supporters", "Болельщики" },
            { "Capacity", "Вместимость" },
            { "Weather", "Погода" },
            { "Temperature", "Температура" },
            { "Rain", "Дождь" },
            { "Snow", "Снег" },
            { "Wind", "Ветер" },
            { "Offense", "Нападение" },
            { "Defense", "Защита" },
            { "Offensive", "Атакующий" },
            { "Defensive", "Защитный" },
            { "Top scorer", "Лучший бомбардир" },
            { "Top players", "Лучшие игроки" },
            { "League leaders", "Лидеры лиги" },
            { "Team leaders", "Лидеры команды" },
            { "Player stats", "Статистика игрока" },
            { "Team stats", "Статистика команды" },
            { "Season stats", "Статистика сезона" },
            { "Career stats", "Статистика за карьеру" }
        };

        public static void Install()
        {
            if (_installed) return;
            try
            {
                _harmony = new Harmony(HarmonyId);
                AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                    TryPatch(assembly);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("[Skyline Sports RU] Runtime translation install failed: " + ex);
            }
        }

        public static void Uninstall()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyLoad -= OnAssemblyLoad;
                if (_harmony != null) _harmony.UnpatchAll(HarmonyId);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning("[Skyline Sports RU] Runtime translation cleanup failed: " + ex.Message);
            }
            _installed = false;
            _harmony = null;
        }

        private static void OnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            TryPatch(args.LoadedAssembly);
        }

        private static void TryPatch(Assembly assembly)
        {
            if (_installed || assembly == null) return;
            Type builder = assembly.GetType("Skyline_Sports_Project.Engine.JsonBuilder", false);
            if (builder == null) return;

            MethodInfo target = builder.GetMethod("Value", BindingFlags.Public | BindingFlags.Instance,
                null, new[] { typeof(string) }, null);
            if (target == null) return;

            MethodInfo prefix = typeof(RuntimeTranslation).GetMethod(nameof(TranslateValue),
                BindingFlags.NonPublic | BindingFlags.Static);
            _harmony.Patch(target, prefix: new HarmonyMethod(prefix));
            _installed = true;
            UnityEngine.Debug.Log("[Skyline Sports RU] Runtime UI text translation attached.");
        }

        private static void TranslateValue(ref string __0)
        {
            if (string.IsNullOrEmpty(__0)) return;
            if (Exact.TryGetValue(__0, out string translated))
            {
                __0 = translated;
                return;
            }

            // Exact-value translations only: avoid changing generated player or team names.
                        __0 = Regex.Replace(__0, @"\bJan\b", "янв", RegexOptions.IgnoreCase);
            __0 = Regex.Replace(__0, @"\bFeb\b", "фев", RegexOptions.IgnoreCase);
            __0 = Regex.Replace(__0, @"\bMar\b", "мар", RegexOptions.IgnoreCase);
            __0 = Regex.Replace(__0, @"\bApr\b", "апр", RegexOptions.IgnoreCase);
            __0 = Regex.Replace(__0, @"\bMay\b", "май", RegexOptions.IgnoreCase);
            __0 = Regex.Replace(__0, @"\bJun\b", "июн", RegexOptions.IgnoreCase);
            __0 = Regex.Replace(__0, @"\bJul\b", "июл", RegexOptions.IgnoreCase);
            __0 = Regex.Replace(__0, @"\bAug\b", "авг", RegexOptions.IgnoreCase);
            __0 = Regex.Replace(__0, @"\bSep\b", "сен", RegexOptions.IgnoreCase);
            __0 = Regex.Replace(__0, @"\bOct\b", "окт", RegexOptions.IgnoreCase);
            __0 = Regex.Replace(__0, @"\bNov\b", "ноя", RegexOptions.IgnoreCase);
            __0 = Regex.Replace(__0, @"\bDec\b", "дек", RegexOptions.IgnoreCase);
        }
    }
}
