using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apriori
{
    public class Regla
    {
        public Regla()
        {

        }
        public Regla(string A, string C)
        {
            this.Antecedente = A;
            this.Consecuente = C;
            this.Confianza = 0;
        }
        public string Antecedente { get; set; }
        public string Consecuente { get; set; }
        public float Confianza { get; set; }
    }
}
