using System;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace XSTTLA.Shared
{
	public class AzureService
	{
		static MobileServiceClient _instance;

		public static MobileServiceClient Instance
		{
			get
			{
				if(_instance == null)
				{
					_instance = new MobileServiceClient(Constants.AzureDomain, Constants.AzureClientId);
					CurrentPlatform.Init();
				}

				return _instance;
			}			
		}

		async public Task SaveLeague(League league)
		{
			await Instance.GetTable<League>().InsertAsync(league);
		}
	}
}

