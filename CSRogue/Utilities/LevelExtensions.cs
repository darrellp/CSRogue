using System;
using System.Collections.Generic;
using System.Linq;
using CSRogue.Interfaces;
using CSRogue.Items;
using CSRogue.Item_Handling;

namespace CSRogue.Utilities
{
    public static class LevelExtensions
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Has each monster do whatever that monster wants to do.
        /// </summary>
        ///
        /// <remarks>   Darrell, 8/29/2016. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void InvokeMonsterAI(this ILevel level)
        {
            foreach (var creature in level.Creatures)
            {
                if (!creature.IsPlayer)
                {
                    creature.InvokeAi();
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Kill a creature. </summary>
        ///
        /// <remarks>   Darrell, 8/29/2016. </remarks>
        ///
        /// <param name="level">    The level to act on. </param>
        /// <param name="victim">   The victim. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void KillCreature(this ILevel level, ICreature victim)
        {
            level.Map.Remove(victim.Location.Column, victim.Location.Row, victim);
            level.Creatures.Remove(victim);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Distribute items on this level. </summary>
        ///
        /// <remarks>   Darrell, 8/29/2016. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void DistributeItems(this ILevel level, Dictionary<Guid, int> rarity)
        {
            if (rarity == null)
            {
                return;
            }
            var itemInfoList = rarity.Keys.Select(guid => level.Factory.InfoFromId[guid]);
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

            level.DistributeItems(rarity, creatureInfoList, true);
            level.DistributeItems(rarity, inanimateInfoList, false);
        }

        public static void DistributeItems(this ILevel level, Dictionary<Guid, int> rarity, List<ItemInfo> itemInfoList, bool areCreatures)
        {
            if (itemInfoList.Count == 0)
            {
                return;
            }

            // Figure out how many items on this level
            var itemCount = level.ItemCount(areCreatures);

            if (level.Factory != null)
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
                    var item = level.SelectItem(rarity, itemInfoList, sumRarity);

                    // Place the selected creature
                    level.PlaceItem(item, areCreatures);
                }
            }
        }

        public static IItem SelectItem(this ILevel level, Dictionary<Guid, int> rarity, List<ItemInfo> itemList, int sumOfRarities)
        {
            var cumulationLimit = Rnd.Global.Next(sumOfRarities);
            var rarityCumulation = 0;

            foreach (var info in itemList)
            {
                rarityCumulation += rarity[info.ItemId];
                if (rarityCumulation > cumulationLimit)
                {
                    return info.CreateItem(level);
                }
            }
            throw new RogueException("Couldn't find creature in SelectCreature");
        }

        public static void PlaceItem(this ILevel level, IItem item, bool isCreature)
        {
            // Find a random floor location
            var location = level.Map.RandomFloorLocation();

            // Is there a creature there?
            while (level.Map[location].Items.Any(i => level.Factory.InfoFromId[i.ItemTypeId].IsCreature))
            {
                // Find another position
                location = level.Map.RandomFloorLocation();
            }

            // Place the item on the map
            level.Map.Drop(location, item);
            item.Location = location;

            if (isCreature)
            {
                level.Creatures.Add((Creature)item);
            }
        }
    }
}
