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

		public List<IItem> Items { get; }

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
			Items = new List<IItem>();
			Terrain = TerrainType.OffMap;
			LitState = LitState.FogOfWar;
		}

		internal MapLocationData(TerrainType terrain, List<IItem> items = null)
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

		public void AddItem(IItem item)
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

		public void RemoveItem(IItem item)
		{
			Items.Remove(item);
		}
	}
}
