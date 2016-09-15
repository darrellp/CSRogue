using System.Collections.Generic;
using CSRogue.GameControl.Commands;
using CSRogue.Interfaces;

namespace CSRogue.GameControl
{
	class CommandQueue
	{
		private readonly Queue<IRogueCommand> _commands = new Queue<IRogueCommand>();
		private readonly Game _game;

		internal CommandQueue(Game game)
		{
			_game = game;
		}

		internal void AddCommand(IRogueCommand command)
		{
			_commands.Enqueue(command);
		}

		internal void ProcessQueue()
		{
			while (_commands.Count != 0)
			{
                _commands.Dequeue()?.Execute(_game);
			}
		}
	}
}
