using AhorcadoCliente.Modelo;
using AhorcadoCliente.ServiciosAhorcado;
using AhorcadoCliente.Utilidades;
using System;
using System.Linq;
using System.Windows;

namespace AhorcadoCliente.Vistas
{
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
                    MessageDialog.Show("Msg_Titulo_ErrorSesion", "Msg_Descripcion_ErrorSesion", MessageDialog.DialogType.ERROR, this);
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
                    Dificultad = TraducirDificultad(h.Dificultad),
                    Resultado = h.Resultado == "Ganó"
                        ? Application.Current.TryFindResource("Historial_Resultado_Ganaste")?.ToString() ?? "Ganaste"
                        : Application.Current.TryFindResource("Historial_Resultado_Perdiste")?.ToString() ?? "Perdiste"
                }).ToList();

                PartidasDataGrid.ItemsSource = historialLocal;
            }
            catch (Exception ex)
            {
                string mensaje = string.Format(
                    Application.Current.TryFindResource("Msg_Descripcion_ErrorHistorial")?.ToString()
                    ?? "Error al cargar el historial: {0}", ex.Message);

                MessageDialog.Show("Msg_Titulo_Error", mensaje, MessageDialog.DialogType.ERROR, this);
            }
        }

        private void btnClicRegresar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private string TraducirDificultad(string dificultadOriginal)
        {
            switch (dificultadOriginal.ToLower())
            {
                case "fácil":
                case "facil":
                    return Application.Current.TryFindResource("Dificultad_Facil")?.ToString() ?? dificultadOriginal;
                case "intermedia":
                case "media":
                    return Application.Current.TryFindResource("Dificultad_Media")?.ToString() ?? dificultadOriginal;
                case "difícil":
                case "dificil":
                    return Application.Current.TryFindResource("Dificultad_Dificil")?.ToString() ?? dificultadOriginal;
                default:
                    return dificultadOriginal;
            }
        }

    }
}
