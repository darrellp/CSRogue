using System.Collections.Generic;
using System.Linq;
using CSRogue.Interfaces;
using static System.Math;

namespace CSRogue.Map_Generation
{
    public class Pathfinding
    {
        private readonly IMap _map;
        private readonly bool _isRoomMap;

        public Pathfinding(IMap map)
        {
            _map = map;
            var roomsMap = map as IRoomsMap;
            if (roomsMap != null)
            {
                _isRoomMap = roomsMap.Rooms.All(r => r is Room);
            }
        }

        public List<MapCoordinates> FindPath(MapCoordinates src, MapCoordinates dst)
        {
            if (_isRoomMap)
            {
                return FindRoomPath(src, dst);
            }
            else
            {
                return AStar(src, dst);
            }
        }

        internal static int EstDistance(MapCoordinates src, MapCoordinates dst)
        {
            // This is the "exact" distance assuming no obstacles but is very ambiguous with
            // lots of points at the same "distance" which causes us to search around a lot more
            // and makes for erratic paths
            
            //return Max(Abs(src.Column - dst.Column), Abs(src.Row - dst.Row));
            
            // This is an overestimate which is normally bad but it works here.
            return Abs(src.Column - dst.Column) + Abs(src.Row - dst.Row);
        }

        private List<MapCoordinates> AStar(MapCoordinates src, MapCoordinates dst)
        {
            return (new AStar(src, dst, _map)).Solve();
        }

        private List<MapCoordinates> FindRoomPath(MapCoordinates src, MapCoordinates dst)
        {
            return AStar(src, dst);
        }
    }
}
