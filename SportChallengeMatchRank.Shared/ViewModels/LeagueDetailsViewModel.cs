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
				SetPropertyChanged(ref _league, value);
				SetPropertyChanged("SportDescription");
				SetPropertyChanged("DateRange");
				SetPropertyChanged("CreatedBy");
				SetPropertyChanged("IsMember");
			}
		}

		#endregion

		async public Task LoadAthlete()
		{
			await RunSafe(InternetService.Instance.GetAthleteById(League.CreatedByAthleteId));
			League.RefreshMemberships();
			League.SetPropertyChanged("CreatedByAthlete");
			SetPropertyChanged("CreatedBy");
		}

		async public Task JoinLeague()
		{
			var membership = new Membership {
				AthleteId = App.CurrentAthlete.Id,
				LeagueId = League.Id,
				CurrentRank = 0,
			};

			await RunSafe(InternetService.Instance.SaveMembership(membership));
			SetPropertyChanged("IsMember");
		}

		async public Task LeaveLeague()
		{
			var membership = App.CurrentAthlete.Memberships.SingleOrDefault(m => m.LeagueId == League.Id);
			await RunSafe(InternetService.Instance.DeleteMembership(membership.Id));
			SetPropertyChanged("IsMember");
		}
	}
}