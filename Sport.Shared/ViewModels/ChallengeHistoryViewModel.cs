using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;
using System;
using Xamarin;

[assembly: Dependency(typeof(Sport.Shared.ChallengeHistoryViewModel))]
namespace Sport.Shared
{
	public class ChallengeHistoryViewModel : BaseViewModel
	{
		bool _hasLoadedBefore;

		Membership _membership;

		public Membership Membership
		{
			get
			{
				return _membership;
			}
			set
			{
				if(value != _membership)
				{
					_hasLoadedBefore = false;

					if(Challenges != null)
						Challenges.Clear();
				}

				SetPropertyChanged(ref _membership, value);
			}
		}

		public ObservableCollection<ChallengeViewModel> Challenges
		{
			get;
			set;
		}

		public ChallengeHistoryViewModel()
		{
			Challenges = new ObservableCollection<ChallengeViewModel>();
		}

		public ICommand GetChallengeHistoryCommand
		{
			get
			{
				return new Command(async() => await GetChallengeHistory(true));
			}
		}

		async public Task GetChallengeHistory(bool forceRefresh = false)
		{
			if(!forceRefresh && _hasLoadedBefore)
				return;

//			using(new Busy(this))
//			{
//				var task = AzureService.Instance.G(League.Id, true);
//				await RunSafe(task);
//
//				if(task.IsFaulted)
//					return;
//
//				LocalRefresh();
//			}
		}

		public void LocalRefresh()
		{
			try
			{
//				League.RefreshMemberships();
//				League.RefreshChallenges();
//
//				var memberships = Challenges.Select(vm => vm.Challenges).ToList();
//
//				var comparer = new MembershipComparer();
//				var toRemove = memberships.Except(League.Memberships, comparer).ToList();
//				var toAdd = League.Memberships.Except(memberships, comparer).ToList();
//
//				toRemove.ForEach(m => Memberships.Remove(Memberships.Single(vm => vm.Membership == m)));
//
//				toAdd.ForEach(m => Memberships.Add(new MembershipViewModel(m)));
//
//				Memberships.Sort(new MembershipSortComparer());
//				Memberships.ToList().ForEach(vm => vm.NotifyPropertiesChanged());
//
//				if(Memberships.Count == 0)
//				{
//					Memberships.Add(new MembershipViewModel(null) {
//						EmptyMessage = "This league has no members yet"
//					});
//				}
			}
			catch(Exception e)
			{
				Insights.Report(e);
			}
		}
	}
}