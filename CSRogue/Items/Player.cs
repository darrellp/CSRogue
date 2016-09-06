using System;
using CSRogue.Item_Handling;
using CSRogue.Map_Generation;
using CSRogue.Utilities;

namespace CSRogue.Items
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Player object. </summary>
	///
	/// <remarks>	Darrellp, 9/16/2011. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	[Item(ItemType.Player)]
	public class Player : Creature
	{
		#region Constructor

	    public Player() : base(default(Guid))
	    {
	        HitPoints = new DieRoll(1, 6).Roll();
	    }
        #endregion
    }
}
