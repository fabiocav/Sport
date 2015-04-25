using System;
using Toasts.Forms.Plugin.Abstractions;
using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
{
	public partial class MembershipDetailsPage : MembershipDetailsXaml
	{
		public Action OnDelete
		{
			get;
			set;
		}

		public MembershipDetailsPage(string membershipId)
		{
			ViewModel.MembershipId = membershipId;
			Initialize();
		}

		protected override void Initialize()
		{
			InitializeComponent();
			Title = "Membership Details";

			btnSaveMembership.Clicked += async(sender, e) =>
			{
				await ViewModel.SaveMembership();
				"Membership saved!".ToToast(ToastNotificationType.Success);
			};

			btnDeleteMembership.Clicked += async(sender, e) =>
			{
				var accepted = await DisplayAlert("Delete Membership?", "Are you totes sure you want to delete this membership?", "Yes", "No");

				if(accepted)
				{
					await ViewModel.DeleteMembership();
					
					if(OnDelete != null)
						OnDelete();

					"Membership removed.".ToToast(ToastNotificationType.Success);
					await Navigation.PopAsync();
				}
			};

			btnChallenge.Clicked += async(sender, e) =>
			{
				var challenge = await ViewModel.ChallengeAthlete(ViewModel.Membership);
				if(challenge != null && challenge.Id != null)
				{
					"{0} has been notified of this honorable duel.".Fmt(ViewModel.Membership.Athlete.Name).ToToast(ToastNotificationType.Success);
				}
			};

			btnRevokeChallenge.Clicked += async(sender, e) =>
			{
				var revoke = await DisplayAlert("Really?", "Are you sure you want to cowardly revoke this honorable duel?", "Sadly, yes", "No - good point");

				if(!revoke)
					return;
					
				await ViewModel.RevokeExistingChallenge(ViewModel.Membership);
				"{0} has been notified of your shameless ways.".Fmt(ViewModel.Membership.Athlete.Name).ToToast(ToastNotificationType.Info);
			};
		}

		protected override void OnParentSet()
		{
			base.OnParentSet();

			if(ParentView == null)
				return;

			var width = ParentView.Bounds.Width / 3;

			photoImage.WidthRequest = width;
			photoImage.HeightRequest = width;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			ViewModel.Membership.LocalRefresh();
			//ViewModel.NotifyPropertiesChanged();
		}
	}

	public partial class MembershipDetailsXaml : BaseContentPage<MembershipDetailsViewModel>
	{
	}
}