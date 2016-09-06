using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

		#region Terrain States
		#region In View
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool InView(this IGameMap map, int x, int y)
		{
			return map.Value(TerrainState.InView, x, y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool InView(this IGameMap map, MapCoordinates crd)
		{
			return map.Value(TerrainState.InView, crd);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetInView(this IGameMap map, int x, int y, bool fOn = true)
		{
			map.Set(TerrainState.InView, x, y, fOn);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetInView(this IGameMap map, MapCoordinates crd, bool fOn = true)
		{
			map.Set(TerrainState.InView, crd, fOn);
		}
		#endregion

		#region Remembered
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Remembered(this IGameMap map, int x, int y)
		{
			return map.Value(TerrainState.Remembered, x, y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Remembered(this IGameMap map, MapCoordinates crd)
		{
			return map.Value(TerrainState.Remembered, crd);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetRemembered(this IGameMap map, int x, int y, bool fOn = true)
		{
			map.Set(TerrainState.Remembered, x, y, fOn);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetRemembered(this IGameMap map, MapCoordinates crd, bool fOn = true)
		{
			map.Set(TerrainState.Remembered, crd, fOn);
		}
		#endregion

		#region FogOfWar
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool FogOfWar(this IGameMap map, int x, int y)
		{
			return map.Value(TerrainState.FogOfWar, x, y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool FogOfWar(this IGameMap map, MapCoordinates crd)
		{
			return map.Value(TerrainState.FogOfWar, crd);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetFogOfWar(this IGameMap map, int x, int y, bool fOn = true)
		{
			map.Set(TerrainState.FogOfWar, x, y, fOn);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetFogOfWar(this IGameMap map, MapCoordinates crd, bool fOn = true)
		{
			map.Set(TerrainState.FogOfWar, crd, fOn);
		}
		#endregion

		#region BlocksView
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool BlocksView(this IGameMap map, int x, int y)
		{
			return map.Value(TerrainState.BlocksView, x, y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool BlocksView(this IGameMap map, MapCoordinates crd)
		{
			return map.Value(TerrainState.BlocksView, crd);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetBlocksView(this IGameMap map, int x, int y, bool fOn = true)
		{
			map.Set(TerrainState.BlocksView, x, y, fOn);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetBlocksView(this IGameMap map, MapCoordinates crd, bool fOn = true)
		{
			map.Set(TerrainState.BlocksView, crd, fOn);
		}
		#endregion

		#region Walkable
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Walkable(this IGameMap map, int x, int y)
		{
			return map.Value(TerrainState.Walkable, x, y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Walkable(this IGameMap map, MapCoordinates crd)
		{
			return map.Value(TerrainState.Walkable, crd);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetWalkable(this IGameMap map, int x, int y, bool fOn = true)
		{
			map.Set(TerrainState.Walkable, x, y, fOn);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetWalkable(this IGameMap map, MapCoordinates crd, bool fOn = true)
		{
			map.Set(TerrainState.Walkable, crd, fOn);
		}
		#endregion

		#region Generic
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Value(this IGameMap map, TerrainState state, int x, int y)
		{
			return (map[x, y].TerrainState & state) != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Value(this IGameMap map, TerrainState state, MapCoordinates crd)
		{
			return (map[crd].TerrainState & state) != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Set(this IGameMap map, TerrainState state, int x, int y, bool fOn = true)
		{
			if (fOn)
			{
				map[x, y].TerrainState |= state;
			}
			else
			{
				map[x, y].TerrainState &= ~state;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Set(this IGameMap map, TerrainState state, MapCoordinates crd, bool fOn = true)
		{
			if (fOn)
			{
				map[crd].TerrainState |= state;
			}
			else
			{
				map[crd].TerrainState &= ~state;
			}
		}
		#endregion
		#endregion
	}
}