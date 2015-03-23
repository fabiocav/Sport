﻿using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SportRankerMatchOn
{
	public partial class AthleteBase : EntityData
	{
		public string Name
		{
			get;
			set;
		}

		public string AuthenticationId
		{
			get;
			set;
		}

		public string Email
		{
			get;
			set;
		}

		public bool IsAdmin
		{
			get;
			set;
		}
	}
}