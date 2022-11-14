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

namespace BTPControlLibrary.Standard
{
    /// <summary>
    /// Interaction logic for TowerState.xaml
    /// </summary>
    public partial class TowerState : UserControl
    {
        public TowerState()
        {
            InitializeComponent();
        }

        private int _state;
        private Color _lasttowerColor;
        private Color _actualtowerColor;

        public int State
        {
            get
            {
                return _state;
            }
            set
            {
                _lasttowerColor = _actualtowerColor;
                _state = value;
                if (_state == 0)
                    _actualtowerColor = Colors.Lime;
                else if (_state == 1)
                    _actualtowerColor = Colors.Yellow;
                else if (_state == 2)
                    _actualtowerColor = Colors.Tomato;
                else if (_state == 3)
                    _actualtowerColor = Colors.DeepSkyBlue;
                else
                    _actualtowerColor = Colors.Gray;

                ColorAnimation();
            }
        }

        public void ColorAnimation()
        {
            ColorAnimation colanim = new ColorAnimation();
            this.Background = new SolidColorBrush(_lasttowerColor);
            colanim.From = _lasttowerColor;
            colanim.To = _actualtowerColor;
            colanim.Duration = TimeSpan.FromSeconds(0.5);
            this.Background.BeginAnimation(SolidColorBrush.ColorProperty, colanim);
        }
    }
}
