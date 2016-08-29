using System;

namespace CSRogue.Item_Handling
{
    public interface IItem
    {
        #region Properties
        ItemInfo Info { get; }
        #endregion
    }
}