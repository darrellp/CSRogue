using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using CSRogue.Items;
using CSRogue.Item_Handling;
using CSRogue.Map_Generation;

namespace CSRogue.Utilities
{
    public static class MapExtensions
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>	Return all neighbors of a cell taking the borders into account. </summary>
        ///
        /// <remarks>	Darrellp, 9/26/2011. </remarks>
        ///
        /// <param name="map">   	The map. </param>
        /// <param name="column">	The location's columns. </param>
        /// <param name="row">   	The location's row. </param>
        ///
        /// <returns>	An enumerable of all neighbors. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static IEnumerable<MapCoordinates> Neighbors(this IMap map, int column, int row)
        {
            var minRowOffset = row == 0 ? 0 : -1;
            var maxRowOffset = row == map.Height - 1 ? 0 : 1;
            var minColumnOffset = column == 0 ? 0 : -1;
            var maxColumnOffset = column == map.Width - 1 ? 0 : 1;

            for (var iRowOffset = minRowOffset; iRowOffset <= maxRowOffset; iRowOffset++)
            {
                for (var iColumnOffset = minColumnOffset; iColumnOffset <= maxColumnOffset; iColumnOffset++)
                {
                    if (iRowOffset != 0 || iColumnOffset != 0)
                    {
                        yield return new MapCoordinates(column + iColumnOffset, row + iRowOffset);
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return all neighbors of a cell taking the borders into account. </summary>
        ///
        /// <remarks>   Darrellp, 9/26/2011. </remarks>
        ///
        /// <param name="map">      The map. </param>
        /// <param name="coords">   The coordinate whose neighbors are desired. </param>
        ///
        /// <returns>   An enumerable of all neighbors. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static IEnumerable<MapCoordinates> Neighbors(this IMap map, MapCoordinates coords)
        {
            return Neighbors(map, coords.Column, coords.Row);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// An IMAP extension method to see if the given location is in the bounds of the map.
        /// </summary>
        ///
        /// <remarks>   Darrell, 8/28/2016. </remarks>
        ///
        /// <param name="map">      The map. </param>
        /// <param name="column">   The column. </param>
        /// <param name="row">      The row. </param>
        ///
        /// <returns>   true if the location is in the map bounds, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool Contains(this IMap map, int column, int row)
        {
            return column >= 0 && row >= 0 && column < map.Width && row < map.Height;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// An IMAP extension method to see if the given location is in the bounds of the map.
        /// </summary>
        ///
        /// <remarks>   Darrell, 8/28/2016. </remarks>
        ///
        /// <param name="map">      The map. </param>
        /// <param name="location"> The location to check. </param>
        ///
        /// <returns>   true if the location is in the map bounds, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool Contains(this IMap map, MapCoordinates location)
        {
            return map.Contains(location.Column, location.Row);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>	Find a random floor location. </summary>
        ///
        /// <remarks>	Darrellp, 10/3/2011. </remarks>
        ///
        /// <returns>	The location found. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static MapCoordinates RandomFloorLocation(this IMap map, bool restrictToRooms = false)
        {
            var rnd = Rnd.Global;
            int row, column;

            while (true)
            {
                // Try a random spot
                row = rnd.Next(map.Height);
                column = rnd.Next(map.Width);

				// Needs to be walkable and have at least three walkable neighbors
	            if (map.Walkable(column, row) && map.Neighbors(column, row).Count(map.Walkable) > 2)
	            {
		            break;
	            }
            }

            // Return it
            return new MapCoordinates(column, row);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Drops an item in the map. </summary>
        ///
        /// <remarks>   Darrellp, 9/16/2011. </remarks>
        ///
        /// <param name="map">  . </param>
        /// <param name="iCol"> The col to drop in. </param>
        /// <param name="iRow"> The row to drop in. </param>
        /// <param name="item"> The item to drop. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void Drop(this IMap map, int iCol, int iRow, IItem item)
        {
            map[iCol,iRow].AddItem(item);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>	Drops an item in the map. </summary>
        ///
        /// <remarks>	Darrellp, 9/16/2011. </remarks>
        ///
        /// <param name="map">	   	The map we're dropping on </param>
        /// <param name="location">	The location. </param>
        /// <param name="item">	   	The item to drop. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void Drop(this IMap map, MapCoordinates location, IItem item)
        {
            map.Drop(location.Column, location.Row, item);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>	Removes an item from the map. </summary>
        ///
        /// <remarks>	Darrellp, 9/16/2011. </remarks>
        ///
        /// <param name="map"> 	The map we're removing from. </param>
        /// <param name="iCol">	The col to remove from. </param>
        /// <param name="iRow">	The row to remove from. </param>
        /// <param name="item">	The item to remove. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void Remove(this IMap map, int iCol, int iRow, IItem item)
        {
            map[iCol, iRow].RemoveItem(item);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Query if there is a creature at a location. </summary>
        ///
        /// <remarks>   Darrellp, 10/15/2011. </remarks>
        ///
        /// <param name="map">      The map. </param>
        /// <param name="location"> The location to check. </param>
        ///
        /// <returns>   The creature at location or null if no creature there. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Creature CreatureAt(this IMap map, MapCoordinates location)
        {
            // Find a creature, if any, at the destination
            return map[location].Items.FirstOrDefault(i => (i as Creature) != null) as Creature;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Find out if there's a creature at a particular location. </summary>
        ///
        /// <remarks>   Darrellp, 10/15/2011. </remarks>
        ///
        /// <param name="map">      The map. </param>
        /// <param name="location"> The location to check. </param>
        ///
        /// <returns>   true if there is a creature at this location, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool IsCreatureAt(this IMap map, MapCoordinates location)
        {
            return map.CreatureAt(location) != null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns true if we want the creature to continue running in this direction.
        /// </summary>
        ///
        /// <remarks>	Darrellp, 10/15/2011. </remarks>
        ///
        /// <param name="map">	   	The map. </param>
        /// <param name="location">	The location to be checked. </param>
        ///
        /// <returns>	true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool ValidRunningMove(this IMap map, MapCoordinates location)
        {
            return map.Walkable(location) && !map.IsCreatureAt(location);
        }

        public static List<MapCoordinates> LocateTerrain(this IMap map, TerrainType type, bool fFirstOnly = false)
        {
            var ret = new List<MapCoordinates>();
            for (var iRow = 0; iRow < map.Height; iRow++)
            {
                for (var iColumn = 0; iColumn < map.Width; iColumn++)
                {
                    if (map[iColumn, iRow].Terrain == type)
                    {
                        ret.Add(new MapCoordinates(iColumn, iRow));
                        if (fFirstOnly)
                        {
                            return ret;
                        }
                    }
                }
            }
            return ret;
        }

        public static List<MapCoordinates> LocateItems(this IMap map, Guid id, bool fFirstOnly = false)
        {
            var ret = new List<MapCoordinates>();
            for (var iRow = 0; iRow < map.Height; iRow++)
            {
                for (var iColumn = 0; iColumn < map.Width; iColumn++)
                {
                    if (map[iColumn, iRow].Items.FirstOrDefault(i => i.ItemTypeId == id) != null)
                    {
                        ret.Add(new MapCoordinates(iColumn, iRow));
                        if (fFirstOnly)
                        {
                            return ret;
                        }
                    }
                }
            }
            return ret;
        }

        public static ICreature LocatePlayer(this IGameMap map)
        {
            for (var iRow = 0; iRow < map.Height; iRow++)
            {
                for (var iColumn = 0; iColumn < map.Width; iColumn++)
                {
                    var trialPlayer = map[iColumn, iRow].Items.FirstOrDefault(i => map.Game.Factory.InfoFromId[i.ItemTypeId].IsPlayer);
                    if (trialPlayer != null)
                    {
                        return (ICreature)trialPlayer;
                    }
                }
            }
            return null;
        }

        public static void SetPlayer(this IGameMap map, bool moveToStairwell = true, IPlayer playerIn = null)
        {
            if (map.Player == null)
            {
                var player = map.LocatePlayer();
                if (player != null)
                {
                    map.Player = (IPlayer)player;
                }
                else if (playerIn != null)
                {
                    map.Player = playerIn;
                }
                else
                {
                    map.Player = new Player();
                }
            }
            if (moveToStairwell)
            {
                var stairwellLoc = map.LocateTerrain(TerrainType.StairsDown, true);
                if (stairwellLoc.Count > 0)
                {
                    map.MoveCreatureTo(map.Player, stairwellLoc[0]);
                }
            }
            
        }

        #region Terrain States
        #region In View
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool InView(this IMap map, int x, int y)
		{
			return map.Value(TerrainState.InView, x, y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool InView(this IMap map, MapCoordinates crd)
		{
			return map.Value(TerrainState.InView, crd);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetInView(this IMap map, int x, int y, bool fOn = true)
		{
			map.Set(TerrainState.InView, x, y, fOn);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetInView(this IMap map, MapCoordinates crd, bool fOn = true)
		{
			map.Set(TerrainState.InView, crd, fOn);
		}
		#endregion

		#region Remembered
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Remembered(this IMap map, int x, int y)
		{
			return map.Value(TerrainState.Remembered, x, y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Remembered(this IMap map, MapCoordinates crd)
		{
			return map.Value(TerrainState.Remembered, crd);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetRemembered(this IMap map, int x, int y, bool fOn = true)
		{
			map.Set(TerrainState.Remembered, x, y, fOn);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetRemembered(this IMap map, MapCoordinates crd, bool fOn = true)
		{
			map.Set(TerrainState.Remembered, crd, fOn);
		}
		#endregion

		#region FogOfWar
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool FogOfWar(this IMap map, int x, int y)
		{
			return map.Value(TerrainState.FogOfWar, x, y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool FogOfWar(this IMap map, MapCoordinates crd)
		{
			return map.Value(TerrainState.FogOfWar, crd);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetFogOfWar(this IMap map, int x, int y, bool fOn = true)
		{
			map.Set(TerrainState.FogOfWar, x, y, fOn);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetFogOfWar(this IMap map, MapCoordinates crd, bool fOn = true)
		{
			map.Set(TerrainState.FogOfWar, crd, fOn);
		}
		#endregion

		#region BlocksView
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool BlocksView(this IMap map, int x, int y)
		{
			return map.Value(TerrainState.BlocksView, x, y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool BlocksView(this IMap map, MapCoordinates crd)
		{
			return map.Value(TerrainState.BlocksView, crd);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetBlocksView(this IMap map, int x, int y, bool fOn = true)
		{
			map.Set(TerrainState.BlocksView, x, y, fOn);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetBlocksView(this IMap map, MapCoordinates crd, bool fOn = true)
		{
			map.Set(TerrainState.BlocksView, crd, fOn);
		}
		#endregion

		#region Walkable
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Walkable(this IMap map, int x, int y)
		{
			return map.Value(TerrainState.Walkable, x, y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Walkable(this IMap map, MapCoordinates crd)
		{
			return map.Value(TerrainState.Walkable, crd);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetWalkable(this IMap map, int x, int y, bool fOn = true)
		{
			map.Set(TerrainState.Walkable, x, y, fOn);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetWalkable(this IMap map, MapCoordinates crd, bool fOn = true)
		{
			map.Set(TerrainState.Walkable, crd, fOn);
		}
		#endregion

		#region Generic
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Value(this IMap map, TerrainState state, int x, int y)
		{
			return (map[x, y].TerrainState & state) != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Value(this IMap map, TerrainState state, MapCoordinates crd)
		{
			return (map[crd].TerrainState & state) != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Set(this IMap map, TerrainState state, int x, int y, bool fOn = true)
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
		public static void Set(this IMap map, TerrainState state, MapCoordinates crd, bool fOn = true)
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
