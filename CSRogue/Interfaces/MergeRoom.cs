using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSRogue.Utilities;

namespace CSRogue.Map_Generation
{
	public class MergeRoom : Room
	{
		internal MergeRoom(char[][] layout, MapCoordinates location, List<Room> neighbors) :
			base(layout, location, neighbors) { }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Transfer terrain. </summary>
		///
		/// <remarks>	This only affects the _layout field.  Darrellp, 9/27/2011. </remarks>
		///
		/// <param name="room">		The room to be joined to this one. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void TransferTerrain(IRoom room)
		{
			// Locals
			int mapColumn;
			int localColumn;

			// For each original column
			for (localColumn = 0, mapColumn = room.Left(); localColumn < room.Width(); localColumn++, mapColumn++)
			{
				// More locals
				int mapRow;
				int localRow;

				// For each original row
				for (localRow = 0, mapRow = room.Top(); localRow < room.Height(); localRow++, mapRow++)
				{
					// Get the terrain for this location
					var character = room.Layout[localColumn][localRow];

					// Is this an exit?
					if (character >= 'a' && character <= 'z')
					{
						// Add it to our list of exits
						var iRoom = (char)(character - 'a');
						var roomExitedTo = room.NeighborRooms[iRoom];
						character = AddExit(roomExitedTo, new MapCoordinates(mapColumn, mapRow));
					}

					// Is it non-null?
					if (character != '\0')
					{
						// Place the new terrain
						this[mapColumn, mapRow] = character;
					}
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Updates the rooms connected to room to point to us. </summary>
		///
		/// <remarks>	Darrellp, 9/28/2011. </remarks>
		///
		/// <param name="room">	The room whose connected rooms are to be joined to this one. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void UpdateConnectedRoomsToPointToUs(Room room)
		{
			// For each exit in the old room
			foreach (var exitInfo in room.ExitMap)
			{
				// Get the room on the other side
				MapCoordinates location = exitInfo.Key;
				IRoom roomExitedTo = exitInfo.Value;
				int indexToUs = roomExitedTo.At(location) - 'a';

				// and point it back to us
				roomExitedTo.NeighborRooms[indexToUs] = this;
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Combine two generic rooms </summary>
		///
		/// <remarks>	Darrellp, 9/27/2011. </remarks>
		///
		/// <param name="room">	The room to be joined to this one. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal void CombineWith(Room room)
		{
			// Clone into a temporary room
			Room roomTemp = new Room(Layout, Location, NeighborRooms);

			// Get the new and old locations, sizes
			int newLeft = Math.Min(this.Left(), room.Left());
			int newRight = Math.Max(this.Right(), room.Right());
			int newTop = Math.Min(this.Top(), room.Top());
			int newBottom = Math.Max(this.Bottom(), room.Bottom());
			int newHeight = newBottom - newTop + 1;
			int newWidth = newRight - newLeft + 1;

			// Set our new location
			_location = new MapCoordinates(newLeft, newTop);

			// Clear our exits
			// They'll come back in from the cloned room
			_neighborRooms = new List<Room>();

			// Allocate a new layout array
			_layout = new char[newWidth][];

			// For each column
			for (int iColumn = 0; iColumn < newWidth; iColumn++)
			{
				// Allocate the column
				Layout[iColumn] = new char[newHeight];
			}

			// Move the clone's data into ourself
			TransferTerrain(roomTemp);

			// Move the other room's data into ourself
			TransferTerrain(room);

			// Update pointers for any rooms which connected to room
			UpdateConnectedRoomsToPointToUs(room);

			// Set up our exit map
			_exitMap = this.MapExitsToRooms();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Adds an exit to the room hooked to another room. </summary>
		///
		/// <remarks>	Darrellp, 9/28/2011. </remarks>
		///
		/// <param name="room">	The room to be joined to this one. </param>
		/// <param name="location">	The location the exit to room will be. </param>
		///
		/// <returns>	The character allocated for the new exit. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal char AddExit(Room room, MapCoordinates location)
		{
			char exitChar = (char)('a' + NeighborRooms.Count);

			NeighborRooms.Add(room);
			this[location] = exitChar;
			ExitMap[location] = room;
			return exitChar;
		}
	}
}
