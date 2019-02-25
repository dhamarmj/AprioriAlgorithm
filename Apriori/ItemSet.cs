using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apriori
{
    public class ItemSet
    {
        public ItemSet(string nombre)
        {
            this.Nombre = nombre;
            this.Soporte = 0;
        }
        public ItemSet(string nombre, int soporte)
        {
            this.Nombre = nombre;
            this.Soporte = soporte;
        }
        public string Nombre { get; set; }
        public int Soporte { get; set; }
    }
}
