using System;
using System.Collections.Generic;
using System.Reflection;
using CSRogue.Items;

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
		public bool IsCreature
		{
		    get { return CreatureInfo != null; }
		}

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
		#endregion

		#region Private variables
		public const int MaxLevel = 50;
		private static readonly Dictionary<Type, ItemInfo> MapTypeToItemInfo = new Dictionary<Type, ItemInfo>();
		private static readonly Dictionary<ItemType, ItemInfo> MapItemTypeToItemInfo = new Dictionary<ItemType, ItemInfo>();
		private static readonly Dictionary<char, Item> MapCharToProxyItem = new Dictionary<char, Item>();
		private static readonly List<ItemInfo>[] MapLevelsToCreatureList = new List<ItemInfo>[MaxLevel + 1];
		private static readonly Dictionary<ItemType, Item> MapItemTypeToProxyItem = new Dictionary<ItemType, Item>();
		#endregion

		#region Static methods to set up info tables
		////////////////////////////////////////////////////////////////////////////////////////////////////

	    ////////////////////////////////////////////////////////////////////////////////////////////////////

	    #endregion

		#region Queries
		internal static List<ItemInfo> CreatureListForLevel(int depth)
		{
			return MapLevelsToCreatureList[depth];
		}
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets the information for an an item's class type. </summary>
		///
		/// <remarks>	Darrellp, 9/17/2011. </remarks>
		///
		/// <param name="type">	The class type. </param>
		///
		/// <returns>	The item information. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static ItemInfo GetItemInfo(Type type)
		{
			return MapTypeToItemInfo[type];
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Create a new item from char. </summary>
		///
		/// <remarks>	Darrellp, 9/20/2011. </remarks>
		///
		/// <param name="character">	The character representing the item. </param>
		///
		/// <returns>	A new item based on the character passed in. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static Item NewItemFromChar(char character)
		{
			// Create a random item if this type is in our dictionary
			return MapCharToProxyItem.ContainsKey(character) ? MapCharToProxyItem[character].RandomItem() : null;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Create a new item from an item type. </summary>
		///
		/// <remarks>	Darrellp, 9/20/2011. </remarks>
		///
		/// <param name="itemType">	The item type for the item. </param>
		///
		/// <returns>	A new item based on the item type passed in. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static Item NewItemFromItemType(ItemType itemType)
		{
			// Create a new item if this type is in our dictionary
			return MapItemTypeToProxyItem.ContainsKey(itemType) ? MapItemTypeToProxyItem[itemType].RandomItem() : null;
		}
		#endregion

		#region Constructor

	    public ItemInfo(
			ItemType itemType = ItemType.Nothing,
			string name = null,
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
