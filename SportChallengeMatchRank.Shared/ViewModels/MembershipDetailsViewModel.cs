using System;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Linq;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.MembershipDetailsViewModel))]

namespace SportChallengeMatchRank.Shared
{
	public class MembershipDetailsViewModel : BaseViewModel
	{
		public MembershipDetailsViewModel()
		{
			Membership = new Membership();
		}

		public MembershipDetailsViewModel(Membership membership = null)
		{
			Membership = membership ?? new Membership();
		}

		Membership membership;
		public const string MembershipPropertyName = "Membership";

		public Membership Membership
		{
			get
			{
				return membership;
			}
			set
			{
				SetProperty(ref membership, value, MembershipPropertyName);
				OnPropertyChanged("CanChallenge");
				OnPropertyChanged("HasChallenged");
			}
		}

		public bool CanRevokeChallenge
		{
			get
			{
				var challenge = Membership.GetExistingChallengeWithAthlete(App.CurrentAthlete);
				return challenge != null && challenge.ChallengerAthleteId == App.CurrentAthlete.Id;
			}
		}

		public bool CanChallenge
		{
			get
			{
				return Membership.CanChallengeAthlete(App.CurrentAthlete);
			}
		}

		async public Task RevokeExistingChallenge(Membership membership)
		{
			var challenge = membership.GetExistingChallengeWithAthlete(App.CurrentAthlete);
			await AzureService.Instance.DeleteChallenge(challenge.Id);
			App.CurrentAthlete.RefreshChallenges();
			Membership.Athlete.RefreshChallenges();

			OnPropertyChanged("HasChallenged");
			OnPropertyChanged("CanChallenge");
		}

		async public Task<Challenge> ChallengeAthlete(Membership membership)
		{
			var challenge = new Challenge {
				ChallengerAthleteId = App.CurrentAthlete.Id,
				ChallengeeAthleteId = membership.AthleteId,
				ProposedTime = DateTime.Now.AddHours(3),
				LeagueId = membership.LeagueId,
			};

			await AzureService.Instance.SaveChallenge(challenge);
			App.CurrentAthlete.RefreshChallenges();
			Membership.Athlete.RefreshChallenges();

			OnPropertyChanged("CanChallenge");
			OnPropertyChanged("HasChallenged");

			return challenge;
		}

		public ICommand SaveCommand
		{
			get
			{
				return new Command(async(param) =>
					await SaveMembership());
			}
		}

		async public Task SaveMembership()
		{
			using(new Busy(this))
			{
				try
				{
					await AzureService.Instance.SaveMembership(Membership);
				}
				catch(Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}

		async public Task DeleteMembership()
		{
			using(new Busy(this))
			{
				try
				{
					await AzureService.Instance.DeleteMembership(Membership.Id);
				}
				catch(Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}
	}
}