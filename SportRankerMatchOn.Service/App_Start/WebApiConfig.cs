using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using SportRankerMatchOn.Service.DataObjects;
using SportRankerMatchOn.Service.Models;

namespace SportRankerMatchOn.Service
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));

            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            // config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            
            Database.SetInitializer(new SportRanker_MatchOnInitializer());
        }
    }

    public class SportRanker_MatchOnInitializer : ClearDatabaseSchemaIfModelChanges<SportRankerMatchOnContext>
    {
        protected override void Seed(SportRankerMatchOnContext context)
        {
            List<Member> Members = new List<Member>
            {
                new Member { Id = Guid.NewGuid().ToString(), Text = "First item", Complete = false },
                new Member { Id = Guid.NewGuid().ToString(), Text = "Second item", Complete = false },
            };

            foreach (Member Member in Members)
            {
                context.Set<Member>().Add(Member);
            }

            base.Seed(context);
        }
    }
}

