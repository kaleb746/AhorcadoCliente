using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AhorcadoCliente.Utilidades
{
    public static class MessageDialog
    {
        public enum DialogType { INFO, WARNING, ERROR }

        public static void Show(string tituloKey, string descripcionKey, DialogType tipo, Window owner = null, params object[] args)
        {
            string titulo = Application.Current.TryFindResource(tituloKey)?.ToString() ?? tituloKey;
            string descripcionBase = Application.Current.TryFindResource(descripcionKey)?.ToString() ?? descripcionKey;

            string descripcion = (args != null && args.Length > 0)
                ? string.Format(descripcionBase, args)
                : descripcionBase;

            MessageBoxImage icono;
            switch (tipo)
            {
                case DialogType.WARNING:
                    icono = MessageBoxImage.Warning;
                    break;
                case DialogType.ERROR:
                    icono = MessageBoxImage.Error;
                    break;
                default:
                    icono = MessageBoxImage.Information;
                    break;
            }


            if (owner != null)
                MessageBox.Show(owner, descripcion, titulo, MessageBoxButton.OK, icono);
            else
                MessageBox.Show(descripcion, titulo, MessageBoxButton.OK, icono);
        }
    }
}
