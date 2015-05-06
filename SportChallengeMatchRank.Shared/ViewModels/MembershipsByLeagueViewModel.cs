using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Windows.Input;
using System.Linq;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.MembershipsByLeagueViewModel))]
namespace SportChallengeMatchRank.Shared
{
	public class MembershipsByLeagueViewModel : BaseViewModel
	{
		bool _hasLoadedBefore;
		Athlete _athlete;
		public const string AthletePropertyName = "Athlete";

		public Athlete Athlete
		{
			get
			{
				return _athlete;
			}
			set
			{
				SetPropertyChanged(ref _athlete, value, AthletePropertyName);
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
				SetPropertyChanged(ref _league, value, LeaguePropertyName);
			}
		}

		public ObservableCollection<Membership> Memberships
		{
			get;
			set;
		}

		public ICommand GetAllMembershipsByAthleteCommand
		{
			get
			{
				return new Command(async() => await GetAllMembershipsByAthlete(true));
			}
		}

		public MembershipsByLeagueViewModel()
		{
			Memberships = new ObservableCollection<Membership>();
		}

		async public Task GetAllMembershipsByAthlete(bool forceRefresh = false)
		{
			if(!forceRefresh && Athlete.Memberships != null && Athlete.Memberships.Count > 0)
				return;

			var task = AzureService.Instance.GetAllLeaguesByAthlete(Athlete);
			await RunSafe(task);
			LocalRefresh();
		}

		public ICommand GetAllMembershipsByLeagueCommand
		{
			get
			{
				return new Command(async() => await GetAllMembershipsByLeague(true));
			}
		}

		public void LocalRefresh()
		{
			Memberships.Clear();

			if(Athlete != null)
			{
				Athlete.RefreshMemberships();
				Athlete.Memberships.ToList().ForEach(Memberships.Add);
			}

			if(League != null)
			{
				League.RefreshMemberships();
				League.Memberships.ToList().ForEach(Memberships.Add);
			}
		}

		async public Task GetAllMembershipsByLeague(bool forceRefresh = false)
		{
			if(!forceRefresh && _hasLoadedBefore)
				return;

			using(new Busy(this))
			{
				LocalRefresh();

				var task = AzureService.Instance.GetAllAthletesByLeague(League);
				await RunSafe(task);

				_hasLoadedBefore = true;
				LocalRefresh();
			}
		}
	}
}