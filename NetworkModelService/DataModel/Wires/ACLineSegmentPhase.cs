using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class ACLineSegmentPhase : PowerSystemResource
    {
		private long _ACLineSegment = 0;
		public long ACLineSegment { 
			get
            {
				return _ACLineSegment;
            }
			set
            {
				_ACLineSegment = value;
            }
		}

        public SinglePhaseKind Phase { get; set; }

        public ACLineSegmentPhase(long globalId) : base(globalId)
		{
		}


		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				ACLineSegmentPhase x = (ACLineSegmentPhase)obj;
				return x.ACLineSegment == this.ACLineSegment && x.Phase == this.Phase;
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
				case ModelCode.ACLINESEGMENTPHASE_PHASE:
				case ModelCode.ACLINESEGMENTPHASE_ACLINESEGMENT:
					return true;
				

				default:
					return base.HasProperty(property);
			}
		}

		public override void GetProperty(Property prop)
		{
			switch (prop.Id)
			{
				case ModelCode.ACLINESEGMENTPHASE_ACLINESEGMENT:
					prop.SetValue(ACLineSegment);
					break;
				case ModelCode.ACLINESEGMENTPHASE_PHASE:
					prop.SetValue((short)Phase);
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
				case ModelCode.ACLINESEGMENTPHASE_ACLINESEGMENT:
					ACLineSegment = property.AsReference();
					break;
				case ModelCode.ACLINESEGMENTPHASE_PHASE:
					Phase = (SinglePhaseKind)property.AsEnum();
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
				return base.IsReferenced;
			}
		}


		public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
		{
			if (ACLineSegment != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
			{
				references[ModelCode.ACLINESEGMENT_PHASES] = new List<long>();
				references[ModelCode.ACLINESEGMENT_PHASES].Add(ACLineSegment);
			}

			base.GetReferences(references, refType);
		}

		public override void AddReference(ModelCode referenceId, long globalId) 
		{
			switch (referenceId)
			{

				default:
					base.AddReference(referenceId, globalId);
					break;
			}
		}

		public override void RemoveReference(ModelCode referenceId, long globalId)
		{
			switch (referenceId)
			{
				default:
					base.RemoveReference(referenceId, globalId);
					break;
			}
		}
	}
}
