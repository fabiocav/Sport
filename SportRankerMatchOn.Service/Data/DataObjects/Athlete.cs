using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SportRankerMatchOn
{
	public partial class Athlete : AthleteBase
	{
		public Athlete()
		{
			Memberships = new HashSet<Membership>();
		}

		public ICollection<Membership> Memberships
		{
			get;
			set;
		}
	}
}