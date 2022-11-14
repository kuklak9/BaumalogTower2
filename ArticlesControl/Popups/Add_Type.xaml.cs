using System;
using System.Data;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using BTPDataBase;
using System.Data.SqlClient;

namespace ArticlesControl.Formatki
{
    /// <summary>
    /// Logika interakcji dla klasy Add_Type.xaml
    /// </summary>
    public partial class Add_Type : BTPControlLibrary.MainWindow
    {
        #region Fields
        public ResourceDictionary dict = new();
        public Dictionary<string, string> dictionary;
        private readonly Dictionary<string, string> custom_columns;
        private readonly Dictionary<string, Tuple<string, Type>> cells = new();

        DataRow DataToEdit;
        private bool isEditt { get; set; }
        #endregion

        #region Functions
        //Add
        public Add_Type(ResourceDictionary resourceDictionary, Dictionary<string, string> lang)
        {
            dictionary = lang;
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);
            InitializeComponent();
            custom_columns = new Dictionary<string, string>();
        }
        //Add when suggested
        public Add_Type(ResourceDictionary resourceDictionary, Dictionary<string, string> lang, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns)
        {
            dictionary = lang;
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);
            this.cells = cells;
            this.custom_columns = custom_columns;
            InitializeComponent();
            DataToEdit = (DataRow)(_dbWMS.Edit_Gatunki_ta.GetData().Select(String.Format("[ID_Gatunku] = '" + cells["ID_Gatunku"].Item1.ToString() + "'")))[0];
        }
        //Edit
        public Add_Type( ResourceDictionary resourceDictionary, Dictionary<string, string> lang, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns, bool isEdit)
        {
            dictionary = lang;
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);
            this.cells = cells;
            this.custom_columns = custom_columns;
            isEditt = true;
            InitializeComponent();
            DataToEdit = (DataRow)(_dbWMS.Edit_Gatunki_ta.GetData().Select(String.Format("[ID_Gatunku] = '" + cells["ID_Gatunku"].Item1.ToString() + "'")))[0];
            Fill_TextBlock(true);
        }

        private void Fill_TextBlock(bool isEdit)
        {
            Type_Name.Text = DataToEdit["Nazwa"].ToString();
            Description.Text = DataToEdit["Opis"].ToString();
            Default_Density.Text = DataToEdit["DomyslnaGestosc"].ToString();
            AutoIndex.Text = DataToEdit["AutoIndeks"].ToString();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isEditt)
            {
                _ = decimal.TryParse(Default_Density.Text, out decimal temp);
                _ = int.TryParse(DataToEdit["ID_Gatunku"].ToString(), out int temp1);
                ArticlesControl.Klasy.Type type1 = new(temp1, Type_Name.Text, Description.Text, temp, AutoIndex.Text);
                type1.Update();
                Close();
            }
            else
            {
                _ = decimal.TryParse(Default_Density.Text, out decimal result);
                ArticlesControl.Klasy.Type type = new(Type_Name.Text, Description.Text, result, AutoIndex.Text);
                type.Insert();
                Close();
            }
        }
        //private void TextBox_TextChanged(object sender, KeyEventArgs e)
        //{
        //    HandyControl.Controls.TextBox? textBox = sender as HandyControl.Controls.TextBox ?? new();
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
        //                //HandyControl.Controls.Growl.Warning(dictionary["Integer_validation"]);
        //            }
        //        }
        //    }
        //}
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
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null)
                    {
                        break;
                    }
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    FrameworkElement? frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
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
    }
}
