using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
{
	public partial class AvailableLeaguesPage : AvailableLeaguesXaml
	{
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
				await Navigation.PushAsync(new LeagueDetailsPage(league));
			};

			var btnCancel = new ToolbarItem {
				Text = "Cancel"		
			};

			btnCancel.Clicked += async(sender, e) =>
			{
				await Navigation.PopModalAsync();		
			};

			ToolbarItems.Add(btnCancel);

			await ViewModel.GetAvailableLeagues(true);
		}
	}

	public partial class AvailableLeaguesXaml : BaseContentPage<AvailableLeaguesViewModel>
	{
	}
}