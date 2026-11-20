using System.Collections.Generic;

namespace Skyline_Sports_Project.Engine;

public class SeasonSummary
{
	public string Season = "";

	public string Champion = "";

	public string RunnerUp = "";

	public string FinalScore = "";

	public bool ChampionIsUser;

	public List<string> Standings = new List<string>();

	public List<string> Playoffs = new List<string>();

	public List<string> Awards = new List<string>();

	public List<string> Leaders = new List<string>();

	public List<string> Notes = new List<string>();
}
