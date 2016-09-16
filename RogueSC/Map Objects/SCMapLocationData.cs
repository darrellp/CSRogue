using CSRogue.Map_Generation;
using SadConsole;

namespace RogueSC.Map_Objects
{
	class SCMapLocationData : MapLocationData
	{
	    private bool _isDoorOpen;
		internal CellAppearance Appearance { get; set; }

	    internal bool HasGroundCover { get; set; } = false;
		internal void ToggleDoor()
		{
			IsDoorOpen = !IsDoorOpen;
		}

		internal bool IsDoorOpen
		{
			get
			{
				return _isDoorOpen;
			}
			set
			{
				_isDoorOpen = value;
			    CheckVisibility();
			    CheckWalkability();
                Appearance = ScRender.ObjectNameToAppearance[_isDoorOpen ? "openDoor" : "closedDoor"];
			}
		}

	    private void CheckWalkability()
	    {
	        if (Terrain == TerrainType.Door && _isDoorOpen)
	        {
                TerrainState |= TerrainState.Walkable;
            }
	        else
	        {
                TerrainState &= ~TerrainState.Walkable;
            }
        }

	    private void CheckVisibility()
	    {
            if (Terrain == TerrainType.Door && _isDoorOpen)
            {
                TerrainState &= ~TerrainState.BlocksView;
            }
            else
            {
                TerrainState |= TerrainState.BlocksView;
            }
        }
    }
}
