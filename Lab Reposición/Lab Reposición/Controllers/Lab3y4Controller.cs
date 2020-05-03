using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Lab_Reposición.Models;
using Newtonsoft.Json;
using System.IO;

namespace Lab_Reposición.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Lab3y4Controller : ControllerBase
    {
        public class Entrada
        {
            public string FilePath { get; set; }
        }
        [HttpGet]
        public string Get()
        {
            return "Lab 2 y 3, LZW y Huffman ";
        }
        [HttpPost("Compresion/{nombre}/{tipo}")]
        public  IActionResult Compresiones([FromBody]object json, string nombre, string tipo)
        {
            var entrada = JsonConvert.DeserializeObject<Entrada>(json.ToString());
            var reader = new StreamReader(entrada.FilePath);
            var longitud = Convert.ToInt32(reader.BaseStream.Length);
            var buffer = reader.ReadToEnd();
            reader.Close();
            var listabytes = new List<string>();
            foreach (var item in buffer)
            {
                listabytes.Add(item.ToString());
            }
            if (tipo == "" || tipo == null)
            {
                return BadRequest();
            }
            else
            {

                if (tipo.ToLower() == "lzw")
                {


                    LWZ_API.Instance.CompresionLZW(listabytes,nombre,entrada.FilePath);
                    return Ok();
                }
                else if (tipo.ToLower() == "huff")
                {
                    //huffman
                    ArbolHuffman.Instance.Compresion_Huffman(entrada.FilePath, nombre);
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
        }
        [HttpPost("Desompresion/{nombre}/{tipo}")]
        public IActionResult Descompresiones([FromBody]object json, string nombre, string tipo)
        {
            var entrada = JsonConvert.DeserializeObject<Entrada>(json.ToString());
            var reader = new StreamReader(entrada.FilePath);

            var longitud = Convert.ToInt32(reader.BaseStream.Length);
            var buffer = reader.ReadToEnd();
            reader.Close();
            var listabytes = new List<string>();
            foreach (var item in buffer)
            {
                listabytes.Add(item.ToString());
            }
            if (tipo == "" || tipo == null)
            {
                return BadRequest();
            }
            else
            {
                if (tipo.ToLower() == "lzw")
                {

                    LWZ_API.Instance.DescompresionLZW(listabytes, entrada.FilePath);
                    return Ok();
                }
                else if (tipo.ToLower() == "huff")
                {
                    ArbolHuffman.Instance.Descompresio_Huffman(entrada.FilePath);

                    return Ok();

                }
                else
                {
                    return BadRequest();

                }
            }
        }
    }
}