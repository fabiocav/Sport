using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;
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

		#region Properties

		string errorMessage;
		public const string ErrorMessagePropertyName = "ErrorMessage";

		public string ErrorMessage
		{
			get
			{
				return errorMessage;
			}
			set
			{
				SetProperty(ref errorMessage, value, ErrorMessagePropertyName);
			}
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
				ErrorMessage = null;
				UpdateMembershipStatus();
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

		#endregion

		public void UpdateMembershipStatus()
		{
			JoinLeague = League.Id == null || App.CurrentAthlete.Memberships.All(m => m.LeagueId != League.Id);
		}

		async public Task<SaveLeagueResult> SaveLeague()
		{
			using(new Busy(this))
			{
				SaveLeagueResult result = SaveLeagueResult.None;
				try
				{
					League.Name = League.Name ?? League.Name.Trim();
					League.Sport = League.Sport ?? League.Sport.Trim();

					result = await AzureService.Instance.SaveLeague(League);

					if(JoinLeague && result == SaveLeagueResult.OK)
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

				return result;
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

		public bool IsValid()
		{
			var sb = new StringBuilder();
			if(string.IsNullOrWhiteSpace(League.Name))
			{
				sb.AppendLine("enter a league name");
			}

			if(string.IsNullOrWhiteSpace(League.Sport))
			{
				sb.AppendLine("enter a sport");
			}

			ErrorMessage = sb.Length > 0 ? sb.ToString() : null;
			return ErrorMessage == null;
		}
	}
}