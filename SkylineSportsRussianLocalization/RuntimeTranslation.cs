using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            { "Single season", "Один сезон" },
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

            // Translate common dynamic phrases without touching player/team names or numeric data.
            __0 = __0.Replace("The qualifiers decide the last spots.", "Последние места определяются по итогам отборочного турнира.")
                     .Replace("through the qualifiers)", "по итогам отборочного турнира)")
                     .Replace("player who grew up in your city", "игрок, выросший в вашем городе")
                     .Replace("players who grew up in your city", "игроки, выросшие в вашем городе")
                     .Replace("Your players called up:", "Ваши игроки вызваны в сборную:")
                     .Replace("more", "ещё")
                     .Replace("spot", "место")
                     .Replace("free agency", "свободные агенты")
                     .Replace("round pick", "выбор раунда")
                     .Replace("from clubs abroad", "из зарубежных клубов");
        }
    }
}
