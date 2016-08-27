using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Effects;

namespace RogueSC.Map_Objects
{
    public static class MapObjectFactory
    {
        internal static Dictionary<string, CellAppearance> ObjectNameToAppearance = new Dictionary<string, CellAppearance>()
        {
            {"wall", new CellAppearance(Color.White, Color.Gray, 176)},
            {"floor", new CellAppearance(Color.DarkGray, Color.Transparent, 46)}
        };

        internal static MapObject CreateObject(string type)
        {
            return new MapObject(ObjectNameToAppearance[type]);
        }
    }

    internal class MapObject
    {
        public CellAppearance Appearance { get; set; }
        public ICellEffect EffectSeen { get; set; }
        public ICellEffect EffectHidden { get; set; }

        public MapObject(CellAppearance appearance)
        {
            Appearance = appearance;

            EffectSeen = new Recolor()
            {
                Foreground = Color.LightGray * 0.3f,
                Background = Color.LightGray * 0.3f,
                DoForeground = true,
                DoBackground = true,
                CloneOnApply = false
            };

            EffectHidden = new Recolor()
            {
                Foreground = Color.Black,
                Background = Color.Black,
                DoForeground = true,
                DoBackground = true,
                CloneOnApply = false
            };
        }

        public MapObject(Color foreground, Color background, int character) : this(new CellAppearance(foreground, background, character)) {}

        public virtual void RenderToCell(Cell sadConsoleCell, bool isFov, bool isExplored)
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
            else if (isExplored)
            {
                sadConsoleCell.Effect = EffectSeen;
                sadConsoleCell.Effect.Apply(sadConsoleCell);
            }
            else
            {
                sadConsoleCell.Effect = EffectHidden;
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

            sadConsoleCell.Effect = EffectSeen;
            sadConsoleCell.Effect.Apply(sadConsoleCell);
        }
    }
}


