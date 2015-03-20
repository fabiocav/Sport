﻿using System;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace SportRankerMatchOn.Shared.Mobile
{
	public class LeagueDetailsViewModel : BaseViewModel
	{
		League _league;
		public const string LeaguePropertyName = "League";

		public LeagueDetailsViewModel()
		{
			League = new League();
		}

		public LeagueDetailsViewModel(League league = null)
		{
			League = league ?? new League();
		}

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
							try
							{
								League.Season = 3;
								League.MemberIds.Add(Guid.NewGuid().ToString());
								League.MemberIds.Add(Guid.NewGuid().ToString());
								League.MemberIds.Add(Guid.NewGuid().ToString());
								League.MemberIds.Add(Guid.NewGuid().ToString());

								var leagues = await AzureService.Instance.GetAllLeagues();

								await AzureService.Instance.SaveLeague(League);
							}
							catch(Exception e)
							{
								Console.WriteLine(e);
							}
						}
					});
			}
		}
	}
}