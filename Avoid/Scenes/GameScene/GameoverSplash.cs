using Avoid.Drawing.Common;
using Avoid.Drawing.UI;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Avoid.Scenes.GameScene
{
	public class GameoverSplash : IRenderable
	{
		private RectangleBackground darkening;
		private RectangleBackground bg;
		private Button scoreLabel;
		private Button highscoreLabel;
		private Button retryButton;
		private Button returnButton;
		private Bounds bounds;
		private int[] pixelCoords = new int[4];
		private App _app;
		
		public bool isHidden;
		


		public GameoverSplash(int score, App app, Action retry)
		{
			darkening = new RectangleBackground(new Bounds(1.1, 1.1, -1.1, -1.1));

			bounds = new Bounds(0.6, 0.5, -0.6, -0.5);
			darkening.Color = new Vector4(0, 0, 0, 0.9f);

			_app = app;

			bg = new RectangleBackground(bounds);
			bg.Color = new Vector4(1, 1, 1, 0.7f);

			scoreLabel = new Button(new Bounds(0.6, 0.5, -0.6, 0.4), "Score: " + score, () => { }, app);
			scoreLabel.colorHover = scoreLabel.colorIdle = new Vector4(1, 1, 1, 0);

			highscoreLabel = new Button(new Bounds(0.6, 0.4, -0.6, 0.25), "Highscore: " + score, () => { }, app);
			highscoreLabel.colorHover = highscoreLabel.colorIdle = new Vector4(1, 1, 1, 0);

			retryButton = new Button(new Bounds(0.59, -0.36, 0.01, -0.49), "Try Again", retry, app);
			retryButton.colorHover = new Vector4(1, 0.5f, 0, 1.1f);
			retryButton.colorIdle = new Vector4(1, 1, 1, 0.9f);
			retryButton.textSprite.textOpacity = 1f;

			returnButton = new Button(new Bounds(-0.01, -0.36, -0.59, -0.49), "Main Menu", () => { app.SetScene(new MainMenuScene()); }, app);
			returnButton.colorHover = new Vector4(0, 0.5f, 1, 1.1f);
			returnButton.colorIdle = new Vector4(1, 1, 1, 0.9f);
			returnButton.textSprite.textOpacity = 1f;
		}
		public void Load()
		{
			bg.Load();
			darkening.Load();
			UpdatePixelScale();

			retryButton.Load();
			retryButton.Update(_app.MouseState);
			scoreLabel.Load();
			scoreLabel.Update(_app.MouseState);
			highscoreLabel.Load();
			highscoreLabel.Update(_app.MouseState);
			returnButton.Load();
			returnButton.Update(_app.MouseState);

		}

		public void Render()
		{
			if (isHidden)
				return;
			darkening.Render();
			bg.Render();
			scoreLabel.Render();
			highscoreLabel.Render();
			retryButton.Render();
			returnButton.Render();
		}

		public void SetScore(int score, int highscore)
		{
			scoreLabel.textSprite.UpdateText("Score: " + score);
			if(score == highscore)
				highscoreLabel.textSprite.UpdateText("New Record!");
			else
			highscoreLabel.textSprite.UpdateText("Highscore: " + highscore);
		}

		public void Update(MouseState state)
		{
			scoreLabel.Update(state);
			highscoreLabel.Update(state);
			retryButton.Update(state);
			returnButton.Update(state);
		}

		private void UpdatePixelScale()
		{
			for (int i = 0; i < 4; i++)
				pixelCoords[i] = (int)((bounds[i] + 1) * _app.Size[i % 2]) / 2;
			pixelCoords[1] = _app.Size.Y - pixelCoords[1];
			pixelCoords[3] = _app.Size.Y - pixelCoords[3];

			bg.UpdateInnerResolution(new Vector2(pixelCoords[0] - pixelCoords[2], pixelCoords[3] - pixelCoords[1]));
			darkening.UpdateInnerResolution(new Vector2i(_app.Size.X, -_app.Size.Y));
		}
	}
}
