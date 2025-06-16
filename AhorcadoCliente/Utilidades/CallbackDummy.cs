using AhorcadoCliente.ServiciosAhorcado;

namespace AhorcadoCliente.Utilidades
{
    public class CallbackDummy : IGestorPrincipalCallback
    {
        public void NotificarJugadorUnido(string usernameInvitado)
        {
        }

        public void NotificarIntentoLetra(char letra, bool acierto, string estadoActualPalabra)
        {
        }
        public void NotificarFinPartida(bool gano, string mensaje)
        {
        }
    }
}