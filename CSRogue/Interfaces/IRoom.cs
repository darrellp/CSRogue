using System.Collections.Generic;
using CSRogue.Map_Generation;

namespace CSRogue.Interfaces
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Room interface. </summary>
	///
	/// <remarks>	
	/// This encapsulates all we need to know about a room - namely, it's shape and how to get from
	/// it to other rooms.  The exits in this structure do not "belong" to this room - i.e., their
	/// position will correspond to floor in the adjoining room.  Every available floor space in the
	/// map will belong to one unique room.  The layout is a 2D array of characters (input in the
	/// constructor as a single string with rows separated by new lines).  Each character should be
	/// either wall or floor characters (obtained from TerrainFactory.TerrainToChar() ) or a lower
	/// case letter.  The letters correspond to exits.  Exit 'a' is an exit to the first room
	/// in the NeighborRooms list, exit 'b' is an exit to the second room, etc..  If you've got more than
	/// 26 exits in a single room, you're out of luck.  Darrellp, 9/27/2011.
	/// 
	/// Rooms exist in a larger IRoomsMap and Location gives their location on that map.  Thus two types
	/// of coordinates exist for a room - local cooredinates which index directly into Layout, and
	/// global or map coordinates which index into the map that the room is embedded in. Darrellp, 8/25/2016
	/// </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public interface IRoom
	{
		char[][] Layout { get; }
		MapCoordinates Location { get; }
		List<Room> NeighborRooms { get; }
		// Room exits in GLOBAL MapCoordinates
		List<MapCoordinates> Exits { get; set; }
		// The following should be null if you don't use Heirarchical AStar
		int[][][] ExitDMaps { get; set; }
	}
}