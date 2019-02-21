namespace TunstallDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Alter_Event_EventZone : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "EventZone", c => c.String(maxLength: 10));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "EventZone");
        }
    }
}
