using System.Collections.Generic;
using CSRogue.GameControl;
using CSRogue.Interfaces;
using CSRogue.Items;
using CSRogue.Item_Handling;
using CSRogue.Utilities;
using RogueSC.Items;

namespace RogueSC.Creatures
{
    class SCCreature : Creature
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>	Gets or sets the hit points. </summary>
        ///
        /// <value>	The hit points. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual int HitPoints { get; set; }

        protected Game Game;
        internal List<Item> Inventory { get; } = new List<Item>();
        private DieRoll HandToHandDamageRoll = new DieRoll("1d6");
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

        internal int CalculateDamage(SCCreature victim)
        {
            int damage = 0;
            if (Inventory.Count > 0)
            {
                // TODO: We've got to have an Equipped list!
                damage = ((Weapon) Inventory[0]).Damage;
            }
            else
            {
                damage = HandToHandDamageRoll.Roll();
            }
            return damage;
        }
    }
}
