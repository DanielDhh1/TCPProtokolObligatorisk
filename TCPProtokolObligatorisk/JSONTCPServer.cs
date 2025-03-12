using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace TCPProtokolObligatorisk
{
    public class JSONTCPServer
    {
        private const int _portNumber = 6970;

        public async Task StartAsync()
        {
            TcpListener server = new TcpListener(IPAddress.Any, _portNumber);
            server.Start();
            Console.WriteLine($"JSON TCP Server has started on port {_portNumber}");

            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                Console.WriteLine("Client connected");
                
                _= HandleClientAsync(client);
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                using (var reader = new StreamReader(client.GetStream()))
                using (var writer = new StreamWriter(client.GetStream()) { AutoFlush = true })
                {
                    await writer.WriteLineAsync("Welcome to the JSON TCP Server");
                    await writer.WriteLineAsync("Send a JSON request with the following format:");
                    await writer.WriteLineAsync("{ \"method\": \"add|subtract|random|close\", \"Number1\": <number>, \"Number2\": <number> }");

                    while (client.Connected)
                    {
                        string jsonRequest = await reader.ReadLineAsync();
                        Console.WriteLine($"Recieved JSON: {jsonRequest}");

                        if (string.IsNullOrWhiteSpace(jsonRequest))
                        {
                            await writer.WriteLineAsync("Invalid input, try again");

                            continue;
                        }

                        try
                        {
                            var request = JsonConvert.DeserializeObject<Request>(jsonRequest);

                            var response = ProcessRequest(request);

                            if (response.Result ==-1 && response.Error == "close")
                            {
                                await writer.WriteLineAsync("The connection has been closed. Goodbye");

                                break;
                            }
                            string jsonResponse = JsonConvert.SerializeObject(response);
                            Console.WriteLine($"Sending JSON: {jsonResponse}");

                            await writer.WriteLineAsync(jsonResponse);
                        }
                        catch (JsonException) 
                        {
                            await writer.WriteLineAsync("Invalid JSON input, try again");
                        }
                        catch (Exception ex)
                        {
                            await writer.WriteLineAsync($"Error: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");    
            }
            finally
            {
                client.Close();
                Console.WriteLine("Client has been disconnected");
            }
        }
        private Response ProcessRequest(Request request)
        {
            switch (request.Method.ToLower())
            {
                case "random":
                    Random random = new Random();
                    int min = Math.Min(request.Number1, request.Number2);
                    int max = Math.Max(request.Number1, request.Number2);
                    int result = random.Next(min, max + 1);
                    return new Response { Result = result };

                case "add":
                    return new Response { Result = request.Number1 + request.Number2 };

                case "subtract":
                    return new Response { Result = request.Number1 - request.Number2 };

                case "close":
                    return new Response { Result = -1, Error = "close" };

                default:
                    return new Response { Result = 0, Error = "Invalid input, try either: 'random' , 'add' , 'subtract' , or 'close'." };
            }
        }
    }
}
