namespace DatabaseHelper.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DeltaOperations",
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
            DropTable("dbo.DeltaOperations");
        }
    }
}
