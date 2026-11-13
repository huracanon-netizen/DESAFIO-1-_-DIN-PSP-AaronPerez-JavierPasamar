using App1_Gestion_Escape_Rooms.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace App1_Gestion_Escape_Rooms.Paginas
{
    public partial class ReservasPage : Page
    {
        private readonly IDbContextFactory<EscapeRoomsContext> escapeRoomsContext;
        private int? idEditando = null;

        public ReservasPage()
        {
            try
            {
                InitializeComponent();

                escapeRoomsContext = App.proveedorServicios.GetRequiredService
                    <IDbContextFactory<EscapeRoomsContext>>();

                CargarCombos();
                CargarReservas();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CargarCombos()
        {
            using (EscapeRoomsContext contexto = escapeRoomsContext.CreateDbContext())
            {
                cbCliente.ItemsSource = contexto.Clientes.ToList();
                cbSala.ItemsSource = contexto.Salas.ToList();
            }
        }

        private void CargarReservas()
        {
            using (EscapeRoomsContext contexto = escapeRoomsContext.CreateDbContext())
            {
                dgReservas.ItemsSource = contexto.Reservas
                    .Include(r => r.ClienteIdClienteNavigation)
                    .Include(r => r.SalaIdSalaNavigation)
                    .ToList();
            }
        }

        private void LimpiarFormulario()
        {
            idEditando = null;
            dpFecha.SelectedDate = null;
            txtHora.Text = "";
            txtJugadores.Text = "";
            cbCliente.SelectedIndex = -1;
            cbSala.SelectedIndex = -1;
            chkPagada.IsChecked = false;
            dgReservas.SelectedItem = null;
        }

        private void dgReservas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Reserva? reserva = dgReservas.SelectedItem as Reserva;
            if (reserva == null)
            {
                return;
            }

            idEditando = reserva.IdReserva;
            dpFecha.SelectedDate = reserva.FechaHora.Date;
            txtHora.Text = reserva.FechaHora.ToString("HH:mm");
            txtJugadores.Text = reserva.NumJugadores.ToString();
            cbCliente.SelectedValue = reserva.ClienteIdCliente;
            cbSala.SelectedValue = reserva.SalaIdSala;
            chkPagada.IsChecked = reserva.Pagada == 1;
        }

        private void Limpiar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dpFecha.SelectedDate == null)
                {
                    MessageBox.Show("Selecciona una fecha.");
                    return;
                }

                TimeSpan hora;
                if (!TimeSpan.TryParseExact(txtHora.Text.Trim(), "hh\\:mm", CultureInfo.InvariantCulture, out hora))
                {
                    MessageBox.Show("La hora debe tener el formato HH:mm (por ejemplo 18:30).");
                    return;
                }

                int jugadores;
                if (!int.TryParse(txtJugadores.Text.Trim(), out jugadores) || jugadores <= 0)
                {
                    MessageBox.Show("El número de jugadores debe ser un entero mayor que 0.");
                    return;
                }

                if (cbCliente.SelectedValue == null || cbSala.SelectedValue == null)
                {
                    MessageBox.Show("Selecciona un cliente y una sala.");
                    return;
                }

                DateTime fechaHora = dpFecha.SelectedDate.Value.Date + hora;
                int idCliente = (int)cbCliente.SelectedValue;
                int idSala = (int)cbSala.SelectedValue;

                using (EscapeRoomsContext contexto = escapeRoomsContext.CreateDbContext())
                {
                    // Evitar dos reservas en la misma sala a la misma hora
                    bool ocupada = contexto.Reservas.Any(r =>
                        r.SalaIdSala == idSala &&
                        r.FechaHora == fechaHora &&
                        r.IdReserva != (idEditando ?? 0));

                    if (ocupada)
                    {
                        MessageBox.Show("Esa sala ya tiene una reserva a esa fecha y hora.");
                        return;
                    }

                    Reserva reserva;
                    if (idEditando == null)
                    {
                        reserva = new Reserva();
                        contexto.Reservas.Add(reserva);
                    }
                    else
                    {
                        reserva = contexto.Reservas.First(r => r.IdReserva == idEditando.Value);
                    }

                    reserva.FechaHora = fechaHora;
                    reserva.NumJugadores = jugadores;
                    reserva.ClienteIdCliente = idCliente;
                    reserva.SalaIdSala = idSala;
                    reserva.Pagada = chkPagada.IsChecked == true ? 1 : 0;

                    contexto.SaveChanges();
                }

                LimpiarFormulario();
                CargarReservas();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Reserva? seleccionada = dgReservas.SelectedItem as Reserva;
                if (seleccionada == null)
                {
                    MessageBox.Show("Selecciona una reserva de la tabla.");
                    return;
                }

                if (MessageBox.Show("¿Eliminar la reserva " + seleccionada.IdReserva + "?", "Confirmar",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                {
                    return;
                }

                using (EscapeRoomsContext contexto = escapeRoomsContext.CreateDbContext())
                {
                    if (contexto.Resultados.Any(r => r.ReservaIdReserva == seleccionada.IdReserva))
                    {
                        MessageBox.Show("No se puede eliminar: la reserva ya tiene un resultado asociado.");
                        return;
                    }

                    Reserva reserva = contexto.Reservas.First(r => r.IdReserva == seleccionada.IdReserva);
                    contexto.Reservas.Remove(reserva);
                    contexto.SaveChanges();
                }

                LimpiarFormulario();
                CargarReservas();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}