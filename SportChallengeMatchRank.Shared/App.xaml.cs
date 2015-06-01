using System;
using System.Collections.Generic;
using Connectivity.Plugin;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
{
	public partial class App : Application
	{
		public IHUDProvider _hud;
		public static int AnimationSpeed = 250;

		public IHUDProvider Hud
		{
			get
			{
				return _hud ?? (_hud = DependencyService.Get<IHUDProvider>());
			}
		}

		public static App Current
		{
			get
			{
				return (App)Application.Current;
			}
		}

		public NavigationPage WelcomeNav
		{
			get;
			set;
		}

		public App()
		{
			InitializeComponent();
			IsNetworkRechable = true;

			CrossConnectivity.Current.ConnectivityChanged += (sender, args) =>
			{
				if(IsNetworkRechable == args.IsConnected)
					return;
					
				IsNetworkRechable = args.IsConnected;
				"Connectivity is now {0}connected".Fmt(!args.IsConnected ? "dis" : "")
						.ToToast(args.IsConnected ? ToastNotificationType.Info : ToastNotificationType.Warning, "Connectivity changed");
			};

			MessagingCenter.Subscribe<BaseViewModel, Exception>(this, "ExceptionOccurred", (viewModel, exception) =>
			{
				Device.BeginInvokeOnMainThread(async() =>
				{
					try
					{
						if(_hud != null)
						{
							_hud.Dismiss();
						}

						var msg = exception.Message;
						var mse = exception as MobileServiceInvalidOperationException;

						if(mse != null)
						{
							var body = await mse.Response.Content.ReadAsStringAsync();
							var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);
							var error = dict["message"].ToString();
							error.ToToast(ToastNotificationType.Warning, "Doh!");
							return;
						}

						if(msg.Length > 300)
							msg = msg.Substring(0, 300);

						msg.ToToast(ToastNotificationType.Error, "Something bad happened");
					}
					catch(Exception e)
					{
						Console.WriteLine(e);
					}
				});
			});

			if(Settings.Instance.AuthToken != null && Settings.Instance.RegistrationComplete)
			{
				MainPage = GetAthleteLeaguesPage();
			}
			else
			{
				WelcomeNav = new NavigationPage(new WelcomeStartPage());
				WelcomeNav.BarTextColor = Color.White;
				MainPage = WelcomeNav;
			}

			#if __IOS__
			object obj;
			if(Resources.TryGetValue("buttonStyle", out obj))
			{
				var style = obj as Style;
				if(style != null)
				{
					style.Setters.Add(VisualElement.WidthRequestProperty, 130);
				}
			}
			#endif
		}

		public static Athlete CurrentAthlete
		{
			get
			{
				return Settings.Instance.AthleteId == null ? null : DataManager.Instance.Athletes.Get(Settings.Instance.AthleteId);
			}
		}

		public static string DeviceToken
		{
			get;
			set;
		}

		public static UserProfile AuthUserProfile
		{
			get;
			set;
		}

		public static bool IsNetworkRechable
		{
			get;
			set;
		}

		public NavigationPage GetAthleteLeaguesPage()
		{
			return new NavigationPage(new AthleteLeaguesPage(Settings.Instance.AthleteId)) {
				Title = "Leagues",
				BarBackgroundColor = (Color)App.Current.Resources["greenPrimary"],
				BarTextColor = Color.White,
			};
		}
	}
}