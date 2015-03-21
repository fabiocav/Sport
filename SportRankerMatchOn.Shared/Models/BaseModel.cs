using System;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Collections.Generic;

namespace SportRankerMatchOn.Shared
{
	public class BaseModel
	{
		[JsonProperty("id")]
		public string Id
		{
			get;
			set;
		}

		#region INotifyPropertyChanged implementation

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string propertyName)
		{
			if(PropertyChanged == null)
				return;

			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		protected void SetProperty<T>(ref T backingStore, T value, string propertyName, Action onChanged = null)
		{
			if(EqualityComparer<T>.Default.Equals(backingStore, value))
				return;

			backingStore = value;

			if(onChanged != null)
				onChanged();

			OnPropertyChanged(propertyName);
		}
	}
}