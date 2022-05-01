using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Avoid.Net
{
	public class UDPSender
	{
		IPAddress broadcast;
		IPEndPoint ep;
		Socket s;
		public UDPSender()
		{
			s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

			broadcast = IPAddress.Parse(File.ReadAllText("Files/mp.txt").Split("\n")[1]);

			ep = new IPEndPoint(broadcast, 11000);
		}

        public void SendData(string data)
		{
			byte[] sendbuf = Encoding.ASCII.GetBytes(data);

			s.SendTo(sendbuf, ep);

		}
	}
}
