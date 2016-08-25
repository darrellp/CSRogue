using System.Collections.Generic;
using CSRogue.Item_Handling;
using CSRogue.Items;
using CSRogue.Map_Generation;

namespace CSRogue.RogueEventArgs
{
	public class CreatureMoveEventArgs : System.EventArgs
	{
		public Map Map { get; private set; }
		public MapCoordinates PreviousCreatureLocation { get; private set; }
		public MapCoordinates CreatureDestination { get; private set; }
		public bool IsFirstTimePlacement { get; private set; }
		public bool IsPlayer { get; private set; }
		public bool IsBlocked { get; private set; }
		public bool IsRunning { get; private set; }
		public List<MapCoordinates> LitAtStartOfRun { get; private set; }

		public CreatureMoveEventArgs(
			Map map,
			Creature creature,
			MapCoordinates previousCreatureLocation,
			MapCoordinates creatureDestination,
			bool isFirstTimePlacement = false,
			bool isBlocked = false,
			bool isRunning = false,
			List<MapCoordinates> litAtStartOfRun = null)
		{
			Map = map;
			PreviousCreatureLocation = previousCreatureLocation;
			CreatureDestination = creatureDestination;
			IsFirstTimePlacement = isFirstTimePlacement;
			IsPlayer = creature.IsPlayer;
			IsBlocked = isBlocked;
			IsRunning = isRunning;
			LitAtStartOfRun = litAtStartOfRun;
		}
	}
}
