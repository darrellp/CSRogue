using System.Collections.Generic;
using CSRogue.GameControl;
using CSRogue.Interfaces;
using CSRogue.Items;
using CSRogue.Item_Handling;
using CSRogue.Utilities;

namespace RogueSC.Creatures
{
    class SCCreature : Creature
    {
        protected Game Game;
        internal List<Item> Inventory { get; } = new List<Item>();
        public SCCreature(ILevel l, ItemInfo i)
        {
            Game = l?.Map.Game;
            var extra = i?.CreatureInfo?.Extra;
            if (extra != null && extra.Length > 0)
            {
                var hitPoints = new DieRoll(extra[0]);
                HitPoints = hitPoints.Roll();
            }
        }
    }
}
