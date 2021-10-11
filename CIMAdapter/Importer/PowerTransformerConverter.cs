namespace FTN.ESI.SIMES.CIM.CIMAdapter.Importer
{
	using FTN.Common;
    using System;

    /// <summary>
    /// PowerTransformerConverter has methods for populating
    /// ResourceDescription objects using PowerTransformerCIMProfile_Labs objects.
    /// </summary>
    public static class PowerTransformerConverter
	{
        #region old 
        /*
		public static void PopulateIdentifiedObjectProperties(FTN.IdentifiedObject cimIdentifiedObject, ResourceDescription rd)
		{
			if ((cimIdentifiedObject != null) && (rd != null))
			{
				if (cimIdentifiedObject.MRIDHasValue)
				{
					rd.AddProperty(new Property(ModelCode.IDOBJ_MRID, cimIdentifiedObject.MRID));
				}
				if (cimIdentifiedObject.NameHasValue)
				{
					rd.AddProperty(new Property(ModelCode.IDOBJ_NAME, cimIdentifiedObject.Name));
				}
				if (cimIdentifiedObject.DescriptionHasValue)
				{
					rd.AddProperty(new Property(ModelCode.IDOBJ_DESCRIPTION, cimIdentifiedObject.Description));
				}
			}
		}

		public static void PopulateLocationProperties(FTN.Location cimLocation, ResourceDescription rd)
		{
			if ((cimLocation != null) && (rd != null))
			{
				PowerTransformerConverter.PopulateIdentifiedObjectProperties(cimLocation, rd);

				if (cimLocation.CategoryHasValue)
				{
					rd.AddProperty(new Property(ModelCode.LOCATION_CATEGORY, cimLocation.Category));
				}
				if (cimLocation.CorporateCodeHasValue)
				{
					rd.AddProperty(new Property(ModelCode.LOCATION_CORPORATECODE, cimLocation.CorporateCode));
				}
			}
		}

		public static void PopulatePowerSystemResourceProperties(FTN.PowerSystemResource cimPowerSystemResource, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimPowerSystemResource != null) && (rd != null))
			{
				PowerTransformerConverter.PopulateIdentifiedObjectProperties(cimPowerSystemResource, rd);

				if (cimPowerSystemResource.CustomTypeHasValue)
				{
					rd.AddProperty(new Property(ModelCode.PSR_CUSTOMTYPE, cimPowerSystemResource.CustomType));
				}
				if (cimPowerSystemResource.LocationHasValue)
				{
					long gid = importHelper.GetMappedGID(cimPowerSystemResource.Location.ID);
					if (gid < 0)
					{
						report.Report.Append("WARNING: Convert ").Append(cimPowerSystemResource.GetType().ToString()).Append(" rdfID = \"").Append(cimPowerSystemResource.ID);
						report.Report.Append("\" - Failed to set reference to Location: rdfID \"").Append(cimPowerSystemResource.Location.ID).AppendLine(" \" is not mapped to GID!");
					}
					rd.AddProperty(new Property(ModelCode.PSR_LOCATION, gid));
				}
			}
		}

		public static void PopulateBaseVoltageProperties(FTN.BaseVoltage cimBaseVoltage, ResourceDescription rd)
		{
			if ((cimBaseVoltage != null) && (rd != null))
			{
				PowerTransformerConverter.PopulateIdentifiedObjectProperties(cimBaseVoltage, rd);

				if (cimBaseVoltage.NominalVoltageHasValue)
				{
					rd.AddProperty(new Property(ModelCode.BASEVOLTAGE_NOMINALVOLTAGE, cimBaseVoltage.NominalVoltage));
				}
			}
		}

		public static void PopulateEquipmentProperties(FTN.Equipment cimEquipment, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimEquipment != null) && (rd != null))
			{
				PowerTransformerConverter.PopulatePowerSystemResourceProperties(cimEquipment, rd, importHelper, report);

				if (cimEquipment.PrivateHasValue)
				{
					rd.AddProperty(new Property(ModelCode.EQUIPMENT_ISPRIVATE, cimEquipment.Private));
				}
				if (cimEquipment.IsUndergroundHasValue)
				{
					rd.AddProperty(new Property(ModelCode.EQUIPMENT_ISUNDERGROUND, cimEquipment.IsUnderground));
				}
			}
		}

		public static void PopulateConductingEquipmentProperties(FTN.ConductingEquipment cimConductingEquipment, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimConductingEquipment != null) && (rd != null))
			{
				PowerTransformerConverter.PopulateEquipmentProperties(cimConductingEquipment, rd, importHelper, report);

				if (cimConductingEquipment.PhasesHasValue)
				{
					rd.AddProperty(new Property(ModelCode.CONDEQ_PHASES, (short)GetDMSPhaseCode(cimConductingEquipment.Phases)));
				}
				if (cimConductingEquipment.RatedVoltageHasValue)
				{
					rd.AddProperty(new Property(ModelCode.CONDEQ_RATEDVOLTAGE, cimConductingEquipment.RatedVoltage));
				}
				if (cimConductingEquipment.BaseVoltageHasValue)
				{
					long gid = importHelper.GetMappedGID(cimConductingEquipment.BaseVoltage.ID);
					if (gid < 0)
					{
						report.Report.Append("WARNING: Convert ").Append(cimConductingEquipment.GetType().ToString()).Append(" rdfID = \"").Append(cimConductingEquipment.ID);
						report.Report.Append("\" - Failed to set reference to BaseVoltage: rdfID \"").Append(cimConductingEquipment.BaseVoltage.ID).AppendLine(" \" is not mapped to GID!");
					}
					rd.AddProperty(new Property(ModelCode.CONDEQ_BASVOLTAGE, gid));
				}
			}
		}

		public static void PopulatePowerTransformerProperties(FTN.PowerTransformer cimPowerTransformer, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimPowerTransformer != null) && (rd != null))
			{
				PowerTransformerConverter.PopulateEquipmentProperties(cimPowerTransformer, rd, importHelper, report);

				if (cimPowerTransformer.FunctionHasValue)
				{
					rd.AddProperty(new Property(ModelCode.POWERTR_FUNC, (short)GetDMSTransformerFunctionKind(cimPowerTransformer.Function)));
				}
				if (cimPowerTransformer.AutotransformerHasValue)
				{
					rd.AddProperty(new Property(ModelCode.POWERTR_AUTO, cimPowerTransformer.Autotransformer));
				}
			}
		}

		public static void PopulateTransformerWindingProperties(FTN.TransformerWinding cimTransformerWinding, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimTransformerWinding != null) && (rd != null))
			{
				PowerTransformerConverter.PopulateConductingEquipmentProperties(cimTransformerWinding, rd, importHelper, report);

				if (cimTransformerWinding.ConnectionTypeHasValue)
				{
					rd.AddProperty(new Property(ModelCode.POWERTRWINDING_CONNTYPE, (short)GetDMSWindingConnection(cimTransformerWinding.ConnectionType)));
				}
				if (cimTransformerWinding.GroundedHasValue)
				{
					rd.AddProperty(new Property(ModelCode.POWERTRWINDING_GROUNDED, cimTransformerWinding.Grounded));
				}
				if (cimTransformerWinding.RatedSHasValue)
				{
					rd.AddProperty(new Property(ModelCode.POWERTRWINDING_RATEDS, cimTransformerWinding.RatedS));
				}
				if (cimTransformerWinding.RatedUHasValue)
				{
					rd.AddProperty(new Property(ModelCode.POWERTRWINDING_RATEDU, cimTransformerWinding.RatedU));
				}
				if (cimTransformerWinding.WindingTypeHasValue)
				{
					rd.AddProperty(new Property(ModelCode.POWERTRWINDING_WINDTYPE, (short)GetDMSWindingType(cimTransformerWinding.WindingType)));
				}
				if (cimTransformerWinding.PhaseToGroundVoltageHasValue)
				{
					rd.AddProperty(new Property(ModelCode.POWERTRWINDING_PHASETOGRNDVOLTAGE, cimTransformerWinding.PhaseToGroundVoltage));
				}
				if (cimTransformerWinding.PhaseToPhaseVoltageHasValue)
				{
					rd.AddProperty(new Property(ModelCode.POWERTRWINDING_PHASETOPHASEVOLTAGE, cimTransformerWinding.PhaseToPhaseVoltage));
				}
				if (cimTransformerWinding.PowerTransformerHasValue)
				{
					long gid = importHelper.GetMappedGID(cimTransformerWinding.PowerTransformer.ID);
					if (gid < 0)
					{
						report.Report.Append("WARNING: Convert ").Append(cimTransformerWinding.GetType().ToString()).Append(" rdfID = \"").Append(cimTransformerWinding.ID);
						report.Report.Append("\" - Failed to set reference to PowerTransformer: rdfID \"").Append(cimTransformerWinding.PowerTransformer.ID).AppendLine(" \" is not mapped to GID!");
					}
					rd.AddProperty(new Property(ModelCode.POWERTRWINDING_POWERTRW, gid));
				}
			}
		}

		public static void PopulateWindingTestProperties(FTN.WindingTest cimWindingTest, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimWindingTest != null) && (rd != null))
			{
				PowerTransformerConverter.PopulateIdentifiedObjectProperties(cimWindingTest, rd);

				if (cimWindingTest.LeakageImpedanceHasValue)
				{
					rd.AddProperty(new Property(ModelCode.WINDINGTEST_LEAKIMPDN, cimWindingTest.LeakageImpedance));
				}
				if (cimWindingTest.LoadLossHasValue)
				{
					rd.AddProperty(new Property(ModelCode.WINDINGTEST_LOADLOSS, cimWindingTest.LoadLoss));
				}
				if (cimWindingTest.NoLoadLossHasValue)
				{
					rd.AddProperty(new Property(ModelCode.WINDINGTEST_NOLOADLOSS, cimWindingTest.NoLoadLoss));
				}
				if (cimWindingTest.PhaseShiftHasValue)
				{
					rd.AddProperty(new Property(ModelCode.WINDINGTEST_PHASESHIFT, cimWindingTest.PhaseShift));
				}
				if (cimWindingTest.LeakageImpedance0PercentHasValue)
				{
					rd.AddProperty(new Property(ModelCode.WINDINGTEST_LEAKIMPDN0PERCENT, cimWindingTest.LeakageImpedance0Percent));
				}
				if (cimWindingTest.LeakageImpedanceMaxPercentHasValue)
				{
					rd.AddProperty(new Property(ModelCode.WINDINGTEST_LEAKIMPDNMAXPERCENT, cimWindingTest.LeakageImpedanceMaxPercent));
				}
				if (cimWindingTest.LeakageImpedanceMinPercentHasValue)
				{
					rd.AddProperty(new Property(ModelCode.WINDINGTEST_LEAKIMPDNMINPERCENT, cimWindingTest.LeakageImpedanceMinPercent));
				}

				if (cimWindingTest.From_TransformerWindingHasValue)
				{
					long gid = importHelper.GetMappedGID(cimWindingTest.From_TransformerWinding.ID);
					if (gid < 0)
					{
						report.Report.Append("WARNING: Convert ").Append(cimWindingTest.GetType().ToString()).Append(" rdfID = \"").Append(cimWindingTest.ID);
						report.Report.Append("\" - Failed to set reference to TransformerWinding: rdfID \"").Append(cimWindingTest.From_TransformerWinding.ID).AppendLine(" \" is not mapped to GID!");
					}
					rd.AddProperty(new Property(ModelCode.WINDINGTEST_POWERTRWINDING, gid));
				}
			}
		}
		*/
        #endregion

        public static void PopulateIdentifiedObjectProperties(IdentifiedObject cim, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
			if ((cim != null) && (rd != null))
            {
				if (cim.MRIDHasValue)
				{
					rd.AddProperty(new Property(ModelCode.IDOBJ_MRID, cim.MRID));
				}
				else if (IdentifiedObject.IsMRIDMandatory)
                {
					DefaultProperty(rd, ModelCode.IDOBJ_MRID);
                }

				if (cim.NameHasValue)
				{
					rd.AddProperty(new Property(ModelCode.IDOBJ_NAME, cim.Name));
				}
				else if (IdentifiedObject.IsNameMandatory)
                {
					DefaultProperty(rd, ModelCode.IDOBJ_NAME);
				}

				if (cim.AliasNameHasValue)
				{
					rd.AddProperty(new Property(ModelCode.IDOBJ_ALIASNAME, cim.AliasName));
				}
				else if (IdentifiedObject.IsAliasNameMandatory)
				{
					DefaultProperty(rd, ModelCode.IDOBJ_ALIASNAME);
				}


			}
		}

        private static void DefaultProperty(ResourceDescription rd, ModelCode modelCode)
        {
			Defaulter.PopulateDefaultProperty(rd, modelCode);
        }

        public static void PopulatePowerSystemResourceProperties(PowerSystemResource cim, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
			if ((cim != null) && (rd != null))
            {
				PopulateIdentifiedObjectProperties(cim, rd, importHelper, report);
			}

		}

		public static void PopulateACLineSegmentPhaseProperties(ACLineSegmentPhase cim, ResourceDescription rd,ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cim != null) && (rd != null))
			{
				PopulatePowerSystemResourceProperties(cim, rd, importHelper, report);

				if (cim.PhaseHasValue)
                {
					rd.AddProperty(new Property(ModelCode.ACLINESEGMENTPHASE_PHASE, (short)cim.Phase));
                }
				else if (ACLineSegmentPhase.IsPhaseMandatory)
                {
					DefaultProperty(rd, ModelCode.ACLINESEGMENTPHASE_PHASE);
				}

				if (cim.ACLineSegmentHasValue)
                {
					long gid = importHelper.GetMappedGID(cim.ACLineSegment.ID);
					if (gid < 0)
					{
						report.Report.Append("WARNING: Convert ").Append(cim.GetType().ToString()).Append(" rdfID = \"").Append(cim.ID);
						report.Report.Append("\" - Failed to set reference to ACLineSegment: rdfID \"").Append(cim.ACLineSegment.ID).AppendLine(" \" is not mapped to GID!");
					}
					rd.AddProperty(new Property(ModelCode.ACLINESEGMENTPHASE_ACLINESEGMENT, gid));
				}
			}
		}

		public static void PopulateEquipmentProperties(Equipment cim, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
			if ((cim != null) && (rd != null))
			{
				PopulatePowerSystemResourceProperties(cim, rd, importHelper, report);
			}
		}



		public static void PopulateConductingEquipmentProperties(ConductingEquipment cim, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
			if ((cim != null) && (rd != null))
			{
				PopulateEquipmentProperties(cim, rd, importHelper, report);
			}
		}



		public static void PopulateConductorProperties(Conductor cim, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
			if ((cim != null) && (rd != null))
			{
				PopulateConductingEquipmentProperties(cim, rd, importHelper, report);
			}
		}



		public static void PopulateACLineSegmentProperties(ACLineSegment cim, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cim != null) && (rd != null))
			{
				PopulateConductorProperties(cim, rd, importHelper, report);


				if (cim.B0chHasValue)
				{
					rd.AddProperty(new Property(ModelCode.ACLINESEGMENT_B0CH, cim.B0ch));
				}
				else if (ACLineSegment.IsB0chMandatory)
				{
					DefaultProperty(rd, ModelCode.ACLINESEGMENT_B0CH);
				}

				if (cim.G0chHasValue)
				{
					rd.AddProperty(new Property(ModelCode.ACLINESEGMENT_G0CH, cim.G0ch));
				}
				else if (ACLineSegment.IsG0chMandatory)
				{
					DefaultProperty(rd, ModelCode.ACLINESEGMENT_G0CH);
				}

				if (cim.R0HasValue)
				{
					rd.AddProperty(new Property(ModelCode.ACLINESEGMENT_R0, cim.R0));
				}
				else if (ACLineSegment.IsR0Mandatory)
				{
					DefaultProperty(rd, ModelCode.ACLINESEGMENT_R0);
				}

				if (cim.X0HasValue)
				{
					rd.AddProperty(new Property(ModelCode.ACLINESEGMENT_X0, cim.X0));
				}
				else if (ACLineSegment.IsX0Mandatory)
				{
					DefaultProperty(rd, ModelCode.ACLINESEGMENT_X0);
				}

				if (cim.XHasValue)
				{
					rd.AddProperty(new Property(ModelCode.ACLINESEGMENT_X, cim.X));
				}
				else if (ACLineSegment.IsXMandatory)
				{
					DefaultProperty(rd, ModelCode.ACLINESEGMENT_X);
				}

				if (cim.RHasValue)
				{
					rd.AddProperty(new Property(ModelCode.ACLINESEGMENT_R, cim.R));
				}
				else if (ACLineSegment.IsRMandatory)
				{
					DefaultProperty(rd, ModelCode.ACLINESEGMENT_R);
				}

				if (cim.GchHasValue)
				{
					rd.AddProperty(new Property(ModelCode.ACLINESEGMENT_GCH, cim.Gch));
				}
				else if (ACLineSegment.IsGchMandatory)
				{
					DefaultProperty(rd, ModelCode.ACLINESEGMENT_GCH);
				}

				if (cim.BchHasValue)
				{
					rd.AddProperty(new Property(ModelCode.ACLINESEGMENT_BCH, cim.Bch));
				}
				else if (ACLineSegment.IsBchMandatory)
				{
					DefaultProperty(rd, ModelCode.ACLINESEGMENT_BCH);
				}
			}
		}

		public static void PopulateMutualCouplingProperties(MutualCoupling cim, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cim != null) && (rd != null))
            {
				PopulateIdentifiedObjectProperties(cim, rd, importHelper, report);

				if (cim.B0chHasValue)
                {
					rd.AddProperty(new Property(ModelCode.MUTUALCOUPLING_B0CH, cim.B0ch));
				}
				else if (MutualCoupling.IsB0chMandatory)
				{
					DefaultProperty(rd, ModelCode.MUTUALCOUPLING_B0CH);
				}

				if (cim.G0chHasValue)
				{
					rd.AddProperty(new Property(ModelCode.MUTUALCOUPLING_G0CH, cim.G0ch));
				}
				else if (MutualCoupling.IsG0chMandatory)
				{
					DefaultProperty(rd, ModelCode.MUTUALCOUPLING_G0CH);
				}

				if (cim.R0HasValue)
				{
					rd.AddProperty(new Property(ModelCode.MUTUALCOUPLING_R0, cim.R0));
				}
				else if (MutualCoupling.IsR0Mandatory)
				{
					DefaultProperty(rd, ModelCode.MUTUALCOUPLING_R0);
				}

				if (cim.X0HasValue)
				{
					rd.AddProperty(new Property(ModelCode.MUTUALCOUPLING_X0, cim.X0));
				}
				else if (MutualCoupling.IsX0Mandatory)
				{
					DefaultProperty(rd, ModelCode.MUTUALCOUPLING_X0);
				}

				if (cim.Distance11HasValue)
				{
					rd.AddProperty(new Property(ModelCode.MUTUALCOUPLING_D11, cim.Distance11));
				}
				else if (MutualCoupling.IsDistance11Mandatory)
				{
					DefaultProperty(rd, ModelCode.MUTUALCOUPLING_D11);
				}

				if (cim.Distance12HasValue)
				{
					rd.AddProperty(new Property(ModelCode.MUTUALCOUPLING_D12, cim.Distance12));
				}
				else if (MutualCoupling.IsDistance12Mandatory)
				{
					DefaultProperty(rd, ModelCode.MUTUALCOUPLING_D12);
				}

				if (cim.Distance21HasValue)
				{
					rd.AddProperty(new Property(ModelCode.MUTUALCOUPLING_D21, cim.Distance21));
				}
				else if (MutualCoupling.IsDistance21Mandatory)
				{
					DefaultProperty(rd, ModelCode.MUTUALCOUPLING_D21);
				}

				if (cim.Distance22HasValue)
				{
					rd.AddProperty(new Property(ModelCode.MUTUALCOUPLING_D22, cim.Distance22));
				}
				else if (MutualCoupling.IsDistance22Mandatory)
				{
					DefaultProperty(rd, ModelCode.MUTUALCOUPLING_D22);
				}

				if (cim.First_TerminalHasValue)
				{
					long gid = importHelper.GetMappedGID(cim.First_Terminal.ID);
					if (gid < 0)
					{
						report.Report.Append("WARNING: Convert ").Append(cim.GetType().ToString()).Append(" rdfID = \"").Append(cim.ID);
						report.Report.Append("\" - Failed to set reference to First_Terminal: rdfID \"").Append(cim.First_Terminal.ID).AppendLine(" \" is not mapped to GID!");
					}
					rd.AddProperty(new Property(ModelCode.MUTUALCOUPLING_FIRSTTERMINAL, gid));
				}
				if (cim.Second_TerminalHasValue)
				{
					long gid = importHelper.GetMappedGID(cim.Second_Terminal.ID);
					if (gid < 0)
					{
						report.Report.Append("WARNING: Convert ").Append(cim.GetType().ToString()).Append(" rdfID = \"").Append(cim.ID);
						report.Report.Append("\" - Failed to set reference to Second_Terminal: rdfID \"").Append(cim.First_Terminal.ID).AppendLine(" \" is not mapped to GID!");
					}
					rd.AddProperty(new Property(ModelCode.MUTUALCOUPLING_SECONDTERMINAL, gid));
				}
			}
		}

		public static void PopulateTerminalProperties(Terminal cim, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cim != null) && (rd != null))
			{
				PopulateIdentifiedObjectProperties(cim, rd, importHelper, report);

				if (cim.ConnectedHasValue)
				{
					rd.AddProperty(new Property(ModelCode.TERMINAL_CONNECTED, cim.Connected));
				}
				else if (Terminal.IsConnectedMandatory)
				{
					DefaultProperty(rd, ModelCode.TERMINAL_CONNECTED);
				}

				if (cim.PhasesHasValue)
				{
					rd.AddProperty(new Property(ModelCode.TERMINAL_PHASE, (short)cim.Phases));
				}
				else if (Terminal.IsPhasesMandatory)
				{
					DefaultProperty(rd, ModelCode.TERMINAL_PHASE);
				}

				if (cim.SequenceNumberHasValue)
				{
					rd.AddProperty(new Property(ModelCode.TERMINAL_SQCNUM, cim.SequenceNumber));
				}
				else if (Terminal.IsSequenceNumberMandatory)
				{
					DefaultProperty(rd, ModelCode.TERMINAL_SQCNUM);
				}

				if (cim.ConductingEquipmentHasValue)
				{
					long gid = importHelper.GetMappedGID(cim.ConductingEquipment.ID);
					if (gid < 0)
					{
						report.Report.Append("WARNING: Convert ").Append(cim.GetType().ToString()).Append(" rdfID = \"").Append(cim.ID);
						report.Report.Append("\" - Failed to set reference to ConductingEquipment: rdfID \"").Append(cim.ConductingEquipment.ID).AppendLine(" \" is not mapped to GID!");
					}
					rd.AddProperty(new Property(ModelCode.TERMINAL_CONDQEQ, gid));
				}
			}
		}

		public static PhaseCode GetDMSPhaseCode(FTN.PhaseCode phases, ImportHelper importHelper, TransformAndLoadReport report)
		{
			switch (phases)
			{
				case FTN.PhaseCode.A:
					return PhaseCode.A;
				case FTN.PhaseCode.AB:
					return PhaseCode.AB;
				case FTN.PhaseCode.ABC:
					return PhaseCode.ABC;
				case FTN.PhaseCode.ABCN:
					return PhaseCode.ABCN;
				case FTN.PhaseCode.ABN:
					return PhaseCode.ABN;
				case FTN.PhaseCode.AC:
					return PhaseCode.AC;
				case FTN.PhaseCode.ACN:
					return PhaseCode.ACN;
				case FTN.PhaseCode.AN:
					return PhaseCode.AN;
				case FTN.PhaseCode.B:
					return PhaseCode.B;
				case FTN.PhaseCode.BC:
					return PhaseCode.BC;
				case FTN.PhaseCode.BCN:
					return PhaseCode.BCN;
				case FTN.PhaseCode.BN:
					return PhaseCode.BN;
				case FTN.PhaseCode.C:
					return PhaseCode.C;
				case FTN.PhaseCode.CN:
					return PhaseCode.CN;
				case FTN.PhaseCode.N:
					return PhaseCode.N;
				case FTN.PhaseCode.s12N:
					return PhaseCode.ABN;
				case FTN.PhaseCode.s1N:
					return PhaseCode.AN;
				case FTN.PhaseCode.s2N:
					return PhaseCode.BN;
				default: return PhaseCode.Unknown;
			}
		}



        /*
		public static TransformerFunction GetDMSTransformerFunctionKind(FTN.TransformerFunctionKind transformerFunction)
		{
			switch (transformerFunction)
			{
				case FTN.TransformerFunctionKind.voltageRegulator:
					return TransformerFunction.Voltreg;
				default:
					return TransformerFunction.Consumer;
			}
		}

		public static WindingType GetDMSWindingType(FTN.WindingType windingType)
		{
			switch (windingType)
			{
				case FTN.WindingType.primary:
					return WindingType.Primary;
				case FTN.WindingType.secondary:
					return WindingType.Secondary;
				case FTN.WindingType.tertiary:
					return WindingType.Tertiary;
				default:
					return WindingType.None;
			}
		}

		public static WindingConnection GetDMSWindingConnection(FTN.WindingConnection windingConnection)
		{
			switch (windingConnection)
			{
				case FTN.WindingConnection.D:
					return WindingConnection.D;
				case FTN.WindingConnection.I:
					return WindingConnection.I;
				case FTN.WindingConnection.Z:
					return WindingConnection.Z;
				case FTN.WindingConnection.Y:
					return WindingConnection.Y;
				default:
					return WindingConnection.Y;
			}
		}

		*/
    }
}
