using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using AhorcadoCliente.ServiciosAhorcado;
using AhorcadoCliente.Vistas;

public class CallbackHandler : IGestorPrincipalCallback
{
    private int _idPartida;
    private int _idJugador;
    private GestorPrincipalClient _cliente;

    public CallbackHandler(Window ventana)
    {
        // Ya no se requiere la ventana directamente
    }

    public void SetDatosPartida(int idPartida, int idJugador, GestorPrincipalClient cliente)
    {
        _idPartida = idPartida;
        _idJugador = idJugador;
        _cliente = cliente;
    }

    public void NotificarJugadorUnido(string usernameInvitado)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            MessageBox.Show($"Se ha unido el jugador: {usernameInvitado}", "Notificación");

            var ventana = new JugarPartidaAnfitrion();
            ventana.Show();

            foreach (var win in Application.Current.Windows.OfType<Window>().ToList())
            {
                if (win != ventana)
                    win.Close();
            }
        });
    }
    public void NotificarIntentoLetra(char letra, bool acierto, string estadoActualPalabra)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var ventana = Application.Current.Windows.OfType<JugarPartidaAnfitrion>().FirstOrDefault();
            if (ventana != null)
            {
                ventana.MostrarPalabra(estadoActualPalabra);
                var btn = ventana.BuscarBotonPorLetra(letra);
                if (btn != null)
                {
                    btn.IsEnabled = false;
                    btn.Background = acierto ? Brushes.Green : Brushes.Red;

                    if (!acierto)
                        ventana.RegistrarError();

                    ventana.ActualizarDibujoAhorcado();
                }
            }
        });
    }
    public void NotificarFinPartida(bool gano, string mensaje)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var ventana = Application.Current.Windows.OfType<JugarPartidaAnfitrion>().FirstOrDefault();
            if (ventana != null)
            {
                MessageBox.Show(mensaje, "Fin de la partida", MessageBoxButton.OK, MessageBoxImage.Information);

                var menu = new MenuPrincipal();
                menu.Show();

                ventana.Close();
            }
        });
    }


}
