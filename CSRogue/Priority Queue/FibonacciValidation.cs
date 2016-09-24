using System.Collections.Generic;
using System.Linq;

namespace Priority_Queue
{
    // ReSharper disable once InconsistentNaming
	public static class FibonacciValidation<BaseType>
	{
		// ReSharper disable once StaticFieldInGenericType
		internal static int ElementCount;

		// Providing this as a place to set a breakpoint
		internal static bool False()
		{
			return false;
		}

		internal static bool IsParentValid(FibonacciWrapper<BaseType> parent)
		{
			if (parent == null)
			{
				return true;
			}
			ElementCount++;
			if (!IsLinkedListValid(parent.FirstChild))
			{
				return False();
			}

			return FibonacciPriorityQueue<BaseType>.EnumerateLinkedList(parent.FirstChild).All(elm => IsParentValid(elm) && elm.CompareTo(parent) >= 0) || False();
		}

		internal static bool IsLinkedListValid(FibonacciWrapper<BaseType> list)
		{
			if (list == null)
			{
				return true;
			}
			var cur = list;
			var nextSibling = list.RightSibling;

			var vals = new HashSet<FibonacciWrapper<BaseType>>();
			while (true)
			{
				if (vals.Contains(cur))
				{
					if (cur != list)
					{
						return False();
					}
					break;
				}
				vals.Add(cur);

				cur = nextSibling;
				if (nextSibling == null)
				{
					return False();
				}
				nextSibling = nextSibling.RightSibling;
			}
			var count = vals.Count;
			vals.Clear();
			cur = list;
			nextSibling = list.LeftSibling;
			while (true)
			{
				if (vals.Contains(cur))
				{
					if (cur != list)
					{
						return False();
					}
					break;
				}
				vals.Add(cur);

				cur = nextSibling;
				if (nextSibling == null)
				{
					return False();
				}
				nextSibling = nextSibling.LeftSibling;
			}
			return vals.Count == count;
		}
	}
}