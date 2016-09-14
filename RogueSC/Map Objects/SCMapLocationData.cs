using CSRogue.Map_Generation;
using SadConsole;

namespace RogueSC.Map_Objects
{
	class SCMapLocationData : MapLocationData
	{
		private bool _isDoorOpen;
		internal CellAppearance Appearance { get; set; }

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
				if (_isDoorOpen)
				{
					TerrainState &= ~TerrainState.BlocksView;
					TerrainState |= TerrainState.Walkable;
					Appearance = ScRender.ObjectNameToAppearance["openDoor"];
				}
				else
				{
					TerrainState &= ~TerrainState.Walkable;
					TerrainState |= TerrainState.BlocksView;
					Appearance = ScRender.ObjectNameToAppearance["closedDoor"];
				}
			}
		}
	}
}
