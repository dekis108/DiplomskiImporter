﻿using FTN.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace FTN.ESI.SIMES.CIM.CIMAdapter.DBHelper
{


    public class DeltaQuerry
    {
        [Key, Column(Order = 0)]
        public string mrid { get; set; }
        [Key, Column(Order = 1)]
        public string FileName { get; set; }

        public long ResourceId { get; set; }

        public DeltaQuerry(string id, string fileName, long rid)
        {
            mrid = id;
            FileName = fileName;
            ResourceId = rid;
        }
        public DeltaQuerry()
        {

        }
    }
}