using System;
using System.IO;
using CSRogue.Item_Handling;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RogueTests
{
    [TestClass]
    public class Initialization
    {
        public static readonly Guid PersonId = new Guid("0D583F58-FA20-4292-A272-37919917644A");
        public static readonly Guid RatId = new Guid("1BA9B9C4-6133-4CD3-92A6-233F0F26CBC0");

        private static ItemFactory _itemFactory;
        public static ItemFactory ItemFactory => _itemFactory;

        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            const string input = @"
// This is a comment
//type										ch	name		weight	value	description								class               IsPlayer
0D583F58-FA20-4292-A272-37919917644A		@	Player		.		.		The Player								RogueTests.Person	X
1BA9B9C4-6133-4CD3-92A6-233F0F26CBC0		r	Rat			.		.		A vile little sewer rat.				RogueTests.Rat
																			These rodents seem to be everywhere!
36BC6779-846D-4949-8F30-7C5F97E5E729		!	Sword		10		15		A sharp, pointy, hurty thingy			RogueTests.Sword
-----------------------------------------------------------------------------------------------------------------------------------------
// Creature data is separated by a row which starts with ---
// 
// No need to fill out IsPlayer for any creatures but player characters.  Name field is only for documentation
// It's never actually used.
// 
//name  type										HP		Lvl	Rarity	Color	Speed	AC
player	0D583F58-FA20-4292-A272-37919917644A		2d6		.	.		.		.		8
rat		1BA9B9C4-6133-4CD3-92A6-233F0F26CBC0		2d6		0	2		Red		.		3
";
            TextReader reader = new StringReader(input);
            _itemFactory = new ItemFactory(reader);
        }
    }
}
