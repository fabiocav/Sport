namespace SportChallengeMatchRank.Service.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RulesUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("SportChallengeMatchRank.League", "RulesUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("SportChallengeMatchRank.League", "RulesUrl");
        }
    }
}
