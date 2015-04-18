using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace SportChallengeMatchRank.Shared
{
	public class BaseViewModel : BaseModel
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
				ProcPropertyChanged(ref _isBusy, value);
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

		public async Task RunSafe(Task task)
		{
			if(!App.IsNetworkRechable)
			{
				if(OnTaskException != null)
					OnTaskException(new WebException("Not connected to the Internet"));

				return;
			}

			Exception exception = null;

			try
			{
				if(!CancellationToken.IsCancellationRequested)
				{
					using(new Busy(this))
					{
						task.Start();
						task.Wait();
					}
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