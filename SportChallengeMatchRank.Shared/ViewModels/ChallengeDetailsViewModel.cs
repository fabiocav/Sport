using System;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Linq;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.ChallengeDetailsViewModel))]

namespace SportChallengeMatchRank.Shared
{
	public class ChallengeDetailsViewModel : BaseViewModel
	{
		public ChallengeDetailsViewModel()
		{
			Challenge = new Challenge();
		}

		public ChallengeDetailsViewModel(Challenge challenge)
		{
			Challenge = challenge;
		}

		Challenge challenge;
		public const string ChallengePropertyName = "Challenge";

		public Challenge Challenge
		{
			get
			{
				return challenge;
			}
			set
			{
				SetProperty(ref challenge, value, ChallengePropertyName);
			}
		}

		public bool CanAccept
		{
			get
			{
				return Challenge.ChallengeeAthleteId == App.CurrentAthlete.Id && !challenge.IsAccepted;
			}
		}

		public bool CanDecline
		{
			get
			{
				return Challenge.ChallengeeAthleteId == App.CurrentAthlete.Id;
			}
		}

		public bool CanRevoke
		{
			get
			{
				return Challenge.ChallengerAthleteId == App.CurrentAthlete.Id;
			}
		}

		async public Task AcceptChallenge()
		{
			using(new Busy(this))
			{
				try
				{
					await AzureService.Instance.AcceptChallenge(Challenge);
					OnPropertyChanged("CanAccept");
					OnPropertyChanged("CanDecline");
					OnPropertyChanged("CanRevoke");
				}
				catch(Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}

		async public Task DeclineChallenge()
		{
			using(new Busy(this))
			{
				try
				{
					await AzureService.Instance.DeclineChallenge(Challenge.Id);
					OnPropertyChanged("CanAccept");
					OnPropertyChanged("CanDecline");
					OnPropertyChanged("CanRevoke");
				}
				catch(Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}
	}
}