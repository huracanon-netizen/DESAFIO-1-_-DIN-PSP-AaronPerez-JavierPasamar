using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using App1_Gestion_Escape_Rooms.Models;
using System.IO;
using System.IO.Pipes;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace App1_Gestion_Escape_Rooms.Paginas
{
    public partial class ProcesarReservaPage : Page
    {
        private readonly IDbContextFactory<EscapeRoomsContext> escapeRoomsContext;
        private static int contadorSolicitudes = 0;

        public ProcesarReservaPage()
        {
            try
            {
                InitializeComponent();

                escapeRoomsContext = App.proveedorServicios.GetRequiredService
                    <IDbContextFactory<EscapeRoomsContext>>();

                CargarReservas();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CargarReservas()
        {
            using (EscapeRoomsContext contexto = escapeRoomsContext.CreateDbContext())
            {
                contexto.Clientes.ToList();
                contexto.Salas.ToList();
                dgReservas.ItemsSource = contexto.Reservas.ToList();
            }
        }

        private string GenerarIdSolicitud()
        {
            int numero = 0;
            using (EscapeRoomsContext contexto = escapeRoomsContext.CreateDbContext())
            {
                foreach (Resultado resultado in contexto.Resultados.ToList())
                {
                    if (resultado.IdResultado > numero)
                    {
                        numero = resultado.IdResultado;
                    }
                }
            }
            if (contadorSolicitudes > numero)
            {
                numero = contadorSolicitudes;
            }
            numero = numero + 1;
            contadorSolicitudes = numero;
            return "SOL-2026-" + numero.ToString().PadLeft(3, '0');
        }

        private string? EnviarPorPipe(string jsonSolicitud)
        {
            using (NamedPipeClientStream cliente = new NamedPipeClientStream(".", "EscapeRoomsPipe", PipeDirection.InOut))
            {
                cliente.Connect(3000);

                StreamWriter escritor = new StreamWriter(cliente);
                escritor.AutoFlush = true;
                StreamReader lector = new StreamReader(cliente);

                escritor.WriteLine(jsonSolicitud);
                return lector.ReadLine();
            }
        }

        private async void Enviar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgReservas.SelectedItem == null)
                {
                    MessageBox.Show("Selecciona una reserva de la tabla.");
                    return;
                }
                Reserva reserva = (Reserva)dgReservas.SelectedItem;

                int tiempoMinutos;
                int pistas;
                if (!int.TryParse(txtTiempo.Text.Trim(), out tiempoMinutos) || !int.TryParse(txtPistas.Text.Trim(), out pistas))
                {
                    MessageBox.Show("El tiempo (en minutos) y las pistas deben ser números enteros.");
                    return;
                }

                int tiempo = tiempoMinutos * 60;

                Solicitud solicitud = new Solicitud();
                solicitud.idSolicitud = GenerarIdSolicitud();
                solicitud.reserva.idReserva = reserva.IdReserva;
                solicitud.reserva.sala = reserva.SalaIdSalaNavigation.Nombre;
                solicitud.reserva.numJugadores = reserva.NumJugadores;
                solicitud.reserva.valida = reserva.Pagada == 1;
                solicitud.partida.tiempoSegundos = tiempo;
                solicitud.partida.duracionMaxMin = reserva.SalaIdSalaNavigation.DuracionMaxMin;
                solicitud.partida.pistasUsadas = pistas;

                string jsonSolicitud = JsonSerializer.Serialize(solicitud);

                btnEnviar.IsEnabled = false;
                txtEstado.Text = "Enviando " + solicitud.idSolicitud + " a la App 2...";

                string? jsonRespuesta = await Task.Run(() => EnviarPorPipe(jsonSolicitud));

                if (jsonRespuesta == null)
                {
                    MessageBox.Show("La App 2 no ha devuelto ninguna respuesta.");
                    txtEstado.Text = "Sin respuesta.";
                    return;
                }

                Respuesta? respuesta = JsonSerializer.Deserialize<Respuesta>(jsonRespuesta);
                if (respuesta == null)
                {
                    MessageBox.Show("La respuesta recibida está vacía.");
                    return;
                }

                if (respuesta.estado != "Procesada correctamente")
                {
                    MessageBox.Show(respuesta.estado + "\nCampo: " + respuesta.campoError + "\n" + respuesta.detalle);
                    txtEstado.Text = respuesta.idSolicitud + ": " + respuesta.estado;
                    return;
                }

                using (EscapeRoomsContext contexto = escapeRoomsContext.CreateDbContext())
                {
                    Resultado resultado = new Resultado();
                    resultado.IdSolicitud = respuesta.idSolicitud;
                    resultado.TiempoSegundos = tiempo;
                    resultado.PistasUsadas = pistas;
                    resultado.Puntuacion = respuesta.puntuacion;
                    resultado.Clasificacion = respuesta.clasificacion;
                    resultado.Estado = respuesta.estado;
                    resultado.FechaProceso = DateTime.Now;
                    resultado.ReservaIdReserva = reserva.IdReserva;

                    contexto.Resultados.Add(resultado);
                    contexto.SaveChanges();
                }

                txtTiempo.Text = "";
                txtPistas.Text = "";
                txtEstado.Text = respuesta.idSolicitud + ": " + respuesta.puntuacion + " puntos - " + respuesta.clasificacion;
            }
            catch (TimeoutException)
            {
                MessageBox.Show("No se ha podido conectar con la App 2. Comprueba que está abierta.");
                txtEstado.Text = "App 2 no disponible.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                btnEnviar.IsEnabled = true;
            }
        }
    }
}