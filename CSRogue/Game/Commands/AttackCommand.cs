using CSRogue.Interfaces;
using CSRogue.Items;
using CSRogue.RogueEventArgs;
using CSRogue.Utilities;

namespace CSRogue.GameControl.Commands
{
	class AttackCommand : CommandBase
	{
		private readonly ICreature _attacker;
		private readonly ICreature _victim;

		public AttackCommand(ICreature attacker, ICreature victim)
		{
			_attacker = attacker;
			_victim = victim;
		}

		public override void Execute(Game game)
		{
			// Invoke an attack event for this attack
			var attackArgs = new AttackEventArgs(_attacker, _victim, _victim.Location);
			game.InvokeEvent(EventType.Attack, game, attackArgs);
		}
	}
}
