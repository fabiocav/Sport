using System;

using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
{
	public class AthleteTabbedPage : TabbedPage
	{
		bool _hasAttemptedAuthentication = false;

		public AthleteTabbedPage()
		{
			Children.Add(new NavigationPage(new AthleteLeaguesPage()) {
					Title = "My Leagues"
				});
			Children.Add(new NavigationPage(new AthleteChallengesPage(App.CurrentAthlete)) {
					Title = "My Challenges"
				});
			Children.Add(new NavigationPage(new AdminPage()) {
					Title = "Admin"
				});

			Application.Current.ModalPopped += (sender, e) =>
			{
				if(e.Modal is AuthenticationPage && App.CurrentAthlete != null)
				{
					DependencyService.Get<AthleteChallengesViewModel>().Athlete = App.CurrentAthlete;
				}
			};
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			EnsureAthleteAuthenticated();
		}

		async public void EnsureAthleteAuthenticated()
		{
			if(App.CurrentAthlete == null && !_hasAttemptedAuthentication)
			{
				_hasAttemptedAuthentication = true;
				await Navigation.PushModalAsync(new AuthenticationPage());
			}
		}
	}
}