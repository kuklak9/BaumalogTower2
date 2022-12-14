using ArticlesControl.Formatki;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace ArticlesControl
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Localization_Selection_Inventory : MainWindow
    {
        #region Fields
        private readonly BlurEffect blur = new();
        public ResourceDictionary dict = new();
        public Dictionary<string, string> dictionary;

        private readonly Add_DataTableAdapters.StanyMagazynowe__Filter_SELECTTableAdapter stanyMagazynowe = new();
        private readonly Dictionary<string, string> custom_columns;
        private readonly Dictionary<string, Tuple<string, Type>> cells = new();
        #endregion

        //Constructor 
        public Localization_Selection_Inventory(ResourceDictionary resourceDictionary, Dictionary<string, string> lang, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns)
        {
            InitializeComponent();
            dg.Hide_Menu();
            this.cells = cells;
            this.custom_columns = custom_columns;
            dict = resourceDictionary;
            dictionary = lang;

            GenerateTraysWithThisArticleList();
        }

        private void GenerateTraysWithThisArticleList()
        {
            DataRow[] dr_array;
            dr_array = (DataRow[])stanyMagazynowe.GetData("000", "000").Select("Indeks = '" + cells["Indeks"].Item1.ToString() + "'");
            DataTable dt = new DataTable();
            List<string> list = new();
            foreach (DataColumn item in dt.Columns)
            {
                list.Add(item.ColumnName.ToString());
            }
            if (dr_array.Length > 0)
            {
                dt = dr_array.CopyToDataTable();
            }
            dg.Maindatatable = dt;
            dg.Load();
            dg.columnnames = list;

        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (dg.Selectedrow != null)
            {
                DataRowView drv = dg.Selectedrow as DataRowView;
                DataRow traycells = drv.Row;
                //if (background.IsThisTrayAvailableForOperator(Convert.ToInt32(traycells["ID_Polki"].ToString())))
                background.WRITE_TrayTransport(Convert.ToInt16(traycells["ID_Polki"].ToString()), Convert.ToInt16(background._window));
                //else
                //{
                //    HandyControl.Controls.MessageBox.Show("Brak dostępu operatora do wybranej półki", "Błąd dostępu", MessageBoxButton.OK, MessageBoxImage.Warning);
                //    return;
                //}

                Get_A_AnP get_Article = new Get_A_AnP(dict, dictionary, cells, custom_columns, cells["Indeks"].Item1.ToString(), traycells);
                get_Article.OpenInventoryWindowEvent += Get_Article_OpenInventoryWindowEvent;
                get_Article.Owner = Window.GetWindow(this);
                Opacity = 0.5;
                Effect = blur;
                get_Article.ShowDialog();
                Opacity = 1;
                Effect = null;
                this.Close();
            }
            else
                HandyControl.Controls.MessageBox.Show("Niepoprawny numer półki", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);

        }

        private void Get_Article_OpenInventoryWindowEvent(string Symbol)
        {
            DataRow dr = stanyMagazynowe.GetData("000", "000").Select(string.Format("Symbol = '{0}'", Symbol))[0];
            Inventory inventory = new(dict, dictionary, dr);
            inventory.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            inventory.ShowDialog();
            Opacity = 1;
            Effect = null;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
