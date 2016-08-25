using System;

namespace CSRogue.Utilities
{
	class RogueException : Exception
	{
		internal RogueException(string strFormat, params object[] values) : base(String.Format(strFormat, values)) {}
	}
}
