using System;

using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
{
	public class AthleteTabbedPage : TabbedPage
	{
		bool _hasAttemptedAuthentication = false;

		public AthleteTabbedPage()
		{
			var id = App.CurrentAthlete == null ? null : App.CurrentAthlete.Id;
			Children.Add(new NavigationPage(new AthleteLeaguesPage()) {
				Title = "My Leagues"
			});
			Children.Add(new NavigationPage(new AthleteChallengesPage(id)) {
				Title = "My Challenges"
			});
			Children.Add(new NavigationPage(new AdminPage()) {
				Title = "Admin"
			});

			Application.Current.ModalPopped += (sender, e) =>
			{
				Console.WriteLine("A: " + App.CurrentAthlete);
				if(e.Modal is AuthenticationPage && App.CurrentAthlete != null)
				{
					DependencyService.Get<AthleteChallengesViewModel>().AthleteId = App.CurrentAthlete == null ? null : App.CurrentAthlete.Id;
					DependencyService.Get<AthleteLeaguesViewModel>().AthleteId = App.CurrentAthlete == null ? null : App.CurrentAthlete.Id;
					Console.WriteLine("Authenticated!");
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