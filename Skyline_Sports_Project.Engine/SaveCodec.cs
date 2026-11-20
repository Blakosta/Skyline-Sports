using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Skyline_Sports_Project.Engine;

public static class SaveCodec
{
	private class TokenWriter
	{
		private readonly StringBuilder _sb = new StringBuilder();

		private bool _first = true;

		private void Raw(string value)
		{
			if (!_first)
			{
				_sb.Append('\u001f');
			}
			_first = false;
			_sb.Append(value);
		}

		public void Int(int value)
		{
			Raw(value.ToString(CultureInfo.InvariantCulture));
		}

		public void Bool(bool value)
		{
			Raw(value ? "1" : "0");
		}

		public void Str(string value)
		{
			Raw((value ?? "").Replace('\u001f', ' '));
		}

		public void Dbl(double value)
		{
			Raw(value.ToString("G17", CultureInfo.InvariantCulture));
		}

		public void Strs(List<string> list)
		{
			Int(list?.Count ?? 0);
			if (list == null)
			{
				return;
			}
			foreach (string v in list)
			{
				Str(v);
			}
		}

		public void IntList(List<int> list)
		{
			Int(list.Count);
			foreach (int v in list)
			{
				Int(v);
			}
		}

		public void Ints(int[] values)
		{
			int n = values?.Length ?? 0;
			while (n > 0 && values[n - 1] == 0)
			{
				n--;
			}
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < n; i++)
			{
				if (i > 0)
				{
					sb.Append(',');
				}
				sb.Append(values[i].ToString(CultureInfo.InvariantCulture));
			}
			Raw(sb.ToString());
		}

		public override string ToString()
		{
			return _sb.ToString();
		}
	}

	private class TokenReader
	{
		private readonly string[] _tokens;

		private int _pos;

		public TokenReader(string blob)
		{
			_tokens = blob.Split(new[] { '\u001f' }, StringSplitOptions.None);
		}

		private string Next()
		{
			if (_pos >= _tokens.Length)
			{
				throw new FormatException("неожиданный конец данных");
			}
			return _tokens[_pos++];
		}

		public int Int()
		{
			return int.Parse(Next(), NumberStyles.Integer, CultureInfo.InvariantCulture);
		}

		public bool Bool()
		{
			return Next() == "1";
		}

		public string Str()
		{
			return Next();
		}

		public double Dbl()
		{
			double v = double.Parse(Next(), NumberStyles.Float, CultureInfo.InvariantCulture);
			if (!double.IsNaN(v) && !double.IsInfinity(v))
			{
				return v;
			}
			return 0.0;
		}

		public List<string> Strs()
		{
			int n = Int();
			if (n < 0 || n > 10000)
			{
				throw new FormatException("некорректный список текстовых значений");
			}
			List<string> list = new List<string>(n);
			for (int i = 0; i < n; i++)
			{
				list.Add(Str());
			}
			return list;
		}

		public int[] Ints()
		{
			string t = Next();
			if (t.Length == 0)
			{
				return new int[0];
			}
			string[] parts = t.Split(new[] { ',' }, StringSplitOptions.None);
			if (parts.Length > 200)
			{
				throw new FormatException("некорректная строка статистики");
			}
			int[] values = new int[parts.Length];
			for (int i = 0; i < parts.Length; i++)
			{
				values[i] = int.Parse(parts[i], NumberStyles.Integer, CultureInfo.InvariantCulture);
			}
			return values;
		}

		public List<int> IntList()
		{
			int n = Int();
			if (n < 0 || n > 100000)
			{
				throw new FormatException("неверный размер списка");
			}
			List<int> list = new List<int>(n);
			for (int i = 0; i < n; i++)
			{
				list.Add(Int());
			}
			return list;
		}
	}

	public const int Version = 21;

	private const char Sep = '\u001f';

	public static string Encode(SportsWorld world)
	{
		TokenWriter w = new TokenWriter();
		w.Int(21);
		w.Int(world.NextId);
		w.Int(world.LastTime);
		w.Int(world.Orgs.Count);
		foreach (Organization o in world.Orgs.Values)
		{
			w.Int(o.Id);
			w.Int((int)o.Level);
			w.Str(o.Name);
			w.Str(o.Mascot);
			w.Str(o.DistrictKey);
			w.Bool(o.IsUser);
		}
		w.Int(world.Teams.Count);
		foreach (Team t in world.Teams.Values)
		{
			w.Int(t.Id);
			w.Int(t.OrgId);
			w.Int((int)t.Sport);
			w.Int((int)t.Level);
			w.Int(t.VenueId);
			w.Int(t.Rating);
			w.Bool(t.IsUser);
			w.Bool(t.Retired);
			w.Int(t.FeaturedMonth);
			w.Int(t.Rotation);
			w.Int(t.Form);
			w.Int(t.RunBonus);
			w.Bool(t.Manual);
			w.Int(t.Budget);
			w.Int(t.Surprise);
			w.Int(t.Cinderella);
			w.Int(t.LastStarYear);
			w.Int(t.CinderellaYear);
			w.Int(t.RetiredNumbers.Count);
			foreach (RetiredNumber n in t.RetiredNumbers)
			{
				w.Int(n.Number);
				w.Str(n.Name);
				w.Str(n.Years);
				w.Str(n.Season);
			}
		}
		w.Int(world.Venues.Count);
		foreach (Venue v in world.Venues.Values)
		{
			w.Int(v.Id);
			w.Int((int)v.Type);
			w.Int((int)v.Level);
		}
		w.Int(world.Leagues.Count);
		foreach (League l in world.Leagues)
		{
			w.Int((int)l.Sport);
			w.Int((int)l.Level);
			w.IntList(l.TeamIds);
			w.IntList(l.PendingTeamIds);
			WriteSeason(w, l.Current);
			WriteSeason(w, l.Previous);
			w.Int(l.Champions.Count);
			foreach (ChampionRecord c in l.Champions)
			{
				w.Str(c.SeasonLabel);
				w.Str(c.TeamName);
				w.Str(c.RunnerUpName);
				w.Str(c.FinalScore);
				w.Bool(c.IsUser);
			}
			w.Int(l.LotteryDoneFor);
			w.Int(l.DraftDoneFor);
			w.Int(l.LeagueYearDoneFor);
			w.Int(l.TradeMonth);
			w.Int(l.Awards.Count);
			foreach (AwardRecord a in l.Awards)
			{
				w.Str(a.SeasonLabel);
				w.Str(a.Award);
				w.Str(a.Trophy);
				w.Int(a.PlayerId);
				w.Str(a.PlayerName);
				w.Int(a.TeamId);
				w.Str(a.TeamName);
				w.Str(a.Detail);
				w.Bool(a.IsUser);
			}
			WriteDraft(w, l.Draft);
			w.Int(l.CoachingDoneFor);
			w.Int(l.Picks.Count);
			foreach (PickAsset a2 in l.Picks)
			{
				w.Int(a2.Id);
				w.Int(a2.Year);
				w.Int(a2.Round);
				w.Int(a2.OriginalTeamId);
				w.Int(a2.OwnerTeamId);
			}
			w.Int(l.Records.Count);
			foreach (RecordBoard b in l.Records)
			{
				w.Str(b.Group);
				w.Str(b.Title);
				w.Bool(b.LowerIsBetter);
				w.Int(b.Entries.Count);
				foreach (RecordEntry e in b.Entries)
				{
					w.Int(e.Id);
					w.Str(e.Name);
					w.Str(e.Team);
					w.Str(e.Season);
					w.Dbl(e.Value);
					w.Str(e.Text);
				}
			}
			w.Int(l.History.Count);
			foreach (SeasonSummary h in l.History)
			{
				w.Str(h.Season);
				w.Str(h.Champion);
				w.Str(h.RunnerUp);
				w.Str(h.FinalScore);
				w.Bool(h.ChampionIsUser);
				w.Strs(h.Standings);
				w.Strs(h.Playoffs);
				w.Strs(h.Awards);
				w.Strs(h.Leaders);
				w.Strs(h.Notes);
			}
			w.Strs(l.SeasonNotes);
			w.Int(l.Rivalries.Count);
			foreach (Rivalry rv in l.Rivalries)
			{
				w.Int(rv.TeamA);
				w.Int(rv.TeamB);
				w.Int(rv.Heat);
				w.Int(rv.WinsA);
				w.Int(rv.WinsB);
				w.Int(rv.Ties);
				w.Int(rv.SeriesA);
				w.Int(rv.SeriesB);
				w.Str(rv.LastMeeting);
				w.Bool(rv.Crosstown);
			}
			WriteHall(w, l.HallOfFame);
			WriteHall(w, l.HallWaiting);
			List<int> champIds = new List<int>();
			foreach (ChampionRecord c2 in l.Champions)
			{
				champIds.Add(c2.TeamId);
			}
			w.IntList(champIds);
		}
		w.Int(world.Players.Count);
		foreach (Player p in world.Players.Values)
		{
			WritePlayer(w, p);
		}
		w.Int(world.Feed.Count);
		foreach (NewsPost n2 in world.Feed)
		{
			w.Int(n2.Id);
			w.Int(n2.Time);
			w.Str(n2.LeagueKey);
			w.Int((int)n2.Kind);
			w.Str(n2.Account);
			w.Str(n2.Handle);
			w.Str(n2.Text);
			w.Bool(n2.IsUser);
			w.Int(n2.Likes);
			w.Int(n2.Reposts);
		}
		w.Int(world.Coaches.Count);
		foreach (Coach c3 in world.Coaches.Values)
		{
			w.Int(c3.Id);
			w.Str(c3.FirstName);
			w.Str(c3.LastName);
			w.Int((int)c3.Sport);
			w.Int((int)c3.Level);
			w.Int((int)c3.Role);
			w.Int(c3.TeamId);
			w.Int(c3.BirthYear);
			w.Int(c3.Development);
			w.Int(c3.Tactics);
			w.Int(c3.Motivation);
			w.Int(c3.Recruiting);
			w.Int(c3.Potential);
			w.Int(c3.PrevOverall);
			w.Int(c3.Seasons);
			w.Int(c3.Wins);
			w.Int(c3.Losses);
			w.Int(c3.Ties);
			w.Int(c3.PlayoffWins);
			w.Int(c3.PlayoffLosses);
			w.Int(c3.Titles);
			w.Int(c3.HiredYear);
			w.Int(c3.BadSeasons);
			w.Str(c3.History);
		}
		w.Int(world.Talks.Count);
		foreach (ContractTalk k in world.Talks)
		{
			w.Int(k.PlayerId);
			w.Int(k.TeamId);
			w.Bool(k.Extension);
			w.Int(k.Ask);
			w.Int(k.AskYears);
			w.Int(k.Interest);
			w.Int(k.Patience);
			w.Int(k.Rounds);
			w.Int(k.Offer);
			w.Int(k.OfferYears);
			w.Int(k.Status);
			w.Str(k.Message);
			w.Int(k.LastMonth);
		}
		List<int> deciding = new List<int>();
		foreach (Player p2 in world.Players.Values)
		{
			if (p2.DecideMonth >= 0)
			{
				deciding.Add(p2.Id);
				deciding.Add(p2.DecideMonth);
			}
		}
		w.IntList(deciding);
		List<int> blocks = new List<int>();
		foreach (Team t2 in world.Teams.Values)
		{
			if (t2.Block.Count != 0)
			{
				blocks.Add(t2.Id);
				blocks.Add(t2.Block.Count);
				blocks.AddRange(t2.Block);
			}
		}
		w.IntList(blocks);
		w.Int(world.Offers.Count);
		foreach (TradeOffer o2 in world.Offers)
		{
			w.Int(o2.Id);
			w.Int(o2.FromTeamId);
			w.Int(o2.ToTeamId);
			w.IntList(o2.Give);
			w.IntList(o2.GivePicks);
			w.IntList(o2.Get);
			w.Int(o2.Month);
		}
		List<int> prestige = new List<int>();
		foreach (Team t3 in world.Teams.Values)
		{
			if (t3.Prestige != 0 || t3.RecruitHours != 0)
			{
				prestige.Add(t3.Id);
				prestige.Add(t3.Prestige);
				prestige.Add(t3.RecruitHours);
			}
		}
		w.IntList(prestige);
		w.Int(world.RecruitBoard.Count);
		foreach (RecruitTarget r in world.RecruitBoard)
		{
			w.Int(r.PlayerId);
			w.Int((int)r.Sport);
			w.Int(r.Year);
			w.Int(r.Stars);
			w.Int(r.CommittedTo);
			w.Bool(r.National);
		}
		w.Int(world.RecruitEfforts.Count);
		foreach (RecruitEffort e2 in world.RecruitEfforts)
		{
			w.Int(e2.PlayerId);
			w.Int(e2.TeamId);
			w.Int(e2.Effort);
			w.Bool(e2.Offered);
			w.Int(e2.Calls);
			w.Bool(e2.Watched);
			w.Bool(e2.HomeVisit);
			w.Bool(e2.OfficialVisit);
			w.Int(e2.Month);
		}
		List<Team> money = new List<Team>();
		foreach (Team t4 in world.Teams.Values)
		{
			if (t4.Dead.Count > 0 || t4.Buyouts > 0)
			{
				money.Add(t4);
			}
		}
		w.Int(money.Count);
		foreach (Team t5 in money)
		{
			w.Int(t5.Id);
			w.Int(t5.Buyouts);
			w.Int(t5.Dead.Count);
			foreach (DeadCharge c4 in t5.Dead)
			{
				w.Int(c4.PlayerId);
				w.Str(c4.Name);
				w.Str(c4.Reason);
				w.IntList(c4.Amounts);
			}
		}
		w.Strs(world.Districts);
		w.Int(world.Retired.Count);
		foreach (Player p3 in world.Retired.Values)
		{
			WritePlayer(w, p3);
			w.Int(p3.RetiredYear);
		}
		w.Int(world.Coaches.Count);
		foreach (Coach c5 in world.Coaches.Values)
		{
			w.Int(c5.Id);
			w.Str(c5.Hometown);
			w.Int(c5.FormerPlayerId);
			w.Str(c5.Played);
		}
		List<Match> boxed = new List<Match>();
		foreach (League l2 in world.Leagues)
		{
			Season[] array = new Season[2] { l2.Current, l2.Previous };
			foreach (Season s in array)
			{
				if (s == null)
				{
					continue;
				}
				foreach (Match g in s.Games)
				{
					if (g.Box != null)
					{
						boxed.Add(g);
					}
				}
			}
		}
		w.Int(boxed.Count);
		foreach (Match g2 in boxed)
		{
			GameBox b2 = g2.Box;
			w.Int(g2.Id);
			w.Int(b2.Lines.Count);
			foreach (BoxLine line in b2.Lines)
			{
				w.Int(line.PlayerId);
				w.Bool(line.Home);
				w.Ints(line.Stats);
			}
			w.Int(b2.Events.Count);
			foreach (BoxEvent e3 in b2.Events)
			{
				w.Int(e3.Period);
				w.Str(e3.Clock);
				w.Bool(e3.Home);
				w.Int(e3.Kind);
				w.Str(e3.Text);
				w.Int(e3.HomeScore);
				w.Int(e3.AwayScore);
			}
			w.IntList(b2.HomePeriods);
			w.IntList(b2.AwayPeriods);
			w.Str(b2.Note);
		}
		w.Int(boxed.Count);
		foreach (Match g3 in boxed)
		{
			w.Int(g3.Id);
			w.Int(g3.Box.Events.Count);
			foreach (BoxEvent e4 in g3.Box.Events)
			{
				w.IntList(e4.Players ?? new List<int>());
			}
		}
		List<Match> live = new List<Match>();
		foreach (League l3 in world.Leagues)
		{
			if (l3.Current == null)
			{
				continue;
			}
			foreach (Match g4 in l3.Current.Games)
			{
				if (g4.Live != null && !g4.Played)
				{
					live.Add(g4);
				}
			}
		}
		w.Int(live.Count);
		foreach (Match g5 in live)
		{
			LiveGame x = g5.Live;
			w.Int(g5.Id);
			w.Int(x.Start);
			w.Int(x.End);
			w.Int(x.HomeScore);
			w.Int(x.AwayScore);
			w.Int((int)x.Result);
			w.Int(x.HomePenalties);
			w.Int(x.AwayPenalties);
			w.Str(x.Stars);
			w.Dbl(x.HomeChance);
			w.Dbl(x.HomeRating);
			w.Dbl(x.AwayRating);
			w.Bool(x.Playoff);
			w.Bool(x.Box != null);
			if (x.Box != null)
			{
				WriteGameBox(w, x.Box);
			}
		}
		WriteV12(w, world);
		List<Team> users = new List<Team>();
		foreach (Team t6 in world.Teams.Values)
		{
			if (t6.IsUser && !t6.Retired)
			{
				users.Add(t6);
			}
		}
		w.Int(users.Count);
		foreach (Team t7 in users)
		{
			w.Int(t7.Id);
			w.Bool(t7.WatchFirst);
			w.Int(t7.LastLiveMonth);
		}
		WriteV14(w, world);
		return w.ToString();
	}

	private static void WriteGameBox(TokenWriter w, GameBox b)
	{
		w.Int(b.Lines.Count);
		foreach (BoxLine line in b.Lines)
		{
			w.Int(line.PlayerId);
			w.Bool(line.Home);
			w.Ints(line.Stats);
		}
		w.Int(b.Events.Count);
		foreach (BoxEvent e in b.Events)
		{
			w.Int(e.Period);
			w.Str(e.Clock);
			w.Bool(e.Home);
			w.Int(e.Kind);
			w.Str(e.Text);
			w.Int(e.HomeScore);
			w.Int(e.AwayScore);
			w.IntList(e.Players ?? new List<int>());
		}
		w.IntList(b.HomePeriods);
		w.IntList(b.AwayPeriods);
		w.Str(b.Note);
	}

	private static void WriteV14(TokenWriter w, SportsWorld world)
	{
		List<int> spot = new List<int>();
		foreach (League l in world.Leagues)
		{
			Season[] array = new Season[2] { l.Current, l.Previous };
			foreach (Season s in array)
			{
				if (s == null)
				{
					continue;
				}
				foreach (Match g in s.Games)
				{
					if (g.Spotlight)
					{
						spot.Add(g.Id);
					}
				}
			}
		}
		w.IntList(spot);
		WriteV14More(w, world);
	}

	private static void ReadV14(TokenReader r, SportsWorld world, int version)
	{
		HashSet<int> spot = new HashSet<int>(r.IntList());
		foreach (League l in world.Leagues)
		{
			Season[] array = new Season[2] { l.Current, l.Previous };
			foreach (Season s in array)
			{
				if (s == null)
				{
					continue;
				}
				foreach (Match g in s.Games)
				{
					if (spot.Contains(g.Id))
					{
						g.Spotlight = true;
					}
				}
			}
		}
		ReadV14More(r, world, version);
	}

	private static void WriteV12(TokenWriter w, SportsWorld world)
	{
		List<Team> farms = new List<Team>();
		foreach (Team t in world.Teams.Values)
		{
			if (t.ParentTeamId >= 0)
			{
				farms.Add(t);
			}
		}
		w.Int(farms.Count);
		foreach (Team t2 in farms)
		{
			w.Int(t2.Id);
			w.Int(t2.ParentTeamId);
		}
	}

	private static void ReadV12(TokenReader r, SportsWorld world, int version)
	{
		int farms = Count(r, 100000, "фарм-команды");
		for (int k = 0; k < farms; k++)
		{
			int id = r.Int();
			int parent = r.Int();
			if (world.Teams.TryGetValue(id, out var t))
			{
				t.ParentTeamId = parent;
			}
		}
	}

	private static GameBox ReadGameBox(TokenReader r)
	{
		GameBox b = new GameBox();
		int lines = Count(r, 1000, "строки протокола матча");
		for (int m = 0; m < lines; m++)
		{
			b.Lines.Add(new BoxLine
			{
				PlayerId = r.Int(),
				Home = r.Bool(),
				Stats = r.Ints()
			});
		}
		int events = Count(r, 1000, "события матча");
		for (int i = 0; i < events; i++)
		{
			b.Events.Add(new BoxEvent
			{
				Period = r.Int(),
				Clock = r.Str(),
				Home = r.Bool(),
				Kind = r.Int(),
				Text = r.Str(),
				HomeScore = r.Int(),
				AwayScore = r.Int(),
				Players = r.IntList()
			});
		}
		b.HomePeriods = r.IntList();
		b.AwayPeriods = r.IntList();
		b.Note = r.Str();
		return b;
	}

	private static void WritePlayer(TokenWriter w, Player p)
	{
		w.Int(p.Id);
		w.Int(p.TeamId);
		w.Str(p.FirstName);
		w.Str(p.LastName);
		w.Int((int)p.Position);
		w.Int(p.Number);
		w.Int(p.BirthYear);
		w.Int(p.ClassYear);
		w.Int(p.Overall);
		w.Int(p.Potential);
		w.Int(p.Seasons);
		w.Ints(p.Season);
		w.Ints(p.LastSeason);
		w.Ints(p.Career);
		w.Int((int)p.Sport);
		w.Int((int)p.Level);
		w.Int((int)p.Status);
		w.Int(p.PrevOverall);
		w.Int(p.Salary);
		w.Int(p.ContractYears);
		w.Int(p.FreeAgentMonth);
		w.Int(p.DraftYear);
		w.Int(p.DraftRound);
		w.Int(p.DraftPick);
		w.Int(p.DraftTeamId);
		w.Str(p.From);
		w.Ints(p.Playoff);
		w.Int((int)p.Priority);
		w.Int(p.LastTeamId);
		w.Int(p.InjuredUntil);
		w.Str(p.Injury);
		w.Strs(p.InjuryLog);
		w.Int(p.Height);
		w.Int(p.Weight);
		w.Str(p.Hometown);
		w.Str(p.Hand);
		w.Int(p.Titles);
		w.Int(p.PeakOvr);
		w.Strs(p.Honors);
		w.Int(p.History.Count);
		foreach (SeasonLine h in p.History)
		{
			w.Str(h.Season);
			w.Int(h.TeamId);
			w.Str(h.Team);
			w.Int((int)h.Level);
			w.Int(h.Ovr);
			w.Ints(h.Stats);
			w.Ints(h.Playoff);
		}
	}

	private static Player ReadPlayer(TokenReader r, int version)
	{
		Player p = new Player
		{
			Id = r.Int(),
			TeamId = r.Int(),
			FirstName = r.Str(),
			LastName = r.Str(),
			Position = (Position)r.Int(),
			Number = r.Int(),
			BirthYear = r.Int(),
			ClassYear = r.Int(),
			Overall = r.Int(),
			Potential = r.Int(),
			Seasons = r.Int(),
			Season = r.Ints(),
			LastSeason = r.Ints(),
			Career = r.Ints()
		};
		if (version >= 5)
		{
			p.Sport = (Sport)r.Int();
			p.Level = (Level)r.Int();
			p.Status = (PlayerStatus)r.Int();
			p.PrevOverall = r.Int();
			p.Salary = r.Int();
			p.ContractYears = r.Int();
			p.FreeAgentMonth = r.Int();
			p.DraftYear = r.Int();
			p.DraftRound = r.Int();
			p.DraftPick = r.Int();
			p.DraftTeamId = r.Int();
			p.From = r.Str();
			p.Playoff = r.Ints();
		}
		if (version >= 6)
		{
			p.Priority = (PlayerPriority)r.Int();
			p.LastTeamId = r.Int();
		}
		if (version >= 8)
		{
			p.InjuredUntil = r.Int();
			p.Injury = r.Str();
			p.InjuryLog = r.Strs();
			p.Height = r.Int();
			p.Weight = r.Int();
			p.Hometown = r.Str();
			p.Hand = r.Str();
			p.Titles = r.Int();
			p.PeakOvr = r.Int();
			p.Honors = r.Strs();
			int lines = Count(r, 1000, "история игрока");
			for (int k = 0; k < lines; k++)
			{
				p.History.Add(new SeasonLine
				{
					Season = r.Str(),
					TeamId = r.Int(),
					Team = r.Str(),
					Level = (Level)r.Int(),
					Ovr = r.Int(),
					Stats = r.Ints(),
					Playoff = r.Ints()
				});
			}
		}
		else
		{
			p.PrevOverall = p.Overall;
		}
		return p;
	}

	private static void WriteHall(TokenWriter w, List<HallEntry> list)
	{
		w.Int(list.Count);
		foreach (HallEntry h in list)
		{
			w.Int(h.PlayerId);
			w.Str(h.Name);
			w.Str(h.Position);
			w.Str(h.Teams);
			w.Str(h.Years);
			w.Str(h.Summary);
			w.Int(h.Score);
			w.Int(h.EligibleYear);
			w.Str(h.Inducted);
			w.Int(h.Number);
			w.Int(h.MainTeamId);
		}
	}

	private static List<HallEntry> ReadHall(TokenReader r)
	{
		int n = Count(r, 100000, "зал славы");
		List<HallEntry> list = new List<HallEntry>(n);
		for (int i = 0; i < n; i++)
		{
			list.Add(new HallEntry
			{
				PlayerId = r.Int(),
				Name = r.Str(),
				Position = r.Str(),
				Teams = r.Str(),
				Years = r.Str(),
				Summary = r.Str(),
				Score = r.Int(),
				EligibleYear = r.Int(),
				Inducted = r.Str(),
				Number = r.Int(),
				MainTeamId = r.Int()
			});
		}
		return list;
	}

	private static void WriteDraft(TokenWriter w, DraftState d)
	{
		w.Bool(d != null);
		if (d == null)
		{
			return;
		}
		w.Int(d.Year);
		w.Bool(d.LotteryDone);
		w.Bool(d.Started);
		w.Bool(d.Complete);
		w.Int(d.Rounds);
		w.Int(d.Month);
		w.Int(d.Lottery.Count);
		foreach (string line in d.Lottery)
		{
			w.Str(line);
		}
		w.IntList(d.Order);
		w.Int(d.Picks.Count);
		foreach (DraftPick p in d.Picks)
		{
			w.Int(p.Number);
			w.Int(p.Round);
			w.Int(p.TeamId);
			w.Int(p.PlayerId);
			w.Str(p.PlayerName);
			w.Str(p.Position);
			w.Int(p.Overall);
			w.Str(p.Grade);
			w.Str(p.From);
			w.Int(p.OriginalTeamId);
		}
	}

	private static DraftState ReadDraft(TokenReader r, int version)
	{
		if (!r.Bool())
		{
			return null;
		}
		DraftState d = new DraftState
		{
			Year = r.Int(),
			LotteryDone = r.Bool(),
			Started = r.Bool(),
			Complete = r.Bool(),
			Rounds = r.Int(),
			Month = r.Int()
		};
		int lines = r.Int();
		if (lines < 0 || lines > 1000)
		{
			throw new FormatException("некорректная жеребьёвка");
		}
		for (int i = 0; i < lines; i++)
		{
			d.Lottery.Add(r.Str());
		}
		d.Order = r.IntList();
		int picks = r.Int();
		if (picks < 0 || picks > 10000)
		{
			throw new FormatException("некорректный драфт");
		}
		for (int j = 0; j < picks; j++)
		{
			d.Picks.Add(new DraftPick
			{
				Number = r.Int(),
				Round = r.Int(),
				TeamId = r.Int(),
				PlayerId = r.Int(),
				PlayerName = r.Str(),
				Position = r.Str(),
				Overall = r.Int(),
				Grade = r.Str(),
				From = r.Str(),
				OriginalTeamId = ((version >= 6) ? r.Int() : (-1))
			});
		}
		return d;
	}

	public static bool Decode(string blob, SportsWorld world, out string error)
	{
		error = "";
		world.Clear();
		if (string.IsNullOrEmpty(blob))
		{
			return true;
		}
		try
		{
			TokenReader r = new TokenReader(blob);
			int version = r.Int();
			if (version < 1 || version > 21)
			{
				error = "Спортивные данные сохранены более новой версией мода (версия данных " + version + ").";
				return false;
			}
			world.NextId = r.Int();
			world.LastTime = UpgradeTime(r.Int(), version);
			int orgCount = r.Int();
			for (int i = 0; i < orgCount; i++)
			{
				Organization o = new Organization
				{
					Id = r.Int(),
					Level = (Level)r.Int(),
					Name = r.Str(),
					Mascot = r.Str(),
					DistrictKey = r.Str(),
					IsUser = r.Bool()
				};
				world.Orgs[o.Id] = o;
			}
			int teamCount = r.Int();
			for (int j = 0; j < teamCount; j++)
			{
				Team t = new Team
				{
					Id = r.Int(),
					OrgId = r.Int(),
					Sport = (Sport)r.Int(),
					Level = (Level)r.Int(),
					VenueId = r.Int(),
					Rating = r.Int(),
					IsUser = r.Bool(),
					Retired = r.Bool()
				};
				if (version >= 4)
				{
					t.FeaturedMonth = r.Int();
					t.Rotation = r.Int();
				}
				if (version >= 5)
				{
					t.Form = r.Int();
					t.RunBonus = r.Int();
					t.Manual = r.Bool();
					t.Budget = r.Int();
				}
				if (version >= 6)
				{
					t.Surprise = r.Int();
					t.Cinderella = r.Int();
					t.LastStarYear = r.Int();
					t.CinderellaYear = r.Int();
				}
				if (version >= 8)
				{
					int retired = Count(r, 1000, "выведенные из обращения номера");
					for (int k = 0; k < retired; k++)
					{
						t.RetiredNumbers.Add(new RetiredNumber
						{
							Number = r.Int(),
							Name = r.Str(),
							Years = r.Str(),
							Season = r.Str()
						});
					}
				}
				world.Teams[t.Id] = t;
			}
			int venueCount = r.Int();
			for (int l = 0; l < venueCount; l++)
			{
				Venue v = new Venue
				{
					Id = r.Int(),
					Type = (VenueType)r.Int(),
					Level = (Level)r.Int()
				};
				world.Venues[v.Id] = v;
			}
			int leagueCount = r.Int();
			for (int m = 0; m < leagueCount; m++)
			{
				League l2 = new League
				{
					Sport = (Sport)r.Int(),
					Level = (Level)r.Int()
				};
				l2.TeamIds = r.IntList();
				l2.PendingTeamIds = r.IntList();
				l2.Current = ReadSeason(r, version);
				l2.Previous = ReadSeason(r, version);
				int champCount = r.Int();
				for (int c = 0; c < champCount; c++)
				{
					l2.Champions.Add(new ChampionRecord
					{
						SeasonLabel = r.Str(),
						TeamName = r.Str(),
						RunnerUpName = r.Str(),
						FinalScore = r.Str(),
						IsUser = r.Bool()
					});
				}
				if (version >= 5)
				{
					l2.LotteryDoneFor = r.Int();
					l2.DraftDoneFor = r.Int();
					l2.LeagueYearDoneFor = r.Int();
					l2.TradeMonth = r.Int();
					int awardCount = r.Int();
					if (awardCount < 0 || awardCount > 100000)
					{
						throw new FormatException("некорректное число наград");
					}
					for (int a = 0; a < awardCount; a++)
					{
						l2.Awards.Add(new AwardRecord
						{
							SeasonLabel = r.Str(),
							Award = r.Str(),
							Trophy = r.Str(),
							PlayerId = r.Int(),
							PlayerName = r.Str(),
							TeamId = r.Int(),
							TeamName = r.Str(),
							Detail = r.Str(),
							IsUser = r.Bool()
						});
					}
					l2.Draft = ReadDraft(r, version);
				}
				else if (l2.Previous != null)
				{
					l2.LotteryDoneFor = l2.Previous.Year;
					l2.DraftDoneFor = l2.Previous.Year;
					l2.LeagueYearDoneFor = l2.Previous.Year;
				}
				if (version >= 6)
				{
					l2.CoachingDoneFor = r.Int();
					int picks = Count(r, 100000, "picks");
					for (int n = 0; n < picks; n++)
					{
						l2.Picks.Add(new PickAsset
						{
							Id = r.Int(),
							Year = r.Int(),
							Round = r.Int(),
							OriginalTeamId = r.Int(),
							OwnerTeamId = r.Int()
						});
					}
					int boards = Count(r, 10000, "records");
					for (int b = 0; b < boards; b++)
					{
						RecordBoard board = new RecordBoard
						{
							Group = r.Str(),
							Title = r.Str(),
							LowerIsBetter = r.Bool()
						};
						int entries = Count(r, 1000, "записи рекордов");
						for (int e = 0; e < entries; e++)
						{
							board.Entries.Add(new RecordEntry
							{
								Id = r.Int(),
								Name = r.Str(),
								Team = r.Str(),
								Season = r.Str(),
								Value = r.Dbl(),
								Text = r.Str()
							});
						}
						l2.Records.Add(board);
					}
					int seasons = Count(r, 1000, "history");
					for (int h = 0; h < seasons; h++)
					{
						l2.History.Add(new SeasonSummary
						{
							Season = r.Str(),
							Champion = r.Str(),
							RunnerUp = r.Str(),
							FinalScore = r.Str(),
							ChampionIsUser = r.Bool(),
							Standings = r.Strs(),
							Playoffs = r.Strs(),
							Awards = r.Strs(),
							Leaders = r.Strs(),
							Notes = r.Strs()
						});
					}
					l2.SeasonNotes = r.Strs();
				}
				if (version >= 8)
				{
					int rivals = Count(r, 100000, "rivalries");
					for (int num = 0; num < rivals; num++)
					{
						l2.Rivalries.Add(new Rivalry
						{
							TeamA = r.Int(),
							TeamB = r.Int(),
							Heat = r.Int(),
							WinsA = r.Int(),
							WinsB = r.Int(),
							Ties = r.Int(),
							SeriesA = r.Int(),
							SeriesB = r.Int(),
							LastMeeting = r.Str(),
							Crosstown = r.Bool()
						});
					}
					l2.HallOfFame = ReadHall(r);
					l2.HallWaiting = ReadHall(r);
					List<int> champIds = r.IntList();
					for (int num2 = 0; num2 < champIds.Count && num2 < l2.Champions.Count; num2++)
					{
						l2.Champions[num2].TeamId = champIds[num2];
					}
				}
				world.Leagues.Add(l2);
			}
			if (version >= 4)
			{
				int playerCount = r.Int();
				if (playerCount < 0 || playerCount > 1000000)
				{
					throw new FormatException("некорректное число игроков");
				}
				for (int num3 = 0; num3 < playerCount; num3++)
				{
					Player p = ReadPlayer(r, version);
					world.Players[p.Id] = p;
				}
			}
			if (version >= 5)
			{
				int postCount = r.Int();
				if (postCount < 0 || postCount > 100000)
				{
					throw new FormatException("некорректная лента новостей");
				}
				for (int num4 = 0; num4 < postCount; num4++)
				{
					world.Feed.Add(new NewsPost
					{
						Id = r.Int(),
						Time = r.Int(),
						LeagueKey = r.Str(),
						Kind = (NewsKind)r.Int(),
						Account = r.Str(),
						Handle = r.Str(),
						Text = r.Str(),
						IsUser = r.Bool(),
						Likes = r.Int(),
						Reposts = r.Int()
					});
				}
			}
			if (version < 8)
			{
				world.LoadedWithoutBios = true;
			}
			if (version >= 6)
			{
				int coaches = Count(r, 100000, "coaches");
				for (int num5 = 0; num5 < coaches; num5++)
				{
					Coach c2 = new Coach
					{
						Id = r.Int(),
						FirstName = r.Str(),
						LastName = r.Str(),
						Sport = (Sport)r.Int(),
						Level = (Level)r.Int(),
						Role = (CoachRole)r.Int(),
						TeamId = r.Int(),
						BirthYear = r.Int(),
						Development = r.Int(),
						Tactics = r.Int(),
						Motivation = r.Int(),
						Recruiting = r.Int(),
						Potential = r.Int(),
						PrevOverall = r.Int(),
						Seasons = r.Int(),
						Wins = r.Int(),
						Losses = r.Int(),
						Ties = r.Int(),
						PlayoffWins = r.Int(),
						PlayoffLosses = r.Int(),
						Titles = r.Int(),
						HiredYear = r.Int(),
						BadSeasons = r.Int(),
						History = r.Str()
					};
					if (version < 7)
					{
						int oldOverall = c2.Overall;
						c2.Development = SportsWorld.RescaleOldCoachRating(c2.Development, c2.Level);
						c2.Tactics = SportsWorld.RescaleOldCoachRating(c2.Tactics, c2.Level);
						c2.Motivation = SportsWorld.RescaleOldCoachRating(c2.Motivation, c2.Level);
						c2.Recruiting = SportsWorld.RescaleOldCoachRating(c2.Recruiting, c2.Level);
						c2.Potential = Math.Max(c2.Overall, SportsWorld.RescaleOldCoachRating(c2.Potential, c2.Level));
						c2.PrevOverall = c2.Overall - (oldOverall - c2.PrevOverall);
					}
					world.Coaches[c2.Id] = c2;
				}
			}
			else
			{
				world.GivePriorities();
				world.LoadedWithoutCoaches = true;
			}
			if (version >= 9)
			{
				int talks = Count(r, 100000, "talks");
				for (int num6 = 0; num6 < talks; num6++)
				{
					world.Talks.Add(new ContractTalk
					{
						PlayerId = r.Int(),
						TeamId = r.Int(),
						Extension = r.Bool(),
						Ask = r.Int(),
						AskYears = r.Int(),
						Interest = r.Int(),
						Patience = r.Int(),
						Rounds = r.Int(),
						Offer = r.Int(),
						OfferYears = r.Int(),
						Status = r.Int(),
						Message = r.Str(),
						LastMonth = r.Int()
					});
				}
				List<int> deciding = r.IntList();
				for (int num7 = 0; num7 + 1 < deciding.Count; num7 += 2)
				{
					if (world.Players.TryGetValue(deciding[num7], out var p2))
					{
						p2.DecideMonth = deciding[num7 + 1];
					}
				}
				List<int> blocks = r.IntList();
				int n2;
				for (int num8 = 0; num8 + 1 < blocks.Count; num8 += 2 + n2)
				{
					world.Teams.TryGetValue(blocks[num8], out var t2);
					n2 = blocks[num8 + 1];
					if (n2 < 0 || num8 + 2 + n2 > blocks.Count)
					{
						throw new FormatException("некорректный список обмена");
					}
					t2?.Block.AddRange(blocks.GetRange(num8 + 2, n2));
				}
				int offers = Count(r, 100000, "offers");
				for (int num9 = 0; num9 < offers; num9++)
				{
					world.Offers.Add(new TradeOffer
					{
						Id = r.Int(),
						FromTeamId = r.Int(),
						ToTeamId = r.Int(),
						Give = r.IntList(),
						GivePicks = r.IntList(),
						Get = r.IntList(),
						Month = r.Int()
					});
				}
			}
			if (version >= 10)
			{
				List<int> prestige = r.IntList();
				for (int num10 = 0; num10 + 2 < prestige.Count; num10 += 3)
				{
					if (world.Teams.TryGetValue(prestige[num10], out var t3))
					{
						t3.Prestige = prestige[num10 + 1];
						t3.RecruitHours = prestige[num10 + 2];
					}
				}
				int board2 = Count(r, 1000000, "recruits");
				for (int num11 = 0; num11 < board2; num11++)
				{
					world.RecruitBoard.Add(new RecruitTarget
					{
						PlayerId = r.Int(),
						Sport = (Sport)r.Int(),
						Year = r.Int(),
						Stars = r.Int(),
						CommittedTo = r.Int(),
						National = r.Bool()
					});
				}
				int efforts = Count(r, 1000000, "действия по набору игроков");
				for (int num12 = 0; num12 < efforts; num12++)
				{
					world.RecruitEfforts.Add(new RecruitEffort
					{
						PlayerId = r.Int(),
						TeamId = r.Int(),
						Effort = r.Int(),
						Offered = r.Bool(),
						Calls = r.Int(),
						Watched = r.Bool(),
						HomeVisit = r.Bool(),
						OfficialVisit = r.Bool(),
						Month = r.Int()
					});
				}
				int money = Count(r, 1000000, "команды с выплатами по расторгнутым контрактам");
				for (int num13 = 0; num13 < money; num13++)
				{
					world.Teams.TryGetValue(r.Int(), out var t4);
					int buyouts = r.Int();
					int charges = Count(r, 100000, "выплаты по расторгнутым контрактам");
					if (t4 != null)
					{
						t4.Buyouts = Math.Max(0, buyouts);
					}
					for (int num14 = 0; num14 < charges; num14++)
					{
						DeadCharge c3 = new DeadCharge
						{
							PlayerId = r.Int(),
							Name = r.Str(),
							Reason = r.Str(),
							Amounts = r.IntList()
						};
						if (t4 != null && c3.Amounts.Count > 0)
						{
							t4.Dead.Add(c3);
						}
					}
				}
				foreach (string d in r.Strs())
				{
					world.NoteDistrict(d);
				}
			}
			if (version >= 11)
			{
				int retired2 = Count(r, 1000000, "завершившие карьеру игроки");
				for (int num15 = 0; num15 < retired2; num15++)
				{
					Player p3 = ReadPlayer(r, version);
					p3.RetiredYear = r.Int();
					p3.Status = PlayerStatus.Retired;
					p3.TeamId = -1;
					world.Retired[p3.Id] = p3;
				}
				int coaches2 = Count(r, 1000000, "данные тренеров");
				for (int num16 = 0; num16 < coaches2; num16++)
				{
					int id = r.Int();
					string home = r.Str();
					int former = r.Int();
					string played = r.Str();
					if (world.Coaches.TryGetValue(id, out var c4))
					{
						c4.Hometown = home;
						c4.FormerPlayerId = (world.Retired.ContainsKey(former) ? former : (-1));
						c4.Played = played;
					}
				}
				Dictionary<int, Match> games = new Dictionary<int, Match>();
				foreach (League l3 in world.Leagues)
				{
					Season[] array = new Season[2] { l3.Current, l3.Previous };
					foreach (Season s in array)
					{
						if (s == null)
						{
							continue;
						}
						foreach (Match g in s.Games)
						{
							games[g.Id] = g;
						}
					}
				}
				int boxes = Count(r, 1000000, "итоговые протоколы матчей");
				for (int num18 = 0; num18 < boxes; num18++)
				{
					int id2 = r.Int();
					GameBox b2 = new GameBox();
					int lines = Count(r, 1000, "строки протокола матча");
					for (int num19 = 0; num19 < lines; num19++)
					{
						b2.Lines.Add(new BoxLine
						{
							PlayerId = r.Int(),
							Home = r.Bool(),
							Stats = r.Ints()
						});
					}
					int events = Count(r, 1000, "события матча");
					for (int num20 = 0; num20 < events; num20++)
					{
						b2.Events.Add(new BoxEvent
						{
							Period = r.Int(),
							Clock = r.Str(),
							Home = r.Bool(),
							Kind = r.Int(),
							Text = r.Str(),
							HomeScore = r.Int(),
							AwayScore = r.Int()
						});
					}
					b2.HomePeriods = r.IntList();
					b2.AwayPeriods = r.IntList();
					b2.Note = r.Str();
					if (games.TryGetValue(id2, out var game) && game.Played)
					{
						game.Box = b2;
					}
				}
				if (version >= 12)
				{
					int withPlayers = Count(r, 1000000, "игроки в протоколе матча");
					for (int num21 = 0; num21 < withPlayers; num21++)
					{
						int id3 = r.Int();
						int events2 = Count(r, 1000, "события матча");
						games.TryGetValue(id3, out var game2);
						for (int num22 = 0; num22 < events2; num22++)
						{
							List<int> players = r.IntList();
							if (game2 != null && game2.Box != null && num22 < game2.Box.Events.Count)
							{
								game2.Box.Events[num22].Players = players;
							}
						}
					}
					int live = Count(r, 10000, "матчи в прямом эфире");
					for (int num23 = 0; num23 < live; num23++)
					{
						int id4 = r.Int();
						LiveGame x = new LiveGame
						{
							Start = r.Int(),
							End = r.Int(),
							HomeScore = r.Int(),
							AwayScore = r.Int(),
							Result = (ResultType)r.Int(),
							HomePenalties = r.Int(),
							AwayPenalties = r.Int(),
							Stars = r.Str(),
							HomeChance = r.Dbl(),
							HomeRating = r.Dbl(),
							AwayRating = r.Dbl(),
							Playoff = r.Bool()
						};
						if (r.Bool())
						{
							x.Box = ReadGameBox(r);
						}
						if (games.TryGetValue(id4, out var game3) && !game3.Played)
						{
							game3.Live = x;
						}
					}
					ReadV12(r, world, version);
				}
				if (version >= 13)
				{
					int users = Count(r, 100000, "этапы матчей в прямом эфире");
					for (int num24 = 0; num24 < users; num24++)
					{
						int id5 = r.Int();
						bool first = r.Bool();
						int last = r.Int();
						if (world.Teams.TryGetValue(id5, out var t5))
						{
							t5.WatchFirst = first;
							t5.LastLiveMonth = last;
						}
					}
				}
				if (version >= 14)
				{
					ReadV14(r, world, version);
				}
			}
			if (version < 10)
			{
				world.FixMixedNames();
			}
			if (version < 12)
			{
				world.RescheduleVenues();
			}
			if (version < 14)
			{
				world.RepickGamesOfTheMonth();
			}
			if (version < 3)
			{
				foreach (League league in world.Leagues)
				{
					league.Current = null;
				}
			}
			Sanitize(world);
			world.Revision++;
			return true;
		}
		catch (Exception ex)
		{
			error = "Не удалось прочитать спортивные данные: " + ex.Message;
			world.Clear();
			return false;
		}
	}

	private static int Count(TokenReader r, int max, string what)
	{
		int n = r.Int();
		if (n < 0 || n > max)
		{
			throw new FormatException("Некорректные данные: " + what);
		}
		return n;
	}

	private static void WriteSeason(TokenWriter w, Season s)
	{
		w.Bool(s != null);
		if (s == null)
		{
			return;
		}
		w.Int(s.Year);
		w.Int(s.StartDay);
		w.Int(s.RegularEndDay);
		w.Int(s.PlayoffEndDay);
		w.Bool(s.PlayoffsCreated);
		w.Bool(s.Complete);
		w.Int(s.ChampionTeamId);
		w.Int(s.Games.Count);
		foreach (Match g in s.Games)
		{
			w.Int(g.Id);
			w.Int(g.Time);
			w.Int(g.HomeTeamId);
			w.Int(g.AwayTeamId);
			w.Int(g.VenueId);
			w.Int((int)g.Stage);
			w.Bool(g.Played);
			w.Int(g.HomeScore);
			w.Int(g.AwayScore);
			w.Int((int)g.Result);
			w.Int(g.HomePenalties);
			w.Int(g.AwayPenalties);
			w.Int(g.SeriesId);
			w.Int(g.GameNumber);
			w.Int(g.PlayoffRound);
			w.Bool(g.Featured);
			w.Str(g.Summary);
		}
		w.Int(s.PlayoffRounds);
		w.IntList(s.PlannedRoundDays);
		w.Int(s.Bracket.Count);
		foreach (PlayoffSeries ps in s.Bracket)
		{
			w.Int(ps.Id);
			w.Int(ps.Round);
			w.Int(ps.Slot);
			w.Int(ps.HighSeed);
			w.Int(ps.LowSeed);
			w.Int(ps.HighTeamId);
			w.Int(ps.LowTeamId);
			w.Int(ps.BestOf);
			w.Bool(ps.Neutral);
			w.Int(ps.WinnerId);
			w.Int(ps.PlannedEndDay);
			w.Int(ps.HighEdge);
			w.Int(ps.LowEdge);
		}
	}

	private static int UpgradeTime(int time, int version)
	{
		if (version >= 3 || time == int.MinValue)
		{
			return time;
		}
		int perDay = ((version == 1) ? 24 : 1440);
		int index = FloorDiv(time, perDay);
		int part = time - index * perDay;
		int minute = ((version == 1) ? (part * 60) : part);
		return GameClock.FromCs2(FloorDiv(index, 12), index - FloorDiv(index, 12) * 12 + 1, minute);
	}

	private static int UpgradeDay(int dayIndex, int version)
	{
		if (version >= 3)
		{
			return dayIndex;
		}
		int year = GameClock.ClampYear(FloorDiv(dayIndex, 12));
		int month = dayIndex - FloorDiv(dayIndex, 12) * 12 + 1;
		return GameClock.DayNumber(new DateTime(year, month, 1));
	}

	private static int FloorDiv(int a, int b)
	{
		int q = a / b;
		if (a % b != 0 && a < 0 != b < 0)
		{
			q--;
		}
		return q;
	}

	private static Season ReadSeason(TokenReader r, int version)
	{
		if (!r.Bool())
		{
			return null;
		}
		Season s = new Season
		{
			Year = r.Int(),
			StartDay = UpgradeDay(r.Int(), version),
			RegularEndDay = UpgradeDay(r.Int(), version),
			PlayoffEndDay = UpgradeDay(r.Int(), version),
			PlayoffsCreated = r.Bool(),
			Complete = r.Bool(),
			ChampionTeamId = r.Int()
		};
		int count = r.Int();
		if (count < 0 || count > 1000000)
		{
			throw new FormatException("некорректное число матчей");
		}
		for (int i = 0; i < count; i++)
		{
			Match g = new Match
			{
				Id = r.Int(),
				Time = UpgradeTime(r.Int(), version),
				HomeTeamId = r.Int(),
				AwayTeamId = r.Int(),
				VenueId = r.Int(),
				Stage = (GameStage)r.Int(),
				Played = r.Bool(),
				HomeScore = r.Int(),
				AwayScore = r.Int(),
				Result = (ResultType)r.Int(),
				HomePenalties = r.Int(),
				AwayPenalties = r.Int()
			};
			if (version >= 3)
			{
				g.SeriesId = r.Int();
				g.GameNumber = r.Int();
				g.PlayoffRound = r.Int();
			}
			if (version >= 4)
			{
				g.Featured = r.Bool();
				g.Summary = r.Str();
			}
			s.Games.Add(g);
		}
		if (version >= 3)
		{
			s.PlayoffRounds = r.Int();
			s.PlannedRoundDays = r.IntList();
			int series = r.Int();
			if (series < 0 || series > 10000)
			{
				throw new FormatException("неверный размер турнирной сетки");
			}
			for (int j = 0; j < series; j++)
			{
				PlayoffSeries ps = new PlayoffSeries
				{
					Id = r.Int(),
					Round = r.Int(),
					Slot = r.Int(),
					HighSeed = r.Int(),
					LowSeed = r.Int(),
					HighTeamId = r.Int(),
					LowTeamId = r.Int(),
					BestOf = r.Int(),
					Neutral = r.Bool(),
					WinnerId = r.Int(),
					PlannedEndDay = r.Int()
				};
				if (version >= 5)
				{
					ps.HighEdge = r.Int();
					ps.LowEdge = r.Int();
				}
				s.Bracket.Add(ps);
			}
		}
		return s;
	}

	private static void Sanitize(SportsWorld world)
	{
		int maxId = 0;
		foreach (int id in world.Orgs.Keys)
		{
			maxId = Math.Max(maxId, id);
		}
		foreach (int id2 in world.Teams.Keys)
		{
			maxId = Math.Max(maxId, id2);
		}
		foreach (int id3 in world.Venues.Keys)
		{
			maxId = Math.Max(maxId, id3);
		}
		foreach (League l in world.Leagues)
		{
			Season[] array = new Season[2] { l.Current, l.Previous };
			foreach (Season s in array)
			{
				if (s == null)
				{
					continue;
				}
				foreach (Match g in s.Games)
				{
					maxId = Math.Max(maxId, Math.Max(g.Id, g.SeriesId));
				}
				foreach (PlayoffSeries ps in s.Bracket)
				{
					maxId = Math.Max(maxId, ps.Id);
				}
			}
		}
		foreach (int id4 in world.Players.Keys)
		{
			maxId = Math.Max(maxId, id4);
		}
		foreach (int id5 in world.Coaches.Keys)
		{
			maxId = Math.Max(maxId, id5);
		}
		foreach (League league in world.Leagues)
		{
			foreach (PickAsset a in league.Picks)
			{
				maxId = Math.Max(maxId, a.Id);
			}
		}
		if (world.NextId <= maxId)
		{
			world.NextId = maxId + 1;
		}
		foreach (Team t in world.Teams.Values)
		{
			if (!Enum.IsDefined(typeof(Sport), t.Sport))
			{
				t.Sport = Sport.Basketball;
			}
			if (!Enum.IsDefined(typeof(Level), t.Level))
			{
				t.Level = Level.Pro;
			}
			t.Rating = Math.Max(25, Math.Min(95, t.Rating));
			if (t.IsUser && !t.Retired && !world.Venues.ContainsKey(t.VenueId))
			{
				t.Retired = true;
			}
		}
		foreach (League league2 in world.Leagues)
		{
			league2.TeamIds.RemoveAll((int key) => !world.Teams.ContainsKey(key));
			league2.PendingTeamIds.RemoveAll((int key) => !world.Teams.ContainsKey(key));
		}
		foreach (Player p in world.Players.Values)
		{
			if (!Enum.IsDefined(typeof(Position), p.Position))
			{
				p.Position = Position.Midfielder;
			}
			if (!Enum.IsDefined(typeof(Sport), p.Sport))
			{
				p.Sport = Sport.Soccer;
			}
			if (!Enum.IsDefined(typeof(Level), p.Level))
			{
				p.Level = Level.Pro;
			}
			if (!Enum.IsDefined(typeof(PlayerStatus), p.Status) || p.Status == PlayerStatus.Retired)
			{
				p.Status = PlayerStatus.FreeAgent;
			}
			if (p.From == null)
			{
				p.From = "";
			}
			if (p.Playoff == null)
			{
				p.Playoff = new int[0];
			}
			p.Salary = Math.Max(0, p.Salary);
			if (!Enum.IsDefined(typeof(PlayerPriority), p.Priority))
			{
				p.Priority = PlayerPriority.Balanced;
			}
		}
		List<int> badCoaches = new List<int>();
		foreach (Coach c in world.Coaches.Values)
		{
			if (!Enum.IsDefined(typeof(Sport), c.Sport) || !Enum.IsDefined(typeof(Level), c.Level))
			{
				badCoaches.Add(c.Id);
				continue;
			}
			if (!Enum.IsDefined(typeof(CoachRole), c.Role))
			{
				c.Role = CoachRole.Head;
			}
			if (c.Role != CoachRole.Head && c.Sport != Sport.Football)
			{
				c.Role = CoachRole.Head;
			}
			if (c.Potential < c.Overall)
			{
				c.Potential = c.Overall;
			}
			if (c.History == null)
			{
				c.History = "";
			}
			Team ct = ((c.TeamId >= 0) ? world.GetTeam(c.TeamId) : null);
			if (c.TeamId >= 0 && (ct == null || ct.Sport != c.Sport))
			{
				c.TeamId = -1;
			}
		}
		foreach (int id6 in badCoaches)
		{
			world.Coaches.Remove(id6);
		}
		HashSet<string> jobs = new HashSet<string>();
		foreach (Coach c2 in world.Coaches.Values)
		{
			if (c2.TeamId >= 0)
			{
				string text = c2.TeamId.ToString();
				int i = (int)c2.Role;
				if (!jobs.Add(text + ":" + i))
				{
					c2.TeamId = -1;
				}
			}
		}
		foreach (Team t2 in world.Teams.Values)
		{
			t2.Form = Math.Max(-1000, Math.Min(1000, t2.Form));
			t2.RunBonus = Math.Max(0, Math.Min(100, t2.RunBonus));
			t2.Cinderella = Math.Max(0, Math.Min(30, t2.Cinderella));
			if (!t2.IsUser)
			{
				t2.Manual = false;
			}
		}
		world.Feed.Sort((NewsPost newsPost, NewsPost b) => b.Id.CompareTo(newsPost.Id));
		if (world.Feed.Count > 400)
		{
			world.Feed.RemoveRange(400, world.Feed.Count - 400);
		}
		world.AfterLoad();
	}

	private static void WriteV14More(TokenWriter w, SportsWorld world)
	{
		List<Team> folded = new List<Team>();
		foreach (Team t in world.Teams.Values)
		{
			if (t.FoldedYear > 0)
			{
				folded.Add(t);
			}
		}
		w.Int(folded.Count);
		foreach (Team t2 in folded)
		{
			w.Int(t2.Id);
			w.Int(t2.FoldedYear);
		}
		WriteV14Rest(w, world);
	}

	private static void ReadV14More(TokenReader r, SportsWorld world, int version)
	{
		int folded = Count(r, 1000000, "объединённые команды");
		for (int k = 0; k < folded; k++)
		{
			int id = r.Int();
			int year = r.Int();
			if (world.Teams.TryGetValue(id, out var t))
			{
				t.FoldedYear = year;
			}
		}
		ReadV14Rest(r, world, version);
	}

	private static void WriteV14Rest(TokenWriter w, SportsWorld world)
	{
		w.Int(world.Leagues.Count);
		foreach (League l in world.Leagues)
		{
			w.Str(l.Key);
			w.Int(l.AllStarYear);
			w.Int(l.AllLeagueYear);
			w.Int(l.AllStars.Count);
			foreach (AllStarPick k in l.AllStars)
			{
				w.Int(k.Year);
				w.Str(k.Season);
				w.Int(k.Kind);
				w.Int(k.PlayerId);
				w.Str(k.Name);
				w.Int(k.TeamId);
				w.Str(k.Team);
				w.Str(k.Pos);
				w.Bool(k.Starter);
				w.Str(k.Detail);
			}
		}
		WriteV14Farms(w, world);
	}

	private static void ReadV14Rest(TokenReader r, SportsWorld world, int version)
	{
		int leagues = Count(r, 1000, "лиги Матча всех звёзд");
		for (int i = 0; i < leagues; i++)
		{
			string key = r.Str();
			int star = r.Int();
			int all = r.Int();
			int n = Count(r, 100000, "выборы на Матч всех звёзд");
			List<AllStarPick> picks = new List<AllStarPick>();
			for (int k = 0; k < n; k++)
			{
				picks.Add(new AllStarPick
				{
					Year = r.Int(),
					Season = r.Str(),
					Kind = r.Int(),
					PlayerId = r.Int(),
					Name = r.Str(),
					TeamId = r.Int(),
					Team = r.Str(),
					Pos = r.Str(),
					Starter = r.Bool(),
					Detail = r.Str()
				});
			}
			League l = world.Leagues.Find((League x) => x.Key == key);
			if (l != null)
			{
				l.AllStarYear = star;
				l.AllLeagueYear = all;
				l.AllStars = picks;
			}
		}
		ReadV14Farms(r, world, version);
	}

	private static void WriteV14Farms(TokenWriter w, SportsWorld world)
	{
		List<Team> prefs = new List<Team>();
		foreach (Team t in world.Teams.Values)
		{
			if (t.FarmPref != 0)
			{
				prefs.Add(t);
			}
		}
		w.Int(prefs.Count);
		foreach (Team t2 in prefs)
		{
			w.Int(t2.Id);
			w.Int(t2.FarmPref);
		}
		WriteV14Soccer(w, world);
	}

	private static void ReadV14Farms(TokenReader r, SportsWorld world, int version)
	{
		int n = Count(r, 100000, "выборы фарм-команд");
		for (int k = 0; k < n; k++)
		{
			int id = r.Int();
			int pref = r.Int();
			if (world.Teams.TryGetValue(id, out var t))
			{
				t.FarmPref = pref;
			}
		}
		ReadV14Soccer(r, world, version);
	}

	private static void WriteV14Soccer(TokenWriter w, SportsWorld world)
	{
		List<Team> budgets = new List<Team>();
		foreach (Team t in world.Teams.Values)
		{
			if (t.TransferBudget >= 0)
			{
				budgets.Add(t);
			}
		}
		w.Int(budgets.Count);
		foreach (Team t2 in budgets)
		{
			w.Int(t2.Id);
			w.Int(t2.TransferBudget);
		}
		w.Int(world.TransferOffers.Count);
		foreach (TransferOffer o in world.TransferOffers)
		{
			w.Int(o.Id);
			w.Int(o.PlayerId);
			w.Int(o.SellerTeamId);
			w.Int(o.BuyerTeamId);
			w.Str(o.BuyerName);
			w.Int(o.Fee);
			w.Int(o.Month);
		}
		w.Int(world.Leagues.Count);
		foreach (League l in world.Leagues)
		{
			w.Str(l.Key);
			w.Strs(l.Transfers);
		}
		WriteV15(w, world);
	}

	private static void WriteV15(TokenWriter w, SportsWorld world)
	{
		List<KeyValuePair<int, GameBox>> boxes = new List<KeyValuePair<int, GameBox>>();
		List<int> kinds = new List<int>();
		foreach (League l in world.Leagues)
		{
			Season[] array = new Season[2] { l.Current, l.Previous };
			foreach (Season s in array)
			{
				if (s == null)
				{
					continue;
				}
				foreach (Match g in s.Games)
				{
					if (g.Box != null && g.Box.Flow.Count > 0)
					{
						boxes.Add(new KeyValuePair<int, GameBox>(g.Id, g.Box));
						kinds.Add(0);
					}
					if (g.Live != null && g.Live.Box != null && g.Live.Box.Flow.Count > 0)
					{
						boxes.Add(new KeyValuePair<int, GameBox>(g.Id, g.Live.Box));
						kinds.Add(1);
					}
				}
			}
		}
		w.Int(boxes.Count);
		for (int j = 0; j < boxes.Count; j++)
		{
			w.Int(boxes[j].Key);
			w.Int(kinds[j]);
			w.IntList(boxes[j].Value.Flow);
		}
		WriteV16(w, world);
	}

	private static void WriteV16(TokenWriter w, SportsWorld world)
	{
		List<League> leagues = world.Leagues.FindAll((League league) => league.PromotionDoneFor != int.MinValue || league.LastUp.Count > 0 || league.LastDown.Count > 0);
		w.Int(leagues.Count);
		foreach (League l in leagues)
		{
			w.Str(l.Key);
			w.Int(l.PromotionDoneFor);
			w.IntList(l.LastUp);
			w.IntList(l.LastDown);
			w.Int(l.LastUpByPlayoff);
		}
		WriteV17(w, world);
	}

	private static void WriteV17(TokenWriter w, SportsWorld world)
	{
		List<Match> games = new List<Match>();
		foreach (League l in world.Leagues)
		{
			Season[] array = new Season[2] { l.Current, l.Previous };
			foreach (Season s in array)
			{
				if (s == null)
				{
					continue;
				}
				foreach (Match g in s.Games)
				{
					if (g.Weather >= 0 || g.RainDelay > 0 || g.PostponedFrom >= 0)
					{
						games.Add(g);
					}
				}
			}
		}
		w.Int(games.Count);
		foreach (Match g2 in games)
		{
			w.Int(g2.Id);
			w.Int(g2.Weather);
			w.Int(g2.RainDelay);
			w.Int(g2.PostponedFrom);
		}
		w.Int(world.CitySampleTime);
		w.Dbl(world.CityTempC);
		w.Dbl(world.CityPrecip);
		w.Dbl(world.CityCloud);
		w.Bool(world.CityRaining);
		w.Bool(world.CitySnowing);
		w.Int(world.CityWeather.Count);
		foreach (CityWeatherMonth m in world.CityWeather)
		{
			w.Int(m.Month);
			w.Int(m.Samples);
			w.Dbl(m.SumTempC);
			w.Int(m.Wet);
			w.Int(m.Snow);
			w.Int(m.Heavy);
		}
		WriteV18(w, world);
	}

	private static void WriteV18(TokenWriter w, SportsWorld world)
	{
		List<int> pairs = new List<int>();
		foreach (League l in world.Leagues)
		{
			Season[] array = new Season[2] { l.Current, l.Previous };
			foreach (Season s in array)
			{
				if (s == null)
				{
					continue;
				}
				foreach (Match g in s.Games)
				{
					if (g.Attendance > 0)
					{
						pairs.Add(g.Id);
						pairs.Add(g.Attendance);
					}
				}
			}
		}
		w.IntList(pairs);
		WriteV19(w, world);
	}

	private static void WriteV19(TokenWriter w, SportsWorld world)
	{
		w.Int(world.Tournaments.Count);
		foreach (Tournament t in world.Tournaments)
		{
			w.Int(t.Id);
			w.Int((int)t.Sport);
			w.Str(t.Name);
			w.Int(t.Year);
			w.Int(t.StartDay);
			w.Bool(t.Under20);
			w.Str(t.Host);
			w.Bool(t.Complete);
			w.Str(t.Gold);
			w.Str(t.Silver);
			w.Str(t.Bronze);
			w.Int(t.Teams.Count);
			foreach (NationTeam n in t.Teams)
			{
				w.Str(n.Nation);
				w.Int(n.Rating);
				w.Int(n.Group);
				w.Bool(n.Qualifier);
				w.IntList(n.PlayerIds);
				w.Int(n.Unnamed);
			}
			w.Int(t.Games.Count);
			foreach (TourGame g in t.Games)
			{
				w.Int(g.Day);
				w.Int(g.Minute);
				w.Str(g.Stage);
				w.Int(g.Home);
				w.Int(g.Away);
				w.Int(g.HomeScore);
				w.Int(g.AwayScore);
				w.Bool(g.Played);
				w.Str(g.Suffix);
				w.Int(g.TieWinner);
			}
		}
		WriteV20(w, world);
	}

	private static void WriteV20(TokenWriter w, SportsWorld world)
	{
		w.Str(world.CountryName);
		w.Str(world.CountryAbbrev);
		WriteV21(w, world);
	}

	private static void WriteV21(TokenWriter w, SportsWorld world)
	{
		w.Int(world.Teams.Count);
		foreach (Team t in world.Teams.Values)
		{
			world.EnsureTicketPrices(t);
			world.EnsureClubBalance(t);
			w.Int(t.Id);
			w.Int(Math.Max(0, t.ClubBalance));
			w.Int(Math.Max(1, Math.Min(1500, t.TicketPriceResidents)));
			w.Int(Math.Max(1, Math.Min(1500, t.TicketPriceVisitors)));
		}
		List<Match> matches = new List<Match>();
		foreach (League l in world.Leagues)
		{
			Season[] seasons = new Season[2] { l.Current, l.Previous };
			foreach (Season s in seasons)
			{
				if (s == null) continue;
				foreach (Match g in s.Games)
				{
					if (g.TicketRevenue > 0 || g.TicketIncomePaid || g.TicketLocalAttendance > 0 || g.TicketVisitorAttendance > 0)
						matches.Add(g);
				}
			}
		}
		w.Int(matches.Count);
		foreach (Match g in matches)
		{
			w.Int(g.Id);
			w.Int(g.TicketLocalAttendance);
			w.Int(g.TicketVisitorAttendance);
			w.Int(g.TicketRevenue);
			w.Bool(g.TicketIncomePaid);
		}
	}

	private static void ReadV20(TokenReader r, SportsWorld world, int version)
	{
		world.CountryName = r.Str();
		world.CountryAbbrev = r.Str();
		if (version >= 21)
			ReadV21(r, world);
	}

	private static void ReadV21(TokenReader r, SportsWorld world)
	{
		int teams = Count(r, 100000, "клубные бюджеты");
		for (int i = 0; i < teams; i++)
		{
			int id = r.Int();
			int balance = r.Int();
			int residents = r.Int();
			int visitors = r.Int();
			if (world.Teams.TryGetValue(id, out var t))
			{
				t.ClubBalance = Math.Max(0, balance);
				t.TicketPriceResidents = Math.Max(1, Math.Min(1500, residents));
				t.TicketPriceVisitors = Math.Max(1, Math.Min(1500, visitors));
			}
		}
		Dictionary<int, Match> byId = new Dictionary<int, Match>();
		foreach (League l in world.Leagues)
		{
			Season[] seasons = new Season[2] { l.Current, l.Previous };
			foreach (Season s in seasons)
			{
				if (s == null) continue;
				foreach (Match g in s.Games) byId[g.Id] = g;
			}
		}
		int matches = Count(r, 1000000, "доходы от билетов");
		for (int i = 0; i < matches; i++)
		{
			int id = r.Int();
			int local = r.Int();
			int visitors = r.Int();
			int revenue = r.Int();
			bool paid = r.Bool();
			if (byId.TryGetValue(id, out var g))
			{
				g.TicketLocalAttendance = Math.Max(0, local);
				g.TicketVisitorAttendance = Math.Max(0, visitors);
				g.TicketRevenue = Math.Max(0, revenue);
				g.TicketIncomePaid = paid;
			}
		}
	}

	private static void ReadV19(TokenReader r, SportsWorld world, int version)
	{
		world.Tournaments.Clear();
		int n = Count(r, 100, "tournaments");
		for (int i = 0; i < n; i++)
		{
			Tournament t = new Tournament
			{
				Id = r.Int(),
				Sport = (Sport)r.Int(),
				Name = r.Str(),
				Year = r.Int(),
				StartDay = r.Int(),
				Under20 = r.Bool(),
				Host = r.Str(),
				Complete = r.Bool(),
				Gold = r.Str(),
				Silver = r.Str(),
				Bronze = r.Str()
			};
			int teams = Count(r, 200, "nations");
			for (int k = 0; k < teams; k++)
			{
				t.Teams.Add(new NationTeam
				{
					Nation = r.Str(),
					Rating = r.Int(),
					Group = r.Int(),
					Qualifier = r.Bool(),
					PlayerIds = r.IntList(),
					Unnamed = r.Int()
				});
			}
			int games = Count(r, 1000, "игры турнира");
			for (int j = 0; j < games; j++)
			{
				TourGame g = new TourGame
				{
					Day = r.Int(),
					Minute = r.Int(),
					Stage = r.Str(),
					Home = r.Int(),
					Away = r.Int(),
					HomeScore = r.Int(),
					AwayScore = r.Int(),
					Played = r.Bool(),
					Suffix = r.Str(),
					TieWinner = r.Int()
				};
				if (g.Home >= 0 && g.Home < t.Teams.Count && g.Away >= 0 && g.Away < t.Teams.Count)
				{
					t.Games.Add(g);
				}
			}
			world.Tournaments.Add(t);
		}
		if (version >= 20)
		{
			ReadV20(r, world, version);
		}
	}

	private static void ReadV18(TokenReader r, SportsWorld world, int version)
	{
		List<int> pairs = r.IntList();
		Dictionary<int, Match> byId = new Dictionary<int, Match>();
		foreach (League l in world.Leagues)
		{
			Season[] array = new Season[2] { l.Current, l.Previous };
			foreach (Season s in array)
			{
				if (s == null)
				{
					continue;
				}
				foreach (Match g in s.Games)
				{
					byId[g.Id] = g;
				}
			}
		}
		for (int j = 0; j + 1 < pairs.Count; j += 2)
		{
			if (byId.TryGetValue(pairs[j], out var g2))
			{
				g2.Attendance = Math.Max(0, pairs[j + 1]);
			}
		}
		if (version >= 19)
		{
			ReadV19(r, world, version);
		}
	}

	private static void ReadV17(TokenReader r, SportsWorld world, int version)
	{
		Dictionary<int, Match> byId = new Dictionary<int, Match>();
		foreach (League l in world.Leagues)
		{
			Season[] array = new Season[2] { l.Current, l.Previous };
			foreach (Season s in array)
			{
				if (s == null)
				{
					continue;
				}
				foreach (Match g in s.Games)
				{
					byId[g.Id] = g;
				}
			}
		}
		int n = Count(r, 1000000, "погода во время матча");
		for (int j = 0; j < n; j++)
		{
			int id = r.Int();
			int weather = r.Int();
			int delay = r.Int();
			int from = r.Int();
			if (byId.TryGetValue(id, out var g2))
			{
				g2.Weather = weather;
				g2.RainDelay = Math.Max(0, delay);
				g2.PostponedFrom = from;
			}
		}
		world.CitySampleTime = r.Int();
		world.CityTempC = r.Dbl();
		world.CityPrecip = r.Dbl();
		world.CityCloud = r.Dbl();
		world.CityRaining = r.Bool();
		world.CitySnowing = r.Bool();
		int months = Count(r, 100, "месяцы погоды города");
		world.CityWeather.Clear();
		for (int k = 0; k < months; k++)
		{
			world.CityWeather.Add(new CityWeatherMonth
			{
				Month = r.Int(),
				Samples = r.Int(),
				SumTempC = r.Dbl(),
				Wet = r.Int(),
				Snow = r.Int(),
				Heavy = r.Int()
			});
		}
		if (version >= 18)
		{
			ReadV18(r, world, version);
		}
	}

	private static void ReadV16(TokenReader r, SportsWorld world, int version)
	{
		int n = Count(r, 1000, "записи о повышении в лиге");
		for (int i = 0; i < n; i++)
		{
			string key = r.Str();
			int done = r.Int();
			List<int> up = r.IntList();
			List<int> down = r.IntList();
			int byPlayoff = r.Int();
			League l = world.Leagues.Find((League x) => x.Key == key);
			if (l != null)
			{
				l.PromotionDoneFor = done;
				l.LastUp = up;
				l.LastDown = down;
				l.LastUpByPlayoff = byPlayoff;
			}
		}
		if (version >= 17)
		{
			ReadV17(r, world, version);
		}
	}

	private static void ReadV15(TokenReader r, SportsWorld world, int version)
	{
		Dictionary<int, Match> games = new Dictionary<int, Match>();
		foreach (League l in world.Leagues)
		{
			Season[] array = new Season[2] { l.Current, l.Previous };
			foreach (Season s in array)
			{
				if (s == null)
				{
					continue;
				}
				foreach (Match g in s.Games)
				{
					games[g.Id] = g;
				}
			}
		}
		int n = Count(r, 1000000, "сценарии матчей");
		for (int k = 0; k < n; k++)
		{
			int id = r.Int();
			int kind = r.Int();
			List<int> flow = r.IntList();
			if (games.TryGetValue(id, out var g2))
			{
				GameBox b;
				if (kind != 1)
				{
					b = g2.Box;
				}
				else
				{
					b = ((g2.Live != null) ? g2.Live.Box : null);
				}
				if (b != null)
				{
					b.Flow = flow;
				}
			}
		}
		if (version >= 16)
		{
			ReadV16(r, world, version);
		}
	}

	private static void ReadV14Soccer(TokenReader r, SportsWorld world, int version)
	{
		int budgets = Count(r, 1000000, "трансферные бюджеты");
		for (int k = 0; k < budgets; k++)
		{
			int id = r.Int();
			int money = r.Int();
			if (world.Teams.TryGetValue(id, out var t))
			{
				t.TransferBudget = Math.Max(0, money);
			}
		}
		int offers = Count(r, 100000, "трансферные предложения");
		world.TransferOffers.Clear();
		for (int i = 0; i < offers; i++)
		{
			TransferOffer o = new TransferOffer
			{
				Id = r.Int(),
				PlayerId = r.Int(),
				SellerTeamId = r.Int(),
				BuyerTeamId = r.Int(),
				BuyerName = r.Str(),
				Fee = r.Int(),
				Month = r.Int()
			};
			if (world.GetPlayer(o.PlayerId) != null && world.GetTeam(o.SellerTeamId) != null)
			{
				world.TransferOffers.Add(o);
			}
		}
		int leagues = Count(r, 1000, "трансферные лиги");
		for (int j = 0; j < leagues; j++)
		{
			string key = r.Str();
			List<string> lines = r.Strs();
			League l = world.Leagues.Find((League x) => x.Key == key);
			if (l != null)
			{
				l.Transfers = lines;
			}
		}
		if (version >= 15)
		{
			ReadV15(r, world, version);
		}
	}
}
