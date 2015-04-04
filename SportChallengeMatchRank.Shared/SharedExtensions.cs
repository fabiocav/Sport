using System.Linq;

namespace SportChallengeMatchRank.Shared
{
	public static partial class Extensions
	{
		public static GameResult[] GetChallengerWinningGames(this Challenge challenge)
		{
			return challenge.GameResults.Where(gr => gr.ChallengerScore > gr.ChallengeeScore).ToArray();
		}

		public static GameResult[] GetChallengeeWinningGames(this Challenge challenge)
		{
			return challenge.GameResults.Where(gr => gr.ChallengeeScore > gr.ChallengerScore).ToArray();
		}

	}
}