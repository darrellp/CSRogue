using System;
using System.Collections.Generic;
using CSRogue.Item_Handling;
using CSRogue.Map_Generation;
using CSRogue.Utilities;

namespace RogueWPF.Mapping
{
    public class ItemFactory : IItemFactory
    {
        public Dictionary<Guid, ItemInfo> InfoFromId { get; } =
            new Dictionary<Guid, ItemInfo>()
        {
            {
                Rat.RatId,
                new ItemInfo
                {
                    ItemId = Rat.RatId,
                    Character = 'r',
                    Description = "A smelly old rat",
                    IsPlayer = false,
                    Name = "Rat",
                    CreatureInfo = new CreatureInfo()
                    {
                        Level = 1,
                        Rarity = 1,
                        HitPoints = new DieRoll("1d6")
                    }
                }
            }
        };

        public IItem Create(Guid id, Level level)
        {
            return new Rat(level);
        }
    }
}