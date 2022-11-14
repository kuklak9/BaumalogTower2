using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BTPLogs.LogQueue
{
    class LogQueue
    {
        public LogQueue(BTPDataBase.DB_WMS_3 DBWMS)
        {
            _dbWMS = DBWMS;
            _backgroundWorker = new System.ComponentModel.BackgroundWorker();
            
            _backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(_backgroundWorker_DoWork);
            _backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(_backgroundWorker_RunWorkerCompleted);

            _tmLog = new System.Timers.Timer();
            _tmLog.Interval = 1000;
            _tmLog.Elapsed += new System.Timers.ElapsedEventHandler(_tmLog_Elapsed);
            _tmLog.AutoReset = true;
            _tmLog.Enabled = true;


        }

        void _tmLog_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!_backgroundWorker.IsBusy && _logList.Count > 0)
            {
                //StartBWLog();
            }
        }

        void _backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            //DoNothing
        }

        void _backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

            System.ComponentModel.BackgroundWorker worker;
            worker = (System.ComponentModel.BackgroundWorker)sender;
            BTPDataBase.DB_WMS_3 dbWMS = e.Argument as BTPDataBase.DB_WMS_3;
            // LogQueueThread lqt = e.Argument as LogQueueThread;
            try
            {
                LogDescritpion ld = _logList.First().Copy();
                _logList.RemoveAt(0);
                dbWMS.Logs.Insert(ld.Type, ld.Description, ld.Username, ld.Device);
                //dbWMS.AddLog(ld.Type, ld.Description, ld.Username);
            }
            catch { }

        }

        private void StartBWLog()
        {
            // LogQueueThread lqt = new LogQueueThread(_dbWMS);
            if (!_backgroundWorker.IsBusy)
                _backgroundWorker.RunWorkerAsync(_dbWMS);
        }

        private System.Timers.Timer _tmLog;
        private System.ComponentModel.BackgroundWorker _backgroundWorker;
        private BTPDataBase.DB_WMS_3 _dbWMS;

        public void AddLog(int type, string desc, string username)
        {
            LogDescritpion ld = new LogDescritpion(type, desc, username, "-");
            _logList.Add(ld);
        }

        public void AddLog(int type, string desc, string username, string device)
        {
            LogDescritpion ld = new LogDescritpion(type, desc, username, device);
            _logList.Add(ld);
        }

        private List<LogDescritpion> _logList = new List<LogDescritpion>();





    }
}
