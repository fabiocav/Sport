using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.TestCloud.Extensions;
using System.Threading;
using System;
using Xamarin.UITest.Queries;
using System.Linq;

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
			Func<AppQuery, AppQuery> menuButton = e => e.Marked("ic_more_vert_white");
			if(platform == Platform.Android)
				menuButton = e => e.Marked("NoResourceEntry-0").Index(app.Query(ee => ee.Marked("NoResourceEntry-0")).Length - 1);

			app.WaitForElement("authButton");
			app.Tap("When the app starts", "authButton");

			app.WaitForElement(e => e.Css("#Email"), "Timed out waiting for Google Oauth form", TimeSpan.FromSeconds(60));
			app.EnterText(e => e.Css("#Email"), Keys.TestEmail, "And I enter my email address");
			app.DismissKeyboard();

			Thread.Sleep(2000);
			if(app.Query(e => e.Css("#next")).Length > 0)
				app.Tap(e => e.Css("#next"));

			app.EnterText(e => e.Css("#Passwd"), Keys.TestPassword, "And I enter my super secret password");
			app.DismissKeyboard();

			app.Tap(e => e.Css("#signIn"), "And I click the Sign In button");

			Thread.Sleep(2000);
			if(app.Query(e => e.Button("Remember")).Length > 0)
				app.Back();

			app.WaitForElement(e => e.Css("#grant_heading"));
			Thread.Sleep(5000);

			int tries = 0;
			while(app.Query(e => e.Marked("aliasText")).Length == 0 && tries < 5)
			{
				app.ScrollDown();
				if(app.Query(e => e.Css("#submit_approve_access")).Length > 0)
				{
					app.Tap(e => e.Css("#submit_approve_access"), "And I accept the terms");
				}

				Thread.Sleep(5000);
				tries++;
			}

			app.WaitForElement(e => e.Marked("aliasText"), "Timed out waiting for aliasText", TimeSpan.FromMinutes(2));
			app.ClearText(e => e.Marked("aliasText"));
			app.EnterText(e => e.Marked("aliasText"), "XTC Tester", "And I enter my alias");
			app.DismissKeyboard();

			app.Tap(e => e.Marked("saveButton"), "And I save my profile");

			Thread.Sleep(2000);
			if(app.Query(e => e.Marked("saveButton")).Length > 0)
				app.Tap(e => e.Marked("saveButton"));
			
			app.Tap("Continue button", e => e.Marked("continueButton"));

			app.Screenshot("Now I should see a list of leagues I have joined");

			//Available leagues
			if(platform == Platform.Android)
				app.Tap("NoResourceEntry-0");
			else if(platform == Platform.iOS)
				app.Tap("ic_add_white");
			
			app.WaitForElement(e => e.Marked("leagueRow"));
			app.Screenshot("Then I should see a list of leagues to join");

			app.Tap(e => e.Marked("leagueRow").Index(0), "Then I should see a league I can join");

			app.WaitForElement("leaderboardButton");
			app.Back(platform);
			app.Tap("Done");

			app.Screenshot("Athlete leagues listview");
			app.ScrollDownAndTap("XTC Tests");

			app.WaitForElement("leaguePhoto");
			app.Screenshot("Then I should see the league details");
			app.ScrollDownEnough(e => e.Marked("leaderboardButton"), "Bottom of list");

			app.Tap("leaderboardButton");

			app.WaitForElement("memberItemRoot");
			app.Screenshot("Leaderboard listview");

			var result = app.Query("*You*")[0];
			app.TapCoordinates(result.Rect.X, result.Rect.Y - result.Rect.Height);
			app.WaitForElement("memberDetailsRoot");
			app.Screenshot("Member details page");

			app.ScrollDownEnough(e => e.Marked("pastButton"), "Bottom of member details page");
			app.Tap("pastButton");

			app.WaitForElement("resultItemRoot");
			app.Screenshot("Challenge history page");

			if(app.Query("resultItemRoot").Length > 0)
			{
				app.Tap("resultItemRoot");
				app.WaitForElement("challengeRoot");
				app.Screenshot("Challenge result page");

//				app.ScrollDownEnough(e => e.Marked("winningLabel"));
//				app.Screenshot("Challenge result page end");

				app.Back(platform);
				app.Tap("Done");
			}

			app.Tap("challengeButton");
			app.Screenshot("Challenge date page");

			app.Tap("datePicker");
			app.Screenshot("Challenge date picker");

			DismissPicker();
			app.Screenshot("End");

			app.Tap("timePicker");
			app.Screenshot("Challenge time picker");

			DismissPicker();
			app.Tap("Cancel");

			app.Back(platform);
			app.Back(platform);

			app.Tap("challengeButton");
			app.Screenshot("Challenge date page");

			app.Tap("datePicker");
			app.Screenshot("Challenge date picker");
			DismissPicker();
			app.Screenshot("End");

			app.Tap("timePicker");
			app.Screenshot("Challenge time picker");
			DismissPicker();
			app.Tap("Cancel");

			app.Screenshot("Back");

			app.Tap(menuButton);
			app.Tap("Cowardly Abandon League");

			app.Screenshot("Confirm");
			app.Tap("No");

			app.Back(platform);
			app.Screenshot("End");

			app.Tap(menuButton);
			app.Screenshot("More options menu");
			app.Tap(e => e.Marked("About"), "About page");

			app.ScrollDownEnough(e => e.Marked("sourceButton"));
			app.Screenshot("Bottom of About page");

			app.Tap("Done");

			app.Tap(menuButton);
			app.Tap(e => e.Marked("My Profile"), "Profile page");
			app.ScrollDownAndTap(e => e.Marked("saveButton"), "Saving profile");

			app.WaitForElement(e => e.Marked("leagueRow"), "Timed out waiting for leagues list", TimeSpan.FromMinutes(2));
			Assert.IsTrue(app.Query(e => e.Marked("leagueRow")).Length > 0);

			app.Screenshot("End of test");
		}

		void DismissPicker()
		{
			if(platform == Platform.Android)
				app.Back();
			else
				app.Tap("Done");
		}
	}
}

