using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Lab_Reposición.Models;
using Lab_Reposición.Data;

namespace Lab_Reposición.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Lab5Controller : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Lab5, Cifrados";
        }
        [HttpPost]
        [Route("cipher/{tipo}")]
        public void Cifrar([FromBody]object json,string tipo)
        {
            var x = JsonConvert.DeserializeObject<Entrada>(json.ToString());
            switch (tipo.ToLower())
            {
                case "cesar":
                    Cifrados.Instance.CifrarCesar(x.LlaveCesar, x.Path, x.Nombre);
                    break;
                case "zigzag":
                    Cifrados.Instance.CifrarRail(x.Carriles, x.Path, x.Nombre);
                    break;
                case "ruta":
                    Cifrados.Instance.CifrarRuta(x.m, x.Direccion, x.Path, x.Nombre);
                    break;
                default:
                    break;
            }
        }
        [HttpPost]
        [Route("decipher/{tipo}")]
        public void Decifrar([FromBody]object json, string tipo)
        {
            var x = JsonConvert.DeserializeObject<Entrada>(json.ToString());
            switch (x.Algoritmo.ToLower())
            {
                case "cesar":
                    Cifrados.Instance.DecifrarCesar(x.LlaveCesar, x.Path, x.Nombre);
                    break;
                case "zigzag":
                    Cifrados.Instance.DecifrarRail(x.Carriles, x.Path, x.Nombre);
                    break;
                case "ruta":
                    Cifrados.Instance.DecifrarRuta(x.m, x.Direccion, x.Path, x.Nombre);
                    break;
                default:
                    break;
            }
        }
    }
}