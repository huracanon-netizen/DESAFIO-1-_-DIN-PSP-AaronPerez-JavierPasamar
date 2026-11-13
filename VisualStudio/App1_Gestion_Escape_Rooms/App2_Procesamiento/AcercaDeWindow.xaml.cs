using System.Windows;

namespace App2_Procesamiento
{
    public partial class AcercaDeWindow : Window
    {
        public AcercaDeWindow()
        {
            InitializeComponent();
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}