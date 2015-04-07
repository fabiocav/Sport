using System;
using System.Threading;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;

namespace SportChallengeMatchRank.Shared
{
	public class BaseViewModel : INotifyPropertyChanged
	{
		bool _isBusy = false;
		public const string IsBusyPropertyName = "IsBusy";
		CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

		public bool IsBusy
		{
			get
			{
				return _isBusy;
			}
			set
			{
				SetProperty(ref _isBusy, value, IsBusyPropertyName);
				OnPropertyChanged("IsNotBusy");
			}
		}

		public bool IsNotBusy
		{
			get
			{
				return !IsBusy;
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

		#region INotifyPropertyChanged implementation

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string propertyName)
		{
			if(PropertyChanged == null)
				return;

			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		#region Task Safety

		public Action<Exception> OnTaskException
		{
			get;
			set;
		}

		public async Task RunSafe(Task task)
		{
			if(!App.IsNetworkRechable)
			{
				if(OnTaskException != null)
					OnTaskException(new WebException("Not connected to the Internet"));

				return;
			}

			Exception exception = null;

			using(new Busy(this))
			{
				try
				{
					if(!CancellationToken.IsCancellationRequested)
					{
						var task2 = Task.Factory.StartNew(() =>
							{
								task.Start();
								task.Wait();
								CancellationToken.ThrowIfCancellationRequested();
							}, CancellationToken);

						await task2;

						Console.WriteLine(task2.IsCanceled);
					}
				}
				catch(TaskCanceledException)
				{
					Console.WriteLine("Task Cancelled");
				}
				catch(AggregateException e)
				{
					var ex = e.InnerException;
					while(ex.InnerException != null)
						ex = ex.InnerException;

					exception = ex;
				}
				catch(Exception e)
				{
					exception = e;
				}
			}

			if(exception != null)
			{
				//TODO Log to Insights
				Console.WriteLine(exception);

				if(OnTaskException != null)
					OnTaskException(exception);
			}
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

		#endregion
	}

	#region Helper Classes


	public class Busy : IDisposable
	{
		readonly BaseViewModel _viewModel;

		public Busy(BaseViewModel viewModel)
		{
			_viewModel = viewModel;
			_viewModel.IsBusy = true;
		}

		public void Dispose()
		{
			_viewModel.IsBusy = false;
		}
	}

	#endregion
}