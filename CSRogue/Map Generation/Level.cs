using System;
using System.Collections.Generic;
using System.Linq;
using CSRogue.Item_Handling;
using CSRogue.Items;
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

	public class Level
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
		/// <param name="excavator">	(Optional) the excavator to create the map. </param>
		/// <param name="seed">			(Optional) the seed. </param>
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public Level(int depth, IGameMap map, IItemFactory factory, Dictionary<Guid, int> rarity, IExcavator excavator = null, int seed = -1 )
		{
			_factory = factory;
		    Map = map ?? new CsRogueMap();
            Depth = depth;
			if (excavator == null)
			{
                excavator = new GridExcavator(seed);
			}
			excavator.Excavate(Map);
			DistributeItems(rarity);
		}
        #endregion

        #region Map Distribution
        // TODO: Allow for more flexibility here
        protected virtual int ItemCount(bool areCreatures)
		{
			return Map.Width * Map.Height / 1000 + Rnd.Global.Next(7) - 3;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Distribute items on this level. </summary>
        ///
        /// <remarks>   Darrell, 8/29/2016. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		private void DistributeItems(Dictionary<Guid, int> rarity)
        {
            var itemInfoList = rarity.Keys.Select(guid => _factory.InfoFromId[guid]);
            var creatureInfoList = new List<ItemInfo>();
            var inanimateInfoList = new List<ItemInfo>();

            foreach (var itemInfo in itemInfoList)
            {
                if (itemInfo.IsCreature)
                {
                    creatureInfoList.Add(itemInfo);
                }
                else
                {
                    inanimateInfoList.Add(itemInfo);
                }
            }

			DistributeItems(rarity, creatureInfoList, true);
			DistributeItems(rarity, inanimateInfoList, false);
		}

		protected virtual void DistributeItems(Dictionary<Guid, int> rarity, List<ItemInfo> itemInfoList, bool areCreatures)
		{
            // Figure out how many monsters on this level
            var itemCount = ItemCount(areCreatures);

            if (_factory != null)
		    {
		        var sumRarity = rarity.Values.Sum();

		        if (sumRarity == 0)
		        {
		            return;
		        }

                // For each item
                for (var iItem = 0; iItem < itemCount; iItem++)
                {
                    // Select an item
                    var item = SelectItem(rarity, itemInfoList, sumRarity);

                    // Place the selected creature
                    PlaceItem(item, areCreatures);
                }
            }
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>	Select a creature from the factory for inclusion in the level. </summary>
        ///
        /// <remarks>	Darrell, 8/29/2016. </remarks>
        ///
        /// <exception cref="RogueException">	Thrown when a Rogue error condition occurs. </exception>
        ///
        /// <param name="rarity">			The rarity. </param>
        /// <param name="itemList">			List of items. </param>
        /// <param name="sumOfRarities">	The sum of rarities. </param>
        ///
        /// <returns>	A Creature. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private IItem SelectItem(Dictionary<Guid, int> rarity, List<ItemInfo> itemList, int sumOfRarities)
        {
            var cumulationLimit = Rnd.Global.Next(sumOfRarities);
            var rarityCumulation = 0;

            foreach (var info in itemList)
            {
                rarityCumulation += rarity[info.ItemId];
                if (rarityCumulation > cumulationLimit)
                {
                    return info.CreateItem(this);
                }
            }
            throw new RogueException("Couldn't find creature in SelectCreature");
        }

        private void PlaceItem(IItem item, bool isCreature)
		{
			// Find a random floor location
			var location = Map.RandomFloorLocation();

			// Is there a creature there?
			while (Map[location].Items.Any(i => _factory.InfoFromId[i.ItemTypeId].IsCreature))
			{
				// Find another position
				location = Map.RandomFloorLocation();
			}

			// Place the item on the map
			Map.Drop(location, item);
			item.Location = location;

            if (isCreature)
            {
                _creatures.Add((Creature)item);
            }
		}
        #endregion

        #region Actions on creatures
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Kill a creature. </summary>
        ///
        /// <remarks>   Darrell, 8/29/2016. </remarks>
        ///
        /// <param name="victim">   The victim. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void KillCreature(ICreature victim)
		{
			Map.Remove(victim.Location.Column, victim.Location.Row, victim);
			_creatures.Remove(victim);
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Has each monster do whatever that monster wants to do.
        /// </summary>
        ///
        /// <remarks>   Darrell, 8/29/2016. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public void InvokeMonsterAI()
		{
			foreach (var creature in _creatures)
			{
				if (!creature.IsPlayer)
				{
				    creature.InvokeAi();
				}
			}
		}
        #endregion
    }
}
