using Avoid.Drawing.Common;
using OpenTK.Mathematics;

namespace Avoid.Gameplay
{
	public class Cursor : IRenderable
	{
		private Sprite sprite;
		private float width;
		private float height;

		public Cursor(Texture texture, Bounds b)
		{
			sprite = new Sprite(b, Shader.GenBasicShader(), texture);
			width = MathF.Abs(b[0] - b[2]) / 2;
			height = MathF.Abs(b[1] - b[3]) / 2;
		} 

		public void Update(Vector2 cursorPosition)
		{
			sprite.ReshapeWithCoords(cursorPosition.X + width, cursorPosition.Y + height, cursorPosition.X - width, cursorPosition.Y - height);
		}

		public void Load()
		{
			sprite.Load();
		}

		public void Render()
		{
			sprite.Render();
		}
	}
}
