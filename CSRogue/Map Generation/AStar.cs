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
        private readonly MapCoordinates _target;
	    private readonly MapCoordinates _start;
        private readonly IMap _map;
        #endregion

        #region Constructor
        public AStar(
            MapCoordinates start,
            MapCoordinates target,
            IMap map)
        {
	        _start = start;
            _target = target;
            _map = map;
        }
        #endregion

        #region Wrappers
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

		class ExitWrapper : IComparable
		{
			#region Private variables
			private readonly MapCoordinates _target;
			public double DistanceFromStart { get; }

			#endregion

			#region Public properties
			public ExitWrapper Backlink { get; }
			public ExitNode ExitNode { get; }
			#endregion

			#region Constructor
			public ExitWrapper(
				ExitNode exitNode,
				MapCoordinates target,
				ExitWrapper backLink,
				int distanceToBacklink)
			{
				ExitNode = exitNode;
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
				var other = obj as ExitWrapper;

				if (other == null)
				{
					throw new ArgumentException("Wrong type of object compared to a LocationWrapper");
				}
				return CurDistanceEstimate().CompareTo(other.CurDistanceEstimate());
			}

			private MapCoordinates CurrentLocation()
			{
				return ExitNode.Room.Exits[ExitNode.ExitIndex] + ExitNode.Room.Location;
			}

			private double CurDistanceEstimate()
			{
				return Pathfinding.EstDistance(CurrentLocation(), _target) + DistanceFromStart;
			}
			#endregion
		}

		#endregion

		#region Solving
		public List<MapCoordinates> Solve(Func<MapCoordinates, bool> fnAcceptable = null)
        {
			var closed = new HashSet<MapCoordinates>();
			var openSet = new FibonacciPriorityQueue<LocationWrapper>();
			var dictOpen = new Dictionary<MapCoordinates, FibonacciWrapper<LocationWrapper>>(closed.Comparer);
            closed.Clear();
			openSet.Add(new LocationWrapper(_start, _target, null, 0));

			while (openSet.Count != 0)
            {
                var next = openSet.Pop();
                if (next.Location == _target)
                {
                    return ReconstructPath(next);
                }
                closed.Add(next.Location);

                foreach (var neighbor in _map.Neighbors(next.Location).Where(m => fnAcceptable == null || fnAcceptable(m)))
                {
                    if (closed.Contains(neighbor))
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
                            openSet.DecreaseKey(locationWrapperInOpen, newWrapper);
                        }
                    }
                    else
                    {
                        dictOpen[neighbor] = openSet.Add(
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

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Solve rooms. </summary>
		///
		/// <remarks>	The tuples in _closed are the room and the index of the exit of the room and
		///				represent the EXIT in that room - not the entrance.  So the next edges after
		///				this room/exit pair will be the edges in the room connected to the corresponding
		///				exit in the current room.  The exception is when the exit is -1 in which case
		///				we're referring to the starting position or ending position.  The list of ints
		///				are the exit indices starting in the starting room.  Since these uniquely identify
		///				the next room there is no need to keep the next room information in the returned
		///				list.
		///
		/// 			Darrell, 10/1/2016. </remarks>
		///
		/// <param name="map"> 	The map. </param>
		/// <param name="pos1">	The first position. </param>
		/// <param name="pos2">	The second position. </param>
		///
		/// <returns>	A room by room list of exit indices taken from start to goal </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public List<int> SolveRooms(IRoomsMap map, MapCoordinates pos1, MapCoordinates pos2)
		{
			var comparer = new ExitComparer();
			var closed = new HashSet<ExitNode>(comparer: comparer);
			var openSet = new FibonacciPriorityQueue<ExitWrapper>();
			var dictOpen = new Dictionary<ExitNode, FibonacciWrapper<ExitWrapper>>();

			if (map[pos1].Room == map[pos2].Room)
			{
				// If they're in the same room there's no exits to traverse so return an empty list
				return new List<int>();
			}
			openSet.Add(new ExitWrapper(new ExitNode(map[pos1].Room, -1), _target, null, 0));
			closed.Clear();

			while (openSet.Count != 0)
			{
				var next = openSet.Pop();
				if (next.ExitNode.Room == map[_target].Room)
				{
					return ReconstructRoomPath(next);
				}
				closed.Add(new ExitNode(next.ExitNode.Room, next.ExitNode.ExitIndex));

				foreach (var neighbor in ExitNeighbors(next))
				{
					if (closed.Contains(neighbor))
					{
						continue;
					}
					var distanceToNeighbor = RoomDistToNeighbor(next.ExitNode, neighbor);
					if (dictOpen.ContainsKey(neighbor))
					{
						var exitWrapperInOpen = dictOpen[neighbor];
						if ((exitWrapperInOpen.Value).DistanceFromStart > next.DistanceFromStart + distanceToNeighbor)
						{
							var newWrapper = new ExitWrapper(
								neighbor,
								_target,
								next,
								distanceToNeighbor);
							openSet.DecreaseKey(exitWrapperInOpen, newWrapper);
						}
					}
					else
					{
						dictOpen[neighbor] = openSet.Add(
							new ExitWrapper(
								neighbor,
								_target,
								next,
								distanceToNeighbor));
					}
				}
			}
			return null;
		}

		class ExitNode
		{
			public readonly int ExitIndex;
			public readonly IRoom Room;

			public ExitNode(IRoom room, int exitIndex)
			{
				Room = room;
				ExitIndex = exitIndex;
			}

			public MapCoordinates Location()
			{
				return Room.Location + Room.Exits[ExitIndex];
			}
		}

	    class ExitComparer : IEqualityComparer<ExitNode>
	    {
		    public bool Equals(ExitNode x, ExitNode y)
		    {
			    return x.Room == y.Room && x.ExitIndex == y.ExitIndex;
		    }

		    public int GetHashCode(ExitNode obj)
		    {
			    return obj.Room.GetHashCode() ^ obj.ExitIndex.GetHashCode();
		    }
	    }

	    private int RoomDistToNeighbor(ExitNode node1, ExitNode node2)
	    {
		    MapCoordinates startPosRoom2 = node1.Location() - node2.Room.Location;
		    return node2.Room.ExitDMaps[node2.ExitIndex][startPosRoom2.Column][startPosRoom2.Row];
	    }

		private List<ExitNode> ExitNeighbors(ExitWrapper next)
	    {
		    var roomCur = next.ExitNode.Room;
		    var iExitCur = next.ExitNode.ExitIndex;
			var roomNext = iExitCur == -1 ? roomCur : roomCur.NeighborRooms[iExitCur];
			var ret = new List<ExitNode>();
		    for(var i = 0; i < roomNext.Exits.Count; i++)
		    {
			    if (i == iExitCur)
			    {
				    continue;
			    }
				ret.Add(new ExitNode(roomNext, i));
		    }
		    return ret;
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

		private static List<int> ReconstructRoomPath(ExitWrapper next)
        {
            var ret = new List<int>();
            while (next != null)
            {
                ret.Add(next.ExitNode.ExitIndex);
                next = next.Backlink;
            }
            ret.Reverse();
            return ret;
        }
        #endregion
    }
}
