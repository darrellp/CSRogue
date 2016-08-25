using System;

namespace CSRogue.Item_Handling
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ItemAttribute : Attribute
	{
		internal ItemType ItemType { get; private set; }
		internal ItemAttribute(ItemType itemType)
		{
			ItemType = itemType;
		}
	}
}
