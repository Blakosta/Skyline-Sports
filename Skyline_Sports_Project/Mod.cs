using System;
using System.Collections.Generic;
using System.Reflection;
using Colossal;
using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using Game.UI.InGame;
using Unity.Entities;

namespace Skyline_Sports_Project;

public class Mod : IMod
{
	public static ILog log = LogManager.GetLogger("Skyline_Sports_Project").SetShowsErrorsInUI(true);

	private static readonly Dictionary<string, int> s_Errors = new Dictionary<string, int>();

	public static Setting Settings { get; private set; }

	public static void Error(string what, Exception ex)
	{
		string key = what + "|" + ex.GetType().Name + "|" + ex.Message;
		s_Errors.TryGetValue(key, out var n);
		n = (s_Errors[key] = n + 1);
		if (n == 1)
		{
			log.Error((object)(what + ": " + ex));
		}
		else if (n % 100 == 0)
		{
			log.Warn((object)(what + " (the same error " + n + " times): " + ex.Message));
		}
	}

	public void OnLoad(UpdateSystem updateSystem)
	{
		log.Info((object)"OnLoad");
		ExecutableAsset asset = default;
		if (GameManager.instance.modManager.TryGetExecutableAsset((IMod)(object)this, ref asset))
		{
			log.Info((object)("Current mod asset at " + ((AssetData)asset).path));
		}
		Settings = new Setting((IMod)(object)this);
		((ModSetting)Settings).RegisterInOptionsUI();
		GameManager.instance.localizationManager.AddSource("en-US", (IDictionarySource)(object)new LocaleEN(Settings));
		AssetDatabase.global.LoadSettings("Skyline_Sports_Project", (object)Settings, (object)new Setting((IMod)(object)this), false);
		updateSystem.UpdateAt<SportsDataSystem>((SystemUpdatePhase)12);
		updateSystem.UpdateAt<SportsUISystem>((SystemUpdatePhase)22);
		AddVenueSection();
	}

	public void OnDispose()
	{
		log.Info((object)"OnDispose");
		if (Settings != null)
		{
			((ModSetting)Settings).UnregisterInOptionsUI();
			Settings = null;
		}
	}

	private static void AddVenueSection()
	{
		try
		{
			World defaultGameObjectInjectionWorld = World.DefaultGameObjectInjectionWorld;
			SportsVenueSection section = defaultGameObjectInjectionWorld.GetOrCreateSystemManaged<SportsVenueSection>();
			SelectedInfoUISystem selectedInfo = defaultGameObjectInjectionWorld.GetOrCreateSystemManaged<SelectedInfoUISystem>();
			FieldInfo field = typeof(SelectedInfoUISystem).GetField("m_MiddleSections", BindingFlags.Instance | BindingFlags.NonPublic);
			List<ISectionSource> middle = ((field == null) ? null : (field.GetValue(selectedInfo) as List<ISectionSource>));
			if (middle == null)
			{
				log.Error((object)"Could not find the building panel's section list. The Sports section won't show.");
				return;
			}
			middle.Insert(0, (ISectionSource)(object)section);
			log.Info((object)"Sports section added to the building panel.");
		}
		catch (Exception ex)
		{
			log.Error((object)("Adding the Sports section failed: " + ex));
		}
	}
}