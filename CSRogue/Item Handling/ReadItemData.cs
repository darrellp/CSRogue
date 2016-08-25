using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CSRogue.Utilities;

namespace CSRogue.Item_Handling
{
	class ReadItemData
	{
		private static readonly char[] Tabs = new[] {'\t'};

		internal static Dictionary<ItemType, ItemInfo> GetData()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
		    // ReSharper disable once AssignNullToNotNullAttribute
			StreamReader dataReader = new StreamReader(assembly.GetManifestResourceStream("CSRogue.Data_Files.ItemData.txt"));
			if (dataReader == null)
			{
				throw new RogueException("Can't find ItemData.txt in resources.");
			}
			Dictionary<ItemType, ItemInfo> mapItemTypeToInfo = new Dictionary<ItemType, ItemInfo>();
			ItemInfo lastInfo = null;

			// While there are lines to read
			while (!dataReader.EndOfStream)
			{
				lastInfo = ProcessLine(mapItemTypeToInfo, dataReader.ReadLine(), lastInfo);
			}
			return mapItemTypeToInfo;
		}

		private static ItemInfo ProcessLine(IDictionary<ItemType, ItemInfo> mapItemTypeToInfo, string readLine, ItemInfo lastInfo)
		{
			if (readLine.StartsWith("//"))
			{
				return null;
			}
			if (readLine[0] == '\t')
			{
				string strContinue = readLine.Trim(Tabs);
				string strOldDescription = lastInfo.Description;
				lastInfo.Description = strOldDescription + " " + strContinue;
				return lastInfo;
			}
			ItemInfo info = new ItemInfo();
			List<string> values = readLine.Split(Tabs).Where(s => s != string.Empty).ToList();
			ItemType itemType = (ItemType) Enum.Parse(typeof (ItemType), values[0]);
			for (int iField = 0; iField < values.Count; iField++)
			{
				ProcessField(info, iField, values[iField]);
			}
			mapItemTypeToInfo[itemType] = info;
			return info;
		}

		private static readonly List<Action<string, ItemInfo>> DispatchTable = new List<Action<string, ItemInfo>>
		    {
				(s, i) => i.ItemType = (ItemType)Enum.Parse(typeof(ItemType), s),
				(s, i) => i.Character = s[0],
				(s, i) => i.Name = s,
				(s, i) => i.Weight = Double.Parse(s),
				(s, i) => i.Value = Int32.Parse(s),
				(s, i) => i.Description = s
		    };

		private static readonly List<Action<string, ItemInfo>> DefaultDispatchTable = new List<Action<string, ItemInfo>>
		    {
				(s, i) => {},
				(s, i) => {},
				(s, i) => i.Name = i.ItemType.ToString(),
				(s, i) => {},
				(s, i) => {},
				(s, i) => {},
		    };

		private static void ProcessField(ItemInfo info, int iField, string value)
		{
			if (value == ".")
			{
				DefaultDispatchTable[iField](value, info);
				return;
			}
			DispatchTable[iField](value, info);
		}
	}
}
