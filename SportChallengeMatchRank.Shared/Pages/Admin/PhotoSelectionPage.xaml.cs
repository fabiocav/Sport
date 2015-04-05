using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
{
	public partial class PhotoSelectionPage : PhotoSelectionXaml
	{
		public PhotoSelectionPage(League league)
		{
			ViewModel.League = league;
			Initialize();
		}

		async protected override void Initialize()
		{
			InitializeComponent();
			Title = "Select Photo";

			btnCancel.Clicked += async(sender, e) =>
			{
				await Navigation.PopModalAsync();
			};

			list.ItemSelected += async(sender, e) =>
			{
				await Navigation.PopModalAsync();
			};

			await ViewModel.GetPhotos(ViewModel.League.Sport);
		}
	}

	public partial class PhotoSelectionXaml : BaseContentPage<PhotoSelectionViewModel>
	{
	}
}