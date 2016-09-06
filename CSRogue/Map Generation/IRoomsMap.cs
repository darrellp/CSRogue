using System.Collections.Generic;

namespace CSRogue.Map_Generation
{
    // An interface to pass to excavators.
    public interface IRoomsMap : IMap
    {
        // If your excavator doesn't produce rooms then this can go unimplemented
        ISet<GenericRoom> Rooms { get; }
    }
}