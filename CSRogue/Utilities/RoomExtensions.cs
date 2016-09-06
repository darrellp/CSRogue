using System.Collections.Generic;
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

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Map exits (in local coordinates) to the rooms they exit to. </summary>
		///
		/// <remarks>	Darrellp, 9/27/2011. </remarks>
		///
		/// <param name="room">	The room to act on. </param>
		///
		/// <returns>	A dictionary mapping local coordinates to the corresponding room </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public static Dictionary<MapCoordinates, GenericRoom> MapExitsToRooms(this IRoom room)
		{
			var exitMap = new Dictionary<MapCoordinates, GenericRoom>();

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
						exitMap[new MapCoordinates(iColumn, iRow) + room.Location] = room.Exits[iRoom];
					}
				}
			}
			return exitMap;
		}
	}
}
