using Avoid.Server.GamePlay;
using System.Globalization;
using System.Net;
using System.Net.Sockets;

namespace Avoid.Server
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Avoid Server v.1.1");
			Console.WriteLine("");
			Console.WriteLine("Working on IP: " + GetLocalIPAddress());
			Console.WriteLine("Working on Port: " + "11000/11001");
			Console.WriteLine("");
			Console.WriteLine("Waiting for players to join");

			//try
			//{
			//CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
			new AvoidServer().Start();
			//}
			//catch (Exception ex)
			//{
			//	Console.WriteLine(ex.ToString());
			//	Console.ReadLine();
			//}
		}

		public static string GetLocalIPAddress()
		{
			var host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (var ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					return ip.ToString();
				}
			}
			throw new Exception("No network adapters with an IPv4 address in the system!");
		}
	}
}