using System.Windows;

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
            LW1 lw1 = new LW1();
            lw1.ShowDialog();
        }

        private void LR3_MolecularDynamics_Click(object sender, RoutedEventArgs e)
        {
            LW3_MolecularDynamicsMethod lW3_MolecularDynamicsMethod = new LW3_MolecularDynamicsMethod();
            lW3_MolecularDynamicsMethod.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LW1_PendulumWindow lW1_PendulumWindow = new LW1_PendulumWindow();
            lW1_PendulumWindow.ShowDialog();
        }
    }
}
