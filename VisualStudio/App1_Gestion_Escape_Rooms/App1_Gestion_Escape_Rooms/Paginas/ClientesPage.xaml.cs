using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using App1_Gestion_Escape_Rooms.Models;
using System.Windows;
using System.Windows.Controls;

namespace App1_Gestion_Escape_Rooms.Paginas
{
    public partial class ClientesPage : Page
    {
        private readonly IDbContextFactory<EscapeRoomsContext> escapeRoomsContext;

        public ClientesPage()
        {
            InitializeComponent();

            escapeRoomsContext = App.proveedorServicios.GetRequiredService
                <IDbContextFactory<EscapeRoomsContext>>();

            CargarClientes();
        }

        private void CargarClientes()
        {
            using (EscapeRoomsContext contexto = escapeRoomsContext.CreateDbContext())
            {
                dgClientes.ItemsSource = contexto.Clientes.ToList();
            }
        }

        private void LimpiarFormulario()
        {
            txtDni.Text = "";
            txtNombre.Text = "";
            txtEmail.Text = "";
            txtTelefono.Text = "";
        }

        private bool ValidarCliente(EscapeRoomsContext contexto, int idActual)
        {
            string dni = txtDni.Text.Trim().ToUpper();
            string nombre = txtNombre.Text.Trim();
            string email = txtEmail.Text.Trim();
            string telefono = txtTelefono.Text.Trim();

            if (dni.Length != 9)
            {
                MessageBox.Show("El DNI debe tener 9 caracteres.");
                return false;
            }
            if (nombre == "")
            {
                MessageBox.Show("El nombre es obligatorio.");
                return false;
            }
            if (email == "" || !email.Contains("@"))
            {
                MessageBox.Show("El email es obligatorio y debe contener @.");
                return false;
            }
            if (telefono != "" && !telefono.All(char.IsDigit))
            {
                MessageBox.Show("El teléfono solo puede contener números.");
                return false;
            }
            if (contexto.Clientes.Any(c => c.Dni == dni && c.IdCliente != idActual))
            {
                MessageBox.Show("Ya existe un cliente con ese DNI.");
                return false;
            }
            if (contexto.Clientes.Any(c => c.Email == email && c.IdCliente != idActual))
            {
                MessageBox.Show("Ya existe un cliente con ese email.");
                return false;
            }
            return true;
        }

        private void dgClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Cliente? seleccionado = dgClientes.SelectedItem as Cliente;
            if (seleccionado != null)
            {
                txtDni.Text = seleccionado.Dni;
                txtNombre.Text = seleccionado.Nombre;
                txtEmail.Text = seleccionado.Email;
                txtTelefono.Text = seleccionado.Telefono ?? "";
            }
        }

        private void Actualizar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CargarClientes();
                LimpiarFormulario();
                txtEstado.Text = "Lista actualizada.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Insertar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (EscapeRoomsContext contexto = escapeRoomsContext.CreateDbContext())
                {
                    if (!ValidarCliente(contexto, 0))
                        return;

                    Cliente cliente = new Cliente
                    {
                        Dni = txtDni.Text.Trim().ToUpper(),
                        Nombre = txtNombre.Text.Trim(),
                        Email = txtEmail.Text.Trim(),
                        Telefono = txtTelefono.Text.Trim() == "" ? null : txtTelefono.Text.Trim()
                    };

                    contexto.Clientes.Add(cliente);
                    contexto.SaveChanges();
                }

                CargarClientes();
                LimpiarFormulario();
                txtEstado.Text = "Cliente insertado correctamente.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Modificar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cliente? seleccionado = dgClientes.SelectedItem as Cliente;
                if (seleccionado == null)
                {
                    MessageBox.Show("Selecciona un cliente de la tabla.");
                    return;
                }

                using (EscapeRoomsContext contexto = escapeRoomsContext.CreateDbContext())
                {
                    if (!ValidarCliente(contexto, seleccionado.IdCliente))
                        return;

                    Cliente? cliente = contexto.Clientes
                        .FirstOrDefault(c => c.IdCliente == seleccionado.IdCliente);

                    if (cliente == null)
                    {
                        MessageBox.Show("El cliente ya no existe.");
                        return;
                    }

                    cliente.Dni = txtDni.Text.Trim().ToUpper();
                    cliente.Nombre = txtNombre.Text.Trim();
                    cliente.Email = txtEmail.Text.Trim();
                    cliente.Telefono = txtTelefono.Text.Trim() == "" ? null : txtTelefono.Text.Trim();

                    contexto.SaveChanges();
                }

                CargarClientes();
                LimpiarFormulario();
                txtEstado.Text = "Cliente modificado correctamente.";
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
                Cliente? seleccionado = dgClientes.SelectedItem as Cliente;
                if (seleccionado == null)
                {
                    MessageBox.Show("Selecciona un cliente de la tabla.");
                    return;
                }

                MessageBoxResult respuesta = MessageBox.Show(
                    "Se eliminará el cliente y todas sus reservas. ¿Continuar?",
                    "Eliminar cliente",
                    MessageBoxButton.YesNo);

                if (respuesta != MessageBoxResult.Yes)
                    return;

                using (EscapeRoomsContext contexto = escapeRoomsContext.CreateDbContext())
                {
                    Cliente? cliente = contexto.Clientes
                        .FirstOrDefault(c => c.IdCliente == seleccionado.IdCliente);

                    if (cliente == null)
                    {
                        MessageBox.Show("El cliente ya no existe.");
                        return;
                    }

                    contexto.Clientes.Remove(cliente);
                    contexto.SaveChanges();
                }

                CargarClientes();
                LimpiarFormulario();
                txtEstado.Text = "Cliente eliminado correctamente.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}