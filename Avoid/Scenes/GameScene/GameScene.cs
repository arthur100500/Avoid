using Avoid.Drawing.Common;
using Avoid.Drawing.UI;
using Avoid.Gameplay;
using Avoid.Gameplay.Obstacle;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avoid.Scenes.GameScene
{
	public class GameScene : IScene
	{
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

		float deathSpeedDecrease = 0.992f;
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

			if (updateCount % 10 == 0 && state == GameState.Running)
			{
				obstacles.Add(Circle.GenRandom());
				obstacles.Last().Load();


				score += obstacles.Count;
			}

			foreach (var v in obstacles)
				if (state == GameState.Running && v.CheckCollision(cpos))
				{

					((Circle)v).CurrentColor = new Vector4(1, 0, 0, 1);
					health -= 0.004f / (((Circle)v).width);
				}
				else
					((Circle)v).CurrentColor = ((Circle)v).Color;

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

			// Cursor
			cursor.Render();
		}

		public void Update()
		{
			Vector2 cpos = -(_app.MouseState.Position / _app.Size) * 2 + new Vector2(1);
			cursor.Update(cpos);

			scoreLabel.Update(_app.MouseState);

			splash?.Update(_app.MouseState);
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

			obstacles.Clear();
			health = 1;
			state = GameState.Running;
			score = 0;
		}
	}
}
