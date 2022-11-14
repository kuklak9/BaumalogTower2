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
using System.Windows.Shapes;

namespace BTPCLPopup
{
    /// <summary>
    /// Interaction logic for MessagePopup.xaml
    /// </summary>
    public partial class MessagePopup : Window
    {
        public MessagePopup(bool yesnoButton)
        {

            InitializeComponent();


            OKButton.Visibility = yesnoButton == true ? Visibility.Hidden : Visibility.Visible;
            YesButton.Visibility = yesnoButton == true ? Visibility.Visible : Visibility.Hidden;
            NoButton.Visibility = yesnoButton == true ? Visibility.Visible : Visibility.Hidden;
        }

        private string _caption;

        public string Caption
        {
            get
            {
                return _caption;
            }
            set
            {
                _caption = value;
                CaptionTextBlock.Text = _caption;
            }
        }

        private string _description;

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                DescriptionTextBlock.Text = _description;
            }
        }

        private bool _value;

        public bool Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }




        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            Value = true;
            this.Close();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void No_Click(object sender, RoutedEventArgs e)
        {
            Value = false;
            this.Close();
        }
    }
}
