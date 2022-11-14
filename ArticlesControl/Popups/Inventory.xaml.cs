using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace ArticlesControl.Formatki
{
    public partial class Inventory : BTPControlLibrary.MainWindow, INotifyPropertyChanged
    {
        #region fields
        private readonly BlurEffect blur = new();
        public bool iscontinue = false;
        public ResourceDictionary dict = new();
        public Dictionary<string, string> dictionary;
        private readonly Dictionary<string, string> custom_columns;
        private readonly Dictionary<string, Tuple<string, Type>> cells = new();
        private readonly DataTable dtFromTray;
        private DataTable dtBusyLoc;
        private DataRow dr;
        private DataRow traycells;
        private bool fromArticle;
        private bool fromTray;
        private int numberLocations;
        private int actualLocationsForInventory;
        List<string> oldparam = new List<string>();
        List<string> newparam = new List<string>();
        List<string> nameparam = new List<string>() { "Tray", "Loc", "Indeks", "Qty", "Weigth Converter", "Note", "Atr1", "Atr2", "Atr3", "Batch", "Attestation", "Melt", "Reservation" };
        #endregion

        #region update label
        public event PropertyChangedEventHandler PropertyChanged;
        private string _whichLocation;
        public string WhichLocation
        {
            get { return _whichLocation; }
            set
            {
                _whichLocation = value;
                OnPropertyChanged();
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region CONSTRUCTORS
        //Constructor for Stock row
        public Inventory(ResourceDictionary resourceDictionary, Dictionary<string, string> lang, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns)
        {
            this.cells = cells;
            this.custom_columns = custom_columns;
            dictionary = lang;
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);

            InitializeComponent();
            DataContext = this;

            if (!_cfg.GetParamBool("TOWER_HasWeight"))
                this.Height = 490;
            FillIndeksCombobox();
            TextBox_Fill(cells);
        }

        //Constructor for Stock row from Article
        public Inventory(ResourceDictionary resourceDictionary, Dictionary<string, string> lang, DataRow traycells)
        {
            this.custom_columns = custom_columns;
            this.traycells = traycells;
            dictionary = lang;
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);

            fromArticle = true;
            InitializeComponent();
            DataContext = this;

            if (!_cfg.GetParamBool("TOWER_HasWeight"))
                this.Height = 490;
            FillIndeksCombobox();
            TextBox_Fill(traycells);
        }

        //Constructor for Tray
        public Inventory(ResourceDictionary resourceDictionary, Dictionary<string, string> lang, DataTable dt)
        {
            dtFromTray = dt;
            actualLocationsForInventory = 0;
            numberLocations = dtFromTray.Rows.Count;
            if (numberLocations > 0)
            {
                dictionary = lang;
                dict = resourceDictionary;
                Resources.MergedDictionaries.Add(dict);

                fromTray = true;

                InitializeComponent();
                DataContext = this;

                if (!_cfg.GetParamBool("TOWER_HasWeight"))
                    this.Height = 490;
                FillIndeksCombobox();
                TextBox_Fill_Tray(dtFromTray);
            }
        }

        #endregion

        #region FUNCTIONS
        private void FillIndeksCombobox()
        {
            Indeks.Items.Clear();
            DataTable dt = _dbWMS.Add_Kar_Artykul_ta.GetData().CopyToDataTable();
            foreach (DataRow row in dt.Rows)
            {
                Indeks.Items.Add(row["Indeks"]);
            }
        }
        private void Before_TextBox_Filling(DataRow cell, bool empty)
        {
            dr = cell;
            Clear_TextBox(empty);
            if (empty == true)
            {
                ID_Tray.Text = GetTrayNumberBySymbol(cell["Symbol"].ToString());
                ID_Localization.Text = cell["Symbol"].ToString();
            }
            else
                TextBox_Fill(cell);



        }
        private void TextBox_Fill_Tray(DataTable dt)
        {

            actualLocationsForInventory++;
            progressLocationStepBar.Minimum = 0;
            progressLocationStepBar.Maximum = numberLocations;
            progressLocationStepBar.Value = actualLocationsForInventory;
            WhichLocation = actualLocationsForInventory + "/" + numberLocations;
            dr = dt.Rows[actualLocationsForInventory - 1];
            try
            {
                dtBusyLoc = _dbWMS.Add_StanyMagazynowe_Filter.GetData("000", "000").Select("Symbol = " + "'" + dr["Symbol"] + "'").CopyToDataTable();
            }
            catch { dtBusyLoc = null; }
            if (dtBusyLoc == null)
                Before_TextBox_Filling(dr, true);
            else
            {
                Before_TextBox_Filling(dtBusyLoc.Rows[0], false);
            }


        }
        private void TextBox_Fill(Dictionary<string, Tuple<string, Type>> cells)
        {

            Indeks.SelectedItem = cells["Indeks"].Item1.ToString();
            ID_Tray.Text = GetTrayNumberBySymbol(cells["Symbol"].Item1.ToString());
            //ID_Localization.Items.Clear();
            //ID_Localization.Items.Add(cells["Symbol"].Item1.ToString());
            //ID_Localization.SelectedValue = cells["Symbol"].Item1.ToString();
            ID_Localization.Text = cells["Symbol"].Item1.ToString();
            Quantity.Text = cells["Ilosc"].Item1.ToString();
            Reservation.Text = cells["Rezerwacja"].Item1.ToString();
            Batch.Text = cells["Partia"].Item1.ToString();
            Attestation.Text = cells["Atest"].Item1.ToString();
            Melt.Text = cells["Wytop"].Item1.ToString();
            Note.Text = cells["Notatka"].Item1.ToString();
            Atr1.Text = cells["Atr1"].Item1.ToString();
            Atr2.Text = cells["Atr2"].Item1.ToString();
            Atr3.Text = cells["Atr3"].Item1.ToString();
            FillParamList(oldparam);
        }
        private void TextBox_Fill(DataRow cell)
        {

            Indeks.SelectedItem = cell["Indeks"].ToString();
            ID_Tray.Text = GetTrayNumberBySymbol(cell["Symbol"].ToString());
            //ID_Localization.Items.Clear();
            //ID_Localization.Items.Add(cell["Symbol"].ToString());
            //ID_Localization.SelectedValue = cell["Symbol"].ToString();
            ID_Localization.Text = cell["Symbol"].ToString();
            Weight_Converter.Text = "";
            Quantity.Text = cell["Ilosc"].ToString();
            Reservation.Text = cell["Rezerwacja"].ToString();
            Batch.Text = cell["Partia"].ToString();
            Attestation.Text = cell["Atest"].ToString();
            Melt.Text = cell["Wytop"].ToString();
            Note.Text = cell["Notatka"].ToString();
            Atr1.Text = cell["Atr1"].ToString();
            Atr2.Text = cell["Atr2"].ToString();
            Atr3.Text = cell["Atr3"].ToString();
            FillParamList(oldparam);
        }
        private void FillParamList(List<string> list)
        {
            list.Add(ID_Tray.Text);
            //list.Add(ID_Localization.SelectedItem.ToString());
            list.Add(ID_Localization.Text);
            if (Indeks.SelectedIndex != -1)
                list.Add(Indeks.SelectedItem.ToString());
            else
                list.Add("");
            list.Add(Quantity.Text);
            list.Add(Weight_Converter.Text);
            list.Add(Note.Text);
            list.Add(Atr1.Text);
            list.Add(Atr2.Text);
            list.Add(Atr3.Text);
            list.Add(Batch.Text);
            list.Add(Attestation.Text);
            list.Add(Melt.Text);
            list.Add(Reservation.Text);
        }
        private void Clear_TextBox(bool empty)
        {
            if (empty)
                Indeks.SelectedIndex = -1;
            ID_Tray.Text = "";
            //ID_Localization.Items.Clear();
            ID_Localization.Text = "";
            Quantity.Text = "";
            Reservation.Text = "";
            Batch.Text = "";
            Attestation.Text = "";
            Melt.Text = "";
            Note.Text = "";
            Atr1.Text = "";
            Atr2.Text = "";
            Atr3.Text = "";
        }
        private string GetTrayNumberBySymbol(string symbol)
        {
            return _dbWMS.Add_Lokalizacje_Select.GetData().Select("Symbol = '" + symbol + "'")[0]["ID_Polka"].ToString();
        }
        private void Make_Inventory()
        {

            try
            {
                int temp1 = Convert.ToInt32(_dbWMS.Add_Lokalizacje_Select.GetData().Select(string.Format("Symbol = '{0}' AND ID_Polka = {1}", ID_Localization.Text, ID_Tray.Text))[0][0]);
                _ = decimal.TryParse(Quantity.Text, out decimal temp3);
                _ = decimal.TryParse(Reservation.Text, out decimal temp4);
                DateTime data = DateTime.Now;

                Stock stock = new(_dbWMS, Indeks.SelectedItem.ToString(), temp1, Convert.ToInt32(ID_Tray.Text), temp3, temp4, Batch.Text, Attestation.Text, Melt.Text, Atr1.Text, Atr2.Text, Atr3.Text, data, Note.Text, Articles.Username);
                stock.Update();
                ArticlesControl.Klasy.Tray tray = new(Convert.ToInt32(ID_Tray.Text), 1);
                tray.SetInventory();

                FillParamList(newparam);
                //background.InventoryLog(nameparam, oldparam, newparam);
                _log.AddLog_Other("Inwentaryzacja", _loggedUser.UserName);
                if (stock.isOk)
                {
                    iscontinue = true;
                }
            }
            catch { }


        }
        #endregion

        #region BUTTONS
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (!fromTray)
            {
                if (Indeks.SelectedItem is null)
                {
                    e.Handled = true;

                    ColorAnimation flash = new(Colors.White, Colors.Red, new Duration(TimeSpan.FromMilliseconds(75D))) { AutoReverse = true, RepeatBehavior = new RepeatBehavior(3D) };
                    Indeks.Background = new SolidColorBrush(Colors.White); // no error???
                    Indeks.Background.BeginAnimation(SolidColorBrush.ColorProperty, flash);
                    return;
                }
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

            if (fromTray)
            {
                if (numberLocations > actualLocationsForInventory)
                {
                    Make_Inventory();
                    TextBox_Fill_Tray(dtFromTray);
                }
                else
                {
                    Make_Inventory();
                    //TODO: dodać zeby kosawać status polki do inwentaryzacji
                    Close();
                }
            }
            else
            {
                try
                {
                    Make_Inventory();
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
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Find_Article_Click(object sender, RoutedEventArgs e)
        {
            Windows.Select_an_article _Article = new(dict, dictionary);
            _Article.Owner = Window.GetWindow(this);
            Opacity = 0.5;
            Effect = blur;
            _Article.ShowDialog();
            Opacity = 1;
            Effect = null;
            FillIndeksCombobox();
            if (_Article.IndeksofArticle == null)
                TextBox_Fill(this.cells);
            else
                Indeks.SelectedItem = _Article.IndeksofArticle;


        }
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

        #endregion

        #region KEYBOARD
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
        #endregion

        #region STEPBAR
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
        #endregion
    }
}
