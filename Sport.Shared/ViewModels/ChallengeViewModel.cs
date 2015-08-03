using System.Threading.Tasks;
using System.Linq;
using Google.Apis.Calendar.v3;
using System;
using Google.Apis.Services;
using Google.Apis.Calendar.v3.Data;
using System.Collections.Generic;

namespace Sport.Shared
{
	public class ChallengeViewModel : BaseViewModel
	{
		public ChallengeViewModel(Challenge challenge)
		{
			Challenge = challenge;
		}

		protected Challenge _challenge;

		public Challenge Challenge
		{
			get
			{
				return _challenge;
			}
			set
			{
				SetPropertyChanged(ref _challenge, value);
			}
		}

		string _emptyMessage;

		public string EmptyMessage
		{
			get
			{
				return _emptyMessage;
			}
			set
			{
				SetPropertyChanged(ref _emptyMessage, value);
			}
		}

		public virtual void NotifyPropertiesChanged()
		{
			SetPropertyChanged("Challenge");
			SetPropertyChanged("EmptyMessage");
		}
	}
}