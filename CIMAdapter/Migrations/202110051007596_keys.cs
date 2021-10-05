namespace FTN.ESI.SIMES.CIM.CIMAdapter.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class keys : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.DeltaQuerries");
            AlterColumn("dbo.DeltaQuerries", "FileName", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.DeltaQuerries", new[] { "mrid", "OperationType", "FileName" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.DeltaQuerries");
            AlterColumn("dbo.DeltaQuerries", "FileName", c => c.String());
            AddPrimaryKey("dbo.DeltaQuerries", "mrid");
        }
    }
}
