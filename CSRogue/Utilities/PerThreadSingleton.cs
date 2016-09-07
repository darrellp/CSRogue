using System.Collections.Generic;
using System.Threading;

namespace CSRogue.Utilities
{
	class PerThreadSingleton<T> where T : class, new()
	{
		private static readonly Dictionary<int, T> PerThreadMapping = new Dictionary<int, T>();
		public static T Instance()
		{
			// Retrieve our thread ID
			var tid = Thread.CurrentThread.ManagedThreadId;

			// Does our library contain this ID?
			if (!PerThreadMapping.ContainsKey(tid))
			{
				// Create the singleton
				var singleton = new T();

				lock (PerThreadMapping)
				{
					// Install it in the dictionary
					PerThreadMapping[tid] = singleton;
				}
			}

			// Retrieve the object from the dictionary
			return PerThreadMapping[tid];
		}
	}
}
