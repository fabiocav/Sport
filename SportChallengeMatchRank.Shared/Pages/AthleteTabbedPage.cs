using System;

using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
{
	public class AthleteTabbedPage : TabbedPage
	{
		public AthleteTabbedPage()
		{
			this.Children.Add(new AthleteLeaguesPage());
			this.Children.Add(new AthleteChallengesPage());
		}
	}
}