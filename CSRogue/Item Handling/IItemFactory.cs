using System;
using System.Collections.Generic;

namespace CSRogue.Item_Handling
{
    public interface IItemFactory
    {
        Dictionary<Guid, ItemInfo> InfoFromId { get; }
    }
}