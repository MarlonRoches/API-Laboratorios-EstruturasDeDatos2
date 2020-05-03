using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab_Reposición.Models
{
    public class Input
    {
        public string cipher { get; set; }
        public int p { get; set; }
        public int q { get; set; }
        public string LlaveCesar { get; set; }
        public string NombreArchivo { get; set; }
        public string Ruta { get; set; }
        public string LlavePublica { get; set; }
        public string LlavePrivada { get; set; }
        public string N { get; set; }
    }
}
