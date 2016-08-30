using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSRogue.Item_Handling
{
	class ReadItemData
	{
		private static readonly char[] Tabs = new[] {'\t'};

		public static Dictionary<Guid, ItemInfo> GetData(TextReader input)
		{
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

		private static ItemInfo ProcessLine(IDictionary<Guid, ItemInfo> mapItemTypeToInfo, string readLine, ItemInfo lastInfo)
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

		private static readonly List<Action<string, ItemInfo>> DispatchTable = new List<Action<string, ItemInfo>>
		    {
				(s, i) => i.ItemId = new Guid(s),
				(s, i) => i.Character = s[0],
				(s, i) => i.Name = s,
				(s, i) => i.Weight = Double.Parse(s),
				(s, i) => i.Value = Int32.Parse(s),
				(s, i) => i.Description = s
		    };

		private static readonly List<Action<string, ItemInfo>> DefaultDispatchTable = new List<Action<string, ItemInfo>>
		    {
				(s, i) => { },
				(s, i) => { },
		        (s, i) => { },
				(s, i) => { },
				(s, i) => { },
				(s, i) => { },
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
