using AhorcadoCliente.ServiciosAhorcado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AhorcadoCliente.Modelo;
using AhorcadoCliente.Utilidades;
using System.ServiceModel;

namespace AhorcadoCliente.Vistas
{
    public partial class RankingJugador : Window
    {
        public RankingJugador()
        {
            InitializeComponent();
            CargarRanking();
        }

        private async void CargarRanking()
        {
            var context = new InstanceContext(new CallbackDummy());
            var client = new GestorPrincipalClient(context);

            try
            {
                var jugadores = await Task.Run(() => client.ObtenerTodosLosJugadores());

                var ranking = jugadores
                    .OrderByDescending(j => j.Puntaje)
                    .Select((j, index) => new RankingJugadorItem
                    {
                        Puesto = index + 1,
                        Usuario = j.Username,
                        Puntaje = j.Puntaje
                    })
                    .ToList();

                RankingDataGrid.ItemsSource = ranking;
            }
            catch (Exception ex)
            {
                string descripcion = string.Format(
                    Application.Current.TryFindResource("Msg_Descripcion_ErrorCargarRanking")?.ToString()
                    ?? "Error al cargar el ranking: {0}", ex.Message);

                MessageDialog.Show("Msg_Titulo_Error", descripcion, MessageDialog.DialogType.ERROR, this);
            }
            finally
            {
                if (client.State == System.ServiceModel.CommunicationState.Faulted)
                    client.Abort();
                else
                    client.Close();
            }
        }

        private void btnClicRegresar(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
