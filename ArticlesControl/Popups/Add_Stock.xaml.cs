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
    /// Logika interakcji dla klasy Add_Stock.xaml
    /// </summary>
    public partial class Add_Stock : Window
    {
        #region Fields
        private readonly BlurEffect blur = new();
        public bool iscontinue = false;
        public ResourceDictionary dict = new();
        public Dictionary<string, string> dictionary;
        private readonly Dictionary<string, string> custom_columns;
        private readonly Dictionary<string, Tuple<string, Type>> cells = new();
        private bool isEdit { get; set; }
        DataRow DataToEdit;
        private readonly DataTable Polki = Connection("Kar_Polki");
        private readonly DataTable Lokalizacje = Connection("Kar_Lokalizacje");
        private readonly List<string?> list = new();
        private readonly Add_DataTableAdapters.Kar_Artykuly_SELECTTableAdapter Add_Kar_Artykul_ta = new();
        private readonly Add_DataTableAdapters.StanyMagazynowe_EDITTableAdapter adapter1 = new();
        #endregion

        #region Constructors
        public Add_Stock(ResourceDictionary resourceDictionary, Dictionary<string, string> lang, int count)
        {
            dictionary = lang;
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);
            DataContext = this;
            InitializeComponent();
            isEdit = false;
            Localization_Fill();
            FillCanvas(new List<object>());
            DataTable Data = Add_Kar_Artykul_ta.GetData();
            list = Data.Rows.OfType<DataRow>()
    .Select(dr => dr.Field<string>("Indeks")).ToList();
            Indeks.ItemsSource = list;
            custom_columns = new Dictionary<string, string>();
        }
        //Constructor for add new row, when selected row some fields are filled
        public Add_Stock(ResourceDictionary resourceDictionary, Dictionary<string, string> lang, Dictionary<string, Tuple<string, Type>> cells, bool isSuggested, Dictionary<string, string> custom_columns)
        {
            this.cells = cells;
            this.custom_columns = custom_columns;
            dictionary = lang;
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);
            DataContext = this;
            InitializeComponent();
            DataToEdit = (DataRow)(adapter1.GetData().Select(String.Format("[Indeks] = '" + cells["Indeks"].Item1.ToString() + "'")))[0];
            TextBox_Fill(true);
            isEdit = false;
            Localization_Fill(isEdit);
            DataTable Data = Add_Kar_Artykul_ta.GetData();
            list = Data.Rows.OfType<DataRow>()
    .Select(dr => dr.Field<string>("Indeks")).ToList();
            Indeks.ItemsSource = list;
        }
        //Constructor for edit row
        public Add_Stock(ResourceDictionary resourceDictionary, Dictionary<string, string> lang, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns)
        {
            this.cells = cells;
            this.custom_columns = custom_columns;
            dictionary = lang;
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);
            DataContext = this;
            InitializeComponent();
            DataToEdit = (DataRow)(adapter1.GetData().Select(String.Format("[Indeks] = '" + cells["Indeks"].Item1.ToString() + "'")))[0];
            DataTable Data = Add_Kar_Artykul_ta.GetData();
            list = Data.Rows.OfType<DataRow>()
    .Select(dr => dr.Field<string>("Indeks")).ToList();
            Localization_Fill(isEdit);
            Indeks.ItemsSource = list;
            isEdit = true;
            TextBox_Fill();
            localization_grid.Visibility = Visibility.Hidden;
            ID_Tray.IsEnabled = false;
            ID_Localization.IsEnabled = false;
            Batch.IsEnabled = false;
            Attestation.IsEnabled = false;
            Melt.IsEnabled = false;
            
            ID_Tray.IsEditable = false;
            ID_Localization.IsEditable = false;
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
        private void TextBox_Fill()
        {
            Indeks.IsEnabled = false;
            Indeks.SelectedValue = DataToEdit["Indeks"].ToString();
            //Indeks.Text = cells["Indeks"].Item1;
            Quantity.Text = DataToEdit["Ilosc"].ToString();
            Reservation.Text = DataToEdit["Rezerwacja"].ToString();
            Batch.Text = DataToEdit["Partia"].ToString();
            Attestation.Text = DataToEdit["Atest"].ToString();
            Melt.Text = DataToEdit["Wytop"].ToString();
            Note.Text = DataToEdit["Notatka"].ToString();
            Atr1.Text = DataToEdit["Atr1"].ToString();
            Atr2.Text = DataToEdit["Atr2"].ToString();
            Atr3.Text = DataToEdit["Atr3"].ToString();
        }
        private void TextBox_Fill(bool isSuggested)
        {
            Quantity.Text = DataToEdit["Ilosc"].ToString();
            Reservation.Text = DataToEdit["Rezerwacja"].ToString();
            Batch.Text = DataToEdit["Partia"].ToString();
            Attestation.Text = DataToEdit["Atest"].ToString();
            Melt.Text = DataToEdit["Wytop"].ToString();
            Note.Text = DataToEdit["Notatka"].ToString();
            Atr1.Text = DataToEdit["Atr1"].ToString();
            Atr2.Text = DataToEdit["Atr2"].ToString();
            Atr3.Text = DataToEdit["Atr3"].ToString();
        }
        public static DataTable Connection(string db)
        {
            //string ip = String.Empty;
            //geting current ip
            //IPHostEntry IPHost = Dns.GetHostEntry(Dns.GetHostName());
            //foreach (var ipAddress in IPHost.AddressList)
            //{
            //    ip = ipAddress.ToString();
            //}
            //command for connectionString
            string connectionString = "Server=192.168.0.200\\testinstance;Database=SmartWMS;User ID=wms;Password=1";
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
        private void Localization_Fill()
        {
            List<object> list1 = new();
            for (int i = 0; i < Polki.Rows.Count; i++)
            {
                list1.Add(Polki.Rows[i]["NrPolki"]);
            }
            list1.Sort();
            ID_Tray.ItemsSource = list1;
            ID_Tray.SelectionChanged += ID_Tray_SelectionChanged;
        }
        private void Localization_Fill(bool isEdit)
        {
            List<object> list1 = new();
            for (int i = 0; i < Polki.Rows.Count; i++)
            {
                list1.Add(Polki.Rows[i]["NrPolki"]);
            }
            list1.Sort();
            ID_Tray.ItemsSource = list1;
            ID_Tray.SelectedValue = Convert.ToInt32(DataToEdit["ID_Polki"].ToString());
            ID_Tray.SelectionChanged += ID_Tray_SelectionChanged;
            List<object> list2 = new();
            for (int i = 0; i < Lokalizacje.Rows.Count; i++)
            {
                if (ID_Tray.SelectedValue.ToString() == Lokalizacje.Rows[i]["ID_Polka"].ToString())
                {
                    list2.Add(Lokalizacje.Rows[i]["Symbol"]);
                }
            }
            list2.Sort();
            ID_Localization.ItemsSource = list2;
            ID_Localization.SelectedIndex = Convert.ToInt32(DataToEdit["ID_Lokalizacji"].ToString());
            FillCanvas(list2);
        }
        private void ID_Tray_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ID_Localization.ItemsSource = null;
            List<object> list1 = new();
            for (int i = 0; i < Lokalizacje.Rows.Count; i++)
            {
                if (ID_Tray.SelectedValue is not null)
                {
                    if (ID_Tray.SelectedValue.ToString() == Lokalizacje.Rows[i]["ID_Polka"].ToString())
                    {
                        list1.Add(Lokalizacje.Rows[i]["Symbol"]);

                    }
                    continue;
                }
                break;
            }
            list1.Sort();
            ID_Localization.ItemsSource = list1;
            FillCanvas(list1);
            ID_Localization.SelectedIndex = 0;
        }
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (Indeks.SelectedValue is null)
            {
                e.Handled = true;

                ColorAnimation flash = new(Colors.White, Colors.Red, new Duration(TimeSpan.FromMilliseconds(75D))) { AutoReverse = true, RepeatBehavior = new RepeatBehavior(3D) };
                Indeks.Background = new SolidColorBrush(Colors.White); // no error???
                Indeks.Background.BeginAnimation(SolidColorBrush.ColorProperty, flash);
                return;
            }
            if (ID_Tray.SelectedValue == null)
            {
                e.Handled = true;

                ColorAnimation flash = new(Colors.White, Colors.Red, new Duration(TimeSpan.FromMilliseconds(75D))) { AutoReverse = true, RepeatBehavior = new RepeatBehavior(3D) };
                ID_Tray.Background = new SolidColorBrush(Colors.White); // no error???
                ID_Tray.Background.BeginAnimation(SolidColorBrush.ColorProperty, flash);
                return;
            }
            if (ID_Localization.SelectedValue == null)
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
            int temp1 = Convert.ToInt32(Lokalizacje.Select(string.Format("Symbol = '{0}' AND ID_Polka = {1}", ID_Localization.SelectedValue, ID_Tray.SelectedValue))[0][0]);
            _ = decimal.TryParse(Quantity.Text, out decimal temp3);
            _ = decimal.TryParse(Reservation.Text, out decimal temp4);
            DateTime data = DateTime.Now;

            Stock stock = new(Indeks.Text, temp1, Convert.ToInt32(ID_Tray.SelectedValue), temp3, temp4, Batch.Text, Attestation.Text, Melt.Text, Atr1.Text, Atr2.Text, Atr3.Text, data, Note.Text, "Default Operator");
            stock.Update();
            if (stock.isOk)
            {
                iscontinue = true;
            }
        }
        private void Insert_Stock()
        {
            int temp1 = Convert.ToInt32(Lokalizacje.Select(string.Format("Symbol = '{0}' AND ID_Polka = {1}", ID_Localization.SelectedValue, ID_Tray.SelectedValue))[0][0]);
            int temp2 = 0;
            if (ID_Tray.SelectedValue is not null)
            {
                for (int i = 0; i < Polki.Rows.Count; i++)
                {
                    if (ID_Tray.SelectedValue.ToString() == Polki.Rows[i]["NrPolki"].ToString())
                    {
                        temp2 = Convert.ToInt32(Polki.Rows[i]["ID_Polki"]);
                        break;
                    }
                }
            }
            _ = decimal.TryParse(Quantity.Text, out decimal temp3);
            _ = decimal.TryParse(Reservation.Text, out decimal temp4);
            DateTime data = DateTime.Now;

            Stock stock = new(Indeks.Text, temp1, temp2, temp3, temp4, Batch.Text, Attestation.Text, Melt.Text, Atr1.Text, Atr2.Text, Atr3.Text, data, Note.Text, "Default Operator");
            stock.Insert();
            if (stock.isOk)
            {
                iscontinue = true;
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
            if (textBox != null)
            {
                if (textBox.Tag.ToString() == "Only_Numbers")
                {
                    textBox.BorderThickness = new Thickness(0, 0, 0, 0);
                    if (!numberlist.Contains(e.Key) || (textBox.Text.Contains('.') && e.Key == Key.OemPeriod) || (e.Key == Key.OemPeriod && textBox.Text.Length == 0) || (textBox.Text.Contains(',') && e.Key == Key.OemComma) || (textBox.Text.Contains(',') && e.Key == Key.OemPeriod))
                    {
                        e.Handled = true;

                        ColorAnimation flash = new(Colors.White, Colors.Red, new Duration(TimeSpan.FromMilliseconds(75D))) { AutoReverse = true, RepeatBehavior = new RepeatBehavior(3D) };
                        textBox.Background = new SolidColorBrush(Colors.White); // no error???
                        textBox.Background.BeginAnimation(SolidColorBrush.ColorProperty, flash);
                        HandyControl.Controls.Growl.Warning(dictionary["Integer_validation"]);
                    }
                }
            }
        }
        public static T FindChild<T>(DependencyObject parent, string childName)
   where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null)
            {
                return null;
            }

            T? foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject? child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T? childType = child as T;
                if (childType is null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild is not null)
                    {
                        break;
                    }
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    FrameworkElement? frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement is not null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
        private void FillCanvas(List<object> list)
        {
            Localization_Visualizer.Children.Clear();
            double y = 0;
            double x = 0;
            if (list.Count == 1)
            {
                y = 5;
                x = 130;
            }
            else if (list.Count == 2)
            {
                y = 5;
                x = 70;
            }
            else if (list.Count == 3)
            {
                y = 5;
                x = 5;
            }
            foreach (object? item in list)
            {
                Button btn = new();
                btn.Click += Btn_Click;
                btn.Content = item.ToString();
                btn.Width = 95;
                btn.Height = 155;
                Canvas.SetLeft(btn, x);
                Canvas.SetTop(btn, y);
                Localization_Visualizer.Children.Add(btn);
                x += 105;
                if (isEdit)
                {
                    btn.IsEnabled = false;
                }
            }
        }
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            ID_Localization.SelectedValue = btn.Content;
        }
        private void Batch_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Batch.IsReadOnly)
            {
                if (HandyControl.Controls.MessageBox.Show(dictionary["Alert_BatchAutoID"], Indeks.Text, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Batch.Text = string.Empty;
                    Batch.IsReadOnly = false;
                }
            }

        }
        private void Indeks_MouseDoubleClick(object sender, RoutedEventArgs ee)
        {
            //Windows.Select_an_article _Article = new(dict, dictionary);
            //_Article.Owner = Window.GetWindow(this);
            //Opacity = 0.5;
            //Effect = blur;
            //_Article.ShowDialog();
            //Opacity = 1;
            //Effect = null;
            //Indeks.Text = _Article.IndeksofArticle;
        }
        #endregion
    }
}
