﻿using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using Toasts.Forms.Plugin.Abstractions;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

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

			ViewModel.OnTaskException = async(exception) =>
			{
				var msg = exception.Message;
				var mse = exception as MobileServiceInvalidOperationException;
				if(mse != null)
				{
					var body = await mse.Response.Content.ReadAsStringAsync();
					var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);
					var error = dict["message"].ToString();
					error.ToToast(ToastNotificationType.Warning, "Doh!");
					return;
				}

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