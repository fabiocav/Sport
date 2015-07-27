using Xamarin.Forms;
using System.Threading.Tasks;
using Connectivity.Plugin;

namespace Sport.Shared
{
	public class BaseContentPage<T> : SuperBaseContentPage where T : BaseViewModel, new()
	{
		protected T _viewModel;

		public T ViewModel
		{
			get
			{
				return _viewModel ?? (_viewModel = new T());
			}
		}

		public BaseContentPage()
		{
			BindingContext = ViewModel;
		}
	}

	public class SuperBaseContentPage : ContentPage
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

		public SuperBaseContentPage()
		{
			BarBackgroundColor = (Color)App.Current.Resources["grayPrimary"];
			BarTextColor = Color.White;

			BackgroundColor = Color.White;
			MessagingCenter.Subscribe<AuthenticationViewModel>(this, "UserAuthenticated", OnAuthenticated);
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
				{
					await Navigation.PopModalAsync();
				}
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

		async protected virtual void OnIncomingPayload(App app, NotificationPayload payload)
		{
		}

		#endregion
	}
}