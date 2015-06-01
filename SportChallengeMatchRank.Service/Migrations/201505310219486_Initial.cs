namespace SportChallengeMatchRank.Service.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "SportChallengeMatchRank.Athlete",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "Id")
                                },
                            }),
                        Name = c.String(),
                        AuthenticationId = c.String(),
                        Email = c.String(),
                        Alias = c.String(),
                        IsAdmin = c.Boolean(nullable: false),
                        DeviceToken = c.String(),
                        DevicePlatform = c.String(),
                        NotificationRegistrationId = c.String(),
                        ProfileImageUrl = c.String(),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion",
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "Version")
                                },
                            }),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "CreatedAt")
                                },
                            }),
                        UpdatedAt = c.DateTimeOffset(precision: 7,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "UpdatedAt")
                                },
                            }),
                        Deleted = c.Boolean(nullable: false,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "Deleted")
                                },
                            }),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.CreatedAt, clustered: true);
            
            CreateTable(
                "SportChallengeMatchRank.Challenge",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "Id")
                                },
                            }),
                        LeagueId = c.String(maxLength: 128),
                        ChallengerAthleteId = c.String(nullable: false, maxLength: 128),
                        ChallengeeAthleteId = c.String(nullable: false, maxLength: 128),
                        CustomMessage = c.String(),
                        ProposedTime = c.DateTimeOffset(precision: 7),
                        DateAccepted = c.DateTimeOffset(precision: 7),
                        DateCompleted = c.DateTimeOffset(precision: 7),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion",
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "Version")
                                },
                            }),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "CreatedAt")
                                },
                            }),
                        UpdatedAt = c.DateTimeOffset(precision: 7,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "UpdatedAt")
                                },
                            }),
                        Deleted = c.Boolean(nullable: false,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "Deleted")
                                },
                            }),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("SportChallengeMatchRank.Athlete", t => t.ChallengeeAthleteId)
                .ForeignKey("SportChallengeMatchRank.Athlete", t => t.ChallengerAthleteId)
                .ForeignKey("SportChallengeMatchRank.League", t => t.LeagueId)
                .Index(t => t.LeagueId)
                .Index(t => t.ChallengerAthleteId)
                .Index(t => t.ChallengeeAthleteId)
                .Index(t => t.CreatedAt, clustered: true);
            
            CreateTable(
                "SportChallengeMatchRank.League",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "Id")
                                },
                            }),
                        Name = c.String(),
                        Description = c.String(),
                        Season = c.Int(nullable: false),
                        MaxChallengeRange = c.Int(nullable: false),
                        MinHoursBetweenChallenge = c.Int(nullable: false),
                        MatchGameCount = c.Int(nullable: false),
                        Sport = c.String(),
                        IsAcceptingMembers = c.Boolean(nullable: false),
                        IsEnabled = c.Boolean(nullable: false),
                        StartDate = c.DateTimeOffset(precision: 7),
                        EndDate = c.DateTimeOffset(precision: 7),
                        HasStarted = c.Boolean(nullable: false),
                        ImageUrl = c.String(),
                        CreatedByAthleteId = c.String(maxLength: 128),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion",
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "Version")
                                },
                            }),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "CreatedAt")
                                },
                            }),
                        UpdatedAt = c.DateTimeOffset(precision: 7,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "UpdatedAt")
                                },
                            }),
                        Deleted = c.Boolean(nullable: false,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "Deleted")
                                },
                            }),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("SportChallengeMatchRank.Athlete", t => t.CreatedByAthleteId)
                .Index(t => t.CreatedByAthleteId)
                .Index(t => t.CreatedAt, clustered: true);
            
            CreateTable(
                "SportChallengeMatchRank.Membership",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "Id")
                                },
                            }),
                        AthleteId = c.String(nullable: false, maxLength: 128),
                        LeagueId = c.String(nullable: false, maxLength: 128),
                        CurrentRank = c.Int(nullable: false),
                        IsAdmin = c.Boolean(nullable: false),
                        LastRankChange = c.DateTimeOffset(precision: 7),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion",
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "Version")
                                },
                            }),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "CreatedAt")
                                },
                            }),
                        UpdatedAt = c.DateTimeOffset(precision: 7,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "UpdatedAt")
                                },
                            }),
                        Deleted = c.Boolean(nullable: false,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "Deleted")
                                },
                            }),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("SportChallengeMatchRank.Athlete", t => t.AthleteId, cascadeDelete: true)
                .ForeignKey("SportChallengeMatchRank.League", t => t.LeagueId, cascadeDelete: true)
                .Index(t => t.AthleteId)
                .Index(t => t.LeagueId)
                .Index(t => t.CreatedAt, clustered: true);
            
            CreateTable(
                "SportChallengeMatchRank.GameResult",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "Id")
                                },
                            }),
                        ChallengeId = c.String(nullable: false, maxLength: 128),
                        Index = c.Int(nullable: false),
                        ChallengerScore = c.Int(nullable: false),
                        ChallengeeScore = c.Int(nullable: false),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion",
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "Version")
                                },
                            }),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "CreatedAt")
                                },
                            }),
                        UpdatedAt = c.DateTimeOffset(precision: 7,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "UpdatedAt")
                                },
                            }),
                        Deleted = c.Boolean(nullable: false,
                            annotations: new Dictionary<string, AnnotationValues>
                            {
                                { 
                                    "ServiceTableColumn",
                                    new AnnotationValues(oldValue: null, newValue: "Deleted")
                                },
                            }),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("SportChallengeMatchRank.Challenge", t => t.ChallengeId, cascadeDelete: true)
                .Index(t => t.ChallengeId)
                .Index(t => t.CreatedAt, clustered: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("SportChallengeMatchRank.GameResult", "ChallengeId", "SportChallengeMatchRank.Challenge");
            DropForeignKey("SportChallengeMatchRank.Challenge", "LeagueId", "SportChallengeMatchRank.League");
            DropForeignKey("SportChallengeMatchRank.Membership", "LeagueId", "SportChallengeMatchRank.League");
            DropForeignKey("SportChallengeMatchRank.Membership", "AthleteId", "SportChallengeMatchRank.Athlete");
            DropForeignKey("SportChallengeMatchRank.League", "CreatedByAthleteId", "SportChallengeMatchRank.Athlete");
            DropForeignKey("SportChallengeMatchRank.Challenge", "ChallengerAthleteId", "SportChallengeMatchRank.Athlete");
            DropForeignKey("SportChallengeMatchRank.Challenge", "ChallengeeAthleteId", "SportChallengeMatchRank.Athlete");
            DropIndex("SportChallengeMatchRank.GameResult", new[] { "CreatedAt" });
            DropIndex("SportChallengeMatchRank.GameResult", new[] { "ChallengeId" });
            DropIndex("SportChallengeMatchRank.Membership", new[] { "CreatedAt" });
            DropIndex("SportChallengeMatchRank.Membership", new[] { "LeagueId" });
            DropIndex("SportChallengeMatchRank.Membership", new[] { "AthleteId" });
            DropIndex("SportChallengeMatchRank.League", new[] { "CreatedAt" });
            DropIndex("SportChallengeMatchRank.League", new[] { "CreatedByAthleteId" });
            DropIndex("SportChallengeMatchRank.Challenge", new[] { "CreatedAt" });
            DropIndex("SportChallengeMatchRank.Challenge", new[] { "ChallengeeAthleteId" });
            DropIndex("SportChallengeMatchRank.Challenge", new[] { "ChallengerAthleteId" });
            DropIndex("SportChallengeMatchRank.Challenge", new[] { "LeagueId" });
            DropIndex("SportChallengeMatchRank.Athlete", new[] { "CreatedAt" });
            DropTable("SportChallengeMatchRank.GameResult",
                removedColumnAnnotations: new Dictionary<string, IDictionary<string, object>>
                {
                    {
                        "CreatedAt",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "CreatedAt" },
                        }
                    },
                    {
                        "Deleted",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "Deleted" },
                        }
                    },
                    {
                        "Id",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "Id" },
                        }
                    },
                    {
                        "UpdatedAt",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "UpdatedAt" },
                        }
                    },
                    {
                        "Version",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "Version" },
                        }
                    },
                });
            DropTable("SportChallengeMatchRank.Membership",
                removedColumnAnnotations: new Dictionary<string, IDictionary<string, object>>
                {
                    {
                        "CreatedAt",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "CreatedAt" },
                        }
                    },
                    {
                        "Deleted",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "Deleted" },
                        }
                    },
                    {
                        "Id",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "Id" },
                        }
                    },
                    {
                        "UpdatedAt",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "UpdatedAt" },
                        }
                    },
                    {
                        "Version",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "Version" },
                        }
                    },
                });
            DropTable("SportChallengeMatchRank.League",
                removedColumnAnnotations: new Dictionary<string, IDictionary<string, object>>
                {
                    {
                        "CreatedAt",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "CreatedAt" },
                        }
                    },
                    {
                        "Deleted",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "Deleted" },
                        }
                    },
                    {
                        "Id",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "Id" },
                        }
                    },
                    {
                        "UpdatedAt",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "UpdatedAt" },
                        }
                    },
                    {
                        "Version",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "Version" },
                        }
                    },
                });
            DropTable("SportChallengeMatchRank.Challenge",
                removedColumnAnnotations: new Dictionary<string, IDictionary<string, object>>
                {
                    {
                        "CreatedAt",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "CreatedAt" },
                        }
                    },
                    {
                        "Deleted",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "Deleted" },
                        }
                    },
                    {
                        "Id",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "Id" },
                        }
                    },
                    {
                        "UpdatedAt",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "UpdatedAt" },
                        }
                    },
                    {
                        "Version",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "Version" },
                        }
                    },
                });
            DropTable("SportChallengeMatchRank.Athlete",
                removedColumnAnnotations: new Dictionary<string, IDictionary<string, object>>
                {
                    {
                        "CreatedAt",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "CreatedAt" },
                        }
                    },
                    {
                        "Deleted",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "Deleted" },
                        }
                    },
                    {
                        "Id",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "Id" },
                        }
                    },
                    {
                        "UpdatedAt",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "UpdatedAt" },
                        }
                    },
                    {
                        "Version",
                        new Dictionary<string, object>
                        {
                            { "ServiceTableColumn", "Version" },
                        }
                    },
                });
        }
    }
}
