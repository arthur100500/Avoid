using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

using Avoid.Drawing.UI;
using Avoid.Drawing.Common;
using Avoid.Gameplay;
using Avoid.Gameplay.Obstacle;
using Avoid.Scenes;
using Avoid.Scenes.GameScene;

namespace Avoid
{
	public class App : GameWindow
	{
		private float oTime;
		IScene scene = new GameScene();
		private App(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
		{
			// Enable things for correct texture opacity handling
			GL.Enable(EnableCap.Texture2D);
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);


			// Window works ok if it is not fixed, but text can become bad if resized
			WindowBorder = WindowBorder.Resizable;
			CursorVisible = false;
			VSync = VSyncMode.Off;
		}

		public static App Create()
		{
			var nativeWindowSettings = new NativeWindowSettings()
			{
				Size = new Vector2i(800, 800),
				Title = "Avoid",
				// This is needed to run on macos
				Flags = ContextFlags.ForwardCompatible,
			};

			App wnd = new App(GameWindowSettings.Default, nativeWindowSettings);
			return wnd;
		}


		protected override void OnLoad()
		{
			base.OnLoad();
			GL.ClearColor(0f, 0f, 0f, 1f);
			scene.Application = this;
			scene.Load();
			Title = scene.Name;
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			base.OnRenderFrame(e);
			GL.Clear(ClearBufferMask.ColorBufferBit);

			scene.Render();
			
			SwapBuffers();
		}


		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			base.OnUpdateFrame(e);

			oTime += (float)e.Time;

			scene.Update();

			while (oTime > 0.004)
			{
				FixedUpdate();
				oTime -= 0.004f;
			}
		}

		private void FixedUpdate()
		{
			scene.FixedUpdate();
		}

		protected override void OnResize(ResizeEventArgs e)
		{
			base.OnResize(e);

			GL.Viewport(0, 0, Size.X, Size.Y);
		}
	}
}
