using System.Collections.Generic;

namespace CSRogue.Map_Generation
{
    public class RoomMap : BaseMap, IRoomsMap
    {
        public RoomMap(ISet<IRoom> rooms, int height, int width) : base(height, width)
        {
            Rooms = rooms;
        }

        public ISet<IRoom> Rooms { get; }
    }
}
