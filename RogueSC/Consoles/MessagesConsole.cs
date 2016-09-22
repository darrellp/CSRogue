using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Consoles;
using Console = SadConsole.Consoles.Console;


namespace RogueSC.Consoles
{
    class MessagesConsole : Console
    {
        public void PrintMessage(string text)
        {
            ShiftUp(1);
            VirtualCursor.Print(text).CarriageReturn();
        }

        public void PrintMessage(ColoredString text)
        {
            ShiftUp(1);
            VirtualCursor.Print(text).CarriageReturn();
        }

        public MessagesConsole(int width, int height) : base(width, height)
        {
            VirtualCursor.Position = new Point(0, height - 1);
        }

        public MessagesConsole(int width, int height, Font font) : base(width, height, font)
        {
        }

        public MessagesConsole(ITextSurfaceRendered textData) : base(textData)
        {
        }
    }
}
