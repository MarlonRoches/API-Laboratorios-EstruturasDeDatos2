using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lab_Reposición.Models
{
    class LWZ_API
    {
        private static LWZ_API _instance = null;
        public static LWZ_API Instance
        {
            get
            {
                if (_instance == null) _instance = new LWZ_API();
                return _instance;
            }
        }

        public string CompresionLZW(List<string> Caracteres, string nombre, string _path)
        {
            int Iteracion = 0;
            string salida = "";  //cambiar por escritura del archivo
            var path = _path;
            string W = "", K = "";
            var DiccionarioGeneral = ObetnerDiccionarioInicial();//poner como parametro el path global de data
            var DiccionarioWK = ObetnerDiccionarioInicial();
            var residuoEscritura = string.Empty;
            var diccionarioescrito = true;
            var retorna = "";
            CompresionLZW();
            return retorna;
            void CompresionLZW()
            {
                Iteracion--;
                foreach (var Caracter in Caracteres)
                {
                    var WK = "";
                    if (W == "")
                    {
                        WK = (Caracter).ToString();
                        Validacion_Diccionario(WK);
                    }
                    else
                    {
                        K = (Caracter).ToString();
                        WK = W + K;
                        Validacion_Diccionario(WK);
                    }
                }

                Agregar_A_Salida(DiccionarioGeneral[W], false);
                EscribirCompress();
            }

            void Validacion_Diccionario(string WK)
            {
                if (diccionarioescrito)
                {
                    EscribirDiccionario();
                }
                if (DiccionarioGeneral.ContainsKey(WK))
                {
                    W = WK;
                }
                else
                {
                    Iteracion++;
                    DiccionarioGeneral.Add(WK, Iteracion); //generamos codigo

                    Agregar_A_Salida(DiccionarioGeneral[W], true);

                    W = K;

                }
            }

            void Agregar_A_Salida(int id, bool caso)
            {
                var carsito = (char)id;
                salida += carsito;
            }

            void EscribirDiccionario()
            {
                string path = $"{Path.GetDirectoryName(_path)}\\{nombre}";

                var File = new FileStream($"{path}_lzw.txt", FileMode.Create);
                var writer = new StreamWriter(File);
                if (diccionarioescrito)
                {
                    foreach (var item in DiccionarioWK)
                    {
                        writer.Write($"{item.Key}|{item.Value}♀");
                    }
                    writer.Write("END");
                    diccionarioescrito = false;
                }
                writer.Close();
            }
            void EscribirCompress()
            {
                string path = Directory.GetCurrentDirectory();

                var File = new FileStream($"{Path.GetDirectoryName(_path)}\\{Path.GetFileNameWithoutExtension(_path)}_lzw.txt", FileMode.Append);
                var writer = new StreamWriter(File);
                foreach (var item in salida)
                {
                    writer.Write(item);
                }
                writer.Close();
                File.Close();
                retorna = $"{Path.GetDirectoryName(_path)}\\{Path.GetFileNameWithoutExtension(_path)}_lzw.txt";
            }
            Dictionary<string, int> ObetnerDiccionarioInicial()
            {
                var Diccionario = new Dictionary<string, int>();
                Iteracion = 0;
                foreach (var Caracter in Caracteres) //Crear diccionario de letras
                {

                    if (!Diccionario.ContainsKey(Convert.ToString(Caracter)))
                    {
                        Diccionario.Add(Convert.ToString(Caracter), Iteracion);
                        Iteracion++;
                    }
                }

                return Diccionario;
            }
        }

        public string DescompresionLZW(List<string> Caracteres, string _path)
        {
            var DiccionarioDescompresion = new Dictionary<int, string>();
            var Iteracion = 0;
            var linea = string.Empty;
            linea += Caracteres[0];
            Caracteres.RemoveAt(0);
            int CodigoViejo = 0, CodigoNuevo = 0;
            string Cadena = string.Empty, Caracter = string.Empty;
            var Salida = string.Empty;
            var vuela = "";
            while (!linea.Contains("END"))
            {

                linea += Caracteres[0];

                Caracteres.RemoveAt(0);
            }
            var caractrer = linea.Split('♀');
            foreach (var item in caractrer)
            {
                if (item == "END")
                {
                    break;
                }
                var temp = item.Split('|');
                if (temp.Length == 3)
                {
                    // tiene | incluido
                    DiccionarioDescompresion.Add(int.Parse(temp[2]), "|");
                }
                else
                {
                    DiccionarioDescompresion.Add(int.Parse(temp[1]), temp[0]);

                }

            }
            Iteracion = DiccionarioDescompresion.Count();
            CodigoViejo = (int)Caracteres[0][0];
            Caracteres.RemoveAt(0);
            Caracter = DiccionarioDescompresion[CodigoViejo];
            Salida += Caracter;

            //MIENTRAS (!EOF)
            while (Caracteres.Count != 0)
            {
                //...LEER cód_nuevo
                CodigoNuevo = (int)Caracteres[0][0];
                Caracteres.RemoveAt(0);

                if (CodigoNuevo == -1)
                {
                    break;
                }
                if (!DiccionarioDescompresion.ContainsKey(CodigoNuevo))
                {
                    Cadena = DiccionarioDescompresion[CodigoViejo] + DiccionarioDescompresion[CodigoViejo][0];
                    DiccionarioDescompresion.Add(Iteracion, Cadena);
                    Iteracion++;
                    Salida += CodigoNuevo;
                    CodigoViejo = CodigoNuevo;
                }
                //...SINO
                else
                {
                    Cadena = DiccionarioDescompresion[CodigoNuevo];
                    Salida += Cadena;
                    Caracter = Cadena[0].ToString();
                    DiccionarioDescompresion.Add(Iteracion, $"{DiccionarioDescompresion[CodigoViejo]}{Caracter}");
                    Iteracion++;
                    CodigoViejo = CodigoNuevo;
                }
            }


            //Escritura en archivo 
            string pathDevuelta = $"{Path.GetDirectoryName(_path)}\\{Path.GetFileNameWithoutExtension(_path)}_Decompress.txt".Replace("_lzw", "");
            var Decompress = new FileStream($"{pathDevuelta}", FileMode.Create);
            var writer = new StreamWriter(Decompress);
            for (int i = 0; i < Salida.Length; i++)
            {
                if (i == 16033)
                {

                }
                writer.Write(Salida[i]);

            }
            writer.Close();
            Decompress.Close();
            return pathDevuelta;
        }

    }

}
