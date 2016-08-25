using System;
using CSRogue.Map_Generation;

namespace CSRogue.GameControl.Commands
{
	public class NewLevelCommand : CommandBase
	{
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets the requested level. </summary>
		///
		/// <value>	level being requested. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public int Level { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public int FOVRows { get; set; }
		public Func<MapCoordinates, MapCoordinates, bool> Filter { get; set; }
		public Excavator Excavator { get; set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Constructor. </summary>
		///
		/// <remarks>	Darrellp, 10/8/2011. </remarks>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public NewLevelCommand() : base(CommandType.NewLevel)
		{
		}
	}
}
