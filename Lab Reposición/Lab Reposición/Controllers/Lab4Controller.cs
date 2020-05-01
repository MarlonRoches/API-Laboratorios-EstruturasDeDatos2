using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Lab_Reposición.Compresiones;
namespace Lab_Reposición.Controllers
{//compresion huff y lzw
    [Route("[controller]")]
    [ApiController]
    public class Lab4Controller : ControllerBase
    {
        public static IWebHostEnvironment _enviroment;
        public Lab4Controller(IWebHostEnvironment environment)
        {
            _enviroment = environment;
        }
        public class Upload
        {
            public IFormFile file { get; set; }

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

            return filestream.Name;
        }

        public async Task<FileStreamResult> DownloadAsync(string Data)
        {
            var memory = new MemoryStream();

            var stream = new FileStream(Data, FileMode.Open);

            await stream.CopyToAsync(memory);
            stream.Close();

            memory.Position = 0;
            var d = File(memory, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(Data));
            return d;
        }
        [HttpPost("Compresion/{tipo}")]
        public async Task<IActionResult> Compresiones([FromForm]Upload file, string tipo)
        {
            var reader = new StreamReader(await SubirArchivo(file.file));
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

                    
                    return await DownloadAsync(LWZ_API.Instance.CompresionLZW(listabytes, file.file.FileName, await SubirArchivo(file.file)));
                }
                else if (tipo.ToLower() == "huff")
                {
                    //huffman
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
        }
        [HttpPost("Desompresion/{tipo}")]
        public async Task<IActionResult> Descompresiones([FromForm]Upload file, string tipo)
        {
            var reader = new StreamReader(await SubirArchivo(file.file));
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

                  
                    return await DownloadAsync(LWZ_API.Instance.DescompresionLZW(listabytes, file.file.FileName, await SubirArchivo(file.file)));
                }
                else if (tipo.ToLower() == "huff")
                {
                    //huffman
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