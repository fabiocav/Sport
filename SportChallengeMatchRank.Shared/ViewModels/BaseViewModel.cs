using System;
using System.Threading;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading.Tasks;

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

		#region Task Safety

		public Action<TaskOutcomeBase> OnTaskException
		{
			get;
			set;
		}

		async public Task<TaskOutcome<T>> RunSafe<T>(Func<T> func, bool notifyOnException = true)
		{
			var outcome = new TaskOutcome<T> {
				Task = new Task<T>(func),
				NotifyOnException = notifyOnException
			};
		
			await RunSafeInternal(outcome.Task, outcome);
			return outcome;
		}

		async public Task<TaskOutcome> RunSafe(Action action, bool notifyOnException = true)
		{
			var outcome = new TaskOutcome {
				Task = new Task(action),
				NotifyOnException = notifyOnException
			};

			await RunSafeInternal(outcome.Task, outcome);
			return outcome;
		}

		async Task<TaskOutcome> RunSafe(Task task, bool notifyOnException = true)
		{
			var outcome = new TaskOutcome {
				Task = task,
				NotifyOnException = notifyOnException
			};

			await RunSafeInternal(task, outcome);
			return outcome;
		}

		async public Task<TaskOutcome<T>> RunSafe<T>(Task<T> task, bool notifyOnException = true)
		{
			var outcome = new TaskOutcome<T> {
				Task = task,
				NotifyOnException = notifyOnException
			};
		
			await RunSafeInternal(task, outcome);
			return outcome;
		}

		async Task RunSafeInternal(Task task, TaskOutcomeBase outcome)
		{
			if(!App.IsNetworkRechable)
			{
				outcome.Condition = TaskResult.Failure;
				outcome.Exception = new Exception("Not connected to network");

				if(OnTaskException != null)
					OnTaskException(outcome);

				return;
			}

			using(new Busy(this))
			{
				try
				{
					task.Start();
					task.Wait();

					if(task.Exception != null)
					{
					}

					outcome.Condition = TaskResult.Success;
				}
				catch(TaskCanceledException)
				{
					Console.WriteLine("Task Cancelled");
					outcome.Condition = TaskResult.Cancelled;
				}
				catch(Exception e)
				{
					outcome.Condition = TaskResult.Failure;
					outcome.Exception = e;
					Console.WriteLine(e);

					if(OnTaskException != null)
						OnTaskException(outcome);
				}
			}
		}

		#endregion
	}

	#region Helper Classes

	public class TaskOutcomeBase
	{
		public TaskOutcomeBase()
		{
			NotifyOnException = true;
		}

		public TaskResult Condition
		{
			get;
			set;
		}

		public Exception Exception
		{
			get;
			set;
		}

		/// <summary>
		/// Set to false for background methods not necessarily invoked by the user
		/// </summary>
		public bool NotifyOnException
		{
			get;
			set;
		}
	}

	public class TaskOutcome : TaskOutcomeBase
	{
		public Task Task
		{
			get;
			set;
		}
	}

	public class TaskOutcome<T> : TaskOutcomeBase
	{
		public Task<T> Task
		{
			get;
			set;
		}

		public T Result
		{
			get
			{
				return Condition == TaskResult.Success ? Task.Result : default(T);
			}
		}
	}

	public enum TaskResult
	{
		None,
		Success,
		Failure,
		Cancelled,
	}

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