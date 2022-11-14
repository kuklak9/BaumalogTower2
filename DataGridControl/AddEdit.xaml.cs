using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
namespace DataGridControl
{
    /// <summary>
    /// Logika interakcji dla klasy AddEdit.xaml
    /// </summary>
    public partial class AddEdit : Window
    {
        public Dictionary<string, Tuple<string, Type>> Data = new();
        public Dictionary<string, Tuple<string, Type>> newData = new();
        public Dictionary<string, string> dictionary;
        public ResourceDictionary dict = new();
        private readonly string culturestring;
        private readonly CultureInfo culture;
        private readonly bool isEdit;
        public AddEdit(Dictionary<string, Tuple<string, Type>> cells, ResourceDictionary resourceDictionary, Dictionary<string, string> lang, bool isEdit)
        {
            culturestring = "en-EN";
            culture = CultureInfo.GetCultureInfo(culturestring);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            CultureInfo? cultures = new CultureInfo(culturestring);
            Thread.CurrentThread.CurrentCulture = cultures;
            Thread.CurrentThread.CurrentUICulture = cultures;
            InitializeComponent();
            Data = cells;
            this.isEdit = isEdit;
            Width = (Math.Ceiling((double)Data.Count / 10) * Width) - (10 * Math.Ceiling((double)Data.Count / 10));
            if (isEdit)
            {
                CreateEditTextLabels(Data.Count);
            }
            else
            {
                CreateAddTextLabels(Data.Count);
            }
            Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["Filter_window_background"].ToString() ?? Brushes.White.ToString());
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);
            dictionary = lang;
        }

        private void CreateAddTextLabels(int count)
        {
            int topmargin = 5;
            int leftmargin = 5;
            string[]? titles = Data.Keys.ToArray<string>();
            Tuple<string, Type>[]? values = Data.Values.ToArray();
            for (int i = 0; i < count; i++)
            {
                if (i % 10 == 0 && i != 0)
                {
                    leftmargin += 240;
                    topmargin = 5;
                }
                //Title of the column
                TextBlock textBlock = new();
                textBlock.FontSize = 12;
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["App_Background"].ToString() ?? Brushes.White.ToString());
                textBlock.Foreground = Brushes.Black;
                Grid1.Children.Add(textBlock);
                textBlock.Height = 55;
                textBlock.Text = titles[i];
                textBlock.Width = 75;
                textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                textBlock.VerticalAlignment = VerticalAlignment.Top;
                textBlock.TextAlignment = TextAlignment.Center;
                textBlock.Margin = new Thickness(leftmargin, topmargin, 0, 0);
                if (values[i].Item2 == typeof(DateTime))
                {
                    HandyControl.Controls.DateTimePicker dateTimePicker = new();
                    dateTimePicker.Name = titles[i];
                    dateTimePicker.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["Filter_input_background"].ToString() ?? Brushes.White.ToString());
                    dateTimePicker.HorizontalAlignment = HorizontalAlignment.Left;
                    dateTimePicker.VerticalAlignment = VerticalAlignment.Top;
                    dateTimePicker.Height = 55;
                    dateTimePicker.Width = 125;
                    dateTimePicker.Text = DateTime.Now.ToString();
                    dateTimePicker.Margin = new Thickness(leftmargin + 80, topmargin, 0, 0);
                    HandyControl.Tools.ConfigHelper.Instance.SetLang("pl");
                    Grid1.Children.Add(dateTimePicker);
                    topmargin += 55;
                }
                else if (values[i].Item2 == typeof(bool))
                {
                    CheckBox ch = new();
                    ch.Name = titles[i];
                    ch.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["Filter_input_background"].ToString() ?? Brushes.White.ToString());
                    ch.HorizontalAlignment = HorizontalAlignment.Left;
                    ch.VerticalAlignment = VerticalAlignment.Top;
                    ch.Height = 55;
                    ch.Width = 55;
                    ch.Margin = new Thickness(leftmargin + 135, topmargin, 0, 0);
                    Grid1.Children.Add(ch);
                    topmargin += 55;
                }
                else if (values[i].Item2 == typeof(int))
                {
                    TextBox textBox = new();
                    textBox.Tag = values[i].Item2;
                    textBox.Name = titles[i];
                    textBox.Text = "0";
                    textBox.PreviewKeyDown += TextBoxInt_KeyDown;
                    textBox.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["Filter_input_background"].ToString() ?? Brushes.White.ToString());
                    textBox.TextWrapping = TextWrapping.WrapWithOverflow;
                    Grid1.Children.Add(textBox);
                    textBox.Height = 55;
                    textBox.Width = 125;
                    textBox.HorizontalAlignment = HorizontalAlignment.Left;
                    textBox.VerticalAlignment = VerticalAlignment.Top;
                    textBox.Margin = new Thickness(leftmargin + 80, topmargin, 0, 0);
                    topmargin += 55; textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                    textBox.VerticalContentAlignment = VerticalAlignment.Center;
                    textBox.FontSize = 12;
                }
                else if (values[i].Item2 == typeof(decimal))
                {
                    TextBox textBox = new();
                    textBox.Tag = values[i].Item2;
                    textBox.PreviewKeyDown += TextBoxDecimal_KeyDown;
                    textBox.Name = titles[i];
                    textBox.Text = "0.0";
                    textBox.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["Filter_input_background"].ToString() ?? Brushes.White.ToString());
                    textBox.TextWrapping = TextWrapping.WrapWithOverflow;
                    Grid1.Children.Add(textBox);
                    textBox.Height = 55;
                    textBox.Width = 125;
                    textBox.HorizontalAlignment = HorizontalAlignment.Left;
                    textBox.VerticalAlignment = VerticalAlignment.Top;
                    textBox.Margin = new Thickness(leftmargin + 80, topmargin, 0, 0);
                    topmargin += 55;
                    textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                    textBox.VerticalContentAlignment = VerticalAlignment.Center;
                    textBox.FontSize = 12;
                }
                else
                {
                    TextBox textBox = new();
                    textBox.Tag = values[i].Item2;
                    textBox.Name = titles[i];
                    textBox.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["Filter_input_background"].ToString() ?? Brushes.White.ToString());
                    textBox.TextWrapping = TextWrapping.WrapWithOverflow;
                    Grid1.Children.Add(textBox);
                    textBox.Height = 55;
                    textBox.Width = 125;
                    textBox.HorizontalAlignment = HorizontalAlignment.Left;
                    textBox.VerticalAlignment = VerticalAlignment.Top;
                    textBox.Margin = new Thickness(leftmargin + 80, topmargin, 0, 0);
                    topmargin += 55;
                    textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                    textBox.VerticalContentAlignment = VerticalAlignment.Center;
                    textBox.FontSize = 12;
                }
            }
        }

        private void TextBoxInt_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
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
                Key.Tab
            };
            TextBox? tb = sender as TextBox ?? new();
            tb.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["Filter_input_background"].ToString() ?? Brushes.White.ToString()) ?? Brushes.White;
            tb.BorderThickness = new Thickness(1, 1, 1, 1);
            if (!numberlist.Contains(e.Key))
            {
                e.Handled = true;
                tb.BorderThickness = new Thickness(3, 3, 3, 3);
                ColorAnimation animation;
                animation = new();
                animation.From = (Color)ColorConverter.ConvertFromString(Application.Current.Resources["Filter_input_background"].ToString());
                animation.To = Colors.Red;
                animation.Duration = new Duration(TimeSpan.FromMilliseconds(200));
                tb.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
                animation.From = Colors.Red;
                animation.To = (Color)ColorConverter.ConvertFromString(Application.Current.Resources["Filter_input_background"].ToString());
                tb.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);

                HandyControl.Controls.Growl.Warning(dictionary["Integer_validation"] + " " + tb.Name);
            }
        }

        private void TextBoxDecimal_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
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
                Key.OemPeriod,
                Key.Enter,
                Key.Escape,
                Key.Left,
                Key.Right,
                Key.Tab
            };
            TextBox? tb = sender as TextBox ?? new();
            tb.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["Filter_input_background"].ToString() ?? Brushes.White.ToString()) ?? Brushes.White;
            tb.BorderThickness = new Thickness(1, 1, 1, 1);
            if (!numberlist.Contains(e.Key) || (tb.Text.Contains('.') && e.Key == Key.OemPeriod) || (e.Key == Key.OemPeriod && tb.Text.Length == 0))
            {
                e.Handled = true;
                tb.BorderThickness = new Thickness(3, 3, 3, 3);
                ColorAnimation animation;
                animation = new();
                animation.From = (Color)ColorConverter.ConvertFromString(Application.Current.Resources["Filter_input_background"].ToString());
                animation.To = Colors.Red;
                animation.Duration = new Duration(TimeSpan.FromMilliseconds(200));
                tb.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
                animation.From = Colors.Red;
                animation.To = (Color)ColorConverter.ConvertFromString(Application.Current.Resources["Filter_input_background"].ToString());
                tb.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
                HandyControl.Controls.Growl.Warning(dictionary["Decimal_validation"] + " " + tb.Name);
            }
        }

        private void CreateEditTextLabels(int count)
        {
            int topmargin = 5;
            int leftmargin = 5;
            string[]? titles = Data.Keys.ToArray<string>();
            Tuple<string, Type>[]? values = Data.Values.ToArray();
            for (int i = 0; i < count; i++)
            {
                if (i % 10 == 0 && i != 0)
                {
                    leftmargin += 240;
                    topmargin = 5;
                }
                //Title of the column
                TextBlock textBlock = new();
                textBlock.FontSize = 12;
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.Text = titles[i];
                textBlock.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["Filter_window_background"].ToString() ?? Brushes.White.ToString());
                textBlock.Foreground = Brushes.WhiteSmoke;
                Grid1.Children.Add(textBlock);
                textBlock.Height = 55;
                textBlock.Width = 75;
                textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                textBlock.VerticalAlignment = VerticalAlignment.Top;
                textBlock.TextAlignment = TextAlignment.Center;
                textBlock.Margin = new Thickness(leftmargin, topmargin, 0, 0);

                if (values[i].Item2 == typeof(DateTime))
                {
                    HandyControl.Controls.DateTimePicker dateTimePicker = new();
                    dateTimePicker.Name = titles[i];
                    dateTimePicker.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["Filter_input_background"].ToString() ?? Brushes.White.ToString());
                    dateTimePicker.HorizontalAlignment = HorizontalAlignment.Left;
                    dateTimePicker.VerticalAlignment = VerticalAlignment.Top;
                    dateTimePicker.Height = 55;
                    dateTimePicker.Width = 125;
                    dateTimePicker.Margin = new Thickness(leftmargin + 80, topmargin, 0, 0);
                    HandyControl.Tools.ConfigHelper.Instance.SetLang("pl");
                    Grid1.Children.Add(dateTimePicker);
                    DateTime oldtime = DateTime.Now;
                    if (DateTime.TryParse(values[i].Item1, out oldtime)) { dateTimePicker.SelectedDateTime = oldtime; }
                    topmargin += 55;
                }
                else if (values[i].Item2 == typeof(bool))
                {
                    CheckBox ch = new();
                    ch.Name = titles[i];
                    ch.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["Filter_input_background"].ToString() ?? Brushes.White.ToString());
                    ch.HorizontalAlignment = HorizontalAlignment.Left;
                    ch.VerticalAlignment = VerticalAlignment.Top;
                    if (bool.TryParse(values[i].Item1, out bool ischecked))
                    {
                        ch.IsChecked = ischecked;
                    }
                    ch.Height = 55;
                    ch.Width = 55;
                    ch.Margin = new Thickness(leftmargin + 135, topmargin, 0, 0);
                    Grid1.Children.Add(ch);
                    topmargin += 55;
                }
                else if (values[i].Item2 == typeof(int))
                {
                    TextBox textBox = new();
                    textBox.PreviewKeyDown += TextBoxInt_KeyDown;
                    textBox.Tag = values[i].Item2;
                    textBox.Name = titles[i];
                    textBox.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["Filter_input_background"].ToString() ?? Brushes.White.ToString());
                    textBox.Text = values[i].Item1;
                    textBox.TextWrapping = TextWrapping.WrapWithOverflow;
                    Grid1.Children.Add(textBox);
                    textBox.Height = 55;
                    textBox.Width = 125;
                    textBox.HorizontalAlignment = HorizontalAlignment.Left;
                    textBox.VerticalAlignment = VerticalAlignment.Top;
                    textBox.Margin = new Thickness(leftmargin + 80, topmargin, 0, 0);
                    topmargin += 55;
                    textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                    textBox.VerticalContentAlignment = VerticalAlignment.Center;
                    textBox.FontSize = 12;
                }
                else if (values[i].Item2 == typeof(decimal))
                {
                    TextBox textBox = new();
                    textBox.Tag = values[i].Item2;
                    textBox.PreviewKeyDown += TextBoxDecimal_KeyDown;
                    textBox.Name = titles[i];
                    textBox.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["Filter_input_background"].ToString() ?? Brushes.White.ToString());
                    textBox.Text = values[i].Item1;
                    textBox.Text = textBox.Text.Replace(',', '.');
                    textBox.TextWrapping = TextWrapping.WrapWithOverflow;
                    Grid1.Children.Add(textBox);
                    textBox.Height = 55;
                    textBox.Width = 125;
                    textBox.HorizontalAlignment = HorizontalAlignment.Left;
                    textBox.VerticalAlignment = VerticalAlignment.Top;
                    textBox.Margin = new Thickness(leftmargin + 80, topmargin, 0, 0);
                    topmargin += 55;
                    textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                    textBox.VerticalContentAlignment = VerticalAlignment.Center;
                    textBox.FontSize = 12;
                }
                else
                {
                    TextBox textBox = new();
                    textBox.Tag = values[i].Item2;
                    textBox.Name = titles[i];
                    textBox.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["Filter_input_background"].ToString() ?? Brushes.White.ToString());
                    textBox.Text = values[i].Item1;
                    textBox.TextWrapping = TextWrapping.WrapWithOverflow;
                    Grid1.Children.Add(textBox);
                    textBox.Height = 55;
                    textBox.Width = 125;
                    textBox.HorizontalAlignment = HorizontalAlignment.Left;
                    textBox.VerticalAlignment = VerticalAlignment.Top;
                    textBox.Margin = new Thickness(leftmargin + 80, topmargin, 0, 0);
                    topmargin += 55;
                    textBox.HorizontalContentAlignment = HorizontalAlignment.Left;
                    textBox.VerticalContentAlignment = VerticalAlignment.Center;
                    textBox.FontSize = 12;
                }
            }
        }
        //Cancel button handling
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        //Submit button handling
        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            bool IsValidate = true;
            List<CheckBox>? checkboxes = Grid1.Children.OfType<CheckBox>().ToList();
            List<TextBox>? Textboxes = Grid1.Children.OfType<TextBox>().ToList();
            List<HandyControl.Controls.DateTimePicker>? Datetimepickers = Grid1.Children.OfType<HandyControl.Controls.DateTimePicker>().ToList();
            newData.Clear();
            foreach (TextBox? item in Textboxes)
            {
                if (item.Background == Brushes.Red)
                {
                    item.Background = (SolidColorBrush?)new BrushConverter().ConvertFrom(Application.Current.Resources["Filter_input_background"].ToString() ?? Brushes.White.ToString());
                }
                if ((Type)item.Tag == typeof(int))
                {
                    Regex regex = new(@"^\d+$");
                    if (item.Text == "") { }
                    else if (!regex.IsMatch(item.Text))
                    {
                        item.Background = Brushes.Red;
                        IsValidate = false;
                        HandyControl.Controls.Growl.Warning(dictionary["Integer_validation"] + " " + item.Name);
                    }
                }
                if ((Type)item.Tag == typeof(decimal))
                {
                    item.Text = item.Text.Replace(',', '.');
                    Regex regex = new(@"^(?:-)?[0-9]{0,11}(?:\.[0-9]{0,4})?$");
                    if (item.Text == "") { item.Text = "0.0"; }
                    else if (!regex.IsMatch(item.Text))
                    {
                        item.Background = Brushes.Red;
                        IsValidate = false;
                        HandyControl.Controls.Growl.Warning(dictionary["Decimal_validation"] + " " + item.Name);
                    }
                }
                newData.Add(item.Name, Tuple.Create(item.Text, (Type)item.Tag));
            }
            foreach (CheckBox checkbox in checkboxes)
            {
                newData.Add(checkbox.Name, Tuple.Create(checkbox.IsChecked.ToString()!, typeof(bool)));
            }
            foreach (HandyControl.Controls.DateTimePicker? item in Datetimepickers)
            {
                newData.Add(item.Name, Tuple.Create(item.SelectedDateTime.ToString()!, typeof(DateTime)));
            }

            if (IsValidate)
            {
                UserControl1.updatecells = newData;
                if (isEdit)
                {
                    HandyControl.Controls.Growl.Success(dictionary["Edit_ROW_succesfull"]);
                }
                else
                {
                    HandyControl.Controls.Growl.Success(dictionary["Add_ROW_succesfull"]);
                }

                Close();
            }
        }
    }
}
