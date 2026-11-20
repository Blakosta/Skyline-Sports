using System;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.SceneFlow;
using Game.Settings;
using Unity.Entities;
using UnityEngine;

namespace Skyline_Sports_Project;

[FileLocation("ModsSettings/Skyline_Sports_Project/Skyline_Sports_Project")]
[SettingsUIGroupOrder(new string[] { "Leagues", "Fans", "Display", "Data", "About" })]
[SettingsUIShowGroupName(new string[] { "Leagues", "Fans", "Display", "Data", "About" })]
public class Setting : ModSetting
{
	public enum TemperatureUnit
	{
		SameAsGame,
		Celsius,
		Fahrenheit
	}

	public const string kSection = "Main";

	public const string kLeagueGroup = "Leagues";

	public const string kDisplayGroup = "Display";

	public const string kDataGroup = "Data";

	public const string kFansGroup = "Fans";

	public const string kAboutGroup = "About";

	public const string ModVersion = "1.0.0";

	[SettingsUISection("Main", "Leagues")]
	[SettingsUISlider(min = 4f, max = 20f, step = 2f, scalarMultiplier = 1f, unit = "integer")]
	public int LeagueSize { get; set; }

	[SettingsUISection("Main", "Leagues")]
	[SettingsUISlider(min = 16f, max = 20f, step = 2f, scalarMultiplier = 1f, unit = "integer")]
	public int SoccerLeagueSize { get; set; }

	[SettingsUISection("Main", "Leagues")]
	[SettingsUISlider(min = 25f, max = 100f, step = 25f, scalarMultiplier = 1f, unit = "percentage")]
	public int SeasonGames { get; set; }

	[SettingsUISection("Main", "Leagues")]
	[SettingsUISlider(min = 0f, max = 200f, step = 25f, scalarMultiplier = 1f, unit = "percentage")]
	public int InjuryFrequency { get; set; }

	[SettingsUISection("Main", "Display")]
	public bool ShowLeagueButton { get; set; }

	[SettingsUISection("Main", "Display")]
	public TemperatureUnit Temperature { get; set; }

	[SettingsUISection("Main", "Fans")]
	public bool RealFans { get; set; }

	[SettingsUISection("Main", "Fans")]
	[SettingsUISlider(min = 50f, max = 3000f, step = 50f, scalarMultiplier = 1f, unit = "integer")]
	public int RealFansCount { get; set; }

	[SettingsUISection("Main", "Fans")]
	public bool VisitorFans { get; set; }

	[SettingsUISection("Main", "Fans")]
	public bool GameDayTourism { get; set; }

	[SettingsUIButton]
	[SettingsUIConfirmation(null, null)]
	[SettingsUISection("Main", "Data")]
	public bool ResetAllData
	{
		set
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Invalid comparison between Unknown and I4
			if ((UnityEngine.Object)(object)GameManager.instance == (UnityEngine.Object)null || (int)GameManager.instance.gameMode != 2)
			{
				Mod.log.Info((object)"Сброс пропущен: город ещё не загружен.");
				return;
			}
			World defaultGameObjectInjectionWorld = World.DefaultGameObjectInjectionWorld;
			((defaultGameObjectInjectionWorld != null) ? defaultGameObjectInjectionWorld.GetExistingSystemManaged<SportsDataSystem>() : null)?.RequestReset();
		}
	}

	[SettingsUISection("Main", "About")]
	public string Version => "1.0.0";

	public Setting(IMod mod)
		: base(mod)
	{
		((Setting)this).SetDefaults();
	}

	public override void ApplyAndSave()
	{
		((Setting)this).Apply();
		try
		{
			AssetDatabase.global.SaveAllSettings();
		}
		catch (Exception ex)
		{
			Mod.log.Error((object)("Не удалось сохранить настройки мода «Спорт России»: " + ex));
		}
	}

	public override void SetDefaults()
	{
		LeagueSize = 8;
		SoccerLeagueSize = 18;
		SeasonGames = 100;
		InjuryFrequency = 100;
		ShowLeagueButton = true;
		Temperature = TemperatureUnit.SameAsGame;
		RealFans = true;
		RealFansCount = 500;
		VisitorFans = false;
		GameDayTourism = false;
	}
}
