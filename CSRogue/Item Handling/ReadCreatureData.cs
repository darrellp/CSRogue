using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CSRogue.Utilities;

namespace CSRogue.Item_Handling
{
	class ReadCreatureData
	{
		private static readonly char[] Tabs = { '\t' };

		internal static Dictionary<Guid, CreatureInfo> GetData(TextReader input)
		{
			var infoList = new Dictionary<Guid, CreatureInfo>();

			// While there are lines to read
			while (true)
			{
			    var line = input.ReadLine();
			    if (line == null)
			    {
			        break;
			    }
				ProcessLine(infoList, line);
			}
			return infoList;
		}

		private static void ProcessLine(IDictionary<Guid, CreatureInfo> infoList, string readLine)
		{
            if (readLine.Trim() == string.Empty || readLine.StartsWith("//"))
            {
                return;
            }
			var info = new CreatureInfo();
			var values = readLine.Split(Tabs).Where(s => s != string.Empty).ToList();
            var itemId = new Guid(values[1]);
            infoList[itemId] = info;

			for (var iField = 1; iField < Math.Min(values.Count, DispatchTable.Count); iField++)
			{
				ProcessField(info, iField, values[iField]);
			}
		    if (values.Count > DispatchTable.Count)
		    {
		        info.Extra = values.Skip(DispatchTable.Count).ToArray();
		    }
		}

		private static readonly List<Action<string, CreatureInfo>> DispatchTable = new List<Action<string, CreatureInfo>>
		    {
                (s, i) => { },
				(s, i) => { },
				(s, i) => i.HitPoints = new DieRoll(s),
				(s, i) => i.Level = int.Parse(s),
				(s, i) => i.Rarity = int.Parse(s),
				(s, i) => i.Color = (RogueColor)Enum.Parse(typeof(RogueColor), s),
				(s, i) => i.Speed = int.Parse(s),
				(s, i) => i.ArmorClass = int.Parse(s)
		    };

		private static readonly List<Action<string, CreatureInfo>> DefaultDispatchTable = new List<Action<string, CreatureInfo>>
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

		private static void ProcessField(CreatureInfo info, int iField, string value)
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
