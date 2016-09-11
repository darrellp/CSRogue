using System;
using System.Collections.Generic;
using CSRogue.Interfaces;
using CSRogue.Item_Handling;

namespace CSRogue.Map_Generation
{
    // TODO: Is this sufficient or should we allow the user to define their own lit states?
    // These are the ones we care about.  They are free to define another orthogonal set for their
    // own use.
   [Flags]
	public enum TerrainState
	{
		InView = 0x1,
		Remembered = 0x2,
		FogOfWar = 0x4,
		BlocksView = 0x8,
		Walkable = 0x10
	}

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A map location data. </summary>
    ///
    /// <remarks>   The information stored at each cell of a map.  Ideally I'd love to make this a
    ///             structure since so many are created but in that case the IMap[x,y] construction 
    ///             returns a copy of the structure and we can't use it to modify the actual data
    ///             in the map which is hugely inconvenient.  I'm not sure of a great way to fix this.
    ///             Darrell, 9/9/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

	public class MapLocationData : IMapLocationData
	{
        #region Public properties
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Items at this location. </summary>
        ///
        /// <value> The items. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<IItem> Items { get; set; }

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

		public TerrainState TerrainState { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the room this location is located in. </summary>
        ///
        /// <value> The room. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public IRoom Room { get; set; }
        #endregion

        #region Constructor
        internal MapLocationData()
		{
			Items = new List<IItem>();
			Terrain = TerrainType.OffMap;
			TerrainState = TerrainState.FogOfWar;
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
        #endregion

        #region Item manipulation
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
        #endregion
    }
}
