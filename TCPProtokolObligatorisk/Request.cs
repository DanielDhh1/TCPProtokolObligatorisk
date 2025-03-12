using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPProtokolObligatorisk
{
    public class Request
    {
        public string Method { get; set; }
        
        public int Number1 { get; set; }
        
        public int Number2 { get; set; }
    }
}
