using System;
using System.Data;
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
using System.Windows.Shapes;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace ArticlesControl.Formatki
{
    /// <summary>
    /// Interaction logic for Add_Order.xaml
    /// </summary>
    public partial class Add_Order : Window
    {
        private readonly BlurEffect blur = new();
        public ResourceDictionary dict = new();
        public Dictionary<string, string> dictionary;
        private readonly Dictionary<string, string> custom_columns;
        private readonly Dictionary<string, Tuple<string, Type>> cells = new();

        private readonly Add_DataTableAdapters.ZleceniaLinie_SELECTTableAdapter Add_ZleceniaLinie_Select = new();
        private string culturestring;
        private  CultureInfo culture;
        //Add constructor
        public Add_Order(ResourceDictionary resourceDictionary, Dictionary<string, string> lang, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns)
        {
            InitializeComponent();
            datagrid.Hide_Menu();
            
            this.cells = cells;
            this.custom_columns = custom_columns;
            dictionary = lang;
            dict = resourceDictionary;
            Add_Data.ZleceniaLinie_SELECTDataTable? Data = Add_ZleceniaLinie_Select.GetData("");
            datagrid.additionaldatatable = Data;

            List<string> list1 = new();
            DataTable dt1 = Data;
            foreach (DataColumn item in dt1.Columns)
            {
                list1.Add(item.ColumnName.ToString());
            }
            datagrid.columnnames = list1;
            datagrid.Maindatatable = Data;
            datagrid.Load();
            datagrid.CallHeaders();
            datagrid.IsEnabled = false;
            datagrid.isOrderLine = true;
            //datagrid.SelectedRowEvent += Datagrid_SelectedRowEvent;
        }
        //Add suggested constructor
        public Add_Order(ResourceDictionary resourceDictionary, Dictionary<string, string> lang, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns, int count)
        {
            culturestring = "en-EN";
            culture = CultureInfo.GetCultureInfo(culturestring);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            CultureInfo? cultures = new(culturestring);
            Thread.CurrentThread.CurrentCulture = cultures;
            Thread.CurrentThread.CurrentUICulture = cultures;
            InitializeComponent();
            datagrid.Hide_Menu();
            this.cells = cells;
            this.custom_columns = custom_columns;
            dictionary = lang;
            dict = resourceDictionary;
            Add_Data.ZleceniaLinie_SELECTDataTable? Data = Add_ZleceniaLinie_Select.GetData(cells["NrZlecenia"].Item1.ToString());
            datagrid.additionaldatatable = Data;

            List<string> list1 = new();
            DataTable dt1 = Data;
            foreach (DataColumn item in dt1.Columns)
            {
                list1.Add(item.ColumnName.ToString());
            }
            datagrid.columnnames = list1;
            datagrid.Maindatatable = Data;
            datagrid.Load();
            datagrid.CallHeaders();
            datagrid.isOrderLine = true;
            //datagrid.SelectedRowEvent += Datagrid_SelectedRowEvent;
            LoadTextboxes();
        }

        private void LoadTextboxes()
        {
            string Formate = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            string time = CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;
            //Order_Date.DisplayDate = DateTime.Parse(cells["DataZlecenia"].Item1.ToString());
            DateTime oldtime = DateTime.Now;
            if (DateTime.TryParse(cells["DataZlecenia"].Item1, out oldtime)) { Order_Date.SelectedDate = oldtime; }

            External_numer.Text = cells["NrZewnetrzny"].Item1.ToString();
            Client.Text = cells["Klient"].Item1.ToString();
            Description.Text = cells["Opis"].Item1.ToString();
            Status.Text = cells["Status"].Item1.ToString();
            Priority.Text = cells["Priorytet"].Item1.ToString();
            Type.Text = cells["Typ"].Item1.ToString();
            Order_source.Text = cells["ZrodloZlecenia"].Item1.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            culturestring = "pl-PL";
            culture = CultureInfo.GetCultureInfo(culturestring);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            CultureInfo? cultures = new(culturestring);
            Thread.CurrentThread.CurrentCulture = cultures;
            Thread.CurrentThread.CurrentUICulture = cultures;

            this.Close();

        }
        private void Batch_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Order_Number.IsReadOnly)
            {
                if (HandyControl.Controls.MessageBox.Show(dictionary["Alert_BatchAutoID"]," ", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Order_Number.Text = string.Empty;
                    Order_Number.IsReadOnly = false;
                }
            }

        }
        private void Order_Number_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Order_Number.IsFocused == false && Order_Number.Text == "")
            {
                Order_Number.Text = "[AUTO_ID]";
                Order_Number.IsReadOnly = true;
            }
        }
        private void TextBox_TextChanged(object sender, KeyEventArgs e)
        {
            HandyControl.Controls.TextBox? textBox = sender as HandyControl.Controls.TextBox ?? new();
            List<Key> numberlist = new()
            {
                Key.NumPad0,
                Key.NumPad1,
                Key.NumPad2,
                Key.NumPad3,
                Key.NumPad4,
                Key.NumPad5,
                Key.NumPad6,
                Key.NumPad7,
                Key.NumPad8,
                Key.NumPad9,
                Key.D0,
                Key.D1,
                Key.D2,
                Key.D3,
                Key.D4,
                Key.D5,
                Key.D6,
                Key.D7,
                Key.D8,
                Key.D9,
                Key.Back,
                Key.Enter,
                Key.Escape,
                Key.Left,
                Key.Right,
                Key.Tab,
                Key.OemComma
            };

            char[] special = { '!', '@', '#', '$', '%', '^', '&', '*', '(', ')' };
            if (textBox != null)
            {
                if (textBox.Tag.ToString() == "Only_Numbers")
                {
                    textBox.BorderThickness = new Thickness(0, 0, 0, 0);
                    if (System.Text.RegularExpressions.Regex.IsMatch(textBox.Text, "[^0-9]") || Keyboard.GetKeyStates(Key.LeftShift) == KeyStates.Down || Keyboard.GetKeyStates(Key.RightShift) == KeyStates.Down || !numberlist.Contains(e.Key) || (textBox.Text.Contains('.') && e.Key == Key.OemPeriod) || (e.Key == Key.OemPeriod && textBox.Text.Length == 0) || (textBox.Text.Contains(',') && e.Key == Key.OemComma) || (textBox.Text.Contains(',') && e.Key == Key.OemPeriod))
                    {
                        e.Handled = true;

                        ColorAnimation flash = new(Colors.White, Colors.Red, new Duration(TimeSpan.FromMilliseconds(75D))) { AutoReverse = true, RepeatBehavior = new RepeatBehavior(3D) };
                        textBox.Background = new SolidColorBrush(Colors.White); // no error???
                        textBox.Background.BeginAnimation(SolidColorBrush.ColorProperty, flash);

                    }
                }
            }
        }
        private void Datagrid_SelectedRowEvent(string value)
        {
            if (value != "")
            {
                Edit_Data.IsEnabled = true;
                Delete_Data.IsEnabled = true;
            }
            else
            {
                Edit_Data.IsEnabled = false;
                Delete_Data.IsEnabled = false;
            }
        }
        private void Add_Data_Click(object sender, RoutedEventArgs e)
        {
            if (datagrid.Selectedrow is not null)
            {
                datagrid.GetCells();
                Opacity = 0.5;
                Effect = blur;
                Add_OrderLine add_OrderLine = new(dict, dictionary, datagrid.cells);
                add_OrderLine.Owner = Window.GetWindow(this);
                add_OrderLine.ShowDialog();
                Opacity = 1;
                Effect = null;
            }
            else
            {
                Opacity = 0.5;
                Effect = blur;
                Add_OrderLine add_OrderLine = new(dict, dictionary);
                add_OrderLine.Owner = Window.GetWindow(this);
                add_OrderLine.ShowDialog();
                Opacity = 1;
                Effect = null;
            }
        }
        private void Edit_Data_Click(object sender, RoutedEventArgs e)
        {
            datagrid.GetCells();
            Opacity = 0.5;
            Effect = blur;
            Add_OrderLine add_OrderLine = new(dict, dictionary, datagrid.cells, true);
            add_OrderLine.Owner = Window.GetWindow(this);
            add_OrderLine.ShowDialog();
            Opacity = 1;
            Effect = null;
        }
    }
}