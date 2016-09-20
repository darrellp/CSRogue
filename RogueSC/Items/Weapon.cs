using CSRogue.Item_Handling;
using CSRogue.Map_Generation;
using CSRogue.Utilities;

namespace RogueSC.Items
{
    internal class Weapon : Item
    {
        protected DieRoll DamageRoll;
        internal int Damage
        {
            get {  return DamageRoll.Roll(); }
        }

        // ReSharper disable once UnusedParameter.Local
        public Weapon(Level l, ItemInfo i)
        {
            DamageRoll = new DieRoll(i.Extra[0]);
        }
    }
}
