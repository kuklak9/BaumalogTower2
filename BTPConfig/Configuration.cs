using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace BTPConfig
{
    public class Configuration
    {
        // Constructor
        public Configuration()
        {
            ReadConfig();
        }

        #region FIELDS
        public int WindowNr = 1;
        private int _towerNr = 1;
        public int TowerNrControl
        {
            get => _towerNr;
            set
            {
                bool towerNrChanged = _towerNr != value;

                _towerNr = value;

                if (towerNrChanged)
                    TowerNrChanged?.Invoke(this, new BTPUtilities.IntEventArgs(_towerNr));
            }
        }
        public int TowerNrCfg
        {
            internal set;
            get;
        }

        public Dictionary<int, String[]> IO_Description = new Dictionary<int, String[]>();
        public Dictionary<String, String> AvailableLanguages = new Dictionary<string, string>();

        public DataTable confTab = new DataTable();
        private ConfTable CT = new ConfTable();
        private DataSet DS = new DataSet();

        public String DBWMSServerName;
        public String DBWMSDbName;
        public String DBWMS_Local;
        public String DBWMSServerName_Remote;
        public String DBWMSDbName_Remote;
        public String DBWMS_Remote;
        public String DBWMSServerName_Remote_new;
        public String DBWMSDbName_Remote_new;
        public String DBWMS_Remote_new;

        public event EventHandler<BTPUtilities.IntEventArgs> TowerNrChanged;

        private ConfTableTableAdapters.ParametryTableAdapter pta = new ConfTableTableAdapters.ParametryTableAdapter();
        private ConfTableTableAdapters.PLC_ZmienneTableAdapter plc_ta = new ConfTableTableAdapters.PLC_ZmienneTableAdapter();
        #endregion

        #region language operator setting
        public void ChangeLanguage(String language)
        {
            SaveLanguage(language);
        }
        public void ChangeOperator(String user)
        {
            SaveOperator(user);
        }
        private void SaveLanguage(String language)
        {
            try
            {
                pta.Update(language, "LANG_Selected");
                pta.Fill(CT.Parametry);
            }
            catch { }
        }
        private void SaveOperator(String user)
        {
            {
                try
                {
                    pta.Update(user, "Operator");
                    pta.Fill(CT.Parametry);
                }
                catch { }
            }
        }
        #endregion

        #region Setting connection string 
        private void GetConfTable()
        {
            pta.Connection.ConnectionString = DBWMS_Local;
            plc_ta.Connection.ConnectionString = DBWMS_Local;
            pta.Fill(CT.Parametry);
            plc_ta.Fill(CT.PLC_Zmienne);

            //Sciezka do zdalnej bazy danych
            DBWMSServerName_Remote = GetParamStr("DB_RemoteServerName");
            DBWMSDbName_Remote = GetParamStr("DB_RemoteDbName");
            DBWMS_Remote = "Data Source=" + DBWMSServerName_Remote + "; Initial Catalog=" + DBWMSDbName_Remote + ";User ID=wms;Password=1";

            DBWMSServerName_Remote_new = GetParamStr("DB_BACAServerName");
            DBWMSDbName_Remote_new = GetParamStr("DB_BACADbName");
            DBWMS_Remote_new = "Data Source=" + DBWMSServerName_Remote_new + "; Initial Catalog=" + DBWMSDbName_Remote_new + ";User ID=wms;Password=1";

            WindowNr = GetParamInt("TOWER_WindowNr");
            TowerNrControl = GetParamInt("TOWER_TowerNumber");
            TowerNrCfg = TowerNrControl;

            if (TowerNrControl == 0)
                TowerNrControl = 1;

            if (WindowNr == 0)
                WindowNr = 1;

            //InitializeDbParametresClasses();
        }
        private void GetDBConfig(DataSet dscfg)
        {
            try
            {
                DBWMSServerName = dscfg.Tables["DBWMS"].Rows[0]["Server"].ToString();
                DBWMSDbName = dscfg.Tables["DBWMS"].Rows[0]["Database"].ToString();
                DBWMS_Local = "Data Source=" + DBWMSServerName + "; Initial Catalog=" + DBWMSDbName + ";User ID=wms;Password=1";
            }
            catch { }
        }
        #endregion

        #region Getting parameters from DB table
        private void InitializeDbParametresClasses()
        {
            var cfgAssembly = System.Reflection.Assembly.GetCallingAssembly();
            var dbParametersClasses = cfgAssembly.GetTypes()
                .Where(r => r.Namespace.Contains("DBParameters")
                    && r.IsClass
                    && r.IsAbstract
                    && r.IsSealed);

            foreach (var dbClass in dbParametersClasses)
            {
                var className = dbClass.Name.ToUpper();

                var staticProperties = dbClass
                    .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

                foreach (var property in staticProperties)
                {
                    var propertyType = property.PropertyType;


                    dynamic value;

                    if (propertyType == typeof(string))
                        value = "";
                    else
                        value = Activator.CreateInstance(propertyType);


                    var valueName = className + "_" + property.Name;


                    if (propertyType == typeof(bool))
                    {
                        value = GetParamBool(valueName);
                    }
                    else if (propertyType == typeof(int))
                    {
                        value = GetParamInt(valueName);
                    }
                    else if (propertyType == typeof(string))
                    {
                        value = GetParamStr(valueName);
                    }


                    property.SetValue(null, value);
                }
            }
        }
        public string ReadLanguage()
        {
            try
            {
                return CT.Parametry.FindByParamID("Lang_Selected").Wartosc;
            }
            catch
            {
                return "lang1";
            }

        }
        public string GetParamStr(string paramName)
        {
            string temp;
            try
            {
                temp = Convert.ToString(CT.Parametry.FindByParamID(paramName).Wartosc);
                return temp;
            }
            catch
            {
                return "";
            }
        }
        public int GetParamInt(string paramName)
        {
            int temp;
            try
            {
                temp = Convert.ToInt32(CT.Parametry.FindByParamID(paramName).Wartosc);
                return temp;
            }
            catch
            {
                return 0;
            }
        }
        public bool GetParamBool(string paramName)
        {
            bool temp = false;
            try
            {
                if (Convert.ToInt32(CT.Parametry.FindByParamID(paramName).Wartosc) == 1)
                    temp = true;
                return temp;
            }
            catch
            {
                return false;
            }
        }
        private void GetLanguageListConfigDB()
        {
            try
            {
                int languageCount = GetParamInt("LANG_Count");

                if (languageCount < 1)
                    languageCount = 1;
                else if (languageCount > 4)
                    languageCount = 4;

                for (int i = 1; i <= languageCount; i++)
                {
                    string langTypeDB = "lang" + i.ToString();
                    AvailableLanguages.Add(langTypeDB, langTypeDB);
                }
            }
            catch { }
        }
        #endregion

        #region IO config
        private void GetIOConfig(DataSet dscfg)
        {
            try
            {
                IO_Description.Clear();
                foreach (DataRow dr in dscfg.Tables["IO"].Rows)
                {
                    try
                    {
                        int nr = Convert.ToInt32(dr[0]);
                        int input = Convert.ToInt32(dr[2]);
                        IO_Description.Add(nr + (input == 1 ? 0 : 1000), new string[] { dr[1].ToString(), dr[3].ToString() });
                    }
                    catch { }
                }
            }
            catch { }
        }
        public ConfTable.PLC_ZmienneRow[] GetPlcValues()
        {
            try
            {
                return CT.PLC_Zmienne.AsEnumerable()
                    .Where(r => r.IsDisabledNull() || !r.Disabled)
                    .Select(r => r).ToArray();
            }
            catch
            {
                return null;
            }
        }
        public ConfTable.PLC_ZmienneRow[] GetPlcLogValues()
        {
            try
            {
                return CT.PLC_Zmienne.AsEnumerable()
                    .Where(r => (r.IsDisabledNull() || !r.Disabled) && !r.IsLoguj_zmianeNull() && r.Loguj_zmiane)
                    .Select(r => r).ToArray();
            }
            catch
            {
                return null;
            }
        }
        public String ReadConfig()
        {
            try
            {
                DataSet dscfg = new DataSet();
                String path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                path = System.IO.Path.GetDirectoryName(path);
                try
                {
                    dscfg.ReadXml(path + "\\Tower.config");
                }
                catch { }
                GetDBConfig(dscfg);

                DataSet dscfgio = new DataSet();
                try
                {
                    dscfgio.Tables.Add("IO");
                    dscfgio.Tables["IO"].Columns.Add("ID");
                    dscfgio.Tables["IO"].Columns.Add("OPIS");
                    dscfgio.Tables["IO"].Columns.Add("INPUT");
                    dscfgio.Tables["IO"].Columns.Add("URZADZENIE");

                    DataSet tmp = new DataSet();
                    tmp.ReadXml(path + "\\Tabs\\signalsComponents\\DI.xml");
                    tmp.Tables[0].Columns.Add("INPUT");
                    foreach (DataRow d in tmp.Tables[0].Rows)
                    {
                        d["INPUT"] = "1";
                    }
                    tmp.ReadXml(path + "\\Tabs\\signalsComponents\\DO.xml");
                    foreach (DataRow d in tmp.Tables[0].Select("ISNULL(INPUT,'')='' "))
                    {
                        d["INPUT"] = "0";
                    }
                    tmp.Tables[0].Columns["NAME"].ColumnName = "OPIS";
                    tmp.Tables[0].Columns["Nr"].ColumnName = "ID";
                    tmp.Tables[0].Columns["Device"].ColumnName = "URZADZENIE";

                    dscfgio.Tables[0].Merge(tmp.Tables[0], true, MissingSchemaAction.Ignore);
                }
                catch { }
                try
                {
                    GetIOConfig(dscfgio);

                }
                catch { }
                GetConfTable();
                GetLanguageListConfigDB();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion
    }
}
