﻿using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.ChallengeDetailsViewModel))]

namespace SportChallengeMatchRank.Shared
{
	public class ChallengeDetailsViewModel : BaseViewModel
	{
		public ChallengeDetailsViewModel()
		{
			Challenge = new Challenge();
		}

		public ChallengeDetailsViewModel(Challenge challenge)
		{
			Challenge = challenge;
		}

		Challenge challenge;
		public const string ChallengePropertyName = "Challenge";

		public Challenge Challenge
		{
			get
			{
				return challenge;
			}
			set
			{
				SetProperty(ref challenge, value, ChallengePropertyName);
			}
		}

		public bool CanAccept
		{
			get
			{
				return Challenge.ChallengeeAthleteId == App.CurrentAthlete.Id && !challenge.IsAccepted && !Challenge.IsCompleted;
			}
		}

		public bool CanDecline
		{
			get
			{
				return Challenge.ChallengeeAthleteId == App.CurrentAthlete.Id && !Challenge.IsCompleted;
			}
		}

		public bool CanRevoke
		{
			get
			{
				return Challenge.ChallengerAthleteId == App.CurrentAthlete.Id && !Challenge.IsCompleted;
			}
		}

		public bool CanPostMatchResults
		{
			get
			{
				return Challenge.IsAccepted && !Challenge.IsCompleted;
			}
		}

		async public Task GetMatchResults(bool forceRefresh = false)
		{
			if(!forceRefresh && Challenge.MatchResult.Count > 0)
				return;

			var task = new Task<List<GameResult>>(() => AzureService.Instance.Client.GetTable<GameResult>().Where(r => r.ChallengeId == Challenge.Id).OrderBy(r => r.Index).ToListAsync().Result);
			await RunSafe(task);
			var results = task.Result;

			Challenge.MatchResult.Clear();
			results.ForEach(Challenge.MatchResult.Add);
			OnPropertyChanged("Challenge");
		}

		async public Task AcceptChallenge()
		{
			await RunSafe(AzureService.Instance.AcceptChallenge(Challenge));
			NotifyPropertiesChanged();
		}

		async public Task DeclineChallenge()
		{
			await RunSafe(AzureService.Instance.DeclineChallenge(Challenge.Id));
			NotifyPropertiesChanged();
		}

		public void NotifyPropertiesChanged()
		{
			OnPropertyChanged("CanAccept");
			OnPropertyChanged("CanDecline");
			OnPropertyChanged("CanRevoke");
			OnPropertyChanged("CanPostMatchResults");
			OnPropertyChanged("Challenge");
		}
	}
}