using System;
using System.Threading.Tasks;

namespace SportChallengeMatchRank.Shared
{
	public interface IAppearance
	{
		void SetNavigationBarBackgroundColor();

		void SetNavigationBarTextColor();

		void SetTabBarBackgroundColor();

		void SetTabBarTextColor();
	}
}