using AhorcadoCliente.ServiciosAhorcado;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using AhorcadoCliente.Utilidades;
using System.ServiceModel;

namespace AhorcadoCliente.Vistas
{
    public partial class IniciarSesion : Window
    {
        private MainWindow mainWindow;
        public IniciarSesion()
        {
            InitializeComponent();
            SuscribirEventosDeValidacion();
            ActualizarEstadoBotonFormulario(clicIniciarSesion);
        }
        private async void btnClicIniciarSesion(object sender, RoutedEventArgs e)
        {

            string usuario = TbUsuario.Text.Trim().ToLower();
            string contrasena = PbPassword.Password;

            var context = new InstanceContext(new CallbackDummy());
            var client = new GestorPrincipalClient(context);
            JugadorDTO jugador = null;

            try
            {
                jugador = await Task.Run(() => client.IniciarSesion(usuario, contrasena));

                if (jugador == null)
                {
                    MessageBox.Show("Usuario o contraseña incorrectos", "Error de inicio de sesión", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                SesionActual.JugadorActual = new Modelo.Jugador
                {
                    Id = jugador.Id,
                    Nombre = jugador.Nombre,
                    ApellidoPaterno = jugador.ApellidoPaterno,
                    ApellidoMaterno = jugador.ApellidoMaterno,
                    FechaDeNacimiento = jugador.FechaDeNacimiento,
                    Telefono = jugador.Telefono,
                    Username = jugador.Username,
                    Correo = jugador.Correo,
                    Contrasena = jugador.Contrasena,
                    Puntaje = jugador.Puntaje
                };
                SesionActual.ClienteWCF = new GestorPrincipalClient(context);

                MessageBox.Show($"¡Bienvenido, {jugador.Nombre}!", "Inicio exitoso", MessageBoxButton.OK, MessageBoxImage.Information);

                var ventanaPrincipal = new MenuPrincipal(); 
                ventanaPrincipal.Show();
                Close();
            }
            catch (CommunicationException ex)
            {
                MessageBox.Show($"Error de comunicación con el servidor: {ex.Message}", "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (client.State == CommunicationState.Faulted)
                    client.Abort();
                else
                    client.Close();
            }
        }
        private void ActualizarEstadoBotonFormulario(Button boton)
        {
            var camposObligatorios = new List<Control> { TbUsuario, PbPassword };

            bool todosCompletos = true;

            foreach (var campo in camposObligatorios)
            {
                if (campo is TextBox tb && string.IsNullOrWhiteSpace(tb.Text))
                {
                    todosCompletos = false;
                    break;
                }
                else if (campo is PasswordBox pb && string.IsNullOrWhiteSpace(pb.Password))
                {
                    todosCompletos = false;
                    break;
                }
            }

            boton.IsEnabled = todosCompletos;
        }
        private void SuscribirEventosDeValidacion()
        {
            TbUsuario.TextChanged += (_, __) => ActualizarEstadoBotonFormulario(clicIniciarSesion);
            PbPassword.PasswordChanged += (_, __) => ActualizarEstadoBotonFormulario(clicIniciarSesion);
        }
        private void btnClicRegistrarJugador(object sender, RoutedEventArgs e)
        {
            var ventana = new CrearCuenta();
            ventana.Owner = this;
            ventana.ShowDialog();
        }
    }
}
