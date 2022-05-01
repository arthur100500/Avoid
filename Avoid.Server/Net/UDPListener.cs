using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avoid.Server.Net
{
	using System;
	using System.Net;
	using System.Net.Sockets;
	using System.Text;

	public class UDPListener
	{
		private const int listenPort = 11000;
		UdpClient listener;
		IPEndPoint groupEP;
		public UDPListener()
		{
			listener = new UdpClient(listenPort);
			groupEP = new IPEndPoint(IPAddress.Any, listenPort);
		}
		public string Recieve()
		{
			byte[] bytes = listener.Receive(ref groupEP);

			//Console.WriteLine($"{groupEP} : {Encoding.ASCII.GetString(bytes, 0, bytes.Length)}");

			return groupEP.ToString() + " : " + Encoding.ASCII.GetString(bytes, 0, bytes.Length);
		}


	}
}
