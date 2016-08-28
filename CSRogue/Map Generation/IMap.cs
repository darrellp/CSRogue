using System.Collections.Generic;

namespace CSRogue.Map_Generation
{
    public interface IMap
    {
        MapLocationData this[int iCol, int iRow] { get; set; }
        MapLocationData this[MapCoordinates loc] { get; set; }
        int Height { get; }
        int Width { get; }

        // Currently the only thing that deals with rooms is GridExcavator.  Feel free
        // to not implement this if you don't need it and aren't using GridExcavator.
        ISet<GenericRoom> Rooms { get; }
    }
}
