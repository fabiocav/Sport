using System;
using System.Threading.Tasks;
using SimpleAuth;
using SimpleAuth.Providers;

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
			await ViewModel.AuthenticateCompletely();
			return App.CurrentAthlete != null;
		}
	}

	public partial class AuthenticationPageXaml : BaseContentPage<AuthenticationViewModel>
	{
	}
}