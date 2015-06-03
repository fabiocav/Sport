using SportChallengeMatchRank.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using SportChallengeMatchRank.Shared;
using UIKit;
using System.Threading.Tasks;

[assembly: ExportRenderer(typeof(SportChallengeMatchRank.Shared.ClearNavigationPage), typeof(ClearNavigationRenderer))]
namespace SportChallengeMatchRank.iOS
{
	public class ClearNavigationRenderer : NavigationRenderer
	{
		protected override System.Threading.Tasks.Task<bool> OnPopViewAsync(Page page, bool animated)
		{
			return base.OnPopViewAsync(page, animated);
		}

		//		async public override void PushViewController(UIViewController viewController, bool animated)
		//		{
		//			var basePage = ((NavigationPage)Element).CurrentPage as SuperBaseContentPage;
		//			if(basePage != null)
		//			{
		//				await Task.Delay(1000);
		//				await UIView.AnimateAsync(1000, () =>
		//				{
		//					NavigationBar.BarTintColor = basePage.BarBackgroundColor.ToUIColor();
		//				});
		//			}
		//
		//
		//			base.PushViewController(viewController, animated);
		//		}

		protected override System.Threading.Tasks.Task<bool> OnPushAsync(Page page, bool animated)
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				ChangeTheme(page);
			});
		
			return base.OnPushAsync(page, animated);
		}

		async void ChangeTheme(Page page)
		{
			var basePage = page as SuperBaseContentPage;
			if(basePage != null)
			{
				NavigationBar.BarTintColor = basePage.BarBackgroundColor.ToUIColor();
				NavigationBar.TintColor = basePage.BarTextColor.ToUIColor();

				var atts = new UITextAttributes {
					TextColor = basePage.BarTextColor.ToUIColor(),
					Font = UIFont.FromName("SegoeUI", 22),
				};
				UINavigationBar.Appearance.SetTitleTextAttributes(atts);
//				await UIView.AnimateAsync(250, () =>
//				{
//					NavigationBar.BarTintColor = basePage.BarBackgroundColor.ToUIColor();
//					NavigationBar.TintColor = basePage.BarTextColor.ToUIColor();
//				});
			}
		}

		protected override System.Threading.Tasks.Task<bool> OnPopToRoot(Page page, bool animated)
		{
			return base.OnPopToRoot(page, animated);
		}
	}
}