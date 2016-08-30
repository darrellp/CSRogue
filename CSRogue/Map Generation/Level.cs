using System.Collections.Generic;
using System.Linq;
using CSRogue.GameControl;
using CSRogue.GameControl.Commands;
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
		private readonly List<Creature> _creatures = new List<Creature>();
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
        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   We are passing down a map but would expect to excavate it ourselves using the
        ///             supplied excavator.  If you excavate the map by yourself that's fine but you
        ///             need to pass down a NullExcavator.  Passing down null (the default) uses a
        ///             GridExcavator.  Darrell, 8/29/2016. </remarks>
        ///
        /// <param name="depth">        The depth of the level. </param>
        /// <param name="map">          The map for the level. </param>
        /// <param name="game">         The game we're part of. </param>
        /// <param name="excavator">    (Optional) the excavator to create the map. </param>
        /// <param name="seed">         (Optional) the seed. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		public Level(int depth, IGameMap map, IItemFactory factory, IExcavator excavator = null, int seed = -1 )
		{
			_factory = factory;
		    Map = map ?? new CsRogueMap();
            Depth = depth;
			if (excavator == null)
			{
                excavator = new GridExcavator(seed);
			}
			excavator.Excavate(Map);
			DistributeItems();
		}
        #endregion

        #region Map Distribution
        // TODO: Allow for more flexibility here
        protected virtual int CreatureCount()
		{
			return Map.Width * Map.Height / 1000 + Rnd.Global.Next(7) - 3;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Distribute items on this level. </summary>
        ///
        /// <remarks>   Darrell, 8/29/2016. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

		private void DistributeItems()
		{
			DistributeCreatures();
			DistributeInanimate();
		}

		protected virtual void DistributeInanimate()
		{
		}

		protected virtual void DistributeCreatures()
		{
            // Figure out how many monsters on this level
            var creatureCount = CreatureCount();

            if (_factory != null)
		    {
		        var infoFromIds = _factory.InfoFromId;
		        var creatureInfoListFactory = infoFromIds.Values;
		        var sumRarity = 
                    infoFromIds.
                    Values.
                    Where(i => i.IsCreature).
                    Select(i => i.CreatureInfo.Rarity).
                    Sum();

		        if (sumRarity == 0)
		        {
		            return;
		        }

                // For each creature
                for (var iCreature = 0; iCreature < creatureCount; iCreature++)
                {
                    // Select a creature
                    var creature = SelectFactoryCreature(creatureInfoListFactory, sumRarity);

                    // Place the selected creature
                    PlaceCreature(creature);
                }
            }
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Select a creature from the factory for inclusion in the level. </summary>
        ///
        /// <remarks>   Darrell, 8/29/2016. </remarks>
        ///
        /// <exception cref="RogueException">   Thrown when a Rogue error condition occurs. </exception>
        ///
        /// <param name="creatureList">     List of creatures. </param>
        /// <param name="sumOfRarities">    The sum of rarities. </param>
        ///
        /// <returns>   A Creature. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private Creature SelectFactoryCreature(IEnumerable<ItemInfo> creatureList, int sumOfRarities)
        {
            var cumulationLimit = Rnd.Global.Next(sumOfRarities);
            var rarityCumulation = 0;

            foreach (ItemInfo creature in creatureList)
            {
                rarityCumulation += creature.CreatureInfo.Rarity;
                if (rarityCumulation > cumulationLimit)
                {
                    return (Creature) _factory.Create(creature.ItemId, this);
                }
            }
            throw new RogueException("Couldn't find creature in SelectCreature");
        }

        private void PlaceCreature(Creature creature)
		{
			// Find a random floor location
			var location = Map.RandomFloorLocation();

			// Is there a creature there?
			while (Map[location].Items.Any(item => _factory.InfoFromId[item.ItemId].IsCreature))
			{
				// Find another position
				location = Map.RandomFloorLocation();
			}

			// Place the monster on the map
			Map.Drop(location, creature);
			creature.Location = location;
			_creatures.Add(creature);
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

        public void KillCreature(Creature victim)
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
