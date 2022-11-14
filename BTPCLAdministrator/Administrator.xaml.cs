using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Effects;
using HandyControl.Tools;
using System.Data;
using System.IO.Ports;
using System.Xml;
using System.Windows.Media.Animation;
using System.Management;
using BTPTwinCatADS;
using System.Data.SqlClient;
using ArticlesControl;
using BTPDataBase;

namespace BTPCLAdministrator
{
    /// <summary>
    /// Interaction logic for Administrator.xaml
    /// </summary>
    public partial class Administrator : BTPControlLibrary.MainPanel
    {
        #region Fields
        public double Dvider_Opacity { get; set; } = 0.0;
        private readonly BlurEffect blur = new();
        private RFIDReader rfid;

        //Lists of keys and values for dictionaries
        public List<string> keys = new();
        public List<string> values = new();
        private List<object> menuitems = new();
        //ResourceDictionary for contextmenus, alerts, etc.
        public ResourceDictionary dict = new();
        public Dictionary<string, string> dictionary;

        //Filters from TabControl
        public static string? DeleteNote;
        private bool ShowEmpty = false;

        public static string ConnectionString = string.Empty;
        public static string Username = string.Empty;
        public static DataTable? UserRights;

        #endregion

        public Administrator()
        {
        }
        protected override void InternalInit()
        {
            #region Init functions
            DataContext = this;
            InitializeComponent();
            RunRFID();
            #endregion

            //Adding events handling buttons on datagrid control, and other initializing functions
            #region Events

            Tray_Datagrid.OnDefaulsData += DatagridInsertDataTableTray;

            Trade_Datagrid.OnDefaulsData += DatagridInsertDataTableTrade;

            Operator_Datagrid.OpenAddWindowEventOperator += OpenAddWindowOperator;
            Operator_Datagrid.OpenEditWindowEventOperator += OpenEditWindowOperator;
            Operator_Datagrid.OpenDeleteWindowEventOperator += OpenDeleteWindowOperator;
            Operator_Datagrid.OnDefaulsData += DatagridInsertDataTableOperator;

            #endregion

            #region Load data to Main and Additional DataTable
            BTPDataBase.Add_Data.Kar_Operator_SELECTDataTable? Data5 = _dbWMS.Add_Operator_Select.GetData();
            Operator_Datagrid.additionaldatatable = Data5;
            DatagridInsertDataTableOperator();
            BTPDataBase.Add_Data.Kar_Polki_HMI_SELECTDataTable? Data6 = _dbWMS.Add_Polki_HMI_Select.GetData(false);
            Tray_Datagrid.additionaldatatable = Data6;
            DatagridInsertDataTableTray();
            #endregion

            //Setting additional data table (necessary for controling headers name when users change it)
            #region Add DataGrids to menuitems for switching in menu
            menuitems.Add(Tray_Datagrid);
            menuitems.Add(Stats_Datagrid);
            menuitems.Add(Logs_Datagrid);
            menuitems.Add(Trade_Datagrid);
            menuitems.Add(Operator_Datagrid);
            #endregion

            //Setting default Language as PL
            #region Dictionary

            ResourceDictionary lang = new();
            lang = _translation.ReadTranslations();
            Resources.MergedDictionaries.Add(lang);
            keys = lang.Keys.Cast<string>().ToList();
            values = lang.Values.Cast<string>().ToList();
            dictionary = keys.Zip(values, (k, v) => new { Key = k, Value = v })
                     .ToDictionary(x => x.Key, x => x.Value);

            Tray_Datagrid.Language_Changed(lang);
            Operator_Datagrid.Language_Changed(lang);
            dict = lang;


            #endregion

        }

        #region Formats for Logs
        private void DatagridInsertDataTableLogs()
        {
            Logs_Datagrid.Hide_Menu();
            DataTable dt = _dbWMS.Logs.GetData();
            List<string> list1 = new();
            foreach (DataColumn item in dt.Columns)
            {
                list1.Add(item.ColumnName.ToString());
            }
            Logs_Datagrid.columnnames = list1;
            Logs_Datagrid.Maindatatable = dt;
            Logs_Datagrid.isTrade = true;
            Logs_Datagrid.Load();
            Logs_Datagrid.CallHeaders();
        }
        #endregion

        #region Formats for Tray
        ////Function for inserting data to datagrid control
        private void DatagridInsertDataTableTray()
        {
            Add_Data.Kar_Polki_HMI_SELECTDataTable? Data = _dbWMS.Add_Polki_HMI_Select.GetData(ShowEmpty);
            List<string> list1 = new();
            DataTable dt1 = Data;
            foreach (DataColumn item in dt1.Columns)
            {
                list1.Add(item.ColumnName.ToString());
            }
            Tray_Datagrid.columnnames = list1;
            Tray_Datagrid.Maindatatable = Data;
            Tray_Datagrid.isTray = true;
            Tray_Datagrid.Load();
            Tray_Datagrid.CallHeaders();
        }
        #endregion

        #region Formats for Trade
        private void DatagridInsertDataTableTrade()
        {
            Add_Data.ObrotyMagazynowe_SELECTDataTable? Data = _dbWMS.Add_ObrotyMagazynowe_Select.GetData();
            List<string> list1 = new();
            DataTable dt1 = Data;
            foreach (DataColumn item in dt1.Columns)
            {
                list1.Add(item.ColumnName.ToString());
            }
            Trade_Datagrid.columnnames = list1;
            Trade_Datagrid.Maindatatable = Data;
            Trade_Datagrid.isTrade = true;
            Trade_Datagrid.Load();
            Trade_Datagrid.CallHeaders();
        }
        #endregion

        #region Formats for Stats
        private void DatagridInsertDataTableStats()
        {
            Stats_Datagrid.Hide_Menu();
            DataTable dt = new DataTable();
            dt.Columns.Add("Nazwa");
            dt.Columns.Add("Wartość");
            DataRow dr = dt.NewRow();
            dr[0] = "test";
            dr[1] = "test";
            dt.Rows.Add(dr);
            List<string> list1 = new();
            foreach (DataColumn item in dt.Columns)
            {
                list1.Add(item.ColumnName.ToString());
            }
            Stats_Datagrid.columnnames = list1;
            Stats_Datagrid.Maindatatable = dt;
            Stats_Datagrid.isTrade = true;
            Stats_Datagrid.Load();
            Stats_Datagrid.CallHeaders();
        }
        #endregion

        #region Formats for Operator
        //Function for inserting data to datagrid control
        private void DatagridInsertDataTableOperator()
        {
            Add_Data.Kar_Operator_SELECTDataTable? Data = _dbWMS.Add_Operator_Select.GetData();
            List<string> list1 = new();
            DataTable dt1 = Data;
            foreach (DataColumn item in dt1.Columns)
            {
                list1.Add(item.ColumnName.ToString());
            }
            Operator_Datagrid.columnnames = list1;
            Operator_Datagrid.Maindatatable = Data;
            Operator_Datagrid.isOperator = true;
            Operator_Datagrid.Load();
            //Operator_Datagrid.CallHeaders();
        }
        //Function opening window for category adding
        public void OpenAddWindowOperator()
        {
            Opacity = 0.5;
            Effect = blur;
            Add_Operator add_Operator = new(dict, dictionary, rfid)
            {
                Owner = Window.GetWindow(this)
            };
            _ = add_Operator.ShowDialog();
            Opacity = 1;
            Effect = null;
            DatagridInsertDataTableOperator();
        }
        //Function opening window for category edditing
        private void OpenEditWindowOperator(Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns)
        {
            Opacity = 0.5;
            Effect = blur;
            Add_Operator add_Operator = new(dict, dictionary, cells, custom_columns, rfid)
            {
                Owner = Window.GetWindow(this)
            };
            _ = add_Operator.ShowDialog();
            Opacity = 1;
            Effect = null;
            DatagridInsertDataTableOperator();
        }
        //Function opening window for category deleting
        private void OpenDeleteWindowOperator(string? ID_Operator)
        {
            _ = _dbWMS.Add_Operator_Select.Delete(ID_Operator);
            Opacity = 0.5;
            Effect = blur;
            if (HandyControl.Controls.MessageBox.Show(dictionary["DeleteRowQuestion"], ID_Operator?.ToString(), MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _ = _dbWMS.Add_Operator_Select.Delete(ID_Operator);
                DatagridInsertDataTableOperator();
                Operator_Datagrid.Refreash();
            }
            Opacity = 1;
            Effect = null;
        }
        #endregion

        #region Other
        private void TopMenuButton_Click(object sender, MouseButtonEventArgs e)
        {
            TabItem tabitem = sender as TabItem;
            ChangeView(Convert.ToInt32(tabitem.Name.ToString().Substring(tabitem.Name.ToString().Length - 1)));
        }
        public void ChangeView(int tabNumber)
        {
            switch (tabNumber)
            {
                //pozmieniaj te isTrade na normalne jak beda juz w bazie

                case 1:
                    if ((bool)_loggedUser.OperatorRights["isTrade"].GetValue(3))
                    {
                        DatagridInsertDataTableLogs();
                        SetInfo(Logs_Datagrid);
                    }
                    else
                        HandyControl.Controls.MessageBox.Show(dictionary["AccessDenied"], "Access Denied", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 2:
                    if ((bool)_loggedUser.OperatorRights["isTrade"].GetValue(3))
                    {
                        DatagridInsertDataTableTrade();
                        SetInfo(Trade_Datagrid);
                    }
                    else
                        HandyControl.Controls.MessageBox.Show(dictionary["AccessDenied"], "Access Denied", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 3:
                    if ((bool)_loggedUser.OperatorRights["isTrade"].GetValue(3))
                    {
                        DatagridInsertDataTableStats();
                        SetInfo(Stats_Datagrid);
                    }
                    else
                        HandyControl.Controls.MessageBox.Show(dictionary["AccessDenied"], "Access Denied", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 4:
                    if ((bool)_loggedUser.OperatorRights["isOperator"].GetValue(3))
                    {
                        DatagridInsertDataTableOperator();
                        SetInfo(Operator_Datagrid);
                    }
                    else
                        HandyControl.Controls.MessageBox.Show(dictionary["AccessDenied"], "Access Denied", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 5:
                    if ((bool)_loggedUser.OperatorRights["isTray"].GetValue(3))
                    {
                        DatagridInsertDataTableTray();
                        SetInfo(Tray_Datagrid);
                    }
                    else
                        HandyControl.Controls.MessageBox.Show(dictionary["AccessDenied"], "Access Denied", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                default:
                    break;
            }
        }
        private void SetInfo(DataGridControl.UserControl1 dg)
        {

            dg.CallHeaders();
            dg.ReadSettings();
            dg.SetButton();
            dg.UpdateAvaiableColumnsForFiltering();
            SetVisibility(dg);
        }
        private void SetVisibility(object selected)
        {
            foreach (object item in menuitems)
            {
                if (selected == item)
                {
                    var y = (DataGridControl.UserControl1)item;
                    y.Visibility = Visibility.Visible;
                }
                else
                {
                    var y = (DataGridControl.UserControl1)item;
                    y.Visibility = Visibility.Hidden;
                }
            }
        }
        #endregion
        private void RunRFID()
        {
            try
            {
                rfid = new RFIDReader("COM8", 19200, 8, Parity.None, StopBits.Two, Handshake.None, true);
            }
            catch { }
        }
    }
}
