using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;

namespace AhorcadoCliente
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static event Action IdiomaCambiado;
        public App()
        {
            this.Startup += OnStartup;
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            CambiarIdioma("es-MX"); 
        }

        public void CambiarIdioma(string codigoCultura)
        {
            var cultura = new CultureInfo(codigoCultura);
            Thread.CurrentThread.CurrentCulture = cultura;
            Thread.CurrentThread.CurrentUICulture = cultura;

            var diccionario = new ResourceDictionary
            {
                Source = new Uri($"Properties/Strings.{cultura.Name}.xaml", UriKind.Relative)
            };

            // Reemplaza el diccionario anterior
            var diccionarioExistente = Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Resources."));
            if (diccionarioExistente != null)
            {
                Resources.MergedDictionaries.Remove(diccionarioExistente);
            }
            Resources.MergedDictionaries.Add(diccionario);

            // Lanza el evento
            IdiomaCambiado?.Invoke();
        }
    }

}

