using System;

namespace SportChallengeMatchRank.Shared
{
	public class BaseModel : BaseNotify
	{
		string _id;

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
	}
}