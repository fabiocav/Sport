using System;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using UIKit;
using SportChallengeMatchRank.Shared;

[assembly: ExportRenderer(typeof(ClearNavPage), typeof(SportChallengeMatchRank.iOS.ClearNavPageRenderer))]

namespace SportChallengeMatchRank.iOS
{
	public class ClearNavPageRenderer : NavigationRenderer
	{
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			NavigationBar.Translucent = true;
			NavigationBar.ShadowImage = new UIImage();
			NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);

			EdgesForExtendedLayout = UIRectEdge.None;
			View.BackgroundColor = UIColor.Clear;
		}

		public override void WillMoveToParentViewController(UIViewController parent)
		{
			base.WillMoveToParentViewController(parent);

			if(parent != null)
			{
				parent.View.BackgroundColor = UIColor.Clear;
				parent.EdgesForExtendedLayout = UIRectEdge.None;
			}
		}

		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			if(NavigationController != null)
				NavigationController.View.BackgroundColor = UIColor.Clear;
			base.OnElementChanged(e);
		}

		protected override System.Threading.Tasks.Task<bool> OnPushAsync(Page page, bool animated)
		{
			return base.OnPushAsync(page, animated);
		}
	}
}