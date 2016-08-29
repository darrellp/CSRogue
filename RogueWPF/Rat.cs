using System;
using CSRogue.Item_Handling;
using CSRogue.Map_Generation;

namespace RogueWPF
{
    //  //type		HP		Lvl	Rarity	Color	Speed	AC
    //  Rat			2d6		0	2		Red		.		3
    public class Rat : CSRogue.Items.Creature
    {
        internal static Guid RatId = new Guid("13F003BB-B723-4358-A04D-C93F20F4DF6D");

        public Rat(Level level) : base(ItemType.Nothing, RatId, level) {}

        public override Item RandomItem()
        {
            throw new NotImplementedException();
        }
    }
}