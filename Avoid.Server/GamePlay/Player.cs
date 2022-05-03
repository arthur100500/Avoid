using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Avoid.Server.GamePlay
{
	public class Player
	{
		public string Name { get; private set; }
		public Vector4 Color { get; private set; }
		public IPEndPoint CallBackIP { get; private set; }
		public int score;
		public float health;
		public Vector2 CursorPosition;
		public Vector2 CursorSpeed;
		public ulong LastFrameUpdated;

		public Player(string name, IPEndPoint ip)
		{
			CallBackIP = ip;
			Random random = new Random();
			Name = name;
			Color = new Vector4(random.NextSingle(), random.NextSingle(), random.NextSingle(), 1f);
		}
	}
}
