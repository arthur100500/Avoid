using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Avoid.Server.Net
{
	public class UDPSender
	{

		Socket s;
		public UDPSender()
		{
			s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		}

		public void SendData(IPEndPoint where, string what)
		{
			byte[] sendbuf = Encoding.ASCII.GetBytes(what);
			where.Port = 11001;
			s.SendTo(sendbuf, where);

			Console.WriteLine($"Message sent to ({where}): " + what);
		}
	}
}
