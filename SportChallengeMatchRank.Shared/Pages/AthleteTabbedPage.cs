using System;
using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
{
	public class AthleteTabbedPage : TabbedPage
	{
		public AthleteTabbedPage()
		{
			var id = App.CurrentAthlete == null ? null : App.CurrentAthlete.Id;

			Children.Add(new NavigationPage(new AthleteLeaguesPage()) {
				Title = "My Leagues",
				//BarBackgroundColor = App.BlueColor
			});
			Children.Add(new NavigationPage(new AthleteChallengesPage(id)) {
				Title = "My Challenges",
				//BarBackgroundColor = App.BlueColor
			});
		}
	}
}