using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace BTPCLMain
{
    public partial class Login : BTPControlLibrary.MainPanel
    {

        public Login() { }

        #region Events
        public delegate void doLogin(string username, string password);
        public event doLogin? DoLoginEvent;
        #endregion

        protected override void InternalInit()
        {
            DataContext = this;
            InitializeComponent();
            FillComboboxAvailableLanguage();
            SetLastTimeSelectedLanguage();
        }

        #region LANGUAGE
        private void SetLastTimeSelectedLanguage()
        {
            language.SelectedIndex =Convert.ToInt16(_cfg.GetParamStr("LANG_Selected").Substring(4, 1)) -1;
        }

        private void FillComboboxAvailableLanguage()
        {
            for (int i = 0; i < _cfg.GetParamInt("LANG_Count"); i++)
            {
                string lang = "lang" + i;
                language.Items.Add(_translation.GetTransalatedLanguageList("lang1", i));
            }
        }

        private void language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        #endregion

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            DoLoginEvent?.Invoke(user_login.Text, user_password.Password);
        }

        public void ClearTB()
        {
            user_login.Text = "";
            user_password.Password = "";
        }


        public void WrongUsernameOrPassword()
        {
            ColorAnimation flash = new(Colors.White, Colors.Red, new Duration(TimeSpan.FromSeconds(0.1))) { AutoReverse = true, RepeatBehavior = new RepeatBehavior(5) };
            user_login.Background = new SolidColorBrush(Colors.White);
            user_login.Background.BeginAnimation(SolidColorBrush.ColorProperty, flash);
            user_password.Background = new SolidColorBrush(Colors.White);
            user_password.Background.BeginAnimation(SolidColorBrush.ColorProperty, flash);

            user_password.Password = "";
        }
    }
}
