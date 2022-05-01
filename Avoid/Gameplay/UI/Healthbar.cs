using Avoid.Drawing.Common;
using Avoid.Drawing.UI;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avoid.Gameplay
{
	public class Healthbar : IRenderable
	{
		RectangleBackground bg;
		RectangleBackground bar;
		int[] pixelCoords;
		Bounds bounds;
		App _app;
		float currenthealth;

		public Healthbar(Bounds b, App app)
		{
			_app = app;
			bounds = b;
			bg = new RectangleBackground(b);
			bg.Color = new Vector4(1, 1, 1, 0.7f);
			bar = new RectangleBackground(b);
			bar.Color = new Vector4(1, 0, 0, 1.4f);
			pixelCoords = new int[4];
		}
		public void Load()
		{
			bg.Load();
			bar.Load();
		}

		public void Render()
		{
			bg.Render();
			bar.Render();

			UpdatePixelScale();
		}

		private void UpdatePixelScale()
		{
			for (int i = 0; i < 4; i++)
				pixelCoords[i] = (int)((bounds[i] + 1) * _app.Size[i % 2]) / 2;
			pixelCoords[1] = _app.Size.Y - pixelCoords[1];
			pixelCoords[3] = _app.Size.Y - pixelCoords[3];

			bg.UpdateInnerResolution(new Vector2(pixelCoords[0] - pixelCoords[2], pixelCoords[3] - pixelCoords[1]));
			bar.UpdateInnerResolution(new Vector2((pixelCoords[0] - pixelCoords[2]) * currenthealth, pixelCoords[3] - pixelCoords[1]));
		}

		public void SetHealth(float h)
		{
			bar.ReshapeWithCoords(-bounds[0] + (bounds[0] - bounds[2]) * (1 - h), bounds[1], -bounds[2], bounds[3]);
			currenthealth = h;
		}
	}
}
