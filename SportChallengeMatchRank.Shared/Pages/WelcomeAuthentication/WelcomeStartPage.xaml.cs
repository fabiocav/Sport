﻿using System;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace SportChallengeMatchRank.Shared
{
	public partial class WelcomeStartPage : WelcomeStartPageXaml
	{
		public WelcomeStartPage()
		{
			Initialize();
		}

		protected override void Initialize()
		{
			BarBackgroundColor = (Color)App.Current.Resources["bluePrimary"];
			BarTextColor = Color.White;

			BackgroundColor = BarBackgroundColor;
			InitializeComponent();
			Title = "Welcome!";

			NavigationPage.SetHasNavigationBar(this, false);

			bool ignoreClicks = false;
			btnAuthenticate.Clicked += async(sender, e) =>
			{
				if(ignoreClicks)
					return;

				ignoreClicks = true;
				await EnsureAthleteAuthenticated();
				if(App.CurrentAthlete != null)
				{
					await label1.FadeTo(0, (uint)App.AnimationSpeed, Easing.SinIn);
					await label2.FadeTo(0, (uint)App.AnimationSpeed, Easing.SinIn);
					await buttonStack.FadeTo(0, (uint)App.AnimationSpeed, Easing.SinIn);
					await Navigation.PushAsync(new SetAliasPage());
				}
				else
				{
					
				}
				ignoreClicks = false;
			};
		}

		protected async override void OnLoaded()
		{
			base.OnLoaded();

			await Task.Delay(300);
			await label1.ScaleTo(1, (uint)App.AnimationSpeed, Easing.SinIn);
			await label2.ScaleTo(1, (uint)App.AnimationSpeed, Easing.SinIn);
			await buttonStack.ScaleTo(1, (uint)App.AnimationSpeed, Easing.SinIn);
		}
	}

	public partial class WelcomeStartPageXaml : BaseContentPage<AuthenticationViewModel>
	{
	}
}