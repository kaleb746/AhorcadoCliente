using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhorcadoCliente.Modelo
{
    public class PartidaDisponible
    {
        public int IdPartida { get; set; }
        public string NombrePalabra { get; set; }
        public string Categoria { get; set; }
        public string Dificultad { get; set; }
        public DateTime Fecha { get; set; }
        public string Idioma { get; set; }
        public int IdJugadorAnfitrion { get; set; }
        public string UsuarioAnfitrion { get; set; }
        public string Usuario => UsuarioAnfitrion;
    }
}
