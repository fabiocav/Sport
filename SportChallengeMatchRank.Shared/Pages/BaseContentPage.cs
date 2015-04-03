using System;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace SportChallengeMatchRank.Shared
{
	public class BaseContentPage<T> : ContentPage where T : BaseViewModel, new()
	{
		public BaseContentPage() : base()
		{
			BindingContext = ViewModel;
			Initialize();

			Application.Current.ModalPopped += async(sender, e) =>
			{
				if(e.Modal is AuthenticationPage && App.CurrentAthlete != null)
				{
					OnUserAuthenticated();
				}
			};
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

		protected virtual void OnUserAuthenticated()
		{

		}

		protected virtual void Initialize()
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