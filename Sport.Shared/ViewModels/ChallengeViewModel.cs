using System.Threading.Tasks;
using System.Linq;

namespace Sport.Shared
{
	public class ChallengeViewModel : BaseViewModel
	{
		public ChallengeViewModel(Challenge challenge)
		{
			Challenge = challenge;
			NotifyPropertiesChanged();
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

		public bool IsChallengerWinningAthlete
		{
			get
			{
				return Challenge != null && Challenge.WinningAthlete != null && Challenge.WinningAthlete.Id == Challenge.ChallengerAthleteId;
			}
		}

		public bool IsChallengeeWinningAthlete
		{
			get
			{
				return Challenge != null && Challenge.WinningAthlete != null && Challenge.WinningAthlete.Id == Challenge.ChallengeeAthleteId;
			}
		}

		public void NotifyPropertiesChanged()
		{
			SetPropertyChanged("Challenge");
			SetPropertyChanged("EmptyMessage");
			SetPropertyChanged("IsChallengeeWinningAthlete");
			SetPropertyChanged("IsChallengerWinningAthlete");
		}
	}
}