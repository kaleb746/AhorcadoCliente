using AhorcadoCliente.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private void btnClicConfiguracion(object sender, RoutedEventArgs e)
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
    }
}