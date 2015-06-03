using System;
using System.Threading.Tasks;
using Xamarin.Forms;
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
			BarBackgroundColor = league.Theme.Light;
			BarTextColor = league.Theme.Dark;

			ViewModel.CreateChallenge(App.CurrentAthlete, challengee, league);
			Challengee = challengee;
			League = league;
			Initialize();
		}

		protected override void Initialize()
		{
			InitializeComponent();
			Title = "Date and Time";

			btnChallenge.Clicked += async(sender, e) =>
			{
				var errors = ViewModel.Validate();

				if(errors != null)
				{
					errors.ToToast(ToastNotificationType.Error);
					return;
				}

				Challenge challenge;
				using(new HUD("Sending challenge..."))
				{
					challenge = await ViewModel.PostChallenge();
				}

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
		}
	}

	public partial class ChallengeDateXaml : BaseContentPage<ChallengeDateViewModel>
	{
	}
}