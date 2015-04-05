using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Text;
using System.Linq;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.LeagueEditViewModel))]

namespace SportChallengeMatchRank.Shared
{
	public class LeagueEditViewModel : BaseViewModel
	{
		bool _wasMember;

		public LeagueEditViewModel()
		{
			League = new League();
		}

		public LeagueEditViewModel(League league = null)
		{
			League = league ?? new League();
		}

		#region Properties

		public bool CanStartLeague
		{
			get
			{
				return League != null && League.Id != null && !League.HasStarted;
			}
		}

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
				OnPropertyChanged("CanStartLeague");
				UpdateMembershipStatus();
			}
		}

		bool isMember;
		public const string IsMemberPropertyName = "IsMember";

		public bool IsMember
		{
			get
			{
				return isMember;
			}
			set
			{
				SetProperty(ref isMember, value, IsMemberPropertyName);
			}
		}

		#endregion

		public void UpdateMembershipStatus()
		{
			_wasMember = League.Id != null && App.CurrentAthlete.Memberships.Any(m => m.LeagueId == League.Id);
			IsMember = _wasMember;
		}

		async public Task<SaveLeagueResult> SaveLeague()
		{
			using(new Busy(this))
			{
				SaveLeagueResult result;
				try
				{
					await FlikrService.Instance.SearchPhotos(League.Sport);

					League.Name = League.Name ?? League.Name.Trim();
					League.Sport = League.Sport ?? League.Sport.Trim();
					League.CreatedByAthleteId = App.CurrentAthlete.Id;

					result = await AzureService.Instance.SaveLeague(League);

					if(!_wasMember && IsMember && result == SaveLeagueResult.OK)
					{
						var membership = new Membership {
							AthleteId = App.CurrentAthlete.Id,
							LeagueId = League.Id,
							CurrentRank = 0,
						};

						await AzureService.Instance.SaveMembership(membership);
					}
				}
				catch(Exception e)
				{
					result = SaveLeagueResult.Failed;
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

		async public Task<DateTime?> StartLeague()
		{
			using(new Busy(this))
			{
				try
				{
					var date = await AzureService.Instance.StartLeague(League.Id);
					League.HasStarted = date != null;
					OnPropertyChanged("CanStartLeague");
					return date;
				}
				catch(Exception e)
				{
					Console.WriteLine(e);
					throw new Exception("League does not have the minimum amount of players (4) to begin");
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