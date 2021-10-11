namespace FTN.ESI.SIMES.CIM.CIMAdapter.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedRID : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.DeltaQuerries", "ResourceId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DeltaQuerries", "ResourceId", c => c.Long(nullable: false));
        }
    }
}
