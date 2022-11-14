using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace ArticlesControl
{
    internal class Stock 
    {
        #region Fields
        //private readonly Add_DataTableAdapters.StanyMagazynowe_EDITTableAdapter stanyMagazynowe = new();

        public delegate void isOK();
        public event isOK IsOkEvent;
        public bool isOk = false;
        private readonly DataTable Lokalizacje = Connection("Kar_Lokalizacje");
        private BTPDataBase.DB_WMS_3 DataBase;
        public string? Indeks { get; set; }
        public string? ID_Lokalizacji { get; set; }
        public int? ID_LokalizacjiInt { get; set; }
        public int? ID_Polki { get; set; }
        public decimal? Ilosc { get; set; }
        public decimal? Rezerwacja { get; set; }
        public string? Partia { get; set; }
        public string? Atest { get; set; }
        public string? Wytop { get; set; }
        public string? Atr1 { get; set; }
        public string? Atr2 { get; set; }
        public string? Atr3 { get; set; }
        public DateTime? Data { get; set; }
        public string? Notatka { get; set; }
        public string? Operator { get; set; }
        #endregion
        private static DataTable Connection(string db)
        {
            string connectionString = ArticlesControl.Articles.ConnectionString;
            SqlConnection con = new(connectionString);
            //SQL command
            SqlCommand cmd = new(string.Format("select * from dbo.{0};", db), con);
            con.Open();
            SqlDataAdapter adapter = new(cmd);
            DataTable dt = new();
            adapter.Fill(dt);
            cmd.Dispose();
            con.Close();
            return dt;
        }
        private void Stock_IsOkEvent()
        {
            isOk = true;
        }
        public Stock(BTPDataBase.DB_WMS_3 DataBase,string Indeks, string ID_Lokalizacji, int ID_Polki, decimal Ilosc, decimal Rezerwacja, string Partia, string Atest, string Wytop, string Atr1, string Atr2, string Atr3, DateTime Data, string Notatka, string Operator)
        {
            IsOkEvent += Stock_IsOkEvent;
            this.DataBase = DataBase;
            this.Indeks = Indeks;
            this.ID_Lokalizacji = ID_Lokalizacji;
            this.ID_Polki = ID_Polki;
            this.Ilosc = Ilosc;
            this.Rezerwacja = Rezerwacja;
            this.Partia = Partia;
            this.Atest = Atest;
            this.Wytop = Wytop;
            this.Atr1 = Atr1;
            this.Atr2 = Atr2;
            this.Atr3 = Atr3;
            this.Data = Data;
            this.Notatka = Notatka;
            this.Operator = Operator;
        }
        public Stock(BTPDataBase.DB_WMS_3 DataBase,string Indeks, int ID_LokalizacjiInt, int ID_Polki, decimal Ilosc, decimal Rezerwacja, string Partia, string Atest, string Wytop, string Atr1, string Atr2, string Atr3, DateTime Data, string Notatka, string Operator)
        {
            IsOkEvent += Stock_IsOkEvent;
            this.DataBase = DataBase;
            this.Indeks = Indeks;
            this.ID_LokalizacjiInt = ID_LokalizacjiInt;
            this.ID_Polki = ID_Polki;
            this.Ilosc = Ilosc;
            this.Rezerwacja = Rezerwacja;
            this.Partia = Partia;
            this.Atest = Atest;
            this.Wytop = Wytop;
            this.Atr1 = Atr1;
            this.Atr2 = Atr2;
            this.Atr3 = Atr3;
            this.Data = Data;
            this.Notatka = Notatka;
            this.Operator = Operator;
        }
        public Stock(BTPDataBase.DB_WMS_3 DataBase)
        {
            IsOkEvent += Stock_IsOkEvent;
            this.DataBase = DataBase;
        }
        public Stock(BTPDataBase.DB_WMS_3 DataBase, string Indeks, string ID_Lokalizacji, int ID_Polki, string Partia, string Atest, string Wytop, string Notatka, string Operator)
        {
            IsOkEvent += Stock_IsOkEvent;
            this.DataBase = DataBase;
            this.Indeks = Indeks;
            this.ID_Lokalizacji = ID_Lokalizacji;
            this.ID_Polki = ID_Polki;
            this.Partia = Partia;
            this.Atest = Atest;
            this.Wytop = Wytop;
            this.Notatka = Notatka;
            this.Operator = Operator;
        }

        public void Insert()
        {
            int? temp;
            if (ID_LokalizacjiInt == null)
            {
                temp = Convert.ToInt32(Lokalizacje.Select(string.Format("Symbol = '{0}' AND ID_Polka = {1}", ID_Lokalizacji, ID_Polki))[0][0]);
            }
            else
            {
                temp = ID_LokalizacjiInt;
            }
            if (Indeks == null)
            {
                Indeks = "";
            }

            if (Partia == null)
            {
                Partia = "";
            }

            if (Atest == null)
            {
                Atest = "";
            }

            if (Wytop == null)
            {
                Wytop = "";
            }

            try
            {
                DataBase.Edit_StanyMagazynowe_ta.Insert(Indeks, temp, ID_Polki, Ilosc, Rezerwacja, Partia, Atest, Wytop, Atr1, Atr2, Atr3, Data, Notatka, Operator);
            }
            catch (Exception e)
            {
                if (HandyControl.Controls.MessageBox.Show(e.Message, Indeks, MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    IsOkEvent();
                }
            }
        }

        public void Delete()
        {
            int temp = Convert.ToInt32(Lokalizacje.Select(string.Format("Symbol = '{0}' AND ID_Polka = {1}", ID_Lokalizacji, ID_Polki))[0][0]);
            try
            {
                DataBase.Edit_StanyMagazynowe_ta.Delete(Indeks, temp, ID_Polki, Partia, Atest, Wytop, Notatka, Operator);
            }
            catch (Exception e)
            {
                if (HandyControl.Controls.MessageBox.Show(e.Message, Indeks, MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    IsOkEvent();
                }
            }
        }

        public void Update()
        {
            int? temp;
            if (ID_LokalizacjiInt == null)
            {
                temp = Convert.ToInt32(Lokalizacje.Select(string.Format("Symbol = '{0}' AND ID_Polka = {1}", ID_Lokalizacji, ID_Polki))[0][0]);
            }
            else
            {
                temp = ID_LokalizacjiInt;
            }
            if (Indeks == null)
            {
                Indeks = "";
            }

            if (Partia == null)
            {
                Partia = "";
            }

            if (Atest == null)
            {
                Atest = "";
            }

            if (Wytop == null)
            {
                Wytop = "";
            }

            try
            {
                DataBase.Edit_StanyMagazynowe_ta.Update(Indeks, temp, ID_Polki, Ilosc, Rezerwacja, Partia, Atest, Wytop, Atr1, Atr2, Atr3, Data, Notatka, Operator);
            }
            catch (Exception e)
            {
                if (HandyControl.Controls.MessageBox.Show(e.Message, Indeks, MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    IsOkEvent();
                }
            }
        }
    }
}
