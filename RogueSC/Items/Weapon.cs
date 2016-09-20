using CSRogue.Item_Handling;
using CSRogue.Map_Generation;
using CSRogue.Utilities;

namespace RogueSC.Items
{
    internal class Weapon : Item
    {
        internal int Damage { get; }
        internal string DamageRollString { get; }

        // ReSharper disable once UnusedParameter.Local
        public Weapon(Level l, ItemInfo i)
        {
            DamageRollString = i.Extra[0];
            var damageRoll = new DieRoll(i.Extra[0]);
            Damage = damageRoll.Roll();
        }
    }
}
