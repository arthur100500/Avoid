using Avoid.Drawing.Common;
using Avoid.Gameplay;
using Avoid.Gameplay.Obstacle;
using Avoid.Net;
using Avoid.Scenes.Multiplayer;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing;
using Cursor = Avoid.Gameplay.Cursor;

namespace Avoid.Scenes.GameScene
{
	public class MultiplayerGameScene : IScene
	{
		// Threads to stop
		private List<Thread> threads = new List<Thread>();

		// Multiplayer stuff
		private string yourName = File.ReadAllText("Files/mp.txt").Split(Environment.NewLine)[0];
		private List<MultiplayerPlayerCursor> otherPlayers = new List<MultiplayerPlayerCursor>();
		private UDPListener listener;
		private UDPSender sender;

		// Gameplay
		private Cursor cursor;
		private List<IObstacle> obstacles = new List<IObstacle>();
		private float health = 1f;

		// UI
		private Healthbar hb;
		private MultiplayerPlayerList mpl;

		// Update counters
		private int updateCount = 0;
		private int score = 0;

		// IScene inheireted
		public string Name => "Avoid. Multiplayer";
		private App _app;
		public App Application { set => _app = value; }

		// State
		private MultiplayerGameState state;
		private MultiplayerGameState State
		{
			get { return state; }
			set { state = value; _app.Title = Name + " " + StateToString(value); }
		}
		private MultiplayerGameoverSplash splash;

		// Thread synching
		private string lastMessageRecieved;
		private bool newMessage;
		private string newCircleMessage;
		private bool gameoverRequest = false;

		private static string StateToString(MultiplayerGameState state)
		{
			switch (state)
			{
				case MultiplayerGameState.WaitingOtherPlayers:
					return "Waiting for other players";
				case MultiplayerGameState.GameOver:
					return "Game Over";
				case MultiplayerGameState.Running:
					return "Running";
				default:
					return "Undefined";
			}
		}
		public void FixedUpdate()
		{
			Vector2 cpos = -(_app.MouseState.Position / _app.Size) * 2 + new Vector2(1);
			// Update gameplay logic
			foreach (var v in obstacles)
				v.Update();
			foreach (var v in obstacles)
				if (v.IsOutOfBounds())
				{
					obstacles.Remove(v);
					break;
				}
			foreach (var v in obstacles)
				if (State == MultiplayerGameState.Running && v.CheckCollision(cpos))
				{
					((Circle)v).CurrentColor = new Vector4(1, 0, 0, 1);
					health -= 0.004f / (((Circle)v).width);
				}
				else
					((Circle)v).CurrentColor = ((Circle)v).Color;

			// Update other player cursors
			var everyoneDead = true;
			foreach (var p in otherPlayers)
			{
				p.Position += p.Speed;
				p.SetToPosition(p.Position);

				if (!p.IsDead)
					everyoneDead = false;
			}

			if (gameoverRequest)
				Gameover();

			if (State == MultiplayerGameState.Running)
				health += 0.001f;

			health = MathF.Min(health, 1);
			if (health < 0)
				health = 0;

			updateCount++;

			hb.SetHealth(health);
			if (State == MultiplayerGameState.Running)
				sender.SendData("PD " + yourName + " " + score + " " + health + " " + cursor.Position + " " + (cursor.Position - cursor.PositionPrev));

			// PARSE ALL MESSAGES
			if (newMessage)
			{
				try
				{
					newMessage = false;
					if (newCircleMessage != null)
					{
						RecieveCircle(newCircleMessage.Replace("AC ", "").Replace("; ", ";"));
						newCircleMessage = null;
					}
					if (lastMessageRecieved.StartsWith("UL "))
						RefillPlayerList(lastMessageRecieved.Replace("UL ", "").Replace("; ", ";"));
					if (lastMessageRecieved.StartsWith("UP "))
						RefreshOtherPlayer(lastMessageRecieved.Replace("UP ", "").Replace("; ", ";"));
				}
				catch (Exception ex) { Console.WriteLine("Something went wrong!\n" + ex.ToString()); }
			}
		}

		private void RefreshOtherPlayer(string v)
		{
			foreach (var p in otherPlayers)
				if (p.Name == v.Split(" ")[0])
				{
					p.Position = FromString2(v.Split(" ")[1]);
					p.Speed = FromString2(v.Split(" ")[2]);
					if (v.Split(" ")[3].Trim() == "0")
					{
						p.IsDead = true;
						if (p.IsDead && !p.WasDead)
						{
							p.WasDead = true;
							mpl.MarkDeadPlayer(p.Name);
						}
					}
				}
		}

		public void Load()
		{
			// Load UI
			Texture cursorT = Texture.LoadFromBitmap(new Bitmap("Files/textures/cursor.png"));
			cursor = new Cursor(cursorT, new Bounds(0.2, 0.2, -0.2, -0.2));
			cursor.Load();

			splash = new MultiplayerGameoverSplash("Idk you should not see this at all", _app);
			splash.Load();
			splash.isHidden = true;

			hb = new Healthbar(new Bounds(0.990, 0.990, 0.05, 0.92), _app);
			hb.Load();

			mpl = new MultiplayerPlayerList(new Bounds(-0.45, 1, -1.00, 0), new List<string>(), _app);
			mpl.RecreateButtons(new List<string>());

			// Init Sockets
			sender = new UDPSender();
			listener = new UDPListener();

			// Start listening thread
			threads.Add(new Thread(() => StartRecieving()));
			threads.Last().Start();

			// Send joining request
			AddYourself(yourName);

			State = MultiplayerGameState.WaitingOtherPlayers;
		}

		private void StartRecieving()
		{
			while (true)
			{
				lastMessageRecieved = listener.Recieve();
				newMessage = true;
				// New circle message
				if (lastMessageRecieved.StartsWith("AC"))
					newCircleMessage = lastMessageRecieved;
				// Gameover
				if (lastMessageRecieved == "GO")
					gameoverRequest = true;

				// Game begin
				if (lastMessageRecieved == "GB")
					Retry();

				Console.WriteLine(lastMessageRecieved);
			}
		}

		public void Render()
		{
			// GAME
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
			foreach (var v in obstacles)
				v.Render();
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

			// UI
			hb.Render();
			mpl.Render();
			splash?.Render();

			// Other players
			foreach (var c in otherPlayers)
				c.Render();

			// Cursor
			cursor.Render();
		}

		public void Update()
		{
			Vector2 cpos = -(_app.MouseState.Position / _app.Size) * 2 + new Vector2(1);
			cursor.Update(cpos);
			splash?.Update(_app.MouseState);

			// Send start game request (menu to be added)
			if (_app.KeyboardState.IsKeyDown(Keys.H))
				SendStartMessage();

		}

		private void SendStartMessage() => sender.SendData("START");
		private void AddYourself(string name) => sender.SendData("ADDPLAYER " + name);

		private void Gameover()
		{
			State = MultiplayerGameState.GameOver;
			gameoverRequest = false;

			var message = "You've won ^^";
			foreach (var p in otherPlayers)
			{
				if (!p.IsDead)
					message = "You've lost :(";
			}

			splash.SetResult(message);
			splash.isHidden = false;
		}

		private void Retry()
		{
			splash.isHidden = true;

			foreach (var p in otherPlayers)
			{
				p.IsDead = p.WasDead = false;
			}
			health = 1;
			State = MultiplayerGameState.Running;
			score = 0;
		}

		private void RecieveCircle(string circledata)
		{
			string[] lines = circledata.Split(" ");

			obstacles.Add(new Circle(float.Parse(lines[0])));
			obstacles.Last().Position = FromString2(lines[1]);
			obstacles.Last().Speed = FromString2(lines[2]);
			((Circle)obstacles.Last()).CurrentColor = FromString4(lines[3]);
			obstacles.Last().Load();
			obstacles.Last().Update();

			if (State == MultiplayerGameState.GameOver || State == MultiplayerGameState.WaitingOtherPlayers)
				State = MultiplayerGameState.Running;
		}

		private Vector2 FromString2(string info)
		{
			var data = info.Replace("(", "").Replace(")", "").Split(";");
			return new Vector2(float.Parse(data[0]), float.Parse(data[1]));
		}

		private Vector4 FromString4(string info)
		{
			var data = info.Replace("(", "").Replace(")", "").Split(";");
			return new Vector4(float.Parse(data[0]), float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
		}

		private void RefillPlayerList(string cmds)
		{
			Texture cursorT = Texture.LoadFromBitmap(new Bitmap("Files/textures/cursor.png"));

			otherPlayers.Clear();
			foreach (var cmd in cmds.Split("\n"))
				try
				{
					if (cmd.Split(" ")[0] != yourName)
						otherPlayers.Add(new MultiplayerPlayerCursor(cursorT, cmd.Split(" ")[0], FromString4(cmd.Split(" ")[1]), _app));
				}
				catch { }

			var namelist = new List<string>();
			foreach (var c in otherPlayers)
			{
				namelist.Add(c.Name);
				c.Load();
			}

			mpl.RecreateButtons(new List<string>() { yourName }.Concat(namelist).ToList());
		}

		public void Close()
		{
			// Here should be code to stop the thread but idk how
		}
	}
}
