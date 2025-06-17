using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using AhorcadoCliente.ServiciosAhorcado;
using AhorcadoCliente.Vistas;
using AhorcadoCliente.Utilidades;

public class CallbackHandler : IGestorPrincipalCallback
{
    private int _idPartida;
    private int _idJugador;
    private GestorPrincipalClient _cliente;

    public CallbackHandler(Window ventana)
    {
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
            string mensaje = string.Format(
                Application.Current.TryFindResource("Msg_Descripcion_JugadorUnido")?.ToString()
                ?? "Se ha unido el jugador: {0}", usernameInvitado);

            MessageDialog.Show("Msg_Titulo_JugadorUnido", mensaje, MessageDialog.DialogType.INFO);

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
                MessageDialog.Show("Msg_Titulo_FinPartida", mensaje, MessageDialog.DialogType.INFO);

                var menu = new MenuPrincipal();
                menu.Show();

                ventana.Close();
            }
        });
    }
}
