using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;

namespace SportChallengeMatchRank.Shared
{
	public static partial class Extensions
	{
		public static void RemoveModel<T>(this ObservableCollection<T> items, string itemId) where T : BaseModel
		{
			items.Where(m => m.Id == itemId).ToList().ForEach(m => items.Remove(m));
		}

		public static void RemoveModel<T>(this ObservableCollection<T> items, T item) where T : BaseModel
		{
			items.RemoveModel(item.Id);
		}

		public static T Get<T>(this ConcurrentDictionary<string, T> dict, string id) where T : BaseModel
		{
			if(id == null)
				return null;

			T v = null;
			dict.TryGetValue(id, out v);
			return v;
		}

		public static void AddOrUpdate<T>(this ConcurrentDictionary<string, T> dict, T model) where T : BaseModel
		{
			if(model == null)
				return;
			
			if(dict.ContainsKey(model.Id))
			{
				dict[model.Id] = model;
			}
			else
			{
				dict.TryAdd(model.Id, model);
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