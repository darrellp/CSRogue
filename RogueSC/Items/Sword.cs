using CSRogue.Item_Handling;
using CSRogue.Map_Generation;
using CSRogue.Utilities;

namespace RogueSC.Items
{
    internal class Sword : Weapon
    {
        public Sword(Level l, ItemInfo i) : base(l, i) { }

        public override string ToString()
        {
            return $"{DamageRollString} Sword";
        }
    }
}
