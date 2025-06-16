using AhorcadoCliente.Modelo;
using AhorcadoCliente.ServiciosAhorcado;
using AhorcadoCliente.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;

namespace AhorcadoCliente.Vistas
{
    public partial class CrearPartida : Window
    {
        private List<Categoria> categorias;
        private List<Dificultad> dificultades;
        private GestorPrincipalClient _client;

        public CrearPartida()
        {
            InitializeComponent();
            var context = new InstanceContext(new CallbackDummy());
            _client = new GestorPrincipalClient(context);
            _ = CargarCategoriasYDificultadesAsync();
        }

        private async Task CargarCategoriasYDificultadesAsync()
        {
            var context = new InstanceContext(new CallbackDummy());
            var client = new GestorPrincipalClient(context);


            try
            {
                var categoriasDto = await Task.Run(() => client.ObtenerCategorias());
                var dificultadesDto = await Task.Run(() => client.ObtenerDificultades());

                categorias = categoriasDto.Select(c => new Categoria
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    NombreIngles = c.NombreIngles
                }).ToList();

                dificultades = dificultadesDto.Select(d => new Dificultad
                {
                    Id = d.Id,
                    Nombre = d.Nombre,
                    NombreIngles = d.NombreIngles
                }).ToList();

                cbCategorias.SelectedValuePath = "Id";
                cbCategorias.ItemsSource = categorias;

                cbDificultades.SelectedValuePath = "Id";
                cbDificultades.ItemsSource = dificultades;
            }
            catch
            {
                MessageBox.Show("Error al cargar categorías o dificultades.");
            }
            finally
            {
                if (client.State == System.ServiceModel.CommunicationState.Faulted)
                    client.Abort();
                else
                    client.Close();
            }
        }
        private async void cbCategorias_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            await CargarPalabrasSiListoAsync();
            ActualizarEstadoBotonCrearPartida();
        }

        private async void cbDificultades_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            await CargarPalabrasSiListoAsync();
            ActualizarEstadoBotonCrearPartida();
        }
        private void cbPalabras_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ActualizarEstadoBotonCrearPartida();
        }
        private async Task CargarPalabrasSiListoAsync()
        {
            if (cbCategorias.SelectedValue is int idCategoria &&
                cbDificultades.SelectedValue is int idDificultad)
            {
                var context = new InstanceContext(new CallbackDummy());
                var client = new GestorPrincipalClient(context);

                try
                {
                    var palabrasDto = await Task.Run(() =>
                        client.ObtenerPalabrasPorCategoriaYDificultad(idCategoria, idDificultad));

                    var palabras = palabrasDto.Select(p => new Palabra
                    {
                        Id = p.Id,
                        Nombre = p.Nombre,
                        NombreIngles = p.NombreIngles,
                        Descripcion = p.Descripcion,
                        DescripcionIngles = p.DescripcionIngles,
                        IdCategoria = p.IdCategoria,
                        IdDificultad = p.IdDificultad
                    }).ToList();

                    cbPalabras.SelectedValuePath = "Id";
                    cbPalabras.ItemsSource = palabras;
                }
                catch
                {
                    MessageBox.Show("Error al cargar las palabras.");
                }
                finally
                {
                    if (client.State == System.ServiceModel.CommunicationState.Faulted)
                        client.Abort();
                    else
                        client.Close();
                }
            }
        }

        private void btnClicRegresar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private async void btnClicCrearSala(object sender, RoutedEventArgs e)
        {
            if (cbPalabras.SelectedValue is int idPalabra)
            {
                if (SesionActual.JugadorActual == null)
                {
                    MessageBox.Show("No hay un jugador activo en sesión.", "Error de sesión", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int idJugador = SesionActual.JugadorActual.Id;

                var callbackHandler = new CallbackHandler(null); 
                var context = new InstanceContext(callbackHandler);
                var client = new GestorPrincipalClient(context);

                try
                {
                    int idPartida = await Task.Run(() => client.CrearPartida(idJugador, idPalabra));

                    if (idPartida > 0)
                    {
                        callbackHandler.SetDatosPartida(idPartida, idJugador, client);

                        SesionActual.ClienteWCF = client;
                        SesionActual.CallbackAnfitrion = callbackHandler;

                        MessageBox.Show("Partida creada exitosamente. Esperando a que otro jugador se una...",
                                        "Partida creada", MessageBoxButton.OK, MessageBoxImage.Information);

                        this.Close();
                    }
                    else if (idPartida == -1)
                    {
                        MessageBox.Show("El estado 'En espera' no existe en el sistema.", "Error de datos", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        MessageBox.Show("Ocurrió un error al crear la partida.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error inesperado: {ex.Message}", "Excepción", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (client.State == CommunicationState.Faulted)
                        client.Abort();
                    else
                        client.Close();
                }
            }
            else
            {
                MessageBox.Show("Seleccione una palabra para crear la partida.", "Campo obligatorio", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void ActualizarEstadoBotonCrearPartida()
        {
            bool dificultadSeleccionada = cbDificultades.SelectedItem != null;
            bool categoriaSeleccionada = cbCategorias.SelectedItem != null;
            bool palabraSeleccionada = cbPalabras.SelectedItem != null;

            BtnCrearSala.IsEnabled = dificultadSeleccionada && categoriaSeleccionada && palabraSeleccionada;
        }

    }
}
