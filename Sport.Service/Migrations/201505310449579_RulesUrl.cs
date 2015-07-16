namespace Sport.Service.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RulesUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("Sport.League", "RulesUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("Sport.League", "RulesUrl");
        }
    }
}
