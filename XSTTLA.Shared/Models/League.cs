using System;
using System.Collections.Generic;

namespace XSTTLA.Shared
{
	public class League
	{
		public League()
		{
			Users = new List<Member>();
			Matches = new List<Match>();
		}

		public string Name
		{
			get;
			set;
		}

		public IList<Member> Users
		{
			get;
			set;
		}

		public IList<Match> Matches
		{
			get;
			set;
		}

		public int Season
		{
			get;
			set;
		}

		public Member Admin
		{
			get;
			set;
		}
	}

	public class Match
	{
		
	}
}

