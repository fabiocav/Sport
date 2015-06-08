using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using Connectivity.Plugin;

namespace SportChallengeMatchRank.Shared
{
	public partial class AuthenticationPage : AuthenticationPageXaml
	{
		public AuthenticationPage()
		{
			Initialize();
		}

		protected override void Initialize()
		{
			InitializeComponent();
			Title = "Authenticating";

			btnAuthenticate.Clicked += (sender, e) =>
			{
				AttemptToAuthenticateAthlete();
			};

//			CrossConnectivity.Current.ConnectivityChanged += (sender, args) =>
//			{
//				
//			};
		}
	}

	public partial class AuthenticationPageXaml : BaseContentPage<AuthenticationViewModel>
	{
	}
}