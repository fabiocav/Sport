using System;

namespace SportRankerMatchOn.Shared
{
	public class Constants
	{
		public static readonly string AuthType = "google-oauth2";
		public static readonly string AuthClientId = "5Qf0iIssIZ7Km9Fiwd041uxbfVdtyAqP";
		public static readonly string AuthDomain = "xsttla.auth0.com";

		#if DEBUG
		public static readonly string AzureDomain = "http://192.168.1.19:51541/";
		#else
		public static readonly string AzureDomain = "https://sportsranker-matchon.azure-mobile.net/";
		#endif

		public static readonly string AzureClientId = "ECLOfrQpIrqSxhKseSPAUtXoEsKYfd70";
	}
}