using System;
using System.Collections.Generic;
using System.Linq;
using CSRogue.Map_Generation;

namespace CSRogue.Utilities
{
    public class DjikstraMap
    {
        private int _height;
        private int _width;
        private int _minLevel = int.MaxValue;
        private int[][] _returnMap;
        private HashSet<MapCoordinates> _atCurrentDepth;
        private HashSet<MapCoordinates> _atLastDepth;
        private Func<int, int, bool> _fnWalkable;
        private Dictionary<int, List<MapCoordinates>> _goals;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Darrell, 9/17/2016. </remarks>
        ///
        /// <param name="map">          The map to build the Djikstra map in. </param>
        /// <param name="goals">        The goals. </param>
        /// <param name="fnWalkable">   A function which takes col, row and returns whether the area
        ///                                                       there should be considered walkable for
        ///                                                       the Djikstra map. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DjikstraMap(int[][] map, Dictionary<int, List<MapCoordinates>> goals, Func<int, int, bool> fnWalkable)
        {
            Init(map, goals, fnWalkable);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   The goals may take different values.  If goal 1 has value n higher than goal 2 that
        ///             means we're willing to go n steps further to reach goal 1 in preference to goal2.
        ///             Darrell, 9/17/2016. </remarks>
        ///
        /// <param name="height">       The height of the bounding box containing our map. </param>
        /// <param name="width">        The width of the bounding box containing our map. </param>
        /// <param name="goals">        The goals. </param>
        /// <param name="fnWalkable">   A function which takes col, row and returns whether the area
        ///                              there should be considered walkable for the Djikstra map. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DjikstraMap(int height, int width, Dictionary<int, List<MapCoordinates>> goals, Func<int, int, bool> fnWalkable)
        {
            var col = Enumerable.Repeat(int.MaxValue, height).ToArray();
            Init( Enumerable.Repeat(0, width).Select(i => (int[])col.Clone()).ToArray(), goals, fnWalkable);
        }

        private void Init(int[][] map, Dictionary<int, List<MapCoordinates>> goals, Func<int, int, bool> fnWalkable)
        {
            _width = map.Length;
            _height = map[0].Length;
            _fnWalkable = fnWalkable;
            _returnMap = map;
            _atLastDepth = new HashSet<MapCoordinates>();
            _goals = goals;
            foreach (var goal in goals)
            {
                var val = goal.Key;

                if (val < _minLevel)
                {
                    _minLevel = val;
                }
            }
            // Initialize with the lowest level goals
            foreach (var goalLocation in _goals[_minLevel])
            {
                _atLastDepth.Add(goalLocation);
                _returnMap[goalLocation.Column][goalLocation.Row] = _minLevel;
            }
        }

        public int[][] CreateMap()
        {
            for (var curDepth = _minLevel + 1; _atLastDepth.Count > 0; curDepth++)
            {
                _atCurrentDepth = new HashSet<MapCoordinates>();
                foreach (var locLast in _atLastDepth)
                {
                    foreach (var locNext in Neighbors(locLast.Column, locLast.Row))
                    {
                        if (_returnMap[locNext.Column][locNext.Row] > curDepth)
                        {
                            _returnMap[locNext.Column][locNext.Row] = curDepth;
                            _atCurrentDepth.Add(locNext);
                        }
                    }
                }
                _atLastDepth = _atCurrentDepth;

                // See if there are any predefined goals to be added at this depth
                if (_goals.ContainsKey(curDepth))
                {
                    foreach (var goalLocation in _goals[curDepth])
                    {
                        // If we've already reached here with a lower value then skip it
                        if (_returnMap[goalLocation.Column][goalLocation.Row] > curDepth)
                        {
                            // Otherwise set it to the goal value and add it to the goals
                            // reached at this depth.
                            _returnMap[goalLocation.Column][goalLocation.Row] = curDepth;
                            _atLastDepth.Add(goalLocation);
                        }
                    }
                }
            }
            return _returnMap;
        }

        private IEnumerable<MapCoordinates> Neighbors(int col, int row)
        {
            for (var iCol = Math.Max(0, col - 1); iCol <= Math.Min(_width - 1, col + 1); iCol++)
            {
                for (var iRow = Math.Max(0, row - 1); iRow <= Math.Min(_height - 1, row + 1); iRow++)
                {
                    if (col != iCol || row != iRow && _fnWalkable(iCol, iRow))
                    {
                        yield return new MapCoordinates(iCol, iRow);
                    }
                }
            }
        }
    }
}
