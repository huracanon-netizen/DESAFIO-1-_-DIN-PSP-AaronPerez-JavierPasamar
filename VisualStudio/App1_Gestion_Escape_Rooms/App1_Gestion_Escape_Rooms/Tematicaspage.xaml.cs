using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using App1_Gestion_Escape_Rooms.Models;
using System.Windows;
using System.Windows.Controls;

namespace App1_Gestion_Escape_Rooms.Paginas
{
    public partial class TematicasPage : Page
    {
        private readonly IDbContextFactory<EscapeRoomsContext> escapeRoomsContext;
        private int? idEditando = null;

        public TematicasPage()
        {
            try
            {
                InitializeComponent();

                escapeRoomsContext = App.proveedorServicios.GetRequiredService
                    <IDbContextFactory<EscapeRoomsContext>>();

                CargarTematicas();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CargarTematicas()
        {
            using (EscapeRoomsContext contexto = escapeRoomsContext.CreateDbContext())
            {
                dgTematicas.ItemsSource = contexto.Tematicas.ToList();
            }
        }

        private void LimpiarFormulario()
        {
            idEditando = null;
            txtNombre.Text = "";
            txtDescripcion.Text = "";
            dgTematicas.SelectedItem = null;
        }

        private void dgTematicas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Tematica? tematica = dgTematicas.SelectedItem as Tematica;
            if (tematica == null)
            {
                return;
            }

            idEditando = tematica.IdTematica;
            txtNombre.Text = tematica.Nombre;
            txtDescripcion.Text = tematica.Descripcion;
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

                using (EscapeRoomsContext contexto = escapeRoomsContext.CreateDbContext())
                {
                    bool repetida = contexto.Tematicas.Any(t =>
                        t.Nombre == nombre &&
                        t.IdTematica != (idEditando ?? 0));

                    if (repetida)
                    {
                        MessageBox.Show("Ya existe una temática con ese nombre.");
                        return;
                    }

                    Tematica tematica;
                    if (idEditando == null)
                    {
                        tematica = new Tematica();
                        contexto.Tematicas.Add(tematica);
                    }
                    else
                    {
                        tematica = contexto.Tematicas.First(t => t.IdTematica == idEditando.Value);
                    }

                    tematica.Nombre = nombre;
                    tematica.Descripcion = txtDescripcion.Text.Trim();

                    contexto.SaveChanges();
                }

                LimpiarFormulario();
                CargarTematicas();
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
                Tematica? seleccionada = dgTematicas.SelectedItem as Tematica;
                if (seleccionada == null)
                {
                    MessageBox.Show("Selecciona una temática de la tabla.");
                    return;
                }

                if (MessageBox.Show("¿Eliminar la temática " + seleccionada.Nombre + "?", "Confirmar",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                {
                    return;
                }

                using (EscapeRoomsContext contexto = escapeRoomsContext.CreateDbContext())
                {
                    if (contexto.Salas.Any(s => s.IdTematica == seleccionada.IdTematica))
                    {
                        MessageBox.Show("No se puede eliminar: hay salas que usan esta temática.");
                        return;
                    }

                    Tematica tematica = contexto.Tematicas.First(t => t.IdTematica == seleccionada.IdTematica);
                    contexto.Tematicas.Remove(tematica);
                    contexto.SaveChanges();
                }

                LimpiarFormulario();
                CargarTematicas();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}