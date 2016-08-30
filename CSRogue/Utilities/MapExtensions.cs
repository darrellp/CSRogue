using System.Collections.Generic;
using System.Linq;
using CSRogue.GameControl;
using CSRogue.Items;
using CSRogue.Item_Handling;
using CSRogue.Map_Generation;
using CSRogue.RogueEventArgs;

namespace CSRogue.Utilities
{
    public static class MapExtensions
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>	Return all neighbors of a cell taking the borders into account. </summary>
        ///
        /// <remarks>	Darrellp, 9/26/2011. </remarks>
        ///
        /// <param name="column">	The location's columns. </param>
        /// <param name="row">		The location's row. </param>
        ///
        /// <returns>	An enumerable of all neighbors. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static IEnumerable<MapCoordinates> Neighbors(this IMap map, int column, int row)
        {
            int minRowOffset = row == 0 ? 0 : -1;
            int maxRowOffset = row == map.Height - 1 ? 0 : 1;
            int minColumnOffset = column == 0 ? 0 : -1;
            int maxColumnOffset = column == map.Width - 1 ? 0 : 1;

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
            // Locals
            Rnd rnd = Rnd.Global;
            int row, column;
            MapLocationData data;

            do
            {
                // Try a random spot
                row = rnd.Next(map.Height);
                column = rnd.Next(map.Width);
                data = map[column, row];
            }
            // We find one that's on some floor terrain
            while (data.Terrain != TerrainType.Floor && (!restrictToRooms || !data.Room.IsCorridor));

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
        /// <param name="location">	The location. </param>
        /// <param name="item">		The item to drop. </param>
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
        /// <param name="iRow">	The row to remove from. </param>
        /// <param name="iCol">	The col to remove from. </param>
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
        /// <summary>	Checks the terrain to see if the creature can move there. </summary>
        ///
        /// <remarks>	Darrellp, 10/15/2011. </remarks>
        ///
        /// <param name="location">	The location to check. </param>
        ///
        /// <returns>	true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool IsWalkable(this IMap map, MapCoordinates location)
        {
            return map[location].Terrain != TerrainType.Wall;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>	Returns true if we want the creature to continue running in this direction. </summary>
        ///
        /// <remarks>	Darrellp, 10/15/2011. </remarks>
        ///
        /// <param name="location">	The location to be checked. </param>
        ///
        /// <returns>	true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool ValidRunningMove(this IMap map, MapCoordinates location)
        {
            return map.IsWalkable(location) && !map.IsCreatureAt(location);
        }
    }
}
