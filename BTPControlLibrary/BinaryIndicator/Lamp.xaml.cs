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

namespace BTPControlLibrary.BinaryIndicator
{
    public partial class Lamp : UserControl
    {
        public Lamp()
        {
            InitializeComponent();
        }

        #region Properties

        private Color _color;

        public Color DefaultColor
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
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
                CreateColor();

            }
        }

        private void CreateColor()
        {
            RadialGradientBrush rgb = new RadialGradientBrush();
            rgb.Center = new Point(0.6, 0.35);
            rgb.GradientOrigin = new Point(0.6, 0.35);
            rgb.RadiusX = 0.67;
            rgb.RadiusY = 0.67;

            TransformGroup tg = new TransformGroup();

            ScaleTransform st = new ScaleTransform();
            st.CenterX = 0.6;
            st.CenterY = 0.35;
            st.ScaleX = 1;
            st.ScaleY = 1;
            tg.Children.Add(st);

            SkewTransform st2 = new SkewTransform();
            st2.AngleX = 0;
            st2.AngleY = 0;
            st2.CenterX = 0.6;
            st2.CenterY = 0.35;
            tg.Children.Add(st2);

            RotateTransform rt = new RotateTransform();
            rt.Angle = -4.447;
            rt.CenterX = 0.6;
            rt.CenterY = 0.35;
            tg.Children.Add(rt);

            TranslateTransform tt = new TranslateTransform();
            tt.X = 0;
            tt.Y = 0;
            tg.Children.Add(tt);

            rgb.RelativeTransform = tg;

            GradientStop gs1 = new GradientStop();
            //gs1.Color = Colors.White;
            gs1.Color = Color.FromArgb(_value ? (byte)105 : (byte)20, DefaultColor.R, DefaultColor.G, DefaultColor.B);
            gs1.Offset = 0;

            GradientStop gs2 = new GradientStop();
            gs2.Color = Color.FromArgb(_value ? (byte)255 : (byte)50, DefaultColor.R, DefaultColor.G, DefaultColor.B);
            gs1.Offset = 1;

            rgb.GradientStops.Add(gs1);
            rgb.GradientStops.Add(gs2);

            ellipse.Fill = rgb;

        }
        #endregion

    }

}
