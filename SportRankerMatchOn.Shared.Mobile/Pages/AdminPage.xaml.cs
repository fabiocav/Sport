﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Windows.Input;

namespace SportRankerMatchOn.Shared.Mobile
{
	public partial class AdminPage : AdminPageXaml
	{
		public AdminPage()
		{
			InitializeComponent();
			Title = "Admin";
		}

		protected override void OnAppearing()
		{
			btnAddLeague.Clicked += (sender, e) =>
			{
				Navigation.PushModalAsync(new NewLeaguePage());	
			};

			base.OnAppearing();
		}
	}

	public partial class AdminPageXaml : BaseContentPage<AdminViewModel>
	{
	}
}