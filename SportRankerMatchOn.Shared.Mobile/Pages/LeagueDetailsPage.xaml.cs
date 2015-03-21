using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Windows.Input;
using System.Threading.Tasks;

namespace SportRankerMatchOn.Shared.Mobile
{
	public partial class LeagueDetailsPage : LeagueDetailsXaml
	{
		public LeagueDetailsPage()
		{
			InitializeComponent();
			Title = "Admin";
		}
	}

	public partial class LeagueDetailsXaml : BaseContentPage<LeagueDetailsViewModel>
	{
	}
}