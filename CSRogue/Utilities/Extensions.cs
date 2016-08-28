using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSRogue.Map_Generation;

namespace CSRogue.Utilities
{
    public static class Extensions
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

            for (int iRowOffset = minRowOffset; iRowOffset <= maxRowOffset; iRowOffset++)
            {
                for (int iColumnOffset = minColumnOffset; iColumnOffset <= maxColumnOffset; iColumnOffset++)
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


    }
}
