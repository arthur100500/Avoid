using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avoid.Net
{
	using System;
	using System.Net;
	using System.Net.Sockets;
	using System.Text;

	public class UDPListener
	{
		private const int listenPort = 11001;
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

			return Encoding.ASCII.GetString(bytes, 0, bytes.Length);
		}
	}
}
