using System;
using System.Collections.Generic;
using CSRogue.Item_Handling;

namespace CSRogue.Interfaces
{
    public interface IItemFactory
    {
        Dictionary<Guid, ItemInfo> InfoFromId { get; }
    }
}