using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.TestCloud.Extensions;
using System.Threading;

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
		public void WelcomeTextIsDisplayed()
		{
			app.Tap("When the app starts", e => e.Marked("authButton"));
			app.EnterText(e => e.Css("#Email"), "rob.testcloud@gmail.com");

			app.EnterText(e => e.Css("#Passwd"), "XamarinTestCloud", "and I enter my credentials");
			app.Tap(e => e.Css("#signIn"), "And I click the Sign In button");

			app.WaitForElement(e => e.Css("#submit_approve_access"));
			app.Tap("Then I can continue", e => e.Css("#submit_approve_access"));

			Thread.Sleep(5000);

			app.WaitForElement(e => e.Marked("aliasText"));
			app.ClearText(e => e.Marked("aliasText"));
			app.EnterText(e => e.Marked("aliasText"), "XTC", "And I enter my alias");
			app.PressEnter();
			app.Tap("Then I tap Save button", e => e.Marked("saveButton"));

			Thread.Sleep(3000);
			app.Tap("Continue button", e => e.Marked("continueButton"));
			Thread.Sleep(3000);
			app.Screenshot("Athlete leagues listview");

			app.Tap("leagueRow");

			app.WaitForElement("leaguePhoto");
			app.Screenshot("Then I should see the league details");
			app.ScrollDownEnough(e => e.Marked("abandonButton"), "Bottom of list");

			app.Tap("leaderboardButton");

			app.WaitForElement("memberItemRoot");
			app.Screenshot("Leaderboard listview");

			app.Tap("memberItemRoot");
			app.WaitForElement("memberDetailsRoot");
			app.Screenshot("Member details page");

			app.Tap("challengeButton");
			app.Screenshot("Challenge date page");

			app.Tap("datePicker");
			app.Screenshot("Challenge date picker");

			app.Tap("timePicker");
			app.Screenshot("Challenge time picker");

			app.ScrollDownEnough(e => e.Marked("pastButton"), "Bottom of member details page");
		}
	}
}

