﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Windows.Input;

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
				ProcPropertyChanged(ref _athlete, value, AthletePropertyName);
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
				ProcPropertyChanged(ref _league, value, LeaguePropertyName);
			}
		}

		public ICommand GetAllMembershipsByAthleteCommand
		{
			get
			{
				return new Command(async() => await GetAllMembershipsByAthlete(true));
			}
		}

		async public Task GetAllMembershipsByAthlete(bool forceRefresh = false)
		{
			if(!forceRefresh && Athlete.Memberships != null && Athlete.Memberships.Count > 0)
				return;
			
			await RunSafe(AzureService.Instance.GetAllLeaguesByAthlete(Athlete));
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
			if(Athlete != null)
				Athlete.RefreshMemberships();

			if(League != null)
				League.RefreshMemberships();
		}

		async public Task GetAllMembershipsByLeague(bool forceRefresh = false)
		{
			if(!forceRefresh && _hasLoadedBefore)
				return;

			LocalRefresh();
			await RunSafe(AzureService.Instance.GetAllAthletesByLeague(League));
			_hasLoadedBefore = true;
			LocalRefresh();
		}
	}
}