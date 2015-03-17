using System;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace SportRankerMatchOn.Shared.Mobile
{
	public class NewLeagueViewModel : BaseViewModel
	{
		string _leagueName;
		public const string LeagueNamePropertyName = "LeagueName";

		public string LeagueName
		{
			get
			{
				return _leagueName;
			}
			set
			{
				SetProperty(ref _leagueName, value, LeagueNamePropertyName);
			}
		}

		bool _isEnabled;
		public const string IsEnabledPropertyName = "IsEnabled";

		public bool IsEnabled
		{
			get
			{
				return _isEnabled;
			}
			set
			{
				SetProperty(ref _isEnabled, value, IsEnabledPropertyName);
			}
		}

		public ICommand SaveLeagueCommand
		{
			get
			{
				return new Command(async(param) =>
				{
//					using(new Busy(this))
					{
						IsBusy = true;
						var league = new League {
							Name = LeagueName,
							Season = 1
						};
		
						try
						{
							await AzureService.Instance.SaveLeague(league);
						}
						catch(Exception e)
						{
							Console.WriteLine(e);
						}
						IsBusy = false;
					}
				});
			}
		}
	}
}