using System;
using CSRogue.RogueEventArgs;
using CSRogue.GameControl.Commands;
using CSRogue.Map_Generation;
using CSRogue.Utilities;

namespace CSRogue.GameControl
{
	enum EventType
	{
		CreatureMove,
		NewLevel,
		Attack
	}

	public class Game
	{
		#region Private variables
		readonly CommandDispatcher _commandDispatcher;
		private readonly CommandQueue _commandQueue;
		#endregion

		#region Public Variables
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets the current level. </summary>
		///
		/// <value>	The current level. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public Level CurrentLevel { get; private set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the excavator. </summary>
		///
		/// <value>	The excavator. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public IExcavator Excavator { get; set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets the map. </summary>
		///
		/// <value>	The map. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public Map Map => CurrentLevel.Map;

	    #endregion

		#region Events
		/// <summary> Event queue for all listeners interested in new level events. </summary>
		public event EventHandler<EventArgs> NewLevelEvent;
		public void InvokeNewLevelEvent(Object sender, EventArgs e)
		{
			var handler = NewLevelEvent;
		    handler?.Invoke(sender, e);
		}

		/// <summary> Event queue for all listeners interested in hero movement events. </summary>
		public event EventHandler<CreatureMoveEventArgs> HeroMoveEvent;
		private void InvokeHeroMoveEvent(Object sender, CreatureMoveEventArgs e)
		{
			var handler = HeroMoveEvent;
		    handler?.Invoke(sender, e);
		}

		public event EventHandler<AttackEventArgs> AttackEvent;
		private void InvokeAttackEvent(Object sender, AttackEventArgs e)
		{
			var handler = AttackEvent;
		    handler?.Invoke(sender, e);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Invoke an event. </summary>
		///
		/// <remarks>	
		/// This is a bit nonstandard, but I want users to be able to deal with only the game object for
		/// all event handling.  That way they don't have to remember whether to set it on the map or the
		/// level and don't have to reset it if the map is destroyed for another level, etc., etc..  So
		/// all the event handling comes through the game object.  That doesn't necessarily mean the
		/// sender will be the game object.  If it makes more sense for the map to be the sender, then we
		/// make the map the sender in spite of the fact that the direct invoker is the Game.
		/// InvokeEvent is the common code that is called by all other parts of the system to invoke one
		/// of these events.  Darrellp, 10/8/2011. 
		/// </remarks>
		///
		/// <exception cref="RogueException">	Thrown when an event type isn't handled in the switch statement. </exception>
		///
		/// <param name="type">		The type of event being invoked. </param>
		/// <param name="sender">	Source of the event. </param>
		/// <param name="e">		Event information. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal void InvokeEvent(EventType type, Object sender, EventArgs e = null)
		{
			switch(type)
			{
				case EventType.CreatureMove:
					InvokeHeroMoveEvent(sender, e as CreatureMoveEventArgs);
					break;

				case EventType.NewLevel:
					InvokeNewLevelEvent(sender, e);
					break;

				case EventType.Attack:
					InvokeAttackEvent(sender, e as AttackEventArgs);
					break;

				default:
					throw new RogueException("Unexpected event invocation");
			}
		}
		#endregion

		#region Modification
		public Game(CommandDispatcher commandDispatcher = null)
		{
			_commandDispatcher = commandDispatcher ?? new CommandDispatcher(this);
			_commandQueue = new CommandQueue(this);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Sets a level. </summary>
		///
		/// <remarks>	Should normally be invoked only by a command.  Darrellp, 10/8/2011. </remarks>
		///
		/// <param name="levelCommand">	The level command which invoked this. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal void SetLevel(NewLevelCommand levelCommand)
		{
			CurrentLevel = new Level(levelCommand.Level, levelCommand.Width, levelCommand.Height, this, levelCommand.Excavator);
			MapCoordinates playerLocation = LocateStairwell();
			Map.SetFov(levelCommand.FOVRows, playerLocation, levelCommand.Filter);
			Map.MoveCreatureTo(Map.Player, playerLocation, true);
			InvokeEvent(EventType.NewLevel, this);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Locates the stairwell. </summary>
		///
		/// <remarks>	Darrellp, 10/8/2011. </remarks>
		///
		/// <exception cref="RogueException">	Thrown when no stairwell was found on the map. </exception>
		///
		/// <returns>	Location of the stairwell. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private MapCoordinates LocateStairwell()
		{
			for (int iRow  = 0; iRow  < Map.Height; iRow ++)
			{
				for (int iColumn = 0; iColumn < Map.Width; iColumn++)
				{
					if (Map.Terrain(iColumn, iRow) == TerrainType.StairsUp)
					{
						return new MapCoordinates(iColumn, iRow);
					}
				}
			}
			throw new RogueException("Stairwell not found in map");
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Queues up a command to be processed in next processing round. </summary>
		///
		/// <remarks>	Darrellp, 10/11/2011. </remarks>
		///
		/// <param name="command">	The command to be enqueued. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public void Enqueue(IRogueCommand command)
		{
			_commandQueue.AddCommand(command);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Enqueue a command and process the queue. </summary>
		///
		/// <remarks>	Darrellp, 10/11/2011. </remarks>
		///
		/// <param name="command">	The command to be enqueued. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public void EnqueueAndProcess(IRogueCommand command)
		{
			Enqueue(command);
			Process();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Process the command queue. </summary>
		///
		/// <remarks>	Darrellp, 10/11/2011. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public void Process()
		{
			_commandQueue.ProcessQueue();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Dispatches a command. </summary>
		///
		/// <remarks>	Relegates this to the command dispatcher.  Darrellp, 10/8/2011. </remarks>
		///
		/// <param name="command">	The command to be dispatched. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal void Dispatch(IRogueCommand command)
		{
			_commandDispatcher.Dispatch(command);
		}

		#endregion
	}
}
