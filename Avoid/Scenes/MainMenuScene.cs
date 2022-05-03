using Avoid.Drawing.UI;
using Avoid.Drawing.Common;
using OpenTK.Mathematics;
using Avoid.Gameplay;
using System.Drawing;
using Avoid.Scenes.GameScene;
using Avoid.Scenes.Multiplayer;

namespace Avoid.Scenes
{
	public class MainMenuScene : IScene
	{
		private App _app;
		private Cursor cursor;
		public App Application { set => _app = value; }

		public string Name => "Avoid. Don't touch the circles!";

		private Sprite logo;


		// Navigation buttons
		private Button singleplayerButton;
		private Button multiplayerButton;
		private Button scoreRecord;
		private Button mpName;
		private Button mpIP;

		public void Close()
		{
			
		}

		public void FixedUpdate()
		{
			
		}

		public void Load()
		{
			logo = new Sprite(new Bounds(0.8, 1, -0.8, 0.5), Shader.GenBasicShader(), Texture.LoadFromBitmap(new Bitmap("Files/textures/mmlogo.png")));
			logo.Load();

			Texture cursorT = Texture.LoadFromBitmap(new Bitmap("Files/textures/cursor.png"));
			cursor = new Cursor(cursorT, new Bounds(0.2, 0.2, -0.2, -0.2));
			cursor.Load();

			singleplayerButton = new Button(new Bounds(0.5, 0.5, -0.5, 0.35), "Play Alone", () => { _app.SetScene(new GameScene.GameScene()); }, _app);
			singleplayerButton.Load();

			singleplayerButton.colorIdle = new Vector4(1, 1, 1, 0.9f);
			singleplayerButton.colorHover = new Vector4(0, 1, 0.5f, 1f);

			multiplayerButton = new Button(new Bounds(0.5, 0.3, -0.5, 0.15), "Play with Friends", () => { _app.SetScene(new MultiplayerGameScene()); }, _app);
			multiplayerButton.Load();

			multiplayerButton.colorIdle = new Vector4(1, 1, 1, 0.9f);
			multiplayerButton.colorHover = new Vector4(0, 0.5f, 1, 1f);

			// info
			scoreRecord = new Button(new Bounds(0.5, 0.1, -0.5, -0.05), "", () => { }, _app);
			scoreRecord.Load();
			scoreRecord.colorIdle = new Vector4(1, 1, 1, 0.0f);
			scoreRecord.colorHover = new Vector4(0, 0.5f, 1, 0f);
			scoreRecord.textSprite.fontSize = 14;
			scoreRecord.textSprite.UpdateText("Your record: " + File.ReadAllText("Files/highscores.txt"));

			mpName = new Button(new Bounds(0.5, 0, -0.5, -0.25), "", () => { }, _app);
			mpName.Load();
			mpName.colorIdle = new Vector4(1, 1, 1, 0.0f);
			mpName.colorHover = new Vector4(0, 0.5f, 1, 0f);
			mpName.textSprite.fontSize = 14;
			mpName.textSprite.UpdateText("Your nickname: " + File.ReadAllText("Files/mp.txt").Split(Environment.NewLine)[0]);

			mpIP = new Button(new Bounds(0.5, -0.1, -0.5, -0.45), "", () => { }, _app);
			mpIP.Load();
			mpIP.colorIdle = new Vector4(1, 1, 1, 0.0f);
			mpIP.colorHover = new Vector4(0, 0.5f, 1, 0f);
			mpIP.textSprite.fontSize = 14;
			mpIP.textSprite.UpdateText("Selected Server IP: " + File.ReadAllText("Files/mp.txt").Split(Environment.NewLine)[1]);
		}

		public void Render()
		{
			logo.Render();
			singleplayerButton.Render();
			multiplayerButton.Render();
			cursor.Render();

			scoreRecord.Render();
			mpIP.Render();
			mpName.Render();
		}

		public void Update()
		{

			Vector2 cpos = -(_app.MouseState.Position / _app.Size) * 2 + new Vector2(1);
			cursor.Update(cpos);

			singleplayerButton.Update(_app.MouseState);
			multiplayerButton.Update(_app.MouseState);
			scoreRecord.Update(_app.MouseState);
			mpIP.Update(_app.MouseState);
			mpName.Update(_app.MouseState);
		}
	}
}
