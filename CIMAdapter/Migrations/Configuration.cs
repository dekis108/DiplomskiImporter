namespace FTN.ESI.SIMES.CIM.CIMAdapter.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FTN.ESI.SIMES.CIM.CIMAdapter.DBHelper.DeltaDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "FTN.ESI.SIMES.CIM.CIMAdapter.DBHelper.DeltaDBContext";
        }

        protected override void Seed(FTN.ESI.SIMES.CIM.CIMAdapter.DBHelper.DeltaDBContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
