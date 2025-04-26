using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1ChatMulti
{

    internal class Client
    {
        private readonly int localPort = 8081;
        private readonly IPAddress multicastAddress;
        private UdpClient receiver;
        private UdpClient sender;

        public string Name { get; set; }
        public event Action<string> WriteMessage;

        public Client()
        {
            multicastAddress = IPAddress.Parse("224.0.0.0");

            receiver = new UdpClient();
            receiver.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            receiver.ExclusiveAddressUse = false;
            receiver.Client.Bind(new IPEndPoint(IPAddress.Any, localPort));
            receiver.JoinMulticastGroup(multicastAddress);
            receiver.MulticastLoopback = true;

            sender = new UdpClient();
            sender.JoinMulticastGroup(multicastAddress);
            sender.MulticastLoopback = true;
        }

        public void Connect(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            Name = name;
            WriteMessage?.Invoke("Your name: " + Name);
        }

        public async Task ReceiveM()
        {
            while (true)
            {
                var result = await receiver.ReceiveAsync();
                string message = Encoding.UTF8.GetString(result.Buffer);

                WriteMessage?.Invoke(message + " [" + result.RemoteEndPoint + "]");
            }
        }

        public async Task SendM(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            byte[] data = Encoding.UTF8.GetBytes($"{Name}: {message}");
            await sender.SendAsync(data, data.Length, new IPEndPoint(multicastAddress, localPort));

            WriteMessage?.Invoke("You: " + message);
        }
    }



}
