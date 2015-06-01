using Xamarin.Forms;
using System;
using System.Threading.Tasks;

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

			//list.ButtonStyle = (Style)App.Current.Resources["blueActionButtonStyle"];
			list.ItemSelected += async(sender, e) =>
			{
				if(list.SelectedItem == null)
					return;

				var league = list.SelectedItem as League;
				list.SelectedItem = null;

				//Empty message?
				if(league.Id == null)
					return;

				var page = new LeagueDetailsPage(league);

				page.OnJoinedLeague = async(l) =>
				{
					ViewModel.LocalRefresh();
					if(OnJoinedLeague != null)
					{
						OnJoinedLeague(l);
					}
				};

				await Navigation.PushAsync(page);
			};

			var btnCancel = new ToolbarItem {
				Text = "Done",
			};

			btnCancel.Clicked += async(sender, e) =>
			{
				await Navigation.PopModalAsync();		
			};

			ToolbarItems.Add(btnCancel);
			await ViewModel.GetAvailableLeagues(true);
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