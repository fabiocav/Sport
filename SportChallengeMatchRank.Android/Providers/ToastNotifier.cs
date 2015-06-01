using System;
using SportChallengeMatchRank.Shared;
using Xamarin.Forms;
using System.Threading.Tasks;
using Android.Widget;
using Android.Support.Design.Widget;

[assembly: Dependency(typeof(SportChallengeMatchRank.Android.ToastNotifier))]

namespace SportChallengeMatchRank.Android
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