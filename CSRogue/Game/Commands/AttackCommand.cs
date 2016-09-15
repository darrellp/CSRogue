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
			// Determine damage
			var damage = CalculateDamage(_attacker, _victim);

			// Hit the victim for that damage
			HitCreatureFor(_victim, damage);
			var expired = _victim.HitPoints == 0;

			// Did the victim die?
			if (expired)
			{
				// Kill the unfortunate victim
				// TODO: check to see if the victim is the player in which case, game over!
				game.CurrentLevel.KillCreature(_victim);
			}

			// Invoke an attack event for this attack
			var attackArgs = new AttackEventArgs(_attacker, _victim, damage, expired, _victim.Location);
			game.InvokeEvent(EventType.Attack, game, attackArgs);
		}

		public virtual void HitCreatureFor(ICreature victim, int damage)
		{
			victim.HitPoints -= damage;
		}

		public virtual int CalculateDamage(ICreature attacker, ICreature victim)
		{
			return victim.IsPlayer ? 0 : victim.HitPoints;
		}
	}
}
