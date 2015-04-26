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
			btnLogIn.Clicked += async(sender, e) =>
			{
				await EnsureAthleteAuthenticated();
				if(App.CurrentAthlete != null)
				{
					await Navigation.PushAsync(new AthleteProfilePage(App.CurrentAthlete.Id));
				}
			};
		}

		protected async override void OnLoaded()
		{
			base.OnLoaded();

			await Task.Delay(500);
			label1.ScaleTo(1, 500, Easing.SinIn);

			await Task.Delay(250);
			label2.ScaleTo(1, 500, Easing.SinIn);

			await Task.Delay(250);
			btnLogIn.ScaleTo(1, 500, Easing.SinIn);
		}
	}

	public partial class WelcomeStartPageXaml : BaseContentPage<AuthenticationViewModel>
	{
	}
}