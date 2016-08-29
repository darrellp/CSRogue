using System;
using System.Collections.Generic;

namespace CSRogue.Item_Handling
{
    public interface IItemFactory
    {
        List<Guid> ItemsProduced();

        Item Create(Guid itemID);
    }
}