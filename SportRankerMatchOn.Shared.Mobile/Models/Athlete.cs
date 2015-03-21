using System;
using Newtonsoft.Json;

namespace SportRankerMatchOn.Shared
{
	public class Athlete : BaseModel
	{
		[JsonProperty("first_name")]
		public string FirstName
		{
			get;
			set;
		}

		[JsonProperty("last_name")]
		public string LastName
		{
			get;
			set;
		}

		[JsonProperty("authentication_id")]
		public string AuthenticationId
		{
			get;
			set;
		}

		[JsonProperty("email")]
		public string Email
		{
			get;
			set;
		}
	}
}

