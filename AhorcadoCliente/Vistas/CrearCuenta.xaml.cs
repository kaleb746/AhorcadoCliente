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
            TxtTituloPrincipal.Text = "Editar Perfil";
            TxtSubtitulo.Text = "Actualiza tu información personal";
            BtnRegistrar.Content = "Guardar cambios";
            CbIdioma.Visibility = Visibility.Visible;
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
                MessageBox.Show("El formato del correo electrónico no es válido. Por favor, revisa que sea similar a ejemplo@dominio.com.",
                                "Correo inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    MessageBox.Show(esEdicion ? "Perfil actualizado correctamente." : "¡Registro exitoso!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else if (resultado == -1)
                {
                    MessageBox.Show("Ya existe un usuario con ese correo o nombre de usuario.", "Duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show("Ocurrió un error al procesar la solicitud.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void btnClicEliminar(object sender, RoutedEventArgs e)
        {
            var confirmar = MessageBox.Show(
                "¿Seguro que deseas eliminar tu perfil? Esta acción es permanente.",
                "Confirmar eliminación",
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
                    MessageBox.Show("Perfil eliminado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    SesionActual.JugadorActual = null;

                    var ventana = new IniciarSesion();
                    ventana.Show();

                    // Cierra todas las ventanas excepto la de inicio de sesión
                    foreach (Window win in Application.Current.Windows)
                    {
                        if (win != ventana)
                            win.Close();
                    }

                }
                else if (resultado == 0)
                {
                    MessageBox.Show("No se encontró el jugador en la base de datos.", "No encontrado", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show("Ocurrió un error al intentar eliminar el perfil.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
