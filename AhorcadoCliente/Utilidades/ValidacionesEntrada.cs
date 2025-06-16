using AhorcadoCliente.Utilidades;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace AhorcadoCliente.Utilidades
{
    public static class ValidacionesEntrada
    {
        public static void ValidarEntrada(TextBox textBox, string patron, int longitudMaxima)
        {
            textBox.TextChanged += (s, e) =>
            {
                string entrada = textBox.Text;
                string limpiado = Regex.Replace(entrada, patron, "");

                if (limpiado.Length > longitudMaxima)
                    limpiado = limpiado.Substring(0, longitudMaxima);

                if (entrada != limpiado)
                {
                    textBox.Text = limpiado;
                    textBox.SelectionStart = limpiado.Length;
                    Animaciones.SacudirTextBox(textBox);
                }
            };
        }
        public static void ValidarEntradaContrasena(PasswordBox passwordBox, string patron, int longitudMaxima)
        {
            passwordBox.PasswordChanged += (s, e) =>
            {
                string entrada = passwordBox.Password;
                string limpiado = Regex.Replace(entrada, patron, "");

                if (limpiado.Length > longitudMaxima)
                    limpiado = limpiado.Substring(0, longitudMaxima);

                if (entrada != limpiado)
                {
                    passwordBox.Password = limpiado;
                    Animaciones.SacudirPasswordBox(passwordBox);
                }
            };
        }
        public static bool EsFormatoCorreoValido(string email)
        {
            return Regex.IsMatch(email, Constantes.PATRON_CORREO_GENERAL);
        }
    }
}