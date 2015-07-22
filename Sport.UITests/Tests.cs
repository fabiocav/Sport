using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.TestCloud.Extensions;
using System.Threading;
using System;

namespace Sport.UITests
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
			app.Tap("When the app starts", "authButton");

			app.WaitForElement(e => e.Css("#Email"), "Timed out waiting for Google Oauth form", TimeSpan.FromSeconds(60));
			app.EnterText(e => e.Css("#Email"), "rob.testcloud@gmail.com", "And I enter my email address");
			if(platform == Platform.Android)
				app.Back(); //Dismiss keyboard

			Thread.Sleep(2000);

			if(app.Query(e => e.Css("#next")).Length > 0)
				app.Tap(e => e.Css("#next"));

			app.EnterText(e => e.Css("#Passwd"), Constants.Password, "And I enter my super secret password");
			if(platform == Platform.Android)
				app.Back(); //Dismiss keyboard
			
			app.ScrollDownAndTap(e => e.Css("#signIn"), "And I click the Sign In button");

			if(app.Query(e => e.Button("Remember")).Length > 0)
			{
				app.Back();
			}

			app.WaitForElement(e => e.Css("#grant_heading"));
			app.ScrollDownAndTap("Then I can continue", e => e.Css("#submit_approve_access"));

			app.WaitForElement(e => e.Marked("aliasText"), "Timed out waiting for aliasText", TimeSpan.FromMinutes(2));
			app.ClearText(e => e.Marked("aliasText"));
			app.EnterText(e => e.Marked("aliasText"), "XTC Tester", "And I enter my alias");
			app.PressEnter();

			app.ScrollDownAndTap("saveButton");

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
			app.ScrollDownAndTap("XTC Tests");

			app.WaitForElement("leaguePhoto");
			app.Screenshot("Then I should see the league details");
			app.ScrollDownEnough(e => e.Marked("abandonButton"), "Bottom of list");

			app.Tap("leaderboardButton");

			app.WaitForElement("memberItemRoot");
			app.Screenshot("Leaderboard listview");

			var result = app.Query("*You*")[0];
			app.TapCoordinates(result.Rect.X, result.Rect.Y - result.Rect.Height);
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

