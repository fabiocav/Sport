﻿using NUnit.Framework;
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

			Thread.Sleep(5000);
			app.WaitForElement("authButton");
			app.Tap("When the app starts", "authButton");

			app.WaitForElement(e => e.Css("#Email"), "Timed out waiting for Google Oauth form", TimeSpan.FromSeconds(60));
			app.EnterText(e => e.Css("#Email"), Keys.TestEmail, "And I enter my email address");
			app.DismissKeyboard();

			Thread.Sleep(2000); //Can't wait for element because it will show but is disabled
			if(app.Query(e => e.Css("#next")).Length > 0)
				app.Tap(e => e.Css("#next"));

			Thread.Sleep(5000); //Google seems to be animating their forms now - delay to let it animate
			app.EnterText(e => e.Css("#Passwd"), Keys.TestPassword, "And I enter my super secret password");
			app.DismissKeyboard();

			app.Tap("And I click the Sign In button", e => e.Css("#signIn"));

			Thread.Sleep(2000); //Can't wait here because the dialog is conditional
			if(app.Query(e => e.Button("Remember")).Length > 0)
				app.Back();

			app.WaitForElement(e => e.Css("#grant_heading"));
			app.ScrollDownTo(e => e.Css("#submit_approve_access"));
			app.Tap("And I accept the terms", e => e.Css("#submit_approve_access"));

//			Thread.Sleep(5000);
//			int tries = 0;
//			while(app.Query(e => e.Marked("aliasText")).Length == 0 && tries < 5)
//			{
//				app.ScrollDown();
//				if(app.Query(e => e.Css("#submit_approve_access")).Length > 0)
//				{
//					app.Tap("And I accept the terms", e => e.Css("#submit_approve_access"));
//				}
//
//				Thread.Sleep(10000);
//				tries++;
//			}

			app.WaitForElement(e => e.Marked("authButton"));
			app.Screenshot("Authentication complete!");

			Thread.Sleep(10000);
			app.WaitForElement(e => e.Marked("aliasText"));
			app.ClearText(e => e.Marked("aliasText"));
			app.EnterText(e => e.Marked("aliasText"), "XTC Tester", "And I enter my alias");
			app.DismissKeyboard();

			app.Tap("And I save my profile", e => e.Marked("saveButton"));

			app.WaitForElement("continueButton");
			app.Tap("Continue button", e => e.Marked("continueButton"));

			app.WaitForElement(e => e.Marked("leagueRow"));
			app.Screenshot("Now I should see a list of leagues I have joined");

			//Available leagues
			if(platform == Platform.Android)
				app.Tap("NoResourceEntry-0");
			else if(platform == Platform.iOS)
				app.Tap("ic_add_white");

			//Thread.Sleep(5000);
			app.WaitForElement(e => e.Marked("leagueRow"));

			Thread.Sleep(10000); //Pausing to allow time for images to load
			app.Screenshot("Then I should see a list of leagues to join");

			app.Tap(e => e.Marked("leagueRow").Index(0));
			app.WaitForElement("leaderboardButton");
			app.Screenshot("Then I should see a league I can join");

			app.Back(platform);
			app.Tap("Done");

			app.Screenshot("Athlete leagues listview");
			app.ScrollDownTo("XTC Tests");
			app.Tap("XTC Tests");

			app.WaitForElement("leaguePhoto");
			app.Screenshot("Then I should see the league details");
			app.ScrollDownTo("leaderboardButton");
			app.Tap("leaderboardButton");

			app.WaitForElement("memberItemRoot");
			app.Screenshot("Leaderboard listview");

			var result = app.Query("*You*")[0];
			app.TapCoordinates(result.Rect.X, result.Rect.Y - result.Rect.Height); //Select player above self
			app.WaitForElement("memberDetailsRoot");
			app.Screenshot("Member details page");

			app.ScrollDownTo("pastButton");
			app.Tap("Bottom of member details page", e => e.Marked("pastButton"));

			app.WaitForElement("challengeItemRoot");
			app.Screenshot("Challenge history page");

			Thread.Sleep(10000); //Need to wait for list to load

			if(app.Query("resultItemRoot").Length > 0)
			{
				app.Tap("resultItemRoot");
				app.WaitForElement("challengeRoot");
				app.Screenshot("Challenge result page");

				app.ScrollDownTo("winningLabel");
				app.Screenshot("Challenge result page bottom");
				app.Back(platform);
				app.Tap("Done");
			}
			else
			{
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

			app.ScrollDownTo("sourceButton");
			app.Screenshot("Bottom of About page");

			app.Tap("Done");

			app.Tap(menuButton);
			app.Tap(e => e.Marked("My Profile"), "Profile page");
			app.ScrollTo("saveButton");
			app.Tap("Saving profile", e => e.Marked("saveButton"));

			app.WaitForElement(e => e.Marked("leagueRow"), "Timed out waiting for leagues list", TimeSpan.FromMinutes(2));
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

