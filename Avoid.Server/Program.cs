using Avoid.Server.GamePlay;
using System.Globalization;

namespace Avoid.Server
{
	public class Program
	{
		public static void Main(string[] args)
		{
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
	}
}