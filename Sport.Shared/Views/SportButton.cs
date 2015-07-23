using System;
using Xamarin.Forms;

namespace Sport.Shared
{
	public class SportButton : Button
	{
		public SportButton() : base()
		{
			Clicked += async(sender, e) =>
			{
				await this.ScaleTo(1.2, 100);
				this.ScaleTo(1, 100);
			};
		}
	}
}