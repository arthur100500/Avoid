using Avoid.Gameplay.Obstacle;
using Avoid.Server.Net;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Avoid.Server.GamePlay
{
	public class AvoidServer
	{
		// NET STUFF
		UDPListener listener;
		UDPSender sender;

		string lastMessageRecieved;
		bool newMessage;

		List<Player> players = new List<Player>();
		private Circle lastGennedCircle;
		private ulong fc = 0;
		private ulong fslp = 0;

		public AvoidServer()
		{
			listener = new UDPListener();
			sender = new UDPSender();
		}

		public void BeforeStart()
		{
			Console.WriteLine("\n\nWaiting for players");
			while (true)
				if (newMessage)
				{
					if (lastMessageRecieved.Contains("ADDPLAYER "))
						RegisterPlyer(lastMessageRecieved.Replace("ADDPLAYER ", ""));
					newMessage = false;
					if (lastMessageRecieved.Contains("START"))
						OnStart();
				}
		}

		private void StartRecieving()
		{
			while (true)
			{
				lastMessageRecieved = listener.Recieve();
				newMessage = true;
			}
		}

		public void OnStart()
		{
			Console.WriteLine("\n\nGame started");
			// Send data to all players that the game has begun
			foreach (var p in players)
				sender.SendData(p.CallBackIP, "GB");

			// While someone is alive
			while (true)
			{
				// Send circle every 0.1 second
				if (lastGennedCircle != null)
				{
					fslp++;
					if (fslp > 100)
					{
						fslp = 0;
						Console.WriteLine("No packages came for 100 iterations. Restarting the server.");
						Restart();
					}
					foreach (var p in players)
					{
						sender.SendData(p.CallBackIP, "AC " + lastGennedCircle.width + " " + lastGennedCircle.Position + " " + lastGennedCircle.Speed + " " + lastGennedCircle.Color);
					}
					lastGennedCircle = null;

					// Check if game shall continue
					var winner = DetectWinCondition();
					if (winner != null)
					{
						// End and restart game
						Console.WriteLine(winner);
						EndAndRestartGame();
					}
				}

				// Update playerdata command
				if (newMessage)
				{

					if (lastMessageRecieved.Contains("PD "))
					{
						UpdatePlayerData(lastMessageRecieved.Replace("PD ", "").Replace("; ", ";"));
						fslp = 0;
					}

					newMessage = false;

					// Send data about other players every 10 loops
					if (fc++ % 1 == 0)
					{
						foreach (var p in players)
						{
							foreach (var t in players)
								if (p.Name != t.Name)
									sender.SendData(p.CallBackIP, "UP " + t.Name + " " + t.CursorPosition + " " + t.CursorSpeed + " " + t.health);
						}
					}
				}
			}
		}

		private void EndAndRestartGame()
		{
			foreach (var p in players)
				p.health = 1;
			foreach (var p in players)
				sender.SendData(p.CallBackIP, "GO");
			BeforeStart();
		}

		private void Restart()
		{
			players.Clear();
			BeforeStart();
		}

		private void UpdatePlayerData(string arg)
		{
			for (int i = 0; i < players.Count; i++)
			{
				//sender.SendData("PD " + yourName + " " + score + " " + health + " " + cursor.Position + " " + cursor.PositionPrev);
				if (players[i].Name == arg.Split(" : ")[1].Split(" ")[0])
				{
					players[i].CursorPosition = FromString2(arg.Split(" : ")[1].Split(" ")[3]);
					players[i].CursorSpeed = FromString2(arg.Split(" : ")[1].Split(" ")[4]);
					players[i].score = int.Parse(arg.Split(" : ")[1].Split(" ")[1]);
					players[i].health = float.Parse(arg.Split(" : ")[1].Split(" ")[2]);
				}
			}
		}

		private string DetectWinCondition()
		{
			int alivePlayers = 0;
			string winner = null;
			foreach (var p in players)
			{
				if (p.health > 0.000001f)
				{
					winner = p.Name;
					alivePlayers++;
				}

			}

			if (alivePlayers <= 1)
				return "WON: " + winner;
			return null;
		}

		public void Start()
		{
			// Setup listening thread
			new Thread(() => StartRecieving()).Start();
			// Setup circle generating thread
			new Thread(() => StartGenningCircles()).Start();
			// Start server
			BeforeStart();
		}

		private void StartGenningCircles()
		{
			while (true)
			{
				Thread.Sleep(40);
				lastGennedCircle = Circle.GenRandom();
			}
		}

		public void RegisterPlyer(string message)
		{
			foreach (var p in players)
				if (message.Split(" : ")[1] == p.Name)
					sender.SendData(new IPEndPoint(IPAddress.Parse(message.Split(" : ")[0].Split(":")[0]), 11001), "Player with this nickname is already playing");
			players.Add(new Player(message.Split(" : ")[1], new IPEndPoint(IPAddress.Parse(message.Split(" : ")[0].Split(":")[0]), 11001)));
			Console.WriteLine("Added player: " + players.Last().Name);
			// some form of callback
			sender.SendData(players.Last().CallBackIP, "Hello! " + players.Last().Name);

			SendPlayerList();
		}

		public void SendPlayerList()
		{
			foreach (var p in players)
			{
				var s = "UL ";
				foreach (var t in players)
					s += t.Name + " " + t.Color + "\n";
				sender.SendData(p.CallBackIP, s);
			}
		}
		private Vector2 FromString2(string info)
		{
			var data = info.Replace("(", "").Replace(")", "").Split(";");
			return new Vector2(float.Parse(data[0]), float.Parse(data[1]));
		}
	}
}
