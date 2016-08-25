using System.Collections.Generic;
using System.Linq;
using CSRogue.Item_Handling;

namespace CSRogue.Map_Generation
{
	public enum LitState
	{
		InView,
		Remembered,
		FogOfWar
	}

	public class MapLocationData
	{
		public List<Item> Items { get; private set; }
		public TerrainType Terrain { get; set; }
		public LitState LitState { get; internal set; }
		public GenericRoom Room { get; internal set; }

		internal MapLocationData()
		{
			Items = new List<Item>();
			Terrain = TerrainType.OffMap;
			LitState = LitState.FogOfWar;
		}

		internal MapLocationData(TerrainType terrain, List<Item> items = null)
			: this()
		{
			if (items != null)
			{
				Items = items;
			}
			Terrain = terrain;
		}

		internal void AddItem(Item item)
		{
			Items.Add(item);
		}

		internal void RemoveItem(Item item)
		{
			Items.Remove(item);
		}

		internal Item FindItemType(ItemType itemType)
		{
			return Items.Where(item => item.ItemType == itemType).First();
		}
	}
}
