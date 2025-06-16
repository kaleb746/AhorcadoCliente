using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhorcadoCliente.Modelo
{
    public class HistorialPartida
    {
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; }
        public string Dificultad { get; set; }
        public string Resultado { get; set; }
    }
}
