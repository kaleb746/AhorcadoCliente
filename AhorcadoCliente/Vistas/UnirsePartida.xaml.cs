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
                    Categoria = p.NombreCategoria,
                    Dificultad = p.NombreDificultad,
                    Fecha = p.FechaCreacion,
                    Idioma = p.Idioma,
                    IdJugadorAnfitrion = p.IdJugadorAnfitrion,
                    UsuarioAnfitrion = p.UsuarioAnfitrion
                }).ToList();

                PartidasDataGrid.ItemsSource = partidas;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener las partidas disponibles: {ex.Message}");
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
                MessageBox.Show("Selecciona una partida para unirte.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    MessageBox.Show("Te uniste exitosamente a la partida.", "Unión exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("No fue posible unirse a la partida. Intenta con otra.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    _client.Close();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al unirse a la partida: {ex.Message}");
                if (_client?.State == CommunicationState.Faulted)
                    _client.Abort();
                else
                    _client?.Close();

                this.Close();
            }
        }

    }
}
