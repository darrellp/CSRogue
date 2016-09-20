using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CSRogue.Interfaces;
using CSRogue.Utilities;

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

				if (line.StartsWith("---"))
				{
					break;
				}
				lastInfo = ProcessLine(mapItemTypeToInfo, line, lastInfo);
			}

			while (true)
			{
				var line = input.ReadLine();
				if (line == null)
				{
					break;
				}

				if (line.StartsWith("---"))
				{
					break;
				}
				ProcessCreatureLine(line, mapItemTypeToInfo);
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
			for (var iField = 0; iField < Math.Min(DispatchTable.Count, values.Count); iField++)
			{
				ProcessField(info, iField, values[iField]);
			}
		    if (values.Count > DispatchTable.Count)
		    {
		        info.Extra = values.Skip(DispatchTable.Count).ToArray();
		    }
			mapItemTypeToInfo[itemId] = info;
			return info;
		}

		private Func<ILevel, ItemInfo, IItem> GetConstructor(Guid id, ItemInfo i, string s)
		{
			var type = _typeAssembly.GetType(s);
			return (level, itemInfo) =>
			{
				var item = (IItem) Activator.CreateInstance(type, level, itemInfo);
				item.ItemTypeId = id;
				return item;
			};
		}

		private void ProcessField(ItemInfo info, int iField, string value)
		{
			if (value == ".")
			{
				DefaultDispatchTable[iField](this, value, info);
				return;
			}
			DispatchTable[iField](this, value, info);
		}

		private static void ProcessCreatureField(CreatureInfo info, int iField, string value)
		{
			if (value == ".")
			{
				DefaultCreatureDispatchTable[iField](value, info);
				return;
			}
			CreatureDispatchTable[iField](value, info);
		}

		private static void ProcessCreatureLine(string readLine, Dictionary<Guid, ItemInfo> itemInfo)
		{

			if (readLine.Trim() == string.Empty || readLine.StartsWith("//"))
			{
				return;
			}
			var info = new CreatureInfo();
			var values = readLine.Split(Tabs).Where(s => s != string.Empty).ToList();
			var id = new Guid(values[1]);

			for (var iField = 1; iField < Math.Min(CreatureDispatchTable.Count, values.Count); iField++)
			{
				ProcessCreatureField(info, iField, values[iField]);
			}
		    if (values.Count > CreatureDispatchTable.Count)
		    {
                info.Extra = values.Skip(CreatureDispatchTable.Count).ToArray();
		    }
			itemInfo[id].CreatureInfo = info;
		}

		private static readonly List<Action<ReadItemData, string, ItemInfo>> DispatchTable
            = new List<Action<ReadItemData, string, ItemInfo>>
		{
			(t, s, i) => i.ItemId = new Guid(s),
			(t, s, i) => i.Character = s[0],
			(t, s, i) => i.Name = s,
			(t, s, i) => i.Description = s,
			(t, s, i) => i.CreateItem = t.GetConstructor(i.ItemId, i, s),
            (t, s, i) => i.IsPlayer = true,
		};

		private static readonly List<Action<ReadItemData, string, ItemInfo>> DefaultDispatchTable = new List<Action<ReadItemData, string, ItemInfo>>
		{
			(t, s, i) => { },
			(t, s, i) => { },
			(t, s, i) => { },
			(t, s, i) => { },
			(t, s, i) => { },
            (t, s, i) => { },
        };

		private static readonly List<Action<string, CreatureInfo>> CreatureDispatchTable = new List<Action<string, CreatureInfo>>
		{
			(s, i) => { },
			(s, i) => { },
			(s, i) => i.HitPoints = new DieRoll(s),
			(s, i) => i.Level = int.Parse(s),
			(s, i) => i.Rarity = int.Parse(s),
			(s, i) => i.Color = (RogueColor)Enum.Parse(typeof(RogueColor), s),
			(s, i) => i.Speed = int.Parse(s),
			(s, i) => i.ArmorClass = int.Parse(s),
        };

		private static readonly List<Action<string, CreatureInfo>> DefaultCreatureDispatchTable = new List<Action<string, CreatureInfo>>
		{
			(s, i) => { },
			(s, i) => { },
			(s, i) => { },
			(s, i) => { },
			(s, i) => { },
			(s, i) => { },
			(s, i) => { },
            (s, i) => { },
        };
	}
}
