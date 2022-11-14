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
using BTPUtilities;
using BTPTwinCatADS;

namespace BTPCLTransport.Visualization
{
    /// <summary>
    /// Interaction logic for StandardTransport.xaml
    /// </summary>
    public partial class StandardTransport : BTPControlLibrary.MainPanel
    {
        public StandardTransport()
        {
            InitializeComponent();
            //tower1.TowerState = 0;
        }
        protected override void InternalInit()
        {
        }
        public override void InputInterface(String name, Object value)
        {
            base.InputInterface(name, value);
            if (name.EqualsIgnoreCase("TOWER_1.HMI_LIFEWORD"))
            {
                short t = ((short)value);
            }
        }

        private void State1_Click(object sender, MouseButtonEventArgs e)
        {
            // tower1.TowerState += 1;
        }

        private void State_1_Click(object sender, MouseButtonEventArgs e)
        {
            //.TowerState += 1;
        }
    }
}
