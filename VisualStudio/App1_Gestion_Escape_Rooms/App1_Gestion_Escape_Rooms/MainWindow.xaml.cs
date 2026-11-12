using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using App1_Gestion_Escape_Rooms.Models;
using System.Windows;

namespace App1_Gestion_Escape_Rooms
{
    public partial class MainWindow : Window
    {
        private readonly IDbContextFactory<EscapeRoomsContext> escapeRoomsContext;

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                escapeRoomsContext = App.proveedorServicios.GetRequiredService
                    <IDbContextFactory<EscapeRoomsContext>>();

                CargarClientes();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        private void CargarClientes()
        {
            using (EscapeRoomsContext contexto = escapeRoomsContext.CreateDbContext())
            {
                dgClientes.ItemsSource = contexto.Clientes.ToList();
            }

        }
        private void Actualizar_Click(object sender, RoutedEventArgs e)
        {
            CargarClientes();
        }
    }
}