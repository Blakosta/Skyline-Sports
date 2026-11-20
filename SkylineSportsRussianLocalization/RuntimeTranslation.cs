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
        private static string[] _russianFirstNames;
        private static string[] _russianLastNames;


        private static readonly Dictionary<string, string> HometownRegions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "ON", "Онтарио" }, { "AB", "Альберта" }, { "SK", "Саскачеван" }, { "MB", "Манитоба" },
            { "BC", "Британская Колумбия" }, { "NS", "Новая Шотландия" }, { "NB", "Нью-Брансуик" },
            { "NL", "Ньюфаундленд и Лабрадор" }, { "QC", "Квебек" }, { "PE", "Остров Принца Эдуарда" },
            { "YT", "Юкон" }, { "NT", "Северо-Западные территории" }, { "NU", "Нунавут" },
            { "HI", "Гавайи" }, { "UT", "Юта" }, { "CA", "Калифорния" }, { "TX", "Техас" },
            { "GA", "Джорджия" }, { "MD", "Мэриленд" }, { "NJ", "Нью-Джерси" }, { "NC", "Северная Каролина" },
            { "DC", "округ Колумбия" }, { "OH", "Огайо" },
            { "France", "Франция" }, { "Ukraine", "Украина" }, { "Czechia", "Чехия" },
            { "Slovakia", "Словакия" }, { "Sweden", "Швеция" }, { "Norway", "Норвегия" },
            { "Denmark", "Дания" }, { "Finland", "Финляндия" }, { "Germany", "Германия" },
            { "Switzerland", "Швейцария" }, { "Austria", "Австрия" }, { "Italy", "Италия" },
            { "Dominican Republic", "Доминиканская Республика" }, { "Venezuela", "Венесуэла" },
            { "Cuba", "Куба" }, { "Puerto Rico", "Пуэрто-Рико" }, { "Mexico", "Мексика" },
            { "Panama", "Панама" }, { "Colombia", "Колумбия" }, { "Nicaragua", "Никарагуа" },
            { "Argentina", "Аргентина" }, { "Spain", "Испания" }, { "Uruguay", "Уругвай" },
            { "Chile", "Чили" }, { "Peru", "Перу" }, { "Costa Rica", "Коста-Рика" },
            { "Ecuador", "Эквадор" }, { "Brazil", "Бразилия" }, { "Portugal", "Португалия" },
            { "Serbia", "Сербия" }, { "Montenegro", "Черногория" }, { "Bosnia", "Босния и Герцеговина" },
            { "England", "Англия" }, { "Scotland", "Шотландия" }, { "Wales", "Уэльс" },
            { "Northern Ireland", "Северная Ирландия" }, { "Australia", "Австралия" },
            { "Japan", "Япония" }, { "South Korea", "Южная Корея" }, { "Ghana", "Гана" },
            { "Nigeria", "Нигерия" }, { "Senegal", "Сенегал" }, { "Mali", "Мали" },
            { "Ivory Coast", "Кот-д’Ивуар" }, { "Guinea", "Гвинея" }, { "Morocco", "Марокко" },
            { "Egypt", "Египет" }, { "Tunisia", "Тунис" }, { "Algeria", "Алжир" },
            { "American Samoa", "Американское Самоа" }, { "New Zealand", "Новая Зеландия" },
            { "Tonga", "Тонга" }, { "Samoa", "Самоа" }
        };

        private static readonly Dictionary<string, string> HometownExceptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Paris", "Париж" }, { "Lyon", "Лион" }, { "Marseille", "Марсель" }, { "Lille", "Лилль" },
            { "Bordeaux", "Бордо" }, { "Nantes", "Нант" }, { "Le Mans", "Ле-Ман" }, { "Strasbourg", "Страсбург" },
            { "Kyiv", "Киев" }, { "Kharkiv", "Харьков" }, { "Lviv", "Львов" }, { "Odesa", "Одесса" },
            { "Dnipro", "Днепр" }, { "Zaporizhzhia", "Запорожье" }, { "Prague", "Прага" }, { "Brno", "Брно" },
            { "Ostrava", "Острава" }, { "Bratislava", "Братислава" }, { "Stockholm", "Стокгольм" },
            { "Gothenburg", "Гётеборг" }, { "Malmo", "Мальмё" }, { "Oslo", "Осло" },
            { "Copenhagen", "Копенгаген" }, { "Helsinki", "Хельсинки" }, { "Berlin", "Берлин" },
            { "Munich", "Мюнхен" }, { "Cologne", "Кёльн" }, { "Hamburg", "Гамбург" },
            { "Zurich", "Цюрих" }, { "Vienna", "Вена" }, { "Milan", "Милан" }, { "Rome", "Рим" },
            { "Naples", "Неаполь" }, { "Turin", "Турин" }, { "Florence", "Флоренция" }, { "Bologna", "Болонья" },
            { "London", "Лондон" }, { "Manchester", "Манчестер" }, { "Liverpool", "Ливерпуль" },
            { "Birmingham", "Бирмингем" }, { "Leeds", "Лидс" }, { "Newcastle", "Ньюкасл" },
            { "Glasgow", "Глазго" }, { "Edinburgh", "Эдинбург" }, { "Cardiff", "Кардифф" },
            { "Belfast", "Белфаст" }, { "Tokyo", "Токио" }, { "Osaka", "Осака" }, { "Yokohama", "Йокогама" },
            { "Nagoya", "Нагоя" }, { "Sapporo", "Саппоро" }, { "Fukuoka", "Фукуока" },
            { "Hiroshima", "Хиросима" }, { "Sendai", "Сендай" }, { "Seoul", "Сеул" },
            { "Busan", "Пусан" }, { "Incheon", "Инчхон" }, { "Daegu", "Тэгу" }, { "Gwangju", "Кванджу" },
            { "Buenos Aires", "Буэнос-Айрес" }, { "Sao Paulo", "Сан-Паулу" }, { "Rio de Janeiro", "Рио-де-Жанейро" },
            { "Mexico City", "Мехико" }, { "Bogota", "Богота" }, { "Montevideo", "Монтевидео" },
            { "Lisbon", "Лиссабон" }, { "Belgrade", "Белград" }, { "Podgorica", "Подгорица" },
            { "Santo Domingo", "Санто-Доминго" }, { "San Juan", "Сан-Хуан" }, { "Panama City", "Панама" },
            { "Auckland", "Окленд" }, { "Pago Pago", "Паго-Паго" }, { "Honolulu", "Гонолулу" },
            { "Toronto", "Торонто" }, { "Montreal", "Монреаль" }, { "Vancouver", "Ванкувер" },
            { "Ottawa", "Оттава" }, { "Calgary", "Калгари" }, { "Edmonton", "Эдмонтон" },
            { "Winnipeg", "Виннипег" }, { "Quebec City", "Квебек" }, { "Hamilton", "Гамильтон" },
            { "Mississauga", "Миссиссога" }, { "Sault Ste. Marie", "Су-Сент-Мари" }, { "St. John's", "Сент-Джонс" },
            { "New York", "Нью-Йорк" }, { "Salt Lake City", "Солт-Лейк-Сити" }, { "Washington", "Вашингтон" },
            { "Houston", "Хьюстон" }, { "Dallas", "Даллас" }, { "Atlanta", "Атланта" },
            { "Baltimore", "Балтимор" }, { "Newark", "Ньюарк" }, { "Charlotte", "Шарлотт" }, { "Columbus", "Колумбус" }
        };

        private static string LocalizeHometown(string value)
        {
            Match match = Regex.Match(value ?? "", @"^(.+?), ([A-Za-z ]{2,})$");
            if (!match.Success) return value;
            string region = match.Groups[2].Value.Trim();
            if (!HometownRegions.TryGetValue(region, out string localizedRegion)) return value;
            string city = match.Groups[1].Value.Trim();
            if (!HometownExceptions.TryGetValue(city, out string localizedCity))
                localizedCity = TransliterateCity(city);
            return localizedCity + ", " + localizedRegion;
        }

        private static string TransliterateCity(string value)
        {
            string input = (value ?? "").ToLowerInvariant();
            string result = "";
            for (int i = 0; i < input.Length; i++)
            {
                string pair = i + 1 < input.Length ? input.Substring(i, 2) : "";
                string digraph = null;
                switch (pair)
                {
                    case "sh": digraph = "ш"; break;
                    case "ch": digraph = "ч"; break;
                    case "zh": digraph = "ж"; break;
                    case "kh": digraph = "х"; break;
                    case "ts": digraph = "ц"; break;
                    case "ya": digraph = "я"; break;
                    case "yu": digraph = "ю"; break;
                    case "yo": digraph = "ё"; break;
                    case "ph": digraph = "ф"; break;
                    case "th": digraph = "т"; break;
                    case "qu": digraph = "кв"; break;
                }
                if (digraph != null) { result += digraph; i++; continue; }
                switch (input[i])
                {
                    case 'a': result += "а"; break; case 'b': result += "б"; break; case 'c': result += "к"; break;
                    case 'd': result += "д"; break; case 'e': result += "е"; break; case 'f': result += "ф"; break;
                    case 'g': result += "г"; break; case 'h': result += "х"; break; case 'i': result += "и"; break;
                    case 'j': result += "дж"; break; case 'k': result += "к"; break; case 'l': result += "л"; break;
                    case 'm': result += "м"; break; case 'n': result += "н"; break; case 'o': result += "о"; break;
                    case 'p': result += "п"; break; case 'r': result += "р"; break; case 's': result += "с"; break;
                    case 't': result += "т"; break; case 'u': result += "у"; break; case 'v': result += "в"; break;
                    case 'w': result += "в"; break; case 'x': result += "кс"; break; case 'y': result += "й"; break;
                    case 'z': result += "з"; break; default: result += input[i]; break;
                }
            }
            string[] words = result.Split(' ');
            for (int i = 0; i < words.Length; i++)
                if (words[i].Length > 0) words[i] = char.ToUpperInvariant(words[i][0]) + words[i].Substring(1);
            return string.Join(" ", words);
        }

        private static readonly Dictionary<string, string> Exact = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "Single season", "Один сезон" },
            { "LIVE", "ПРЯМОЙ ЭФИР" },
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
            { "SameAsGame", "Как в игре" },
            { "Celsius", "Цельсий" },
            { "Fahrenheit", "Фаренгейт" },
            { "integer", "целое число" },
            { "percentage", "проценты" },
            { "LW", "ЛК" }, { "RW", "ПК" }, { "PG", "РЗ" }, { "SG", "АЗ" },
            { "SF", "ЛФ" }, { "PF", "ТФ" }, { "QB", "КБ" }, { "RB", "РБ" },
            { "WR", "ПР" }, { "TE", "ТЭ" }, { "OL", "ЛН" }, { "DL", "ЛЗ" },
            { "LB", "ЛБ" }, { "CB", "КБ" }, { "SP", "ПТ" }, { "RP", "ПРЛ" },
            { "SS", "ШС" }, { "LF", "ЛП" }, { "CF", "ЦП" }, { "RF", "ПП" },
            { "GK", "ВР" }, { "DF", "ЗЩ" }, { "MF", "ПЗ" }, { "FW", "НП" },
            { "Taiwan", "Тайвань" },
            { "SH", "БР" },
            { "YC", "ЖК" },
            { "RC", "КК" },
            { "GA", "ПГ" },
            { "CS", "СМ" },
            { "playoff", "плей-офф" },
            { "rain", "дождь" },
            { "snow", "снег" },
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
            { "USA", "США" },
            { "United States", "США" },







            { "(kick)", "(удар)" },
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

            // Optional content patches must never prevent the UI translation from working.
            Type playerNames = assembly.GetType("Skyline_Sports_Project.Engine.PlayerNames", false);
            if (playerNames != null)
            {
                try
                {
                    MethodInfo worldShare = playerNames.GetMethod("WorldShare", BindingFlags.Public | BindingFlags.Static);
                    if (worldShare != null)
                    {
                        MethodInfo russianNames = typeof(RuntimeTranslation).GetMethod(
                            nameof(ForceRussianNameOrigin), BindingFlags.NonPublic | BindingFlags.Static);
                        _harmony.Patch(worldShare, postfix: new HarmonyMethod(russianNames));
                    }
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogWarning("[Skyline Sports RU] Russian name-origin patch skipped: " + ex.Message);
                }

                try
                {
                    FieldInfo firstField = playerNames.GetField("FirstUS", BindingFlags.NonPublic | BindingFlags.Static);
                    FieldInfo lastField = playerNames.GetField("LastUS", BindingFlags.NonPublic | BindingFlags.Static);
                    _russianFirstNames = firstField == null ? null : firstField.GetValue(null) as string[];
                    _russianLastNames = lastField == null ? null : lastField.GetValue(null) as string[];
                    MethodInfo pickNames = playerNames.GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .FirstOrDefault(m => m.Name == "Pick" && m.GetParameters().Length == 7);
                    if (pickNames != null && _russianFirstNames != null && _russianLastNames != null)
                    {
                        MethodInfo russianPlayerName = typeof(RuntimeTranslation).GetMethod(
                            nameof(EnsureRussianPlayerName), BindingFlags.NonPublic | BindingFlags.Static);
                        _harmony.Patch(pickNames, postfix: new HarmonyMethod(russianPlayerName));
                    }
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogWarning("[Skyline Sports RU] Russian player-name patch skipped: " + ex.Message);
                }
            }

            try
            {
                Type nameGenerator = assembly.GetType("Skyline_Sports_Project.Engine.NameGenerator", false);
                MethodInfo schoolName = nameGenerator == null ? null : nameGenerator.GetMethods(
                    BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(m =>
                        m.Name == "HighSchoolName" && m.ReturnType == typeof(string));
                if (schoolName != null)
                {
                    MethodInfo russianSchool = typeof(RuntimeTranslation).GetMethod(
                        nameof(LocalizeSchoolName), BindingFlags.NonPublic | BindingFlags.Static);
                    _harmony.Patch(schoolName, postfix: new HarmonyMethod(russianSchool));
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning("[Skyline Sports RU] School-name patch skipped: " + ex.Message);
            }
        }

        private static void ForceRussianNameOrigin(ref double __result)
        {
            __result = 0.0;
        }

        private static void EnsureRussianPlayerName(Random __2, ref string __4, ref string __5)
        {
            if (__2 == null || _russianFirstNames == null || _russianLastNames == null) return;
            if (ContainsLatinLetters(__4) || ContainsLatinLetters(__5))
            {
                __4 = _russianFirstNames[__2.Next(_russianFirstNames.Length)];
                __5 = _russianLastNames[__2.Next(_russianLastNames.Length)];
            }
        }

        private static bool ContainsLatinLetters(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            foreach (char c in value)
                if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')) return true;
            return false;
        }

        private static void LocalizeSchoolName(ref string __result)
        {
            if (!string.IsNullOrEmpty(__result) && __result.EndsWith(" High", StringComparison.Ordinal))
                __result = __result.Substring(0, __result.Length - " High".Length) + " школа";
        }

        private static void TranslateValue(ref string __0)
        {
            if (string.IsNullOrEmpty(__0)) return;
            if (Exact.TryGetValue(__0, out string translated))
            {
                __0 = translated;
                return;
            }

            // Hometown labels are generated data: localize both city and region without touching player/team names.
            string hometown = LocalizeHometown(__0);
            if (!string.Equals(hometown, __0, StringComparison.Ordinal))
            {
                __0 = hometown;
                return;
            }

            // Dynamic announcements: translate the sentence while keeping generated names intact.
            const string prospectsPrefix = "No young prospects on the market right now. A new class arrives every ";
            if (__0.StartsWith(prospectsPrefix, StringComparison.Ordinal))
            {
                string month = __0.Substring(prospectsPrefix.Length).TrimEnd('.');
                switch (month)
                {
                    case "January": month = "январе"; break;
                    case "February": month = "феврале"; break;
                    case "March": month = "марте"; break;
                    case "April": month = "апреле"; break;
                    case "May": month = "мае"; break;
                    case "June": month = "июне"; break;
                    case "July": month = "июле"; break;
                    case "August": month = "августе"; break;
                    case "September": month = "сентябре"; break;
                    case "October": month = "октябре"; break;
                    case "November": month = "ноябре"; break;
                    case "December": month = "декабре"; break;
                }
                __0 = "Сейчас на рынке нет молодых перспективных игроков. Новый набор появится в " + month + ".";
            }

            if (__0.StartsWith("We have ", StringComparison.Ordinal))
            {
                __0 = "Мы " + __0.Substring("We have ".Length);
                __0 = __0.Replace(". Thank you!", ". Спасибо!");
            }

            // A player's name is kept verbatim; only the English sentence frame is localized.
            Match freeAgent = Regex.Match(__0, @"^(.+?) was (.+?) and is now a free agent\.(.*)$");
            if (freeAgent.Success)
                __0 = freeAgent.Groups[1].Value + " теперь свободный агент." + freeAgent.Groups[3].Value;

            Match folded = Regex.Match(__0, @"^FOLDED: The (.+?) have folded after (?:the )?(.*?) season\.(.*)$");
            if (folded.Success)
            {
                string season = folded.Groups[2].Value.Trim();
                __0 = "РАСФОРМИРОВАНИЕ: команда «" + folded.Groups[1].Value + "» прекратила существование" +
                    (season.Length == 0 ? " после окончания сезона." : " после сезона " + season + ".") + folded.Groups[3].Value;
            }

            // International tournament post with city-country participation and local call-ups.
            if (__0.StartsWith("NATIONAL TEAMS: the ", StringComparison.Ordinal))
            {
                string body = __0.Substring("NATIONAL TEAMS: the ".Length);
                Match header = Regex.Match(body, @"^(.*?) starts (.*?) in (.*?)\.(.*)$");
                if (header.Success)
                {
                    string result = "СБОРНЫЕ: турнир «" + header.Groups[1].Value + "» начинается " +
                        header.Groups[2].Value + " в " + header.Groups[3].Value + ".";
                    string tail = header.Groups[4].Value;
                    Match home = Regex.Match(tail, @"^\s+(.+?), your city's country, (plays in the qualifiers|is in the main event)(?:, with (\d+) players? who grew up in your city)?\.");
                    if (home.Success)
                    {
                        string country = home.Groups[1].Value;
                        if (Exact.TryGetValue(country, out string translatedCountry)) country = translatedCountry;
                        result += " Сборная " + country +
                            (home.Groups[2].Value == "plays in the qualifiers" ? " участвует в отборочном турнире" : " участвует в основном турнире");
                        if (home.Groups[3].Success)
                        {
                            int localCount;
                            if (int.TryParse(home.Groups[3].Value, out localCount))
                                result += ", в составе " + localCount + (localCount == 1 ? " игрок, выросший в вашем городе" : " игроков, выросших в вашем городе");
                        }
                        result += ".";
                        tail = tail.Substring(home.Length);
                    }
                    Match calledUp = Regex.Match(tail, @"^\s+Your players called up: (.*?)(?: and (\d+) more)?\.$");
                    if (calledUp.Success)
                    {
                        result += " Ваши игроки, вызванные в сборную: " + calledUp.Groups[1].Value;
                        if (calledUp.Groups[2].Success) result += " и ещё " + calledUp.Groups[2].Value;
                        result += ".";
                        tail = "";
                    }
                    __0 = result + tail;
                }
            }

            if (__0.StartsWith("FLIP: ", StringComparison.Ordinal))
            {
                string text = __0.Substring("FLIP: ".Length).TrimEnd('!');
                text = Regex.Replace(text, @"(\d+)-star\b", "$1-звёздный");
                text = text.Replace(" flips from ", " меняет команду «")
                    .Replace(" from ", " из ")
                    .Replace(" to ", "» на команду «");
                __0 = "ПЕРЕХОД: " + text + "»!";
            }
            else if (__0.StartsWith("COMMIT: ", StringComparison.Ordinal))
            {
                string text = __0.Substring("COMMIT: ".Length).TrimEnd('!');
                text = Regex.Replace(text, @"(\d+)-star\b", "$1-звёздный");
                text = text.Replace(" from ", " из ")
                    .Replace(" commits to ", " выбирает команду «");
                __0 = "ПОДПИСАНИЕ: " + text + "»!";
            }

            if (__0.StartsWith("TRANSFER: The ", StringComparison.Ordinal))
            {
                string text = __0.Substring("TRANSFER: The ".Length).Replace(" sign ", "» подписывает игрока ");
                text = Regex.Replace(text, @" OVR\) from (?:the )?", " ОБЩ) из клуба «");
                text = text.Replace(" from ", " из клуба «").Replace(" for ", "» за ");
                __0 = "ТРАНСФЕР: команда «" + text;
            }

            Match lastSeason = Regex.Match(__0, @"^\s*Last season (.+?) came (down|up)\.(.*)$");
            if (lastSeason.Success)
                __0 = lastSeason.Groups[1].Value +
                    (lastSeason.Groups[2].Value == "down" ? " в прошлом сезоне выбыла в лигу ниже." : " в прошлом сезоне поднялась в лигу выше.") +
                    lastSeason.Groups[3].Value;

            Match medalNews = Regex.Match(__0, @"^(.+?) \((золотая|серебряная|бронзовая) with (.+?)\)$");
            if (medalNews.Success)
            {
                string country = medalNews.Groups[3].Value;
                if (Exact.TryGetValue(country, out string medalCountry)) country = medalCountry;
                __0 = medalNews.Groups[1].Value + " (" + medalNews.Groups[2].Value + " медаль, сборная " + country + ")";
            }
            __0 = __0.Replace(" spot", " место");

            Match hand = Regex.Match(__0, @"^Bats ([LRS]), throws ([LRS])$");
            if (hand.Success)
            {
                string batHand = hand.Groups[1].Value == "L" ? "левой" : hand.Groups[1].Value == "R" ? "правой" : "обеими";
                string throwHand = hand.Groups[2].Value == "L" ? "левой" : hand.Groups[2].Value == "R" ? "правой" : "обеими";
                __0 = "Бьёт " + batHand + ", бросает " + throwHand;
            }

            __0 = Regex.Replace(__0, @"\b(\d{1,2}(?::\d{2})?)\s+AM\b", "$1 утра");
            __0 = Regex.Replace(__0, @"\b(\d{1,2}(?::\d{2})?)\s+PM\b", "$1 вечера");

            Match overtime = Regex.Match(__0, @"^(\d*)OT$");
            if (overtime.Success) __0 = overtime.Groups[1].Value + "ОТ";

            // Football drive summaries are complete values, not standalone dictionary fragments.
            Match plays = Regex.Match(__0, @"^(\d+) plays?, (-?\d+) yds?$");
            if (plays.Success)
            {
                int count;
                int.TryParse(plays.Groups[1].Value, out count);
                __0 = plays.Groups[1].Value + (count == 1 ? " розыгрыш, " : " розыгрышей, ") + plays.Groups[2].Value + " ярдов";
            }

            // Tournament overview and date range.
            if (__0.StartsWith("In ", StringComparison.Ordinal))
            {
                __0 = Regex.Replace(__0, @"^In (.+?), ", "В городе $1, ");
                __0 = __0.Replace(" Under 20.", " Турнир для игроков до 20 лет.")
                    .Replace(" nations", " сборных")
                    .Replace(" through the qualifiers", " прошли отборочный турнир")
                    .Replace("The qualifiers decide the last spots.", "Последние места определяются по итогам отборочного турнира.")
                    .Replace("National teams are picked from the players in your sports world by where they're from; the spots nobody fills go to players from clubs abroad.",
                        "Составы сборных формируются по месту происхождения игроков; свободные места занимают спортсмены зарубежных клубов.");
            }

            Match draftPick = Regex.Match(__0, @"^(.+?) (1st|2nd|3rd|\d+th) round pick(?: \(от (.+?) \))?$");
            if (draftPick.Success)
            {
                string ordinal = draftPick.Groups[2].Value;
                string round = ordinal == "1st" ? "первого" : ordinal == "2nd" ? "второго" : ordinal == "3rd" ? "третьего" : ordinal.Substring(0, ordinal.Length - 2) + "-го";
                __0 = draftPick.Groups[1].Value + ", выбор " + round + " раунда" +
                    (draftPick.Groups[3].Success ? " (из команды " + draftPick.Groups[3].Value + ")" : "");
            }

            __0 = Regex.Replace(__0, @"^(\d+)(?:st|nd|rd|th) период$", "$1-й период");
            __0 = Regex.Replace(__0, @"^(\d+)(?:st|nd|rd|th) четверть$", "$1-я четверть");
            __0 = Regex.Replace(__0, @"^(\d+)(?:st|nd|rd|th) овертайм$", "$1-й овертайм");
            __0 = Regex.Replace(__0, @"^(Верх|Низ) (\d+)(?:st|nd|rd|th)$", "$1 $2-го");

            if (__0.StartsWith("Мы ", StringComparison.Ordinal) ||
                __0.StartsWith("ПЕРЕХОД: ", StringComparison.Ordinal) ||
                __0.StartsWith("ПОДПИСАНИЕ: ", StringComparison.Ordinal) ||
                __0.StartsWith("ТРАНСФЕР: ", StringComparison.Ordinal))
            {
                __0 = Regex.Replace(__0, @"\b(LW|RW|PG|SG|SF|PF|QB|RB|WR|TE|OL|DL|LB|CB|SP|RP|SS|LF|CF|RF|GK|DF|MF|FW)\b",
                    match => Exact.TryGetValue(match.Value, out string position) ? position : match.Value);
            }

            __0 = Regex.Replace(__0, @"\bTD\b", "ТД");
            __0 = Regex.Replace(__0, @"\bFG\b", "полевых голов");
            __0 = Regex.Replace(__0, @"\bGroup ([A-Z])\b", "Группа $1");
            __0 = Regex.Replace(__0, @"([A-Z][a-z]{2} \d{1,2}) to ([A-Z][a-z]{2} \d{1,2}, \d{4})", "$1 — $2");

            // Month abbreviations can appear inside complete date labels.
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