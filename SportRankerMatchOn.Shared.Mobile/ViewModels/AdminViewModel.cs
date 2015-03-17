using System;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace SportRankerMatchOn.Shared.Mobile
{
	public class AdminViewModel : BaseViewModel
	{
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