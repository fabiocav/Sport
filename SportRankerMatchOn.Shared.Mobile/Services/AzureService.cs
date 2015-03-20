using System;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;

namespace SportRankerMatchOn.Shared.Mobile
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

		async public Task<List<League>> GetAllLeagues()
		{
			var list = await Client.GetTable<League>().ToListAsync();
			return list;
		}

		async public Task<List<League>> GetAllEnabledLeagues()
		{
			var list = await Client.GetTable<League>().Where(l => l.IsEnabled).OrderBy(l => l.Name).ToListAsync();
			return list;
		}

		async public Task<League> GetLeagueByName(string name)
		{
			var list = await Client.GetTable<League>().Where(l => l.Name == name).Take(1).ToListAsync();
			DefaultLeague = list.FirstOrDefault();
			return DefaultLeague;
		}

		async public Task SaveLeague(League league)
		{
			ServicePointManager.ServerCertificateValidationCallback = delegate
				(object sender, System.Security.Cryptography.X509Certificates.X509Certificate pCertificate, System.Security.Cryptography.X509Certificates.X509Chain pChain, System.Net.Security.SslPolicyErrors pSSLPolicyErrors)
			{
				//if (pSSLPolicyErrors == System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch && pCertificate.Issuer == "CN=Entrust Certification Authority - L1C, OU=\"(c) 2009 Entrust, Inc.\", OU=www.entrust.net/rpa is incorporated by reference, O=\"Entrust, Inc.\", C=US")
				{
					return true;
				}
				//if (pSSLPolicyErrors == System.Net.Security.SslPolicyErrors.None)
				//    return true;
				//return false;
			};

			await Client.GetTable<League>().InsertAsync(league);
		}

		async public Task SaveMember(Member member)
		{
			await Client.GetTable<Member>().InsertAsync(member);
		}
	}
}

