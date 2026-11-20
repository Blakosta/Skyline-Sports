using System;
using System.Collections.Generic;

namespace Skyline_Sports_Project.Engine;

public class PanelView
{
	public const string FeedPage = "#feed";

	public const string IntlPage = "#intl";

	public int TeamId = -1;

	public int PartnerId = -1;

	public int PlayerId = -1;

	public int GameId = -1;

	public bool Office;

	public string TradeMode = "";

	public List<int> Shop = new List<int>();

	public string Want = "";

	public string LeagueKey = "";

	public string Tab = "";

	public string TeamTab = "";

	public List<int> Give = new List<int>();

	public List<int> Get = new List<int>();

	public List<int> GivePicks = new List<int>();

	public List<int> GetPicks = new List<int>();

	public string Notice = "";

	public bool NoticeOk = true;

	public static PanelView Parse(string text)
	{
		PanelView v = new PanelView();
		if (string.IsNullOrEmpty(text))
		{
			return v;
		}
		string[] array = text.Split(new[] { ';' }, StringSplitOptions.None);
		foreach (string part in array)
		{
			int eq = part.IndexOf('=');
			if (eq <= 0)
			{
				continue;
			}
			string key = part.Substring(0, eq).Trim();
			string value = part.Substring(eq + 1).Trim();
			int n;
			switch (key)
			{
			case "team":
				if (int.TryParse(value, out n))
				{
					v.TeamId = n;
				}
				break;
			case "partner":
				if (int.TryParse(value, out n))
				{
					v.PartnerId = n;
				}
				break;
			case "player":
				if (int.TryParse(value, out n))
				{
					v.PlayerId = n;
				}
				break;
			case "game":
				if (int.TryParse(value, out n))
				{
					v.GameId = n;
				}
				break;
			case "office":
				v.Office = value == "1";
				break;
			case "tradeMode":
				v.TradeMode = value;
				break;
			case "shop":
				v.Shop = Ids(value);
				break;
			case "want":
				v.Want = value;
				break;
			case "league":
				v.LeagueKey = value;
				break;
			case "tab":
				v.Tab = value;
				break;
			case "teamTab":
				v.TeamTab = value;
				break;
			case "give":
				v.Give = Ids(value);
				break;
			case "get":
				v.Get = Ids(value);
				break;
			case "givePicks":
				v.GivePicks = Ids(value);
				break;
			case "getPicks":
				v.GetPicks = Ids(value);
				break;
			}
		}
		return v;
	}

	public static List<int> Ids(string csv)
	{
		List<int> list = new List<int>();
		if (string.IsNullOrEmpty(csv))
		{
			return list;
		}
		string[] array = csv.Split(new[] { ',' }, StringSplitOptions.None);
		for (int i = 0; i < array.Length; i++)
		{
			if (int.TryParse(array[i].Trim(), out var n) && !list.Contains(n))
			{
				list.Add(n);
			}
			if (list.Count >= 6)
			{
				break;
			}
		}
		return list;
	}

	public bool SamePage(PanelView other)
	{
		if (other != null && TeamId == other.TeamId && LeagueKey == other.LeagueKey && Tab == other.Tab && TeamTab == other.TeamTab && PartnerId == other.PartnerId && PlayerId == other.PlayerId && TradeMode == other.TradeMode && GameId == other.GameId)
		{
			return Office == other.Office;
		}
		return false;
	}
}
