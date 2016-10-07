using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CSRogue.Interfaces;
using CSRogue.Utilities;

namespace CSRogue.Map_Generation
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Generic room. </summary>
	///
	/// <remarks>	
	/// The main difference . Darrellp, 8/25/2016
	/// </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public class Room : IRoom
	{
		#region Private properties
		private char[][] _layout;
		private MapCoordinates _location;
		private List<Room> _neighborRooms;
	    private List<MapCoordinates> _exits = new List<MapCoordinates>();
#if DEBUG
		internal Dictionary<MapCoordinates, IRoom> _exitMap;
#else
		private Dictionary<MapCoordinates, IRoom> _exitMap;
#endif
		private int[][][] _exitDjikstraMaps;
#endregion

#region IRoom Properties
		/// <summary>	The layout map. </summary>
		public char[][] Layout => _layout;

		/// <summary>	The location in the parent map where the upper left corner of this room is located. </summary>
		public MapCoordinates Location => _location;

		/// <summary>	The neighboring rooms. </summary>
		public List<Room> NeighborRooms => _neighborRooms;

		public List<MapCoordinates> Exits
		{
			get { return _exits; }
			set { _exits = value; }
		}

		public int[][][] ExitDMaps => _exitDjikstraMaps;

#endregion

#region Properties
		/// <summary>   Matches global coordinates for exits to the rooms they lead to. </summary>
		public Dictionary<MapCoordinates, IRoom> ExitMap => _exitMap ?? (_exitMap = this.MapExitsToRooms());

		int[][][] IRoom.ExitDMaps
		{
			get
			{
				return ExitDMaps;
			}

			set
			{
				_exitDjikstraMaps = value;
			}
		}

#endregion

#region Djikstra Mapping
		internal void SetupDjikstraMapExits()
	    {
		    _exitDjikstraMaps = new int[_exits.Count][][];
		    for (var iExit = 0; iExit < _exits.Count; iExit++)
		    {
			    var exitLocation = _exits[iExit];
                _exitDjikstraMaps[iExit] = this.DjikstraMap(exitLocation);
	        }
	    }

	    internal int ExitToExitDistance(int iExit1, int iExit2)
	    {
	        var exit2Location = _exits[iExit2];
	        return _exitDjikstraMaps[iExit1][exit2Location.Column][exit2Location.Row];
	    }
#endregion

#region Constructors
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>	Basic setup code called by all constructors. </summary>
        ///
        /// <remarks>	Darrellp, 9/28/2011. </remarks>
        ///
        /// <param name="layout">			The layout. </param>
        /// <param name="location">			The location. </param>
        /// <param name="neighbors">		The neighboring rooms. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        internal void Setup(char[][] layout, MapCoordinates location, List<Room> neighbors)
		{
			_layout = layout;
			_neighborRooms = neighbors ?? new List<Room>();
			_location = location;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Constructor from a 2D character layout. </summary>
		///
		/// <remarks>	Darrellp, 9/27/2011. </remarks>
		///
		/// <param name="layout">			The layout. </param>
		/// <param name="location">			The location. </param>
		/// <param name="neighbors">		The exits. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal Room(char[][] layout, MapCoordinates location, List<Room> neighbors)
		{
			Setup(layout, location, neighbors);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Constructor from just a width, height and location. </summary>
		///
		/// <remarks>	Darrellp, 9/28/2011. </remarks>
		///
		/// <param name="width">			The width. </param>
		/// <param name="height">			The height. </param>
		/// <param name="location">			The location. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal Room(int width, int height, MapCoordinates location)
		{
			var layout = new char[width][];
			for (var iColumn = 0; iColumn < width; iColumn++)
			{
				layout[iColumn] = new char[height];
			}
			Setup(layout, location, null);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Constructor from a string layout. </summary>
		///
		/// <remarks>	Darrellp, 9/27/2011. </remarks>
		///
		/// <param name="layout">			The layout. </param>
		/// <param name="location">			The location. </param>
		/// <param name="neighbors">		The exits. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal Room(string layout, MapCoordinates location, List<Room> neighbors)
		{
			var rows = layout.Split('\n').Select(s => s.TrimEnd('\r')).ToList();
			var layoutArray = new char[rows[0].Length][];


			for (var iColumn = 0; iColumn < layoutArray.Length; iColumn++)
			{
				layoutArray[iColumn] = new char[rows.Count];
				for (var iRow = 0; iRow < rows.Count; iRow++)
				{
					layoutArray[iColumn][iRow] = rows[iRow][iColumn];
				}
			}
			Setup(layoutArray, location, neighbors);
		}
#endregion

#region Queries
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Return true if a location is part of this room's floor. </summary>
		///
		/// <remarks>	Darrellp, 9/29/2011. </remarks>
		///
		/// <param name="location">	The location in global coordinates to be tested. </param>
		///
		/// <returns>	true if location is part of our room, false if not. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public bool ContainsPosition(MapCoordinates location)
		{
			return location.Column >= this.Left() && location.Column <= this.Right() &&
			       location.Row >= this.Top() && location.Row <= this.Bottom() &&
			       this[location] == '.';
		}
#endregion

#region Indexer
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Indexer using global (map) row and column arguments. </summary>
		///
		/// <value>	The indexed item. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public char this[int iCol, int iRow]
		{
			get
			{
				return Layout[iCol - Location.Column][iRow - Location.Row];
			}
			set
			{
				Layout[iCol - Location.Column][iRow - Location.Row] = value;
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Indexer using a MapCoordinates argument. </summary>
		///
		/// <value>	The indexed item. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public char this[MapCoordinates location]
		{
			get
			{
				var local = location - Location;
				return Layout[local.Column][local.Row];
			}
			set
			{
				var local = location - Location;
				Layout[local.Column][local.Row] = value;
			}
		}
#endregion

#region Modification
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Transfer terrain. </summary>
		///
		/// <remarks>	This only affects the _layout field.  Darrellp, 9/27/2011. </remarks>
		///
		/// <param name="room">		The room to be joined to this one. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void TransferTerrain(Room room)
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
						var iRoom = (char) (character - 'a');
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
		/// <summary>	Combine two generic rooms </summary>
		///
		/// <remarks>	Darrellp, 9/27/2011. </remarks>
		///
		/// <param name="room">	The room to be joined to this one. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal void CombineWith(Room room)
		{
			// Clone into a temporary room
			var roomTemp = new Room(Layout, Location, NeighborRooms);

			// Get the new and old locations, sizes
			var newLeft = Math.Min(this.Left(), room.Left());
			var newRight = Math.Max(this.Right(), room.Right());
			var newTop = Math.Min(this.Top(), room.Top());
			var newBottom = Math.Max(this.Bottom(), room.Bottom());
			var newHeight = newBottom - newTop + 1;
			var newWidth = newRight - newLeft + 1;

			// Set our new location
			_location = new MapCoordinates(newLeft, newTop);

			// Clear our exits
			// They'll come back in from the cloned room
			_neighborRooms = new List<Room>();
			Exits.Clear();

			// Allocate a new layout array
			_layout = new char[newWidth][];

			// For each column
			for (var iColumn = 0; iColumn < newWidth; iColumn++)
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
				var location = exitInfo.Key;
				var roomExitedTo = exitInfo.Value;
				var indexToUs = roomExitedTo.At(location) - 'a';

				// and point it back to us
				roomExitedTo.NeighborRooms[indexToUs] = this;
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Adds an exit to the room hooked to another generic room. </summary>
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
			var exitChar = (char) ('a' + NeighborRooms.Count);

			NeighborRooms.Add(room);
			this[location] = exitChar;
			Debug.Assert(_exitMap == null || !_exitMap.ContainsKey(new MapCoordinates(6, 8)));
			ExitMap[location] = room;
			Debug.Assert(_exitMap == null || !_exitMap.ContainsKey(new MapCoordinates(6, 8)));
			_exits.Add(location);
			return exitChar;
		}
#endregion

#region Display
		public override string ToString()
		{
			var sbret = new StringBuilder();
			for (var iRow = 0; iRow < this.Height(); iRow++)
			{
				for (var iCol = 0; iCol < this.Width(); iCol++)
				{
					var chNext = Layout[iCol][iRow];
					if (chNext == '\0')
					{
						chNext = ' ';
					}
					sbret.Append(chNext);
				}
				sbret.Append(Environment.NewLine);
			}

			return sbret.ToString();
		}
#endregion
	}
}
