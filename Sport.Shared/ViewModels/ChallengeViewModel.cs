using System.Threading.Tasks;
using System.Linq;

namespace Sport.Shared
{
	public class ChallengeViewModel : BaseViewModel
	{
		public ChallengeViewModel(Challenge challenge)
		{
			Challenge = challenge;
		}

		public Challenge Challenge
		{
			get;
			set;
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

		public void NotifyPropertiesChanged()
		{
			SetPropertyChanged("Challenge");
			SetPropertyChanged("EmptyMessage");
		}
	}
}