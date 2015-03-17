using System;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace XSTTLA.Shared
{
	public class BaseContentPage<T> : ContentPage where T : BaseViewModel, new()
	{
		public BaseContentPage() : base()
		{
			BindingContext = ViewModel;
		}

		T _viewModel;

		public T ViewModel
		{
			get
			{
				return _viewModel ?? (_viewModel = new T());
			}
		}

		public void EnsureUserAuthenticated()
		{
			if(AppSettings.AuthUserProfile == null)
			{
				MessagingCenter.Send<BaseContentPage<T>>(this, "AuthenticateUser");
			}
		}

		protected override void OnAppearing()
		{
			EnsureUserAuthenticated();
			base.OnAppearing();
		}
	}
}