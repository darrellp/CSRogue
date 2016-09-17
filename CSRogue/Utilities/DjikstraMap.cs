using System;
using System.Collections.Generic;
using System.Linq;
using CSRogue.Map_Generation;

namespace CSRogue.Utilities
{
    class DjikstraMap
    {
        private readonly int _height;
        private readonly int _width;
        private readonly int[][] _returnMap;
        private HashSet<MapCoordinates> _atCurrentDepth;
        private HashSet<MapCoordinates> _atLastDepth;
        private readonly Func<int, int, bool> _fnWalkable;

        public DjikstraMap(int height, int width, List<MapCoordinates> goals, Func<int, int, bool> fnWalkable)
        {
            _height = height;
            _width = width;
            _fnWalkable = fnWalkable;
            _returnMap = Enumerable.Repeat(Enumerable.Repeat(int.MaxValue, height).ToArray(), width).ToArray();
            _atLastDepth = new HashSet<MapCoordinates>();
            foreach (var goalLocation in goals)
            {
                _returnMap[goalLocation.Column][goalLocation.Row] = 0;
                _atLastDepth.Add(goalLocation);
            }
        }

        public int[][] CreateMap(int width, int height, List<MapCoordinates> goals, Func<int, int, bool> fnWalkable)
        {
            for (var curDepth = 1; _atLastDepth.Count > 0; curDepth++)
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
            }
            return _returnMap;
        }

        private IEnumerable<MapCoordinates> Neighbors(int col, int row)
        {
            for (var iCol = Math.Max(0, col - 1); iCol <= Math.Min(_width, col + 1); iCol++)
            {
                for (var iRow = Math.Max(0, row - 1); iRow <= Math.Min(_height, row + 1); iRow++)
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
