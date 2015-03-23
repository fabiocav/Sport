using System;
using System.Collections.Generic;
using System.Linq;

namespace SportRankerMatchOn.Shared
{
	public class DataManager
	{
		public DataManager()
		{
			Leagues = new Dictionary<string, League>();
			Athletes = new Dictionary<string, Athlete>();
			Memberships = new Dictionary<string, Membership>();
		}

		static DataManager _instance;

		public static DataManager Instance
		{
			get
			{
				if(_instance == null)
					_instance = new DataManager();

				return _instance;
			}			
		}

		public Dictionary<string, League> Leagues
		{
			get;
			set;
		}

		public Dictionary<string, Athlete> Athletes
		{
			get;
			set;
		}

		public Dictionary<string, Membership> Memberships
		{
			get;
			set;
		}

		public void EnsureItemsLinked()
		{
			foreach(var l in Leagues.Values)
			{
				l.Memberships.Clear();
				foreach(var mId in l.MembershipIds)
				{
					Membership member = null;
					Memberships.TryGetValue(mId, out member);

					if(member == null)
						continue;
					
					l.Memberships.Add(member);
				}
			}

			foreach(var a in Athletes.Values)
			{
				a.Memberships.Clear();
				foreach(var mId in a.MembershipIds)
				{
					Membership member = null;
					Memberships.TryGetValue(mId, out member);

					if(member == null)
						continue;

					a.Memberships.Add(member);
				}
			}

			foreach(var m in Memberships.Values)
			{
				League l;
				Athlete a;
				Leagues.TryGetValue(m.LeagueId, out l);
				Athletes.TryGetValue(m.AthleteId, out a);

				m.Athlete = a;
				m.League = l;
			}
		}
	}
}