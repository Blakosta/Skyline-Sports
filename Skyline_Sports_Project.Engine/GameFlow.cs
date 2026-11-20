using System;
using System.Collections.Generic;

namespace Skyline_Sports_Project.Engine;

public class GameFlow
{
	private class FbParams
	{
		public double PassRate;

		public double RunMean;

		public double RunSd;

		public double RunBigP;

		public double RunBigMean;

		public double RunFumble;

		public double CatchFumble;

		public double SackP;

		public double IntP;

		public double CmpP;

		public double CmpMean;

		public double CmpBigP;

		public double CmpBigMean;

		public double FgA;

		public double FgB;

		public int FgMax;

		public double Xp;

		public double TwoPt;

		public double TwoPtRate;

		public double PuntNet;

		public double KickStart;

		public double KickSd;

		public double TouchbackP;

		public double Touchback;

		public double KickRetTd;

		public double PuntRetTd;

		public double Onside;

		public int Runoff;

		public int Hurry;

		public int Milk;

		public bool TwoMinuteWarning;

		public bool CollegeClock;
	}

	private enum Tempo
	{
		Normal,
		Hurry,
		Milk
	}

	public int HomeScore;

	public int AwayScore;

	public ResultType Result;

	public int HomePens;

	public int AwayPens;

	public int Periods;

	public readonly List<FlowEvent> Events = new List<FlowEvent>();

	public readonly List<int> HomeLine = new List<int>();

	public readonly List<int> AwayLine = new List<int>();

	public readonly List<int> Track = new List<int>();

	public int[] HomeStats = new int[0];

	public int[] AwayStats = new int[0];

	public string Note = "";

	private readonly Random _r;

	private readonly Level _level;

	private int _weather = -1;

	private readonly double _diff;

	private readonly bool _playoff;

	public const int FT_ATT = 0;

	public const int FT_CMP = 1;

	public const int FT_PYD = 2;

	public const int FT_PTD = 3;

	public const int FT_INT = 4;

	public const int FT_SACK = 5;

	public const int FT_SACKYD = 6;

	public const int FT_RUSH = 7;

	public const int FT_RYD = 8;

	public const int FT_RTD = 9;

	public const int FT_FUM = 10;

	public const int FT_PUNT = 11;

	public const int FT_FGA = 12;

	public const int FT_FGM = 13;

	public const int FT_XPA = 14;

	public const int FT_XPM = 15;

	public const int FT_PLAYS = 16;

	public const int FT_FD = 17;

	public const int FT_LEN = 18;

	public const int FK_PASS = 1;

	public const int FK_RUN = 2;

	public const int FK_FG = 3;

	public const int FK_SAFETY = 4;

	public const int FK_PICK6 = 5;

	public const int FK_FUMBLE_TD = 6;

	public const int FK_KICK_RET = 7;

	public const int FK_PUNT_RET = 8;

	public const int FK_TWO_PT = 9;

	public const int DR_TD = 0;

	public const int DR_FG = 1;

	public const int DR_MISSED_FG = 2;

	public const int DR_PUNT = 3;

	public const int DR_INT = 4;

	public const int DR_FUMBLE = 5;

	public const int DR_DOWNS = 6;

	public const int DR_HALF = 7;

	public const int DR_SAFETY = 8;

	public const int DR_GAME = 9;

	public const int DriveInts = 8;

	private FbParams _fp;

	private int _fPeriod;

	private int _fClock;

	private int _fYard;

	private int _fDown;

	private int _fToGo;

	private bool _fHomeBall;

	private bool _fPendingKick;

	private bool _fKickingHome;

	private bool _fHomeReceivesFirst;

	private bool _fOver;

	private bool _fInOt;

	private bool _fOtReceiverHome;

	private bool _fCollegeOt;

	private int _homeTimeouts;

	private int _awayTimeouts;

	private readonly int[] _otPoss = new int[2];

	private bool _driveOpen;

	private int _driveStart;

	private int _drivePeriod;

	private int _drivePlays;

	private int _driveYards;

	private int _driveStartYard;

	private bool _driveHome;

	public const int HS_SHOTS = 0;

	public const int HS_PPO = 1;

	public const int HS_PPG = 2;

	public const int HS_EN = 3;

	public const int HS_SHG = 4;

	public const int HS_LEN = 5;

	public const int HK_EV = 1;

	public const int HK_PP = 2;

	public const int HK_SH = 3;

	public const int HK_EN = 4;

	public const int SK_GOAL = 1;

	public const int SK_PEN = 2;

	public const int SK_OG = 3;

	public const int SK_YELLOW = 4;

	public const int SK_RED = 5;

	private static double BaseballEdge(Level level)
	{
		return level switch
		{
			Level.College => 1.12, 
			Level.Pro => 1.08, 
			_ => 1.1, 
		};
	}

	private void PlayBaseball()
	{
		double perInning = ((_level == Level.HighSchool) ? 0.61 : ((_level == Level.College) ? 0.665 : 0.49)) * WeatherCodes.Scoring(Sport.Baseball, _weather);
		double size = ((_level == Level.Pro) ? 0.44 : 0.5);
		double num = BaseballEdge(_level);
		double homeMul = Math.Exp(num * _diff);
		double awayMul = Math.Exp((0.0 - num) * _diff);
		bool ghost = _level == Level.Pro;
		int inning;
		for (inning = 1; inning <= 25; inning++)
		{
			double phase;
			if (inning == 1)
			{
				phase = 1.12;
			}
			else if (inning <= 3)
			{
				phase = 0.95;
			}
			else
			{
				phase = ((inning <= 6) ? 1.02 : 0.95);
			}
			bool extra = inning > 9;
			int away = ((extra & ghost) ? GhostRunnerInning(awayMul) : RunsInInning(perInning * phase * awayMul, size));
			AwayLine.Add(away);
			if (away > 0)
			{
				AddRuns(home: false, inning, away);
			}
			if (inning >= 9 && HomeScore > AwayScore)
			{
				HomeLine.Add(-1);
				break;
			}
			int home = ((extra & ghost) ? GhostRunnerInning(homeMul) : RunsInInning(perInning * phase * homeMul, size));
			if (inning >= 9 && HomeScore + home > AwayScore)
			{
				int needed = AwayScore - HomeScore + 1;
				home = (Chance(0.3) ? Math.Min(home, needed + 3) : needed);
				HomeLine.Add(home);
				AddRuns(home: true, inning, home);
				break;
			}
			HomeLine.Add(home);
			if (home > 0)
			{
				AddRuns(home: true, inning, home);
			}
			if (inning >= 9 && HomeScore != AwayScore)
			{
				break;
			}
		}
		if (HomeScore == AwayScore)
		{
			if (Chance(Logistic(3.0 * _diff)))
			{
				HomeLine[HomeLine.Count - 1] = Math.Max(0, HomeLine[HomeLine.Count - 1]) + 1;
				AddRuns(home: true, inning, 1);
			}
			else
			{
				AwayLine[AwayLine.Count - 1]++;
				AddRuns(home: false, inning, 1);
			}
		}
		Periods = Math.Max(9, AwayLine.Count);
		if (Periods > 9)
		{
			Result = ResultType.ExtraInnings;
		}
	}

	private void AddRuns(bool home, int inning, int runs)
	{
		if (home)
		{
			HomeScore += runs;
		}
		else
		{
			AwayScore += runs;
		}
	}

	private int RunsInInning(double mean, double size)
	{
		double lambda = Gamma(size) * mean / size;
		return Math.Min(15, Poisson(lambda));
	}

	private int GhostRunnerInning(double mul)
	{
		double pScore = Math.Min(0.85, 0.6 * Math.Pow(mul, 0.6));
		if (!Chance(pScore))
		{
			return 0;
		}
		return 1 + RunsInInning(0.45 * mul, 0.6);
	}

	private double Gamma(double shape)
	{
		if (shape < 1.0)
		{
			double u = _r.NextDouble();
			return Gamma(shape + 1.0) * Math.Pow((u <= 0.0) ? 1E-12 : u, 1.0 / shape);
		}
		double d = shape - 1.0 / 3.0;
		double c = 1.0 / Math.Sqrt(9.0 * d);
		for (int guard = 0; guard < 100; guard++)
		{
			double x = Normal(0.0, 1.0);
			double v = 1.0 + c * x;
			if (!(v <= 0.0))
			{
				v = v * v * v;
				double u2 = _r.NextDouble();
				if (u2 < 1.0 - 0.0331 * x * x * x * x || Math.Log(Math.Max(1E-300, u2)) < 0.5 * x * x + d * (1.0 - v + Math.Log(v)))
				{
					return d * v;
				}
			}
		}
		return shape;
	}

	private static double BasketballEdge(Level level)
	{
		return level switch
		{
			Level.College => 0.27, 
			Level.Pro => 0.27, 
			_ => 0.245, 
		};
	}

	private void PlayBasketball()
	{
		int quarter;
		if (_level == Level.Pro)
		{
			quarter = 720;
		}
		else
		{
			quarter = ((_level == Level.College) ? 600 : 480);
		}
		double trip;
		if (_level == Level.Pro)
		{
			trip = 14.45;
		}
		else
		{
			trip = ((_level == Level.College) ? 17.4 : 18.0);
		}
		int shotClock;
		if (_level == Level.Pro)
		{
			shotClock = 24;
		}
		else
		{
			shotClock = ((_level == Level.College) ? 30 : 35);
		}
		double[] dist;
		if (_level != Level.Pro)
		{
			dist = ((_level != Level.College) ? new double[5] { 0.56, 0.05, 0.29, 0.1, 0.0 } : new double[5] { 0.53, 0.04, 0.3, 0.125, 0.005 });
		}
		else
		{
			dist = new double[5] { 0.5, 0.035, 0.31, 0.145, 0.01 };
		}
		double ft;
		if (_level == Level.Pro)
		{
			ft = 0.78;
		}
		else
		{
			ft = ((_level == Level.College) ? 0.72 : 0.65);
		}
		double num = BasketballEdge(_level);
		double homeMul = Math.Exp(num * _diff);
		double awayMul = Math.Exp((0.0 - num) * _diff);
		bool homeBall = Chance(0.5);
		int elapsed = 0;
		int minuteMark = 60;
		for (int period = 1; period <= 4 || HomeScore == AwayScore; period++)
		{
			if (period > 14)
			{
				if (Chance(Logistic(3.0 * _diff)))
				{
					HomeScore++;
				}
				else
				{
					AwayScore++;
				}
				break;
			}
			int num2 = ((period <= 4) ? quarter : 300);
			double homeForm = Math.Exp(Normal(0.0, 0.045));
			double awayForm = Math.Exp(Normal(0.0, 0.045));
			int startHome = HomeScore;
			int startAway = AwayScore;
			if (period > 4)
			{
				homeBall = Chance(0.5);
			}
			int left = num2;
			while (left > 0)
			{
				bool late = period >= 4 && left <= 60;
				int m = Margin(homeBall);
				double mul = (homeBall ? (homeMul * homeForm) : (awayMul * awayForm));
				if (m < 0)
				{
					mul *= 1.0 + Math.Min(0.04, 0.0015 * (double)(-m));
				}
				if (period == 4 && left < 360 && Math.Abs(m) >= 20)
				{
					mul *= ((m > 0) ? 0.93 : 1.0);
				}
				int pts;
				int secs;
				if (late && m > 0 && m <= 10 && Chance(0.75))
				{
					secs = 2 + _r.Next(0, 4);
					pts = (Chance(ft) ? 1 : 0) + (Chance(ft) ? 1 : 0);
				}
				else if (late && m < 0 && m >= -9)
				{
					secs = 5 + _r.Next(0, 6);
					double p3 = 0.36 * Math.Sqrt(mul);
					double p4 = 0.18 * mul;
					double roll = _r.NextDouble();
					pts = ((roll < p3) ? 3 : ((roll < p3 + p4) ? 2 : 0));
				}
				else
				{
					secs = Math.Max(3, Math.Min(shotClock, (int)Math.Round(Normal(trip + 0.4, 5.0))));
					if (late && m > 0)
					{
						secs = Math.Min(shotClock, secs + 6);
					}
					pts = PointsOnTrip(dist, mul);
				}
				secs = Math.Max(2, Math.Min(secs, left));
				left -= secs;
				elapsed += secs;
				if (homeBall)
				{
					HomeScore += pts;
				}
				else
				{
					AwayScore += pts;
				}
				for (; elapsed >= minuteMark; minuteMark += 60)
				{
					Track.Add(HomeScore);
					Track.Add(AwayScore);
				}
				homeBall = !homeBall;
			}
			HomeLine.Add(HomeScore - startHome);
			AwayLine.Add(AwayScore - startAway);
			Periods = period;
		}
		if (Track.Count == 0 || Track[Track.Count - 2] != HomeScore || Track[Track.Count - 1] != AwayScore)
		{
			Track.Add(HomeScore);
			Track.Add(AwayScore);
		}
		if (Periods > 4)
		{
			Result = ResultType.Overtime;
		}
	}

	private int PointsOnTrip(double[] dist, double mul)
	{
		double s = 0.0;
		double[] p = new double[5];
		for (int i = 1; i < 5; i++)
		{
			p[i] = dist[i] * mul;
			s += p[i];
		}
		if (s > 0.85)
		{
			for (int j = 1; j < 5; j++)
			{
				p[j] *= 0.85 / s;
			}
		}
		double roll = _r.NextDouble();
		for (int k = 1; k < 5; k++)
		{
			if (roll < p[k])
			{
				return k;
			}
			roll -= p[k];
		}
		return 0;
	}

	private GameFlow(Random random, Level level, double diff, bool playoff)
	{
		_r = random;
		_level = level;
		_diff = diff;
		_playoff = playoff;
	}

	public static GameFlow Play(Sport sport, Level level, double diff, bool playoff, Random random, int weather = -1)
	{
		GameFlow f = new GameFlow(random, SportRules.Scale(level), diff, playoff);
		f._weather = weather;
		switch (sport)
		{
		case Sport.Football:
			f.PlayFootball();
			break;
		case Sport.Hockey:
			f.PlayHockey();
			break;
		case Sport.Basketball:
			f.PlayBasketball();
			break;
		case Sport.Baseball:
			f.PlayBaseball();
			break;
		default:
			f.PlaySoccer();
			break;
		}
		return f;
	}

	private bool Chance(double p)
	{
		return _r.NextDouble() < p;
	}

	private double Normal(double mean, double sd)
	{
		double u1 = 1.0 - _r.NextDouble();
		double u2 = _r.NextDouble();
		return mean + sd * Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(Math.PI * 2.0 * u2);
	}

	private double Exponential(double mean)
	{
		return (0.0 - mean) * Math.Log(1.0 - _r.NextDouble());
	}

	private int Poisson(double lambda)
	{
		if (lambda <= 0.0)
		{
			return 0;
		}
		double l = Math.Exp(0.0 - lambda);
		double p = 1.0;
		int k = 0;
		do
		{
			k++;
			p *= _r.NextDouble();
		}
		while (p > l);
		return k - 1;
	}

	private static double Logistic(double x)
	{
		return 1.0 / (1.0 + Math.Exp(0.0 - x));
	}

	private static int Clamp(int v, int lo, int hi)
	{
		if (v >= lo)
		{
			if (v <= hi)
			{
				return v;
			}
			return hi;
		}
		return lo;
	}

	private void Add(int period, int seconds, bool home, int points, int kind, int yards = 0, int extra = -1, string clock = "")
	{
		Events.Add(new FlowEvent
		{
			Period = period,
			Seconds = seconds,
			Home = home,
			Points = points,
			Kind = kind,
			Yards = yards,
			Extra = extra,
			Clock = clock
		});
		if (home)
		{
			HomeScore += points;
		}
		else
		{
			AwayScore += points;
		}
	}

	private static double FootballEdge(Level level)
	{
		return level switch
		{
			Level.College => 0.56, 
			Level.Pro => 0.53, 
			_ => 0.54, 
		};
	}

	private static FbParams FootballParams(Level level)
	{
		return level switch
		{
			Level.College => new FbParams
			{
				PassRate = 0.52,
				RunMean = 4.1,
				RunSd = 3.4,
				RunBigP = 0.12,
				RunBigMean = 13.0,
				RunFumble = 0.01,
				CatchFumble = 0.006,
				SackP = 0.058,
				IntP = 0.021,
				CmpP = 0.66,
				CmpMean = 8.6,
				CmpBigP = 0.17,
				CmpBigMean = 18.0,
				FgA = 4.21,
				FgB = 0.077,
				FgMax = 52,
				Xp = 0.975,
				TwoPt = 0.45,
				TwoPtRate = 0.05,
				PuntNet = 39.0,
				KickStart = 25.0,
				KickSd = 7.0,
				TouchbackP = 0.55,
				Touchback = 25.0,
				KickRetTd = 0.005,
				PuntRetTd = 0.006,
				Onside = 0.15,
				Runoff = 31,
				Hurry = 12,
				Milk = 37,
				TwoMinuteWarning = false,
				CollegeClock = true
			}, 
			Level.HighSchool => new FbParams
			{
				PassRate = 0.4,
				RunMean = 4.1,
				RunSd = 3.6,
				RunBigP = 0.13,
				RunBigMean = 14.0,
				RunFumble = 0.016,
				CatchFumble = 0.01,
				SackP = 0.06,
				IntP = 0.03,
				CmpP = 0.59,
				CmpMean = 8.4,
				CmpBigP = 0.18,
				CmpBigMean = 19.0,
				FgA = 3.35,
				FgB = 0.09,
				FgMax = 40,
				Xp = 0.85,
				TwoPt = 0.42,
				TwoPtRate = 0.12,
				PuntNet = 31.0,
				KickStart = 33.0,
				KickSd = 9.0,
				TouchbackP = 0.12,
				Touchback = 20.0,
				KickRetTd = 0.012,
				PuntRetTd = 0.012,
				Onside = 0.2,
				Runoff = 37,
				Hurry = 15,
				Milk = 40,
				TwoMinuteWarning = false,
				CollegeClock = false
			}, 
			_ => new FbParams
			{
				PassRate = 0.54,
				RunMean = 3.72,
				RunSd = 3.2,
				RunBigP = 0.1,
				RunBigMean = 12.0,
				RunFumble = 0.009,
				CatchFumble = 0.006,
				SackP = 0.062,
				IntP = 0.018,
				CmpP = 0.69,
				CmpMean = 8.1,
				CmpBigP = 0.16,
				CmpBigMean = 17.0,
				FgA = 5.39,
				FgB = 0.08,
				FgMax = 57,
				Xp = 0.955,
				TwoPt = 0.48,
				TwoPtRate = 0.06,
				PuntNet = 41.5,
				KickStart = 28.0,
				KickSd = 6.0,
				TouchbackP = 0.18,
				Touchback = 35.0,
				KickRetTd = 0.004,
				PuntRetTd = 0.005,
				Onside = 0.08,
				Runoff = 33,
				Hurry = 14,
				Milk = 39,
				TwoMinuteWarning = true,
				CollegeClock = false
			}, 
		};
	}

	private int PeriodLength(int period)
	{
		if (period > 4)
		{
			return 600;
		}
		return 900;
	}

	private int Elapsed()
	{
		if (_fPeriod > 4)
		{
			return 3600 + (_fPeriod - 5) * 600 + (600 - _fClock);
		}
		return (_fPeriod - 1) * 900 + (900 - _fClock);
	}

	private int SecLeftHalf()
	{
		if (_fPeriod != 1 && _fPeriod != 3)
		{
			return _fClock;
		}
		return _fClock + 900;
	}

	private int SecLeftGame()
	{
		if (_fPeriod >= 4)
		{
			return _fClock;
		}
		return _fClock + (4 - _fPeriod) * 900;
	}

	private int Margin(bool home)
	{
		if (!home)
		{
			return AwayScore - HomeScore;
		}
		return HomeScore - AwayScore;
	}

	private int[] FStats(bool home)
	{
		if (!home)
		{
			return AwayStats;
		}
		return HomeStats;
	}

	private static void FootballWeather(FbParams p, int code)
	{
		if (code >= 0)
		{
			switch (WeatherCodes.Kind(code))
			{
			case WeatherKind.Dome:
				return;
			case WeatherKind.Snow:
				p.PassRate -= 0.08;
				p.CmpP -= 0.05;
				p.CmpBigP *= 0.75;
				p.RunMean -= 0.25;
				p.RunFumble *= 1.6;
				p.CatchFumble *= 1.5;
				p.IntP *= 1.15;
				p.FgA -= 0.6;
				p.FgMax -= 7;
				p.Xp -= 0.04;
				p.PuntNet -= 4.0;
				break;
			case WeatherKind.HeavyRain:
			case WeatherKind.Storm:
				p.PassRate -= 0.06;
				p.CmpP -= 0.04;
				p.CmpBigP *= 0.8;
				p.RunFumble *= 1.5;
				p.CatchFumble *= 1.4;
				p.IntP *= 1.1;
				p.FgA -= 0.4;
				p.FgMax -= 5;
				p.Xp -= 0.025;
				p.PuntNet -= 3.0;
				break;
			case WeatherKind.Rain:
				p.PassRate -= 0.03;
				p.CmpP -= 0.02;
				p.RunFumble *= 1.25;
				p.CatchFumble *= 1.2;
				p.FgA -= 0.2;
				p.FgMax -= 2;
				p.Xp -= 0.01;
				break;
			}
			if (WeatherCodes.TempF(code) < 25)
			{
				p.FgA -= 0.25;
				p.FgMax -= 3;
				p.PuntNet -= 1.5;
			}
		}
	}

	private void PlayFootball()
	{
		_fp = FootballParams(_level);
		FootballWeather(_fp, _weather);
		HomeStats = new int[18];
		AwayStats = new int[18];
		_fHomeReceivesFirst = Chance(0.5);
		_fPeriod = 1;
		_fClock = 900;
		_fPendingKick = true;
		_fKickingHome = !_fHomeReceivesFirst;
		_homeTimeouts = 3;
		_awayTimeouts = 3;
		bool collegeOt = _level != Level.Pro;
		for (int guard = 0; guard < 2000; guard++)
		{
			if (_fOver)
			{
				break;
			}
			if (_fClock <= 0)
			{
				if (!EndFootballPeriod(collegeOt))
				{
					break;
				}
			}
			else if (_fPendingKick)
			{
				Kickoff(_fKickingHome, safetyKick: false);
			}
			else
			{
				Snap();
			}
		}
		if (_driveOpen)
		{
			CloseDrive(9);
		}
		if (collegeOt && HomeScore == AwayScore)
		{
			CollegeOvertime();
		}
		if (HomeScore == AwayScore)
		{
			Add(_fPeriod, PeriodLength(_fPeriod) - 1, Chance(Logistic(3.0 * _diff)), 3, 3, 35);
		}
		Periods = Math.Max(4, _fPeriod);
		if (Periods > 4)
		{
			Result = ResultType.Overtime;
		}
	}

	private bool EndFootballPeriod(bool collegeOt)
	{
		if (_fPeriod == 1 || _fPeriod == 3)
		{
			_fPeriod++;
			_fClock = 900;
			return true;
		}
		if (_fPeriod == 2)
		{
			if (_driveOpen)
			{
				CloseDrive(7);
			}
			_fPeriod = 3;
			_fClock = 900;
			_fPendingKick = true;
			_fKickingHome = _fHomeReceivesFirst;
			_homeTimeouts = 3;
			_awayTimeouts = 3;
			return true;
		}
		if (_fPeriod == 4 && !_fInOt)
		{
			if (_driveOpen)
			{
				CloseDrive(9);
			}
			if ((HomeScore != AwayScore) | collegeOt)
			{
				return false;
			}
			_fInOt = true;
			_fPeriod = 5;
			_fClock = 600;
			_fOtReceiverHome = Chance(0.5);
			_fPendingKick = true;
			_fKickingHome = !_fOtReceiverHome;
			_homeTimeouts = 2;
			_awayTimeouts = 2;
			return true;
		}
		if (HomeScore != AwayScore)
		{
			if (_driveOpen)
			{
				CloseDrive(9);
			}
			return false;
		}
		if (_fPeriod >= 12)
		{
			return false;
		}
		_fPeriod++;
		_fClock = 600;
		return true;
	}

	private void StartDrive(bool home, int yard)
	{
		_fHomeBall = home;
		_fYard = Clamp(yard, 1, 99);
		_fDown = 1;
		_fToGo = Math.Min(10, 100 - _fYard);
		_driveOpen = true;
		_driveHome = home;
		_drivePeriod = _fPeriod;
		_driveStart = Elapsed();
		_drivePlays = 0;
		_driveYards = 0;
		_driveStartYard = _fYard;
		if (_fInOt)
		{
			_otPoss[(!home) ? 1u : 0u]++;
		}
	}

	private void CloseDrive(int result)
	{
		if (_driveOpen)
		{
			_driveOpen = false;
			Track.Add(_driveHome ? 1 : 0);
			Track.Add(_drivePeriod);
			Track.Add(_driveStart);
			Track.Add(Math.Max(_driveStart, Elapsed()));
			Track.Add(_drivePlays);
			Track.Add(_driveYards);
			Track.Add(result);
			Track.Add(_driveStartYard);
			if (_fInOt && OtDecided())
			{
				_fOver = true;
			}
		}
	}

	private bool OtDecided()
	{
		if (HomeScore == AwayScore)
		{
			return false;
		}
		if (_otPoss[0] >= 1 && _otPoss[1] >= 1)
		{
			return true;
		}
		return HomeScore > AwayScore != _fOtReceiverHome;
	}

	private void RunClock(int seconds)
	{
		int before = _fClock;
		_fClock -= seconds;
		if (_fp.TwoMinuteWarning && (_fPeriod == 2 || _fPeriod == 4) && before > 120 && _fClock < 120)
		{
			_fClock = 120;
		}
		if (_fClock < 0)
		{
			_fClock = 0;
		}
	}

	private void Kickoff(bool kickingHome, bool safetyKick)
	{
		_fPendingKick = false;
		bool receiving = !kickingHome;
		int kickMargin = Margin(kickingHome);
		bool late = (_fPeriod == 4 && SecLeftGame() <= 300) || (_fPeriod == 4 && SecLeftGame() <= 480 && kickMargin <= -9);
		if ((!safetyKick & late) && !_fInOt && kickMargin < 0 && kickMargin >= -16)
		{
			RunClock(3);
			if (Chance(_fp.Onside))
			{
				StartDrive(kickingHome, 45 + _r.Next(0, 6));
			}
			else
			{
				StartDrive(receiving, 52 + _r.Next(0, 6));
			}
			return;
		}
		if (Chance(safetyKick ? (_fp.KickRetTd * 0.7) : _fp.KickRetTd))
		{
			RunClock(12);
			ReturnTouchdown(receiving, 7, 90 + _r.Next(0, 11));
			return;
		}
		int start;
		if (safetyKick)
		{
			start = (int)Math.Round(Normal(_fp.KickStart + 10.0, 8.0));
			RunClock(6);
		}
		else if (Chance(_fp.TouchbackP))
		{
			start = (int)_fp.Touchback;
		}
		else
		{
			start = (int)Math.Round(Normal(_fp.KickStart, _fp.KickSd) + (Chance(0.03) ? Exponential(18.0) : 0.0));
			RunClock(6);
		}
		StartDrive(receiving, Clamp(start, 5, 70));
	}

	private void ReturnTouchdown(bool scoringHome, int kind, int yards)
	{
		int extra = TryAfter(scoringHome, allowed: true);
		Add(_fPeriod, PeriodLength(_fPeriod) - _fClock, scoringHome, 6 + Math.Max(0, extra), kind, Clamp(yards, 1, 100), extra);
		_fPendingKick = true;
		_fKickingHome = scoringHome;
		if (_fInOt && OtDecided())
		{
			_fOver = true;
		}
	}

	private void Punt()
	{
		bool off = _fHomeBall;
		FStats(off)[11]++;
		CloseDrive(3);
		RunClock(8);
		if (Chance(_fp.PuntRetTd))
		{
			ReturnTouchdown(!off, 8, Math.Max(40, 100 - (_fYard + (int)_fp.PuntNet) + 10));
			return;
		}
		double net = Normal(_fp.PuntNet, 7.5);
		int landing = _fYard + (int)Math.Round(net);
		int start = ((landing >= 100) ? 20 : (100 - Math.Min(99, landing)));
		if (landing >= 100 && _level == Level.Pro)
		{
			start = 20;
		}
		StartDrive(!off, Clamp(start, 1, 80));
	}

	private int FieldGoalDistance()
	{
		return 100 - _fYard + 17;
	}

	private double FieldGoalChance(int distance)
	{
		return Logistic(_fp.FgA - _fp.FgB * (double)distance);
	}

	private void FieldGoal()
	{
		bool off = _fHomeBall;
		int dist = FieldGoalDistance();
		FStats(off)[12]++;
		RunClock(5);
		if (Chance(FieldGoalChance(dist)))
		{
			FStats(off)[13]++;
			Add(_fPeriod, PeriodLength(_fPeriod) - _fClock, off, 3, 3, dist);
			CloseDrive(1);
			_fPendingKick = true;
			_fKickingHome = off;
		}
		else
		{
			CloseDrive(2);
			StartDrive(!off, Math.Max(20, 100 - (_fYard - 7)));
		}
	}

	private int TryAfter(bool home, bool allowed)
	{
		if (!allowed)
		{
			return -1;
		}
		int m = Margin(home) + 6;
		bool flag = _fPeriod >= 4;
		bool goForTwo = ((!flag) ? (Chance(_fp.TwoPtRate) || (_fPeriod == 3 && (m == -2 || m == -5) && Chance(0.5))) : (m == -2 || m == -5 || m == -10 || m == 1 || m == 5 || m == 12));
		if (flag && m == -1 && SecLeftGame() < 30 && Chance(0.3))
		{
			goForTwo = true;
		}
		int[] st = FStats(home);
		if (goForTwo)
		{
			if (!Chance(_fp.TwoPt * Math.Exp(0.3 * OffEdge(home))))
			{
				return -2;
			}
			return 2;
		}
		st[14]++;
		if (Chance(_fp.Xp))
		{
			st[15]++;
			return 1;
		}
		return 0;
	}

	private void Touchdown(bool home, int kind, int yards)
	{
		bool walkOff = _fInOt && ((_otPoss[0] >= 1 && _otPoss[1] >= 1) || home != _fOtReceiverHome) && Margin(home) + 6 > 0;
		int extra = TryAfter(home, !walkOff);
		Add(_fPeriod, PeriodLength(_fPeriod) - _fClock, home, 6 + Math.Max(0, extra), kind, Clamp(yards, 1, 99), extra);
		CloseDrive(0);
		_fPendingKick = true;
		_fKickingHome = home;
	}

	private double OffEdge(bool home)
	{
		double e = FootballEdge(_level) * (home ? _diff : (0.0 - _diff));
		int m = Margin(home);
		if (_fPeriod >= 4 && m >= 21)
		{
			e -= 0.25;
		}
		if (_fPeriod >= 4 && m <= -21)
		{
			e += 0.15;
		}
		return Math.Max(-1.6, Math.Min(1.6, e));
	}

	private Tempo FootballTempo(bool home)
	{
		int m = Margin(home);
		int game = SecLeftGame();
		int half = SecLeftHalf();
		if (_fInOt)
		{
			if (m >= 0)
			{
				return Tempo.Normal;
			}
			return Tempo.Hurry;
		}
		if (_fPeriod == 2 && half <= 120 && _fYard >= 20)
		{
			return Tempo.Hurry;
		}
		if (_fPeriod == 4 && m < 0 && (game <= 300 || (m <= -9 && game <= 600) || m <= -17))
		{
			return Tempo.Hurry;
		}
		if (_fPeriod == 4 && m == 0 && game <= 150 && _fYard >= 20)
		{
			return Tempo.Hurry;
		}
		if (_fPeriod == 3 && m <= -21)
		{
			return Tempo.Hurry;
		}
		if (_fPeriod == 4 && m > 0)
		{
			return Tempo.Milk;
		}
		if (_fPeriod == 3 && m >= 17)
		{
			return Tempo.Milk;
		}
		return Tempo.Normal;
	}

	private double PassChance(bool home, Tempo tempo)
	{
		double p = _fp.PassRate;
		switch (tempo)
		{
		case Tempo.Hurry:
			p = Math.Max(p, 0.82);
			break;
		case Tempo.Milk:
			p -= 0.2;
			break;
		}
		int m = Margin(home);
		if (_fPeriod >= 3 && m <= -14 && tempo != Tempo.Hurry)
		{
			p += 0.12;
		}
		if (_fDown == 3 && _fToGo >= 7)
		{
			p = Math.Max(p, 0.85);
		}
		else if (_fDown >= 3 && _fToGo <= 2)
		{
			p = Math.Min(p, 0.35);
		}
		else if (_fDown == 2 && _fToGo >= 8)
		{
			p += 0.08;
		}
		if (_fYard >= 97)
		{
			p -= 0.15;
		}
		return Math.Max(0.12, Math.Min(0.95, p));
	}

	private void Snap()
	{
		bool off = _fHomeBall;
		int m = Margin(off);
		int game = SecLeftGame();
		int half = SecLeftHalf();
		int defTimeouts = (off ? _awayTimeouts : _homeTimeouts);
		if (!_fInOt && _fPeriod == 4 && m > 0 && (5 - _fDown - defTimeouts) * 40 >= game)
		{
			_drivePlays++;
			FStats(off)[16]++;
			FStats(off)[7]++;
			FStats(off)[8]--;
			_driveYards--;
			_fClock = 0;
		}
		else if (_fPeriod == 2 && half <= 30 && _fYard < 55 && m >= -3)
		{
			_fClock = 0;
		}
		else if (!_fInOt && half <= 6 && FieldGoalDistance() <= _fp.FgMax + 2 && (_fPeriod == 2 || m >= -3))
		{
			FieldGoal();
		}
		else if (_fDown == 4 && !FourthDownGo(off, m, game))
		{
			if (FieldGoalDistance() <= _fp.FgMax)
			{
				FieldGoal();
			}
			else
			{
				Punt();
			}
		}
		else if (!_fInOt && _fPeriod == 4 && m < 0 && m >= -8 && game <= 7 && _fYard >= 45)
		{
			_drivePlays++;
			int[] st = FStats(off);
			st[16]++;
			st[0]++;
			RunClock(8);
			if ((double)(Chance(0.06) ? 1 : 0) > 0.0)
			{
				st[1]++;
				st[2] += 100 - _fYard;
				st[3]++;
				_driveYards += 100 - _fYard;
				Touchdown(off, 1, 100 - _fYard);
			}
			else if (Chance(0.25))
			{
				st[4]++;
				CloseDrive(4);
				_fClock = 0;
			}
			_fClock = 0;
		}
		else
		{
			Tempo tempo = FootballTempo(off);
			if (Chance(PassChance(off, tempo)))
			{
				PassPlay(off, tempo);
			}
			else
			{
				RunPlay(off, tempo);
			}
		}
	}

	private bool FourthDownGo(bool off, int m, int game)
	{
		bool inRange = FieldGoalDistance() <= _fp.FgMax;
		if (_fInOt)
		{
			if (m < -3)
			{
				return true;
			}
			if (m < 0)
			{
				return !inRange;
			}
			if (!inRange && _fYard >= 50)
			{
				return _fToGo <= 3;
			}
			return false;
		}
		if (_fPeriod == 4)
		{
			if (m < 0 && game <= 150)
			{
				if (inRange)
				{
					return m < -3;
				}
				return true;
			}
			if (m < -8 && game <= 420)
			{
				return true;
			}
			if (m < -3 && game <= 240)
			{
				if (inRange)
				{
					return m < -3;
				}
				return true;
			}
			if (m > 0 && game <= 300)
			{
				if (_fToGo <= 1 && _fYard >= 40 && _fYard <= 60)
				{
					return Chance(0.25);
				}
				return false;
			}
		}
		if (_fToGo <= 1 && _fYard >= 40)
		{
			return Chance((_level == Level.Pro) ? 0.62 : 0.55);
		}
		if (_fToGo <= 2 && _fYard >= 60 && !inRange)
		{
			return Chance(0.6);
		}
		if (_fToGo <= 3 && _fYard >= 55 && _fYard < 80)
		{
			return Chance(0.25);
		}
		if (_level == Level.HighSchool && !inRange && _fYard >= 50 && _fToGo <= 5)
		{
			return Chance(0.45);
		}
		if (_fYard >= 92 && _fToGo <= 3)
		{
			return Chance(0.4);
		}
		return false;
	}

	private void AfterPlayClock(bool off, Tempo tempo, int playTime, bool clockStops)
	{
		int runoff;
		if (clockStops)
		{
			runoff = 0;
		}
		else
		{
			runoff = tempo switch
			{
				Tempo.Milk => _fp.Milk, 
				Tempo.Hurry => _fp.Hurry, 
				_ => _fp.Runoff + _r.Next(-4, 5), 
			};
			int game = SecLeftGame();
			int half = SecLeftHalf();
			bool lateGame = !_fInOt && _fPeriod == 4 && game <= 180;
			if (lateGame && Margin(off) < 0 && (off ? _homeTimeouts : _awayTimeouts) > 0 && game - playTime < 150)
			{
				if (off)
				{
					_homeTimeouts--;
				}
				else
				{
					_awayTimeouts--;
				}
				runoff = 0;
			}
			else if (lateGame && Margin(off) > 0 && (off ? _awayTimeouts : _homeTimeouts) > 0)
			{
				if (off)
				{
					_awayTimeouts--;
				}
				else
				{
					_homeTimeouts--;
				}
				runoff = 0;
			}
			else if (_fPeriod == 2 && half <= 120 && tempo == Tempo.Hurry && (off ? _homeTimeouts : _awayTimeouts) > 0 && half < 60)
			{
				if (off)
				{
					_homeTimeouts--;
				}
				else
				{
					_awayTimeouts--;
				}
				runoff = 0;
			}
		}
		RunClock(playTime + runoff);
	}

	private bool OutOfBoundsStops()
	{
		if (_level != Level.Pro || _fInOt)
		{
			return true;
		}
		if (_fPeriod != 2 || SecLeftHalf() > 120)
		{
			if (_fPeriod == 4)
			{
				return SecLeftGame() <= 300;
			}
			return false;
		}
		return true;
	}

	private void FirstDownCheck(bool off, int gained)
	{
		_fYard += gained;
		_driveYards += gained;
		if (gained >= _fToGo)
		{
			FStats(off)[17]++;
			_fDown = 1;
			_fToGo = Math.Min(10, 100 - _fYard);
			return;
		}
		_fToGo -= gained;
		_fDown++;
		if (_fDown > 4)
		{
			CloseDrive(6);
			StartDrive(!off, 100 - _fYard);
		}
	}

	private void RunPlay(bool off, Tempo tempo)
	{
		double e = OffEdge(off);
		int[] st = FStats(off);
		_drivePlays++;
		st[16]++;
		st[7]++;
		double y = Normal(_fp.RunMean + 1.5 * e, _fp.RunSd);
		if (Chance(_fp.RunBigP * Math.Exp(0.7 * e)))
		{
			y += Exponential(_fp.RunBigMean);
		}
		if (_fYard >= 95)
		{
			y = Math.Min(y, Normal(1.6 + e, 1.6));
		}
		int yards = Math.Max(-4, (int)Math.Round(y));
		if (_fYard + yards >= 100)
		{
			yards = 100 - _fYard;
			st[8] += yards;
			st[9]++;
			_driveYards += yards;
			RunClock(6);
			Touchdown(off, 2, yards);
			return;
		}
		if (_fYard + yards <= 0)
		{
			st[8] += yards;
			Safety(off);
			return;
		}
		st[8] += yards;
		if (Chance(_fp.RunFumble * Math.Exp(-0.5 * e)))
		{
			st[10]++;
			_fYard += yards;
			_driveYards += yards;
			RunClock(6);
			Turnover(off, 5, 6, 0.06);
			return;
		}
		bool stops = Chance((tempo == Tempo.Hurry) ? 0.3 : 0.08) && OutOfBoundsStops();
		int downBefore = _fDown;
		FirstDownCheck(off, yards);
		if (!_driveOpen || _fHomeBall != off)
		{
			RunClock(6);
			return;
		}
		bool firstDown = _fDown == 1 && downBefore >= 1;
		if ((_fp.CollegeClock & firstDown) && SecLeftHalf() <= 120)
		{
			stops = true;
		}
		AfterPlayClock(off, tempo, 6, stops);
	}

	private void PassPlay(bool off, Tempo tempo)
	{
		double e = OffEdge(off);
		int[] st = FStats(off);
		_drivePlays++;
		st[16]++;
		if (Chance(_fp.SackP * Math.Exp(-0.6 * e)))
		{
			int loss = Math.Max(1, (int)Math.Round(3.0 + Exponential(4.0)));
			st[5]++;
			st[6] += loss;
			if (_fYard - loss <= 0)
			{
				Safety(off);
			}
			else if (Chance(0.05))
			{
				st[10]++;
				_fYard -= loss;
				_driveYards -= loss;
				RunClock(6);
				Turnover(off, 5, 6, 0.1);
			}
			else
			{
				FirstDownCheck(off, -loss);
				if (!_driveOpen || _fHomeBall != off)
				{
					RunClock(6);
				}
				else
				{
					AfterPlayClock(off, tempo, 6, clockStops: false);
				}
			}
			return;
		}
		st[0]++;
		double intP = _fp.IntP * Math.Exp(-0.8 * e) * ((tempo == Tempo.Hurry) ? 1.35 : 1.0);
		if (Chance(intP))
		{
			st[4]++;
			int air = Math.Max(0, (int)Math.Round(Normal(11.0, 7.0)));
			_fYard = Math.Min(99, _fYard + air);
			RunClock(7);
			Turnover(off, 4, 5, 0.09);
			return;
		}
		double cmp = Math.Max(0.3, Math.Min(0.85, _fp.CmpP + 0.09 * e - ((tempo == Tempo.Hurry) ? 0.04 : 0.0) - ((_fDown == 3 && _fToGo >= 8) ? 0.06 : 0.0)));
		if (!Chance(cmp))
		{
			FirstDownCheck(off, 0);
			if (!_driveOpen || _fHomeBall != off)
			{
				RunClock(6);
			}
			else
			{
				AfterPlayClock(off, tempo, 6, clockStops: true);
			}
			return;
		}
		st[1]++;
		double y = Math.Max(-2.0, Normal(_fp.CmpMean + 1.2 * e, 5.0));
		if (Chance(_fp.CmpBigP * Math.Exp(0.7 * e)))
		{
			y += Exponential(_fp.CmpBigMean);
		}
		int yards = (int)Math.Round(y);
		if (_fYard + yards >= 100)
		{
			yards = 100 - _fYard;
			st[2] += yards;
			st[3]++;
			_driveYards += yards;
			RunClock(7);
			Touchdown(off, 1, yards);
			return;
		}
		st[2] += yards;
		if (Chance(_fp.CatchFumble))
		{
			st[10]++;
			_fYard = Math.Max(1, _fYard + yards);
			_driveYards += yards;
			RunClock(7);
			Turnover(off, 5, 6, 0.06);
			return;
		}
		bool stops = Chance((tempo == Tempo.Hurry) ? 0.45 : 0.15) && OutOfBoundsStops();
		int downBefore = _fDown;
		FirstDownCheck(off, yards);
		if (!_driveOpen || _fHomeBall != off)
		{
			RunClock(7);
			return;
		}
		if (_fp.CollegeClock && _fDown == 1 && downBefore >= 1 && SecLeftHalf() <= 120)
		{
			stops = true;
		}
		AfterPlayClock(off, tempo, 7, stops);
	}

	private void Turnover(bool off, int result, int returnKind, double returnTd)
	{
		CloseDrive(result);
		if (!_fOver)
		{
			if (!_fCollegeOt && Chance(returnTd))
			{
				ReturnTouchdown(!off, returnKind, 100 - _fYard + _r.Next(0, 6));
				return;
			}
			int back = (int)Math.Round(Math.Max(0.0, Normal(7.0, 8.0)));
			StartDrive(!off, 100 - _fYard + back);
		}
	}

	private void Safety(bool off)
	{
		RunClock(6);
		CloseDrive(8);
		Add(_fPeriod, PeriodLength(_fPeriod) - _fClock, !off, 2, 4);
		if (_fInOt && OtDecided())
		{
			_fOver = true;
			return;
		}
		if (_fClock > 0)
		{
			Kickoff(off, safetyKick: true);
			return;
		}
		_fPendingKick = true;
		_fKickingHome = off;
	}

	private void CollegeOvertime()
	{
		bool firstHome = Chance(0.5);
		_fCollegeOt = true;
		int start = ((_level == Level.HighSchool) ? 90 : 75);
		for (int round = 1; round <= 12; round++)
		{
			if (HomeScore != AwayScore)
			{
				break;
			}
			_fPeriod = 4 + round;
			_fClock = 600;
			bool[] array = new bool[2]
			{
				firstHome,
				!firstHome
			};
			foreach (bool home in array)
			{
				bool second = home != firstHome;
				int secBase = (second ? 300 : 0);
				if (round >= 3)
				{
					FStats(home)[16]++;
					if (Chance(_fp.TwoPt * Math.Exp(0.3 * OffEdge(home))))
					{
						Add(_fPeriod, secBase + 20, home, 2, 9, 3);
					}
				}
				else
				{
					OtPossession(home, start, round, secBase, second);
				}
			}
		}
		if (HomeScore == AwayScore)
		{
			Add(_fPeriod, 590, Chance(Logistic(3.0 * _diff)), 2, 9, 3);
		}
	}

	private void OtPossession(bool home, int start, int round, int secBase, bool second)
	{
		_fClock = 600 - secBase;
		StartDrive(home, start);
		for (int guard = 0; guard < 40; guard++)
		{
			if (!_driveOpen)
			{
				break;
			}
			if (_fHomeBall != home)
			{
				break;
			}
			int need = -Margin(home);
			int sec = Math.Min(secBase + 290, secBase + 12 * _drivePlays + 5);
			_fClock = 600 - sec;
			bool inRange = FieldGoalDistance() <= _fp.FgMax;
			bool kick = false;
			if (_fDown == 4)
			{
				kick = inRange && (!second || need <= 3);
			}
			else if (((second && need <= 0) & inRange) && _fDown == 3 && FieldGoalDistance() <= 40)
			{
				kick = true;
			}
			if (kick)
			{
				int dist = FieldGoalDistance();
				FStats(home)[12]++;
				if (Chance(FieldGoalChance(dist)))
				{
					FStats(home)[13]++;
					Add(_fPeriod, sec, home, 3, 3, dist);
					CloseDrive(1);
				}
				else
				{
					CloseDrive(2);
				}
				return;
			}
			bool flag = Chance(PassChance(home, Tempo.Normal));
			int before = Events.Count;
			if (flag)
			{
				PassPlay(home, Tempo.Normal);
			}
			else
			{
				RunPlay(home, Tempo.Normal);
			}
			if (Events.Count > before)
			{
				FlowEvent e = Events[Events.Count - 1];
				e.Seconds = sec;
				if (round >= 2 && e.Home == home && (e.Kind == 1 || e.Kind == 2) && e.Extra == 1)
				{
					FStats(home)[14]--;
					FStats(home)[15]--;
					bool good = Chance(_fp.TwoPt);
					e.Extra = (good ? 2 : (-2));
					e.Points = (good ? 8 : 6);
					if (home)
					{
						HomeScore += (good ? 1 : (-1));
					}
					else
					{
						AwayScore += (good ? 1 : (-1));
					}
				}
			}
			if (_fHomeBall != home || !_driveOpen)
			{
				break;
			}
		}
		if (_driveOpen && _fHomeBall == home)
		{
			CloseDrive(6);
		}
		else
		{
			_driveOpen = false;
		}
		_fPendingKick = false;
	}

	private static double HockeyEdge(Level level)
	{
		return level switch
		{
			Level.College => 1.05, 
			Level.Pro => 1.05, 
			_ => 1.03, 
		};
	}

	private void PlayHockey()
	{
		HomeStats = new int[5];
		AwayStats = new int[5];
		double es;
		if (_level != Level.HighSchool)
		{
			es = ((_level == Level.College) ? 2.7 : 2.5);
		}
		else
		{
			es = 3.0;
		}
		double num;
		if (_level != Level.HighSchool)
		{
			num = ((_level == Level.College) ? 3.4 : 2.8);
		}
		else
		{
			num = 3.8;
		}
		double ppConv;
		if (_level != Level.HighSchool)
		{
			ppConv = ((_level == Level.College) ? 0.2 : 0.21);
		}
		else
		{
			ppConv = 0.22;
		}
		double shotsPer60;
		if (_level != Level.HighSchool)
		{
			shotsPer60 = ((_level == Level.College) ? 28.0 : 26.3);
		}
		else
		{
			shotsPer60 = 25.5;
		}
		double esRate = es / 3600.0;
		double penRate = num * 1.17 / 3600.0;
		double ppRate = (0.0 - Math.Log(1.0 - ppConv)) / 120.0;
		double shRate = 0.00012;
		double enRate = 0.0055000000000000005;
		double k = HockeyEdge(_level);
		double homeMul = Math.Exp(k * _diff);
		double awayMul = Math.Exp((0.0 - k) * _diff);
		double[] periodMul = new double[3] { 0.95, 1.16, 0.93 };
		int pullDown1 = 105 + _r.Next(0, 90);
		int pullDown2 = 200 + _r.Next(0, 100);
		double homeShotsExp = 0.0;
		double awayShotsExp = 0.0;
		int homePp = 0;
		int awayPp = 0;
		for (int period = 1; period <= 3; period++)
		{
			for (int t = 0; t < 1200; t += 10)
			{
				int left = 1200 - t;
				int m = HomeScore - AwayScore;
				double hr = esRate * periodMul[period - 1] * homeMul;
				double ar = esRate * periodMul[period - 1] * awayMul;
				if (period >= 2 && m != 0)
				{
					double push = 1.0 + 0.1 * (double)Math.Min(2, Math.Abs(m));
					double sit = 1.0 - 0.07 * (double)Math.Min(2, Math.Abs(m));
					if (m < 0)
					{
						hr *= push;
						ar *= sit;
					}
					else
					{
						ar *= push;
						hr *= sit;
					}
				}
				double hs = shotsPer60 / 3600.0 * Math.Sqrt(homeMul);
				double aws = shotsPer60 / 3600.0 * Math.Sqrt(awayMul);
				if (period >= 2 && m < 0)
				{
					hs *= 1.12;
					aws *= 0.92;
				}
				else if (period >= 2 && m > 0)
				{
					aws *= 1.12;
					hs *= 0.92;
				}
				int homeKind = 1;
				int awayKind = 1;
				if (homePp > 0)
				{
					hr = ppRate * Math.Sqrt(homeMul) * 1.0;
					ar = shRate;
					hs *= 1.8;
					aws *= 0.4;
					homeKind = 2;
					awayKind = 3;
				}
				else if (awayPp > 0)
				{
					ar = ppRate * Math.Sqrt(awayMul);
					hr = shRate;
					aws *= 1.8;
					hs *= 0.4;
					awayKind = 2;
					homeKind = 3;
				}
				bool num2 = period == 3 && ((m == -1 && left <= pullDown1) || (m == -2 && left <= pullDown2));
				bool awayPulled = period == 3 && ((m == 1 && left <= pullDown1) || (m == 2 && left <= pullDown2));
				if (num2)
				{
					hr *= 2.3;
					ar = enRate * Math.Sqrt(awayMul);
					hs *= 1.8;
					homeKind = ((homePp <= 0) ? 1 : 2);
					awayKind = 4;
				}
				if (awayPulled)
				{
					ar *= 2.3;
					hr = enRate * Math.Sqrt(homeMul);
					aws *= 1.8;
					awayKind = ((awayPp <= 0) ? 1 : 2);
					homeKind = 4;
				}
				homeShotsExp += hs * 10.0;
				awayShotsExp += aws * 10.0;
				double pH = 1.0 - Math.Exp((0.0 - hr) * 10.0);
				double pA = 1.0 - Math.Exp((0.0 - ar) * 10.0);
				double roll = _r.NextDouble();
				if (roll < pH)
				{
					HockeyGoal(home: true, period, t + _r.Next(10), homeKind, ref homePp, ref awayPp);
				}
				else if (roll < pH + pA)
				{
					HockeyGoal(home: false, period, t + _r.Next(10), awayKind, ref homePp, ref awayPp);
				}
				if (homePp <= 0 && awayPp <= 0)
				{
					double pen = penRate * 10.0 * ((period == 3 && left < 300) ? 0.7 : 1.0);
					double p2 = _r.NextDouble();
					if (p2 < pen)
					{
						awayPp = 120;
						AwayStats[1]++;
					}
					else if (p2 < 2.0 * pen)
					{
						homePp = 120;
						HomeStats[1]++;
					}
				}
				else
				{
					homePp = Math.Max(0, homePp - 10);
					awayPp = Math.Max(0, awayPp - 10);
				}
			}
		}
		Periods = 3;
		HomeStats[0] = (int)Math.Round(homeShotsExp + Normal(0.0, 2.5));
		AwayStats[0] = (int)Math.Round(awayShotsExp + Normal(0.0, 2.5));
		if (HomeScore != AwayScore)
		{
			FixShots();
			return;
		}
		if (_playoff)
		{
			for (int ot = 1; ot <= 8; ot++)
			{
				if (HomeScore != AwayScore)
				{
					break;
				}
				Periods = 3 + ot;
				for (int i = 0; i < 1200; i += 10)
				{
					if (HomeScore != AwayScore)
					{
						break;
					}
					double hr2 = esRate * homeMul;
					double ar2 = esRate * awayMul;
					HomeStats[0] += (Chance(7.0 / 90.0 * Math.Sqrt(homeMul)) ? 1 : 0);
					AwayStats[0] += (Chance(7.0 / 90.0 * Math.Sqrt(awayMul)) ? 1 : 0);
					double pH2 = 1.0 - Math.Exp((0.0 - hr2) * 10.0);
					double pA2 = 1.0 - Math.Exp((0.0 - ar2) * 10.0);
					double roll2 = _r.NextDouble();
					int dummy = 0;
					int dummy2 = 0;
					if (roll2 < pH2)
					{
						HockeyGoal(home: true, Periods, i + _r.Next(10), 1, ref dummy, ref dummy2);
					}
					else if (roll2 < pH2 + pA2)
					{
						HockeyGoal(home: false, Periods, i + _r.Next(10), 1, ref dummy, ref dummy2);
					}
				}
			}
			if (HomeScore == AwayScore)
			{
				int d1 = 0;
				int d2 = 0;
				HockeyGoal(Chance(Logistic(3.0 * _diff)), Periods, 1199, 1, ref d1, ref d2);
			}
			Result = ResultType.Overtime;
			FixShots();
			return;
		}
		Periods = 4;
		double otRate = 0.00145;
		for (int j = 0; j < 300; j += 10)
		{
			if (HomeScore != AwayScore)
			{
				break;
			}
			double pH3 = 1.0 - Math.Exp((0.0 - otRate) * homeMul * 10.0);
			double pA3 = 1.0 - Math.Exp((0.0 - otRate) * awayMul * 10.0);
			if (Chance(0.3))
			{
				HomeStats[0]++;
			}
			if (Chance(0.3))
			{
				AwayStats[0]++;
			}
			double roll3 = _r.NextDouble();
			int d3 = 0;
			int d4 = 0;
			if (roll3 < pH3)
			{
				HockeyGoal(home: true, 4, j + _r.Next(10), 1, ref d3, ref d4);
			}
			else if (roll3 < pH3 + pA3)
			{
				HockeyGoal(home: false, 4, j + _r.Next(10), 1, ref d3, ref d4);
			}
		}
		if (HomeScore != AwayScore)
		{
			Result = ResultType.Overtime;
			FixShots();
			return;
		}
		Periods = 5;
		Result = ResultType.Shootout;
		double ph = Math.Min(0.6, 0.32 * Math.Exp(0.25 * k * _diff));
		double pa = Math.Min(0.6, 0.32 * Math.Exp(-0.25 * k * _diff));
		int hg = 0;
		int ag = 0;
		for (int round = 1; round <= 3; round++)
		{
			if (Chance(ph))
			{
				hg++;
			}
			if (ag + (4 - round) < hg)
			{
				break;
			}
			if (Chance(pa))
			{
				ag++;
			}
			if (hg + (3 - round) < ag || ag + (3 - round) < hg)
			{
				break;
			}
		}
		for (int l = 4; l <= 30; l++)
		{
			if (hg != ag)
			{
				break;
			}
			if (Chance(ph))
			{
				hg++;
			}
			if (Chance(pa))
			{
				ag++;
			}
		}
		if (hg == ag)
		{
			if (Chance(0.5))
			{
				hg++;
			}
			else
			{
				ag++;
			}
		}
		HomePens = hg;
		AwayPens = ag;
		Note = "Shootout " + Math.Max(hg, ag) + "-" + Math.Min(hg, ag);
		if (hg > ag)
		{
			HomeScore++;
		}
		else
		{
			AwayScore++;
		}
		FixShots();
	}

	private void HockeyGoal(bool home, int period, int seconds, int kind, ref int homePp, ref int awayPp)
	{
		Add(period, Math.Min(1199, seconds), home, 1, kind);
		int[] st = (home ? HomeStats : AwayStats);
		st[0]++;
		if (kind == 2)
		{
			st[2]++;
			if (home)
			{
				homePp = 0;
			}
			else
			{
				awayPp = 0;
			}
		}
		if (kind == 4)
		{
			st[3]++;
		}
		if (kind == 3)
		{
			st[4]++;
		}
	}

	private void FixShots()
	{
		int hEn = HomeStats[3];
		int aEn = AwayStats[3];
		int hGoals = 0;
		int aGoals = 0;
		foreach (FlowEvent @event in Events)
		{
			if (@event.Home)
			{
				hGoals++;
			}
			else
			{
				aGoals++;
			}
		}
		HomeStats[0] = Math.Max(HomeStats[0], hGoals - hEn + 8);
		AwayStats[0] = Math.Max(AwayStats[0], aGoals - aEn + 8);
	}

	private static double SoccerEdge(Level level)
	{
		return level switch
		{
			Level.College => 1.0, 
			Level.Pro => 0.98, 
			_ => 1.0, 
		};
	}

	private void PlaySoccer()
	{
		double num = ((_level == Level.HighSchool) ? 1.8 : ((_level == Level.College) ? 1.6 : 1.45)) * WeatherCodes.Scoring(Sport.Soccer, _weather);
		double k = SoccerEdge(_level);
		double homeMul = Math.Exp(k * _diff);
		double awayMul = Math.Exp((0.0 - k) * _diff);
		int added1 = 1 + Poisson(2.3);
		int added2 = 3 + Poisson(3.2);
		double baseRate = num / 99.0;
		bool homeRed = false;
		bool awayRed = false;
		for (int half = 1; half <= 2; half++)
		{
			int regular = 45;
			int added3 = ((half == 1) ? added1 : added2);
			for (int i = 1; i <= regular + added3; i++)
			{
				int minute = ((half == 1) ? Math.Min(i, 45) : Math.Min(45 + i, 90));
				int plus = ((i > regular) ? (i - regular) : 0);
				double w;
				if (plus <= 0)
				{
					if (minute <= 15)
					{
						w = 0.86;
					}
					else if (minute <= 30)
					{
						w = 0.92;
					}
					else if (minute <= 45)
					{
						w = 0.98;
					}
					else if (minute <= 60)
					{
						w = 1.02;
					}
					else
					{
						w = ((minute <= 75) ? 1.06 : 1.2);
					}
				}
				else
				{
					w = ((half == 1) ? 1.2 : 1.35);
				}
				int m = HomeScore - AwayScore;
				double hr = baseRate * w * homeMul;
				double ar = baseRate * w * awayMul;
				int num2;
				double num3;
				if (m != 0)
				{
					if (half == 2)
					{
						num2 = ((minute >= 70) ? 1 : 0);
						if (num2 != 0)
						{
							num3 = 1.3;
							goto IL_01b8;
						}
					}
					else
					{
						num2 = 0;
					}
					num3 = 1.15;
					goto IL_01b8;
				}
				goto IL_01f5;
				IL_01b8:
				double push = num3;
				double sit = ((num2 != 0) ? 1.0 : 0.9);
				if (m < 0)
				{
					hr *= push;
					ar *= sit;
				}
				else
				{
					ar *= push;
					hr *= sit;
				}
				goto IL_01f5;
				IL_01f5:
				if (homeRed)
				{
					hr *= 0.65;
					ar *= 1.3;
				}
				if (awayRed)
				{
					ar *= 0.65;
					hr *= 1.3;
				}
				string clock = ((plus > 0) ? (minute + "+" + plus + "'") : (minute + "'"));
				int seconds = minute * 60 + plus * 10 + _r.Next(10);
				double roll = _r.NextDouble();
				double pH = 1.0 - Math.Exp(0.0 - hr);
				double pA = 1.0 - Math.Exp(0.0 - ar);
				if (roll < pH)
				{
					SoccerGoal(home: true, half, seconds, clock);
				}
				else if (roll < pH + pA)
				{
					SoccerGoal(home: false, half, seconds, clock);
				}
				double cardW = 0.6 + 0.8 * ((half == 1) ? ((double)i / 50.0) : (1.0 + (double)i / 50.0)) / 2.0;
				if (Chance(19.0 / 980.0 * cardW))
				{
					Add(half, seconds, home: true, 0, 4, 0, -1, clock);
				}
				if (Chance(19.0 / 980.0 * cardW))
				{
					Add(half, seconds, home: false, 0, 4, 0, -1, clock);
				}
				if (!homeRed && Chance(0.0011224489795918367 * cardW))
				{
					homeRed = true;
					Add(half, seconds, home: true, 0, 5, 0, -1, clock);
				}
				if (!awayRed && Chance(0.0011224489795918367 * cardW))
				{
					awayRed = true;
					Add(half, seconds, home: false, 0, 5, 0, -1, clock);
				}
			}
		}
		Periods = 2;
		if (!_playoff || HomeScore != AwayScore)
		{
			return;
		}
		Result = ResultType.Penalties;
		double conv;
		if (_level == Level.Pro)
		{
			conv = 0.75;
		}
		else
		{
			conv = ((_level == Level.College) ? 0.7 : 0.66);
		}
		double ph = Math.Min(0.92, conv * Math.Exp(0.15 * k * _diff));
		double pa = Math.Min(0.92, conv * Math.Exp(-0.15 * k * _diff));
		bool homeFirst = Chance(0.5);
		int hg = 0;
		int ag = 0;
		int hTaken = 0;
		int aTaken = 0;
		for (int kick = 0; kick < 10; kick++)
		{
			if (kick % 2 == 0 == homeFirst)
			{
				hTaken++;
				if (Chance(ph))
				{
					hg++;
				}
			}
			else
			{
				aTaken++;
				if (Chance(pa))
				{
					ag++;
				}
			}
			if (hg + (5 - hTaken) < ag || ag + (5 - aTaken) < hg)
			{
				break;
			}
		}
		for (int round = 6; round <= 40; round++)
		{
			if (hg != ag)
			{
				break;
			}
			if (Chance(ph))
			{
				hg++;
			}
			if (Chance(pa))
			{
				ag++;
			}
		}
		if (hg == ag)
		{
			if (Chance(0.5))
			{
				hg++;
			}
			else
			{
				ag++;
			}
		}
		HomePens = hg;
		AwayPens = ag;
	}

	private void SoccerGoal(bool home, int half, int seconds, string clock)
	{
		int kind = ((!(_r.NextDouble() < 0.09)) ? 1 : 2);
		Add(half, seconds, home, 1, kind, 0, -1, clock);
	}
}
