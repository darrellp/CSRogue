using CSRogue.Utilities;

namespace CSRogue.Item_Handling
{
	public class CreatureInfo
	{
		public DieRoll HitPoints { get; set; }
		public int Level { get; set; }
		public int Rarity { get; set; }
		public RogueColor Color { get; set; }
		public int Speed { get; set; }
		public int ArmorClass { get; set; }

		internal CreatureInfo(
			string hitPoints = "1d1",
			int level = 0,
			int rarity = 1,
			RogueColor color = RogueColor.White,
			int speed = 100,
			int armorClass = 0)
		{
			Level = level;
			Rarity = rarity;
			Color = color;
			Speed = speed;
			ArmorClass = armorClass;
			HitPoints = new DieRoll(hitPoints);
		}
	}
}
