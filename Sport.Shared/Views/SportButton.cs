using System;
using Xamarin.Forms;
using System.Diagnostics;

namespace Sport.Shared
{
	public class SportButton : Button
	{
		public SportButton() : base()
		{
//			Clicked += async(sender, e) =>
//			{
//				var btn = (SportButton)sender;
//				await btn.ScaleTo(1.2, 100);
//				btn.ScaleTo(1, 100);
//			};
			Debug.WriteLine("Constructor called for {0} {1}".Fmt(GetType().Name, GetHashCode()));
		}

		~SportButton()
		{
			Debug.WriteLine("Destructor called for {0} {1}".Fmt(GetType().Name, GetHashCode()));
		}
	}
}