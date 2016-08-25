using CSRogue.Items;

namespace CSRogue.GameControl.Commands
{
	public class MovementCommand : CommandBase
	{
		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets a value indicating whether we should run. </summary>
		///
		/// <value>	true if we should run, false if not. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public bool Run { get; private set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>	Gets or sets the creature to be moved. </summary>
		///
		/// <value>	The creature. </value>
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public Creature Creature { get; private set; }

		////////////////////////////////////////////////////////////////////////////////////////////////////
	    ///  <summary>	Constructor. </summary>
	    /// 
	    ///  <remarks>	Darrellp, 10/6/2011. </remarks>
	    /// 
	    ///  <param name="commandType">	Type of movement. </param>
	    ///  <param name="shouldRun">	true if we should run. </param>
	    /// <param name="creature">     Creature for this command </param>
	    ////////////////////////////////////////////////////////////////////////////////////////////////////
	    public MovementCommand(CommandType commandType, bool shouldRun = false, Creature creature = null) : base (commandType)
		{
			Run = shouldRun;
			Creature = creature;
		}
	}
}
