using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AhorcadoCliente.Modelo;
using AhorcadoCliente.ServiciosAhorcado;
using AhorcadoCliente.Utilidades;

namespace AhorcadoCliente.Vistas
{
    /// <summary>
    /// Lógica de interacción para CrearCuenta.xaml
    /// </summary>
    public partial class CrearCuenta : Window
    {
        private Jugador jugadorExistente;
        private bool esEdicion = false;

        public CrearCuenta()
        {
            InitializeComponent();
            ValidarEntrada();
            SuscribirEventosDeValidacion();
            ActualizarEstadoBotonFormulario(BtnRegistrar);
        }
        public CrearCuenta(Jugador jugador) : this()
        {
            jugadorExistente = jugador;
            esEdicion = true;
            CargarDatosJugador(jugador);
            TxtTituloPrincipal.Text = Application.Current.TryFindResource("CrearCuenta_Titulo_EditarPerfil")?.ToString() ?? "Editar Perfil";
            TxtSubtitulo.Text = Application.Current.TryFindResource("CrearCuenta_Descripcion_EditarPerfil")?.ToString() ?? "Actualiza tu información personal";
            BtnRegistrar.Content = Application.Current.TryFindResource("CrearCuenta_Btn_GuardarCambios")?.ToString() ?? "Guardar cambios";
            BtnEliminar.Visibility = Visibility.Visible;
        }
        private void CargarDatosJugador(Jugador jugador)
        {
            TbNombre.Text = jugador.Nombre;
            TbApellidoPaterno.Text = jugador.ApellidoPaterno;
            TbApellidoMaterno.Text = jugador.ApellidoMaterno;
            TbTelefono.Text = jugador.Telefono;
            TbUsuario.Text = jugador.Username;
            TbCorreo.Text = jugador.Correo;
            PfContraseña.Password = jugador.Contrasena;
            DpFechaNacimiento.SelectedDate = jugador.FechaDeNacimiento;
            ActualizarEstadoBotonFormulario(BtnRegistrar);
        }
        private void btnClicRegresar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void ValidarEntrada()
        {
            var validaciones = new (TextBox, string, int)[]
            {
                (TbNombre, Constantes.PATRON_TEXTO_GENERAL, Constantes.LONGITUG_MAXIMA),
                (TbApellidoPaterno, Constantes.PATRON_TEXTO_GENERAL, Constantes.LONGITUG_MAXIMA),
                (TbApellidoMaterno, Constantes.PATRON_TEXTO_GENERAL, Constantes.LONGITUG_MAXIMA),
                (TbTelefono, @"[^\d]", Constantes.LONGITUG_TELEFONO),
                (TbUsuario, Constantes.PATRON_TEXTO_GENERAL, Constantes.LONGITUG_MAXIMA)
            };

            foreach (var (textBox, patron, longitudMaxima) in validaciones)
                ValidacionesEntrada.ValidarEntrada(textBox, patron, longitudMaxima);

            ValidacionesEntrada.ValidarEntradaContrasena(PfContraseña, Constantes.PATRON_TEXTO_GENERAL, Constantes.LONGITUG_MAXIMA);
        }
        private void SuscribirEventosDeValidacion()
        {
            TbNombre.TextChanged += (_, __) => ActualizarEstadoBotonFormulario(BtnRegistrar);
            TbApellidoPaterno.TextChanged += (_, __) => ActualizarEstadoBotonFormulario(BtnRegistrar);
            TbApellidoMaterno.TextChanged += (_, __) => ActualizarEstadoBotonFormulario(BtnRegistrar);
            TbTelefono.TextChanged += (_, __) => ActualizarEstadoBotonFormulario(BtnRegistrar);
            TbUsuario.TextChanged += (_, __) => ActualizarEstadoBotonFormulario(BtnRegistrar);
            TbCorreo.TextChanged += (_, __) => ActualizarEstadoBotonFormulario(BtnRegistrar);
            PfContraseña.PasswordChanged += (_, __) => ActualizarEstadoBotonFormulario(BtnRegistrar);
            DpFechaNacimiento.SelectedDateChanged += (_, __) => ActualizarEstadoBotonFormulario(BtnRegistrar);
        }

        private void ActualizarEstadoBotonFormulario(Button boton)
        {
            var camposObligatorios = new List<Control>
            {
                TbNombre, TbApellidoPaterno, TbApellidoMaterno,
                TbTelefono, TbUsuario, TbCorreo, PfContraseña, DpFechaNacimiento
            };
            bool todosCompletos = true;
            foreach (var campo in camposObligatorios)
            {
                switch (campo)
                {
                    case TextBox tb when string.IsNullOrWhiteSpace(tb.Text):
                    case PasswordBox pb when string.IsNullOrWhiteSpace(pb.Password):
                    case DatePicker dp when dp.SelectedDate == null:
                        todosCompletos = false;
                        break;
                }
                if (!todosCompletos) break;
            }
            boton.IsEnabled = todosCompletos;
        }
        private async void btnClicRegistrarJugador(object sender, RoutedEventArgs e)
        {
            string correo = TbCorreo.Text.Trim();

            if (!ValidacionesEntrada.EsFormatoCorreoValido(correo))
            {
                MessageDialog.Show("Msg_Titulo_CorreoInvalido", "Msg_Descripcion_CorreoInvalido", MessageDialog.DialogType.WARNING, this);
                return;
            }
            try
            {
                var jugador = new Jugador
                {
                    Id = jugadorExistente?.Id ?? 0,
                    Nombre = TbNombre.Text.Trim(),
                    ApellidoPaterno = TbApellidoPaterno.Text.Trim(),
                    ApellidoMaterno = TbApellidoMaterno.Text.Trim(),
                    FechaDeNacimiento = DpFechaNacimiento.SelectedDate ?? DateTime.MinValue,
                    Telefono = TbTelefono.Text.Trim(),
                    Username = TbUsuario.Text.Trim(),
                    Correo = TbCorreo.Text.Trim(),
                    Contrasena = PfContraseña.Password,
                    Puntaje = jugadorExistente?.Puntaje ?? 0
                };

                var dto = new JugadorDTO
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

                var context = new InstanceContext(new CallbackDummy());
                var client = new GestorPrincipalClient(context);
                int resultado;

                try
                {
                    resultado = esEdicion
                        ? await Task.Run(() => client.ActualizarJugador(dto))
                        : await Task.Run(() => client.RegistrarJugador(dto));
                }
                finally
                {
                    if (client.State == System.ServiceModel.CommunicationState.Faulted)
                        client.Abort();
                    else
                        client.Close();
                }

                if (resultado == 1)
                {
                    SesionActual.JugadorActual = jugador;
                    MessageDialog.Show("Msg_Titulo_RegistroExitoso",esEdicion ? "Msg_Descripcion_PerfilActualizado" : "Msg_Descripcion_RegistroExitoso",MessageDialog.DialogType.INFO,this);
                    this.Close();
                }
                else if (resultado == -1)
                {
                    MessageDialog.Show("Msg_Titulo_Duplicado", "Msg_Descripcion_Duplicado", MessageDialog.DialogType.WARNING, this);
                }
                else
                {
                    MessageDialog.Show("Msg_Titulo_ErrorRegistro", "Msg_Descripcion_ErrorRegistro", MessageDialog.DialogType.ERROR, this);
                }
            }
            catch (Exception ex)
            {
                string mensaje = string.Format(
                Application.Current.TryFindResource("Msg_Descripcion_ErrorInesperado")?.ToString() ?? "Error inesperado: {0}", ex.Message);
                MessageDialog.Show("Msg_Titulo_Error", mensaje, MessageDialog.DialogType.ERROR, this);
            }
        }
        private async void btnClicEliminar(object sender, RoutedEventArgs e)
        {
            var confirmar = MessageBox.Show(
                Application.Current.TryFindResource("Msg_Descripcion_ConfirmarEliminacion")?.ToString() ?? "¿Seguro que deseas eliminar tu perfil? Esta acción es permanente.",
                Application.Current.TryFindResource("Msg_Titulo_ConfirmarEliminacion")?.ToString() ?? "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirmar != MessageBoxResult.Yes) return;

            var context = new InstanceContext(new CallbackDummy());
            var client = new GestorPrincipalClient(context);
            try
            {
                int resultado = await Task.Run(() => client.EliminarJugador(jugadorExistente.Id));

                if (resultado == 1)
                {
                    MessageDialog.Show("Msg_Titulo_PerfilEliminado", "Msg_Descripcion_PerfilEliminado", MessageDialog.DialogType.INFO, this);
                    SesionActual.JugadorActual = null;

                    var ventana = new IniciarSesion();
                    ventana.Show();

                    foreach (Window win in Application.Current.Windows)
                    {
                        if (win != ventana)
                            win.Close();
                    }

                }
                else if (resultado == 0)
                {
                    MessageDialog.Show("Msg_Titulo_NoEncontrado", "Msg_Descripcion_NoEncontrado", MessageDialog.DialogType.WARNING, this);
                }
                else
                {
                    MessageDialog.Show("Msg_Titulo_ErrorEliminar", "Msg_Descripcion_ErrorEliminar", MessageDialog.DialogType.ERROR, this);
                }
            }
            catch (Exception ex)
            {
                string mensaje = string.Format(Application.Current.TryFindResource("Msg_Descripcion_ErrorInesperado")?.ToString() ?? "Error inesperado: {0}", ex.Message);
                MessageDialog.Show("Msg_Titulo_Error", mensaje, MessageDialog.DialogType.ERROR, this);
            }
        }
    }
}
