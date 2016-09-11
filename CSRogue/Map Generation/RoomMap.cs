using System;
using System.Collections.Generic;

namespace CSRogue.Map_Generation
{
    public class RoomMap : BaseMap, IRoomsMap
    {
        public RoomMap(
			ISet<IRoom> rooms,
			int height, int width,
			Func<IMapLocationData> dataCreator = null) : base(height, width, dataCreator)
        {
            Rooms = rooms;
        }

        public ISet<IRoom> Rooms { get; }
    }
}
