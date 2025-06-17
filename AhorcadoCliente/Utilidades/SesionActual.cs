using AhorcadoCliente.Modelo;
using AhorcadoCliente.ServiciosAhorcado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AhorcadoCliente.Utilidades
{
    public static class SesionActual
    {
        public static Jugador JugadorActual { get; set; }
        public static GestorPrincipalClient ClienteWCF { get; set; }
        public static CallbackHandler CallbackAnfitrion { get; set; }
        public static Window VentanaMenuPrincipal { get; set; }
        public static string IdiomaActual { get; set; }
    }
}
