using Microsoft.WindowsAzure.Mobile.Service;
using System.Collections.Generic;

namespace SportRankerMatchOn.Shared
{
	public partial class League : EntityData
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

		public List<string> MemberIds
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