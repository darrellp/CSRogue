﻿using System;
using CSRogue.Map_Generation;

namespace CSRogue.Item_Handling
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Information about individual items. </summary>
	///
	/// <remarks>	Darrellp, 9/17/2011. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public class ItemInfo
	{
		#region Public Properties
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets a value indicating whether this object is creature. </summary>
		///
		/// <value>	true if this object is creature, false if not. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public bool IsCreature => CreatureInfo != null;

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets a value indicating whether this object is player. </summary>
		///
		/// <value>	true if this object is player, false if not. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public bool IsPlayer { get; set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets the character representation of this item. </summary>
		///
		/// <value>	The character. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public char Character { get; set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets the type of the item. </summary>
		///
		/// <value>	The type of the item. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
        public Guid ItemId { get; set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the name. </summary>
		///
		/// <value>	The name. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public string Name { get; set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the weight of the item. </summary>
		///
		/// <value>	The weight of the item. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public double Weight { get; set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the value of the object. </summary>
		///
		/// <value>	The value. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public int Value { get; set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets a description of the item. </summary>
		///
		/// <value>	The description. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public string Description { get; set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	If this item is a creature, gets the information describing the creature. </summary>
		///
		/// <value>	Information describing the creature. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public CreatureInfo CreatureInfo { get; set; } 

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the item. </summary>
		///
		/// <value>	An action to create the item. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public Func<Level, IItem> CreateItem { get; set; }
		#endregion

		#region Static methods to set up info tables
		////////////////////////////////////////////////////////////////////////////////////////////////////

	    ////////////////////////////////////////////////////////////////////////////////////////////////////

	    #endregion

		#region Constructor

	    public ItemInfo(string name = null,
			char character = ' ',
			double weight = 0,
			int value = 0,
			string description = "A singularly uninteresting item")
		{
			Name = name;
			Character = character;
			Weight = weight;
			Value = value;
			Description = description;
		}
		#endregion
	}
}
