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
	}
}