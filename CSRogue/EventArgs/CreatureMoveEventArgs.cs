using System.Collections.Generic;
using CSRogue.Items;
using CSRogue.Map_Generation;

namespace CSRogue.RogueEventArgs
{
	public class CreatureMoveEventArgs : System.EventArgs
	{
		public IGameMap GameMap { get; private set; }
		public MapCoordinates PreviousCreatureLocation { get; private set; }
		public MapCoordinates CreatureDestination { get; private set; }
		public bool IsFirstTimePlacement { get; private set; }
		public bool IsBlocked { get; private set; }
		public bool IsRunning { get; private set; }
		public List<MapCoordinates> LitAtStartOfRun { get; private set; }
        public ICreature Creature { get; private set; }

		public CreatureMoveEventArgs(
			IGameMap gameMap,
			ICreature creature,
			MapCoordinates previousCreatureLocation,
			MapCoordinates creatureDestination,
			bool isFirstTimePlacement = false,
			bool isBlocked = false,
			bool isRunning = false,
			List<MapCoordinates> litAtStartOfRun = null)
		{
			GameMap = gameMap;
			PreviousCreatureLocation = previousCreatureLocation;
			CreatureDestination = creatureDestination;
			IsFirstTimePlacement = isFirstTimePlacement;
			IsBlocked = isBlocked;
			IsRunning = isRunning;
			LitAtStartOfRun = litAtStartOfRun;
		    Creature = creature;
		}
	}
}
