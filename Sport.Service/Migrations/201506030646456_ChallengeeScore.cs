namespace Sport.Service.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChallengeeScore : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Sport.GameResult", "ChallengerScore", c => c.Int());
            AlterColumn("Sport.GameResult", "ChallengeeScore", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("Sport.GameResult", "ChallengeeScore", c => c.Int(nullable: false));
            AlterColumn("Sport.GameResult", "ChallengerScore", c => c.Int(nullable: false));
        }
    }
}
