using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Toasts.Forms.Plugin.Abstractions;
using XLabs.Forms.Controls;

namespace SportChallengeMatchRank.Shared
{
	public partial class ChallengeDatePage : ChallengeDateXaml
	{
		public Action<Challenge> OnChallengeSent
		{
			get;
			set;
		}

		public Athlete Challengee
		{
			get;
			private set;
		}

		public League League
		{
			get;
			private set;
		}

		public ChallengeDatePage(Athlete challengee, League league)
		{
			Challengee = challengee;
			League = league;
			Initialize();
		}

		protected async override void Initialize()
		{
			InitializeComponent();
			Title = "Set Date and Time";

			datePicker.DateSelected += async(sender, e) =>
			{
				Console.WriteLine(datePicker.Date);
				//await ViewModel.CrossReferenceCalendars(App.CurrentAthlete, Challengee, datePicker.Date);
			};

			btnChallenge.Clicked += async(sender, e) =>
			{
				var errors = ViewModel.Validate();

				if(errors != null)
				{
					errors.ToToast(ToastNotificationType.Error, "Please fix this error");
					return;
				}

				var challenge = await ViewModel.ChallengeAthlete(App.CurrentAthlete, Challengee, League);

				if(OnChallengeSent != null && challenge != null && challenge.Id != null)
					OnChallengeSent(challenge);
			};

			var btnCancel = new ToolbarItem {
				Text = "Cancel"
			};

			btnCancel.Clicked += async(sender, e) =>
			{
				await Navigation.PopModalAsync();		
			};

			ToolbarItems.Add(btnCancel);

			await Task.Delay(1000);
			datePicker.Focus();
		}
	}

	public partial class ChallengeDateXaml : BaseContentPage<ChallengeDateViewModel>
	{
	}
}