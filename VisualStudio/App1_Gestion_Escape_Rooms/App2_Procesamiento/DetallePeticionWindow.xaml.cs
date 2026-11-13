using System.Windows;

namespace App2_Procesamiento
{
    public partial class DetallePeticionWindow : Window
    {
        public DetallePeticionWindow(PeticionRegistro peticion)
        {
            InitializeComponent();

            txtId.Text = peticion.IdPeticion;
            txtFecha.Text = peticion.FechaHoraRecepcion.ToString();
            txtTipo.Text = peticion.TipoOperacion;
            txtEstado.Text = peticion.EstadoFinal;
            txtCampo.Text = peticion.CampoError;
            txtDetalle.Text = peticion.Detalle;
            txtTecnico.Text = peticion.ErrorTecnico;
            txtPuntuacion.Text = peticion.Puntuacion.ToString();
            txtClasificacion.Text = peticion.Clasificacion;
            txtJson.Text = peticion.JsonRecibido;
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}