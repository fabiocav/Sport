using System;
using Xamarin.Forms;
using Connectivity.Plugin;
using Toasts.Forms.Plugin.Abstractions;

namespace SportChallengeMatchRank.Shared
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();
			IsNetworkRechable = true;
			MainPage = new AthleteTabbedPage();

			CrossConnectivity.Current.ConnectivityChanged += (sender, args) =>
			{
				if(IsNetworkRechable == args.IsConnected)
					return;
					
				IsNetworkRechable = args.IsConnected;
				"Connectivity is now {0}connected".Fmt(!args.IsConnected ? "dis" : "")
						.ToToast(args.IsConnected ? ToastNotificationType.Info : ToastNotificationType.Warning, "Connectivity changed");
			};
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