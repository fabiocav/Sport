namespace SportChallengeMatchRank.Service.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChallengeeScore : DbMigration
    {
        public override void Up()
        {
            AlterColumn("SportChallengeMatchRank.GameResult", "ChallengerScore", c => c.Int());
            AlterColumn("SportChallengeMatchRank.GameResult", "ChallengeeScore", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("SportChallengeMatchRank.GameResult", "ChallengeeScore", c => c.Int(nullable: false));
            AlterColumn("SportChallengeMatchRank.GameResult", "ChallengerScore", c => c.Int(nullable: false));
        }
    }
}
