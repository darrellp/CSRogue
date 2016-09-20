using CSRogue.Interfaces;
using CSRogue.Item_Handling;
using CSRogue.Map_Generation;

namespace RogueSC.Creatures
{
    internal class Hero : SCCreature, IPlayer
    {
        public Hero(Level l, ItemInfo i) : base(l, i) { }
    }
}
