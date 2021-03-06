﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CSRogue.Utilities;

namespace CSRogue.Map_Generation
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Grid excavator. </summary>
	///
	/// <remarks>	
	/// Excavates within a grid structure in a map.  
	/// 
	/// BEWARE!!!!  BEWARE!!!!  BEWARE!!!!  BEWARE!!!!  BEWARE!!!!  BEWARE!!!!  BEWARE!!!!
	/// 
	/// I hate duplicated code and lots and lots of this code would be essentially duplicated for
	/// horizontal and vertical cases normally.  To avoid this, I've written lots of non-directional
	/// routines which use the Dir enum to specify direction.  I make heavy use of them in this code.
	/// The problem is, when writing non-directional code, you really can't use the normal
	/// directional terms like "top", "bottom", "row" or "column".  I first tried writing everything
	/// in terms of low and high coordinates and perpindicular and parallel directions but this
	/// really obscured the meaning of what was going on.  At least for me, my brain wants to
	/// stubbornly think in directional terms.  Not only that, but the variable names had to really
	/// bloat to say "PerpindicularLargeCoordinate" rather than "right".  It's both difficult to
	/// write and difficult to read.  Finally, the definition of "perpindicular" and "parallel" is
	/// problematic.  If the direction is vertical, does "parallel" mean columns which run vertically
	/// or rows whose indices change verticall?  Also, if there are several directions involved,
	/// which is "the" direction these terms apply to?  So I finally gave up - at least on using
	/// these precise but awkward and confusing naming conventions.  Now variables are named assuming
	/// one direction - typically vertical.  So the low coordinate in the perpindicular direction
	/// becomes "left", etc..  These are imprecise naming conventions given the non-directionality of
	/// the code, but they are FAR easier to understand - at least in the case of my small brain.
	/// Beware, though! When a variable is named "top" it might actually be the left side when the
	/// routine is called with a direction of horizontal. Darrellp, 9/25/2011. 
	/// 
	/// Grid Excavator strategy
	/// The Grid excavator here divides the entire map into room "cells".  The cells are roughly
	/// evenly spaced both horizontally and vertically.  The baseCellWidth and baseCellHeight give
	/// the size of the cells.  Within each cell a randomly sized/positioned room is placed in it's
	/// interior.  This is done by LocateRooms().  After the location of the rooms is determined we
	/// call DetermineRoomConnections() to determine with rooms will be connected to which.  This
	/// uses a random spanning tree to ensure that all rooms are connected and then adds in a rew
	/// extra random connections so that we have some cycles in the room map.  If the rooms were
	/// connected with corridors at this point the grid structure would be very obvious.  To avoid
	/// this, we pick some of the connections and merge the two connected rooms rather than placing
	/// a corridor between them.  At this point, we've merely decided to merge them but nothing
	/// has actually been excavated yet.  This is done in DetermineRoomMerges().  Finally, each
	/// room is actually excavated on the map.  The last thing that is done is that each of the
	/// connections (both merges and corridors) are excavated and doors are placed.  Two rooms
	/// get merged just by picking a row/column between them and then extending them to merge
	/// at that row/column.  This requires certain overlap conditions to be met which are checked
	/// in DetermineRoomMerges.  If these conditions are not met then the rooms are not merged
	/// and are connected with corridors as usual.
	/// 
	/// Finally, walls are placed, the stairways are placed and the rooms are connected up.
	/// The corridors count as rooms and each room as a list of exits and where those exits are
	/// and which rooms the exits connect to.  Obviously, corridors have only two exits.  Thus
	/// the final structure is a graph of rooms and their connections along with information about
	/// the shape of each room and where it's exits are located.
	/// </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public class GridExcavator : IExcavator
	{
		#region Private variables
		private readonly int _baseCellWidth;
		private readonly int _baseCellHeight;
		private readonly int _minRoomWidth;
		private readonly int _minRoomHeight;
		private readonly int _pctMergeChance;
		private readonly int _pctDoorChance;
		private readonly int _seed;
		private RectangularRoom[][] _rooms;
		private GridConnections _connections;
		private GridConnections _merges;
		private Rnd _rnd;
		readonly Dictionary<RectangularRoom, GenericRoom> _mapRoomToGenericRooms = new Dictionary<RectangularRoom, GenericRoom>();
		#endregion

		#region Constructor
		public GridExcavator(
			int seed = -1,
			int baseCellWidth = 15,
			int baseCellHeight = 13,
			int minRoomWidth = 5,
			int minRoomHeight = 5,
			int pctMergeChance = 50,
			int pctDoorChance = 50)
		{
			_baseCellWidth = baseCellWidth;
			_baseCellHeight = baseCellHeight;
			_minRoomWidth = minRoomWidth;
			_minRoomHeight = minRoomHeight;
			_pctMergeChance = pctMergeChance;
			_pctDoorChance = pctDoorChance;
			_seed = seed;
		} 
		#endregion

		#region Main entry point
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Excavate rooms into the map by grid. </summary>
		///
		/// <remarks>	Darrellp, 9/18/2011. </remarks>
		///
		/// <exception cref="RogueException">	Thrown when the map is too small. </exception>
		///
		/// <param name="map">	The map to be excavated. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public void Excavate(IRoomsMap map)
		{
			// Seed the random number generator properly
			_rnd = new Rnd(_seed);

			// Are we too small to make a map?);
			if (map.Width < _baseCellWidth || map.Height < _baseCellHeight)
			{
				// Throw an exception
				throw new RogueException("Attempting to excavate with too small a map");
			}

			// Locate the rooms in the map
			LocateRooms(map);

			// Create our connections objects
			_connections = new GridConnections(_rooms.Length, _rooms[0].Length);
			_merges = new GridConnections(_rooms.Length, _rooms[0].Length);

			// Determine which rooms should be connected
			DetermineRoomConnections();

			// Determine which rooms should be merged and resize those rooms
			DetermineRoomMerges();

			// For each column in the grid
			foreach (var room in _rooms.SelectMany(roomRow => roomRow))
			{
				// Excavate the room
				ExcavateRoom(map, room);
			}

			// Excavate between the rooms
			ExcavateRoomConnections(map);

			// Do any cleanup
			PostProcess(map);
		}
		#endregion

		#region Grid layout
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Connects the rooms into a network. </summary>
		///
		/// <remarks>	Darrellp, 9/19/2011. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void DetermineRoomConnections()
		{
			// Connect all the grid cells
			_connections.ConnectCells(_rnd);

			// Add some random connections
			AddRandomConnections(Math.Min(_rooms.Length, _rooms[0].Length), _connections);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Excavate all corridors on the map. </summary>
		///
		/// <remarks>	Darrellp, 9/20/2011. </remarks>
		///
		/// <param name="map">	The map to be excavated. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void ExcavateRoomConnections(IRoomsMap map)
		{
			// For every connection
			foreach (var info in _connections.Connections.Where(ci => ci.IsConnected))
			{
				// Get the pertinent information
				MapCoordinates location1 = info.Location;
				MapCoordinates location2 = info.Location.NextLarger(info.Dir);
				RectangularRoom room1 = RoomAt(location1);
				RectangularRoom room2 = RoomAt(location2);

				// Are the rooms to be merged?
				if (_merges.IsConnected(location1, location2))
				{
					// merge them
					ExcavateMerge(map, room1, room2, info.Dir);
				}
				else
				{
					// Build a corridor between them
					ExcavateCorridor(map, room1, room2, info.Dir);
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Check overlap between two rooms. </summary>
		///
		/// <remarks>	Darrellp, 9/30/2011. </remarks>
		///
		/// <param name="topRoom">		The top room. </param>
		/// <param name="bottomRoom">	The bottom room. </param>
		/// <param name="dirOther">		The direction to check overlap along. </param>
		///
		/// <returns>	true if they overlap, false if they don't. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private static bool CheckOverlap(RectangularRoom topRoom, RectangularRoom bottomRoom, Dir dirOther)
		{
			if (topRoom == null || bottomRoom == null)
			{
				return false;
			}

			// Get the appropriate coordinates
			int topRoomsLeft = topRoom.Location[dirOther];
			int topRoomsRight = topRoomsLeft + topRoom.Size(dirOther) - 1;
			int bottomRoomsLeft = bottomRoom.Location[dirOther];
			int bottomRoomsRight = bottomRoomsLeft + bottomRoom.Size(dirOther) - 1;

			// Get the high and low points of the overlap
			int overlapLeft = Math.Max(topRoomsLeft, bottomRoomsLeft) + 1;
			int overlapRight = Math.Min(topRoomsRight, bottomRoomsRight) - 1;

			// return true if they overlap
			return overlapLeft < overlapRight;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Check a merge for overlap between the merging rooms. </summary>
		///
		/// <remarks>	
		/// Names are named as though dir was vertical and dirOther horizontal. Darrellp, 9/25/2011. 
		/// </remarks>
		///
		/// <param name="topRoom">		The top room. </param>
		/// <param name="bottomRoom">	The bottom room. </param>
		/// <param name="dirOther">		The dir other. </param>
		///
		/// <returns>	true if it succeeds, false if it fails. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private bool CheckMergeForOverlap(RectangularRoom topRoom, RectangularRoom bottomRoom, Dir dirOther)
		{
			// Get our neighboring rooms in the dirOther direction
			RectangularRoom topLeft = RoomAt(topRoom.GridLocation.NextSmaller(dirOther));
			RectangularRoom topRight = RoomAt(topRoom.GridLocation.NextLarger(dirOther));
			RectangularRoom bottomLeft = RoomAt(bottomRoom.GridLocation.NextSmaller(dirOther));
			RectangularRoom bottomRight = RoomAt(bottomRoom.GridLocation.NextLarger(dirOther));

			// Ensure that we overlap with our merge target and not with anything else
			return CheckOverlap(topRoom, bottomRoom, dirOther) &&
				   !CheckOverlap(topLeft, bottomRoom, dirOther) &&
				   !CheckOverlap(topRight, bottomRoom, dirOther) &&
				   !CheckOverlap(topRoom, bottomLeft, dirOther) &&
				   !CheckOverlap(topRoom, bottomRight, dirOther);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Excavate a merge between two rooms. </summary>
		///
		/// <remarks>	
		/// Names are named as though dir was vertical and dirOther horizontal. Darrellp, 9/22/2011. 
		/// </remarks>
		///
		/// <param name="map">			The map. </param>
		/// <param name="topRoom">		The top room. </param>
		/// <param name="bottomRoom">	The bottom room. </param>
		/// <param name="dir">			The dir. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void ExcavateMerge(IRoomsMap map, RectangularRoom topRoom, RectangularRoom bottomRoom, Dir dir)
		{
			// Get the opposite direction
			Dir dirOther = MapCoordinates.OtherDirection(dir);

			// Are the rooms unmergable?
			if (!CheckOverlap(topRoom, bottomRoom, dirOther))
			{
				// Should have caught this in MergeTwoRooms - throw exception
				throw new RogueException("Non-overlapping rooms made it to ExcavateMerge");
			}

			// Get the appropriate coordinates
			int topRoomsLeft = topRoom.Location[dirOther];
			int topRoomsRight = topRoomsLeft + topRoom.Size(dirOther) - 1;
			int bottomRoomsLeft = bottomRoom.Location[dirOther];
			int bottomRoomsRight = bottomRoomsLeft + bottomRoom.Size(dirOther) - 1;

			// Get the high and low points of the overlap
			int overlapLeft = Math.Max(topRoomsLeft, bottomRoomsLeft) + 1;
			int overlapRight = Math.Min(topRoomsRight, bottomRoomsRight) - 1;

			// Create our new merged generic room
			GenericRoom groomTop = _mapRoomToGenericRooms[topRoom];
			GenericRoom groomBottom = _mapRoomToGenericRooms[bottomRoom];
			groomTop.CombineWith(groomBottom);

			// For each column in the grid
			foreach (RectangularRoom[] roomColumn in _rooms)
			{
				// For each row in the grid
				for (int iRow = 0; iRow < _rooms[0].Length; iRow++)
				{
					// Get the rect room at that spot
					RectangularRoom room = roomColumn[iRow];

					// Is it mapped to our defunct bottom room?
					if (_mapRoomToGenericRooms[room] == groomBottom)
					{
						// Map it to our shiny new top room
						_mapRoomToGenericRooms[room] = groomTop;
					}
				}
			}

			// Get the location we're going to start the clearing at
			int topRoomsBottom = topRoom.Location[dir] + topRoom.Size(dir) - 1;
			MapCoordinates currentLocation = MapCoordinates.CreateUndirectional(topRoomsBottom, overlapLeft, dir);
			char floorChar = TerrainFactory.TerrainToChar(TerrainType.Floor);

			// For each spot along the overlap
			for (int iCol = overlapLeft; iCol <= overlapRight; iCol++)
			{
				// Clear out the two walls of the abutting rooms
				currentLocation[dirOther] = iCol;
			    map[currentLocation].Terrain = TerrainType.Floor;
				groomTop[currentLocation] = floorChar;
				currentLocation[dir] = topRoomsBottom + 1;
                map[currentLocation].Terrain = TerrainType.Floor;
				groomTop[currentLocation] = floorChar;
				currentLocation[dir] = topRoomsBottom;
			}
			Debug.Assert(groomBottom == groomTop || !_mapRoomToGenericRooms.ContainsValue(groomBottom));
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Add random connections to the rooms. </summary>
		///
		/// <remarks>	Darrellp, 9/20/2011. </remarks>
		///
		/// <param name="connectionCount">	Number of random connections to add. </param>
		/// <param name="connections">		The room connections. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void AddRandomConnections(int connectionCount, GridConnections connections)
		{
			// For the number of random connections to be added
			for (int iConnection = 0; iConnection < connectionCount; iConnection++)
			{
				// Add a random connection...
				connections.MakeRandomConnection(_rnd);
			}
		} 
		#endregion

		#region Excavation
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Merge rooms. </summary>
		///
		/// <remarks>	
		/// This doesn't actually excavate the two rooms - it just resizes them so that they will merge
		/// properly and marks them down to be merged in the merge grid structure.  Darrellp, 9/25/2011. 
		/// </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void DetermineRoomMerges()
		{
			// For each connection
			foreach (var connectionInfo in _connections.Connections.Where(ci => ci.IsConnected))
			{
				// Is it a connection that we wish to merge?
				if (_rnd.Next(100) < _pctMergeChance)
				{
					// Retrieve it's location
					MapCoordinates gridLocation = connectionInfo.Location;

					// and merge the rooms
					MergeTwoRooms(gridLocation, connectionInfo.Dir);
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Merge two rooms. </summary>
		///
		/// <remarks>	Named as though dir was vertical.  Darrellp, 9/22/2011. </remarks>
		///
		/// <param name="topRoomsGridLocation">	The location of the top room in the grid. </param>
		/// <param name="dir">					The direction of the merge. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void MergeTwoRooms(MapCoordinates topRoomsGridLocation, Dir dir)
		{
			// Get grid coordinates of the other room
			MapCoordinates bottomRoomsGridLocation = topRoomsGridLocation.NextLarger(dir);
			Dir otherDir = MapCoordinates.OtherDirection(dir);

			// Retrieve both rooms
			RectangularRoom topRoom = RoomAt(topRoomsGridLocation);
			RectangularRoom bottomRoom = RoomAt(bottomRoomsGridLocation);

			// Are the rooms unmergable?
			if (!CheckMergeForOverlap(topRoom, bottomRoom, otherDir))
			{
				// Return and don't merge them - use normal corridors
				return;
			}

			// Remove their generic counterparts
			_mapRoomToGenericRooms.Remove(topRoom);
			_mapRoomToGenericRooms.Remove(bottomRoom);

			// Get their current coordinates, etc.
			int topRoomsBottom = topRoom.LargeCoord(dir);
			int topRoomsTop = topRoom.SmallCoord(dir);
			int bottomRoomsTop = bottomRoom.SmallCoord(dir);
			int bottomRoomsHeight = bottomRoom.Size(dir);
			int topRoomsWidth = topRoom.Size(otherDir);
			int bottomRoomsWidth = bottomRoom.Size(otherDir);

			// Pick a random spot between the rooms to merge them
			// This will be the new inside coord of the small coordinate room
			int mergeRow = _rnd.Next(topRoomsBottom, bottomRoomsTop);

			// Determine all the new coordinates
			int topRoomsNewHeight = mergeRow - topRoomsTop + 1;
			int bottomRoomsNewHeight = bottomRoomsTop - mergeRow + bottomRoomsHeight - 1;
			MapCoordinates bottomRoomsLocation = bottomRoom.Location;
			bottomRoomsLocation[dir] = mergeRow + 1;

			// Create our new expanded rooms
			RectangularRoom roomTopNew = RectangularRoom.CreateUndirectional(
				topRoom.Location,
				topRoomsNewHeight,
				topRoomsWidth,
				topRoomsGridLocation[dir],
				topRoomsGridLocation[otherDir],
				dir);
			RectangularRoom roomBottomNew = RectangularRoom.CreateUndirectional(
				bottomRoomsLocation,
				bottomRoomsNewHeight,
				bottomRoomsWidth,
				bottomRoomsGridLocation[dir],
				bottomRoomsGridLocation[otherDir],
				dir);

			// Install the new rooms
			SetRoomAt(topRoomsGridLocation, roomTopNew);
			SetRoomAt(bottomRoomsGridLocation, roomBottomNew);

			// Create the new generic rooms
			// We don't create the single merged generic room until we excavate because we don't know
			// the layout until then.
			_mapRoomToGenericRooms[roomTopNew] = roomTopNew.ToGeneric();
			_mapRoomToGenericRooms[roomBottomNew] = roomBottomNew.ToGeneric();

			// Mark this in our merges structure
			_merges.Connect(topRoomsGridLocation, bottomRoomsGridLocation);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Excavate a room, putting walls on the border. </summary>
		///
		/// <remarks>	Darrellp, 9/19/2011. </remarks>
		///
		/// <param name="map">	The map to be excavated. </param>
		/// <param name="room">	The room to carve out of the map. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private static void ExcavateRoom(IRoomsMap map, RectangularRoom room)
		{
			// For each column in the room
			for (var iColumn = room.Left + 1; iColumn < room.Right; iColumn++)
			{
				// For each row in the room
				for (var iRow = room.Top + 1; iRow < room.Bottom; iRow++)
				{
					// Place the appropriate terrain
					map[iColumn, iRow].Terrain = TerrainType.Floor;
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Excavate a straight corridor run either vertically or horizontally. </summary>
		///
		/// <remarks>	
		/// Excavates from (startParallel, perpindicular) to (endParallel, perpindicular) inclusive if
		/// fVertical.  If not fVertical, swap coordinates.  start and end parallel coordinates do not
		/// have to be in numerical order.  This is a unidirectional function but, as usual, names are
		/// named as though dir was vertical.  Darrellp, 9/19/2011. 
		/// </remarks>
		///
		/// <param name="map">		The map to be excavated. </param>
		/// <param name="column">	The perpindicular coordinate. </param>
		/// <param name="endRow1">	The starting parallel coordinate. </param>
		/// <param name="endRow2">	The ending parallel coordinate. </param>
		/// <param name="groom">	The room being prepared for this corridor. </param>
		/// <param name="dir">		The direction of the corridor. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private static void ExcavateCorridorRun(IRoomsMap map, int column, int endRow1, int endRow2, GenericRoom groom, Dir dir)
		{
			// We work with small and large coords rather than start and end
			int startRow = Math.Min(endRow1, endRow2);
			int endRow = Math.Max(endRow1, endRow2);
			char floorChar = TerrainFactory.TerrainToChar(TerrainType.Floor);

			// Create the starting location
			MapCoordinates currentLocation = MapCoordinates.CreateUndirectional(startRow, column, dir);

			// For each row in the run
			for (int iRow = startRow; iRow <= endRow; iRow++)
			{
				// Place our terrain
				currentLocation[dir] = iRow;
			    map[currentLocation].Terrain = TerrainType.Floor;
				groom[currentLocation] = floorChar;
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Excavate bend. </summary>
		///
		/// <remarks>	
		/// "Perpindicular" here refers to the coordinate perpindicular to the orientation of the two
		/// rooms.  If the rooms are oriented vertically then perpindicular refers to the horizontal (x)
		/// coordinate wo that startPerpindicular is the starting column. If they're oriented vertically,
		/// startPerpindicular is the starting row.  Parallel refers to the coordinate parallel to the
		/// orientation.  Bend is always in the perpindicular coordinate. Unidirectional but named as
		/// though dir was vertical.  Darrellp, 9/18/2011. 
		/// </remarks>
		///
		/// <param name="map">			The map to be excavated. </param>
		/// <param name="startColumn">	The start perpindicular. </param>
		/// <param name="endColumn">	The end perpindicular. </param>
		/// <param name="startRow">		The start parallel. </param>
		/// <param name="endRow">		The end parallel. </param>
		/// <param name="bend">			The bend coordinate. </param>
		/// <param name="groom">		The room being prepared for this corridor. </param>
		/// <param name="dir">			The direction the bend is supposed to run. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private static void ExcavateBend(IRoomsMap map, int startColumn, int endColumn, int startRow, int endRow, int bend, GenericRoom groom, Dir dir)
		{
			Dir otherDir = MapCoordinates.OtherDirection(dir);

			// Create corridor to the bend
			ExcavateCorridorRun(map, startColumn, startRow, bend, groom, dir);

			// Create the cross corridor at the bend
			ExcavateCorridorRun(map, bend, startColumn, endColumn, groom, otherDir);

			// Create the corridor from the bend to the destination
			ExcavateCorridorRun(map, endColumn, bend, endRow, groom, dir);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Excavates between two rooms. </summary>
		///
		/// <remarks>	Variables named from the perspective of dir being vertical.  Darrellp, 9/18/2011. </remarks>
		///
		/// <param name="map">			The map to be excavated. </param>
		/// <param name="roomTop">		The first room. </param>
		/// <param name="roomBottom">	The second room. </param>
		/// <param name="dir">			The direction to excavate in. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void ExcavateCorridor(IRoomsMap map, RectangularRoom roomTop, RectangularRoom roomBottom, Dir dir)
		{
			// Locals
			MapCoordinates bottomEntrance, topEntrance;

			// Get the entrances to each room
			GetEntrances(roomTop, roomBottom, dir, out topEntrance, out bottomEntrance);

			// Allocate the generic room
			int genericWidth = Math.Abs(topEntrance.Column - bottomEntrance.Column) + 1;
			int genericHeight = Math.Abs(topEntrance.Row - bottomEntrance.Row) + 1;
			int genericLeft = Math.Min(topEntrance.Column, bottomEntrance.Column);
			int genericBottom = Math.Min(topEntrance.Row, bottomEntrance.Row);
			MapCoordinates genericLocation = new MapCoordinates(genericLeft, genericBottom);
			GenericCorridor corridor = new GenericCorridor(genericWidth, genericHeight, genericLocation);

			// Excavate a connection between the two rooms
			CreateBend(map, dir, topEntrance, bottomEntrance, corridor);

			// Put the exits in the appropriate generic rooms
			GenericRoom groomTop = _mapRoomToGenericRooms[roomTop];
			GenericRoom groomBottom = _mapRoomToGenericRooms[roomBottom];
			corridor.AddExit(groomTop, topEntrance);
			corridor.AddExit(groomBottom, bottomEntrance);
			groomTop.AddExit(corridor, topEntrance);
			groomBottom.AddExit(corridor, bottomEntrance);


			// Should we put a door in the top room?
			if (_rnd.Next(100) < _pctDoorChance)
			{
				// Place the door
				map[topEntrance].Terrain = TerrainType.Door;
			}

			// Should we put a door in the bottom room?
			if (_rnd.Next(100) < _pctDoorChance)
			{
                // Place the door
                map[bottomEntrance].Terrain = TerrainType.Door;
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Excavate between connected rooms in a grid connection. </summary>
		///
		/// <remarks>	Variables named from the perspective that dir is vertical Darrellp, 9/19/2011. </remarks>
		///
		/// <param name="map">				The map to be excavated. </param>
		/// <param name="dir">				The direction the merge will take place in. </param>
		/// <param name="topEntrance">		The small coordinate entrance. </param>
		/// <param name="bottomEntrance">	The large coordinate entrance. </param>
		/// <param name="groom">			The room being prepared for this corridor. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void CreateBend(IRoomsMap map, Dir dir, MapCoordinates topEntrance, MapCoordinates bottomEntrance, GenericRoom groom)
		{
			// locals
			Dir otherDir = MapCoordinates.OtherDirection(dir);
			int startRow = topEntrance[dir];
			int endRow = bottomEntrance[dir];
			int startColumn = topEntrance[otherDir];
			int endColumn = bottomEntrance[otherDir];

			// Determine bend location
			int bendRow = _rnd.Next(startRow + 1, endRow);

			// Excavate the bend between the two rooms
			ExcavateBend(map, startColumn, endColumn, startRow, endRow, bendRow, groom, dir);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Determine entrances for two connected rooms in a grid connection. </summary>
		///
		/// <remarks>	Variables named from the perspective that dir is vertical.  Darrellp, 9/19/2011. </remarks>
		///
		/// <param name="room1">			The first room. </param>
		/// <param name="room2">			The second room. </param>
		/// <param name="dir">				The direction we're excavating. </param>
		/// <param name="topEntrance">		[out] The small coordinate entrance. </param>
		/// <param name="bottomEntrance">	[out] The large coordinate entrance. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void GetEntrances(
			RectangularRoom room1,
			RectangularRoom room2,
			Dir dir,
			out MapCoordinates topEntrance,
			out MapCoordinates bottomEntrance)
		{
			// Determine room order in the orientation direction
			RectangularRoom topRoom = room1;
			RectangularRoom bottomRoom = room2;
			int iGrid1 = room1.GridCoord(dir);
			int iGrid2 = room2.GridCoord(dir);

			// Is the coordinate for room 1 less than room 2?
			if (iGrid1 > iGrid2)
			{
				// Set large coordinate to room1, small to room2
				bottomRoom = room1;
				topRoom = room2;
			}

			// Determine entrances for each room
			topEntrance = topRoom.PickSpotOnWall(_rnd, dir == Dir.Vert ? Wall.Bottom : Wall.Right);
			bottomEntrance = bottomRoom.PickSpotOnWall(_rnd, dir == Dir.Vert ? Wall.Top : Wall.Left);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Locates the rooms on the map. </summary>
		///
		/// <remarks>	Darrellp, 9/19/2011. </remarks>
		///
		/// <param name="map">	The map to be excavated. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void LocateRooms(IRoomsMap map)
		{
			// Max number of cells we can fit with at least base cell size
			int gridWidth = map.Width / _baseCellWidth;
			int gridHeight = map.Height / _baseCellHeight;

			// Size of cells we can manage total
			int baseRoomWidth = map.Width / gridWidth;
			int baseRoomHeight = map.Height / gridHeight;

			// Remainders for Bresenham algorithm
			int widthRemainder = map.Width % baseRoomWidth;
			int heightRemainder = map.Height % baseRoomHeight;

			// Tally for Bresenham
			int widthTally = gridWidth / 2;
			int heightTally = gridHeight / 2;

			// First column is on the left
			int mapColumn = 0;

			// Array of rooms in the grid
			_rooms = new RectangularRoom[gridWidth][];

			// For each grid column
			for (int gridColumn = 0; gridColumn < gridWidth; gridColumn++)
			{
				// Reset the map row to 0
				int mapRow = 0;

				// Determine the current map column
				int currentWidth = baseRoomWidth;
				widthTally += widthRemainder;

				// Do we need to bump width ala Bresenham?
				if (widthTally >= gridWidth)
				{
					// Bump width
					currentWidth++;
					widthTally -= gridWidth;
				}

				// Create the row of rooms for this column
				_rooms[gridColumn] = new RectangularRoom[gridHeight];

				// For each row of the grid
				for (int gridRow = 0; gridRow < gridHeight; gridRow++)
				{
					// Determine the current map row
					int currentHeight = baseRoomHeight;
					heightTally += heightRemainder;

					// Do we need to bump height ala Bresenham?
					if (heightTally >= gridHeight)
					{
						// Bump height
						currentHeight++;
						heightTally -= gridHeight;
					}

					// Create a room in the grid cell
					MapCoordinates gridLocation = new MapCoordinates(gridColumn, gridRow);
					MapCoordinates cellLocation = new MapCoordinates(mapColumn, mapRow);
					RectangularRoom room = CreateRoomInCell(gridLocation, cellLocation, currentWidth, currentHeight);

					// Place it in the grid
					_rooms[gridColumn][gridRow] = room;

					// Advance the map row
					mapRow += currentHeight;
				}

				// Advance the map column
				mapColumn += currentWidth;
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Creates a room in a grid cell. </summary>
		///
		/// <remarks>	Darrellp, 9/20/2011. </remarks>
		///
		/// <param name="gridLocation">	The grid location of the cell. </param>
		/// <param name="cellLocation">	The cell location in the map. </param>
		/// <param name="cellWidth">	Width of the current cell. </param>
		/// <param name="cellHeight">	Height of the current cell. </param>
		///
		/// <returns>	The newly created room. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private RectangularRoom CreateRoomInCell(MapCoordinates gridLocation, MapCoordinates cellLocation, int cellWidth, int cellHeight)
		{
			// Locals
			int startRow, startColumn, endRow, endColumn;

			// Determine start and end columns
			_rnd.RandomSpan(cellLocation.Column + 1, cellLocation.Column + cellWidth - 2, _minRoomWidth, out startColumn, out endColumn);

			// Determine start and end rows
			_rnd.RandomSpan(cellLocation.Row + 1, cellLocation.Row + cellHeight - 2, _minRoomHeight, out startRow, out endRow);
			MapCoordinates mapLocation = new MapCoordinates(startColumn, startRow);

			// Return newly created room
			RectangularRoom room = new RectangularRoom(mapLocation, gridLocation, endColumn - startColumn + 1, endRow - startRow + 1);
			_mapRoomToGenericRooms[room] = room.ToGeneric();

			return room;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Post process. </summary>
		///
		/// <remarks>	
		/// Put in final walls, etc. based on the excavation that was done in the main part of the
		/// excavation process.  Darrellp, 9/25/2011. 
		/// </remarks>
		///
		/// <param name="map">	The map. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void PostProcess(IRoomsMap map)
		{
			// Complete all the walls
			CompleteWalls(map);

			// Place room info
			AssignRoomsToCells(map);

			// Place stairs
			PlaceStairs(map);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Assigns a room to each floor spot on the map and adds all the rooms to the map. </summary>
		///
		/// <remarks>	Darrellp, 9/29/2011. </remarks>
		///
		/// <param name="map">	The map. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void AssignRoomsToCells(IRoomsMap map)
		{
			// For each room
			foreach (var groom in _mapRoomToGenericRooms.Values)
			{
				// Add in adjoining corridors
				// Corridors were never added to _mapRoomToGenericRooms so we have
				// to add them here

				// For each adjoining room/corridor
				foreach (var groomAdjoin in groom.NeighborRooms)
				{
					// Add the corridor to the map
					AssignTilesInRoom(map, groomAdjoin);
				}

				// Add the room to the map
				AssignTilesInRoom(map, groom);
			}
		}

		private static void AssignTilesInRoom(IRoomsMap map, GenericRoom groom)
		{
			// Is this room being newly added to the map?
			if (map.Rooms.Add(groom))
			{
				// For each tile in the room
				foreach (var tileLocation in groom.Tiles)
				{
					// Assign the room to that tile
					map[tileLocation].Room = groom;
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Complete walls around any off map sites next to floors. </summary>
		///
		/// <remarks>	Darrellp, 9/26/2011. </remarks>
		///
		/// <param name="map">	The map. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private static void CompleteWalls(IRoomsMap map)
		{
			for (int iRow = 0; iRow < map.Height; iRow++)
			{
				// For each Column
				for (int iColumn = 0; iColumn < map.Width; iColumn++)
				{
					// Is there a no floor here?
					if (map[iColumn, iRow].Terrain != TerrainType.Floor)
					{
						continue;
					}

					// Get the neighboring off map locations
					var offmapLocations = map.Neighbors(iColumn, iRow).Where(location => map[location].Terrain == TerrainType.OffMap);

					// For each neighboring off map location
					foreach (var location in offmapLocations)
					{
						// Turn it into stone wall
						map[location].Terrain = TerrainType.Wall;
					}
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Place the stairs. </summary>
		///
		/// <remarks>	Darrellp, 9/26/2011. </remarks>
		///
		/// <param name="map">	The map. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void PlaceStairs(IRoomsMap map)
		{
			RectangularRoom downRoom = RoomAt(_connections.FirstRoomConnected);
			RectangularRoom upRoom = RoomAt(_connections.LastRoomConnected);
			MapCoordinates downLocation = downRoom.PickSpotInRoom(_rnd);
			MapCoordinates upLocation = upRoom.PickSpotInRoom(_rnd);

		    map[downLocation].Terrain = TerrainType.StairsDown;
		    map[upLocation].Terrain = TerrainType.StairsUp;
		}
		#endregion

		#region Accessing
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Checks whether a grid location is within the bounds of our grid. </summary>
		///
		/// <remarks>	Darrellp, 9/30/2011. </remarks>
		///
		/// <param name="gridLocation">	The grid location. </param>
		///
		/// <returns>	true if it's in bounds, false if not. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private bool WithinGrid(MapCoordinates gridLocation)
		{
			int column = gridLocation.Column;
			int row = gridLocation.Row;
			return column >= 0 && column < _rooms.Length &&
			       row >= 0 && row < _rooms[0].Length;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Retrieve the room at a grid location. </summary>
		///
		/// <remarks>	Darrellp, 9/23/2011. </remarks>
		///
		/// <param name="gridLocation">	The grid location. </param>
		///
		/// <returns>	The room occupying that grid location. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private RectangularRoom RoomAt(MapCoordinates gridLocation)
		{
			return !WithinGrid(gridLocation) ? null : _rooms[gridLocation.Column][gridLocation.Row];
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Sets a room at a grid location. </summary>
		///
		/// <remarks>	Darrellp, 9/23/2011. </remarks>
		///
		/// <param name="gridLocation">	The grid location. </param>
		/// <param name="room">			The room to be placed there.. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void SetRoomAt(MapCoordinates gridLocation, RectangularRoom room)
		{
			_rooms[gridLocation.Column][gridLocation.Row] = room;
		}
		#endregion
	}
}
