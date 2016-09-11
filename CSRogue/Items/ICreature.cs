using CSRogue.Item_Handling;

namespace CSRogue.Items
{
	public interface ICreature : IItem
	{
		bool IsPlayer { get; }
		bool IsAlive { get; set; }
		int HitPoints { get; set; }
		void InvokeAi();
	}
}
