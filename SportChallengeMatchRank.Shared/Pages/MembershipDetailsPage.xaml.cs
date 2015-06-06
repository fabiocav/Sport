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
			Title = ViewModel.Membership.Athlete.Name;

			btnChallenge.Clicked += async(sender, e) =>
			{
				var conflict = ViewModel.Membership.GetChallengeConflictReason(App.CurrentAthlete);
				if(conflict != null)
				{
					conflict.ToToast();
					return;
				}

				var datePage = new ChallengeDatePage(ViewModel.Membership.Athlete, ViewModel.Membership.League);

				datePage.OnChallengeSent = async(challenge) =>
				{
					ViewModel.NotifyPropertiesChanged();
					await Navigation.PopModalAsync();
					await Navigation.PopAsync();

					"Challenge sent - it's soooo on!".Fmt(ViewModel.Membership.Athlete.Name).ToToast(ToastNotificationType.Success);
				};

				await Navigation.PushModalAsync(new NavigationPage(datePage));
			};

			ViewModel.SetPropertyChanged("CanChallenge");
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