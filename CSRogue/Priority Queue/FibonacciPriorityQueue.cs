using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Priority_Queue
{
    // ReSharper disable once InconsistentNaming
	public class FibonacciPriorityQueue<BaseType> : IEnumerable<BaseType>
	{
		#region Private Variables
		private FibonacciWrapper<BaseType> _min;
		private readonly Func<BaseType, BaseType, int> _compare;
		#endregion

		#region Properties
		///<summary>
		/// Count of items in the priority queue
		///</summary>
		public int Count { get; set; }
		#endregion

		#region Constructor
		public FibonacciPriorityQueue(Func<BaseType, BaseType, int> compare = null)
		{
			Count = 0;
			_compare = compare;
		    if (compare == null && !typeof(IComparable).IsAssignableFrom(typeof(BaseType)))
		    {
		        throw new ArgumentException("Didn't pass in an IComparable and didn't supply a comparer in Fibonacci Priority Queue ctor");
		    }
		}
		#endregion

		#region Utility functions
		static bool IsSingleTon(FibonacciWrapper<BaseType> element)
		{
			return ReferenceEquals(element, element.RightSibling);
		}

		static bool IsSingletonOrUnattached(FibonacciWrapper<BaseType> element)
		{
			return element.RightSibling == null || IsSingleTon(element);
		}

		static FibonacciWrapper<BaseType> CombineLists(FibonacciWrapper<BaseType> list1, FibonacciWrapper<BaseType> list2)
		{
			if (list1 == null)
			{
				return list2;
			}
			if (list2 == null)
			{
				return list1;
			}

			var list1Next = list1.RightSibling;
			list1.RightSibling = list2.RightSibling;
			list2.RightSibling.LeftSibling = list1;
			list1Next.LeftSibling = list2;
			list2.RightSibling = list1Next;
			ThrowBadList(list1);
			return list1;
		}

		internal static IEnumerable<FibonacciWrapper<BaseType>> EnumerateLinkedList(FibonacciWrapper<BaseType> list)
		{
			if (list == null)
			{
				yield break;
			}
			var cur = list;
			var nextSibling = list.RightSibling;

			do
			{
				yield return cur;
				cur = nextSibling;
				if (nextSibling == null)
				{
					yield break;
				}
				nextSibling = nextSibling.RightSibling;
			} while (!ReferenceEquals(cur, list));
		}

		private static FibonacciWrapper<BaseType> RemoveFromList(FibonacciWrapper<BaseType> element)
		{
			if (IsSingletonOrUnattached(element))
			{
				// If we are not on a list or on a singleton list, there's nothing to do;
				return element;
			}
			var oldList = element.LeftSibling;
			element.LeftSibling.RightSibling = element.RightSibling;
			element.RightSibling.LeftSibling = element.LeftSibling;

			// Turn element into a singleton list
			element.LeftSibling = element.RightSibling = element;
			ThrowBadList(oldList);
			return element;
		}

		private void Consolidate()
		{
			var degreeToRoot = new FibonacciWrapper<BaseType>[64];
			var rootList = EnumerateLinkedList(_min).ToList();

			foreach (var element in rootList)
			{
				var smallerRoot = element;
				var curDegree = element.Degree;
				while (degreeToRoot[curDegree] != null)
				{
					var largerRoot = degreeToRoot[curDegree];
					if (smallerRoot.CompareTo(largerRoot) > 0)
					{
						var swapT = smallerRoot;
						smallerRoot = largerRoot;
						largerRoot = swapT;
					}
					HeapLink(largerRoot, smallerRoot);
					degreeToRoot[curDegree] = null;
					curDegree++;
				}
				degreeToRoot[curDegree] = smallerRoot;
			}
			_min = null;
			foreach (var root in degreeToRoot.Where(elm => elm != null).Select(RemoveFromList))
			{
				if (_min == null)
				{
					_min = root;
				}
				else
				{
					CombineLists(_min, root);
					if (root.CompareTo(_min) < 0)
					{
						_min = root;
					}
				}
			}
		}

		private void HeapLink(FibonacciWrapper<BaseType> newChild, FibonacciWrapper<BaseType> newParent)
		{
			RemoveFromList(newChild);
			if (newParent.FirstChild == null)
			{
				newParent.FirstChild = newChild;
			}
			else
			{
				CombineLists(newParent.FirstChild, newChild);
			}
			newParent.Degree++;
			newChild.Marked = false;
			newChild.Parent = newParent;
		}

		private void CascadingCut(FibonacciWrapper<BaseType> element)
		{
			var parent = element.Parent;
			if (parent != null)
			{
				if (!element.Marked)
				{
					element.Marked = true;
				}
				else
				{
					Cut(element, parent);
					CascadingCut(parent);
				}
			}
		}

		private void Cut(FibonacciWrapper<BaseType> element, FibonacciWrapper<BaseType> parent)
		{
			if (ReferenceEquals(parent.FirstChild, element))
			{
				parent.FirstChild = IsSingleTon(element) ? null : element.RightSibling;
			}
			element = RemoveFromList(element);
			parent.Degree--;
			CombineLists(_min, element);
			element.Parent = null;
			element.Marked = false;
		}
		#endregion

		#region Validation

		[Conditional("DEBUG")]
		private static void ThrowBadList(FibonacciWrapper<BaseType> list)
		{
			if (!FibonacciValidation<BaseType>.IsLinkedListValid(list))
			{
				throw new InvalidOperationException("Bad list");
			}
		}

		[Conditional("DEBUG")]
		private static void ThrowBadFpq(FibonacciPriorityQueue<BaseType> fpq)
		{
			if (!fpq.Validate())
			{
				throw new InvalidOperationException("Bad Fibonacci Priority Queue");
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		///  Validates this Fibonacci priority queue.
		/// </summary>
		/// <remarks> There's a "False()" call that is made whenever this fails.  Setting a breakpoint there
		/// will allow you to see where the problem is arising from.  Darrellp - 6/3/14	</remarks>
		/// <returns><c>true</c> if everything is kosher, <c>false</c> otherwise.</returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public bool Validate()
		{
			FibonacciValidation<BaseType>.ElementCount = 0;
			if (!FibonacciValidation<BaseType>.IsLinkedListValid(_min))
			{
				return FibonacciValidation<BaseType>.False();
			}
			if (EnumerateLinkedList(_min).Any(parent => !FibonacciValidation<BaseType>.IsParentValid(parent)))
			{
				return FibonacciValidation<BaseType>.False();
			}
			if (FibonacciValidation<BaseType>.ElementCount != Count)
			{
				return FibonacciValidation<BaseType>.False();
			}
			return true;
		}
		#endregion

		#region Priority Queue Operations
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		///  Insert an FibonacciWrapper value into the priority queue.
		/// </summary>
		/// <remarks>	Darrellp, 2/17/2011.	</remarks>
		/// <param name="val">Value to insert.</param>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		private void AddWrapper(FibonacciWrapper<BaseType> val)
		{
			if (_min == null)
			{
				_min = val;
				_min.LeftSibling = _min.RightSibling = _min;
			}
			else
			{
				CombineLists(_min, val);
				if (val.CompareTo(_min) < 0)
				{
					_min = val;
				}
			}
			Count++;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		///  Insert a value into the priority queue of type BaseType.
		/// </summary>
		/// <remarks>	Darrellp, 2/17/2011.	</remarks>
		/// <param name="value">Value to insert.</param>
		/// <returns>Cookie to use to reference the object later</returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public virtual FibonacciWrapper<BaseType> Add(BaseType value)
		{
			var wrapper = new FibonacciWrapper<BaseType>(value, _compare);
			AddWrapper(wrapper);
			return wrapper;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		///  Unions the specified heap with our heap and returns the result.
		/// </summary>
		/// <remarks>	Both original heaps are destroyed	</remarks>
		/// <param name="heap">The heap to be unioned in.</param>
		/// <returns>Union of the two heaps</returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public FibonacciPriorityQueue<BaseType> Union(FibonacciPriorityQueue<BaseType> heap)
		{
			var ret = new FibonacciPriorityQueue<BaseType>();
			var ourMin = _min;
			var theirMin = heap._min;

			ret._min = CombineLists(_min, heap._min);
			if (ourMin != null && theirMin != null && ourMin.CompareTo(theirMin) > 0)
			{
				ret._min = theirMin;
			}
			ret.Count = Count + heap.Count;
			return ret;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		///  Extracts and deletes the minimum value from the priority queue.
		/// </summary>
		/// <remarks>	Darrellp - 6/1/14	</remarks>
		/// <param name="fNoMin">No current minimum if set to <c>true</c>.</param>
		/// <returns> Minimum value of type BaseType.</returns>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public BaseType Pop(out bool fNoMin)
		{
			var ret = _min;
			if (_min == null)
			{
				fNoMin = true;
				return default(BaseType);
			}

			fNoMin = false;
			foreach (var child in EnumerateLinkedList(_min.FirstChild))
			{
				child.Parent = null;
			}
			CombineLists(_min, _min.FirstChild);
			var newMin = _min.RightSibling;
			var fSingletonMin = IsSingleTon(_min);
			RemoveFromList(_min);
			if (fSingletonMin)
			{
				_min = null;
			}
			else
			{
				_min = newMin;
				Consolidate();
			}
			Count--;
			return ret == null? default(BaseType) : ret.Value;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Extracts and deletes the minimum value from the priority queue.
        /// </summary>
        /// <remarks>	Darrellp - 6/1/14	</remarks>
        /// <returns> Minimum value of type BaseType - BaseType's default if there is no current minimum.</returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public BaseType Pop()
		{
			bool fNoMin;
			var ret = Pop(out fNoMin);
			ThrowBadFpq(this);
			return ret;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Peeks at the min element without deleting it.
        /// </summary>
        /// <remarks>	Darrellp - 6/1/14	</remarks>
        /// <param name="fNoMin">No minimum if set to <c>true</c>.</param>
        /// <returns>Smallest element in queue or default(BaseType) if no smallest element.</returns>
        /// <exception cref="System.IndexOutOfRangeException">Peeking at an empty priority queue</exception>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public FibonacciWrapper<BaseType> PeekWrapper(out bool fNoMin)
		{
			fNoMin = _min == null;
			return fNoMin ? null : _min;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Peeks at the min element without deleting it.
        /// </summary>
        /// <remarks> This is just a convenience routine  - Darrellp - 6/1/14	</remarks>
        /// <returns>Smallest element in queue or default(BaseType) if no smallest element.</returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public FibonacciWrapper<BaseType> PeekWrapper()
		{
			bool fNoMin;
			return PeekWrapper(out fNoMin);
		}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Peeks at the min element without deleting it.
        /// </summary>
        /// <remarks>	Darrellp - 6/1/14	</remarks>
        /// <param name="fNoMin">No minimum if set to <c>true</c>.</param>
        /// <returns>Smallest element in queue or default(BaseType) if no smallest element.</returns>
        /// <exception cref="System.IndexOutOfRangeException">Peeking at an empty priority queue</exception>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public BaseType Peek(out bool fNoMin)
        {
            fNoMin = _min == null;
            return fNoMin ? null : _min;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Peeks at the min element without deleting it.
        /// </summary>
        /// <remarks> This is just a convenience routine  - Darrellp - 6/1/14	</remarks>
        /// <returns>Smallest element in queue or default(BaseType) if no smallest element.</returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public BaseType Peek()
        {
            bool fNoMin;
            return PeekWrapper(out fNoMin);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Places an element whose value has changed.
        /// </summary>
        /// <remarks>
        ///  This is the guts of DecreaseKey but we use it for Delete also.
        ///		Darrellp - 6/3/14
        /// </remarks>
        /// <param name="element">The element.</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void PlaceElement(FibonacciWrapper<BaseType> element)
		{
			var parent = element.Parent;
			if (parent != null && parent.CompareTo(element) > 0)
			{
				Cut(element, parent);
				CascadingCut(parent);
			}
			if (element.CompareTo(_min) < 0)
			{
				_min = element;
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		///  Decreases the key for an element.
		/// </summary>
		/// <remarks>	Darrellp - 6/3/14	</remarks>
		/// <param name="xObj">The cookie representing the element.</param>
		/// <param name="newValue">The new smaller value.</param>
		/// <exception cref="System.ArgumentException">
		/// DecreaseKey recieved invalid cookie
		/// or
		/// Key value passed to DecreaseKey greater than current value
		/// </exception>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public void DecreaseKey(object xObj, BaseType newValue)
		{
			var element = xObj as FibonacciWrapper<BaseType>;
			if (element == null)
			{
				throw new ArgumentException("DecreaseKey recieved invalid cookie");
			}
			var valueThis = element.Value;
			int cmp;

			if (_compare != null)
			{
				cmp = _compare(valueThis, newValue);
			}
			else
			{
				var icmp = element.Value as IComparable;

				if (icmp == null)
				{
					throw new InvalidOperationException("_compare needs to be set of BaseType needs to be of type IComparable");
				}
				cmp = icmp.CompareTo(newValue);
			}
			if (cmp == 0)
			{
				return;
			}
			if (cmp < 0)
			{
				throw new ArgumentException("Key value passed to DecreaseKey greater than current value");
			}
			element.Value = newValue;
			PlaceElement(element);
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		///  Deletes the specified object.
		/// </summary>
		/// <remarks>	Darrellp - 6/4/14	</remarks>
		/// <param name="xObj">The cookie for the object to be deleted.</param>
		/// <exception cref="System.ArgumentException">Delete recieved invalid cookie</exception>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public void Delete(object xObj)
		{
			var element = xObj as FibonacciWrapper<BaseType>;
			if (element == null)
			{
				throw new ArgumentException("Delete recieved invalid cookie");
			}
			element.InfinitelyNegative = true;
			PlaceElement(element);
            ThrowBadFpq(this);
            Pop();
		}
		#endregion

		#region IEnumerable members
		protected IEnumerator<BaseType> GetEnumerator()
		{
			var returns = new Stack<FibonacciWrapper<BaseType>>();
			var cur = _min;

			while (true)
			{
				foreach (var root in EnumerateLinkedList(cur))
				{
					returns.Push(root);
				}
				if (returns.Count == 0)
				{
					break;
				}
				cur = returns.Pop();
				yield return cur.Value;
				cur = cur.FirstChild;
			}
		}

		IEnumerator<BaseType> IEnumerable<BaseType>.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion
	}
}
