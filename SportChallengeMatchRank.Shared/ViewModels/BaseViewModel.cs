using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Xamarin.Forms;
using Xamarin;

namespace SportChallengeMatchRank.Shared
{
	public class BaseViewModel : BaseNotify
	{
		bool _isBusy = false;
		CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

		public bool IsBusy
		{
			get
			{
				return _isBusy;
			}
			set
			{
				SetPropertyChanged(ref _isBusy, value);
				SetPropertyChanged("IsNotBusy");
			}
		}

		public bool IsNotBusy
		{
			get
			{
				return !IsBusy;
			}
		}

		#region Task Safety

		public Action<Exception> OnTaskException
		{
			get;
			set;
		}

		public async Task RunSafe(Task task, bool notifyOnError = true)
		{
			if(!App.IsNetworkRechable)
			{
				if(OnTaskException != null)
					OnTaskException(new WebException("Not connected to the Information Super Highway"));
			}

			Exception exception = null;

			try
			{
				await Task.Run(() =>
				{
					if(!CancellationToken.IsCancellationRequested)
					{
						task.Start();
						task.Wait();
					}
				});
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
			catch(WebException e)
			{
				exception = e;
			}
			catch(Exception e)
			{
				exception = e;
			}

			if(exception != null)
			{
				//TODO Log to Insights
				Insights.Report(exception);
				Console.WriteLine(exception);

				if(notifyOnError)
				{
					MessagingCenter.Send<BaseViewModel, Exception>(this, "ExceptionOccurred", exception);
				}
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
		object _sync = new object();
		readonly BaseViewModel _viewModel;

		public Busy(BaseViewModel viewModel)
		{
			_viewModel = viewModel;
			Device.BeginInvokeOnMainThread(() =>
			{
				lock(_sync)
				{
					Console.WriteLine("Busy");
					_viewModel.IsBusy = true;
				}
			});
		}

		public void Dispose()
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				lock(_sync)
				{
					_viewModel.IsBusy = false;
					Console.WriteLine("Not Busy");
				}
			});
		}
	}

	#endregion
}