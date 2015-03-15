using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace XSTTLA.Shared
{
	public class AdminViewModel
	{
		public ICommand AddLeagueCommand
		{
			get
			{
				return new Command(async(param) =>
				{
//					var league = new League {
//						Name = "Xamarin"
//					};
//					await AzureService.Instance.SaveLeague(league);
				});
			}
		}

	}
}

