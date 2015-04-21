using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using SportChallengeMatchRank.Shared;

namespace SportChallengeMatchRank.Shared
{
	public class BaseNotify : INotifyPropertyChanged, IDisposable
	{
		readonly Dictionary<string, List<Action>> _actions = new Dictionary<string, List<Action>>();

		public event PropertyChangedEventHandler PropertyChanged;

		public BaseNotify()
		{
			PropertyChanged += OnPropertyChanged;
		}

		public virtual void Dispose()
		{
			ClearEvents();
		}

		internal bool SetPropertyChanged<T>(ref T currentValue, T newValue, [CallerMemberName] string propertyName = "")
		{
			return PropertyChanged.SetProperty(this, ref currentValue, newValue, propertyName);
		}

		internal void SetPropertyChanged(string propertyName)
		{
			if(PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			List<Action> actionList;
			if(!_actions.TryGetValue(propertyChangedEventArgs.PropertyName, out actionList))
				return;

			foreach(Action action in actionList)
			{
				action();
			}
		}

		public void SubscribeToProperty(string property, Action action)
		{
			List<Action> actionList;
			if(!_actions.TryGetValue(property, out actionList))
				actionList = new List<Action>();

			actionList.Add(action);
			_actions[property] = actionList;
		}

		public void UnSubscribeToProperty(string property, Action action)
		{
			List<Action> actionList;
			if(!_actions.TryGetValue(property, out actionList))
				return;

			if(actionList.Contains(action))
				actionList.Remove(action);

			_actions[property] = actionList;
		}

		public void UnSubscribeToProperty(string property)
		{
			List<Action> actionList;
			if(!_actions.TryGetValue(property, out actionList))
				return;

			actionList.Clear();
			_actions[property] = actionList;
		}

		public void ClearEvents()
		{
			_actions.Clear();
			if(PropertyChanged == null)
				return;

			var invocation = PropertyChanged.GetInvocationList();
			foreach(var p in invocation)
				PropertyChanged -= (PropertyChangedEventHandler)p;
		}
	}

	public interface IDirty
	{
		bool IsDirty
		{
			get;
			set;
		}
	}
}

namespace System.ComponentModel
{
	public static class BaseNotify
	{
		public static bool SetProperty<T>(this PropertyChangedEventHandler handler, object sender, ref T currentValue, T newValue, [CallerMemberName] string propertyName = "")
		{
			if(EqualityComparer<T>.Default.Equals(currentValue, newValue))
				return false;
			
			currentValue = newValue;

			var dirty = sender as IDirty;

			if(dirty != null)
				dirty.IsDirty = true;

			if(handler == null)
				return true;

			handler.Invoke(sender, new PropertyChangedEventArgs(propertyName));
			return true;
		}
	}
}