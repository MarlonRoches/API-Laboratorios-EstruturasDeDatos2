using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lab_Reposición.Models
{
    public class ArbolHuffman
    {
        private static ArbolHuffman _instance = null;
        public static ArbolHuffman Instance
        {
            get
            {
                if (_instance == null) _instance = new ArbolHuffman();
                return _instance;
            }
        }
        #region Variables

        List<NodoHuffman> Arbol = new List<NodoHuffman>();
        private int bufferLength = 10000;
        Dictionary<char, decimal> Letras = new Dictionary<char, decimal>();
        List<NodoHuffman> DiccionarioPrefijos = new List<NodoHuffman>();
        int cantidad_de_letras = 0;
        int max = 0;
        Dictionary<char, string> IndexID = new Dictionary<char, string>();
        string GlobalPath = null;
        string rutaAGuardar;
        bool escrito;
        Dictionary<char, string> DicPrefijos = new Dictionary<char, string>();
        private string rutadesalida;
        Dictionary<string, string> LetPrefijos = new Dictionary<string, string>();
        #endregion

        //C:\Users\roche\Desktop\BIBLIA COMPLETA.txt
        //C:\Users\roche\Desktop\Tea.txt
        public string Compresion_Huffman(string _root,string nombrenuevo)
        {
            GlobalPath = _root;
            var file = new FileStream(GlobalPath, FileMode.OpenOrCreate);
            var Lector = new StreamReader(file);
            var byteBuffer = Lector.ReadToEnd();//buffer


            foreach (var Caracter in byteBuffer)
            {
                cantidad_de_letras++;
                var car = (char)Caracter;
                if (Letras.ContainsKey(car)) //si lo tiene
                {
                    Letras[car]++;
                }
                else// no lo tien
                {
                    Letras.Add(car, 1);
                }
            } //llenar el diccionario con la cantidad total de letras


            var keys = Letras.Keys;
            var ArrayProbabilidades = new decimal[cantidad_de_letras];
            int num = 0;
            foreach (var key in keys) //calculando probabilidades para luego meterlas en el diccionario
            {
                ArrayProbabilidades[num] = Letras[key] / cantidad_de_letras;
                num++;

            }
            var DiccionarioAuxiliar = new Dictionary<char, decimal>();
            num = 0;
            foreach (var key in keys)
            {
                DiccionarioAuxiliar.Add(key, ArrayProbabilidades[num]);
                num++;
            } //llenar auxiliar
            Letras = DiccionarioAuxiliar;
            file.Close();//cerramos coneccion con archivo
            InsertarEnLaLista();
            EscribirDiccionario(nombrenuevo);
            PrefijoMasGrande();
            ComprimirTexto(nombrenuevo);
            Arbol = new List<NodoHuffman>();
            bufferLength = 10000;
            Letras = new Dictionary<char, decimal>();
            DiccionarioPrefijos = new List<NodoHuffman>();
            cantidad_de_letras = 0;
            max = 0;
            IndexID = new Dictionary<char, string>();
            GlobalPath = "";
            rutaAGuardar = "";
            escrito = false;
            DicPrefijos = new Dictionary<char, string>();

            LetPrefijos = new Dictionary<string, string>();
            return rutadesalida;
        }
        void InsertarEnLaLista()
        {
            var asignado = false;
            DiccionarioPrefijos = Arbol.OrderBy(x => x.Probabilidad).ToList();
            foreach (var item in Letras) //PARA CADA NODO DEL DICCIONARIO
            {
                var nuevo = new NodoHuffman();
                //Llenar Nodo
                nuevo.Nombre = (byte)item.Key;
                nuevo.Probabilidad = item.Value;
                Arbol.Add(nuevo);
            }
            Arbol = Arbol.OrderBy(x => x.Probabilidad).ToList();
            if (asignado == false)
            {
                DiccionarioPrefijos = Arbol.OrderBy(x => x.Probabilidad).ToList();
                asignado = true;
            }

            var n = 1;
            while (Arbol.Count != 1)
            {
                var NuevoPadre = new NodoHuffman();
                var nodo = new NodoHuffman();
                //daba problemas por que el nodo papa estaba declarado afuera, entonces siempre era el mismo
                #region Asigncacion Padre
                NuevoPadre.Derecha = Arbol.First();
                NuevoPadre.Izquierda = Arbol.ElementAt(1);
                NuevoPadre.esHoja = false;
                NuevoPadre.Izquierda.Padre = NuevoPadre;
                NuevoPadre.Derecha.Padre = NuevoPadre;
                if (NuevoPadre.Izquierda != null)
                {
                    NuevoPadre.Izquierda.SoyIzquierda = true;
                    NuevoPadre.Izquierda.SoyDerecha = false;
                }
                if (NuevoPadre.Derecha != null)
                {
                    NuevoPadre.Derecha.SoyDerecha = true;
                    NuevoPadre.Derecha.SoyIzquierda = false;
                }
                NuevoPadre.Probabilidad = NuevoPadre.Derecha.Probabilidad + NuevoPadre.Izquierda.Probabilidad;


                #endregion
                Arbol.RemoveAt(0);
                Arbol.RemoveAt(0);
                Arbol.Add(NuevoPadre);
                n++;
                Arbol = Arbol.OrderBy(x => x.Probabilidad).ToList();
            }
            Prefijos(Arbol[0], "");

        }
        void Prefijos(NodoHuffman _Actual, string prefijo)
        {
            if (_Actual.Derecha == null && _Actual.Izquierda == null)
            {
                DicPrefijos.Add((char)_Actual.Nombre, prefijo);
                LetPrefijos.Add(((char)_Actual.Nombre).ToString(), prefijo);
            }
            else
            {
                if (_Actual.Derecha != null)
                {
                    Prefijos(_Actual.Derecha, prefijo + 1);

                }
                if (_Actual.Izquierda != null)
                {
                    Prefijos(_Actual.Izquierda, prefijo + 0);
                }

            }
        }
        void EscribirDiccionario(string Nombre)
        {
            var path = Path.GetDirectoryName(GlobalPath);
            var Name = Path.GetFileNameWithoutExtension(GlobalPath);
            var file = new FileStream($"{path}\\Huff_{Nombre}.txt", FileMode.OpenOrCreate);
            var writer = new StreamWriter(file);
            foreach (var item in DicPrefijos)
            {
                writer.WriteLine($"{item.Key.ToString()}|{item.Value}^");//178
            }
            writer.Write("END");//179│
            writer.Close();
            file.Close();
            rutadesalida = $"{path}\\Huff_Compressed_{Name}.txt";

        }
        void ComprimirTexto(string Nombre)
        {
            var textocomprimido = string.Empty;
            var path = Path.GetDirectoryName(GlobalPath);
            var Name = Path.GetFileNameWithoutExtension(GlobalPath);
            var Compressed = new FileStream($"{path}\\Huff_{Nombre}.txt", FileMode.Append);
            var writer = new StreamWriter(Compressed);
            var DeCompressed = new FileStream(GlobalPath, FileMode.OpenOrCreate);
            var Lector = new StreamReader(DeCompressed);
            var byteBuffer = Lector.ReadToEnd();//buffer
            var x = string.Empty;
            var ver = "";
            foreach (var Caracter in byteBuffer)
            {
                x += DicPrefijos[Caracter];
                ver += Caracter;
                if (x.Length >= 8)
                {
                    var bytewrt1 = (char)String_A_Byte(x.Substring(0, 8));
                    x = x.Remove(0, 8);
                    textocomprimido += bytewrt1;
                }
            }
            x = x.PadLeft(8, '0');
            var bytewrt = (char)String_A_Byte(x);
            x = x.Remove(0, 8);
            textocomprimido += bytewrt;

            writer.Write(textocomprimido);
            writer.Close();
            Lector.Close();
            DeCompressed.Close();
            Compressed.Close();
            rutadesalida = $"{path}\\Huff_{Name}.txt";

        }
        void PrefijoMasGrande()
        {
            foreach (var item in DicPrefijos)
            {
                if (item.Value.Length > max)
                {
                    max = item.Value.Length;
                }
            }
        }
        byte String_A_Byte(string bufer) //String binario a byte
        {

            int num, binVal, decVal = 0, baseVal = 1, rem;
            num = int.Parse(bufer);
            binVal = num;

            while (num > 0)
            {
                rem = num % 10;
                decVal = decVal + rem * baseVal;
                num = num / 10;

                baseVal = baseVal * 2;
            }
            return Convert.ToByte(decVal);
        }


        public string Descompresio_Huffman(string _path)
        {
            var Reconstruido = new Dictionary<string, char>();
            GlobalPath = _path;
            var file = new FileStream(GlobalPath, FileMode.Open);
            var lector = new StreamReader(file);
            string diccionario = string.Empty;
            var CaracterActual = lector.Read();
            var position = 0;
            var cont = 0;
            while (!diccionario.Contains("END"))
            {
                diccionario += (char)((byte)CaracterActual);
                CaracterActual = lector.Read();
                cont++;
            }
            position = diccionario.Replace("\r", "").Length + 1;
            var RawDoc = diccionario.Replace("\r\n", "").Replace("END", "").Split('^');
            var textocompreso = "";
            while (!lector.EndOfStream)
            {
                textocompreso += (char)CaracterActual;
                CaracterActual = lector.Read();
            }
            textocompreso += (char)CaracterActual;
            foreach (var item in RawDoc)
            {
                if (item == "")
                {
                    break;
                }
                var splited = item.Split('|');
                if (splited[0] == "")
                {
                    Reconstruido.Add(splited[1], '|');

                }
                else
                {
                    Reconstruido.Add(splited[1], (char)(byte)splited[0][0]);

                }
            }
            var path = Path.GetDirectoryName(GlobalPath); var descompreso = string.Empty;
            var Name = Path.GetFileNameWithoutExtension(GlobalPath);
            var actual = string.Empty;
            var decompresofile = new FileStream($"{path}\\DesComp_{Name}.txt".Replace("Huff_", ""), FileMode.Create);
            var writer = new BinaryWriter(decompresofile);
            var salida = "";
            foreach (var item in textocompreso)
            {
                actual += Convert.ToString(item, 2).PadLeft(8, '0');

            }
            while (actual != "")
            {

                for (int i = 0; i < actual.Length; i++)
                {
                    var x = actual.Substring(0, i);
                    if (Reconstruido.ContainsKey(x))
                    {
                        if (actual.Length < 8)
                        {
                            actual = "";
                            break;
                        }
                        writer.Write(Reconstruido[x]);
                        salida += Reconstruido[x];
                        actual = actual.Remove(0, i);
                    }
                    else
                    {
                        if (actual.Length < 8)
                        {
                            actual = "";
                            break;
                        }
                    }
                }
            }
            writer.Close();
            file.Close();
            lector.Close();
            return $"{path}\\DesComp_{Name}.txt".Replace("Huff_Compressed_", "");

        }

    }

}
