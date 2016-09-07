using CSRogue.Item_Handling;
using CSRogue.Map_Generation;

namespace CSRogue.Items
{
	public interface ICreature : IItem
	{
		bool IsPlayer { get; }
		int HitPoints { get; set; }
		void InvokeAi();
	}
}
