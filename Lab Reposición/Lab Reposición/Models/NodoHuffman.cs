using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab_Reposición.Models
{
    public class NodoHuffman
    {
        public bool esHoja { get; set; }
        public string Prefijo { get; set; }
        public byte Nombre { get; set; }
        public decimal Probabilidad { get; set; }
        public bool SoyDerecha;
        public bool SoyIzquierda;
        public NodoHuffman Padre;
        public NodoHuffman Izquierda;
        public NodoHuffman Derecha;

        public NodoHuffman()
        {
            esHoja = true;
            Prefijo = "";
            Probabilidad = 0;
            Padre = null;
            Izquierda = null;
            Derecha = null;
            SoyDerecha = false;
            SoyIzquierda = false;
        }

    }
}
