using Xamarin.Forms;
using System.Threading.Tasks;
using Connectivity.Plugin;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Xamarin;

namespace Sport.Shared
{
	public class BaseContentPage<T> : MainBaseContentPage where T : BaseViewModel, new()
	{
		protected T _viewModel;

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

	public class MainBaseContentPage : ContentPage
	{
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

		public MainBaseContentPage()
		{
			BarBackgroundColor = (Color)App.Current.Resources["grayPrimary"];
			BarTextColor = Color.White;
			BackgroundColor = Color.White;
			SubscribeToAuthentication();
			SubscribeToIncomingPayload();
		}

		~MainBaseContentPage()
		{
			#if DEBUG
			Debug.WriteLine("Destructor called for {0}".Fmt(GetType().Name));
			#endif
		}

		void SubscribeToIncomingPayload()
		{
			var self = new WeakReference<MainBaseContentPage>(this);
			Action<App, NotificationPayload> action = (app, payload) =>
			{
				MainBaseContentPage v;
				if(!self.TryGetTarget(out v))
					return;

				v.OnIncomingPayload(payload);
			};
			MessagingCenter.Subscribe<App, NotificationPayload>(this, "IncomingPayloadReceived", action);
		}

		void SubscribeToAuthentication()
		{
			var self = new WeakReference<MainBaseContentPage>(this);
			Action<AuthenticationViewModel> action = (vm) =>
			{
				MainBaseContentPage v;
				if(!self.TryGetTarget(out v))
					return;

				v.OnAuthenticated();
			};
			MessagingCenter.Subscribe<AuthenticationViewModel>(this, "UserAuthenticated", action);
		}

		public bool HasInitialized
		{
			get;
			private set;
		}

		protected virtual void OnLoaded()
		{
			TrackPage(new Dictionary<string, string>());
		}

		internal virtual void OnUserAuthenticated()
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

			base.OnAppearing();
		}

		void OnAuthenticated()
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

		protected virtual void TrackPage(Dictionary<string, string> metadata)
		{
			var identifier = GetType().Name;
			Insights.Track(identifier, metadata);
		}

		async protected virtual void OnIncomingPayload(NotificationPayload payload)
		{
		}

		#region Authentication

		public async Task EnsureUserAuthenticated()
		{
			if(App.CurrentAthlete != null)
				return;
			
			var authViewModel = DependencyService.Get<AuthenticationViewModel>();
			if(Settings.Instance.AuthToken != null)
			{
				var authPage = new AuthenticationPage();
				await Navigation.PushModalAsync(authPage);
				await authPage.AttemptToAuthenticateAthlete();

				if(App.CurrentAthlete != null)
					Navigation.PopModalAsync();
			}
			else
			{
				await authViewModel.GetUserProfile(true);

				if(App.AuthUserProfile != null)
					await authViewModel.EnsureAthleteRegistered();
			}
		}

		async protected void LogoutUser()
		{
			var decline = await DisplayAlert("Really?", "Are you sure you want to log out?", "Yes", "No");

			if(!decline)
				return;

			var authViewModel = DependencyService.Get<AuthenticationViewModel>();
			authViewModel.LogOut();

			App.Current.SetToWelcomePage(); 
		}

		#endregion
	}
}