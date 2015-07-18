﻿using System;
using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using ImageCircle.Forms.Plugin.Droid;
using Sport.Shared;
using XLabs.Forms;
using Android.Content.PM;

namespace Sport.Android
{
	[Activity(Label = "Sport", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/GrayTheme", ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : XFormsApplicationDroid
	{
		protected override void OnCreate(Bundle bundle)
		{
			Xamarin.Insights.Initialize(Constants.InsightsApiKey, this);

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

//				Window window = activity.getWindow();
//				window.addFlags(WindowManager LayoutParams.FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS);
//				window.clearFlags(WindowManager.LayoutParams.FLAG_TRANSLUCENT_STATUS);
//				window.setStatusBarColor(activity.getResources().getColor(R.color.example_color));
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}
		}
	}
}