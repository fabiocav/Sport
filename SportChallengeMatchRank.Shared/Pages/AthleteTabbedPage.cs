using System;

using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
{
	public class AthleteTabbedPage : TabbedPage
	{
		public AthleteTabbedPage()
		{
			Children.Add(new AthleteLeaguesPage {
					Title = "Leagues"
				});
			Children.Add(new AthleteChallengesPage());
			Children.Add(new AdminPage());
		}
	}
}