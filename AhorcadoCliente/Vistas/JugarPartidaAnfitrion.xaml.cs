using AhorcadoCliente.Utilidades;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AhorcadoCliente.Vistas
{
    public partial class JugarPartidaAnfitrion : Window
    {
        private int _errores = 0;

        public JugarPartidaAnfitrion()
        {
            InitializeComponent();
            ActualizarDibujoAhorcado();
        }

        public void MostrarPalabra(string palabraParcial)
        {
            WPPalabraContainer.Children.Clear();

            foreach (char c in palabraParcial)
            {
                var letra = new ContentControl
                {
                    Style = (Style)FindResource("PalabraLetraStyle"),
                    Content = c == '_' ? "_" : c.ToString()
                };

                WPPalabraContainer.Children.Add(letra);
            }
        }

        public void ActualizarDibujoAhorcado()
        {
            int intento = Math.Min(_errores, 6); 
            string ruta = $"/AhorcadoCliente;component/Recursos/Iconos/Dibujo_Intento{intento}.png";
            ImgAhorcado.Source = new BitmapImage(new Uri(ruta, UriKind.Relative));
        }

        public Button BuscarBotonPorLetra(char letra)
        {
            letra = char.ToUpperInvariant(letra);
            foreach (var btn in FindVisualChildren<Button>(this))
            {
                if (btn.Content != null && btn.Content.ToString().ToUpperInvariant() == letra.ToString())
                {
                    return btn;
                }
            }
            return null;
        }

        public void RegistrarError()
        {
            _errores++;
            ActualizarDibujoAhorcado();
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child is T variable)
                    {
                        yield return variable;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
        private void btnClicAbandonar(object sender, RoutedEventArgs e)
        {
            var resultado = MessageBox.Show(
                Application.Current.TryFindResource("Msg_Descripcion_ConfirmarAbandono")?.ToString() ??
                "¿Estás seguro que deseas abandonar la partida? Serás declarado perdedor.",
                Application.Current.TryFindResource("Msg_Titulo_ConfirmarAbandono")?.ToString() ??
                "Confirmar abandono",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultado != MessageBoxResult.Yes)
                return;

            try
            {
                var cliente = SesionActual.ClienteWCF;
                int idJugador = SesionActual.JugadorActual.Id;
                int idPartida = cliente.ObtenerPartidaActivaDeJugador(idJugador);

                if (idPartida == 0)
                {
                    MessageDialog.Show("Msg_Titulo_Error", "Msg_Descripcion_SinPartidaActiva", MessageDialog.DialogType.ERROR, this);
                    return;
                }

                cliente.AbandonarPartida(idJugador, idPartida);
            }
            catch (Exception ex)
            {
                MessageDialog.Show("Msg_Titulo_Error", "Msg_Descripcion_ErrorAbandono", MessageDialog.DialogType.ERROR, this, ex.Message);
            }
            finally
            {
                var ventana = new MenuPrincipal();
                ventana.Show();
                this.Close();
            }
        }
    }
}
