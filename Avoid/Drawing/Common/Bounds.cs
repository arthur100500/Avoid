using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avoid.Drawing.Common
{
	public class Bounds
	{
		private float[] coordinates;
		public Bounds(float x1, float y1, float x2, float y2)
		{
			coordinates = new float[] { x1, y1, x2, y2 };
		}

		public Bounds(double x1, double y1, double x2, double y2)
		{
			coordinates = new float[] { (float)x1, (float)y1, (float)x2, (float)y2 };
		}
		public float this[int index]
		{
			get
			{
				return coordinates[index];
			}
			set
			{
				coordinates[index] = value;
			}
		}
	}
}
