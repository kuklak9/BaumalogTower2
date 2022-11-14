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
using BTPDataBase;
using BTPTwinCatADS;

namespace ArticlesControl.Formatki
{
    /// <summary>
    /// Logika interakcji
    /// </summary>
    public partial class Get_A_AnP : BTPControlLibrary.MainWindow
    {
        #region Fields
        private readonly BlurEffect blur = new();
        public bool iscontinue = false;
        public ResourceDictionary dict = new();
        public Dictionary<string, string> dictionary;
        private readonly Dictionary<string, string> custom_columns;
        private readonly Dictionary<string, Tuple<string, Type>> cells = new();
        private readonly DataRow traycells;
        private bool fromArticle;
        public delegate void openInventoryWindow(string Symbol);
        public event openInventoryWindow? OpenInventoryWindowEvent;
        private readonly List<string?> list = new();

        #endregion

        public override void InputInterface(string name, object value)
        {
            base.InputInterface(name, value);
            switch (name)
            {
                case "TRAY_IN_WINDOW":
                    short[] v = (short[])value;
                    Submit.IsEnabled = (v[_plc.WindowNr] == Convert.ToInt32(ID_Tray.Text)) ? true : false;
                    break;
                case "TOWER_SIGNALS":
                    short[] va = (short[])value;
                    SetWeight(va[0]);
                    Weight_tara_PLC.Text = va[0].ToString();
                    break;
            }
        }

        #region Constructors
        //Constructor for Stock GET
        public Get_A_AnP(ResourceDictionary resourceDictionary, Dictionary<string, string> lang, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns)
        {
            this.cells = cells;
            this.custom_columns = custom_columns;
            dictionary = lang;
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);
            DataContext = this;
            InitializeComponent();
            //if (background._hasweight == 0)
            //    this.Height = 500;
            TextBox_Fill(cells);

        }

        //Constructor for Article GET
        public Get_A_AnP(ResourceDictionary resourceDictionary, Dictionary<string, string> lang, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns, string indeks, DataRow traycells)
        {
            this.cells = cells;
            this.custom_columns = custom_columns;
            this.traycells = traycells;
            dictionary = lang;
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);
            DataContext = this;
            InitializeComponent();
            //if (background._hasweight == 0)
            //    this.Height = 500;
            fromArticle = true;

            TextBox_Fill(cells, traycells);
        }
        #endregion

        #region Functions
        private void TabItem1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TabControl.SelectedIndex = 0;
            Step.StepIndex = 0;
        }
        private void TabItem2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TabControl.SelectedIndex = 1;
            Step.StepIndex = 1;
        }

        private void SetWeight(short v)
        {
            Weight_tara_PLC.Text = v.ToString();
            try
            {
                CalculatedCountTextBox.Text = Math.Abs(v / Convert.ToDouble(Weight_Converter)).ToString();
            }
            catch { }
        }

        private void TextBox_Fill(Dictionary<string, Tuple<string, Type>> cells, DataRow traycells)
        {
            Indeks.Text = cells["Indeks"].Item1.ToString();
            ID_Tray.Text = traycells["ID_Polki"].ToString();
            ID_Localization.Text = traycells["Symbol"].ToString();
            Quantity_Available.Text = traycells["Ilosc"].ToString();
            Reservation.Text = cells["Rezerwacja"].Item1.ToString();
            Batch.Text = traycells["Partia"].ToString();
            Attestation.Text = traycells["Atest"].ToString();
            Melt.Text = traycells["Wytop"].ToString();
            Note.Text = traycells["Notatka"].ToString();
            Atr1.Text = traycells["Atr1"].ToString();
            Atr2.Text = traycells["Atr2"].ToString();
            Atr3.Text = traycells["Atr3"].ToString();
        }

        private void TextBox_Fill(Dictionary<string, Tuple<string, Type>> cells)
        {
            Indeks.Text = cells["Indeks"].Item1.ToString();
            ID_Tray.Text = GetTrayNumberBySymbol(cells["Symbol"].Item1.ToString());
            ID_Localization.Text = cells["Symbol"].Item1.ToString();
            Quantity_Available.Text = cells["Ilosc"].Item1.ToString();
            Reservation.Text = cells["Rezerwacja"].Item1.ToString();
            Batch.Text = cells["Partia"].Item1.ToString();
            Attestation.Text = cells["Atest"].Item1.ToString();
            Melt.Text = cells["Wytop"].Item1.ToString();
            Note.Text = cells["Notatka"].Item1.ToString();
            Atr1.Text = cells["Atr1"].Item1.ToString();
            Atr2.Text = cells["Atr2"].Item1.ToString();
            Atr3.Text = cells["Atr3"].Item1.ToString();
        }
        private string GetTrayNumberBySymbol(string symbol)
        {
            return _dbWMS.Add_Lokalizacje_Select.GetData().Select("Symbol = '" + symbol + "'")[0]["ID_Polka"].ToString();
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
                    if (Empty_Tray.IsChecked == true /*&& background._accesslevel >= 60*/)
                    {
                        openInventoryWindow? handler = OpenInventoryWindowEvent;
                        OpenInventoryWindowEvent?.Invoke(ID_Localization.Text);
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //private void Cancel_Click(object sender, RoutedEventArgs e)
        //{
        //    this.Close();
        //}

        private void Get_All_Click(object sender, RoutedEventArgs e)
        {
            Quantity.Text = Quantity_Available.Text;
        }

        private void Update_Stock()
        {
            int temp1 = Convert.ToInt32(_dbWMS.Add_Lokalizacje_Select.GetData().Select(string.Format("Symbol = '{0}' AND ID_Polka = {1}", ID_Localization.Text, ID_Tray.Text))[0][0]);
            decimal count = Convert.ToDecimal(Quantity.Text);
            decimal count_available = Convert.ToDecimal(Quantity_Available.Text);
            if (count > count_available)
            {
                MessageBox.Show("Nie można pobrać więcej niż jest na półce");
                return;
            }
            decimal temp3 = count_available - count;
            _ = decimal.TryParse(Reservation.Text, out decimal temp4);
            DateTime data = DateTime.Now;

            Stock stock = new(_dbWMS,Indeks.Text, temp1, Convert.ToInt32(ID_Tray.Text), temp3, temp4, Batch.Text, Attestation.Text, Melt.Text, Atr1.Text, Atr2.Text, Atr3.Text, data, Note.Text, Articles.Username);
            stock.Update();
            if ((bool)Empty_Tray.IsChecked)
            {
                ArticlesControl.Klasy.Tray tray = new(Convert.ToInt32(ID_Tray.Text), 10);
                tray.SetInventory();
            }

            if (stock.isOk)
            {
                iscontinue = true;
            }
        }

        private void Change_Quantity_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string operation = btn.Content.ToString();
            int available_count = Convert.ToInt32(Convert.ToDouble(Quantity_Available.Text));
            int value = 0;
            if (Quantity.Text != "")
                value = Convert.ToInt32(Convert.ToDouble(Quantity.Text));
            if (operation == "+" && value < available_count)
                value += 1;
            else if (operation == "-" && value > 0)
            {
                value -= 1;
            }

            Quantity.Text = value.ToString();

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
           //plc.WRITE_Bit(background.ads.WEIGHT_ZERO);
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Quantity.Text = CalculatedCountTextBox.Text;
        }
        #endregion
    }
}