﻿using System.Collections.Generic;
using CSRogue.GameControl;
using CSRogue.Interfaces;
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
            foreach (var newlyLitLocation in map.Fov.NewlySeen())
            {
                map.SetInView(newlyLitLocation);
				map.SetRemembered(newlyLitLocation);
            }
            foreach (var previouslyLitLocation in map.Fov.NewlyUnseen())
            {
				map.SetInView(previouslyLitLocation, false);
			}
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   An IGameMap extension method that scans the fov for the player. </summary>
        ///
        /// <remarks>   Just doing a scan with the FOV is not enough.  The FOV intentionally doesn't know
        ///             how to manipulate the map because sometimes the manipulation is different than others.
        ///             If we're scanning for a monster we don't change what is lit on the map for instance.
        ///             In order to modify the map properly we need to call Relight after the scan which is
        ///             what we do here. Darrell, 9/14/2016. </remarks>
        ///
        /// <param name="map">  The map to act on. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void ScanPlayer(this IGameMap map)
        {
            map.Fov.Scan(map.Player.Location);
            map.Relight();
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
            ICreature creature,
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
            map.Game?.InvokeEvent(
                creature.IsPlayer ? EventType.HeroMove : EventType.CreatureMove, map,
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

        public static void NotifyOfBlockage(this IGameMap map, IPlayer creature, MapCoordinates blockedLocation)
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