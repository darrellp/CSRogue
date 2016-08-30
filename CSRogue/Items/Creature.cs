using System;
using CSRogue.Item_Handling;
using CSRogue.Map_Generation;

namespace CSRogue.Items
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Base class for creatures. </summary>
	///
	/// <remarks>	Darrellp, 9/16/2011. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public abstract class Creature : Item
	{
		#region Internal properties
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the hit points. </summary>
		///
		/// <value>	The hit points. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		internal int HitPoints { get; set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the location. </summary>
		///
		/// <value>	The location. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public MapCoordinates Location { get; set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets a value indicating whether this object is the player. </summary>
		///
		/// <value>	true if this object is player, false if not. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public bool IsPlayer { get; set; }
		#endregion

		#region Constructor

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Darrellp, 9/16/2011. </remarks>
        ///
        /// <param name="itemId">   (Optional) Item ID. </param>
        /// <param name="level">    (Optional) the level. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

	    protected Creature(Guid itemId = default(Guid), Level level = null)
			: base(itemId)
		{
		    ItemInfo info;

			IsPlayer = itemId == HeroId;
            if (level != null)
            {
                info = level.Factory.InfoFromId[itemId];
			    HitPoints = info.CreatureInfo.HitPoints.Roll();
            }
		}
        #endregion

        #region Virtual methods
        public virtual void InvokeAi()
	    {
	    }
        #endregion  
    }
}
