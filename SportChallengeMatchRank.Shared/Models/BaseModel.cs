using System;
using Newtonsoft.Json;

namespace SportChallengeMatchRank.Shared
{
	public class BaseModel : BaseNotify, IDirty
	{
		string _id;

		[JsonProperty("id")]
		public string Id
		{
			get
			{
				return _id;
			}
			set
			{
				SetPropertyChanged(ref _id, value);
			}
		}

		DateTime? _dateCreated;

		public DateTime? DateCreated
		{
			get
			{
				return _dateCreated;
			}
			set
			{
				SetPropertyChanged(ref _dateCreated, value);
			}
		}

		public bool IsDirty
		{
			get;
			set;
		}
	}
}