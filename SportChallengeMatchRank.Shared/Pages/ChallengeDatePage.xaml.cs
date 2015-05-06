using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
{
	public partial class ChallengeDatePage : ChallengeDateXaml
	{
		public Athlete Challengee
		{
			get;
			private set;
		}

		public ChallengeDatePage(Athlete challengee)
		{
			Challengee = challengee;
			Initialize();
		}

		protected async override void Initialize()
		{
			InitializeComponent();
			Title = "Set Date and Time";

			datePicker.DateSelected += async(sender, e) =>
			{
				Console.WriteLine(datePicker.Date);	
				await ViewModel.CrossReferenceCalendars(App.CurrentAthlete, Challengee, datePicker.Date);
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