using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPLogs.LogQueue
{
    class LogQueueThread
    {
        //public LogQueueThread(BTPDataBase.DB_WMS DBWMS)
        public LogQueueThread(BTPDataBase.DB_WMS_3 DBWMS)
        {
            _dbWMS = DBWMS;
        }
        //private BTPDataBase.DB_WMS _dbWMS;
        private BTPDataBase.DB_WMS_3 _dbWMS;

        public void BW_Log(System.ComponentModel.BackgroundWorker worker, System.ComponentModel.DoWorkEventArgs e, LogDescritpion LD)
        {
            try
            {
                _dbWMS.Logs.Insert(LD.Type, LD.Description, LD.Username, "-");
                //_dbWMS.AddLog(LD.Type, LD.Description, LD.Username);

                e.Result = true;
            }
            catch
            {
                e.Result = false;
            }

        }
    }
}
