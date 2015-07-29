using System;
using Xamarin.Forms;
using System.Linq;

namespace Sport.Shared
{
	public partial class AboutPage : AboutPageXaml
	{
		public AboutPage()
		{
			Initialize();
			Title = "About";
		}

		protected override void Initialize()
		{
			InitializeComponent();
		}

		void HandleXamarinClicked(object sender, EventArgs e)
		{
			Device.OpenUri(new Uri("http://xamarin.com/forms"));
		}

		void HandleViewSourceClicked(object sender, EventArgs e)
		{
			Device.OpenUri(new Uri(Keys.SourceCodeUrl));
		}
	}

	public partial class AboutPageXaml : BaseContentPage<BaseViewModel>
	{

	}
}