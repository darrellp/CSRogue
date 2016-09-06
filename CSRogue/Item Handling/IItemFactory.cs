using System;
using System.Collections.Generic;
using CSRogue.Map_Generation;

namespace CSRogue.Item_Handling
{
    public interface IItemFactory
    {
        Dictionary<Guid, ItemInfo> InfoFromId { get; }
    }
}