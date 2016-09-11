using System;
using System.Collections.Generic;
using System.Linq;
using CSRogue.Interfaces;
using CSRogue.Utilities;

namespace CSRogue.Map_Generation
{
    public class FOV
	{
        #region Private Variables
		private HashSet<MapCoordinates> _currentFOV = new HashSet<MapCoordinates>();
		private HashSet<MapCoordinates> _previousFOV = new HashSet<MapCoordinates>();
		private MapCoordinates _location;
		private readonly IMap _csRogueMap;
		private readonly int _lightRadius;
		#endregion

		#region Properties
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the filter for where light goes. </summary>
		///
		/// <value>	The filter. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public Func<MapCoordinates, MapCoordinates, bool> Filter { get; set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Return an enumeration of currently visible tile locations. </summary>
		///
		/// <value>	The currently seen tile locations. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public IEnumerable<MapCoordinates> CurrentlySeen => _currentFOV;

	    ////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Return an enumeration of newly seen tile locations. </summary>
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
		/// <summary>	Return an enumeration of newly invisible tile locations </summary>
		///
		/// <value>	The now unseen. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public IEnumerable<MapCoordinates> NewlyUnseen
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
		/// <param name="csRogueMap">		    The map we're viewing. </param>
		/// <param name="lightRadius">	Distance light can reach. </param>
		/// <param name="filter">	    Filter function to determine which values are allowed. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public FOV(IMap csRogueMap, int lightRadius, Func<MapCoordinates, MapCoordinates, bool> filter = null)
		{
			_csRogueMap = csRogueMap;
			_lightRadius = lightRadius;
			if (filter == null)
			{
				filter = (locHero, locTile) => true;
			}
			Filter = filter;
		}
        #endregion

        #region Scanning
        // This scanning modified from Adam Milazzo's code at this page:
        //  http://www.adammil.net/blog/v125_roguelike_vision_algorithms.html
        // I modified it SLIGHTLY to work in my context and it dropped in like a charm.  My commenting software puts my name in the
        // comments below but the vast majority of credit goes to Adam Milazzo.
        public void Compute(MapCoordinates origin, int rangeLimit)
        {
            _currentFOV.Add(new MapCoordinates(origin.Column, origin.Row));
            for (var octant = 0; octant < 8; octant++)
            {
                Compute(octant, origin, rangeLimit, 1, new Slope(1, 1), new Slope(0, 1));
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Represents the slope Y/X as a rational number. </summary>
        ///
        /// <remarks>   Darrell, 8/28/2016. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private struct Slope
        {
            public Slope(int y, int x)
            {
                Y = y;
                X = x;
            }

            // this > y/x
            public bool Greater(int y, int x)
            {
                return Y * x > X * y;
            }

            // this >= y/x
            public bool GreaterOrEqual(int y, int x)
            {
                return Y * x >= X * y;
            }

            // this < y/x
            public bool Less(int y, int x)
            {
                return Y * x < X * y;
            }

            public readonly int X;
            public readonly int Y;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Computes the FOV. </summary>
        /// 
        /// <remarks> Darrell, 8/28/2016. </remarks>
        ///
        /// <param name="octant">       The octant to be scanned. </param>
        /// <param name="origin">       The spot from which the FOV is computed. </param>
        /// <param name="rangeLimit">   The range limit. </param>
        /// <param name="x">            The x coordinate. </param>
        /// <param name="top">          The top slope. </param>
        /// <param name="bottom">       The bottom slope. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void Compute(int octant, MapCoordinates origin, int rangeLimit, int x, Slope top, Slope bottom)
        {
            // throughout this function there are references to various parts of tiles. a tile's coordinates refer to its
            // center, and the following diagram shows the parts of the tile and the vectors from the origin that pass through
            // those parts. given a part of a tile with vector u, a vector v passes above it if v > u and below it if v < u
            //    g         center:        y / x
            // a------b   a top left:      (y*2+1) / (x*2-1)   i inner top left:      (y*4+1) / (x*4-1)
            // |  /\  |   b top right:     (y*2+1) / (x*2+1)   j inner top right:     (y*4+1) / (x*4+1)
            // |i/__\j|   c bottom left:   (y*2-1) / (x*2-1)   k inner bottom left:   (y*4-1) / (x*4-1)
            //e|/|  |\|f  d bottom right:  (y*2-1) / (x*2+1)   m inner bottom right:  (y*4-1) / (x*4+1)
            // |\|__|/|   e middle left:   (y*2) / (x*2-1)
            // |k\  /m|   f middle right:  (y*2) / (x*2+1)     a-d are the corners of the tile
            // |  \/  |   g top center:    (y*2+1) / (x*2)     e-h are the corners of the inner (wall) diamond
            // c------d   h bottom center: (y*2-1) / (x*2)     i-m are the corners of the inner square (1/2 tile width)
            //    h
            for (; x <= rangeLimit; x++)
            {
                // compute the Y coordinates of the top and bottom of the sector. we maintain that top > bottom
                int topY;

                // if top == ?/1 then it must be 1/1 because 0/1 < top <= 1/1. this is special-cased because top
                //  starts at 1/1 and remains 1/1 as long as it doesn't hit anything, so it's a common case
                if (top.X == 1) 
                {              
                    topY = x;
                }
                else // top < 1
                {
                    // Get the tile that the top vector enters from the left. Since our coordinates refer to the center of the
                    // tile, this is (x-0.5)*top+0.5, which can be computed as (x-0.5)*top+0.5 = (2(x+0.5)*top+1)/2 =
                    // ((2x+1)*top+1)/2. Since top == a/b, this is ((2x+1)*a+b)/2b. If it enters a tile at one of the left
                    // corners, it will round up, so it'll enter from the bottom-left and never the top-left
                    //
                    // The Y coordinate of the tile entered from the left
                    // Now it's possible that the vector passes from the left side of the tile up into the tile above before
                    // exiting from the right side of this column so we may need to increment topY
                    topY = ((x * 2 - 1) * top.Y + top.X) / (top.X * 2); 
                                                                        
                    // If the tile blocks light (i.e. is a wall)...                                                    
                    if (BlocksLight(x, topY, octant, origin))
                    {
                        // If the tile entered from the left blocks light, whether it passes into the tile above depends on the shape
                        // of the wall tile as well as the angle of the vector. If the tile has does not have a beveled top-left
                        // corner, then it is blocked. The corner is beveled if the tiles above and to the left are not walls. We can
                        // ignore the tile to the left because if it was a wall tile, the top vector must have entered this tile from
                        // the bottom-left corner, in which case it can't possibly enter the tile above.
                        //
                        // Otherwise, with a beveled top-left corner, the slope of the vector must be greater than or equal to the
                        // slope of the vector to the top center of the tile (x*2, topY*2+1) in order for it to miss the wall and
                        // pass into the tile above
                        if (top.GreaterOrEqual(topY * 2 + 1, x * 2) && !BlocksLight(x, topY + 1, octant, origin)) topY++;
                    }
                    else
                    {
                        // Since this tile doesn't block light, there's nothing to stop it from passing into the tile above, and it
                        // does so if the vector is greater than the vector for the bottom-right corner of the tile above. However,
                        // there is one additional consideration - later code in this method assumes that if a tile blocks light then
                        // it must be visible, so if the tile above blocks light we have to make sure the light actually impacts the
                        // wall shape. There are three cases:
                        // 
                        //      1) the tile above is clear, in which case the vector must be above the bottom-right corner of the tile above
                        //      2) the tile above blocks light and does not have a beveled bottom-right corner, in which case the
                        //         vector must be above the bottom-right corner
                        //      3) the tile above blocks light and does have a beveled bottom-right corner, in which case
                        //         the vector must be above the bottom center of the tile above (i.e. the corner of the beveled edge).
                        // 
                        // Now it's possible to merge 1 and 2 into a single check, and we get the following: if the tile above and to
                        // the right is a wall, then the vector must be above the bottom-right corner. Otherwise, the vector must be
                        // above the bottom center. this works because if the tile above and to the right is a wall, then there are
                        // two cases:
                        // 
                        //      1) the tile above is also a wall, in which case we must check against the bottom-right corner,
                        //      2) the tile above is not a wall, in which case the vector passes into it if it's above the bottom-right corner.
                        //
                        // So either way we use the bottom-right corner in that case. Now, if the tile above and to the right
                        // is not a wall, then we again have two cases:
                        // 
                        //      1) the tile above is a wall with a beveled edge, in which case we must check against the bottom center
                        //      2) the tile above is not a wall, in which case it will only be visible if light passes through the inner
                        //         square, and the inner square is guaranteed to be no larger than a wall diamond, so if it wouldn't pass
                        //         through a wall diamond then it can't be visible, so there's no point in incrementing topY even if light
                        //         passes through the corner of the tile above. so we might as well use the bottom center for both cases.

                        // center
                        var ax = x * 2;

                        // use bottom-right if the tile above and right is a wall
                        if (BlocksLight(x + 1, topY + 1, octant, origin))
                        {
                            ax++;
                        }
                        if (top.Greater(topY * 2 + 1, ax))
                        {
                            topY++;
                        }
                    }
                }

                // if bottom == 0/?, then it's hitting the tile at Y=0 dead center. this is special-cased because
                // bottom.Y starts at zero and remains zero as long as it doesn't hit anything, so it's common 
                int bottomY;
                if (bottom.Y == 0)
                {
                    bottomY = 0;
                }
                else
                {
                    // The tile that the bottom vector enters from the left
                    // code below assumes that if a tile is a wall then it's visible, so if the tile contains a wall we have to
                    // ensure that the bottom vector actually hits the wall shape. It misses the wall shape if the top-left corner
                    // is beveled and bottom >= (bottomY*2+1)/(x*2). Finally, the top-left corner is beveled if the tiles to the
                    // left and above are clear. we can assume the tile to the left is clear because otherwise the bottom vector
                    // would be greater, so we only have to check above
                    bottomY = ((x * 2 - 1) * bottom.Y + bottom.X) / (bottom.X * 2);
                    if (bottom.GreaterOrEqual(bottomY * 2 + 1, x * 2) && BlocksLight(x, bottomY, octant, origin) &&
                        !BlocksLight(x, bottomY + 1, octant, origin))
                    {
                        bottomY++;
                    }
                }

                // Go through the tiles in the column now that we know which ones could possibly be visible
                var wasOpaque = -1; // 0:false, 1:true, -1:not applicable
                var rangeLimitSq = rangeLimit * rangeLimit;
                for (var y = topY; y >= bottomY; y--) // use a signed comparison because y can wrap around when decremented
                {
                    if (rangeLimit < 0 || GetDistanceSq(x, y) <= rangeLimitSq) // skip the tile if it's out of visual range
                    {
                        var isOpaque = BlocksLight(x, y, octant, origin);

                        // every tile where topY > y > bottomY is guaranteed to be visible. also, the code that initializes topY and
                        // bottomY guarantees that if the tile is opaque then it's visible. so we only have to do extra work for the
                        // case where the tile is clear and y == topY or y == bottomY. if y == topY then we have to make sure that
                        // the top vector is above the bottom-right corner of the inner square. if y == bottomY then we have to make
                        // sure that the bottom vector is below the top-left corner of the inner square
                        var isVisible =
                            isOpaque || ((y != topY || top.Greater(y * 4 - 1, x * 4 + 1)) && (y != bottomY || bottom.Less(y * 4 + 1, x * 4 - 1)));

                        // NOTE: if you want the algorithm to be either fully or mostly symmetrical, replace the line above with the
                        // following line (and uncomment the Slope.LessOrEqual method). the line ensures that a clear tile is visible
                        // only if there's an unobstructed line to its center. if you want it to be fully symmetrical, also remove
                        // the "isOpaque ||" part and see NOTE comments further down
                        // bool isVisible = isOpaque || ((y != topY || top.GreaterOrEqual(y, x)) && (y != bottomY || bottom.LessOrEqual(y, x)));
                        if (isVisible)
                        {
                            SetVisible(x, y, octant, origin);
                        }

                        // if we found a transition from clear to opaque or vice versa, adjust the top and bottom vectors
                        // but don't bother adjusting them if this is the last column anyway
                        if (x != rangeLimit)
                        {
                            if (isOpaque)
                            {
                                // If we found a transition from clear to opaque, this sector is done in this column,
                                // so adjust the bottom vector upward and continue processing it in the next column.
                                // If the opaque tile has a beveled top-left corner, move the bottom vector up to the top center.
                                // Otherwise, move it up to the top left. The corner is beveled if the tiles above and to the left are
                                // clear. we can assume the tile to the left is clear because otherwise the vector would be higher, so
                                // we only have to check the tile above.
                                if (wasOpaque == 0) 
                                {
                                    // top center by default                
                                    int nx = x * 2, ny = y * 2 + 1;

                                    // NOTE: If you're using full symmetry and want more expansive walls (recommended), comment out the next if statement
                                    if (BlocksLight(x, y + 1, octant, origin))
                                    {
                                        // top left if the corner is not beveled
                                        nx--;
                                    }

                                    // We have to maintain the invariant that top > bottom, so the new sector
                                    // created by adjusting the bottom is only valid if that's the case
                                    // if we're at the bottom of the column, then just adjust the current sector rather than recursing
                                    // since there's no chance that this sector can be split in two by a later transition back to clear
                                    if (top.Greater(ny, nx))
                                    {
                                        // don't recurse unless necessary
                                        if (y == bottomY)
                                        {
                                            bottom = new Slope(ny, nx);
                                            break;
                                        }
                                        Compute(octant, origin, rangeLimit, x + 1, top, new Slope(ny, nx));
                                    }
                                    // the new bottom is greater than or equal to the top, so the new sector is empty and we'll ignore
                                    // it. if we're at the bottom of the column, we'd normally adjust the current sector rather than
                                    else
                                    {
                                        // recursing, so that invalidates the current sector and we're done
                                        if (y == bottomY) return;
                                    }
                                }
                                wasOpaque = 1;
                            }
                            else
                            {
                                // If we found a transition from opaque to clear, adjust the top vector downwards
                                if (wasOpaque > 0)
                                {
                                    // If the opaque tile has a beveled bottom-right corner, move the top vector down to the bottom center -
                                    // otherwise, move it down to the bottom right. The corner is beveled if the tiles below and to the right
                                    // are clear. We know the tile below is clear because that's the current tile, so just check to the right
                                    
                                    // The bottom of the opaque tile (oy*2-1) equals the top of this tile (y*2+1)
                                    int nx = x * 2, ny = y * 2 + 1;

                                    // NOTE: if you're using full symmetry and want more expansive walls (recommended), comment out the next line

                                    // Check the right of the opaque tile (y+1), not this one
                                    if (BlocksLight(x + 1, y + 1, octant, origin))
                                    {
                                        nx++;
                                    }
                                    // We have to maintain the invariant that top > bottom. if not, the sector is empty and we're done
                                    if (bottom.GreaterOrEqual(ny, nx))
                                    {
                                        return;
                                    }
                                    top = new Slope(ny, nx);
                                }
                                wasOpaque = 0;
                            }
                        }
                    }
                }

                // if the column didn't end in a clear tile, then there's no reason to continue processing the current sector
                // because that means either 1) wasOpaque == -1, implying that the sector is empty or at its range limit, or 2)
                // wasOpaque == 1, implying that we found a transition from clear to opaque and we recursed and we never found
                // a transition back to clear, so there's nothing else for us to do that the recursive method hasn't already. (if
                // we didn't recurse (because y == bottomY), it would have executed a break, leaving wasOpaque equal to 0.)
                if (wasOpaque != 0)
                {
                    break;
                }
            }
        }

        // NOTE: the code duplication between BlocksLight and SetVisible is for performance. don't refactor the octant
        // translation out unless you don't mind an 18% drop in speed
        bool BlocksLight(int x, int y, int octant, MapCoordinates origin)
        {
            int nx = origin.Column, ny = origin.Row;
            switch (octant)
            {
                case 0: nx += x; ny -= y; break;
                case 1: nx += y; ny -= x; break;
                case 2: nx -= y; ny -= x; break;
                case 3: nx -= x; ny -= y; break;
                case 4: nx -= x; ny += y; break;
                case 5: nx -= y; ny += x; break;
                case 6: nx += y; ny += x; break;
                case 7: nx += x; ny += y; break;
            }
            return IsOpaqueAt(new MapCoordinates(nx, ny));
        }

        void SetVisible(int x, int y, int octant, MapCoordinates origin)
        {
            int nx = origin.Column, ny = origin.Row;
            switch (octant)
            {
                case 0: nx += x; ny -= y; break;
                case 1: nx += y; ny -= x; break;
                case 2: nx -= y; ny -= x; break;
                case 3: nx -= x; ny -= y; break;
                case 4: nx -= x; ny += y; break;
                case 5: nx -= y; ny += x; break;
                case 6: nx += y; ny += x; break;
                case 7: nx += x; ny += y; break;
            }
            if (_csRogueMap.Contains(nx, ny))
            {
                _currentFOV.Add(new MapCoordinates(nx, ny));
            }
        }

        int GetDistanceSq(int x, int y)
        {
            return x * x + y * y;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>	Scans the map for visible tiles.  </summary>
        ///
        /// <remarks>	This does two things - it moves the last seen tiles into _previousFOV and puts
        ///             newly seen tiles in _curentFOV.  That wasy we don't have to deal with tiles we
        ///             saw on the previous scan and we can deal only with newly seen and newly
        ///             unseen tiles which are returned in NewlySeen and NewlyUnseen properties
        ///             respectively.  CurrentlySeen returns an enumerable of all the tiles
        ///             visible in this scan.
        ///            
        ///              Darrellp, 10/2/2011. </remarks>
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
            _currentFOV.Add(location);
            for (var octant = 0; octant < 8; octant++)
            {
                Compute(octant, location, _lightRadius, 1, new Slope(1, 1), new Slope(0, 1));
            }
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////
        ///  <summary>	
        ///  Query if the map is clear to view at a particular location. It may not be because it's
        ///  outside the boundaries of the map or outside the filter function or off the map.  The out
        ///  parameter blocked will be true when it's actually blocked rather than opaque for one of these
        ///  other reasons. 
        ///  </summary>
        /// 
        ///  <remarks>	Darrellp, 10/2/2011. </remarks>
        /// <param name="location">	The viewpoint from which visibility is calculated. </param>
        /// 
        ///  <returns>	true if opaque at, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool IsOpaqueAt(MapCoordinates location)
		{
			if (!Filter(_location, location) || !_csRogueMap.Contains(location))
			{
				return true;
			}
			var terrain = _csRogueMap[location].Terrain;
			return terrain == TerrainType.Wall || terrain == TerrainType.OffMap;
		} 
        #endregion
	}
}
