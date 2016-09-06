using System.Collections.Generic;
using CSRogue.GameControl;
using CSRogue.Items;
using CSRogue.Map_Generation;
using CSRogue.RogueEventArgs;

namespace CSRogue.Utilities
{
    public static class GameMapExtensions
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>	Marks the newly lit and formerly lit spots on the map. </summary>
        ///
        /// <remarks>	Darrellp, 10/15/2011. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void Relight(this IGameMap map)
        {
            foreach (var newlyLitLocation in map.Fov.NewlySeen)
            {
                map.SetInView(newlyLitLocation);
				map.SetRemembered(newlyLitLocation);
            }
            foreach (var previouslyLitLocation in map.Fov.NewlyUnseen)
            {
				map.SetInView(previouslyLitLocation, false);
			}
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>	Move creature to a new location. </summary>
        ///
        /// <remarks>	Darrellp, 10/15/2011. </remarks>
        ///
        /// <param name="map">					 	The map to act on. </param>
        /// <param name="creature">				 	The creature. </param>
        /// <param name="newLocation">			 	The new location. </param>
        /// <param name="firstTimeHeroPlacement">	(Optional) true when placing the hero the first time. </param>
        /// <param name="run">					 	(Optional) true when this is part of a run. </param>
        /// <param name="litAtStartOfRun">		 	(Optional) A list of lit locations at the start of a
        /// 										run. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void MoveCreatureTo(
            this IGameMap map,
            Creature creature,
            MapCoordinates newLocation,
            bool firstTimeHeroPlacement = false,
            bool run = false,
            List<MapCoordinates> litAtStartOfRun = null)
        {
            // Get the data from the current location
            var data = map[creature.Location];
            var oldPosition = creature.Location;

            // Remove the creature from this location
            data.RemoveItem(creature);

            // Place the creature at the new location
            creature.Location = newLocation;
            map[creature.Location].AddItem(creature);

            // If it's the player and there's a FOV to be calculated
            if (creature.IsPlayer && !run && map.Fov != null)
            {
                // Rescan for FOV
                map.Fov.Scan(creature.Location);
                map.Relight();
            }

            // If we've got a game object
            // Invoke the move event through it
            map.Game?.InvokeEvent(EventType.CreatureMove, map,
                new CreatureMoveEventArgs(
                    map,
                    creature,
                    oldPosition,
                    newLocation,
                    firstTimeHeroPlacement,
                    isBlocked: false,
                    isRunning: run,
                    litAtStartOfRun: litAtStartOfRun));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>	Notify the user that the player was blocked. </summary>
        ///
        /// <remarks>	Darrellp, 10/15/2011. </remarks>
        ///
        /// <param name="map">			  	The map to act on. </param>
        /// <param name="creature">		  	The creature. </param>
        /// <param name="blockedLocation">	The blocked location. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void NotifyOfBlockage(this IGameMap map, Creature creature, MapCoordinates blockedLocation)
        {
            map.Game.InvokeEvent(EventType.CreatureMove, map,
                new CreatureMoveEventArgs(
                    map,
                    creature,
                    creature.Location,
                    blockedLocation,
                    isBlocked: true));
        }
	}
}