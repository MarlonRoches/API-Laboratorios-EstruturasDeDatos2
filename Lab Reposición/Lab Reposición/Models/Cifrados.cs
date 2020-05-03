using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_Reposición.Models
{
        public class Cifrados
        {
            private static Cifrados _instance = null;
            public static Cifrados Instance
            {
                get
                {
                    if (_instance == null) _instance = new Cifrados();
                    return _instance;
                }
            }
            string GlobalPath = string.Empty;
            #region VariablesGlobales
            string original_path = string.Empty;
            string index_p10 = "2416390875";
            //string Index_PermutacionSeleccionada = "39860127";
            string index_p8 = "52637498";
            string index_p4 = "0321";
            string index_Expand = "13023201";
            string index_inicial = "15203746";
            string index_IPinverse = "30246175";
            string index_leftshift1 = "12340";
            string[,] S0 = new string[4, 4];
            string[,] S1 = new string[4, 4];
            #endregion


            #region Ruta

            public void Ruta(int m, string tipo, string _Path, string nombre)
            {
                GlobalPath = _Path;
                var File = new FileStream(GlobalPath, FileMode.Open);
                var reader = new StreamReader(File);
                var raw_text = reader.ReadToEnd();
                var salida = string.Empty;
                var n = 0;

                while ((n * m) < raw_text.Length)
                {
                    n++;
                }
                File.Close();
                switch (tipo.ToLower())
                {
                    case "ver-hor":
                        salida = Vertical_a_Horizontal(raw_text, m, n);
                        break;

                    case "ver-ver":
                        salida = raw_text;
                        break;

                    case "hor-ver":
                        salida = Horizontal_a_Vertical(raw_text, m, n);
                        break;
                    case "hor-hor":
                        salida = raw_text;
                        break;

                    case "horario-hor":
                        salida = LecturaHoraria(raw_text, m, n, true);
                        break;

                    case "horario-der":
                        salida = LecturaHoraria(raw_text, m, n, false);
                        break;

                    case "anti-der":
                        break;

                    case "anti-izq":
                        break;
                    default:
                        break;
                }
                var Cifrado = new FileStream($"{Path.GetDirectoryName(GlobalPath)}\\{nombre}.txt", FileMode.Create);
                var writer = new StreamWriter(Cifrado);
                writer.Write(salida);
                Cifrado.Close();
            }

            public void DecifrarRuta(int m, int n, bool tipo, bool Horario, bool Horizontal, string _Path)
            {
                GlobalPath = _Path;

            }
            public string Horizontal_a_Vertical(string Texto, int m, int n)
            {
                var i = 0;
                var matriz = new string[n, m];

                for (int y = 0; y < n; y++)
                {
                    for (int x = 0; x < m; x++)
                    {
                        if (i < Texto.Length)
                        {

                            matriz[y, x] = Texto[i].ToString();
                            i++;
                        }
                        else
                        {
                            matriz[y, x] = "_";
                        }

                    }
                }
                var salida = string.Empty;
                for (int x = 0; x < m; x++)
                {
                    for (int y = 0; y < n; y++)
                    {

                        salida += matriz[y, x];
                        i++;

                    }
                }
                return salida;
            }
            public string Vertical_a_Horizontal(string Texto, int m, int n)
            {

                var i = 0;
                var matriz = new string[n, m];


                for (int x = 0; x < m; x++)
                {
                    for (int y = 0; y < n; y++)
                    {
                        if (i < Texto.Length)
                        {

                            matriz[y, x] = Texto[i].ToString();
                            i++;
                        }
                        else
                        {
                            matriz[y, x] = "_";
                        }

                    }
                }
                var salida = string.Empty;
                for (int y = 0; y < n; y++)
                {
                    for (int x = 0; x < m; x++)
                    {

                        salida += matriz[y, x];
                        i++;

                    }
                }
                return salida;
            }
            public string LecturaHoraria(string Texto, int m, int n, bool lectura)
            {
                bool derecha = true, izquierda = false, abajo = false;
                var matrizc = new int[m, n];
                int x = 0, y = -1;

                for (int k = 1; k <= n * m; k++)
                {
                    if (izquierda)
                    {
                        y--;
                        if (y == -1)
                        {
                            y = 0; x--;
                            izquierda = false;
                        }
                        else if (matrizc[x, y] != 0)
                        {
                            y++; x--;
                            izquierda = false;
                        }
                    }
                    else if (derecha)
                    {
                        y++;
                        if (y == n)
                        {
                            y = n - 1; x++;
                            derecha = false;
                            abajo = true;
                        }
                        else if (matrizc[x, y] != 0)
                        {
                            y--; x++;
                            derecha = false;
                            abajo = true;
                        }
                    }
                    else if (abajo)
                    {
                        x++;
                        if (x == m)
                        {
                            x = m - 1; y--;
                            abajo = false;
                            izquierda = true;
                        }
                        else if (matrizc[x, y] != 0)
                        {
                            y--; x--;
                            abajo = false;
                            izquierda = true;
                        }
                    }
                    else
                    {
                        x--;
                        if (x == -1 || matrizc[x, y] != 0)
                        {
                            x++; y++;
                            derecha = true;
                        }
                    }

                    matrizc[x, y] = Texto[k - 1];
                }

                //lectura 
                var salida = string.Empty;
                if (lectura)
                {//horizontal

                    for (int ym = 0; ym < m; ym++)
                    {
                        for (int xm = 0; xm < n; xm++)
                        {
                            salida += (char)matrizc[ym, xm];
                        }
                    }
                }
                else
                {//vertical
                    for (int xm = 0; xm < n; xm++)
                    {
                        for (int ym = 0; ym < m; ym++)
                        {
                            salida += (char)matrizc[ym, xm];
                        }
                    }
                }

                return salida;
            }


            #endregion
            int Calcular_n(int m, string texto)
            {
                var n = 0;
                while ((n * m) < texto.Length)
                {
                    n++;
                }
                return n;
            }
            #region Rail
            public void CifrarRail(int grado, string _path, string NonmbreArchivo)
            {
                GlobalPath = _path;
                var decoded = new FileStream(GlobalPath, FileMode.Open);
                var reader = new StreamReader(decoded);
                var text = reader.ReadToEnd();
                var nivel = new string[grado];
                var index = 0;
                bool direction = false;
                var ciclos = text.Length / grado;
                while (text != "")
                {
                    if (index < 4) //abajo
                    {
                        for (int i = 0; i < grado; i++)
                        {
                            if (text == "")
                            {
                                break;
                            }
                            nivel[i] = nivel[i] + text[0].ToString();
                            text = text.Remove(0, 1);
                            index++; direction = false;
                        }
                        if (text == "")
                        {
                            break;
                        }
                    }//para abajo
                    else //arriba
                    {
                        for (int i = grado - 2; i > 0; i--)
                        {
                            if (text == "")
                            {
                                for (int r = i; r > -1; r--)
                                {
                                    nivel[r] = nivel[r] + "$";
                                }
                                break;
                            }
                            nivel[i] = nivel[i] + text[0].ToString();
                            text = text.Remove(0, 1);
                            direction = true;
                        }
                        index = 0;
                    } //para arriba

                }

                //Escritura
                var nombrearchivo = $"{Path.GetDirectoryName(decoded.Name)}\\{NonmbreArchivo}_ZigZag.txt";
                var encoded = new FileStream(nombrearchivo, FileMode.OpenOrCreate); //archivo
                var writer = new BinaryWriter(encoded);
                var codedtext = "";
                foreach (var item in nivel)
                {
                    codedtext = codedtext + item;
                }
                var temp = codedtext.ToArray();
                foreach (var item in temp)
                {
                    writer.Write(item);

                }
                decoded.Close();
                encoded.Close();


                var monitor = 0;
            }
            public void DecifrarRail(int grado, string _path, string NonmbreArchivo)
            {
                GlobalPath = _path;
                var Original = new FileStream(GlobalPath, FileMode.Open);
                var reader = new StreamReader(Original);
                var ciphertext = reader.ReadToEnd();
                var m = (ciphertext.Length + (2 * grado) - 3) / ((2 * grado) - 2);
                var midtline = (m - 1) * 2;
                var lastline = m - 1;
                var nivel = new string[grado];
                //primer nivel
                nivel[0] = ciphertext.Substring(0, m);
                ciphertext = ciphertext.Remove(0, m);
                //intermedios
                for (int i = 1; i < grado - 1; i++)
                {
                    nivel[i] = ciphertext.Substring(0, midtline);
                    ciphertext = ciphertext.Remove(0, midtline);

                }
                //final
                nivel[grado - 1] = ciphertext;
                var uncipher = string.Empty;
                while (nivel[0] != "" && nivel[grado - 1] != "")
                {
                    var index = 0;
                    for (int i = 0; i < grado; i++)
                    {
                        uncipher = uncipher + nivel[i][0];
                        nivel[i] = nivel[i].Remove(0, 1);
                    }
                    for (int i = grado - 2; i > 0; i--)
                    {
                        uncipher = uncipher + nivel[i][0];
                        nivel[i] = nivel[i].Remove(0, 1);
                    }
                }
                uncipher = uncipher.Replace('$', ' ');


                var nombrearchivo = $"{Path.GetDirectoryName(Original.Name)}\\{NonmbreArchivo}_ZZD.txt".Replace("_ZigZag", "");
                var decoded = new FileStream(nombrearchivo, FileMode.OpenOrCreate); //archivo
                var writer = new BinaryWriter(decoded);
                var codedtext = "";


                foreach (var item in uncipher)
                {
                    writer.Write(item);

                }
                decoded.Close();
                Original.Close();


                var matrix = new string[grado];
            }
            // listo
            #endregion
            #region Cesar

            public void CifrarCesar(string clave, string _path, string nombre)//recibe el archivo a cifrar
            {
                GlobalPath = _path;
                var Alfabeto = LlenarAlfabeto_Cifrado();
                var write = "";
                var AlfabetoCifrado = new Dictionary<int, char>();
                byte[] KEYENCRYPTER = Encoding.ASCII.GetBytes(clave);
                var n = 0;
                foreach (var item in KEYENCRYPTER)
                {
                    if (!AlfabetoCifrado.ContainsValue((char)item))
                    {
                        AlfabetoCifrado.Add(n, (char)item);
                        n++;
                    }
                }
                AlfabetoCifrado = LlenarCesar_Cifrado(AlfabetoCifrado);

                #region Variables De Acceso
                var Original = new FileStream(GlobalPath, FileMode.Open); //archivo
                var reader = new StreamReader(Original);
                var nombrearchivo = $"{Path.GetDirectoryName(Original.Name)}\\Ces_{nombre}.txt";
                var encoded = new FileStream(nombrearchivo, FileMode.OpenOrCreate); //archivo
                var writer = new BinaryWriter(encoded);

                #endregion
                var text = reader.ReadToEnd();
                foreach (var item in text)
                {
                    if (Alfabeto.ContainsKey(item) == false)
                    {
                        Alfabeto.Add(item, Alfabeto.Count);
                    }
                    if (AlfabetoCifrado.ContainsValue(item) == false)
                    {
                        AlfabetoCifrado.Add(AlfabetoCifrado.Count, item);
                    }

                    var monitor = Alfabeto[item];
                    var monitorcecar = AlfabetoCifrado[monitor];
                    write = $"{write}{monitorcecar}";
                }
                foreach (var item in write)
                {
                    writer.Write(item);
                }
                Original.Close();
                encoded.Close();
            }
            public void DecifrarCesar(string codigo, string _path, string nombre)
            {
                GlobalPath = _path;

                var Alfabeto = LlenarAlfabeto_DeCifrado();
                var write = "";
                var AlfabetoCifrado = new Dictionary<char, int>();
                byte[] KEYENCRYPTER = Encoding.ASCII.GetBytes(codigo);
                var n = 0;
                foreach (var item in KEYENCRYPTER)
                {
                    if (!AlfabetoCifrado.ContainsValue((char)item))
                    {
                        AlfabetoCifrado.Add((char)item, n);
                        n++;
                    }
                }
                AlfabetoCifrado = LlenarCesar_DeCifrado(AlfabetoCifrado);

                var Cifrado = new FileStream(GlobalPath, FileMode.Open); //archivo
                var reader = new StreamReader(Cifrado);
                var nombrearchivo = $"{Path.GetDirectoryName(Cifrado.Name)}\\{nombre}_Des.txt".Replace("Ces_", "");

                var decoded = new FileStream(nombrearchivo, FileMode.OpenOrCreate); //archivo
                var writer = new BinaryWriter(decoded);


                var text = reader.ReadToEnd();
                foreach (var item in text)
                {
                    if (Alfabeto.ContainsValue(item) == false)
                    {
                        Alfabeto.Add(Alfabeto.Count, item);
                    }
                    if (AlfabetoCifrado.ContainsKey(item) == false)
                    {
                        AlfabetoCifrado.Add(item, AlfabetoCifrado.Count);
                    }

                    var monitor = AlfabetoCifrado[item];
                    var monitorcecar = Alfabeto[monitor];
                    write = $"{write}{monitorcecar}";
                }
                foreach (var item in write)
                {
                    writer.Write(item);
                }
                decoded.Close();
                Cifrado.Close();
            }
            Dictionary<char, int> LlenarAlfabeto_Cifrado()
            {
                var temp = new Dictionary<char, int>();
                int cont = 0;
                // var fTemp = new FileStream("C:\\Users\\roche\\Desktop\\Lab22_Cifrado\\Alfabeto.txt", FileMode.Open);
                // var reader = new StreamReader(fTemp);
                var bufer = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZÁÉÍÓÚabcdefghijklmnñopqrstuvwxyzáéíóú";

                foreach (var item in bufer)
                {
                    if (!temp.ContainsKey(item))
                    {
                        temp.Add((char)item, cont);
                        cont++;
                    }

                }
                // fTemp.Close();
                return temp;
            }
            Dictionary<int, char> LlenarCesar_Cifrado(Dictionary<int, char> actual)
            {

                // var fTemp = new FileStream("C:\\Users\\roche\\Desktop\\Lab22_Cifrado\\Alfabeto.txt", FileMode.Open);
                //  var reader = new StreamReader(fTemp);
                var texto = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZÁÉÍÓÚabcdefghijklmnñopqrstuvwxyzáéíóú";
                foreach (var item in texto)
                {
                    if (actual.ContainsValue(item) == false)
                    {
                        actual.Add(actual.Count, item);
                    }

                }


                //  fTemp.Close();
                return actual;
            }
            Dictionary<int, char> LlenarAlfabeto_DeCifrado()
            {
                var temp = new Dictionary<int, char>();
                int cont = 0;
                //  var fTemp = new FileStream("C:\\Users\\roche\\Desktop\\Lab22_Cifrado\\Alfabeto.txt", FileMode.Open);
                //  var reader = new StreamReader(fTemp);
                var bufer = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZÁÉÍÓÚabcdefghijklmnñopqrstuvwxyzáéíóú";

                foreach (var item in bufer)
                {
                    if (!temp.ContainsKey(item))
                    {
                        temp.Add(cont, (char)item);
                        cont++;
                    }

                }
                //  fTemp.Close();
                return temp;
            }
            Dictionary<char, int> LlenarCesar_DeCifrado(Dictionary<char, int> actual)
            {

                //  var fTemp = new FileStream("C:\\Users\\roche\\Desktop\\Lab22_Cifrado\\Alfabeto.txt", FileMode.Open);
                //  var reader = new StreamReader(fTemp);
                var texto = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZÁÉÍÓÚabcdefghijklmnñopqrstuvwxyzáéíóú";
                foreach (var item in texto)
                {
                    if (actual.ContainsKey(item) == false)
                    {
                        actual.Add(item, actual.Count);
                    }

                }


                // fTemp.Close();
                return actual;
            }
            #endregion

        } //listo
    
}
