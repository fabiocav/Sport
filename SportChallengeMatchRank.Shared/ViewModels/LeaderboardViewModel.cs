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

		public ObservableCollection<Membership> Memberships
		{
			get;
			set;
		}

		public LeaderboardViewModel()
		{
			Memberships = new ObservableCollection<Membership>();
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
			League.RefreshMemberships();
			var comparer = new MembershipComparer();
			var toRemove = Memberships.Except(League.Memberships, comparer).ToList();
			var toAdd = League.Memberships.Except(Memberships, comparer).OrderBy(r => r.CurrentRank).ToList();

			toRemove.ForEach(l => Memberships.Remove(l));

			foreach(var m in toAdd)
			{
				var prev = Memberships.SingleOrDefault(mem => mem.CurrentRank == m.CurrentRank - 1);
				var index = 0;

				if(prev != null)
					index = Memberships.IndexOf(prev) + 1;
				
				Memberships.Insert(index, m);
			}
		}

		public class MembershipComparer : IEqualityComparer<Membership>
		{
			public bool Equals(Membership x, Membership y)
			{
				return x?.Id == y?.Id && x.UpdatedAt == y.UpdatedAt && x.CurrentRank == y.CurrentRank;
			}

			public int GetHashCode(Membership obj)
			{
				return obj.Id != null ? obj.Id.GetHashCode() : base.GetHashCode();
			}
		}
	}
}