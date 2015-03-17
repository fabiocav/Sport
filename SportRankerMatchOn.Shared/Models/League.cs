using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SportRankerMatchOn.Shared
{
	public partial class League
	{
		public League()
		{
			//Users = new List<Member>();
			//Matches = new List<Match>();
		}

		public string Name
		{
			get;
			set;
		}

		//[JsonIgnore]
		//public IList<Member> Users
		//{
		//	get;
		//	set;
		//}

		//[JsonIgnore]
		//public IList<Match> Matches
		//{
		//	get;
		//	set;
		//}

		public int Season
		{
			get;
			set;
		}
	}

	public class Match
	{
		
	}
}

