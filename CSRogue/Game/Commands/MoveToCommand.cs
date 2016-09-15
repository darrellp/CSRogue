using CSRogue.Interfaces;
using CSRogue.Items;
using CSRogue.Map_Generation;
using CSRogue.Utilities;

namespace CSRogue.GameControl.Commands
{
    public class MoveToCommand : CommandBase
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>	Gets or sets the creature to be moved. </summary>
        ///
        /// <value>	The creature. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ICreature Creature { get; private set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>	Gets or sets the location the creature is to be moved to. </summary>
        ///
        /// <value>	The location. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public MapCoordinates Location { get; private set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>	Constructor. </summary>
        ///
        /// <remarks>	Darrellp, 10/6/2011. </remarks>
        ///
        /// <param name="creature">	The creature to be moved. </param>
        /// <param name="location">	The location to move the creature to. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public MoveToCommand(ICreature creature, MapCoordinates location)
        {
            Creature = creature;
            Location = location;
        }

        public override void Execute(Game game)
        {
            // If there isn't already a creature there
            if (Creature.IsAlive && game.Map.CreatureAt(Location) == null)
            {
                // Then move the creature there
                game.CurrentLevel.Map.MoveCreatureTo(Creature, Location);
            }
        }
    }
}
