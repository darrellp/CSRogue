using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using CSRogue.GameControl;
using CSRogue.GameControl.Commands;
using CSRogue.Map_Generation;

namespace RogueWPF.UI
{
	class KeyboardHandler
	{
		private readonly Game _game;
		private readonly MainWindow _mainWindow;
		private static readonly Dictionary<Key, CommandType> Directions = new Dictionary<Key, CommandType>()
		    {
		        {Key.Left, CommandType.MoveLeft},
		        {Key.Right, CommandType.MoveRight},
		        {Key.Up, CommandType.MoveUp},
		        {Key.Down, CommandType.MoveDown},
		        {Key.Home, CommandType.MoveUpperLeft},
		        {Key.End, CommandType.MoveLowerLeft},
		        {Key.PageUp, CommandType.MoveUpperRight},
		        {Key.PageDown, CommandType.MoveLowerRight},
		        {Key.Clear, CommandType.StayPut},
		    };

		internal KeyboardHandler(Game game)
		{
			_game = game;
			_mainWindow = (MainWindow)Application.Current.MainWindow;
		}

		internal void DispatchOnKey(Key key)
		{
			IRogueCommand command = null;

			if (Directions.ContainsKey(key))
			{
				bool shouldRun = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
				command = new MovementCommand(Directions[key], shouldRun);
			}

			_mainWindow.MessageTextblock.Text = string.Empty;
			_game.EnqueueAndProcess(command);
		}
	}
}
