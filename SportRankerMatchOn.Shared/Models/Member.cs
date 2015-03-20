using System;
using System.Collections.Generic;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SportRankerMatchOn.Shared
{
	public class Member : BaseModel
	{
		[JsonProperty("first-name")]
		public string FirstName
		{
			get;
			set;
		}

		[JsonProperty("last-name")]
		public string LastName
		{
			get;
			set;
		}

		[JsonProperty("authentication-id")]
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