using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SportRankerMatchOn
{
	public partial class LeagueBase : EntityData
	{
		public string Name
		{
			get;
			set;
		}

		public int Season
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
	}
}