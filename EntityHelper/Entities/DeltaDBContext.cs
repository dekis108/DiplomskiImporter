using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace EntityHelper.Entities
{
    public class DeltaDBContext : DbContext
    {
        static string path = "data source=DESKTOP-HDBFPKR\\SQLEXPRESS02;Initial Catalog=DiplomskiImporter;Integrated Security=SSPI;";

        public DbSet<DeltaOperation> Delta { get; set; }

        public DeltaDBContext() : base(path) { }


    }
}
