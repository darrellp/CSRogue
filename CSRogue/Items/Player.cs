using System;
using CSRogue.Utilities;

namespace CSRogue.Items
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Player object. </summary>
	///
	/// <remarks>	Darrellp, 9/16/2011. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public class Player : Creature
	{
		#region Constructor
	    public Player()
	    {
	        HitPoints = new DieRoll(1, 6).Roll();
	    }
        #endregion
    }
}
