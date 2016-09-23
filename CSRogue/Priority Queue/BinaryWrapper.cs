using System;

namespace Priority_Queue
{
    // ReSharper disable once InconsistentNaming
	public class BinaryWrapper<BaseType> : ILocatable
	{
		public BaseType Value { get; set; }
		public int Index { get; set; }
		private readonly Func<BaseType, BaseType, int> _compare;

		public BinaryWrapper(BaseType value, Func<BaseType, BaseType, int> compare = null)
		{
			Value = value;
			Index = -1;
			_compare = compare;
		}

        public static implicit operator BaseType(BinaryWrapper<BaseType> value)
        {
            return value.Value;
        }

        public static implicit operator BinaryWrapper<BaseType>(BaseType value)
        {
            return new BinaryWrapper<BaseType>(value);
        }

        public int CompareTo(object obj)
		{
			var otherWrapper = obj as BinaryWrapper<BaseType>;
			if (otherWrapper == null)
			{
				throw new ArgumentException("Different types in FibonacciWrapper<BaseType>.CompareTo()");
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
	}
}
