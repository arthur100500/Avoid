using System;

namespace Avoid
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			try
			{
				App.Create().Run();
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.ToString());
				Console.ReadLine();
			}
		}
	}
}