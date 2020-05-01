using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Lab_Reposición.Cifrados;
using Lab_Reposición.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace Lab_Reposición.Controllers
{//transposicion
    [Route("[controller]")]
    [ApiController]
    public class Lab5Controller : ControllerBase
    {
        public static IWebHostEnvironment _enviroment;
        public Lab5Controller(IWebHostEnvironment environment)
        {
            _enviroment = environment;
        }
        public class Upload
        {
            public IFormFile file { get; set; }
            public string LlaveCesar { get; set; }
            public int Carriles { get; set; }
            public int m { get; set; }
            public string Direccion { get; set; }
            public string Nombre { get; set; }

        }
        
        public async Task<string> SubirArchivo(IFormFile input)
        {
            
                if (!Directory.Exists(_enviroment.WebRootPath + "\\Upload\\"))
                {
                    Directory.CreateDirectory(_enviroment.WebRootPath + "\\Upload\\");
                }
            if (true)
            {

            }
                var filestream = System.IO.File.Create(_enviroment.WebRootPath + "\\Upload\\" + input.FileName);
                await input.CopyToAsync(filestream);
                filestream.Flush();
                filestream.Close();

            return filestream.Name; ;
        }

        public async Task<FileStreamResult> DownloadAsync(string Data)
        {
            var memory = new MemoryStream();

            var stream = new FileStream(_enviroment.WebRootPath + "\\Upload\\" + Data + ".txt", FileMode.Open);
            
                await stream.CopyToAsync(memory);
            stream.Close();

            memory.Position = 0;
            return File(memory, System.Net.Mime.MediaTypeNames.Application.Octet, Data + ".txt");
        }


        #region Lab5 Transposicion
        [HttpPost]
        [Route("Cifrar/{cifrado}")]
        public async Task<IActionResult> CifrarAsync([FromForm ]Upload Data, string cifrado)
        {
            switch (cifrado.ToLower())
            {
                case "cesar":
                    Transposicion.Instance.CifrarCesar(Data.LlaveCesar, 
                     await SubirArchivo(Data.file), Data.Nombre);
                    break;
                case "zigzag":
                    Transposicion.Instance.CifrarRail(Data.Carriles,
           await SubirArchivo(Data.file), Data.Nombre);

                    break;
                case "ruta":
                    Transposicion.Instance.Ruta(Data.m, Data.Direccion,
           await SubirArchivo(Data.file), Data.Nombre);

                    break;
                default:
                    break;
            }
            return await DownloadAsync(Data.Nombre);
        }
        [HttpPost]
        [Route("Decifrar/{cifrado}")]
        public async Task<IActionResult> DecifrarAsync([FromForm]Upload Data, string cifrado)
        {
            switch (cifrado.ToLower())
            {
                case "cesar":
                    Transposicion.Instance.DecifrarCesar(Data.LlaveCesar,
                     await SubirArchivo(Data.file), Data.Nombre);
                    break;
                case "zigzag":
                    Transposicion.Instance.DecifrarRail(Data.Carriles,
           await SubirArchivo(Data.file), Data.Nombre);

                    break;
                case "ruta":
                    Transposicion.Instance.Ruta(Data.m, Data.Direccion,
           await SubirArchivo(Data.file), Data.Nombre);

                    break;
                default:
                    break;
            }
            return await DownloadAsync(Data.Nombre);
        }
        #endregion

        #region Lab6

        [HttpPost("Lab6/CifrarCesar")]
        public void CifrarArchivoPriv([FromBody] object Json, IFormFile file)
        {
            var Entrada = JsonConvert.DeserializeObject<Input>(Json.ToString());
            if (Entrada.LlavePublica == null)
            {
                var CesarKey = RSA.Instance.RSA_Uncipher(int.Parse(Entrada.LlavePrivada), int.Parse(Entrada.N), Entrada.LlaveCesar);
                CifrarCesar(CesarKey, Entrada.Ruta, Entrada.NombreArchivo);

            }
            else
            {
                var CesarKey = RSA.Instance.RSA_Uncipher(int.Parse(Entrada.LlavePublica), int.Parse(Entrada.N), Entrada.LlaveCesar);
                CifrarCesar(CesarKey, Entrada.Ruta, Entrada.NombreArchivo);

            }
        }

        [HttpPost("Lab6/DecifrarCesar")]
        public void DecifrarArchivoPriv([FromBody] object Json)
        {
            var Entrada = JsonConvert.DeserializeObject<Input>(Json.ToString());
            if (Entrada.LlavePublica == null)
            {
                var CesarKey = RSA.Instance.RSA_Uncipher(int.Parse(Entrada.LlavePrivada), int.Parse(Entrada.N), Entrada.LlaveCesar);
                DecifrarCesar(CesarKey, Entrada.Ruta, Entrada.NombreArchivo);

            }
            else
            {
                var CesarKey = RSA.Instance.RSA_Uncipher(int.Parse(Entrada.LlavePublica), int.Parse(Entrada.N), Entrada.LlaveCesar);
                DecifrarCesar(CesarKey, Entrada.Ruta, Entrada.NombreArchivo);

            }
        }

        #region Cesar

        public void CifrarCesar(string clave, string _path, string nombre)//recibe el archivo a cifrar
        {
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
            var Original = new FileStream(_path, FileMode.Open); //archivo
            var reader = new StreamReader(Original);
            var nombrearchivo = $"{Path.GetDirectoryName(Original.Name)}\\{nombre}.txt";
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

            var Alfabeto = LlenarAlfabeto_DeCifrado();
            var write = "";
            var AlfabetoCifrado = new Dictionary<char, int>();
            byte[] KEYENCRYPTER = Encoding.ASCII.GetBytes(codigo);
            int n = 0;
            foreach (var item in KEYENCRYPTER)
            {
                if (!AlfabetoCifrado.ContainsValue((char)item))
                {
                    AlfabetoCifrado.Add((char)item, n);
                    n++;
                }
            }
            AlfabetoCifrado = LlenarCesar_DeCifrado(AlfabetoCifrado);

            var Cifrado = new FileStream(_path, FileMode.Open); //archivo
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
        #endregion
    }
}