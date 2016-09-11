using System;
using CSRogue.RogueEventArgs;
using CSRogue.GameControl.Commands;
using CSRogue.Interfaces;
using CSRogue.Item_Handling;
using CSRogue.Utilities;

namespace CSRogue.GameControl
{
	enum EventType
	{
        HeroMove,
		CreatureMove,
		NewLevel,
		Attack
	}

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A game. </summary>
    ///
    /// <remarks>   Game is pretty much just a gathering place for all the information about the
    ///             game as well as a dispatcher for the events which occur during the game.  In
    ///             particular, the current level, the item factory/information are in here.
    ///             
    ///             The Game object also coordinates tasks and their execution.  Tasks can be
    ///             queued up and then processed in one turn.  This ensures that two conflicting
    ///             events happen in a definitive order and separates events from each other.
    ///             
    ///             The events included in CSRogue are not sacrosanct.  Users can make up their own
    ///             tasks or modify these and have them scheduled along with all the other tasks.
    ///             Darrell, 9/9/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

	public class Game
	{
		#region Private variables
	    private readonly CommandQueue _commandQueue;
		#endregion

		#region Public Variables
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets the current level. </summary>
		///
		/// <value>	The current level. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public ILevel CurrentLevel { get; internal set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets the map. </summary>
		///
		/// <value>	The map. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public IGameMap Map => CurrentLevel?.Map;

	    #endregion

		#region Events
		/// <summary> Event queue for all listeners interested in new level events. </summary>
		public event EventHandler<NewLevelEventArgs> NewLevelEvent;
		public void InvokeNewLevelEvent(Object sender, NewLevelEventArgs e)
		{
            NewLevelEvent?.Invoke(sender, e);
		}

		/// <summary> Event queue for all listeners interested in hero movement events. </summary>
		public event EventHandler<CreatureMoveEventArgs> HeroMoveEvent;
		private void InvokeHeroMoveEvent(Object sender, CreatureMoveEventArgs e)
		{
            HeroMoveEvent?.Invoke(sender, e);
		}

		public event EventHandler<AttackEventArgs> AttackEvent;
		private void InvokeAttackEvent(Object sender, AttackEventArgs e)
		{
            AttackEvent?.Invoke(sender, e);
		}

        /// <summary> Event queue for all listeners interested in hero movement events. </summary>
        public event EventHandler<CreatureMoveEventArgs> CreatureMoveEvent;
        private void InvokeCreatureMoveEvent(Object sender, CreatureMoveEventArgs e)
        {
            CreatureMoveEvent?.Invoke(sender, e);
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
                case EventType.HeroMove:
                    InvokeHeroMoveEvent(sender, e as CreatureMoveEventArgs);
                    break;

                case EventType.CreatureMove:
					InvokeCreatureMoveEvent(sender, e as CreatureMoveEventArgs);
					break;

				case EventType.NewLevel:
					InvokeNewLevelEvent(sender, e as NewLevelEventArgs);
					break;

				case EventType.Attack:
					InvokeAttackEvent(sender, e as AttackEventArgs);
					break;

				default:
					throw new RogueException("Unexpected event invocation");
			}
		}
        #endregion

        #region Constructor
        public Game(IItemFactory factory)
		{
		    Factory = factory;
			_commandQueue = new CommandQueue(this);
		}
        #endregion

        #region Modification
	    public IItemFactory Factory { get; set; }
        #endregion

        #region Event Queuing
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

	    internal void EndTurn()
	    {
			CurrentLevel.InvokeMonsterAI();
		}
		#endregion
	}
}
