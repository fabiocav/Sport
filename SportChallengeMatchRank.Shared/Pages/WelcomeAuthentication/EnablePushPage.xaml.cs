using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
{
	public partial class EnablePushPage : EnablePushPageXaml
	{
		public Action OnSave
		{
			get;
			set;
		}

		public EnablePushPage()
		{
			NavigationPage.SetHasNavigationBar(this, false);
			Initialize();
		}

		protected override void Initialize()
		{
			BarBackgroundColor = (Color)App.Current.Resources["purplePrimary"];
			BarTextColor = Color.White;

			BackgroundColor = BarBackgroundColor;
			InitializeComponent();
			Title = "Enable Push";

			profileStack.Opacity = 0;
			MessagingCenter.Subscribe<App>(this, "RegisteredForRemoteNotifications", (app) =>
			{
				Device.BeginInvokeOnMainThread(() =>
				{
					MessagingCenter.Unsubscribe<App>(this, "RegisteredForRemoteNotifications");
					RegisteredForPushNotificationSuccess();
				});
			});

			var ignoreClicks = false;
			btnPush.Clicked += async(sender, e) =>
			{
				if(ignoreClicks)
					return;

				ignoreClicks = true;

				#if !DEBUG
				var push = DependencyService.Get<IPushNotifications>();
				if(!push.IsRegistered)
				{
					push.RegisterForPushNotifications();
				}
				#else
				await Task.Delay(1000);
				btnPush.Text = "Thanks! We'll be in touch";
				await Task.Delay(1000);

				await profileStack.LayoutTo(new Rectangle(0, Content.Width * -1, profileStack.Width, profileStack.Height), (uint)App.AnimationSpeed, Easing.SinIn);
				await label1.FadeTo(0, (uint)App.AnimationSpeed, Easing.SinIn);
				await btnPush.LayoutTo(new Rectangle(Content.Width, btnPush.Bounds.Y, btnPush.Bounds.Width, btnPush.Height), (uint)App.AnimationSpeed, Easing.SinIn);
				await btnContinue.LayoutTo(new Rectangle(Content.Width, btnContinue.Bounds.Y, btnContinue.Width, btnPush.Height), (uint)App.AnimationSpeed, Easing.SinIn);
				MoveToMainPage();
				#endif
			};

			btnContinue.Clicked += (sender, e) =>
			{
				MoveToMainPage();
			};
		}

		protected async override void OnLoaded()
		{
			profileStack.Layout(new Rectangle(0, profileStack.Height * -1, profileStack.Width, profileStack.Height));
			base.OnLoaded();

			profileStack.Opacity = 1;
			await Task.Delay(300);
			await profileStack.LayoutTo(new Rectangle(0, 0, profileStack.Width, profileStack.Height), (uint)App.AnimationSpeed, Easing.SinIn);
			await label1.ScaleTo(1, (uint)App.AnimationSpeed, Easing.SinIn);
			await buttonStack.ScaleTo(1, (uint)App.AnimationSpeed, Easing.SinIn);
		}

		async Task RegisteredForPushNotificationSuccess()
		{
			var task = AzureService.Instance.UpdateAthleteNotificationHubRegistration(App.CurrentAthlete);
			await ViewModel.RunSafe(task);

			if(task.IsFaulted)
				return;

			btnPush.Text = "Thanks! We'll be in touch";
			await Task.Delay(600);

			await label1.FadeTo(0, (uint)App.AnimationSpeed, Easing.SinIn);

			btnPush.FadeTo(0, (uint)App.AnimationSpeed, Easing.SinIn);
			await btnPush.LayoutTo(new Rectangle(Content.Width, btnPush.Bounds.Y, btnPush.Bounds.Width, btnPush.Height), (uint)App.AnimationSpeed, Easing.SinIn);

			btnContinue.FadeTo(0, (uint)App.AnimationSpeed, Easing.SinIn);
			await btnContinue.LayoutTo(new Rectangle(Content.Width, btnContinue.Bounds.Y, btnContinue.Bounds.Width, btnContinue.Height), (uint)App.AnimationSpeed, Easing.SinIn);

			MoveToMainPage();
		}

		async void MoveToMainPage()
		{
			Settings.Instance.RegistrationComplete = true;
			Settings.Instance.Save();

			await profileStack.FadeTo(0, (uint)App.AnimationSpeed, Easing.SinIn);
			await label1.FadeTo(0, (uint)App.AnimationSpeed, Easing.SinIn);
			await buttonStack.FadeTo(0, (uint)App.AnimationSpeed, Easing.SinIn);
			var nav = new AthleteLeaguesPage(App.CurrentAthlete.Id).GetNavigationPage();
			nav.BarBackgroundColor = (Color)App.Current.Resources["grayPrimary"];
			nav.BarTextColor = Color.White;

			App.Current.MainPage = nav;
		}
	}

	public partial class EnablePushPageXaml : BaseContentPage<AthleteProfileViewModel>
	{
	}
}