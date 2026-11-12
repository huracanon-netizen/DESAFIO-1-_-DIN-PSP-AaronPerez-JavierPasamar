using System.IO;
using System.IO.Pipes;
using System.Text.Json;
using System.Windows;

namespace App2_Procesamiento
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Task.Run(EscucharPipe);
        }

        private async Task EscucharPipe()
        {
            while (true)
            {
                try
                {
                    using (NamedPipeServerStream servidor = new NamedPipeServerStream("EscapeRoomsPipe", PipeDirection.InOut))
                    {
                        servidor.WaitForConnection();

                        StreamReader lector = new StreamReader(servidor);
                        StreamWriter escritor = new StreamWriter(servidor);
                        escritor.AutoFlush = true;

                        string? json = lector.ReadLine();

                        await Dispatcher.InvokeAsync(() =>
                        {
                            lstPeticiones.Items.Add("Petición recibida");
                        });

                        Respuesta respuesta = ProcesarMensaje(json);

                        escritor.WriteLine(JsonSerializer.Serialize(respuesta));

                        await Dispatcher.InvokeAsync(() =>
                        {
                            lstPeticiones.Items.Add(respuesta.idSolicitud + " - " + respuesta.estado);
                        });
                    }
                }
                catch (Exception ex)
                {
                    await Dispatcher.InvokeAsync(() =>
                    {
                        lstPeticiones.Items.Add("Error técnico: " + ex.Message);
                    });
                }
            }
        }

        private Respuesta ProcesarMensaje(string? json)
        {
            Respuesta respuesta = new Respuesta();

            if (json == null || json.Trim() == "")
            {
                return Error(respuesta, "solicitud", "No se ha recibido ningún dato");
            }

            try
            {
                Solicitud? solicitud = JsonSerializer.Deserialize<Solicitud>(json);
                if (solicitud == null)
                {
                    return Error(respuesta, "solicitud", "El JSON recibido está vacío");
                }
                return Procesar(solicitud);
            }
            catch (JsonException ex)
            {
                return Error(respuesta, "json", ex.Message);
            }
        }

        private Respuesta Procesar(Solicitud solicitud)
        {
            Respuesta respuesta = new Respuesta();
            respuesta.idSolicitud = solicitud.idSolicitud;

            if (solicitud.idSolicitud == "")
            {
                return Error(respuesta, "idSolicitud", "El identificador de la solicitud es obligatorio");
            }
            if (solicitud.reserva.valida == false)
            {
                return Error(respuesta, "reserva.valida", "La reserva no es válida");
            }
            if (solicitud.reserva.numJugadores <= 0)
            {
                return Error(respuesta, "reserva.numJugadores", "El número de jugadores debe ser mayor que 0");
            }
            if (solicitud.partida.duracionMaxMin <= 0)
            {
                return Error(respuesta, "partida.duracionMaxMin", "La duración máxima debe ser mayor que 0");
            }
            if (solicitud.partida.tiempoSegundos <= 0)
            {
                return Error(respuesta, "partida.tiempoSegundos", "El tiempo debe ser mayor que 0");
            }
            if (solicitud.partida.tiempoSegundos > solicitud.partida.duracionMaxMin * 60)
            {
                return Error(respuesta, "partida.tiempoSegundos", "El tiempo supera la duración máxima de la sala");
            }
            if (solicitud.partida.pistasUsadas < 0)
            {
                return Error(respuesta, "partida.pistasUsadas", "Las pistas usadas no pueden ser negativas");
            }

            int tiempoMaximo = solicitud.partida.duracionMaxMin * 60;
            int puntuacion = 1000 - (solicitud.partida.tiempoSegundos * 200 / tiempoMaximo) - solicitud.partida.pistasUsadas * 20;
            if (puntuacion < 0)
            {
                puntuacion = 0;
            }

            string clasificacion;
            if (puntuacion >= 750)
            {
                clasificacion = "Excelente";
            }
            else if (puntuacion >= 500)
            {
                clasificacion = "Normal";
            }
            else
            {
                clasificacion = "Mejorable";
            }

            respuesta.estado = "Procesada correctamente";
            respuesta.puntuacion = puntuacion;
            respuesta.clasificacion = clasificacion;
            return respuesta;
        }

        private Respuesta Error(Respuesta respuesta, string campoError, string detalle)
        {
            respuesta.estado = "Error de validación";
            respuesta.campoError = campoError;
            respuesta.detalle = detalle;
            return respuesta;
        }
    }
}