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
    public partial class Tray_Selection_Give : BTPControlLibrary.MainWindow
    {
        #region Fields
        private readonly BlurEffect blur = new();
        public ResourceDictionary dict = new();
        public Dictionary<string, string> dictionary;

        private readonly DataTable polki_dimensions = Connection("Kar_Polki");

        private readonly Dictionary<string, string> custom_columns;
        private readonly Dictionary<string, Tuple<string, Type>> cells = new();
        DataRow[] dr_array;
        DataRow? dr;
        private bool isFromListTray;
        private bool isEmptyTray;
        private bool isWrongTray;
        #endregion

        //Constructor 
        public Tray_Selection_Give(ResourceDictionary resourceDictionary, Dictionary<string, string> lang, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns)
        {
            InitializeComponent();
            dg.Hide_Menu();
            dg.SelectedCellChangedEvent += DG_SelectionChanged;

            this.cells = cells;
            this.custom_columns = custom_columns;
            dictionary = lang;
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);

            GenerateTraysWithThisArticleList();
        }
        public static DataTable Connection(string db)
        {
            string connectionString = _cfg.DBWMS_Remote_new;
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
        public override void InputInterface(string name, object value)
        {
            base.InputInterface(name, value);
            switch (name)
            {
                case "TRAY_IN_WINDOW":
                    short[] v = (short[])value;
                    SetButtonTrayFromWindow(v[_plc.WindowNr]);
                    break;
            }
        }
        private void DG_SelectionChanged(DataRow dr_event)
        {
            if (dr_event is not null)
            {
                dr = dr_event;
                ID_Tray.SelectedItem = dr["ID_Polki"].ToString();
                ID_Localization.SelectedItem = dr["Symbol"].ToString();
            }
            else
                dg.Selectedrow = null;
        }
        private void GenerateTraysWithThisArticleList()
        {
            dr_array = (DataRow[])_dbWMS.Add_StanyMagazynowe_Filter.GetData("000", "000").Select("Indeks = '" + cells["Indeks"].Item1.ToString() + "'");
            DataTable dt = new DataTable();
            List<string> list = new();
            foreach (DataColumn item in dt.Columns)
            {
                list.Add(item.ColumnName.ToString());
            }
            if (dr_array.Length > 0)
                dt = dr_array.CopyToDataTable();
            dg.Maindatatable = dt;
            dg.Load();
            dg.columnnames = list;

            List<string> avaliableTrayList = new List<string>();

            //generowanie półek do combobox
            foreach (DataRow i in _dbWMS.Add_Polki_HMI_Select.GetData(true).Rows)
            {
                avaliableTrayList.Add(i[0].ToString());
            }
            foreach (DataRow i in dt.Rows)
            {
                avaliableTrayList.Add(i[1].ToString());
            }
            avaliableTrayList.Sort((a, b) => a.CompareTo(b));
            ID_Tray.ItemsSource = avaliableTrayList;
        }
        private void SetButtonTrayFromWindow(short tray)
        {
            if (tray != 0)
            {
                bt_trayfromwindow.IsEnabled = true;
                bt_trayfromwindow.Content = tray.ToString();
            }
            else
            {
                bt_trayfromwindow.IsEnabled = false;
                bt_trayfromwindow.Content = "BRAK";
            }
        }
        private void CheckSelectedTray()
        {

            try
            {
                DataTable dt = _dbWMS.Add_StanyMagazynowe_Filter.GetData("000", "000").Select("ID_Polki = " + ID_Tray.SelectedItem.ToString()).CopyToDataTable();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["Indeks"].ToString() == cells["Indeks"].Item1.ToString()) // z dobrym materiałem ale tylko pierwsza lok.???
                        {
                            dr = row;
                            isFromListTray = true;
                            break;
                        }
                        else // z innym materiałem
                        {
                            isWrongTray = true;
                            break;
                        }
                    }
                }
            }
            catch { isEmptyTray = true; }

        }
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            CheckSelectedTray();
            if (Convert.ToInt32(ID_Tray.SelectedItem.ToString()) < Int16.MaxValue)
            {
                if (ID_Tray.SelectedValue is not null && ID_Localization.SelectedValue is not null)
                {
                    if (isFromListTray)
                    {
                        //background.WRITE_TrayTransport(Convert.ToInt16(ID_Tray.SelectedItem.ToString()), Convert.ToInt16(background._window));
                        Give_A_AnP give_Article = new Give_A_AnP(dict, dictionary, cells, custom_columns, dr, true);
                        give_Article.Owner = Window.GetWindow(this);
                        Opacity = 0.5;
                        Effect = blur;
                        give_Article.ShowDialog();
                        Opacity = 1;
                        Effect = null;
                        isFromListTray = false;
                        this.Close();

                    }
                    else if (isEmptyTray)
                    {
                        //background.WRITE_TrayTransport(Convert.ToInt16(ID_Tray.SelectedItem.ToString()), Convert.ToInt16(background._window));
                        Give_A_AnP give_Article = new Give_A_AnP(dict, dictionary, cells, custom_columns, ID_Tray.SelectedItem.ToString(), ID_Localization.SelectedItem.ToString(), false);
                        give_Article.Owner = Window.GetWindow(this);
                        Opacity = 0.5;
                        Effect = blur;
                        try
                        {
                            give_Article.ShowDialog();
                            Opacity = 1;
                            Effect = null;
                            this.Close();
                        }
                        catch
                        {
                            Opacity = 1;
                            Effect = null;
                        }
                        isEmptyTray = false;
                    }
                    else if (isWrongTray)
                    {
                        HandyControl.Controls.MessageBox.Show("Wybrana półka posiada juz inny artykuł", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        isWrongTray = false;
                    }
                }
                else
                    HandyControl.Controls.MessageBox.Show(dictionary["WrongTrayNumber"], "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
                HandyControl.Controls.MessageBox.Show(dictionary["WrongTray_LagrerThenInt16"], "Error", MessageBoxButton.OK, MessageBoxImage.Warning);


        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Find_Empty_Tray_Click(object sender, RoutedEventArgs e)
        {
            ID_Tray.Text = _dbWMS.Add_Polki_HMI_Select.GetData(true)[0][0].ToString();
            DatagridFocusLost();
        }
        private void Empty_Tray_From_Window_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ID_Tray.SelectedItem = bt_trayfromwindow.Content.ToString();
                DatagridFocusLost();
            }
            catch 
            {
                HandyControl.Controls.MessageBox.Show(dictionary["WrongTrayForThisArticle"]);
            }

        }
        private void DatagridFocusLost()
        {
            try
            {
                dg.SelectedItemByIndex(-1);
            }
            catch { }

        }
        private void NKB_GotFocus(object sender, RoutedEventArgs e)
        {
            ////dg.Selectedrow = null;
            //TextBox tb = sender as TextBox;
            //nkb.FocusedTextBox = tb;
            //if (kb.Visibility == Visibility.Visible)
            //    kb.Hide();

        }
        private void NKB_TextChanged(object sender, TextChangedEventArgs e)
        {
            //TextBox tb = sender as TextBox;
            //List<string> nkb_char = new() { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "," };

            //if (tb != null)
            //{
            //    tb.BorderThickness = new Thickness(0, 0, 0, 0);
            //    int start_index = (tb.Text.Length - 1);
            //    if (start_index < 0)
            //        return;
            //    if (!nkb_char.Contains(tb.Text.Substring(start_index)))
            //    {
            //        nkb.Backspace();
            //        e.Handled = true;

            //        ColorAnimation flash = new(Colors.White, Colors.Red, new Duration(TimeSpan.FromMilliseconds(75D))) { AutoReverse = true, RepeatBehavior = new RepeatBehavior(3D) };
            //        tb.Background = new SolidColorBrush(Colors.White);
            //        tb.Background.BeginAnimation(SolidColorBrush.ColorProperty, flash);

            //        //HandyControl.Controls.Growl.Warning(dictionary["Integer_validation"]);
            //    }

            //}
        }

        #region LOCATION AUTO CREATOR
        private void FillCanvas(List<ArticlesControl.Klasy.Localization> list)
        {
            Localization_Visualizer.Children.Clear();

            foreach (ArticlesControl.Klasy.Localization? item in list)
            {
                Button btn = new();
                btn.Click += Btn_Click;
                btn.Focusable = false;
                btn.BorderThickness = new Thickness(2, 2, 2, 2);
                btn.BorderBrush = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["datagrid_headers"].ToString() ?? String.Empty);
                btn.Content = item.Symbol.ToString();
                btn.Width = (double)(((item.WymiarX * 1.00) / (item.PolkaX * 1.00)) * localization_grid.Width);
                btn.Height = (double)((item.WymiarY * 1.00 / item.PolkaY * 1.00) * localization_grid.Height);
                Canvas.SetLeft(btn, ((double)(item.PozX * 1.00 / item.PolkaX) * localization_grid.Width));
                Canvas.SetTop(btn, ((double)(item.PozY * 1.00 / item.PolkaY * 1.00) * localization_grid.Height));
                Localization_Visualizer.Children.Add(btn);
                //if (isEdit)
                //{
                //    btn.IsEnabled = false;
                //}
            }
        }
        private void FillCanvas(List<ArticlesControl.Klasy.Localization> list, string selected)
        {
            Localization_Visualizer.Children.Clear();

            foreach (ArticlesControl.Klasy.Localization? item in list)
            {//datagrid_headers
                Button btn = new();
                btn.Click += Btn_Click;
                btn.Focusable = false;
                btn.BorderThickness = new Thickness(2, 2, 2, 2);
                btn.BorderBrush = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["datagrid_headers"].ToString() ?? String.Empty);
                btn.Content = item.Symbol.ToString();
                if (btn.Content.ToString() == selected)
                {
                    btn.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["App_Background"].ToString() ?? String.Empty);
                }
                btn.Width = (double)(((item.WymiarX * 1.00) / (item.PolkaX * 1.00)) * localization_grid.Width);
                btn.Height = (double)((item.WymiarY * 1.00 / item.PolkaY * 1.00) * localization_grid.Height);
                Canvas.SetLeft(btn, ((double)(item.PozX * 1.00 / item.PolkaX) * localization_grid.Width));
                Canvas.SetTop(btn, ((double)(item.PozY * 1.00 / item.PolkaY * 1.00) * localization_grid.Height));
                Localization_Visualizer.Children.Add(btn);
                //if (isEdit)
                //{
                //    btn.IsEnabled = false;
                //}
            }
        }
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            foreach (Button item in Localization_Visualizer.Children)
            {
                item.Background = Brushes.White;
            }
            btn.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["App_Background"].ToString() ?? "#000000");
            ID_Localization.SelectedItem = btn.Content.ToString();

        }
        private void ID_Tray_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ID_Localization.ItemsSource = null;
            List<ArticlesControl.Klasy.Localization> list1 = new();
            List<string> combosource = new();
            DataTable data = new();
            int xpolki;
            int ypolki;

            data = _dbWMS.Add_Lokalizacje_Select.GetData().Select("ID_Polka = '" + ID_Tray.SelectedItem.ToString() + "'").CopyToDataTable();
            _ = int.TryParse(polki_dimensions.Select("NrPolki = '" + ID_Tray.SelectedItem.ToString() + "'").CopyToDataTable().Rows[0]["Dlugosc_def"].ToString(), out xpolki);
            _ = int.TryParse(polki_dimensions.Select("NrPolki = '" + ID_Tray.SelectedItem.ToString() + "'").CopyToDataTable().Rows[0]["Szerokosc_def"].ToString(), out ypolki);

            foreach (DataRow item in data.Rows)
            {
                combosource.Add(item["Symbol"].ToString());
                list1.Add(new ArticlesControl.Klasy.Localization(Convert.ToInt32(item["ID_Lokalizacji"].ToString()), item["Symbol"].ToString(), Convert.ToInt32(item["WymiarX"].ToString()), Convert.ToInt32(item["WymiarY"].ToString()), Convert.ToInt32(item["WymiarZ"].ToString()), Convert.ToInt32(item["PozX"].ToString()), Convert.ToInt32(item["PozY"].ToString()), xpolki, ypolki));
            }
            ID_Localization.ItemsSource = combosource;
            ID_Localization.SelectedIndex = -1;
            FillCanvas(list1);


            Submit.IsEnabled = ID_Tray.SelectedIndex != -1 && ID_Localization.SelectedIndex != -1;

        }
        private void ID_Localization_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<ArticlesControl.Klasy.Localization> list1 = new();
            DataTable data = new();
            int xpolki;
            int ypolki;
            data = _dbWMS.Add_Lokalizacje_Select.GetData().Select("ID_Polka = '" + ID_Tray.SelectedValue.ToString() + "'").CopyToDataTable();
            _ = int.TryParse(polki_dimensions.Select("NrPolki = '" + ID_Tray.SelectedValue.ToString() + "'").CopyToDataTable().Rows[0]["Dlugosc_def"].ToString(), out xpolki);
            _ = int.TryParse(polki_dimensions.Select("NrPolki = '" + ID_Tray.SelectedValue.ToString() + "'").CopyToDataTable().Rows[0]["Szerokosc_def"].ToString(), out ypolki);


            foreach (DataRow item in data.Rows)
            {
                list1.Add(new ArticlesControl.Klasy.Localization(Convert.ToInt32(item["ID_Lokalizacji"].ToString()), item["Symbol"].ToString(), Convert.ToInt32(item["WymiarX"].ToString()), Convert.ToInt32(item["WymiarY"].ToString()), Convert.ToInt32(item["WymiarZ"].ToString()), Convert.ToInt32(item["PozX"].ToString()), Convert.ToInt32(item["PozY"].ToString()), xpolki, ypolki));
            }
            if (ID_Localization.SelectedValue is null)
            {
                ID_Localization.SelectedIndex = -1;
                //FillCanvas(list1, ID_Localization.SelectedValue.ToString() ?? string.Empty);
            }
            else
            {
                FillCanvas(list1, ID_Localization.SelectedValue.ToString() ?? string.Empty);
            }

            Submit.IsEnabled = ID_Tray.SelectedIndex != -1 && ID_Localization.SelectedIndex != -1;
        }
        #endregion
        private void dg_LostFocus(object sender, RoutedEventArgs e)
        {
            dg.SelectedItemByIndex(-1);
        }
    }
}
