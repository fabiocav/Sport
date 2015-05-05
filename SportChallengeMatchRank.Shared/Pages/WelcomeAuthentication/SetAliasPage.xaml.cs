using System;
using Toasts.Forms.Plugin.Abstractions;
using System.Threading.Tasks;
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace SportChallengeMatchRank.Shared
{
	public partial class SetAliasPage : SetAliasPageXaml
	{
		public Action OnSave
		{
			get;
			set;
		}

		public SetAliasPage()
		{
			NavigationPage.SetHasNavigationBar(this, false);
			ViewModel.AthleteId = App.CurrentAthlete.Id;

			//ViewModel.Athlete.Alias = null;

			Initialize();
		}

		protected async override void Initialize()
		{
			InitializeComponent();
			Title = "Athlete Alias";

//			txtAlias.Font = Font.SystemFontOfSize();
//				FontFamily = "HelveticaNeue-Light",
//				FontSize = "36"
//			};

			btnSave.Clicked += async(sender, e) =>
			{
				if(string.IsNullOrWhiteSpace(ViewModel.Athlete.Alias))
				{
					"Please enter an alias.".ToToast(ToastNotificationType.Warning, "Oops");
					return;
				}

				var success = await ViewModel.SaveAthlete();

				if(success)
				{
					if(OnSave != null)
						OnSave();

					await Navigation.PushAsync(new ChooseLeaguesPage());
				}
			};
		}

		protected async override void OnLoaded()
		{
			base.OnLoaded();

			await Task.Delay(300);
			await label1.ScaleTo(1, (uint)App.AnimationSpeed, Easing.SinIn);
			await profileStack.ScaleTo(1, (uint)App.AnimationSpeed, Easing.SinIn);
			await buttonStack.ScaleTo(1, (uint)App.AnimationSpeed, Easing.SinIn);

			await Task.Delay(2000);

			if(IsVisible && string.IsNullOrEmpty(ViewModel.Athlete.Alias))
				txtAlias.Focus();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			ViewModel.Athlete.RefreshChallenges();
			ViewModel.Athlete.RefreshMemberships();
		}
	}

	public partial class SetAliasPageXaml : BaseContentPage<AthleteProfileViewModel>
	{
	}
}