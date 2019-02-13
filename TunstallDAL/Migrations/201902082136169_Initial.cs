namespace TunstallDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AccountCode = c.String(maxLength: 75),
                        CallerId = c.String(maxLength: 25),
                        EventCode = c.String(maxLength: 255),
                        EventTime = c.Long(nullable: false),
                        EventTimeStamp = c.DateTime(nullable: false),
                        Qualifier = c.String(maxLength: 25),
                        Zone = c.String(maxLength: 50),
                        LineId = c.String(maxLength: 25),
                        UnitModel = c.String(maxLength: 50),
                        TestMode = c.Boolean(nullable: false),
                        MCCallOrigination = c.Boolean(nullable: false),
                        Latitude = c.Double(),
                        Longitude = c.Double(),
                        VerificationURL = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Events");
        }
    }
}
