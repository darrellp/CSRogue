using System.Collections.Generic;
using CSRogue.Interfaces;
using CSRogue.Item_Handling;

namespace CSRogue.Utilities
{
	public static class MapLocationDataExtensions
	{
		public static void AddItem(this IMapLocationData data, IItem item)
		{
			if (data.Items == null)
			{
				data.Items = new List<IItem>();
			}

			data.Items.Add(item);
		}

		public static void RemoveItem(this IMapLocationData data, IItem item)
		{
			data.Items?.Remove(item);
		}
	}
}
