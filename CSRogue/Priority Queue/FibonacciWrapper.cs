using System;

namespace Priority_Queue
{
    // ReSharper disable once InconsistentNaming
    public class FibonacciWrapper<BaseType>
    {
		public BaseType Value { get; set; }
		public FibonacciWrapper<BaseType> FirstChild { get; set; }
		public FibonacciWrapper<BaseType> Parent { get; set; }
		public FibonacciWrapper<BaseType> LeftSibling { get; set; }
		public FibonacciWrapper<BaseType> RightSibling { get; set; }
		public int Degree { get; set; }
		public bool Marked { get; set; }
		private readonly Func<BaseType, BaseType, int> _compare;
		internal bool InfinitelyNegative { get; set; }

		public FibonacciWrapper(BaseType value, Func<BaseType, BaseType, int> compare = null)
		{
			Value = value;
			Degree = 0;
			FirstChild = null;
			Marked = false;
			Parent = null;
			LeftSibling = RightSibling = this;
			InfinitelyNegative = false;
			_compare = compare;
		}

        public static implicit operator FibonacciWrapper<BaseType>(BaseType value)
        {
            return new FibonacciWrapper<BaseType>(value);
        }

        public static implicit operator BaseType(FibonacciWrapper<BaseType> value)
        {
            return value.Value;
        }

        public int CompareTo(object obj)
		{
			var otherWrapper = obj as FibonacciWrapper<BaseType>;

            if (otherWrapper == null)
            {
                throw new ArgumentException("Comparing FibonacciWrapper with incompatible type");
            }

			// Infinitely negative values are always smaller than other values
			if (InfinitelyNegative)
			{
				return -1;
			}
			if (otherWrapper.InfinitelyNegative)
			{
				return 1;
			}
			if (_compare != null)
			{
				return _compare(Value, otherWrapper.Value);
			}

			var cmpThis = Value as IComparable;
			var cmpOther = otherWrapper.Value as IComparable;
			if (cmpThis == null || cmpOther == null)
			{
				throw new InvalidOperationException("No comparison function and Attrs are not IComparable");
			}
			return cmpThis.CompareTo(cmpOther);
		}

		public override string ToString()
		{
			return "[" + Value.ToString() + "]";
		}

	    public int Index { get; set; }
    }
}
