using System;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;

[assembly: Dependency(typeof(SportRankerMatchOn.Shared.MembershipDetailsViewModel))]

namespace SportRankerMatchOn.Shared
{
	public class MembershipDetailsViewModel : BaseViewModel
	{
		public MembershipDetailsViewModel()
		{
			Membership = new Membership();
		}

		public MembershipDetailsViewModel(Membership membership = null)
		{
			Membership = membership ?? new Membership();
		}

		Membership membership;
		public const string MembershipPropertyName = "Membership";

		public Membership Membership
		{
			get
			{
				return membership;
			}
			set
			{
				SetProperty(ref membership, value, MembershipPropertyName);
			}
		}

		public ICommand SaveCommand
		{
			get
			{
				return new Command(async(param) =>
					await SaveMember());
			}
		}

		async public Task SaveMember()
		{
			using(new Busy(this))
			{
				try
				{
					await AzureService.Instance.SaveMembership(Membership);
				}
				catch(Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}

		async public Task DeleteMember()
		{
			using(new Busy(this))
			{
				try
				{
					await AzureService.Instance.DeleteMembership(Membership.Id);
				}
				catch(Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}
	}
}