using Xamarin.Forms;
using System.Threading.Tasks;
using System;

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

		public BaseContentPage()
		{
			BindingContext = ViewModel;

			MessagingCenter.Subscribe<AuthenticationViewModel>(this, "UserAuthenticated", (viewModel) =>
			{
				if(App.CurrentAthlete != null)
					OnUserAuthenticated();
			});

			AuthenticationViewModel.SubscribeToProperty("AuthenticationStatus", () =>
			{
				Console.WriteLine(AuthenticationViewModel.AuthenticationStatus);
				Device.BeginInvokeOnMainThread(() =>
				{
//					App.Current.Hud.DisplayProgress(AuthenticationViewModel.AuthenticationStatus);
				});
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
			if(!HasInitialized)
			{
				HasInitialized = true;
				OnLoaded();
			}

			base.OnAppearing();
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
				//AuthenticationViewModel.IsBusy = true;
				await AttemptToAuthenticateAthlete(force);

				//AuthenticationViewModel.IsBusy = false;
			}

			if(showHud)
				App.Current.Hud.Dismiss();
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