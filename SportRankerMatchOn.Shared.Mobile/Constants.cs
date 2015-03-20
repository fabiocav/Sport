using System;

namespace SportRankerMatchOn.Shared.Mobile
{
	public class Constants
	{
		public static string AuthType = "google-oauth2";
		public static string AuthClientId = "5Qf0iIssIZ7Km9Fiwd041uxbfVdtyAqP";
		public static string AuthDomain = "SportRankerMatchOn.auth0.com";

		#if DEBUG
		public static string AzureDomain = "http://192.168.1.19:51541/";
		#else
		public static string AzureDomain = "https://sportsranker-matchon.azure-mobile.net/";
		#endif

		public static string AzureClientId = "ECLOfrQpIrqSxhKseSPAUtXoEsKYfd70";
	}
}