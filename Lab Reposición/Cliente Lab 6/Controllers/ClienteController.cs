using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Lab_Reposición.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace Lab_Reposición.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        static string PublicKey;
        static string PrivateKey;
        static string N;
        static bool Llave = false;
        [HttpGet("getPublicKey")]
        public string Inicio([FromBody] object Json)
        {
            return "Lab #6 RSA";
        }
        [HttpPost("getPublicKey")]
        public string GetPublicKey([FromBody] object Json)
        {
            var Entrada = JsonConvert.DeserializeObject<Input>(Json.ToString());
            var Keys = RSA.Instance.GetPublicKey(Entrada.p, Entrada.q).Split('-');

            PublicKey = Keys[0];
            PrivateKey = Keys[1];
            N = Keys[2];

            Llave = true;
            return $"Llave Publica: {Keys[0]}, N: {Keys[2]}";
        }
        [HttpPost("getPrivateKey")]

        [HttpPost("CifrarArchivo")]
        public async Task<string> CifrarArchivoConPublicaAsync([FromBody] object Json)
        {
            if (Llave)
            {

                var Entrada = JsonConvert.DeserializeObject<Input>(Json.ToString());
                Entrada.LlaveCesar = RSA.Instance.RSA_Cipher(int.Parse(PublicKey), int.Parse(N), Entrada.LlaveCesar);
                Entrada.N = N;
                Entrada.LlavePublica = null;
                Entrada.LlavePrivada = PrivateKey;
                // enviar al servidor y generar archivo
                var json = JsonConvert.SerializeObject(Entrada);
                //enviar a api y generar token

                var cliente = new HttpClient();

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var respose = await cliente.PostAsync("https://localhost:44384/api/Backend/CifrarCesar", content);

                return $"Ok";

            }
            else
            {
                return $"Generar Llaves Primero";

            }

        }
        [HttpPost("DecifrarArchivo")]
        public async Task<string> DeifrarArchivoConPublicaAsync([FromBody] object Json)
        {
            if (Llave)
            {

                var Entrada = JsonConvert.DeserializeObject<Input>(Json.ToString());
                Entrada.LlaveCesar = RSA.Instance.RSA_Cipher(int.Parse(PublicKey), int.Parse(N), Entrada.LlaveCesar);
                Entrada.N = N;
                Entrada.LlavePublica = null;
                Entrada.LlavePrivada = PrivateKey;
                // enviar al servidor y generar archivo
                var json = JsonConvert.SerializeObject(Entrada);
                //enviar a api y generar token

                var cliente = new HttpClient();

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var respose = await cliente.PostAsync("https://localhost:44384/api/Backend/DecifrarCesar", content);

                return $"Ok";

            }
            else
            {
                return $"Generar Llaves Primero";

            }

        }
    }
}