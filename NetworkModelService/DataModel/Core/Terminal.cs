using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FTN.Common;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class Terminal : IdentifiedObject
    {
        public bool Connected { get; set; }

        public PhaseCode PhaseCode { get; set; } 

        public int SequenceNumber { get; set; }

        private List<long> _HasFirstMutualCoupling = new List<long>();
        public List<long> HasFirstMutualCoupling { 
            get
            {
                return _HasFirstMutualCoupling;
            } 
            set
            {
                _HasFirstMutualCoupling = value;
            }
        }

        private List<long> _HasSecondMutualCoupling = new List<long>();

        public List<long> HasSecondMutualCoupling { 
            get
            {
                return _HasSecondMutualCoupling;
            }
            set
            {
                _HasSecondMutualCoupling = value;
            }
        }

        private long _ConductingEquipment = 0;
        public long ConductingEquipment { 
            get
            {
                return _ConductingEquipment;
            }
            set
            {
                _ConductingEquipment = value;
            }
        }

        public Terminal(long globalId) : base(globalId)
        {
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                Terminal x = (Terminal)obj;
                return (x.Connected == this.Connected && x.PhaseCode == this.PhaseCode && x.SequenceNumber == this.SequenceNumber &&
                  CompareHelper.CompareLists(x.HasFirstMutualCoupling, this.HasFirstMutualCoupling) &&
                  CompareHelper.CompareLists(x.HasSecondMutualCoupling, this.HasSecondMutualCoupling) && x.ConductingEquipment == this.ConductingEquipment);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool HasProperty(ModelCode t)
        {
            switch (t)
            {
                case ModelCode.TERMINAL_CONDQEQ:
                case ModelCode.TERMINAL_CONNECTED:
                case ModelCode.TERMINAL_HASFIRSTMUTUALCOUPL:
                case ModelCode.TERMINAL_HASSECONDTMUTUALCOUPL:
                case ModelCode.TERMINAL_PHASE:
                case ModelCode.TERMINAL_SQCNUM:
                    return true;

                default:
                    return base.HasProperty(t);
            }
        }

        public override void GetProperty(Property prop)
        {
            switch (prop.Id)
            {
                case ModelCode.TERMINAL_CONDQEQ:
                    prop.SetValue(ConductingEquipment);
                    break;
                case ModelCode.TERMINAL_CONNECTED:
                    prop.SetValue(Connected);
                    break;
                case ModelCode.TERMINAL_HASFIRSTMUTUALCOUPL:
                    prop.SetValue(HasFirstMutualCoupling);
                    break;
                case ModelCode.TERMINAL_HASSECONDTMUTUALCOUPL:
                    prop.SetValue(HasSecondMutualCoupling);
                    break;
                case ModelCode.TERMINAL_PHASE:
                    prop.SetValue((short)PhaseCode);
                    break;
                case ModelCode.TERMINAL_SQCNUM:
                    prop.SetValue(SequenceNumber);
                    break;

                default:
                    base.GetProperty(prop);
                    break;
            }
        }

        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.TERMINAL_CONDQEQ:
                    ConductingEquipment = property.AsReference();
                    break;
                case ModelCode.TERMINAL_CONNECTED:
                    Connected = property.AsBool();
                    break;
                case ModelCode.TERMINAL_PHASE:
                    PhaseCode = (PhaseCode)property.AsEnum();
                    break;
                case ModelCode.TERMINAL_SQCNUM:
                    SequenceNumber = property.AsInt();
                    break;

                default:
                    base.SetProperty(property);
                    break;
            }
        }

        public override bool IsReferenced
        {
            get
            {
                return HasFirstMutualCoupling.Count > 0 || HasSecondMutualCoupling.Count > 0 || base.IsReferenced;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (HasFirstMutualCoupling != null && HasFirstMutualCoupling.Count > 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.TERMINAL_HASFIRSTMUTUALCOUPL] = HasFirstMutualCoupling.GetRange(0, HasFirstMutualCoupling.Count);
            }

            if (HasSecondMutualCoupling != null && HasSecondMutualCoupling.Count > 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.TERMINAL_HASSECONDTMUTUALCOUPL] = HasSecondMutualCoupling.GetRange(0, HasSecondMutualCoupling.Count);
            }

            if (ConductingEquipment != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.CONDEQ_TERMINALS] = new List<long>();
                references[ModelCode.CONDEQ_TERMINALS].Add(ConductingEquipment);
            }


            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.MUTUALCOUPLING_FIRSTTERMINAL:
                    HasFirstMutualCoupling.Add(globalId);
                    break;


                case ModelCode.MUTUALCOUPLING_SECONDTERMINAL:
                    HasSecondMutualCoupling.Add(globalId);
                    break;


                default:
                    base.AddReference(referenceId, globalId);
                    break;
            }
        }

        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.MUTUALCOUPLING_FIRSTTERMINAL:

                    if (HasFirstMutualCoupling.Contains(globalId))
                    {
                        HasFirstMutualCoupling.Remove(globalId);
                    }
                    else
                    {
                        CommonTrace.WriteTrace(CommonTrace.TraceWarning, "Entity (GID = 0x{0:x16}) doesn't contain reference 0x{1:x16}.", this.GlobalId, globalId);
                    }

                    break;

                case ModelCode.MUTUALCOUPLING_SECONDTERMINAL:

                    if (HasSecondMutualCoupling.Contains(globalId))
                    {
                        HasSecondMutualCoupling.Remove(globalId);
                    }
                    else
                    {
                        CommonTrace.WriteTrace(CommonTrace.TraceWarning, "Entity (GID = 0x{0:x16}) doesn't contain reference 0x{1:x16}.", this.GlobalId, globalId);
                    }

                    break;

                default:
                    base.RemoveReference(referenceId, globalId);
                    break;
            }
        }

    }
}
