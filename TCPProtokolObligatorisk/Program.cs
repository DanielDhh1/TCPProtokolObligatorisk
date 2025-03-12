using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPProtokolObligatorisk
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            TCPServer server = new TCPServer();
            JSONTCPServer jsonServer = new JSONTCPServer();

            await Task.WhenAll(server.StartAsync(), jsonServer.StartAsync());
        }
    }
}
