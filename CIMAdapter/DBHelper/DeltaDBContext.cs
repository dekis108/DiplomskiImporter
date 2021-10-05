using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;


namespace FTN.ESI.SIMES.CIM.CIMAdapter.DBHelper
{
    public class DeltaDBContext : DbContext
    {
        static string path = "data source=DESKTOP-HDBFPKR\\SQLEXPRESS02;Initial Catalog=DiplomskiImporter;Integrated Security=SSPI;";

        public DbSet<DeltaQuerry> Delta { get; set; }

        public DeltaDBContext() : base(path) { }


    }
}
