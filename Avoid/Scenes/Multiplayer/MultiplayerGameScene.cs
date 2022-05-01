using Avoid.Drawing.Common;
using Avoid.Drawing.UI;
using Avoid.Gameplay;
using Avoid.Gameplay.Obstacle;
using Avoid.Net;
using Avoid.Scenes.Multiplayer;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cursor = Avoid.Gameplay.Cursor;

namespace Avoid.Scenes.GameScene
{
	public class MultiplayerGameScene : IScene
	{
		string yourName = File.ReadAllText("Files/mp.txt").Split(Environment.NewLine)[0];
		List<PlayerCursor> otherPlayers = new List<PlayerCursor>();

		UDPListener listener;
		UDPSender sender;

		private Cursor cursor;
		private List<IObstacle> obstacles = new List<IObstacle>();

		int updateCount = 0;
		int score = 0;
		float health = 1f;
		Healthbar hb;

		Button scoreLabel;

		public string Name => "Avoid. Game";

		private App _app;
		public App Application { set => _app = value; }

		// State
		GameState state = GameState.Running;
		GameoverSplash splash;

		float deathSpeedDecrease = 1;
		private string lastMessageRecieved;
		private bool newMessage;

		public void FixedUpdate()
		{
			
			Vector2 cpos = -(_app.MouseState.Position / _app.Size) * 2 + new Vector2(1);

			foreach (var v in obstacles)
			{

				if (state == GameState.GameOver)
				{
					v.Speed *= deathSpeedDecrease;
				}
				v.Update();
			}

			foreach (var v in obstacles)
				if (v.IsOutOfBounds())
				{
					obstacles.Remove(v);
					scoreLabel.UpdateText("Score: " + score);
					break;
				}

			foreach (var v in obstacles)
				if (state == GameState.Running && v.CheckCollision(cpos))
				{

					((Circle)v).CurrentColor = new Vector4(1, 0, 0, 1);
					health -= 0.004f / (((Circle)v).width);
				}
				else
					((Circle)v).CurrentColor = ((Circle)v).Color;

			foreach(var p in otherPlayers)
			{
				p.Position += p.Speed;
				p.SetToPosition(p.Position);
			}

			if (updateCount % 10 == 0 && state == GameState.Running)
			{

				score += obstacles.Count;
			}

			if (state == GameState.Running)
				health += 0.001f;
			health = MathF.Min(health, 1);
			if (health < 0)
			{
				if (state != GameState.GameOver)
					Gameover();
				health = 0;
			}

			updateCount++;

			hb.SetHealth(health);
			sender.SendData("PD " + yourName + " " + score + " " + health + " " + cursor.Position + " " + (cursor.Position - cursor.PositionPrev));

			// PARSE ALL MESSAGES
			if (newMessage)
			{
				try
				{
					newMessage = false;
					if (lastMessageRecieved.StartsWith("AC "))
						RecieveCircle(lastMessageRecieved.Replace("AC ", "").Replace("; ", ";"));
					if (lastMessageRecieved.StartsWith("UL "))
						RefillPlayerList(lastMessageRecieved.Replace("UL ", "").Replace("; ", ";"));
					if (lastMessageRecieved.StartsWith("UP "))
						RefreshOtherPlayer(lastMessageRecieved.Replace("UP ", "").Replace("; ", ";"));

					Console.WriteLine("Successfully done");
				}
				catch (Exception ex) { Console.WriteLine("Something went wrong!\n" + ex.ToString()); }
			}
		}

		private void RefreshOtherPlayer(string v)
		{
			foreach (var p in otherPlayers)
			{
				if (p.Name == v.Split(" ")[0])
				{
					p.Position = FromString2(v.Split(" ")[1]);
					p.Speed = FromString2(v.Split(" ")[2]);
				}
			}
		}

		public void Load()
		{
			// LOAD
			Texture cursorT = Texture.LoadFromBitmap(new Bitmap("Files/textures/cursor.png"));
			cursor = new Cursor(cursorT, new Bounds(0.2, 0.2, -0.2, -0.2));
			cursor.Load();

			scoreLabel = new Button(new Bounds(-0.05, 1, -0.95, 0.85), "Score: 0", () => { }, _app);
			scoreLabel.colorIdle = new Vector4(1, 1, 1, 0.0f);
			scoreLabel.colorHover = new Vector4(1, 1, 1, 0.0f);
			scoreLabel.textSprite.textOpacity = 1f;
			scoreLabel.Load();

			hb = new Healthbar(new Bounds(0.95, 0.95, 0.05, 0.87), _app);

			hb.Load();

			// Net
			sender = new UDPSender();
			listener = new UDPListener();
			// Start listening thread
			new Thread(() => StartRecieving()).Start();

			AddYourself(yourName);
		}

		private void StartRecieving()
		{
			while (true)
			{
				lastMessageRecieved = listener.Recieve();
				newMessage = true;

				Console.WriteLine(lastMessageRecieved);
			}
		}

		private void AddYourself(string name)
		{
			sender.SendData("ADDPLAYER " + name);
		}

		public void Render()
		{
			// GAME
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);

			foreach (var v in obstacles)
				v.Render();

			// UI
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
			scoreLabel.Render();
			hb.Render();

			splash?.Render();

			// Other players
			foreach (var c in otherPlayers)
			{
				c.Render();
			}

			// Cursor
			cursor.Render();
		}

		public void Update()
		{
			Vector2 cpos = -(_app.MouseState.Position / _app.Size) * 2 + new Vector2(1);

			cursor.Update(cpos);

			scoreLabel.Update(_app.MouseState);

			splash?.Update(_app.MouseState);

			// START GAME
			if (_app.KeyboardState.IsKeyDown(Keys.H))
			{
				SendStartMessage();
			}
		}

		private void SendStartMessage()
		{
			sender.SendData("START");
		}

		private void Gameover()
		{
			state = GameState.GameOver;
			if (splash == null)
			{
				splash = new GameoverSplash(score, _app, Retry);
				splash.Load();
			}
			// Highscore
			int hscore = int.Parse(File.ReadAllText("Files/highscores.txt"));
			if (hscore < score)
			{
				File.WriteAllText("Files/highscores.txt", score.ToString());
			}
			splash.SetScore(score, Math.Max(score, hscore));
			splash.isHidden = false;
		}

		private void Retry()
		{
			splash.isHidden = true;

			//obstacles.Clear();
			health = 1;
			state = GameState.Running;
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
						otherPlayers.Add(new PlayerCursor(cursorT, cmd.Split(" ")[0], FromString4(cmd.Split(" ")[1]), _app));
				}
				catch (Exception ex) { }

			foreach (var c in otherPlayers)
			{
				c.Load();
			}
		}
	}
}
