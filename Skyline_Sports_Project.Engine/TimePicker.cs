using System;
using System.Collections.Generic;

namespace Skyline_Sports_Project.Engine;

public class TimePicker
{
	private readonly Random _random;

	private readonly Dictionary<string, int> _used = new Dictionary<string, int>();

	public TimePicker(Random random)
	{
		_random = random;
	}

	public TimeOption Pick(string scope, IList<TimeOption> options)
	{
		if (options == null || options.Count == 0)
		{
			return null;
		}
		double total = 0.0;
		List<int> open = new List<int>();
		for (int i = 0; i < options.Count; i++)
		{
			int used = Used(scope, options[i]);
			if (options[i].Cap <= 0 || used < options[i].Cap)
			{
				open.Add(i);
				total += Math.Max(0.0001, options[i].Weight);
			}
		}
		int chosen;
		if (open.Count == 0)
		{
			chosen = 0;
			int best = int.MaxValue;
			for (int j = 0; j < options.Count; j++)
			{
				int used2 = Used(scope, options[j]);
				if (used2 < best)
				{
					best = used2;
					chosen = j;
				}
			}
		}
		else
		{
			double r = _random.NextDouble() * total;
			chosen = open[open.Count - 1];
			foreach (int i2 in open)
			{
				r -= Math.Max(0.0001, options[i2].Weight);
				if (r <= 0.0)
				{
					chosen = i2;
					break;
				}
			}
		}
		string key = Key(scope, options[chosen]);
		_used.TryGetValue(key, out var count);
		_used[key] = count + 1;
		return options[chosen];
	}

	private int Used(string scope, TimeOption o)
	{
		_used.TryGetValue(Key(scope, o), out var count);
		return count;
	}

	private static string Key(string scope, TimeOption o)
	{
		return scope + "|" + o.DayOffset + "|" + o.Minute;
	}
}
