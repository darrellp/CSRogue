namespace CSRogue.Item_Handling
{
	#region Item type enumeration
	public enum ItemType
	{
		Nothing,
		Player,
		Rat,
	} 
	#endregion

	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Generic item class. </summary>
	///
	/// <remarks>	Darrellp, 9/16/2011. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public abstract class Item
	{
		#region Properties
		public ItemType ItemType { get; private set; } 
		#endregion

		#region Constructor
		internal Item(ItemType type)
		{
			ItemType = type;
		} 

		internal Item() : this(ItemType.Nothing) { }
		#endregion

		#region Produce random item
		internal abstract Item RandomItem();
		#endregion
	}
}
