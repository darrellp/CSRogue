namespace CSRogue.Interfaces
{
	public interface ICreature : IItem
	{
		bool IsPlayer { get; }
		bool IsAlive { get; set; }
		void InvokeAi();
	}
}
