namespace Skyline_Sports_Project.Engine;

public class Slot
{
	public SlotTemplate Template;

	public int[] Days;

	public bool Optional;

	public int Size => Days.Length;

	public int FirstDay => Days[0];
}
