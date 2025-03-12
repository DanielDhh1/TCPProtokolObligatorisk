using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace TCPProtokolObligatorisk
{
    public class TCPServer
    {
        private const int _portNumber = 6969;
        public async Task StartAsync()
        {
            TcpListener server = new TcpListener(IPAddress.Any, _portNumber);
            server.Start();
            Console.WriteLine($"TCP server started on port {_portNumber}");

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
                    await writer.WriteLineAsync(
                        "Choose an option:\n" +
                        "1. Random.\n" +
                        "2. Add.\n" +
                        "3. Subtract.\n" +
                        "4. Close.\n" +
                        "Type in what you would like to choose.");

                    while (client.Connected)
                    {
                        string input = (await reader.ReadLineAsync())?.Trim().ToLower();

                        if (string.IsNullOrWhiteSpace(input))
                        {
                            await writer.WriteLineAsync("The input is invalid.");
                            continue;
                        }
                        if (input == "close")
                        {
                            await writer.WriteLineAsync("The connection has been closed. Goodbye");
                            break;
                        }
                        string response = await ProcessCommandAsync(input, reader, writer);
                        await writer.WriteLineAsync(response);
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
        private async Task<string> ProcessCommandAsync(string command, StreamReader reader, StreamWriter writer)
        {
            switch (command)
            {
                case "random":
                    await writer.WriteLineAsync("Enter two numbers: Number1 Number2:");
                    break;

                case "add":
                    await writer.WriteLineAsync("Enter two numbers: Number1 Number2:");
                    break;

                case "subtract":
                    await writer.WriteLineAsync("Enter two numbers: Number1 Number2:");
                    break;

                default:
                    return "Invalid input, try either: 'random' , 'add' , 'subtract' , or 'close'.";
            }

            string numbersInput = (await reader.ReadLineAsync())?.Trim();
            int[] numbers = ParseNumbers(numbersInput);

            if (numbers == null || numbers.Length != 2)
            {
                return "Invalid input, enter two numbers seperated by a space: Number1 Number2.";
            }
            return command switch
            {
                "random" => GenerateRandomNumber(numbers[0], numbers[1]),
                "add" => $"The sum of {numbers[0]} and {numbers[1]} is: {numbers[0] + numbers[1]}.",
                "subtract" => $"The difference between {numbers[0]} and {numbers[1]} is: {numbers[0] - numbers[1]}.",
                 _ => "Try again."
            };
        }

        private int[]? ParseNumbers(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;

            try
            {
                return Array.ConvertAll(input.Split(), int.Parse);
            }
            catch
            {
                return null;
            }
        }

        private string GenerateRandomNumber(int num1, int num2)
        {
            Random random = new Random();
            int min = Math.Min(num1, num2);
            int max = Math.Max(num1, num2);
            int randomNumber = random.Next(min, max);
            return $"Your random number between {min} and {max} is: {randomNumber}";
        }
    }
 }

