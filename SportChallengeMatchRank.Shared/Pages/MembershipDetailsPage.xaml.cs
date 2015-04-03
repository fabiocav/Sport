﻿using Xamarin.Forms;
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
				var accepted = await DisplayAlert("Delete Membership?", "Are you totes sure you want to delete this membership?", "Yes", "No");

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
				if(challenge != null)
				{
					await DisplayAlert("Challenge Sent!", "{0} has been notified of this honorable duel.".Fmt(ViewModel.Membership.Athlete.Name), "OK");
				}
			};

			btnRevokeChallenge.Clicked += async(sender, e) =>
			{
				var revoke = await DisplayAlert("Really?", "Are you sure you want to cowardly revoke this honorable duel?", "Sadly, yes", "No - good point");

				if(!revoke)
					return;
					
				await ViewModel.RevokeExistingChallenge(ViewModel.Membership);
				await DisplayAlert("Challenge revoked", "{0} has been notified of your shameless ways.".Fmt(ViewModel.Membership.Athlete.Name), "OK");
			};
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			ViewModel.Membership.LocalRefresh();
			ViewModel.OnPropertyChanged("CanChallenge");
		}
	}

	public partial class MembershipDetailsXaml : BaseContentPage<MembershipDetailsViewModel>
	{
	}
}