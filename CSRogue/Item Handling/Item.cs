using System;
using CSRogue.Map_Generation;

namespace CSRogue.Item_Handling
{
	#region Item type enumeration
    // TODO: How best to extend this to arbitrary types created by a third party?
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
    public abstract class Item : IItem
	{
        public static readonly Guid NothingId = new Guid("999F0242-DDD9-44DC-A179-6B9F2D614F76");
        public static readonly Guid HeroId = new Guid("0D583F58-FA20-4292-A272-37919917644A");

        #region Properties
        public ItemType ItemType { get; private set; }
        public Guid ItemId { get; }
        #endregion

        #region Constructor
        internal Item(ItemType type, Guid id = default(Guid))
		{
            if (id == default(Guid))
            {
                id = NothingId;
            }
            ItemId = id;
			ItemType = type;
		} 

		internal Item() : this(ItemType.Nothing) { }
		#endregion

		#region Produce random item
		public abstract Item RandomItem();
		#endregion
	}
}
