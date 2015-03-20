﻿using System;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace SportRankerMatchOn.Shared.Mobile
{
	public class NewLeagueViewModel : BaseViewModel
	{
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
				SetProperty(ref _league, value, LeaguePropertyName);
			}
		}

		public ICommand SaveLeagueCommand
		{
			get
			{
				return new Command(async(param) =>
					{
						using(new Busy(this))
						{
							IsBusy = true;
							try
							{
								await AzureService.Instance.SaveLeague(League);
							}
							catch(Exception e)
							{
								Console.WriteLine(e);
							}
							IsBusy = false;
						}
					});
			}
		}
	}
}