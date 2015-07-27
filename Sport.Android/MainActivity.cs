﻿using System;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using ImageCircle.Forms.Plugin.Droid;
using Sport.Shared;
using Xamarin.Forms.Platform.Android;

[assembly:Xamarin.Forms.Dependency(typeof(Sport.Android.MainActivity))]

namespace Sport.Android
{
	[Activity(Label = "Sport", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/GrayTheme", ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : FormsApplicationActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			Xamarin.Insights.Initialize(Keys.InsightsApiKey, this);

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
				Console.WriteLine(e);
			}
		}
	}
}