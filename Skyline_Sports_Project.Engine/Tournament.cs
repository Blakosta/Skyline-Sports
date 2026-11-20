using System.Collections.Generic;

namespace Skyline_Sports_Project.Engine;

public class Tournament
{
	public int Id;

	public Sport Sport;

	public string Name = "";

	public int Year;

	public int StartDay;

	public bool Under20;

	public string Host = "";

	public bool Complete;

	public string Gold = "";

	public string Silver = "";

	public string Bronze = "";

	public List<NationTeam> Teams = new List<NationTeam>();

	public List<TourGame> Games = new List<TourGame>();

	public string Label => Year + " " + Name;
}
