using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace BTPDataImportExport
{
    public class TrayConfigIE
    {
        public BTPUtilities.SerializableDictionary<int, BTPUtilities.Tray> Config = null;

        public bool Export(Dictionary<int, BTPUtilities.Tray> trayConfig, String path)
        {
            Config = TrayConfigToSeriazableDictionary(trayConfig);

            if (Config == null)
                return false;

            return Serialize(path, this);
        }

        public short[] TrayConfigToShortTable(Dictionary<int, BTPUtilities.Tray> trayConfig)
        {
            short[] rv = new short[801];

            rv[0] = 1;


            int i = 1;
            foreach (KeyValuePair<int, BTPUtilities.Tray> kvp in trayConfig)
            {
                rv[i] = Convert.ToInt16(kvp.Value.ID);
                i++;
                rv[i] = Convert.ToInt16(kvp.Value.Height);
                i++;
                rv[i] = Convert.ToInt16(kvp.Value.Position);
                i++;
                rv[i] = Convert.ToInt16(kvp.Value.Column);
                i++;

            }

            for (int j = i; j < 801; j++)
            {
                rv[j] = 0;
            }


            return rv;
        }

        public Dictionary<int, BTPUtilities.Tray> Import(String path)
        {
            TrayConfigIE t = Deserialize(path);
            if (t == null)
                return null;

            Dictionary<int, BTPUtilities.Tray> rv = new Dictionary<int, BTPUtilities.Tray>();

            try
            {
                foreach (KeyValuePair<int, BTPUtilities.Tray> kvp in t.Config)
                {
                    rv.Add(kvp.Key, kvp.Value);
                }
            }
            catch
            {
                return null;
            }

            return rv;


        }

        private static BTPUtilities.SerializableDictionary<int, BTPUtilities.Tray> TrayConfigToSeriazableDictionary(Dictionary<int, BTPUtilities.Tray> trayConfig)
        {
            BTPUtilities.SerializableDictionary<int, BTPUtilities.Tray> rv = new BTPUtilities.SerializableDictionary<int, BTPUtilities.Tray>();

            if (trayConfig == null)
                return null;

            foreach (KeyValuePair<int, BTPUtilities.Tray> kvp in trayConfig)
            {
                try
                {
                    rv.Add(kvp.Key, kvp.Value);
                }
                catch { }
            }

            if (trayConfig.Count == rv.Count)
                return rv;
            else
                return null;

        }

        private static bool Serialize(String filePath, TrayConfigIE tcie)
        {
            try
            {
                XmlSerializer xmls = new XmlSerializer(tcie.GetType());
                TextWriter WriteFileStream = new StreamWriter(filePath);
                xmls.Serialize(WriteFileStream, tcie);
                WriteFileStream.Close();
                return true;
            }
            catch
            {
                return false;
            }


        }

        public static TrayConfigIE Deserialize(String filePath)
        {
            TrayConfigIE t = null;

            try
            {
                XmlSerializer xmls = new XmlSerializer(typeof(TrayConfigIE));

                FileStream ReadFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                t = (TrayConfigIE)xmls.Deserialize(ReadFileStream);

                ReadFileStream.Close();
            }
            catch { }

            return t;
        }


    }
}
