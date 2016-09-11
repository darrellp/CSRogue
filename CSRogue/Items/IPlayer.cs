namespace CSRogue.Items
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Interface for player. </summary>
	///
	/// <remarks>	This interface is solely to identify the player.  Any object which inherits from Creature
	/// 			and implements this interface will be identified as the player.
	/// 			Darrell, 9/7/2016. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////

	public interface IPlayer : ICreature
	{
	}
}