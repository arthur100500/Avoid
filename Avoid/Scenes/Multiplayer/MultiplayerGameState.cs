using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avoid.Scenes.GameScene
{
	public enum MultiplayerGameState
	{
		Running = 0,
		Stopped = 1,
		GameOver = 2,
		WaitingOtherPlayers = 3
	}
}
