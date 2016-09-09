using System.Collections.Generic;
using System.Linq;
using CSRogue.Items;
using CSRogue.Map_Generation;
using CSRogue.RogueEventArgs;
using CSRogue.Utilities;

namespace CSRogue.GameControl.Commands
{
    #region enums

    public enum MoveDirection
    { 
        MoveLeft,
        MoveRight,
        MoveUp,
        MoveDown,
        MoveUpperLeft,
        MoveLowerLeft,
        MoveUpperRight,
        MoveLowerRight,
        StayPut,
    }
    #endregion

    public class MovementCommand : CommandBase
	{
        #region Private variables
        private static readonly Dictionary<MoveDirection, MapCoordinates> Directions =
            new Dictionary<MoveDirection, MapCoordinates>
            {
                {MoveDirection.MoveLeft, new MapCoordinates(-1, 0)},
                {MoveDirection.MoveRight, new MapCoordinates(1, 0)},
                {MoveDirection.MoveUp, new MapCoordinates(0, -1)},
                {MoveDirection.MoveDown, new MapCoordinates(0, 1)},
                {MoveDirection.MoveUpperLeft, new MapCoordinates(-1, -1)},
                {MoveDirection.MoveLowerLeft, new MapCoordinates(-1, 1)},
                {MoveDirection.MoveUpperRight, new MapCoordinates(1, -1)},
                {MoveDirection.MoveLowerRight, new MapCoordinates(1, 1)},
                {MoveDirection.StayPut, new MapCoordinates(0, 0)},
            };

        private MapCoordinates _direction;
        private MoveDirection _moveDirection;
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>	Gets or sets a value indicating whether we should run. </summary>
        ///
        /// <value>	true if we should run, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool Run { get; private set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the creature to be moved. </summary>
		///
		/// <value>	The creature. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public ICreature Creature { get; private set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
	    ///  <summary>	Constructor. </summary>
	    /// 
	    ///  <remarks>	Darrellp, 10/6/2011. </remarks>
	    /// 
	    ///  <param name="commandType">	Type of movement. </param>
	    ///  <param name="shouldRun">	true if we should run. </param>
	    /// <param name="creature">     Creature for this command </param>
	    ////////////////////////////////////////////////////////////////////////////////////////////////////
	    public MovementCommand(MoveDirection moveDirection, bool shouldRun = false, ICreature creature = null)
		{
			Run = shouldRun;
			Creature = creature;
	        _moveDirection = moveDirection;
	        _direction = Directions[_moveDirection];
		}

        public override bool Execute(Game game)
        {
            // Is it a stay put command?
            if (_moveDirection == MoveDirection.StayPut)
            {
                // Let the monsters do their thing and...
                game.CurrentLevel.InvokeMonsterAI();
                return true;
            }
            var newLocation = game.Map.Player.Location + _direction;
            Creature victim;

            if (Run)
            {
                // Locals
                var proposedLocation = game.Map.Player.Location + _direction;
                var maybeBlocksUs = proposedLocation + _direction;

                // If we're blocked before we start...
                if (!game.Map.ValidRunningMove(proposedLocation))
                {
                    // Nothing to do...just return true
                    return true;
                }

                var litAtStartOfRun = game.Map.Fov.CurrentlySeen.ToList();

                // While we're not blocked ahead
                while (game.Map.ValidRunningMove(maybeBlocksUs))
                {
                    // Move to the proposed location
                    MakePlayerMove(game, proposedLocation, run: true);

                    // And advance our two pointers along
                    proposedLocation = maybeBlocksUs;
                    maybeBlocksUs = maybeBlocksUs + _direction;
                }

                // Last move is made without running
                // This is so the caller knows when to update his screen
                MakePlayerMove(game, proposedLocation, run: false, litAtStartOfRun: litAtStartOfRun);
            }
            // Else if some unfortunate is in our way
            else if ((victim = game.Map.CreatureAt(newLocation)) != null)
            {
                // Attack him
                MakeAttack(game, game.Map.Player, victim);

                // Monsters do their thing
                game.CurrentLevel.InvokeMonsterAI();
            }
            // Else if this is valid terrain
            else if (game.Map.Walkable(newLocation))
            {
                // Move the player
                MakePlayerMove(game, newLocation);
            }
            else
            {
                game.Map.NotifyOfBlockage(game.Map.Player, newLocation);
            }
            return true;
        }

        private void MakePlayerMove(Game game, MapCoordinates location, bool run = false, List<MapCoordinates> litAtStartOfRun = null)
        {
            game.Map.MoveCreatureTo(game.Map.Player, location, run: run, litAtStartOfRun: litAtStartOfRun);
            game.CurrentLevel.InvokeMonsterAI();
            game.Process();
        }

        public virtual void MakeAttack(Game game, ICreature attacker, ICreature victim)
        {
            // Determine damage
            var damage = victim.IsPlayer ? 0 : victim.HitPoints;

            // Hit the victim for that damage
            HitCreatureFor(victim, damage);
            var expired = victim.HitPoints == 0;

            // Did the victim die?
            if (expired)
            {
                // Kill the unfortunate victim
                // TODO: check to see if the victim is the player in which case, game over!
                game.CurrentLevel.KillCreature(victim);
            }

            // Invoke an attack event for this attack
            var attackArgs = new AttackEventArgs(attacker, victim, damage, expired, victim.Location);
            game.InvokeEvent(EventType.Attack, game, attackArgs);
        }

        private static void HitCreatureFor(ICreature victim, int damage)
        {
            victim.HitPoints -= damage;
        }
    }
}
