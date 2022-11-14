using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;

namespace DataGridControl
{
    /// <summary>
    /// Code behind made for filtering number values from datagrid.
    /// </summary>
    public partial class Window3 : Window
    {
        #region Fields & Variables
        public Dictionary<string, string> dictionar = new();
        public ResourceDictionary dict = new();
        public DataView dv = new();
        public string numbercolumn;
        public string oldfilter = string.Empty;
        public List<decimal> listview = new();
        #endregion

        #region Functions
        //Main Constructor
        public Window3(ResourceDictionary resourceDictionary, DataView dv, string numbercolumn, Dictionary<string, string> dictionar)
        {
            InitializeComponent();
            dict = resourceDictionary;
            this.dictionar = dictionar;
            Resources.MergedDictionaries.Add(dict);
            this.dv = dv;
            this.numbercolumn = numbercolumn;
            UserControl1.Isnumberfilter = false;
            Column_name.Text += " " + numbercolumn;
            combobox.SelectedIndex = 0;
        }
        //Alternate constructor made for editing existing filter
        public Window3(ResourceDictionary resourceDictionary, DataView dv, string numbercolumn, Dictionary<string, string> dictionar, string oldfilter)
        {
            InitializeComponent();
            dict = resourceDictionary;
            this.dictionar = dictionar;
            Resources.MergedDictionaries.Add(dict);
            this.dv = dv;
            this.numbercolumn = numbercolumn;
            this.oldfilter = oldfilter;
            DataTable dt = dv.ToTable();
            string text = "";
            int startindex = 0;
            double avg = 0;
            UserControl1.Isnumberfilter = false;
            Column_name.Text += " " + numbercolumn;
            combobox.SelectedIndex = 0;
            if (oldfilter.Contains(" = "))
            {
                combobox.SelectedIndex = 0;
                startindex = oldfilter.IndexOf('=') + 1;
                text = oldfilter[startindex..];
                //text = oldfilter.Substring(startindex+1);
                text = text.Replace("'", "");
                text = string.Concat(text.Where(x => !char.IsWhiteSpace(x)));
            }
            else if (oldfilter.Contains("<>"))
            {
                combobox.SelectedIndex = 1;
                startindex = oldfilter.IndexOf("<>") + 1;
                text = oldfilter[startindex..];
                text = text.Replace("'", "");
                text = text.Replace("<", "");
                text = text.Replace(">", "");
                text = string.Concat(text.Where(x => !char.IsWhiteSpace(x)));
            }
            else if (oldfilter.Contains("> '"))
            {
                combobox.SelectedIndex = 2;
                startindex = oldfilter.IndexOf(">") + 1;
                text = oldfilter[startindex..];
                text = text.Replace("'", "");
                text = text.Replace(">", "");
                text = string.Concat(text.Where(x => !char.IsWhiteSpace(x)));
            }
            else if (oldfilter.Contains(">="))
            {
                combobox.SelectedIndex = 3;
                startindex = oldfilter.IndexOf(">=") + 1;
                text = oldfilter[startindex..];
                text = text.Replace("'", "");
                text = text.Replace("=", "");
                text = text.Replace(">", "");
                text = string.Concat(text.Where(x => !char.IsWhiteSpace(x)));
            }
            else if (oldfilter.Contains("< '"))
            {
                combobox.SelectedIndex = 4;
                startindex = oldfilter.IndexOf('<') + 1;
                text = oldfilter[startindex..];
                text = text.Replace("'", "");
                text = text.Replace("<", "");
            }
            else if (oldfilter.Contains("<="))
            {
                combobox.SelectedIndex = 5;
                startindex = oldfilter.IndexOf("<=") + 1;
                text = oldfilter[startindex..];
                text = text.Replace("'", "");
                text = text.Replace("=", "");
                text = text.Replace("<", "");
                text = string.Concat(text.Where(x => !char.IsWhiteSpace(x)));
            }
            else
            {
                if (Convert.ToDouble(dt.Rows[0][numbercolumn]) > avg)
                {
                    combobox.SelectedIndex = 7;
                }
                else
                {
                    combobox.SelectedIndex = 6;
                }
            }
            textbox.Text = text;
        }
        //Function handling submit button.
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Condition to accept , and . as input decimal number.
            if (textbox.Text.Contains('.'))
            {
                textbox.Text = textbox.Text.Replace('.', ',');
            }
            //Condition for parsing textbox input to decimal value.
            if (decimal.TryParse(textbox.Text, out decimal input)) { }
            else if (combobox.SelectedIndex != 6 && combobox.SelectedIndex != 7)
            {
                textbox.Text = string.Empty;
                MessageBox.Show(dictionar["Wrong_Char_Alert"]);
                return;
            }
            //Setting main window field depending on choosen combobox item.
            switch (combobox.SelectedIndex)
            {
                case 0:
                    //equals
                    UserControl1.NumberColumnFilterString = string.Format("[" + numbercolumn + "]" + " = '" + Convert.ToString(input) + "'");
                    UserControl1.Isnumberfilter = true;
                    break;
                case 1:
                    //not equals
                    UserControl1.NumberColumnFilterString = string.Format("[" + numbercolumn + "]" + " <> '" + Convert.ToString(input) + "'");
                    UserControl1.Isnumberfilter = true;
                    break;
                case 2:
                    //More than
                    UserControl1.NumberColumnFilterString = string.Format("[" + numbercolumn + "]" + " > '" + Convert.ToString(input) + "'");
                    UserControl1.Isnumberfilter = true;
                    break;
                case 3:
                    //More than equal
                    UserControl1.NumberColumnFilterString = string.Format("[" + numbercolumn + "]" + " >= '" + Convert.ToString(input) + "'");
                    UserControl1.Isnumberfilter = true;
                    break;
                case 4:
                    //less than
                    UserControl1.NumberColumnFilterString = string.Format("[" + numbercolumn + "]" + " < '" + Convert.ToString(input) + "'");
                    UserControl1.Isnumberfilter = true;
                    break;
                case 5:
                    //less than equal
                    UserControl1.NumberColumnFilterString = string.Format("[" + numbercolumn + "]" + " <= '" + Convert.ToString(input) + "'");
                    UserControl1.Isnumberfilter = true;
                    break;
                default:
                    break;
            }
            Close();
        }
        //Function handling cancel button.
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (oldfilter != "")
            {
                UserControl1.NumberColumnFilterString = oldfilter;
                UserControl1.Isnumberfilter = true;
                Close();
            }
            else
            {
                UserControl1.NumberColumnFilterString = string.Empty;
                Close();
            }
        }
        #endregion
    }
}
