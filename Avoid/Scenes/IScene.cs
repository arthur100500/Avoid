using Avoid.Drawing.Common;

namespace Avoid
{
	public interface IScene : IRenderable
	{
		public App Application{set;}
		public void Update();
		public string Name { get;}
		public void FixedUpdate();
		void Close();
	}
}
