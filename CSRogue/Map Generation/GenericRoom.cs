using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSRogue.Utilities;

namespace CSRogue.Map_Generation
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Generic room. </summary>
	///
	/// <remarks>	
	/// This encapsulates all we need to know about a room - namely, it's shape and how to get from
	/// it to other rooms.  The exits in this structure do not "belong" to this room - i.e., their
	/// position will correspond to floor in the adjoining room.  Every available floor space in the
	/// map will belong to one unique room.  The layout is a 2D array of characters (input in the
	/// constructor as a single string with rows separated by new lines).  Each character should be
	/// either wall or floor characters (obtained from TerrainFactory.TerrainToChar() ) or a lower
	/// case letter.  The letters correspond to exits.  Exit 'a' is an exit to the first room
	/// in the _exits list, exit 'b' is an exit to the second room, etc..  If you've got more than
	/// 26 exits in a single room, you're out of luck.  Darrellp, 9/27/2011.
    /// 
    /// Rooms exist in a larger "map" and Location gives their location on that map.  Thus two types
    /// of coordinates exist for a room - local cooredinates which index directly into Layout, and
    /// global or map coordinates which index into the map that the room is embedded in. Darrellp, 8/25/2016
	/// </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public class GenericRoom : IRoom
	{
		private char[][] _layout;
		private MapCoordinates _location;
		private List<GenericRoom> _exits;
		private Dictionary<MapCoordinates, GenericRoom> _exitMap;

		#region Private variables
		#endregion

		#region Public Properties

		public char[][] Layout => _layout;

		public MapCoordinates Location => _location;

		public List<GenericRoom> Exits => _exits;

		#endregion

		#region Properties

		/// <summary>   Matches global coordinates for exits to the rooms they lead to. </summary>
		public Dictionary<MapCoordinates, GenericRoom> ExitMap => _exitMap;

		/// <summary>   Gives coordinates for all the exits </summary>
		public IEnumerable<MapCoordinates> ExitCoordinates => ExitMap.Keys;

        /// <summary>   The neighboring rooms. </summary>
	    public IEnumerable<GenericRoom> NeighborRooms => ExitMap.Values;

        /// <summary>   Returns true if this is a corridor. </summary>
	    public virtual bool IsCorridor => false;

        /// <summary>   The width of the room's layout. </summary>
        public int Width => Layout.Length;

        /// <summary>   The height of the room's layout. </summary>
        public int Height => Layout[0].Length;

        /// <summary>   The left coordinate for the room. </summary>
        public int Left => Location.Column;

        /// <summary>   The top coordinate for the room. </summary>
        public int Top => Location.Row;

        /// <summary>   The right coordinate for the room. </summary>
        public int Right => Left + Width - 1;

        /// <summary>   The bottom coordinate for the room. </summary>
	    public int Bottom => Top + Height - 1;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a tag for the room. </summary>
        ///
        /// <remarks> Put anything in it you like </remarks>
        /// <value> The tag. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
		public object Tag { get; set; }
		#endregion

		#region Constructors
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Basic setup code called by all constructors. </summary>
		///
		/// <remarks>	Darrellp, 9/28/2011. </remarks>
		///
		/// <param name="layout">	The layout. </param>
		/// <param name="location">	The location. </param>
		/// <param name="exits">	The exits. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal void Setup(char[][] layout, MapCoordinates location, List<GenericRoom> exits)
		{
			_layout = layout;
			_exits = exits ?? new List<GenericRoom>();
			_location = location;
			_exitMap = this.MapExitsToRooms();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Constructor from a 2D character layout. </summary>
		///
		/// <remarks>	Darrellp, 9/27/2011. </remarks>
		///
		/// <param name="layout">	The layout. </param>
		/// <param name="location">	The location. </param>
		/// <param name="exits">	The exits. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal GenericRoom(char[][] layout, MapCoordinates location, List<GenericRoom> exits)
		{
			Setup(layout, location, exits);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Constructor from just a width, height and location. </summary>
		///
		/// <remarks>	Darrellp, 9/28/2011. </remarks>
		///
		/// <param name="width">	The width. </param>
		/// <param name="height">	The height. </param>
		/// <param name="location">	The location. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal GenericRoom(int width, int height, MapCoordinates location)
		{
			char[][] layout = new char[width][];
			for (int iColumn = 0; iColumn < width; iColumn++)
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
		/// <param name="layout">	The layout. </param>
		/// <param name="location">	The location. </param>
		/// <param name="exits">	The exits. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal GenericRoom(string layout, MapCoordinates location, List<GenericRoom> exits)
		{
			List<string> rows = layout.Split('\n').Select(s => s.TrimEnd('\r')).ToList();
			char[][] layoutArray = new char[rows[0].Length][];


			for (int iColumn = 0; iColumn < layoutArray.Length; iColumn++)
			{
				layoutArray[iColumn] = new char[rows.Count];
				for (int iRow = 0; iRow < rows.Count; iRow++)
				{
					layoutArray[iColumn][iRow] = rows[iRow][iColumn];
				}
			}
			Setup(layoutArray, location, exits);
		}

		protected GenericRoom()
		{
		}
		#endregion

		#region Queries
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Returns an enumeration of all our tile positions. </summary>
		///
		/// <value>	The tile positions. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public IEnumerable<MapCoordinates> Tiles
		{
			get
			{
				for (var iRow = 0; iRow < Height; iRow++)
				{
					for (var iColumn = 0; iColumn < Width; iColumn++)
					{
						if (Layout[iColumn][iRow] == '.')
						{
							yield return new MapCoordinates(iColumn + Location.Column, iRow + Location.Row);
						}
					}
				}
			}
		}

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
			return location.Column >= Left && location.Column <= Right &&
			       location.Row >= Top && location.Row <= Bottom &&
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
				MapCoordinates local = location - Location;
				return Layout[local.Column][local.Row];
			}
			set
			{
				MapCoordinates local = location - Location;
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
		/// <param name="groom">		The room to be joined to this one. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void TransferTerrain(GenericRoom groom)
		{
			// Locals
			int mapColumn;
			int localColumn;

			// For each original column
			for (localColumn = 0, mapColumn = groom.Left; localColumn < groom.Width; localColumn++, mapColumn++)
			{
				// More locals
				int mapRow;
				int localRow;

				// For each original row
				for (localRow = 0, mapRow = groom.Top; localRow < groom.Height; localRow++, mapRow++)
				{
					// Get the terrain for this location
					char character = groom.Layout[localColumn][localRow];

					// Is this an exit?
					if (character >= 'a' && character <= 'z')
					{
						// Add it to our list of exits
						int iRoom = (char) (character - 'a');
						GenericRoom groomExitedTo = groom.Exits[iRoom];
						character = AddExit(groomExitedTo, new MapCoordinates(mapColumn, mapRow));
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
		/// <param name="groom">	The room to be joined to this one. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal void CombineWith(GenericRoom groom)
		{
			// Clone into a temporary room
			GenericRoom roomTemp = new GenericRoom(Layout, Location, Exits);

			// Get the new and old locations, sizes
			int newLeft = Math.Min(Left, groom.Left);
			int newRight = Math.Max(Right, groom.Right);
			int newTop = Math.Min(Top, groom.Top);
			int newBottom = Math.Max(Bottom, groom.Bottom);
			int newHeight = newBottom - newTop + 1;
			int newWidth = newRight - newLeft + 1;

			// Set our new location
			_location = new MapCoordinates(newLeft, newTop);

			// Clear our exits
			// They'll come back in from the cloned room
			_exits = new List<GenericRoom>();

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
			TransferTerrain(groom);

			// Update pointers for any rooms which connected to groom
			UpdateConnectedRoomsToPointToUs(groom);

			// Set up our exit map
			_exitMap = this.MapExitsToRooms();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Updates the rooms connected to groom to point to us. </summary>
		///
		/// <remarks>	Darrellp, 9/28/2011. </remarks>
		///
		/// <param name="groom">	The room whose connected rooms are to be joined to this one. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void UpdateConnectedRoomsToPointToUs(GenericRoom groom)
		{
			// For each exit in the old room
			foreach (var exitInfo in groom.ExitMap)
			{
				// Get the room on the other side
				MapCoordinates location = exitInfo.Key;
				GenericRoom groomExitedTo = exitInfo.Value;
				int indexToUs = groomExitedTo[location] - 'a';

				// and point it back to us
				groomExitedTo.Exits[indexToUs] = this;
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Adds an exit to the room hooked to another generic room. </summary>
		///
		/// <remarks>	Darrellp, 9/28/2011. </remarks>
		///
		/// <param name="groom">	The room to be joined to this one. </param>
		/// <param name="location">	The location the exit to groom will be. </param>
		///
		/// <returns>	The character allocated for the new exit. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal char AddExit(GenericRoom groom, MapCoordinates location)
		{
			char exitChar = (char) ('a' + Exits.Count);

			Exits.Add(groom);
			this[location] = exitChar;
			ExitMap[location] = groom;
			return exitChar;
		}
		#endregion

		#region Display
		public override string ToString()
		{
			StringBuilder sbret = new StringBuilder();
			for (int iRow = 0; iRow < Height; iRow++)
			{
				for (int iCol = 0; iCol < Width; iCol++)
				{
					char chNext = Layout[iCol][iRow];
					if (chNext == '\0')
					{
						chNext = ' ';
					}
					sbret.Append(chNext);
				}
				sbret.Append("\r\n");
			}

			return sbret.ToString();
		}
		#endregion
	}
}
