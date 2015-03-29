using Xamarin.Forms;
using System.Threading.Tasks;
using System;
using System.Linq;

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

		protected override void Initialize()
		{
			InitializeComponent();
			Title = "Membership Details";

			btnSaveMembership.Clicked += async(sender, e) =>
			{
				await ViewModel.SaveMembership();
				await Navigation.PopModalAsync();
			};

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

			btnChallenge.Clicked += async(sender, e) =>
			{
				var challenge = await ViewModel.ChallengeAthlete(ViewModel.Membership);
				DisplayAlert("Challenge Sent!", "{0} has been notified of this honorable duel.".Fmt(ViewModel.Membership.Athlete.Name), "OK");
			};
		}
	}

	public partial class MembershipDetailsXaml : BaseContentPage<MembershipDetailsViewModel>
	{
	}
}