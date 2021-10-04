using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class Conductor : ConductingEquipment
    {


		public Conductor(long globalId) : base(globalId)
		{
		}

		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				Conductor x = (Conductor)obj;
				return true; //?
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

				default:
					return base.HasProperty(property);
			}
		}

		public override void GetProperty(Property prop)
		{
			switch (prop.Id)
			{

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



		public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
		{


			base.GetReferences(references, refType);
		}



	}

}
