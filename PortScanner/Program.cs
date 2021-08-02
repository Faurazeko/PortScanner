using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

namespace PortScanner
{
    class Program
    {
        static private List<int> AvailablePorts = new List<int>();

        const int LASTPORT = 65535;

        static void Main()
        {
            Console.Write("Port scanner by Faurazeko\nEnter IP: ");

            while (true)
            {
                var ports = CheckAllPorts(Console.ReadLine(), 100);
                Console.WriteLine("\n\n\nSo, here the available ports:");
                foreach (var item in ports)
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine("-===END===-");

                Console.Write("If you wanna scan some another IP, just enter this IP: ");
            }
        }

        static public int[] CheckAllPorts(string host, int ThreadsCount)
        {
            Thread[] threads = new Thread[ThreadsCount];

            int portsPerThread = LASTPORT / ThreadsCount;

            int tempPort = 0;

            for (int i = 1; i < ThreadsCount; i++)
            {
                int firstPort = tempPort;
                int lastPort = tempPort + portsPerThread;
                tempPort = lastPort + 1;

                threads[i] = new Thread( () => CheckPortsRange(host, firstPort, lastPort));
            }
            threads[0] = new Thread( () => CheckPortsRange(host, tempPort, LASTPORT));

            Console.WriteLine("Processing. Please wait");

            foreach (var thread in threads)
                thread.Start();

            foreach (var thread in threads)
                thread.Join();

            return AvailablePorts.ToArray();
        }

        static public void CheckPortsRange(string host, int firstPort, int lastPort)
        {

            for (int i = firstPort; i < lastPort; i++)
            {
                if(CheckPort(host, i))
                {
                    AvailablePorts.Add(i);
                    Console.WriteLine($"Founded available port - {i}");
                }
                else
                    Console.WriteLine($"{i} is blocked :(");
            }
        }

        static public bool CheckPort(string host, int port)
        {
            if (port > LASTPORT || port < 0)
                return false;

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            try
            {
                sock.Connect(host, port);
            }
            catch (Exception)
            {
                return false;
            }

            sock.Disconnect(false);
            return true;
        }
    }
}
