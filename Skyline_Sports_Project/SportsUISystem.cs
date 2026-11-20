
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Colossal.UI.Binding;
using Game.SceneFlow;
using Game.UI;
using Game.UI.InGame;
using Skyline_Sports_Project.Engine;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Skyline_Sports_Project;

[CompilerGenerated]
public class SportsUISystem : UISystemBase
{
	private SportsDataSystem m_Data;
	private SelectedInfoUISystem m_SelectedInfo;
	private ValueBinding<string> m_PanelData;
	private ValueBinding<bool> m_ShowButton;
	private int m_LastRevision = int.MinValue;
	private int m_LastDay = int.MinValue;
	private int m_LastLiveTick = int.MinValue;
	private float m_Timer;
	private bool m_PanelOpen;
	private PanelView m_View = new PanelView();
	private bool m_UnitsUnreadable;
	private PropertyInfo m_UiSettingsProp;
	private MemberInfo m_UnitMember;

	[Preserve]
	protected override void OnCreate()
	{
		((UISystemBase)this).OnCreate();
		m_Data = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SportsDataSystem>();
		m_SelectedInfo = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SelectedInfoUISystem>();
		((UISystemBase)this).AddBinding((IBinding)(object)(m_PanelData = new ValueBinding<string>("SkylineSports", "PanelData", "{}", (IWriter<string>)null, (EqualityComparer<string>)null)));
		((UISystemBase)this).AddBinding((IBinding)(object)(m_ShowButton = new ValueBinding<bool>("SkylineSports", "ShowButton", true, (IWriter<bool>)null, (EqualityComparer<bool>)null)));
		((UISystemBase)this).AddBinding((IBinding)(object)new TriggerBinding<string>("SkylineSports", "SetCityName", (Action<string>)OnSetCityName, (IReader<string>)null));
		((UISystemBase)this).AddBinding((IBinding)(object)new TriggerBinding<bool>("SkylineSports", "SetPanelOpen", (Action<bool>)OnSetPanelOpen, (IReader<bool>)null));
		((UISystemBase)this).AddBinding((IBinding)(object)new TriggerBinding<string>("SkylineSports", "SetView", (Action<string>)OnSetView, (IReader<string>)null));
		((UISystemBase)this).AddBinding((IBinding)(object)new TriggerBinding<string>("SkylineSports", "PanelCommand", (Action<string>)OnPanelCommand, (IReader<string>)null));
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Invalid comparison between Unknown and I4
		((UISystemBase)this).OnUpdate();
		if ((Object)(object)GameManager.instance == (Object)null || (int)GameManager.instance.gameMode != 2)
		{
			return;
		}
		m_Timer += Time.unscaledDeltaTime;
		if (m_Timer < 0.5f)
		{
			return;
		}
		m_Timer = 0f;
		try
		{
			Setting settings = Mod.Settings;
			m_ShowButton.Update(settings?.ShowLeagueButton ?? true);
			bool celsius = UseCelsius(settings);
			if (celsius != WeatherCodes.UseCelsius)
			{
				WeatherCodes.UseCelsius = celsius;
				m_LastRevision = int.MinValue;
			}
			int day = (m_Data.TryGetNow(out var now) ? GameClock.DayOf(now) : int.MinValue);
			int revision = m_Data.Sports.Revision;
			int liveTick = m_Data.Sports.LiveTick;
			if (revision != m_LastRevision || day != m_LastDay || liveTick != m_LastLiveTick)
			{
				bool num = revision != m_LastRevision || liveTick != m_LastLiveTick;
				m_LastRevision = revision;
				m_LastDay = day;
				m_LastLiveTick = liveTick;
				if (m_PanelOpen)
				{
					RefreshPanel();
				}
				if (num)
				{
					m_SelectedInfo.RequestUpdate();
				}
			}
		}
		catch (Exception ex)
		{
			Mod.Error("Sports panel update failed", ex);
		}
	}

	private bool UseCelsius(Setting settings)
	{
		if (settings != null && settings.Temperature == Setting.TemperatureUnit.Celsius)
		{
			return true;
		}
		if (settings != null && settings.Temperature == Setting.TemperatureUnit.Fahrenheit)
		{
			return false;
		}
		if (m_UnitsUnreadable)
		{
			return false;
		}
		try
		{
			if ((Object)(object)GameManager.instance == (Object)null)
			{
				return false;
			}
			object shared = typeof(GameManager).GetProperty("settings")?.GetValue(GameManager.instance, null) ?? typeof(GameManager).GetField("settings")?.GetValue(GameManager.instance);
			if (shared == null)
			{
				return false;
			}
			if (m_UiSettingsProp == null)
			{
				m_UiSettingsProp = shared.GetType().GetProperty("userInterface", BindingFlags.Instance | BindingFlags.Public);
			}
			object ui = m_UiSettingsProp?.GetValue(shared, null);
			if (ui == null)
			{
				m_UnitsUnreadable = true;
				Mod.log.Info((object)"Temperature: the game's unit setting wasn't found, showing Fahrenheit (pick Celsius in Options).");
				return false;
			}
			if (m_UnitMember == null)
			{
				m_UnitMember = (MemberInfo)(((object)ui.GetType().GetProperty("unitSystem", BindingFlags.Instance | BindingFlags.Public)) ?? ((object)ui.GetType().GetField("unitSystem", BindingFlags.Instance | BindingFlags.Public)));
			}
			object unit;
			if (m_UnitMember is PropertyInfo)
			{
				unit = ((PropertyInfo)m_UnitMember).GetValue(ui, null);
			}
			else
			{
				unit = ((m_UnitMember is FieldInfo) ? ((FieldInfo)m_UnitMember).GetValue(ui) : null);
			}
			if (unit == null)
			{
				m_UnitsUnreadable = true;
				Mod.log.Info((object)"Temperature: the game's unit setting wasn't found, showing Fahrenheit (pick Celsius in Options).");
				return false;
			}
			return unit.ToString() == "Metric";
		}
		catch (Exception ex)
		{
			m_UnitsUnreadable = true;
			Mod.log.Info((object)("Temperature: couldn't read the game's unit setting (" + ex.Message + "), showing Fahrenheit."));
			return false;
		}
	}

	private void RefreshPanel()
	{
		bool timeKnown = m_Data.TryGetNow(out var now);
		m_PanelData.Update(Views.LeaguePanel(m_Data.Sports, timeKnown ? now : m_Data.NowOrLast(), timeKnown, m_View));
	}

	private void OnSetPanelOpen(bool open)
	{
		m_PanelOpen = open;
		if (!open)
		{
			return;
		}
		try
		{
			RefreshPanel();
		}
		catch (Exception ex)
		{
			Mod.Error("Sports panel update failed", ex);
		}
	}

	private void OnSetView(string view)
	{
		PanelView next = PanelView.Parse(view);
		if (!next.SamePage(m_View))
		{
			next.Notice = "";
		}
		else
		{
			next.Notice = m_View.Notice;
			next.NoticeOk = m_View.NoticeOk;
		}
		m_View = next;
		if (!m_PanelOpen)
		{
			return;
		}
		try
		{
			RefreshPanel();
		}
		catch (Exception ex)
		{
			Mod.Error("Sports panel update failed", ex);
		}
	}

	private void OnPanelCommand(string command)
	{
		try
		{
			string[] a = (command ?? "").Split('\u001f', StringSplitOptions.None);
			int now = m_Data.NowOrLast();
			SportsWorld w = m_Data.Sports;
			CommandResult r = a[0] switch
			{
				"manage" => w.SetManaged(Int(a, 1), Int(a, 2) == 1, now),
				"resign" => w.Resign(Int(a, 1), now),
				"release" => w.Release(Int(a, 1), now, a.Length > 2 && a[2] == "stretch"),
				"sign" => w.SignFreeAgent(Int(a, 1), Int(a, 2), now),
				"trade" => w.ProposeTrade(Int(a, 1), Int(a, 2), Ids(a, 3), Ids(a, 4), Ids(a, 5), Ids(a, 6), now),
				"fireCoach" => w.FireCoach(Int(a, 1), now),
				"hireCoach" => w.HireCoach(Int(a, 1), Int(a, 2), Int(a, 3), now),
				"draft" => w.DraftPlayer(Int(a, 1), Int(a, 2), now),
				"autopick" => w.AutoPick(Int(a, 1), now),
				"offer" => w.OfferContract(Int(a, 1), Int(a, 2), Int(a, 3), Int(a, 4), now),
				"acceptAsk" => w.AcceptAsk(Int(a, 1), Int(a, 2), now),
				"block" => w.ToggleBlock(Int(a, 1), now),
				"acceptOffer" => w.AcceptOffer(Int(a, 1), now),
				"declineOffer" => w.DeclineOffer(Int(a, 1)),
				"recruit" => w.Recruit(Int(a, 1), Int(a, 2), (a.Length > 3) ? a[3] : "", now),
				"callUp" => w.CallUp(Int(a, 1), Int(a, 2), now),
				"watchFirst" => w.ToggleWatchFirst(Int(a, 1), now),
				"farmPref" => w.SetFarmPref(Int(a, 1), Int(a, 2), now),
				"sendDown" => w.SendDown(Int(a, 1), Int(a, 2), now),
				"bid" => w.BidForPlayer(Int(a, 1), Int(a, 2), Int(a, 3), now),
				"acceptTransfer" => w.AcceptTransferOffer(Int(a, 1), now),
				"declineTransfer" => w.DeclineTransferOffer(Int(a, 1)),
				"country" => w.SetCountry((a.Length > 1) ? a[1] : "", (a.Length > 2) ? a[2] : "", now),
				_ => CommandResult.Fail("Неизвестная команда."),
			};
			m_View.Notice = r.Message;
			m_View.NoticeOk = r.Ok;
			if (m_PanelOpen)
			{
				RefreshPanel();
			}
			m_SelectedInfo.RequestUpdate();
		}
		catch (Exception ex)
		{
			Mod.Error("Sports panel command failed", ex);
		}
	}

	private static int Int(string[] a, int i)
	{
		if (i >= a.Length || !int.TryParse(a[i], out var v))
		{
			return -1;
		}
		return v;
	}

	private static List<int> Ids(string[] a, int i)
	{
		List<int> list = new List<int>();
		if (i >= a.Length)
		{
			return list;
		}
		string[] array = a[i].Split(',', StringSplitOptions.None);
		for (int j = 0; j < array.Length; j++)
		{
			if (int.TryParse(array[j], out var v))
			{
				list.Add(v);
			}
		}
		return list;
	}

	private void OnSetCityName(string cityName)
	{
		string clean = NameGenerator.CleanName(cityName, 40);
		if (clean.Length > 0)
		{
			m_Data.CityName = clean;
		}
	}

	[Preserve]
	public SportsUISystem()
	{
	}
}
