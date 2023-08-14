using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Введите MAC-адрес отправителя:");
        string senderMacAddress = Console.ReadLine()!;

        Console.WriteLine("Введите IP-адрес отправителя:");
        string senderIpAddress = Console.ReadLine()!;

        Console.WriteLine("Введите MAC-адрес получателя:");
        string receiverMacAddress = Console.ReadLine()!;

        Console.WriteLine("Введите IP-адрес получателя:");
        string receiverIpAddress = Console.ReadLine()!;

        Console.WriteLine("Введите загрузку канала в Мбит/с (от 1 до 1000):");
        int channelLoad = int.Parse(Console.ReadLine()!);

        int packetsSent = 0;
        int packetsReceived = 0;
        int packetsLost = 0;

        try
        {
            using (UdpClient sender = new UdpClient())
            using (UdpClient receiver = new UdpClient())
            {
                IPAddress senderIp = IPAddress.Parse(senderIpAddress);
                IPAddress receiverIp = IPAddress.Parse(receiverIpAddress);

                PhysicalAddress senderMac = PhysicalAddress.Parse(senderMacAddress);
                PhysicalAddress receiverMac = PhysicalAddress.Parse(receiverMacAddress);

                sender.Connect(receiverIp, 0);
                receiver.Client.Bind(new IPEndPoint(senderIp, 0));

                byte[] data = new byte[1024];
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);


                int load = Math.Min(channelLoad, 1000);

                for (int i = 0; i < load; i++)
                {
                    sender.Send(data, data.Length);
                    packetsSent++;
                }

                Thread.Sleep(1000);

                while (receiver.Available > 0)
                {
                    receiver.Receive(ref remoteEndPoint);
                    packetsReceived++;
                }

                packetsLost = packetsSent - packetsReceived;

                Console.WriteLine($"Отправлено пакетов: {packetsSent}");
                Console.WriteLine($"Принято пакетов: {packetsReceived}");
                Console.WriteLine($"Потеряно пакетов: {packetsLost}");
                Console.WriteLine();

                packetsSent = 0;
                packetsReceived = 0;
                packetsLost = 0;

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}