using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Avoid.Drawing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Avoid.Drawing.UI
{
	public class TextSprite : IRenderable
	{
		// Parameters for text texture
		private Bitmap bmp;
		private int width = 480;
		private int height = 50;

		// Text opacity
		public float textOpacity = 1f;

		// Renderables
		private Sprite sprite;

        public TextSprite(Bounds bounds, string text, Vector2i size)
		{
			width = Math.Abs(size.X);
			height = Math.Abs(size.Y);

			bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			Texture texture = Texture.LoadFromBitmap(bmp);
			Shader shader = new Shader(File.ReadAllText("Files/shaders/basic.vert"), File.ReadAllText("Files/shaders/brighttext.frag"));

			sprite = new Sprite(bounds, shader, texture);
			CreateTextTexture(text);

		}

		private void CreateTextTexture(string text)
		{
			PrivateFontCollection collection = new PrivateFontCollection();
			collection.AddFontFile(@"Files/font.ttf");
			FontFamily fontFamily = new FontFamily("Open Sans", collection);

			Font font = new Font(fontFamily, 28);
			
			var gfx = Graphics.FromImage(bmp);
			var brush = Brushes.White;

			gfx.TextRenderingHint = TextRenderingHint.AntiAlias;
			gfx.Clear(Color.Transparent);
			gfx.DrawString(text, font, brush, new PointF(0, 0));

			sprite.texture.UpdateData(bmp);
		}

		public void Render()
		{
			UpdateUniforms();

			sprite.Render();
		}

		private void UpdateUniforms()
		{
			sprite.shader.Use();
			var location = sprite.shader.GetUniformLocation("opacity");
			GL.Uniform1(location, 1f - textOpacity);
		}

		public void Load()
		{
			sprite.Load();
		}

		public void UpdateText(string text)
		{
			CreateTextTexture(text);
		}
	}
}
