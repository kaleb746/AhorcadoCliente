namespace AhorcadoCliente.Utilidades

{
    public static class Constantes
    {
        public static readonly int LONGITUG_MAXIMA = 32;
        public static readonly int LONGITUG_TELEFONO = 10;

        public static readonly string PATRON_TEXTO_GENERAL = @"[^A-Za-zÁÉÍÓÚÜÑáéíóúüñ0-9\s\-\.,()/#]+$";
        public static readonly string PATRON_CORREO_GENERAL = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
    }
}
