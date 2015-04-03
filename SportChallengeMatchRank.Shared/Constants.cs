using System;

namespace SportChallengeMatchRank.Shared
{
	public class Constants
	{
		public static readonly string AuthType = "google-oauth2";
		public static readonly string AuthClientId = "5Qf0iIssIZ7Km9Fiwd041uxbfVdtyAqP";
		public static readonly string AuthDomain = "xsttla.auth0.com";

		#if DEBUG
		public static readonly string AzureDomain = "http://192.168.1.19:51541/";
		//public static readonly string AzureDomain = "http://10.0.0.217:51541/";
		#else
		public static readonly string AzureDomain = "https://sportchallengematchrank.azure-mobile.net/";
		#endif

		public static readonly string GoogleApiClientId = "AIzaSyC_7O_tpjYGywqB7pPDuzuyUZiYKGZADF4";
		public static readonly string AzureClientId = "FSKOHcURfLmRDDKzUdZBuOxpmAlQyc62";
	}
}