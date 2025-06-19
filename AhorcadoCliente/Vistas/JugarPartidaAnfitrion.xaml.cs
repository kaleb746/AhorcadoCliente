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
            int intento = Math.Min(_errores + 1, 6); 
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

        }
    }
}
