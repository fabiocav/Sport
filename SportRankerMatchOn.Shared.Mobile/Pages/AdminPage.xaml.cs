using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SportRankerMatchOn.Shared.Mobile
{
	public partial class AdminPage : AdminPageXaml
	{
		public AdminPage()
		{
			InitializeComponent();
			Title = "Admin";
		}
	}

	public partial class AdminPageXaml : BaseContentPage<AdminViewModel>
	{
		
	}
}

