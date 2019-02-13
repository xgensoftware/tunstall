namespace TunstallDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Alter_Event_IsProcessed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "IsProcessed", c => c.Boolean(nullable: false));
            AddColumn("dbo.Events", "ServiceId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "ServiceId");
            DropColumn("dbo.Events", "IsProcessed");
        }
    }
}
