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

        private bool ValidarCliente(EscapeRoomsContext contexto, int idActual)
        {
            string dni = txtDni.Text.Trim().ToUpper();
            string nombre = txtNombre.Text.Trim();
            string email = txtEmail.Text.Trim().ToLower();
            string telefono = txtTelefono.Text.Trim();

            if (dni.Length != 9)
            {
                MessageBox.Show("El DNI debe tener 9 caracteres.");
                return false;
            }
            if (nombre == "" || nombre.Length > 100)
            {
                MessageBox.Show("El nombre es obligatorio y tiene un máximo de 100 caracteres.");
                return false;
            }
            if (email == "" || !email.Contains("@") || email.Length > 100)
            {
                MessageBox.Show("El email es obligatorio, debe contener @ y tiene un máximo de 100 caracteres.");
                return false;
            }
            if (telefono.Length > 20)
            {
                MessageBox.Show("El teléfono tiene un máximo de 20 caracteres.");
                return false;
            }
            foreach (char caracter in telefono)
            {
                if (!char.IsDigit(caracter))
                {
                    MessageBox.Show("El teléfono solo puede contener números.");
                    return false;
                }
            }

            foreach (Cliente otro in contexto.Clientes.ToList())
            {
                if (otro.IdCliente != idActual && otro.Dni.ToUpper() == dni)
                {
                    MessageBox.Show("Ya existe un cliente con ese DNI.");
                    return false;
                }
                if (otro.IdCliente != idActual && otro.Email.ToLower() == email)
                {
                    MessageBox.Show("Ya existe un cliente con ese email.");
                    return false;
                }
            }
            return true;
        }

        private void Actualizar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CargarClientes();
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
                    {
                        return;
                    }

                    Cliente cliente = new Cliente();
                    cliente.Dni = txtDni.Text.Trim().ToUpper();
                    cliente.Nombre = txtNombre.Text.Trim();
                    cliente.Email = txtEmail.Text.Trim().ToLower();
                    if (txtTelefono.Text.Trim() != "")
                    {
                        cliente.Telefono = txtTelefono.Text.Trim();
                    }

                    contexto.Clientes.Add(cliente);
                    contexto.SaveChanges();
                }

                CargarClientes();
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
                if (dgClientes.SelectedItem == null)
                {
                    MessageBox.Show("Selecciona un cliente de la tabla.");
                    return;
                }
                Cliente seleccionado = (Cliente)dgClientes.SelectedItem;

                using (EscapeRoomsContext contexto = escapeRoomsContext.CreateDbContext())
                {
                    if (!ValidarCliente(contexto, seleccionado.IdCliente))
                    {
                        return;
                    }

                    foreach (Cliente cliente in contexto.Clientes.ToList())
                    {
                        if (cliente.IdCliente == seleccionado.IdCliente)
                        {
                            cliente.Dni = txtDni.Text.Trim().ToUpper();
                            cliente.Nombre = txtNombre.Text.Trim();
                            cliente.Email = txtEmail.Text.Trim().ToLower();
                            if (txtTelefono.Text.Trim() != "")
                            {
                                cliente.Telefono = txtTelefono.Text.Trim();
                            }
                            else
                            {
                                cliente.Telefono = null;
                            }
                        }
                    }

                    contexto.SaveChanges();
                }

                CargarClientes();
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
                if (dgClientes.SelectedItem == null)
                {
                    MessageBox.Show("Selecciona un cliente de la tabla.");
                    return;
                }
                Cliente seleccionado = (Cliente)dgClientes.SelectedItem;

                using (EscapeRoomsContext contexto = escapeRoomsContext.CreateDbContext())
                {
                    foreach (Cliente cliente in contexto.Clientes.ToList())
                    {
                        if (cliente.IdCliente == seleccionado.IdCliente)
                        {
                            contexto.Clientes.Remove(cliente);
                        }
                    }

                    contexto.SaveChanges();
                }

                CargarClientes();
                txtEstado.Text = "Cliente eliminado correctamente.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}