﻿using Xamarin.Forms;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Sport.Shared
{
	public partial class AvailableLeaguesPage : AvailableLeaguesXaml
	{
		public AvailableLeaguesPage()
		{
			Initialize();
		}

		public Action<League> OnJoinedLeague
		{
			get;
			set;
		}

		async protected override void Initialize()
		{
			InitializeComponent();
			Title = "Available Leagues";

			list.ItemSelected += async(sender, e) =>
			{
				if(list.SelectedItem == null)
					return;

				var vm = list.SelectedItem as LeagueViewModel;
				list.SelectedItem = null;

				//Empty message
				if(vm.League.Id == null)
					return;

				var page = new LeagueDetailsPage(vm.League);

				page.OnJoinedLeague = async(l) =>
				{
					ViewModel.LocalRefresh();
					if(OnJoinedLeague != null)
					{
						OnJoinedLeague(l);
					}

					await Navigation.PopAsync();
				};

				await Navigation.PushAsync(page);
			};

			AddDoneButton();

			using(new HUD("Getting leagues..."))
			{
				await ViewModel.GetAvailableLeagues(true);
				ViewModel.Leagues.Select(vm => vm.League).ToList().ForEach(App.Current.GetTheme);
			}
		}

		protected override void OnDisappearing()
		{
			ViewModel.CancelTasks();
			base.OnDisappearing();
		}
	}

	public partial class AvailableLeaguesXaml : BaseContentPage<AvailableLeaguesViewModel>
	{
	}
}