using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using ApiInterface.Models;

namespace ApiInterface
{
    public class server
    {
        //Levanta el servidor y espera solicitudes asincronicas.
        public static async Task start()
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, 11000);
            listener.Start();

            Console.WriteLine($"Servidor inicializado en {IPAddress.Loopback}:11000");

            try
            {
                while (true)
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    Thread handleRequest = new Thread(() => requestHandler(client));
                    handleRequest.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en el servidor {ex.Message}.");
            }
            finally
            {
                listener.Stop();
            }

        }
        
        public static async void requestHandler(TcpClient client)
        {
            NetworkStream clientStream = client.GetStream();
            string rawMessage = await getRawMessage(clientStream);
            var requestObject = convertJsonToRequest(rawMessage);
        }

        public static async Task<String> getRawMessage(NetworkStream clientStream)
        {
            byte[] buffer = new byte[1024];
            int bytesRead = await clientStream.ReadAsync(buffer, 0, buffer.Length);
            string incomingMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            incomingMessage = incomingMessage.TrimEnd("<EOM>".ToCharArray());

            return incomingMessage;
        }

        public static Request convertJsonToRequest(string rawMessage)
        {
            return JsonSerializer.Deserialize<Request>(rawMessage) ?? throw new Exception("Invalid request.");
        }
    }
}
