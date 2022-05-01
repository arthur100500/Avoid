using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avoid.Gameplay.Obstacle
{
	public interface IObstacle : Drawing.Common.IRenderable
	{
		public Vector2 Position { get; set; }
		public Vector2 Speed { get; set; }
		public bool CheckCollision(Vector2 position);
		public bool IsOutOfBounds();
		public void Update();
	}
}
