using System;
using Xamarin.Forms;
using System.Linq;

namespace Sport.Shared
{
	public partial class AboutPage : AboutPageXaml
	{
		public Action OnDelete
		{
			get;
			set;
		}

		public AboutPage()
		{
			Initialize();
			Title = "About";
		}

		protected async override void Initialize()
		{
			InitializeComponent();
		}

		void HandleViewSourceClicked(object sender, EventArgs e)
		{
			Device.OpenUri(new Uri(Constants.SourceCodeUrl));
		}
	}

	public partial class AboutPageXaml : BaseContentPage<BaseViewModel>
	{

	}
}