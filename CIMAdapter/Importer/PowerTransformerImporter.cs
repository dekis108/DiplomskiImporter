using System;
using System.Collections.Generic;
using CIM.Model;
using FTN.Common;
using FTN.ESI.SIMES.CIM.CIMAdapter.DBHelper;
using FTN.ESI.SIMES.CIM.CIMAdapter.Manager;
using System.Linq;
using FTN.ServiceContracts;

namespace FTN.ESI.SIMES.CIM.CIMAdapter.Importer
{
    /// <summary>
    /// PowerTransformerImporter
    /// </summary>
    public class PowerTransformerImporter
	{
		/// <summary> Singleton </summary>
		private static PowerTransformerImporter ptImporter = null;
		private static object singletoneLock = new object();


		private Dictionary<long, long> ClientToServerGID;

		private ConcreteModel concreteModel;
		private Delta delta;
		private ImportHelper importHelper;
		private TransformAndLoadReport report;
		private NetworkModelGDAProxy GdaQueryProxy;


		#region Properties
		public static PowerTransformerImporter Instance
		{
			get
			{
				if (ptImporter == null)
				{
					lock (singletoneLock)
					{
						if (ptImporter == null)
						{
							ptImporter = new PowerTransformerImporter();
							ptImporter.Reset();
						}
					}
				}
				return ptImporter;
			}
		}

		public Delta NMSDelta
		{
			get 
			{
				return delta;
			}
		}
		#endregion Properties


		public void Reset()
		{
			concreteModel = null;
			delta = new Delta();
			importHelper = new ImportHelper();
			report = null;
		}

		public TransformAndLoadReport CreateNMSDelta(ConcreteModel cimConcreteModel, string fileName, ServiceContracts.NetworkModelGDAProxy gdaQueryProxy)
		{
			LogManager.Log("Importing PowerTransformer Elements...", LogLevel.Info);
			report = new TransformAndLoadReport();
			concreteModel = cimConcreteModel;
			delta.ClearDeltaOperations();

			if ((concreteModel != null) && (concreteModel.ModelMap != null))
			{
				try
				{
					// convert into DMS elements
					ConvertModelAndPopulateDelta(fileName, gdaQueryProxy);
				}
				catch (Exception ex)
				{
					string message = string.Format("{0} - ERROR in data import - {1}", DateTime.Now, ex.Message);
					LogManager.Log(message);
					report.Report.AppendLine(ex.Message);
					report.Success = false;
				}
			}
			LogManager.Log("Importing PowerTransformer Elements - END.", LogLevel.Info);
			return report;
		}

        /// <summary>
        /// Method performs conversion of network elements from CIM based concrete model into DMS model.
        /// </summary>
        private void ConvertModelAndPopulateDelta(string fileName, ServiceContracts.NetworkModelGDAProxy gdaQueryProxy)
		{
			LogManager.Log("Loading elements and creating delta...", LogLevel.Info);

			//// import all concrete model types (DMSType enum)
			/*
			ImportBaseVoltages();
			ImportLocations();
			ImportPowerTransformers();
			ImportTransformerWindings();
			ImportWindingTests();
			*/

			//TODO: promeniti ove importe da na deltu dodaju korektnu operaciju(update, delete)
			GdaQueryProxy = gdaQueryProxy;
			ClientToServerGID = new Dictionary<long, long>();
			ImportACLineSegment(fileName);
			ImportACLineSegmentPhase(fileName);
			ImportTerminal(fileName);
			ImportMutualCoupling(fileName);
			AssignDeleteDeltaOperation(fileName);
			FixReferenceIdsToUpdatedEntities();




			LogManager.Log("Loading elements and creating delta completed.", LogLevel.Info);
		}

        private void FixReferenceIdsToUpdatedEntities()
        {
            foreach(ResourceDescription entity in delta.InsertOperations)
            {
				FixReferenceId(entity);
			}

			foreach (ResourceDescription entity in delta.UpdateOperations)
			{
				FixReferenceId(entity);
			}
		}

        private void FixReferenceId(ResourceDescription entity)
        {
            foreach(Property prop in entity.Properties)
            {
				if (prop.Type == PropertyType.Reference)
                {
					if (ClientToServerGID.ContainsKey(prop.PropertyValue.LongValue))
                    {
						long gid = ClientToServerGID[prop.PropertyValue.LongValue];
						prop.PropertyValue.LongValue = gid;
					}
                }
            }
        }

        private void ImportACLineSegment(string fileName)
		{
			SortedDictionary<string, object> cimACLineSegments = concreteModel.GetAllObjectsOfType("FTN.ACLineSegment");
			if (cimACLineSegments != null)
			{
				foreach (KeyValuePair<string, object> cimClassPair in cimACLineSegments)
				{
					FTN.ACLineSegment cimTerminal = cimClassPair.Value as FTN.ACLineSegment;
					ResourceDescription rd = CreateACLineSegmentResourceDescription(cimTerminal);
					if (rd != null)
					{
                        AssignInsertOrUpdateDeltaOperation(rd, fileName, ModelCode.ACLINESEGMENT);
                        report.Report.Append("ACLineSegment ID = ").Append(cimTerminal.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
					}
					else
					{
						report.Report.Append("ACLineSegment ID = ").Append(cimTerminal.ID).AppendLine(" FAILED to be converted");
					}
				}
				report.Report.AppendLine();
			}
		}

		private void AssignDeleteDeltaOperation(string fileName)
        {
			using (var db = new DeltaDBContext())
            {
				List<DeltaQuerry> entities = db.Delta.Where(x => x.FileName == fileName).ToList();

				foreach(var entity in entities)
                {
					var inInsert = delta.InsertOperations.FindIndex(x => x.Properties.Find(y => y.Id == ModelCode.IDOBJ_MRID).PropertyValue.StringValue == entity.mrid);
					var inUpdate = delta.UpdateOperations.FindIndex(x => x.Properties.Find(y => y.Id == ModelCode.IDOBJ_MRID).PropertyValue.StringValue == entity.mrid);

					if (inInsert < 0 && inUpdate < 0)
                    {
						long globalId = GdaQueryProxy.GetServerwiseGlobalId(entity.mrid);
						List<Property> props = new List<Property>()
						{
							new Property(ModelCode.IDOBJ_MRID, entity.mrid)
						};
						ResourceDescription rd = new ResourceDescription(globalId, props);
						delta.AddDeltaOperation(DeltaOpType.Delete, rd, true);
                    }
				}
			}
		}

        private void AssignInsertOrUpdateDeltaOperation(ResourceDescription rd, string fileName, ModelCode modelType)
        {
			using (var db = new DeltaDBContext())
			{
				if (db.Delta.Any(x => x.FileName == fileName))
				{
					string mrid = rd.Properties.Find(x => x.Id == ModelCode.IDOBJ_MRID).PropertyValue.StringValue;

					if (db.Delta.Any(x => x.FileName == fileName && x.mrid == mrid))
					{
						long globalId = GdaQueryProxy.GetServerwiseGlobalId(mrid);
						ClientToServerGID.Add(rd.Id, globalId);
						rd.Id = globalId;
						delta.AddDeltaOperation(DeltaOpType.Update, rd, true);
					}
					else
                    {
						delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
					}
				}
				else
				{
					delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
				}
			}
		}

        private ResourceDescription CreateACLineSegmentResourceDescription(FTN.ACLineSegment cim)
		{
			ResourceDescription rd = null;
			if (cim != null)
			{
				long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.ACLINESEGMENT, importHelper.CheckOutIndexForDMSType(DMSType.ACLINESEGMENT));
				rd = new ResourceDescription(gid);
				importHelper.DefineIDMapping(cim.ID, gid);

				////populate ResourceDescription
				PowerTransformerConverter.PopulateACLineSegmentProperties(cim, rd, importHelper, report);
			}
			return rd;

		}


		private void ImportACLineSegmentPhase(string fileName)
		{
			SortedDictionary<string, object> cimClass = concreteModel.GetAllObjectsOfType("FTN.ACLineSegmentPhase");
			if (cimClass != null)
			{
				foreach (KeyValuePair<string, object> cimClassPair in cimClass)
				{
					FTN.ACLineSegmentPhase cimTerminal = cimClassPair.Value as FTN.ACLineSegmentPhase;

					ResourceDescription rd = CreateACLineSegmentPhaseResourceDescription(cimTerminal);
					if (rd != null)
					{
						AssignInsertOrUpdateDeltaOperation(rd, fileName, ModelCode.ACLINESEGMENTPHASE);
						report.Report.Append("ACLineSegmentPhase ID = ").Append(cimTerminal.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
					}
					else
					{
						report.Report.Append("ACLineSegmentPhase ID = ").Append(cimTerminal.ID).AppendLine(" FAILED to be converted");
					}
				}
				report.Report.AppendLine();
			}
		}

		private ResourceDescription CreateACLineSegmentPhaseResourceDescription(FTN.ACLineSegmentPhase cim)
		{
			ResourceDescription rd = null;
			if (cim != null)
			{
				long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.ACLINESEGMENTPHASE, 
					importHelper.CheckOutIndexForDMSType(DMSType.ACLINESEGMENTPHASE));
				rd = new ResourceDescription(gid);
				importHelper.DefineIDMapping(cim.ID, gid);

				////populate ResourceDescription
				PowerTransformerConverter.PopulateACLineSegmentPhaseProperties(cim, rd, importHelper, report);
			}
			return rd;

		}

		private void ImportTerminal(string fileName)
		{
			SortedDictionary<string, object> cimTerminals = concreteModel.GetAllObjectsOfType("FTN.Terminal");
			if (cimTerminals != null)
			{
				foreach (KeyValuePair<string, object> cimTerminalsPair in cimTerminals)
				{
					FTN.Terminal cimTerminal = cimTerminalsPair.Value as FTN.Terminal;

					ResourceDescription rd = CreateTerminalResourceDescription(cimTerminal);
					if (rd != null)
					{
						AssignInsertOrUpdateDeltaOperation(rd, fileName, ModelCode.TERMINAL);
						report.Report.Append("Terminal ID = ").Append(cimTerminal.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
					}
					else
					{
						report.Report.Append("Terminal ID = ").Append(cimTerminal.ID).AppendLine(" FAILED to be converted");
					}
				}
				report.Report.AppendLine();
			}
		}

		private ResourceDescription CreateTerminalResourceDescription(FTN.Terminal cim)
		{
			//throw new NotImplementedException();

			ResourceDescription rd = null;
			if (cim != null)
			{
				long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.TERMINAL, importHelper.CheckOutIndexForDMSType(DMSType.TERMINAL));
				rd = new ResourceDescription(gid);
				importHelper.DefineIDMapping(cim.ID, gid);

				////populate ResourceDescription
				PowerTransformerConverter.PopulateTerminalProperties(cim, rd, importHelper, report);
			}
			return rd;
		}

		private void ImportMutualCoupling(string fileName)
		{
			SortedDictionary<string, object> cimClass = concreteModel.GetAllObjectsOfType("FTN.MutualCoupling");
			if (cimClass != null)
			{
				foreach (KeyValuePair<string, object> cimClassPair in cimClass)
				{
					FTN.MutualCoupling cimTerminal = cimClassPair.Value as FTN.MutualCoupling;

					ResourceDescription rd = CreateMutualCouplingResourceDescription(cimTerminal);
					if (rd != null)
					{
						AssignInsertOrUpdateDeltaOperation(rd, fileName, ModelCode.MUTUALCOUPLING);
						report.Report.Append("MutualCoupling ID = ").Append(cimTerminal.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
					}
					else
					{
						report.Report.Append("MutualCoupling ID = ").Append(cimTerminal.ID).AppendLine(" FAILED to be converted");
					}
				}
				report.Report.AppendLine();
			}
		}

		private ResourceDescription CreateMutualCouplingResourceDescription(FTN.MutualCoupling cim)
		{
			ResourceDescription rd = null;
			if (cim != null)
			{
				long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.MUTUALCOUPLING,
					importHelper.CheckOutIndexForDMSType(DMSType.MUTUALCOUPLING));
				rd = new ResourceDescription(gid);
				importHelper.DefineIDMapping(cim.ID, gid);

				////populate ResourceDescription
				PowerTransformerConverter.PopulateMutualCouplingProperties(cim, rd, importHelper, report);
			}
			return rd;

		}



		/*

		private void ImportBaseVoltages()
		{
			SortedDictionary<string, object> cimBaseVoltages = concreteModel.GetAllObjectsOfType("FTN.BaseVoltage");
			if (cimBaseVoltages != null)
			{
				foreach (KeyValuePair<string, object> cimBaseVoltagePair in cimBaseVoltages)
				{
					FTN.BaseVoltage cimBaseVoltage = cimBaseVoltagePair.Value as FTN.BaseVoltage;

					ResourceDescription rd = CreateBaseVoltageResourceDescription(cimBaseVoltage);
					if (rd != null)
					{
						delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
						report.Report.Append("BaseVoltage ID = ").Append(cimBaseVoltage.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
					}
					else
					{
						report.Report.Append("BaseVoltage ID = ").Append(cimBaseVoltage.ID).AppendLine(" FAILED to be converted");
					}
				}
				report.Report.AppendLine();
			}
		}

		private ResourceDescription CreateBaseVoltageResourceDescription(FTN.BaseVoltage cimBaseVoltage)
		{
			ResourceDescription rd = null;
			if (cimBaseVoltage != null)
			{
				long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.BASEVOLTAGE, importHelper.CheckOutIndexForDMSType(DMSType.BASEVOLTAGE));
				rd = new ResourceDescription(gid);
				importHelper.DefineIDMapping(cimBaseVoltage.ID, gid);

				////populate ResourceDescription
				PowerTransformerConverter.PopulateBaseVoltageProperties(cimBaseVoltage, rd);
			}
			return rd;
		}
		
		private void ImportLocations()
		{
			SortedDictionary<string, object> cimLocations = concreteModel.GetAllObjectsOfType("FTN.Location");
			if (cimLocations != null)
			{
				foreach (KeyValuePair<string, object> cimLocationPair in cimLocations)
				{
					FTN.Location cimLocation = cimLocationPair.Value as FTN.Location;

					ResourceDescription rd = CreateLocationResourceDescription(cimLocation);
					if (rd != null)
					{
						delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
						report.Report.Append("Location ID = ").Append(cimLocation.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
					}
					else
					{
						report.Report.Append("Location ID = ").Append(cimLocation.ID).AppendLine(" FAILED to be converted");
					}
				}
				report.Report.AppendLine();
			}
		}

		private ResourceDescription CreateLocationResourceDescription(FTN.Location cimLocation)
		{
			ResourceDescription rd = null;
			if (cimLocation != null)
			{
				long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.LOCATION, importHelper.CheckOutIndexForDMSType(DMSType.LOCATION));
				rd = new ResourceDescription(gid);
				importHelper.DefineIDMapping(cimLocation.ID, gid);

				////populate ResourceDescription
				PowerTransformerConverter.PopulateLocationProperties(cimLocation, rd);
			}
			return rd;
		}

		private void ImportPowerTransformers()
		{
			SortedDictionary<string, object> cimPowerTransformers = concreteModel.GetAllObjectsOfType("FTN.PowerTransformer");
			if (cimPowerTransformers != null)
			{
				foreach (KeyValuePair<string, object> cimPowerTransformerPair in cimPowerTransformers)
				{
					FTN.PowerTransformer cimPowerTransformer = cimPowerTransformerPair.Value as FTN.PowerTransformer;

					ResourceDescription rd = CreatePowerTransformerResourceDescription(cimPowerTransformer);
					if (rd != null)
					{
						delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
						report.Report.Append("PowerTransformer ID = ").Append(cimPowerTransformer.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
					}
					else
					{
						report.Report.Append("PowerTransformer ID = ").Append(cimPowerTransformer.ID).AppendLine(" FAILED to be converted");
					}
				}
				report.Report.AppendLine();
			}
		}

		private ResourceDescription CreatePowerTransformerResourceDescription(FTN.PowerTransformer cimPowerTransformer)
		{
			ResourceDescription rd = null;
			if (cimPowerTransformer != null)
			{
				long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.POWERTR, importHelper.CheckOutIndexForDMSType(DMSType.POWERTR));
				rd = new ResourceDescription(gid);
				importHelper.DefineIDMapping(cimPowerTransformer.ID, gid);

				////populate ResourceDescription
				PowerTransformerConverter.PopulatePowerTransformerProperties(cimPowerTransformer, rd, importHelper, report);
			}
			return rd;
		}

		private void ImportTransformerWindings()
		{
			SortedDictionary<string, object> cimTransformerWindings = concreteModel.GetAllObjectsOfType("FTN.TransformerWinding");
			if (cimTransformerWindings != null)
			{
				foreach (KeyValuePair<string, object> cimTransformerWindingPair in cimTransformerWindings)
				{
					FTN.TransformerWinding cimTransformerWinding = cimTransformerWindingPair.Value as FTN.TransformerWinding;

					ResourceDescription rd = CreateTransformerWindingResourceDescription(cimTransformerWinding);
					if (rd != null)
					{
						delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
						report.Report.Append("TransformerWinding ID = ").Append(cimTransformerWinding.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
					}
					else
					{
						report.Report.Append("TransformerWinding ID = ").Append(cimTransformerWinding.ID).AppendLine(" FAILED to be converted");
					}
				}
				report.Report.AppendLine();
			}
		}

		private ResourceDescription CreateTransformerWindingResourceDescription(FTN.TransformerWinding cimTransformerWinding)
		{
			ResourceDescription rd = null;
			if (cimTransformerWinding != null)
			{
				long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.POWERTRWINDING, importHelper.CheckOutIndexForDMSType(DMSType.POWERTRWINDING));
				rd = new ResourceDescription(gid);
				importHelper.DefineIDMapping(cimTransformerWinding.ID, gid);

				////populate ResourceDescription
				PowerTransformerConverter.PopulateTransformerWindingProperties(cimTransformerWinding, rd, importHelper, report);
			}
			return rd;
		}

		private void ImportWindingTests()
		{
			SortedDictionary<string, object> cimWindingTests = concreteModel.GetAllObjectsOfType("FTN.WindingTest");
			if (cimWindingTests != null)
			{
				foreach (KeyValuePair<string, object> cimWindingTestPair in cimWindingTests)
				{
					FTN.WindingTest cimWindingTest = cimWindingTestPair.Value as FTN.WindingTest;

					ResourceDescription rd = CreateWindingTestResourceDescription(cimWindingTest);
					if (rd != null)
					{
						delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
						report.Report.Append("WindingTest ID = ").Append(cimWindingTest.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
					}
					else
					{
						report.Report.Append("WindingTest ID = ").Append(cimWindingTest.ID).AppendLine(" FAILED to be converted");
					}
				}
				report.Report.AppendLine();
			}
		}

		private ResourceDescription CreateWindingTestResourceDescription(FTN.WindingTest cimWindingTest)
		{
			ResourceDescription rd = null;
			if (cimWindingTest != null)
			{
				long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.WINDINGTEST, importHelper.CheckOutIndexForDMSType(DMSType.WINDINGTEST));
				rd = new ResourceDescription(gid);
				importHelper.DefineIDMapping(cimWindingTest.ID, gid);

				////populate ResourceDescription
				PowerTransformerConverter.PopulateWindingTestProperties(cimWindingTest, rd, importHelper, report);
			}
			return rd;
		}
		*/
	}
}

