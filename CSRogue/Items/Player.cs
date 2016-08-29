using CSRogue.Item_Handling;
using CSRogue.Map_Generation;

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
		public Player() : base(ItemType.Player, Item.HeroId) { }
        #endregion

        #region Produce a random player
        public override Item RandomItem()
        {
            return new Player();
        }
        #endregion
    }
}
