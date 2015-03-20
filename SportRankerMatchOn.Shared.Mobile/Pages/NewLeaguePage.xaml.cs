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

		public ICommand SaveLeagueCommand
		{
			get
			{
				return new Command(async(args) =>
					{
						await SaveLeague();
					});
			}
		}

		async Task SaveLeague()
		{
			await AzureService.Instance.SaveLeague(ViewModel.League);
		}
	}

	public partial class NewLeaguePageXaml : BaseContentPage<NewLeagueViewModel>
	{
	}
}