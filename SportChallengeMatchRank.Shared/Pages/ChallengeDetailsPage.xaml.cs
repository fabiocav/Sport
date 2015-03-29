using Xamarin.Forms;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace SportChallengeMatchRank.Shared
{
	public partial class ChallengeDetailsPage : ChallengeDetailsXaml
	{
		public ChallengeDetailsPage(Challenge member = null)
		{
			ViewModel.Challenge = member ?? new Challenge();
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
			Title = "Challenge Details";

			btnAccept.Clicked += async(sender, e) =>
			{
			};

			btnDecline.Clicked += async(sender, e) =>
			{
			};
		}
	}

	public partial class ChallengeDetailsXaml : BaseContentPage<ChallengeDetailsViewModel>
	{
	}
}