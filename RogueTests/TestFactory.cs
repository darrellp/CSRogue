using System;
using System.Collections.Generic;
using System.IO;
using CSRogue.Item_Handling;
using CSRogue.Map_Generation;

namespace RogueTests
{
    public class Person : IItem
    {
        public Guid ItemTypeId { get; set; }
        public MapCoordinates Location { get; set; }
        public char Ch { get;} = '@';
		public Person(Level l) { }
    }

	public class Rat : IItem
    {
		public Guid ItemTypeId { get; set; }
		public MapCoordinates Location { get; set; }
        public char Ch { get;} = 'r';
		public Rat(Level l) { }
    }

    public class TestFactory : IItemFactory
    {
        public static Guid PersonId = new Guid("0D583F58-FA20-4292-A272-37919917644A");
        public static Guid RatId = new Guid("1BA9B9C4-6133-4CD3-92A6-233F0F26CBC0");

        public Dictionary<Guid, ItemInfo> InfoFromId { get; }
        public TestFactory()
        {
            const string input = @"
//type										ch	name		weight	value	description								class
0D583F58-FA20-4292-A272-37919917644A		@	Player		.		.		The Player								RogueTests.Person
1BA9B9C4-6133-4CD3-92A6-233F0F26CBC0		r	Rat			.		.		A vile little sewer rat.				RogueTests.Rat
																			These rodents seem to be everywhere!
";
            TextReader reader = new StringReader(input);
            InfoFromId = (new ReadItemData()).GetData(reader);
        }
    }
}