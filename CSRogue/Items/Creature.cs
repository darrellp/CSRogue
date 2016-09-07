﻿using System;
using CSRogue.Item_Handling;
using CSRogue.Map_Generation;

namespace CSRogue.Items
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Base class for creatures. </summary>
	///
	/// <remarks>	Darrellp, 9/16/2011. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public abstract class Creature : IItem, ICreature
	{
		#region Internal properties
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the hit points. </summary>
		///
		/// <value>	The hit points. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public int HitPoints { get; set; }

	    ////////////////////////////////////////////////////////////////////////////////////////////////////
	    /// <summary>	Gets or sets the identifier of the item type. </summary>
	    ///
	    /// <value>	The identifier of the item type. </value>
	    ////////////////////////////////////////////////////////////////////////////////////////////////////

	    public Guid ItemTypeId { get; set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
	    /// <summary>	Gets or sets the location. </summary>
		///
		/// <value>	The location. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public MapCoordinates Location { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the character representing the creature. </summary>
        ///
        /// <value> The character. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

	    public char Ch { get; set; }

	    ////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets a value indicating whether this object is the player. </summary>
		///
		/// <value>	true if this object is player, false if not. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public bool IsPlayer => this is IPlayer;
		#endregion

		#region Constructor

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Darrellp, 9/16/2011. </remarks>
        ///
        /// <param name="itemTypeId">   (Optional) Item ID. </param>
        /// <param name="level">    (Optional) the level. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

	    protected Creature(Guid itemTypeId = default(Guid), Level level = null)
		{
	        ItemTypeId = itemTypeId;

            if (level != null)
            {
	            var info = level.Factory.InfoFromId[itemTypeId];
	            HitPoints = info.CreatureInfo.HitPoints.Roll();
            }
		}
        #endregion

        #region Virtual methods
        public void InvokeAi()
	    {
	    }
        #endregion  
    }
}
