using System.Collections.Generic;

namespace CSRogue.Map_Generation
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Values that represent directions. </summary>
    ///
    /// <remarks>   There is LOTS of code which would be identical except for swapping out X/Column and
    ///             Y/Row coordinates.  Duplication is evil and I don't want it in the code if I can avoid
    ///             it.  If I used only fields for coordinates then I just have to make separate versions
    ///             of code which uses pt.X and pt.Y or use ?: operator everywhere.  To avoid this, I've
    ///             supplied a way of indexing the different coordinates which uses a single Dir argument
    ///             to an indexer rather than separate Row and Column fields.  So pt[Dir.Horiz] returns
    ///             the Column and pt[Dir.Vert] returns the row.  The direction can be passed into routines
    ///             and therefore implement both versions of the code without duplicating effort.
    ///             
    ///             None of this is exposed publicly right now.  It might makes sense to do so, but I don't
    ///             want to overly complicate things.
    ///             
    ///             Darrellp, 8/25/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

	enum Dir
	{
		Horiz,
		Vert
	}

	public struct MapCoordinates
	{
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Returns the "other direction from a direction. </summary>
        ///
        /// <remarks>   Darrellp, 8/25/2016. </remarks>
        ///
        /// <param name="dir">  The direction. </param>
        ///
        /// <returns>   Dir.Horiz if dir is Dir.Vert, Dir.Vert if dir is Dir.Horiz. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		internal static Dir OtherDirection(Dir dir)
		{
			return dir == Dir.Horiz ? Dir.Vert : Dir.Horiz;
		}

		public override int GetHashCode()
		{
		    // ReSharper disable NonReadonlyMemberInGetHashCode
			return (17 * 23 + Column) * 23 + Row;
		    // ReSharper restore NonReadonlyMemberInGetHashCode
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the column. </summary>
        ///
        /// <value> The column. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public int Column { get; private set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the row. </summary>
        ///
        /// <value> The row. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public int Row { get; private set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Darrellp, 8/25/2016. </remarks>
        ///
        /// <param name="column">   The column. </param>
        /// <param name="row">      The row. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public MapCoordinates(int column, int row) : this()
		{
			Column = column;
			Row = row;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Determines if two points are within a certain distance of each other. </summary>
        ///
        /// <remarks>   Darrellp, 8/25/2016. </remarks>
        ///
        /// <param name="loc1">                     The first location. </param>
        /// <param name="loc2">                     The second location. </param>
        /// <param name="radius">                   The radius. </param>
        /// <param name="aspectRatioHorizontal">    The horizontal aspect ratio. </param>
        /// <param name="aspectRatioVertical">      The vertical aspect ratio. </param>
        ///
        /// <returns>   true if the two points are within radius of each other, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public static bool Within(MapCoordinates loc1, MapCoordinates loc2, int radius, int aspectRatioHorizontal, int aspectRatioVertical)
		{
			var colDiff = (loc1.Column - loc2.Column) * aspectRatioHorizontal / aspectRatioVertical;
			var rowDiff = loc1.Row - loc2.Row;

			return rowDiff * rowDiff + colDiff * colDiff <= radius * radius;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Indexer to get or set a coordinate specified by dir.
        /// </summary>
        ///
        /// <param name="dir">  The coordinate to return - dir.Horiz for columns, dir.Vert for rows. </param>
        ///
        /// <returns>   The proper coordinate. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		internal int this[Dir dir]
		{
			get
			{
				return dir == Dir.Horiz ? Column : Row;
			}
			set
			{
				if (dir == Dir.Horiz)
				{
					Column = value;
				}
				else
				{
					Row = value;
				}
			}
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Addition operator. </summary>
        ///
        /// <remarks>   Darrellp, 8/25/2016. </remarks>
        ///
        /// <param name="loc1"> The first location. </param>
        /// <param name="loc2"> The second location. </param>
        ///
        /// <returns>   The result of the operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public static MapCoordinates operator +(MapCoordinates loc1, MapCoordinates loc2)
		{
			return new MapCoordinates(loc1.Column + loc2.Column, loc1.Row + loc2.Row);
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Subtraction operator. </summary>
        ///
        /// <remarks>   Darrellp, 8/25/2016. </remarks>
        ///
        /// <param name="loc1"> The first location. </param>
        /// <param name="loc2"> The second location. </param>
        ///
        /// <returns>   The result of the operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public static MapCoordinates operator -(MapCoordinates loc1, MapCoordinates loc2)
		{
			return new MapCoordinates(loc1.Column - loc2.Column, loc1.Row - loc2.Row);
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Increments the proper coordinate. </summary>
        ///
        /// <remarks>   Darrellp, 8/25/2016. </remarks>
        ///
        /// <param name="dir">  The direction to increment. </param>
        ///
        /// <returns>   The incremented MapCoordinates. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		internal MapCoordinates NextLarger(Dir dir)
		{
			var column = dir == Dir.Horiz ? Column + 1 : Column;
			var row = dir == Dir.Horiz ? Row : Row + 1;
			return new MapCoordinates(column, row);
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Decrements the proper coordinate. </summary>
        ///
        /// <remarks>   Darrellp, 8/25/2016. </remarks>
        ///
        /// <param name="dir">  The direction to decrement. </param>
        ///
        /// <returns>   The decremented MapCoordinates. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        internal MapCoordinates NextSmaller(Dir dir)
		{
			var column = dir == Dir.Horiz ? Column - 1 : Column;
			var row = dir == Dir.Horiz ? Row : Row - 1;
			return new MapCoordinates(column, row);
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Creates a MapCoordinates by specifying values relative to a direction. </summary>
        ///
        /// <remarks>   Darrellp, 8/25/2016. </remarks>
        ///
        /// <param name="par">  The parallel value - Column for Horz, Row for Vert. </param>
        /// <param name="perp"> The perpindicular value - Row for Horz, Column for Vert. </param>
        /// <param name="dir">  The direction to orient to. </param>
        ///
        /// <returns>   The new MapCoordinates. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		internal static MapCoordinates CreateUndirectional(int par, int perp, Dir dir)
		{
			return dir == Dir.Horiz ? new MapCoordinates(par, perp) : new MapCoordinates(perp, par);
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Neighbors of a position. </summary>
        ///
        /// <remarks>   These are just the direct horizontal and vertical neighbors of the
        ///             position.  Edges are taken into account so any returned neighbors are
        ///             guaranteed to lie in a rectangle from (0,0) to (width, height)
        ///             
        ///             Darrellp, 8/25/2016. </remarks>
        ///
        /// <param name="mapWidth">     Width of the map. </param>
        /// <param name="mapHeight">    Height of the map. </param>
        ///
        /// <returns>   A List&lt;MapCoordinates&gt; of neighbors </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		internal List<MapCoordinates> Neighbors(int mapWidth, int mapHeight)
		{
			// Allocate the list
			var ret = new List<MapCoordinates>();

			if (Column > 0)
			{
				ret.Add(new MapCoordinates(Column - 1, Row));
			}
			if (Column < mapWidth - 1)
			{
				ret.Add(new MapCoordinates(Column + 1, Row));
			}
			if (Row > 0)
			{
				ret.Add(new MapCoordinates(Column, Row - 1));
			}
			if (Row < mapHeight - 1)
			{
				ret.Add(new MapCoordinates(Column, Row + 1));
			}

			return ret;
		}

		public override string ToString()
		{
			return "(" + Column + ", " + Row + ")";
		}


		public static bool operator ==(MapCoordinates v1, MapCoordinates v2)
		{
			return v1.Equals(v2);
		}

		public static bool operator !=(MapCoordinates v1, MapCoordinates v2)
		{
			return !v1.Equals(v2);
		}

		public override bool Equals(object obj)
		{
			if (obj is MapCoordinates)
			{
				return Equals((MapCoordinates)obj);
			}
			return false;
		}

		public bool Equals(MapCoordinates other)
		{
			return Row.Equals(other.Row) && Column.Equals(other.Column);
		}

	}
}
