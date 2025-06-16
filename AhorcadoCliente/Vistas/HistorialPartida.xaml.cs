using AhorcadoCliente.Modelo;
using AhorcadoCliente.ServiciosAhorcado;
using AhorcadoCliente.Utilidades;
using System;
using System.Linq;
using System.Windows;

namespace AhorcadoCliente.Vistas
{
    /// <summary>
    /// Lógica de interacción para HistorialPartida.xaml
    /// </summary>
    public partial class HistorialPartida : Window
    {
        public HistorialPartida()
        {
            InitializeComponent();
            Loaded += HistorialPartida_Loaded;
        }

        private void HistorialPartida_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SesionActual.JugadorActual == null || SesionActual.ClienteWCF == null)
                {
                    MessageBox.Show("No se encontró la sesión del jugador.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                    return;
                }

                int idJugador = SesionActual.JugadorActual.Id;
                var cliente = SesionActual.ClienteWCF;

                var historialDTO = cliente.ObtenerHistorialDeJugador(idJugador);

                var historialLocal = historialDTO.Select(h => new Modelo.HistorialPartida
                {
                    Fecha = h.Fecha,
                    Usuario = h.Usuario,
                    Dificultad = h.Dificultad,
                    Resultado = h.Resultado == "Ganó" ? "Ganaste" : "Perdiste"
                }).ToList();

                PartidasDataGrid.ItemsSource = historialLocal;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el historial: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnClicRegresar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
