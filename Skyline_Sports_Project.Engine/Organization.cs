namespace Skyline_Sports_Project.Engine;

public class Organization
{
	public int Id;

	public Level Level;

	public string Name = "";

	public string Mascot = "";

	public string DistrictKey = "";

	public bool IsUser;

	public string FullName
	{
		get
		{
			if (!string.IsNullOrEmpty(Mascot))
			{
				return Name + " " + Mascot;
			}
			return Name;
		}
	}
}
