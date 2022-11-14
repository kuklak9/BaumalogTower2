using ArticlesControl.Klasy;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace ArticlesControl.Formatki
{
    /// <summary>
    /// Logika interakcji dla klasy Add_Unit.xaml
    /// </summary>
    public partial class Add_Unit : BTPControlLibrary.MainWindow
    {
        public ResourceDictionary dict = new();
        public Dictionary<string, string> dictionary;
        public Add_Unit(ResourceDictionary resourceDictionary, Dictionary<string, string> lang)
        {
            dictionary = lang;
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(Precision.Text, out int result);
            Unit unit = new(Symbol.Text, Unit_Name.Text, result);
            unit.Insert();
            Close();
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

            //        //HandyControl.Controls.Growl.Warning(dictionary["Integer_validation"]);
            //    }

            //}
        }
    }
}
