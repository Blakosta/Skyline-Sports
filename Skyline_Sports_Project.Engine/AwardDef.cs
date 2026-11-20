using System;

namespace Skyline_Sports_Project.Engine;

public class AwardDef
{
	public string Award;

	public string Trophy;

	public bool Playoff;

	public bool Headline;

	public Func<AwardContext, Player, int[], bool> Eligible;

	public Func<AwardContext, Player, int[], double> Score;

	public Func<AwardContext, Player, int[], string> Detail;
}
