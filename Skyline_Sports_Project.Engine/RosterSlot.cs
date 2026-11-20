namespace Skyline_Sports_Project.Engine;

public class RosterSlot
{
	public Position Pos;

	public int Count;

	public int Starters;

	public RosterSlot(Position pos, int count, int starters)
	{
		Pos = pos;
		Count = count;
		Starters = starters;
	}
}
