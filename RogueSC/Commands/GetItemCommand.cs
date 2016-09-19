using System;
using CSRogue.Interfaces;
using CSRogue.Item_Handling;
using Game = CSRogue.GameControl.Game;

namespace RogueSC.Commands
{
    class GetItemCommand : IRogueCommand
    {
        /// <summary> Event queue for all listeners interested in new level events. </summary>
        internal static event EventHandler<GetItemEventArgs> GetItemEvent;
        internal void InvokeGetItemEvent(Object sender, GetItemEventArgs e)
        {
            GetItemEvent?.Invoke(sender, e);
        }

        public void Execute(Game game)
        {
            SCMap scMap = (SCMap)game.Map;
            var playerLocation = scMap.Player.Location;
            if (scMap[playerLocation].Items.Count > 0)
            {
                // TODO: bring up a nice dialog in case there are several items to see which to pick up.
                // Right now we just pick up the first item.
                scMap.Hero.Inventory.Add((Item)scMap[playerLocation].Items[0]);
                scMap[playerLocation].Items.RemoveAt(0);
                //RenderToCell(scMap.GetAppearance(playerLocation), this[playerLocation.Column, playerLocation.Row], true);
            }
            InvokeGetItemEvent(game, new GetItemEventArgs(scMap.Hero.Inventory[0]));
        }
    }
}
