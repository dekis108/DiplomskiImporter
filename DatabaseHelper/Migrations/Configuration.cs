namespace DatabaseHelper.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DatabaseHelper.EFClasses.DeltaDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            //AppDomain.CurrentDomain.SetData("DataDirectory", @"DESKTOP-HDBFPKR\SQLEXPRESS02");
        }

        protected override void Seed(DatabaseHelper.EFClasses.DeltaDBContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
