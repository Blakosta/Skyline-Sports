using System;

namespace Skyline_Sports_Project.Engine;

public struct DateRange(DateTime from, DateTime to)
{
	public DateTime From = from;

	public DateTime To = to;

	public bool Contains(DateTime d)
	{
		if (d.Date >= From.Date)
		{
			return d.Date <= To.Date;
		}
		return false;
	}
}
