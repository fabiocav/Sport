using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Windows.Input;
using System.Linq;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.AthleteChallengesViewModel))]
namespace SportChallengeMatchRank.Shared
{
	public class AthleteChallengesViewModel : BaseViewModel
	{
		public AthleteChallengesViewModel()
		{
			LocalRefresh();
		}

		bool _hasLoadedBefore;
		public const string HasLoadedBeforePropertyName = "HasLoadedBefore";

		public bool HasLoadedBefore
		{
			get
			{
				return _hasLoadedBefore;
			}
			set
			{
				SetProperty(ref _hasLoadedBefore, value, HasLoadedBeforePropertyName);
			}
		}

		ObservableCollection<Challenge> _challenges = new ObservableCollection<Challenge>();
		public const string ChallengesPropertyName = "Challenges";

		public ObservableCollection<Challenge> Challenges
		{
			get
			{
				return _challenges;
			}
			set
			{
				SetProperty(ref _challenges, value, ChallengesPropertyName);
			}
		}

		public ICommand GetChallengesCommand
		{
			get
			{
				return new Command(async() => await GetChallenges(true));
			}
		}

		public void LocalRefresh()
		{
			if(App.CurrentAthlete == null)
				return;

			Challenges.Clear();
			App.CurrentAthlete.Challenges.ForEach(Challenges.Add);
		}

		async public Task GetChallenges(bool forceRefresh = false)
		{
			if(App.CurrentAthlete == null)
				return;

			if(!forceRefresh && _hasLoadedBefore)
			{
				LocalRefresh();
				return;
			}

			Challenges.Clear();
			using(new Busy(this))
			{
				//	Console.WriteLine(IsBusy);
				await AzureService.Instance.GetAllChallengesByAthlete(App.CurrentAthlete);
				_hasLoadedBefore = true;
				LocalRefresh();
			}
			//Console.WriteLine(IsBusy);
		}
	}
}