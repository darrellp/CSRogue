using System.Collections.Generic;
using System.Linq;
using CSRogue.Interfaces;
using CSRogue.Items;
using CSRogue.Map_Generation;
using CSRogue.Utilities;

namespace CSRogue.GameControl.Commands
{
    public class MovePlayerCommand : CommandBase
	{
        #region Private variables
        private MapCoordinates _direction;
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>	Gets or sets a value indicating whether we should run. </summary>
        ///
        /// <value>	true if we should run, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool Run { get; }

	    ////////////////////////////////////////////////////////////////////////////////////////////////////
	    /// <summary>	Constructor. </summary>
	    ///
	    /// <remarks>	Darrellp, 10/6/2011. </remarks>
	    ///
	    /// <param name="direction">	Type of movement. </param>
	    /// <param name="shouldRun">	(Optional) true if we should run. </param>
	    ////////////////////////////////////////////////////////////////////////////////////////////////////

	    public MovePlayerCommand(MapCoordinates direction, bool shouldRun = false)
		{
			Run = shouldRun;
	        _direction = direction;
		}

        public override void Execute(Game game)
        {
            var newLocation = game.Map.Player.Location + _direction;
            ICreature victim;

            if (Run)
            {
                // Locals
                var proposedLocation = newLocation;
                var maybeBlocksUs = proposedLocation + _direction;

                // If we're blocked before we start...
                if (!game.Map.ValidRunningMove(proposedLocation))
                {
                    // Nothing to do...just return true
                    return;
                }

                var litAtStartOfRun = game.Map.Fov.CurrentlySeen().ToList();

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
            else if ((victim = game.Map.CreatureAt(newLocation)) != null && !victim.IsPlayer)
            {
                // Attack him
				AttackCommand cmd = new AttackCommand(game.Map.Player, victim);
				game.Enqueue(cmd);

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
        }

        private void MakePlayerMove(Game game, MapCoordinates location, bool run = false, List<MapCoordinates> litAtStartOfRun = null)
        {
            game.Map.MoveCreatureTo(game.Map.Player, location, run: run, litAtStartOfRun: litAtStartOfRun);
	        game.EndTurn();
        }
    }
}
