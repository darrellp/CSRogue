using CSRogue.Item_Handling;

namespace CSRogue.Items
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Player object. </summary>
	///
	/// <remarks>	Darrellp, 9/16/2011. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	[Item(ItemType.Player)]
	public class Player : Creature
	{
		#region Constructor
		public Player() : base(ItemType.Player) { }
		#endregion

		#region Produce a random player
		internal override Item RandomItem()
		{
			return new Player();
		} 
		#endregion
	}
}
