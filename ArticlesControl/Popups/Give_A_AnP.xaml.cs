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
    /// Logika interakcji dla klasy Add_Stock.xaml
    /// </summary>
    public partial class Give_A_AnP : BTPControlLibrary.MainWindow
    {
        #region Fields
        private readonly BlurEffect blur = new();
        public bool iscontinue = false;
        public ResourceDictionary dict = new();
        public Dictionary<string, string> dictionary;
        private readonly Dictionary<string, string> custom_columns;
        private readonly Dictionary<string, Tuple<string, Type>> cells = new();
        private bool isEdit;
        private int actualTrayInWindow;


        #endregion

        public override void InputInterface(string name, object value)
        {
            base.InputInterface(name, value);
            switch (name)
            {
                case "TRAY_IN_WINDOW":
                    short[] v = (short[])value;
                    actualTrayInWindow = v[_plc.WindowNr];
                    SetSubmitButton();
                    break;
                case "TOWER_SIGNALS":
                    short[] va = (short[])value;
                    SetWeight(va[0]);
                    Weight_tara_PLC.Text = va[0].ToString();
                    break;
            }
        }

        #region Constructors
        //Constructor for Stock row
        public Give_A_AnP(ResourceDictionary resourceDictionary, Dictionary<string, string> lang, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns)
        {
            this.cells = cells;
            this.custom_columns = custom_columns;
            dictionary = lang;
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);
            DataContext = this;
            isEdit = true;
            InitializeComponent();
            if (!_cfg.GetParamBool("TOWER_HasWeight"))
                this.Height = 490;
            TextBox_Fill(cells);

        }

        //Constructor for Article GIVE with selected tray
        public Give_A_AnP(ResourceDictionary resourceDictionary, Dictionary<string, string> lang, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns, DataRow traycells, bool isEdited)
        {
            this.cells = cells;
            this.custom_columns = custom_columns;
            dictionary = lang;
            dict = resourceDictionary;
            isEdit = isEdited;
            Resources.MergedDictionaries.Add(dict);
            DataContext = this;
            InitializeComponent();
            if (!_cfg.GetParamBool("TOWER_HasWeight"))
                this.Height = 490;
            TextBox_Fill(cells, traycells);
        }

        //Constructor for Article GIVE with manual selected tray number 
        public Give_A_AnP(ResourceDictionary resourceDictionary, Dictionary<string, string> lang, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns, string tray, string localization, bool isEdited)
        {
            this.cells = cells;
            this.custom_columns = custom_columns;
            dictionary = lang;
            dict = resourceDictionary;
            isEdit = isEdited;
            Resources.MergedDictionaries.Add(dict);
            DataContext = this;
            InitializeComponent();
            if (!_cfg.GetParamBool("TOWER_HasWeight"))
                this.Height = 490;
            TextBox_Fill(cells["Indeks"].Item1.ToString(), localization, tray);

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

        private void DisableTextBoxes(bool value)
        {
            ID_Localization.IsEnabled = !value;
            ArticleLinkWithLocation.IsEnabled = !value;
            Batch.IsEnabled = !value;
            Reservation.IsEnabled = !value;
            Attestation.IsEnabled = !value;
            Melt.IsEnabled = !value;
            Note.IsEnabled = !value;
            Atr1.IsEnabled = !value;
            Atr2.IsEnabled = !value;
            Atr3.IsEnabled = !value;
        }

        private void TextBox_Fill(string indeks, string tray, string localization)
        {
            try
            {
                DisableTextBoxes(false);
                Indeks.Text = indeks;
                ID_Tray.Text = tray;
                ID_Localization.Text = localization;
            }
            catch
            {
                HandyControl.Controls.MessageBox.Show("Półka nie istnieje lub nie zostały dla niej zdefiniowane lokalizacje.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                Close();
            }

        }

        private void TextBox_Fill(Dictionary<string, Tuple<string, Type>> cells)
        {
            DisableTextBoxes(true);
            Indeks.Text = cells["Indeks"].Item1.ToString();
            ID_Tray.Text = GetTrayNumberBySymbol(cells["Symbol"].Item1.ToString());
            ID_Localization.Text = (cells["Symbol"].Item1.ToString());
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

        private void TextBox_Fill(Dictionary<string, Tuple<string, Type>> cells, DataRow traycells)
        {
            DisableTextBoxes(true);
            Indeks.Text = cells["Indeks"].Item1.ToString();
            ID_Tray.Text = traycells["ID_Polki"].ToString();
            ID_Localization.Text = (traycells["Symbol"].ToString());
            Quantity_Available.Text = traycells["Ilosc"].ToString();
            Reservation.Text = traycells["Rezerwacja"].ToString();
            Batch.Text = traycells["Partia"].ToString();
            Attestation.Text = traycells["Atest"].ToString();
            Melt.Text = traycells["Wytop"].ToString();
            Note.Text = traycells["Notatka"].ToString();
            Atr1.Text = traycells["Atr1"].ToString();
            Atr2.Text = traycells["Atr2"].ToString();
            Atr3.Text = traycells["Atr3"].ToString();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (Indeks.Text is null)
            {
                e.Handled = true;

                ColorAnimation flash = new(Colors.White, Colors.Red, new Duration(TimeSpan.FromMilliseconds(75D))) { AutoReverse = true, RepeatBehavior = new RepeatBehavior(3D) };
                Indeks.Background = new SolidColorBrush(Colors.White); // no error???
                Indeks.Background.BeginAnimation(SolidColorBrush.ColorProperty, flash);
                return;
            }
            if (ID_Tray.Text == null)
            {
                e.Handled = true;

                ColorAnimation flash = new(Colors.White, Colors.Red, new Duration(TimeSpan.FromMilliseconds(75D))) { AutoReverse = true, RepeatBehavior = new RepeatBehavior(3D) };
                ID_Tray.Background = new SolidColorBrush(Colors.White); // no error???
                ID_Tray.Background.BeginAnimation(SolidColorBrush.ColorProperty, flash);
                return;
            }
            if (ID_Localization.Text == null)
            {
                e.Handled = true;

                ColorAnimation flash = new(Colors.White, Colors.Red, new Duration(TimeSpan.FromMilliseconds(75D))) { AutoReverse = true, RepeatBehavior = new RepeatBehavior(3D) };
                ID_Localization.Background = new SolidColorBrush(Colors.White); // no error???
                ID_Localization.Background.BeginAnimation(SolidColorBrush.ColorProperty, flash);
                return;
            }
            try
            {
                if (isEdit)
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
                else
                {
                    Insert_Stock();
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
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Update_Stock()
        {
            int temp1 = Convert.ToInt32(_dbWMS.Add_Lokalizacje_Select.GetData().Select(string.Format("Symbol = '{0}' AND ID_Polka = {1}", ID_Localization.Text, ID_Tray.Text))[0][0]);
            decimal count = Convert.ToDecimal(Quantity.Text);
            decimal count_available = Convert.ToDecimal(Quantity_Available.Text);

            decimal temp3 = count_available + count;
            _ = decimal.TryParse(Reservation.Text, out decimal temp4);
            DateTime data = DateTime.Now;

            Stock stock = new(_dbWMS, Indeks.Text, temp1, Convert.ToInt32(ID_Tray.Text), temp3, temp4, Batch.Text, Attestation.Text, Melt.Text, Atr1.Text, Atr2.Text, Atr3.Text, data, Note.Text, Articles.Username);
            stock.Update();
            isEdit = false;
            if (stock.isOk)
            {
                iscontinue = true;
            }
        }

        private void Insert_Stock()
        {
            int temp1 = Convert.ToInt32(_dbWMS.Add_Lokalizacje_Select.GetData().Select(string.Format("Symbol = '{0}' AND ID_Polka = {1}", ID_Localization.Text, ID_Tray.Text))[0][0]);
            int temp2 = 0;
            if (ID_Tray.Text is not null)
            {
                for (int i = 0; i < _dbWMS.Add_Polki_Select.GetData(false).Rows.Count; i++)
                {
                    if (ID_Tray.Text.ToString() == _dbWMS.Add_Polki_Select.GetData(false).Rows[i]["NrPolki"].ToString())
                    {
                        temp2 = Convert.ToInt32(_dbWMS.Add_Polki_Select.GetData(false).Rows[i]["ID_Polki"]);
                        break;
                    }
                }
            }
            _ = decimal.TryParse(Quantity.Text, out decimal temp3);
            _ = decimal.TryParse(Reservation.Text, out decimal temp4);
            DateTime data = DateTime.Now;

            Stock stock = new(_dbWMS, Indeks.Text, temp1, temp2, temp3, temp4, Batch.Text, Attestation.Text, Melt.Text, Atr1.Text, Atr2.Text, Atr3.Text, data, Note.Text, Articles.Username);
            stock.Insert();
            if (stock.isOk)
            {
                iscontinue = true;
            }
        }


        #endregion

        private void Change_Quantity_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string operation = btn.Content.ToString();
            int value = 0;
            if (Quantity.Text != "")
                value = Convert.ToInt32(Convert.ToDouble(Quantity.Text));

            if (operation == "+")
                value += 1;
            else if (operation == "-" && value > 0)
            {
                value -= 1;
            }

            Quantity.Text = value.ToString();

        }

        private void SetSubmitButton()
        {

            if (actualTrayInWindow == Convert.ToInt32(ID_Tray.Text))
                EnableButton(true);
            else
                EnableButton(false);
        }

        private void EnableButton(bool bv)
        {
            Submit.IsEnabled = bv;
        }

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
    }
}
