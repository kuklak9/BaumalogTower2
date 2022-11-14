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
    public partial class Add_Article : BTPControlLibrary.MainWindow
    {
        #region Fields
        private readonly BlurEffect blur = new();
        public bool iscontinue = false;
        public ResourceDictionary dict = new();
        public Dictionary<string, string> dictionary;
        private readonly Dictionary<string, string> custom_columns;
        private readonly Dictionary<string, Tuple<string, Type>> cells = new();
        private readonly DataTable Gatunki = Connection("Kar_Gatunki");
        private readonly DataTable Jednostki = Connection("Kar_Jednostki");
        private readonly DataTable Kategorie = Connection("Kar_Kategorie");
        private readonly DataTable KlasySkładowania = Connection("Kar_KlasySkladowania");

        private bool isEdit { get; set; }
        DataRow DataToEdit;
        #endregion

        #region Constructors
        //Constructor for add new row
        public Add_Article(ResourceDictionary resourceDictionary, Dictionary<string, string> lang, int count)
        {
            dictionary = lang;
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);
            DataContext = this;
            InitializeComponent();
            Combobox_Fill();
            isEdit = false;
            custom_columns = new Dictionary<string, string>();
        }
        //Constructor for add new row, when selected row some fields are filled
        public Add_Article(ResourceDictionary resourceDictionary, Dictionary<string, string> lang, Dictionary<string, Tuple<string, Type>> cells, bool isSuggested, Dictionary<string, string> custom_columns)
        {
            this.cells = cells;
            this.custom_columns = custom_columns;
            dictionary = lang;
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);
            DataContext = this;
            InitializeComponent();
            DataToEdit = (DataRow)(_dbWMS.Add_Kar_Artykul_ta.GetData().Select(String.Format("[Indeks] = '" + cells["Indeks"].Item1.ToString() + "'")))[0];

            Textbox_Fill(true);
            Combobox_Fill(true);
            isEdit = false;
        }
        //Constructor for edit row
        public Add_Article(ResourceDictionary resourceDictionary, Dictionary<string, string> lang, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns)
        {
            this.cells = cells;
            this.custom_columns = custom_columns;
            dictionary = lang;
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);
            DataContext = this;
            InitializeComponent();
            DataToEdit = (DataRow)(_dbWMS.Add_Kar_Artykul_ta.GetData().Select(String.Format("[Indeks] = '" + cells["Indeks"].Item1.ToString() + "'")))[0];
            Textbox_Fill();
            Combobox_Fill(true);
            isEdit = true;
        }
        #endregion

        #region Functions
        private void Textbox_Fill()
        {
            Indeks.IsEnabled = false;
            Indeks.Text = DataToEdit["Indeks"].ToString();
            Nazwa.Text = DataToEdit["Nazwa"].ToString();
            Opis.Text = DataToEdit["Opis"].ToString();
            Atr1.Text = DataToEdit["Atr1"].ToString();
            Atr2.Text = DataToEdit["Atr2"].ToString();
            //Atr3.Text = DataToEdit["Atr3"].ToString();
            Wymiar1.Text = DataToEdit["Wymiar1"].ToString();
            Wymiar2.Text = DataToEdit["Wymiar2"].ToString();
            Wymiar3.Text = DataToEdit["Wymiar3"].ToString();
            Wymiar4.Text = DataToEdit["Wymiar4"].ToString();
            Wymiar5.Text = DataToEdit["Wymiar5"].ToString();
            Wymiar6.Text = DataToEdit["Wymiar6"].ToString();
            Wymiar7.Text = DataToEdit["Wymiar7"].ToString();
            Wymiar8.Text = DataToEdit["Wymiar8"].ToString();
            StanMinimalny.Text = DataToEdit["StanMinimalny"].ToString();
            Stan.Text = DataToEdit["Stan"].ToString();
            Rezerwacja.Text = DataToEdit["Rezerwacja"].ToString();
            WagaJednostkowa.Text = DataToEdit["WagaJednostkowa"].ToString();
        }
        private void Textbox_Fill(bool isSuggested)
        {
            Atr1.Text = DataToEdit["Atr1"].ToString();
            Atr2.Text = DataToEdit["Atr2"].ToString();
            //Atr3.Text = DataToEdit["Atr3"].ToString();
            Wymiar1.Text = DataToEdit["Wymiar1"].ToString();
            Wymiar2.Text = DataToEdit["Wymiar2"].ToString();
            Wymiar3.Text = DataToEdit["Wymiar3"].ToString();
            Wymiar4.Text = DataToEdit["Wymiar4"].ToString();
            Wymiar5.Text = DataToEdit["Wymiar5"].ToString();
            Wymiar6.Text = DataToEdit["Wymiar6"].ToString();
            Wymiar7.Text = DataToEdit["Wymiar7"].ToString();
            Wymiar8.Text = DataToEdit["Wymiar8"].ToString();
            StanMinimalny.Text = DataToEdit["StanMinimalny"].ToString();
            Stan.Text = DataToEdit["Stan"].ToString();
            Rezerwacja.Text = DataToEdit["Rezerwacja"].ToString();
            WagaJednostkowa.Text = DataToEdit["WagaJednostkowa"].ToString();
        }
        private void Combobox_Fill(bool isEdit)
        {
            //DataTable Gatunki = Connection("Kar_Gatunki");
            //DataTable Jednostki = Connection("Kar_Jednostki");
            //DataTable Kategorie = Connection("Kar_Kategorie");
            //DataTable KlasySkładowania = Connection("Kar_KlasySkladowania");
            //Button bt1 = new();
            //bt1.Content = dictionary["Add_item"];
            //Button bt2 = new();
            //bt2.Content = dictionary["Add_item"];
            //bt2.Click += Bt2_Click;
            Button bt3 = new();
            bt3.Content = dictionary["Add_item"];
            bt3.Click += Bt3_Click;
            //Button bt4 = new();
            //bt4.Content = dictionary["Add_item"];
            //bt4.Click += Bt4_Click;
            List<ComboBox> comboboxes = new();
            List<object> list = new();
            List<object> list2 = new();
            List<object> list3 = new();
            List<object> list4 = new();
            for (int i = 0; i < KlasySkładowania.Rows.Count; i++)
            {
                list.Add(KlasySkładowania.Rows[i]["ID_KlasySkladowania"]);
            }
            list.Sort();
            //list.Add(bt1);
            ID_KlasySkladowania.ItemsSource = list;
            for (int i = 0; i < KlasySkładowania.Rows.Count; i++)
            {
                if (DataToEdit["ID_KlasySkladowania"].ToString() == KlasySkładowania.Rows[i]["ID_KlasySkladowania"].ToString())
                {
                    ID_KlasySkladowania.SelectedValue = KlasySkładowania.Rows[i]["ID_KlasySkladowania"];
                    break;
                }
            }
            for (int i = 0; i < Gatunki.Rows.Count; i++)
            {
                list3.Add(Gatunki.Rows[i]["Nazwa"]);
            }
            list3.Sort();
            list3.Add(bt3);
            ID_Gatunku.ItemsSource = list3;
            for (int i = 0; i < Gatunki.Rows.Count; i++)
            {
                if (DataToEdit["Gatunek"].ToString() == Gatunki.Rows[i]["Nazwa"].ToString())
                {
                    ID_Gatunku.SelectedValue = Gatunki.Rows[i]["Nazwa"];
                    break;
                }
            }
            for (int i = 0; i < Kategorie.Rows.Count; i++)
            {
                list2.Add(Kategorie.Rows[i]["Nazwa"]);
            }
            list2.Sort();
            //list2.Add(bt2);
            ID_Kategorii.ItemsSource = list2;
            for (int i = 0; i < Kategorie.Rows.Count; i++)
            {
                if (DataToEdit["Kategoria"].ToString() == Kategorie.Rows[i]["Nazwa"].ToString())
                {
                    ID_Kategorii.SelectedValue = Kategorie.Rows[i]["Nazwa"];
                    break;
                }
            }
            for (int i = 0; i < Jednostki.Rows.Count; i++)
            {
                list4.Add(Jednostki.Rows[i]["Symbol"]);
            }
            list4.Sort();
            //list4.Add(bt4);
            ID_Jednostki.ItemsSource = list4;
            for (int i = 0; i < Jednostki.Rows.Count; i++)
            {
                if (DataToEdit["ID_Jednostki"].ToString() == Jednostki.Rows[i]["ID_Jednostki"].ToString())
                {
                    ID_Jednostki.SelectedValue = Jednostki.Rows[i]["Symbol"];
                    break;
                }
            }
            Rotation_class_combobox.Items.Add("A");
            Rotation_class_combobox.Items.Add("B");
            Rotation_class_combobox.Items.Add("C");
            Rotation_class_combobox.SelectedValue = DataToEdit["KlasaRotacji"].ToString();
            comboboxes.Add(ID_KlasySkladowania);
            comboboxes.Add(ID_Gatunku);
            comboboxes.Add(ID_Kategorii);
            comboboxes.Add(ID_Jednostki);
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
        public void Combobox_Fill()
        {
            //DataTable Gatunki = Connection("Kar_Gatunki");
            //DataTable Jednostki = Connection("Kar_Jednostki");
            //DataTable Kategorie = Connection("Kar_Kategorie");
            //DataTable KlasySkładowania = Connection("Kar_KlasySkladowania");
            //Button bt1 = new();
            //bt1.Content = dictionary["Add_item"];
            //Button bt2 = new();
            //bt2.Content = dictionary["Add_item"];
            //bt2.Click += Bt2_Click;
            Button bt3 = new();
            bt3.Content = dictionary["Add_item"];
            bt3.Click += Bt3_Click;
            //Button bt4 = new();
            //bt4.Content = dictionary["Add_item"];
            //bt4.Click += Bt4_Click;
            List<object> list = new();
            List<object> list2 = new();
            List<object> list3 = new();
            List<object> list4 = new();
            for (int i = 0; i < KlasySkładowania.Rows.Count; i++)
            {
                list.Add(KlasySkładowania.Rows[i]["ID_KlasySkladowania"]);
            }
            list.Sort();
            //list.Add(bt1);
            ID_KlasySkladowania.ItemsSource = list;
            for (int i = 0; i < Kategorie.Rows.Count; i++)
            {
                list2.Add(Kategorie.Rows[i]["Nazwa"]);
            }
            list2.Sort();
            //list2.Add(bt2);
            ID_Kategorii.ItemsSource = list2;
            for (int i = 0; i < Gatunki.Rows.Count; i++)
            {
                list3.Add(Gatunki.Rows[i]["Nazwa"]);
            }
            list3.Sort();
            list3.Add(bt3);
            ID_Gatunku.ItemsSource = list3;
            for (int i = 0; i < Jednostki.Rows.Count; i++)
            {
                list4.Add(Jednostki.Rows[i]["Nazwa"]);
            }
            list4.Sort();
            //list4.Add(bt4);
            ID_Jednostki.ItemsSource = list4;
            Rotation_class_combobox.Items.Add("A");
            Rotation_class_combobox.Items.Add("B");
            Rotation_class_combobox.Items.Add("C");

        }
        //private void Bt4_Click(object sender, RoutedEventArgs e)
        //{
        //    Add_Unit add_Unit = new(dict, dictionary);
        //    add_Unit.Owner = Window.GetWindow(this);
        //    Opacity = 0.5;
        //    Effect = blur;
        //    add_Unit.ShowDialog();
        //    Opacity = 1;
        //    Effect = null;
        //}
        //private void Bt2_Click(object sender, RoutedEventArgs e)
        //{
        //    Add_Category add_Category = new(dict, dictionary);
        //    add_Category.Owner = Window.GetWindow(this);
        //    Opacity = 0.5;
        //    Effect = blur;
        //    add_Category.ShowDialog();
        //    Opacity = 1;
        //    Effect = null;
        //}
        private void Bt3_Click(object sender, RoutedEventArgs e)
        {
            //Add_Type add_type = new(dict, dictionary);
            //add_type.Owner = Window.GetWindow(this);
            //Opacity = 0.5;
            //Effect = blur;
            //add_type.ShowDialog();
            //Opacity = 1;
            //Effect = null;
        }
        //private void TextBox_TextChanged(object sender, KeyEventArgs e)
        //{
        //    HandyControl.Controls.TextBox? textBox = sender as HandyControl.Controls.TextBox ?? new();
        //    string name = textBox.Name;
        //    TextBox foundTextBox =
        //        FindChild<HandyControl.Controls.TextBox>(this, name);
        //    List<Key> numberlist = new()
        //    {
        //        Key.NumPad0,
        //        Key.NumPad1,
        //        Key.NumPad2,
        //        Key.NumPad3,
        //        Key.NumPad4,
        //        Key.NumPad5,
        //        Key.NumPad6,
        //        Key.NumPad7,
        //        Key.NumPad8,
        //        Key.NumPad9,
        //        Key.D0,
        //        Key.D1,
        //        Key.D2,
        //        Key.D3,
        //        Key.D4,
        //        Key.D5,
        //        Key.D6,
        //        Key.D7,
        //        Key.D8,
        //        Key.D9,
        //        Key.Back,
        //        Key.Enter,
        //        Key.Escape,
        //        Key.Left,
        //        Key.Right,
        //        Key.Tab,
        //        Key.OemComma
        //    };
        //    if (textBox != null)
        //    {
        //        if (textBox.Tag.ToString() == "Only_Numbers")
        //        {
        //            textBox.BorderThickness = new Thickness(0, 0, 0, 0);
        //            if (!numberlist.Contains(e.Key) || (textBox.Text.Contains('.') && e.Key == Key.OemPeriod) || (e.Key == Key.OemPeriod && textBox.Text.Length == 0) || (textBox.Text.Contains(',') && e.Key == Key.OemComma) || (textBox.Text.Contains(',') && e.Key == Key.OemPeriod))
        //            {
        //                e.Handled = true;

        //                ColorAnimation flash = new(Colors.White, Colors.Red, new Duration(TimeSpan.FromMilliseconds(75D))) { AutoReverse = true, RepeatBehavior = new RepeatBehavior(3D) };
        //                textBox.Background = new SolidColorBrush(Colors.White); // no error???
        //                textBox.Background.BeginAnimation(SolidColorBrush.ColorProperty, flash);
        //                HandyControl.Controls.Growl.Warning(dictionary["Integer_validation"]);
        //            }
        //        }
        //    }
        //}
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isEdit)
                {
                    Update_Article();
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
                    Insert_Article();
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
            catch (Exception E)
            {
                HandyControl.Controls.MessageBox.Show("Error", E.Message, MessageBoxButton.OK);
            }
        }
        private void Insert_Article()
        {
            int temp = 0;
            if (ID_Jednostki.SelectedValue is not null)
            {
                for (int i = 0; i < Jednostki.Rows.Count; i++)
                {
                    if (ID_Jednostki.SelectedValue.ToString() == Jednostki.Rows[i]["Nazwa"].ToString())
                    {
                        temp = Convert.ToInt32(Jednostki.Rows[i]["ID_Jednostki"].ToString());
                        break;
                    }
                }
            }
            int temp1 = 0;
            if (ID_Kategorii.SelectedValue is not null)
            {
                for (int i = 0; i < Kategorie.Rows.Count; i++)
                {
                    if (ID_Kategorii.SelectedValue.ToString() == Kategorie.Rows[i]["Nazwa"].ToString())
                    {
                        temp1 = Convert.ToInt32(Kategorie.Rows[i]["ID_Kategorii"].ToString());
                        break;
                    }
                }
            }
            int temp2 = 0;
            if (ID_Gatunku.SelectedValue is not null)
            {
                for (int i = 0; i < Gatunki.Rows.Count; i++)
                {
                    if (ID_Gatunku.SelectedValue.ToString() == Gatunki.Rows[i]["Nazwa"].ToString())
                    {
                        temp2 = Convert.ToInt32(Gatunki.Rows[i]["ID_Gatunku"].ToString());
                        break;
                    }
                }
            }
            int temp3 = 0;
            if (ID_KlasySkladowania.SelectedValue is not null)
            {
                for (int i = 0; i < KlasySkładowania.Rows.Count; i++)
                {
                    if (ID_KlasySkladowania.SelectedValue.ToString() == KlasySkładowania.Rows[i]["ID_KlasySkladowania"].ToString())
                    {
                        temp3 = Convert.ToInt32(KlasySkładowania.Rows[i]["ID_KlasySkladowania"].ToString());
                        break;
                    }
                }
            }
            //int.TryParse(ID_KlasySkladowania.SelectedIndex.ToString(), out int temp3);
            _ = decimal.TryParse(Wymiar1.Text, out decimal temp4);
            _ = decimal.TryParse(Wymiar2.Text, out decimal temp5);
            _ = decimal.TryParse(Wymiar3.Text, out decimal temp6);
            _ = decimal.TryParse(Wymiar4.Text, out decimal temp7);
            _ = decimal.TryParse(Wymiar5.Text, out decimal temp8);
            _ = decimal.TryParse(Wymiar6.Text, out decimal temp9);
            _ = decimal.TryParse(Wymiar7.Text, out decimal temp10);
            _ = decimal.TryParse(Wymiar8.Text, out decimal temp11);
            _ = decimal.TryParse(WagaJednostkowa.Text, out decimal temp12);
            _ = decimal.TryParse(Stan.Text, out decimal temp13);
            _ = decimal.TryParse(Rezerwacja.Text, out decimal temp14);
            _ = decimal.TryParse(StanMinimalny.Text, out decimal temp15);
            string? rotationclass = string.Empty;
            if (Rotation_class_combobox.SelectedValue is not null)
            {
                rotationclass = Rotation_class_combobox.SelectedValue.ToString() ?? "";
            }

            Article new_Article = new(_dbWMS, Indeks.Text, Nazwa.Text, Opis.Text, Atr1.Text, Atr2.Text, "",
                rotationclass, temp, temp1, temp2, temp3, temp4, temp5, temp6, temp7, temp8, temp9, temp10, temp11, temp12, temp13, temp14, temp15);

            new_Article.Insert();
            if (new_Article.IsOk)
            {
                iscontinue = true;
            }
        }
        private void Update_Article()
        {
            int temp = 0;
            if (ID_Jednostki.SelectedValue is not null)
            {
                for (int i = 0; i < Jednostki.Rows.Count; i++)
                {
                    if (ID_Jednostki.SelectedValue.ToString() == Jednostki.Rows[i]["Symbol"].ToString())
                    {
                        temp = Convert.ToInt32(Jednostki.Rows[i]["ID_Jednostki"].ToString());
                        break;
                    }
                }
            }
            int temp1 = 0;
            if (ID_Kategorii.SelectedValue is not null)
            {
                for (int i = 0; i < Kategorie.Rows.Count; i++)
                {
                    if (ID_Kategorii.SelectedValue.ToString() == Kategorie.Rows[i]["Nazwa"].ToString())
                    {
                        temp1 = Convert.ToInt32(Kategorie.Rows[i]["ID_Kategorii"].ToString());
                        break;
                    }
                }
            }
            int temp2 = 0;
            if (ID_Gatunku.SelectedValue is not null)
            {
                for (int i = 0; i < Gatunki.Rows.Count; i++)
                {
                    if (ID_Gatunku.SelectedValue.ToString() == Gatunki.Rows[i]["Nazwa"].ToString())
                    {
                        temp2 = Convert.ToInt32(Gatunki.Rows[i]["ID_Gatunku"].ToString());
                        break;
                    }
                }
            }
            int temp3 = 0;
            if (ID_KlasySkladowania is not null)
            {
                if (ID_KlasySkladowania.SelectedValue is not null)
                {
                    for (int i = 0; i < KlasySkładowania.Rows.Count; i++)
                    {
                        if (ID_KlasySkladowania.SelectedValue.ToString() == KlasySkładowania.Rows[i]["ID_KlasySkladowania"].ToString())
                        {
                            temp3 = Convert.ToInt32(KlasySkładowania.Rows[i]["ID_KlasySkladowania"].ToString());
                            break;
                        }
                    }
                }

            }
            //int.TryParse(ID_KlasySkladowania.SelectedIndex.ToString(), out int temp3);
            _ = decimal.TryParse(Wymiar1.Text, out decimal temp4);
            _ = decimal.TryParse(Wymiar2.Text, out decimal temp5);
            _ = decimal.TryParse(Wymiar3.Text, out decimal temp6);
            _ = decimal.TryParse(Wymiar4.Text, out decimal temp7);
            _ = decimal.TryParse(Wymiar5.Text, out decimal temp8);
            _ = decimal.TryParse(Wymiar6.Text, out decimal temp9);
            _ = decimal.TryParse(Wymiar7.Text, out decimal temp10);
            _ = decimal.TryParse(Wymiar8.Text, out decimal temp11);
            _ = decimal.TryParse(WagaJednostkowa.Text, out decimal temp12);
            _ = decimal.TryParse(Stan.Text, out decimal temp13);
            _ = decimal.TryParse(Rezerwacja.Text, out decimal temp14);
            _ = decimal.TryParse(StanMinimalny.Text, out decimal temp15);
            string? rotationclass = string.Empty;
            if (Rotation_class_combobox.SelectedValue is not null)
            {
                rotationclass = Rotation_class_combobox.SelectedValue.ToString() ?? "";
            }
            Article new_Article = new(_dbWMS, Indeks.Text, Nazwa.Text, Opis.Text, Atr1.Text, Atr2.Text, "",
                rotationclass, temp, temp1, temp2, temp3, temp4, temp5, temp6, temp7, temp8, temp9, temp10, temp11, temp12, temp13, temp14, temp15);
            new_Article.Update();
        }
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
        private void TabItem3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TabControl.SelectedIndex = 2;
            Step.StepIndex = 2;
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
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
            //    }
            //}
        }
        #endregion


    }
}
