using Xamarin.Forms;
using System.Threading.Tasks;
using System;

namespace SportChallengeMatchRank.Shared
{
	public partial class MembershipDetailsPage : MembershipDetailsXaml
	{
		public MembershipDetailsPage(Membership member = null)
		{
			ViewModel.Membership = member ?? new Membership();
			Initialize();
		}

		public Action OnDelete
		{
			get;
			set;
		}

		void Initialize()
		{
			InitializeComponent();
			Title = "Membership Details";

			btnSaveMembership.Clicked += async(sender, e) =>
			{
				await ViewModel.SaveMembership();
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
					await ViewModel.DeleteMembership();
					
					if(OnDelete != null)
						OnDelete();
						
					await Navigation.PopModalAsync();
				}
			};
		}
	}

	public partial class MembershipDetailsXaml : BaseContentPage<MembershipDetailsViewModel>
	{
	}
}