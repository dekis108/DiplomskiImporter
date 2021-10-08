﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using CIM.Model;
using CIMParser;
using FTN.Common;
using FTN.ESI.SIMES.CIM.CIMAdapter.DBHelper;
using FTN.ESI.SIMES.CIM.CIMAdapter.Importer;
using FTN.ESI.SIMES.CIM.CIMAdapter.Manager;
using FTN.ServiceContracts;

namespace FTN.ESI.SIMES.CIM.CIMAdapter
{
	public class CIMAdapter
	{
        private NetworkModelGDAProxy gdaQueryProxy = null;
       
		public CIMAdapter()
		{
		}

        private NetworkModelGDAProxy GdaQueryProxy
        {
            get
            {
                if (gdaQueryProxy != null)
                {
                    gdaQueryProxy.Abort();
                    gdaQueryProxy = null;
                }

                gdaQueryProxy = new NetworkModelGDAProxy("NetworkModelGDAEndpoint");
                gdaQueryProxy.Open();

                return gdaQueryProxy;
            }
        }

		public Delta CreateDelta(Stream extract, SupportedProfiles extractType, out string log, string fileName)
		{
			Delta nmsDelta = null;
			ConcreteModel concreteModel = null;
			Assembly assembly = null;
			string loadLog = string.Empty;
			string transformLog = string.Empty;

			if (LoadModelFromExtractFile(extract, extractType, ref concreteModel, ref assembly, out loadLog))
			{
				DoTransformAndLoad(assembly, concreteModel, extractType, out nmsDelta, out transformLog, fileName);
			}
			log = string.Concat("Load report:\r\n", loadLog, "\r\nTransform report:\r\n", transformLog);

			return nmsDelta;
		}

		public string ApplyUpdates(Delta delta, string fileName)
		{
			string updateResult = "Apply Updates Report:\r\n";
			System.Globalization.CultureInfo culture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

			if ((delta != null) && (delta.NumberOfOperations != 0))
			{
				//// NetworkModelService->ApplyUpdates
                updateResult = GdaQueryProxy.ApplyUpdate(delta).ToString();
				if (updateResult.Contains("Update result: Succeeded"))
                {
					SaveDeltaToDb(delta, fileName);
				}
			}

			Thread.CurrentThread.CurrentCulture = culture;
			return updateResult;
		}

		private void SaveDeltaToDb(Delta delta, string fileName)
		{
			using (var db = new DeltaDBContext())
			{

				foreach(ResourceDescription rd in delta.DeleteOperations)
                {
					string mrid = rd.Properties.Find(x => x.Id == ModelCode.IDOBJ_MRID).PropertyValue.StringValue;
					var toRemove = db.Delta.Where(x => x.FileName == fileName && x.mrid == mrid);
					db.Delta.RemoveRange(toRemove);
                }

				foreach (ResourceDescription rd in delta.InsertOperations)
				{
					string mrid = rd.Properties.Find(x => x.Id == ModelCode.IDOBJ_MRID).PropertyValue.StringValue;
					db.Delta.Add(new DeltaQuerry(mrid, fileName, rd.Id));
				}
				db.SaveChanges();
			}
		}


		private bool LoadModelFromExtractFile(Stream extract, SupportedProfiles extractType, ref ConcreteModel concreteModelResult, ref Assembly assembly, out string log)
		{
			bool valid = false;
			log = string.Empty;

			System.Globalization.CultureInfo culture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
			try
			{
				ProfileManager.LoadAssembly(extractType, out assembly);
				if (assembly != null)
				{
					CIMModel cimModel = new CIMModel();
					CIMModelLoaderResult modelLoadResult = CIMModelLoader.LoadCIMXMLModel(extract, ProfileManager.Namespace, out cimModel);
					if (modelLoadResult.Success)
					{
						concreteModelResult = new ConcreteModel();
						ConcreteModelBuilder builder = new ConcreteModelBuilder();
						ConcreteModelBuildingResult modelBuildResult = builder.GenerateModel(cimModel, assembly, ProfileManager.Namespace, ref concreteModelResult);

						if (modelBuildResult.Success)
						{
							valid = true;
						}
						log = modelBuildResult.Report.ToString();
					}
					else
					{
						log = modelLoadResult.Report.ToString();
					}
				}
			}
			catch (Exception e)
			{
				log = e.Message;
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = culture;
			}
			return valid;
		}

		private bool DoTransformAndLoad(Assembly assembly, ConcreteModel concreteModel, SupportedProfiles extractType, out Delta nmsDelta, out string log, string fileName)
		{
			nmsDelta = null;
			log = string.Empty;
			bool success = false;
			try
			{
				LogManager.Log(string.Format("Importing {0} data...", extractType), LogLevel.Info);

				switch (extractType)
				{
					case SupportedProfiles.PowerTransformer:
						{
							// transformation to DMS delta					
							TransformAndLoadReport report = PowerTransformerImporter.Instance.CreateNMSDelta(concreteModel, fileName, GdaQueryProxy);

							if (report.Success)
							{
								nmsDelta = PowerTransformerImporter.Instance.NMSDelta;
								success = true;
							}
							else
							{
								success = false;
							}
							log = report.Report.ToString();
							PowerTransformerImporter.Instance.Reset();

							break;
						}
					default:
						{
							LogManager.Log(string.Format("Import of {0} data is NOT SUPPORTED.", extractType), LogLevel.Warning);
							break;
						}
				}

				return success;
			}
			catch (Exception ex)
			{
				LogManager.Log(string.Format("Import unsuccessful: {0}", ex.StackTrace), LogLevel.Error);
				return false;
			}
		}

	}
}
