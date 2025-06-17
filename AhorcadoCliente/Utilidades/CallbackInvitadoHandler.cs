using AhorcadoCliente.Vistas;
using AhorcadoCliente.ServiciosAhorcado;
using AhorcadoCliente.Utilidades;
using System.Windows;
using System.Linq;
using System.Windows.Media;
using System;

public class CallbackInvitadoHandler : IGestorPrincipalCallback
{
    private Window _ventana;
    private int _idPartida;
    private int _idJugador;
    private GestorPrincipalClient _cliente;

    public CallbackInvitadoHandler(Window ventana)
    {
        _ventana = ventana;
    }

    public void SetDatosPartida(int idPartida, int idJugador, GestorPrincipalClient cliente)
    {
        _idPartida = idPartida;
        _idJugador = idJugador;
        _cliente = cliente;
    }

    public void NotificarJugadorUnido(string usernameInvitado)
    {
        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
        {
            var jugarPartida = new JugarPartida(_idPartida, _idJugador, _cliente);
            jugarPartida.Show();

            foreach (Window win in Application.Current.Windows.OfType<Window>().ToList())
            {
                if (win != jugarPartida)
                {
                    win.Close();
                }
            }
        }));
    }

    public void NotificarIntentoLetra(char letra, bool acierto, string estadoActualPalabra)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var ventanaJuego = Application.Current.Windows.OfType<JugarPartida>().FirstOrDefault();
            if (ventanaJuego != null)
            {
                ventanaJuego.MostrarPalabra(estadoActualPalabra);
                var btn = ventanaJuego.BuscarBotonPorLetra(letra);
                if (btn != null)
                {
                    btn.IsEnabled = false;
                    if (!acierto)
                        btn.Background = Brushes.Red;
                }
            }
        });
    }

    public void NotificarFinPartida(bool gano, string mensaje)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var ventanaJuego = Application.Current.Windows.OfType<JugarPartida>().FirstOrDefault();
            if (ventanaJuego != null)
            {
                ventanaJuego.MostrarFinPartida(gano, mensaje); 
            }
        });
    }
}
