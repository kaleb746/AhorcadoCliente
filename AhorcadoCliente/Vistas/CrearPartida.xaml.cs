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
            App.IdiomaCambiado += RefrescarCombos;
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
                MessageDialog.Show("Msg_Titulo_Error", "Msg_Descripcion_ErrorCategoriasDificultades", MessageDialog.DialogType.ERROR, this);
            }
            finally
            {
                if (client.State == CommunicationState.Faulted)
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
                    MessageDialog.Show("Msg_Titulo_Error", "Msg_Descripcion_ErrorPalabras", MessageDialog.DialogType.ERROR, this);
                }
                finally
                {
                    if (client.State == CommunicationState.Faulted)
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
                    MessageDialog.Show("Msg_Titulo_ErrorSesion", "Msg_Descripcion_ErrorSesion", MessageDialog.DialogType.ERROR, this);
                    return;
                }

                int idJugador = SesionActual.JugadorActual.Id;
                var callbackHandler = new CallbackHandler(null);
                var context = new InstanceContext(callbackHandler);
                var client = new GestorPrincipalClient(context);

                try
                {
                    string idioma = string.IsNullOrEmpty(SesionActual.IdiomaActual) ? "es" : SesionActual.IdiomaActual;
                    int idPartida = await Task.Run(() => client.CrearPartida(idJugador, idPalabra, idioma));

                    if (idPartida > 0)
                    {
                        callbackHandler.SetDatosPartida(idPartida, idJugador, client);

                        SesionActual.ClienteWCF = client;
                        SesionActual.CallbackAnfitrion = callbackHandler;

                        MessageDialog.Show("Msg_Titulo_CreacionPartida", "Msg_Descripcion_CreacionPartida", MessageDialog.DialogType.INFO, this);
                        this.Close();
                    }
                    else if (idPartida == -1)
                    {
                        MessageDialog.Show("Msg_Titulo_EstadoNoExiste", "Msg_Descripcion_EstadoNoExiste", MessageDialog.DialogType.WARNING, this);
                    }
                    else
                    {
                        MessageDialog.Show("Msg_Titulo_Error", "Msg_Descripcion_ErrorCrearPartida", MessageDialog.DialogType.ERROR, this);
                    }
                }
                catch (Exception ex)
                {
                    string mensaje = string.Format(
                        Application.Current.TryFindResource("Msg_Descripcion_ErrorInesperado")?.ToString() ?? "Error inesperado: {0}", ex.Message);
                    MessageDialog.Show("Msg_Titulo_Error", mensaje, MessageDialog.DialogType.ERROR, this);
                }
            }
            else
            {
                MessageDialog.Show("Msg_Titulo_CampoObligatorio", "Msg_Descripcion_SeleccionarPalabra", MessageDialog.DialogType.WARNING, this);
            }
        }
        private void ActualizarEstadoBotonCrearPartida()
        {
            bool dificultadSeleccionada = cbDificultades.SelectedItem != null;
            bool categoriaSeleccionada = cbCategorias.SelectedItem != null;
            bool palabraSeleccionada = cbPalabras.SelectedItem != null;

            BtnCrearSala.IsEnabled = dificultadSeleccionada && categoriaSeleccionada && palabraSeleccionada;
        }
        private void RefrescarCombos()
        {
            cbCategorias.Items.Refresh();
            cbDificultades.Items.Refresh();
            cbPalabras.Items.Refresh();
        }

    }
}
