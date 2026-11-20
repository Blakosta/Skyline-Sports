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
            { "American Samoa", "Американское Самоа" }
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

            // Translate dynamic announcements while leaving player and team names intact.
            __0 = __0.Replace("FOLDED: The ", "РАСПУСК: ")
                     .Replace(" have folded after ", " завершили выступления после сезона ")
                     .Replace("NATIONAL TEAMS: the ", "СБОРНЫЕ: ")
                     .Replace(" starts ", " начинают выступление ")
                     .Replace(" Your players called up: ", " Ваши игроки вызваны в сборную: ")
                     .Replace(" and ", " и ")
                     .Replace(" more", " ещё")
                     .Replace("FLIP: ", "ПЕРЕМАНИЛИ: ")
                     .Replace("COMMIT: ", "ПОДПИСАНИЕ: ")
                     .Replace(" flips from ", " переходит из ")
                     .Replace(" commits to ", " выбирает команду ")
                     .Replace("TRANSFER: The ", "ТРАНСФЕР: ")
                     .Replace(" sign ", " подписывает ")
                     .Replace(" from the ", " из команды ")
                     .Replace("Bats ", "Бьёт: ")
                     .Replace(", throws ", ", бросает: ")
                     .Replace("The qualifiers decide the last spots.", "Последние места определяются по итогам отборочного турнира.")
                     .Replace("through the qualifiers)", "по итогам отборочного турнира)")
                     .Replace("player who grew up in your city", "игрок, выросший в вашем городе")
                     .Replace("players who grew up in your city", "игроки, выросшие в вашем городе")
                     .Replace("Your players called up:", "Ваши игроки вызваны в сборную:")
                     .Replace("free agency", "свободные агенты")
                     .Replace("round pick", "выбор раунда")
                     .Replace("from clubs abroad", "из зарубежных клубов")
                     .Replace(" plays in the qualifiers", " участвует в отборочном турнире")
                     .Replace(" is in the main event", " участвует в основном турнире")
                     .Replace(" nations", " сборных")
                     .Replace("We have ", "У нас ")
                     .Replace(". Thank you!", ". Спасибо!")
                     .Replace(" Last season ", " В прошлом сезоне ");

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
