using FTN.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace FTN.ESI.SIMES.CIM.CIMAdapter.Importer
{
    public static class Defaulter
    {
        private static string path = @"..\Resources\defaulter.xml";

        public static void PopulateDefaultProperty(ResourceDescription rd, ModelCode modelCode)
        {
            string result = "";

            XmlDocument xml = new XmlDocument();
            xml.Load(path);

            XmlElement elt = (xml.SelectSingleNode("defaulter") as XmlElement).SelectSingleNode(modelCode.ToString()) as XmlElement;
            if (elt == null)
            {
                throw new Exception($"{modelCode} not found in Defaulter parameters.");
            }

            result = elt.InnerText;

            switch(modelCode)
            {
                case ModelCode.TERMINAL_CONNECTED:
                    bool res = result == "true" || result == "True" || result == "TRUE";
                    rd.AddProperty(new Property(modelCode, res));
                    break;

                case ModelCode.ACLINESEGMENTPHASE_PHASE:
                    string suffix = result.Split('.')[1];
                    SinglePhaseKind singlePhaseKind = (SinglePhaseKind)Enum.Parse(typeof(SinglePhaseKind), suffix);
                    rd.AddProperty(new Property(modelCode, (short)singlePhaseKind));
                    break;


                case ModelCode.TERMINAL_PHASE:
                    string suffix2 = result.Split('.')[1];
                    PhaseCode phaseCode = (PhaseCode)Enum.Parse(typeof(PhaseCode), suffix2);
                    rd.AddProperty(new Property(modelCode, (short)phaseCode));
                    break;

                case ModelCode.IDOBJ_ALIASNAME:
                case ModelCode.IDOBJ_NAME:
                    rd.AddProperty(new Property(modelCode, result));
                    break;

                case ModelCode.TERMINAL_SQCNUM:
                    rd.AddProperty(new Property(modelCode, Int32.Parse(result)));
                    break;

                default:
                    rd.AddProperty(new Property(modelCode, float.Parse(result)));
                    break;
            }

        }
    }
}
