using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using App1_Gestion_Escape_Rooms.Models;
using System.Windows;
using System.Windows.Controls;

namespace App1_Gestion_Escape_Rooms.Paginas
{
    public partial class ResultadosPage : Page
    {
        private readonly IDbContextFactory<EscapeRoomsContext> escapeRoomsContext;

        public ResultadosPage()
        {
            try
            {
                InitializeComponent();

                escapeRoomsContext = App.proveedorServicios.GetRequiredService
                    <IDbContextFactory<EscapeRoomsContext>>();

                CargarResultados();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CargarResultados()
        {
            using (EscapeRoomsContext contexto = escapeRoomsContext.CreateDbContext())
            {
                dgResultados.ItemsSource = contexto.Resultados.ToList();
            }
        }

        private void Actualizar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CargarResultados();
                txtEstado.Text = "Lista actualizada.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}