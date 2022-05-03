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
				File.WriteAllText("ErrorLog-" + DateTime.Now.ToShortDateString + ".txt", ex.ToString());
				Console.ReadLine();
			}
		}
	}
}