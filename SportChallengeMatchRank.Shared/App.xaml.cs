using System;
using Xamarin.Forms;
using Connectivity.Plugin;
using Toasts.Forms.Plugin.Abstractions;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System.Collections.Generic;

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

//			if(Settings.Instance.AuthToken != null)
//			{
//				MainPage = new MasterDetailPage();
//			}
//			else
			{
				WelcomeNav = new ClearNavPage();
				WelcomeNav.Navigation.PushAsync(new WelcomeStartPage());
				WelcomeNav.BarTextColor = Color.FromHex("#FFFFFF");
				MainPage = WelcomeNav;
			}
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
	}
}