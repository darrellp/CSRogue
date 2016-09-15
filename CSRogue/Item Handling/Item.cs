using System;
using CSRogue.Interfaces;
using CSRogue.Map_Generation;

namespace CSRogue.Item_Handling
{
	#region Item type enumeration
    // TODO: How best to extend this to arbitrary types created by a third party?
	public enum ItemType
	{
		Nothing,
		Player,
		Rat,
	}

    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>	Generic item class. </summary>
    ///
    /// <remarks>	Darrellp, 9/16/2011. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public abstract class Item : IItem
	{
		#region Properties
		public Guid ItemTypeId { get; set; }
        public MapCoordinates Location { get; set; }
        public char Ch { get; set; }
        #endregion
	}
}
