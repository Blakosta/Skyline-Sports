using System.Collections.Generic;

namespace Skyline_Sports_Project.Engine;

public class Team
{
	public int Id;

	public int OrgId;

	public Sport Sport;

	public Level Level;

	public int VenueId = -1;

	public string StadiumName = "";

	public int StadiumCapacity = 500;

	public int Rating = 50;

	public bool IsUser;

	public bool Retired;

	public int FeaturedMonth = -1;

	public int Rotation;

	public int Form;

	public int RunBonus;

	public bool Manual;

	public int Budget;

	public int Surprise;

	public int Cinderella;

	public int LastStarYear = int.MinValue;

	public int CinderellaYear = int.MinValue;

	public List<RetiredNumber> RetiredNumbers = new List<RetiredNumber>();

	public List<int> Block = new List<int>();

	public int Prestige;

	public int RecruitHours;

	public List<DeadCharge> Dead = new List<DeadCharge>();

	public int Buyouts;

	public int ParentTeamId = -1;

	public int TransferBudget = -1;

	public int ClubBalance = -1;

	public int SponsorTier;

	public int SponsorFrequency;

	public int SponsorLastPeriod = int.MinValue;

	public int TicketPriceResidents;

	public int TicketPriceVisitors;

	public int FarmPref;

	public int FoldedYear;

	public bool WatchFirst;

	public int LastLiveMonth = int.MinValue;
}
