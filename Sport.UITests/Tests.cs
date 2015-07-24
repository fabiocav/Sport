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
			app.WaitForElement("authButton");
			app.Tap("When the app starts", "authButton");

			app.WaitForElement(e => e.Css("#Email"), "Timed out waiting for Google Oauth form", TimeSpan.FromSeconds(60));
			app.EnterText(e => e.Css("#Email"), "rob.testcloud@gmail.com", "And I enter my email address");
			if(platform == Platform.Android && (app.Query(e => e.Css("#signIn")).Length == 0 && app.Query(e => e.Css("#next")).Length == 0))
				app.Back(); //Dismiss keyboard

			Thread.Sleep(2000);

			if(app.Query(e => e.Css("#next")).Length > 0)
				app.Tap(e => e.Css("#next"));

			app.EnterText(e => e.Css("#Passwd"), Constants.Password, "And I enter my super secret password");
			if(platform == Platform.Android && app.Query(e => e.Css("#signIn")).Length == 0)
				app.Back(); //Dismiss keyboard
			
			app.Tap(e => e.Css("#signIn"), "And I click the Sign In button");

			Thread.Sleep(2000);
			if(app.Query(e => e.Button("Remember")).Length > 0)
				app.Back();

			Thread.Sleep(5000);
			app.WaitForElement(e => e.Css("#grant_heading"));
			app.Tap("Then I can continue", e => e.Css("#submit_approve_access"));

			Thread.Sleep(10000);
			int tries = 0;
			while(tries < 10 && app.Query("aliasText").Length == 0)
			{
				if(tries == 0)
					app.LogToDevice(e => e.All());
				
				Thread.Sleep(2000);

				if(app.Query(e => e.Css("#submit_approve_access")).Length > 0)
					app.Tap(e => e.Css("#submit_approve_access"));
				
				tries++;
			}

			app.WaitForElement(e => e.Marked("aliasText"), "Timed out waiting for aliasText", TimeSpan.FromMinutes(2));
			app.ClearText(e => e.Marked("aliasText"));
			app.EnterText(e => e.Marked("aliasText"), "XTC Tester", "And I enter my alias");
			app.PressEnter();

			app.Tap(e => e.Marked("saveButton"), "And I save my profile");
//			Thread.Sleep(300);
//			app.Screenshot("Possible toast error?");

			Thread.Sleep(5000);
			app.Tap("Continue button", "continueButton");

			Thread.Sleep(5000);

			app.Screenshot("Now I should see a list of leagues I have joined");

			//Availalbe leagues
			if(platform == Platform.Android)
				app.Tap("NoResourceEntry-0");

			if(platform == Platform.iOS)
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
			app.Tap("abandonButton");
			app.Screenshot("Confirm");
			app.Tap("No");

			app.Back(platform);
			app.Screenshot("End");
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

