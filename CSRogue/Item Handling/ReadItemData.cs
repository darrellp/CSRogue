using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CSRogue.Map_Generation;

namespace CSRogue.Item_Handling
{
	class ReadItemData
	{
		private static readonly char[] Tabs = new[] {'\t'};

		private Assembly _typeAssembly;

		public Dictionary<Guid, ItemInfo> GetData(TextReader input, Assembly typeAssembly = null)
		{
			_typeAssembly = typeAssembly ?? Assembly.GetCallingAssembly();


			var mapItemTypeToInfo = new Dictionary<Guid, ItemInfo>();
			ItemInfo lastInfo = null;

			// While there are lines to read
			while (true)
			{
			    var line = input.ReadLine();
			    if (line == null)
			    {
			        break;
			    }
				lastInfo = ProcessLine(mapItemTypeToInfo, line, lastInfo);
			}
			return mapItemTypeToInfo;
		}

		private ItemInfo ProcessLine(IDictionary<Guid, ItemInfo> mapItemTypeToInfo, string readLine, ItemInfo lastInfo)
		{
			if (readLine.Trim() == string.Empty || readLine.StartsWith("//"))
			{
				return lastInfo;
			}
			if (readLine[0] == '\t')
			{
				var strContinue = readLine.Trim(Tabs);
				var strOldDescription = lastInfo.Description;
				lastInfo.Description = strOldDescription + " " + strContinue;
				return lastInfo;
			}
			var info = new ItemInfo();
			var values = readLine.Split(Tabs).Where(s => s != string.Empty).ToList();
		    var itemId = new Guid(values[0]);
			for (var iField = 0; iField < values.Count; iField++)
			{
				ProcessField(info, iField, values[iField]);
			}
			mapItemTypeToInfo[itemId] = info;
			return info;
		}

		private readonly List<Action<ReadItemData, string, ItemInfo>> _dispatchTable = new List<Action<ReadItemData, string, ItemInfo>>
		    {
				(t, s, i) => i.ItemId = new Guid(s),
				(t, s, i) => i.Character = s[0],
				(t, s, i) => i.Name = s,
				(t, s, i) => i.Weight = Double.Parse(s),
				(t, s, i) => i.Value = Int32.Parse(s),
				(t, s, i) => i.Description = s,
				(t, s, i) => i.CreateItem = t.GetConstructor(i.ItemId, s)
		    };

		private Func<Level, IItem> GetConstructor(Guid id, string s)
		{
			var type = _typeAssembly.GetType(s);
			return l =>
			{
				var item = (IItem) Activator.CreateInstance(type, l);
				item.ItemTypeId = id;
				return item;
			};
		}

		private static readonly List<Action<ReadItemData, string, ItemInfo>> DefaultDispatchTable = new List<Action<ReadItemData, string, ItemInfo>>
		    {
				(t, s, i) => { },
				(t, s, i) => { },
		        (t, s, i) => { },
				(t, s, i) => { },
				(t, s, i) => { },
				(t, s, i) => { },
				(t, s, i) => { },
			};

		private void ProcessField(ItemInfo info, int iField, string value)
		{
			if (value == ".")
			{
				DefaultDispatchTable[iField](this, value, info);
				return;
			}
			_dispatchTable[iField](this, value, info);
		}
	}
}
