using System;

namespace Priority_Queue
{
    // ReSharper disable once InconsistentNaming
    public class BinaryQueueDithDeletionsTyped<BaseType> : BinaryQueueWithDeletions<BinaryWrapper<BaseType>> where BaseType : IComparable
    {
        #region Public overrides

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>	Removes and returns the largest object. </summary>
        ///
        /// <remarks>	Darrellp, 2/19/2011. </remarks>
        ///
        /// <returns>	The previous largest object. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public new BaseType Pop(out bool fNoMin)
        {
            var valRet = base.Pop(out fNoMin);
            return fNoMin ? default(BaseType) : valRet.Value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Pops smallest element from the stack.
        /// </summary>
        /// <remarks>	Darrellp - 6/4/14	</remarks>
        /// <returns>Smallest element</returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public new BaseType Pop()
        {
            bool fNoMin;
            return Pop(out fNoMin);
        }

        public new BaseType Peek(out bool fNoMin)
        {
            var valRet = base.Peek(out fNoMin);
            return fNoMin ? default(BaseType) : valRet.Value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Peeks at the smallest element without removing it.
        /// </summary>
        /// <remarks>	Darrellp - 6/4/14	</remarks>
        /// <returns>Smallest element or default(BaseType) if no smallest element.</returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public new BaseType Peek()
        {
            bool fNoMin;
            return Peek(out fNoMin);
        }

        public object Add(BaseType val)
        {
            var wrapper = new BinaryWrapper<BaseType>(val);
            base.Add(wrapper);
            return wrapper;
        }
        #endregion

    }
}
