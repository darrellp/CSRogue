using CSRogue.Map_Generation;
using SadConsole;

namespace RogueSC.Map_Objects
{
	class SCMapLocationData : MapLocationData
	{
	    private bool _hasGroundCover = false;
	    private bool _isDoorOpen;
		internal CellAppearance Appearance { get; set; }

	    internal bool HasGroundCover
	    {
	        get { return _hasGroundCover; }
	        set
	        {
	            _hasGroundCover = value;
	            if (value)
	            {
	                TerrainState |= TerrainState.BlocksView;
	            }
	            else
	            {
	                TerrainState &= ~TerrainState.BlocksView;
	            }
	        }
	    }
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
			    Appearance = _isDoorOpen ? ScRender.OpenDoorAppearance : ScRender.ClosedDoorAppearance;
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
