﻿using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
{
	public partial class EnablePushPage : EnablePushPageXaml
	{
		public Action OnSave
		{
			get;
			set;
		}

		public EnablePushPage()
		{
			NavigationPage.SetHasNavigationBar(this, false);
			Initialize();
		}

		protected async override void Initialize()
		{
			InitializeComponent();
			Title = "Enable Push";

			MessagingCenter.Subscribe<App>(this, "RegisteredForRemoteNotifications", (app) =>
			{
				Device.BeginInvokeOnMainThread(() =>
				{
					MessagingCenter.Unsubscribe<App>(this, "RegisteredForRemoteNotifications");
					RegisteredForPushNotificationSuccess();
				});
			});

			btnPush.Clicked += async(sender, e) =>
			{
				var push = DependencyService.Get<IPushNotifications>();

				#if !DEBUG
				if(!push.IsRegistered)
				{
					push.RegisterForPushNotifications();
				}
				#endif

				await Task.Delay(1000);
				btnPush.Text = "Thanks! We'll be in touch.";
				await Task.Delay(1000);
				await btnPush.LayoutTo(new Rectangle(Content.Width, btnPush.Bounds.Y, btnPush.Bounds.Width, btnPush.Height), 350, Easing.SinIn);

			};

			btnContinue.Clicked += (sender, e) =>
			{
				Settings.Instance.RegistrationComplete = true;
				Settings.Instance.Save();
				App.Current.MainPage = new AthleteLeaguesPage().GetNavigationPage();
			};

//			list.ItemSelected += async(sender, e) =>
//			{
//				if(list.SelectedItem == null)
//					return;
//				
//				var league = list.SelectedItem as League;
//				list.SelectedItem = null;
//
//				if(league.Id == null)
//					return;
//
//				var detailsPage = new LeagueDetailsPage(league);
//				var navPage = new NavigationPage(detailsPage);
//				var cancel = new ToolbarItem {
//					Text = "Cancel",
//				};
//
//				detailsPage.OnJoinedLeague = (l) =>
//				{
//					ViewModel.LocalRefresh();
//					Navigation.PopModalAsync();
//				};
//					
//				cancel.Clicked += (sender2, e2) =>
//				{
//					Navigation.PopModalAsync();
//				};
//
//				navPage.ToolbarItems.Add(cancel);
//				await Navigation.PushModalAsync(navPage);	
//			};

			await ViewModel.GetAvailableLeagues();
		}

		protected async override void OnLoaded()
		{
			base.OnLoaded();

			await Task.Delay(300);
			await label1.ScaleTo(1, (uint)App.AnimationSpeed, Easing.SinIn);
			//await list.ScaleTo(1, (uint)App.AnimationSpeed, Easing.SinIn);
			await buttonStack.ScaleTo(1, (uint)App.AnimationSpeed, Easing.SinIn);
		}

		async Task RegisteredForPushNotificationSuccess()
		{
			var task = AzureService.Instance.UpdateAthleteNotificationHubRegistration(App.CurrentAthlete);
			await ViewModel.RunSafe(task);

			if(task.IsFaulted)
				return;

			btnPush.Text = "Thanks! We'll be in touch.";
			await Task.Delay(1000);
			await btnPush.LayoutTo(new Rectangle(Content.Width, btnPush.Bounds.Y, btnPush.Bounds.Width, btnPush.Height), 350, Easing.SinIn);
		}
	}

	public partial class EnablePushPageXaml : BaseContentPage<AvailableLeaguesViewModel>
	{
	}
}