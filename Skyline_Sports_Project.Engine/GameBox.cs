using System.Collections.Generic;

namespace Skyline_Sports_Project.Engine;

public class GameBox
{
	public List<BoxLine> Lines = new List<BoxLine>();

	public List<BoxEvent> Events = new List<BoxEvent>();

	public List<int> HomePeriods = new List<int>();

	public List<int> AwayPeriods = new List<int>();

	public string Note = "";

	public List<int> Flow = new List<int>();
}
