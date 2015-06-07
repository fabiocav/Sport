using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.LeaderboardViewModel))]
namespace SportChallengeMatchRank.Shared
{
	public class LeaderboardViewModel : BaseViewModel
	{
		bool _hasLoadedBefore;

		League _league;

		public League League
		{
			get
			{
				return _league;
			}
			set
			{
				if(value != _league)
				{
					_hasLoadedBefore = false;
					Memberships.Clear();
				}

				SetPropertyChanged(ref _league, value);
			}
		}

		public ObservableCollection<MembershipViewModel> Memberships
		{
			get;
			set;
		}

		public LeaderboardViewModel()
		{
			Memberships = new ObservableCollection<MembershipViewModel>();
		}

		public ICommand GetLeaderboardCommand
		{
			get
			{
				return new Command(async() => await GetLeaderboard(true));
			}
		}

		async public Task GetLeaderboard(bool forceRefresh = false)
		{
			if(!forceRefresh && _hasLoadedBefore)
				return;

			using(new Busy(this))
			{
				LocalRefresh();

				var task = AzureService.Instance.GetAllAthletesForLeague(League);
				await RunSafe(task);

				_hasLoadedBefore = true;
				LocalRefresh();
			}
		}

		public void LocalRefresh()
		{
			var memberships = Memberships.Select(vm => vm.Membership).ToList();

			var comparer = new MembershipComparer();
			var toRemove = memberships.Except(League.Memberships, comparer).ToList();
			var toAdd = League.Memberships.Except(memberships, comparer).ToList();

			toRemove.ForEach(m => Memberships.Remove(Memberships.Single(vm => vm.Membership == m)));

			foreach(var m in toAdd)
			{
				var prev = Memberships.Select(v => v.Membership).SingleOrDefault(mem => mem.CurrentRank == m.CurrentRank - 1);
				var index = 0;

				if(prev != null)
					index = Memberships.Select(v => v.Membership).ToList().IndexOf(prev) + 1;

				var vm = new MembershipViewModel(m);
				Memberships.Insert(index, vm);
			}

			Memberships.ToList().ForEach(vm => vm.Membership.LocalRefresh());
		}
	}
}