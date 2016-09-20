using System.Collections.Generic;
using CSRogue.Interfaces;
using CSRogue.Map_Generation;

namespace CSRogue.Utilities
{
	public static class RoomExtensions
	{
		/// <summary>   The height of the room. </summary>
		public static int Height(this IRoom room)
		{
			return room.Layout[0].Length;
		}

		/// <summary>   The width of the room. </summary>
		public static int Width(this IRoom room)
		{
			return room.Layout.Length;
		}

		/// <summary>   The left coordinate for the room. </summary>
		public static int Left(this IRoom room)
		{
			return room.Location.Column;
		}

		/// <summary>  The top coordinate for the room. </summary>
		public static int Top(this IRoom room)
		{
			return room.Location.Row;
		}

		/// <summary>  The right coordinate for the room. </summary>
		public static int Right(this IRoom room)
		{
			return room.Left() + room.Width() - 1;
		}

		/// <summary>  The bottom coordinate for the room. </summary>
		public static int Bottom(this IRoom room)
		{
			return room.Top() + room.Height() - 1;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Returns an enumeration of all our tile positions. </summary>
		///
		/// <value>	The tile positions. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static IEnumerable<MapCoordinates> Tiles(this IRoom room)
		{
			for (var iRow = 0; iRow < room.Height(); iRow++)
			{
				for (var iColumn = 0; iColumn < room.Width(); iColumn++)
				{
					if (room.Layout[iColumn][iRow] == '.')
					{
						yield return new MapCoordinates(iColumn + room.Location.Column, iRow + room.Location.Row);
					}
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Indexer using global (map) row and column arguments. </summary>
		///
		/// <value>	The indexed item. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static char At(this IRoom room, int iCol, int iRow)
		{
			return room.Layout[iCol - room.Location.Column][iRow - room.Location.Row];
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Indexer using global (map) row and column arguments. </summary>
		///
		/// <value>	The indexed item. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static char At(this IRoom room, MapCoordinates crd)
		{
			return room.At(crd.Column, crd.Row);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Map exits (in local coordinates) to the rooms they exit to. </summary>
		///
		/// <remarks>	Darrellp, 9/27/2011. </remarks>
		///
		/// <param name="room">	The room to act on. </param>
		///
		/// <returns>	A dictionary mapping local coordinates to the corresponding room </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static Dictionary<MapCoordinates, IRoom> MapExitsToRooms(this IRoom room)
		{
			var exitMap = new Dictionary<MapCoordinates, IRoom>();

			// Clear out any current entries
			exitMap.Clear();

			// For each column
			for (var iColumn = 0; iColumn < room.Width(); iColumn++)
			{
				// For each Row
				for (var iRow = 0; iRow < room.Height(); iRow++)
				{
					// Get the current terrain character
					var thisCharacter = room.Layout[iColumn][iRow];

					// Is it an exit?
					if (thisCharacter >= 'a' && thisCharacter <= 'z')
					{
						// Get the corresponding room's index
						var iRoom = thisCharacter - 'a';

						// and map the location to the room it exits to
						exitMap[new MapCoordinates(iColumn, iRow) + room.Location] = room.NeighborRooms[iRoom];
					}
				}
			}
			return exitMap;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   An IRoom extension method that produces a djikstra map given a single goal cell. </summary>
        ///
        /// <remarks>   Darrell, 9/18/2016. </remarks>
        ///
        /// <param name="room"> The room to act on. </param>
        /// <param name="goal"> The goal cell location. </param>
        ///
        /// <returns>   The Djikstra map. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
	    public static int[][] DjikstraMap(this IRoom room, MapCoordinates goal)
	    {
            var goals = new Dictionary<int, List<MapCoordinates>>()
            {
                {0, new List<MapCoordinates>() {goal}}
            };
	        return room.DjikstraMap(goals);
	    }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// An IRoom extension method that produces a djikstra map given multiple goals and priorities.
        /// </summary>
        ///
        /// <remarks>   Darrell, 9/18/2016. </remarks>
        ///
        /// <param name="room">     The room to act on. </param>
        /// <param name="goals">    The goals. </param>
        ///
        /// <returns>   The Djikstra map. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static int[][] DjikstraMap(this IRoom room, Dictionary<int, List<MapCoordinates>> goals)
        {
            var djikstra = new DjikstraMap(room.Height(), room.Width(), goals, (c, r) =>
            {
                var tile = room.Layout[c][r];
                return tile == '.' || char.IsLetter(tile);
            });
            return djikstra.CreateMap();
        }
    }
}
