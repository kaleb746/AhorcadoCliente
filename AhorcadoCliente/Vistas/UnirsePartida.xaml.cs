using AhorcadoCliente.Modelo;
using AhorcadoCliente.Utilidades;
using AhorcadoCliente.ServiciosAhorcado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;

namespace AhorcadoCliente.Vistas
{
    public partial class UnirsePartida : Window
    {
        private GestorPrincipalClient _client;
        private InstanceContext _context;

        public UnirsePartida()
        {
            InitializeComponent();
            _ = CargarPartidasDisponiblesAsync();
        }

        private async Task CargarPartidasDisponiblesAsync()
        {
            var tempContext = new InstanceContext(new CallbackDummy());
            var tempClient = new GestorPrincipalClient(tempContext);
            try
            {
                int idJugadorActual = SesionActual.JugadorActual.Id;

                var partidasDto = await Task.Run(() =>
                    tempClient.ObtenerPartidasDisponibles(idJugadorActual));

                var partidas = partidasDto.Select(p => new PartidaDisponible
                {
                    IdPartida = p.IdPartida,
                    NombrePalabra = p.NombrePalabra,
                    Categoria = p.Idioma.StartsWith("en") ? TraducirCategoria(p.NombreCategoria) : p.NombreCategoria,
                    Dificultad = p.Idioma.StartsWith("en") ? TraducirDificultad(p.NombreDificultad) : p.NombreDificultad,
                    Fecha = p.FechaCreacion,
                    Idioma = p.Idioma,
                    IdJugadorAnfitrion = p.IdJugadorAnfitrion,
                    UsuarioAnfitrion = p.UsuarioAnfitrion
                }).ToList();


                PartidasDataGrid.ItemsSource = partidas;
            }
            catch (Exception ex)
            {
                string mensaje = string.Format(
                    Application.Current.TryFindResource("Msg_Descripcion_ErrorCargarPartidas")?.ToString() ??
                    "Error al obtener las partidas disponibles: {0}", ex.Message);

                MessageDialog.Show("Msg_Titulo_Error", mensaje, MessageDialog.DialogType.ERROR, this);
            }
            finally
            {
                if (tempClient.State == CommunicationState.Faulted)
                    tempClient.Abort();
                else
                    tempClient.Close();
            }
        }

        private void btnClicRegresar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void btnClicUnirse(object sender, RoutedEventArgs e)
        {
            var partidaSeleccionada = PartidasDataGrid.SelectedItem as PartidaDisponible;

            if (partidaSeleccionada == null)
            {
                MessageDialog.Show("Msg_Titulo_SinSeleccion", "Msg_Descripcion_SinSeleccion", MessageDialog.DialogType.WARNING, this);
                return;
            }

            int idJugador = SesionActual.JugadorActual.Id;
            string username = SesionActual.JugadorActual.Username;
            int idPartida = partidaSeleccionada.IdPartida;

            var handler = new CallbackInvitadoHandler(this);
            var context = new InstanceContext(handler);
            var cliente = new GestorPrincipalClient(context);

            _client = cliente;

            handler.SetDatosPartida(idPartida, idJugador, cliente);

            try
            {
                bool exito = await Task.Run(() =>
                    _client.UnirseAPartida(idPartida, idJugador, username));

                if (exito)
                {
                    MessageDialog.Show("Msg_Titulo_UnirseExitoso", "Msg_Descripcion_UnirseExitoso", MessageDialog.DialogType.INFO, this);
                }
                else
                {
                    MessageDialog.Show("Msg_Titulo_UnirseFallido", "Msg_Descripcion_UnirseFallido", MessageDialog.DialogType.ERROR, this);
                    _client.Close();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                string mensaje = string.Format(
                    Application.Current.TryFindResource("Msg_Descripcion_ErrorUnirse")?.ToString() ??
                    "Error al unirse a la partida: {0}", ex.Message);

                MessageDialog.Show("Msg_Titulo_Error", mensaje, MessageDialog.DialogType.ERROR, this);

                if (_client?.State == CommunicationState.Faulted)
                    _client.Abort();
                else
                    _client?.Close();

                this.Close();
            }
        }
        private string TraducirDificultad(string dificultad)
        {
            switch (dificultad)
            {
                case "Fácil": return "Easy";
                case "Intermedia": return "Intermediate";
                case "Difícil": return "Hard";
                default: return dificultad;
            }
        }
        private string TraducirCategoria(string categoria)
        {
            switch (categoria)
            {
                case "Animales": return "Animals";
                case "Frutas": return "Fruits";
                case "Paises": return "Countries";
                case "Colores": return "Colors";
                case "Profesiones": return "Professions";
                case "Partes del cuerpo": return "Body Parts";
                case "Deportes": return "Sports";
                case "Instrumentos": return "Instruments";
                case "Transporte": return "Transport";
                case "Ropa": return "Clothes";
                default: return categoria;
            }
        }


    }
}