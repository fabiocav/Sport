using System;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace SportChallengeMatchRank.Shared
{
	public partial class WelcomeStartPage : WelcomeStartPageXaml
	{
		public WelcomeStartPage()
		{
			Initialize();
		}

		protected override void Initialize()
		{
			InitializeComponent();
			Title = "Welcome!";

			NavigationPage.SetHasNavigationBar(this, false);

			btnAuthenticate.Clicked += async(sender, e) =>
			{
				await EnsureAthleteAuthenticated();
				if(App.CurrentAthlete != null)
				{
					await Navigation.PushAsync(new SetAliasPage());
				}
			};
		}

		protected async override void OnLoaded()
		{
			base.OnLoaded();

			await Task.Delay(300);
			await label1.ScaleTo(1, (uint)App.AnimationSpeed, Easing.SinIn);
			await label2.ScaleTo(1, (uint)App.AnimationSpeed, Easing.SinIn);
			await btnAuthenticate.ScaleTo(1, (uint)App.AnimationSpeed, Easing.SinIn);
		}
	}

	public partial class WelcomeStartPageXaml : BaseContentPage<AuthenticationViewModel>
	{
	}
}