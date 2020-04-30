using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lab_Reposición.Models
{
    class RSA
    {
        private string original_path;

        public string GlobalPath { get; set; }
        #region RSA
        private static RSA _instance = null;

        public static RSA Instance
        {
            get
            {
                if (_instance == null) _instance = new RSA();
                return _instance;
            }
        }
        public string GetPublicKey(int p, int q)
        {
            var monitor = 0;
            var PublicKey = 0;
            var PrivateKey = 0;
            //n y phi(n)
            var N = p * q;
            var Phin = (p - 1) * (q - 1);
            var random = new Random();
            var coprimo = false;
            int lel = MCD(p, q);

            //numero entre 1 y phi(n) y verificar que sean coprimos
            while (coprimo != true && monitor != 1)
            {
                PublicKey = random.Next(1, Phin);
                //ya son coprimos
                coprimo = coprimos(PublicKey, Phin);
                //obtener inverso multiplicativo0
                PrivateKey = modInverse(PublicKey, Phin);
                //1 si el inverso esta bien
                monitor = (PublicKey * PrivateKey) % Phin;
            }

            return $"{PublicKey}-{PrivateKey}-{N}";
        }
        public string RSA_Cipher(int Key, int n, string Cesar)
        {
            var PublicKey = Key;
            var PrivateKey = 0;
            //n y phi(n)
            var N = n;
            var Phin = (Key - 1) * (n - 1);
            var random = new Random();
            int lel = MCD(Key, n);
            var salida = string.Empty;
            foreach (var caracter in Cesar)
            {
                salida += ((char)Ecuacion(caracter, Key, N));

            }
            return salida;
        }

        public string RSA_Uncipher(int Llave, int N, string LlaveCifrada)
        {
            //Decifrando

            var salida = string.Empty;
            foreach (var caracter in LlaveCifrada)
            {
                salida += ((char)Ecuacion(caracter, Llave, N));
            }
            return salida;

        }
        int Ecuacion(char caracter, int key, int n)
        {
            return int.Parse(Convert.ToString(BigInteger.ModPow((int)caracter, key, n)));
        }
        bool coprimos(int a, int b)
        {
            int delete = MCD(a, b);
            if (delete == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        int MCD(int a, int b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }
            return a == 0 ? b : a;
        }
        int modInverse(int a, int m)
        {
            a = a % m;
            for (int x = 1; x < m; x++)
                if ((a * x) % m == 1)
                    return x;
            return 0;
        }

        #endregion
    }
}