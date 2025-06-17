using AhorcadoCliente.ServiciosAhorcado;
using AhorcadoCliente.Utilidades;
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

namespace AhorcadoCliente.Vistas
{
    /// <summary>
    /// Lógica de interacción para MenuPrincipal.xaml
    /// </summary>
    public partial class MenuPrincipal : Window
    {
        public MenuPrincipal()
        {
            SesionActual.VentanaMenuPrincipal = this;
            InitializeComponent();
        }

        private void btnClicEditar(object sender, RoutedEventArgs e)
        {
            if (SesionActual.JugadorActual != null)
            {
                var ventana = new CrearCuenta(SesionActual.JugadorActual);
                ventana.Owner = this;
                ventana.ShowDialog();
            }
            else
            {
                MessageDialog.Show("Msg_Titulo_ErrorSesion", "Msg_Error_SesionNoEncontrada", MessageDialog.DialogType.ERROR, this);
            }
        }
        private void btnClicMostrarRanking(object sender, RoutedEventArgs e)
        {
            var ventana = new RankingJugador();
            ventana.Owner = this;      
            ventana.ShowDialog();    
        }

        private void btnClicCrearPartida(object sender, RoutedEventArgs e)
        {
            var ventana = new CrearPartida();
            ventana.Owner = this;
            ventana.ShowDialog();
        }

        private void btnClicBuscarPartida(object sender, RoutedEventArgs e)
        {
            var ventana = new UnirsePartida();
            ventana.Owner = this;
            ventana.ShowDialog();
        }

        private void btnClicHistorialPartidas(object sender, RoutedEventArgs e)
        {
            var ventana = new HistorialPartida();
            ventana.Owner = this;
            ventana.ShowDialog();
        }
        private async void btnClicEliminar(object sender, RoutedEventArgs e)
        {
            if (SesionActual.JugadorActual == null)
            {
                MessageDialog.Show("Msg_Titulo_ErrorSesion", "Msg_Error_SesionNoEncontrada", MessageDialog.DialogType.ERROR, this);
                return;
            }

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
                int resultado = await Task.Run(() => client.EliminarJugador(SesionActual.JugadorActual.Id));

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
                string mensaje = string.Format(
                    Application.Current.TryFindResource("Msg_Descripcion_ErrorInesperado")?.ToString() ?? "Error inesperado: {0}", ex.Message);
                MessageDialog.Show("Msg_Titulo_Error", mensaje, MessageDialog.DialogType.ERROR, this);
            }
        }
        private void btnClicCerrarSesion(object sender, RoutedEventArgs e)
        {
            var confirmar = MessageBox.Show(
                Application.Current.TryFindResource("Msg_Descripcion_ConfirmarCerrarSesion")?.ToString() ?? "¿Deseas cerrar sesión?",
                Application.Current.TryFindResource("Msg_Titulo_CerrarSesion")?.ToString() ?? "Cerrar sesión",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmar != MessageBoxResult.Yes)
                return;

            SesionActual.JugadorActual = null;

            var ventana = new IniciarSesion();
            ventana.Show();

            foreach (Window win in Application.Current.Windows)
            {
                if (win != ventana)
                    win.Close();
            }
        }

    }
}