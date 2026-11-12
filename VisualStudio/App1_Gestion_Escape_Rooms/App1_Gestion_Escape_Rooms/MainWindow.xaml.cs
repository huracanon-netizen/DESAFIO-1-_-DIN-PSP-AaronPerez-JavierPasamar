using System.Windows;
using System.Windows.Controls;
using App1_Gestion_Escape_Rooms.Paginas;

namespace App1_Gestion_Escape_Rooms
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new ClientesPage());
        }

        private void MenuClientes_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ClientesPage());
        }

        private void MenuTematicas_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Pantalla Temáticas (aún no creada)");
        }

        private void MenuSalas_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Pantalla Salas (aún no creada)");
        }

        private void MenuMonitores_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Pantalla Monitores (aún no creada)");
        }

        private void MenuReservas_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Pantalla Reservas (aún no creada)");
        }

        private void MenuResultados_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Pantalla Resultados (aún no creada)");
        }

        private void MenuProcesarReserva_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Aquí enviaremos la reserva a la App2");
        }

        private void MenuAcercaDe_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Gestión Escape Rooms\nVersión 1.0\nAutores: ...");
        }
    }
}