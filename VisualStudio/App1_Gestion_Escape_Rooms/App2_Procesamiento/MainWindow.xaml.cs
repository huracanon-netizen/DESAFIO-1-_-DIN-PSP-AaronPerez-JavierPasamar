using System.Collections.ObjectModel;
using System.IO;
using System.IO.Pipes;
using System.Text.Json;
using System.Windows;

namespace App2_Procesamiento
{
    public partial class MainWindow : Window
    {
        private const string RutaRegistro = "registro_peticiones.json";
        private ObservableCollection<PeticionRegistro> peticiones = new ObservableCollection<PeticionRegistro>();

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                dgPeticiones.ItemsSource = peticiones;
                CargarRegistro();

                Task.Run(EscucharPipe);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CargarRegistro()
        {
            try
            {
                using (StreamReader lector = new StreamReader(RutaRegistro))
                {
                    string json = lector.ReadToEnd();
                    List<PeticionRegistro>? lista = JsonSerializer.Deserialize<List<PeticionRegistro>>(json);
                    if (lista != null)
                    {
                        foreach (PeticionRegistro peticion in lista)
                        {
                            peticiones.Add(peticion);
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se ha podido cargar el registro: " + ex.Message);
            }
        }

        private void GuardarRegistro()
        {
            try
            {
                JsonSerializerOptions opciones = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                using (StreamWriter escritor = new StreamWriter(RutaRegistro))
                {
                    escritor.Write(JsonSerializer.Serialize(peticiones, opciones));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se ha podido guardar el registro: " + ex.Message);
            }
        }

        private async Task EscucharPipe()
        {
            while (true)
            {
                PeticionRegistro registro = new PeticionRegistro();
                registro.TipoOperacion = "Cálculo de puntuación";

                try
                {
                    using (NamedPipeServerStream servidor = new NamedPipeServerStream("EscapeRoomsPipe", PipeDirection.InOut))
                    {
                        servidor.WaitForConnection();
                        registro.FechaHoraRecepcion = DateTime.Now;

                        StreamReader lector = new StreamReader(servidor);
                        StreamWriter escritor = new StreamWriter(servidor);
                        escritor.AutoFlush = true;

                        string? json = lector.ReadLine();
                        registro.JsonRecibido = json;

                        Respuesta respuesta = ProcesarMensaje(json);

                        escritor.WriteLine(JsonSerializer.Serialize(respuesta));
                        servidor.WaitForPipeDrain();

                        registro.IdPeticion = respuesta.idSolicitud;
                        registro.EstadoFinal = respuesta.estado;
                        registro.CampoError = respuesta.campoError;
                        registro.Puntuacion = respuesta.puntuacion;
                        registro.Clasificacion = respuesta.clasificacion;
                        if (respuesta.estado == "Error técnico")
                        {
                            registro.ErrorTecnico = respuesta.detalle;
                        }
                        else
                        {
                            registro.Detalle = respuesta.detalle;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (registro.FechaHoraRecepcion == DateTime.MinValue)
                    {
                        registro.FechaHoraRecepcion = DateTime.Now;
                    }
                    registro.EstadoFinal = "Error técnico";
                    registro.ErrorTecnico = ex.Message;
                    await Task.Delay(1000);
                }

                if (registro.IdPeticion == "")
                {
                    registro.IdPeticion = "(sin identificador)";
                }

                await Dispatcher.InvokeAsync(() =>
                {
                    peticiones.Add(registro);
                    GuardarRegistro();
                });
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
                respuesta.estado = "Error técnico";
                respuesta.detalle = ex.Message;
                return respuesta;
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
                return Error(respuesta, "reserva.valida", "La reserva no es válida (no está pagada)");
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

        private void VerDetalle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgPeticiones.SelectedItem == null)
                {
                    MessageBox.Show("Selecciona una petición de la tabla.");
                    return;
                }
                PeticionRegistro seleccionada = (PeticionRegistro)dgPeticiones.SelectedItem;

                DetallePeticionWindow ventana = new DetallePeticionWindow(seleccionada);
                ventana.Owner = this;
                ventana.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AcercaDe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AcercaDeWindow ventana = new AcercaDeWindow();
                ventana.Owner = this;
                ventana.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}