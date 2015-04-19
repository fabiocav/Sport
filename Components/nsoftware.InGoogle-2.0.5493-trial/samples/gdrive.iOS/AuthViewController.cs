using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using nsoftware.InGoogle;

namespace nsoftware.GdriveDemo
{
	partial class AuthViewController : UIViewController
	{
		Oauth oauth;
		String authURL;
		public GdriveViewController sendingController;


		public AuthViewController(IntPtr handle) : base(handle)
		{
			oauth = new Oauth(this);

			oauth.OnSSLServerAuthentication += (s, e) =>
			{
				e.Accept = true;
			};
		}

		async public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var g = new UITapGestureRecognizer(() => View.EndEditing(true));
			g.CancelsTouchesInView = false; //for iOS5
			View.AddGestureRecognizer(g);

			try
			{
				oauth.ClientProfile = OauthClientProfiles.cfMobile;
				oauth.ClientId = "236481934978-balamfh77inmje2nfeq3bphdg3udt03t.apps.googleusercontent.com";
				oauth.ClientSecret = "wRYG9TlQZLXnlpUbu-0X_BwF";
				oauth.ServerAuthURL = "https://accounts.google.com/o/oauth2/device/code";
				oauth.ServerTokenURL = "https://accounts.google.com/o/oauth2/token";
				oauth.AuthorizationScope = "https://www.googleapis.com/auth/calendar https://www.googleapis.com/auth/userinfo.email https://www.googleapis.com/auth/userinfo.profile";
				authURL = await oauth.GetAuthorizationURLAsync();

				Console.WriteLine(authURL);
				Console.WriteLine(oauth.Config("DeviceUserCode"));
				txtAuthURL.Text = authURL;
				txtDeviceCode.Text = oauth.Config("DeviceUserCode");


			}
			catch(Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		partial void btnDone_TouchUpInside(UIButton sender)
		{
			try
			{
				sendingController.authToken = oauth.GetAuthorization();
				if(sendingController.authToken != "")
				{
					sendingController.authorizeGdrive();
					DismissViewController(true, null);
				}
				else
					new UIAlertView("Token not retreived!", "Please follow instructions above and/or tap \"Done\" again.", null, "OK", null).Show();
			}
			catch(Exception ex)
			{
				new UIAlertView("Error!", ex.Message, null, "OK", null).Show();
			}
		}
	}
}
