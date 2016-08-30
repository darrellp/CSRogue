using System;
using CSRogue.Map_Generation;
using Malison.Core;

namespace CSRogue.Item_Handling
{
    public interface IItem
    {
        // Guid which uniquely represents the type of this item
        Guid ItemTypeId { get; }
        MapCoordinates Location { get; set; }
        Character Ch { get; set; }
    }
}