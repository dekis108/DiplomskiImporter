using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using FTN.Common;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
	public class ConductingEquipment : Equipment
	{
		//private PhaseCode phases;
		//private float ratedVoltage;
		//private long baseVoltage = 0;

		List<long> _Terminals = new List<long>();
        public List<long> Terminals { 
			get
            {
				return _Terminals;
            }
			set
            {
				_Terminals = value;
            }
		}

        public ConductingEquipment(long globalId) : base(globalId) 
		{
		}

		/*
		public PhaseCode Phases
		{
			get
			{
				return phases;
			}

			set
			{
				phases = value;
			}
		}

		public float RatedVoltage
		{
			get { return ratedVoltage; }
			set { ratedVoltage = value; }
		}

		public long BaseVoltage
		{
			get { return baseVoltage; }
			set { baseVoltage = value; }
		}
		*/
		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				ConductingEquipment x = (ConductingEquipment)obj;
				return CompareHelper.CompareLists(x.Terminals, this.Terminals);
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
				case ModelCode.CONDEQ_TERMINALS:
					return true;

				default:
					return base.HasProperty(property);
			}
		}

		public override void GetProperty(Property prop)
		{
			switch (prop.Id)
			{
				case ModelCode.CONDEQ_TERMINALS:
					prop.SetValue(Terminals);
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
				default:
					base.SetProperty(property);
					break;
			}
		}

		public override bool IsReferenced
		{
			get
			{
				return Terminals.Count > 0 || base.IsReferenced;
			}
		}


		public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
		{
			/*
			if (baseVoltage != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
			{
				references[ModelCode.CONDEQ_BASVOLTAGE] = new List<long>();
				references[ModelCode.CONDEQ_BASVOLTAGE].Add(baseVoltage);
			}
			*/

			if (Terminals != null && Terminals.Count > 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
			{
				references[ModelCode.CONDEQ_TERMINALS] = Terminals.GetRange(0, Terminals.Count);
			}


			base.GetReferences(references, refType);
		}

		public override void AddReference(ModelCode referenceId, long globalId) 
		{
			switch (referenceId)
			{
				case ModelCode.TERMINAL_CONDQEQ:
					Terminals.Add(globalId);
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
				case ModelCode.TERMINAL_CONDQEQ:

					if (Terminals.Contains(globalId))
					{
						Terminals.Remove(globalId);
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
