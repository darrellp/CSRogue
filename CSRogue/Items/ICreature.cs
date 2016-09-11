using CSRogue.Item_Handling;

namespace CSRogue.Items
{
	public interface ICreature : IItem
	{
		bool IsPlayer { get; }
		int HitPoints { get; set; }
		void InvokeAi();
	}
}
