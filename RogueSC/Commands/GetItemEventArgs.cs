using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSRogue.Interfaces;
using CSRogue.Item_Handling;
using CSRogue.Map_Generation;

namespace RogueSC.Commands
{
    internal class GetItemEventArgs : System.EventArgs
    {
        internal GetItemEventArgs(Item itemPickedUp)
        {
            ItemPickedUp = itemPickedUp;
        }

        internal Item ItemPickedUp { get; private set; }
    }
}
