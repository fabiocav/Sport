using System;
using Xamarin.Forms;
using System.Linq;

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

		protected async override void Initialize()
		{
			InitializeComponent();
			Title = "Membership";

			btnSaveMembership.Clicked += async(sender, e) =>
			{
				await ViewModel.SaveMembership();
				"Saved".ToToast(ToastNotificationType.Success);
			};

			btnDeleteMembership.Clicked += async(sender, e) =>
			{
				var accepted = await DisplayAlert("Delete Membership?", "Are you sure you want to delete this membership?", "Yes", "No");

				if(accepted)
				{
					await ViewModel.DeleteMembership();
					
					if(OnDelete != null)
						OnDelete();

					"Deleted.".ToToast(ToastNotificationType.Success);
					await Navigation.PopAsync();
				}
			};

			btnChallenge.Clicked += async(sender, e) =>
			{
				var conflict = ViewModel.Membership.GetChallengeConflictReason(App.CurrentAthlete);
				if(conflict != null)
				{
					conflict.ToToast(ToastNotificationType.Error);
					return;
				}

				var datePage = new ChallengeDatePage(ViewModel.Membership.Athlete, ViewModel.Membership.League);

				datePage.OnChallengeSent = async(challenge) =>
				{
					ViewModel.NotifyPropertiesChanged();
					await Navigation.PopModalAsync();
					"{0} has been notified of this honorable duel.".Fmt(ViewModel.Membership.Athlete.Name).ToToast(ToastNotificationType.Success);
				};

				await Navigation.PushModalAsync(new NavigationPage(datePage));
			};

			btnRevokeChallenge.Clicked += async(sender, e) =>
			{
				var revoke = await DisplayAlert("Really?", "Are you sure you want to revoke this challenge?", "Sadly", "No");

				if(!revoke)
					return;
					
				using(new HUD("Revoking..."))
				{
					await ViewModel.RevokeExistingChallenge(ViewModel.Membership);
				}

				"Revoked".Fmt(ViewModel.Membership.Athlete.Name).ToToast();
			};

			await ViewModel.RunSafe(AzureService.Instance.GetAllChallengesByAthlete(ViewModel.Membership.Athlete));
			ViewModel.SetPropertyChanged("CanChallenge");
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
			ViewModel.NotifyPropertiesChanged();
		}

		protected override async void OnIncomingPayload(App app, NotificationPayload payload)
		{
			var reload = false;

			string membershipId = null;
			string winningAthleteId = null;
			string losingAthleteId = null;

			if(payload.Payload.TryGetValue("membershipId", out membershipId) && membershipId == ViewModel.MembershipId)
				reload = true;

			if(payload.Payload.TryGetValue("winningAthleteId", out winningAthleteId) && payload.Payload.TryGetValue("losingAthleteId", out losingAthleteId))
			{
				reload |= winningAthleteId == ViewModel.Membership.AthleteId || losingAthleteId == ViewModel.Membership.AthleteId;
			}

//			reload |= payload.Payload.TryGetValue("challengeId", out challengeId) && ViewModel.Membership.Athlete.AllChallenges.Any(c => c.Id == challengeId);

			if(reload)
			{
				await ViewModel.RefreshMembership();
			}
		}
	}

	public partial class MembershipDetailsXaml : BaseContentPage<MembershipDetailsViewModel>
	{

	}
}