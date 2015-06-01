using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using SportChallengeMatchRank;
using SportChallengeMatchRank.Service.Models;
using Microsoft.WindowsAzure.Mobile.Service.Security.Providers;
using System.Data.Entity.Migrations;
using SportChallengeMatchRank.Service.Migrations;

namespace SportChallengeMatchRank.Service
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();
			//options.LoginProviders.Remove(typeof(GoogleLoginAuthenticationProvider));
			//options.LoginProviders.Add(typeof(GoogleLoginAuthenticationProvider));

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));
			//config.SetIsHosted(true);
            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            // config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            
            //Database.SetInitializer(new SportRanker_MatchOnInitializer());
			var migrator = new DbMigrator(new Configuration());
			migrator.Update();
		}
    }

    public class SportRanker_MatchOnInitializer : ClearDatabaseSchemaIfModelChanges<AppDataContext>
    {
        protected override void Seed(AppDataContext context)
        {
            base.Seed(context);
        }
    }
}

