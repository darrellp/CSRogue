using System.Collections.Generic;
using CSRogue.Utilities;

namespace CSRogue.Map_Generation
{
	enum Wall
	{
		Left,
		Right,
		Top,
		Bottom
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Represents a rectangular room. </summary>
	///
	/// <remarks>	Darrellp, 9/18/2011. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	class RectangularRoom
	{
		#region Internal properties
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the location of the upper left corner. </summary>
		///
		/// <value>	The location. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal MapCoordinates Location { get; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the grid location. </summary>
		///
		/// <value>	The grid location. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal MapCoordinates GridLocation { get; private set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the grid column the room occupies. </summary>
		///
		/// <value>	The grid column. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal int GridColumn { get { return GridLocation.Column; }}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the grid row the room occupies. </summary>
		///
		/// <value>	The grid row. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal int GridRow{ get { return GridLocation.Row;}}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the width of the room. </summary>
		///
		/// <value>	The width. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal int Width { get; private set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the height of the room. </summary>
		///
		/// <value>	The height. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal int Height { get; private set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets the top row. </summary>
		///
		/// <value>	The top row. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal int Top { get {return Location.Row;} }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets the left column. </summary>
		///
		/// <value>	The left column. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal int Left { get {return Location.Column;}}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets the bottom row. </summary>
		///
		/// <value>	The bottom row. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal int Bottom
		{
			get
			{
				return Location.Row + Height - 1;
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets the right column. </summary>
		///
		/// <value>	The right column. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal int Right
		{
			get
			{
				return Location.Column + Width - 1;
			}
		}
		#endregion

		#region Constructor
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Constructor. </summary>
		///
		/// <remarks>	Darrellp, 9/19/2011. </remarks>
		///
		/// <param name="location">		coordinates of the upper left corner of the room. </param>
		/// <param name="gridLocation">	The grid location. </param>
		/// <param name="width">		The width. </param>
		/// <param name="height">		The height. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal RectangularRoom(MapCoordinates location, MapCoordinates gridLocation, int width, int height)
		{
			Location = location;
			Width = width;
			Height = height;
			GridLocation = gridLocation;
		}

		internal RectangularRoom(MapCoordinates location, int width, int height) :
			this(location, new MapCoordinates(), width, height) {}

		internal static RectangularRoom CreateUndirectional(MapCoordinates location, int sizeInDir, int sizeInOtherDir, int gridCoordInDir, int gridCoordInOtherDir, Dir dir)
		{
			MapCoordinates gridLocation;
			if (dir == Dir.Vert)
			{
				gridLocation = new MapCoordinates(gridCoordInOtherDir, gridCoordInDir);
				return new RectangularRoom(location, gridLocation, sizeInOtherDir, sizeInDir);
			}
			gridLocation = new MapCoordinates(gridCoordInDir, gridCoordInOtherDir);
			return new RectangularRoom(location, gridLocation, sizeInDir, sizeInOtherDir);
		}
		#endregion
		#region Info
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Serves as a hash function for a room. </summary>
		///
		/// <remarks>	Darrellp, 9/26/2011. </remarks>
		///
		/// <returns>	A hash code for the current <see cref="T:System.Object" />. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public override int GetHashCode()
		{
			return (((Location.Column ^ ((Location.Row << 13) | (Location.Row >> 19))) ^ ((Width << 26) | (Width >> 6))) ^ ((Height << 7) | (Height >> 25)));
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Converts this object to a generic room with no exits. </summary>
		///
		/// <remarks>	Darrellp, 9/26/2011. </remarks>
		///
		/// <returns>	This object as a GenericRoom. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal GenericRoom ToGeneric()
		{
			char[][] layout = new char[Width][];
			char floorChar = TerrainFactory.TerrainToChar(TerrainType.Floor);

			for (int iColumn = 0; iColumn < Width; iColumn++)
			{
				layout[iColumn] = new char[Height];
				for (int iRow = 0; iRow < Height; iRow++)
				{
					bool fBorder = iRow == 0 || iRow == Height - 1 || iColumn == 0 || iColumn == Width - 1;
					if (!fBorder)
					{
						layout[iColumn][iRow] = floorChar;
					}
				}
			}
			return new GenericRoom(layout, Location, new List<GenericRoom>());
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Pick a spot on one of the walls of the room. </summary>
		///
		/// <remarks>	
		/// This is a spot ON the wall - not outside it.  It thus lies within the confines of the room.
		/// Darrellp, 9/20/2011. 
		/// </remarks>
		///
		/// <param name="rnd">				The random number generator. </param>
		/// <param name="wall">				The wall. </param>
		/// <param name="fIncludeCorners">	true to include corners in our consideration for a spot. </param>
		///
		/// <returns>	Coordinates of the spot chosen. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal MapCoordinates PickSpotOnWall(Rnd rnd, Wall wall, bool fIncludeCorners = false)
		{
			// Locals
			int column, row;

			// Our values get bumped to avoid corners if need be
			int cornerBump = fIncludeCorners ? 0 : 1;

			// If we're at the top or bottom
			if (wall == Wall.Top || wall == Wall.Bottom)
			{
				// Pick a random column
				int left = Left + cornerBump;
				int right = Right - cornerBump;
				column = rnd.Next(left, right + 1);

				// Set row to top or bottom
				row = wall == Wall.Top ? Top : Bottom;
			}
			else
			{
				// Pick a random row
				int top = Top + cornerBump;
				int bottom = Bottom - cornerBump;
				row = rnd.Next(top, bottom + 1);

				// Set column to left or right
				column = wall == Wall.Left ? Left : Right;
			}

			// Return our new spot
			return new MapCoordinates(column, row);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Pick a random spot in the room. </summary>
		///
		/// <remarks>	This is a spot IN the room - not on the walls.  Darrellp, 9/20/2011. </remarks>
		///
		/// <returns>	Selected spot. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal MapCoordinates PickSpotInRoom(Rnd rnd)
		{
			return new MapCoordinates(
				rnd.Next(Left + 1, Right),
				rnd.Next(Top + 1, Bottom));
		}

		internal int Size(Dir dir)
		{
			return dir == Dir.Horiz ? Width : Height;
		}

		internal int SmallCoord(Dir dir)
		{
			return dir == Dir.Horiz ? Left : Top;
		}

		internal int LargeCoord(Dir dir)
		{
			return dir == Dir.Horiz ? Right : Bottom;
		}

		internal int GridCoord(Dir dir)
		{
			return dir == Dir.Horiz ? GridColumn : GridRow;
		}
		#endregion
	}
}
