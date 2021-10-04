using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class ACLineSegment : Conductor
    {
        public float B0CH { get; set; }
        public float BCH { get; set; }
        public float G0CH { get; set; }
        public float GCH { get; set; }

        public float R { get; set; }
        public float R0 { get; set; }
        public float X { get; set; }

        public float X0 { get; set; }


		List<long> _ACLineSegmentPhases = new List<long>();
        public List<long> ACLineSegmentPhases { 
			get
            {
				return _ACLineSegmentPhases;
            }
			set
            {
				_ACLineSegmentPhases = value;
            }
		}


        public ACLineSegment(long globalId) : base(globalId)
		{
			//ACLineSegmentPhases = new List<long>();
		}

		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				ACLineSegment x = (ACLineSegment)obj;
				return CompareHelper.CompareLists(this.ACLineSegmentPhases, x.ACLineSegmentPhases) && x.B0CH == this.B0CH &&
					x.BCH == this.BCH && x.G0CH == this.G0CH && x.GCH == this.GCH && x.R == this.R && x.R0 == this.R0 && x.X == this.X && x.X0 == this.X0;
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

		public override bool HasProperty(ModelCode property)
		{
			switch (property)
			{
				case ModelCode.ACLINESEGMENT_B0CH:
				case ModelCode.ACLINESEGMENT_BCH:
				case ModelCode.ACLINESEGMENT_G0CH:
				case ModelCode.ACLINESEGMENT_GCH:
				case ModelCode.ACLINESEGMENT_PHASES:
				case ModelCode.ACLINESEGMENT_R:
				case ModelCode.ACLINESEGMENT_R0:
				case ModelCode.ACLINESEGMENT_X:
				case ModelCode.ACLINESEGMENT_X0:
					return true;
				default:
					return base.HasProperty(property);
			}
		}

		public override void GetProperty(Property prop)
		{
			switch (prop.Id)
			{
				case ModelCode.ACLINESEGMENT_B0CH:
					prop.SetValue(B0CH);
					break;
				case ModelCode.ACLINESEGMENT_BCH:
					prop.SetValue(BCH);
					break;
				case ModelCode.ACLINESEGMENT_G0CH:
					prop.SetValue(G0CH);
					break;
				case ModelCode.ACLINESEGMENT_GCH:
					prop.SetValue(GCH);
					break;
				case ModelCode.ACLINESEGMENT_PHASES:
					prop.SetValue(ACLineSegmentPhases);
					break;
				case ModelCode.ACLINESEGMENT_R:
					prop.SetValue(R);
					break;
				case ModelCode.ACLINESEGMENT_R0:
					prop.SetValue(R0);
					break;
				case ModelCode.ACLINESEGMENT_X:
					prop.SetValue(X);
					break;
				case ModelCode.ACLINESEGMENT_X0:
					prop.SetValue(X0);
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
				case ModelCode.ACLINESEGMENT_B0CH:
					B0CH = property.AsFloat();
					break;
				case ModelCode.ACLINESEGMENT_BCH:
					BCH = property.AsFloat();
					break;
				case ModelCode.ACLINESEGMENT_G0CH:
					G0CH = property.AsFloat();
					break;
				case ModelCode.ACLINESEGMENT_GCH:
					GCH = property.AsFloat();
					break;
				case ModelCode.ACLINESEGMENT_R:
					R = property.AsFloat();
					break;
				case ModelCode.ACLINESEGMENT_R0:
					R0 = property.AsFloat();
					break;
				case ModelCode.ACLINESEGMENT_X:
					X = property.AsFloat();
					break;
				case ModelCode.ACLINESEGMENT_X0:
					X0 = property.AsFloat();
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
				return ACLineSegmentPhases.Count > 0 || base.IsReferenced;
			}
		}

		public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
		{
			if (ACLineSegmentPhases != null && ACLineSegmentPhases.Count > 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
			{
				references[ModelCode.ACLINESEGMENT_PHASES] = ACLineSegmentPhases.GetRange(0, ACLineSegmentPhases.Count);
			}

			base.GetReferences(references, refType);
		}

		public override void AddReference(ModelCode referenceId, long globalId)
		{
			switch (referenceId)
			{
				case ModelCode.ACLINESEGMENTPHASE_ACLINESEGMENT:
					ACLineSegmentPhases.Add(globalId);
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
				case ModelCode.ACLINESEGMENTPHASE_ACLINESEGMENT:

					if (ACLineSegmentPhases.Contains(globalId))
					{
						ACLineSegmentPhases.Remove(globalId);
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
