using System;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace SportRankerMatchOn.Shared
{
	public class AdminViewModel : BaseViewModel
	{
		public ICommand AddLeagueCommand
		{
			get
			{
				return new Command(async(param) =>
				{
					using(new Busy(this))
					{
						var league = new League {
							Name = "Xamarin",
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
					}
				});
			}
		}

		public ICommand AddMemberCommand
		{
			get
			{
				return new Command(async(param) =>
				{
					using(new Busy(this))
					{
						var member = new Member {
							FirstName = "Rob",
							LastName = "DeRosa",
						};

						try
						{
							await AzureService.Instance.SaveMember(member);
						}
						catch(Exception e)
						{
							Console.WriteLine(e);
						}
					}
				});
			}
		}
	}
}