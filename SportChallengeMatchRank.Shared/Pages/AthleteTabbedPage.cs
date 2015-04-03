using System;

using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
{
	public class AthleteTabbedPage : TabbedPage
	{
		public AthleteTabbedPage()
		{
			Children.Add(new NavigationPage(new AthleteLeaguesPage()) {
					Title = "My Leagues"
				});
			Children.Add(new NavigationPage(new AthleteChallengesPage()) {
					Title = "My Challenges"
				});
			Children.Add(new NavigationPage(new AdminPage()) {
					Title = "Admin"
				});
		}


		protected override void OnAppearing()
		{
			base.OnAppearing();
		}
	}
}