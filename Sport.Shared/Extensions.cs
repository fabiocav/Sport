using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections;

namespace Sport.Shared
{
	public static partial class Extensions
	{
		public static void EnsureLeaguesThemed(this IList<League> leagues, bool reset = false)
		{
			foreach(var l in leagues)
			{
				App.Current.GetTheme(l, reset);
			}
		}

		public static void Sort<T>(this ObservableCollection<T> collection, IComparer<T> comparer)
		{
			List<T> sorted = collection.ToList();
			sorted.Sort(comparer);

			for(int i = 0; i < sorted.Count(); i++)
				collection.Move(collection.IndexOf(sorted[i]), i);
		}

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

			//TODO move to an IRefreshable interface
			var athlete = model as Athlete;
			if(athlete != null)
			{
				athlete.RefreshMemberships();
			}

			var league = model as League;
			if(league != null)
			{
				league.RefreshMemberships();
				league.RefreshChallenges();
			}

			if(dict.ContainsKey(model.Id))
			{
				if(!model.Equals(dict[model.Id]))
					dict[model.Id] = model;
			}
			else
			{
				dict.TryAdd(model.Id, model);
			}
		}

		public static void ToToast(this string message, ToastNotificationType type = ToastNotificationType.Info, string title = null)
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				var notificator = DependencyService.Get<IToastNotifier>();
				notificator.Notify(type, title ?? type.ToString().ToUpper(), message, TimeSpan.FromSeconds(2.5f));
			});
		}
	}
}