using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class MutualCoupling : IdentifiedObject
    {
		public float B0CH { get; set; }
		public float G0CH { get; set; }

        public float Distance11 { get; set; }

        public float Distance12 { get; set; }

        public float Distance21 { get; set; }

        public float Distance22 { get; set; }

        public float R0 { get; set; }

		public float X0 { get; set; }

		private long _FirstTerminal = 0;
        public long FirstTerminal { 
			get
            {
				return _FirstTerminal;
            }
			set
            {
				_FirstTerminal = value;
            }
		}

		private long _SecondTerminal;
        public long SecondTerminal { 
			get
            {
				return _SecondTerminal;
            }
			set
            {
				_SecondTerminal = value;
            }
		}


        public MutualCoupling(long globalId) : base(globalId)
		{

		}

		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				MutualCoupling x = (MutualCoupling)obj;
				return  x.B0CH == this.B0CH &&  x.G0CH == this.G0CH && x.R0 == this.R0 && x.X0 == this.X0 && x.Distance11 == this.Distance11 &&
					x.Distance12 == this.Distance12 && x.Distance21 == this.Distance21 && x.Distance22 == this.Distance22 && 
					x.FirstTerminal == this.FirstTerminal && x.SecondTerminal == this.SecondTerminal;
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
				case ModelCode.MUTUALCOUPLING_B0CH:
				case ModelCode.MUTUALCOUPLING_G0CH:
				case ModelCode.MUTUALCOUPLING_R0:
				case ModelCode.MUTUALCOUPLING_X0:
				case ModelCode.MUTUALCOUPLING_D11:
				case ModelCode.MUTUALCOUPLING_D12:
				case ModelCode.MUTUALCOUPLING_D21:
				case ModelCode.MUTUALCOUPLING_D22:
				case ModelCode.MUTUALCOUPLING_FIRSTTERMINAL:
				case ModelCode.MUTUALCOUPLING_SECONDTERMINAL:
					return true;
				default:
					return base.HasProperty(property);
			}
		}

		public override void GetProperty(Property prop)
		{
			switch (prop.Id)
			{
				case ModelCode.MUTUALCOUPLING_B0CH:
					prop.SetValue(B0CH);
					break;
				case ModelCode.MUTUALCOUPLING_G0CH:
					prop.SetValue(G0CH);
					break;
				case ModelCode.MUTUALCOUPLING_R0:
					prop.SetValue(R0);
					break;
				case ModelCode.MUTUALCOUPLING_X0:
					prop.SetValue(X0);
					break;
				case ModelCode.MUTUALCOUPLING_D11:
					prop.SetValue(Distance11);
					break;
				case ModelCode.MUTUALCOUPLING_D12:
					prop.SetValue(Distance12);
					break;
				case ModelCode.MUTUALCOUPLING_D21:
					prop.SetValue(Distance21);
					break;
				case ModelCode.MUTUALCOUPLING_D22:
					prop.SetValue(Distance22);
					break;
				case ModelCode.MUTUALCOUPLING_FIRSTTERMINAL:
					prop.SetValue(FirstTerminal);
					break;
				case ModelCode.MUTUALCOUPLING_SECONDTERMINAL:
					prop.SetValue(SecondTerminal);
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
				case ModelCode.MUTUALCOUPLING_B0CH:
					B0CH = property.AsFloat();
					break;
				case ModelCode.MUTUALCOUPLING_G0CH:
					G0CH = property.AsFloat();
					break;
				case ModelCode.MUTUALCOUPLING_R0:
					R0 = property.AsFloat();
					break;
				case ModelCode.MUTUALCOUPLING_X0:
					X0 = property.AsFloat();
					break;
				case ModelCode.MUTUALCOUPLING_D11:
					Distance11 = property.AsFloat();
					break;
				case ModelCode.MUTUALCOUPLING_D12:
					Distance12 = property.AsFloat();
					break;
				case ModelCode.MUTUALCOUPLING_D21:
					Distance21 = property.AsFloat();
					break;
				case ModelCode.MUTUALCOUPLING_D22:
					Distance22 = property.AsFloat();
					break;
				case ModelCode.MUTUALCOUPLING_FIRSTTERMINAL:
					FirstTerminal = property.AsReference();
					break;
				case ModelCode.MUTUALCOUPLING_SECONDTERMINAL:
					SecondTerminal = property.AsReference();
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
			if (FirstTerminal != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
			{
				references[ModelCode.TERMINAL_HASFIRSTMUTUALCOUPL] = new List<long>();
				references[ModelCode.TERMINAL_HASFIRSTMUTUALCOUPL].Add(FirstTerminal);
			}

			if (SecondTerminal != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
			{
				references[ModelCode.TERMINAL_HASSECONDTMUTUALCOUPL] = new List<long>();
				references[ModelCode.TERMINAL_HASSECONDTMUTUALCOUPL].Add(SecondTerminal);
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
