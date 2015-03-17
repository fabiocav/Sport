using System;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

namespace XSTTLA.Shared
{
	public class AzureService
	{
		static AzureService _instance;

		public static AzureService Instance
		{
			get
			{
				return _instance ?? (_instance = new AzureService());
			}
		}

		public League DefaultLeague
		{
			get;
			set;
		}

		MobileServiceClient _client;

		public MobileServiceClient Client
		{
			get
			{
				if(_client == null)
				{
					_client = new MobileServiceClient(Constants.AzureDomain, Constants.AzureClientId);
					CurrentPlatform.Init();
				}

				return _client;
			}			
		}

		async public Task<League> GetLeagueByName(string name)
		{
			var list = await Client.GetTable<League>().Where(l => l.Name == name).Take(1).ToListAsync();
			DefaultLeague = list.FirstOrDefault();
			return DefaultLeague;
		}

		async public Task SaveLeague(League league)
		{
			await Client.GetTable<League>().InsertAsync(league);
		}
	}
}

