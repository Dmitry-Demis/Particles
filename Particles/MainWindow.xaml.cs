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

namespace Particles
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LR1_Pendulum_Click(object sender, RoutedEventArgs e)
        {
            LW1_PendulumWindow lw1PendulumWindow = new LW1_PendulumWindow();
            lw1PendulumWindow.ShowDialog();
        }

        private void LR3_MolecularDynamics_Click(object sender, RoutedEventArgs e)
        {
            LW3_MolecularDynamicsMethod lW3_MolecularDynamicsMethod = new LW3_MolecularDynamicsMethod();
            lW3_MolecularDynamicsMethod.ShowDialog();
        }
    }
}
