using System;
using Sport.Shared;
using Xamarin.Forms;
using System.Threading.Tasks;
using Android.Widget;
using Android.Support.Design.Widget;
using Android.App;

[assembly: Dependency(typeof(Sport.Android.ToastNotifier))]

namespace Sport.Android
{
	public class ToastNotifier : IToastNotifier
	{
		#region IToastNotifier implementation

		public Task<bool> Notify(ToastNotificationType type, string title, string description, TimeSpan duration, object context = null)
		{
			var taskCompletionSource = new TaskCompletionSource<bool>();

//			global::Android.Views.View parent = ((Activity)Forms.Context).Window.DecorView.FindViewById(16908290);
//			Snackbar.Make(parent, description, Snackbar.LengthLong).Show();
			Toast.MakeText(Forms.Context, description, ToastLength.Short).Show();

			return taskCompletionSource.Task;
		}

		public void HideAll()
		{
		}

		#endregion
	}
}