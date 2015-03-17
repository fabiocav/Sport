using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace XSTTLA.Shared
{
	public class League : BaseModel
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

		[JsonIgnore]
		public IList<Member> Users
		{
			get;
			set;
		}

		[JsonIgnore]
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

		[JsonIgnore]
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

