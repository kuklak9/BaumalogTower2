using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.IO.Ports;
using System.Windows.Media.Animation;
using System.Windows.Media;
using BTPControlLibrary;

namespace BTPCLAdministrator
{
    /// <summary>
    /// Logika interakcji dla klasy Add_Operator.xaml
    /// </summary>
    public partial class Add_Operator : BTPControlLibrary.MainWindow
    {
        public ResourceDictionary dict = new();
        public Dictionary<string, string> dictionary;
        private readonly DataTable Grupy = Connection("Kar_Grupy");
        private readonly DataTable rights = Connection("Def_GrupyPrawa");
        private readonly Dictionary<string, string> custom_columns;
        private readonly Dictionary<string, Tuple<string, Type>> cells = new();
        private readonly bool isEdit = false;
        RFIDReader rfidreader;

        public Add_Operator(ResourceDictionary resourceDictionary, Dictionary<string, string> lang, RFIDReader rfid)
        {
            dictionary = lang;
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);

            InitializeComponent();
            Combobox_Fill(false);

            rfidreader = rfid;
            rfidreader.RFIDWasReaded += Rfidreader_RFIDWasReaded;
        }
        public Add_Operator(ResourceDictionary resourceDictionary, Dictionary<string, string> lang, Dictionary<string, Tuple<string, Type>> cells, Dictionary<string, string> custom_columns, RFIDReader rfid)
        {
            this.cells = cells;
            this.custom_columns = custom_columns;
            dictionary = lang;
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);
            InitializeComponent();
            Combobox_Fill(false);

            rfidreader = rfid;
            rfidreader.RFIDWasReaded += Rfidreader_RFIDWasReaded;

            Login.Text = cells["Login"].Item1.ToString();
            Login.IsEnabled = false;
            ID_Card.Text = cells["Card_ID"].Item1.ToString();
            Op_Name.Text = cells["Imie"].Item1.ToString();
            Surname.Text = cells["Nazwisko"].Item1.ToString();
            ID_Group.SelectedValue = Grupy.Select("ID_Grupy = '" + cells["ID_Grupy"].Item1.ToString() + "'")[0]["Nazwa"].ToString();
            Atr1.Text = cells["Atr1"].Item1.ToString();
            Password.Width = 270;
            Password.IsEnabled = false;
            isPasswordChange.Visibility = Visibility.Visible;
            isEdit = true;
        }
        private void Rfidreader_RFIDWasReaded(string? ID_Card_Number)
        {
            this.Dispatcher.BeginInvoke(new Action(() => UpdateID_Card(ID_Card_Number.ToString())), System.Windows.Threading.DispatcherPriority.Background, null); ;
        }
        private void UpdateID_Card(string ID_Card_Number)
        {
            ID_Card.Text = ID_Card_Number;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isEdit)
            {
                if ((bool)isPasswordChange.IsChecked)
                {
                    string password = PassEncrypt(Password.Password);
                    _ = int.TryParse(Grupy.Select("Nazwa = '" + ID_Group.SelectedValue.ToString() + "'")[0]["ID_Grupy"].ToString(), out int id_group);
                    Operator op = new(_dbWMS, Login.Text, password, Op_Name.Text, Surname.Text, Atr1.Text, id_group);
                    op.Update();
                    Close();
                }
                else
                {
                    string password = "";
                    _ = int.TryParse(Grupy.Select("Nazwa = '" + ID_Group.SelectedValue.ToString() + "'")[0]["ID_Grupy"].ToString(), out int id_group);
                    Operator op = new(_dbWMS, Login.Text, password, Op_Name.Text, Surname.Text, Atr1.Text, id_group);
                    op.Update();
                    Close();
                }
            }
            else
            {
                string password = PassEncrypt(Password.Password);
                _ = int.TryParse(Grupy.Select("Nazwa = '" + ID_Group.SelectedValue.ToString() + "'")[0]["ID_Grupy"].ToString(), out int id_group);
                Operator op = new(_dbWMS, Login.Text, password, Op_Name.Text, Surname.Text, Atr1.Text, id_group);
                op.Insert();
                Close();
            }

        }
        public static string PassEncrypt(string Password)
        {

            byte[] salt = Encoding.ASCII.GetBytes("9369888615820315512323246139138941212977");
            Rfc2898DeriveBytes? pbkdf2 = new(Password, salt, 1000, HashAlgorithmName.SHA512);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashByts = new byte[36];

            Array.Copy(salt, 0, hashByts, 0, 16);
            Array.Copy(hash, 0, hashByts, 16, 20);

            return Convert.ToBase64String(hashByts);
        }
        private void Combobox_Fill(bool isEdit)
        {
            Button bt1 = new()
            {
                Content = dictionary["Add_item"]
            };
            List<ComboBox> comboboxes = new();
            int seletedindex = 0;
            for (int i = 0; i < Grupy.Rows.Count; i++)
            {
                _ = ID_Group.Items.Add(Grupy.Rows[i]["Nazwa"]);
            }
            ID_Group.SelectedIndex = seletedindex;

            comboboxes.Add(ID_Group);

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
            _ = adapter.Fill(dt);
            cmd.Dispose();
            con.Close();
            return dt;
        }

        private void ID_Group_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GroupSymbol.Text = Grupy.Select("Nazwa = '" + ID_Group.SelectedValue.ToString() + "'").CopyToDataTable().Rows[0]["Symbol"].ToString();
        }

        private void isPasswordChange_Checked(object sender, RoutedEventArgs e)
        {
            Password.IsEnabled = true;
        }

        private void isPasswordChange_Unchecked(object sender, RoutedEventArgs e)
        {
            Password.IsEnabled = false;
            Password.Password = null;
        }

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
    }
}
