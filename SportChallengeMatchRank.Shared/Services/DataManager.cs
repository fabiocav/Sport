using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;

namespace SportChallengeMatchRank.Shared
{
	public class DataManager
	{
		public DataManager()
		{
			Leagues = new ConcurrentDictionary<string, League>();
			Athletes = new ConcurrentDictionary<string, Athlete>();
			Memberships = new ConcurrentDictionary<string, Membership>();
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

		public ConcurrentDictionary<string, League> Leagues
		{
			get;
			set;
		}

		public ConcurrentDictionary<string, Athlete> Athletes
		{
			get;
			set;
		}

		public ConcurrentDictionary<string, Membership> Memberships
		{
			get;
			set;
		}
	}
}