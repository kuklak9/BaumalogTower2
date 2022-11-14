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
using ArticlesControl.Formatki;
using System.Windows.Media.Effects;
using HandyControl.Tools;
using System.Data;
using ArticlesControl;
using System.IO.Ports;
using System.Xml;
using System.Windows.Media.Animation;
using System.Management;
using BTPTwinCatADS;
using BTPDataBase;

namespace ArticlesControl
{
    /// <summary>
    /// Interaction logic for Articles.xaml
    /// </summary>
    public partial class Articles : BTPControlLibrary.MainPanel
    {
        #region Fields
        public double Dvider_Opacity { get; set; } = 0.0;
        private readonly BlurEffect blur = new();

        //Lists of keys and values for dictionaries
        public List<string> keys = new();
        public List<string> values = new();
        private List<object> menuitems = new();
        //ResourceDictionary for contextmenus, alerts, etc.
        public ResourceDictionary dict = new();
        public Dictionary<string, string> dictionary;

        //Filters from TabControl
        private string? Gatunek_Filter;
        private string? Kategoria_Filter;
        private string? Order_Filter;
        public static string? DeleteNote;
        private bool ShowEmpty = false;
        private DataTable dtOrderLines;
        private int orderLineIndex;
        private bool IsSelectedCategoryItemColored = false;

        public static string ConnectionString = string.Empty;
        public static string Username = string.Empty;
        public static DataTable? UserRights;

        private int OrderType = 0;
        private int OrderStatus = 0;
        #endregion

        public Articles() { }
        protected override void InternalInit()
        {
            #region Init functions

            DataGridColors();
            DataContext = this;
            InitializeComponent();
            ButtonsColors();
            DataGridColors();
            MainAppColors();
            //AddColorstoDictionary();
            AdditionalWindowsColors();
            OrderLine_Datagrid.Hide_Menu();
            Order_Datagrid.Show_Menu();
            #endregion

            //Adding events handling buttons on datagrid control, and other initializing functions
            #region Events
            Articles_Datagrid.OpenAddWindowEvent += OpenAddWindow;
            Articles_Datagrid.OpenSuggestedAddWindowEvent += OpenSuggestedAddWindow;
            Articles_Datagrid.OpenEditWindowEvent += OpenEditWindow;
            Articles_Datagrid.OpenDeleteWindowEvent += OpenDeleteWindow;
            Articles_Datagrid.OnDefaulsData += DatagridInsertDataTableArticle;
            Articles_Datagrid.OnDefaulsData += Refresh_Trees;
            Articles_Datagrid.OpenTraysGetWindowEvent += OpenTraysGetWindow;
            Articles_Datagrid.OpenTraysGiveWindowEvent += OpenTraysGiveWindow;

            Stock_Datagrid.OnDefaulsData += DatagridInsertDataTableStock;
            Stock_Datagrid.OnDefaulsData += Refresh_Trees;
            Stock_Datagrid.OpenGetWindowEventStock += OpenGetWindowStock;
            Stock_Datagrid.OpenGiveWindowEventStock += OpenGiveWindowStock;
            Stock_Datagrid.OpenInventoryWindowEventStock += OpenInventoryWindowStock;
            Stock_Datagrid.OpenDeleteWindowEventStock += OpenDeleteWindowStock;

            Type_Datagrid.OpenAddWindowEventType += OpenAddWindowType;
            Type_Datagrid.OpenSuggestedAddWindowEventType += OpenSuggestedAddWindowType;
            Type_Datagrid.OpenEditWindowEventType += OpenEditWindowType;
            Type_Datagrid.OpenDeleteWindowEventType += OpenDeleteWindowType;
            Type_Datagrid.OnDefaulsData += DatagridInsertDataTableType;
            Type_Datagrid.OnDefaulsData += Refresh_Trees;

            Order_Datagrid.OnDefaulsData += DatagridInsertDataTableOrder;
            Order_Datagrid.SelectedRowEvent += Order_Datagrid_SelectedRowEvent;
            Order_Datagrid.RunOrdersManualEvent += Order_Datagrid_RunOrdersManualEvent;
            Order_Datagrid.RunOrdersAutomaticEvent += Order_Datagrid_RunOrdersAutomaticEvent;

            OrderLine_Datagrid.SelectedIndexEvent += OrderLine_Datagrid_SelectedIndexEvent;

            Tray_Datagrid.OnDefaulsData += DatagridInsertDataTableTray;
            Tray_Datagrid.ShowEmptyTraysEvent += Tray_Datagrid_ShowEmptyTraysEvent;
            Tray_Datagrid.ShowArticlesOnThisTrayEvent += Tray_Datagrid_ShowArticlesOnThisTrayEvent;
            Tray_Datagrid.OpenInventoryWindowEventTray += Tray_Datagrid_OpenInventoryWindowEventTray;

            #endregion
            #region Load data to Main and Additional DataTable
            Add_Data.Kar_Artykuly_SELECTDataTable? Data = _dbWMS.Add_Kar_Artykul_ta.GetData();
            Articles_Datagrid.additionaldatatable = Data;
            DatagridInsertDataTableArticle();
            Add_Data.StanyMagazynowe_SELECTDataTable? Data1 = _dbWMS.Add_StanyMagazynowe_ta.GetData();
            Stock_Datagrid.additionaldatatable = Data1;
            DatagridInsertDataTableStock();
            Add_Data.Kar_Gatunki_SELECTDataTable? Data2 = _dbWMS.Add_Gatunki_ta.GetData();
            Type_Datagrid.additionaldatatable = Data2;
            DatagridInsertDataTableType();
            Add_Data.Zlecenia_SELECTDataTable? Data4 = _dbWMS.Add_Zlecenia_Select.GetData(0, 0);
            Order_Datagrid.additionaldatatable = Data4;
            DatagridInsertDataTableOrder();

            Add_Data.Kar_Polki_HMI_SELECTDataTable? Data6 = _dbWMS.Add_Polki_HMI_Select.GetData(false);
            Tray_Datagrid.additionaldatatable = Data6;
            DatagridInsertDataTableTray();
            #endregion

            //Setting additional data table (necessary for controling headers name when users change it)
            #region Add DataGrids to menuitems for switching in menu
            menuitems.Add(Articles_Datagrid);
            menuitems.Add(Stock_Datagrid);
            menuitems.Add(Type_Datagrid);
            menuitems.Add(Order_Grid);
            menuitems.Add(Tray_Datagrid);

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

            Articles_Datagrid.Language_Changed(lang);
            Stock_Datagrid.Language_Changed(lang);
            Type_Datagrid.Language_Changed(lang);
            Tray_Datagrid.Language_Changed(lang);
            Order_Datagrid.Language_Changed(lang);
            OrderLine_Datagrid.Language_Changed(lang);
            dict = lang;


            #endregion

        }
        #region ColorSzary

        private void DataGridColors()
        {
            //additionalDictionary.Source = new Uri("/ArticlesControl;component/StringResources.pl-PL.xaml", UriKind.RelativeOrAbsolute);
            //Resources.MergedDictionaries.Add(additionalDictionary);
            //Data Grid Background
            Application.Current.Resources["datagrid_gridbackground"] = (SolidColorBrush?)new BrushConverter().ConvertFrom("#BEBCBA");
            Application.Current.Resources["datagrid_gridbackground"] = (SolidColorBrush?)new BrushConverter().ConvertFrom("#BEBCBA");
            //Data Grid Scrollbar 
            Application.Current.Resources["datagrid_ScrollBarColor"] = (SolidColorBrush?)new BrushConverter().ConvertFrom("#6b6b6b");
            //Data Grid Cells
            Application.Current.Resources["datagrid_CellIsSelectedBackgroundColor"] = Brushes.DeepSkyBlue;
            Application.Current.Resources["datagrid_CellIsSelectedForegroundColor"] = Brushes.Black;
            Application.Current.Resources["datagrid_IsMouseOverCellColor"] = (SolidColorBrush?)new BrushConverter().ConvertFrom("#ff9215");
            Application.Current.Resources["datagrid_Alternatingrow"] = (SolidColorBrush?)new BrushConverter().ConvertFrom("#EDDAD1");
            Application.Current.Resources["datagrid_RowForeground"] = Brushes.Black;
            //Data Grid Header "#397D81" //F09070 //#62605E
            Application.Current.Resources["datagrid_headers"] = (SolidColorBrush?)new BrushConverter().ConvertFrom("#62605E");
            Application.Current.Resources["datagrid_headersfilter"] = Brushes.YellowGreen;
            Application.Current.Resources["datagrid_IsMouseOverHeaderColor"] = Brushes.OrangeRed;
            Application.Current.Resources["datagrid_sortingarrow"] = Brushes.Black;
            Application.Current.Resources["datagrid_headersforeground"] = Brushes.White;
            //Data Grid Scrollbar 
            Application.Current.Resources["datagrid_ScrollBarColor"] = (SolidColorBrush?)new BrushConverter().ConvertFrom("#ffffff");
            //Data Grid Combobox for hiding columns
            Application.Current.Resources["datagrid_combobox"] = (SolidColorBrush?)new BrushConverter().ConvertFrom("#000000");
            Application.Current.Resources["datagrid_togglebuttoncolumnvisible"] = Application.Current.Resources["datagrid_headers"];
            Application.Current.Resources["datagrid_togglebuttoncolumnnotvisible"] = (SolidColorBrush?)new BrushConverter().ConvertFrom("#EEEEEE");
        }
        //Function setting colors to main app elements, and background using linear gradiend
        private void MainAppColors()
        {
            //#BEBCBA
            LinearGradientBrush background = new()
            {
                StartPoint = new System.Windows.Point(0.5, 0),
                EndPoint = new System.Windows.Point(0.5, 1)
            };
            background.GradientStops.Add(new GradientStop((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#EEEEEE"), 0.1));
            background.GradientStops.Add(new GradientStop((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#C4C4C4"), 0.5));
            background.GradientStops.Add(new GradientStop((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#BEBCBA"), 0.9));
            //Menu_Background.Fill = (SolidColorBrush?)new BrushConverter().ConvertFrom("#EEEEEE");
            //Main_Background.Fill = (SolidColorBrush?)new BrushConverter().ConvertFrom("#BEBCBA");
            //Bottom_Background.Fill = (SolidColorBrush?)new BrushConverter().ConvertFrom("#BEBCBA");
            //BaumalogApp.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom("#C28B67");
            Application.Current.Resources["App_Background"] = (SolidColorBrush?)new BrushConverter().ConvertFrom("#BEBCBA");
            Application.Current.Resources["App_MenuItemMouseOver"] = (SolidColorBrush?)new BrushConverter().ConvertFrom("#EEEEEE");
            //Language_selectbox.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom("#BEBAB6");
        }
        //Function setting colors to additional windows
        private void AdditionalWindowsColors()
        {
            Application.Current.Resources["Filter_input_background"] = (SolidColorBrush?)new BrushConverter().ConvertFrom("#F09070");
        }
        //Function setting colors to buttons
        private void ButtonsColors()
        {
            Application.Current.Resources["Submit_button"] = Brushes.GreenYellow;
            Application.Current.Resources["Cancel_button"] = Brushes.Red;
            Application.Current.Resources["Buttons_Controls_Color1"] = (SolidColorBrush?)new BrushConverter().ConvertFrom("#EEEEEE");

            Application.Current.Resources["Buttons_Controls_Color2"] = (SolidColorBrush?)new BrushConverter().ConvertFrom("#f88562");
            Application.Current.Resources["App_MenuItemBackground"] = (SolidColorBrush?)new BrushConverter().ConvertFrom("#EEEEEE");
        }
        #endregion

        #region Formats for Articles
        //Function for inserting data to datagrid control
        private void DatagridInsertDataTableArticle()
        {
            //var Data = Add_Kar_Artykul_ta.GetData();
            Kategoria_Filter = string.IsNullOrEmpty(Kategoria_Filter) ? "000" : Kategoria_Filter;
            Gatunek_Filter = string.IsNullOrEmpty(Gatunek_Filter) ? "000" : Gatunek_Filter;
            Add_Data.Kar_Artykuly_SELECTDataTable? Data = _dbWMS.Add_Kar_Artykul_ta.GetDataBy(Kategoria_Filter, Gatunek_Filter);
            List<string> list = new();
            DataTable dt = Data;
            foreach (DataColumn item in dt.Columns)
            {
                list.Add(item.ColumnName.ToString());
            }
            Articles_Datagrid.columnnames = list;
            Articles_Datagrid.Maindatatable = Data;
            Articles_Datagrid.isArticle = true;
            Articles_Datagrid.Load();
            //Articles_Datagrid.CallHeaders();
        }
        //Function opening window for article adding
        public void OpenAddWindow(int count)
        {
            Add_Article add_Article = new(dict, dictionary, count);
            add_Article.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            add_Article.ShowDialog();
            Effect = null;
            Opacity = 1;
            DatagridInsertDataTableArticle();
            Articles_Datagrid.Refreash();

        }
        //Function opening window for article editing
        private void OpenEditWindow(Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns)
        {
            Add_Article add_Article = new(dict, dictionary, cells, custom_columns);
            add_Article.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            add_Article.ShowDialog();
            Opacity = 1;
            Effect = null;
            DatagridInsertDataTableArticle();
            Articles_Datagrid.Refreash();
        }
        //Function opening window for article deleting
        private void OpenDeleteWindow(string Indeks)
        {
            Opacity = 0.5;
            Effect = blur;
            if (HandyControl.Controls.MessageBox.Show(dictionary["DeleteRowQuestion"], Indeks, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Article article = new(_dbWMS, Indeks);
                article.Delete();
                DatagridInsertDataTableArticle();
            }
            else
            {
            }
            Opacity = 1;
            Effect = null;
        }
        //Function opening window for article adding, when row is selected
        public void OpenSuggestedAddWindow(int count, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns)
        {
            Add_Article add_Article = new(dict, dictionary, cells, true, custom_columns);
            add_Article.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            add_Article.ShowDialog();
            Opacity = 1;
            Effect = null;
            DatagridInsertDataTableArticle();
            Articles_Datagrid.Refreash();
        }
        //Function opening window for article getting
        public void OpenTraysGetWindow(int count, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns)
        {

            DataRow[] dr_array = _dbWMS.Add_StanyMagazynowe_Filter.GetData("000", "000").Select(string.Format("Indeks = '{0}'", cells["Indeks"].Item1.ToString()));
            if (dr_array.Length > 1)
            {
                Tray_Selection_Get tray_Selection = new(dict, dictionary, cells, custom_columns);
                tray_Selection.Owner = Window.GetWindow(this);
                Opacity = 0.5;
                Effect = blur;
                tray_Selection.ShowDialog();
                Opacity = 1;
                Effect = null;
            }
            else if (dr_array.Length == 1)
            {
                //TODO:Transport polki
                Get_A_AnP get_Article = new Get_A_AnP(dict, dictionary, cells, custom_columns, cells["Indeks"].Item1.ToString(), dr_array[0]);
                get_Article.OpenInventoryWindowEvent += Get_Article_OpenInventoryWindowEvent;
                get_Article.Owner = Window.GetWindow(this);
                Opacity = 0.5;
                Effect = blur;
                get_Article.ShowDialog();
                Opacity = 1;
                Effect = null;
            }
            else
            {
                HandyControl.Controls.MessageBox.Show("Wskazany artykuł nie znajduje się w magazynie.", "Błąd pobrania", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            DatagridInsertDataTableArticle();
            Articles_Datagrid.Refreash();
        }
        //Function opening window for article giving
        public void OpenTraysGiveWindow(int count, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns)
        {
            Tray_Selection_Give tray_Selection = new(dict, dictionary, cells, custom_columns);
            tray_Selection.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            tray_Selection.ShowDialog();
            Opacity = 1;
            Effect = null;
            DatagridInsertDataTableArticle();
            Articles_Datagrid.Refreash();
        }
        #endregion

        #region Formats for Stock
        //Function for inserting data to datagrid control
        private void DatagridInsertDataTableStock()
        {
            Kategoria_Filter = string.IsNullOrEmpty(Kategoria_Filter) ? "000" : Kategoria_Filter;
            Gatunek_Filter = string.IsNullOrEmpty(Gatunek_Filter) ? "000" : Gatunek_Filter;
            Add_Data.StanyMagazynowe__Filter_SELECTDataTable? Data = _dbWMS.Add_StanyMagazynowe_Filter.GetData(Kategoria_Filter, Gatunek_Filter);
            //Add_Data.StanyMagazynowe_SELECTDataTable? Data = Add_StanyMagazynowe_ta.GetData();
            List<string> list1 = new();
            DataTable dt1 = Data;
            foreach (DataColumn item in dt1.Columns)
            {
                list1.Add(item.ColumnName.ToString());
            }
            Stock_Datagrid.columnnames = list1;
            Stock_Datagrid.Maindatatable = Data;
            Stock_Datagrid.isStock = true;
            Stock_Datagrid.Load();
            Stock_Datagrid.CallHeaders();
        }

        //Function for inserting data to datagrid control
        private void DatagridInsertDataTableStock(int tray)
        {
            Kategoria_Filter = string.IsNullOrEmpty(Kategoria_Filter) ? "000" : Kategoria_Filter;
            Gatunek_Filter = string.IsNullOrEmpty(Gatunek_Filter) ? "000" : Gatunek_Filter;
            Add_Data.StanyMagazynowe__Filter_SELECTDataTable? Data = _dbWMS.Add_StanyMagazynowe_Filter.GetData(Kategoria_Filter, Gatunek_Filter);
            DataTable dt = new();
            DataRow[] dr_array = Data.Select("ID_Polki = '" + tray + "'");
            if (dr_array.Count() > 0)
                dt = dr_array.CopyToDataTable();
            //Add_Data.StanyMagazynowe_SELECTDataTable? Data = Add_StanyMagazynowe_ta.GetData();
            List<string> list1 = new();
            DataTable dt1 = Data;
            foreach (DataColumn item in dt1.Columns)
            {
                list1.Add(item.ColumnName.ToString());
            }
            Stock_Datagrid.columnnames = list1;
            Stock_Datagrid.Maindatatable = dt;
            Stock_Datagrid.isStock = true;
            Stock_Datagrid.Load();
            Stock_Datagrid.CallHeaders();
        }

        //Function opening window for stock deleting
        private void OpenDeleteWindowStock(string Indeks, string ID_Lokalizacji, int ID_Polki, string Partia, string Atest, string Wytop)
        {
            Opacity = 0.5;
            Effect = blur;
            DeleteNote = string.Empty;
            if (HandyControl.Controls.MessageBox.Show(dictionary["DeleteRowQuestion"], Indeks, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Stock stock = new(_dbWMS, Indeks, ID_Lokalizacji, ID_Polki, Partia, Atest, Wytop, DeleteNote, Username);
                stock.Delete();
                DatagridInsertDataTableStock();
                Stock_Datagrid.Refreash();
            }
            else
            {
            }
            Opacity = 1;
            Effect = null;
        }

        public void OpenGetWindowStock(int count, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns)
        {
            Get_A_AnP get_Article = new(dict, dictionary, cells, custom_columns);
            get_Article.Owner = Window.GetWindow(this);
            get_Article.OpenInventoryWindowEvent += Get_Article_OpenInventoryWindowEvent;
            Opacity = 0.5;
            Effect = blur;
            get_Article.ShowDialog();
            Opacity = 1;
            Effect = null;
            DatagridInsertDataTableStock();
            Stock_Datagrid.Refreash();


        }
        public void Get_Article_OpenInventoryWindowEvent(string Symbol)
        {
            DataRow dr = _dbWMS.Add_StanyMagazynowe_Filter.GetData("000", "000").Select(string.Format("Symbol = '{0}'", Symbol))[0];
            Inventory inventory = new(dict, dictionary, dr);
            inventory.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            inventory.ShowDialog();
            Opacity = 1;
            Effect = null;
            DatagridInsertDataTableStock();
            Stock_Datagrid.Refreash();
        }

        public void OpenGiveWindowStock(int count, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns)
        {
            Give_A_AnP give_Article = new(dict, dictionary, cells, custom_columns);
            give_Article.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            give_Article.ShowDialog();
            Opacity = 1;
            Effect = null;
            DatagridInsertDataTableStock();
            Stock_Datagrid.Refreash();
        }
        public void OpenInventoryWindowStock(int count, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns)
        {
            Inventory inventory = new(dict, dictionary, cells, custom_columns);
            inventory.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            inventory.ShowDialog();
            Opacity = 1;
            Effect = null;
            DatagridInsertDataTableStock();
            Stock_Datagrid.Refreash();
        }


        #endregion

        #region Formats for Type
        //Function for inserting data to datagird control
        private void DatagridInsertDataTableType()
        {
            Add_Data.Kar_Gatunki_SELECTDataTable? Data = _dbWMS.Add_Gatunki_ta.GetData();
            List<string> list2 = new();
            DataTable dt2 = Data;
            foreach (DataColumn item in dt2.Columns)
            {
                list2.Add(item.ColumnName.ToString());
            }
            Type_Datagrid.columnnames = list2;
            Type_Datagrid.Maindatatable = Data;
            Type_Datagrid.isType = true;
            Type_Datagrid.Load();
            Type_Datagrid.CallHeaders();
        }
        //Function opening window for type adding
        public void OpenAddWindowType(int count)
        {
            Add_Type add_Type = new(dict, dictionary);
            try
            {
                add_Type.Owner = Window.GetWindow(this);
            }
            catch (Exception)
            {
                add_Type.Topmost = true;
                add_Type.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            Opacity = 0.5;
            Effect = blur;
            add_Type.ShowDialog();
            Opacity = 1;
            Effect = null;
            DatagridInsertDataTableType();
            Type_Datagrid.Refreash();
            Refresh_Trees();
        }
        //Function opening window for type editing
        private void OpenEditWindowType(Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns)
        {
            Add_Type add_Type = new(dict, dictionary, cells, custom_columns, true);
            add_Type.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            add_Type.ShowDialog();
            Opacity = 1;
            Effect = null;
            DatagridInsertDataTableType();
            Type_Datagrid.Refreash();
            Refresh_Trees();
        }
        //Function opening window for type deleting
        private void OpenDeleteWindowType(int? ID_Gatunku)
        {
            Opacity = 0.5;
            Effect = blur;
            if (HandyControl.Controls.MessageBox.Show(dictionary["DeleteRowQuestion"], ID_Gatunku.ToString(), MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                ArticlesControl.Klasy.Type type = new(ID_Gatunku);
                type.Delete();
                DatagridInsertDataTableType();
                Type_Datagrid.Refreash();
            }
            Opacity = 1;
            Effect = null;
            Refresh_Trees();
        }
        //Function opening window for type adding, when row is selected
        public void OpenSuggestedAddWindowType(int count, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns)
        {
            Add_Type add_Type = new(dict, dictionary, cells, custom_columns);
            add_Type.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            add_Type.ShowDialog();
            Opacity = 1;
            Effect = null;
            DatagridInsertDataTableType();
            Type_Datagrid.Refreash();
            Refresh_Trees();
        }
        #endregion

        #region Formats for Order
        //Function for inserting data to datagrid control
        private void DatagridInsertDataTableOrder()
        {
            Add_Data.Zlecenia_SELECTDataTable? Data = _dbWMS.Add_Zlecenia_Select.GetData(OrderStatus, OrderType);
            List<string> list1 = new();
            DataTable dt1 = Data;
            foreach (DataColumn item in dt1.Columns)
            {
                list1.Add(item.ColumnName.ToString());
            }
            Order_Datagrid.columnnames = list1;
            Order_Datagrid.Maindatatable = Data;
            Order_Datagrid.isOrder = true;
            Order_Datagrid.Load();
            Order_Datagrid.CallHeaders();

        }
        //private void OpenSuggestedAddWindowEventOrder(int count, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns)
        //{
        //    Opacity = 0.5;
        //    Effect = blur;
        //    Add_Order add_Order = new(dict, dictionary, cells, custom_columns);
        //    add_Order.Owner = Window.GetWindow(this);
        //    add_Order.ShowDialog();
        //    Opacity = 1;
        //    Effect = null;
        //    DatagridInsertDataTableOrder();
        //    Order_Datagrid.Refreash();
        //}
        ////Function opening window for category adding
        //public void OpenAddWindowOrder(Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns)
        //{
        //    Opacity = 0.5;
        //    Effect = blur;
        //    Add_Order add_Order = new(dict, dictionary, cells, custom_columns);
        //    add_Order.Owner = Window.GetWindow(this);
        //    add_Order.ShowDialog();
        //    Opacity = 1;
        //    Effect = null;
        //    DatagridInsertDataTableOrder();
        //    Order_Datagrid.Refreash();

        //}
        ////Function opening window for category edditing
        //private void OpenEditWindowOrder(Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns)
        //{
        //    Opacity = 0.5;
        //    Effect = blur;
        //    Add_Order add_Order = new(dict, dictionary, cells, custom_columns, 0);
        //    add_Order.Owner = Window.GetWindow(this);
        //    add_Order.ShowDialog();
        //    Opacity = 1;
        //    Effect = null;
        //    DatagridInsertDataTableOrder();
        //    Order_Datagrid.Refreash();
        //}
        ////Function opening window for order deleting
        //private void OpenDeleteWindowOrder(int? NrZlecenia)
        //{

        //}
        private void Order_Datagrid_RunOrdersAutomaticEvent(Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns)
        {
            Opacity = 0.5;
            Effect = blur;
            if (dtOrderLines.Rows.Count>0)
            {
                Orders_Run run_Orders = new(dict, dictionary, cells, custom_columns, dtOrderLines);
                run_Orders.Owner = Window.GetWindow(this);
                run_Orders.ShowDialog();
                Opacity = 1;
                Effect = null;
                DatagridInsertDataTableOrder();
                Order_Datagrid.Refreash();
            }
            else
            {
                HandyControl.Controls.MessageBox.Show("No OrderLine", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                Opacity = 1;
                Effect = null;
            }

        }
        private void Order_Datagrid_RunOrdersManualEvent(Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns)
        {
            Opacity = 0.5;
            Effect = blur;
            Orders_Run run_Orders = new(dict, dictionary, cells, custom_columns, dtOrderLines, orderLineIndex);
            run_Orders.Owner = Window.GetWindow(this);
            run_Orders.ShowDialog();
            Opacity = 1;
            Effect = null;
            DatagridInsertDataTableOrder();
            Order_Datagrid.Refreash();
        }
        private void Order_Datagrid_SelectedRowEvent(string value)
        {
            if (Order_Datagrid.Selectedrow is not null)
            {
                Order_Filter = value;
                DatagridInsertDataTableOrderLine();
                Order_Line_Name.Text = value;
            }
            else
            {
                Order_Line_Name.Text = "";
                Order_Filter = "";
                DatagridInsertDataTableOrderLine();
            }
        }
        #endregion

        #region Formats for OrderLine

        private void DatagridInsertDataTableOrderLine()
        {
            Add_Data.ZleceniaLinie_SELECTDataTable? Data = _dbWMS.Add_ZleceniaLinie_Select.GetData(Order_Filter);
            List<string> list1 = new();
            DataTable dt1 = Data;
            foreach (DataColumn item in dt1.Columns)
            {
                list1.Add(item.ColumnName.ToString());
            }
            OrderLine_Datagrid.columnnames = list1;
            OrderLine_Datagrid.Maindatatable = Data;
            dtOrderLines = Data;
            OrderLine_Datagrid.isOrder = true;
            OrderLine_Datagrid.isOrderLine = true;
            OrderLine_Datagrid.Load();
            OrderLine_Datagrid.CallHeaders();

        }

        private void OrderLine_Datagrid_SelectedIndexEvent(int value)
        {
            orderLineIndex = value;
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

        private void Tray_Datagrid_ShowEmptyTraysEvent(bool value)
        {
            ShowEmpty = value;
            DatagridInsertDataTableTray();
        }

        private void Tray_Datagrid_OpenInventoryWindowEventTray(int ID_Polki)
        {
            DataTable dt = _dbWMS.Add_Lokalizacje_Select.GetData().Select("ID_Polka = " + ID_Polki).CopyToDataTable();

            Inventory inventory = new(dict, dictionary, dt);
            inventory.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            inventory.ShowDialog();
            Opacity = 1;
            Effect = null;
            DatagridInsertDataTableTray();
            Tray_Datagrid.Refreash();
        }

        private void Tray_Datagrid_ShowArticlesOnThisTrayEvent(int ID_Polki)
        {
            TabControl_base.SelectedIndex = 1;
            DatagridInsertDataTableStock(ID_Polki);
            Stock_Datagrid.CallHeaders();
            Stock_Datagrid.ReadSettings();
            Stock_Datagrid.SetButton();
            SetVisibility(Stock_Datagrid);

            Show_Type();
            TabControl2.Visibility = Visibility.Hidden;
            SetMargins();
            if (TabControl.IsVisible)
                TreeHide.Visibility = Visibility.Visible;
            else
                TreeShow.Visibility = Visibility.Visible;

            Stock_Datagrid.MakePlaceForShowHideButton(true);
        }

        #endregion

        #region Other
        private void Split_grid_button(object sender, RoutedEventArgs e)
        {
            if (Order_Grid.RowDefinitions[0].Height == new GridLength(225, GridUnitType.Star))
            {
                Order_Grid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
                Order_Grid.RowDefinitions[2].Height = new GridLength(0, GridUnitType.Star);
            }
            else
            {
                Order_Grid.RowDefinitions[2].Height = new GridLength(1, GridUnitType.Star);
                Order_Grid.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Star);
            }
            Order_Datagrid.ScrollIntoViewSelectedRow();
        }
        private void GridSplitter_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Order_Datagrid.ScrollIntoViewSelectedRow();
        }
        private void TopMenuButton_Click(object sender, MouseButtonEventArgs e)
        {
            TabItem tabitem = sender as TabItem;
            ChangeView(Convert.ToInt32(tabitem.Name.ToString().Substring(tabitem.Name.ToString().Length - 1)));
        }
        public void ChangeView(int tabNumber)
        {
            switch (tabNumber)
            {
                case 1:
                    if ((bool)_loggedUser.OperatorRights["isArticle"].GetValue(3))
                    {
                        DatagridInsertDataTableArticle();
                        SetInfo(Articles_Datagrid);
                    }
                    else
                        HandyControl.Controls.MessageBox.Show(dictionary["AccessDenied"], "Access Denied", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 2:
                    if ((bool)_loggedUser.OperatorRights["isStock"].GetValue(3))
                    {

                        DatagridInsertDataTableStock();
                        SetInfo(Stock_Datagrid);
                    }
                    else
                        HandyControl.Controls.MessageBox.Show(dictionary["AccessDenied"], "Access Denied", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 3:
                    if ((bool)_loggedUser.OperatorRights["isType"].GetValue(3))
                    {
                        DatagridInsertDataTableType();
                        SetInfo(Type_Datagrid);
                    }
                    else
                        HandyControl.Controls.MessageBox.Show(dictionary["AccessDenied"], "Access Denied", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 4:
                    if ((bool)_loggedUser.OperatorRights["isOrder"].GetValue(3))
                    {
                        DatagridInsertDataTableOrder();
                        SetInfo(Order_Datagrid);
                        SetVisibility(Order_Grid);
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
        private bool wasOrderView = false;
        private void SetInfo(DataGridControl.UserControl1 dg)
        {

            if (dg == Articles_Datagrid || dg == Stock_Datagrid || dg == Order_Datagrid)
            {

                if (dg == Order_Datagrid)
                {
                    ShowOrderFilterTab();
                    MoveMargins(false);
                    dg.MakePlaceForShowHideButton(false);
                    wasOrderView = true;
                }
                else
                {
                    if (wasOrderView)
                    {
                        wasOrderView = false;
                        SetMargins();
                    }
                    Show_Type();
                    TabControl2.Visibility = Visibility.Hidden;
                    if (TabControl.IsVisible)
                        TreeHide.Visibility = Visibility.Visible;
                    else
                        TreeShow.Visibility = Visibility.Visible;

                    dg.MakePlaceForShowHideButton(true);
                }
            }
            else
            {
                _ = HideTab();
                TreeHide.Visibility = Visibility.Hidden;
                TreeShow.Visibility = Visibility.Hidden;
                dg.MakePlaceForShowHideButton(false);
            }

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
                    if (item.GetType() == typeof(Grid))
                    {
                        var y = (Grid)item;
                        y.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        var y = (DataGridControl.UserControl1)item;
                        y.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    if (item.GetType() == typeof(Grid))
                    {
                        var y = (Grid)item;
                        y.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        var y = (DataGridControl.UserControl1)item;
                        y.Visibility = Visibility.Hidden;
                    }
                }
            }
        }
        #endregion

        #region Functions for TabControl
        private async Task<Task<bool>> ShowOrderFilterTab()
        {
            //TabControl.Visibility = Visibility.Hidden;
            //TabControl2.Visibility = Visibility.Visible;
            TreeHide.Visibility = Visibility.Hidden;
            TreeShow.Visibility = Visibility.Hidden;
            DoubleAnimation da = new()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.4)
            };
            if (TabControl.Visibility == Visibility.Hidden)
            {
                TabControl2.Visibility = Visibility.Visible;
                TabControl2.BeginAnimation(OpacityProperty, da);
                MoveMargins(true);
            }
            else
            {
                TabControl.Visibility = Visibility.Hidden;
                TabControl2.Visibility = Visibility.Visible;
                TabControl2.Opacity = 1;
            }


            TaskCompletionSource<bool>? tcs = new();
            await Task.Delay(TimeSpan.FromSeconds(0.4));
            return tcs.Task;
        }
        //Function for showing tab control
        private void Order_Filder_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem? selected = (TreeViewItem?)Order_Filter_tree.SelectedItem;
            if (selected.Tag.ToString() == "10")
            {
                Order_Datagrid.Change_SelectionMode(DataGridSelectionMode.Extended);
            }
            else
            {
                Order_Datagrid.Change_SelectionMode(DataGridSelectionMode.Single);
            }
            if (selected.Parent is TreeViewItem)
            {
                TreeViewItem? SelectedParent = (TreeViewItem?)selected.Parent ?? new();
                if (SelectedParent.Tag.ToString() != "0")
                {
                    _ = int.TryParse(SelectedParent.Tag.ToString(), out OrderType);
                    _ = int.TryParse(selected.Tag.ToString(), out OrderStatus);
                }
                else
                {
                    _ = int.TryParse(selected.Tag.ToString(), out OrderType);
                    OrderStatus = 0;
                }
            }
            else
            {
                OrderStatus = 0;
                OrderType = 0;
            }

            DatagridInsertDataTableOrder();
        }
        private async Task<Task<bool>> ShowTab()
        {
            if (TabControl2.IsVisible)
            {
                DoubleAnimation daa = new()
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromSeconds(0.4)
                };
                TabControl2.BeginAnimation(OpacityProperty, daa);
                TabControl2.Visibility = Visibility.Collapsed;
                SetMargins();
            }
            TabControl.Visibility = Visibility.Visible;
            TreeShow.Visibility = Visibility.Collapsed;
            TreeHide.Visibility = Visibility.Visible;
            DoubleAnimation da = new()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.4)
            };

            TabControl.BeginAnimation(OpacityProperty, da);
            TaskCompletionSource<bool>? tcs = new();
            await Task.Delay(TimeSpan.FromSeconds(0.4));
            return tcs.Task;
        }
        //Function for hiding tab control
        private async Task<Task<bool>> HideTab()
        {
            if (TabControl2.IsVisible)
            {
                DoubleAnimation daa = new()
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromSeconds(0.4)
                };
                TabControl2.BeginAnimation(OpacityProperty, daa);
                TabControl2.Visibility = Visibility.Collapsed;
                SetMargins();
            }
            TreeHide.Visibility = Visibility.Collapsed;
            TreeShow.Visibility = Visibility.Visible;
            DoubleAnimation da = new()
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.4)
            };
            da.Completed += Da_Completed;
            TabControl.BeginAnimation(OpacityProperty, da);
            TaskCompletionSource<bool>? tcs = new();
            await Task.Delay(TimeSpan.FromSeconds(0.4));
            return tcs.Task;
        }
        //Catching event when hiding animation is done
        private void Da_Completed(object? sender, EventArgs e)
        {
            TabControl.Visibility = Visibility.Collapsed;
        }
        private void Hide_Type()
        {
            TabControl.SelectedIndex = 0;
            TabItem_Type.Visibility = Visibility.Hidden;
            Expand_Category_TREE();
        }
        private void Expand_Category_TREE()
        {
            foreach (TreeViewItem item in Category_Tree.Items)
            {
                item.IsExpanded = true;
                if (item.Items is not null)
                {
                    ExpandAllNodes(item);
                }
            }
            static void ExpandAllNodes(TreeViewItem treeItem)
            {
                treeItem.IsExpanded = true;
                foreach (TreeViewItem? childItem in treeItem.Items.OfType<TreeViewItem>())
                {
                    ExpandAllNodes(childItem);
                }
            }
        }
        private void Show_Type()
        {
            TabItem_Type.Visibility = Visibility.Visible;
        }
        //Function for instering data into treeview
        private void Load_Category_Tree()
        {
            Category_Tree.Items.Clear();
            Add_Data DS = new();
            _ = _dbWMS.kategoriaTree_SELECT.Fill(DS.KategoriaTree_SELECT);
            //Seting list of items(treeview elements)
            List<Klasy.Item> Items = new();
            foreach (Add_Data.KategoriaTree_SELECTRow poziomtree in DS.KategoriaTree_SELECT.Rows)
            {
                Items.Add(new Klasy.Item(poziomtree.Nazwa, poziomtree.PoziomTree.ToString()[^3..], poziomtree.PoziomTree.ToString()[0..^3]));
            }
            List<TreeViewItem> list = TreeViewItems(Items);
            BuildCategoryTree(list);
        }
        //Function building treeview items with right relations, ang properties representing relations of items
        private void BuildCategoryTree(List<TreeViewItem> items)
        {
            /*
             * Header -> Nazwa kategorii
             * Tag -> Właściwość wskazująca kolejność relacji w danym węźle
             * DisplayMemberPath -> Własciwość wskazująca na rodzica elementu (root nie posiada rodzica)
             */
            TreeViewItem root = new()
            {
                Header = dictionary["All"],
                Tag = "000",
                DisplayMemberPath = "",
                Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["App_MenuItemBackground"].ToString() ?? string.Empty),
            };
            root.Selected += Root_Selected;
            root.Unselected += Root_Unselected;
            items.Add(root);
            foreach (TreeViewItem item in items)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (item.DisplayMemberPath == items[i].DisplayMemberPath + items[i].Tag)
                    {
                        item.Selected += Root_Selected;
                        item.Unselected += Root_Unselected;
                        _ = items[i].Items.Add(item);
                        break;
                    }
                }
            }
            root.MinHeight = 40;

            _ = Category_Tree.Items.Add(root);
        }
        private void Root_Unselected(object sender, RoutedEventArgs e)
        {
            TreeViewItem? root = sender as TreeViewItem ?? new();
            root.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["App_MenuItemBackground"].ToString() ?? string.Empty);
            if (root is not null)
            {
                _ = FindChild<Grid>(root);
                Border b = FindChild<Border>(root);
                if (b is not null)
                {
                    TreeViewItem tvi = FindChild<TreeViewItem>(b);
                    if (tvi is not null)
                    {
                        if (tvi.IsSelected)
                        {
                            tvi.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["App_MenuItemBackground"].ToString() ?? string.Empty);
                        }
                    }
                    else
                    {
                        b.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["App_MenuItemBackground"].ToString() ?? string.Empty);
                    }

                }
            }
            IsSelectedCategoryItemColored = false;
        }
        // Looks for a child control within a parent by type
        public static T FindChild<T>(DependencyObject parent)
            where T : DependencyObject
        {
            // confirm parent is valid.
            if (parent == null)
            {
                return null;
            }

            if (parent is T)
            {
                return parent as T;
            }

            DependencyObject? foundChild = null;

            if (parent is FrameworkElement)
            {
                _ = (parent as FrameworkElement).ApplyTemplate();
            }

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject? child = VisualTreeHelper.GetChild(parent, i);
                foundChild = FindChild<T>(child);
                if (foundChild != null)
                {
                    break;
                }
            }
            return foundChild as T;
        }
        private void Root_Selected(object sender, RoutedEventArgs e)
        {
            if (IsSelectedCategoryItemColored == false)
            {
                TreeViewItem? root = sender as TreeViewItem ?? new();
                root.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["datagrid_headers"].ToString() ?? string.Empty);
                if (root is not null)
                {
                    _ = FindChild<Grid>(root);
                    Border b = FindChild<Border>(root);
                    if (b is not null)
                    {
                        TreeViewItem tvi = FindChild<TreeViewItem>(b);
                        if (tvi is not null)
                        {
                            if (tvi.IsSelected)
                            {
                                tvi.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["datagrid_headers"].ToString() ?? string.Empty);
                            }
                        }
                        else
                        {
                            b.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["datagrid_headers"].ToString() ?? string.Empty);
                        }

                    }
                }
                IsSelectedCategoryItemColored = true;
            }
        }
        //Function building list of TreeViewItems from list of items(custom class, from db - now as I write this I have the impression that I should rename this class xD)
        private List<TreeViewItem> TreeViewItems(List<Klasy.Item> Children)
        {
            List<TreeViewItem> TreeViewItems = new();
            foreach (Klasy.Item? item in Children)
            {
                TreeViewItem treeViewItem = new()
                {
                    Header = item.Nazwa,
                    Tag = item.Id,
                    DisplayMemberPath = item.ParentId,
                    MinHeight = 40
                };
                TreeViewItems.Add(treeViewItem);
                treeViewItem.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["App_MenuItemBackground"].ToString() ?? string.Empty);
            }
            return TreeViewItems;
        }
        //Function building treeview for type, here wont be any nested nodes, so function is much simpler
        private void Load_Type_Tree()
        {
            Type_Tree.Items.Clear();
            Add_Data DS = new();
            _ = _dbWMS.Add_Gatunki_ta.Fill(DS.Kar_Gatunki_SELECT);
            TreeViewItem root = new()
            {
                Header = dictionary["All"],
                Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["App_MenuItemBackground"].ToString() ?? string.Empty),
                MinHeight = 40
            };
            root.Selected += Root_Selected;
            root.Unselected += Root_Unselected;
            foreach (Add_Data.Kar_Gatunki_SELECTRow item in DS.Kar_Gatunki_SELECT.Rows)
            {
                _ = root.Items.Add(new TreeViewItem
                {
                    Header = item.Nazwa,
                    Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["App_MenuItemBackground"].ToString() ?? string.Empty),
                    Tag = item.Nazwa,
                    MinHeight = 40
                });
            }
            foreach (TreeViewItem item in root.Items)
            {
                item.Selected += Root_Selected;
                item.Unselected += Root_Unselected;
            }

            root.MinHeight = 40;
            _ = Type_Tree.Items.Add(root);
        }
        //Function handling category tree changed selected element - datagrids are updated and sorted according to selected item (TODO -> Add other datagrids)
        private void Category_Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            int id = 0;
            TreeViewItem selected = (TreeViewItem)Category_Tree.SelectedItem;
            if (selected is not null)
            {
                //if (Category_Datagrid.Visibility == Visibility.Visible)
                //{
                //    Add_Data DS = new();
                //    _ = kategoriaTree_SELECT.Fill(DS.KategoriaTree_SELECT);
                //    //Seting list of items(treeview elements)
                //    //_ = new();
                //    foreach (Add_Data.KategoriaTree_SELECTRow poziomtree in DS.KategoriaTree_SELECT.Rows)
                //    {
                //        if (poziomtree.Nazwa == selected.Header.ToString())
                //        {
                //            if (selected.DisplayMemberPath + selected.Tag == poziomtree.PoziomTree)
                //            {
                //                id = poziomtree.ID_Kategorii;
                //            }
                //        }
                //    }
                //    Category_Datagrid.Change_Selection(id);
                //}
                //else
                //{
                Kategoria_Filter = selected.DisplayMemberPath + selected.Tag;
                DatagridInsertDataTableArticle();
                DatagridInsertDataTableStock();
                //}
            }
        }
        //Function should change color of tab header when selection is changeg (does not work, probably on visualtree it's not tabitem)
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HandyControl.Controls.TabControl tab = sender as HandyControl.Controls.TabControl ?? new();
            if (tab is not null)
            {
                foreach (TabItem item in tab.Items)
                {
                    item.Background = !item.IsSelected ? (SolidColorBrush?)new BrushConverter().ConvertFrom("#C4C4C4") : (Brush)Brushes.GreenYellow;
                }
            }
        }
        //Function handling category tree changed selected element - datagrids are updated and sorted according to selected item (TODO -> Add other datagrids)
        private void Type_Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem selected = (TreeViewItem)Type_Tree.SelectedItem;
            if (selected is not null)
            {
                Gatunek_Filter = selected.Tag is not null ? selected.Tag.ToString() ?? string.Empty : string.Empty;
                DatagridInsertDataTableArticle();
                DatagridInsertDataTableStock();
            }

        }
        //Function for updating trees
        private void Refresh_Trees()
        {
            Load_Category_Tree();
            Load_Type_Tree();
            Expand_Category_TREE();
        }
        private void Category_Datagrid_SelectedRowEvent(string row)
        {
            string path = "000";
            Add_Data DS = new();
            _ = _dbWMS.kategoriaTree_SELECT.Fill(DS.KategoriaTree_SELECT);
            //Seting list of items(treeview elements)
            List<Klasy.Item> Items = new();
            foreach (Add_Data.KategoriaTree_SELECTRow poziomtree in DS.KategoriaTree_SELECT.Rows)
            {
                if (poziomtree.ID_Kategorii.ToString() == row)
                {
                    path = poziomtree.PoziomTree;
                    break;
                }
            }

            foreach (TreeViewItem item in Category_Tree.Items)
            {
                if (item.DisplayMemberPath + item.Tag == path)
                {
                    item.IsSelected = true;
                    break;
                }
                if (item.Items is not null)
                {
                    ExpandAllNodes(item);
                }
            }
            void ExpandAllNodes(TreeViewItem treeItem)
            {
                if (treeItem.DisplayMemberPath + treeItem.Tag == path)
                {
                    treeItem.IsSelected = true;
                    return;
                }
                foreach (TreeViewItem? childItem in treeItem.Items.OfType<TreeViewItem>())
                {
                    ExpandAllNodes(childItem);
                }
            }
        }
        private void TreeShow_Click(object sender, RoutedEventArgs e)
        {
            _ = ShowTab();

            MoveMargins(true);
        }
        private void TreeHide_Click(object sender, RoutedEventArgs e)
        {
            _ = HideTab();
            if (Category_Tree.SelectedItem is not null || Type_Tree.SelectedItem is not null)
            {
                TreeViewItem? x = (TreeViewItem?)Category_Tree.SelectedItem ?? new();
                TreeViewItem? y = (TreeViewItem?)Type_Tree.SelectedItem ?? new();
                TreeShow.Background = x.DisplayMemberPath is not "" || y.Tag is not null
                    ? Brushes.GreenYellow
                    : (Brush?)(SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["App_Background"].ToString() ?? "ffffff");
            }
            else
            {
                TreeShow.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["App_Background"].ToString() ?? "ffffff");
            }
            SetMargins();
        }

        //Function for moving margins when tab control is hiding.
        private void SetMargins()
        {
            Articles_Datagrid.SetMargin();
            Stock_Datagrid.SetMargin();
            Type_Datagrid.SetMargin();
            Tray_Datagrid.SetMargin();
            Order_Datagrid.SetMargin();
            OrderLine_Datagrid.SetMargin();
        }
        //Function for moving margins when tab control is showing up.
        private void MoveMargins(bool withAniamtion)
        {
            Articles_Datagrid.MoveMargin(true);
            Stock_Datagrid.MoveMargin(true);
            Type_Datagrid.MoveMargin(true);
            Order_Datagrid.MoveMargin(withAniamtion);
            OrderLine_Datagrid.MoveMargin(withAniamtion);
        }
        #endregion

        private void Articles_Loaded(object sender, RoutedEventArgs e)
        {
            Load_Category_Tree();
            Load_Type_Tree();
            ChangeView(1);
        }

        private void OrderTreeShow_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OrderTreeHide_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
