using CSRogue.Interfaces;
using CSRogue.Items;
using CSRogue.Map_Generation;

namespace CSRogue.RogueEventArgs
{
	public class AttackEventArgs : System.EventArgs
	{
		public ICreature Attacker { get; private set; }
		public ICreature Victim { get; private set;}
		public int Damage { get; private set;}
		public bool VictimDied { get; private set;  }
		public MapCoordinates VictimLocation { get; private set; }

		public AttackEventArgs(
			ICreature attacker, 
			ICreature victim, 
			int damage, 
			bool victimDied, 
			MapCoordinates victimLocation)
		{
			Attacker = attacker;
			Victim = victim;
			Damage = damage;
			VictimDied = victimDied;
			VictimLocation = victimLocation;
		}
	}
}
