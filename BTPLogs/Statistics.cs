using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace BTPLogs
{
    public class Statistics
    {

        public Dictionary<int, BTPUtilities.Tray> TrayConfig = null;
        private System.ComponentModel.BackgroundWorker _backgroundWorker = null;

        BTPDataBase.DB_WMS_3 _db = null;
        BTPConfig.Configuration _cfg = null;
        System.Timers.Timer _tm = null;

        public Statistics(BTPDataBase.DB_WMS_3 db, BTPConfig.Configuration cfg)
        {
            _db = db;

            _backgroundWorker = new System.ComponentModel.BackgroundWorker();

            _backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(_backgroundWorker_DoWork);

            _tm = new System.Timers.Timer();
            _tm.Interval = 100;
            _tm.Elapsed += new System.Timers.ElapsedEventHandler(_tm_Elapsed);
            _tm.AutoReset = true;
            _tm.Enabled = true;
        }

        List<String[]> _statList = new List<String[]>();

        void _tm_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!_backgroundWorker.IsBusy && _statList.Count > 0)
            {
                _backgroundWorker.RunWorkerAsync(_statList);
            }
        }

        void _backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                List<String[]> pomList = (List<String[]>)e.Argument;
                if (pomList.Count > 0)
                {
                    bool added = false;
                    switch (pomList[0][0])
                    {
                        case "t":
                            _db.AddTrayUse(Convert.ToInt32(pomList[0][1]));
                            added = true;
                            break;
                        case "a":
                            _db.AddArticleUse(pomList[0][1]);
                            added = true;
                            break;
                    }
                    if (added)
                        pomList.RemoveAt(0);
                }
            }
            catch { }
        }

        public void ClearArticleUse()
        { 
            _db.ClearArticleUse();
        }

        public void ClearTrayUse()
        {
            _db.ClearTrayUse();
        }

        public void TrayUsed(int trayNr)
        {
            _statList.Add(new String[] { "t", trayNr.ToString() });
        }

        public void ArticleUse(String art)
        {
            _statList.Add(new String[] { "a", art });
        }

        public DataTable GetTrayUse()
        {
            try
            {
                DataTable trays = _db.GetTrayUse();

                DataTable rv = new DataTable();
                foreach (DataColumn dc in trays.Columns)
                {
                    rv.Columns.Add(dc.ColumnName);
                }

                foreach (KeyValuePair<int, BTPUtilities.Tray> kvp in TrayConfig.OrderBy(k => k.Value.ID))
                {
                    bool added = false;
                    foreach (DataRow dr in trays.Rows)
                    {
                        if (Convert.ToInt32(dr[0]) == kvp.Value.ID)
                        {
                            rv.Rows.Add(dr[0], dr[1], dr[2]);
                            added = true;
                            break;
                        }
                    }
                    if (!added)
                        rv.Rows.Add(kvp.Value.ID, 0, "C");
                }

                return rv;

            }
            catch
            {
                return null;
            }



        }

        public String GetArticleGroup(String art)
        {
            return _db.GetGroupForArticle(art);
        }

        public DataTable GetArticleUse()
        {
            return _db.GetArticleUse();
        }



    }
}
