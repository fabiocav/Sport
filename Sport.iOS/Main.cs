using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using System.Diagnostics;

namespace Sport.iOS
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main(string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			try
			{
				UIApplication.Main(args, null, "AppDelegate");
			}
			catch(Exception e)
			{
				var ex = e.GetBaseException();
				Console.WriteLine("**SPORT MAIN EXCEPTION**\n\n" + ex);
				Xamarin.Insights.Report(ex, Xamarin.Insights.Severity.Critical);
				throw;
			}

			AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
			{
				try
				{
					var ex = ((Exception)e.ExceptionObject).GetBaseException();
					Console.WriteLine("**SPORT UNHANDLED EXCEPTION**\n\n" + ex);
					Xamarin.Insights.Report(ex, Xamarin.Insights.Severity.Critical);
				}
				catch
				{
				}
			};
		}
	}
}