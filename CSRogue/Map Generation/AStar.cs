using System;
using System.Collections.Generic;
using System.Linq;
using CSRogue.Interfaces;
using CSRogue.Utilities;
using Priority_Queue;

namespace CSRogue.Map_Generation
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A* algorithm for CSRogue. </summary>
    ///
    /// <remarks>   Darrell, 9/22/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public class AStar
    {
        #region Private Variables
        private readonly HashSet<MapCoordinates> _closed = new HashSet<MapCoordinates>();
        private readonly FibonacciPriorityQueue<LocationWrapper> _openSet =
            new FibonacciPriorityQueue<LocationWrapper>();
        private readonly MapCoordinates _target;
        private readonly IMap _map;
        #endregion

        #region Constructor
        public AStar(
            MapCoordinates start,
            MapCoordinates target,
            IMap map)
        {
            _target = target;
            _map = map;
            _openSet.Add(new LocationWrapper(start, _target, null, 0));
        }
        #endregion

        #region Location Wrapper
        class LocationWrapper : IComparable
        {
            #region Private variables
            private readonly MapCoordinates _target;
            public double DistanceFromStart { get; }

            #endregion

            #region Public properties
            public LocationWrapper Backlink { get; }
            public MapCoordinates Location { get; }
            #endregion

            #region Constructor
            public LocationWrapper(
                MapCoordinates location,
                MapCoordinates target,
                LocationWrapper backLink,
                int distanceToBacklink)
            {
                Location = location;
                _target = target;
                Backlink = backLink;
                if (backLink != null)
                {
                    DistanceFromStart = backLink.DistanceFromStart + distanceToBacklink;
                }
            }
            #endregion

            #region Comparison
            public int CompareTo(object obj)
            {
                var other = obj as LocationWrapper;

                if (other == null)
                {
                    throw new ArgumentException("Wrong type of object compared to a LocationWrapper");
                }
                return CurDistanceEstimate().CompareTo(other.CurDistanceEstimate());
            }

            private double CurDistanceEstimate()
            {
                return Pathfinding.EstDistance(Location, _target) + DistanceFromStart;
            }
            #endregion
        }
        #endregion

        #region Solving
        public List<MapCoordinates> Solve(Func<MapCoordinates, bool> fnAcceptable = null)
        {
            var dictOpen = new Dictionary<MapCoordinates, FibonacciWrapper<LocationWrapper>>(_closed.Comparer);
            _closed.Clear();

            while (_openSet.Count != 0)
            {
                var next = _openSet.Pop();
                if (next.Location == _target)
                {
                    return ReconstructPath(next);
                }
                _closed.Add(next.Location);

                foreach (var neighbor in _map.Neighbors(next.Location).Where(m => fnAcceptable == null || fnAcceptable(m)))
                {
                    if (_closed.Contains(neighbor))
                    {
                        continue;
                    }
                    var distanceToNeighbor = 1;
                    if (dictOpen.ContainsKey(neighbor))
                    {
                        var locationWrapperInOpen = dictOpen[neighbor];
                        if (((LocationWrapper)locationWrapperInOpen).DistanceFromStart > next.DistanceFromStart + distanceToNeighbor)
                        {
                            var newWrapper = new LocationWrapper(
                                neighbor,
                                _target,
                                next,
                                distanceToNeighbor);
                            _openSet.DecreaseKey(locationWrapperInOpen, newWrapper);
                        }
                    }
                    else
                    {
                        dictOpen[neighbor] = _openSet.Add(
                            new LocationWrapper(
                                neighbor,
                                _target,
                                next,
                                distanceToNeighbor));
                    }
                }
            }
            return null;
        }

        private static List<MapCoordinates> ReconstructPath(LocationWrapper next)
        {
            var ret = new List<MapCoordinates>();
            while (next != null)
            {
                ret.Add(next.Location);
                next = next.Backlink;
            }
            ret.Reverse();
            return ret;
        }
        #endregion
    }
}
