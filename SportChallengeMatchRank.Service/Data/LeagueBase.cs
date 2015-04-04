using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SportChallengeMatchRank
{
	public partial class LeagueBase : EntityData
	{
		public string Name
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public int Season
		{
			get;
			set;
		}

		public int MaxChallengeRange
		{
			get;
			set;
		}

		public int MatchGameCount
		{
			get;
			set;
		}

		public string Sport
		{
			get;
			set;
		}

		public bool IsAcceptingMembers
		{
			get;
			set;
		}

		public bool IsEnabled
		{
			get;
			set;
		}

		public DateTimeOffset? StartDate
		{
			get;
			set;
		}

		public DateTimeOffset? EndDate
		{
			get;
			set;
		}

		public bool HasStarted
		{
			get;
			set;
		}

		public string ImageUrl
		{
			get;
			set;
		}

		public string CreatedByAthleteId
		{
			get;
			set;
		}
	}
}