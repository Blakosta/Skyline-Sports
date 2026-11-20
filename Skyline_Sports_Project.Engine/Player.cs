using System.Collections.Generic;

namespace Skyline_Sports_Project.Engine;

public class Player
{
	public int Id;

	public int TeamId;

	public Sport Sport;

	public Level Level;

	public PlayerStatus Status;

	public string FirstName = "";

	public string LastName = "";

	public Position Position;

	public int Number;

	public int BirthYear;

	public int ClassYear;

	public int Overall;

	public int Potential;

	public int PrevOverall;

	public int Seasons;

	public int Salary;

	public int ContractYears;

	public int FreeAgentMonth = -1;

	public int DraftYear;

	public int DraftRound;

	public int DraftPick;

	public int DraftTeamId = -1;

	public string From = "";

	public int[] Season = new int[0];

	public int[] LastSeason = new int[0];

	public int[] Career = new int[0];

	public int[] Playoff = new int[0];

	public PlayerPriority Priority;

	public int LastTeamId = -1;

	public int InjuredUntil = int.MinValue;

	public string Injury = "";

	public List<string> InjuryLog = new List<string>();

	public int Height;

	public int Weight;

	public string Hometown = "";

	public string Hand = "";

	public int Titles;

	public int PeakOvr;

	public List<string> Honors = new List<string>();

	public List<SeasonLine> History = new List<SeasonLine>();

	public int DecideMonth = -1;

	public int RetiredYear;

	public string Name => FirstName + " " + LastName;

	public string ShortName => ((FirstName.Length > 0) ? (FirstName.Substring(0, 1) + ". ") : "") + LastName;
}
