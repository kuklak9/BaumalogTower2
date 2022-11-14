using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Controls;
using System.IO;
using BTPUtilities;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;

namespace BTPTranslation
{
    public class Translation
    {

        BTPConfig.Configuration _cfg = null;
        private TransAppTexts CT = new TransAppTexts();
        private TransAppTextsTableAdapters.TlumaczeniaTableAdapter pta = new TransAppTextsTableAdapters.TlumaczeniaTableAdapter();
        private TransAppTextsTableAdapters.WPFTranslationTableAdapter wpftrans = new TransAppTextsTableAdapters.WPFTranslationTableAdapter();
        public event EventHandler<EventArgs> TransalationsLoaded;

        public Translation(BTPConfig.Configuration cfg)
        {
            _cfg = cfg;

            Task.Factory.StartNew(
                () =>
                {
                    while (!ReadTransalationFromDB())
                    {
                        //Thread.Sleep(30000);
                    }
                },
                cancellationToken: CancellationToken.None,
                creationOptions: TaskCreationOptions.None,
                scheduler: TaskScheduler.Default);
        }

        public void TranslateControl(String language, Object mp, string parentWindowPath, string prefixForSetWindowControls)
        {
            //  String project = mp.GetType().Assembly.GetName().Name;
            String control = parentWindowPath;
            Control c = mp as Control;
            if (c == null)
                return;

            TranslateControl(language, c, control, false, prefixForSetWindowControls);
        }

        public ResourceDictionary ReadTranslations()
        {
            try
            {
                ResourceDictionary lang = new();
                DataRow[] translations = wpftrans.GetData().Select("Control = 'WPF'");
                foreach (DataRow item in translations)
                {
                    lang.Add(item["Path"], item[_cfg.GetParamStr("LANG_Selected")]);
                }
                return lang;
            }
            catch { return null; }
        }

        public void TranslateControl(String language, Object mp)
        {
            //  String project = mp.GetType().Assembly.GetName().Name;
            String control = mp.GetType().FullName.Replace("+", ".");
            Control c = mp as Control;
            if (c == null)
                return;

            TranslateControl(language, c, control, false);
        }


        private void TranslateControl(String language, Control c, /*String project, */String controlParentName, bool req)
        {
            //if (req && (c.GetType().BaseType.FullName.Contains("UserControl") || c.GetType().BaseType.FullName.Contains("MainPanel")))
            //{
            //    return;
            //}

            //foreach (Control ctrl in c.)
            //{
            //    if (ctrl.HasChildren)
            //    {
            //        TranslateControl(language, ctrl, /*project,*/ controlParentName, true);
            //    }
            //    String s = GetTraslatedText(language, /*project,*/ controlParentName, ctrl.Name, 0);
            //    if (!String.IsNullOrWhiteSpace(s))
            //        ctrl.Text = s;
            //}
        }

        /// <summary>
        /// Tłumaczenie jeżeli okno osazdone 
        /// </summary>
        private void TranslateControl(String language, Control c, /*String project, */String controlParentName, bool req, string prefixForSetWindowControls)
        {
            //if (req && (c.GetType().BaseType.FullName.Contains("UserControl") || c.GetType().BaseType.FullName.Contains("MainPanel")))
            //{
            //    return;
            //}

            //foreach (Control ctrl in c.Controls)
            //{
            //    if (ctrl.HasChildren)
            //    {
            //        TranslateControl(language, ctrl, /*project,*/ controlParentName, true, prefixForSetWindowControls);
            //    }
            //    String s = GetTraslatedText(language, /*project,*/ controlParentName, prefixForSetWindowControls + ctrl.Name, 0);
            //    if (!String.IsNullOrWhiteSpace(s))
            //        ctrl.Text = s;
            //}
        }


        public String GetTraslatedText(String language, /*String project,*/ String control, String path, int nr)
        {
            return GetTraslatedText(language, "Control", /*project, */ control, path, nr);
        }

        public String GetTraslatedText(String language, Object mp, Object c, int nr)
        {
            String control = mp.GetType().FullName.Replace("+", ".");
            String path = ((System.Windows.Controls.Control)c).Name;
            return GetTraslatedText(language, "Control", /*project, */ control, path, nr);
        }


        public String GetTraslatedText(String language, Object mp, String path, int nr)
        {
            String control = mp.GetType().FullName.Replace("+", ".");
            return GetTraslatedText(language, "Control", control, path, nr);
        }

        public DataTable GetTransportSystem()
        {
            DataTable dt = new DataTable();
            try
            {
                dt = pta.GetDataBy("TransportSystem");
            }
            catch { return dt; }
            return dt;
        }

        public System.Data.DataTable GetStatisticsDescriptionDT(String language)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            try
            {
                dt = pta.GetDataBy("Statistics");
            }
            catch { return dt; }
            return dt;
        }

        public string[][] GetDistinktParams(string[] conditions)
        {
            string[][] temp = new string[conditions.Length][];
            string[] filtredColums = new string[] { "Typ", "Control", "Path", "Nr" };
            //bool[] isFiltred = new bool[conditions.Length];

            //for(int i = 0; i < conditions.Length; i++)
            //{
            //    if (conditions[i] != "")
            //        isFiltred[i] = true;
            //}
            try
            {   //Błąd - brak warunków
                DataTable sourceDT = GetLanguageDataTablev2(conditions, filtredColums);
                for (int i = 0; i < conditions.Length; i++)
                {
                    DataView dv = sourceDT.DefaultView;
                    dv.Sort = sourceDT.Columns[i].ColumnName + " ASC";
                    DataTable dt = dv.ToTable(true, sourceDT.Columns[i].ColumnName);
                    List<string> ls = new List<string>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        ls.Add(dr[0].ToString());
                    }
                    temp[i] = ls.ToArray();
                }
            }
            catch { }
            return temp;
        }

        public DataTable GetLanguageDataTablev2(string[] conditions, string[] filtredColums)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                if (!String.IsNullOrEmpty(conditions[i]))
                    conditions[i] = conditions[i].ToLower();
            }
            DataTable dt = CT.Tlumaczenia;
            //foreach (DataRow dr in dt.Rows)
            //{
            //    for (int i = 0; i < 3; i++)
            //        dr[i] = dr[i].ToString().ToLower();
            //}

            for (int i = 0; i < conditions.Length; i++)
            {
                if (!String.IsNullOrEmpty(conditions[i]))
                {
                    try
                    {
                        dt = dt.AsEnumerable().Where(myRow => myRow.Field<string>(filtredColums[i]).ToString().ToLower().Contains(conditions[i])).CopyToDataTable();
                    }
                    catch
                    {
                        dt = dt.AsEnumerable().Where(myRow => myRow.Field<Int32>(filtredColums[i]).ToString().ToString().ToLower().Contains(conditions[i])).CopyToDataTable();
                    }
                }
            }
            dt = dt.DefaultView.ToTable();
            return dt;
        }

        public DataTable GetLanguageDataTable(string[] conditions, bool[] isFiltred)
        {
            DataTable dt = CT.Tlumaczenia;
            string[] filtredColums = new string[] { "Typ", "Control", "lang" + conditions[0] };
            for (int i = 1; i < isFiltred.Length; i++)
            {
                if (isFiltred[i])
                    dt = dt.AsEnumerable().Where(myRow => myRow.Field<string>(filtredColums[i - 1]).Contains(conditions[i])).CopyToDataTable();
            }
            dt = dt.DefaultView.ToTable(dt.TableName, false, "Control", "Path", "Nr", "lang" + conditions[0]);
            return dt;
        }

        public String GetTransalatedLanguageList(String language, int nr)
        {
            String tt = "";
            tt = GetTraslatedText(language, "LANG", /*project, */ "BTPCLMenu.LeftMain", "comboBox1", nr);

            if (String.IsNullOrEmpty(tt))
                return "lang" + (nr + 1).ToString();

            return tt;
        }


        public String[] GetAlarmDescriptionFromDB(/*int source*/String alarmPath, int alarm, String lang)
        {
            String[] rv = new string[2];

            rv[0] = GetTraslatedText(lang, "Alarm", "Alarm", alarmPath, -1);
            rv[1] = GetTraslatedText(lang, "Alarm", "Alarm", alarmPath, alarm);

            if (String.IsNullOrEmpty(rv[0]))
                rv[0] = alarmPath;

            if (String.IsNullOrEmpty(rv[1]))
                rv[1] = "Error: " + alarm.ToString();

            return rv;
        }


        public String GetTraslatedText(String language, String type, /*String project,*/ String control, String path, int nr)
        {
            try
            {
                if (CT.Tlumaczenia.Rows.Contains(new object[] { type, nr, path, control }))
                {
                    return CT.Tlumaczenia.Rows.Find(new object[] { type, nr, path, control })[language].ToString();
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        private bool ReadTransalationFromDB()
        {
            try
            {
                if (_cfg.GetParamBool("LANG_IsGlobal"))
                    pta.Connection.ConnectionString = "Data Source=" + _cfg.DBWMSServerName_Remote + "; Initial Catalog=" + _cfg.DBWMSDbName_Remote + ";User ID=wms;Password=1";
                else
                    pta.Connection.ConnectionString = "Data Source=" + _cfg.DBWMSServerName + "; Initial Catalog=" + _cfg.DBWMSDbName + ";User ID=wms;Password=1";

                pta.Fill(CT.Tlumaczenia);

                TransalationsLoaded?.Invoke(this, new EventArgs());

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public void SetControlTransalation(string control, string path, int nr, string lang1, string lang2, string lang3, string lang4)
        {
            try
            {
                if (CT.Tlumaczenia.Rows.Contains(new object[] { "Control", nr, path, control }))
                {
                    var q = CT.Tlumaczenia.Where(r => r.Typ == "Control" && r.Nr == nr && r.Path == path && r.Control == control)
                        .Select(r => r).First();

                    q["lang1"] = lang1;
                    q["lang2"] = lang2;
                    q["lang3"] = lang3;
                    q["lang4"] = lang4;

                    pta.UpdateQuery(q.Islang1Null() ? "" : q.lang1,
                        q.Islang2Null() ? "" : q.lang2,
                        q.Islang3Null() ? "" : q.lang3,
                        q.Islang4Null() ? "" : q.lang4,
                        q.Typ,
                        q.Control,
                        q.Path,
                        q.Nr);
                }
                else
                {
                    pta.Insert("Control", control, path, nr, lang1, lang2, lang3, lang4);
                }

                pta.Fill(CT.Tlumaczenia);
            }
            catch { }
        }


        public TransAppTexts.TlumaczeniaDataTable GetTranslationDataTable()
        {
            return CT.Tlumaczenia;
        }


        public string GetTraslatedTextForDgvColumnHeader(String language, String control, String path, int nr)
        {
            try
            {
                return CT.Tlumaczenia.AsEnumerable()
                    .Where(r => r.Typ == "Control"
                        && r.Control == control
                        && r.Path == path
                        && r.Nr == nr)
                    .Select(r => r[language])
                    .First().ToString();
            }
            catch
            {
                return "";
            }
        }

        public String[] GetAllTraslatedText(String control, String path, int nr)
        {
            try
            {
                if (CT.Tlumaczenia.Rows.Contains(new object[] { "Control", nr, path, control }))
                {
                    var q = CT.Tlumaczenia.AsEnumerable().Where(r => r.Typ == "Control"
                            && r.Control == control
                            && r.Path == path
                            && r.Nr == nr)
                        .Select(r => r).First();

                    return new string[]
                    {
                        q.Islang1Null() ? "" : q.lang1,
                        q.Islang2Null() ? "" : q.lang2,
                        q.Islang3Null() ? "" : q.lang3,
                        q.Islang4Null() ? "" : q.lang4,
                    };
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }


    }
}
