using System;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;

[assembly: Dependency(typeof(SportRankerMatchOn.Shared.LeagueDetailsViewModel))]

namespace SportRankerMatchOn.Shared
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
			JoinLeague = league.Id == null;
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
					bool wasNew = League.Id == null;
					await AzureService.Instance.SaveLeague(League);

					if(JoinLeague)
					{
						await AzureService.Instance.SaveMembership(new Membership {
								AthleteId = App.CurrentAthlete.Id,
								LeagueId = League.Id,
								CurrentRank = 0,
							});
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