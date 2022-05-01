using Avoid.Drawing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Avoid.Gameplay
{
	public class Cursor : IRenderable
	{
		private Sprite sprite;
		private float width;
		private float height;
		public Vector2 Position { get; private set; }
		public Vector2 PositionPrev { get; private set; }
		public Vector4 Color { get; set; }

		public Cursor(Texture texture, Bounds b)
		{
			sprite = new Sprite(b, new Shader(File.ReadAllText("Files/shaders/basic.vert"), File.ReadAllText("Files/shaders/coloredbasic.frag")), texture);
			width = MathF.Abs(b[0] - b[2]) / 2;
			height = MathF.Abs(b[1] - b[3]) / 2;
			Color = new Vector4(1, 0.5f, 0.1f, 1);
		} 

		public void Update(Vector2 cursorPosition)
		{
			sprite.ReshapeWithCoords(cursorPosition.X + width, cursorPosition.Y + height, cursorPosition.X - width, cursorPosition.Y - height);
			Position = cursorPosition;
			PositionPrev = Position;
		}

		public void Load()
		{
			sprite.Load();
		}

		public void Render()
		{
			sprite.shader.Use();
			var loc = sprite.shader.GetUniformLocation("color");
			GL.Uniform4(loc, Color);
			sprite.Render();
		}
	}
}
