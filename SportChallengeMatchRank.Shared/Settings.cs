using System;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

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
				return _instance ?? (_instance = Settings.Load());
			}
		}

		public string AuthToken
		{
			get;
			set;
		}

		public string RefreshToken
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

		public string DeviceRegistrationId
		{
			get;
			set;
		}

		public static void FizzyBuzzy(int i)
		{
			bool five = i % 5 != 0;
			bool three = i % 3 != 0;

			if(!five)
				Console.Write("fizz");

			if(!three)
				Console.Write("buzz");

			if(five && three)
				Console.Write(i);

			Console.WriteLine();
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

		public static Settings Load()
		{
			Debug.WriteLine(string.Format("Loading settings: {0}", _filePath));
			var settings = Helpers.LoadFromFile<Settings>(_filePath) ?? new Settings();
			return settings;
		}
	}
}

