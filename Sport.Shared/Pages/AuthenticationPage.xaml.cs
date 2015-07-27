using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using Connectivity.Plugin;

namespace Sport.Shared
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
				//AttemptToAuthenticateAthlete();
			};
		}

		async public Task<bool> AttemptToAuthenticateAthlete(bool force = false)
		{
			await ViewModel.GetUserProfile(force);

			if(App.AuthUserProfile != null)
				await ViewModel.EnsureAthleteRegistered();

			return Settings.Instance.AuthToken != null;
		}
	}

	public partial class AuthenticationPageXaml : BaseContentPage<AuthenticationViewModel>
	{
	}
}