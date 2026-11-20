using System;
using System.Reflection;
using System.Text.RegularExpressions;
using HarmonyLib;

namespace SkylineSportsRussianLocalization
{
    /// <summary>
    /// Covers dashboard labels and complete mixed-language team cards that are not
    /// represented by standalone dictionary entries in the original UI.
    /// </summary>
    internal static class AdditionalUiTranslations
    {
        private const string HarmonyId = "Blakosta.SkylineSports.RussianLocalization.Dashboard";
        private static Harmony _harmony;
        private static bool _patched;

        public static void Install()
        {
            if (_harmony != null) return;
            _harmony = new Harmony(HarmonyId);
            AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                TryPatch(assembly);
        }

        public static void Uninstall()
        {
            AppDomain.CurrentDomain.AssemblyLoad -= OnAssemblyLoad;
            if (_harmony != null) _harmony.UnpatchAll(HarmonyId);
            _harmony = null;
            _patched = false;
        }

        private static void OnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            TryPatch(args.LoadedAssembly);
        }

        private static void TryPatch(Assembly assembly)
        {
            if (_patched || assembly == null) return;
            Type builder = assembly.GetType("Skyline_Sports_Project.Engine.JsonBuilder", false);
            if (builder == null) return;
            MethodInfo target = builder.GetMethod("Value", BindingFlags.Public | BindingFlags.Instance,
                null, new[] { typeof(string) }, null);
            if (target == null) return;
            MethodInfo prefix = typeof(AdditionalUiTranslations).GetMethod(
                nameof(Translate), BindingFlags.NonPublic | BindingFlags.Static);
            _harmony.Patch(target, prefix: new HarmonyMethod(prefix) { priority = Priority.First });
            _patched = true;
            UnityEngine.Debug.Log("[Skyline Sports RU] Dashboard translation attached.");
        }

        private static void Translate(ref string __0)
        {
            if (string.IsNullOrEmpty(__0)) return;
            string s = null;
            switch (__0)
            {
                case "My teams": s = "Мои команды"; break;
                case "Front office": s = "Руководство клуба"; break;
                case "Sports feed": s = "Спортивная лента"; break;
                case "International": s = "Международные турниры"; break;
                case "LEAGUES": s = "ЛИГИ"; break;
                case "PRO": s = "ПРОФЕССИОНАЛЬНЫЕ ЛИГИ"; break;
                case "COLLEGE": s = "СТУДЕНЧЕСКИЕ ЛИГИ"; break;
                case "PRO · 2 TEAMS": s = "ПРОФЕССИОНАЛЬНЫЕ · 2 КОМАНДЫ"; break;
                case "COLLEGE · 1 TEAM": s = "СТУДЕНЧЕСКИЕ · 1 КОМАНДА"; break;
                case "Football": s = "Американский футбол"; break;
                case "Hockey": s = "Хоккей"; break;
                case "Results come in when the month ends.": s = "Результаты появятся в конце месяца."; break;
                case "AI general manager": s = "ИИ-генеральный менеджер"; break;
                case "Calendar": s = "Календарь"; break;
                case "Regular season starts:": s = "Начало регулярного сезона:"; break;
                case "Tonight": s = "Сегодня вечером"; break;
                case "Winter": s = "Зима"; break;
            }
            if (s != null) { __0 = s; return; }

            s = __0;
            s = s.Replace("Head coach ", "Главный тренер ");
            s = s.Replace("Game of the month:", "Матч месяца:");
            s = s.Replace("Next:", "Следующий матч:");
            s = s.Replace("Tonight at ", "Сегодня в ");
            s = s.Replace(" vs ", " против ");
            s = s.Replace(" at ", " в гостях у ");
            s = s.Replace("University of ", "Университет ");
            s = s.Replace("Football · OVR ", "Американский футбол · ОБЩ ");
            s = s.Replace("Hockey · OVR ", "Хоккей · ОБЩ ");
            s = s.Replace(" more games this month.", " матчей осталось в этом месяце.");
            if (s.StartsWith("The ", StringComparison.Ordinal)) s = s.Substring(4);
            s = s.Replace(" season starts ", " сезон начинается ");
            s = Regex.Replace(s, @"\b(Sun|Sunday)\b", "воскресенье", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"\b(Mon|Monday)\b", "понедельник", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"\b(Tue|Tuesday)\b", "вторник", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"\b(Wed|Wednesday)\b", "среду", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"\b(Thu|Thursday)\b", "четверг", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"\b(Fri|Friday)\b", "пятницу", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"\b(Sat|Saturday)\b", "субботу", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"\bSeptember\b", "сентября", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"\bOctober\b", "октября", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"\bNovember\b", "ноября", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"\bDecember\b", "декабря", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"\bJanuary\b", "января", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"\bFebruary\b", "февраля", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"\bMarch\b", "марта", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"\bApril\b", "апреля", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"\bMay\b", "мая", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"\bJune\b", "июня", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"\bJuly\b", "июля", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"\bAugust\b", "августа", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"\b(\d{1,2})(:\d{2})? вечера\b", m =>
            {
                int hour = int.Parse(m.Groups[1].Value);
                if (hour < 12) hour += 12;
                return hour.ToString("00") + (m.Groups[2].Success ? m.Groups[2].Value : ":00");
            });
            s = Regex.Replace(s, @"\b(\d{1,2})(:\d{2})? утра\b", m =>
            {
                int hour = int.Parse(m.Groups[1].Value);
                if (hour == 12) hour = 0;
                return hour.ToString("00") + (m.Groups[2].Success ? m.Groups[2].Value : ":00");
            });
            if (!string.Equals(s, __0, StringComparison.Ordinal)) __0 = s;
        }
    }
}
