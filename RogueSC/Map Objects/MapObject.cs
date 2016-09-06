using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Effects;

namespace RogueSC.Map_Objects
{
    internal class MapObject
    {
        public CellAppearance Appearance { get; set; }
        public ICellEffect EffectHidden { get; set; }
        public ICellEffect EffectSeen { get; set; }

        static readonly ICellEffect EffectSeenDefault = new Recolor()
        {
            Foreground = Color.LightGray * 0.6f,
            Background = Color.LightGray * 0.2f,
            DoForeground = true,
            DoBackground = true,
            CloneOnApply = false
        };

        static readonly ICellEffect EffectHiddenDefault = new Recolor()
        {
            Foreground = Color.Black,
            Background = Color.Black,
            DoForeground = true,
            DoBackground = true,
            CloneOnApply = false
            };

        public MapObject(CellAppearance appearance)
        {
            Appearance = appearance;
            EffectHidden = EffectHiddenDefault;
            EffectSeen = EffectSeenDefault;
        }

        public MapObject(Color foreground, Color background, int character) : this(new CellAppearance(foreground, background, character)) {}

        public virtual void RenderToCell(Cell sadConsoleCell, bool isFov)
        {
            Appearance.CopyAppearanceTo(sadConsoleCell);

            // Clear out the old effect if there was one
            if (sadConsoleCell.Effect != null)
            {
                sadConsoleCell.Effect.Clear(sadConsoleCell);
                sadConsoleCell.Effect = null;
            }

            if (isFov)
            {
                // Do nothing if it's in view, it's a normal colored square
                // You could do something later like check how far the cell is from the player and tint it
            }
            else
            {
                sadConsoleCell.Effect = EffectSeen;
                sadConsoleCell.Effect.Apply(sadConsoleCell);
            }
        }

        public virtual void RemoveCellFromView(Cell sadConsoleCell)
        {
            // Clear out the old effect if there was one
            if (sadConsoleCell.Effect != null)
            {
                sadConsoleCell.Effect.Clear(sadConsoleCell);
                sadConsoleCell.Effect = null;
            }

            sadConsoleCell.Effect = EffectHidden;
            sadConsoleCell.Effect.Apply(sadConsoleCell);
        }
    }
}


