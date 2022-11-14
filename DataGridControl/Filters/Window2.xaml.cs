using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DataGridControl
{
    /// <summary>
    /// Code behind made for filtering DateTime values from datagrid.
    /// </summary>
    public partial class Window2 : Window
    {
        #region Fields & Variables
        public ResourceDictionary dict = new();
        public string columnname;
        public string filtr = string.Empty;
        public string oldfilter = string.Empty;
        private readonly string culturestring = string.Empty;
        private readonly CultureInfo culture = CultureInfo.InvariantCulture;
        #endregion

        #region Functions
        //Main constructor
        public Window2(ResourceDictionary resourceDictionary, string columnname)
        {
            culturestring = CultureInfo.CurrentCulture.Name;
            culture = CultureInfo.GetCultureInfo(culturestring);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            CultureInfo? cultures = new CultureInfo(culturestring);
            Thread.CurrentThread.CurrentCulture = cultures;
            Thread.CurrentThread.CurrentUICulture = cultures;
            InitializeComponent();
            dict = resourceDictionary;
            Resources.MergedDictionaries.Add(dict);
            this.columnname = columnname;
            calendar.IsEnabled = false;
            calendar_2.IsEnabled = false;
            Column_name.Text += " " + columnname;
            FilterCombobox.SelectedIndex = 0;
        }
        //Alternative constructor for window to edit existing filter
        public Window2(ResourceDictionary resourceDictionary, string columnname, string oldfilter)
        {
            InitializeComponent();
            dict = resourceDictionary;
            Column_name.Text += " " + columnname;
            Resources.MergedDictionaries.Add(dict);
            this.oldfilter = oldfilter;
            this.columnname = columnname;
            calendar.IsEnabled = false;
            calendar_2.IsEnabled = false;
            string text = string.Empty;
            int startindex = 0;
            int endindex = 0;
            DateTime oldtime;
            if (oldfilter.Contains(" <= ") && oldfilter.Contains(" >= "))
            {
                FilterCombobox.SelectedIndex = 3;
                startindex = oldfilter.IndexOf(">=") + 4;
                endindex = oldfilter.IndexOf("and") - 2;
                //text = oldfilter.Substring(startindex, (endindex - startindex));
                text = oldfilter[startindex..endindex];
                text = text.Replace("'", "");
                text = text.Replace("00:00:00", "");
                text = string.Concat(text.Where(x => !char.IsWhiteSpace(x)));
                if (DateTime.TryParse(text, out oldtime)) { }
                calendar.SelectedDate = oldtime;
                startindex = oldfilter.IndexOf("<=") + 2;
                text = oldfilter[startindex..];
                text = text.Replace("'", "");
                text = text.Replace("00:00:00", "");
                text = string.Concat(text.Where(x => !char.IsWhiteSpace(x)));
                if (DateTime.TryParse(text, out oldtime)) { }
                calendar_2.SelectedDate = oldtime;
            }
            else if (oldfilter.Contains(" >= "))
            {
                FilterCombobox.SelectedIndex = 2;
                startindex = oldfilter.IndexOf('=') + 1;
                text = oldfilter[startindex..];
                text = text.Replace("'", "");
                text = text.Replace("00:00:00", "");
                text = string.Concat(text.Where(x => !char.IsWhiteSpace(x)));
                if (DateTime.TryParse(text, out oldtime)) { }
                calendar.SelectedDate = oldtime;
            }
            else if (oldfilter.Contains(" <= "))
            {
                FilterCombobox.SelectedIndex = 1;
                startindex = oldfilter.IndexOf('=') + 1;
                text = oldfilter[startindex..];
                text = text.Replace("'", "");
                text = text.Replace("00:00:00", "");
                text = string.Concat(text.Where(x => !char.IsWhiteSpace(x)));
                if (DateTime.TryParse(text, out oldtime)) { }
                calendar.SelectedDate = oldtime;
            }
            //textbox.Text = text;
        }
        //Function handling visibility of datepickers, and setting date which was choosen from combobox.
        private void Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime dt = DateTime.Today;

            switch (FilterCombobox.SelectedIndex)
            {
                //equals
                case 0:
                    calendar.SelectedDate = DateTime.Today;
                    calendar.IsEnabled = true;
                    calendar_2.IsEnabled = false;
                    calendar_2.SelectedDate = null;
                    ShowReduced();
                    break;
                //before
                case 1:
                    calendar.SelectedDate = DateTime.Today;
                    calendar.IsEnabled = true;
                    calendar_2.IsEnabled = false;
                    calendar_2.SelectedDate = null;
                    ShowReduced();
                    break;
                //after
                case 2:
                    calendar.SelectedDate = DateTime.Today.AddDays(-1);
                    calendar.IsEnabled = true;
                    calendar_2.IsEnabled = false;
                    calendar_2.SelectedDate = null;
                    ShowReduced();
                    break;
                //between
                case 3:
                    calendar.IsEnabled = true;
                    calendar_2.IsEnabled = true;
                    calendar.SelectedDate = DateTime.Today.AddDays(-1);
                    calendar_2.SelectedDate = DateTime.Today;
                    ShowExpanded();
                    break;
                //yesterday
                case 4:
                    calendar.IsEnabled = false;
                    calendar_2.IsEnabled = false;
                    calendar.SelectedDate = DateTime.Today.AddDays(-1);
                    calendar_2.SelectedDate = null;
                    ShowReduced();
                    break;
                //today
                case 5:
                    calendar.IsEnabled = false;
                    calendar_2.IsEnabled = false;
                    calendar.SelectedDate = DateTime.Today;
                    calendar_2.SelectedDate = null;
                    ShowReduced();
                    break;
                //This week
                case 6:
                    calendar.IsEnabled = false;
                    calendar_2.IsEnabled = false;
                    ShowExpanded();
                    if (dt.DayOfWeek == DayOfWeek.Tuesday)
                    {
                        calendar.SelectedDate = dt.AddDays(-1);
                        calendar_2.SelectedDate = dt.AddDays(5);
                    }
                    else if (dt.DayOfWeek == DayOfWeek.Wednesday)
                    {
                        calendar.SelectedDate = dt.AddDays(-2);
                        calendar_2.SelectedDate = dt.AddDays(4);
                    }
                    else if (dt.DayOfWeek == DayOfWeek.Thursday)
                    {
                        calendar.SelectedDate = dt.AddDays(-3);
                        calendar_2.SelectedDate = dt.AddDays(3);
                    }
                    else if (dt.DayOfWeek == DayOfWeek.Friday)
                    {
                        calendar.SelectedDate = dt.AddDays(-4);
                        calendar_2.SelectedDate = dt.AddDays(2);
                    }
                    else if (dt.DayOfWeek == DayOfWeek.Saturday)
                    {
                        calendar.SelectedDate = dt.AddDays(-5);
                        calendar_2.SelectedDate = dt.AddDays(1);
                    }
                    else if (dt.DayOfWeek == DayOfWeek.Sunday)
                    {
                        calendar.SelectedDate = dt.AddDays(-6);
                        calendar_2.SelectedDate = dt;
                    }
                    else
                    {
                        calendar.SelectedDate = dt;
                        calendar_2.SelectedDate = dt.AddDays(6);
                    }
                    break;
                //Last Week
                case 7:
                    DateTime dt2 = DateTime.Today;
                    calendar.IsEnabled = false;
                    calendar_2.IsEnabled = false;
                    ShowExpanded();
                    switch (dt2.DayOfWeek)
                    {
                        case DayOfWeek.Sunday:
                            calendar.SelectedDate = DateTime.Today.AddDays(-13);
                            calendar_2.SelectedDate = DateTime.Today.AddDays(-7);
                            break;
                        case DayOfWeek.Monday:
                            calendar.SelectedDate = DateTime.Today.AddDays(-7);
                            calendar_2.SelectedDate = DateTime.Today.AddDays(-1);
                            break;
                        case DayOfWeek.Tuesday:
                            calendar.SelectedDate = DateTime.Today.AddDays(-8);
                            calendar_2.SelectedDate = DateTime.Today.AddDays(-2);
                            break;
                        case DayOfWeek.Wednesday:
                            calendar.SelectedDate = DateTime.Today.AddDays(-9);
                            calendar_2.SelectedDate = DateTime.Today.AddDays(-3);
                            break;
                        case DayOfWeek.Thursday:
                            calendar.SelectedDate = DateTime.Today.AddDays(-10);
                            calendar_2.SelectedDate = DateTime.Today.AddDays(-4);
                            break;
                        case DayOfWeek.Friday:
                            calendar.SelectedDate = DateTime.Today.AddDays(-11);
                            calendar_2.SelectedDate = DateTime.Today.AddDays(-5);
                            break;
                        case DayOfWeek.Saturday:
                            calendar.SelectedDate = DateTime.Today.AddDays(-12);
                            calendar_2.SelectedDate = DateTime.Today.AddDays(-6);
                            break;
                        default:
                            break;
                    }
                    break;
                //This Month
                case 8:
                    calendar.IsEnabled = false;
                    calendar_2.IsEnabled = false;
                    calendar.SelectedDate = new DateTime(dt.Year, dt.Month, 1);
                    calendar_2.SelectedDate = new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month));
                    ShowExpanded();
                    break;
                //Last Month
                case 9:
                    calendar.IsEnabled = false;
                    calendar_2.IsEnabled = false;
                    calendar.SelectedDate = new DateTime(dt.Year, dt.Month - 1, 1);
                    calendar_2.SelectedDate = new DateTime(dt.Year, dt.Month - 1, DateTime.DaysInMonth(dt.Year, dt.Month - 1));
                    ShowExpanded();
                    break;
                //This Quart
                case 10:
                    calendar.IsEnabled = false;
                    calendar_2.IsEnabled = false;
                    ShowExpanded();
                    if (dt.Month >= 1 && dt.Month <= 3)
                    {
                        calendar.SelectedDate = new DateTime(dt.Year, 1, 1);
                        calendar_2.SelectedDate = new DateTime(dt.Year, 3, DateTime.DaysInMonth(dt.Year, 3));
                    }
                    else if (dt.Month >= 4 && dt.Month <= 6)
                    {
                        calendar.SelectedDate = new DateTime(dt.Year, 4, 1);
                        calendar_2.SelectedDate = new DateTime(dt.Year, 6, DateTime.DaysInMonth(dt.Year, 6));
                    }
                    else if (dt.Month >= 7 && dt.Month <= 9)
                    {
                        calendar.SelectedDate = new DateTime(dt.Year, 7, 1);
                        calendar_2.SelectedDate = new DateTime(dt.Year, 9, DateTime.DaysInMonth(dt.Year, 9));
                    }
                    else
                    {
                        calendar.SelectedDate = new DateTime(dt.Year, 10, 1);
                        calendar_2.SelectedDate = new DateTime(dt.Year, 12, DateTime.DaysInMonth(dt.Year, 12));
                    }
                    break;
                //Last Quart
                case 11:
                    calendar.IsEnabled = false;
                    calendar_2.IsEnabled = false;
                    ShowExpanded();
                    if (dt.Month >= 1 && dt.Month <= 3)
                    {
                        calendar.SelectedDate = new DateTime(dt.Year - 1, 10, 1);
                        calendar_2.SelectedDate = new DateTime(dt.Year - 1, 12, DateTime.DaysInMonth(dt.Year, 12));
                    }
                    else if (dt.Month >= 4 && dt.Month <= 6)
                    {
                        calendar.SelectedDate = new DateTime(dt.Year, 1, 1);
                        calendar_2.SelectedDate = new DateTime(dt.Year, 3, DateTime.DaysInMonth(dt.Year, 3));
                    }
                    else if (dt.Month >= 7 && dt.Month <= 9)
                    {
                        calendar.SelectedDate = new DateTime(dt.Year, 4, 1);
                        calendar_2.SelectedDate = new DateTime(dt.Year, 6, DateTime.DaysInMonth(dt.Year, 6));
                    }
                    else
                    {
                        calendar.SelectedDate = new DateTime(dt.Year, 7, 1);
                        calendar_2.SelectedDate = new DateTime(dt.Year, 9, DateTime.DaysInMonth(dt.Year, 9));
                    }
                    break;
                //This Year
                case 12:
                    calendar.IsEnabled = false;
                    calendar_2.IsEnabled = false;
                    ShowExpanded();
                    calendar.SelectedDate = new DateTime(dt.Year, 1, 1);
                    calendar_2.SelectedDate = new DateTime(dt.Year, 12, 31);
                    break;
                //Last Year
                case 13:
                    calendar.IsEnabled = false;
                    calendar_2.IsEnabled = false;
                    ShowExpanded();
                    calendar.SelectedDate = new DateTime(dt.Year - 1, 1, 1);
                    calendar_2.SelectedDate = new DateTime(dt.Year - 1, 12, 31);
                    break;
                default:
                    filtr = "Case empty";
                    break;
            }
        }
        //Submit button handling.
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DateTime dt = DateTime.Today;
            DateTime dt2 = DateTime.Today;
            DateTime dt3;
            //setting main window field depending on choosen filter.
            switch (FilterCombobox.SelectedIndex)
            {
                //equals
                case 0:
                    DateTime dt1 = (DateTime)calendar.SelectedDate!;
                    UserControl1.DateColumnFilterString = string.Format(" >= '{0}' and [{1}] <= '{2}'", calendar.SelectedDate, columnname, dt1.AddDays(1));
                    break;
                //before
                case 1:
                    UserControl1.DateColumnFilterString = string.Format(" <= '{0}'", calendar.SelectedDate);
                    break;
                //after
                case 2:
                    UserControl1.DateColumnFilterString = string.Format(" >= '{0}'", calendar.SelectedDate);
                    break;
                //between
                case 3:
                    dt2 = (DateTime)calendar.SelectedDate!;
                    dt3 = (DateTime)calendar_2.SelectedDate!;
                    UserControl1.DateColumnFilterString = string.Format(" >= '{0}' and [{1}] <= '{2}'", dt2, columnname, dt3.AddDays(1));
                    break;
                //yesterday
                case 4:
                    //calendar.SelectedDate = DateTime.Today.AddDays(-1);
                    UserControl1.DateColumnFilterString = string.Format(" >= '{0}' and [{1}] <= '{2}'", DateTime.Today.AddDays(-1), columnname, DateTime.Today);
                    break;
                //today
                case 5:
                    calendar.SelectedDate = DateTime.Today;
                    UserControl1.DateColumnFilterString = string.Format(" >= '{0}' and [{1}] <= '{2}'", DateTime.Today, columnname, DateTime.Today.AddDays(1));
                    break;
                //This week
                case 6:
                    filtr = string.Empty;
                    //DateTime dt = DateTime.Today;
                    if (dt.DayOfWeek == DayOfWeek.Tuesday)
                    {
                        dt = dt.AddDays(-1);
                        dt2 = dt2.AddDays(5);
                    }
                    else if (dt.DayOfWeek == DayOfWeek.Wednesday)
                    {
                        dt = dt.AddDays(-2);
                        dt2 = dt2.AddDays(4);
                    }
                    else if (dt.DayOfWeek == DayOfWeek.Thursday)
                    {
                        dt = dt.AddDays(-3);
                        dt2 = dt2.AddDays(3);
                    }
                    else if (dt.DayOfWeek == DayOfWeek.Friday)
                    {
                        dt = dt.AddDays(-4);
                        dt2 = dt2.AddDays(2);
                    }
                    else if (dt.DayOfWeek == DayOfWeek.Saturday)
                    {
                        dt = dt.AddDays(-5);
                        dt2 = dt2.AddDays(1);
                    }
                    else if (dt.DayOfWeek == DayOfWeek.Sunday)
                    {
                        dt = dt.AddDays(-6);
                        dt2 = DateTime.Today;
                    }
                    else
                    {
                        dt = DateTime.Today;
                        dt2 = dt2.AddDays(6);
                    }
                    UserControl1.DateColumnFilterString = string.Format(" >= '{0}' and [{1}] <= '{2}'", dt, columnname, dt2.AddDays(1));
                    break;
                //Last Week
                case 7:
                    //DateTime dt = DateTime.Today;
                    if (dt.DayOfWeek == DayOfWeek.Tuesday)
                    {
                        dt = dt.AddDays(-8);
                        dt2 = dt2.AddDays(-2);
                    }
                    else if (dt.DayOfWeek == DayOfWeek.Wednesday)
                    {
                        dt = dt.AddDays(-9);
                        dt2 = dt2.AddDays(-3);
                    }
                    else if (dt.DayOfWeek == DayOfWeek.Thursday)
                    {
                        dt = dt.AddDays(-10);
                        dt2 = dt2.AddDays(-4);
                    }
                    else if (dt.DayOfWeek == DayOfWeek.Friday)
                    {
                        dt = dt.AddDays(-11);
                        dt2 = dt2.AddDays(-5);
                    }
                    else if (dt.DayOfWeek == DayOfWeek.Saturday)
                    {
                        dt = dt.AddDays(-12);
                        dt2 = dt2.AddDays(-6);
                    }
                    else if (dt.DayOfWeek == DayOfWeek.Sunday)
                    {
                        dt = dt.AddDays(-13);
                        dt2 = dt2.AddDays(-7);
                    }
                    else
                    {
                        dt = dt.AddDays(-7);
                        dt2 = dt2.AddDays(-1);
                    }
                    UserControl1.DateColumnFilterString = string.Format(" >= '{0}' and [{1}] <= '{2}'", dt, columnname, dt2.AddDays(1));
                    break;
                //This Month
                case 8:
                    dt = new DateTime(dt.Year, dt.Month, 1);
                    dt2 = new DateTime(dt.Year, dt.Month, (DateTime.DaysInMonth(dt.Year, dt.Month)));
                    UserControl1.DateColumnFilterString = string.Format(" >= '{0}' and [{1}] <= '{2}'", dt, columnname, dt2.AddDays(1));
                    break;
                //Last Month
                case 9:
                    dt = DateTime.Today;
                    dt = new DateTime(dt.Year, dt.Month - 1, 1);
                    dt2 = new DateTime(dt.Year, dt.Month - 1, (DateTime.DaysInMonth(dt.Year, dt.Month - 1)));
                    UserControl1.DateColumnFilterString = string.Format(" >= '{0}' and [{1}] <= '{2}'", dt, columnname, dt2.AddDays(1));
                    break;
                //This Quart
                case 10:
                    dt = DateTime.Today;
                    if (dt.Month >= 1 && dt.Month <= 3)
                    {
                        dt2 = new DateTime(dt.Year, 1, 1);
                        dt3 = new DateTime(dt.Year, 3, DateTime.DaysInMonth(dt.Year, 3));
                    }
                    else if (dt.Month >= 4 && dt.Month <= 6)
                    {
                        dt2 = new DateTime(dt.Year, 4, 1);
                        dt3 = new DateTime(dt.Year, 6, DateTime.DaysInMonth(dt.Year, 6));
                    }
                    else if (dt.Month >= 7 && dt.Month <= 9)
                    {
                        dt2 = new DateTime(dt.Year, 7, 1);
                        dt3 = new DateTime(dt.Year, 9, DateTime.DaysInMonth(dt.Year, 9));
                    }
                    else
                    {
                        dt2 = new DateTime(dt.Year, 10, 1);
                        dt3 = new DateTime(dt.Year, 12, DateTime.DaysInMonth(dt.Year, 12));
                    }
                    UserControl1.DateColumnFilterString = string.Format(" >= '{0}' and [{1}] <= '{2}'", dt2, columnname, dt3.AddDays(1));
                    break;
                //Last Quart
                case 11:
                    dt = DateTime.Today;
                    if (dt.Month >= 1 && dt.Month <= 3)
                    {
                        dt2 = new DateTime(dt.Year - 1, 10, 1);
                        dt3 = new DateTime(dt.Year - 1, 12, DateTime.DaysInMonth(dt.Year - 1, 12));
                    }
                    else if (dt.Month >= 4 && dt.Month <= 6)
                    {
                        dt2 = new DateTime(dt.Year, 1, 1);
                        dt3 = new DateTime(dt.Year, 3, DateTime.DaysInMonth(dt.Year, 3));
                    }
                    else if (dt.Month >= 7 && dt.Month <= 9)
                    {
                        dt2 = new DateTime(dt.Year, 4, 1);
                        dt3 = new DateTime(dt.Year, 6, DateTime.DaysInMonth(dt.Year, 6));
                    }
                    else
                    {
                        dt2 = new DateTime(dt.Year, 7, 1);
                        dt3 = new DateTime(dt.Year, 9, DateTime.DaysInMonth(dt.Year, 9));
                    }
                    UserControl1.DateColumnFilterString = string.Format(" >= '{0}' and [{1}] <= '{2}'", dt2, columnname, dt3.AddDays(1));
                    break;
                //This Year
                case 12:
                    dt = DateTime.Today;
                    dt2 = new DateTime(dt.Year, 1, 1);
                    dt3 = new DateTime(dt.Year, 12, 31);
                    UserControl1.DateColumnFilterString = string.Format(" >= '{0}' and [{1}] <= '{2}'", dt2, columnname, dt3.AddDays(1));
                    break;
                //Last Year
                case 13:
                    dt = DateTime.Today;
                    dt2 = new DateTime(dt.Year - 1, 1, 1);
                    dt3 = new DateTime(dt.Year - 1, 12, 31);
                    UserControl1.DateColumnFilterString = string.Format(" >= '{0}' and [{1}] <= '{2}'", dt2, columnname, dt3.AddDays(1));
                    break;
                default:
                    filtr = "Case empty";
                    break;
            }
            Close();
        }
        //Cancel button
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (oldfilter != "")
            {
                UserControl1.DateColumnFilterString = oldfilter[columnname.Length..];
                Close();
            }
            else
            {
                UserControl1.DateColumnFilterString = string.Empty;
                Close();
            }
        }
        private void ShowExpanded()
        {
            if (this.Height == 260)
            {
                DoubleAnimation Window_H = new()
                {
                    From = 260,
                    To = 315,
                    Duration = TimeSpan.FromSeconds(0.8)
                };
                DoubleAnimation GroupBox_H = new()
                {
                    From = 180,
                    To = 255,
                    Duration = TimeSpan.FromSeconds(0.8)
                };
                this.BeginAnimation(HeightProperty, Window_H);
                GroupBox.BeginAnimation(HeightProperty, GroupBox_H);
                calendar_2.Visibility = Visibility.Visible;
            }

        }
        private void ShowReduced()
        {
            if (this.Height == 315)
            {
                DoubleAnimation Window_H = new()
                {
                    From = 315,
                    To = 260,
                    Duration = TimeSpan.FromSeconds(0.8)
                };
                DoubleAnimation GroupBox_H = new()
                {
                    From = 255,
                    To = 180,
                    Duration = TimeSpan.FromSeconds(0.8)
                };
                this.BeginAnimation(HeightProperty, Window_H);
                GroupBox.BeginAnimation(HeightProperty, GroupBox_H);
                calendar_2.Visibility = Visibility.Collapsed;
            }

        }
        #endregion
    }
}
