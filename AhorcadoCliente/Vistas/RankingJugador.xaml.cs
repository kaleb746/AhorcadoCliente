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
                MessageBox.Show($"Error al cargar el ranking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
