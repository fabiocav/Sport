﻿using System;
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
				Debug.WriteLine(e.GetBaseException());
				throw;
			}

			AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) =>
			{
				Debug.WriteLine(((Exception)e.ExceptionObject).GetBaseException());
			};

		}
	}
}