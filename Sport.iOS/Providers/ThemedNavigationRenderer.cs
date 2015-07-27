using Sport.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Sport.Shared;
using UIKit;
using System.Threading.Tasks;
using System.Reflection;
using System;
using System.Collections.Generic;

[assembly: ExportRenderer(typeof(ThemedNavigationPage), typeof(ThemedNavigationRenderer))]
namespace Sport.iOS
{
	public class ThemedNavigationRenderer : NavigationRenderer
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

		public override void ViewDidLoad()
		{
			Element.PropertyChanged += HandlePropertyChanged;
			base.ViewDidLoad();
		}

		void HandlePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if(e.PropertyName == NavigationPage.BarTextColorProperty.PropertyName)
			{
				//This is here to override the default Forms behavior which modifies this value based on BarTextColor luminosity > .5
				UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;
			}
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


				UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;

				//Hoping to animate the color transition at some point
//				var atts = new UITextAttributes {
//					TextColor = basePage.BarTextColor.ToUIColor(),
//					Font = UIFont.FromName("SegoeUI", 22),
//				};
//				UINavigationBar.Appearance.SetTitleTextAttributes(atts);
//
//				await UIView.AnimateAsync(250, () =>
//				{
//					NavigationBar.BarTintColor = basePage.BarBackgroundColor.ToUIColor();
//					NavigationBar.TintColor = basePage.BarTextColor.ToUIColor();
//				});
			}
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				var navPage = (NavigationPage)Element;
				navPage.PropertyChanged -= HandlePropertyChanged;
			}
		}
	}
}