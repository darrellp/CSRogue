using CSRogue.Interfaces;

namespace CSRogue.Items
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Player object. </summary>
	///
	/// <remarks>	This class is mainly to delineate the player.  Anything that derives from Player
    ///             will be recognized as a player since we use "as IPlayer" to recognize them.
    ///             Darrellp, 9/16/2011. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public class Player : Creature, IPlayer
	{
    }
}
