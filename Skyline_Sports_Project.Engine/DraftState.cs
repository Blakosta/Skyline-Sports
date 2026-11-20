using System.Collections.Generic;

namespace Skyline_Sports_Project.Engine;

public class DraftState
{
	public int Year;

	public bool LotteryDone;

	public bool Started;

	public bool Complete;

	public int Rounds;

	public int Month = -1;

	public List<string> Lottery = new List<string>();

	public List<int> Order = new List<int>();

	public List<DraftPick> Picks = new List<DraftPick>();

	public DraftPick NextPick
	{
		get
		{
			foreach (DraftPick p in Picks)
			{
				if (p.PlayerId < 0)
				{
					return p;
				}
			}
			return null;
		}
	}
}
