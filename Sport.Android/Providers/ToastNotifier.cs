using System;
using Sport.Shared;
using Xamarin.Forms;
using System.Threading.Tasks;
using Android.Widget;
using Android.Support.Design.Widget;

[assembly: Dependency(typeof(Sport.Android.ToastNotifier))]

namespace Sport.Android
{
	public class ToastNotifier : IToastNotifier
	{
		#region IToastNotifier implementation

		public Task<bool> Notify(ToastNotificationType type, string title, string description, TimeSpan duration, object context = null)
		{
			var taskCompletionSource = new TaskCompletionSource<bool>();

			//var snackBar = new SnackBar();
			Toast.MakeText(Forms.Context, description, ToastLength.Short).Show();

			return taskCompletionSource.Task;
		}

		public void HideAll()
		{
		}

		#endregion
	}
}