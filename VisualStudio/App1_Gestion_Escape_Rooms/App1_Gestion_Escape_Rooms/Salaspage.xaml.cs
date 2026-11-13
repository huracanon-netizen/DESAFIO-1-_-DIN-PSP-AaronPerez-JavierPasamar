using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using App1_Gestion_Escape_Rooms.Models;
using System.Windows;
using System.Windows.Controls;

namespace App1_Gestion_Escape_Rooms.Paginas
{
    public partial class SalasPage : Page
    {
        private readonly IDbContextFactory<EscapeRoomsContext> escapeRoomsContext;
        private int? idEditando = null;

        public SalasPage()
        {
            try
            {
                InitializeComponent();

                escapeRoomsContext = App.proveedorServicios.GetRequiredService
                    <IDbContextFactory<EscapeRoomsContext>>();

                CargarCombos();
                CargarSalas();
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
                cbTematica.ItemsSource = contexto.Tematicas.ToList();
            }
        }

        private void CargarSalas()
        {
            using (EscapeRoomsContext contexto = escapeRoomsContext.CreateDbContext())
            {
                dgSalas.ItemsSource = contexto.Salas
                    .Include(s => s.IdTematicaNavigation)
                    .ToList();
            }
        }

        private void LimpiarFormulario()
        {
            idEditando = null;
            txtNombre.Text = "";
            txtDificultad.Text = "";
            txtDuracion.Text = "";
            txtMinJugadores.Text = "";
            txtMaxJugadores.Text = "";
            txtPrecio.Text = "";
            cbTematica.SelectedIndex = -1;
            dgSalas.SelectedItem = null;
        }

        private void dgSalas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Sala? sala = dgSalas.SelectedItem as Sala;
            if (sala == null)
            {
                return;
            }

            idEditando = sala.IdSala;
            txtNombre.Text = sala.Nombre;
            txtDificultad.Text = sala.Dificultad;
            txtDuracion.Text = sala.DuracionMaxMin.ToString();
            txtMinJugadores.Text = sala.MinJugadores.ToString();
            txtMaxJugadores.Text = sala.MaxJugadores.ToString();
            txtPrecio.Text = sala.Precio.ToString("0.00");
            cbTematica.SelectedValue = sala.IdTematica;
        }

        private void Limpiar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string nombre = txtNombre.Text.Trim();
                if (nombre == "")
                {
                    MessageBox.Show("El nombre es obligatorio.");
                    return;
                }

                string dificultad = txtDificultad.Text.Trim();
                if (dificultad == "")
                {
                    MessageBox.Show("La dificultad es obligatoria.");
                    return;
                }

                int duracion;
                if (!int.TryParse(txtDuracion.Text.Trim(), out duracion) || duracion <= 0)
                {
                    MessageBox.Show("La duración máxima debe ser un entero mayor que 0.");
                    return;
                }

                int minJugadores;
                int maxJugadores;
                if (!int.TryParse(txtMinJugadores.Text.Trim(), out minJugadores) || minJugadores <= 0 ||
                    !int.TryParse(txtMaxJugadores.Text.Trim(), out maxJugadores) || maxJugadores <= 0)
                {
                    MessageBox.Show("Los jugadores mínimo y máximo deben ser enteros mayores que 0.");
                    return;
                }

                if (minJugadores > maxJugadores)
                {
                    MessageBox.Show("El mínimo de jugadores no puede ser mayor que el máximo.");
                    return;
                }

                decimal precio;
                if (!decimal.TryParse(txtPrecio.Text.Trim(), out precio) || precio < 0)
                {
                    MessageBox.Show("El precio debe ser un número válido (por ejemplo 15,50).");
                    return;
                }

                int? idTematica = null;
                if (cbTematica.SelectedValue != null)
                {
                    idTematica = (int)cbTematica.SelectedValue;
                }

                using (EscapeRoomsContext contexto = escapeRoomsContext.CreateDbContext())
                {
                    bool repetida = contexto.Salas.Any(s =>
                        s.Nombre == nombre &&
                        s.IdSala != (idEditando ?? 0));

                    if (repetida)
                    {
                        MessageBox.Show("Ya existe una sala con ese nombre.");
                        return;
                    }

                    Sala sala;
                    if (idEditando == null)
                    {
                        sala = new Sala();
                        contexto.Salas.Add(sala);
                    }
                    else
                    {
                        sala = contexto.Salas.First(s => s.IdSala == idEditando.Value);
                    }

                    sala.Nombre = nombre;
                    sala.Dificultad = dificultad;
                    sala.DuracionMaxMin = duracion;
                    sala.MinJugadores = minJugadores;
                    sala.MaxJugadores = maxJugadores;
                    sala.Precio = precio;
                    sala.IdTematica = idTematica;

                    contexto.SaveChanges();
                }

                LimpiarFormulario();
                CargarSalas();
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
                Sala? seleccionada = dgSalas.SelectedItem as Sala;
                if (seleccionada == null)
                {
                    MessageBox.Show("Selecciona una sala de la tabla.");
                    return;
                }

                if (MessageBox.Show("¿Eliminar la sala " + seleccionada.Nombre + "?", "Confirmar",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                {
                    return;
                }

                using (EscapeRoomsContext contexto = escapeRoomsContext.CreateDbContext())
                {
                    Sala sala = contexto.Salas
                        .Include(s => s.Reservas)
                        .Include(s => s.MonitorHasSalas)
                        .First(s => s.IdSala == seleccionada.IdSala);

                    if (sala.Reservas.Count > 0)
                    {
                        MessageBox.Show("No se puede eliminar: la sala tiene reservas asociadas.");
                        return;
                    }

                    if (sala.MonitorHasSalas.Count > 0)
                    {
                        MessageBox.Show("No se puede eliminar: la sala tiene monitores asignados.");
                        return;
                    }

                    contexto.Salas.Remove(sala);
                    contexto.SaveChanges();
                }

                LimpiarFormulario();
                CargarSalas();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}