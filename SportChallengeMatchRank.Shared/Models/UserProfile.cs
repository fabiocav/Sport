using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SportChallengeMatchRank.Shared
{
	public class Name
	{

		[JsonProperty("familyName")]
		public string FamilyName
		{
			get;
			set;
		}

		[JsonProperty("givenName")]
		public string GivenName
		{
			get;
			set;
		}
	}

	public class Image
	{

		[JsonProperty("url")]
		public string Url
		{
			get;
			set;
		}

		[JsonProperty("isDefault")]
		public bool IsDefault
		{
			get;
			set;
		}
	}

	public class UserProfile
	{
		[JsonProperty("email")]
		public string Email
		{
			get;
			set;
		}

		[JsonProperty("kind")]
		public string Kind
		{
			get;
			set;
		}

		[JsonProperty("etag")]
		public string Etag
		{
			get;
			set;
		}

		[JsonProperty("gender")]
		public string Gender
		{
			get;
			set;
		}

		[JsonProperty("objectType")]
		public string ObjectType
		{
			get;
			set;
		}

		[JsonProperty("id")]
		public string Id
		{
			get;
			set;
		}

		[JsonProperty("displayName")]
		public string DisplayName
		{
			get;
			set;
		}

		[JsonProperty("name")]
		public Name Name
		{
			get;
			set;
		}

		[JsonProperty("url")]
		public string Url
		{
			get;
			set;
		}

		[JsonProperty("image")]
		public Image Image
		{
			get;
			set;
		}

		[JsonProperty("isPlusUser")]
		public bool IsPlusUser
		{
			get;
			set;
		}

		[JsonProperty("circledByCount")]
		public int CircledByCount
		{
			get;
			set;
		}

		[JsonProperty("verified")]
		public bool Verified
		{
			get;
			set;
		}
	}
}