using System.Collections.Generic;

namespace Skyline_Sports_Project.Engine;

public class NationTeam
{
	public string Nation = "";

	public int Rating = 50;

	public int Group = -1;

	public bool Qualifier;

	public List<int> PlayerIds = new List<int>();

	public int Unnamed;

	internal double Strength;
}
