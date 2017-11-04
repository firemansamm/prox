using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace reverseproxy
{
	internal class Program
	{

		private static void Main(string[] args)
		{

		    if (args.Length != 1)
		    {
		        Console.WriteLine("usage: {0} [hostname]", AppDomain.CurrentDomain.FriendlyName);
                return;
		    }

			TcpListener tcpListener = new TcpListener(IPAddress.Any, 4114);
			tcpListener.Start(100);
			while (true)
			{
				SocketHandler socketHandler = new SocketHandler(tcpListener.AcceptSocket(), args[0]);
				new Thread(socketHandler.handleConnection).Start();
			}
		}
	}
}