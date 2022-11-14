using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;


namespace ArticlesControl.Formatki
{
    /// <summary>
    /// Logika interakcji
    /// </summary>
    public partial class Orders_Run : BTPControlLibrary.MainWindow
    {
        #region Fields
        private readonly BlurEffect blur = new();
        public bool iscontinue = false;
        public ResourceDictionary dict = new();
        public Dictionary<string, string> dictionary;
        private readonly Dictionary<string, string> custom_columns;
        private readonly Dictionary<string, Tuple<string, Type>> cells = new();

        public delegate void openInventoryWindow(string Symbol);
        public event openInventoryWindow? OpenInventoryWindowEvent;

        private DataTable dtOrderLines;
        private readonly DataTable polki = Connection("Kar_Polki");
        #endregion

        public override void InputInterface(string name, object value)
        {
            base.InputInterface(name, value);
            switch (name)
            {
                case "TRAY_IN_WINDOW":
                    short[] v = (short[])value;
                    SetSubmitButton(v[_plc.WindowNr]);
                    break;
                case "TOWER_SIGNALS":
                    short[] va = (short[])value;
                    SetWeight(va[0]);
                    Weight_tara_PLC.Text = va[0].ToString();
                    break;
            }
        }

        #region Constructors

        //Constructor for Selected Order 
        public Orders_Run(ResourceDictionary resourceDictionary, Dictionary<string, string> lang, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns, DataTable dtOrderLines)
        {
            this.cells = cells;
            this.custom_columns = custom_columns;
            this.dtOrderLines = dtOrderLines;
            dictionary = lang;
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);
            DataContext = this;

            InitializeComponent();

            ID_Localization.SelectionChanged += ID_Localization_SelectionChanged;
            ID_Tray.SelectionChanged += ID_Tray_SelectionChanged;

            if (!_cfg.GetParamBool("TOWER_HasWeight"))
                WeightSize(true);

            TextBox_Fill(dtOrderLines.Rows[0], false);
            FillCanvas(new List<ArticlesControl.Klasy.Localization>());



        }
        // COnstructor for selected line 
        public Orders_Run(ResourceDictionary resourceDictionary, Dictionary<string, string> lang, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns, DataTable dtOrderLines, int indexval)
        {
            this.cells = cells;
            this.custom_columns = custom_columns;
            this.dtOrderLines = dtOrderLines;
            dictionary = lang;
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);
            DataContext = this;

            InitializeComponent();
            TextBox_Fill(dtOrderLines.Rows[indexval], false);
            if (!_cfg.GetParamBool("TOWER_HasWeight"))
                WeightSize(true);
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

        private void ID_Tray_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ID_Tray.Items.Count > 0)
            {
                ID_Localization.ItemsSource = null;
                List<ArticlesControl.Klasy.Localization> list1 = new();
                List<string> combosource = new();
                DataTable data = new();
                int xpolki;
                int ypolki;

                data = _dbWMS.Add_Lokalizacje_Select.GetData().Select("ID_Polka = '" + ID_Tray.SelectedItem.ToString() + "'").CopyToDataTable();
                _ = int.TryParse(polki.Select("NrPolki = '" + ID_Tray.SelectedItem.ToString() + "'").CopyToDataTable().Rows[0]["Dlugosc_def"].ToString(), out xpolki);
                _ = int.TryParse(polki.Select("NrPolki = '" + ID_Tray.SelectedItem.ToString() + "'").CopyToDataTable().Rows[0]["Szerokosc_def"].ToString(), out ypolki);

                foreach (DataRow item in data.Rows)
                {
                    combosource.Add(item["Symbol"].ToString());
                    list1.Add(new ArticlesControl.Klasy.Localization(Convert.ToInt32(item["ID_Lokalizacji"].ToString()), item["Symbol"].ToString(), Convert.ToInt32(item["WymiarX"].ToString()), Convert.ToInt32(item["WymiarY"].ToString()), Convert.ToInt32(item["WymiarZ"].ToString()), Convert.ToInt32(item["PozX"].ToString()), Convert.ToInt32(item["PozY"].ToString()), xpolki, ypolki));
                }
                ID_Localization.ItemsSource = combosource;
                FillCanvas(list1);
            }


        }
        private void ID_Localization_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ID_Localization.Items.Count == 0)
            {
                List<ArticlesControl.Klasy.Localization> list1 = new();
                DataTable data = new();
                int xpolki;
                int ypolki;
                data = _dbWMS.Add_Lokalizacje_Select.GetData().Select("ID_Polka = '" + ID_Tray.SelectedValue.ToString() + "'").CopyToDataTable();
                _ = int.TryParse(polki.Select("NrPolki = '" + ID_Tray.SelectedValue.ToString() + "'").CopyToDataTable().Rows[0]["Dlugosc_def"].ToString(), out xpolki);
                _ = int.TryParse(polki.Select("NrPolki = '" + ID_Tray.SelectedValue.ToString() + "'").CopyToDataTable().Rows[0]["Szerokosc_def"].ToString(), out ypolki);


                foreach (DataRow item in data.Rows)
                {
                    list1.Add(new ArticlesControl.Klasy.Localization(Convert.ToInt32(item["ID_Lokalizacji"].ToString()), item["Symbol"].ToString(), Convert.ToInt32(item["WymiarX"].ToString()), Convert.ToInt32(item["WymiarY"].ToString()), Convert.ToInt32(item["WymiarZ"].ToString()), Convert.ToInt32(item["PozX"].ToString()), Convert.ToInt32(item["PozY"].ToString()), xpolki, ypolki));
                }
                if (ID_Localization.SelectedValue is null)
                {
                    ID_Localization.SelectedIndex = 0;
                    //FillCanvas(list1, ID_Localization.SelectedValue.ToString() ?? string.Empty);
                }
                else
                {
                    FillCanvas(list1, ID_Localization.SelectedValue.ToString() ?? string.Empty);
                }
            }
        }
        private DataRow GetPositionForOrderLine(string NrZlecenia, int? NrPozycji, int? Kierunek, int? TypKompletacji, int? Polka, int? PR, string Operator, int? Okno)
        {
            Operator = null;
            Okno = null;
            try
            {
                DataTable dt = _dbWMS.ZlecenieRealizacja_PobierzPozycje_ta.GetData(NrZlecenia, NrPozycji, Kierunek, TypKompletacji, Polka, PR, ArticlesControl.Articles.Username, Okno).CopyToDataTable();
                DataRow dr = dt.Rows[0];
                return dr;
            }
            catch
            {
                DataRow dr_null = null;
                return dr_null;
            }

        }
        #endregion

        #region Functions
        private void SetWeight(short v)
        {
            Weight_tara_PLC.Text = v.ToString();
            try
            {
                CalculatedCountTextBox.Text = Math.Abs(v / Convert.ToDouble(Weight_Converter)).ToString();
            }
            catch { }
        }
        private void TextBox_Fill(DataRow cells, bool arrow)
        {
            Order_Name.Text = cells["NrZlecenia"].ToString();
            Indeks.Text = cells["Indeks"].ToString();
            Position_Number.Text = cells["NrPozycji"].ToString();
            Qty.Text = arrow == true ? cells["IloscPozycja"].ToString() : cells["Ilosc"].ToString();
            QtyConfirmed.Text = cells["IloscZatwierdzona"].ToString();
            SetTrayInfo(cells);
        }
        private void SetTrayInfo(DataRow dr)
        {

            DataRow info = GetPositionForOrderLine(dr["NrZlecenia"].ToString(), Convert.ToInt32(dr["NrPozycji"]), 0, -2, 0, 0, ArticlesControl.Articles.Username, 0);


            ID_Tray.ItemsSource = null;
            ID_Tray.ItemsSource = new List<object> { (info["Polka"]) };
            ID_Tray.SelectedItem = info["Polka"];
            object loc = _dbWMS.Add_Lokalizacje_Select.GetData().Select("ID_Lokalizacji = '" + info["ID_Lokalizacji"] + "'")[0]["Symbol"];
            ID_Localization.ItemsSource = null;
            ID_Localization.ItemsSource = new List<object> { loc };
            ID_Localization.SelectedItem = loc;
            Quantity_Available.Text = info["IloscPolka"].ToString();

            GetGiveOperation(true);
        }

        private void GetGiveOperation(bool isGet)
        {
            SetControlsColor(ID_Tray, isGet);
            SetControlsColor(ID_Localization, isGet);
            FindEmptyTrayButton.IsEnabled = !isGet;
            Localization_Visualizer.IsEnabled = !isGet;

        }

        private void SetControlsColor(object obj, bool disable)
        {
            if (obj.GetType() == typeof(HandyControl.Controls.TextBox))
            {
                TextBox tb = (TextBox)obj;
                if (disable)
                {
                    tb.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["DisabledTextBox"].ToString());
                    tb.IsEnabled = true;
                    tb.Focusable = false;
                    tb.IsReadOnly = true;
                }
                else
                {
                    tb.Background = default;
                    tb.IsEnabled = true;
                    tb.Focusable = true;
                    tb.IsReadOnly = false;
                }


            }
            else if (obj.GetType() == typeof(HandyControl.Controls.ComboBox))
            {
                ComboBox cb = (ComboBox)obj;
                if (disable)
                {
                    cb.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["DisabledTextBox"].ToString());
                    cb.IsEnabled = true;
                    cb.Focusable = false;
                    cb.IsReadOnly = true;
                    cb.MaxDropDownHeight = 0;
                }
                else
                {
                    cb.Background = default;
                    cb.IsEnabled = true;
                    cb.Focusable = true;
                    cb.IsReadOnly = false;
                    cb.MaxDropDownHeight = default;
                }
            }
        }
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (Quantity.Text == null || Convert.ToInt32(Convert.ToDouble(Quantity.Text)) > Convert.ToInt32(Convert.ToDouble(Quantity_Available.Text)))
            {
                e.Handled = true;

                ColorAnimation flash = new(Colors.White, Colors.Red, new Duration(TimeSpan.FromMilliseconds(75D))) { AutoReverse = true, RepeatBehavior = new RepeatBehavior(3D) };
                //Quantity.Background = new SolidColorBrush(Colors.White); // no error???
                Quantity.Background.BeginAnimation(SolidColorBrush.ColorProperty, flash);
                return;
            }

            try
            {
                Update_Stock();
                if (iscontinue)
                {
                    e.Handled = true;
                    iscontinue = false;
                }
                else
                {
                    Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void Update_Stock()
        {

        }
        private void WeightSize(bool val)
        {
            if (val)
            {
                this.Width = 770;
                //OrderLineAllGroupbox.Width = 750;
            }
            else
            {
                this.Width = 1024;
                //OrderLineAllGroupbox.Width = 1004;
            }
        }
        private void Change_Quantity_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                string operation = btn.Content.ToString();
                int available_count = Convert.ToInt32(Convert.ToDouble(Quantity_Available.Text));
                int value = 0;
                if (Quantity.Text != "")
                    value = Convert.ToInt32(Convert.ToDouble(Quantity.Text));
                if (operation == "+" && value < (Convert.ToInt32(Qty.Text) - Convert.ToInt32(QtyConfirmed.Text)) && value < Convert.ToInt32(Quantity_Available.Text))
                    value += 1;
                else if (operation == "-" && value > 0)
                {
                    value -= 1;
                }
                Quantity.Text = value.ToString();
            }
            catch { }
        }
        private void SetSubmitButton(short v)
        {

            if (v == Convert.ToInt32(ID_Tray.Text))
                EnableButton(true);
            else
                EnableButton(false);

        }
        private void EnableButton(bool bv)
        {
            Confirm.IsEnabled = bv;
        }
        #endregion

        private void KB_GotFocus(object sender, RoutedEventArgs e)
        {
            //TextBox tb = sender as TextBox;
            //kb.FocusedTextBox = tb;
            //if (nkb.Visibility == Visibility.Visible)
            //    nkb.Hide();
        }
        private void NKB_GotFocus(object sender, RoutedEventArgs e)
        {
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
        #region WEIGHT
        private void Tara_Click(object sender, RoutedEventArgs e)
        {
            //background.WRITE_Bit(background.ads.WEIGHT_ZERO);
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Quantity.Text = CalculatedCountTextBox.Text;
        }
        #endregion
        private void btUP_Click(object sender, RoutedEventArgs e)
        {
            TextBox_Fill(GetPositionForOrderLine(Order_Name.Text.ToString(), Convert.ToInt32(Position_Number.Text), 1, 0, 0, 0, "sdsad", 0), true);
        }
        private void btDOWN_Click(object sender, RoutedEventArgs e)
        {
            TextBox_Fill(GetPositionForOrderLine(Order_Name.Text.ToString(), Convert.ToInt32(Position_Number.Text), -1, 0, 0, 0, "sdsad", 0), true);
        }
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void BringTrayButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //background.WRITE_TrayTransport(Convert.ToInt16(ID_Tray.Text), Convert.ToInt16(background._window));
            }
            catch { }

        }
        private void FindEmptyTray_Click(object sender, RoutedEventArgs e)
        {

        }
        #region generator lokalizacji
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
        #endregion

    }

}
