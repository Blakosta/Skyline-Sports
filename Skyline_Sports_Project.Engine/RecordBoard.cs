using System.Collections.Generic;

namespace Skyline_Sports_Project.Engine;

public class RecordBoard
{
	public string Group = "";

	public string Title = "";

	public bool LowerIsBetter;

	public List<RecordEntry> Entries = new List<RecordEntry>();
}
