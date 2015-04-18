using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace SportChallengeMatchRank.Shared
{
	public class BaseModel : INotifyPropertyChanged, IDisposable
	{
		readonly Dictionary<string, List<Action>> _actions = new Dictionary<string, List<Action>>();

		public event PropertyChangedEventHandler PropertyChanged;

		string _id;

		[JsonProperty("id")]
		public string Id
		{
			get
			{
				return _id;
			}
			set
			{
				ProcPropertyChanged(ref _id, value);
			}
		}

		DateTime? _dateCreated;

		public DateTime? DateCreated
		{
			get
			{
				return _dateCreated;
			}
			set
			{
				ProcPropertyChanged(ref _dateCreated, value);
			}
		}

		public BaseModel()
		{
			PropertyChanged += OnPropertyChanged;
		}

		public void Dispose()
		{
			ClearEvents();
		}

		internal bool ProcPropertyChanged<T>(ref T currentValue, T newValue, [CallerMemberName] string propertyName = "")
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