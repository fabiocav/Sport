using System;
using Toasts.Forms.Plugin.Abstractions;
using System.Threading.Tasks;
using Xamarin.Forms;

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
			ViewModel.AthleteId = App.CurrentAthlete.Id;
			Initialize();
		}

		protected async override void Initialize()
		{
			InitializeComponent();
			Title = "Athlete Alias";

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
			label1.ScaleTo(1, 500, Easing.SinIn);

			await Task.Delay(250);
			profileStack.ScaleTo(1, 500, Easing.SinIn);

			await Task.Delay(250);
			buttonStack.ScaleTo(1, 500, Easing.SinIn);

			await Task.Delay(2000);
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