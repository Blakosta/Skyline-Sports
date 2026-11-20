using System.Collections.Generic;

namespace Skyline_Sports_Project.Engine;

public class BoxEvent
{
	public int Period;

	public string Clock = "";

	public bool Home;

	public int Kind;

	public string Text = "";

	public int HomeScore;

	public int AwayScore;

	public List<int> Players = new List<int>();
}
