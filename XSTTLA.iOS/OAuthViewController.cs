using System;
using UIKit;
using Auth0.SDK;
using CoreGraphics;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace XSTTLA.iOS
{
	public class OAuthViewController : UIViewController
	{
		UILabel _label;
		Auth0Client _auth;

		async public override void ViewDidLoad()
		{
			_auth = new Auth0Client("xsttla.auth0.com", "5Qf0iIssIZ7Km9Fiwd041uxbfVdtyAqP");
			_label = new UILabel(View.Frame);
			_label.TextAlignment = UITextAlignment.Center;

			var btn = new UIButton(UIButtonType.RoundedRect);
			btn.Frame = new CGRect(0, 500, 400, 40);
			btn.SetTitle("log out", UIControlState.Normal);

			Add(_label);
			Add(btn);

			btn.TouchUpInside += (sender, e) =>
			{
				_auth.Logout();
				AuthenticateIfNeeded();

			};

			View.BackgroundColor = UIColor.White;
			base.ViewDidLoad();

			try
			{
				var j = await _auth.RefreshToken("fACPsQQnSSudTv2e27cROQbNrbhaQhV79z7PF16B7jmy6");
				var k = await _auth.RenewIdToken();
				Console.Write(j);
				Console.Write(k);

				var client = new HttpClient();
				client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJodHRwczovL3hzdHRsYS5hdXRoMC5jb20vIiwic3ViIjoiZ29vZ2xlLW9hdXRoMnwxMTYzNjUzNjI4NjIzNjU5NjA2OTIiLCJhdWQiOiI1UWYwaUlzc0laN0ttOUZpd2QwNDF1eGJmVmR0eUFxUCIsImV4cCI6MTQyNjM5NDc3NywiaWF0IjoxNDI2MzU4Nzc3fQ.3eDirbLxCwRwefVGExL5PjP5Ma8f93Glgem_fGh2kNA");
				var json = await client.GetStringAsync("https://xsttla.auth0.com/api/users/google-oauth2|116365362862365960692");

				if(json != null)
				{
					_auth.CurrentUser.Profile = JObject.Parse(json);
				}
				Console.WriteLine(json);
				AuthenticateIfNeeded();
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}
		}

		async public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
		}

		async void AuthenticateIfNeeded()
		{
			try
			{
				if(_auth.CurrentUser == null || _auth.CurrentUser.Profile == null || _auth.HasTokenExpired())
				{
					var user = await _auth.LoginAsync(this, "google-oauth2", true);
					Console.WriteLine(_auth.CurrentUser.IdToken);
					Console.WriteLine(_auth.CurrentUser.RefreshToken);
					Console.WriteLine(user.Profile);
				}

				if(_auth.CurrentUser != null)
				{
					_label.Text = _auth.CurrentUser.Profile["email"].ToString();
				}
			}
			catch(Exception e)
			{
				_label.Text = string.Empty;
			}
		}
	}
}

