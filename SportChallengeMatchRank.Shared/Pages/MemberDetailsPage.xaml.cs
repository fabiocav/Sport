using Xamarin.Forms;
using System.Threading.Tasks;

namespace SportChallengeMatchRank.Shared
{
	public partial class MembershipDetailsPage : MembershipDetailsXaml
	{
		public MembershipDetailsPage(Membership member = null)
		{
			ViewModel.Membership = member ?? new Membership();
			InitializeComponent();
			Title = "Membership Details";

			btnSaveMembership.Clicked += async(sender, e) =>
			{
				var isNew = ViewModel.Membership.Id == null;
				await ViewModel.SaveMember();
//				var landingVm = DependencyService.Get<MLandingViewModel>();
//
//				if(isNew)
//				{
//					landingVm.AllMemberships.Add(ViewModel.Member);
//				}
//				else
//				{
//				}

				await Navigation.PopModalAsync();
			};

			var btnCancel = new ToolbarItem {
				Text = "Cancel"		
			};

			btnCancel.Clicked += async(sender, e) =>
			{
				await Navigation.PopModalAsync();		
			};

			ToolbarItems.Add(btnCancel);

			btnDeleteMembership.Clicked += async(sender, e) =>
			{
				var accepted = await DisplayAlert("Delete Membership?", "Are you totes sure you want to delete this membership?", "Yeah brah!", "Nah");

				if(accepted)
				{
//					await ViewModel.DeleteMember();
//					var landingVm = DependencyService.Get<MemberLandingViewModel>();
//					landingVm.AllMemberships.Remove(ViewModel.Member);
//					await Navigation.PopModalAsync();
				}
			};
		}
	}

	public partial class MembershipDetailsXaml : BaseContentPage<MembershipDetailsViewModel>
	{
	}
}