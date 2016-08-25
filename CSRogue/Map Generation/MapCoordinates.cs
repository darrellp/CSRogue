using System.Collections.Generic;

namespace CSRogue.Map_Generation
{
	enum Dir
	{
		Horiz,
		Vert
	}

	public struct MapCoordinates
	{
		internal static Dir OtherDirection(Dir dir)
		{
			return dir == Dir.Horiz ? Dir.Vert : Dir.Horiz;
		}

		public override int GetHashCode()
		{
			return (17 * 23 + Column) * 23 + Row;
		}

		public int Column { get; private set; }

		public int Row { get; private set; }

		public MapCoordinates(int column, int row) : this()
		{
			Column = column;
			Row = row;
		}

		public static bool Within(MapCoordinates loc1, MapCoordinates loc2, int radius, int aspectRatioHorizontal, int aspectRatioVertical)
		{
			int colDiff = (loc1.Column - loc2.Column) * aspectRatioHorizontal / aspectRatioVertical;
			int rowDiff = loc1.Row - loc2.Row;

			return rowDiff * rowDiff + colDiff * colDiff <= radius * radius;
		}

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

		public static MapCoordinates operator +(MapCoordinates loc1, MapCoordinates loc2)
		{
			return new MapCoordinates(loc1.Column + loc2.Column, loc1.Row + loc2.Row);
		}

		public static MapCoordinates operator -(MapCoordinates loc1, MapCoordinates loc2)
		{
			return new MapCoordinates(loc1.Column - loc2.Column, loc1.Row - loc2.Row);
		}

		internal MapCoordinates NextLarger(Dir dir)
		{
			int column = dir == Dir.Horiz ? Column + 1 : Column;
			int row = dir == Dir.Horiz ? Row : Row + 1;
			return new MapCoordinates(column, row);
		}

		internal MapCoordinates NextSmaller(Dir dir)
		{
			int column = dir == Dir.Horiz ? Column - 1 : Column;
			int row = dir == Dir.Horiz ? Row : Row - 1;
			return new MapCoordinates(column, row);
		}

		internal static MapCoordinates CreateUndirectional(int par, int perp, Dir dir)
		{
			return dir == Dir.Horiz ? new MapCoordinates(par, perp) : new MapCoordinates(perp, par);
		}

		internal List<MapCoordinates> Neighbors(int mapWidth, int mapHeight)
		{
			// Allocate the list
			List<MapCoordinates> ret = new List<MapCoordinates>();

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
