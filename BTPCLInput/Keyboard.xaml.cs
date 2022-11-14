using System;
using System.Windows;
using System.Windows.Controls;


namespace BTPCLInput
{
    /// <summary>
    /// Interaction logic for Keyboard.xaml
    /// </summary>
    public partial class Keyboard : Window
    {
        private TextBox focusedTextBox;
        public TextBox FocusedTextBox
        {
            get { return focusedTextBox; }
            set
            {
                focusedTextBox = value;
                if (focusedTextBox != null)
                {
                    focusedTextBox.Text = "";
                    tb_text.Text = "";
                    this.Show();
                }
                else
                    CloseKeyboard();
            }
        }
        public Keyboard()
        {
            InitializeComponent();
            WindowPosition();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = sender as Button;
                tb_text.Text = tb_text.Text + DecodeKeyFromKeybord(btn);
                this.focusedTextBox.Text = this.focusedTextBox.Text + DecodeKeyFromKeybord(btn);
            }
            catch { }

        }
        private string DecodeKeyFromKeybord(Button btn)
        {
            return btn.Content.ToString();
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            tb_text.Text = "";
            this.focusedTextBox.Text = "";
        }
        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            CloseKeyboard();
        }
        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            if (tb_text.Text != "")
            {
                tb_text.Text = tb_text.Text.Substring(0, tb_text.Text.Length - 1);
                this.focusedTextBox.Text = this.focusedTextBox.Text.Substring(0, this.focusedTextBox.Text.Length - 1);
            }
        }
        private void WindowPosition()
        {
            this.Topmost = true;
            //this.Left = Application.Current.MainWindow.Left + Application.Current.MainWindow.Width - this.Width;
            //this.Top = Application.Current.MainWindow.Top + Application.Current.MainWindow.Height - this.Height;
            this.Left = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Top = System.Windows.SystemParameters.PrimaryScreenHeight;
        }
        private void Window_Deactivated(object sender, EventArgs e)
        {
            CloseKeyboard();
        }
        private void CloseKeyboard()
        {
            this.Hide();
        }


        //public void ClearFocus()
        //{
        //    try
        //    {
        //        if (focusedTextBox is not null)
        //        {
        //            FrameworkElement parent = (FrameworkElement)focusedTextBox.Parent;
        //            while (parent != null && parent is IInputElement && !((IInputElement)parent).Focusable)
        //            {
        //                parent = (FrameworkElement)parent.Parent;
        //            }
        //            DependencyObject scope = FocusManager.GetFocusScope(focusedTextBox);
        //            FocusManager.SetFocusedElement(scope, parent as IInputElement);
        //        }
        //    }
        //    catch { };
        //}
    }
}
