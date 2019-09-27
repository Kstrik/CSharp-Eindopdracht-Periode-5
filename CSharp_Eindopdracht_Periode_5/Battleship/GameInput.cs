using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Battleship
{
    public delegate void KeyDownEventHandler(Key key);
    public delegate void KeyUpEventHandler(Key key);

    public delegate void MouseDownEventHandler(MouseButton button, Point position);
    public delegate void MouseUpEventHandler(MouseButton button, Point position);
    public delegate void MouseMoveEventHandler(Point position);

    public class GameInput
    {
        public static KeyDownEventHandler KeyDown;
        public static KeyUpEventHandler KeyUp;

        public static MouseDownEventHandler MouseDown;
        public static MouseUpEventHandler MouseUp;
        public static MouseMoveEventHandler MouseMove;

        private static List<Key> keysDown = new List<Key>();

        private GameInput()
        {
            
        }

        public static bool IsKeyDown(Key key)
        {
            return keysDown.Contains(key);
        }

        public static void OnKeyDown(Key key)
        {
            if (!keysDown.Contains(key))
                keysDown.Add(key);
            KeyDown?.Invoke(key);
        }

        public static void OnKeyUp(Key key)
        {
            keysDown.Remove(key);
            KeyUp?.Invoke(key);
        }

        public static void OnMouseDown(MouseButton button, Point position)
        {
            MouseDown?.Invoke(button, position);
        }

        public static void OnMouseUp(MouseButton button, Point position)
        {
            MouseUp?.Invoke(button, position);
        }

        public static void OnMouseMove(Point position)
        {
            MouseMove?.Invoke(position);
        }
    }
}
