using ClosedXML.Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Xml;

namespace DataGridControl
{
    /// <summary>
    /// Interaction logic for DataGrid
    /// </summary>
    public partial class UserControl1 : BTPControlLibrary.MainPanel
    {
        #region Fields & Variables
        //Set of Events handling actions, when Datagrid gets Article data from DB
        #region Events for Article
        public delegate void openAddWindow(int count);
        public event openAddWindow? OpenAddWindowEvent;
        public delegate void openSuggestedAddWindow(int count, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns);
        public event openSuggestedAddWindow? OpenSuggestedAddWindowEvent;
        public delegate void openEditWindow(Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns);
        public event openEditWindow? OpenEditWindowEvent;
        public delegate void openDeleteWindow(string Indeks);
        public event openDeleteWindow? OpenDeleteWindowEvent;
        public DataRowView? Selectedrow;

        public delegate void openTraysGetWindow(int count, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns);
        public event openTraysGetWindow? OpenTraysGetWindowEvent;
        public delegate void openTraysGiveWindow(int count, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns);
        public event openTraysGiveWindow? OpenTraysGiveWindowEvent;
        #endregion
        //Set of Events handling actions, when Datagrid gets Stock data from DB
        #region Events for Stock
        public delegate void openAddWindowStock(int count);
        public event openAddWindowStock? OpenAddWindowEventStock;
        public delegate void openSuggestedAddWindowStock(int count, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns);
        public event openSuggestedAddWindowStock? OpenSuggestedAddWindowEventStock;
        public delegate void openEditWindowStock(Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns);
        public event openEditWindowStock? OpenEditWindowEventStock;
        public delegate void openDeleteWindowStock(string Indeks, string ID_Lokalizacji, int ID_Polki, string Partia, string Atest, string Wytop);
        public event openDeleteWindowStock? OpenDeleteWindowEventStock;

        public delegate void openGetWindowStock(int count, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns);
        public event openGetWindowStock? OpenGetWindowEventStock;
        public delegate void openGiveWindowStock(int count, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns);
        public event openGiveWindowStock? OpenGiveWindowEventStock;
        public delegate void openInventoryWindowStock(int count, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns);
        public event openInventoryWindowStock? OpenInventoryWindowEventStock;
        #endregion
        //Set of Events handling actions, when Datagrid gets Type data from DB
        #region Events for Type
        public delegate void openAddWindowType(int count);
        public event openAddWindowType? OpenAddWindowEventType;
        public delegate void openSuggestedAddWindowType(int count, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns);
        public event openSuggestedAddWindowType? OpenSuggestedAddWindowEventType;
        public delegate void openEditWindowType(Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns);
        public event openEditWindowType? OpenEditWindowEventType;
        public delegate void openDeleteWindowType(int? ID_Gatunku);
        public event openDeleteWindowType? OpenDeleteWindowEventType;
        #endregion
        //Set of Events handling actions, when Datagrid gets Orders data from DB
        #region Events for Order
        public delegate void openAddWindowOrder(Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns);
        public event openAddWindowOrder? OpenAddWindowEventOrder;
        public delegate void openSuggestedAddWindowOrder(int count, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns);
        public event openSuggestedAddWindowOrder? OpenSuggestedAddWindowEventOrder;
        public delegate void openEditWindowOrder(Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns);
        public event openEditWindowOrder? OpenEditWindowEventOrder;
        public delegate void openDeleteWindowOrder(int? NrZlecenia);
        public event openDeleteWindowOrder? OpenDeleteWindowEventOrder;
        public delegate void selectedRow(string value);
        public event selectedRow? SelectedRowEvent;
        public delegate void selectedIndex(int value);
        public event selectedIndex? SelectedIndexEvent;
        public delegate void runAutomatic(Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns);
        public event runAutomatic? RunOrdersAutomaticEvent;
        public delegate void runManual(Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns);
        public event runManual? RunOrdersManualEvent;
        #endregion
        //Set of Events handling actions, when Datagrid gets Operator data from DB
        #region Events for Operator
        public delegate void openAddWindowOperator();
        public event openAddWindowOperator? OpenAddWindowEventOperator;
        public delegate void openSuggestedAddWindowOperator(int count, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns);
        public event openSuggestedAddWindowOrder? OpenSuggestedAddWindowEventOperator;
        public delegate void openEditWindowOperator(Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns);
        public event openEditWindowOperator? OpenEditWindowEventOperator;
        public delegate void openDeleteWindowOperator(string? ID_Operator);
        public event openDeleteWindowOperator? OpenDeleteWindowEventOperator;
        #endregion

        #region Other Events
        public delegate void selectedCellChanged(DataRow dr);
        public event selectedCellChanged? SelectedCellChangedEvent;
        #endregion 
        //Set of Lists for storing columnnames, filters, keys and values etc.
        #region Lists
        private List<string> keys = new();
        private List<string> values = new();
        private readonly List<string> Headers = new();
        private readonly List<string> VisibleHeaders = new();
        public List<string> columnnames = new();
        public List<string> filters = new();
        #endregion
        //Set of Events handling actions, when Datagrid gets Tray data from DB
        #region Events for Tray
        public delegate void openAddWindowTray();
        public event openAddWindowTray? OpenAddWindowEventTray;
        public delegate void openEditWindowTray(Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns);
        public event openEditWindowTray? OpenEditWindowEventTray;
        public delegate void openDeleteWindowTray(int? ID_Polki, string NrPolki);
        public event openDeleteWindowTray? OpenDeleteWindowEventTray;
        public delegate void showEmptyTrays(bool value);
        public event showEmptyTrays? ShowEmptyTraysEvent;
        public delegate void showArticlesOnThisTray(int ID_Polki);
        public event showArticlesOnThisTray? ShowArticlesOnThisTrayEvent;
        public delegate void openInventoryWindowTray(int ID_Polki);
        public event openInventoryWindowTray? OpenInventoryWindowEventTray;
        #endregion
        //Set of Dictionaries for storing cells, column names etc.
        #region Dictionaries
        private  ResourceDictionary dict = new();
        private readonly ResourceDictionary dictColors = new();
        private Dictionary<string, string> dictionary;
        private readonly Dictionary<string, string> custom_columns = new();
        public Dictionary<string, Tuple<string, Type>> cells = new();
        public static Dictionary<string, Tuple<string, Type>> updatecells = new();
        private CultureInfo culture = CultureInfo.InvariantCulture;


        public delegate void DefaulsData();
        public event DefaulsData? OnDefaulsData;
        #endregion

        //Set of DataTables for storing data in datagrid
        #region DataTables
        private DataTable _Maindatatable { get; set; } = new();
        public DataTable Maindatatable
        {
            get => _Maindatatable;
            set
            {
                if (_Maindatatable != value)
                {
                    _Maindatatable = value;
                }
            }
        }
        public DataTable additionaldatatable = new();
        #endregion
        //Set of fields and variables
        #region Other Fields & events
        public bool isArticle { get; set; }
        public bool isStock { get; set; }
        public bool isType { get; set; }
        public bool isTray { get; set; }
        public bool isOrder { get; set; }
        public bool isOrderLine { get; set; }
        public bool isTrade { get; set; }
        public bool isOperator { get; set; }
        public bool isLogs { get; set; }
        public bool isStats { get; set; }


        public static string DateColumnFilterString { get; set; } = string.Empty;
        public static string NumberColumnFilterString { get; set; } = string.Empty;
        public static string TextColumnFilterString { get; set; } = string.Empty;
        public static string? Columnnametofilter { get; set; } = string.Empty;
        public static bool Isnamechanging { get; set; } = false;
        public static bool Isnumberfilter { get; set; } = false;
        public static bool Istextfilter { get; set; } = false;
        private string Filtrstring { get; set; } = string.Empty;
        private static bool HideOrVisible { get; set; } = true;
        private string culturestring = string.Empty;
        private int updaterowindex;
        private readonly BlurEffect blur = new();
        #endregion
        #endregion

        #region Functions

        //Main constructor
        public UserControl1()
        {
            InitializeComponent();
            //Initialize dictionary for the App. Pol as default language.
            dict = _translation.ReadTranslations();
            Resources.MergedDictionaries.Add(dict);
            keys = dict.Keys.Cast<string>().ToList();
            values = dict.Values.Cast<string>().ToList();
            dictionary = keys.Zip(values, (k, v) => new { Key = k, Value = v })
                     .ToDictionary(x => x.Key, x => x.Value);
            datagrid.Visibility = Visibility.Visible;
        }

        protected override void InternalInit()
        {

        }

        #region Buttons
        //Refresh button handling. 
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnDefaulsData();
            //Enabling buttons for apply and save settings of datagrid customization.
            //Save data from database to Maindatatable.
            //Maindatatable = Connection();
            //Setting datagrid items from maindatatable
            datagrid.ItemsSource = Maindatatable.DefaultView;
            //Idk why, but this made combobox default select empty. Headers is filled in readsettings().
            ColumnHide_ComboBox.ItemsSource = Headers;
            //Settings from xml file setting up.
            if (File.Exists(string.Format(Maindatatable.TableName + "_Settings.xml")))
            {
                ReadSettings();
            }
            AddRowFilters("");
            ColumnHide_ComboBox.SelectedIndex = 0;
            Headers.Clear();
            string header;
            foreach (DataGridColumn item in datagrid.Columns)
            {
                header = item.Header.ToString() ?? string.Empty;
                if (header[0] != '_' && !header.Contains("ID_"))
                {
                    Headers.Add(header);
                }
            }
            ColumnHide_ComboBox.ItemsSource = Headers;
            //ColumnHide_ComboBox.Items.Refresh();
            //datagrid.Items.Refresh();
            ReadSettings();
        }
        //Handling of hidding columns by buttons in combobox. <TODO: Set color of buttons when settings are read from xml file>
        private void Toggle_Click(object sender, RoutedEventArgs e)
        {
            Button? bt = sender as Button ?? new();
            string collumn_name_to_hide = bt.Content.ToString() ?? string.Empty;
            bool isfilteron = false;
            foreach (string? item in filters)
            {
                if (item.Contains(collumn_name_to_hide))
                {
                    isfilteron = true;
                    break;
                }
            }
            if (isfilteron)
            {
                //MessageBox.Show(dictionary["hiding_column_with_filter"]);
                HandyControl.Controls.MessageBox.Show(dictionary["hiding_column_with_filter"], "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {

                foreach (DataGridColumn c in datagrid.Columns)
                {
                    if (c.Header.ToString() == collumn_name_to_hide)
                    {
                        c.Visibility = c.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                        bt.Background = c.Visibility == Visibility.Visible ? Brushes.YellowGreen : Brushes.OrangeRed;
                        break;
                    }
                }
            }
            WriteSettings();
            ReadSettings();
            UpdateAvaiableColumnsForFiltering();
        }
        //Public Function adding columns avaialbe for filtering 
        public void UpdateAvaiableColumnsForFiltering()
        {
            VisibleHeaders.Clear();
            foreach (DataGridColumn c in datagrid.Columns)
            {
                if (c.Visibility == Visibility.Visible)
                {
                    VisibleHeaders.Add(c.Header.ToString());
                }
            }
            Column_to_filter_Combobox.ItemsSource = null;
            Column_to_filter_Combobox.ItemsSource = VisibleHeaders;
        }
        //Function handling delete all filter button.DatagridSorting
        private void Delete_filter_button_Click(object sender, RoutedEventArgs e)
        {
            if (filters.Count >= 1)
            {
                DateColumnFilterString = string.Empty;
                NumberColumnFilterString = string.Empty;
                TextColumnFilterString = string.Empty;
                filters.Clear();
                Filtrstring = string.Empty;
                AddRowFilters("");
                Delete_filter_button.IsEnabled = false;
                Delete_filter_button.Background = Brushes.Silver;
            }
            else
            {
                Delete_filter_button.IsEnabled = false;
                Delete_filter_button.Background = Brushes.Silver;
            }
        }
        //Function for handling show/hide button
        private void Show_hide_button_Click(object sender, RoutedEventArgs e)
        {
            HideOrVisible = true;
            bool isfilteron = false;
            foreach (string? item in filters)
            {
                if (item.Length != 0)
                {
                    isfilteron = true;
                    break;
                }
            }
            if (isfilteron)
            {
                HandyControl.Controls.MessageBox.Show(dictionary["hiding_column_with_filter"], "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (HideOrVisible)
                {
                    foreach (DataGridColumn? item in datagrid.Columns)
                    {
                        if (item.Header.ToString()?.Length > 3)
                        {
                            if (item.Header.ToString()?[..3] == "ID_")
                            {
                                continue;
                            }
                            if (item.Header.ToString()?[0] == '_')
                            {
                                continue;
                            }
                            //item.Visibility = Visibility.Visible;
                        }
                        item.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    Show_image.Visibility = Visibility.Collapsed;
                    hide_image.Visibility = Visibility.Visible;
                    foreach (DataGridColumn? item in datagrid.Columns)
                    {
                        item.Visibility = Visibility.Collapsed;
                    }
                }
                WriteSettings();
            }
            UpdateAvaiableColumnsForFiltering();
        }
        //Handling add data button on datagrid. If table contains kar_artykuły, triggers special window from main window, if not it auto generate template for adding row.
        private void Add_Data_Click(object sender, RoutedEventArgs e)
        {
            if (datagrid.SelectedItem is not null)
            {
                datagrid.CurrentItem = datagrid.SelectedItem;
                updatecells.Clear();
                datagrid.SelectionUnit = DataGridSelectionUnit.FullRow;
                DataGridRow? r = new();
                DependencyObject? dep = (DependencyObject)e.OriginalSource;
                while ((dep != null) && (dep is not DataGridCell))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }
                //if (dep == null) return;
                if (dep is DataGridCell)
                {
                    DataGridCell? cell = dep as DataGridCell ?? new();
                    cell.Focus();

                    while ((dep != null) && (dep is not DataGridRow))
                    {
                        dep = VisualTreeHelper.GetParent(dep);
                    }
                    r = dep as DataGridRow ?? new();
                    datagrid.SelectedItem = r.DataContext;
                }
                DataRowView? dataRowView = datagrid.CurrentItem as DataRowView;
                DataRow dr = dataRowView!.Row;
                int currentRowIndex = datagrid.Items.IndexOf(datagrid.CurrentItem);
                for (int i = 0; i < Maindatatable.Rows.Count; i++)
                {
                    if (Maindatatable.Rows[i].Equals(dr))
                    {
                        currentRowIndex = i;
                    }
                }
                DataGridRow row = (DataGridRow)datagrid.ItemContainerGenerator
                                                  .ContainerFromIndex(currentRowIndex);
                updaterowindex = currentRowIndex;
                //cells = GetCells(row);
                cells = GetCells(currentRowIndex);
            }
            if (isArticle)
            {
                if (datagrid.SelectedItem is not null)
                {
                    openSuggestedAddWindow? handler = OpenSuggestedAddWindowEvent;
                    OpenSuggestedAddWindowEvent?.Invoke(Maindatatable.Rows.Count, cells, custom_columns);
                }
                else
                {
                    openAddWindow? handler = OpenAddWindowEvent;
                    OpenAddWindowEvent?.Invoke(Maindatatable.Rows.Count);
                }
            }
            else if (isStock)
            {
                if (datagrid.SelectedItem is not null)
                {
                    openSuggestedAddWindowStock? handler = OpenSuggestedAddWindowEventStock;
                    OpenSuggestedAddWindowEventStock?.Invoke(Maindatatable.Rows.Count, cells, custom_columns);
                }
                else
                {
                    openAddWindowStock? handler = OpenAddWindowEventStock;
                    OpenAddWindowEventStock?.Invoke(Maindatatable.Rows.Count);
                }
            }
            else if (isType)
            {
                if (datagrid.SelectedItem is not null)
                {
                    openSuggestedAddWindowType? handler = OpenSuggestedAddWindowEventType;
                    OpenSuggestedAddWindowEventType?.Invoke(Maindatatable.Rows.Count, cells, custom_columns);

                }
                else
                {
                    openAddWindowType? handler = OpenAddWindowEventType;
                    OpenAddWindowEventType?.Invoke(Maindatatable.Rows.Count);
                }
            }
            else if (isOrder)
            {
                if (datagrid.SelectedItem is not null)
                {
                    openSuggestedAddWindowOrder? handler = OpenSuggestedAddWindowEventOrder;
                    OpenSuggestedAddWindowEventOrder?.Invoke(Maindatatable.Rows.Count, cells, custom_columns);
                }
                else
                {
                    cells.Clear();
                    openAddWindowOrder? handler = OpenAddWindowEventOrder;
                    OpenAddWindowEventOrder?.Invoke(cells, custom_columns);
                }
            }
            else if (isOperator)
            {
                openAddWindowOperator? handler = OpenAddWindowEventOperator;
                OpenAddWindowEventOperator?.Invoke();
            }
            else
            {
                AddData(sender, e);
            }
        }
        //Handling delete data button on datagrid. If table contains kar_artykuły, triggers special window from main window, if not it auto generate template for edditing row.
        private void Delete_Data_Click(object sender, RoutedEventArgs e)
        {
            datagrid.CurrentItem = datagrid.SelectedItem;
            updatecells.Clear();
            datagrid.SelectionUnit = DataGridSelectionUnit.FullRow;
            DataGridRow? r = new();
            DependencyObject? dep = (DependencyObject)e.OriginalSource;
            while (dep is not null and not DataGridCell)
            {
                dep = VisualTreeHelper.GetParent(dep);
            }
            //if (dep == null) return;
            if (dep is DataGridCell)
            {
                DataGridCell? cell = dep as DataGridCell ?? new();
                _ = cell.Focus();

                while (dep is not null and not DataGridRow)
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }
                r = dep as DataGridRow ?? new();
                datagrid.SelectedItem = r.DataContext;
            }
            DataRowView? dataRowView = datagrid.CurrentItem as DataRowView;
            DataRow dr = dataRowView!.Row;
            int currentRowIndex = datagrid.Items.IndexOf(datagrid.CurrentItem);
            for (int i = 0; i < Maindatatable.Rows.Count; i++)
            {
                if (Maindatatable.Rows[i].Equals(dr))
                {
                    currentRowIndex = i;
                }
            }
            DataGridRow row = (DataGridRow)datagrid.ItemContainerGenerator
                                              .ContainerFromIndex(currentRowIndex);
            updaterowindex = currentRowIndex;
            //cells = GetCells(row);
            cells = GetCells(currentRowIndex);
            //datagrid.CurrentItem = datagrid.SelectedItem;
            //updatecells.Clear();
            //datagrid.SelectionUnit = DataGridSelectionUnit.FullRow;
            //DataGridRow? r = new();
            //DependencyObject? dep = (DependencyObject)e.OriginalSource;
            //while ((dep != null) && (dep is not DataGridCell))
            //{
            //    dep = VisualTreeHelper.GetParent(dep);
            //}
            ////if (dep == null) return;
            //if (dep is DataGridCell)
            //{
            //    DataGridCell? cell = dep as DataGridCell ?? new();
            //    cell.Focus();

            //    while ((dep != null) && (dep is not DataGridRow))
            //    {
            //        dep = VisualTreeHelper.GetParent(dep);
            //    }
            //    r = dep as DataGridRow ?? new();
            //    datagrid.SelectedItem = r.DataContext;
            //}
            //DataRowView? dataRowView = datagrid.CurrentItem as DataRowView;
            //DataRow dr = dataRowView!.Row;
            //int currentRowIndex = datagrid.Items.IndexOf(datagrid.CurrentItem);
            //for (int i = 0; i < Maindatatable.Rows.Count; i++)
            //{
            //    if (Maindatatable.Rows[i].Equals(dr))
            //    {
            //        currentRowIndex = i;
            //    }
            //}
            //DataGridRow row = (DataGridRow)datagrid.ItemContainerGenerator
            //                                  .ContainerFromIndex(currentRowIndex);
            //updaterowindex = currentRowIndex;

            //cells = GetCells(currentRowIndex);
            if (isArticle)
            {
                openDeleteWindow? handler = OpenDeleteWindowEvent;
                OpenDeleteWindowEvent?.Invoke(cells["Indeks"].Item1.ToString());
            }
            else if (isStock)
            {
                openDeleteWindowStock? handler = OpenDeleteWindowEventStock;
                if (OpenDeleteWindowEventStock is not null)
                {
                    //, int ID_Lokalizacji, int ID_Polki, string Partia, string Atest, string Wytop
                    OpenDeleteWindowEventStock?.Invoke(cells["Indeks"].Item1.ToString(), cells["Symbol"].Item1, Convert.ToInt32(cells["ID_Polki"].Item1.ToString()), cells["Partia"].Item1.ToString(), cells["Atest"].Item1.ToString(), cells["Wytop"].Item1.ToString());
                }
            }
            else if (isType)
            {
                openDeleteWindowType? handler = OpenDeleteWindowEventType;
                if (OpenDeleteWindowEventType is not null)
                {
                    _ = int.TryParse(cells["ID_Gatunku"].Item1.ToString(), out int temp);
                    OpenDeleteWindowEventType?.Invoke(temp);
                }
            }
            else if (isOrder)
            {
                openDeleteWindowOrder? handler = OpenDeleteWindowEventOrder;
                if (OpenDeleteWindowEventOrder is not null)
                {
                    _ = int.TryParse(cells["NrZlecenia"].Item1.ToString(), out int temp);
                    OpenDeleteWindowEventOrder?.Invoke(temp);
                }
            }
            else if (isOperator)
            {
                openDeleteWindowOperator? handler = OpenDeleteWindowEventOperator;
                if (OpenDeleteWindowEventOperator is not null)
                {
                    OpenDeleteWindowEventOperator?.Invoke(cells["Login"].Item1.ToString());
                }
            }
            else
            {
                EditData(sender, e);
            }
        }
        //Handling get article
        private void Get_Click(object sender, RoutedEventArgs e)
        {
            if (datagrid.SelectedItem is not null)
            {
                datagrid.CurrentItem = datagrid.SelectedItem;
                updatecells.Clear();
                datagrid.SelectionUnit = DataGridSelectionUnit.FullRow;
                DataGridRow? r = new();
                DependencyObject? dep = (DependencyObject)e.OriginalSource;
                while ((dep != null) && (dep is not DataGridCell))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }
                //if (dep == null) return;
                if (dep is DataGridCell)
                {
                    DataGridCell? cell = dep as DataGridCell ?? new();
                    cell.Focus();

                    while ((dep != null) && (dep is not DataGridRow))
                    {
                        dep = VisualTreeHelper.GetParent(dep);
                    }
                    r = dep as DataGridRow ?? new();
                    datagrid.SelectedItem = r.DataContext;
                }
                DataRowView? dataRowView = datagrid.CurrentItem as DataRowView;
                DataRow dr = dataRowView!.Row;
                int currentRowIndex = datagrid.Items.IndexOf(datagrid.CurrentItem);
                for (int i = 0; i < Maindatatable.Rows.Count; i++)
                {
                    if (Maindatatable.Rows[i].Equals(dr))
                    {
                        currentRowIndex = i;
                    }
                }
                DataGridRow row = (DataGridRow)datagrid.ItemContainerGenerator
                                                  .ContainerFromIndex(currentRowIndex);
                updaterowindex = currentRowIndex;
                //cells = GetCells(row);
                cells = GetCells(currentRowIndex);
            }
            if (isArticle)
            {
                if (datagrid.SelectedItem is not null)
                {
                    openTraysGetWindow? handler = OpenTraysGetWindowEvent;
                    OpenTraysGetWindowEvent?.Invoke(Maindatatable.Rows.Count, cells, custom_columns);
                }
            }
            else if (isStock)
            {
                if (datagrid.SelectedItem is not null)
                {
                    openGetWindowStock? handler = OpenGetWindowEventStock;
                    OpenGetWindowEventStock?.Invoke(Maindatatable.Rows.Count, cells, custom_columns);
                }
            }
        }
        //Handling give article
        private void Give_Click(object sender, RoutedEventArgs e)
        {
            if (datagrid.SelectedItem is not null)
            {
                datagrid.CurrentItem = datagrid.SelectedItem;
                updatecells.Clear();
                datagrid.SelectionUnit = DataGridSelectionUnit.FullRow;
                DataGridRow? r = new();
                DependencyObject? dep = (DependencyObject)e.OriginalSource;
                while ((dep != null) && (dep is not DataGridCell))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }
                //if (dep == null) return;
                if (dep is DataGridCell)
                {
                    DataGridCell? cell = dep as DataGridCell ?? new();
                    cell.Focus();

                    while ((dep != null) && (dep is not DataGridRow))
                    {
                        dep = VisualTreeHelper.GetParent(dep);
                    }
                    r = dep as DataGridRow ?? new();
                    datagrid.SelectedItem = r.DataContext;
                }
                DataRowView? dataRowView = datagrid.CurrentItem as DataRowView;
                DataRow dr = dataRowView!.Row;
                int currentRowIndex = datagrid.Items.IndexOf(datagrid.CurrentItem);
                for (int i = 0; i < Maindatatable.Rows.Count; i++)
                {
                    if (Maindatatable.Rows[i].Equals(dr))
                    {
                        currentRowIndex = i;
                    }
                }
                DataGridRow row = (DataGridRow)datagrid.ItemContainerGenerator
                                                  .ContainerFromIndex(currentRowIndex);
                updaterowindex = currentRowIndex;
                //cells = GetCells(row);
                cells = GetCells(currentRowIndex);
            }
            if (isArticle)
            {
                if (datagrid.SelectedItem is not null)
                {
                    openTraysGiveWindow? handler = OpenTraysGiveWindowEvent;
                    OpenTraysGiveWindowEvent?.Invoke(Maindatatable.Rows.Count, cells, custom_columns);
                }
            }
            else if (isStock)
            {
                if (datagrid.SelectedItem is not null)
                {
                    openGiveWindowStock? handler = OpenGiveWindowEventStock;
                    OpenGiveWindowEventStock?.Invoke(Maindatatable.Rows.Count, cells, custom_columns);
                }
            }
        }
        //Handling do inventory
        private void Inventory_Click(object sender, RoutedEventArgs e)
        {
            if (datagrid.SelectedItem is not null)
            {
                datagrid.CurrentItem = datagrid.SelectedItem;
                updatecells.Clear();
                datagrid.SelectionUnit = DataGridSelectionUnit.FullRow;
                DataGridRow? r = new();
                DependencyObject? dep = (DependencyObject)e.OriginalSource;
                while ((dep != null) && (dep is not DataGridCell))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }
                //if (dep == null) return;
                if (dep is DataGridCell)
                {
                    DataGridCell? cell = dep as DataGridCell ?? new();
                    cell.Focus();

                    while ((dep != null) && (dep is not DataGridRow))
                    {
                        dep = VisualTreeHelper.GetParent(dep);
                    }
                    r = dep as DataGridRow ?? new();
                    datagrid.SelectedItem = r.DataContext;
                }
                DataRowView? dataRowView = datagrid.CurrentItem as DataRowView;
                DataRow dr = dataRowView!.Row;
                int currentRowIndex = datagrid.Items.IndexOf(datagrid.CurrentItem);
                for (int i = 0; i < Maindatatable.Rows.Count; i++)
                {
                    if (Maindatatable.Rows[i].Equals(dr))
                    {
                        currentRowIndex = i;
                    }
                }
                DataGridRow row = (DataGridRow)datagrid.ItemContainerGenerator
                                                  .ContainerFromIndex(currentRowIndex);
                updaterowindex = currentRowIndex;
                //cells = GetCells(row);
                cells = GetCells(currentRowIndex);
            }
            if (isStock)
            {
                if (datagrid.SelectedItem is not null)
                {
                    openInventoryWindowStock? handler = OpenInventoryWindowEventStock;
                    OpenInventoryWindowEventStock?.Invoke(Maindatatable.Rows.Count, cells, custom_columns);
                }
            }
            if (isTray)
            {
                if (datagrid.SelectedItem is not null)
                {
                    DataRow dr = (DataRow)Selectedrow.Row;
                    openInventoryWindowTray? handler = OpenInventoryWindowEventTray;
                    OpenInventoryWindowEventTray?.Invoke(Convert.ToInt32(dr["NrPolki"]));

                }
            }

        }
        //Handling button showing empty tray (empty tray)
        private void ShowEmpty_Checked(object sender, RoutedEventArgs e)
        {
            showEmptyTrays? handler = ShowEmptyTraysEvent;
            if (ShowEmptyTraysEvent is not null)
            {
                ShowEmptyTraysEvent?.Invoke(true);
            }
            txt_empty.Visibility = Visibility.Collapsed;
            img_empty.Visibility = Visibility.Collapsed;
            txt_all.Visibility = Visibility.Visible;
            img_all.Visibility = Visibility.Visible;
        }
        //Handling button showing empty tray (all tray)
        private void ShowEmpty_Unchecked(object sender, RoutedEventArgs e)
        {
            showEmptyTrays? handler = ShowEmptyTraysEvent;
            if (ShowEmptyTraysEvent is not null)
            {
                ShowEmptyTraysEvent?.Invoke(false);
            }
            txt_all.Visibility = Visibility.Collapsed;
            img_all.Visibility = Visibility.Collapsed;
            txt_empty.Visibility = Visibility.Visible;
            img_empty.Visibility = Visibility.Visible;
        }
        //Handling visibility of Buttons depending of actual tab item
        public void SetButton()
        {
            if (isArticle)
            {
                Add_Data.Visibility = Visibility.Visible;
                Get_Data.Visibility = Visibility.Visible;
                Give_Data.Visibility = Visibility.Visible;
                Edit_Data.Visibility = Visibility.Visible;
                Delete_Data.Visibility = Visibility.Visible;
                Inventory_Data.Visibility = Visibility.Hidden;
                Show_Article.Visibility = Visibility.Hidden;
                Empty_Fill_Tray.Visibility = Visibility.Hidden;
                Start_Orders_A.Visibility = Visibility.Hidden;
                Start_Orders_M.Visibility = Visibility.Hidden;
            }
            else if (isStock)
            {
                Add_Data.Visibility = Visibility.Hidden;
                Delete_Data.Visibility = Visibility.Visible;
                Edit_Data.Visibility = Visibility.Hidden;
                Get_Data.Visibility = Visibility.Visible;
                Give_Data.Visibility = Visibility.Visible;
                Inventory_Data.Visibility = Visibility.Visible;
                Show_Article.Visibility = Visibility.Hidden;
                Empty_Fill_Tray.Visibility = Visibility.Hidden;
                Start_Orders_A.Visibility = Visibility.Hidden;
                Start_Orders_M.Visibility = Visibility.Hidden;
            }
            else if (isType)
            {
                Add_Data.Visibility = Visibility.Visible;
                Delete_Data.Visibility = Visibility.Visible;
                Edit_Data.Visibility = Visibility.Visible;
                Get_Data.Visibility = Visibility.Hidden;
                Give_Data.Visibility = Visibility.Hidden;
                Inventory_Data.Visibility = Visibility.Hidden;
                Show_Article.Visibility = Visibility.Hidden;
                Empty_Fill_Tray.Visibility = Visibility.Hidden;
                Start_Orders_A.Visibility = Visibility.Hidden;
                Start_Orders_M.Visibility = Visibility.Hidden;
            }
            else if (isOrder)
            {
                Inventory_Data.Visibility = Visibility.Hidden;
                Add_Data.Visibility = Visibility.Hidden;
                Delete_Data.Visibility = Visibility.Hidden;
                Edit_Data.Visibility = Visibility.Hidden;
                Get_Data.Visibility = Visibility.Hidden;
                Give_Data.Visibility = Visibility.Hidden;
                Show_Article.Visibility = Visibility.Hidden;
                Empty_Fill_Tray.Visibility = Visibility.Hidden;
                Start_Orders_A.Visibility = Visibility.Visible;
                Start_Orders_M.Visibility = Visibility.Visible;
            }
            else if (isTray)
            {
                Add_Data.Visibility = Visibility.Hidden;
                Delete_Data.Visibility = Visibility.Hidden;
                Edit_Data.Visibility = Visibility.Hidden;
                Get_Data.Visibility = Visibility.Hidden;
                Give_Data.Visibility = Visibility.Hidden;
                Show_Article.Visibility = Visibility.Visible;
                Inventory_Data.Visibility = Visibility.Visible;
                Empty_Fill_Tray.Visibility = Visibility.Visible;
                Start_Orders_A.Visibility = Visibility.Hidden;
                Start_Orders_M.Visibility = Visibility.Hidden;
            }
            else if (isTrade)
            {
                Inventory_Data.Visibility = Visibility.Hidden;
                Add_Data.Visibility = Visibility.Hidden;
                Delete_Data.Visibility = Visibility.Hidden;
                Edit_Data.Visibility = Visibility.Hidden;
                Get_Data.Visibility = Visibility.Hidden;
                Give_Data.Visibility = Visibility.Hidden;
                Show_Article.Visibility = Visibility.Hidden;
                Empty_Fill_Tray.Visibility = Visibility.Hidden;
                Start_Orders_A.Visibility = Visibility.Hidden;
                Start_Orders_M.Visibility = Visibility.Hidden;
            }
            else if (isOperator)
            {
                Inventory_Data.Visibility = Visibility.Hidden;
                Add_Data.Visibility = Visibility.Visible;
                Delete_Data.Visibility = Visibility.Visible;
                Edit_Data.Visibility = Visibility.Visible;
                Get_Data.Visibility = Visibility.Hidden;
                Give_Data.Visibility = Visibility.Hidden;
                Show_Article.Visibility = Visibility.Hidden;
                Empty_Fill_Tray.Visibility = Visibility.Hidden;
                Start_Orders_A.Visibility = Visibility.Hidden;
                Start_Orders_M.Visibility = Visibility.Hidden;
            }
            SetButtonsDependingOnUserAccess();
        }
        //Handling Export to excel button click. 
        private void Export_To_Excel_Click(object sender, RoutedEventArgs e)
        {
            XLWorkbook wb = new();
            DataView dv = new(Maindatatable);
            Filtrstring = string.Empty;
            foreach (string item in filters)
            {
                //condition for multi-filtering
                if (Filtrstring.Length > 1)
                {
                    Filtrstring += " and ";
                }
                Filtrstring += item;
            }
            //Final filtering with all conditions.
            dv.RowFilter = Filtrstring;
            DataTable dt = dv.ToTable();
            string header;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dt.Columns[i].ColumnName = Headers[i];
            }
            foreach (DataGridColumn item in datagrid.Columns)
            {
                header = item.Header.ToString() ?? string.Empty;
                if (item.Visibility != Visibility.Visible)
                {
                    dt.Columns.Remove(header);
                }
            }
            wb.Worksheets.Add(dt, "WorksheetName");
            wb.SaveAs(String.Format(datagrid.Name + "export.xlsx"));
            OpenWithExcel(String.Format(datagrid.Name + "export.xlsx"));
            wb.Worksheets.Delete("WorksheetName");
        }
        //Handling edit data button on datagrid. If table contains kar_artykuły, triggers special window from main window, if not it auto generate template for edditing row.
        private void Edit_Data_Click(object sender, RoutedEventArgs e)
        {
            datagrid.CurrentItem = datagrid.SelectedItem;
            updatecells.Clear();
            datagrid.SelectionUnit = DataGridSelectionUnit.FullRow;
            DataGridRow? r = new();
            DependencyObject? dep = (DependencyObject)e.OriginalSource;
            while ((dep != null) && (dep is not DataGridCell))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }
            //if (dep == null) return;
            if (dep is DataGridCell)
            {
                DataGridCell? cell = dep as DataGridCell ?? new();
                cell.Focus();

                while ((dep != null) && (dep is not DataGridRow))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }
                r = dep as DataGridRow ?? new();
                datagrid.SelectedItem = r.DataContext;
            }
            DataRowView? dataRowView = datagrid.CurrentItem as DataRowView;
            DataRow dr = dataRowView!.Row;
            int currentRowIndex = datagrid.Items.IndexOf(datagrid.CurrentItem);
            for (int i = 0; i < Maindatatable.Rows.Count; i++)
            {
                if (Maindatatable.Rows[i].Equals(dr))
                {
                    currentRowIndex = i;
                }
            }
            DataGridRow row = (DataGridRow)datagrid.ItemContainerGenerator
                                              .ContainerFromIndex(currentRowIndex);
            updaterowindex = currentRowIndex;
            //cells = GetCells(row);
            cells = GetCells(currentRowIndex);
            if (isArticle)
            {
                openEditWindow? handler = OpenEditWindowEvent;
                if (OpenEditWindowEvent is not null)
                {
                    OpenEditWindowEvent(cells, custom_columns);
                }
            }
            else if (isStock)
            {
                openEditWindowStock? handler = OpenEditWindowEventStock;
                if (OpenEditWindowEventStock is not null)
                {
                    OpenEditWindowEventStock(cells, custom_columns);
                }
            }
            else if (isType)
            {
                openEditWindowType? handler = OpenEditWindowEventType;
                if (OpenEditWindowEventType is not null)
                {
                    OpenEditWindowEventType(cells, custom_columns);
                }
            }
            else if (isOrder)
            {
                openEditWindowOrder? handler = OpenEditWindowEventOrder;
                if (OpenEditWindowEventOrder is not null)
                {
                    OpenEditWindowEventOrder(cells, custom_columns);
                }
            }
            else if (isOperator)
            {
                openEditWindowOperator? handler = OpenEditWindowEventOperator;
                if (OpenEditWindowEventOperator is not null)
                {
                    OpenEditWindowEventOperator(cells, custom_columns);
                }
            }
            else
            {
                EditData(sender, e);
            }
        }
        //
        private void Show_Article_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Selectedrow is not null)
                {
                    DataRow dr = (DataRow)Selectedrow.Row;
                    ShowArticlesOnThisTrayEvent?.Invoke(Convert.ToInt32(dr["NrPolki"]));
                }
            }
            catch { }

        }
        //Handling manual started order
        private void Start_Orders_M_Click(object sender, RoutedEventArgs e)
        {
            runManual? handler = RunOrdersManualEvent;
            RunOrdersManualEvent?.Invoke(cells, custom_columns);
        }
        //Handling automatic started order
        private void Start_Orders_A_Click(object sender, RoutedEventArgs e)
        {
            runAutomatic? handler = RunOrdersAutomaticEvent;
            RunOrdersAutomaticEvent?.Invoke(cells, custom_columns);
        }
        #endregion

        #region Initializing Functions
        //Automatic function of datagrid made for setting visibility of settings columns. 
        private void Datagrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {

            if (e.Column.Header.ToString()?[0] == '_')
            {
                e.Column.Visibility = Visibility.Collapsed;
            }
            if (e.Column.Header.ToString()?.Length > 3 && e.Column.Header.ToString()?[..3] == "ID_")
            {
                e.Column.Visibility = Visibility.Collapsed;
            }
            if (e.PropertyType == typeof(System.DateTime))
            {
                culturestring = CultureInfo.CurrentCulture.Name;
                culture = CultureInfo.GetCultureInfo(culturestring);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                CultureInfo? cultures = new(culturestring);
                Thread.CurrentThread.CurrentCulture = cultures;
                Thread.CurrentThread.CurrentUICulture = cultures;
                string Formate = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
                string time = CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;
                //(e.Column as DataGridTextColumn).Binding.StringFormat = Formate + " " + time;
                (e.Column as DataGridTextColumn)!.Binding = new Binding(e.PropertyName) { StringFormat = Formate + " " + time, ConverterCulture = culture };
            }
        }
        //Automatic function of datagrid made for set grid settings from database.
        private void Datagrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (Maindatatable.Columns.Contains("_color"))
            {
                //Getting index of _color column
                int _colorcolumnindex = ((DataView)datagrid.ItemsSource).Table!.Columns["_color"]!.Ordinal;
                //Setting color of the row depending on hex value in _color column.
                if (Convert.ToString(((DataRowView)(e.Row.DataContext)).Row.ItemArray[_colorcolumnindex]?.ToString()) != "" && ((DataRowView)(e.Row.DataContext)).Row.ItemArray[_colorcolumnindex]?.ToString()?[0] == '#')
                {
                    string? color = Convert.ToString(((DataRowView)(e.Row.DataContext)).Row.ItemArray[_colorcolumnindex]?.ToString()) ?? Brushes.White.ToString();
                    e.Row.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(color);
                }
            }
            if (Maindatatable.Columns.Contains("_BackColor"))
            {
                //Getting index of _color column
                int _colorcolumnindex = ((DataView)datagrid.ItemsSource).Table!.Columns["_BackColor"]!.Ordinal;
                //Setting color of the row depending on hex value in _color column.
                if (Convert.ToString(((DataRowView)(e.Row.DataContext)).Row.ItemArray[_colorcolumnindex]?.ToString()) != "")
                {
                    string? color = Convert.ToString(((DataRowView)(e.Row.DataContext)).Row.ItemArray[_colorcolumnindex]?.ToString()) ?? Brushes.White.ToString();
                    e.Row.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(color);
                }
            }
        }
        //Function for setting color of toggle button.
        private void Toggle_Loaded(object sender, RoutedEventArgs e)
        {
            Button? bt = sender as Button ?? new();
            string? collumn_name_to_hide = bt.Content.ToString();
            foreach (DataGridColumn c in datagrid.Columns)
            {
                if (c.Header.ToString() == collumn_name_to_hide)
                {
                    if (collumn_name_to_hide?.Length > 3)
                    {
                        if (collumn_name_to_hide[..3] == "ID_")
                        {
                            bt.Visibility = Visibility.Collapsed;
                            //bt.Visibility = Visibility.Visible;
                            //bt.Background = Brushes.DarkGray;
                            //bt.IsEnabled = false;
                            //return;
                        }
                    }
                    bt.Background = c.Visibility == Visibility.Visible ? Brushes.YellowGreen : Brushes.OrangeRed;
                    break;
                }
            }

        }
        //Function for setting color of header when column is filtered.
        private void HedearGrid_Loaded(object sender, RoutedEventArgs e)
        {
            Grid? grid = sender as Grid ?? new();
            ContentPresenter t = FindVisualChild<ContentPresenter>(grid);
            if (Regex.IsMatch(Filtrstring, string.Format(@"\b{0}\b", t.Content)))
            {
                grid.Background = Brushes.YellowGreen; /*(SolidColorBrush?)new BrushConverter().ConvertFrom(this.Resources["datagrid_headersfilter"].ToString() ?? "");*/
                t.Content += Convert.ToString((char)0x27C7);
            }
        }
        #endregion

        #region Datagrid actions
        //Function handling Refreshing data in datagrid
        public void Refreash()
        {
            datagrid.ItemsSource = Maindatatable.DefaultView;
            //Idk why, but this made combobox default select empty. Headers is filled in readsettings().
            ColumnHide_ComboBox.ItemsSource = Headers;
            //Settings from xml file setting up.
            if (File.Exists(string.Format(Maindatatable.TableName + "_Settings.xml")))
            {
                ReadSettings();
            }
            AddRowFilters("");
            ColumnHide_ComboBox.SelectedIndex = 0;
            Headers.Clear();
            string header;
            foreach (DataGridColumn item in datagrid.Columns)
            {
                header = item.Header.ToString() ?? string.Empty;
                if (header[0] != '_' && !header.Contains("ID_"))
                {
                    Headers.Add(header);
                }
            }
            ColumnHide_ComboBox.ItemsSource = Headers;
            ReadSettings();
        }
        public void Load()
        {
            Headers.Clear();
            string header;
            foreach (DataGridColumn item in datagrid.Columns)
            {
                header = item.Header.ToString() ?? string.Empty;
                if (header[0] != '_' && !header.Contains("ID_"))
                {
                    Headers.Add(header);
                }
            }
            ColumnHide_ComboBox.ItemsSource = Headers;
            datagrid.ItemsSource = Maindatatable.DefaultView;
            datagrid.IsSynchronizedWithCurrentItem = false;
            datagrid.Items.Refresh();
            ReadSettings();
            AddRowFilters("");
        }
        //Filing headers list with name of columns in datagrid. Made for handling column hiding
        private void HeaderCombobox()
        {
            ColumnHide_ComboBox.SelectedIndex = 0;
            Headers.Clear();
            for (int i = 0; i < datagrid.Columns.Count; i++)
            {
                Headers.Add("");
            }
            string header;
            foreach (DataGridColumn item in datagrid.Columns)
            {
                header = item.Header.ToString() ?? string.Empty;
                if (header[0] != '_' && !header.Contains("ID_"))
                {
                    //Headers.Insert(item.DisplayIndex, header);
                    Headers[item.DisplayIndex] = header;
                }
            }
            ColumnHide_ComboBox.ItemsSource = Headers;
            ColumnHide_ComboBox.Items.Refresh();
        }
        //Function for enabling save button, when columns order was changed
        private void Datagrid_ColumnHeaderDragCompleted(object sender, DragCompletedEventArgs e)
        {
            Headers.Clear();
            for (int i = 0; i < datagrid.Columns.Count; i++)
            {
                Headers.Add("");
            }
            string header;
            foreach (DataGridColumn item in datagrid.Columns)
            {
                header = item.Header.ToString() ?? string.Empty;
                if (header[0] != '_' && !header.Contains("ID_"))
                {
                    //Headers.Insert(item.DisplayIndex, header);
                    Headers[item.DisplayIndex] = header;
                }
            }
            ColumnHide_ComboBox.ItemsSource = Headers;
            ColumnHide_ComboBox.Items.Refresh();
            WriteSettings();
        }
        //Function for setting save button color, when datagrid property has changed
        private void DataGridColumnHeader_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender.GetType() == typeof(DataGridColumnHeader))
            {
                DataGridColumnHeader column = (DataGridColumnHeader)sender;
                if (column.IsMouseOver == true)
                {
                    WriteSettings();
                    ColumnHide_ComboBox.Items.Refresh();
                }
            }

        }
        //Function handling context menu opened at datagrid headers. 
        private void DataGridColumnHeader_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            DataGridColumnHeader dg = (DataGridColumnHeader)sender;
            bool wasfilter = false;
            bool wasbc = false;
            bool wasb2 = false;
            if (dg.Content.ToString()!.Contains((char)0x27C7))
            {
                dg.Content = dg.Content.ToString()!.Replace(Convert.ToString((char)0x27C7), "");
                wasfilter = true;
            }
            if (dg.Content.ToString()!.Contains((char)0x25BC))
            {
                dg.Content = dg.Content.ToString()!.Replace(Convert.ToString((char)0x25BC), "");
                wasbc = true;
            }
            if (dg.Content.ToString()!.Contains((char)0x25B2))
            {
                dg.Content = dg.Content.ToString()!.Replace(Convert.ToString((char)0x25B2), "");
                wasb2 = true;
            }
            //Saving column names to DataTable
            //DataTable dt = Maindatatable;
            DataView? dv = datagrid.ItemsSource as DataView;
            DataTable dt = dv!.ToTable();
            int counter = 0;
            Brush? color = dg.Background;
            foreach (DataGridColumn column in datagrid.Columns)
            {

                dt.Columns[counter].ColumnName = column.Header.ToString();
                counter++;
            }
            //sw.Stop();
            //Prepering all context menu items for column headers <Edit filter, Delete filter, Add filter, Change name>
            ContextMenu contextMenu = new();
            MenuItem item1 = new();
            MenuItem item2 = new();
            MenuItem item3 = new();
            MenuItem item4 = new();
            MenuItem item5 = new();
            MenuItem item6 = new();
            MenuItem item7 = new();
            MenuItem item8 = new();
            MenuItem item9 = new();
            MenuItem item10 = new();
            MenuItem item11 = new();
            item11.Click += Delete_sorting;
            item11.Click += Delete_Char;
            item11.Header = dictionary["Delete_sorting"] + " " + dg.Content.ToString();
            item10.Click += EditTextFilter;
            item10.Header = dictionary["Edit_filter"] + " " + dg.Content.ToString();
            item9.Click += EditNumberFilter;
            item9.Header = dictionary["Edit_filter"] + " " + dg.Content.ToString();
            item8.Click += EditDateFilter;
            item8.Header = dictionary["Edit_filter"] + " " + dg.Content.ToString();
            item7.Click += DeleteFilter;
            item7.Header = dictionary["Delete_filter"] + " " + dg.Content.ToString();
            item6.Click += DeleteFilter;
            item6.Header = dictionary["Delete_filter"] + " " + dg.Content.ToString();
            item5.Click += DeleteFilter;
            item5.Header = dictionary["Delete_filter"] + " " + dg.Content.ToString();
            item4.Click += TextFilter;
            item4.Header = dictionary["Add_filter"] + " " + dg.Content.ToString();
            item3.Click += Numberfilter;
            item3.Header = dictionary["Add_filter"] + " " + dg.Content.ToString();
            item2.Click += Datefilter;
            item2.Header = dictionary["Add_filter"] + " " + dg.Content.ToString();
            item1.Click += ColumnNameChange;
            item1.Header = dictionary["ColumnChangeName"] + " " + dg.Content.ToString();

            string filterstring = string.Empty;
            void Delete_sorting(object sender, System.EventArgs e)
            {
                int counter1 = 0;
                string filtertodelete = sender.ToString() ?? string.Empty;
                foreach (DataGridColumn column in datagrid.Columns)
                {
                    dt.Columns[counter1].ColumnName = column.Header.ToString();
                    if (dt.Columns[counter1].ColumnName.Contains((char)0x25B2))
                    {
                        dt.Columns[counter1].ColumnName = dt.Columns[counter1].ColumnName.Replace(Convert.ToString((char)0x25B2), "");
                        column.Header = dt.Columns[counter1].ColumnName;
                    }
                    else if (dt.Columns[counter1].ColumnName.Contains((char)0x25BC))
                    {
                        dt.Columns[counter1].ColumnName = dt.Columns[counter1].ColumnName.Replace(Convert.ToString((char)0x25BC), "");
                        column.Header = dt.Columns[counter1].ColumnName;
                    }
                    if (Regex.IsMatch(filtertodelete, string.Format(@"\b{0}\b", dt.Columns[counter1].ColumnName)))
                    {
                        filtertodelete = dt.Columns[counter1].ColumnName;
                        break;
                    }
                    counter1++;
                }
                ICollectionView view = CollectionViewSource.GetDefaultView(datagrid.ItemsSource);
                if (view != null)
                {
                    view.SortDescriptions.Clear();
                    foreach (DataGridColumn column in datagrid.Columns)
                    {
                        if ((string)column.Header == filtertodelete)
                        {
                            column.SortDirection = null;
                            break;
                        }
                    }
                }
                string header = string.Empty;
                foreach (DataGridColumn item in datagrid.Columns)
                {
                    header = item.Header.ToString() ?? string.Empty;
                    if (header.Contains((char)0x25BC) || header.Contains((char)0x25B2))
                    {
                        int itemindex = item.DisplayIndex;
                        datagrid.Columns[itemindex].Header = header.Substring(0, header.Length - 1);
                        string t = header[..(header.Length - 1)];
                        break;
                    }
                }
                datagrid.Items.Refresh();
            }
            void Delete_Char(object sender, EventArgs e)
            {
                wasbc = false;
                wasb2 = false;
            }
            //Saving all filters into one string.
            filterstring = string.Join("", filters);
            string t = dg.Content.ToString() ?? string.Empty;
            //Handling of context menu items visibility depending on datatype, and current filters.
            //MessageBox.Show(filterstring + " : " + dg.Content?.ToString());
            if (!Regex.IsMatch(filterstring, string.Format(@"\b{0}\b", dg.Content?.ToString())))
            {
                contextMenu.Items.Add(item1);
            }
            if (dt.Columns[t]?.DataType == typeof(DateTime))
            {
                if (Regex.IsMatch(filterstring, string.Format(@"\b{0}\b", dg.Content?.ToString())))
                {
                    contextMenu.Items.Add(item7);
                    contextMenu.Items.Add(item8);
                }
                else
                {
                    contextMenu.Items.Add(item2);
                }
            }
            else if (dt.Columns[t]?.DataType == typeof(decimal) || dt.Columns[t]?.DataType == typeof(int))
            {
                if (Regex.IsMatch(filterstring, string.Format(@"\b{0}\b", dg.Content?.ToString())))
                {
                    contextMenu.Items.Add(item6);
                    contextMenu.Items.Add(item9);
                }
                else
                {
                    contextMenu.Items.Add(item3);
                }
            }
            else if (dt.Columns[t]?.DataType == typeof(string))
            {
                if (Regex.IsMatch(filterstring, string.Format(@"\b{0}\b", dg.Content?.ToString())))
                {
                    contextMenu.Items.Add(item5);
                    contextMenu.Items.Add(item10);
                }
                else
                {
                    contextMenu.Items.Add(item4);
                }
            }
            if (dg.SortDirection != null)
            {
                contextMenu.Items.Add(item11);
            }
            contextMenu.IsOpen = true;
            contextMenu.Closed += ContextMenu_Closed;
            void ContextMenu_Closed(object sender, RoutedEventArgs e)
            {
                if (wasfilter)
                {
                    dg.Content += Convert.ToString((char)0x27C7);
                }
                if (wasbc)
                {
                    dg.Content += Convert.ToString((char)0x25BC);
                }
                if (wasb2)
                {
                    dg.Content += Convert.ToString((char)0x25B2);
                }
            }
        }
        //Function for changing column name.
        private void ColumnNameChange(object sender, System.EventArgs e)
        {
            WriteSettings();
            MenuItem? item1 = (MenuItem)sender;
            string header = item1.Header.ToString() ?? string.Empty;
            Window1 window1 = new(header.Replace(dictionary["ColumnChangeName"] + " ", ""), dictionary, dict, Headers, Maindatatable.TableName);
            Opacity = 0.5;
            Effect = blur;
            window1.ShowDialog();
            Opacity = 1;
            Effect = null;
            //WriteSettings();
            window1.Topmost = true;
            window1.Focus();
            ReadSettings();
        }
        //Function for refreshing filtr buttons
        private void RefreshFiltrButtons()
        {
            if (Column_to_filter_Combobox.SelectedItem is null || Column_to_filter_Combobox.SelectedItem.ToString() == "")
            {
                Delete_current_filter_button.IsEnabled = false;
                Add_filter_button.IsEnabled = false;
            }
            else
            {
                string filterstring = string.Join("", filters);
                if (filterstring.Contains(Column_to_filter_Combobox.SelectedItem.ToString()))
                {
                    Delete_current_filter_button.IsEnabled = true;
                    Add_filter_button.IsEnabled = true;
                }
                else
                {
                    Delete_current_filter_button.IsEnabled = false;
                    Add_filter_button.IsEnabled = true;
                }
            }
        }
        //Function for enabling filtr button, when user select item from combobox.
        private void Column_to_filter_Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshFiltrButtons();
        }
        //Function for setting buttons depending on user laws.
        private void SetButtonsDependingOnUserAccess()
        {
            if (isArticle)
            {
                Add_Data.IsEnabled = (bool)_loggedUser.OperatorRights["isArticle"].GetValue(0);
                Edit_Data.IsEnabled = datagrid.SelectedItem is not null ? (bool)_loggedUser.OperatorRights["isArticle"].GetValue(1) : false;
                Delete_Data.IsEnabled = datagrid.SelectedItem is not null ? (bool)_loggedUser.OperatorRights["isArticle"].GetValue(2) : false;
                Get_Data.IsEnabled = datagrid.SelectedItem is not null ? (bool)_loggedUser.OperatorRights["isArticle"].GetValue(0) : false;
                Give_Data.IsEnabled = datagrid.SelectedItem is not null ? (bool)_loggedUser.OperatorRights["isArticle"].GetValue(0) : false;
            }
            else if (isStock)
            {
                Add_Data.IsEnabled = (bool)_loggedUser.OperatorRights["isStock"].GetValue(0);

                Edit_Data.IsEnabled = datagrid.SelectedItem is not null ? (bool)_loggedUser.OperatorRights["isStock"].GetValue(1) : false;
                Delete_Data.IsEnabled = datagrid.SelectedItem is not null ? (bool)_loggedUser.OperatorRights["isStock"].GetValue(2) : false;
                Get_Data.IsEnabled = datagrid.SelectedItem is not null ? (bool)_loggedUser.OperatorRights["isStock"].GetValue(1) : false;
                Give_Data.IsEnabled = datagrid.SelectedItem is not null ? (bool)_loggedUser.OperatorRights["isStock"].GetValue(1) : false;
                Inventory_Data.IsEnabled = datagrid.SelectedItem is not null ? (bool)_loggedUser.OperatorRights["isStock"].GetValue(1) : false;

            }
            else if (isType)
            {
                Add_Data.IsEnabled = (bool)_loggedUser.OperatorRights["isType"].GetValue(0);
                Edit_Data.IsEnabled = datagrid.SelectedItem is not null ? (bool)_loggedUser.OperatorRights["isType"].GetValue(1) : false;
                Delete_Data.IsEnabled = datagrid.SelectedItem is not null ? (bool)_loggedUser.OperatorRights["isType"].GetValue(2) : false;
            }
            else if (isTray)
            {
                // setting below
            }
            else if (isOrder)
            {
                Start_Orders_A.IsEnabled = datagrid.SelectedItem is not null ? (bool)_loggedUser.OperatorRights["isOrder"].GetValue(0) : false;
                Start_Orders_M.IsEnabled = datagrid.SelectedItem is not null ? (bool)_loggedUser.OperatorRights["isOrder"].GetValue(0) : false;
            }
            else if (isTrade)
            {
                //there are no buttons
            }
            else if (isOperator)
            {
                Add_Data.IsEnabled = (bool)_loggedUser.OperatorRights["isOperator"].GetValue(0);
                Edit_Data.IsEnabled = datagrid.SelectedItem is not null ? (bool)_loggedUser.OperatorRights["isOperator"].GetValue(1) : false;
                Delete_Data.IsEnabled = datagrid.SelectedItem is not null ? (bool)_loggedUser.OperatorRights["isOperator"].GetValue(2) : false;
            }
        }
        //Function for enabling edit button, when user select row.
        private void Datagrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

            Selectedrow = (DataRowView)datagrid.SelectedValue;
            SetButtonsDependingOnUserAccess();

            if (datagrid.SelectedItem is not null)
            {
                DataRow dr_event = (DataRow)Selectedrow.Row;

                selectedCellChanged? _handler = SelectedCellChangedEvent;
                SelectedCellChangedEvent?.Invoke(dr_event);
            }
            if (isTray)
            {
                if (datagrid.SelectedItem is not null)
                {
                    DataRowView? dataRowView = datagrid.CurrentItem as DataRowView;
                    DataRow dr = (DataRow)dataRowView.Row;
                    bool boolval = Convert.ToInt32(Convert.ToDouble(dr["Ilosc"])) > 0;

                    Show_Article.IsEnabled = boolval ? (bool)_loggedUser.OperatorRights["isArticle"].GetValue(3) : false;
                    Inventory_Data.IsEnabled = boolval ? (bool)_loggedUser.OperatorRights["isStock"].GetValue(1) : false;
                }
                else
                {
                    Show_Article.IsEnabled = false;
                    Inventory_Data.IsEnabled = false;
                }

            }

            if (isOrder)
            {
                DataRowView? dataRowView = datagrid.CurrentItem as DataRowView;
                if (dataRowView is not null)
                {
                    DataRow dr = dataRowView!.Row;
                    datagrid.CurrentItem = datagrid.SelectedItem;
                    int currentRowIndex = datagrid.Items.IndexOf(datagrid.CurrentItem);
                    for (int i = 0; i < Maindatatable.Rows.Count; i++)
                    {
                        if (Maindatatable.Rows[i].Equals(dr))
                        {
                            currentRowIndex = i;
                        }
                    }
                    DataGridRow row = (DataGridRow)datagrid.ItemContainerGenerator.ContainerFromIndex(currentRowIndex);
                    updaterowindex = currentRowIndex;
                    cells = GetCells(currentRowIndex);
                    if (cells.Count is not 0)
                    {
                        selectedRow? handler = SelectedRowEvent;
                        SelectedRowEvent?.Invoke(cells["NrZlecenia"].Item1.ToString());
                    }
                }
            }
            if (isOrderLine)
            {
                selectedIndex? handler = SelectedIndexEvent;
                SelectedIndexEvent?.Invoke(datagrid.SelectedIndex);
            }
        }
        //Function for set selected row programmatically
        private void Datagrid_CurrentCellChanged(object sender, EventArgs e)
        {
            if (datagrid.CurrentColumn is not null)
            {
                Column_to_filter_Combobox.SelectedItem = datagrid.CurrentColumn.Header.ToString();
            }
        }
        //Function for write settings and reorganize combobox when columns reordered.
        private void Datagrid_ColumnReordered(object sender, DataGridColumnEventArgs e)
        {
            Headers.Clear();
            for (int i = 0; i < datagrid.Columns.Count; i++)
            {
                Headers.Add("");
            }
            string header;
            foreach (DataGridColumn item in datagrid.Columns)
            {
                header = item.Header.ToString() ?? string.Empty;
                if (header[0] != '_' && !header.Contains("ID_"))
                {
                    //Headers.Insert(item.DisplayIndex, header);
                    Headers[item.DisplayIndex] = header;
                }
            }
            ColumnHide_ComboBox.ItemsSource = Headers;
            ColumnHide_ComboBox.Items.Refresh();
            WriteSettings();
        }
        //Function for setting text in combobox. DOESNT WORK
        private void ColumnHide_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ColumnHide_ComboBox.Text = dictionary["Show_hide_column"];
            ColumnHide_ComboBox.SelectedItem = null;
        }
        //Function for get details of doubleclicked row in messagebox. 
        private void DataGridCell_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            int currentRowIndex = datagrid.Items.IndexOf(datagrid.CurrentItem);
            DataGridRow row = (DataGridRow)datagrid.ItemContainerGenerator
                                              .ContainerFromIndex(currentRowIndex);
            string info = string.Empty;
            int counter = 0;
            foreach (DataGridColumn item in datagrid.Columns)
            {
                DataGridCell cell = GetCell(datagrid, row, counter);
                if (cell == null)
                {
                    counter++;
                    continue;
                }
                if (cell.ToString().Contains(':'))
                {
                    info += item.Header.ToString() + ": " + cell.ToString()[(cell.ToString().IndexOf(':') + 1)..] + " \n";
                }
                counter++;
                // ^ uncomment else, when u want to show columns without values.
            }
            HandyControl.Controls.MessageBox.Show(info, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        //Function for add to header arrow char, visualizing sort direction.
        #endregion

        #region Read/Write Settings
        //Function for read settings of datagrid from settings.xml file.
        public void ReadSettings()
        {
            VisibleHeaders.Clear();
            if (File.Exists(string.Format(Maindatatable.TableName + "_Settings.xml")))
            {
                XmlTextReader xmlReader = new(string.Format(Maindatatable.TableName + "_Settings.xml"));
                DataSet dataSet = new("Settings");
                dataSet.ReadXml(xmlReader);
                int counter = 0;
                Headers.Clear();
                string header;
                try
                {
                    custom_columns.Clear();
                    foreach (DataGridColumn c in datagrid.Columns)
                    {
                        try
                        {
                            //custom_columns.Add(c.Header.ToString(), additionaldatatable.Columns[counter].ColumnName.ToString());
                            custom_columns.Add(c.Header.ToString() ?? "", columnnames[counter]);
                        }
                        catch (Exception)
                        {
                            break;
                        }
                        //custom_columns.Add(Maindatatable.Columns[counter].ColumnName.ToString(), c.Header.ToString());
                        c.Header = dataSet.Tables[0].Rows[counter]["Header"];
                        bool succes = double.TryParse((string)dataSet.Tables[0].Rows[counter]["ActualWidth"], out double width);
                        //if (succes && width > 20) c.Width = width;
                        //else c.Width = DataGridLength.Auto;
                        c.Width = width;
                        c.DisplayIndex = Convert.ToInt32(dataSet.Tables[0].Rows[counter]["DisplayIndex"]);
                        c.Visibility = (string)dataSet.Tables[0].Rows[counter]["Visibility"] == "Visible" ? Visibility.Visible : Visibility.Collapsed;
                        counter++;
                        header = c.Header.ToString() ?? string.Empty;
                        //if (c.Visibility == Visibility.Visible)
                        //    VisibleHeaders.Add(c.Header.ToString());
                        if (header[0] != '_' && !header.Contains("ID_"))
                        {
                            Headers.Add(header);
                        }
                    }
                    xmlReader.Close();
                    ColumnHide_ComboBox.ItemsSource = Headers;
                }
                catch (Exception)
                {
                    xmlReader.Close();
                    WriteSettings();
                }
                finally
                {
                    xmlReader.Close();
                }
                xmlReader.Close();
            }
            else
            {
                DataSet dataSet = new("Settings");
                DataTable dataTable = new(datagrid.Name);
                dataTable.Columns.Add(new DataColumn("Header"));
                dataTable.Columns.Add(new DataColumn("ActualWidth"));
                dataTable.Columns.Add(new DataColumn("DisplayIndex"));
                dataTable.Columns.Add(new DataColumn("Visibility"));
                foreach (DataGridColumn c in datagrid.Columns)
                {
                    double width = c.Visibility == Visibility.Visible ? c.ActualWidth : 60;
                    if (c.ActualWidth.Equals(20))
                    {
                        var temp = c.Visibility;
                        c.Visibility = Visibility.Visible;
                        c.Width = DataGridLength.Auto;
                        datagrid.ScrollIntoView(null, c);
                        datagrid.UpdateLayout();
                        width = c.ActualWidth;
                        c.Visibility = temp;
                        datagrid.UpdateLayout();
                        dataTable.Rows.Add(c.Header, width, c.DisplayIndex, c.Visibility);
                        continue;
                    }
                    //c.Width = DataGridLength.Auto;
                    dataTable.Rows.Add(c.Header, Convert.ToInt32(c.ActualWidth), c.DisplayIndex, c.Visibility);
                    //dataTable.Rows.Add(c.Header, width, c.DisplayIndex, c.Visibility);
                }
                XmlWriter xmlWriter = new XmlTextWriter(string.Format(Maindatatable.TableName + "_Settings.xml"), null);
                dataSet.Tables.Add(dataTable);
                dataSet.WriteXml(xmlWriter);
                xmlWriter.Close();
            }
            //Column_to_filter_Combobox.ItemsSource = VisibleHeaders;
        }
        //Public WriteSettings function read 4 properties from datagrid and saving them on xml file.
        public void WriteSettings()
        {
            DataSet dataSet = new("Settings");
            DataTable dataTable = new(datagrid.Name);
            dataTable.Columns.Add(new DataColumn("Header"));
            dataTable.Columns.Add(new DataColumn("ActualWidth"));
            dataTable.Columns.Add(new DataColumn("DisplayIndex"));
            dataTable.Columns.Add(new DataColumn("Visibility"));
            foreach (DataGridColumn c in datagrid.Columns)
            {
                double width = c.ActualWidth;
                if (width == 20)
                {
                    c.Visibility = Visibility.Visible;
                    c.Width = DataGridLength.Auto;
                    datagrid.ScrollIntoView(null, c);
                    datagrid.UpdateLayout();
                    width = c.ActualWidth;
                    c.Visibility = Visibility.Collapsed;
                    datagrid.UpdateLayout();
                    dataTable.Rows.Add(c.Header, width, c.DisplayIndex, c.Visibility);
                    continue;
                }

                dataTable.Rows.Add(c.Header, Convert.ToInt32(c.ActualWidth), c.DisplayIndex, c.Visibility);
                //dataTable.Rows.Add(c.Header, width, c.DisplayIndex, c.Visibility);
            }
            XmlWriter xmlWriter = new XmlTextWriter(string.Format(Maindatatable.TableName + "_Settings.xml"), null);
            dataSet.Tables.Add(dataTable);
            dataSet.WriteXml(xmlWriter);
            xmlWriter.Close();
            ReadSettings();
        }
        #endregion

        #region Filters
        //Function deleting filts from selected column
        private void Delete_current_filter_button_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilter(sender, e, Column_to_filter_Combobox.SelectedItem.ToString());
            Delete_current_filter_button.IsEnabled = false;
        }
        //Function Add filtrs for selected column
        private void Add_filter_button_Click(object sender, RoutedEventArgs e)
        {
            DataView? dv = datagrid.ItemsSource as DataView;
            DataTable dt = dv!.ToTable();
            int counter = 0;
            foreach (DataGridColumn column in datagrid.Columns)
            {
                dt.Columns[counter].ColumnName = column.Header.ToString();
                counter++;
            }
            string filterstring = string.Join("", filters);
            string columntofilter = Column_to_filter_Combobox.SelectedItem.ToString() ?? string.Empty;
            //Handling of context menu items visibility depending on datatype, and current filters.
            //MessageBox.Show(filterstring + " : " + dg.Content?.ToString());

            if (dt.Columns[columntofilter]?.DataType == typeof(DateTime))
            {
                if (Regex.IsMatch(filterstring, string.Format(@"\b{0}\b", Column_to_filter_Combobox.SelectedItem.ToString())))
                {
                    EditDateFilter(sender, e, columntofilter);
                }
                else
                {
                    Datefilter(sender, e, columntofilter);
                }
            }
            else if (dt.Columns[columntofilter]?.DataType == typeof(decimal) || dt.Columns[columntofilter]?.DataType == typeof(int))
            {
                if (Regex.IsMatch(filterstring, string.Format(@"\b{0}\b", Column_to_filter_Combobox.SelectedItem.ToString())))
                {
                    EditNumberFilter(sender, e, columntofilter);
                }
                else
                {
                    Numberfilter(sender, e, columntofilter);
                }
            }
            else if (dt.Columns[columntofilter]?.DataType == typeof(string))
            {
                if (Regex.IsMatch(filterstring, string.Format(@"\b{0}\b", Column_to_filter_Combobox.SelectedItem.ToString())))
                {
                    EditTextFilter(sender, e, columntofilter);
                }
                else
                {
                    TextFilter(sender, e, columntofilter);
                }
            }

            RefreshFiltrButtons();
        }
        //Adding new row filtering function or refreshing filtering when string parameter is empty string. <Can be isolated as second function>
        private void AddRowFilters(string newfilter)
        {
            //27C7
            //condition for filtering data without adding new filter case.

            if (newfilter.Length > 1)
            {
                filters.Add(newfilter);
                Delete_filter_button.IsEnabled = true;
                Delete_filter_button.Background = Brushes.YellowGreen;
            }
            DataTable dt = Maindatatable;
            DataView dv = new(dt);
            datagrid.ItemsSource = Maindatatable.DefaultView;
            //this must be here, to get filters working when column header is changed during one or more filter added before.
            ReadSettings();
            //Preparing filter string for rowfilter.
            Filtrstring = string.Empty;
            foreach (string item in filters)
            {
                //condition for multi-filtering
                if (Filtrstring.Length > 1)
                {
                    Filtrstring += " and ";
                }
                Filtrstring += item;
            }
            if (datagrid.Columns.Count != 0 && dt.Columns.Count != 0)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (!dt.Columns.Contains(datagrid.Columns[i].Header.ToString()))
                    {
                        dt.Columns[i].ColumnName = datagrid.Columns[i].Header.ToString();
                    }
                }
            }
            //Final filtering with all conditions.
            dv.RowFilter = Filtrstring;
            datagrid.ItemsSource = dv;
            //ReadSettings();
            datagrid.Items.Refresh();
            if (File.Exists(string.Format(Maindatatable.TableName + "_Settings.xml")))
            {
                XmlTextReader xmlReader = new(string.Format(Maindatatable.TableName + "_Settings.xml"));
                DataSet dataSet = new("Settings");
                dataSet.ReadXml(xmlReader);
                int counter = 0;
                //Headers.Clear();
                foreach (DataGridColumn c in datagrid.Columns)
                {
                    c.Header = dataSet.Tables[0].Rows[counter]["Header"];
                    c.DisplayIndex = Convert.ToInt32(dataSet.Tables[0].Rows[counter]["DisplayIndex"]);
                    c.Visibility = (string)dataSet.Tables[0].Rows[counter]["Visibility"] == "Visible" ? Visibility.Visible : Visibility.Collapsed;
                    counter++;
                }
                xmlReader.Close();
                ColumnHide_ComboBox.ItemsSource = Headers;
            }
        }
        //Function for deleting filters from data grid view.
        private void DeleteFilter(object sender, System.EventArgs e)
        {
            Filtrstring = string.Empty;
            DataView? dv = datagrid.ItemsSource as DataView;
            DataTable dt = dv!.ToTable();
            string filtertodelete = sender.ToString() ?? string.Empty;
            int counter = 0;
            //Geting column name from filter to delete.
            //Regex.IsMatch(filterstring, String.Format(@"\b{0}\b", dt.Columns[counter].ColumnName));
            foreach (DataGridColumn column in datagrid.Columns)
            {
                dt.Columns[counter].ColumnName = column.Header.ToString();
                if (Regex.IsMatch(filtertodelete, string.Format(@"\b{0}\b", dt.Columns[counter].ColumnName)))
                {
                    filtertodelete = dt.Columns[counter].ColumnName;
                    break;
                }
                counter++;
            }
            //Getting column index.
            int indextodelete = 0;
            foreach (string? item in filters)
            {
                if (item.Contains(filtertodelete))
                {
                    indextodelete = filters.IndexOf(item);
                    break;
                }
            }
            filters.Remove(filters[indextodelete]);
            if (filters.Count < 1)
            {
                Delete_filter_button.Background = Brushes.Transparent;
                Delete_filter_button.IsEnabled = false;
            }
            //Function call to update applied filters on datagrid.
            AddRowFilters("");
            //HandyControl.Controls.Growl.Success(dictionary["Filter_deleted_successfully"]);
        }
        //Function for deleting filters from data grid view.
        private void DeleteFilter(object sender, System.EventArgs e, string columnname)
        {
            Filtrstring = string.Empty;
            DataView? dv = datagrid.ItemsSource as DataView;
            DataTable dt = dv!.ToTable();
            string filtertodelete = columnname;
            int counter = 0;
            //Geting column name from filter to delete.
            //Regex.IsMatch(filterstring, String.Format(@"\b{0}\b", dt.Columns[counter].ColumnName));
            foreach (DataGridColumn column in datagrid.Columns)
            {
                dt.Columns[counter].ColumnName = column.Header.ToString();
                if (Regex.IsMatch(filtertodelete, string.Format(@"\b{0}\b", dt.Columns[counter].ColumnName)))
                {
                    filtertodelete = dt.Columns[counter].ColumnName;
                    break;
                }
                counter++;
            }
            //Getting column index.
            int indextodelete = 0;
            foreach (string? item in filters)
            {
                if (item.Contains(filtertodelete))
                {
                    indextodelete = filters.IndexOf(item);
                    break;
                }
            }
            if (filters.Count != 0)
            {
                filters.Remove(filters[indextodelete]);
                if (filters.Count < 1)
                {
                    Delete_filter_button.Background = Brushes.Transparent;
                    Delete_filter_button.IsEnabled = false;
                }
                //Function call to update applied filters on datagrid.
                AddRowFilters("");
                //HandyControl.Controls.Growl.Success(dictionary["Filter_deleted_successfully"]);
            }
        }
        //Function for editing existing text filter.
        private void EditTextFilter(object sender, System.EventArgs e)
        {
            DataView? dv = datagrid.ItemsSource as DataView;
            DataTable dt = dv!.ToTable();
            int counter = 0;
            Columnnametofilter = sender.ToString() ?? string.Empty;
            foreach (DataGridColumn c in datagrid.Columns)
            {
                dt.Columns[counter].ColumnName = c.Header.ToString();
                if (Regex.IsMatch(Columnnametofilter, string.Format(@"\b{0}\b", dt.Columns[counter].ColumnName)))
                {
                    Columnnametofilter = dt.Columns[counter].ColumnName;
                    break;
                }
                counter++;
            }
            string oldfilter = string.Empty;
            foreach (string? item in filters)
            {
                if (Regex.IsMatch(item, string.Format(@"\b{0}\b", Columnnametofilter)))
                {
                    oldfilter = item;
                    break;
                }
            }
            filters.Remove(oldfilter);
            Window4 window4 = new(dict, Columnnametofilter, oldfilter);
            window4.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            window4.ShowDialog();
            Opacity = 1;
            Effect = null;
            TextColumnFilter();
        }
        //Function for editing existing number filter.
        private void EditNumberFilter(object sender, System.EventArgs e)
        {
            string oldfilter = string.Empty;
            foreach (string? item in filters)
            {
                if (item.Contains(Columnnametofilter!))
                {
                    oldfilter = item;
                    break;
                }
            }
            filters.Remove(oldfilter);
            //Case when user want to edit filter using average. If there was used filter above/below average, datagrid is refreshig to get previous amount of data.
            //If there is used other filter, editing this filter makes that average filter is applied for already filtered data. BUG OR FEATURE?!?!
            if (oldfilter.ToLower().Contains("in"))
            {
                AddRowFilters("");
            }
            DataView? dv = datagrid.ItemsSource as DataView;
            DataTable dt = dv!.ToTable();
            int counter = 0;
            Columnnametofilter = sender.ToString() ?? string.Empty;
            foreach (DataGridColumn c in datagrid.Columns)
            {
                dt.Columns[counter].ColumnName = c.Header.ToString();
                if (Regex.IsMatch(Columnnametofilter, string.Format(@"\b{0}\b", dt.Columns[counter].ColumnName)))
                {
                    Columnnametofilter = dt.Columns[counter].ColumnName;
                    break;
                }
                counter++;
            }
            Window3 window3 = new(dict, dv, Columnnametofilter, dictionary, oldfilter);
            window3.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            window3.ShowDialog();
            Opacity = 1;
            Effect = null;
            NumberColumnFilter();
        }
        //Function for editing existing date filter.
        private void EditDateFilter(object sender, System.EventArgs e)
        {
            DataView? dv = datagrid.ItemsSource as DataView;
            DataTable dt = dv!.ToTable();
            int counter = 0;
            Columnnametofilter = sender.ToString() ?? string.Empty;
            foreach (DataGridColumn c in datagrid.Columns)
            {
                dt.Columns[counter].ColumnName = c.Header.ToString();
                if (Regex.IsMatch(Columnnametofilter, string.Format(@"\b{0}\b", dt.Columns[counter].ColumnName)))
                {
                    Columnnametofilter = dt.Columns[counter].ColumnName;
                    break;
                }
                counter++;
            }
            string oldfilter = string.Empty;
            foreach (string? item in filters)
            {
                if (item.Contains(Columnnametofilter))
                {
                    oldfilter = item;
                    break;
                }
            }
            filters.Remove(oldfilter);
            Window2 window2 = new(dict, Columnnametofilter, oldfilter);
            window2.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            window2.ShowDialog();
            Opacity = 1;
            Effect = null;
            DateColumnFilter();
        }
        //Function for adding filters to columns contains string values.
        private void TextFilter(object sender, System.EventArgs e)
        {
            DataView? dv = datagrid.ItemsSource as DataView;
            DataTable dt = dv!.ToTable();
            int counter = 0;
            Columnnametofilter = sender.ToString() ?? string.Empty;
            string header;
            foreach (DataGridColumn column in datagrid.Columns)
            {
                header = column.Header.ToString() ?? string.Empty;
                if (!Regex.IsMatch(Columnnametofilter, string.Format(@"\b{0}\b", column.Header.ToString())))
                {
                    counter++;
                    continue;
                }
                if (!dt.Columns.Contains(header))
                {
                    dt.Columns[counter].ColumnName = column.Header.ToString();
                }
                string t = Columnnametofilter[Columnnametofilter.IndexOf(dt.Columns[counter].ColumnName)..];
                t = t.Replace(" Items.Count:0", "");
                if (t.Equals(dt.Columns[counter].ColumnName))
                {
                    t = dt.Columns[counter].ColumnName;
                    Columnnametofilter = t;
                    break;
                }
                counter++;
            }
            Window4 window4 = new(dict, Columnnametofilter);
            window4.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            window4.ShowDialog();
            Opacity = 1;
            Effect = null;
            TextColumnFilter();
        }
        //Function for editing existing text filter.
        private void EditTextFilter(object sender, System.EventArgs e, string columnname)
        {
            DataView? dv = datagrid.ItemsSource as DataView;
            DataTable dt = dv!.ToTable();
            int counter = 0;
            Columnnametofilter = columnname;
            foreach (DataGridColumn c in datagrid.Columns)
            {
                dt.Columns[counter].ColumnName = c.Header.ToString();
                if (Regex.IsMatch(Columnnametofilter, string.Format(@"\b{0}\b", dt.Columns[counter].ColumnName)))
                {
                    Columnnametofilter = dt.Columns[counter].ColumnName;
                    break;
                }
                counter++;
            }
            string oldfilter = string.Empty;
            foreach (string? item in filters)
            {
                if (Regex.IsMatch(item, string.Format(@"\b{0}\b", Columnnametofilter)))
                {
                    oldfilter = item;
                    break;
                }
            }
            filters.Remove(oldfilter);
            Window4 window4 = new(dict, Columnnametofilter, oldfilter);
            window4.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            window4.ShowDialog();
            Opacity = 1;
            Effect = null;
            TextColumnFilter();
        }
        //Function for editing existing number filter.
        private void EditNumberFilter(object sender, System.EventArgs e, string columnname)
        {
            string oldfilter = string.Empty;
            foreach (string? item in filters)
            {
                if (item.Contains(Columnnametofilter!))
                {
                    oldfilter = item;
                    break;
                }
            }
            filters.Remove(oldfilter);
            //Case when user want to edit filter using average. If there was used filter above/below average, datagrid is refreshig to get previous amount of data.
            //If there is used other filter, editing this filter makes that average filter is applied for already filtered data. BUG OR FEATURE?!?!
            if (oldfilter.ToLower().Contains("in"))
            {
                AddRowFilters("");
            }
            DataView? dv = datagrid.ItemsSource as DataView;
            DataTable dt = dv!.ToTable();
            int counter = 0;
            Columnnametofilter = columnname;
            foreach (DataGridColumn c in datagrid.Columns)
            {
                dt.Columns[counter].ColumnName = c.Header.ToString();
                if (Regex.IsMatch(Columnnametofilter, string.Format(@"\b{0}\b", dt.Columns[counter].ColumnName)))
                {
                    Columnnametofilter = dt.Columns[counter].ColumnName;
                    break;
                }
                counter++;
            }
            Window3 window3 = new(dict, dv, Columnnametofilter, dictionary, oldfilter);
            window3.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            window3.ShowDialog();
            Opacity = 1;
            Effect = null;
            NumberColumnFilter();
        }
        //Function for editing existing date filter.
        private void EditDateFilter(object sender, System.EventArgs e, string columnname)
        {
            DataView? dv = datagrid.ItemsSource as DataView;
            DataTable dt = dv!.ToTable();
            int counter = 0;
            Columnnametofilter = columnname;
            foreach (DataGridColumn c in datagrid.Columns)
            {
                dt.Columns[counter].ColumnName = c.Header.ToString();
                if (Regex.IsMatch(Columnnametofilter, string.Format(@"\b{0}\b", dt.Columns[counter].ColumnName)))
                {
                    Columnnametofilter = dt.Columns[counter].ColumnName;
                    break;
                }
                counter++;
            }
            string oldfilter = string.Empty;
            foreach (string? item in filters)
            {
                if (item.Contains(Columnnametofilter))
                {
                    oldfilter = item;
                    break;
                }
            }
            filters.Remove(oldfilter);
            Window2 window2 = new(dict, Columnnametofilter, oldfilter);
            window2.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            window2.ShowDialog();
            Opacity = 1;
            Effect = null;
            DateColumnFilter();
        }
        //Function for adding filters to columns contains string values.
        private void TextFilter(object sender, System.EventArgs e, string columnname)
        {
            DataView? dv = datagrid.ItemsSource as DataView;
            DataTable dt = dv!.ToTable();
            int counter = 0;
            Columnnametofilter = columnname;
            string header;
            foreach (DataGridColumn column in datagrid.Columns)
            {
                header = column.Header.ToString() ?? string.Empty;
                if (!Regex.IsMatch(Columnnametofilter, string.Format(@"\b{0}\b", column.Header.ToString())))
                {
                    counter++;
                    continue;
                }
                if (!dt.Columns.Contains(header))
                {
                    dt.Columns[counter].ColumnName = column.Header.ToString();
                }
                string t = Columnnametofilter[Columnnametofilter.IndexOf(dt.Columns[counter].ColumnName)..];
                t = t.Replace(" Items.Count:0", "");
                if (t.Equals(dt.Columns[counter].ColumnName))
                {
                    t = dt.Columns[counter].ColumnName;
                    Columnnametofilter = t;
                    break;
                }
                counter++;
            }
            Window4 window4 = new(dict, Columnnametofilter);
            window4.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            window4.ShowDialog();
            Opacity = 1;
            Effect = null;
            TextColumnFilter();
        }
        // function for add filters to columns containing string values, can be triggered from other windows.
        private void TextColumnFilter()
        {
            if (TextColumnFilterString.Length != 0)
            {
                Delete_filter_button.IsEnabled = true;
                Delete_filter_button.Background = Brushes.YellowGreen;
                AddRowFilters(TextColumnFilterString);
            }
        }
        //Function for adding filters to columns containing integers and decimal values.
        private void Numberfilter(object sender, System.EventArgs e)
        {
            DataView? dv = datagrid.ItemsSource as DataView;
            DataTable dt = dv!.ToTable();
            int counter = 0;
            Columnnametofilter = sender.ToString() ?? string.Empty;
            foreach (DataGridColumn column in datagrid.Columns)
            {
                dt.Columns[counter].ColumnName = column.Header.ToString();
                if (Regex.IsMatch(Columnnametofilter, string.Format(@"\b{0}\b", dt.Columns[counter].ColumnName)))
                {
                    Columnnametofilter = dt.Columns[counter].ColumnName;
                    break;
                }
                counter++;
            }
            Window3 window3 = new(dict, dv, Columnnametofilter, dictionary);
            window3.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            window3.ShowDialog();
            Opacity = 1;
            Effect = null;
            NumberColumnFilter();
        }
        //Function for adding filters to columns containing integers and decimal values.
        private void Numberfilter(object sender, System.EventArgs e, string columnname)
        {
            DataView? dv = datagrid.ItemsSource as DataView;
            DataTable dt = dv!.ToTable();
            int counter = 0;
            Columnnametofilter = columnname;
            foreach (DataGridColumn column in datagrid.Columns)
            {
                dt.Columns[counter].ColumnName = column.Header.ToString();
                if (Regex.IsMatch(Columnnametofilter, string.Format(@"\b{0}\b", dt.Columns[counter].ColumnName)))
                {
                    Columnnametofilter = dt.Columns[counter].ColumnName;
                    break;
                }
                counter++;
            }
            Window3 window3 = new(dict, dv, Columnnametofilter, dictionary);
            window3.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            window3.ShowDialog();
            Opacity = 1;
            Effect = null;
            NumberColumnFilter();
        }
        //Public function for adding filters to columns containing numberic values, can be triggered from other windows.
        private void NumberColumnFilter()
        {
            AddRowFilters(NumberColumnFilterString);
        }
        //Function for adding filters to columns containing DateTime values.
        private void Datefilter(object sender, System.EventArgs e)
        {
            Columnnametofilter = sender.ToString() ?? string.Empty;
            DataView? dvv = datagrid.ItemsSource as DataView;
            DataTable dt = dvv!.ToTable();
            int counter = 0;
            foreach (DataGridColumn column in datagrid.Columns)
            {
                dt.Columns[counter].ColumnName = column.Header.ToString();
                if (Regex.IsMatch(Columnnametofilter, string.Format(@"\b{0}\b", dt.Columns[counter].ColumnName)))
                {
                    Columnnametofilter = dt.Columns[counter].ColumnName;
                    break;
                }
                counter++;
            }
            Window2 window2 = new(dict, Columnnametofilter);
            window2.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            window2.ShowDialog();
            Opacity = 1;
            Effect = null;
            DateColumnFilter();
        }
        //Function for adding filters to columns containing DateTime values.
        private void Datefilter(object sender, System.EventArgs e, string columnname)
        {
            Columnnametofilter = columnname;
            DataView? dvv = datagrid.ItemsSource as DataView;
            DataTable dt = dvv!.ToTable();
            int counter = 0;
            foreach (DataGridColumn column in datagrid.Columns)
            {
                dt.Columns[counter].ColumnName = column.Header.ToString();
                if (Regex.IsMatch(Columnnametofilter, string.Format(@"\b{0}\b", dt.Columns[counter].ColumnName)))
                {
                    Columnnametofilter = dt.Columns[counter].ColumnName;
                    break;
                }
                counter++;
            }
            Window2 window2 = new(dict, Columnnametofilter);
            window2.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            window2.ShowDialog();
            Opacity = 1;
            Effect = null;
            DateColumnFilter();
        }
        //function for adding filters to columns containing DateTime values, can be triggered from other windows.
        private void DateColumnFilter()
        {
            if (DateColumnFilterString.Length != 0)
            {
                DataView? dvv = datagrid.ItemsSource as DataView;
                DataTable dt = dvv!.ToTable();
                int counter = 0;
                foreach (DataGridColumn column in datagrid.Columns)
                {
                    dt.Columns[counter].ColumnName = column.Header.ToString();
                    if (Regex.IsMatch(Columnnametofilter!, string.Format(@"\b{0}\b", dt.Columns[counter].ColumnName)))
                    {
                        Columnnametofilter = dt.Columns[counter].ColumnName;
                        break;
                    }
                    counter++;
                }
                AddRowFilters("[" + Columnnametofilter + "]" + DateColumnFilterString);
            }
        }
        #endregion

        #region Auxiliary functions
        //Get Collection of cells with column names and data types. Use when wan to get all cells from all columns.
        private Dictionary<string, Tuple<string, Type>> GetCells(int rowindex)
        {
            //Dictionary<string, Tuple<string, Type>> Cells = new();
            //if (rowindex is not -1)
            //{

            //    for (int i = 0; i < Maindatatable.Columns.Count; i++)
            //    {
            //        if (custom_columns.ContainsKey(Maindatatable.Columns[i].ColumnName.ToString()))
            //        {
            //            if (Maindatatable.Columns[i].DataType == typeof(DateTime))
            //            {
            //                DateTime dt = (DateTime)Maindatatable.Rows[rowindex][i];
            //                //Cells.Add(Maindatatable.Columns[i].ColumnName.ToString(), Tuple.Create(dt.ToString("MM/dd/yyyy hh:mm:ss tt"), Maindatatable.Columns[i].DataType));
            //                Cells.Add(custom_columns[Maindatatable.Columns[i].ColumnName.ToString()], Tuple.Create(dt.ToString("MM/dd/yyyy hh:mm:ss tt"), Maindatatable.Columns[i].DataType));
            //                continue;
            //            }
            //            //Cells.Add(Maindatatable.Columns[i].ColumnName.ToString(), Tuple.Create(Maindatatable.Rows[rowindex][i].ToString()!, Maindatatable.Columns[i].DataType));
            //            Cells.Add(custom_columns[Maindatatable.Columns[i].ColumnName.ToString()], Tuple.Create(Maindatatable.Rows[rowindex][i].ToString()!, Maindatatable.Columns[i].DataType));
            //        }
            //        else
            //        {
            //            if (Maindatatable.Columns[i].DataType == typeof(DateTime))
            //            {
            //                DateTime dt = (DateTime)Maindatatable.Rows[rowindex][i];
            //                Cells.Add(Maindatatable.Columns[i].ColumnName.ToString(), Tuple.Create(dt.ToString("MM/dd/yyyy hh:mm:ss tt"), Maindatatable.Columns[i].DataType));
            //                continue;
            //            }
            //            Cells.Add(Maindatatable.Columns[i].ColumnName.ToString(), Tuple.Create(Maindatatable.Rows[rowindex][i].ToString()!, Maindatatable.Columns[i].DataType));
            //        }
            //    }
            //}
            //return Cells;
            Dictionary<string, Tuple<string, Type>> Cells = new();
            if (rowindex is not -1)
            {

                for (int i = 0; i < Maindatatable.Columns.Count; i++)
                {
                    if (custom_columns.ContainsKey(Maindatatable.Columns[i].ColumnName.ToString()))
                    {
                        if (Maindatatable.Columns[i].DataType == typeof(DateTime))
                        {
                            DateTime dt = (DateTime)Maindatatable.Rows[rowindex][i];
                            //Cells.Add(Maindatatable.Columns[i].ColumnName.ToString(), Tuple.Create(dt.ToString("MM/dd/yyyy hh:mm:ss tt"), Maindatatable.Columns[i].DataType));
                            Cells.Add(custom_columns[Maindatatable.Columns[i].ColumnName.ToString()], Tuple.Create(dt.ToString("MM/dd/yyyy hh:mm:ss tt"), Maindatatable.Columns[i].DataType));
                            continue;
                        }
                        //Cells.Add(Maindatatable.Columns[i].ColumnName.ToString(), Tuple.Create(Maindatatable.Rows[rowindex][i].ToString()!, Maindatatable.Columns[i].DataType));
                        Cells.Add(custom_columns[Maindatatable.Columns[i].ColumnName.ToString()], Tuple.Create(Maindatatable.Rows[rowindex][i].ToString()!, Maindatatable.Columns[i].DataType));
                    }
                    else
                    {
                        if (Maindatatable.Columns[i].DataType == typeof(DateTime))
                        {
                            DateTime dt = (DateTime)Maindatatable.Rows[rowindex][i];
                            Cells.Add(Maindatatable.Columns[i].ColumnName.ToString(), Tuple.Create(dt.ToString("MM/dd/yyyy hh:mm:ss tt"), Maindatatable.Columns[i].DataType));
                            continue;
                        }
                        Cells.Add(Maindatatable.Columns[i].ColumnName.ToString(), Tuple.Create(Maindatatable.Rows[rowindex][i].ToString()!, Maindatatable.Columns[i].DataType));
                    }
                }
            }
            return Cells;
        }
        private void Window1_Closing(object sender, CancelEventArgs e)
        {
            if (Isnamechanging == true)
            {
                ReadSettings();
            }
        }
        //Function for calling headers function from main window
        public void CallHeaders()
        {
            HeaderCombobox();
        }
        //Function for getting cell from container. Necessary for keeping virtualization turned on. 
        private static DataGridCell GetCell(DataGrid dataGrid, DataGridRow rowContainer, int column)
        {
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                if (presenter == null)
                {
                    /* if the row has been virtualized away, call its ApplyTemplate() method 
                     * to build its visual tree in order for the DataGridCellsPresenter
                     * and the DataGridCells to be created */
                    rowContainer.ApplyTemplate();
                    presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                }
                if (presenter != null)
                {
                    DataGridCell? cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                    if (cell == null)
                    {
                        /* bring the column into view
                         * in case it has been virtualized away */
                        dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
                        cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    }
                    return cell ?? new();
                }
            }
            return null;
        }
        //function for finding child in visual tree
        private static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is not null && child is T t)
                {
                    return t;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child!);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }
        //Minimalizing main window and opening exported xlsx file
        private void OpenWithExcel(string file)
        {
            //process.Kill();
            Process proc = Process.Start(@"cmd.exe ", String.Format(@"/c " + file));
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }
        //Function calling window for editing data in selected row
        private void EditData(object sender, System.EventArgs e)
        {
            AddEdit edit = new(cells, dict, dictionary, true);
            edit.Title = dictionary["EditData"];
            edit.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            edit.ShowDialog();
            Opacity = 1;
            Effect = null;
            if (updatecells.Count != 0)
            {
                List<string>? list = updatecells.Keys.ToList<string>();
                for (int i = 0; i < Maindatatable.Columns.Count; i++)
                {
                    DataColumn column = Maindatatable.Columns[list[i]] ?? new();
                    if (column.DataType == typeof(string))
                    {
                        Maindatatable.Rows[updaterowindex][list[i]] = updatecells[list[i]].Item1;
                    }
                    else if (column.DataType == typeof(DateTime) && DateTime.TryParse(updatecells[list[i]].Item1, out DateTime dt))
                    {
                        //DateTime.TryParse(updatecells[list[i]].Item1, out dt);
                        Maindatatable.Rows[updaterowindex][list[i]] = dt;
                    }
                    else if ((column.DataType == typeof(decimal) || column.DataType == typeof(double)) && decimal.TryParse(updatecells[list[i]].Item1, out decimal dec))
                    {
                        //decimal.TryParse(updatecells[list[i]].Item1, out dec);
                        Maindatatable.Rows[updaterowindex][list[i]] = dec;
                    }
                    else if (column.DataType == typeof(bool) && bool.TryParse(updatecells[list[i]].Item1, out bool bo))
                    {
                        //bool.TryParse(updatecells[list[i]].Item1, out bo);
                        Maindatatable.Rows[updaterowindex][list[i]] = bo;
                    }
                    else if (column.DataType == typeof(int) && int.TryParse(updatecells[list[i]].Item1, out int integ))
                    {
                        //int.TryParse(updatecells[list[i]].Item1, out integ);
                        Maindatatable.Rows[updaterowindex][list[i]] = integ;
                    }
                }
                datagrid.ItemsSource = Maindatatable.DefaultView;
                datagrid.Items.Refresh();
                //AddRowFilters("");
            }
        }
        //Function for calling window for adding new data to data table
        private void AddData(object sender, System.EventArgs e)
        {
            AddEdit add = new(cells, dict, dictionary, false);
            add.Title = dictionary["AddData"];
            add.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            add.ShowDialog();
            Opacity = 1;
            Effect = null;
            DataRow row;
            row = Maindatatable.NewRow();
            if (updatecells.Count != 0)
            {
                List<string>? list = updatecells.Keys.ToList<string>();
                for (int i = 0; i < Maindatatable.Columns.Count; i++)
                {
                    DataColumn column = Maindatatable.Columns[list[i]] ?? new();
                    if (column.DataType == typeof(string))
                    {
                        row[list[i]] = updatecells[list[i]].Item1;
                    }
                    else if (column.DataType == typeof(DateTime) && DateTime.TryParse(updatecells[list[i]].Item1, out DateTime dt))
                    {
                        //DateTime.TryParse(updatecells[list[i]].Item1, out dt);
                        row[list[i]] = dt;
                    }
                    else if ((column.DataType == typeof(decimal) || column.DataType == typeof(double)) && decimal.TryParse(updatecells[list[i]].Item1, out decimal dec))
                    {
                        //decimal.TryParse(updatecells[list[i]].Item1, out dec);
                        row[list[i]] = dec;
                    }
                    else if (column.DataType == typeof(bool) && bool.TryParse(updatecells[list[i]].Item1, out bool bo))
                    {
                        //bool.TryParse(updatecells[list[i]].Item1, out bo);
                        row[list[i]] = bo;
                    }
                    else if (column.DataType == typeof(int) && int.TryParse(updatecells[list[i]].Item1, out int integ))
                    {
                        //int.TryParse(updatecells[list[i]].Item1, out integ);
                        row[list[i]] = integ;
                    }
                }
                Maindatatable.Rows.Add(row);
                datagrid.ItemsSource = Maindatatable.DefaultView;
                //AddRowFilters("");
                datagrid.Items.Refresh();
            }
        }
        //Function for making place for show/hide categories/species
        public void MakePlaceForShowHideButton(bool val)
        {
            if (val)
            {
                LoadDatabase.Margin = new Thickness(55, 0, 0, 0);
                div1.Margin = new Thickness(107, 0, 0, 0);
                Get_Data.Margin = new Thickness(110, 0, 0, 0);
                Give_Data.Margin = new Thickness(195, 0, 0, 0);
                Inventory_Data.Margin = new Thickness(280, 0, 0, 0);
                div2.Margin = new Thickness(417, 0, 0, 0);
            }
            else
            {
                LoadDatabase.Margin = new Thickness(0, 0, 0, 0);
                div1.Margin = new Thickness(52, 0, 0, 0);
                Get_Data.Margin = new Thickness(55, 0, 0, 0);
                Give_Data.Margin = new Thickness(140, 0, 0, 0);
                Inventory_Data.Margin = new Thickness(225, 0, 0, 0);
                div2.Margin = new Thickness(362, 0, 0, 0);
            }
        }
        //Function for moving grid from left margin to make space for other control
        public void MoveMargin(bool withAnimation)
        {
            if (withAnimation)
            {
                ThicknessAnimation da = new()
                {
                    From = new Thickness(0, 0, 0, 0),
                    To = new Thickness(192, 0, 0, 0),
                    Duration = TimeSpan.FromSeconds(0.4)
                };
                datagrid.BeginAnimation(MarginProperty, da);
            }
            else
                datagrid.Margin = new Thickness(192, 0, 0, 0);


        }
        //Function for setting grid margins default
        public void SetMargin()
        {
            if (datagrid.Margin == new Thickness(192, 0, 0, 0))
            {


                ThicknessAnimation da = new()
                {
                    From = new Thickness(192, 0, 0, 0),
                    To = new Thickness(0, 0, 0, 0),
                    Duration = TimeSpan.FromSeconds(0.4)
                };
                datagrid.BeginAnimation(MarginProperty, da);
            }
            if (isOrder)
            {
                datagrid.Margin = new Thickness(2, 6, 0, 0);
            }

        }
        public void GetCells()
        {
            datagrid.CurrentItem = datagrid.SelectedItem;
            updatecells.Clear();
            datagrid.SelectionUnit = DataGridSelectionUnit.FullRow;
            DataRowView? dataRowView = datagrid.CurrentItem as DataRowView;
            DataRow dr = dataRowView!.Row;
            int currentRowIndex = datagrid.Items.IndexOf(datagrid.CurrentItem);
            for (int i = 0; i < Maindatatable.Rows.Count; i++)
            {
                if (Maindatatable.Rows[i].Equals(dr))
                {
                    currentRowIndex = i;
                }
            }
            DataGridRow row = (DataGridRow)datagrid.ItemContainerGenerator
                                              .ContainerFromIndex(currentRowIndex);
            updaterowindex = currentRowIndex;
            //cells = GetCells(row);
            cells = GetCells(currentRowIndex);
        }
        public void Hide_Menu()
        {
            Datagrid_menu.Visibility = Visibility.Collapsed;
            DataGridControl_Grid.RowDefinitions[0].Height = GridLength.Auto;
        }
        public void Show_Menu()
        {
            Datagrid_menu.Visibility = Visibility.Visible;
            DataGridControl_Grid.RowDefinitions[0].MinHeight = 50;
        }
        public void HideGetGiveButtonForSelect_an_article()
        {
            Get_Data.Visibility = Visibility.Hidden;
            Give_Data.Visibility = Visibility.Hidden;
        }
        public void ScrollIntoViewSelectedRow()
        {
            if (datagrid.SelectedItem is not null)
            {
                datagrid.ScrollIntoView(datagrid.SelectedItem);
            }
        }
        public void Change_SelectionMode(DataGridSelectionMode mode)
        {
            datagrid.SelectionMode = mode;
        }
        #endregion

        #endregion

        public void SelectedItemByIndex(int val)
        {
            datagrid.SelectedIndex = val;
        }

        //Public function for changing language
        public void Language_Changed(ResourceDictionary dict)
        {
            //dict.Source = diction.Source;
            Resources.MergedDictionaries.Add(dict);
            keys = dict.Keys.Cast<string>().ToList();
            values = dict.Values.Cast<string>().ToList();
            dictionary = keys.Zip(values, (k, v) => new { Key = k, Value = v })
                     .ToDictionary(x => x.Key, x => x.Value);
            if (culturestring == "pl-PL")
            {
                culturestring = "en-EN";
            }
            else
            {
                culturestring = "pl-PL";
            }
            datagrid.Items.Refresh();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateAvaiableColumnsForFiltering();
        }
    }
}