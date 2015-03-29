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
				_canChallenge = null;
				OnPropertyChanged("CanChallenge");
			}
		}

		private bool? _canChallenge;

		public bool CanChallenge
		{
			get
			{
				if(!_canChallenge.HasValue)
				{
					_canChallenge = false;
					var membership = App.CurrentAthlete.Memberships.SingleOrDefault(m => m.LeagueId == Membership.LeagueId);

					if(membership != null)
					{
						var diff = Membership.CurrentRank - membership.CurrentRank;
						_canChallenge = diff > 0 && diff <= Membership.League.MaxChallengeRange;
					}

					if(_canChallenge.Value)
					{
						var alreadyChallenged = App.CurrentAthlete.Challenges.Any(c => (c.ChallengeeAthleteId == App.CurrentAthlete.Id ||
							                        c.ChallengerAthleteId == App.CurrentAthlete.Id) && c.LeagueId == Membership.LeagueId);

						_canChallenge = !alreadyChallenged;
					}
				}

				return _canChallenge.Value;
			}
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

			OnPropertyChanged("CanChallenge");
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