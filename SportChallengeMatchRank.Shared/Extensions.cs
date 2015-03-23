using System;
using System.Threading;
using System.Collections.Generic;

namespace SportChallengeMatchRank.Shared
{
	public static class Extensions
	{
		public static T Get<T>(this Dictionary<string, T> dict, string id) where T : BaseModel
		{
			T v = null;
			dict.TryGetValue(id, out v);
			return v;
		}

		public static void AddOrUpdate<T>(this Dictionary<string, T> dict, T model) where T : BaseModel
		{
			if(dict.ContainsKey(model.Id))
			{
				dict[model.Id] = model;
			}
			else
			{
				dict.Add(model.Id, model);
			}
		}

		public static bool ContainsNoCase(this string s, string contains)
		{
			if(s == null || contains == null)
				return false;

			return s.IndexOf(contains, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		public static string TrimStart(this string s, string toTrim)
		{
			if(s.StartsWith(toTrim, true, Thread.CurrentThread.CurrentCulture))
				return s.Substring(toTrim.Length);

			return s;
		}

		public static string TrimEnd(this string s, string toTrim)
		{
			if(s.EndsWith(toTrim, true, Thread.CurrentThread.CurrentCulture))
				return s.Substring(0, s.Length - toTrim.Length);

			return s;
		}

		public static string Fmt(this string s, params object[] args)
		{
			return string.Format(s, args);
		}
	}
}