using System;
using System.Collections.Generic;
using System.Linq;
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
		public ItemType ItemType { get; set; }
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
		/// <summary>	Gets a particular type of attribute from a set of attributes. </summary>
		///
		/// <remarks>	Darrellp, 10/6/2011. </remarks>
		///
		/// <typeparam name="TAttType">	The type of the attribute we're looking for. </typeparam>
		/// <param name="attributes">	The attributes to select from. </param>
		///
		/// <returns>	The located attribute </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		static TAttType GetAttribute<TAttType>(IEnumerable<object> attributes) where TAttType : Attribute
		{
			return attributes.First(at => at.GetType() == typeof(TAttType)) as TAttType;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Initialises the item information for all item classes. </summary>
		///
		/// <remarks>	Darrellp, 9/17/2011. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		static ItemInfo()
		{
			// Retrieve dictionaries from data files
			Dictionary<ItemType, ItemInfo> mapItemTypeToItemInfo = ReadItemData.GetData();
			Dictionary<ItemType, CreatureInfo> mapItemTypeToCreatureInfo = ReadCreatureData.GetData();

			// Get our assembly
			Assembly asm = Assembly.GetExecutingAssembly();

			// For every potential level
			for (int iLevel = 0; iLevel <= MaxLevel; iLevel++)
			{
				MapLevelsToCreatureList[iLevel] = new List<ItemInfo>();
			}

			// For each item type in the assembly
			foreach (Type type in asm.GetTypes().Where(IsItemType))
			{
				// Map the type to the item info
				RegisterType(mapItemTypeToItemInfo, mapItemTypeToCreatureInfo, type);
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Register an item's type. </summary>
		///
		/// <remarks>	Darrellp, 10/6/2011. </remarks>
		///
		/// <param name="mapItemTypeToItemInfo">		Map from ItemType to ItemInfo. </param>
		/// <param name="mapItemTypeToCreatureInfo">	Map from ItemType to CreatureInfo. </param>
		/// <param name="type">							The class type being registered. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private static void RegisterType(
			IDictionary<ItemType, ItemInfo> mapItemTypeToItemInfo, 
			IDictionary<ItemType, CreatureInfo> mapItemTypeToCreatureInfo,
			Type type)
		{
			// Get the item type from the type attributes
			object[] attributes = type.GetCustomAttributes(true);
			ItemAttribute itemAttribute = GetAttribute<ItemAttribute>(attributes);
			ItemType itemType = itemAttribute.ItemType;
			ItemInfo itemInfo = mapItemTypeToItemInfo[itemType];
			CreatureInfo creatureInfo = null;

			// See if the type is the player type
			itemInfo.IsPlayer = type == typeof(Player);

			// Is this item a creature type?
			if (mapItemTypeToCreatureInfo.ContainsKey(itemType))
			{
				// Retrieve it's creature info
				creatureInfo = mapItemTypeToCreatureInfo[itemType];

				// If the creature isn't a player
				if (!itemInfo.IsPlayer)
				{
					// For each level this creature may appear in
					for (int iLevel = creatureInfo.Level; iLevel <= Math.Min(creatureInfo.Level + 3, MaxLevel); iLevel++)
					{
						// Add it to the level's list
						MapLevelsToCreatureList[iLevel].Add(itemInfo);
					}
				}
			}

			// Set Creature info if any
			itemInfo.CreatureInfo = creatureInfo;

			// Put item info in the type dictionary
			MapTypeToItemInfo[type] = itemInfo;
			MapItemTypeToItemInfo[itemType] = itemInfo;

			// Create the proxy for this type
			Item proxy = Activator.CreateInstance(type) as Item;

			// Put it in the character dictionary
			MapCharToProxyItem[itemInfo.Character] = proxy;

			// And in the itemType dictionary
			MapItemTypeToProxyItem[itemInfo.ItemType] = proxy;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Returns true if the passed in type is an item type. </summary>
		///
		/// <remarks>	Darrellp, 9/20/2011. </remarks>
		///
		/// <param name="type">	The class type. </param>
		///
		/// <returns>	true if item type, false if not. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private static bool IsItemType(Type type)
		{
			// Ensure it's in the CSRogue.Items" namespace and has an Item attribute attached
			return type.Namespace == "CSRogue.Items" &&
			    Array.Find(type.GetCustomAttributes(true), at => at.GetType() == typeof (ItemAttribute)) != null;
		}
		#endregion

		#region Queries
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets the information for an instance of an item. </summary>
		///
		/// <remarks>	Darrellp, 9/17/2011. </remarks>
		///
		/// <param name="item">	The item to retrieve information for. </param>
		///
		/// <returns>	The item information. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static ItemInfo GetItemInfo(Item item)
		{
			return GetItemInfo(item.ItemType);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets item information from item type. </summary>
		///
		/// <remarks>	Darrellp, 10/6/2011. </remarks>
		///
		/// <param name="itemType">	The item type for the item. </param>
		///
		/// <returns>	The ItemInfo object for this item type. </returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public static ItemInfo GetItemInfo(ItemType itemType)
		{
			return MapItemTypeToItemInfo[itemType];
		}

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
			ItemType = itemType;
			Value = value;
			Description = description;
		}
		#endregion
	}
}
