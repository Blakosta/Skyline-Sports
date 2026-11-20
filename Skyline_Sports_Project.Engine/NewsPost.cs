namespace Skyline_Sports_Project.Engine;

public class NewsPost
{
	public int Id;

	public int Time;

	public string LeagueKey = "";

	public NewsKind Kind;

	public string Account = "";

	public string Handle = "";

	public string Text = "";

	public bool IsUser;

	public int Likes;

	public int Reposts;
}
