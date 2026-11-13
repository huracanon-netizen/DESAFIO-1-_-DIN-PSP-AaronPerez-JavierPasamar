using System.Windows;
using System.Windows.Controls;
using App1_Gestion_Escape_Rooms.Paginas;

namespace App1_Gestion_Escape_Rooms
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                MainFrame.Navigate(new ClientesPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuClientes_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ClientesPage());
        }

        private void MenuTematicas_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TematicasPage());
        }

        private void MenuSalas_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SalasPage());
        }

        private void MenuMonitores_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Pantalla Monitores (aún no creada)");
        }

        private void MenuReservas_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ReservasPage());
        }

        private void MenuResultados_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ResultadosPage());
        }

        private void MenuProcesarReserva_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProcesarReservaPage());
        }

        private void MenuAcercaDe_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Gestión Escape Rooms\nVersión 1.0\nAutores: Aarón Pérez Ruiz y Javier Pasamar");
        }
    }
}