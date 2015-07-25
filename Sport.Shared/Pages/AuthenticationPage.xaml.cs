﻿using System;
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
	}

	public partial class AuthenticationPageXaml : BaseContentPage<AuthenticationViewModel>
	{
	}
}