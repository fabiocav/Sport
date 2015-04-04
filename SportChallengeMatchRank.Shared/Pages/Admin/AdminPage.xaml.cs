using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Windows.Input;

namespace SportChallengeMatchRank.Shared
{
	public partial class AdminPage : AdminPageXaml
	{
		public AdminPage()
		{
			Initialize();
		}

		protected override void Initialize()
		{
			base.Initialize();

			Title = "Admin";
			InitializeComponent();

			btnLeagues.Clicked += async(sender, e) =>
			{
				await Navigation.PushAsync(new LeagueLandingPage());	
			};

			btnAthletes.Clicked += async(sender, e) =>
			{
				await Navigation.PushAsync(new AthleteLandingPage());	
			};
		}

		//		public ICommand OrgSettingsCommand
		//		{
		//			get
		//			{
		//				return new Command(() => Navigation.PushAsync(new AthleteLandingPage()));
		//			}
		//		}
	}

	public partial class AdminPageXaml : BaseContentPage<AdminViewModel>
	{
	}
}