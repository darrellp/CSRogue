using System.Collections.Generic;

namespace CSRogue.Interfaces
{
    // An interface to pass to excavators.
    public interface IRoomsMap : IMap
    {
        // If your excavator doesn't produce rooms then this can go unimplemented
        ISet<IRoom> Rooms { get; }
    }
}