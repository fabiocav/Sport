using System;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Linq;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.LeagueDetailsViewModel))]

namespace SportChallengeMatchRank.Shared
{
	public class LeagueDetailsViewModel : BaseViewModel
	{
		public LeagueDetailsViewModel()
		{
			League = new League();
		}

		public LeagueDetailsViewModel(League league = null)
		{
			League = league ?? new League();
		}

		League _league;
		public const string LeaguePropertyName = "League";

		public League League
		{
			get
			{
				return _league;
			}
			set
			{
				SetProperty(ref _league, value, LeaguePropertyName);
				JoinLeague = _league.Id == null;
			}
		}

		bool _joinLeague;
		public const string JoinLeaguePropertyName = "JoinLeague";

		public bool JoinLeague
		{
			get
			{
				return _joinLeague;
			}
			set
			{
				SetProperty(ref _joinLeague, value, JoinLeaguePropertyName);
			}
		}

		public ICommand SaveLeagueCommand
		{
			get
			{
				return new Command(async(param) =>
					await SaveLeague());
			}
		}

		async public Task SaveLeague()
		{
			using(new Busy(this))
			{
				try
				{
					await AzureService.Instance.SaveLeague(League);

					if(JoinLeague)
					{
						var membership = new Membership {
							AthleteId = App.CurrentAthlete.Id,
							LeagueId = League.Id,
							CurrentRank = 0,
						};

						await AzureService.Instance.SaveMembership(membership);
						App.CurrentAthlete.Memberships.Add(membership);
					}
				}
				catch(Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}

		async public Task DeleteLeague()
		{
			using(new Busy(this))
			{
				try
				{
					await AzureService.Instance.DeleteLeague(League.Id);
				}
				catch(Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}
	}
}