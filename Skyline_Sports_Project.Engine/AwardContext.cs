using System;
using System.Collections.Generic;

namespace Skyline_Sports_Project.Engine;

public class AwardContext
{
	public int TeamGames;

	public int Year;

	public readonly Dictionary<int, double> WinPct = new Dictionary<int, double>();

	public readonly Dictionary<int, double> PointsPerGame = new Dictionary<int, double>();

	public int Half => Math.Max(1, TeamGames / 2);

	public double TeamWinPct(Player p)
	{
		if (!WinPct.TryGetValue(p.TeamId, out var v))
		{
			return 0.5;
		}
		return v;
	}
}
