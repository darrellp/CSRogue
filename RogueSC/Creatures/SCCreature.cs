using System.Collections.Generic;
using CSRogue.GameControl;
using CSRogue.Interfaces;
using CSRogue.Items;
using CSRogue.Item_Handling;

namespace RogueSC.Creatures
{
    class SCCreature : Creature
    {
        protected Game Game;
        internal List<Item> Inventory { get; } = new List<Item>();

        public SCCreature(ILevel l)
        {
            Game = l?.Map.Game;
        }
    }
}
