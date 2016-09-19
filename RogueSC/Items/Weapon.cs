using CSRogue.Item_Handling;
using CSRogue.Map_Generation;
using CSRogue.Utilities;

namespace RogueSC.Items
{
    internal class Weapon : Item
    {
        protected static DieRoll DamageRoll;

        internal int Damage { get; }

        // ReSharper disable once UnusedParameter.Local
        public Weapon(Level l, ItemInfo i)
        {
            if (DamageRoll == null)
            {
                DamageRoll = new DieRoll(i.Extra[0]);
            }
            Damage = DamageRoll.Roll();
        }
    }
}
