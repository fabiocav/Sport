using System;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Linq;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.ChallengeDetailsViewModel))]

namespace SportChallengeMatchRank.Shared
{
	public class ChallengeDetailsViewModel : BaseViewModel
	{
		public ChallengeDetailsViewModel()
		{
			Challenge = new Challenge();
		}

		public ChallengeDetailsViewModel(Challenge challenge)
		{
			Challenge = challenge;
		}

		Challenge challenge;
		public const string ChallengePropertyName = "Challenge";

		public Challenge Challenge
		{
			get
			{
				return challenge;
			}
			set
			{
				SetProperty(ref challenge, value, ChallengePropertyName);
			}
		}

		async public Task AcceptChallenge()
		{
			using(new Busy(this))
			{
				try
				{
					await AzureService.Instance.AcceptChallenge(Challenge.Id);
				}
				catch(Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}

		async public Task DeclineChallenge()
		{
			using(new Busy(this))
			{
				try
				{
					await AzureService.Instance.DeclineChallenge(Challenge.Id);
				}
				catch(Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}
	}
}