using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CSRogue.Item_Handling
{
	public class ItemFactory : IItemFactory
	{
		public Dictionary<Guid, ItemInfo> InfoFromId { get; }
		public ItemFactory(TextReader reader)
		{
			InfoFromId = (new ReadItemData()).GetData(reader, Assembly.GetCallingAssembly());
		}
	}
}
