using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Lab_Reposición.Models;
using System.Web;
using System.Text;
//lab 3
namespace Lab_Reposición.Compresiones
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
        Dictionary<byte, string> DicPrefijos = new Dictionary<byte, string>();
        Dictionary<string, string> LetPrefijos = new Dictionary<string, string>();
        #endregion

        //C:\Users\roche\Desktop\BIBLIA COMPLETA.txt
        //C:\Users\roche\Desktop\Tea.txt
        public void Compresion_Huffman(string _root)
        {
            GlobalPath = _root;
            var file = new FileStream(GlobalPath, FileMode.OpenOrCreate);
            var Lector = new BinaryReader(file);
            var byteBuffer = new byte[bufferLength];//buffer

            while (Lector.BaseStream.Position != Lector.BaseStream.Length)
            {
                byteBuffer = Lector.ReadBytes(bufferLength);
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

            }
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
            EscribirDiccionario();
            PrefijoMasGrande();
            ComprimirTexto();
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
                Prefijos(Arbol[0],"");
            
        }
         void Prefijos(NodoHuffman _Actual, string prefijo)
        {
            if (_Actual.Derecha == null && _Actual.Izquierda == null)
            {
                DicPrefijos.Add(_Actual.Nombre, prefijo);
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
         void EscribirDiccionario()
        {
            var path = Path.GetDirectoryName(GlobalPath);
            var Name = Path.GetFileNameWithoutExtension(GlobalPath);
            var file = new FileStream($"{path}\\Compressed_{Name}.huff",FileMode.OpenOrCreate);
            var writer = new StreamWriter(file);
            foreach (var item in DicPrefijos)
            {
                writer.WriteLine($"{item.Key.ToString()}|{item.Value}^");//178
            }
            writer.Write("END");//179│
            writer.Close();
            file.Close();
        }
         void ComprimirTexto()
        {
            var textocomprimido = string.Empty;
            var path = Path.GetDirectoryName(GlobalPath);
            var Name = Path.GetFileNameWithoutExtension(GlobalPath);
            var Compressed = new FileStream($"{path}\\Compressed_{Name}.huff", FileMode.Append);
            var writer = new BinaryWriter(Compressed);
            var DeCompressed = new FileStream(GlobalPath, FileMode.OpenOrCreate);
            var Lector = new BinaryReader(DeCompressed);
            var byteBuffer = new byte[bufferLength];//buffer
            var x = string.Empty;
            while (Lector.BaseStream.Position != Lector.BaseStream.Length)
            {
                byteBuffer = Lector.ReadBytes(bufferLength);
                foreach (var Caracter in byteBuffer)
                {
                    x += DicPrefijos[Caracter];
                    if (x.Length >= 8)
                    {
                        var bytewrt = (Char)String_A_Byte(x.Substring(0, 8));
                        x = x.Remove(0, 8);
                        textocomprimido += bytewrt;
                         writer.Write(bytewrt);
                    }
                } 

            }
            DeCompressed.Close();
            Compressed.Close();
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


        public void Descompresio_Huffman(string _path)
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
                cont ++;
            }
            position= diccionario.Replace("\r","").Length+1;
            var RawDoc = diccionario.Replace("\r\n","").Replace("END","").Split('^');

            foreach (var item in RawDoc)
            {
                if (item =="")
                {
                    break;
                }
                var splited = item.Split('|');
                Reconstruido.Add(splited[1],(char)(byte)int.Parse(splited[0]));
            }
            
            var path = Path.GetDirectoryName(GlobalPath); var descompreso = string.Empty;
            var Name = Path.GetFileNameWithoutExtension(GlobalPath);            var actual = string.Empty;
              var decompresofile = new FileStream($"{path}\\DesComp_{Name}.txt", FileMode.Create);
            var writer = new BinaryWriter(decompresofile);
            
            while (cont != lector.BaseStream.Length)
            {
                actual += Convert.ToString(CaracterActual ,2).PadLeft(8, '0');
                for (int i = 0; i < actual.Length; i++)
                {
                    var x = actual.Substring(0, i);
                    if (Reconstruido.ContainsKey(x))
                    {
                        writer.Write(Reconstruido[x]);
                        actual = actual.Remove(0, i);
                    }
                }
                cont++;
                CaracterActual = lector.Read();
            }
            writer.Close();
            file.Close();
            lector.Close();
        }

    }
}