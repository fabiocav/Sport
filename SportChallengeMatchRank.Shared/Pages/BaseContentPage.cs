using Xamarin.Forms;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace SportChallengeMatchRank.Shared
{
	public class BaseContentPage<T> : ContentPage where T : BaseViewModel, new()
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

		public BaseContentPage()
		{
			BarBackgroundColor = (Color)App.Current.Resources["grayPrimary"];
			BarTextColor = Color.White;

			BindingContext = ViewModel;
			BackgroundColor = Color.White;
			MessagingCenter.Subscribe<App, NotificationPayload>(this, "IncomingPayloadReceived", OnIncomingPayload);
			MessagingCenter.Subscribe<AuthenticationViewModel>(this, "UserAuthenticated", (viewModel) =>
			{
				if(App.CurrentAthlete != null)
					OnUserAuthenticated();
			});
		}

		T _viewModel;

		public bool HasInitialized
		{
			get;
			private set;
		}

		public T ViewModel
		{
			get
			{
				return _viewModel ?? (_viewModel = DependencyService.Get<T>());
			}
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

			base.OnAppearing();
		}

		public NavigationPage GetNavigationPage()
		{
			var nav = new NavigationPage(this);
			this.ApplyTheme(nav);
			return nav;
		}

		public void ApplyTheme(NavigationPage nav)
		{
			nav.BarBackgroundColor = BarBackgroundColor;
			nav.BarTextColor = BarTextColor;
		}

		protected void AddDoneButton()
		{
			var btnDone = new ToolbarItem {
				Text = "Done",
			};

			btnDone.Clicked += async(sender, e) =>
			{
				await Navigation.PopModalAsync();		
			};

			ToolbarItems.Add(btnDone);			
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
			AuthenticationViewModel.OnDisplayAuthForm = (url) => Device.BeginInvokeOnMainThread(() =>
			{
				App.Current.Hud.Dismiss();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      
				var webView = new WebView {
					Source = url			
				};

				var page = new ContentPage {
					Content = webView
				};

				Navigation.PushModalAsync(page);
			});

			AuthenticationViewModel.OnHideAuthForm = async() =>
			{
				await Navigation.PopModalAsync();
			};

			await AuthenticationViewModel.GetUserProfile(force);
			if(App.AuthUserProfile != null)
			{
				await AuthenticationViewModel.EnsureAthleteRegistered();
			}

			return true;
		}

		#endregion
	}
}