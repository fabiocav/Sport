using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SportRankerMatchOn
{
	public partial class MembershipDto : MembershipBase
	{
		public DateTimeOffset? JoinDate
		{
			get;
			set;
		}

		public bool IsAdmin
		{
			get;
			set;
		}
	}
}