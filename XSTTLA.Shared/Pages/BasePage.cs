using System;
using Xamarin.Forms;

namespace XSTTLA.Shared
{
	public class BasePage<T> : ContentPage where T : BaseViewModel, new()
	{
		public BasePage() : base()
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
	}
}