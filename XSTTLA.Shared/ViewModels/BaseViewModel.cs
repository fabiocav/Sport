using System;
using System.Threading;
using System.ComponentModel;
using System.Collections.Generic;

namespace XSTTLA.Shared
{
	public class BaseViewModel : INotifyPropertyChanged
	{
		private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

		public event EventHandler IsBusyChanged;

		bool _isBusy;
		public const string IsBusyPropertyName = "IsBusy";

		public bool IsBusy
		{
			get
			{
				return _isBusy;
			}
			set
			{
				SetProperty(ref _isBusy, value, IsBusyPropertyName);
			}
		}

		protected void SetProperty<T>(ref T backingStore, T value, string propertyName, Action onChanged = null)
		{
			if(EqualityComparer<T>.Default.Equals(backingStore, value))
				return;

			backingStore = value;

			if(onChanged != null)
				onChanged();

			OnPropertyChanged(propertyName);
		}

		public CancellationToken CancellationToken
		{
			get
			{
				return _cancellationTokenSource.Token;
			}
		}

		public virtual void CancelTasks()
		{
			if(!_cancellationTokenSource.IsCancellationRequested && CancellationToken.CanBeCanceled)
			{
				_cancellationTokenSource.Cancel();
				_cancellationTokenSource = new CancellationTokenSource();
			}
		}

		#region INotifyPropertyChanged implementation

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string propertyName)
		{
			if(PropertyChanged == null)
				return;

			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}

	public class Busy : IDisposable
	{
		readonly BaseViewModel _viewModel;

		public Busy(BaseViewModel viewModel)
		{
			_viewModel = viewModel;
			_viewModel.IsBusy = true;
		}

		#region IDisposable implementation

		public void Dispose()
		{
			_viewModel.IsBusy = false;
		}

		#endregion
	}
}