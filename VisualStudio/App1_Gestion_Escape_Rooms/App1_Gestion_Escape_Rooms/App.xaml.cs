using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using App1_Gestion_Escape_Rooms.Models;
using System.Windows;

namespace App1_Gestion_Escape_Rooms
{
    public partial class App : Application
    {
        public static IServiceProvider proveedorServicios { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs args)
        {
            try
            {
                base.OnStartup(args);
                ServiceCollection service = new ServiceCollection();
                service.AddPooledDbContextFactory<EscapeRoomsContext>(ConfigurarContexto);
                proveedorServicios = service.BuildServiceProvider();
                MainWindow ventana = new MainWindow();
                ventana.Show();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }

        private void ConfigurarContexto(DbContextOptionsBuilder optionsBuilder)
        {
            // La contraseña no tiene nada porque yo utilizo xampp, si no habria que poner la contraseña de la base de datos
            String cadenaConexion = "server=localhost;database=escape_rooms;user=root;password=";       
            optionsBuilder.UseMySql(cadenaConexion, ServerVersion.AutoDetect(cadenaConexion));
        }
    }
}