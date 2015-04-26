
namespace SportChallengeMatchRank.Shared
{
	public partial class AdminPage : AdminPageXaml
	{
		public AdminPage()
		{
			Initialize();
		}

		protected override void Initialize()
		{
			base.Initialize();

			Title = "Admin";
			InitializeComponent();

			btnLeagues.Clicked += async(sender, e) =>
			{
				await Navigation.PushAsync(new LeagueLandingPage());	
			};

			btnAthletes.Clicked += async(sender, e) =>
			{
				await Navigation.PushAsync(new AthleteListPage());	
			};
		}

		//		public ICommand OrgSettingsCommand
		//		{
		//			get
		//			{
		//				return new Command(() => Navigation.PushAsync(new AthleteListPagePage()));
		//			}
		//		}
	}

	public partial class AdminPageXaml : BaseContentPage<AdminViewModel>
	{
	}
}