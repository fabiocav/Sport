using Xamarin.Forms;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Connectivity.Plugin;

namespace Sport.Shared
{
	public class BaseContentPage<T> : SuperBaseContentPage where T : BaseViewModel, new()
	{
		T _viewModel;

		public T ViewModel
		{
			get
			{
				return _viewModel ?? (_viewModel = DependencyService.Get<T>());
			}
		}

		public BaseContentPage()
		{
			BindingContext = ViewModel;
		}
	}

	public class SuperBaseContentPage : ContentPage
	{
		public AuthenticationViewModel AuthenticationViewModel
		{
			get
			{
				return DependencyService.Get<AuthenticationViewModel>();
			}
		}

		public Color BarTextColor
		{
			get;
			set;
		}

		public Color BarBackgroundColor
		{
			get;
			set;
		}

		public SuperBaseContentPage()
		{
			BarBackgroundColor = (Color)App.Current.Resources["grayPrimary"];
			BarTextColor = Color.White;

			BackgroundColor = Color.White;
			MessagingCenter.Subscribe<AuthenticationViewModel>(this, "UserAuthenticated", OnAuthenticated);
//			MessagingCenter.Subscribe<App, NotificationPayload>(this, "IncomingPayloadReceived", OnIncomingPayload);
//			MessagingCenter.Subscribe<AuthenticationViewModel>(this, "UserAuthenticated", (viewModel) =>
//			{
//				if(App.CurrentAthlete != null)
//					OnUserAuthenticated();
//			});
		}


		public bool HasInitialized
		{
			get;
			private set;
		}

		protected virtual void OnLoaded()
		{
			
		}

		protected virtual void OnUserAuthenticated()
		{

		}

		protected virtual void Initialize()
		{
		}

		protected override void OnAppearing()
		{
			var nav = this.Parent as NavigationPage;
			if(nav != null)
			{
				nav.BarBackgroundColor = BarBackgroundColor;
				nav.BarTextColor = BarTextColor;
			}

			if(!HasInitialized)
			{
				HasInitialized = true;
				OnLoaded();
			}

			if(!CrossConnectivity.Current.IsConnected)
			{
				
			}

			MessagingCenter.Subscribe<App, NotificationPayload>(this, "IncomingPayloadReceived", OnIncomingPayload);

			base.OnAppearing();
		}

		protected override void OnDisappearing()
		{
			MessagingCenter.Unsubscribe<App, NotificationPayload>(this, "IncomingPayloadReceived");
			//MessagingCenter.Unsubscribe<AuthenticationViewModel>(this, "UserAuthenticated");

			base.OnDisappearing();
		}

		void OnAuthenticated(AuthenticationViewModel viewModel)
		{
			if(App.CurrentAthlete != null)
				OnUserAuthenticated();
		}

		public NavigationPage GetNavigationPage()
		{
			var nav = new ThemedNavigationPage(this);
			ApplyTheme(nav);
			return nav;
		}

		public void ApplyTheme(NavigationPage nav)
		{
			nav.BarBackgroundColor = BarBackgroundColor;
			nav.BarTextColor = BarTextColor;
		}

		public void AddDoneButton(string text = "Done", ContentPage page = null)
		{
			var btnDone = new ToolbarItem {
				Text = text,
			};

			btnDone.Clicked += async(sender, e) =>
			{
				await Navigation.PopModalAsync();		
			};

			page = page ?? this;
			page.ToolbarItems.Add(btnDone);
		}

		#region Authentication

		async public Task EnsureAthleteAuthenticated(bool showHud = false, bool force = false)
		{
			if((App.CurrentAthlete != null) && !force)
				return;

			if(showHud)
				App.Current.Hud.DisplayProgress("Authenticating");

			using(new Busy(AuthenticationViewModel))
			{
				await AttemptToAuthenticateAthlete(force);
			}

			if(showHud)
				App.Current.Hud.Dismiss();
		}

		async protected virtual void OnIncomingPayload(App app, NotificationPayload payload)
		{
		}

		async public Task<bool> AttemptToAuthenticateAthlete(bool force = false)
		{
			await AuthenticationViewModel.GetUserProfile(force);

			if(App.AuthUserProfile != null)
				await AuthenticationViewModel.EnsureAthleteRegistered();

			return Settings.Instance.AuthToken != null;
		}

		#endregion
	}
}