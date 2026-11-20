using System;

namespace Skyline_Sports_Project.Engine;

public class GameSimulator
{
	private readonly Random _random;

	public GameSimulator(Random random)
	{
		_random = random;
	}

	public void Play(Match game, Sport sport, Level level, double homeRating, double awayRating, int weather = -1)
	{
		level = SportRules.Scale(level);
		bool neutral = game.VenueId == -2;
		bool playoff = game.Stage != GameStage.Regular;
		double diff = (homeRating - awayRating + (neutral ? 0.0 : Momentum.HomeAdvantage(sport, level))) * Momentum.Sharpness(sport, level) * (playoff ? Momentum.PlayoffIntensity(sport) : 1.0) / 100.0;
		GameFlow flow = GameFlow.Play(sport, level, diff, playoff, _random, weather);
		int home = flow.HomeScore;
		int away = flow.AwayScore;
		ResultType result = flow.Result;
		game.HomePenalties = flow.HomePens;
		game.AwayPenalties = flow.AwayPens;
		if (sport == Sport.Hockey)
		{
			game.HomePenalties = 0;
			game.AwayPenalties = 0;
		}
		game.Flow = flow;
		game.HomeScore = home;
		game.AwayScore = away;
		game.Result = result;
		game.Played = true;
	}

	private int FootballScore(double touchdownRate, double fieldGoalRate)
	{
		int score = 0;
		int tds = Poisson(touchdownRate);
		for (int i = 0; i < tds; i++)
		{
			double r = _random.NextDouble();
			score += ((r < 0.06) ? 6 : ((r < 0.1) ? 8 : 7));
		}
		score += 3 * Poisson(fieldGoalRate);
		if (Chance(0.03))
		{
			score += 2;
		}
		return score;
	}

	private int Poisson(double lambda)
	{
		if (lambda <= 0.0)
		{
			return 0;
		}
		if (lambda > 30.0)
		{
			return Math.Max(0, (int)Math.Round(Normal(lambda, Math.Sqrt(lambda))));
		}
		double l = Math.Exp(0.0 - lambda);
		int k = 0;
		double p = 1.0;
		do
		{
			k++;
			p *= _random.NextDouble();
		}
		while (p > l);
		return k - 1;
	}

	private double Normal(double mean, double sd)
	{
		double u1 = 1.0 - _random.NextDouble();
		double u2 = _random.NextDouble();
		double z = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(Math.PI * 2.0 * u2);
		return mean + sd * z;
	}

	private bool Chance(double p)
	{
		return _random.NextDouble() < p;
	}

	private static double Logistic(double x)
	{
		return 1.0 / (1.0 + Math.Exp(0.0 - x));
	}
}
