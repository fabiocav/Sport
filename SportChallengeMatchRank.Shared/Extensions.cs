using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using System.Threading.Tasks;

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

			//TODO move to an IRefreshable interface
			var athlete = model as Athlete;
			if(athlete != null)
			{
				athlete.RefreshChallenges();
				athlete.RefreshMemberships();
			}

			var league = model as League;
			if(league != null)
			{
				league.RefreshMemberships();
			}

			if(dict.ContainsKey(model.Id))
			{
				dict[model.Id] = model;
			}
			else
			{
				dict.TryAdd(model.Id, model);
			}
		}

		public static void ToToast(this string message, ToastNotificationType type, string title = null)
		{
			var notificator = DependencyService.Get<IToastNotifier>();
			notificator.Notify(type, title ?? type.ToString().ToUpper(), message, TimeSpan.FromSeconds(2.5f));
		}
	}
}