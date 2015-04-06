using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using Toasts.Forms.Plugin.Abstractions;

namespace SportChallengeMatchRank.Shared
{
	public class BaseContentPage<T> : ContentPage where T : BaseViewModel, new()
	{
		public BaseContentPage()
		{
			BindingContext = ViewModel;

			Application.Current.ModalPopped += (sender, e) =>
			{
				if(e.Modal is AuthenticationPage && App.CurrentAthlete != null)
				{
					OnUserAuthenticated();
				}
			};

			ViewModel.OnTaskException = (outcome) =>
			{
				if(!outcome.NotifyOnException)
					return;

				var msg = outcome.Exception.Message;

				if(msg.Length > 300)
					msg = msg.Substring(0, 300);

				msg.ToToast(ToastNotificationType.Error, "Something bad happened");
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