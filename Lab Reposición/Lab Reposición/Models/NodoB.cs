using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab_Reposición.Models
{
    public class NodoB
    {
        public int Grado;
        public Bebida AuxSubir;
        public Bebida[] Datos;
        public NodoB[] Hijos;
        public NodoB Padre;

        public NodoB(int _grado)
        {
            Datos = new Bebida[_grado - 1];
            Hijos = new NodoB[_grado];
            Grado = _grado;

        }
    }
}
