using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;

namespace SportChallengeMatchRank.Shared
{
	public class Settings
	{
		static Settings _instance;
		static readonly string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "settings.json");

		public static Settings Instance
		{
			get
			{
				if(_instance == null)
					Settings.Load();

				return _instance;
			}
		}

		public string AuthToken
		{
			get;
			set;
		}

		public string AuthUserID
		{
			get;
			set;
		}

		public string AthleteId
		{
			get;
			set;
		}

		public Task Save()
		{
			return Task.Factory.StartNew(() =>
				{
					Debug.WriteLine(string.Format("Saving settings: {0}", _filePath));
					var json = JsonConvert.SerializeObject(this);
					using(var sw = new StreamWriter(_filePath, false))
					{
						sw.Write(json);
					}
				});
		}

		public static void Load()
		{
			Debug.WriteLine(string.Format("Loading settings: {0}", _filePath));
			_instance = Helpers.LoadFromFile<Settings>(_filePath);

			if(_instance == null)
				_instance = new Settings();
		}
	}
}

