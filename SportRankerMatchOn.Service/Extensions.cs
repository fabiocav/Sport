using Newtonsoft.Json;
using SportRankerMatchOn.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportRankerMatchOn.Service
{
	public static class Extensions
	{
		public static void LoadMemberIds(this League league)
		{
			if(league != null)
			{
				if(!string.IsNullOrWhiteSpace(league.MemberIdsString))
				{
					league.MemberIds = JsonConvert.DeserializeObject<List<string>>(league.MemberIdsString);
				}
			}
		}
	}
}