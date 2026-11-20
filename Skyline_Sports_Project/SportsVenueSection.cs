using System;
using Colossal.UI.Binding;
using Game.UI;
using Game.UI.InGame;
using Skyline_Sports_Project.Engine;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Skyline_Sports_Project;

public class SportsVenueSection : InfoSectionBase
{
	private SportsDataSystem m_Data;

	private SelectedInfoUISystem m_SelectedInfo;

	private string m_Json = "{}";

	private string m_Message = "";

	private Entity m_MessageFor = Entity.Null;

	protected override string group => "SportsVenueSection";

	[Preserve]
	[Preserve]
	protected override void OnCreate()
	{
		((InfoSectionBase)this).OnCreate();
		m_Data = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SportsDataSystem>();
		m_SelectedInfo = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SelectedInfoUISystem>();
		((UISystemBase)this).AddBinding((IBinding)(object)new TriggerBinding<string>("SkylineSports", "VenueCommand", (Action<string>)OnVenueCommand, (IReader<string>)null));
	}

	[Preserve]
	protected override void Reset()
	{
		m_Json = "{}";
	}

	[Preserve]
	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		((InfoSectionBase)this).visible = m_Data != null && m_Data.IsBuilding(((InfoSectionBase)this).selectedEntity);
	}

	[Preserve]
	protected override void OnProcess()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (m_MessageFor != ((InfoSectionBase)this).selectedEntity)
			{
				m_Message = "";
			}
			int venueId = m_Data.VenueIdFor(((InfoSectionBase)this).selectedEntity);
			m_Json = Views.VenueSection(m_Data.Sports, venueId, m_Data.BuildingName(((InfoSectionBase)this).selectedEntity), m_Message, m_Data.NowOrLast());
		}
		catch (Exception ex)
		{
			Mod.Error("Sports section failed", ex);
			m_Json = "{}";
		}
	}

	[Preserve]
	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("data");
		writer.Write(m_Json);
	}

	private void OnVenueCommand(string command)
	{
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		CommandResult result = null;
		try
		{
			Entity building = ((InfoSectionBase)this).selectedEntity;
			if (!m_Data.IsBuilding(building))
			{
				return;
			}
			m_Data.ApplySettings();
			string[] parts = (command ?? "").Split('\u001f', StringSplitOptions.None);
			string name = ((parts.Length != 0) ? parts[0] : "");
			int venueId = m_Data.VenueIdFor(building);
			int now = m_Data.NowOrLast();
			SportsWorld sports = m_Data.Sports;
			switch (name)
			{
			case "make":
				if (!Enum.IsDefined(typeof(VenueType), IntArg(parts, 1)))
				{
					return;
				}
				result = m_Data.MakeVenue(building, (VenueType)IntArg(parts, 1));
				break;
			case "type":
				if (!Enum.IsDefined(typeof(VenueType), IntArg(parts, 1)))
				{
					return;
				}
				result = sports.SetVenueType(venueId, (VenueType)IntArg(parts, 1));
				break;
			case "level":
				if (!Enum.IsDefined(typeof(Level), IntArg(parts, 1)))
				{
					return;
				}
				result = sports.SetVenueLevel(venueId, (Level)IntArg(parts, 1));
				break;
			case "add":
				if (!Enum.IsDefined(typeof(Sport), IntArg(parts, 1)))
				{
					return;
				}
				result = sports.AddTeam(venueId, (Sport)IntArg(parts, 1), m_Data.NameContextFor(building), now);
				break;
			case "moveTeam":
				result = sports.MoveTeam(IntArg(parts, 1), venueId, m_Data.BuildingName(building), now);
				break;
			case "removeTeam":
				result = sports.RemoveTeam(IntArg(parts, 1), now);
				break;
			case "rename":
				result = sports.RenameOrg(IntArg(parts, 1), StrArg(parts, 2), StrArg(parts, 3));
				break;
			case "reroll":
				result = sports.RerollOrg(IntArg(parts, 1), m_Data.NameContextFor(building));
				break;
			case "removeVenue":
				result = m_Data.RemoveVenue(venueId);
				break;
			default:
				Mod.log.Warn((object)("Unknown venue command: " + name));
				return;
			}
			Mod.log.Info((object)("Venue command '" + name + "': " + (result.Ok ? "ok" : "refused") + ". " + result.Message));
			m_Message = result.Message;
			m_MessageFor = building;
		}
		catch (Exception ex)
		{
			Mod.Error("Venue command failed (" + command + ")", ex);
			m_Message = "Произошла ошибка. Подробности записаны в журнал Skyline Sports Project.";
			m_MessageFor = ((InfoSectionBase)this).selectedEntity;
		}
		m_SelectedInfo.RequestUpdate();
	}

	private static int IntArg(string[] parts, int index)
	{
		if (parts.Length > index && int.TryParse(parts[index], out var value))
		{
			return value;
		}
		return -1;
	}

	private static string StrArg(string[] parts, int index)
	{
		if (parts.Length <= index)
		{
			return "";
		}
		return parts[index];
	}

	[Preserve]
	public SportsVenueSection()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
	}
}
