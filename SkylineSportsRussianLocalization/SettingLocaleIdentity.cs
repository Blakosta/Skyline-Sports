using Game.Modding;
using Game.Settings;

namespace Skyline_Sports_Project
{
    // Locale-key identity mirror only. No settings are registered and no gameplay code is changed.
    public sealed class Setting : ModSetting
    {
        public enum TemperatureUnit { SameAsGame, Celsius, Fahrenheit }

        public const string kSection = "Main";

        public int LeagueSize { get; set; }
        public int SeasonGames { get; set; }
        public int InjuryFrequency { get; set; }
        public bool ShowLeagueButton { get; set; }
        public TemperatureUnit Temperature { get; set; }
        public bool RealFans { get; set; }
        public int RealFansCount { get; set; }
        public bool VisitorFans { get; set; }
        public bool GameDayTourism { get; set; }
        public bool ResetAllData { get; set; }
        public string Version { get; set; }

        public Setting(IMod mod) : base(mod) { }
    }
}