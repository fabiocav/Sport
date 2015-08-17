using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Sport.Shared
{
	public partial class EnablePushPage : EnablePushPageXaml
	{
		//Flag to disable click that will allow the button text to be updated
		bool _ignoreClicks;

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
			InitializeComponent();
			Title = "Enable Push";

			var theme = App.Current.GetThemeFromColor("purple");
			BackgroundColor = theme.Primary;
			profileStack.Opacity = 0;
			profileStack.Theme = theme;

			btnPush.Clicked += (sender, e) =>
			{
				if(_ignoreClicks)
					return;

				_ignoreClicks = true;
				ViewModel.RegisterForPushNotifications(RegisteredForPushNotificationSuccess);
			};

			btnContinue.Clicked += (sender, e) =>
			{
				if(_ignoreClicks)
					return;
				
				_ignoreClicks = true;
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

		void RegisteredForPushNotificationSuccess()
		{
			Device.BeginInvokeOnMainThread(async() =>
			{
				if(App.CurrentAthlete.DeviceToken != null)
				{
					btnPush.Text = "Thanks! We'll be in touch";
					await Task.Delay(App.AnimationSpeed);
					await label1.FadeTo(0, (uint)App.AnimationSpeed, Easing.SinIn);
					await profileStack.LayoutTo(new Rectangle(0, Content.Width * -1, profileStack.Width, profileStack.Height), (uint)App.AnimationSpeed, Easing.SinIn);

					btnPush.FadeTo(0, (uint)App.AnimationSpeed, Easing.SinIn);
					await btnPush.LayoutTo(new Rectangle(Content.Width, btnPush.Bounds.Y, btnPush.Bounds.Width, btnPush.Height), (uint)App.AnimationSpeed, Easing.SinIn);

					btnContinue.FadeTo(0, (uint)App.AnimationSpeed, Easing.SinIn);
					await btnContinue.LayoutTo(new Rectangle(Content.Width, btnContinue.Bounds.Y, btnContinue.Bounds.Width, btnContinue.Height), (uint)App.AnimationSpeed, Easing.SinIn);
					await Task.Delay(1000);
					MoveToMainPage();
				}
				else
				{
					_ignoreClicks = false;
					"Unable to register for push notifications".ToToast();
				}
			});
		}

		async void MoveToMainPage()
		{
			Settings.Instance.RegistrationComplete = true;
			await Settings.Instance.Save();

			var page = new AthleteLeaguesPage(App.CurrentAthlete.Id);
			page.OnUserAuthenticated();

			await Task.Delay(1000);
			await Navigation.PushAsync(page);

			Navigation.RemovePage(Navigation.NavigationStack[0]); //WelcomeStartPage
			Navigation.RemovePage(Navigation.NavigationStack[0]); //SetAliasPage
			Navigation.RemovePage(Navigation.NavigationStack[0]); //EnablePushPage
		}
	}

	public partial class EnablePushPageXaml : BaseContentPage<AthleteProfileViewModel>
	{
	}
}