using System;
using Newtonsoft.Json;

namespace SportRankerMatchOn.Shared
{
	public class Athlete : BaseModel
	{
		public Athlete()
		{
		}

		public Athlete(UserProfile profile)
		{
			Name = profile.Name;
			AuthenticationId = profile.UserId;
			Email = profile.Email;
		}

		[JsonProperty("name")]
		public string Name
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

