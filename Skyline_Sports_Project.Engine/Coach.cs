using System;

namespace Skyline_Sports_Project.Engine;

public class Coach
{
	public int Id;

	public string FirstName = "";

	public string LastName = "";

	public Sport Sport;

	public Level Level;

	public CoachRole Role;

	public int TeamId = -1;

	public int BirthYear;

	public int Development;

	public int Tactics;

	public int Motivation;

	public int Recruiting;

	public int Potential;

	public int PrevOverall;

	public int Seasons;

	public int Wins;

	public int Losses;

	public int Ties;

	public int PlayoffWins;

	public int PlayoffLosses;

	public int Titles;

	public int HiredYear;

	public int BadSeasons;

	public string History = "";

	public string Hometown = "";

	public int FormerPlayerId = -1;

	public string Played = "";

	public string Name => FirstName + " " + LastName;

	public int Overall
	{
		get
		{
			if (Role == CoachRole.Head)
			{
				return (int)Math.Round(0.3 * (double)Development + 0.35 * (double)Tactics + 0.2 * (double)Motivation + 0.15 * (double)Recruiting);
			}
			return (int)Math.Round(0.35 * (double)Development + 0.5 * (double)Tactics + 0.15 * (double)Motivation);
		}
	}
}
