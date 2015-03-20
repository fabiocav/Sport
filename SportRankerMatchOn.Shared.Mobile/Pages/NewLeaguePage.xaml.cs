using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Windows.Input;
using System.Threading.Tasks;

namespace SportRankerMatchOn.Shared.Mobile
{
	public partial class NewLeaguePage : NewLeaguePageXaml
	{
		public NewLeaguePage()
		{
			InitializeComponent();
			Title = "Admin";
		}
	}

	public partial class NewLeaguePageXaml : BaseContentPage<LeagueDetailsViewModel>
	{
	}
}