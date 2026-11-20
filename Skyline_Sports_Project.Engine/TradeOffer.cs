using System.Collections.Generic;

namespace Skyline_Sports_Project.Engine;

public class TradeOffer
{
	public int Id;

	public int FromTeamId;

	public int ToTeamId;

	public List<int> Give = new List<int>();

	public List<int> GivePicks = new List<int>();

	public List<int> Get = new List<int>();

	public int Month;
}
