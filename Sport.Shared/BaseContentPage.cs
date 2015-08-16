using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin;
using Xamarin.Forms;
using SimpleAuth.Providers;
using SimpleAuth;
using Newtonsoft.Json;

namespace Sport.Shared
{
	/// <summary>
	/// Each ContentPage is required to align with a corresponding ViewModel
	/// ViewModels will be the BindingContext by default
	/// </summary>
	public class BaseContentPage<T> : MainBaseContentPage where T : BaseViewModel, new()
	{
		protected T _viewModel;

		/// <summary>
		/// ViewModels are created once for the lifetime of the app - properties are updates as new pages/data is loaded
		/// </summary>
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
			Debug.WriteLine("Destructor called for {0}".Fmt(GetType().Name));
		}

		void SubscribeToIncomingPayload()
		{
			var weakSelf = new WeakReference(this);
			Action<App, NotificationPayload> action = (app, payload) =>
			{
				var self = (MainBaseContentPage)weakSelf.Target;
				self.OnIncomingPayload(payload);
			};
			MessagingCenter.Subscribe<App, NotificationPayload>(this, "IncomingPayloadReceived", action);
		}

		void SubscribeToAuthentication()
		{
			var weakSelf = new WeakReference(this);
			Action<AuthenticationViewModel> action = (vm) =>
			{
				var self = (MainBaseContentPage)weakSelf.Target;
				self.OnAuthenticated();
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
			var nav = Parent as NavigationPage;
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

		protected void SetTheme(League l)
		{
			if(l == null || l.Theme == null)
				return;
			
			BarBackgroundColor = l.Theme.Light;
			BarTextColor = l.Theme.Dark;
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
			await Navigation.PopModalAsync();

			page = page ?? this;
			page.ToolbarItems.Add(btnDone);
		}

		protected virtual void TrackPage(Dictionary<string, string> metadata)
		{
			var identifier = GetType().Name;
			Insights.Track(identifier, metadata);
		}

		protected virtual void OnIncomingPayload(NotificationPayload payload)
		{
		}

		#region Authentication

		async protected void LogoutUser()
		{
			var decline = await DisplayAlert("For ultra sure?", "Are you sure you want to log out?", "Yes", "No");

			if(!decline)
				return;

			var authViewModel = DependencyService.Get<AuthenticationViewModel>();
			authViewModel.LogOut();

			App.Current.StartRegistrationFlow(); 
		}

		#endregion
	}
}