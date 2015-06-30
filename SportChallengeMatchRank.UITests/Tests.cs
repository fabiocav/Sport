using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.TestCloud.Extensions;
using System.Threading;
using System;

namespace SportChallengeMatchRank.UITests
{
	[TestFixture(Platform.Android)]
	[TestFixture(Platform.iOS)]
	public class Tests
	{
		IApp app;
		Platform platform;

		public Tests(Platform platform)
		{
			this.platform = platform;
		}

		[SetUp]
		public void BeforeEachTest()
		{
			app = AppInitializer.StartApp(platform);
		}

		[Test]
		public void JoinLeagueAndChallenge()
		{
			app.Tap("authButton");
			app.EnterText(e => e.Css("#Email"), "rob.testcloud@gmail.com");

			if(app.Query(e => e.Css("#next")).Length > 0)
				app.Tap(e => e.Css("#next"));

			app.EnterText(e => e.Css("#Passwd"), Constants.Password, "and I enter my credentials");
			app.ScrollDownAndTap(e => e.Css("#signIn"), "And I click the Sign In button");

			if(app.Query(e => e.Button("Remember")).Length > 0)
			{
				app.Back();
			}

			app.WaitForElement(e => e.Css("#grant_heading"));
			app.ScrollDownAndTap("Then I can continue", e => e.Css("#submit_approve_access"));

			app.WaitForElement(e => e.Marked("aliasText"), "Timed out waiting for aliasText", TimeSpan.FromMinutes(2));
			app.ClearText(e => e.Marked("aliasText"));
			app.EnterText(e => e.Marked("aliasText"), "XTC", "And I enter my alias");
			app.PressEnter();

			int count = 0;
			while(app.Query("saveButton").Length == 0 && count < 5)
			{
				app.ScrollDown();
				count++;
			}
			
			app.Tap("saveButton");

			Thread.Sleep(5000);
			app.ScrollDownAndTap("Continue button", e => e.Marked("continueButton"));

			/*
			Thread.Sleep(5000);
			if(platform == Platform.Android)
				app.Tap("NoResourceEntry-0");

			if(platform == Platform.iOS)
				app.Tap("ic_person_add_white");

			app.Screenshot("Then I should see a list of leagues to join");

			app.ScrollDownAndTap(e => e.Marked("League for XTC tests"), "Then I select the 'League for XTC tests'");
			app.ScrollDownAndTap("And I tap the JOIN button", e => e.Marked("joinButton"));
			app.Tap("Done");
			*/

			app.Screenshot("Athlete leagues listview");

			app.Tap("leagueRow");

			app.WaitForElement("leaguePhoto");
			app.Screenshot("Then I should see the league details");
			app.ScrollDownEnough(e => e.Marked("abandonButton"), "Bottom of list");

			app.Tap("leaderboardButton");

			app.WaitForElement("memberItemRoot");
			app.Screenshot("Leaderboard listview");

			app.Tap(e => e.Marked("memberItemRoot").Index(1));
			app.WaitForElement("memberDetailsRoot");
			app.Screenshot("Member details page");

			app.ScrollDownEnough(e => e.Marked("pastButton"), "Bottom of member details page");

			app.Tap("challengeButton");
			app.Screenshot("Challenge date page");

			app.Tap("datePicker");
			app.Screenshot("Challenge date picker");

			if(platform == Platform.Android)
				app.Back();
			else
				app.Tap("Done");

			app.Screenshot("End");

			app.Tap("timePicker");
			app.Screenshot("Challenge time picker");

			if(platform == Platform.Android)
				app.Back();
			else
				app.Tap("Done");

			app.Tap("Cancel");

			if(platform == Platform.Android)
			{
				app.Screenshot("Back");
				app.Back();
				app.Screenshot("Back");
				app.Back();
			}

			if(platform == Platform.iOS)
			{
				app.Screenshot("Back");
				app.Tap("Back");
				app.Screenshot("Back");
				app.Tap("Back");
			}

			app.Screenshot("Back");
			app.Tap("abandonButton");
			app.Screenshot("Confirm");
			app.Tap("No");

			app.Screenshot("End");
		}
	}
}

