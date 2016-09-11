using System;
using System.Collections.Generic;
using CSRogue.Interfaces;
using CSRogue.Items;
using CSRogue.Item_Handling;
using CSRogue.Utilities;

namespace CSRogue.Map_Generation
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A level. </summary>
    ///
    /// <remarks>   Main purpose of a level is to create/contain a map and put the player on 
    ///             a downstairs cell.  Also we trigger monster movement at the Level..er...level.
    ///             Darrell, 8/29/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

	public class Level : ILevel
	{
		#region Private fields
        private readonly IItemFactory _factory;

        /// <summary>   The creatures on this level. </summary>
		private readonly List<ICreature> _creatures = new List<ICreature>();
		#endregion

		#region Public Properties
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the depth of the level. </summary>
        ///
        /// <value> The depth. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
		public int Depth { get; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the map for the level. </summary>
        ///
        /// <value> The map. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
		public IGameMap Map { get; }

        public IItemFactory Factory => _factory;

        public List<ICreature> Creatures => _creatures;
        #endregion

		#region Constructor

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Constructor. </summary>
		///
		/// <remarks>
		/// We are passing down a map but would expect to excavate it ourselves using the supplied
		/// excavator.  If you excavate the map by yourself that's fine but you need to pass down a
		/// NullExcavator.  Passing down null (the default) uses a GridExcavator.  Darrell, 8/29/2016.
		/// </remarks>
		///
		/// <param name="depth">		The depth of the level. </param>
		/// <param name="map">			The map for the level. </param>
		/// <param name="factory">  	The items in our game. </param>
		/// <param name="rarity">   	The rarity. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public Level(int depth, IGameMap map, IItemFactory factory, Dictionary<Guid, int> rarity)
		{
			_factory = factory;
		    Map = map;
            Depth = depth;
			this.DistributeItems(rarity);
		}
        #endregion

        #region Map Distribution
        // TODO: Allow for more flexibility here
        public int ItemCount(bool areCreatures)
		{
			return Map.Width * Map.Height / 1000 + Rnd.Global.Next(7) - 3;
		}
        #endregion
    }
}
