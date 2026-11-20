namespace Skyline_Sports_Project.Engine;

public class LiveSnapshot
{
	public double Progress;

	public string Status = "";

	public int HomeScore;

	public int AwayScore;

	public GameBox Box = new GameBox();

	public int DriveHome = -1;

	public int DrivePlays;

	public int DriveYards;

	public int DriveYard;
}
