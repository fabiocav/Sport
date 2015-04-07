using Xamarin.Forms;
using System;
using System.Threading.Tasks;
using Toasts.Forms.Plugin.Abstractions;

namespace SportChallengeMatchRank.Shared
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

				var league = list.SelectedItem as League;
				list.SelectedItem = null;

				var page = new LeagueDetailsPage(league);
				page.OnJoinedLeague = async(l) =>
				{
					ViewModel.LocalRefresh();
					if(OnJoinedLeague != null)
					{
						OnJoinedLeague(l);
					}

					if(ViewModel.Leagues.Count == 0)
					{
						await Navigation.PopModalAsync();
					}
				};

				await Navigation.PushAsync(page);
			};

			var btnCancel = new ToolbarItem {
				Text = "Cancel"
			};

			btnCancel.Clicked += async(sender, e) =>
			{
				await Navigation.PopModalAsync();		
			};

			ToolbarItems.Add(btnCancel);
			await ViewModel.GetAvailableLeagues();
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