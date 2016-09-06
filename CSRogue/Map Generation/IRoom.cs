using System.Collections.Generic;

namespace CSRogue.Map_Generation
{
	public interface IRoom
	{
		char[][] Layout { get; }
		MapCoordinates Location { get; }
		List<GenericRoom> Exits { get; }
	}
}