using SportChallengeMatchRank.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using SportChallengeMatchRank.Shared;
using UIKit;
using System.Threading.Tasks;
using System.Reflection;
using System;
using System.Collections.Generic;

[assembly: ExportRenderer(typeof(ClearNavigationPage), typeof(ClearNavigationRenderer))]
namespace SportChallengeMatchRank.iOS
{
	public class ClearNavigationRenderer : NavigationRenderer
	{
		protected override Task<bool> OnPushAsync(Page page, bool animated)
		{
			ChangeTheme(page);
			return base.OnPushAsync(page, animated);
		}

		public override UIViewController PopViewController(bool animated)
		{
			var obj = Element.GetType().InvokeMember("StackCopy", BindingFlags.GetProperty | BindingFlags.NonPublic, Type.DefaultBinder, Element, null);
			if(obj != null)
			{
				var pages = obj as Stack<Page>;
				if(pages != null && pages.Count >= 2)
				{
					var copy = new Page[pages.Count];
					pages.CopyTo(copy, 0);

					var prev = copy[1];
					ChangeTheme(prev);
				}
			}
			return base.PopViewController(animated);
		}

		void ChangeTheme(Page page)
		{
			var basePage = page as SuperBaseContentPage;
			if(basePage != null)
			{
				NavigationBar.BarTintColor = basePage.BarBackgroundColor.ToUIColor();
				NavigationBar.TintColor = basePage.BarTextColor.ToUIColor();

				var titleAttributes = new UIStringAttributes();
				titleAttributes.Font = UIFont.FromName("SegoeUI", 22);
				titleAttributes.ForegroundColor = basePage.BarTextColor == Color.Default ? titleAttributes.ForegroundColor ?? UINavigationBar.Appearance.TintColor : basePage.BarTextColor.ToUIColor();
				NavigationBar.TitleTextAttributes = titleAttributes;


				// set Tint color (i. e. Back Button arrow and Text)
				UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;
//				var atts = new UITextAttributes {
//					TextColor = basePage.BarTextColor.ToUIColor(),
//					Font = UIFont.FromName("SegoeUI", 22),
//				};
//				UINavigationBar.Appearance.SetTitleTextAttributes(atts);

//				await UIView.AnimateAsync(250, () =>
//				{
//					NavigationBar.BarTintColor = basePage.BarBackgroundColor.ToUIColor();
//					NavigationBar.TintColor = basePage.BarTextColor.ToUIColor();
//				});
			}
		}
	}
}