﻿using System;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.AthleteEditViewModel))]

namespace SportChallengeMatchRank.Shared
{
	public class AthleteEditViewModel : BaseViewModel
	{
		public AthleteEditViewModel()
		{
			Athlete = new Athlete();
		}

		public AthleteEditViewModel(Athlete athlete = null)
		{
			Athlete = athlete ?? new Athlete();
		}

		Athlete _athlete;

		public Athlete Athlete
		{
			get
			{
				return _athlete;
			}
			set
			{
				SetPropertyChanged(ref _athlete, value);
			}
		}

		public ICommand SaveAthleteCommand
		{
			get
			{
				return new Command(async(param) =>
					await SaveAthlete());
			}
		}

		async public Task SaveAthlete()
		{
			await RunSafe(InternetService.Instance.SaveAthlete(Athlete));
		}

		async public Task DeleteAthlete()
		{
			await RunSafe(InternetService.Instance.DeleteAthlete(Athlete.Id));
		}
	}
}