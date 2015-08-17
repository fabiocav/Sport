using System;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using ImageCircle.Forms.Plugin.Droid;
using Sport.Shared;
using Xamarin.Forms.Platform.Android;
using System.Diagnostics;

[assembly:Xamarin.Forms.Dependency(typeof(Sport.Android.MainActivity))]

namespace Sport.Android
{
	[Activity(Label = "Sport", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/GrayTheme", ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : FormsApplicationActivity
	{
		public static bool IsRunning
		{
			get;
			private set;
		}

		protected override void OnCreate(global::Android.OS.Bundle bundle)
		{
			Xamarin.Insights.Initialize(Keys.InsightsApiKey, this);

			AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
			{
				try
				{
					var ex = ((Exception)e.ExceptionObject).GetBaseException();
					Console.WriteLine("**SPORT MAIN ACTIVITY EXCEPTION**\n\n" + ex);
					Xamarin.Insights.Report(ex, Xamarin.Insights.Severity.Critical);
				}
				catch
				{
				}
			};

			try
			{
				base.OnCreate(bundle);
				Window.SetSoftInputMode(SoftInput.AdjustPan);
				Xamarin.Forms.Forms.Init(this, bundle);
				ImageCircleRenderer.Init();

				Xamarin.Forms.Forms.ViewInitialized += (sender, e) =>
				{
					if(!string.IsNullOrWhiteSpace(e.View.StyleId))
					{
						e.NativeView.ContentDescription = e.View.StyleId;
					}
				};

				LoadApplication(new App());

				var color = new ColorDrawable(Color.Transparent);
				ActionBar.SetIcon(color);
			}
			catch(Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e);
			}
		}

		protected override void OnPause()
		{
			Debug.WriteLine("PAUSE");
			IsRunning = false;
			base.OnPause();
		}

		protected override void OnResume()
		{
			Debug.WriteLine("RESUME");
			IsRunning = true;
			base.OnResume();
		}

		protected override void OnStop()
		{
			Debug.WriteLine("STOP");
			base.OnStop();
		}

		protected override void OnStart()
		{
			Debug.WriteLine("START");
			base.OnStart();
		}
	}
}