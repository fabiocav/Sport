using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;

namespace Sport.Shared
{
	public class DataManager
	{
		public DataManager()
		{
			Leagues = new ConcurrentDictionary<string, League>();
			Athletes = new ConcurrentDictionary<string, Athlete>();
			Memberships = new ConcurrentDictionary<string, Membership>();
			Challenges = new ConcurrentDictionary<string, Challenge>();
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

		public ConcurrentDictionary<string, Challenge> Challenges
		{
			get;
			set;
		}
	}
}