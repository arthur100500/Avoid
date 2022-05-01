using Avoid.Drawing.Common;
using Avoid.Drawing.UI;
using Avoid.Gameplay;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avoid.Scenes.Multiplayer
{
	public class PlayerCursor : IRenderable
	{
		public string Name;
		public float Health;

		private Cursor cursor;
		private Button nameLabel;
		private float nlW = 0.6f;
		private float nlH = 0.4f;
		private float vertoffset = 0.001f;

		public Vector2 Position;
		public Vector2 Speed;

		public PlayerCursor(Texture t, string name, Vector4 cursorColor, App app)
		{
			Name = name;
			cursor = new Cursor(t, new Bounds(0.2, 0.2, -0.2, -0.2));
			nameLabel = new Button(new Bounds(0.3, 0.1, -0.1, -0.3), name, () => { }, app);
			nameLabel.textSprite.fontSize = 15;
			nameLabel.UpdateText(name);
			cursor.Color = cursorColor;
		}
		public void Load()
		{
			cursor.Load();
			nameLabel.Load();
		}

		public void Render()
		{
			cursor.Render();
			nameLabel.Render();
		}

		public void SetToPosition(Vector2 pos)
		{
			cursor.Update(pos);
			nameLabel.textSprite.sprite.ReshapeWithCoords(pos.X - nlW / 2, pos.Y + nlH/ 2 + vertoffset, pos.X + nlW / 2, pos.Y - nlH / 2 + vertoffset);
		}
	}
}
