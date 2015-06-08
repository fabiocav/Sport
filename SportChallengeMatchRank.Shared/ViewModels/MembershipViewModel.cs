using System.Threading.Tasks;
using System.Linq;

namespace SportChallengeMatchRank.Shared
{
	public class MembershipViewModel : BaseViewModel
	{
		public MembershipViewModel(Membership membership)
		{
			Membership = membership;
		}

		public Membership Membership
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
			SetPropertyChanged("Membership");
		}
	}
}