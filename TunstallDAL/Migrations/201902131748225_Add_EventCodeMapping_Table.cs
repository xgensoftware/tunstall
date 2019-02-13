namespace TunstallDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_EventCodeMapping_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EventCodeMappings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ExternalEventCode = c.String(maxLength: 10),
                        InternalEventCode = c.String(maxLength: 10),
                        Integration = c.String(maxLength: 25),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EventCodeMappings");
        }
    }
}
