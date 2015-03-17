using Microsoft.WindowsAzure.Mobile.Service;

namespace SportRankerMatchOn.Service.DataObjects
{
	public class Member : EntityData
	{
		public string Text
		{
			get;
			set;
		}

		public bool Complete
		{
			get;
			set;
		}
	}
}