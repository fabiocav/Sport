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

		public void NotifyPropertiesChanged()
		{
			SetPropertyChanged("Membership");
		}
	}
}