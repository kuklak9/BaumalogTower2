using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;


namespace BTPCLAdministrator
{
    public sealed class RFIDReader
    {
        private static SerialPort _rfid = new SerialPort();
        public string DataRFID { get; set; }

        private string _portcom;
        private int _baudrate;
        private int _databits;
        private Parity _parity;
        private StopBits _stopbits;
        private Handshake _handshake;
        private bool _dtrenable;

        #region event
        public delegate void rfidWasReaded(string? ID_Card_Number);
        public event rfidWasReaded? RFIDWasReaded;
        #endregion

        public RFIDReader(string portcom, int baudrate, int databits, Parity parity, StopBits stopbits, Handshake handshake, bool dtrenable)
        {
            if (_rfid.PortName != portcom)
            {
                _portcom = portcom;
                _baudrate = baudrate;
                _databits = databits;
                _parity = parity;
                _stopbits = stopbits;
                _handshake = handshake;
                _dtrenable = dtrenable;

                CreateRFIDReader();
            }
            else
            {
                _rfid.Dispose();
            }
        }

        private void CreateRFIDReader()
        {
            try
            {
                _rfid.PortName = _portcom;
                _rfid.BaudRate = _baudrate;
                _rfid.DataBits = _databits;
                _rfid.Parity = _parity;
                _rfid.StopBits = _stopbits;
                _rfid.Handshake = _handshake;
                _rfid.DtrEnable = _dtrenable;
                _rfid.Open();

                _rfid.DataReceived += RFID_DataReceived;
                _rfid.Disposed += RFID_Disposed;
            }
            catch { }
        }

        private void RFID_Disposed(object? sender, EventArgs e)
        {
            CreateRFIDReader();
        }

        public void RFID_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                DataRFID = "";
                DataRFID = _rfid.ReadExisting();
                DataRFID = DataRFID.Replace("\r", "");
                if (RFIDWasReaded is not null && DataRFID != "")
                    RFIDWasReaded?.Invoke(DataRFID);
            }
            catch { }
        }
    }
}

