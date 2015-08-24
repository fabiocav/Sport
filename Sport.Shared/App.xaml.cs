using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Connectivity.Plugin;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin;
using System.Threading.Tasks;

namespace Sport.Shared
{
	public partial class App : Application
	{
		#region Properties

		public IHUDProvider _hud;
		NotificationPayload _shelvedPayload;
		public static int AnimationSpeed = 250;

		public static string AuthToken
		{
			get;
			set;
		}

		public IHUDProvider Hud
		{
			get
			{
				return _hud ?? (_hud = DependencyService.Get<IHUDProvider>());
			}
		}

		public new static App Current
		{
			get
			{
				return (App)Application.Current;
			}
		}

		public static Athlete CurrentAthlete
		{
			get
			{
				return Settings.Instance.AthleteId == null ? null : DataManager.Instance.Athletes.Get(Settings.Instance.AthleteId);
			}
		}

		public static bool IsNetworkRechable
		{
			get;
			set;
		}

		public static List<string> PraisePhrases
		{
			get;
			set;
		}

		public static List<string> AvailableLeagueColors
		{
			get;
			set;
		}

		public Dictionary<string, string> UsedLeagueColors
		{
			get;
			set;
		} = new Dictionary<string, string>();

		#endregion

		#region Methods

		public App()
		{
			#region Linker

			//			if(false)
			//			{
			//				new WelcomeStartPageXaml();
			//				new SetAliasPageXaml();
			//				new EnablePushPageXaml();
			//			}

			#endregion

			SetDefaultPropertyValues();

			InitializeComponent();
			MessagingCenter.Subscribe<BaseViewModel, Exception>(this, "ExceptionOccurred", OnAppExceptionOccurred);
			IsNetworkRechable = CrossConnectivity.Current.IsConnected;

			CrossConnectivity.Current.ConnectivityChanged += (sender, args) =>
			{
				IsNetworkRechable = args.IsConnected;
			};

			if(Settings.Instance.AthleteId == null || !Settings.Instance.RegistrationComplete)
			{
				StartRegistrationFlow();
			}
			else
			{
				StartAuthenticationFlow();
			}

			MessagingCenter.Subscribe<App, NotificationPayload>(this, "IncomingPayloadReceived", (sender, payload) => OnIncomingPayload(payload));
			MessagingCenter.Subscribe<App>(this, "AuthenticationComplete", (sender) => OnAuthenticationComplete());
		}

		/// <summary>
		/// Kicks off the main application flow - this is the typical route taken once a user is registered
		/// </summary>
		void StartAuthenticationFlow()
		{
			//Create our entry page and add it to a NavigationPage, then apply a randomly assigned color theme
			var page = new AthleteLeaguesPage();
			var navPage = new ThemedNavigationPage(page);
			page.ApplyTheme(navPage);

			MainPage = navPage;
		}

		/// <summary>
		/// Kicks off the registration flow so the user can register and authenticate
		/// </summary>
		internal void StartRegistrationFlow()
		{
			MainPage = new WelcomeStartPage().GetNavigationPage();
		}

		/// <summary>
		/// All application exceptions should be routed through this method so they get process/displayed to the user in a consistent manner
		/// </summary>
		void OnAppExceptionOccurred(BaseViewModel viewModel, Exception exception)
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

						if(dict != null && dict.ContainsKey("message"))
						{
							var error = dict["message"].ToString();
							error.ToToast(ToastNotificationType.Warning, "Doh!");
						}
						return;
					}

					if(msg.Length > 300)
						msg = msg.Substring(0, 300);

					msg.ToToast(ToastNotificationType.Error, "Something bad happened");
				}
				catch(Exception e)
				{
					Debug.WriteLine(e);
				}
			});
		}

		internal async Task OnIncomingPayload(NotificationPayload payload)
		{
			if(payload == null)
				return;

			if(App.CurrentAthlete == null)
			{
				_shelvedPayload = payload;
				return;
			}

			string challengeId;
			if(payload.Payload.TryGetValue("challengeId", out challengeId))
			{
				try
				{
					var vm = new BaseViewModel();
					var task = AzureService.Instance.GetChallengeById(challengeId);
					await vm.RunSafe(task);
					var details = new ChallengeDetailsPage(task.Result);
					details.AddDoneButton();
		
					await App.Current.MainPage.Navigation.PushModalAsync(details.GetNavigationPage());
				}
				catch(Exception e)
				{
					Insights.Report(e);
					Console.WriteLine(e);
				}
			}
		}

		async void OnAuthenticationComplete()
		{
			await OnIncomingPayload(_shelvedPayload);

			_shelvedPayload = null;
		}

		#region Theme

		/// <summary>
		/// Assigns a league a randomly-chosen theme from an existing finite list
		/// </summary>
		/// <returns>The theme.</returns>
		public ColorTheme GetTheme(League league)
		{
			if(league.Id == null)
				return null;

			league.Theme = null;
			var remaining = App.AvailableLeagueColors.Except(App.Current.UsedLeagueColors.Values).ToList();
			if(remaining.Count == 0)
				remaining.AddRange(App.AvailableLeagueColors);

			var random = new Random().Next(0, remaining.Count - 1);
			var color = remaining[random];

			if(App.Current.UsedLeagueColors.ContainsKey(league.Id))
			{
				color = App.Current.UsedLeagueColors[league.Id];
			}
			else
			{
				App.Current.UsedLeagueColors.Add(league.Id, color);
			}

			var theme = GetThemeFromColor(color);

			if(App.Current.Resources.ContainsKey("{0}Medium".Fmt(color)))
				theme.Medium = (Color)App.Current.Resources["{0}Medium".Fmt(color)];

			return theme;
		}

		public ColorTheme GetThemeFromColor(string color)
		{
			return new ColorTheme {
				Primary = (Color)App.Current.Resources["{0}Primary".Fmt(color)],
				Light = (Color)App.Current.Resources["{0}Light".Fmt(color)],
				Dark = (Color)App.Current.Resources["{0}Dark".Fmt(color)],
			};
		}

		void SetDefaultPropertyValues()
		{
			AvailableLeagueColors = new List<string> {
				"green",
				"blue",
				"red",
				"yellow",
				"asphalt",
				"purple"
			};

			PraisePhrases = new List<string> {
				"sensational",
				"crazmazing",
				"stellar",
				"ill",
				"peachy keen",
				"the bees knees",
				"the cat's pajamas",
				"the coolest kid in the cave",
				"killer",
				"aces",
				"wicked awesome",
				"kinda terrific",
				"top notch",
				"impressive",
				"legit",
				"nifty",
				"spectaculawesome",
				"supernacular",
				"bad to the bone",
				"radical",
				"neat",
				"a hoss boss",
				"mad chill",
				"super chill",
				"a beast",
				"funky fresh",
				"slammin it",
			};
		}

		#endregion

		#endregion
	}

	#region LeagueTheme

	public class ColorTheme
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

	#endregion
}