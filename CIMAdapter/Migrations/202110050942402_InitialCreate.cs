namespace FTN.ESI.SIMES.CIM.CIMAdapter.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DeltaQuerries",
                c => new
                    {
                        mrid = c.String(nullable: false, maxLength: 128),
                        OperationType = c.Byte(nullable: false),
                        FileName = c.String(),
                    })
                .PrimaryKey(t => t.mrid);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DeltaQuerries");
        }
    }
}
