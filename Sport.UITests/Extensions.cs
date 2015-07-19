﻿using System;
using System.Linq;
using System.Threading;
using Xamarin.UITest;
using Xamarin.UITest.Android;
using Xamarin.UITest.Queries;
using System.Text;
using System.Diagnostics;

namespace Sport.UITests
{
	public class Constants
	{
		public static readonly string ApiKey = "a835baddbb619daf8c09f1e49756e81f";
		public static readonly string Password = "XamarinTestCloud";
	}
}

namespace Xamarin.TestCloud.Extensions
{
	public static class Extensions
	{
		public static void ScrollDownAndTap(this IApp app, Func<AppQuery, AppWebQuery> lambda = null, string screenshot = null)
		{
			app.ScrollDownEnough(lambda);
			app.Tap(lambda);

			if(screenshot != null)
				app.Screenshot(screenshot);
		}

		public static void ScrollDownAndTap(this IApp app, string screenshot, Func<AppQuery, AppWebQuery> lambda)
		{
			app.ScrollDownEnough(lambda);

			if(screenshot != null)
				app.Screenshot(screenshot);

			app.Tap(lambda);
		}

		public static void ScrollUpAndTap(this IApp app, Func<AppQuery, AppWebQuery> lambda = null, string screenshot = null)
		{
			app.ScrollUpEnough(lambda);
			app.Tap(lambda);

			if(screenshot != null)
				app.Screenshot(screenshot);
		}

		public static void ScrollUpAndTap(this IApp app, string screenshot, Func<AppQuery, AppWebQuery> lambda)
		{
			app.ScrollUpEnough(lambda);

			if(screenshot != null)
				app.Screenshot(screenshot);

			app.Tap(lambda);
		}

		public static void ScrollDownAndTap(this IApp app, string marked, string screenshot = null)
		{
			app.ScrollDownAndTap(e => e.Marked(marked), screenshot);
		}

		public static void ScrollDownAndTap(this IApp app, Func<AppQuery, AppQuery> lambda = null, string screenshot = null)
		{
			app.ScrollDownEnough(lambda);
			app.Tap(lambda);

			if(screenshot != null)
				app.Screenshot(screenshot);
		}

		public static void ScrollDownAndTap(this IApp app, string screenshot, Func<AppQuery, AppQuery> lambda)
		{
			app.ScrollDownEnough(lambda);

			if(screenshot != null)
				app.Screenshot(screenshot);

			app.Tap(lambda);
		}

		public static void ScrollUpAndTap(this IApp app, Func<AppQuery, AppQuery> lambda = null, string screenshot = null)
		{
			app.ScrollUpEnough(lambda);
			app.Tap(lambda);

			if(screenshot != null)
				app.Screenshot(screenshot);
		}

		public static void ScrollUpAndTap(this IApp app, string screenshot, Func<AppQuery, AppQuery> lambda)
		{
			app.ScrollUpEnough(lambda);

			if(screenshot != null)
				app.Screenshot(screenshot);

			app.Tap(lambda);
		}

		/// <summary>
		/// Incrementally scrolls down until the desired elements are found
		/// </summary>
		public static AppResult[] ScrollDownEnough(this IApp app, Func<AppQuery, AppQuery> lambda, string screenshot = null)
		{
			AppResult rootView = null;
			int count = 0;
			const int maxTries = 20;

			AppResult[] lastTry;
			while(count < maxTries)
			{
				lastTry = app.Query(lambda);

				if(lastTry.Any())
				{
					if(screenshot != null)
						app.Screenshot(screenshot);

					return lastTry;
				}

				if(rootView == null)
				{
					rootView = app.Query(e => e.All()).FirstOrDefault();

					if(rootView == null)
						throw new Exception("Unable to get root view");
				}

				float gap = rootView.Rect.Height / 3;
				//var write = rootView.Rect.CenterX.ToString() + " - " + (rootView.Rect.CenterY + gap).ToString() + " x " + rootView.Rect.CenterX + " - " + (rootView.Rect.CenterY - gap).ToString();
				app.DragCoordinates(rootView.Rect.CenterX, rootView.Rect.CenterY + gap, rootView.Rect.CenterX, rootView.Rect.CenterY - gap);
				count++;
			}

			if(count == maxTries)
			{
				throw new Exception("Unable to scroll down to find element");
			}

			return new AppResult[0];
		}

		public static AppResult[] ScrollUpEnough(this IApp app, Func<AppQuery, AppQuery> lambda, string screenshot = null)
		{
			AppResult rootView = null;
			int count = 0;
			const int maxTries = 20;

			AppResult[] lastTry;
			while(count < maxTries)
			{
				lastTry = app.Query(lambda);

				if(lastTry.Any())
				{
					if(screenshot != null)
						app.Screenshot(screenshot);

					return lastTry;
				}

				if(rootView == null)
				{
					rootView = app.Query(e => e.All()).FirstOrDefault();

					if(rootView == null)
						throw new Exception("Unable to get root view");
				}

				//Will try to scroll +/-100 from the vertical center point
				float gap = rootView.Rect.Height / 5;
				//app.DragCoordinates(rootView.Rect.CenterX, rootView.Rect.CenterY - gap, rootView.Rect.CenterX, rootView.Rect.CenterY + gap);
				app.ScrollUp();
				count++;
			}

			if(count == maxTries)
			{
				throw new Exception("Unable to scroll down to find element");
			}

			return new AppResult[0];
		}

		public static AppWebResult[] ScrollDownEnough(this IApp app, Func<AppQuery, AppWebQuery> lambda, string screenshot = null)
		{
			AppResult rootView = null;
			int count = 0;
			const int maxTries = 20;

			AppWebResult[] lastTry;
			while(count < maxTries)
			{
				lastTry = app.Query(lambda);

				if(lastTry.Any())
				{
					if(screenshot != null)
						app.Screenshot(screenshot);

					return lastTry;
				}

				if(rootView == null)
				{
					rootView = app.Query(e => e.All()).FirstOrDefault();

					if(rootView == null)
						throw new Exception("Unable to get root view");
				}

				float gap = rootView.Rect.Height / 5;
				Console.WriteLine(rootView.Rect.CenterX.ToString() + " - " + (rootView.Rect.CenterY + gap).ToString() + " x " + rootView.Rect.CenterX + " - " + (rootView.Rect.CenterY - gap).ToString());
				app.DragCoordinates(rootView.Rect.CenterX, rootView.Rect.CenterY + gap, rootView.Rect.CenterX, rootView.Rect.CenterY - gap);
				count++;
			}

			if(count == maxTries)
			{
				throw new Exception("Unable to scroll down to find element");
			}

			return new AppWebResult[0];
		}

		public static AppWebResult[] ScrollUpEnough(this IApp app, Func<AppQuery, AppWebQuery> lambda, string screenshot = null)
		{
			AppResult rootView = null;
			int count = 0;
			const int maxTries = 20;

			AppWebResult[] lastTry;
			while(count < maxTries)
			{
				lastTry = app.Query(lambda);

				if(lastTry.Any())
				{
					if(screenshot != null)
						app.Screenshot(screenshot);

					return lastTry;
				}

				if(rootView == null)
				{
					rootView = app.Query(e => e.All()).FirstOrDefault();

					if(rootView == null)
						throw new Exception("Unable to get root view");
				}

				//Will try to scroll +/-100 from the vertical center point
				float gap = rootView.Rect.Height / 5;
				//app.DragCoordinates(rootView.Rect.CenterX, rootView.Rect.CenterY - gap, rootView.Rect.CenterX, rootView.Rect.CenterY + gap);
				app.ScrollUp();
				count++;
			}

			if(count == maxTries)
			{
				throw new Exception("Unable to scroll down to find element");
			}

			return new AppWebResult[0];
		}

		/// TEMPORARY HACK TO ALLOW LOGGING STRING DATA TO THE DEVICE LOG
		public static void LogToDevice(this IApp app, string text, params object[] formatArgs)
		{
			try
			{
				var finalText = formatArgs.Length > 0 ? string.Format(text, formatArgs) : text;
				Console.WriteLine(finalText);
				app.Invoke("*******Xamarin Log*******", finalText);
			}
			catch(Exception)
			{
			}
		}

		public static void LogToDevice(this IApp app, Func<AppQuery, AppQuery> lambda = null)
		{
			if(lambda == null)
			{
				lambda = e => e.All();
			}

			var results = app.Query(lambda);
			app.LogToDevice(results.ToString(true));
		}

		public static void WaitThenEnterText(this IApp app, Func<AppQuery, AppQuery> lambda, string text, string screenshot = null)
		{
			app.WaitForElement(lambda);
			app.EnterText(lambda, text);

			if(screenshot != null)
				app.Screenshot(screenshot);
		}

		public static void EnterText(this IApp app, string marked, string text, string screenshot)
		{
			app.EnterText(marked, text);
			app.Screenshot(screenshot);
		}

		public static void Tap(this IApp app, string screenshot, string marked)
		{
			app.Screenshot(screenshot);
			app.Tap(marked);
		}

		public static void EnterText(this IApp app, Func<AppQuery, AppWebQuery> lambda, string text, string screenshot)
		{
			app.EnterText(lambda, text);
			app.Screenshot(screenshot);
		}

		public static void EnterText(this IApp app, Func<AppQuery, AppQuery> lambda, string text, string screenshot)
		{
			app.EnterText(lambda, text);
			app.Screenshot(screenshot);
		}

		public static void Tap(this IApp app, string screenshot, Func<AppQuery, AppQuery> lambda)
		{
			app.Screenshot(screenshot);
			app.Tap(lambda);
		}

		public static void Tap(this IApp app, Func<AppQuery, AppQuery> lambda, string screenshot)
		{
			app.Tap(lambda);
			app.Screenshot(screenshot);
		}

		public static void Tap(this IApp app, string screenshot, Func<AppQuery, AppWebQuery> lambda)
		{
			app.Screenshot(screenshot);
			app.Tap(lambda);
		}

		public static void Tap(this IApp app, Func<AppQuery, AppWebQuery> lambda, string screenshot)
		{
			app.Tap(lambda);
			app.Screenshot(screenshot);
		}

		public static void WaitThenTapIfExists(this IApp app, Func<AppQuery, AppQuery> lambda, int timeout = 5, string screenshot = null)
		{
			int count = 0;

			while(count < timeout && app.Query(lambda).Length == 0)
			{
				Thread.Sleep(1000);
				count++;
			}

			if(app.Query(lambda).Length > 0)
			{
				if(screenshot != null)
					app.Screenshot(screenshot);

				app.Tap(lambda);
			}
		}

		public static void WaitThenTap(this IApp app, string screenshot, Func<AppQuery, AppQuery> lambda, int seconds = 20)
		{
			app.WaitForElement(lambda, "Timed out waiting for element", TimeSpan.FromSeconds(seconds));

			if(screenshot != null)
				app.Screenshot(screenshot);

			app.Tap(lambda);
		}

		public static void WaitThenTap(this IApp app, Func<AppQuery, AppQuery> lambda, string screenshot = null, int seconds = 20)
		{
			app.WaitForElement(lambda);
			app.Tap(lambda);

			if(screenshot != null)
				app.Screenshot(screenshot);
		}

		public static string ToString(this AppResult[] result, bool repl)
		{
			var sb = new StringBuilder();
			var index = 0;

			foreach(var res in result)
			{
				var innerSb = new StringBuilder();
				innerSb.AppendLine("{");
				innerSb.AppendLine(string.Format("    Index         - {0}", index));
				innerSb.AppendLine(string.Format("    Class         - {0}", res.Class));
				innerSb.AppendLine(string.Format("    Description   - {0}", res.Description));

				if(res.Text != null)
					innerSb.AppendLine(string.Format("    Text           - {0}", res.Text));

				innerSb.AppendLine(string.Format("    ID            - {0}", res.Id));
				innerSb.AppendLine(string.Format("    Rect          - {0} x {1}, {2} x {3}", res.Rect.X, res.Rect.Y, res.Rect.Width, res.Rect.Height));
				innerSb.AppendLine("}");
				innerSb.AppendLine("");

				sb.Append(innerSb.ToString());
				index++;
			}

			return sb.ToString();
		}
	}
}