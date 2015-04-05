using Xamarin.Forms;
using System;

namespace SportChallengeMatchRank.Shared
{
	public partial class PhotoSelectionPage : PhotoSelectionXaml
	{
		public PhotoSelectionPage(League league)
		{
			ViewModel.League = league;
			Initialize();
		}

		public Action OnImageSelected
		{
			get;
			set;
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
				ViewModel.League.ImageUrl = (string)e.SelectedItem;

				if(OnImageSelected != null)
					OnImageSelected();
			};

			await ViewModel.GetPhotos(ViewModel.League.Sport);
		}
	}

	public partial class PhotoSelectionXaml : BaseContentPage<PhotoSelectionViewModel>
	{
	}
}