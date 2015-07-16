using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Sport.Shared
{
	public partial class SetAliasPage : SetAliasPageXaml
	{
		public Action OnSave
		{
			get;
			set;
		}

		public SetAliasPage()
		{
			NavigationPage.SetHasNavigationBar(this, false);
			ViewModel.AthleteId = App.CurrentAthlete.Id;

			Initialize();
		}

		protected async override void Initialize()
		{
			BarBackgroundColor = (Color)App.Current.Resources["redPrimary"];
			BarTextColor = Color.White;

			BackgroundColor = BarBackgroundColor;
			InitializeComponent();
			profileStack.Opacity = 0;

			Title = "Athlete Alias";

			var ignoreClicks = false;
			btnSave.Clicked += async(sender, e) =>
			{
				if(ignoreClicks)
					return;

				if(string.IsNullOrWhiteSpace(ViewModel.Athlete.Alias))
				{
					"Please enter an alias.".ToToast(ToastNotificationType.Warning);
					return;
				}

				ignoreClicks = true;

				bool success;
				success = await ViewModel.SaveAthlete();
				if(success)
				{
					if(OnSave != null)
						OnSave();

					await profileStack.LayoutTo(new Rectangle(0, Content.Width * -1, profileStack.Width, profileStack.Height), (uint)App.AnimationSpeed, Easing.SinIn);
					await label1.FadeTo(0, (uint)App.AnimationSpeed, Easing.SinIn);
					await aliasBox.FadeTo(0, (uint)App.AnimationSpeed, Easing.SinIn);
					await label2.FadeTo(0, (uint)App.AnimationSpeed, Easing.SinIn);
					await buttonStack.FadeTo(0, (uint)App.AnimationSpeed, Easing.SinIn);

					await Navigation.PushAsync(new EnablePushPage());
				}
				ignoreClicks = success;
			};
		}

		protected async override void OnLoaded()
		{
			profileStack.Layout(new Rectangle(0, profileStack.Height * -1, profileStack.Width, profileStack.Height));
			base.OnLoaded();

			await Task.Delay(300);
			profileStack.Opacity = 1;

			await profileStack.LayoutTo(new Rectangle(0, 0, profileStack.Width, profileStack.Height), (uint)App.AnimationSpeed, Easing.SinIn);
			await label1.ScaleTo(1, (uint)App.AnimationSpeed, Easing.SinIn);
			await aliasBox.ScaleTo(1, (uint)App.AnimationSpeed, Easing.SinIn);
			await label2.ScaleTo(1, (uint)App.AnimationSpeed, Easing.SinIn);
			await buttonStack.ScaleTo(1, (uint)App.AnimationSpeed, Easing.SinIn);

			await Task.Delay(2000);

			if(IsVisible && string.IsNullOrEmpty(ViewModel.Athlete.Alias))
				txtAlias.Focus();
		}
	}

	public partial class SetAliasPageXaml : BaseContentPage<AthleteProfileViewModel>
	{
	}
}