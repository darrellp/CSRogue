using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CSRogue.Utilities;

namespace CSRogue.Item_Handling
{
	class ReadCreatureData
	{
		private static readonly char[] Tabs = new[] { '\t' };

		internal static Dictionary<ItemType, CreatureInfo> GetData()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
		    // ReSharper disable once AssignNullToNotNullAttribute
			StreamReader dataReader = new StreamReader(assembly.GetManifestResourceStream("CSRogue.Data_Files.CreatureData.txt"));
			if (dataReader == null)
			{
				throw new RogueException("Couldn't find CreatureData.txt in resources");
			}
			Dictionary<ItemType, CreatureInfo> infoList = new Dictionary<ItemType, CreatureInfo>();

			// While there are lines to read
			while (!dataReader.EndOfStream)
			{
				ProcessLine(infoList, dataReader.ReadLine());
			}
			return infoList;
		}

		private static void ProcessLine(IDictionary<ItemType, CreatureInfo> infoList, string readLine)
		{
			if (readLine.StartsWith("//"))
			{
				return;
			}
			CreatureInfo info = new CreatureInfo();
			List<string> values = readLine.Split(Tabs).Where(s => s != string.Empty).ToList();
			ItemType itemType = (ItemType) Enum.Parse(typeof (ItemType), values[0]);
			infoList[itemType] = info;

			for (int iField = 1; iField < values.Count; iField++)
			{
				ProcessField(info, iField, values[iField]);
			}
		}

		private static readonly List<Action<string, CreatureInfo>> DispatchTable = new List<Action<string, CreatureInfo>>
		    {
				(s, i) => { },
				(s, i) => i.HitPoints = new DieRoll(s),
				(s, i) => i.Level = Int32.Parse(s),
				(s, i) => i.Rarity = Int32.Parse(s),
				(s, i) => i.Color = (RogueColor)Enum.Parse(typeof(RogueColor), s),
				(s, i) => i.Speed = Int32.Parse(s),
				(s, i) => i.ArmorClass = Int32.Parse(s)
		    };

		private static readonly List<Action<string, CreatureInfo>> DefaultDispatchTable = new List<Action<string, CreatureInfo>>
		    {
				(s, i) => {},
				(s, i) => {},
				(s, i) => {},
				(s, i) => {},
				(s, i) => {},
				(s, i) => {},
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
