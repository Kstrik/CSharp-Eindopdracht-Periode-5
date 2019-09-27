using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Battleship
{
    public delegate void KeyDownEventHandler(Key key);
    public delegate void KeyUpEventHandler(Key key);

    public class GameInput
    {
        public static KeyDownEventHandler KeyDown;
        public static KeyUpEventHandler KeyUp;

        private GameInput()
        {

        }

        public static void OnKeyDown(Key key)
        {
            KeyDown?.Invoke(key);
        }

        public static void OnKeyUp(Key key)
        {
            KeyUp?.Invoke(key);
        }
    }
}
