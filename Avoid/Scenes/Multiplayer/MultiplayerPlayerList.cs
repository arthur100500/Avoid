using Avoid.Drawing.Common;
using Avoid.Drawing.UI;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avoid.Scenes.Multiplayer
{
	public class MultiplayerPlayerList : IRenderable
	{
		private float nodeHeight = 0.11f;
		private float nodeOffset = 0.01f;
		public List<Button> buttons = new List<Button>();
		Bounds bounds;
		App _app;
		RectangleBackground background;
		public MultiplayerPlayerList(Bounds b, List<string> names, App app)
		{
			_app = app;
			background = new RectangleBackground(b);
			background.Color = new Vector4(1f, 1f, 1f, 0.6f);
			bounds = b;
			for (int i = 0; i < names.Count; i++)
			{
				buttons.Add(new Button(new Bounds(b[0] - nodeOffset, b[1] - i * nodeHeight - nodeOffset, b[2] + nodeOffset, b[1] - (i + 1) * nodeHeight - 2 * nodeOffset), names[i], () => { }, app));
				buttons.Last().textSprite.fontSize = 20;
				buttons.Last().UpdateText(names[i]);
			}
			background.ReshapeWithCoords(bounds[0], bounds[1] - names.Count * nodeHeight + 2 * nodeOffset, bounds[2], bounds[3]);
		}

		public void RecreateButtons(List<string> names)
		{
			buttons.Clear();
			var b = bounds;
			for (int i = 0; i < names.Count; i++)
			{
				buttons.Add(new Button(new Bounds(b[0] - nodeOffset, b[1] - i * nodeHeight - nodeOffset, b[2] + nodeOffset, b[1] - (i + 1) * nodeHeight), names[i], () => { }, _app));
				buttons.Last().textSprite.fontSize = 20;
				buttons.Last().UpdateText(names[i]);
			}
			background.ReshapeWithCoords(-bounds[0], bounds[1] - names.Count * nodeHeight -nodeOffset, -bounds[2], bounds[1]);
			Load();
		}
		public void Render()
		{
			background.Render();
			UpdatePixelScale();
			foreach (var button in buttons)
			{
				button.Update(_app.MouseState);
				button.Render();
			}
		}

		public void Load()
		{
			background.Load();
			foreach (var button in buttons)
			{
				button.Load();

			}
		}

		private void UpdatePixelScale()
		{
			int[] pixelCoords = new int[4];
			for (int i = 0; i < 4; i++)
				pixelCoords[i] = (int)((bounds[i] + 1) * _app.Size[i % 2]) / 2;
			pixelCoords[1] = _app.Size.Y - pixelCoords[1];
			pixelCoords[3] = _app.Size.Y - pixelCoords[3];

			background.UpdateInnerResolution(new Vector2(pixelCoords[0] - pixelCoords[2], pixelCoords[3] - pixelCoords[1]));
		}

		public void MarkDeadPlayer(string name)
		{
			foreach (var button in buttons)
				if (button.textSprite.Text == name)
					button.colorIdle = button.colorHover = new Vector4(0.9f, 0.1f, 0.1f, 0.9f);
		}

		internal void MarkAllPlayersAlive()
		{
			foreach (var button in buttons)
				button.colorIdle = button.colorHover = new Vector4(1f, 1f, 1f, 0.9f);
		}
	}
}
