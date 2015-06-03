using System;
using System.Collections.Generic;
using Connectivity.Plugin;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using Xamarin.Forms;
using System.Linq;

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
			Colors = new List<string> {
					"green",
					"blue",
					"red",
					"yellow",
					"asphalt",
					"purple"
			};
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
				MainPage = new AthleteLeaguesPage().GetNavigationPage();
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

		static List<string> Colors
		{
			get;
			set;
		}

		public void GetTheme(League league)
		{
			if(league.Theme != null)
				return;
			
			var remaining = App.Colors.Except(Settings.Instance.LeagueColors.Values).ToList();
			if(remaining.Count == 0)
				remaining.AddRange(App.Colors);

			var color = remaining[new Random().Next(0, remaining.Count)];
			if(Settings.Instance.LeagueColors.ContainsKey(league.Id))
			{
				color = Settings.Instance.LeagueColors[league.Id];
			}
			else
			{
				Settings.Instance.LeagueColors.Add(league.Id, color);
			}

			var theme = new LeagueTheme {
				Primary = (Color)App.Current.Resources["{0}Primary".Fmt(color)],
				Light = (Color)App.Current.Resources["{0}Light".Fmt(color)],
				Dark = (Color)App.Current.Resources["{0}Dark".Fmt(color)],
			};

			if(App.Current.Resources.ContainsKey("{0}Medium".Fmt(color)))
				theme.Medium = (Color)App.Current.Resources["{0}Medium".Fmt(color)];

			league.Theme = theme;
		}
	}

	public class LeagueTheme
	{
		public Color Primary
		{
			get;
			set;
		}

		public Color Light
		{
			get;
			set;
		}

		public Color Dark
		{
			get;
			set;
		}

		public Color Medium
		{
			get;
			set;
		}

		public Color PrimaryText
		{
			get;
			set;
		}
	}
}