using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace DataGridControl
{
    /// <summary>
    /// Code behind made for filtering text values from datagrid.
    /// </summary>
    public partial class Window4 : Window
    {
        #region Fields & Variables
        public Dictionary<string, string> dictionar = new();
        public ResourceDictionary dict = new();
        public string textcolumn;
        public string oldfilter = string.Empty;
        #endregion

        #region Functions
        //Main constructor
        public Window4(ResourceDictionary resourceDictionary, string textcolumn)
        {
            InitializeComponent();
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);
            this.textcolumn = textcolumn;
            combobox.SelectedIndex = 0;
            UserControl1.Istextfilter = false;
            Column_name.Text += " " + textcolumn;
        }
        //Alternative constructor for window to edit existing filter.
        public Window4(ResourceDictionary resourceDictionary, string textcolumn, string oldfilter)
        {
            InitializeComponent();
            Column_name.Text += " " + textcolumn;
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);
            this.textcolumn = textcolumn;
            this.oldfilter = oldfilter;
            string text = string.Empty;
            int startindex = 0;
            UserControl1.Istextfilter = false;
            //Setting selected index on combobox and Text field on textbox for editing existing filter
            if (oldfilter.Contains('='))
            {
                combobox.SelectedIndex = 0;
                startindex = oldfilter.IndexOf('=') + 1;
                text = oldfilter[startindex..];
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
            else if (oldfilter.Contains('%') && !Regex.IsMatch(oldfilter, string.Format(@"\b{0}\b", "NOT LIKE")))
            {
                combobox.SelectedIndex = 2;
                startindex = oldfilter.IndexOf("'%") + 1;
                text = oldfilter[startindex..];
                text = text.Replace("'", "");
                text = text.Replace("%", "");
                text = string.Concat(text.Where(x => !char.IsWhiteSpace(x)));
            }
            else if (oldfilter.Contains('%') && Regex.IsMatch(oldfilter, string.Format(@"\b{0}\b", "NOT LIKE")))
            {
                combobox.SelectedIndex = 3;
                startindex = oldfilter.IndexOf("'%") + 1;
                text = oldfilter[startindex..];
                text = text.Replace("'", "");
                text = text.Replace("%", "");
                text = string.Concat(text.Where(x => !char.IsWhiteSpace(x)));
            }
            else if (oldfilter.Contains("*'"))
            {
                combobox.SelectedIndex = 4;
                startindex = oldfilter.IndexOf("LIKE") + 4;
                text = oldfilter[startindex..];
                text = text.Replace("'", "");
                text = text.Replace("*", "");
                text = string.Concat(text.Where(x => !char.IsWhiteSpace(x)));
            }
            else
            {
                combobox.SelectedIndex = 5;
                startindex = oldfilter.IndexOf("'*") + 1;
                text = oldfilter[startindex..];
                text = text.Replace("'", "");
                text = text.Replace("*", "");
                text = string.Concat(text.Where(x => !char.IsWhiteSpace(x)));
            }
            textbox.Text = text;
        }
        //Cancel button.
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (oldfilter != "")
            {
                UserControl1.TextColumnFilterString = oldfilter;
                UserControl1.Istextfilter = true;
                Close();
            }
            else
            {
                UserControl1.TextColumnFilterString = string.Empty;
                Close();
            }
        }
        //Submit button.
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Setting main window field depending on choosen comobobox item.
            switch (combobox.SelectedIndex)
            {
                case 0:
                    //equals
                    UserControl1.TextColumnFilterString = string.Format("[" + textcolumn + "]" + " = '" + textbox.Text + "'");
                    UserControl1.Istextfilter = true;
                    break;
                case 1:
                    //not_equals
                    UserControl1.TextColumnFilterString = string.Format("[" + textcolumn + "]" + " <> '" + textbox.Text + "'");
                    UserControl1.Istextfilter = true;
                    break;
                case 2:
                    //contains
                    UserControl1.TextColumnFilterString = string.Format("[" + textcolumn + "]" + " LIKE '%" + textbox.Text + "%'");
                    UserControl1.Istextfilter = true;
                    break;
                case 3:
                    //not contains
                    UserControl1.TextColumnFilterString = string.Format("[" + textcolumn + "]" + " NOT LIKE '%" + textbox.Text + "%'");
                    UserControl1.Istextfilter = true;
                    break;
                case 4:
                    //starts with
                    UserControl1.TextColumnFilterString = string.Format("[" + textcolumn + "]" + " LIKE '" + textbox.Text + "*'");
                    UserControl1.Istextfilter = true;
                    break;
                case 5:
                    //ends with
                    UserControl1.TextColumnFilterString = string.Format("[" + textcolumn + "]" + " LIKE '*" + textbox.Text + "'");
                    UserControl1.Istextfilter = true;
                    break;
                default:
                    break;
            }
            Close();
        }
        #endregion
    }
}
