using System;
using System.Collections.Generic;
using System.Linq;
using CSRogue.Utilities;

namespace CSRogue.Map_Generation
{
	public class FOV
	{
		#region Private Variables
		private int _maxRow;
		private HashSet<MapCoordinates> _currentFOV = new HashSet<MapCoordinates>();
		private HashSet<MapCoordinates> _previousFOV = new HashSet<MapCoordinates>();
		private MapCoordinates _location;
		private readonly Map _map;
		private readonly int _rowCount;
		#endregion

		#region Properties
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the filter for where light goes. </summary>
		///
		/// <value>	The filter. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public Func<MapCoordinates, MapCoordinates, bool> Filter { get; set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Return an enumeration of newly visible tile locations. </summary>
		///
		/// <value>	The newly seen. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public IEnumerable<MapCoordinates> CurrentlySeen => _currentFOV;

	    ////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Return an enumeration of newly visible tile locations. </summary>
		///
		/// <value>	The newly seen. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public IEnumerable<MapCoordinates> NewlySeen
		{
			get
			{
				return _currentFOV.Where(loc => !_previousFOV.Contains(loc));
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Return an enumeration of tiles which were visible but are now infisible </summary>
		///
		/// <value>	The now unseen. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public IEnumerable<MapCoordinates> NowUnseen
		{
			get
			{
				return _previousFOV.Where(loc => !_currentFOV.Contains(loc));
			}
		} 
		#endregion

		#region Constructor
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Constructor. </summary>
		///
		/// <remarks>	
		/// The filter function must form a starlike shape around the hero since we mark anything outside
		/// of it as opaque.  This speeds up performance. Darrellp, 10/2/2011. 
		/// </remarks>
		///
		/// <param name="map">		The map we're viewing. </param>
		/// <param name="rowCount">	Number of rows max to scan. </param>
		/// <param name="filter">	Filter function to determine which values are allowed. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public FOV(Map map, int rowCount, Func<MapCoordinates, MapCoordinates, bool> filter = null)
		{
			_map = map;
			_rowCount = rowCount;
			if (filter == null)
			{
				filter = (locHero, locTile) => true;
			}
			Filter = filter;
		} 
		#endregion

		#region Scanning
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Scans the map for visible tiles.  </summary>
		///
		/// <remarks>	Darrellp, 10/2/2011. </remarks>
		///
		/// <param name="location">	The viewpoint from which visibility is calculated. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public void Scan(MapCoordinates location)
		{
			_location = location;

			var tmp = _previousFOV;
			_previousFOV = _currentFOV;
			_currentFOV = tmp;
			_currentFOV.Clear();

			ScanOctant(Dir.Vert, rowIncrement: -1, colIncrement: 1);
			ScanOctant(Dir.Vert, rowIncrement: 1, colIncrement: 1);
			ScanOctant(Dir.Vert, rowIncrement: -1, colIncrement: -1);
			ScanOctant(Dir.Vert, rowIncrement: 1, colIncrement: -1);
			ScanOctant(Dir.Horiz, rowIncrement: -1, colIncrement: 1);
			ScanOctant(Dir.Horiz, rowIncrement: 1, colIncrement: 1);
			ScanOctant(Dir.Horiz, rowIncrement: -1, colIncrement: -1);
			ScanOctant(Dir.Horiz, rowIncrement: 1, colIncrement: -1);
            //ScanQuadrant(Dir.Vert, rowIncrement: -1, colIncrement: 1);
			//ScanQuadrant(Dir.Vert, rowIncrement: 1, colIncrement: 1);
			//ScanQuadrant(Dir.Horiz, rowIncrement: -1, colIncrement: 1);
			//ScanQuadrant(Dir.Horiz, rowIncrement: 1, colIncrement: 1);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Scans a quadrant. </summary>
		///
		/// <remarks>	
		/// The original paper I read on this advocated scanning in octants.  This may be a teeny bit
		/// better from a quality point of view since you'll be going the same direction symmetrically in
		/// each octant, but I don't think any user is going to notice.  Doing it in quadrants speeds
		/// things up with little, if any, decrease in quality.  Darrellp, 10/2/2011. 
		/// </remarks>
		///
		/// <param name="dir">			The direction we're scanning in. </param>
		/// <param name="rowIncrement">	The amount to increment the row by during the scan. </param>
		/// <param name="colIncrement">	The amount to increment the column by during the scan. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void ScanOctant(Dir dir, int rowIncrement, int colIncrement)
		{
			Dir dirOther = MapCoordinates.OtherDirection(dir);

            // TODO: I believe that by going from -colIncrement to colIncrement I'm doing a quadrant rather than an octant here so doubling our effort.
			BresenhamStepper leftStepper = new BresenhamStepper(_location, _location + MapCoordinates.CreateUndirectional(-colIncrement, rowIncrement, dirOther));
			BresenhamStepper rightStepper = new BresenhamStepper(_location, _location + MapCoordinates.CreateUndirectional(colIncrement, rowIncrement, dirOther));
            
			IEnumerator<MapCoordinates> leftEnumerator = leftStepper.Steps.GetEnumerator();
			IEnumerator<MapCoordinates> rightEnumerator = rightStepper.Steps.GetEnumerator();
 			_maxRow = _location[dir] + rowIncrement * (_rowCount - 1);

           // Move beyond the starting position (presumably the player)
		    leftEnumerator.MoveNext();
		    rightEnumerator.MoveNext();

            // We have to imcrement the row by rowIncrement to account for the fact that we've stepped beyond the player's location above...
			Scan(leftEnumerator, rightEnumerator, _location[dir] + rowIncrement, rowIncrement: rowIncrement, colIncrement: colIncrement, dirOther: dirOther);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Scans a quadrant for visible tiles. </summary>
		///
		/// <remarks>	Darrellp, 10/2/2011. </remarks>
		///
		/// <param name="leftEnumerator">	The left side to be scanned. </param>
		/// <param name="rightEnumerator">	The right side to be scanned. </param>
		/// <param name="iRow">				The row to start the scan on. </param>
		/// <param name="rowIncrement">		The amount to increment the row by during the scan. </param>
		/// <param name="colIncrement">		The amount to increment the column by during the scan. </param>
		/// <param name="dirOther">			The direction perpindicular to the scan. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void Scan(
			IEnumerator<MapCoordinates> leftEnumerator,
			IEnumerator<MapCoordinates> rightEnumerator,
			int iRow,
			int rowIncrement,
			int colIncrement,
			Dir dirOther)
		{
			// For each row
			for (; iRow != _maxRow + rowIncrement; iRow += rowIncrement)
			{
				// Figure out the rightmost and leftmost columns
				leftEnumerator.MoveNext();
				rightEnumerator.MoveNext();
				int rightColumn = rightEnumerator.Current[dirOther];
				int leftColumn = leftEnumerator.Current[dirOther];
				int order = Math.Sign(rightColumn - leftColumn);

				// Are the columns in the wrong order?
				if (order != 0 && order != colIncrement)
				{
					// Return with nothing to check
					return;
				}

				// For each column
				for (int iColumn = leftColumn; iColumn != rightColumn + colIncrement; iColumn += colIncrement)
				{
					// Get the current position into MapCoordinates
					MapCoordinates currentLocation = MapCoordinates.CreateUndirectional(iColumn, iRow, dirOther);
					bool blocked;

				    if (!_map.Contains(currentLocation))
				    {
				        continue;
				    }
					// If the terrain is opaque at this position
					if (IsOpaqueAt(_map, currentLocation, out blocked))
					{
						// If we're actually blocked
						if (blocked)
						{
							// The opaque tile is visible
							_currentFOV.Add(currentLocation);
						}

						// Continue on but restrict ourselves to the new cleared area
						int? beginRow = dirOther == Dir.Vert ? null : (int?)(iRow + rowIncrement);
						int? beginColumn = dirOther == Dir.Vert ? (int?)(iRow + rowIncrement) : null;

						// Make a recursive call for the unshaded region to the left of the blocker
						BresenhamStepper newRightStepper = new BresenhamStepper(
							_location,
							MapCoordinates.CreateUndirectional(iColumn - colIncrement, iRow, dirOther),
							beginColumn,
							beginRow);
						IEnumerator<MapCoordinates> newRightEnumerator = newRightStepper.Steps.GetEnumerator();
						Scan(leftEnumerator, newRightEnumerator, iRow + rowIncrement, rowIncrement, colIncrement, dirOther);

						// Look for another cleared run
						bool scannedToEndOfLine = true;
						
						// For each column after the blocker...
						for (iColumn += colIncrement; iColumn != rightColumn + colIncrement; iColumn += colIncrement)
						{
							// Make this our current location
							currentLocation = MapCoordinates.CreateUndirectional(iColumn, iRow, dirOther);

							// Is it clear yet?
							if (!IsOpaqueAt(_map, currentLocation, out blocked))
							{
								// We have more clear area to deal with - break from the loop
								scannedToEndOfLine = false;
								break;
							}
							// if we're actually blocked
							if (blocked)
							{
								// This opaque tile is visible
								_currentFOV.Add(currentLocation);
							}
						}

						// Did we not manage to find any more clear area?
						if (scannedToEndOfLine)
						{
							// We're done
							return;
						}

						// Add the new location to the currently viewable sites
						_currentFOV.Add(currentLocation);

						// Continue on but restrict ourselves to the new cleared area
						beginRow = dirOther == Dir.Vert ? null : (int?)iRow;
						beginColumn = dirOther == Dir.Vert ? (int?)iRow : null;
						//leftEnumerator = new BresenhamStepper(_location, currentLocation, beginColumn, beginRow, tally:(Math.Abs(iRow - _location.Row))).Steps.GetEnumerator();
						leftEnumerator = new BresenhamStepper(_location, currentLocation, beginColumn, beginRow).Steps.GetEnumerator();
						continue;
					}

					// Add the new location to the currently viewable sites
					_currentFOV.Add(currentLocation);
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	
		/// Query if the map is clear to view at a particular location. It may not be because it's
		/// outside the boundaries of the map or outside the filter function or off the map.  The out
		/// parameter blocked will be true when it's actually blocked rather than opaque for one of these
		/// other reasons. 
		/// </summary>
		///
		/// <remarks>	Darrellp, 10/2/2011. </remarks>
		///
		/// <param name="map">		The map we're viewing. </param>
		/// <param name="location">	The viewpoint from which visibility is calculated. </param>
		/// <param name="blocked">	[out] True if the view is actually blocked. </param>
		///
		/// <returns>	true if opaque at, false if not. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private bool IsOpaqueAt(Map map, MapCoordinates location, out bool blocked)
		{
			if (!Filter(_location, location) || !map.Contains(location))
			{
				blocked = false;
				return true;
			}
			MapLocationData data = map[location];
			blocked = data.Terrain == TerrainType.Wall;
			return blocked || data.Terrain == TerrainType.OffMap;
		} 
		#endregion
	}
}
