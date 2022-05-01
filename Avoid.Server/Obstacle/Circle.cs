using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avoid.Gameplay.Obstacle
{
	public class Circle
	{
		public float width;
		private static Random random = new Random();
		public Vector2 Position { get; set; }
		public Vector2 Speed { get; set; }
		public Vector4 Color;
		public Vector4 CurrentColor { get; set; }

		public Circle(float width)
		{
			this.width = width;
			Color = new Vector4(0.1f, 0.5f, 0.3f, 0.9f);
		}



		public bool CheckCollision(Vector2 position)
		{
			return ((position - Position).LengthSquared < width * width / 4);
		}

		public static Circle GenRandom()
		{
			var Circle = new Circle(random.NextSingle());

			Circle.Color = GenColor();

			// Choose point in screen
			var point = new Vector2(2 * random.NextSingle() - 1, 2 * random.NextSingle() - 1);

			// Choose speed vector
			Circle.Speed = new Vector2((2 * random.NextSingle() - 1) * 0.005f / Circle.width, (2 * random.NextSingle() - 1) * 0.001f / Circle.width);

			Circle.Position = (point - Circle.Speed).Normalized() * -2;

			return Circle;
		}

		public bool IsOutOfBounds()
		{
			return (Position.LengthSquared > 5);
		}

		private static Vector4 GenColor()
		{
			Vector4 color = new Vector4(random.NextSingle() * 0.2f, random.NextSingle(), random.NextSingle(), 1);
			while (color[2] < 0.5f && color[1] < 0.5f)
				color = new Vector4(random.NextSingle() * 0.2f, random.NextSingle(), random.NextSingle(), 1);
			return color;
		}
	}
}
