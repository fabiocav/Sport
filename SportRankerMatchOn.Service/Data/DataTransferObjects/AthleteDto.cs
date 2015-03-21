using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SportRankerMatchOn
{
	public partial class AthleteDto : AthleteBase
	{
		[JsonProperty("league_association_ids")]
		public List<string> LeagueAssociationIds
		{
			get;
			set;
		}
	}
}