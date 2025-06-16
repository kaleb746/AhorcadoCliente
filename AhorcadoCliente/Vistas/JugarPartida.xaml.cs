using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System;
using System.Windows.Media;
using AhorcadoCliente.ServiciosAhorcado;
using System.Windows.Media.Imaging;
using System.Linq;

namespace AhorcadoCliente.Vistas
{
    public partial class JugarPartida : Window
    {
        private GestorPrincipalClient _cliente;
        private int _idPartida;
        private int _idJugador;
        private int _errores = 0;

        public JugarPartida(int idPartida, int idJugador, GestorPrincipalClient cliente)
        {
            InitializeComponent();
            _idPartida = idPartida;
            _idJugador = idJugador;
            _cliente = cliente;

            Loaded += JugarPartida_Loaded;
        }
        private void JugarPartida_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string estadoPalabra = _cliente.ObtenerEstadoPalabra(_idPartida);

                if (!string.IsNullOrEmpty(estadoPalabra))
                    MostrarPalabra(estadoPalabra);
                else
                    MostrarPalabra("_____");

                AsignarEventosBotonesLetras();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en Loaded: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void AsignarEventosBotonesLetras()
        {
            foreach (var boton in FindVisualChildren<Button>(this))
            {
                if (boton.Content is string contenido && contenido.Length == 1)
                {
                    boton.Click += BotonLetra_Click;
                }
            }
        }
        private void BotonLetra_Click(object sender, RoutedEventArgs e)
        {
            var boton = sender as Button;
            if (boton == null || boton.Content == null)
                return;

            char letra = boton.Content.ToString().ToUpperInvariant()[0];
            boton.IsEnabled = false;

            try
            {
                bool acierto = _cliente.IntentarLetra(_idPartida, _idJugador, letra);

                if (acierto)
                {
                    boton.Background = Brushes.Green;
                }
                else
                {
                    boton.Background = Brushes.Red;
                    RegistrarError();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al intentar letra: {ex.Message}");
            }
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
        public void ActualizarDibujoAhorcado()
        {
            int intento = Math.Min(_errores + 1, 6); // Limita el valor a 6
            string ruta = $"/AhorcadoCliente;component/Recursos/Iconos/Dibujo_Intento{intento}.png";
            ImgAhorcado.Source = new BitmapImage(new Uri(ruta, UriKind.Relative));
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
        public void MostrarFinPartida(bool gano, string mensaje)
        {
            MessageBox.Show(mensaje, gano ? "¡Victoria!" : "Derrota", MessageBoxButton.OK,
                            gano ? MessageBoxImage.Information : MessageBoxImage.Warning);

            foreach (var boton in FindVisualChildren<Button>(this))
            {
                if (boton.Content is string contenido && contenido.Length == 1)
                {
                    boton.IsEnabled = false;
                }
            }
            var ventana = new MenuPrincipal();
            ventana.Show();
            this.Close();
        }

    }
}