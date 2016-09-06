using System;
using CSRogue.Map_Generation;

namespace CSRogue.Item_Handling
{
    public interface IItem
    {
        // Guid which uniquely represents the type of this item
        Guid ItemTypeId { get; set; }
        MapCoordinates Location { get; set; }
        char Ch { get;}
    }
}