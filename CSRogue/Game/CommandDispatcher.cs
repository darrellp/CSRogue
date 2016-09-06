using System.Collections.Generic;
using System.Linq;
using CSRogue.GameControl.Commands;
using CSRogue.Items;
using CSRogue.Map_Generation;
using CSRogue.RogueEventArgs;
using CSRogue.Utilities;

namespace CSRogue.GameControl
{
	public class CommandDispatcher
	{
		#region Private variables
		private readonly Game _game;
		private static readonly Dictionary<CommandType, MapCoordinates> Directions =
			new Dictionary<CommandType, MapCoordinates>
		    {
		        {CommandType.MoveLeft, new MapCoordinates(-1, 0)},
		        {CommandType.MoveRight, new MapCoordinates(1, 0)},
		        {CommandType.MoveUp, new MapCoordinates(0, -1)},
		        {CommandType.MoveDown, new MapCoordinates(0, 1)},
		        {CommandType.MoveUpperLeft, new MapCoordinates(-1, -1)},
		        {CommandType.MoveLowerLeft, new MapCoordinates(-1, 1)},
		        {CommandType.MoveUpperRight, new MapCoordinates(1, -1)},
		        {CommandType.MoveLowerRight, new MapCoordinates(1, 1)},
				{CommandType.StayPut, new MapCoordinates(0, 0)},
		    };
		#endregion

		#region Properties
		IGameMap Map => _game.CurrentLevel.Map;

	    #endregion

		#region Constructor
		public CommandDispatcher(Game game)
		{
			_game = game;
		}
		#endregion

		#region Command Handling
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Dispatches a command. </summary>
		///
		/// <remarks>	Darrellp, 10/8/2011. </remarks>
		///
		/// <param name="command">	The command to be dispatched. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public void Dispatch(IRogueCommand command)
		{
			// Is the command null?
			if (command == null)
			{
				return;
			}

			// Can we handle it as a direction command?
			if (HandleMovementCommands(command))
			{
				return;
			}

			// Switch on the command type
			switch (command.Command)
			{
				case CommandType.NewLevel:
					_game.SetLevel((NewLevelCommand)command);
					break;

				case CommandType.MoveTo:
					MoveTo((MoveToCommand)command);
					break;
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Execute a MoveTo command. </summary>
		///
		/// <remarks>	Darrellp, 10/14/2011. </remarks>
		///
		/// <param name="command">	The MoveTo command to be executed. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void MoveTo(MoveToCommand command)
		{
			// If there isn't already a creature there
			if (Map.CreatureAt(command.Location) == null)
			{
				// Then move the creature there
				_game.CurrentLevel.Map.MoveCreatureTo(command.Creature, command.Location);
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Handle movement commands. </summary>
		///
		/// <remarks>	Darrellp, 10/8/2011. </remarks>
		///
		/// <param name="command">	The command to be dispatched. </param>
		///
		/// <returns>	true if this was a movement command, false otherwise. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public virtual bool HandleMovementCommands(IRogueCommand command)
		{
			// Is it a direction command?
			if (Directions.ContainsKey(command.Command))
			{
				// Is it a stay put command?
				if (command.Command == CommandType.StayPut)
				{
					// Let the monsters do their thing and...
					_game.CurrentLevel.InvokeMonsterAI();

					return true;
				}

				// Get values from the command
				MapCoordinates offset = Directions[command.Command];
				bool run = ((MovementCommand)command).Run;
				MapCoordinates newLocation = Map.Player.Location + offset;
				Creature victim;

				// Are we running?
				if (run)
				{
					// Locals
					var proposedLocation = Map.Player.Location + offset;
					var maybeBlocksUs = proposedLocation + offset;

					// If we're blocked before we start...
					if (!Map.ValidRunningMove(proposedLocation))
					{
						// Nothing to do...just return true
						return true;
					}

					List<MapCoordinates> litAtStartOfRun = _game.Map.Fov.CurrentlySeen.ToList();

					// While we're not blocked ahead
					while (Map.ValidRunningMove(maybeBlocksUs))
					{
						// Move to the proposed location
						MakePlayerMove(proposedLocation, run:true);

						// And advance our two pointers along
						proposedLocation = maybeBlocksUs;
						maybeBlocksUs = maybeBlocksUs + offset;
					}

					// Last move is made without running
					// This is so the caller knows when to update his screen
					MakePlayerMove(proposedLocation, run:false, litAtStartOfRun:litAtStartOfRun);
				}
				// Else if some unfortunate is in our way
				else if ((victim = Map.CreatureAt(newLocation)) != null)
				{
					// Attack him
					MakeAttack(Map.Player, victim);

					// Monsters do their thing
					_game.CurrentLevel.InvokeMonsterAI();
				}
				// Else if this is valid terrain
				else if (Map.Walkable(newLocation))
				{
					// Move the player
					MakePlayerMove(newLocation);
				}
				else
				{
					Map.NotifyOfBlockage(Map.Player, newLocation);
				}
				return true;
			}

			// Unhandled as a movement key
			return false;
		}

		private void MakePlayerMove(MapCoordinates location, bool run = false, List<MapCoordinates> litAtStartOfRun = null)
		{
			Map.MoveCreatureTo(Map.Player, location, run:run, litAtStartOfRun:litAtStartOfRun);
			_game.CurrentLevel.InvokeMonsterAI();
			_game.Process();
		}

		public virtual void MakeAttack(Creature attacker, Creature victim)
		{
			// Determine damage
			int damage = victim.IsPlayer ? 0 : victim.HitPoints;

			// Hit the victim for that damage
			HitCreatureFor(victim, damage);
			bool expired = victim.HitPoints == 0;

			// Did the victim die?
			if (expired)
			{
				// Kill the unfortunate victim
				// TODO: check to see if the victim is the player in which case, game over!
				_game.CurrentLevel.KillCreature(victim);
			}

			// Invoke an attack event for this attack
			AttackEventArgs attackArgs = new AttackEventArgs(attacker, victim, damage, expired, victim.Location);
			_game.InvokeEvent(EventType.Attack, _game, attackArgs);
		}

		private static void HitCreatureFor(Creature victim, int damage)
		{
			victim.HitPoints -= damage;
		}
		#endregion
	}
}
