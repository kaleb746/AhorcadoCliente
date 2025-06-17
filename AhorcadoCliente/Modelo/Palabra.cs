using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhorcadoCliente.Modelo
{
    public class Palabra
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string NombreIngles { get; set; }
        public string Descripcion { get; set; }
        public string DescripcionIngles { get; set; }
        public int IdCategoria { get; set; }
        public int IdDificultad { get; set; }
        public override string ToString()
        {
            string idioma = CultureInfo.CurrentUICulture.Name;
            return idioma.StartsWith("en") ? NombreIngles : Nombre;
        }
    }
}

