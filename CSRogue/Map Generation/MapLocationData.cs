using System.Collections.Generic;
using System.Linq;
using CSRogue.Item_Handling;

namespace CSRogue.Map_Generation
{
    // TODO: Is this sufficient or should we allow the user to define their own lit states?
	public enum LitState
	{
		InView,
		Remembered,
		FogOfWar
	}

	public class MapLocationData
	{
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Items at this location. </summary>
        ///
        /// <value> The items. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public List<Item> Items { get; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the type of terrain at this location. </summary>
        ///
        /// <value> The terrain. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public TerrainType Terrain { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the lit state of this location. </summary>
        ///
        /// <value> The lit state. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public LitState LitState { get; internal set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the room this location is located in. </summary>
        ///
        /// <value> The room. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public GenericRoom Room { get; internal set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   A tag to put whatever you'd like at this location. </summary>
        ///
        /// <value> The tag. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public object Tag { get; set; }

		internal MapLocationData()
		{
			Items = new List<Item>();
			Terrain = TerrainType.OffMap;
			LitState = LitState.FogOfWar;
		}

		internal MapLocationData(TerrainType terrain, List<Item> items = null)
			: this()
		{
			if (items != null)
			{
				Items = items;
			}
			Terrain = terrain;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Adds an item at this location. </summary>
        ///
        /// <remarks>   Darrellp, 8/25/2016. </remarks>
        ///
        /// <param name="item"> The item to be added. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public void AddItem(Item item)
		{
			Items.Add(item);
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Removes the item described by item from this location. </summary>
        ///
        /// <remarks>   Darrellp, 8/25/2016. </remarks>
        ///
        /// <param name="item"> The item to be removed. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public void RemoveItem(Item item)
		{
			Items.Remove(item);
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Searches for the first item with a given item type. </summary>
        ///
        /// <remarks>   Darrellp, 8/25/2016. </remarks>
        ///
        /// <param name="itemType"> Type of the item being searched for. </param>
        ///
        /// <returns>   The found item. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		internal Item FindItemType(ItemType itemType)
		{
			return Items.First(item => item.ItemType == itemType);
		}
	}
}
