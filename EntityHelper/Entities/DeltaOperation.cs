using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace EntityHelper.Entities
{
    public enum DeltaOpType : byte
    {
        Insert = 0,
        Update = 1,
        Delete = 2
    }

    public class DeltaOperation
    {
        [Key]
        public string mrid { get; set; }
        public DeltaOpType OperationType { get; set; }
        public string FileName { get; set; }
    }

}
