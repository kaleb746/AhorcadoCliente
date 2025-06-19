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
                    MessageDialog.Show("Msg_Titulo_ErrorInicioSesion", "Msg_Error_InicioSesion", MessageDialog.DialogType.ERROR, this);
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
                if (cbIdiomas.SelectedItem is ComboBoxItem itemSeleccionado && itemSeleccionado.Tag is string idiomaSeleccionado)
                {
                    SesionActual.IdiomaActual = idiomaSeleccionado;
                }

                MessageDialog.Show("Msg_Titulo_Bienvenida", "Msg_Bienvenida", MessageDialog.DialogType.INFO, this, jugador.Nombre);

                var ventanaPrincipal = new MenuPrincipal(); 
                ventanaPrincipal.Show();
                Close();
            }
            catch (CommunicationException ex)
            {
                MessageDialog.Show("Msg_Titulo_ErrorConexion", "Msg_Error_ConexionServidor", MessageDialog.DialogType.ERROR, this, ex.Message);
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

        private void cbIdiomas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbIdiomas.SelectedItem is ComboBoxItem itemSeleccionado)
            {
                string codigoCultura = itemSeleccionado.Tag.ToString(); 

                var app = (App)Application.Current;
                app.CambiarIdioma(codigoCultura);

                string idiomaBase = codigoCultura.Split('-')[0];
                SesionActual.IdiomaActual = idiomaBase;
            }
        }

    }
}
