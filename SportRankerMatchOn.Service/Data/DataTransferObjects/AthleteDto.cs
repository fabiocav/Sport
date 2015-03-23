using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SportRankerMatchOn
{
	public partial class AthleteDto : AthleteBase
	{
		public List<string> MembershipIds
		{
			get;
			set;
		}
	}
}