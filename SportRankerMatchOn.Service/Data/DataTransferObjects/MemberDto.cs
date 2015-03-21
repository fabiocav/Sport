using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SportRankerMatchOn
{
	public partial class MemberDto : MemberBase
	{
		[JsonProperty("join_date")]
		public DateTimeOffset? JoinDate
		{
			get;
			set;
		}
	}
}