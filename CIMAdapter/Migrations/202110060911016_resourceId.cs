namespace FTN.ESI.SIMES.CIM.CIMAdapter.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class resourceId : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.DeltaQuerries");
            AddColumn("dbo.DeltaQuerries", "ResourceId", c => c.Long(nullable: false));
            AddPrimaryKey("dbo.DeltaQuerries", new[] { "mrid", "FileName" });
            DropColumn("dbo.DeltaQuerries", "OperationType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DeltaQuerries", "OperationType", c => c.Byte(nullable: false));
            DropPrimaryKey("dbo.DeltaQuerries");
            DropColumn("dbo.DeltaQuerries", "ResourceId");
            AddPrimaryKey("dbo.DeltaQuerries", new[] { "mrid", "OperationType", "FileName" });
        }
    }
}
