using Xamarin.Forms;
using System.Threading.Tasks;
using System;
using System.Linq;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.LeagueDetailsViewModel))]

namespace SportChallengeMatchRank.Shared
{
	public class LeagueDetailsViewModel : BaseViewModel
	{
		#region Properties

		public string CreatedBy
		{
			get
			{
				return League == null || League.CreatedByAthlete == null ? null : "created on {0} by {1}".Fmt(League.DateCreated.Value.ToString("MMMM dd, yyyy"), League.CreatedByAthlete.Name);
			}
		}

		public string SportDescription
		{
			get
			{
				return League == null ? null : "the honorable pastime of {0}".Fmt(League.Sport);
			}
		}

		public bool IsMember
		{
			get
			{
				return App.CurrentAthlete != null && League != null && App.CurrentAthlete.Memberships.Any(m => m.League.Id == League.Id);
			}
		}

		League _league;
		public const string LeaguePropertyName = "League";

		public string DateRange
		{
			get
			{
				if(League == null)
					return null;
				
				var range = "open season";

				if(League != null && League.StartDate.HasValue)
					range = "beginning {0}".Fmt(League.StartDate.Value.ToString("MMM dd, yyyy"));

				if(League != null && League.EndDate.HasValue)
					range += "- {0}".Fmt(League.EndDate.Value.ToString("MMM dd, yyyy"));

				return range;
			}
		}

		public League League
		{
			get
			{
				return _league;
			}
			set
			{
				SetProperty(ref _league, value, LeaguePropertyName);
				OnPropertyChanged("SportDescription");
				OnPropertyChanged("DateRange");
				OnPropertyChanged("CreatedBy");
				OnPropertyChanged("IsMember");
			}
		}

		#endregion

		async public Task LoadAthlete()
		{
			using(new Busy(this))
			{
				try
				{
					await AzureService.Instance.GetAthleteById(League.CreatedByAthleteId);
					League.RefreshMemberships();
					League.OnPropertyChanged("CreatedByAthlete");
					OnPropertyChanged("CreatedBy");
				}
				catch(Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}

		async public Task JoinLeague()
		{
			using(new Busy(this))
			{
				try
				{
					var membership = new Membership {
						AthleteId = App.CurrentAthlete.Id,
						LeagueId = League.Id,
						CurrentRank = 0,
					};

					await AzureService.Instance.SaveMembership(membership);
					OnPropertyChanged("IsMember");
				}
				catch(Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}

		async public Task LeaveLeague()
		{
			using(new Busy(this))
			{
				try
				{
					var membership = App.CurrentAthlete.Memberships.SingleOrDefault(m => m.LeagueId == League.Id);
					await AzureService.Instance.DeleteMembership(membership.Id);
					OnPropertyChanged("IsMember");
				}
				catch(Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}
	}
}