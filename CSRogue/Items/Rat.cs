using CSRogue.Item_Handling;

namespace CSRogue.Items
{

	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Rat. </summary>
	///
	/// <remarks>	Darrellp, 9/16/2011. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	[Item(ItemType.Rat)]
	class Rat : Creature
	{
		#region Constructor
		public Rat() : base(ItemType.Rat) { } 
		#endregion

		#region Produce a random rat
		internal override Item RandomItem()
		{
			return new Rat();
		} 
		#endregion
	}
}
