using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace ArticlesControl.Klasy
{
    internal class Tray
    {
        #region Fields

        public int? ID_Polki { get; set; }
        public int? ID_Regalu { get; set; }
        public int? NrPolki { get; set; }
        public int? ID_Typu { get; set; }
        public int? ID_Status { get; set; }
        public int? ID_Kategorii { get; set; }
        public int? ID_Przeznaczenia { get; set; }

        #endregion

        #region Constructors

        public Tray(int? ID_Polki, int? ID_Typu, int? ID_Status, int? ID_Kategorii, int? ID_Przeznaczenia)
        {
            this.ID_Polki = ID_Polki;
            this.ID_Typu = ID_Typu;
            this.ID_Status = ID_Status;
            this.ID_Kategorii = ID_Kategorii;
            this.ID_Przeznaczenia = ID_Przeznaczenia;
        }

        public Tray(int? ID_Polki)
        {
            this.ID_Polki = ID_Polki;
        }

        public Tray(int? ID_Polki, int? ID_Status)
        {
            this.ID_Polki = ID_Polki;
            this.ID_Status = ID_Status;
        }
        public Tray(int? ID_Regalu, int? NrPolki, int? ID_Typu, int? ID_Status, int? ID_Kategorii, int? ID_Przeznaczenia)
        {
            this.ID_Regalu = ID_Regalu;
            this.NrPolki = NrPolki;
            this.ID_Typu = ID_Typu;
            this.ID_Status = ID_Status;
            this.ID_Kategorii = ID_Kategorii;
            this.ID_Przeznaczenia = ID_Przeznaczenia;
        }

        #endregion

        #region Functions

        //public void Insert()
        //{
        //    try
        //    {
        //        Add_Tray.Insert(ID_Regalu, ID_Typu, ID_Przeznaczenia, NrPolki, ID_Status, ID_Kategorii);
        //    }
        //    catch (Exception e)
        //    {
        //        if (HandyControl.Controls.MessageBox.Show(e.Message, e.Message, MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK)
        //        {

        //        }
        //    }
        //}

        //public void Delete()
        //{
        //    try
        //    {
        //        Add_Tray.Delete(ID_Polki);
        //    }
        //    catch (Exception e)
        //    {
        //        if (HandyControl.Controls.MessageBox.Show(e.Message, e.Message, MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK)
        //        {

        //        }
        //    }
        //}

        //public void Update()
        //{
        //    try
        //    {
        //        Add_Tray.Update(ID_Polki, ID_Typu, ID_Status, ID_Kategorii, ID_Przeznaczenia);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        public void SetInventory()
        {
            string connectionString = ArticlesControl.Articles.ConnectionString;
            SqlConnection con = new(connectionString);
            SqlCommand cmd = new(string.Format("UPDATE [SmartWMS].[dbo].[Kar_Polki] SET ID_Status = {0} WHERE ID_Polki = {1}", ID_Status, ID_Polki), con);
            con.Open();
            SqlDataAdapter adapter = new(cmd);
            DataTable dt = new();
            adapter.Fill(dt);
            cmd.Dispose();
            con.Close();
        }

        #endregion

    }
}
