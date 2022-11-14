using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml;

namespace DataGridControl
{
    /// <summary>
    /// Code behind made for changing column names in datagrid.
    /// </summary>
    public partial class Window1 : Window
    {
        #region Fields & Variables
        public string Oldcolumnname { get; set; }
        public Dictionary<string, string> Alert { get; set; }
        public ResourceDictionary dict = new();
        public List<string> Headers = new();
        private string TableName { get; set; }
        #endregion

        #region Functions
        //Main constructor
        public Window1(string tekst, Dictionary<string, string> dictionary, ResourceDictionary resourceDictionary, List<string> Headers, string tablename)
        {
            InitializeComponent();
            TableName = tablename;
            this.Headers = Headers;
            Oldcolumnname = tekst;
            string? column = string.Empty;
            XmlTextReader xmlReader = new(string.Format(TableName + "_Settings.xml"));
            DataSet dataSet = new("Settings");
            dataSet.ReadXml(xmlReader);
            xmlReader.Close();
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                if (Oldcolumnname.Equals(dataSet.Tables[0].Rows[i]["Header"].ToString()))
                {
                    column = dataSet.Tables[0].Rows[i]["Header"].ToString();
                    break;
                }
            }
            if (column!.Contains((char)0x25B2))
            {
                colunnametextbox.Text = column.Replace(Convert.ToString((char)0x25B2), "");
            }
            else if (column.Contains((char)0x25BC))
            {
                colunnametextbox.Text = column.Replace(Convert.ToString((char)0x25BC), "");
            }
            else
            {
                colunnametextbox.Text = column;
            }
            Alert = dictionary;
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);
        }
        //Submit button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            colunnametextbox.Text = colunnametextbox.Text.Replace(" ", "_");
            if (colunnametextbox.Text == "")
            {
                HandyControl.Controls.Growl.Warning(Alert["alert_cannotbeempty"]);
            }
            else if (colunnametextbox.Text.Length <= 2)
            {
                HandyControl.Controls.Growl.Warning(Alert["alert_tooshort"]);
            }
            else if (colunnametextbox.Text.Length >= 17)
            {
                HandyControl.Controls.Growl.Warning(Alert["alert_toolong"]);
            }
            //else if (colunnametextbox.Text.Contains(' ')) HandyControl.Controls.Growl.Warning(Alert["alert_containtspace"]);
            else if (Headers.Contains(colunnametextbox.Text))
            {
                HandyControl.Controls.Growl.Warning(Alert["alert_columnnameexist"]);
            }
            else if (colunnametextbox.Text[..3] == "ID_")
            {
            }
            else
            {
                HandyControl.Controls.Growl.Success(Alert["NameChangedSuccessfully"]);
                ChangeName();
                Close();
            }
            ColorAnimation animation = new()
            {
                From = (Color)ColorConverter.ConvertFromString(Application.Current.Resources["Filter_input_background"].ToString()),
                To = Colors.Red,
                Duration = new Duration(TimeSpan.FromMilliseconds(400))
            };
            colunnametextbox.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
            animation.From = Colors.Red;
            animation.To = (Color)ColorConverter.ConvertFromString(Application.Current.Resources["Filter_input_background"].ToString());
            colunnametextbox.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }
        //Function for changing column name (only local, saved on xml settings file)
        private void ChangeName()
        {

            XmlTextReader xmlReader = new(string.Format(TableName + "_Settings.xml"));
            DataSet dataSet = new("Settings");
            using (xmlReader)
            {
                string column = string.Empty;
                dataSet.ReadXml(xmlReader);
                string header = string.Empty;
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    header = dataSet.Tables[0].Rows[i]["Header"].ToString() ?? string.Empty;
                    if (Oldcolumnname == header)
                    {
                        dataSet.Tables[0].Rows[i]["Header"] = colunnametextbox.Text;
                        break;
                    }
                }
                xmlReader.Close();
                //for (int i = 0; i < UserControl1.filters.Count; i++)
                //{
                //    if (UserControl1.filters[i].Contains(column))
                //    {
                //        UserControl1.filters[i] = UserControl1.filters[i].Replace(column, colunnametextbox.Text);
                //        break;
                //    }
                //}
            }

            XmlWriter xmlWriter = new XmlTextWriter(string.Format(TableName + "_Settings.xml"), null);
            dataSet.WriteXml(xmlWriter);
            xmlWriter.Close();
            xmlReader.Close();
        }
        //Cancel button
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            UserControl1.Isnamechanging = false;
            Close();
        }
        #endregion
    }
}
