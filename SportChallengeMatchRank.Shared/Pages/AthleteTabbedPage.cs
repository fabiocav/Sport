using System;
using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
{
	public class AthleteTabbedPage : TabbedPage
	{
		public AthleteTabbedPage()
		{
			Children.Add(new NavigationPage(new AthleteLeaguesPage(Settings.Instance.AthleteId)) {
				Title = "My Leagues",
				//BarBackgroundColor = App.BlueColor
			});
			Children.Add(new NavigationPage(new AthleteChallengesPage(Settings.Instance.AthleteId)) {
				Title = "My Challenges",
				//BarBackgroundColor = App.BlueColor
			});
		}
	}
}