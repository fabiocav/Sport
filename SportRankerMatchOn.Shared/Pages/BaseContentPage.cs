using System;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace SportRankerMatchOn.Shared
{
	public class BaseContentPage<T> : ContentPage where T : BaseViewModel, new()
	{
		public BaseContentPage() : base()
		{
			BindingContext = ViewModel;
		}

		T _viewModel;

		public bool HasInitialized
		{
			get;
			private set;
		}

		public T ViewModel
		{
			get
			{
				return _viewModel ?? (_viewModel = DependencyService.Get<T>());
			}
		}

		public void EnsureUserAuthenticated()
		{
			if(App.AuthUserProfile == null)
			{
				MessagingCenter.Send<BaseContentPage<T>>(this, "AuthenticateUser");
			}
		}

		protected virtual void OnLoaded()
		{
			
		}

		protected override void OnAppearing()
		{
			if(!HasInitialized)
			{
				HasInitialized = true;
				OnLoaded();
			}

			base.OnAppearing();
		}
	}
}