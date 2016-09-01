using System;
using System.Collections.Generic;
using System.IO;
using CSRogue.Item_Handling;
using CSRogue.Map_Generation;

namespace RogueTests
{
    public class TestFactory : IItemFactory
    {
        public static Guid PersonId = new Guid("0D583F58-FA20-4292-A272-37919917644A");
        public static Guid RatId = new Guid("1BA9B9C4-6133-4CD3-92A6-233F0F26CBC0");

        class Person : IItem
        {
            public Guid ItemTypeId { get; } = PersonId;
            public MapCoordinates Location { get; set; }
            public char Ch { get; set; } = '@';
        }

        class Rat : IItem
        {
            public Guid ItemTypeId { get; } = RatId;
            public MapCoordinates Location { get; set; }
            public char Ch { get; set; } = 'r';
        }

        static readonly Dictionary<Guid, Func<IItem>> GuidToCreator = new Dictionary<Guid, Func<IItem>>()
        {
            {RatId, () => new Rat()},
            {PersonId, () => new Person() }
        };
        public Dictionary<Guid, ItemInfo> InfoFromId { get; }
        public IItem Create(Guid id, Level level)
        {
            return GuidToCreator[id]();
        }

        public TestFactory()
        {
            const string input = @"
//type										ch	name		weight	value	description
0D583F58-FA20-4292-A272-37919917644A		@	Player		.		.		The Player
1BA9B9C4-6133-4CD3-92A6-233F0F26CBC0		r	Rat			.		.		A vile little sewer rat.
																			These rodents seem to be everywhere!
";

            TextReader reader = new StringReader(input);
            InfoFromId = ReadItemData.GetData(reader);
        }
    }
}