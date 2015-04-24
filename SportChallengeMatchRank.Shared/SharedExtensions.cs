using System.Linq;
using System;
using System.Threading;

namespace SportChallengeMatchRank.Shared
{
	public static partial class Extensions
	{
		public static bool ContainsNoCase(this string s, string contains)
		{
			if(s == null || contains == null)
				return false;

			return s.IndexOf(contains, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		public static string TrimStart(this string s, string toTrim)
		{
			if(s.StartsWith(toTrim, true, Thread.CurrentThread.CurrentCulture))
				return s.Substring(toTrim.Length);

			return s;
		}

		public static string TrimEnd(this string s, string toTrim)
		{
			if(s.EndsWith(toTrim, true, Thread.CurrentThread.CurrentCulture))
				return s.Substring(0, s.Length - toTrim.Length);

			return s;
		}

		public static string Fmt(this string s, params object[] args)
		{
			return string.Format(s, args);
		}

		public static GameResult[] GetChallengerWinningGames(this Challenge challenge)
		{
			return challenge.MatchResult.Where(gr => gr.ChallengerScore > gr.ChallengeeScore).ToArray();
		}

		public static GameResult[] GetChallengeeWinningGames(this Challenge challenge)
		{
			return challenge.MatchResult.Where(gr => gr.ChallengeeScore > gr.ChallengerScore).ToArray();
		}

		public static bool IsEmpty(this string s)
		{
			return string.IsNullOrWhiteSpace(s);
		}

		public static string ToOrdinal(this int num)
		{
			if(num <= 0)
				return num.ToString();

			switch(num % 100)
			{
				case 11:
				case 12:
				case 13:
					return num + "th";
			}

			switch(num % 10)
			{
				case 1:
					return num + "st";
				case 2:
					return num + "nd";
				case 3:
					return num + "rd";
				default:
					return num + "th";
			}

		}
	}
}